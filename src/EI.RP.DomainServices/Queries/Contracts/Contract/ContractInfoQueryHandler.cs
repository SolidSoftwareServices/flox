using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.DomainServices.Infrastructure.Mappers;
using NLog;

namespace EI.RP.DomainServices.Queries.Contracts.Contract
{
	internal class ContractInfoQueryHandler : QueryHandler<ContractInfoQuery>
	{
		private readonly ISapRepositoryOfCrmUmc _repository;		
		private readonly IDomainMapper _domainMapper;
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public ContractInfoQueryHandler(ISapRepositoryOfCrmUmc repository, IDomainMapper domainMapper)
		{
			_repository = repository;
			_domainMapper = domainMapper;
		}

		protected override Type[] ValidQueryResultTypes { get; } = { typeof(ContractItem) };

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			ContractInfoQuery query)
		{
			var contracts = await _repository.NewQuery<BusinessAgreementDto>()
								.Key(query.AccountNumber)
								.NavigateTo<ContractItemDto>()
								.Expand(x => x.ContractItemEXTAttrs)
								.GetMany();

			var result = await Task.WhenAll(contracts
					.Select(x => _domainMapper.Map<ContractItemDto, ContractItem>(x)));

			return result.Cast<TQueryResult>().ToArray();
		}

	}
}