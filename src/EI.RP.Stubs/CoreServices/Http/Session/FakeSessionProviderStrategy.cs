using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Sap;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EI.RP.Stubs.CoreServices.Http.Session
{
	public class FakeSessionProviderStrategy: AbstractSessionProviderStrategy,ISapSession
	{
		private readonly IDictionary<string, object> _store=new Dictionary<string, object>();
		private ClaimsPrincipal _currentUserClaimsPrincipal;

		public override void Set<TValue>(string key, TValue value)
		{
			if (_store.ContainsKey(key))
			{
				_store[key] = value;
			}
			else
			{
				_store.Add(key, value);
			}
		}

		public override TValue Get<TValue>(string key)
		{
			if (!_store.ContainsKey(key))
			{
				return default(TValue);
			}
			return (TValue)_store[key];
		}

		public override void Remove(string key)
		{
			_store.Remove(key);
		}

		public override ClaimsPrincipal CurrentUserClaimsPrincipal
		{
			get => _currentUserClaimsPrincipal;
		}

		public override bool IsAnonymous()
		{
			return CurrentUserClaimsPrincipal?.Identity == null || !CurrentUserClaimsPrincipal.Identity.IsAuthenticated;
		}

		public override async Task CreateSession(params Claim[] claims)
		{
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);
			_currentUserClaimsPrincipal = principal;
		}

		public override async Task EndCurrentSession()
		{
			_currentUserClaimsPrincipal = null;
		}
		public void Reset()
		{
			_sapCsrf = Guid.NewGuid().ToString();
			EndCurrentSession().Wait();
			_store.Clear();
		}
		public override void DeleteWhereKey(Func<string, bool> predicateFunc)
		{
			var enumerable = _store.Keys.Where(predicateFunc).ToArray();
			foreach (var key in enumerable)
			{
				_store.Remove(key);
			}
		}

		public override IEnumerable<string> GetAllKeys()
		{
			return _store.Keys.ToArray();
		}

		public override void DisableContext()
		{
			
		}

		private string _sapCsrf = Guid.NewGuid().ToString();
		public string SapCsrf
		{
			get =>!IsAnonymous() ? _sapCsrf : throw new InvalidOperationException();
			set => _sapCsrf = value;
		}

		public string SapJsonCookie { get; set; } = "{}";

	
	}
}
