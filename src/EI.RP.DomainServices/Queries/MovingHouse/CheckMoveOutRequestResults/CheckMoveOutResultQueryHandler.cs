using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataModels.Sap.CrmUmc.Functions;
using EI.RP.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;

namespace EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults
{
	internal class CheckMoveOutResultQueryHandler : QueryHandler<CheckMoveOutRequestResultQuery>
	{
		private readonly ISapRepositoryOfCrmUmc _repository;

		public CheckMoveOutResultQueryHandler(ISapRepositoryOfCrmUmc repository)
		{
			_repository = repository;
		}

		protected override Type[] ValidQueryResultTypes { get; } = { typeof(CheckMoveOutRequestResult) };

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			CheckMoveOutRequestResultQuery queryModel)
		{
            var requestResult = new CheckMoveOutRequestResult();

            try
            {                
                var result = await _repository.ExecuteFunctionWithSingleResult(new CheckMoveOutFunction { Query = { ContractID = queryModel.ContractID } },false);
                requestResult.IsMoveOutOk = result.Result;  
            }
            catch (DomainException ex)
            {
                if (ex.DomainError.Equals(ResidentialDomainError.FeeNotifier))
                {
                    requestResult.HasExitFee = true;
                    requestResult.IsMoveOutOk = true;
                } else
                {
                    throw;
                }
            }

            return new[]
			{
                requestResult
            }.Cast<TQueryResult>().ToArray();
		}
	}
}