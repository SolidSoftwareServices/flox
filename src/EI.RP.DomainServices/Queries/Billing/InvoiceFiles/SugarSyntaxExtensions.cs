using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Billing;

namespace EI.RP.DomainServices.Queries.Billing.InvoiceFiles
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{
		public static async Task<InvoiceFile> GetInvoiceFile(this IDomainQueryResolver provider, string accountNumber,
			string referenceDocNumber, bool byPassPipeline = false)
		{
			return (await provider.FetchAsync<InvoiceFileQuery, InvoiceFile>(new InvoiceFileQuery
			{
				ReferenceDocNumber = referenceDocNumber,
				AccountNumber = accountNumber
			}, byPassPipeline)).SingleOrDefault();
		}
	}
}