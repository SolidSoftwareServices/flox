using System;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Encryption
{
	public interface IEncryptionSettings
	{

		Task<string> EncryptionPassPhraseAsync();
		
		Task<string> EncryptionSaltValueAsync();

		Task<string> EncryptionInitVectorAsync ();
	}
}