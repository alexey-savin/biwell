using BiWell.Payment.Models;

namespace BiWell.Payment.Interfaces
{
    public interface IPaymentNotificationProcessor
    {
        void ProcessNotification(PaymentNotificationData data);
    }
}
