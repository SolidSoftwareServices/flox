using System.Threading.Tasks;

namespace EI.RP.CoreServices.Cqrs.Events
{
    public interface IEventApiEventPublisher
    {
        Task Publish<TMessage>(TMessage eventToPublish) where TMessage: IEventApiMessage;
    }
}