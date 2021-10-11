using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayPublishedLanguage.Events
{
    public class AccountUpdated
    {
        public string IbanCode { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }

        public AccountUpdated(string IbanCode, DateTime date, double Amount)
        {

        }
    }
}
