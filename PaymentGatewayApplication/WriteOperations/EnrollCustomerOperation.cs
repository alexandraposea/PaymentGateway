using Abstractions;
using PaymentGatewayData;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage;
using PaymentGatewayPublishedLanguage.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayApplication.WriteOperations
{
    public class EnrollCustomerOperation : IWriteOperation<EnrollCustomerCommand>
    {
        public IEventSender eventSender;
        private readonly Database _database;
        public EnrollCustomerOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(EnrollCustomerCommand operation)
        {
            var random = new Random();
            var database = Database.GetInstance();

            Person person = new Person();
            person.Cnp = operation.UniqueIdentifier;
            person.Name = operation.Name;
            if (operation.ClientType == "Company")
            {
                person.TypeOfPerson = PersonType.Company;
            }
            else if (operation.ClientType == "Individual")
            {
                person.TypeOfPerson = PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupported person type");
            }


            database.Persons.Add(person);

            Account account = new Account();
            account.Type = operation.AccountType;
            account.Currency = operation.Currency;
            account.Balance = 0;
            account.IbanCode = random.Next(100000).ToString();

            database.Accounts.Add(account);


            CustomerEnrolled eventCustomerEnroll = new(operation.Name, operation.UniqueIdentifier, operation.ClientType);
            eventSender.SendEvent(eventCustomerEnroll);
            database.SaveChanges();

        }
    }
}
