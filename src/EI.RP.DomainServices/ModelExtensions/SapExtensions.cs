using System;
using System.Linq;

namespace EI.RP.DomainServices.ModelExtensions
{

	public static class SapExtensions
	{
		/// <summary>
		///    
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string AdaptToSapPasswordFormat(this string password)
		{
			if (password.StartsWith("!") || password.StartsWith("?")) password = "X" + password;

			if (password.Length >= 3)
			{
				var hasConsecutiveChar = HasConsecutiveChars(password, 3);
				if (hasConsecutiveChar)
				{
					if (password.StartsWith("X"))
						password = "x" + password;
					else
						password = "X" + password;
				}
			}

			if (password.Length > 40)
				password = password.Substring(0, 40);
			return password;
		}

        private static readonly string[] NiPortalUsers = new string[]
        {
            "ASHMURFY@GMAIL.COM", "B.BRANNIGAN@BTINTERNET.COM", "BGORMLEY1@HOTMAIL.CO.UK", "BLACKEROYD@AOL.COM",
            "BMCFARLINE@BTINTERNET.COM", "BRENDANGALLEN@GMAIL.COM", "BRIAN@BMALLON.NET", "CEEMULLIGAN@YAHOO.CO.UK",
            "CELINELAVERY@HOTMAIL.CO.UK", "CHRIS.SINCLAIR2@BTINTERNET.COM", "CRANNY4@HOTMAIL.CO.UK",
            "DOUGLASKING100@HOTMAIL.COM", "EAMON.BRADLEY@HOTMAIL.CO.UK", "GARETHMCATEER@BTINTERNET.COM",
            "IAN.BEANEY@IDNET.COM", "JIM.CUSH@BT.COM", "JOHN.CORR@WDR-RT-TAGGART.COM", "JOHNBELL1850@HOTMAIL.COM",
            "LIAMMCKINNEY@AOL.COM", "LIZDORAN04@GMAIL.COM", "MAEVEMCLAUGHLIN80@GOOGLEMAIL.COM", "MARKINSON@HSCNI.NET",
            "MSBMBE@GOOGLEMAIL.COM", "NINAKELLY33@HOTMAIL.COM", "OLLIE15.TM@OUTLOOK.COM", "PASCAL.MCCRUMLISH@GMAIL.COM",
            "PK@CONTRACTSERVICESNI.COM", "SAOIRSE2@HOTMAIL.CO.UK", "SHIRLEY.FERGUSON@HOTMAIL.COM", "WNOCKWNOCK@AOL.COM"
        };

        public static string AdaptToSapUserNameFormat(this string userName)
        {
            if (userName.Trim() == String.Empty || userName == null)
                throw new ArgumentException("Not a valid User Name");

            if (NiPortalUsers.Any(x => x.Contains(userName.Trim().ToUpper())))
            {
                userName = userName.Trim() + "_ROI";
            }

            if (userName.Length > 40)
			{
				userName = userName.Substring(0, 40);
			}
			return userName.TrimEnd('.', ' ');
		}
		
		private static bool HasConsecutiveChars(string source, int sequenceLength)
		{
			if (string.IsNullOrEmpty(source) || source.Length == 1) return false;

			var charCount = 1;

			for (var i = 0; i < 2; i++)
			{
				var c = source[i];
				if (c == source[i + 1])
				{
					charCount++;

					if (charCount == sequenceLength) return true;
				}
				else
				{
					charCount = 1;
				}
			}

			return false;
		}
	}
}