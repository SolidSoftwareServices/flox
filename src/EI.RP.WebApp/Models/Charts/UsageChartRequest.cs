using System;

namespace EI.RP.WebApp.Models.Charts
{
    public class UsageChartRequest
    {
        public string AccountNumber { get; set; }
        public string Period { get; set; }
        public DateTime Date { get; set; }

     
    }
}
