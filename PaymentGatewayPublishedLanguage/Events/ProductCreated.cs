﻿using MediatR;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class ProductCreated : INotification
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }

    }
}
