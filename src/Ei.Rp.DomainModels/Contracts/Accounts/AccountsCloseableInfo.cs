using System;
using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.Contracts.Accounts
{
	public class AccountsCloseableInfo : IQueryResult
	{
		public string ElectricityAccountNumber { get; set; }
		public string GasAccountNumber { get; set; }
		public DateTime ElectricityEffectiveClosingDate { get; set; }
		public DateTime GasEffectiveClosingDate { get; set; }
		public bool CanClose { get; set; }
		public string ReasonCannotClose { get; set; }
		public string UpfLcpe { get; set; }
	}
}