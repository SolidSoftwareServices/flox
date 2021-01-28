﻿using EI.RP.DataServices.SAP.Clients.Infrastructure.Session;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Infrastructure
{
	class SessionDataHolder : ISapSessionDataRepository
	{
		public static readonly SessionDataHolder Instance = new SessionDataHolder();
		public SessionDataHolder() { }
		public string SapCsrf { get; set; }
		public string SapJsonCookie { get; set; }
	}
}