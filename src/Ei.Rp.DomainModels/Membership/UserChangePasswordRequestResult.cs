using EI.RP.CoreServices.Cqrs.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.Cqrs.Commands;

namespace Ei.Rp.DomainModels.Membership
{
    public class UserChangePasswordRequestResult:IQueryResult
    {
		public string Email { get; set; }
		public string StatusCode { get; set; }

		public string TemporalPassword { get; set; }
    }
}
