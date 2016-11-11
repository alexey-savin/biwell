namespace BiWell.Payment.Models.Checkout
{
    public class TicketResponse : Response
    {
        public string ticket { get; set; }
        public bool receiverEmailNotRequired { get; set; }
        public bool isCashOnDeliveryOnly { get; set; }
    }
}