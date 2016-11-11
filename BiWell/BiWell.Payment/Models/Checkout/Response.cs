namespace BiWell.Payment.Models.Checkout
{
    public class Response
    {
        public bool error { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}