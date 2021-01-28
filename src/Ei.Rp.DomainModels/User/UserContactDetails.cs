using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.User
{
    public class UserContactDetails: IQueryResult
    {
        public string AccountNumber { get; set; }
        public string ContactEMail { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string AlternativePhoneNumber { get; set; }
        public string LoginEMail { get; set; }
        public IEnumerable<CommunicationPreference> CommunicationPreference { get; set; }
    }

    public class CommunicationPreference
    {
        public CommunicationPreferenceType PreferenceType { get; set; }
        public bool Accepted { get; set; }
    }
}
