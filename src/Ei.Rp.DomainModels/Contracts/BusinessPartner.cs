using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.Contracts
{
	public class BusinessPartner : IQueryResult
	{
		public int CommunicationsLevel { get; set; }
		public int MeterConfiguration { get; set; }
		public string NumPartner { get; set; }
		public string Description { get; set; }
	}
}