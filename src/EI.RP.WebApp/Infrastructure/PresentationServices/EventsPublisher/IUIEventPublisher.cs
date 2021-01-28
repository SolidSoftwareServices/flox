using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.EventsPublisher
{
	public interface IUIEventPublisher
	{
		Task Publish(UiEventInfo eventInfo);
	}


	class UiEventPublisherAdapter : IUIEventPublisher
	{
		private readonly IEventApiEventPublisher _publisher;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public UiEventPublisherAdapter(IEventApiEventPublisher publisher,IClientInfoResolver clientInfoResolver,IUserSessionProvider userSessionProvider)
		{
			_publisher = publisher;
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}
		private static class EventAction
		{
			public const long LastOperationWasSuccessful = 1;
			public const long LastOperationFailed = 2;
		}
		public async Task Publish(UiEventInfo eventInfo)
		{
			if (!eventInfo.IsValid())
			{
				throw new InvalidOperationException("Invalid event info");
			}
			await _publisher.Publish(new EventApiEvent(_clientInfoResolver)
			{
				Description = eventInfo.Description,
				ContractAccount = eventInfo.AccountNumber != null ? long.Parse(eventInfo.AccountNumber) : default(long),
				Partner = eventInfo.Partner != null ? long.Parse(eventInfo.Partner) : default(long),
				MPRN = eventInfo.MPRN,
				CategoryId = eventInfo.CategoryId,
				SubCategoryId = eventInfo.SubCategoryId,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				ActionId = EventAction.LastOperationWasSuccessful

			}.Validate());
		}
	}
}