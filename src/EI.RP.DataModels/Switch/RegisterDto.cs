using System;
using System.Runtime.Serialization;

namespace EI.RP.DataModels.Switch
{
    public class RegisterDto
    {
        public string Device { get; set; }
        public string TimeofUsePeriod { get; set; }
        public string MPRN { get; set; }
        public int RegisterType { get; set; }
    }
}