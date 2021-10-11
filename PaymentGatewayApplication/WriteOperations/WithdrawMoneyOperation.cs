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
    public class WithdrawMoneyOperation : IWriteOperation<WithdrawMoneyCommand>
    {
        IEventSender eventSender;
        public WithdrawMoneyOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(WithdrawMoneyCommand operation)
        {
            Database database = Database.GetInstance();

            Account account;
            Person person;

            if (operation.AccountId.HasValue)
            {
                account = database.Accounts.FirstOrDefault(x => x.AccountId == operation.AccountId);
            }
            else
            {
                account = database.Accounts.FirstOrDefault(x => x.IbanCode == operation.IbanCode);
            }

            if (operation.PersonId.HasValue)
            {
                person = database.Persons.FirstOrDefault(x => x.PersonId == operation.PersonId);
            }
            else
            {
                person = database.Persons.FirstOrDefault(x => x.Cnp == operation.UniqueIdentifier);
            }

            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if (person == null)
            {
                throw new Exception("Person not found");
            }

            var exists = database.Accounts.Any(x => x.PersonId == person.PersonId && x.AccountId == account.AccountId);

            if (!exists)
            {
                throw new Exception("The person is not associated with the account!");
            }

            if(operation.Amount > account.Balance)
            {
                throw new Exception("You don't have sufficient funds!");
            }

            account.AccountId = operation.AccountId;
            person.PersonId = operation.PersonId;
            Transaction transaction = new Transaction();
            transaction.Amount = -operation.Amount;
            transaction.Currency = operation.Currency;
            transaction.Date = operation.DateOfTransaction;
            account.Balance -= operation.Amount;

            database.Transactions.Add(transaction);

            TransactionCreated eventTransactionCreated = new(operation.Amount, operation.Currency, operation.DateOfTransaction);
            AccountUpdated eventAccountUpdated = new(operation.IbanCode, operation.DateOfOperation, operation.Amount);
            eventSender.SendEvent(eventTransactionCreated);
            eventSender.SendEvent(eventAccountUpdated);
            database.SaveChanges();
        }
    }
}
