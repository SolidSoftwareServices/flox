using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Http.Session
{
	public interface IUserSessionProvider
    {
        void Set<TValue>(string key, TValue value) where TValue : class;
        TValue Get<TValue>(string key) where TValue : class;
        void Remove(string key);
		/// <summary>
		/// Retrieve the current user claims
		/// </summary>
		ClaimsPrincipal CurrentUserClaimsPrincipal { get; }

        string UserName { get; }
        string ActingAsBusinessPartnerId { get; set; }
        string ActingAsUserName { get; set; }

        bool IsAnonymous();
        Task CreateSession(params Claim[] claims);
        Task EndCurrentSession();
        void DeleteWhereKey(Func<string, bool> predicateFunc);


        void AsUser(params Claim[] claims);
	    IEnumerable<string> GetAllKeys();
	    void DisableContext();

	  
	}
}
