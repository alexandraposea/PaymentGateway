using Abstractions;
using System;

namespace PaymentGatewayExternalService
{
    public class EventSender : IEventSender
    {
        public void SendEvent(object e)
        {
            Console.WriteLine("Event" + e.GetType().FullName);
        }
    }
}