using System.Threading.Tasks;
using Microsoft.Data.Edm;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Edmx
{
	public interface IEdmxResolver
	{
		IEdmModel Parse(string edmxString);
		Microsoft.OData.Edm.IEdmModel Parse2(string edmxString);
	}
}