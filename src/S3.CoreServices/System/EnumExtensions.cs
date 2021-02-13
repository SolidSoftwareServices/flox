using System;

namespace S3.CoreServices.System
{
	public static class EnumExtensions
	{

		public static bool IsOf<TEnum>(this string src) where TEnum : struct
		{
			return Enum.TryParse(src,true,out TEnum nothing);
		}

		public static TEnum ToEnum<TEnum>(this string src,bool ignoreCase=true) where TEnum:struct
		{
			return (TEnum)Enum.Parse(typeof(TEnum), src,ignoreCase);
		}

		
	}
}