using System;
using System.Collections.Generic;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using Newtonsoft.Json;

namespace EI.RP.UiFlows.Core.Flows.Screens.Metadata
{
	internal sealed class RuntimeScreenMetaData : IScreenMetaData, IEquatable<RuntimeScreenMetaData>
	{
		[JsonProperty] private string _containedFlowType;

		private UiFlowScreenModel _userData;
		public DateTime DateCreated { get; set; } = DateTime.UtcNow;
		public IEnumerable<UiFlowUserInputError> Errors { get; set; } = new List<UiFlowUserInputError>();


		public bool Equals(RuntimeScreenMetaData other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return _containedFlowType == other._containedFlowType
			       && FlowHandler == other.FlowHandler
			       && ContainerProperties.IsEqualTo(other.ContainerProperties)
			       && ContainerController == other.ContainerController
			       && ContainedFlowHandler == other.ContainedFlowHandler
			       && ContainedFlowStartType == other.ContainedFlowStartType;
		}

		public UiFlowScreenModel UserData
		{
			get => _userData;
			set
			{
				if (!ReferenceEquals(this, value.Metadata)) value.Metadata = this;
				_userData = value;
			}
		}

		public string FlowHandler { get; set; }
		public string FlowScreenName { get; set; }
		public IDictionary<string, object> ContainerProperties { get; } = new Dictionary<string, object>();
		public string ContainerController { get; set; }

		public string ContainedFlowHandler { get; set; }
		public string ContainedFlowStartType { get; set; }

		[JsonIgnore]
		public string ContainedFlowType
		{
			get => _containedFlowType;
			set
			{
				if (!IsFlowContainer)
					throw new InvalidOperationException(
						$"{UserData.GetType().FullName}.{nameof(UiFlowScreenModel)} .cctor parameter {IsFlowContainer} does not allow to add a contained flow");

				//TODO: whatever this does, it should not be here
				if (_containedFlowType != null && _containedFlowType != value) ContainedFlowHandler = null;
				if (_containedFlowType != value) _containedFlowType = value;
			}
		}


		public bool IsFlowContainer { get; set; }

		public string FlowType { get; set; }


		public bool IsContainedInController()
		{
			return !string.IsNullOrEmpty(ContainerController);
		}


		public bool IsWellFormed(out string reason)
		{
			reason = null;
			var result = true;
			if (FlowHandler == null || FlowScreenName == null)
			{
				reason = "Some properties have not been set";
				result = false;
			}


			return result;
		}


		[Obsolete("TODO: MOVE TO THE STEP DATA?")]
		public void AddError(UiFlowUserInputError error)
		{
			if (error == null || string.IsNullOrWhiteSpace(error.ErrorMessage))
				throw new ArgumentNullException(nameof(error));
			var lst = (List<UiFlowUserInputError>) Errors;
			lst.Add(error);
			Errors = lst;
		}


		[Obsolete("TODO: need to be fixed?")]
		public IScreenMetaData IncludedInStep()
		{
			return this;
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is RuntimeScreenMetaData other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _containedFlowType != null ? _containedFlowType.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (FlowHandler != null ? FlowHandler.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ DateCreated.GetHashCode();
				hashCode = (hashCode * 397) ^ (ContainerProperties != null ? ContainerProperties.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ContainerController != null ? ContainerController.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ContainedFlowHandler != null ? ContainedFlowHandler.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^
				           (ContainedFlowStartType != null ? ContainedFlowStartType.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(RuntimeScreenMetaData left, RuntimeScreenMetaData right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RuntimeScreenMetaData left, RuntimeScreenMetaData right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return
				$"{nameof(FlowHandler)}: {FlowHandler}, {nameof(FlowScreenName)}: {FlowScreenName}, {nameof(DateCreated)}: {DateCreated}, {nameof(ContainerProperties)}: {ContainerProperties}, {nameof(ContainerController)}: {ContainerController}, {nameof(ContainedFlowHandler)}: {ContainedFlowHandler}, {nameof(ContainedFlowStartType)}: {ContainedFlowStartType}, {nameof(ContainedFlowType)}: {ContainedFlowType}";
		}
	}
}