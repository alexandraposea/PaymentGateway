using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.Application.Queries;
using PaymentGateway.Data;
using PaymentGateway.ExternalService;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static PaymentGateway.PublishedLanguage.Commands.PurchaseProductCommand;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .Build();

            var services = new ServiceCollection();

            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            services.RegisterBusinessServices(Configuration);
            services.AddPaymentDataAccess(Configuration);


            services.Scan(scan => scan
               .FromAssemblyOf<ListOfAccounts>()
               .AddClasses(classes => classes.AssignableTo<IValidator>())
               .AsImplementedInterfaces()
               .WithScopedLifetime());



            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));

            services.AddMediatR(typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly);

            services.AddScopedContravariant<INotificationHandler<INotification>, AllEventsHandler>(typeof(CustomerEnrolled).Assembly);
            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<PaymentDbContext>();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            // use
            var enrollCustomer = new EnrollCustomerCommand
            {
                Name = "Ion Popescu",
                UniqueIdentifier = "2870304373758666",
                ClientType = "Company",
                AccountType = "Economii",
                Currency = "RON"
            };

            await mediator.Send(enrollCustomer, cancellationToken);

            var createAccountCommand = new CreateAccountCommand
            {
                Balance = 23,
                Currency = "RON",
                IbanCode = "23RO54INGB7953235479",
                Type = "Economii",
                Status = "activ",
                Limit = 10000,
                UniqueIdentifier = "2970304234563"
            };

            await mediator.Send(createAccountCommand, cancellationToken);

            var depositMoneyCommand = new DepositMoneyCommand
            {
                Amount = 300,
                IbanCode = "23RO54INGB7953235479",
                UniqueIdentifier = "2970304234563",
                Currency = "RON",
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now
            };

            await mediator.Send(depositMoneyCommand, cancellationToken);

            var withdrawMoneyCommand = new WithdrawMoneyCommand
            {
                Amount = 50,
                IbanCode = "23RO54INGB7953235479",
                UniqueIdentifier = "2970304234563",
                Currency = "RON",
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now
            };

            await mediator.Send(withdrawMoneyCommand, cancellationToken);

            var product = new Product
            {
                Limit = 10,
                Name = "Pantofi",
                Currency = "Eur",
                Value = 10
            };

            var product1 = new Product
            {
                Limit = 5,
                Name = "pantaloni",
                Currency = "Eur",
                Value = 5
            };

            var product2 = new Product
            {
                Limit = 3,
                Name = "Camasa",
                Currency = "Eur",
                Value = 3
            };

            dbContext.Products.Add(product);
            dbContext.Products.Add(product1);
            dbContext.Products.Add(product2);

            dbContext.SaveChanges();

            var listaProduse = new List<PurchaseProductDetail>();

            var prodCmd1 = new PurchaseProductDetail
            {
                ProductId = product1.ProductId,
                Quantity = 2
            };

            listaProduse.Add(prodCmd1);

            var prodCmd2 = new PurchaseProductDetail
            {
                ProductId = product2.ProductId,
                Quantity = 3
            };

            listaProduse.Add(prodCmd2);

            var purchaseProductCommand = new PurchaseProductCommand
            {
                IbanCode = "23RO54INGB7953235479",
                UniqueIdentifier = "2970304234563",
                ProductDetails = listaProduse
            };

            await mediator.Send(purchaseProductCommand, cancellationToken);

            var query = new ListOfAccounts.Query
            {
                Cnp = "2970304234563"
            };

            var result = await mediator.Send(query, cancellationToken);
        }
    }
}
