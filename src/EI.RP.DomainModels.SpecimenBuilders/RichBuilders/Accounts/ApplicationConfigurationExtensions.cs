using System.Linq;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.Mocks;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts
{
	internal static class ApplicationConfigurationExtensions
	{
		public static void ConfigureAccountInfo(this DomainQueryResolverMock queryResolver, AccountInfo accountInfoModel, PointReferenceNumber prn)
		{
			var result = accountInfoModel.ToOneItemArray();

			queryResolver.ExpectQuery(new AccountInfoQuery
			{
				AccountNumber = accountInfoModel.AccountNumber,
			}, result);

			queryResolver.ExpectQuery(new AccountInfoQuery
			{
				AccountNumber = accountInfoModel.AccountNumber,
			}, result.Select(_=>(AccountOverview)_).ToArray());

			queryResolver.ExpectQuery(new AccountInfoQuery
			{
				Prn = prn,
			}, result);
			

			queryResolver.ExpectQuery(new AccountInfoQuery
			{
				Prn = prn,
			}, result.Select(_=>(AccountOverview)_).ToArray());

		}

		
	}
}