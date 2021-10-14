using Abstractions;
using PaymentGateway.Application;
using PaymentGatewayData;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage.Events;
using PaymentGatewayPublishedLanguage.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace PaymentGatewayApplication.WriteOperations
{
    public class CreateAccountOperation : IRequestHandler<CreateAccountCommand>

    {
        private readonly IMediator _mediator;
        private readonly AccountOptions _accountOptions;
        private readonly Database _database;
        public CreateAccountOperation(IMediator mediator, AccountOptions accountOptions, Database database)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _database = database;
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
                person = _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.Cnp == request.UniqueIdentifier);
            }

            if (person == null)
            {
                throw new Exception("Person not found!");
            }

            account.PersonId = person.PersonId;
            _database.Accounts.Add(account);
            AccountCreated eventAccountCreated = new(request.IbanCode, request.Type, request.Status);

            await _mediator.Publish(eventAccountCreated, cancellationToken);
            Database.SaveChanges();
            return Unit.Value;
        }
    }
}
