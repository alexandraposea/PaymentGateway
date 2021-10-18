using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentGateway.Models
{
    public partial class ProductXTransaction
    {
        public int ProductId { get; set; }
        public int TransactionId { get; set; }
        public double Quantity { get; set; }
        public decimal Value { get; set; }
        public string Name { get; set; }

        public virtual Product Product { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
