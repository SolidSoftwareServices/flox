namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.Head
{
	public class InputModel
	{
        public string Title { get; set; }
        public MetaTag[] MetaTags { get; set; } = new MetaTag[0];
        public StyleTag[] StyleTags { get; set; } = new StyleTag[0];
	}
}
