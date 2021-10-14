using MediatR;
using System;

namespace PaymentGatewayPublishedLanguage.Events
{
    public class AccountUpdated: INotification
    {
        public string IbanCode { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }

        public AccountUpdated(string ibanCode, DateTime date, double amount)
        {
            IbanCode = ibanCode;
            Date = date;
            Amount = amount;
        }
    }
}
