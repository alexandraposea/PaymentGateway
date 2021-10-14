using Abstractions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGatewayExternalService
{
    public class AllEventsSender : INotificationHandler<INotification>
    {
        public Task Handle(INotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(notification);
            return Task.CompletedTask;
        }

    }
}