using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.User
{
    public class PhoneMetaData: IQueryResult
    {
        public string ContactNumberType { get; set; }
    }
}
