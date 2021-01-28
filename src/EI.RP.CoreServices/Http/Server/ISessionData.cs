using System;
using System.Collections.Generic;
using System.Text;

namespace EI.RP.CoreServices.Http.Server
{
    public interface ISessionData
    {
        void Set<TValue>(string key, TValue value) where TValue : class;
        TValue Get<TValue>(string key) where TValue:class;
    }
}
