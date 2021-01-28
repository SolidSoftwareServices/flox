using System.Threading.Tasks;
using EI.RP.CoreServices.Profiling;

namespace EI.RP.CoreServices.Encryption
{

	public interface IEncryptionService
	{


		Task<string> EncryptAsync(string source, bool addPrefix = false);
		Task<string> DecryptAsync(string source,bool onlyIfHasPrefix=false);
		
		Task<TModel> DecryptModelAsync<TModel>(TModel request) where TModel : class;
		string GetSha1(string source);
	}
}
