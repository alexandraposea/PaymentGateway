using MediatR;
using System;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class AccountUpdated : INotification
    {
        public string IbanCode { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public AccountUpdated(string ibanCode, DateTime date, decimal amount)
        {
            IbanCode = ibanCode;
            Date = date;
            Amount = amount;
        }
    }
}
