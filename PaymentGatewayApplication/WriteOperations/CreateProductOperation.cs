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
        private readonly Database _database;

        public CreateProductOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {

            var product = new Product
            {
                ProductId = request.ProductId,
                Name = request.Name,
                Value = request.Value,
                Currency = request.Currency,
                Limit = request.Limit
            };

            _database.Products.Add(product);
            ProductCreated eventProductCreated = new() { Name = request.Name, Currency = request.Currency, Limit = request.Limit, Value = request.Value };
            await _mediator.Publish(eventProductCreated, cancellationToken);
            Database.SaveChanges();
            return Unit.Value;
        }
    }
}
