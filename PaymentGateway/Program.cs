using Abstractions;
using PaymentGatewayApplication.WriteOperations;
using PaymentGatewayExternalService;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage;
using PaymentGatewayPublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using static PaymentGatewayPublishedLanguage.WriteSide.PurchaseProductCommand;

namespace PaymentGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            Account firstAccount = new Account();
            firstAccount.Balance = 100;
            Console.WriteLine(firstAccount.Balance);

            Person firstPerson = new Person();
            firstPerson.Name = "Alexandra";
            Console.WriteLine(firstPerson.Name);

            Product firstService = new Product();
            firstService.Name = ("Service 1");
            firstService.Value = 20.5;
            Console.WriteLine(firstService.Name + " " + firstService.Value);

            Console.WriteLine("-----------------------------------");

            Console.WriteLine("\n--------Enroll customer operation-------\n");
            EnrollCustomerCommand enrollCustomerCommand = new EnrollCustomerCommand();
            enrollCustomerCommand.Name = "Marina Carey";
            enrollCustomerCommand.UniqueIdentifier = "2970304234566";
            enrollCustomerCommand.ClientType = "Individual";
            enrollCustomerCommand.AccountType = "Economii";
            enrollCustomerCommand.Currency = "RON";
            IEventSender eventSender = new EventSender();
            EnrollCustomerOperation enrollCustomerOperation = new EnrollCustomerOperation(eventSender);
            enrollCustomerOperation.PerformOperation(enrollCustomerCommand);

            Console.WriteLine("\n--------Create account operation-------\n");
            CreateAccountCommand createAccountCommand = new CreateAccountCommand();
            createAccountCommand.Balance = 23;
            createAccountCommand.Currency = "RON";
            createAccountCommand.IbanCode = "23RO54INGB7953235479";
            createAccountCommand.Type = "Economii";
            createAccountCommand.Status = "activ";
            createAccountCommand.Limit = 10000;
            createAccountCommand.UniqueIdentifier = "2970304234566";
            CreateAccountOperation createAccountOperation = new CreateAccountOperation(eventSender);
            createAccountOperation.PerformOperation(createAccountCommand);

            Console.WriteLine("\n--------Deposit money operation-------\n");
            DepositMoneyCommand depositMoneyCommand = new DepositMoneyCommand();
            depositMoneyCommand.Amount = 300;
            depositMoneyCommand.IbanCode = "23RO54INGB7953235479";
            depositMoneyCommand.UniqueIdentifier = "2970304234566";
            depositMoneyCommand.Currency = "RON";
            depositMoneyCommand.DateOfTransaction = DateTime.Now;
            depositMoneyCommand.DateOfOperation = DateTime.Now;
            DepositMoneyOperation depositMoneyOperation = new DepositMoneyOperation(eventSender);
            depositMoneyOperation.PerformOperation(depositMoneyCommand);

            Console.WriteLine("\n--------Withdraw money operation-------\n");
            WithdrawMoneyCommand withdrawMoneyCommand = new WithdrawMoneyCommand();
            withdrawMoneyCommand.Amount = 50;
            withdrawMoneyCommand.IbanCode = "23RO54INGB7953235479";
            withdrawMoneyCommand.UniqueIdentifier = "2970304234566";
            withdrawMoneyCommand.Currency = "RON";
            withdrawMoneyCommand.DateOfTransaction = DateTime.Now;
            withdrawMoneyCommand.DateOfOperation = DateTime.Now;
            WithdrawMoneyOperation withdrawMoneyOperation = new WithdrawMoneyOperation(eventSender);
            withdrawMoneyOperation.PerformOperation(withdrawMoneyCommand);

            Console.WriteLine("\n--------Create product operation-------\n");
            CreateProductCommand createProductCommand = new CreateProductCommand();
            createProductCommand.ProductId = 1;
            createProductCommand.Name = "Mere";
            createProductCommand.Value = 4;
            createProductCommand.Currency = "RON";
            createProductCommand.Limit = 6;
            CreateProductOperation createProductOperation = new CreateProductOperation(eventSender);
            createProductOperation.PerformOperation(createProductCommand);

            CreateProductCommand createProductCommand2 = new CreateProductCommand();
            createProductCommand2.ProductId = 2;
            createProductCommand2.Name = "Pere";
            createProductCommand2.Value = 6;
            createProductCommand2.Currency = "RON";
            createProductCommand2.Limit = 10;
            CreateProductOperation createProductOperation2 = new CreateProductOperation(eventSender);
            createProductOperation2.PerformOperation(createProductCommand2);

            Console.WriteLine("\n--------Purchase product operation-------\n");
            PurchaseProductCommand purchaseProductCommand = new PurchaseProductCommand();
            purchaseProductCommand.IbanCode = "23RO54INGB7953235479";
            purchaseProductCommand.UniqueIdentifier = "2970304234566";
            purchaseProductCommand.ProductDetails = new List<PurchaseProductDetail>
            {
                new PurchaseProductDetail { ProductId = createProductCommand.ProductId, Quantity = 3 },
                new PurchaseProductDetail { ProductId = createProductCommand2.ProductId, Quantity = 4 }
            };
            PurchaseProductOperation purchaseProductOperation = new PurchaseProductOperation(eventSender);
            purchaseProductOperation.PerformOperation(purchaseProductCommand);
        }
    }
}
