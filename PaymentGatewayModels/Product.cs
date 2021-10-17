namespace PaymentGateway.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }
    }
}
