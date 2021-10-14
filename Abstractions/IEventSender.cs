using System;

namespace Abstractions
{
    public interface IEventSender
    {
        void SendEvent(Object e);
    }
}
