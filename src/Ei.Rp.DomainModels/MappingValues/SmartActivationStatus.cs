namespace Ei.Rp.DomainModels.MappingValues
{
	public enum SmartActivationStatus
	{

		/// <summary>
		/// Smart features and eligibility are not available
		/// </summary>
		SmartNotAvailable=0,

		/// <summary>
		/// it is smart but not eligible
		/// </summary>
		SmartButNotEligible,

		/// <summary>
		/// it is smart and not active but eligible
		/// </summary>
		SmartAndEligible,

		/// <summary>
		/// already smart active
		/// </summary>
		SmartActive,
	}
}