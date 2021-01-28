using EI.RP.WebApp.Models.Shared;

namespace EI.RP.WebApp.Models
{
    public class ErrorViewModel : LayoutWithNavModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
