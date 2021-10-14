using Abstractions;
using PaymentGatewayData;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using PaymentGatewayPublishedLanguage.Events;

namespace PaymentGatewayApplication.WriteOperations
{
    public class PurchaseProductOperation : IRequestHandler<PurchaseProductCommand>
    {
        private readonly IMediator _mediator;
        public PurchaseProductOperation(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
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

            var totalAmount = 0d;
            Product product;
            // select sum(quantity) as TotalQUantity, productId from input group by productid => listOfTotals
            // select sum(i.quantity*p.price) from input i inner join product p on productid..
            //operation.ProductDetails.GroupBy() // Sum() // GroupByJoin
            foreach (var item in request.ProductDetails)
            {
                product = database.Products.FirstOrDefault(x => x.ProductId == item.ProductId);
                // select * from Product where id = item.ProductId
                // var totalPerProduct = listOfTotals.First(x-> x.productid == productid);

                // if(totalPerProduct.TotalQUantity > product.Limit)
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
            database.Transactions.Add(transaction);
            account.Balance -= totalAmount;

            foreach (var item in request.ProductDetails)
            {
                product = database.Products.FirstOrDefault(x => x.ProductId == item.ProductId);
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
            database.SaveChanges();
            return Unit.Value;
        }
    }
}
