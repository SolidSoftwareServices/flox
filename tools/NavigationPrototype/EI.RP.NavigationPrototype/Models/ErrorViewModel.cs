using System;

namespace EI.RP.NavigationPrototype.Models
{
	public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}