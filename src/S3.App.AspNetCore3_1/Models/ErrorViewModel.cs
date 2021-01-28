using System;

namespace S3.App.AspNetCore3_1.Models
{
	public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}