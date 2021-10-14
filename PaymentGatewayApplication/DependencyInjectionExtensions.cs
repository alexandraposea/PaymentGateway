using Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGatewayApplication.WriteOperations;
using PaymentGatewayData;
using static PaymentGatewayApplication.Queries.ListOfAccounts;

namespace PaymentGateway.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<EnrollCustomerOperation>();
            //services.AddTransient<CreateAccountOperation>();
            //services.AddTransient<DepositMoneyOperation>();
            //services.AddTransient<WithdrawMoneyOperation>();
            //services.AddTransient<PurchaseProductOperation>();

            services.AddSingleton<Database>();

            services.AddTransient<IValidator<Query>, Validator>();
            services.AddTransient<QueryHandler>();

            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var options = new AccountOptions
                {
                    InitialBalance = config.GetValue("AccountOptions:InitialBalance", 0)
                };
                return options;
            });

            return services;
        }
    }
}