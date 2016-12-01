﻿using BiWell.Payment.Models;
using BiWell.Payment.Helpers;
using System;
using System.Globalization;
using System.Web.Mvc;
using RestSharp;
using BiWell.Payment.Models.Checkout;
using System.Collections.Generic;

namespace BiWell.Payment.Controllers
{
    public class OrderController : Controller
    {
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

                var response = orderApiClient.GetTotals(orderApiClient.CreateCredentials(), orderId);
                if (response.Success == 0)
                {
                    throw new InvalidOperationException(response.Message);
                }

                orderDetails.Amount = decimal.Parse(response.BalanceDue, CultureInfo.InvariantCulture);
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
            FillCheckoutParameters(deliveryParameters);

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

        private void FillCheckoutParameters(DeliveryParameters deliveryParameters)
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
            deliveryParameters.callbackURL = "about:blank";
        }
    }
}