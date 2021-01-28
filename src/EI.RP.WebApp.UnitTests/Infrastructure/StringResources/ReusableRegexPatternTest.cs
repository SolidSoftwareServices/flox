using System.Collections;
using System.Text.RegularExpressions;
using EI.RP.WebApp.Infrastructure.StringResources;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace EI.RP.WebApp.UnitTests.Infrastructure.StringResources
{
    [TestFixture]
    public class ReusableRegexPatternTest
    {
        [TestCaseSource(typeof(PhoneNumberCases), "TestCases")]
        public bool PhoneNumberTest(string phoneNumber)
        {
            var regex = new Regex(ReusableRegexPattern.ValidPhoneNumber);
            return regex.IsMatch(phoneNumber);
        }

        [TestCaseSource(typeof(PasswordCases), "TestCases")]
        public bool PasswordTest(string password)
        {
	        var regex = new Regex(ReusableRegexPattern.RegexPassword);
	        return regex.IsMatch(password);
        }

		[TestCaseSource(typeof(AccountCommentsCases), "TestCases")]
		public bool AccountComments(string comments)
		{
			var regex = new Regex(ReusableRegexPattern.ValidAccountQuery);
			return regex.IsMatch(comments);
		}
	}

    public class PhoneNumberCases
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData("08603662222").Returns(true);
                yield return new TestCaseData("086 0366 2222").Returns(true);
                yield return new TestCaseData("086.0366.2222").Returns(true);
                yield return new TestCaseData("00353891234567").Returns(true);
                yield return new TestCaseData("+353891234567").Returns(true);
                yield return new TestCaseData("+44 7700 900667").Returns(true);
                yield return new TestCaseData("00447700900667").Returns(true);
                yield return new TestCaseData("0044 121 496 0031").Returns(true);
                yield return new TestCaseData("00441214960031").Returns(true);
                yield return new TestCaseData("0044.121.496.0031").Returns(true);
                yield return new TestCaseData("+441214960031").Returns(true);

                yield return new TestCaseData("").Returns(false);
                yield return new TestCaseData(" ").Returns(false);
                yield return new TestCaseData("0").Returns(false);
                yield return new TestCaseData("0   1").Returns(false);
                yield return new TestCaseData("0123").Returns(false);
                yield return new TestCaseData("0123 ").Returns(false);
                yield return new TestCaseData("+1234").Returns(false);
                yield return new TestCaseData(" +1234").Returns(false);
                yield return new TestCaseData("++353").Returns(false);
                yield return new TestCaseData("01a23").Returns(false);
                yield return new TestCaseData("a0123").Returns(false);
                yield return new TestCaseData("a0123a").Returns(false);
                yield return new TestCaseData("123 456").Returns(false);
                yield return new TestCaseData("01234").Returns(false);
                yield return new TestCaseData("12345").Returns(false);
                yield return new TestCaseData("+12345").Returns(false);
                yield return new TestCaseData("00353").Returns(false);
                yield return new TestCaseData("++3531234").Returns(false);
                yield return new TestCaseData("0044121a4960031").Returns(false);
                yield return new TestCaseData("0044.121+496.0031").Returns(false);
                yield return new TestCaseData("+44121a4960031").Returns(false);
                yield return new TestCaseData("0123456789012345678901234567890123456789").Returns(false);
                yield return new TestCaseData("01234567890123456789012345678901234567890").Returns(false);
            }
        }
    }

	public class PasswordCases
	{
		public static IEnumerable TestCases
		{
			get
			{
				yield return new TestCaseData("Test12345").Returns(true);

				yield return new TestCaseData("").Returns(false);
				yield return new TestCaseData(" ").Returns(false);
				yield return new TestCaseData("1234").Returns(false);
				yield return new TestCaseData("abcdefg").Returns(false);
				yield return new TestCaseData("Test12").Returns(false);
			}
		}
	}

	public class AccountCommentsCases
	{
		public static IEnumerable TestCases
		{
			get
			{
				yield return new TestCaseData("Test Comment").Returns(true);
				yield return new TestCaseData("Test CommentTest CommentTest CommentTest CommentTest CommentTest CommentTest CommentTest CommentTest CommentTest Comment").Returns(true);
				yield return new TestCaseData(" ").Returns(true);
				yield return new TestCaseData("1235435276327647424751212537371263123619").Returns(true);

				yield return new TestCaseData("").Returns(false);
				yield return new TestCaseData("Test123<ExampleTestComment").Returns(false);
				yield return new TestCaseData("Aqwercyvb>890076555766866866878").Returns(false);
				yield return new TestCaseData("Testing||Testing||Testing").Returns(false);

			}
		}
	}
}
