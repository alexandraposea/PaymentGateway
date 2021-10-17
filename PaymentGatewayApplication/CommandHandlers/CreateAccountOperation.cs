
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace PaymentGateway.Application.CommandHandlers
{
    public class CreateAccountOperation : IRequestHandler<CreateAccountCommand>

    {
        private readonly IMediator _mediator;
        private readonly AccountOptions _accountOptions;
        private readonly PaymentDbContext _dbContext;
        public CreateAccountOperation(IMediator mediator, AccountOptions accountOptions, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Balance = _accountOptions.InitialBalance,
                Currency = request.Currency,
                IbanCode = request.IbanCode,
                Type = request.Type,
                Status = request.Status,
                Limit = request.Limit
            };

            Person person;
            if (request.PersonId.HasValue)
            {
                person = _dbContext.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _dbContext.Persons.FirstOrDefault(x => x.Cnp == request.UniqueIdentifier);
            }

            if (person == null)
            {
                throw new Exception("Person not found!");
            }

            account.PersonId = person.PersonId;
            _dbContext.Accounts.Add(account);
            AccountCreated eventAccountCreated = new(request.IbanCode, request.Type, request.Status);

            await _mediator.Publish(eventAccountCreated, cancellationToken);
            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
