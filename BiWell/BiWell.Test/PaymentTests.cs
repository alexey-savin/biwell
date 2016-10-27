using BiWell.Payment.Controllers;
using BiWell.Payment.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace BiWell.Test
{
    [TestClass]
    public class PaymentTest
    {
        [TestMethod]
        public void Can_Echo()
        {
            // Arrange
            PayUrlController controller = new PayUrlController(null);

            // Act
            var result = controller.Echo();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, "Bad StatusCode");
            Assert.AreEqual("BiWell Web Api", result.Content.ReadAsStringAsync().Result, "Unexpected content");
        }

        [TestMethod]
        public void PaymentNotification_Success()
        {
            // Arrange
            PayUrlController controller = new PayUrlController(null);

            PaymentNotificationData data = new PaymentNotificationData()
            {
                mnt_id = "44",
                mnt_transaction_id = "ORDER-1025"
            };

            // Act
            var result = controller.PaymentNotification(data);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, "Bad StatusCode");
            Assert.AreEqual("SUCCESS", result.Content.ReadAsStringAsync().Result, "Unexpected content");
        }

        [TestMethod]
        public void Fail_When_Notification_Data_Is_Null()
        {
            // Arrange
            PayUrlController controller = new PayUrlController(null);

            PaymentNotificationData data = null;

            // Act
            var result = controller.PaymentNotification(data);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, "Bad StatusCode");
            Assert.AreEqual("FAIL", result.Content.ReadAsStringAsync().Result, "Unexpected content");
        }

        [TestMethod]
        public void Fail_When_Notification_Data_Is_Wrong()
        {
            // Arrange
            PayUrlController controller = new PayUrlController(null);

            PaymentNotificationData data = new PaymentNotificationData();
            
            // Act
            var result = controller.PaymentNotification(data);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, "Bad StatusCode");
            Assert.AreEqual("FAIL", result.Content.ReadAsStringAsync().Result, "Unexpected content");
        }
    }
}
