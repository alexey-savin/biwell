using Newtonsoft.Json;
using RestSharp;

namespace BiWell.CheckOut.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://platform.checkout.ru";

            //
            var client = new RestClient(baseUrl);
            var request = new RestRequest("service/login/ticket/{apiKey}", Method.GET);
            request.AddUrlSegment("apiKey", Properties.Settings.Default.ApiKey);

            IRestResponse<TicketResponse> response = client.Execute<TicketResponse>(request);
            TicketResponse tr = response.Data;

            bool isValidSession = false;
            if (!tr.error)
            {
                System.Console.WriteLine("Ticket = " + tr.ticket);
                isValidSession = true;
            }
            else
            {
                System.Console.WriteLine("Ticket error: " + tr.errorMessage);
            }

            if (isValidSession)
            {
                request = new RestRequest();
                request.Method = Method.POST;
                request.Resource = "service/order/create";

                CreateOrderRequest coReq = new CreateOrderRequest();
                coReq.apiKey = Properties.Settings.Default.ApiKey;
                coReq.order = new OrderRequest
                {
                    shopOrderId = "test-001",
                    forcedCost = 0,
                    forceLabelPrinting = false,
                    paymentMethod = "cash",
                    comment = "this is a test order",
                    goods = new ProductRequest[]
                    {
                        new ProductRequest
                        {
                            name = "Teapot Silver",
                            code = "13137",
                            quantity = 1,
                            assessedCost = 100.23M,
                            payCost = 100.21M,
                            weight = 0.5M
                        },
                        new ProductRequest
                        {
                            name = "Gold Pot",
                            code = "109313",
                            quantity = 1,
                            assessedCost = 101.23M,
                            payCost = 101.21M,
                            weight = 0.5M
                        }
                    }
                };
                coReq.delivery = new DeliveryRequest
                {
                    deliveryId = -1,
                    placeFiasId = "53bcff9a-1b70-4492-9b3f-b128a7de0727",
                    type = "pvz",
                    addressPvz = "Moscow",
                    cost = 10.0M,
                    minTerm = 1,
                    maxTerm = 3
                };
                coReq.user = new UserRequest
                {
                    fullname = "Получатель посылки",
                    email = "sinnert1997@hotmail.com",
                    phone = "+7(905)782-99-32"
                };

                request.AddJsonBody(coReq);

                // debug
                var json = JsonConvert.SerializeObject(coReq);
                
                var r = client.Execute<CreateOrderResponse>(request);
                CreateOrderResponse coResp  = r.Data;

                if (coResp == null)
                {
                    System.Console.WriteLine($"{r.StatusDescription}: {r.ErrorMessage}");
                }
                else 
                {
                    if (!coResp.error)
                    {
                        System.Console.WriteLine("Order was created = " + coResp.order.id);
                    }
                    else
                    {
                        System.Console.WriteLine("Create order error: " + coResp.errorMessage);
                    }
                }
            }

            System.Console.ReadKey();
        }
    }
}
