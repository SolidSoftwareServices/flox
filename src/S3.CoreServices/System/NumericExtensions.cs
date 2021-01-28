namespace S3.CoreServices.System
{
	public static class NumericExtensions
	{
		public static long ToLong(this string src)
		{
			return src != null ? long.Parse(src) : default(long);
		}

		public static bool IsBetween(this int src, int bottom,int top, bool inclusive=true)
		{
			return inclusive
				? src >= bottom && src <= top
				: src > bottom && src < top;
		}
	}
}