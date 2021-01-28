using System;
using System.Collections.Generic;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;
using EI.RP.WebApp.Flows.AppFlows;

namespace EI.RP.WebApp.Infrastructure.Flows
{
    public class NavigationItem
    {
        public string[] ClassList { get; set; }
        public string Role { get; set; }
        public ResidentialPortalFlowType[] RelatedFlowTypes { get; set; } = new ResidentialPortalFlowType[0];
        public AnchorLinkItem AnchorLink { get; set; }
        public string GetClassListString()
        {
            return ClassList == null ? string.Empty : string.Join(" ", ClassList);
        }
        public int Order { get; set; }

        public override string ToString()
        {
	        return $"{nameof(AnchorLink)}: {AnchorLink}";
        }

        public class AnchorLinkItem
        {
            public string TestId { get; set; }
            public string[] ClassList { get; set; }
            public string Href { get; set; } = "#";
            public string Label { get; set; }
            public string Role { get; set; }
            public FlowActionItem FlowAction { get; set; }

            public string Identifier { get; } = Guid.NewGuid().ToString();
            public string GetClassListString()
            {
                return ClassList == null ? string.Empty : string.Join(" ", ClassList);
            }

            public override string ToString()
            {
	            return $"{nameof(Label)}: {Label}";
            }
        }

        public class FlowActionItem
        {
            public string FlowName { get; set; }
            public FlowActionTagHelper.FlowActionType FlowActionType { get; set; } = FlowActionTagHelper.FlowActionType.TriggerEvent;
            public FlowActionTagHelper.StartFlowLocation FlowLocation { get; set; } = FlowActionTagHelper.StartFlowLocation.NotRelevantToThis;
            public object FlowParameters { get; set; }
            public FlowActionTagHelper.TagType SerializeAsTagType { get; set; } = FlowActionTagHelper.TagType.NotRelevantToThis;
            public ScreenEvent TriggerEvent { get; set; }
            public ResidentialPortalFlowType EventFlowType { get; set; }
            public IEnumerable<EventAdditionalField> EventAdditionalFields { get; set; } = new EventAdditionalField[0];


        }

        public class EventAdditionalField
        {
            public string PropertyPath { get; set; }
            public string Value { get; set; }
        }
    }
}
