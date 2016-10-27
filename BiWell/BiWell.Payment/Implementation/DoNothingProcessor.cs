using BiWell.Payment.Interfaces;
using BiWell.Payment.Models;

namespace BiWell.Payment.Implementation
{
    public class DoNothingProcessor : IPaymentNotificationProcessor
    {
        public void ProcessNotification(PaymentNotificationData data)
        {
            // NOTHING TODO
        }
    }
}