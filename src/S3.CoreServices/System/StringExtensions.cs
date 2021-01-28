using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace S3.CoreServices.System
{
	public static class StringExtensions
	{
		/// <summary>
		/// gets the last length characters of an string
		/// </summary>
		/// <param name="source"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string GetEndSubstring(this string source,int length)
		{
			return source?.Substring(Math.Max(0, source.Length - length));
		}
		public static string ToBase64String(this string source)
		{
			return source == null ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
		}

		public static string ToBase64UrlEncoded(this string s)
		{
			if (s == null) return null;

			var bytes = Encoding.UTF8.GetBytes(s);

			s = Convert.ToBase64String(bytes);
			s = s.Split('=')[0]; // Remove any trailing '='s
			s = s.Replace('+', '-'); // 62nd char of encoding
			s = s.Replace('/', '_'); // 63rd char of encoding

			return s;
		}

		public static string FromBase64UrlEncoded(this string source)
		{
			if (source == null) return null;

			source = source.Replace('-', '+'); // 62nd char of encoding 
			source = source.Replace('_', '/'); // 63rd char of encoding

			switch (source.Length % 4) // Pad with trailing '='source
			{
				case 0: break; // No pad chars in this case
				case 2:
					source += "==";
					break; // Two pad chars
				case 3:
					source += "=";
					break; // One pad char
				default: throw new Exception("Illegal base64url string!");
			}

			var bytes = Convert.FromBase64String(source);
			return Encoding.UTF8.GetString(bytes);
		}

		public static string Mask(this string source, char maskChar, int numCharsToMask)
		{
			return source.Substring(numCharsToMask).PadLeft(source.Length, maskChar);
		}


		public static string PadRightExact(this string src, int exactWidth, char paddingChar = ' ')
		{
			return src.PadRight(exactWidth, paddingChar).Substring(0, exactWidth);
		}

		public static string PadLeftExact(this string src, int exactWidth, char paddingChar = ' ')
		{
			return src.PadLeft(exactWidth, paddingChar).Substring(0, exactWidth);
		}

		/// <summary>
		/// https://stackoverflow.com/questions/17326185/what-are-the-different-kinds-of-cases
		/// </summary>
		public static string ToPascalCase(this string src)
		{
			var result = new StringBuilder();
			var nonWordChars = new Regex(@"[^a-zA-Z0-9]+");
			var tokens = nonWordChars.Split(src);
			foreach (var token in tokens)
			{
				result.Append(PascalCaseSingleWord(token));
			}

			return result.ToString();
			string PascalCaseSingleWord(string word)
			{
				var match = Regex.Match(word, @"^(?<word>\d+|^[a-z]+|[A-Z]+|[A-Z][a-z]+|\d[a-z]+)+$");
				var groups = match.Groups["word"];

				var textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
				var sb = new StringBuilder();
				foreach (var capture in groups.Captures.Cast<Capture>())
				{
					sb.Append(textInfo.ToTitleCase(capture.Value.ToLower()));
				}
				return sb.ToString();
			}
		}
		/// <summary>
		/// https://stackoverflow.com/questions/17326185/what-are-the-different-kinds-of-cases
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToKebabCase(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return Regex.Replace(
					value,
					"(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
					"-$1",
					RegexOptions.Compiled)
				.Trim()
				.ToLower();
		}

		public static string Truncate(this string str, int maxLength)
		{
			if (string.IsNullOrEmpty(str))
				return str;
			return str.Substring(0, Math.Min(str.Length, maxLength));
		}
	}

}