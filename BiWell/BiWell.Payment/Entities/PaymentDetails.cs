using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace BiWell.Payment.Entities
{
    public class PaymentDetails
    {
        [Required(ErrorMessage = "Номер заказа должен быть указан")]
        [Display(Name = "Номер заказа")]
        public string OrderId { get; set; }
        
        public decimal Amount { get; set; }

        public string FormattedAmount => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Amount);

        public string WidgetUrl
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Properties.Settings.Default.MNT_WIDGET_HOST);
                sb.Append("?");
                sb.Append($"MNT_ID={Properties.Settings.Default.MNT_ID}");
                sb.Append($"&MNT_TRANSACTION_ID={OrderId}");
                sb.Append($"&MNT_AMOUNT={FormattedAmount}");
                sb.Append($"&MNT_CURRENCY_CODE={Properties.Settings.Default.MNT_CURRENCY_CODE}");

                return sb.ToString();
            }
        }
    }
}