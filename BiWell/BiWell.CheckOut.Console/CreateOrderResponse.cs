namespace BiWell.CheckOut.Console
{
    public class CreateOrderResponse : Response
    {
        public OrderResponse order { get; set; }
        public DeliveryResponse delivery { get; set; }
    }

    public class OrderResponse
    {
        public int id { get; set; }
    }

    public class DeliveryResponse
    {
        public int id { get; set; }
        public string serviceName { get; set; }
        public decimal cost { get; set; }
    }
}
