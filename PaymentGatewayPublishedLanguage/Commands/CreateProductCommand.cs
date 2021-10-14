

using MediatR;

namespace PaymentGatewayPublishedLanguage.Commands
{
    public class CreateProductCommand : IRequest
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }
    }
}
