using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using PaymentGateway.PublishedLanguage.Events;

namespace PaymentGateway.Application.CommandHandlers
{
    public class PurchaseProductOperation : IRequestHandler<PurchaseProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;

        public PurchaseProductOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
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

            var totalAmount = 0.0m;
            Product product;

            foreach (var item in request.ProductDetails)
            {
                product = _dbContext.Products.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Insufficient stocks!");
                }
                product.Limit -= item.Quantity;

                totalAmount += (Decimal)item.Quantity * product.Value;
            }

            if (account.Balance < totalAmount)
            {
                throw new Exception("You have insufficient funds!");
            }

            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Date = DateTime.Now,
                Amount = -totalAmount,
                Currency = account.Currency,
                Type = "Purchase"
            };
            _dbContext.Transactions.Add(transaction);
            account.Balance -= totalAmount;
            _dbContext.SaveChanges();

            foreach (var item in request.ProductDetails)
            {
                product = _dbContext.Products.FirstOrDefault(x => x.ProductId == item.ProductId);
                var productXTransaction = new ProductXTransaction
                {
                    TransactionId = transaction.TransactionId,
                    ProductId = product.ProductId,
                    Quantity = item.Quantity,
                    Value = product.Value,
                    Name = product.Name
                };
                _dbContext.ProductXTransactions.Add(productXTransaction);
            }


            ProductPurchased eventProductPurchased = new() { ProductDetails = request.ProductDetails };
            await _mediator.Publish(eventProductPurchased, cancellationToken);
            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
