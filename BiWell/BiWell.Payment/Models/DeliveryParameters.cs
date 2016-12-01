namespace BiWell.Payment.Models
{
    public class DeliveryParameters
    {
        public string ticket { get; set; }
        public string ver => "1";
        public string callbackURL { get; set; }

        public DeliveryItem[] Items { get; set; }
        public DeliveryAddress Address { get; set; }
        public DeliveryRecipient Recipient { get; set; }
    }
}