using BiWell.Payment.Models;
using BiWell.Payment.Helpers;
using System;
using System.Linq;
using System.Globalization;
using System.Web.Mvc;
using RestSharp;
using BiWell.Payment.Models.Checkout;
using System.Collections.Generic;

namespace BiWell.Payment.Controllers
{
    public class OrderController : Controller
    {
        private const string OrderWithValidPayment = "ORDER_WITH_VALID_PAYMENT";
        private const string OrderWithNoPayment = "ORDER_WITH_NO_PAYMENT";
        private const string NoOrderOnRecord = "NO_ORDER_ON_RECORD";

        public ActionResult Payment(OrderDetails orderDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(orderDetails.OrderId))
                {
                    throw new InvalidOperationException("Номер заказа не указан");
                }

                int orderId = 0;
                if (!int.TryParse(orderDetails.OrderId, out orderId))
                {
                    throw new InvalidCastException("Некорректный формат номера заказа");
                }

                var orderApiClient = ByDesignAPIHelper.CreateOrderAPIClient();
                var orderApiCred = orderApiClient.CreateCredentials();

                var responseOrderInfo = orderApiClient.GetOrderInfo_V2(orderApiCred, orderId);
                if (responseOrderInfo.Success == 0)
                {
                    throw new InvalidOperationException(responseOrderInfo.Message);
                }

                int custNum = 0;
                if (int.TryParse(responseOrderInfo.CustomerNumber, out custNum))
                {
                    if (custNum < 2000)
                    {
                        string[] startKitIds = Properties.Settings.Default.Freedom_StartKitItemId.Split(',');
                        bool isStartKitFound = false;

                        foreach (var startKitId in startKitIds)
                        {
                            if (isStartKitFound) break;

                            var responseCustDidOrder = orderApiClient.CheckOrderedItemForCustomerDIDWithinDate_V2(
                            orderApiCred,
                            responseOrderInfo.CustomerNumber,
                            startKitId,
                            new DateTime(2016, 12, 14));

                            if (responseCustDidOrder.Success == 0)
                            {
                                throw new InvalidOperationException($"Не удается проверить заказы с товаром '{startKitId}' для кастомера '{responseOrderInfo.CustomerNumber}'");
                            }

                            isStartKitFound = (responseCustDidOrder.OrderID > 0); // checking without payment

                            if (!isStartKitFound) 
                            {
                                var orderDetailsResponse = orderApiClient.GetOrderDetailsInfo_V2(orderApiCred, orderId);
                                if (orderDetailsResponse.Success == 0)
                                {
                                    throw new InvalidOperationException(orderDetailsResponse.Message);
                                }

                                isStartKitFound = orderDetailsResponse.OrderDetailsResponse
                                    .Any(x => x.ProductID.Equals(startKitId));
                            }
                        }

                        if (!isStartKitFound)
                        {
                            throw new InvalidOperationException("Стартовый продуктовый набор должен быть заказан");
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("Некоррекный формат кода кастомера");
                }

                var responseTotals = orderApiClient.GetTotals(orderApiCred, orderId);
                if (responseTotals.Success == 0)
                {
                    throw new InvalidOperationException(responseTotals.Message);
                }

                orderDetails.Amount = decimal.Parse(responseTotals.BalanceDue, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return View("OrderError", ex);
            }

            return View(orderDetails);
        }

        public ActionResult Delivery(OrderDetails orderDetails)
        {
            DeliveryParameters deliveryParameters = null;

            try
            {
                if (string.IsNullOrEmpty(orderDetails.OrderId))
                {
                    throw new InvalidOperationException("Номер заказа не указан");
                }

                int orderId = 0;
                if (!int.TryParse(orderDetails.OrderId, out orderId))
                {
                    throw new InvalidCastException("Некорректный формат номера заказа");
                }

                deliveryParameters = CreateDeliveryParameters(orderId);
            }
            catch(Exception ex)
            {
                return View("OrderError", ex);
            }

            return View(deliveryParameters);
        }

        private DeliveryParameters CreateDeliveryParameters(int orderId)
        {
            DeliveryParameters deliveryParameters = new DeliveryParameters();

            FillFromFreedom(deliveryParameters, orderId);
            FillItemWeights(deliveryParameters);
            FillCheckoutParameters(deliveryParameters, orderId);

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

        private void FillCheckoutParameters(DeliveryParameters deliveryParameters, int orderId)
        {
            var checkoutApiClient = new RestClient(Properties.Settings.Default.CheckoutBaseUrl);
            var request = new RestRequest("service/login/ticket/{apiKey}", Method.GET);
            request.AddUrlSegment("apiKey", Properties.Settings.Default.CheckoutApiKey);

            IRestResponse<TicketResponse> response = checkoutApiClient.Execute<TicketResponse>(request);
            TicketResponse ticketResponse = response.Data;

            if (ticketResponse.error)
            {
                throw new InvalidOperationException("Ошибка получения сессионного ключа: " + ticketResponse.errorMessage);
            }

            deliveryParameters.ticket = ticketResponse.ticket;
            deliveryParameters.callbackURL = $@"http://payment.biwell.ru/app/order/payment/{orderId}";
        }
    }
}