using System;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Cqrs.Queries
{
	public enum QueryCacheResultsType
	{
		NoCache,
		UserSpecific,
		AllUsersShared
	}

	

	
    public interface IQueryModel
    {
	    QueryCacheResultsType CacheResults { get; }
	    bool IsValidQuery(out string[] notValidReasons);
    }
}