using Abstractions;
using MediatR;
using PaymentGatewayData;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage.Events;
using PaymentGatewayPublishedLanguage.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGatewayApplication.WriteOperations
{
    public class WithdrawMoneyOperation : IRequestHandler<WithdrawMoneyCommand>
    {
        IEventSender eventSender;
        public WithdrawMoneyOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
        {
            Database database = Database.GetInstance();

            Account account;
            Person person;

            if (request.AccountId.HasValue)
            {
                account = database.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = database.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanCode);
            }

            if (request.PersonId.HasValue)
            {
                person = database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = database.Persons.FirstOrDefault(x => x.Cnp == request.UniqueIdentifier);
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

            if (request.Amount > account.Balance)
            {
                throw new Exception("You don't have sufficient funds!");
            }

            account.AccountId = request.AccountId;
            person.PersonId = request.PersonId;
            Transaction transaction = new Transaction();
            transaction.Amount = -request.Amount;
            transaction.Currency = request.Currency;
            transaction.Date = request.DateOfTransaction;
            account.Balance -= request.Amount;

            database.Transactions.Add(transaction);

            TransactionCreated eventTransactionCreated = new(request.Amount, request.Currency, request.DateOfTransaction);
            AccountUpdated eventAccountUpdated = new(request.IbanCode, request.DateOfOperation, request.Amount);
            eventSender.SendEvent(eventTransactionCreated);
            eventSender.SendEvent(eventAccountUpdated);
            database.SaveChanges();
            return Unit.Task;
        }

    }
}
