using System.Threading.Tasks;
using PRZ.PushCenter.Subscriptions;

namespace PRZ.PushCenter.Push.Handler
{
    public interface IPushMessageHandler
    {
        SubscriptionType SubscriptionType { get; }

        Task Handle(PushMessage message);
    }
}