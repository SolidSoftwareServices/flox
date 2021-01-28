using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class InstallationDiscStatusType : TypedStringValue<InstallationDiscStatusType>
    {

        [JsonConstructor]
        private InstallationDiscStatusType()
        {
        }

        private InstallationDiscStatusType(string value, string debuggerFriendlyDisplayValue = null) : base(value, debuggerFriendlyDisplayValue, true)
        {
        }

        public static readonly InstallationDiscStatusType New = new InstallationDiscStatusType("00", nameof(New));
        public static readonly InstallationDiscStatusType NewReleaseUnnecessary = new InstallationDiscStatusType("01", "New, release unnecessary");
        public static readonly InstallationDiscStatusType DeRegistered = new InstallationDiscStatusType("05", nameof(DeRegistered));
		public static readonly InstallationDiscStatusType Released = new InstallationDiscStatusType("10", nameof(Released));
        public static readonly InstallationDiscStatusType DisconnectionStarted = new InstallationDiscStatusType("20", "Disconnection started");
        public static readonly InstallationDiscStatusType DisconnectionCarriedOut = new InstallationDiscStatusType("21", "Disconnection carried out");
        public static readonly InstallationDiscStatusType ReconnectionStarted = new InstallationDiscStatusType("22", "Reconnection started");
        public static readonly InstallationDiscStatusType RemovalStarted = new InstallationDiscStatusType("23", "Device removal started");
        public static readonly InstallationDiscStatusType RemovalExecuted = new InstallationDiscStatusType("24", "Device Removal Executed");
        public static readonly InstallationDiscStatusType InstallationStarted = new InstallationDiscStatusType("25", "Device installation started");
        public static readonly InstallationDiscStatusType InstallationCarriedOut = new InstallationDiscStatusType("26", "Device installation carried out");
        public static readonly InstallationDiscStatusType AllRemovedAndDisconnected = new InstallationDiscStatusType("27", "All Devices Removed and Disconnected");
        public static readonly InstallationDiscStatusType ReconnectionCarriedOut  = new InstallationDiscStatusType("30", "Reconnection carried out completely");
        public static readonly InstallationDiscStatusType Completed = new InstallationDiscStatusType("99", nameof(Completed));

    }

}