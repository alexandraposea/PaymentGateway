﻿using Abstractions;
using MediatR;
using PaymentGatewayData;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage.Commands;
using PaymentGatewayPublishedLanguage.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGatewayApplication.WriteOperations
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly IMediator _mediator;
        public EnrollCustomerOperation(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {
            var random = new Random();
            var database = Database.GetInstance();

            var person = new Person
            {
                Cnp = request.UniqueIdentifier,
                Name = request.Name
            };
            if (request.ClientType == "Company")
            {
                person.TypeOfPerson = PersonType.Company;
            }
            else if (request.ClientType == "Individual")
            {
                person.TypeOfPerson = PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupported person type");
            }


            database.Persons.Add(person);

            var account = new Account
            {
                Type = request.AccountType,
                Currency = request.Currency,
                Balance = 0,
                IbanCode = random.Next(100000).ToString()
            };

            database.Accounts.Add(account);


            CustomerEnrolled eventCustomerEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType);
            await _mediator.Publish(eventCustomerEnroll, cancellationToken);
            database.SaveChanges();
            return Unit.Value;
        }
    }
}
