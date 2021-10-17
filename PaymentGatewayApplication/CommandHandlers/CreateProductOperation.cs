using Abstractions;
using MediatR;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using PaymentGateway.PublishedLanguage.Events;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.CommandHandlers
{
    public class CreateProductOperation : IRequestHandler<CreateProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;

        public CreateProductOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
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

            _dbContext.Products.Add(product);
            ProductCreated eventProductCreated = new() { Name = request.Name, Currency = request.Currency, Limit = request.Limit, Value = request.Value };
            await _mediator.Publish(eventProductCreated, cancellationToken);
            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
