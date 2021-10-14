﻿using MediatR;
using System;

namespace PaymentGatewayPublishedLanguage.Events
{
    public class TransactionCreated : INotification
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }

        public TransactionCreated(double amount, string currency, DateTime date)
        {
            Amount = amount;
            Currency = currency;
            Date = date;
        }
    }
}
