using System;

namespace Ei.Rp.DomainModels.Contracts.Accounts
{
	public class AccountOverview :AccountBase, IEquatable<AccountOverview>
	{
		private AccountOverview(AccountBase source):base(source)
		{
			
		}
		public AccountOverview()
		{
		}


		public bool Equals(AccountOverview other)
		{
			return base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((AccountOverview) obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(AccountOverview left, AccountOverview right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(AccountOverview left, AccountOverview right)
		{
			return !Equals(left, right);
		}

		public static implicit operator AccountOverview(AccountInfo src)
		{
			return new AccountOverview(src);
		}
	}
}