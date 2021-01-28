using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.Membership
{
    public class ForgotPasswordRequestResult : IQueryResult
    {
        public bool IsValid { get; set; }
        public string Email { get; set; }
        public string TemporalPassword { get; set; }
		public string StatusCode { get; set; }
	}
}
