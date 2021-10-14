using MediatR;
using System;

namespace PaymentGatewayPublishedLanguage.Commands
{
    public class WithdrawMoneyCommand : IRequest
    {
        public double Amount { get; set; }
        public string IbanCode { get; set; }
        public int? AccountId { get; set; }
        public string UniqueIdentifier { get; set; }
        public int? PersonId { get; set; }
        public string Currency { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public DateTime DateOfOperation { get; set; }
    }
}
