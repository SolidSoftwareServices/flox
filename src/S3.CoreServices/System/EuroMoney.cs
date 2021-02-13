using System;
using Newtonsoft.Json;

namespace S3.CoreServices.System
{
	public class EuroMoney
	{
		public EuroMoney(decimal amount)
		{
			Amount = amount;
		}

		public EuroMoney(string amount)
		{
			Amount = decimal.Parse(amount);
		}

		[JsonConstructor]
		private EuroMoney()
		{
		}

		public decimal? Amount { get; set; } = null;

		public static EuroMoney Zero => new EuroMoney(0m);

		public decimal? ToDecimal()
		{
			return Amount;
		}

		public decimal? ToDecimal(decimal tolerance = 0M)
		{
			if (tolerance < 0M)
			{
				throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance should be greater or equal to zero");
			}

			return Amount.GetValueOrDefault(0) < tolerance
				? null
				: Amount;
		}

		public override string ToString()
        {
            ThrowIfNotSet();
            return $"€{Amount.Value.ToString($"N")}";
        }
		public string ToString(string format = "N")
        {
            ThrowIfNotSet();
            return $"€{Amount.Value.ToString(format)}";
        }
		public string ToStringCents(string format = "N")
		{
			ThrowIfNotSet();
			return $"{(Amount.Value*100).ToString(format)}c";
		}
        private void ThrowIfNotSet()
        {
            if (!Amount.HasValue)
            {
                throw new ArgumentNullException(nameof(Amount), "Not a valid value");
            }
        }

        protected bool Equals(EuroMoney other)
		{
			return Amount == other.Amount;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((EuroMoney) obj);
		}

		public override int GetHashCode()
		{

			return Amount?.GetHashCode()??0;
		}

		public static bool operator ==(EuroMoney left, EuroMoney right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(EuroMoney left, EuroMoney right)
		{
			return !Equals(left, right);
		}

		public static implicit operator EuroMoney(string src)
		{
			return decimal.Parse(src.Replace("€", string.Empty).Trim());
		}
		public static implicit operator EuroMoney(decimal src)
		{
			return new EuroMoney(src);
		}
		public static implicit operator decimal(EuroMoney src)
		{
            src.ThrowIfNotSet();
            return src.Amount.Value;
		}

		public static implicit operator EuroMoney(double src)
		{
			return new EuroMoney((decimal)src);
		}
		public static implicit operator EuroMoney(float src)
		{
			return new EuroMoney((decimal)src);
		}


        public static EuroMoney operator -(decimal a, EuroMoney b)
        {
            return a-b.Amount;
        }
        public static EuroMoney operator -(EuroMoney b)
        {
            return - b.Amount;
        }

        public static EuroMoney operator +(decimal a, EuroMoney b)
        {
            return a + b.Amount;
        }

        public static EuroMoney operator *(decimal a, EuroMoney b)
        {
            return a * b.Amount;
        }

        public static EuroMoney operator /(decimal a, EuroMoney b)
        {
            return a / b.Amount;
        }

    }
}