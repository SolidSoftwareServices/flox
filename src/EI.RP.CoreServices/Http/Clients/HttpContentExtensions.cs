using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Http.Clients
{
	public static class HttpContentExtensions
	{
		public static async Task<string> ReadContentAsTextTransparentlyAsync(this HttpContent source)
		{
			using (var stream = new MemoryStream())
			{
				await source.CopyToAsync(stream);
				using (var reader = new StreamReader(stream))
				{
					return await reader.ReadToEndAsync();
				}
			}
		}

	}
}