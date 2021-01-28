using System;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.MoveHouse
{
    public enum OutputType
    {
        NotExecuted = 0,
        Passed,
        Failed
    }

    public class MovingHouseRulesValidationResult : IQueryResult, IEquatable<MovingHouseRulesValidationResult>
	{
		public OutputType Output { get; set; }
		public MovingHouseValidationType MovingHouseValidationType { get; set; }

		public bool Equals(MovingHouseRulesValidationResult other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Output == other.Output && Equals(MovingHouseValidationType, other.MovingHouseValidationType);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MovingHouseRulesValidationResult) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Output.GetHashCode() * 397) ^ (MovingHouseValidationType != null ? MovingHouseValidationType.GetHashCode() : 0);
			}
		}

		public static bool operator ==(MovingHouseRulesValidationResult left, MovingHouseRulesValidationResult right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MovingHouseRulesValidationResult left, MovingHouseRulesValidationResult right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(MovingHouseValidationType)}: {MovingHouseValidationType}, {nameof(Output)}: {Output}";
		}
	}
}