using System.Threading.Tasks;

namespace S3.CoreServices.Encryption
{
	public interface IEncryptionSettings
	{

		Task<string> EncryptionPassPhraseAsync();
		
		Task<string> EncryptionSaltValueAsync();

		Task<string> EncryptionInitVectorAsync ();
	}
}