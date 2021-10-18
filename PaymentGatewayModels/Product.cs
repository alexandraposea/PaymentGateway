using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentGateway.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductXtransactions = new HashSet<ProductXTransaction>();
        }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }

        public virtual ICollection<ProductXTransaction> ProductXtransactions { get; set; }
    }
}
