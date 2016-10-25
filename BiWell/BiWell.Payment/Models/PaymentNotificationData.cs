namespace BiWell.Payment.Models
{
    public class PaymentNotificationData
    {
        public string mnt_id { get; set; }
        public string mnt_transaction_id { get; set; }
        public string mnt_operation_id { get; set; }
        public string mnt_amount { get; set; }
        public string mnt_currency_code { get; set; }
        public string mnt_subscriber_id { get; set; }
        public string mnt_test_mode { get; set; }
        public string mnt_signature { get; set; }
    }
}