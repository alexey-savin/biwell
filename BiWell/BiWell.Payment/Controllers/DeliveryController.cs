using BiWell.Payment.ByDesignOrderAPI;
using BiWell.Payment.Helpers;
using BiWell.Payment.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
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
                ordersToDelivery = ReadOrdersToDelivery();
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
            List<DeliveryParameters> ordersToDelivery = ReadOrdersToDelivery();

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
                            Phone = x.ContactInfo.Phone.Substring(x.ContactInfo.Phone.Length - 10),
                            Phone2 = "",
                            Email = x.Recipient.Email,
                            IssueType = 151,
                            PaymentType = 0,
                            PayCost = x.Items.Sum(di => di.PayCost),
                            BalanceDue = 0,
                            Weight = x.Items.Sum(di => di.Weight) * 1000,
                            PlacesQty = 1,
                            DeliveryCode = "120.2.1",
                            PostIndex = x.DeliveryAddress.PostIndex,
                            Region = x.DeliveryAddress.Place,
                            Address = x.DeliveryAddress.Street_1,
                            DeliveryTimeFrom = "10:00",
                            DeliveryTimeTo = "18:00",
                            Comment = "",
                            Items = x.ItemsString
                        });
                    gv.DataBind();
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                }
            }
            Response.Flush();
            Response.End();
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
            requestOrderListRecent.EvalDateLastModified = false;
            var responseOrderList = orderApiClient.GetOrderListRecent(requestOrderListRecent);

            foreach (var orderList in responseOrderList.GetOrderListRecentResult)
            {
                DeliveryParameters orderToDelivery = new DeliveryParameters
                {
                    OrderId = orderList.OrderID,
                    CreatedAt = orderList.CreatedDate
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
                deliveryItems.Add(new DeliveryItem
                {
                    ItemId = responseItem.ProductID,
                    Name = responseItem.Description,
                    Quantity = responseItem.Quantity,
                    Cost = decimal.Parse(responseItem.TaxableAmount, CultureInfo.InvariantCulture),
                    PayCost = decimal.Parse(responseItem.TaxableAmount, CultureInfo.InvariantCulture)
                });
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
        }

        private void FillItemWeights(DeliveryParameters deliveryParameters)
        {
            BiWellEntities db = new BiWellEntities();

            foreach (var item in deliveryParameters.Items)
            {
                var itemWeight = db.ItemWeights.Find(item.ItemId);
                if (itemWeight == null)
                {
                    throw new InvalidOperationException($"Весовая характеристика не найдена для {item.ItemId}: {item.Name}");
                }

                item.Weight = itemWeight.Weight;
            }
        }
    }
}