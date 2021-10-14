using MediatR;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class AccountCreated : INotification
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
