using MediatR;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.CommandHandlers
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;
        public EnrollCustomerOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {
            var random = new Random();

            var person = new Person
            {
                Cnp = request.UniqueIdentifier,
                Name = request.Name
            };
            if (request.ClientType == "Company")
            {
                person.TypeOfPerson = (int)PersonType.Company;
            }
            else if (request.ClientType == "Individual")
            {
                person.TypeOfPerson = (int)PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupported person type");
            }


            _dbContext.Persons.Add(person);
            _dbContext.SaveChanges();

            var account = new Account
            {
                PersonId = person.PersonId,
                Type = request.AccountType,
                Currency = request.Currency,
                Balance = 0,
                IbanCode = random.Next(100000).ToString(),
                Status = "Active"
            };

            _dbContext.Accounts.Add(account);

            CustomerEnrolled eventCustomerEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType);
            await _mediator.Publish(eventCustomerEnroll, cancellationToken);
            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
