using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using Ei.Rp.DomainModels.SmartActivation;

namespace EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter
{
	public class ActivateSmartMeterCommand : DomainCommand, IEquatable<ActivateSmartMeterCommand>
    {       
        public ActivateSmartMeterCommand(ElectricityPointReferenceNumber mprn, 
										 string electricityAccountNumber,
										 SmartPlan selectedPlan, 
										 DayOfWeek? selectedPlanFreeDay,
										 bool monthlyBilling,
										 int monthlyBillingSelectedDay,
										 IEnumerable<SetUpDirectDebitDomainCommand> commandsToExecute)
		{
			MPRN = mprn;
			ElectricityAccountNumber = electricityAccountNumber;
			SelectedPlan = selectedPlan;
			SelectedPlanFreeDay = selectedPlanFreeDay;
			MonthlyBilling = monthlyBilling;
			MonthlyBillingSelectedDay = monthlyBillingSelectedDay;
			CommandsToExecute = commandsToExecute ?? new SetUpDirectDebitDomainCommand[0];
		}

		public ElectricityPointReferenceNumber MPRN { get; }
		public string ElectricityAccountNumber { get; }
		public SmartPlan SelectedPlan { get; }
		public DayOfWeek? SelectedPlanFreeDay { get; }
		public bool MonthlyBilling { get; }
		public int MonthlyBillingSelectedDay { get; }
		public IEnumerable<SetUpDirectDebitDomainCommand> CommandsToExecute { get; }

		public bool Equals(ActivateSmartMeterCommand other)
        {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
			return MPRN == other.MPRN && ElectricityAccountNumber == other.ElectricityAccountNumber &&
				SelectedPlan.Equals(other.SelectedPlan) && SelectedPlanFreeDay == other.SelectedPlanFreeDay &&
				MonthlyBilling == other.MonthlyBilling &&
				MonthlyBillingSelectedDay == other.MonthlyBillingSelectedDay &&
				CommandsToExecute.SequenceEqual(other.CommandsToExecute);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActivateSmartMeterCommand) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (MPRN != null ? MPRN.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ElectricityAccountNumber != null ? ElectricityAccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SelectedPlan != null ? SelectedPlan.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SelectedPlanFreeDay != null ? SelectedPlanFreeDay.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ MonthlyBilling.GetHashCode();
				hashCode = (hashCode * 397) ^ MonthlyBillingSelectedDay.GetHashCode();
				hashCode = (hashCode * 397) ^ (CommandsToExecute != null ? CommandsToExecute.GetHashCode() : 0);
				return hashCode;
            }
        }

        public static bool operator ==(ActivateSmartMeterCommand left, ActivateSmartMeterCommand right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ActivateSmartMeterCommand left, ActivateSmartMeterCommand right)
        {
            return !Equals(left, right);
        }        
    }
}