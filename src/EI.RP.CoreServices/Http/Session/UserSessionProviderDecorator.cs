using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EI.RP.CoreServices.System.Identity;

namespace EI.RP.CoreServices.Http.Session
{
	class UserSessionProviderDecorator : IUserSessionProvider
	{
		private readonly IRealUserSessionProvider _defaultSessionProvider;
		private readonly IActingAsUserSessionProvider _actingAsUserSessionProvider;
		private IUserSessionProvider _current;

		public UserSessionProviderDecorator(IRealUserSessionProvider defaultSessionProvider):this(defaultSessionProvider,null)
		{

		}

		public UserSessionProviderDecorator(IRealUserSessionProvider defaultSessionProvider,IActingAsUserSessionProvider actingAsUserSessionProvider)
		{
			_defaultSessionProvider = defaultSessionProvider;
			_actingAsUserSessionProvider = actingAsUserSessionProvider;
			_current = _defaultSessionProvider;
		}
		public void AsUser(params Claim[]claims)
		{
			if (_actingAsUserSessionProvider == null)
			{
				throw new InvalidOperationException($"Cannot act as another user without registering a service for {nameof(IActingAsUserSessionProvider)}");
			}

			_actingAsUserSessionProvider.AsUser(claims);
			_current = _actingAsUserSessionProvider;
		}

		public IEnumerable<string> GetAllKeys()
		{
			return _current.GetAllKeys();
		}

		public void DisableContext()
		{
			_defaultSessionProvider.DisableContext();
			_actingAsUserSessionProvider.DisableContext();
		}

		


		public void Set<TValue>(string key, TValue value) where TValue : class
		{
			_current.Set(key,value);
		}

		public TValue Get<TValue>(string key) where TValue : class
		{
			return _current.Get<TValue>(key);
		}

		public void Remove(string key)
		{
			_current.Remove(key);
		}

		public ClaimsPrincipal CurrentUserClaimsPrincipal => _current.CurrentUserClaimsPrincipal;
		public string UserName => _current.UserName;

		public string ActingAsBusinessPartnerId
		{
			get => _current.ActingAsBusinessPartnerId;
			set => _current.ActingAsBusinessPartnerId = value;
		}

		public string ActingAsUserName
		{
			get => _current.ActingAsUserName;
			set => _current.ActingAsUserName = value;
		}

		public bool IsAnonymous()
		{
			return _current.IsAnonymous();
		}

		public Task CreateSession(params Claim[] claims)
		{
			return _current.CreateSession(claims);
		}

		public Task EndCurrentSession()
		{
			return _current.EndCurrentSession();
		}

		public void DeleteWhereKey(Func<string, bool> predicateFunc)
		{
			_current.DeleteWhereKey(predicateFunc);
		}

		
	}
}