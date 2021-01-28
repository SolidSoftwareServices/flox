using System;

namespace EI.RP.CoreServices.Caching.Models
{
	internal class CachedItem : IEquatable<CachedItem>
	{
		private static readonly Guid GeneratorInstanceId = Guid.NewGuid();

		public virtual Guid GeneratedByInstanceId { get; set; } = GeneratorInstanceId;
		public DateTime CreatedTimeUtc { get; set; } = DateTime.UtcNow;
		public object Item { get; set; }

		public string ItemType { get; set; }

		public TValue ItemAs<TValue>()
		{
			return (TValue) Item;
		}

		public bool Equals(CachedItem other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return GeneratedByInstanceId.Equals(other.GeneratedByInstanceId) && CreatedTimeUtc.Equals(other.CreatedTimeUtc) && Equals(Item, other.Item);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CachedItem) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = GeneratedByInstanceId.GetHashCode();
				hashCode = (hashCode * 397) ^ CreatedTimeUtc.GetHashCode();
				hashCode = (hashCode * 397) ^ (Item != null ? Item.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(CachedItem left, CachedItem right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CachedItem left, CachedItem right)
		{
			return !Equals(left, right);
		}
	}

	
}