using System;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.System;

namespace EI.RP.CoreServices.Cqrs.Events
{
    public partial class EventApiEvent: IEventApiMessage
    {
	    private string _mprn = 0.ToString();
	    private string _description;

	    public EventApiEvent(IClientInfoResolver clientInfoResolver)
        {
            ApplicationName = "EISMART";

            IpAddress = clientInfoResolver.ResolveIp();
            BrowserVersion = clientInfoResolver.ResolveBrowserVersion();
            OperatingSystem = clientInfoResolver.ResolveOperatingSystem();
            Hardware = clientInfoResolver.ResolveDevice();
            DeviceInfo = clientInfoResolver.ResolveUserAgent();
            Timestamp = DateTime.Now;
        }


        public long Id { get; set; }
        public long? CategoryId { get; set; }
        public long? SubCategoryId { get; set; }
        public long ActionId { get; set; }

        public string Description
        {
	        get => _description;
	        set => _description = value?.Substring(0,Math.Min(50,value.Length));

        }

        public DateTime? Timestamp { get; }

        public string MPRN
        {
	        get => _mprn;
	        set => _mprn = value.GetEndSubstring(6);
        }

        public string IpAddress { get;  }
        public long? Partner { get; set; } = 0;
        public long? ContractAccount { get; set; }
        public string Username { get; set; }
        public string ApplicationName { get;}
        public string DeviceInfo { get; }
        public string OperatingSystem { get;}
        public string Hardware { get; }
        public string BrowserVersion { get; }

        public override string ToString()
        {
	        return $"{nameof(Id)}: {Id}, {nameof(CategoryId)}: {CategoryId}, {nameof(SubCategoryId)}: {SubCategoryId}, {nameof(ActionId)}: {ActionId}, {nameof(Description)}: {Description}, {nameof(ContractAccount)}: {ContractAccount}, {nameof(Username)}: {Username}";
        }
        

    }
}