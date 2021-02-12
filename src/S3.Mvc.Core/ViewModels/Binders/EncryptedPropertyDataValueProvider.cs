using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using S3.CoreServices.Encryption;

namespace S3.Mvc.Core.ViewModels.Binders
{
	public class EncryptedPropertyDataValueProvider : IValueProvider
	{
		private readonly IValueProvider _innerValueProvider;
		private readonly IEncryptionService _encryptionService;

		public EncryptedPropertyDataValueProvider(IValueProvider innerValueProvider,
			IEncryptionService encryptionService)
		{
			_innerValueProvider = innerValueProvider;
			_encryptionService = encryptionService;
		}

		public bool ContainsPrefix(string prefix)
		{
			return _innerValueProvider.ContainsPrefix(prefix);
		}

		public ValueProviderResult GetValue(string key)
		{
			return GetValueAsync(key).GetAwaiter().GetResult();
		}

		public async Task<ValueProviderResult> GetValueAsync(string key)
		{
			var val = _innerValueProvider.GetValue(key);

			return new ValueProviderResult(await _encryptionService.DecryptAsync(val.FirstValue, true), val.Culture);
		}
	}
}