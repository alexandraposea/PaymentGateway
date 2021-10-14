using System.Collections.Generic;

namespace PaymentGatewayModels
{
    public class Person
    {
        public int? PersonId { get; set; }
        public string Name { get; set; }
        public string Cnp { get; set; }
        public PersonType TypeOfPerson { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}
