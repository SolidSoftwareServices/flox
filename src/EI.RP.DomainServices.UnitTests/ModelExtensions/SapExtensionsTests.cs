using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using EI.RP.DomainServices.ModelExtensions;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.ModelExtensions
{
	[TestFixture]
	class SapExtensionsTests
	{
		[TestCase("","")]
		[TestCase("!", "X!")]
		[TestCase("?", "X?")]
		[TestCase("XxX", "XxX")]
		[TestCase("XXX", "xXXX")]
		[TestCase("YYY", "XYYY")]
		[TestCase("!XXX", "X!XXX")]
		[TestCase("?XXX", "X?XXX")]
		[TestCase("!dxF6tUt81dprZRDdI066chju0ybP6oaYcc3dMTk", "X!dxF6tUt81dprZRDdI066chju0ybP6oaYcc3dMT")]
		[TestCase("?dxF6tUt81dprZRDdI066chju0ybP6oaYcc3dMTk", "X?dxF6tUt81dprZRDdI066chju0ybP6oaYcc3dMT")]
		[TestCase("JdxF6tUt81dprZRDdI066chju0ybP6oaYcc3dMTksdafasdfasdfasdfasdfasdfafda", "JdxF6tUt81dprZRDdI066chju0ybP6oaYcc3dMTk")]
		[TestCase("!JdxF6tUt81dprZRDdI066chju0ybP6oaYcc3dMTksdafasdfasdfasdfasdfasdfafda", "X!JdxF6tUt81dprZRDdI066chju0ybP6oaYcc3dM")]
		[TestCase("?JdxF6tUt81dprZRDdI066chju0ybP6oaYcc3dMTksdafasdfasdfasdfasdfasdfafda", "X?JdxF6tUt81dprZRDdI066chju0ybP6oaYcc3dM")]
		public void CanAdaptPasswordToSapFormat(string userPwd, string sapPwd)
		{
			var actual = userPwd.AdaptToSapPasswordFormat();

			Assert.AreEqual(sapPwd,actual);
		}
		[TestCase("SIGNUPTESTABCDE12341ABCDE12345ABCDE1234.......1@esb.ie", "SIGNUPTESTABCDE12341ABCDE12345ABCDE1234")]
		[TestCase("SIGNUPTESTABCDE12341ABCDE12345ABCDE1234.1@esb.ie", "SIGNUPTESTABCDE12341ABCDE12345ABCDE1234")]
		[TestCase("ABCDE1234.1@esb.ie", "ABCDE1234.1@esb.ie")]
		public void CanAdaptUserNameToSapFormat(string username, string expected)
		{
			var actual = username.AdaptToSapUserNameFormat();

			Assert.AreEqual(expected, actual);
		}

        public static IEnumerable<TestCaseData> CanAdaptUserNameForLegacyNIUsers_Cases()
        {
            var  niPortalUsers = new[]
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

            foreach (var niPortalUser in niPortalUsers)
            {
                yield return BuildCaseFor(niPortalUser.ToUpper());
                yield return BuildCaseFor(niPortalUser.ToLower());
                yield return BuildCaseFor("      "+niPortalUser.ToUpper() + "      ");
                yield return BuildCaseFor("      " + niPortalUser.ToLower() + "      ");
            }

            yield return new TestCaseData(string.Empty).SetName($"{nameof(CanAdaptUserNameForLegacyNIUsers)}-(empty)").Returns(null);
            yield return new TestCaseData("  ").SetName($"{nameof(CanAdaptUserNameForLegacyNIUsers)}-(whitespaces)").Returns(null);
            yield return new TestCaseData(null).SetName($"{nameof(CanAdaptUserNameForLegacyNIUsers)}-(null)").Returns(null);
            TestCaseData BuildCaseFor(string portalUser)
            {
                return new TestCaseData(portalUser).SetName($"{nameof(CanAdaptUserNameForLegacyNIUsers)}-{portalUser}").Returns($"{portalUser.Trim()}_ROI");
            }
        }
       [TestCaseSource(nameof(CanAdaptUserNameForLegacyNIUsers_Cases))]
        public string CanAdaptUserNameForLegacyNIUsers(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                Assert.Throws<ArgumentException>(() =>
                    (userName ?? string.Empty).AdaptToSapUserNameFormat());
               return null;
            }

            return userName.AdaptToSapUserNameFormat();

        }
    }
}
