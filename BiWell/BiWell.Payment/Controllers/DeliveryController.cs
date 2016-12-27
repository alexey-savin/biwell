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
            Response.AddHeader("content-disposition", "attachment;filename=Delivery.xls");
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
                            Phone = x.Recipient.Phone,
                            Phone2 = "",
                            Email = x.Recipient.Email,
                            IssueType = 80,
                            PaymentType = 0,
                            PayCost = 0,
                            BalanceDue = 0,
                            Weight = x.Items.Sum(di => di.Weight) * 1000,
                            PlacesQty = 0,
                            DeliveryCode = "120.2.1",
                            PostIndex = x.Address.PostIndex,
                            Region = x.Address.Place,
                            Address = x.Address.Street,
                            DeliveryTimeFrom = "",
                            DeliveryTimeTo = "",
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
                var orderToDelivery = CreateDeliveryParameters(orderList.OrderID);

                orderToDelivery.OrderId = orderList.OrderID;
                orderToDelivery.CreatedAt = orderList.CreatedDate;

                result.Add(orderToDelivery);
            }

            return result;
        }

        private DeliveryParameters CreateDeliveryParameters(int orderId)
        {
            DeliveryParameters deliveryParameters = new DeliveryParameters();

            FillFromFreedom(deliveryParameters, orderId);
            FillItemWeights(deliveryParameters);

            return deliveryParameters;
        }

        private void FillFromFreedom(DeliveryParameters deliveryParameters, int orderId)
        {
            var orderApiClient = ByDesignAPIHelper.CreateOrderAPIClient();
            var orderDetailsResponse = orderApiClient.GetOrderDetailsInfo_V2(orderApiClient.CreateCredentials(), orderId);
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

            var orderInfoResponse = orderApiClient.GetOrderInfo_V2(orderApiClient.CreateCredentials(), orderId);
            if (orderInfoResponse.Success == 0)
            {
                throw new InvalidOperationException(orderInfoResponse.Message);
            }

            deliveryParameters.Status = orderInfoResponse.Status;
            deliveryParameters.ShipMethodId = orderInfoResponse.ShipMethodID;
            deliveryParameters.ShipMethod = orderInfoResponse.ShipMethod;

            DeliveryAddress address = new DeliveryAddress
            {
                PostIndex = orderInfoResponse.ShipPostalCode,
                Place = orderInfoResponse.ShipCity,
                Street = orderInfoResponse.ShipStreet1
            };

            DeliveryRecipient recipient = new DeliveryRecipient
            {
                FullName = orderInfoResponse.ShipName1,
                Phone = orderInfoResponse.ShipPhone,
                Email = orderInfoResponse.ShipEmail
            };

            deliveryParameters.Items = deliveryItems.ToArray();
            deliveryParameters.Address = address;
            deliveryParameters.Recipient = recipient;
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