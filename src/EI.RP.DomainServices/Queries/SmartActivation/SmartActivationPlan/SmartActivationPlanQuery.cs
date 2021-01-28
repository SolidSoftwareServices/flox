using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan
{
	public class SmartActivationPlanQuery : IQueryModel, IEquatable<SmartActivationPlanQuery>
    {
	    public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;

	    public string AccountNumber { get; set; }

	    public bool OnlyActive { get; set; }

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

	    public bool Equals(SmartActivationPlanQuery other)
	    {
		    if (ReferenceEquals(null, other)) return false;
		    if (ReferenceEquals(this, other)) return true;
		    return CacheResults == other.CacheResults;
	    }

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((SmartActivationPlanQuery) obj);
	    }

	    public override int GetHashCode()
	    {
		    return (int) CacheResults;
	    }

	    public static bool operator ==(SmartActivationPlanQuery left, SmartActivationPlanQuery right)
	    {
		    return Equals(left, right);
	    }

	    public static bool operator !=(SmartActivationPlanQuery left, SmartActivationPlanQuery right)
	    {
		    return !Equals(left, right);
	    }

	    public override string ToString()
	    {
		    return $"{nameof(CacheResults)}: {CacheResults}, {nameof(AccountNumber)}: {AccountNumber}, {nameof(OnlyActive)}: {OnlyActive}";
	    }
    }
}