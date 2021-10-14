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
        private readonly IMediator _mediator;
        public CreateProductOperation(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Database database = Database.GetInstance();

            Product product = new Product();
            product.ProductId = request.ProductId;
            product.Name = request.Name;
            product.Value = request.Value;
            product.Currency = request.Currency;
            product.Limit = request.Limit;

            database.Products.Add(product);
            ProductCreated eventProductCreated = new ProductCreated { Name = request.Name, Currency = request.Currency, Limit = request.Limit, Value = request.Value };
            await _mediator.Publish(eventProductCreated, cancellationToken);
            database.SaveChanges();
            return Unit.Value;
        }

    }
}
