using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using EI.RP.CoreServices.System;

namespace EI.RP.CoreServices.Http.Clients
{
	public static class HttpHeadersExtensions
	{
		public static THeader AddOrAppendAuthorizationValue<THeader>(this THeader headers, string value)where THeader:HttpHeaders
		{
			return headers.AddIfNotExistsSeparatedByComma("Authorization", value);
		}

		public static THeader AddIfNotExistsSeparatedByComma<THeader>(this THeader headers, string header,string value) where THeader:HttpHeaders
		{
			if (header == null) throw new ArgumentNullException(nameof(header));
			if (value == null) throw new ArgumentNullException(nameof(value));
			string newValue = null;
			if (headers.Contains(header))
			{
				var values = headers.GetValues(header)?.ToArray()??new string[0];
				if( values.Any(x=>x==value)) return headers;
				newValue = string.Join(",", values.Union(value.ToOneItemArray()));
				headers.Remove(header);
			}
			else
			{
				newValue = value;
			}

			headers.TryAddWithoutValidation(header, newValue);
			
			return headers;
		}
		public static THeader AddOrUpdateExisting<THeader>(this THeader headers, string header, string value)where THeader:HttpHeaders
		{
			if (header == null) throw new ArgumentNullException(nameof(header));
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (headers.Contains(header))
			{
				headers.Remove(header);
			}
			

			headers.TryAddWithoutValidation(header, value);

			return headers;
		}
	}
}