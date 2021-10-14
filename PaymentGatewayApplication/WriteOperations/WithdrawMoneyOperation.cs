using MediatR;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class WithdrawMoneyOperation : IRequestHandler<WithdrawMoneyCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public WithdrawMoneyOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
        {

            Account account;
            Person person;

            if (request.AccountId.HasValue)
            {
                account = _database.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = _database.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanCode);
            }

            if (request.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.Cnp == request.UniqueIdentifier);
            }

            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if (person == null)
            {
                throw new Exception("Person not found");
            }

            var exists = _database.Accounts.Any(x => x.PersonId == person.PersonId && x.AccountId == account.AccountId);

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
            var transaction = new Transaction
            {
                Amount = -request.Amount,
                Currency = request.Currency,
                Date = request.DateOfTransaction
            };
            account.Balance -= request.Amount;

            _database.Transactions.Add(transaction);

            TransactionCreated eventTransactionCreated = new(request.Amount, request.Currency, request.DateOfTransaction);
            AccountUpdated eventAccountUpdated = new(request.IbanCode, request.DateOfOperation, request.Amount);
            await _mediator.Publish(eventTransactionCreated, cancellationToken);
            await _mediator.Publish(eventAccountUpdated, cancellationToken);
            _database.SaveChanges();
            return Unit.Value;
        }
    }
}
