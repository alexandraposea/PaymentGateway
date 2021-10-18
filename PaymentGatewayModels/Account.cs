using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentGateway.Models
{
    public partial class Account
    {
        public Account()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int AccountId { get; set; }
        public int PersonId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string IbanCode { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal Limit { get; set; }

        public virtual Person Person { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
