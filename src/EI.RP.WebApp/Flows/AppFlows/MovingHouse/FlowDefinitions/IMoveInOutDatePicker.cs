using System;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
    public interface IMoveInOutDatePicker : IUiFlowScreenModel
    {
        MoveInOutDatePicker MoveInOutDatePicker { get; set; }
    }

    public class MoveInOutDatePicker
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a valid date")]
        public DateTime? MovingInOutSelectedDateTime { get; set; }
        public double Intervals { get; set; }
        public string DatePickerTitle { get; set; }
        public string DatePickerHoverPopupDescription { get; set; }
        public string DatePickerDescription { get; set; }
		public DateTimeRange SelectableDateRange { get; set; }


	}
}