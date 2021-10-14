﻿using System;

namespace PaymentGatewayModels
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
    }
}
