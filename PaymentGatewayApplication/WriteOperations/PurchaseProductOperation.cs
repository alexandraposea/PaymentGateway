﻿using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using PaymentGateway.PublishedLanguage.Events;

namespace PaymentGateway.Application.WriteOperations
{
    public class PurchaseProductOperation : IRequestHandler<PurchaseProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;

        public PurchaseProductOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
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

            var totalAmount = 0d;
            Product product;

            foreach (var item in request.ProductDetails)
            {
                product = _database.Products.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Insufficient stocks!");
                }
                product.Limit -= item.Quantity;

                totalAmount += item.Quantity * product.Value;
            }

            if (account.Balance < totalAmount)
            {
                throw new Exception("You have insufficient funds!");
            }

            var transaction = new Transaction
            {
                Amount = -totalAmount
            };
            _database.Transactions.Add(transaction);
            account.Balance -= totalAmount;

            foreach (var item in request.ProductDetails)
            {
                product = _database.Products.FirstOrDefault(x => x.ProductId == item.ProductId);
                var productXTransaction = new ProductXTransaction
                {
                    TransactionId = transaction.TransactionId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Value = product.Value,
                    Name = product.Name
                };
            }

            ProductPurchased eventProductPurchased = new() { ProductDetails = request.ProductDetails };
            await _mediator.Publish(eventProductPurchased, cancellationToken);
            _database.SaveChanges();
            return Unit.Value;
        }
    }
}
