using MediatR;
using System.Collections.Generic;
using static PaymentGatewayPublishedLanguage.Commands.PurchaseProductCommand;

namespace PaymentGatewayPublishedLanguage.Events
{
    public class ProductPurchased : INotification
    {
        public List<PurchaseProductDetail> ProductDetails = new();
    }
}
