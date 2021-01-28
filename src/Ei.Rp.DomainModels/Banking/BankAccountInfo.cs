using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Banking
{
	public class BankAccountInfo : IQueryResult
	{
		public string AccountNumber { get; set; }
		public string IBAN { get; set; }
		public PaymentMethodType PaymentMethod { get; set; }
		public string BIC { get; set; }
		public string NameInBankAccount { get; set; }
        public string BankAccountId { get; set; }
    }
}