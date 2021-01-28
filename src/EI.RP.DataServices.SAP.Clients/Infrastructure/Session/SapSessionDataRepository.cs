using System.Linq;
using System.Security.Claims;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Sap;
using EI.RP.CoreServices.System;
using Microsoft.AspNetCore.Http;

namespace EI.RP.DataServices.SAP.Clients.Infrastructure.Session
{
    class SapSessionDataRepository : ISapSessionDataRepository,ISapSession
    {
        private readonly IUserSessionProvider _userSessionProvider;
        

        public SapSessionDataRepository(IUserSessionProvider userSessionProvider)
        {
            _userSessionProvider = userSessionProvider;
        }

        public string SapCsrf
        {
	        get
	        {
		        return _userSessionProvider.Get<string>("RPSapCsrf") ?? ((ClaimsIdentity)_userSessionProvider.CurrentUserClaimsPrincipal.Identity).FindFirst(x => x.Type == "RPSapCsrf")?.Value;
	        }
	        set => _userSessionProvider.Set("RPSapCsrf", value);
        }

        public string SapJsonCookie
        {
            get => _userSessionProvider.Get<string>("RPSapCookie") ?? ((ClaimsIdentity)_userSessionProvider.CurrentUserClaimsPrincipal.Identity).FindFirst(x => x.Type == "RPSapCookie")?.Value;
            set => _userSessionProvider.Set("RPSapCookie", value);
        }
   
     
    }
}