using System;
using System.Collections.Generic;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;
using EI.RP.UiFlows.Core.Flows.Screens.Metadata;
using Newtonsoft.Json;

namespace EI.RP.UiFlows.Core.Flows.Screens.Models
{
	public class UiFlowScreenModel : IUiFlowScreenModel, IEquatable<UiFlowScreenModel>
	{
		[JsonConstructor]
		public UiFlowScreenModel() : this(false)
		{
		}

		protected UiFlowScreenModel(bool isContainer)
		{
			Metadata = new RuntimeScreenMetaData
			{
				UserData = this
			};

			Metadata.IsFlowContainer = isContainer;
		}

		[JsonIgnore] internal RuntimeScreenMetaData Metadata { get; set; }

		public bool Equals(UiFlowScreenModel other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Metadata, other.Metadata);
		}

		//TODO: INTERNALIZE
		[JsonProperty]
		public string FlowHandler
		{
			get => Metadata.FlowHandler;
			set => Metadata.FlowHandler = value;
		}

		

		[JsonProperty]
		public string FlowType
		{
			get => Metadata.FlowType;
			set => Metadata.FlowType= value;
		}
		[JsonProperty]
		public string FlowScreenName
		{
			get => Metadata.FlowScreenName;
			set => Metadata.FlowScreenName = value;
		}

		[JsonProperty] public string ScreenTitle { get; set; }

		/// <summary>
		///     Indicates if the flow view model is valid for a given step
		/// </summary>
		/// <returns></returns>
		public virtual bool IsValidFor(ScreenName screenStep)
		{
			return true;
		}

		public TScreenFlow GetFlowType<TScreenFlow>() where TScreenFlow:struct
		{
			return FlowType.ToEnum<TScreenFlow>();
		}

		public IEnumerable<UiFlowUserInputError> Errors
		{
			get => Metadata.Errors;
			set => Metadata.Errors = value;
		}

		/// <summary>
		///     The events Specified dont trigger validation
		/// </summary>
		public virtual IEnumerable<ScreenEvent> DontValidateEvents { get; } =
			new[] {ScreenEvent.ErrorOccurred, ScreenEvent.Start};

		public void ClearContainedFlow()
		{
			Metadata.ContainedFlowType = null;
		}

		public void SetContainedFlow<TScreenFlow>(TScreenFlow newContainedFlow, string containedFlowStartType = null)
		{
			Metadata.ContainedFlowType = newContainedFlow.ToString();
			Metadata.ContainedFlowStartType = containedFlowStartType;
		}

		public string GetContainedFlowStartType()
		{
			return Metadata.ContainedFlowStartType;
		}

		public string GetContainedFlowHandler()
		{
			return Metadata.ContainedFlowHandler;
		}

		public bool HasContainedFlow()
		{
			return !string.IsNullOrEmpty(GetContainedFlowHandler());
		}

		public TScreenFlow? GetContainedFlow<TScreenFlow>() where TScreenFlow : struct
		{
			return Metadata.ContainedFlowType?.ToEnum<TScreenFlow>();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((UiFlowScreenModel) obj);
		}

		public override int GetHashCode()
		{
			return Metadata != null ? Metadata.GetHashCode() : 0;
		}

		public static bool operator ==(UiFlowScreenModel left, UiFlowScreenModel right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(UiFlowScreenModel left, UiFlowScreenModel right)
		{
			return !Equals(left, right);
		}
	}
}