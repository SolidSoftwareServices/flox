using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;
using EI.RP.DataModels.Sap.CrmUmcUrm.Functions;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Membership;

namespace EI.RP.DomainServices.Queries.Membership.CreatePasswordRequestResults
{
	internal class CreatePasswordRequestResultQueryHandler : QueryHandler<CreatePasswordRequestResultQuery>
	{
		private readonly ISapRepositoryOfCrmUmcUrm _repositoryOfCrmUmcUrm;

		public CreatePasswordRequestResultQueryHandler(ISapRepositoryOfCrmUmcUrm repositoryOfCrmUmcUrm)
		{
			_repositoryOfCrmUmcUrm = repositoryOfCrmUmcUrm;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(CreatePasswordRequestResult)};


		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			CreatePasswordRequestResultQuery queryModel)
		{
			var requestDetails =
				await _repositoryOfCrmUmcUrm.ExecuteFunctionWithSingleResult(new GetRequestDetailsFunction {Query = {ID = queryModel.RequestId}});

			return new[]
			{
				new CreatePasswordRequestResult
				{
					Email = requestDetails.USERNAME,
					StatusCode = requestDetails.STATUS_CODE,
					DateOfBirth = requestDetails.Birthday
				}
			}.Cast<TQueryResult>().ToArray();
		}
	}
}