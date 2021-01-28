using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;

namespace EI.RP.WebApp.Infrastructure.Caching.PreLoad
{
	internal class SessionProviderForCachePreloader : IActingAsUserSessionProvider
	{
		private string _actingAsUserName;

		public void Set<TValue>(string key, TValue value) where TValue : class
		{
			throw new NotImplementedException();
		}

		public TValue Get<TValue>(string key) where TValue : class
		{
			var claim = CurrentUserClaimsPrincipal.Claims.SingleOrDefault(x =>
				x.Type.Equals(key, StringComparison.InvariantCultureIgnoreCase));
			// ReSharper disable once PossibleNullReferenceException
			return claim.Value as TValue;
		}

		public void Remove(string key)
		{
			throw new NotImplementedException();
		}

		public ClaimsPrincipal CurrentUserClaimsPrincipal { get; private set; }

		public string UserName => CurrentUserClaimsPrincipal.Identity.Name;
		public string ActingAsBusinessPartnerId { get; set; }

		public string ActingAsUserName
		{
			get => _actingAsUserName;
			set => _actingAsUserName = value?.ToLowerInvariant();
		}

		public bool IsAnonymous()
		{
			return CurrentUserClaimsPrincipal?.Identity == null|| !CurrentUserClaimsPrincipal.Identity.IsAuthenticated;
		}

		public Task CreateSession(params Claim[] claims)
		{
			throw new NotImplementedException();
		}

		public Task EndCurrentSession()
		{
			throw new NotImplementedException();
		}

		public void DeleteWhereKey(Func<string, bool> predicateFunc)
		{
			throw new NotImplementedException();
		}

		public void AsUser(params Claim[] claims)
		{
			CurrentUserClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
			ActingAsUserName = CurrentUserClaimsPrincipal.Identity.Name;
		}

		public IEnumerable<string> GetAllKeys()
		{
			throw new NotImplementedException();
		}

		public void DisableContext()
		{
			
		}
	}
}