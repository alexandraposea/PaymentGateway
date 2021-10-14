using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayPublishedLanguage.Events
{
    public class AccountCreated: INotification
    {
        public string IbanCode { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public AccountCreated(string ibanCode, string type, string status)
        {
            IbanCode = ibanCode;
            Type = type;
            Status = status;
        }
    }
}
