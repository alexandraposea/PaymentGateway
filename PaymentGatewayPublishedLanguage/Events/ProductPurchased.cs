using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentGatewayPublishedLanguage.Commands.PurchaseProductCommand;

namespace PaymentGatewayPublishedLanguage.Events
{
    public class ProductPurchased: INotification
    {
        public List<PurchaseProductDetail> ProductDetails = new List<PurchaseProductDetail>();
    }
}
