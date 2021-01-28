using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System.Identity;
using EI.RP.DataModels.Events;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Infrastructure;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using FluentValidation;
using NLog;

namespace EI.RP.DomainServices.Commands.BusinessPartner.Deregister
{

    internal class DeRegisterBusinessPartnerCommandHandler : 
		ICommandHandler<DeRegisterBusinessPartnerCommand>, 
		ICommandValidator<DeRegisterBusinessPartnerCommand>,
		ICommandEventBuilder<DeRegisterBusinessPartnerCommand>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly IUserSessionProvider _userSessionProvider;
		private readonly ISapRepositoryOfCrmUmc _repositoryOfCrmUmc;
		private readonly ICommandHandler<PublishBusinessActivityDomainCommand> _businessActivityPublisher;
        private readonly IClientInfoResolver _clientInfoResolver;

        public DeRegisterBusinessPartnerCommandHandler(IUserSessionProvider userSessionProvider,ISapRepositoryOfCrmUmc repositoryOfCrmUmc, ICommandHandler<PublishBusinessActivityDomainCommand> businessActivityPublisher, IClientInfoResolver clientInfoResolver)
		{
			_userSessionProvider = userSessionProvider;
			_repositoryOfCrmUmc = repositoryOfCrmUmc;
			_businessActivityPublisher = businessActivityPublisher;
			_clientInfoResolver = clientInfoResolver;
        }

		public Task ValidateAsync(DeRegisterBusinessPartnerCommand command)
		{
			if (!_userSessionProvider.IsAgentOrAdmin())
				throw new ValidationException("Operation not authorized for the user");

			return Task.CompletedTask;
		}

		private readonly List<ContractItemDto> _successfulResults=new List<ContractItemDto>();
		private readonly List<ContractItemDto> _erroredResults = new List<ContractItemDto>();
		public async Task ExecuteAsync(DeRegisterBusinessPartnerCommand command)
        {
            var userAccount =await (_repositoryOfCrmUmc.NewQuery<AccountDto>().Key(command.PartnerNumber).GetOne());

            var contracts = (await _repositoryOfCrmUmc
					.NewQuery<AccountDto>()
					.Key(command.PartnerNumber)
					.NavigateTo<ContractItemDto>()
					.Expand(x => x.BusinessAgreement)
					.GetMany())
				.ToArray();
			var businessAgreementId = contracts.First().BusinessAgreementID;

			await RemoveAccountRegistration();
			foreach (var contract in contracts)
			{
				try
				{
					await ClearEBillerFlag(contract);
					_successfulResults.Add(contract);
				}
				catch (Exception ex)
				{
					Logger.Warn(()=>ex.ToString());
					_erroredResults.Add(contract);
				}
			}

			async Task ClearEBillerFlag(ContractItemDto contract)
			{
				var activityType = command.IsSingleUserBusinessPartner
					? PublishBusinessActivityDomainCommand.BusinessActivityType.DeregisterSingleUserBp
					: PublishBusinessActivityDomainCommand.BusinessActivityType.DeregisterMultipleUserBp;

				await _businessActivityPublisher.ExecuteAsync(
					new PublishBusinessActivityDomainCommand(activityType, command.PartnerNumber,
						businessAgreementId));

				contract.BusinessAgreement.EBiller = string.Empty;

				await _repositoryOfCrmUmc.Update(contract.BusinessAgreement);
			}

			async Task RemoveAccountRegistration()
			{
				var additionalAccount = new AdditionalAccountRegistrationDto();
				additionalAccount.BusinessAgreementID = businessAgreementId;
				additionalAccount.PoD = string.Empty;
				additionalAccount.UsrCategory = UserCategoryType.None;
				additionalAccount.UserName = userAccount.Username;

				await _repositoryOfCrmUmc.Delete(additionalAccount);
			}
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(DeRegisterBusinessPartnerCommand command)
		{

			var currentUser=_userSessionProvider
				.CurrentUserClaimsPrincipal
				.Claims
				.Single(x => x.Type == ClaimTypes.Email)
				.Value;

			var result = _successfulResults
				.Select(x=>BuildEvent(x, EventAction.LastOperationWasSuccessful))
				.Union(_erroredResults.Select(x => BuildEvent(x, EventAction.LastOperationFailed)))
				.ToArray();
			return Task.FromResult(result);
			IEventApiMessage BuildEvent(ContractItemDto contract, long actionId)
			{
				var @event = new EventApiEvent(_clientInfoResolver);
				@event.CategoryId = EventCategory.AdminEvent;
				@event.SubCategoryId = EventSubCategory.BusinessPartnerDeregistration;

				@event.ActionId = actionId;
				@event.Username = currentUser;
				@event.Partner = long.Parse(command.PartnerNumber);
				@event.ContractAccount = long.Parse(contract.BusinessAgreementID);
				@event.Description = "De-Register BP";
				return @event;
			}

		}

		

		public Task<IEventApiMessage[]> BuildEventsOnError(DeRegisterBusinessPartnerCommand command, AggregateException exceptions)
		{
			return Task.FromResult<IEventApiMessage[]>(null);
		}
	}

}