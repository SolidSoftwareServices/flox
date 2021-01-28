using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Infrastructure.Mappers;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using NLog;

namespace EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan
{
	internal class SmartActivationPlanQueryHandler : QueryHandler<SmartActivationPlanQuery>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IResidentialPortalDataRepository _repository;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IDomainMapper _mapper;

		public SmartActivationPlanQueryHandler(IResidentialPortalDataRepository repository,
			IDomainQueryResolver queryResolver,IDomainMapper mapper)
		{
			_repository = repository;
			_queryResolver = queryResolver;
			_mapper = mapper;
		}

		protected override Type[] ValidQueryResultTypes { get; } =
			{typeof(Ei.Rp.DomainModels.SmartActivation.SmartPlan)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			SmartActivationPlanQuery query)
		{
			var accounts = await _queryResolver.GetDuelFuelAccountsByAccountNumber(query.AccountNumber, true);

			var planGroup = accounts.Count() == 1
				? SmartPlanGroup.Single
				: SmartPlanGroup.Dual;
			var plans = await _repository.GetSmartActivationPlans(planGroup);
			var dataModels = await Task.WhenAll(plans
				.Where(x => x.IsAvailable)
				.Select(x=>_mapper.Map<SmartActivationPlanDataModel,Ei.Rp.DomainModels.SmartActivation.SmartPlan>(x)));

			return dataModels.Cast<TQueryResult>().ToArray();
		}
	}
}