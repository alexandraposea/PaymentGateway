using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentGateway.Models
{
    public partial class Person
    {
        public Person()
        {
            Accounts = new HashSet<Account>();
        }

        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Cnp { get; set; }
        public int? TypeOfPerson { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
