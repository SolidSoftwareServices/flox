using System;

namespace S3.UiFlows.Core.Configuration
{
	public class ScreenTransition : IEquatable<ScreenTransition>
	{
		public ScreenTransition(string eventName, string destinationScreen, string conditionDescription = null)
		{
			EventName = eventName;
			DestinationScreen = destinationScreen;
			DisplayName = conditionDescription == null ? eventName : $"{eventName} <{conditionDescription}>";
		}

		public string DestinationScreen { get; }
		public string EventName { get; }

		public string DisplayName { get; }

		public bool Equals(ScreenTransition other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(DestinationScreen, other.DestinationScreen) &&
			       string.Equals(EventName, other.EventName) && string.Equals(DisplayName, other.DisplayName);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ScreenTransition) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = DestinationScreen != null ? DestinationScreen.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (EventName != null ? EventName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DisplayName != null ? DisplayName.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			return $"{DisplayName} ==> {DestinationScreen}";
		}
	}
}