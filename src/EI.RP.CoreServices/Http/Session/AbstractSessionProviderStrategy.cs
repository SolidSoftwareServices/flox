using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using NLog;

namespace EI.RP.CoreServices.Http.Session
{
    public abstract class AbstractSessionProviderStrategy : IRealUserSessionProvider
    {
	    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		public abstract void Set<TValue>(string key, TValue value) where TValue : class;
		public abstract TValue Get<TValue>(string key) where TValue : class;
		public abstract void Remove(string key);
		public abstract ClaimsPrincipal CurrentUserClaimsPrincipal { get;}
		public string UserName => IsAnonymous()?null:CurrentUserClaimsPrincipal.Identity.Name;
		public string ActingAsBusinessPartnerId
		{
			get => Get<string>(nameof(ActingAsBusinessPartnerId));
			set => Set(nameof(ActingAsBusinessPartnerId), value);
		}

		public string ActingAsUserName
		{
			get => Get<string>(nameof(ActingAsUserName));
			set => Set(nameof(ActingAsUserName), value?.ToLowerInvariant());
		}

		public abstract bool IsAnonymous();
		public abstract Task CreateSession(params Claim[] claims);
		public abstract Task EndCurrentSession();
		public abstract void DeleteWhereKey(Func<string, bool> predicateFunc);

		public virtual void AsUser(params Claim[] claims)
		{
			Logger.Warn(()=>$"{nameof(AsUser)} not implemented.");
		}

	    public abstract IEnumerable<string> GetAllKeys();
	    public abstract void DisableContext();

	    protected ClaimsPrincipal BuildPrincipal(Claim[] claims)
		{
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

			var principal = new ClaimsPrincipal(identity);
			return principal;
		}

	}
}