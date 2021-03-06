using MediatR;
using System.Collections.Generic;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class PurchaseProductCommand : IRequest
    {
        public List<PurchaseProductDetail> ProductDetails = new();
        public int? PersonId { get; set; }
        public string UniqueIdentifier { get; set; }
        public int? AccountId { get; set; }
        public string IbanCode { get; set; }

        public class PurchaseProductDetail
        {
            public int ProductId { get; set; }
            public double Quantity { get; set; }

        }

    }

}
