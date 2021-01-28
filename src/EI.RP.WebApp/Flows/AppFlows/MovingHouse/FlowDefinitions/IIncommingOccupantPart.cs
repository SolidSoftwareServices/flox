using System;
using System.ComponentModel.DataAnnotations;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.UiFlows.Core.Flows.Screens.Models;

using EI.RP.WebApp.Infrastructure.StringResources;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
    public interface IIncommingOccupantPart : IUiFlowScreenModel
    {
        LettingFields UserLettingFields { get; set; }
    }
    public class LettingFields
    {
        public bool IncomingOccupant { get; set; }

        [RequiredIf(nameof(IncomingOccupant), IfValue = true,
            ErrorMessage = "Please provide the name of the incoming occupier")]
        [RegularExpression(ReusableRegexPattern.ValidName, ErrorMessage =
            "Please enter a valid letting agent name")]
        public string LettingAgentName { get; set; }

        [RequiredIf(nameof(IncomingOccupant), IfValue = true,
            ErrorMessage = "You must enter a valid phone number")]
        [RegularExpression(ReusableRegexPattern.ValidPhoneNumber, ErrorMessage =
            "You must enter a valid phone number")]
        public string LettingPhoneNumber { get; set; }

        [RequiredIf(nameof(IncomingOccupant), IfValue = true,
            ErrorMessage = "Please accept the occupier details.")]
        public bool OccupierDetailsAccepted { get; set; }
    }
}