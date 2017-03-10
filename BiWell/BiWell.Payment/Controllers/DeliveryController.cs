using BiWell.Payment.ByDesignOrderAPI;
using BiWell.Payment.Helpers;
using BiWell.Payment.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BiWell.Payment.Controllers
{
    public class DeliveryController : Controller
    {
        // GET: Delivery
        public ActionResult Index()
        {
            List<DeliveryParameters> ordersToDelivery = null;

            try
            {
                ordersToDelivery = ReadOrdersToDeliveryEx();
            }
            catch (Exception ex)
            {
                return View("DeliveryError", ex);
            }

            return View(ordersToDelivery);
        }

        public ActionResult ExportExcel()
        {
            ExportExcelAsHtml();

            return RedirectToAction("Index");
        }

        private void ExportExcelAsHtml()
        {
            List<DeliveryParameters> ordersToDelivery = ReadOrdersToDeliveryEx();

            Response.Clear();
            Response.AddHeader("content-disposition", $"attachment;filename=Delivery_{DateTime.Today.ToShortDateString()}.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            Response.ContentEncoding = Encoding.Unicode;
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    var gv = new GridView();
                    gv.DataSource = ordersToDelivery
                        .Where(x => x.IsPosted)
                        .Where(x => !x.IsSelfPickup)
                        .OrderByDescending(x => x.OrderId)
                        .Select(x => new
                        {
                            OrderId = x.OrderId,
                            FIO = x.Recipient.FullName,
                            Phone = x.ContactInfo.FormattedPhone,
                            Phone2 = "",
                            Email = x.Recipient.Email,
                            IssueType = 151,
                            PaymentType = 0,
                            PayCost = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", x.ActualItems.Sum(di => di.PayCost * di.Quantity)),
                            BalanceDue = "0.00",
                            Weight = x.Items.Sum(di => di.Weight * di.Quantity) * 1000,
                            PlacesQty = 1,
                            DeliveryCode = x.DeliveryCode,
                            PostIndex = x.DeliveryAddress.PostIndex,
                            Region = x.DeliveryAddress.Place,
                            Address = x.DeliveryAddress.Street_1,
                            DeliveryTimeFrom = "10:00",
                            DeliveryTimeTo = "18:00",
                            Comment = "",
                            ShippingTotal = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", x.ShippingTotal),
                            Items = x.ActualItemsString
                        });
                    gv.DataBind();
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                }
            }
            Response.Flush();
            Response.End();
        }

        private List<DeliveryParameters> ReadOrdersToDeliveryEx()
        {
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();

            var result = new List<DeliveryParameters>();

            using (var context = new BiWellEntities())
            {
                context.Database.Log = s => Debug.WriteLine(s);

                //var dateFrom = DateTime.Today.AddDays(-Properties.Settings.Default.Freedom_RecentPeriodLength);

                var dbOrders = context.order_table
                    .Where(o => o.status == "Posted")
                    .Where(o => o.shipping_method_id != null)
                    .Where(o => o.shipping_method_id != Properties.Settings.Default.Freedom_SelfPickupShipMethodId)
                    .OrderByDescending(o => o.modified_at)
                    .Take(Properties.Settings.Default.BiWell_OrdersToDeliveryCount);

                foreach (var dbOrder in dbOrders)
                {
                    var orderToDelivery = new DeliveryParameters
                    {
                        OrderId = dbOrder.order_id,
                        CreatedAt = dbOrder.created_at,
                        ModifiedAt = dbOrder.modified_at
                    };

                    orderToDelivery.Status = dbOrder.status;
                    orderToDelivery.ShipMethodId = dbOrder.shipping_method_id.Value;
                    orderToDelivery.ShipMethod = context.shipping_method.FirstOrDefault(m => m.id == dbOrder.shipping_method_id.Value)?.description;

                    FillFromFreedomDetails(orderToDelivery);
                    FillItemWeights(orderToDelivery);

                    result.Add(orderToDelivery);
                }
            }

            //stopWatch.Stop();
            //TimeSpan ts = stopWatch.Elapsed;

            return result;
        }

        private List<DeliveryParameters> ReadOrdersToDelivery()
        {
            var result = new List<DeliveryParameters>();

            var orderApiClient = ByDesignAPIHelper.CreateOrderAPIClient();
            var orderApiCred = orderApiClient.CreateCredentials();

            var requestOrderListRecent = new GetOrderListRecentRequest();
            requestOrderListRecent.Credentials = orderApiCred;
            requestOrderListRecent.PeriodType = Properties.Settings.Default.Freedom_RecentPeriodType;
            requestOrderListRecent.PeriodLength = Properties.Settings.Default.Freedom_RecentPeriodLength;
            requestOrderListRecent.EvalDateLastModified = true;
            var responseOrderList = orderApiClient.GetOrderListRecent(requestOrderListRecent);

            foreach (var orderList in responseOrderList.GetOrderListRecentResult)
            {
                var orderToDelivery = new DeliveryParameters
                {
                    OrderId = orderList.OrderID,
                    CreatedAt = orderList.CreatedDate,
                    ModifiedAt = orderList.LastModifiedDate
                };

                FillFromFreedomHeader(orderToDelivery);

                if (orderToDelivery.IsPosted && !orderToDelivery.IsSelfPickup)
                {
                    FillFromFreedomDetails(orderToDelivery);
                    FillItemWeights(orderToDelivery);

                    result.Add(orderToDelivery);
                }
            }

            return result;
        }

        private void FillFromFreedomHeader(DeliveryParameters deliveryParameters)
        {
            var orderApiClient = ByDesignAPIHelper.CreateOrderAPIClient();

            var responseOrderInfo = orderApiClient.GetOrderInfo_V2(orderApiClient.CreateCredentials(), deliveryParameters.OrderId);
            if (responseOrderInfo.Success == 0)
            {
                throw new InvalidOperationException(responseOrderInfo.Message);
            }

            deliveryParameters.Status = responseOrderInfo.Status;
            deliveryParameters.ShipMethodId = responseOrderInfo.ShipMethodID;
            deliveryParameters.ShipMethod = responseOrderInfo.ShipMethod;
        }

        private void FillFromFreedomDetails(DeliveryParameters deliveryParameters)
        {
            var orderApiClient = ByDesignAPIHelper.CreateOrderAPIClient();

            var orderDetailsResponse = orderApiClient.GetOrderDetailsInfo_V2(orderApiClient.CreateCredentials(), deliveryParameters.OrderId);
            if (orderDetailsResponse.Success == 0)
            {
                throw new InvalidOperationException(orderDetailsResponse.Message);
            }

            List<DeliveryItem> deliveryItems = new List<DeliveryItem>();
            foreach (var responseItem in orderDetailsResponse.OrderDetailsResponse)
            {
                DeliveryItem deliveryItem = new DeliveryItem
                {
                    ItemId = responseItem.ProductID,
                    Name = responseItem.Description,
                    Quantity = responseItem.Quantity,
                    Cost = decimal.Parse(responseItem.TaxableAmount, CultureInfo.InvariantCulture),
                    PayCost = decimal.Parse(responseItem.TaxableAmount, CultureInfo.InvariantCulture)
                };

                FillItemCosts(deliveryItem);

                deliveryItems.Add(deliveryItem);
            }

            var responseOrderInfo = orderApiClient.GetOrderInfo_V2(orderApiClient.CreateCredentials(), deliveryParameters.OrderId);
            if (responseOrderInfo.Success == 0)
            {
                throw new InvalidOperationException(responseOrderInfo.Message);
            }

            OrderClientInfo orderClientInfo = OrderClientInfo.ExtractFromOrder(responseOrderInfo); // extracting order client info
            var onlineApiClient = ByDesignAPIHelper.CreateOnlineAPIClient();
            ClientContactInfo contactInfo = new ClientContactInfo();

            if (orderClientInfo.IsRep)
            {
                var responseRepInfo = onlineApiClient.GetRepInfo_V3(onlineApiClient.CreateCredentials(), orderClientInfo.ClientNumber);
                if (responseRepInfo.Success == 0)
                {
                    throw new InvalidOperationException(responseOrderInfo.Message);
                }

                contactInfo.FirstName = responseRepInfo.Firstname;
                contactInfo.LastName = responseRepInfo.Lastname;
                contactInfo.Email = responseRepInfo.Email;
                contactInfo.Phone = responseRepInfo.Phone1;
            }
            else
            {
                var responseCustInfo = onlineApiClient.GetCustomerInfo_v3(onlineApiClient.CreateCredentials(), orderClientInfo.ClientNumber);
                if (responseCustInfo.Success == 0)
                {
                    throw new InvalidOperationException(responseOrderInfo.Message);
                }

                contactInfo.FirstName = responseCustInfo.FirstName;
                contactInfo.LastName = responseCustInfo.LastName;
                contactInfo.Email = responseCustInfo.Email;
                contactInfo.Phone = responseCustInfo.Phone1;
            }

            Address deliveryAddress = new Address
            {
                PostIndex = responseOrderInfo.ShipPostalCode,
                Country = responseOrderInfo.ShipCountry,
                State = responseOrderInfo.ShipState,
                Place = responseOrderInfo.ShipCity,
                Street_1 = responseOrderInfo.ShipStreet1,
                Street_2 = responseOrderInfo.ShipStreet2
            };

            DeliveryRecipient recipient = new DeliveryRecipient
            {
                FullName = responseOrderInfo.ShipName1,
                Phone = responseOrderInfo.ShipPhone,
                Email = responseOrderInfo.ShipEmail
            };

            deliveryParameters.Items = deliveryItems.ToArray();
            deliveryParameters.DeliveryAddress = deliveryAddress;
            deliveryParameters.Recipient = recipient;
            deliveryParameters.ContactInfo = contactInfo;

            var responseTotals = orderApiClient.GetTotals(orderApiClient.CreateCredentials(), deliveryParameters.OrderId);
            if (responseTotals.Success == 0)
            {
                throw new InvalidOperationException(responseTotals.Message);
            }

            deliveryParameters.ShippingTotal = decimal.Parse(responseTotals.ShippingTotal, CultureInfo.InvariantCulture); 
        }

        private void FillItemWeights(DeliveryParameters deliveryParameters)
        {
            using (var context = new BiWellEntities())
            {
                foreach (var item in deliveryParameters.Items)
                {
                    var itemWeight = context.ItemWeights.Find(item.ItemId);
                    if (itemWeight == null)
                    {
                        throw new InvalidOperationException($"Весовая характеристика не найдена для {item.ItemId}: {item.Name}");
                    }

                    item.Weight = itemWeight.Weight;
                }
            }
        }

        private void FillItemCosts(DeliveryItem deliveryItem)
        {
            var onlineApiClient = ByDesignAPIHelper.CreateOnlineAPIClient();

            var getInventoryReturn = onlineApiClient.GetInventory_SingleItem(
                onlineApiClient.CreateCredentials(), "1", null, deliveryItem.ItemId, "Wholesale", 0).FirstOrDefault();

            if (getInventoryReturn != null)
            {
                deliveryItem.Cost = Convert.ToDecimal(getInventoryReturn.Price);
                deliveryItem.PayCost = deliveryItem.Cost;
            }
        }
    }
}