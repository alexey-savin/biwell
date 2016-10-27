using System;
using BiWell.Payment.Interfaces;
using BiWell.Payment.Models;

namespace BiWell.Payment.Implementation
{
    public class ByDesignOrderProcessor : IPaymentNotificationProcessor
    {
        public void ProcessNotification(PaymentNotificationData data)
        {
            // UPDATE ByDesign Order Status to Posted if it is Entered
        }
    }
}