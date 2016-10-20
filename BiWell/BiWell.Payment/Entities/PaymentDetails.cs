using System.ComponentModel.DataAnnotations;

namespace BiWell.Payment.Entities
{
    public class PaymentDetails
    {
        [Required(ErrorMessage = "Номер заказа должен быть указан")]
        [Display(Name = "Номер заказа")]
        public string OrderId { get; set; }

        [Required(ErrorMessage = "Сумма платежа должна быть указана")]
        [Display(Name = "Сумма платежа")]
        public decimal Amount { get; set; }
    }
}