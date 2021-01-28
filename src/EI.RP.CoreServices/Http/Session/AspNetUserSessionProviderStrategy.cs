using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace EI.RP.CoreServices.Http.Session
{
	public class AspNetUserSessionProviderStrategy : AbstractSessionProviderStrategy
	{
		private const string NoContextUserKey = "ContextUser";
		private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAppCookieSettings _settings;
        private bool _isContextEnabled=true;

		private readonly ConcurrentDictionary<string,object> _noContextData=new ConcurrentDictionary<string, object>();

        public AspNetUserSessionProviderStrategy(IHttpContextAccessor contextAccessor,IAppCookieSettings settings)
        {
	        _contextAccessor = contextAccessor;
	        _settings = settings;
        }

    
        public override void Set<TValue>(string key, TValue value)
        {
	        if (_isContextEnabled)
	        {
		        _contextAccessor.HttpContext.Session.Set(key, value?.ToByteArray());
	        }
	        else
	        {
		        _noContextData.AddOrUpdate(key, value, (a, b) => value);
	        }
        }

		public override TValue Get<TValue>(string key)
		{
			TValue result=default(TValue);

			if (_isContextEnabled)
	        {
		        if (_contextAccessor.HttpContext.Session.TryGetValue(key, out var value))
		        {
			        result =  value?.ToObject<TValue>();
		        }
	        }
	        else
	        {
				if(_noContextData.TryGetValue(key, out var storedValue))
				{
					result= (TValue)storedValue;
				}
				
	        }

			return result;
		}
		public override void Remove(string key)
		{
			if (_isContextEnabled)
				_contextAccessor.HttpContext.Session.Remove(key);
			else
				_noContextData.RemoveIfExists(key);
		}

		public override ClaimsPrincipal CurrentUserClaimsPrincipal
		{
			get
			{
				if (_isContextEnabled)
					return _contextAccessor?.HttpContext?.User;
				else
					return _noContextData.ContainsKey(NoContextUserKey)
						? (ClaimsPrincipal) _noContextData[NoContextUserKey]
						: null;
			}
		}

		public override bool IsAnonymous()
		{
			return CurrentUserClaimsPrincipal?.Identity == null || !CurrentUserClaimsPrincipal.Identity.IsAuthenticated;
		}

		public override async Task CreateSession(params Claim[] claims)
        {
	        var principal = BuildPrincipal(claims);
			if(_isContextEnabled)
			{
				await _contextAccessor.HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme, principal,
				new AuthenticationProperties
				{
					IsPersistent = true,
					AllowRefresh = true,
					ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
					IssuedUtc = DateTimeOffset.UtcNow
				});
			}
			else
			{
				_noContextData.AddOrUpdate(NoContextUserKey, principal, (a, b) => principal);
			}
        }



		public override async Task EndCurrentSession()
		{
			if (_isContextEnabled)
			{
				var httpContext = _contextAccessor.HttpContext;
				foreach (var cookie in httpContext.Request.Cookies.Where(x => _settings.AppCookieNames.Any(x.Key.StartsWith)))
				{
					httpContext.Response.Cookies.Delete(cookie.Key);
				}
				await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				httpContext.Session.Clear();
			}
			else
			{
				_noContextData.TryRemove(NoContextUserKey, out var nothing);
			}
		}

		public override void DeleteWhereKey(Func<string, bool> predicateFunc)
		{
			var enumerable = GetAllKeys().Where(predicateFunc).ToArray();
			foreach (var key in enumerable)
			{
				_contextAccessor.HttpContext.Session.Remove(key);
			}
		}

		public override IEnumerable<string> GetAllKeys()
		{
			return _contextAccessor.HttpContext.Session.Keys;
		}

		public override void DisableContext()
		{
			_isContextEnabled = false;
		}
	}
}