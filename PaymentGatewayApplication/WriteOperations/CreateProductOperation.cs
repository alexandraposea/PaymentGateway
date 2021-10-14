using Abstractions;
using MediatR;
using PaymentGatewayData;
using PaymentGatewayModels;
using PaymentGatewayPublishedLanguage.Commands;
using PaymentGatewayPublishedLanguage.Events;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGatewayApplication.WriteOperations
{
    public class CreateProductOperation : IRequestHandler<CreateProductCommand>
    {
        IEventSender eventSender;
        public CreateProductOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Database database = Database.GetInstance();

            Product product = new Product();
            product.ProductId = request.ProductId;
            product.Name = request.Name;
            product.Value = request.Value;
            product.Currency = request.Currency;
            product.Limit = request.Limit;

            database.Products.Add(product);
            ProductCreated eventProductCreated = new ProductCreated { Name = operation.Name, Currency = operation.Currency, Limit = operation.Limit, Value = operation.Value };
            eventSender.SendEvent(eventProductCreated);
            database.SaveChanges();
            return Unit.Task;
        }

    }
}
