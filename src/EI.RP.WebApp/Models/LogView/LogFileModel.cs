using System;

namespace EI.RP.WebApp.Models.LogView
{
    public class LogFileModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
	    public DateTime LastChanged { get; set; }
    }
}