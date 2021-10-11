using PaymentGatewayModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayPublishedLanguage
{
    public class EnrollCustomerCommand
    {
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }
        public string ClientType { get; set; }
        public string AccountType { get; set; }
        public string Currency { get; set; }

    }
}
