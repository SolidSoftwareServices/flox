using System;
using System.Text.RegularExpressions;

namespace S3.CoreServices.System
{
	public sealed class EmailAddress:IEquatable<EmailAddress>
	{
		private readonly string _email;
		
		public EmailAddress(string str)
		{
			_email = str.Trim();
		}

		public void IsValid()
		{
			const string emailPattern = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
									  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
			if (_email == null || _email.Length <= 0 || !Regex.Match(_email.Trim(), emailPattern, RegexOptions.IgnoreCase).Success)
			{
				throw new ArgumentException("Not a valid Email address");
			}
		}

		public static implicit operator string(EmailAddress src)
		{
			return src._email;
		}

		public static implicit operator EmailAddress(string src)
		{
			return new EmailAddress(src);
		}

		public override string ToString()
		{
			return _email;
		}

		public bool Equals(EmailAddress other)
		{
			return string.Equals(_email, other._email, StringComparison.InvariantCultureIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is EmailAddress other && Equals(other);
		}

		public override int GetHashCode()
		{
			return (_email != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_email) : 0);
		}

		public static bool operator ==(EmailAddress left, EmailAddress right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(EmailAddress left, EmailAddress right)
		{
			return !Equals(left, right);
		}
	}
}