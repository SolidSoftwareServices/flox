using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

namespace S3.CoreServices.Encryption
{
	internal interface ICryptoTransformFactory
	{
		Task<ICryptoTransform> Create();
		bool Return(Task<ICryptoTransform> obj);
	}
	internal interface IEncryptTransformFactory:ICryptoTransformFactory
	{
	}	
	internal interface IDecryptTransformFactory:ICryptoTransformFactory
	{
	}	
	abstract class CryptoTransformFactory : PooledObjectPolicy<Task<ICryptoTransform>>
	{
		private readonly IEncryptionSettings _settings;
		private readonly bool _isEncryptor;

		protected CryptoTransformFactory(IEncryptionSettings settings, bool isEncryptor)
		{
			_settings = settings;
			_isEncryptor = isEncryptor;
		}

		public override async Task<ICryptoTransform> Create()
		{
			return await BuildTransformAsync(_isEncryptor);
		}

		public override bool Return(Task<ICryptoTransform> obj)
		{
			return true;
		}

		protected async Task<ICryptoTransform> BuildTransformAsync(bool forEncryption)
		{
			const int passwordIteration = 2;
			const string hashAlgorithm = "MD5";
			const int keySize = 256;

			var saltValueAsync = _settings.EncryptionSaltValueAsync();
			var initVectorAsync = _settings.EncryptionInitVectorAsync();
			var phraseAsync = _settings.EncryptionPassPhraseAsync();


			var saltValueBytes = Encoding.UTF8.GetBytes(await saltValueAsync);
			var initVectorBytes = Encoding.UTF8.GetBytes(await initVectorAsync);

			var password = new PasswordDeriveBytes(await phraseAsync, saltValueBytes,
				hashAlgorithm,
				passwordIteration);

			var keyBytes = password.GetBytes(keySize / 8);

			var symmetricKey = new AesManaged();

			symmetricKey.Padding =PaddingMode.PKCS7;

			symmetricKey.Mode = CipherMode.CBC;

			return forEncryption
				? symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
				: symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
		}
	}

	class EncryptTransformFactory : CryptoTransformFactory,IEncryptTransformFactory
	{
		public EncryptTransformFactory(IEncryptionSettings settings) : base(settings,true)
		{
		}
	}
	class DecryptTransformFactory : CryptoTransformFactory,IDecryptTransformFactory
	{
		public DecryptTransformFactory(IEncryptionSettings settings) : base(settings,false)
		{
		}
	}
}