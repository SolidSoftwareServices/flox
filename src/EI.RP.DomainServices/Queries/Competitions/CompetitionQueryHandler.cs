using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using Ei.Rp.DomainModels;
using Ei.Rp.DomainModels.Competitions;
using NLog;

namespace EI.RP.DomainServices.Queries.Competitions
{
	internal class CompetitionQueryHandler : QueryHandler<CompetitionQuery>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IResidentialPortalDataRepository _repository;

		public CompetitionQueryHandler(IResidentialPortalDataRepository repository)
		{
			_repository = repository;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(CompetitionEntry)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			CompetitionQuery queryModel)
		{
			CompetitionEntryDto entry = null;
			try
			{
				entry = await _repository.GetCompetitionEntry(queryModel.UserEmail);
			}
			catch (Exception ex)
			{
				//swallowing the exception is valid in this case. DO NOT COPY
				Logger.Error(() => $"Could not fetch competition entry for {queryModel.UserEmail}. {ex}");
			}

            return entry != null
                ? new CompetitionEntry {Answer = entry.Answer}.ToOneItemArray().Cast<TQueryResult>().ToArray()
                : new TQueryResult[0];
        }
	}
}