namespace BiWell.Payment.Models
{
    public class OrderClientInfo
    {
        public string ClientNumber { get; private set; }
        public bool IsRep { get; private set; }

        public static OrderClientInfo ExtractFromOrder(ByDesignOrderAPI.GetOrderResponse_V2 responseOrderInfo)
        {
            string custOrRep = string.Empty;

            bool isRep = false;
            if (!string.IsNullOrEmpty(responseOrderInfo.CustomerNumber))
            {
                custOrRep = responseOrderInfo.CustomerNumber;
            }
            else
            {
                custOrRep = responseOrderInfo.RepNumber;
                isRep = true;
            }

            OrderClientInfo result = new OrderClientInfo
            {
                ClientNumber = custOrRep,
                IsRep = isRep
            };

            return result;
        }
    }
}