using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataServices;

namespace EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo
{
    public class EqualizerPaymentSetupInfoQuery : IQueryModel, IEquatable<EqualizerPaymentSetupInfoQuery>
    {
	    public EqualizerPaymentSetupInfoQuery()
	    {
		    _firstPaymentDateTime = ResolveAdvanceRule(DateTime.Today.AddDays(10).Date);

	    }

	    private DateTime ResolveAdvanceRule(DateTime date)=> date.Day > 28?  date.FirstDayOfNextMonth() :date;
		    


	    public string AccountNumber { get; set; }

	    private DateTime _firstPaymentDateTime;

		public DateTime FirstPaymentDateTime
	    {
		    get => _firstPaymentDateTime;
		    set => _firstPaymentDateTime = ResolveAdvanceRule(value);
	    } 


		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public bool IsValidQuery(out string[] notValidReasons)
        {
	        var result = new List<string>();
	        if (string.IsNullOrWhiteSpace(AccountNumber))
	        {
		        result.Add($"Must specify {nameof(AccountNumber)}");
	        }
	    

	        notValidReasons = result.ToArray();
	        return !result.Any();
        }

		public bool Equals(EqualizerPaymentSetupInfoQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AccountNumber == other.AccountNumber && FirstPaymentDateTime.Equals(other.FirstPaymentDateTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EqualizerPaymentSetupInfoQuery) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AccountNumber != null ? AccountNumber.GetHashCode() : 0) * 397) ^ FirstPaymentDateTime.GetHashCode();
            }
        }

        public static bool operator ==(EqualizerPaymentSetupInfoQuery left, EqualizerPaymentSetupInfoQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EqualizerPaymentSetupInfoQuery left, EqualizerPaymentSetupInfoQuery right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(AccountNumber)}: {AccountNumber}, {nameof(FirstPaymentDateTime)}: {FirstPaymentDateTime}";
        }
    }
}
