using System;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.Contracts.ChangeSmartPlanToStandard
{
	public class ChangeSmartPlanToStandardCommand : DomainCommand, IEquatable<ChangeSmartPlanToStandardCommand>
    {       
        public ChangeSmartPlanToStandardCommand(string electricityAccountNumber)
		{
			ElectricityAccountNumber = electricityAccountNumber;
        }

		public string ElectricityAccountNumber { get; }

		public bool Equals(ChangeSmartPlanToStandardCommand other)
        {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return ElectricityAccountNumber == other.ElectricityAccountNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChangeSmartPlanToStandardCommand) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = !string.IsNullOrEmpty(ElectricityAccountNumber) ? ElectricityAccountNumber.GetHashCode() : 0;
				return hashCode;
            }
        }

        public static bool operator ==(ChangeSmartPlanToStandardCommand left, ChangeSmartPlanToStandardCommand right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChangeSmartPlanToStandardCommand left, ChangeSmartPlanToStandardCommand right)
        {
            return !Equals(left, right);
        }        
    }
}