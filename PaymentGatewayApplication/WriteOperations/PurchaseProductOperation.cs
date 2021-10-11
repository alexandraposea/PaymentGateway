using Abstractions;
using PaymentGatewayData;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage.Events;
using PaymentGatewayPublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayApplication.WriteOperations
{
    public class PurchaseProductOperation : IWriteOperation<PurchaseProductCommand>
    {
        IEventSender eventSender;
        public PurchaseProductOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(PurchaseProductCommand operation)
        {
            Database database = Database.GetInstance();
            Account account;
            Person person;

            if (operation.AccountId.HasValue)
            {
                account = database.Accounts.FirstOrDefault(x => x.AccountId == operation.AccountId);
            }
            else
            {
                account = database.Accounts.FirstOrDefault(x => x.IbanCode == operation.IbanCode);
            }

            if (operation.PersonId.HasValue)
            {
                person = database.Persons.FirstOrDefault(x => x.PersonId == operation.PersonId);
            }
            else
            {
                person = database.Persons.FirstOrDefault(x => x.Cnp == operation.UniqueIdentifier);
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

            var totalAmount = 0.0;
            //Product product;
            foreach (var item in operation.ProductDetails)
            {
                var product = database.Products.FirstOrDefault(x => x.ProductId == item.ProductId);

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

            Transaction transaction = new Transaction();
            transaction.Amount = -totalAmount;
            database.Transactions.Add(transaction);
            account.Balance -= totalAmount;

            foreach (var item in operation.ProductDetails)
            {
                var product = database.Products.FirstOrDefault(x => x.ProductId == item.ProductId);
                ProductXTransaction productXTransaction = new ProductXTransaction();
                productXTransaction.TransactionId = transaction.TransactionId;
                productXTransaction.ProductId = item.ProductId;
                productXTransaction.Quantity = item.Quantity;
                productXTransaction.Value = product.Value;
                productXTransaction.Name = product.Name;
            }

            ProductPurchased eventProductPurchased = new ProductPurchased { ProductDetails = operation.ProductDetails };
            eventSender.SendEvent(eventProductPurchased);
            database.SaveChanges();
        }
    }
}
