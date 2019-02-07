using System.Threading.Tasks;
using PRZ.PushCenter.Bll.Subscriptions;

namespace PRZ.PushCenter.Bll.Push.Handler
{
    public interface IPushMessageHandler
    {
        SubscriptionType SubscriptionType { get; }

        Task Handle(PushMessage message);
    }
}