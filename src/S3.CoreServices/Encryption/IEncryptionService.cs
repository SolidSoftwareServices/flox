using System.Threading.Tasks;

namespace S3.CoreServices.Encryption
{

	public interface IEncryptionService
	{


		Task<string> EncryptAsync(string source, bool addPrefix = false);
		Task<string> DecryptAsync(string source,bool onlyIfHasPrefix=false);
		
		Task<TModel> DecryptModelAsync<TModel>(TModel request) where TModel : class;
		string GetSha1(string source);
	}
}
