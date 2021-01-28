using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;
using EI.RP.DataModels.Sap.CrmUmcUrm.Functions;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Membership;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.ModelExtensions;

namespace EI.RP.DomainServices.Queries.Membership.ForgotPasswordRequestResults
{
	internal class ForgotPasswordRequestResultQueryHandler : QueryHandler<ForgotPasswordRequestResultQuery>
	{
		private readonly ISapRepositoryOfCrmUmcUrm _repository;
		private readonly ISapRepositoryOfUserManagement _repositoryOfUserManagement;

		public ForgotPasswordRequestResultQueryHandler(ISapRepositoryOfCrmUmcUrm repository, ISapRepositoryOfUserManagement repositoryOfUserManagement)
		{
			_repository = repository;
			_repositoryOfUserManagement = repositoryOfUserManagement;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(ForgotPasswordRequestResult) };


		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			ForgotPasswordRequestResultQuery queryModel)
		{
			var requestDetailsTask =
				_repository.ExecuteFunctionWithSingleResult(new GetRequestDetailsFunction {Query={ID = queryModel.RequestId}});

			var tempPasswordTask =
				_repository.ExecuteFunctionWithSingleResult(new GetTempPasswordFunction{Query={ID=queryModel.RequestId}});

			await Task.WhenAll(requestDetailsTask, tempPasswordTask);

			var requestDetails = await requestDetailsTask;
			var tempPassword = await tempPasswordTask;

			var isValid = false;
			var statusCode = requestDetails.STATUS_CODE;
			var email = requestDetails.USERNAME?.AdaptToSapUserNameFormat();
			var temporalPassword = tempPassword.Passcode?.FromBase64UrlEncoded();

			if (statusCode == UserRequestStatusCode.Open)
			{
				isValid = true;
			}
			else if (statusCode == UserRequestStatusCode.Completed)
			{
				try
				{
					var sessionInfo = await _repositoryOfUserManagement.LoginUser(
						email,
						temporalPassword
					);

					isValid = true;
				}
				catch (Exception)
				{
					isValid = false;
				}
			}

			return new[]
			{
				new ForgotPasswordRequestResult
				{
					IsValid = isValid,
					Email = email,
					TemporalPassword = temporalPassword,
					StatusCode = statusCode
				}
			}.Cast<TQueryResult>().ToArray();
		}
	}
}