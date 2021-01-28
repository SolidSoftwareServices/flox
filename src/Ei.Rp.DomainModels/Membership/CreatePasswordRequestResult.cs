using System;
using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.Membership
{
    public class CreatePasswordRequestResult : IQueryResult
    {
        public string Email { get; set; }
        public string StatusCode { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
