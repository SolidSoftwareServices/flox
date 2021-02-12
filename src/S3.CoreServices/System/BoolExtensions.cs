using System;

namespace S3.CoreServices.System
{
	public static class BoolExtensions
	{
		public static int ToInt(this bool src)
		{
			return src ? 1 : 0;
		}

		public static string ToIntString(this bool src)
		{
			return (src ? 1 : 0).ToString();
		}

		public static string ToYesNoString(this bool src)
		{
			return src ? "Yes" : "No";
		}

		public static bool ToBooleanFromYesNoString(this string src)
		{
			const string yes = "Yes";
			const string no = "No";

			if (src.Equals(yes, StringComparison.InvariantCultureIgnoreCase)) return true;
			if (src.Equals(no, StringComparison.InvariantCultureIgnoreCase)) return false;
			throw new ArgumentOutOfRangeException($"Only ['{yes}','{no}'] supported");
		}
	}
}