namespace PaymentGatewayModels
{
    public class Account
    {
        public int? AccountId { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public string IbanCode { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public double Limit { get; set; }
        public int? PersonId { get; set; }
    }
}
