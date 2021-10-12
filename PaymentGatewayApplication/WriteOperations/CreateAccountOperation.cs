using Abstractions;
using PaymentGateway.Application;
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
        private readonly IEventSender _eventSender;
        private readonly AccountOptions _accountOptions;

        public CreateAccountOperation(IEventSender eventSender, AccountOptions accountOptions)
        {
            _eventSender = eventSender;
            _accountOptions = accountOptions;
        }
        public void PerformOperation(CreateAccountCommand operation)
        {
            Database database = Database.GetInstance();
            Account account = new Account
            {
                Balance = _accountOptions.InitialBalance,
                Currency = operation.Currency,
                IbanCode = operation.IbanCode,
                Type = operation.Type,
                Status = operation.Status,
                Limit = operation.Limit
            };

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
            AccountCreated eventAccountCreated = new(operation.IbanCode, operation.Type, operation.Status);
            
            _eventSender.SendEvent(eventAccountCreated);
            database.SaveChanges();

        }
    }
}
