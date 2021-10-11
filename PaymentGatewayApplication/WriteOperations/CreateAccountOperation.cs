using Abstractions;
using PaymentGatewayData;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage.Events;
using PaymentGatewayPublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayApplication.WriteOperations
{
    public class CreateAccountOperation : IWriteOperation<CreateAccountCommand>

    {
        public IEventSender eventSender;

        public CreateAccountOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(CreateAccountCommand operation)
        {
            Database database = Database.GetInstance();
            Account account = new Account();
            account.Balance = operation.Balance;
            account.Currency = operation.Currency;
            account.IbanCode = operation.IbanCode;
            account.Type = operation.Type;
            account.Status = operation.Status;
            account.Limit = operation.Limit;

            Person person;
            if (operation.PersonId.HasValue)
            {
                person = database.Persons.FirstOrDefault(x => x.PersonId == operation.PersonId);

            }
            else
            {
                person = database.Persons?.FirstOrDefault(x => x.Cnp == operation.UniqueIdentifier);
            }

            if (person == null)
            {
                throw new Exception("Person not found!");
            }

            account.PersonId = person.PersonId;
            database.Accounts.Add(account);
            database.SaveChanges();

            AccountCreated eventAccountCreated = new(operation.IbanCode, operation.Type, operation.Status);
            eventSender.SendEvent(eventAccountCreated);
        }
    }
}
