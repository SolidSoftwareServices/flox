using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using S3.CoreServices.System;
using S3.CoreServices.System.FastReflection;

namespace S3.CoreServices.Encryption
{
	internal class DefaultEncryptionService : IEncryptionService
	{
		private const string EncryptedPrefix = "EF-*";

		private static readonly string DecryptModelPropertiesCacheKeyPrefix =
			$"{typeof(DefaultEncryptionService).FullName}{nameof(DecryptModelAsync)}";

		private readonly ObjectPool<Task<ICryptoTransform>> _decryptTransformFactory;
		private readonly ObjectPool<Task<ICryptoTransform>> _encryptTransformFactory;

		public DefaultEncryptionService(ObjectPool<Task<ICryptoTransform>> encryptTransformFactory,
			ObjectPool<Task<ICryptoTransform>> decryptTransformFactory)
		{
			_encryptTransformFactory = encryptTransformFactory;
			_decryptTransformFactory = decryptTransformFactory;
		}

		
		public async Task<string> EncryptAsync(string plainText, bool addPrefix = false)
		{


			string result;

			if (!string.IsNullOrEmpty(plainText) && (!addPrefix ||
			                                         !plainText.StartsWith(EncryptedPrefix,
				                                         StringComparison.InvariantCultureIgnoreCase)))
			{
				var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
				byte[] cipherTextBytes;

				using (var memoryStream = new MemoryStream())
				{
					var transform = _encryptTransformFactory.Get();
					try
					{
						using (var cryptoStream = new CryptoStream(memoryStream, await transform, CryptoStreamMode.Write))
						{
							cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

							cryptoStream.FlushFinalBlock();

							cipherTextBytes = memoryStream.ToArray();
						}
					}
					finally
					{
						_encryptTransformFactory.Return(transform);
					}
				}

				result = $"{(addPrefix ? EncryptedPrefix : string.Empty)}{Convert.ToBase64String(cipherTextBytes)}";
			}
			else
			{
				result = plainText;
			}

			return result;

		}


		public async Task<string> DecryptAsync(string source, bool onlyIfHasPrefix = false)
		{

			string result;
			if (!onlyIfHasPrefix || (source != null &&
			                         source.StartsWith(EncryptedPrefix,
				                         StringComparison.InvariantCultureIgnoreCase)))
			{
				if (onlyIfHasPrefix)
				{
					source = source.Substring(EncryptedPrefix.Length);
				}


				var cipherTextBytes = Convert.FromBase64String(source);
				int decryptedByteCount;
				byte[] plainTextBytes;

				using (var memoryStream = new MemoryStream(cipherTextBytes))
				{
					var cryptoTransform = _decryptTransformFactory.Get();
					try
					{
						using (var cryptoStream = new CryptoStream(memoryStream,
							await cryptoTransform,
							CryptoStreamMode.Read))
						{
							plainTextBytes = new byte[cipherTextBytes.Length];

							decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
							cryptoStream.Flush();
							cryptoStream.Close();
						}

						memoryStream.Flush();
					}
					finally
					{
						_decryptTransformFactory.Return(cryptoTransform);
					}
				}

				var decrypt = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
				result = decrypt;
			}
			else
			{
				result = source;
			}

			return result;
		}

		public async Task<TModel> DecryptModelAsync<TModel>(TModel request) where TModel : class
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var result = request.CloneDeep();
			if (result != null)
				foreach (var propertyInfo in result.GetPropertiesFast(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty,
					cacheKey: $"{DecryptModelPropertiesCacheKeyPrefix}{typeof(TModel).FullName}"))
					if (request.GetPropertyValueFast(propertyInfo) is string source)
						result.SetPropertyValueFast(propertyInfo, await DecryptAsync(source, true));

			return result;
		}


		public string GetSha1(string source)
		{
			using (var sha1 = new SHA1Managed())
			{
				var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (var b in hash) sb.Append(b.ToString("x2"));

				return sb.ToString();
			}
		}
	}
}