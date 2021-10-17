using MediatR;
using System;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class WithdrawMoneyCommand : IRequest
    {
        public decimal Amount { get; set; }
        public string IbanCode { get; set; }
        public int? AccountId { get; set; }
        public string UniqueIdentifier { get; set; }
        public int? PersonId { get; set; }
        public string Currency { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public DateTime DateOfOperation { get; set; }
    }
}
