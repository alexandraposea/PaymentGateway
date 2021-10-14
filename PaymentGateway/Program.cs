using Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGatewayApplication.Queries;
using PaymentGatewayApplication.WriteOperations;
using PaymentGatewayData;
using PaymentGatewayExternalService;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using static PaymentGatewayPublishedLanguage.Commands.PurchaseProductCommand;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
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

            //Console.WriteLine("\n--------Enroll customer operation-------\n");
            //EnrollCustomerCommand enrollCustomerCommand = new EnrollCustomerCommand();
            //enrollCustomerCommand.Name = "Marina Carey";
            //enrollCustomerCommand.UniqueIdentifier = "2970304234566";
            //enrollCustomerCommand.ClientType = "Individual";
            //enrollCustomerCommand.AccountType = "Economii";
            //enrollCustomerCommand.Currency = "RON";
            //IEventSender eventSender = new EventSender();
            //EnrollCustomerOperation enrollCustomerOperation = new EnrollCustomerOperation(eventSender);
            //enrollCustomerOperation.PerformOperation(enrollCustomerCommand);

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // setup
            var services = new ServiceCollection();
            services.RegisterBusinessServices(Configuration);

            services.AddSingleton<IEventSender, EventSender>();
            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();

            // use
            var enrollCustomer = new EnrollCustomerCommand
            {
                Name = "Ion Popescu",
                UniqueIdentifier = "2970304234566",
                ClientType = "Individual",
                AccountType = "Economii",
                Currency = "RON"
            };

            var enrollCustomerOperation = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            enrollCustomerOperation.Handle(enrollCustomer, default);

            //Console.WriteLine("\n--------Create account operation-------\n");
            //CreateAccountCommand createAccountCommand = new CreateAccountCommand();
            //createAccountCommand.Balance = 23;
            //createAccountCommand.Currency = "RON";
            //createAccountCommand.IbanCode = "23RO54INGB7953235479";
            //createAccountCommand.Type = "Economii";
            //createAccountCommand.Status = "activ";
            //createAccountCommand.Limit = 10000;
            //createAccountCommand.UniqueIdentifier = "2970304234566";
            //CreateAccountOperation createAccountOperation = new CreateAccountOperation(eventSender);
            //createAccountOperation.PerformOperation(createAccountCommand);

            var createAccountCommand = new CreateAccountCommand
            {
                Balance = 23,
                Currency = "RON",
                IbanCode = "23RO54INGB7953235479",
                Type = "Economii",
                Status = "activ",
                Limit = 10000,
                UniqueIdentifier = "2970304234566"
            };
            var createAccountOperation = serviceProvider.GetRequiredService<CreateAccountOperation>();
            createAccountOperation.Handle(createAccountCommand, default);
            var database = Database.GetInstance();

            //Console.WriteLine("\n--------Deposit money operation-------\n");
            //DepositMoneyCommand depositMoneyCommand = new DepositMoneyCommand();
            //depositMoneyCommand.Amount = 300;
            //depositMoneyCommand.IbanCode = "23RO54INGB7953235479";
            //depositMoneyCommand.UniqueIdentifier = "2970304234566";
            //depositMoneyCommand.Currency = "RON";
            //depositMoneyCommand.DateOfTransaction = DateTime.Now;
            //depositMoneyCommand.DateOfOperation = DateTime.Now;
            //DepositMoneyOperation depositMoneyOperation = new DepositMoneyOperation(eventSender);
            //depositMoneyOperation.PerformOperation(depositMoneyCommand);

            var depositMoneyCommand = new DepositMoneyCommand
            {
                Amount = 300,
                IbanCode = "23RO54INGB7953235479",
                UniqueIdentifier = "2970304234566",
                Currency = "RON",
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now
            };

            var makeDeposit = serviceProvider.GetRequiredService<DepositMoneyOperation>();
            makeDeposit.Handle(depositMoneyCommand, default);

            //Console.WriteLine("\n--------Withdraw money operation-------\n");
            //WithdrawMoneyCommand withdrawMoneyCommand = new WithdrawMoneyCommand();
            //withdrawMoneyCommand.Amount = 50;
            //withdrawMoneyCommand.IbanCode = "23RO54INGB7953235479";
            //withdrawMoneyCommand.UniqueIdentifier = "2970304234566";
            //withdrawMoneyCommand.Currency = "RON";
            //withdrawMoneyCommand.DateOfTransaction = DateTime.Now;
            //withdrawMoneyCommand.DateOfOperation = DateTime.Now;
            //WithdrawMoneyOperation withdrawMoneyOperation = new WithdrawMoneyOperation(eventSender);
            //withdrawMoneyOperation.PerformOperation(withdrawMoneyCommand);

            var withdrawMoneyCommand = new WithdrawMoneyCommand
            {
                Amount = 50,
                IbanCode = "23RO54INGB7953235479",
                UniqueIdentifier = "2970304234566",
                Currency = "RON",
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now
            };

            var makeWithdraw = serviceProvider.GetRequiredService<WithdrawMoneyOperation>();
            makeWithdraw.Handle(withdrawMoneyCommand, default);

            //Console.WriteLine("\n--------Create product operation-------\n");
            //CreateProductCommand createProductCommand = new CreateProductCommand();
            //createProductCommand.ProductId = 1;
            //createProductCommand.Name = "Mere";
            //createProductCommand.Value = 4;
            //createProductCommand.Currency = "RON";
            //createProductCommand.Limit = 6;
            //CreateProductOperation createProductOperation = new CreateProductOperation(eventSender);
            //createProductOperation.PerformOperation(createProductCommand);

            //CreateProductCommand createProductCommand2 = new CreateProductCommand();
            //createProductCommand2.ProductId = 2;
            //createProductCommand2.Name = "Pere";
            //createProductCommand2.Value = 6;
            //createProductCommand2.Currency = "RON";
            //createProductCommand2.Limit = 10;
            //CreateProductOperation createProductOperation2 = new CreateProductOperation(eventSender);
            //createProductOperation2.PerformOperation(createProductCommand2);

            var produs = new Product
            {
                ProductId = 1,
                Limit = 10,
                Name = "Pantofi",
                Currency = "Eur",
                Value = 10
            };

            var produs1 = new Product
            {
                ProductId = 2,
                Limit = 5,
                Name = "pantaloni",
                Currency = "Eur",
                Value = 5
            };

            var produs2 = new Product
            {
                ProductId = 3,
                Limit = 3,
                Name = "Camasa",
                Currency = "Eur",
                Value = 3
            };

            database.Products.Add(produs);
            database.Products.Add(produs1);
            database.Products.Add(produs2);

            //Console.WriteLine("\n--------Purchase product operation-------\n");
            //PurchaseProductCommand purchaseProductCommand = new PurchaseProductCommand();
            //purchaseProductCommand.IbanCode = "23RO54INGB7953235479";
            //purchaseProductCommand.UniqueIdentifier = "2970304234566";
            //purchaseProductCommand.ProductDetails = new List<PurchaseProductDetail>
            //{
            //    new PurchaseProductDetail { ProductId = createProductCommand.ProductId, Quantity = 3 },
            //    new PurchaseProductDetail { ProductId = createProductCommand2.ProductId, Quantity = 4 }
            //};
            //PurchaseProductOperation purchaseProductOperation = new PurchaseProductOperation(eventSender);
            //purchaseProductOperation.PerformOperation(purchaseProductCommand);

            var listaProduse = new List<PurchaseProductDetail>();

            var prodCmd1 = new PurchaseProductDetail
            {
                ProductId = 1,
                Quantity = 2
            };

            listaProduse.Add(prodCmd1);

            var prodCmd2 = new PurchaseProductDetail
            {
                ProductId = 2,
                Quantity = 3
            };

            listaProduse.Add(prodCmd2);

            var purchaseProductCommand = new PurchaseProductCommand
            {
                IbanCode = "23RO54INGB7953235479",
                UniqueIdentifier = "2970304234566",
                ProductDetails = listaProduse
            };

            var purchaseProductOperation = serviceProvider.GetRequiredService<PurchaseProductOperation>();
            purchaseProductOperation.Handle(purchaseProductCommand, default);

            var query = new ListOfAccounts.Query
            {
                Cnp = "2970304234566"
            };

            var handler = serviceProvider.GetRequiredService<ListOfAccounts.QueryHandler>();
            var result = handler.Handle(query, default).GetAwaiter().GetResult();
        }
    }
}
