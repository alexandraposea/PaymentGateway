using MediatR;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.CommandHandlers
{
    public class DepositMoneyOperation : IRequestHandler<DepositMoneyCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;

        public DepositMoneyOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {

            Account account;
            Person person;

            if (request.AccountId.HasValue)
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanCode);
            }

            if (request.PersonId.HasValue)
            {
                person = _dbContext.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _dbContext.Persons.FirstOrDefault(x => x.Cnp == request.UniqueIdentifier);
            }

            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if (person == null)
            {
                throw new Exception("Person not found");
            }

            var exists = _dbContext.Accounts.Any(x => x.PersonId == person.PersonId && x.AccountId == account.AccountId);

            if (!exists)
            {
                throw new Exception("The person is not associated with the account!");
            }

            //account.AccountId = request.AccountId;
            //person.PersonId = request.PersonId;
            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                Date = request.DateOfTransaction,
                Type = "Deposit"
            };

            account.Balance += request.Amount;

            _dbContext.Transactions.Add(transaction);

            TransactionCreated eventTransactionCreated = new(request.Amount, request.Currency, request.DateOfTransaction);
            AccountUpdated eventAccountUpdated = new(request.IbanCode, request.DateOfOperation, request.Amount);
            await _mediator.Publish(eventTransactionCreated, cancellationToken);
            await _mediator.Publish(eventAccountUpdated, cancellationToken);
            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
