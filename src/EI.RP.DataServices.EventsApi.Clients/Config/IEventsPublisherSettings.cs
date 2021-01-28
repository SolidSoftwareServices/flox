using System;
using System.Threading.Tasks;

namespace EI.RP.DataServices.EventsApi.Clients.Config
{
    public interface IEventsPublisherSettings
    {
        bool UseMockEventsPublisher { get; }
        string EvenLogApiUrlPrefix { get; }
        TimeSpan CheckForPendingEventsInterval { get; }
        Task<string> EventsBearerTokenProviderUrlAsync();
    }
}