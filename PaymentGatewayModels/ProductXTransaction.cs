namespace PaymentGateway.Models
{
    public class ProductXTransaction
    {
        public int ProductId { get; set; }
        public int TransactionId { get; set; }
        public double Quantity { get; set; }
        public decimal Value { get; set; }
        public string Name { get; set; }
    }
}
