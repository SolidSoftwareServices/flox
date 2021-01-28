using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.ChangePassword
{
    internal class ChangePasswordPageEnergyService : MyAccountEnergyServicesPage

    {
        public ChangePasswordPageEnergyService(ResidentialPortalApp app) : base(app)
        {
        }

        protected override bool IsInPage()
        {
            var result = base.IsInPage() && Document.QuerySelector("#changePasswordHeader")?.TextContent ==
                         "Change Password";
            return result;
        }
    }
}
