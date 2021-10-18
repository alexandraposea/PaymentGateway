using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentGateway.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            ProductXtransactions = new HashSet<ProductXTransaction>();
        }

        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<ProductXTransaction> ProductXtransactions { get; set; }
    }
}
