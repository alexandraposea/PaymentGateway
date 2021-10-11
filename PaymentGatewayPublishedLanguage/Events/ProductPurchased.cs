using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentGatewayPublishedLanguage.WriteSide.PurchaseProductCommand;

namespace PaymentGatewayPublishedLanguage.Events
{
    public class ProductPurchased
    {
        public List<PurchaseProductDetail> ProductDetails = new List<PurchaseProductDetail>();
    }
}
