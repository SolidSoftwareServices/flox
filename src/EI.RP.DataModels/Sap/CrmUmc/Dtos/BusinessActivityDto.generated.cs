using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = true, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class BusinessActivityDto : ODataDtoModel
    {
        public override string CollectionName() => "BusinessActivities";
        public override object[] UniqueId()
        {
            return new object[]{ActivityID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Sortable]
        [Filterable]
        public virtual ReasonDto Reason
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string ActivityID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string InteractionRecordID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        [Filterable]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string PremiseID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string BusinessAgreementID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string Priority
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(20)]
        public virtual string ActivityReasonID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        [Filterable]
        public virtual string ChannelID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1320)]
        public virtual string Note
        {
            get;
            set;
        }

        = string.Empty;
        [Filterable]
        public virtual DateTime? DateFrom
        {
            get => _DateFrom;
            set => _DateFrom = CompensateSapDateTimeBug(value);
        }

        private DateTime? _DateFrom;
        [Required(AllowEmptyStrings = true)]
        [StringLength(13)]
        [Filterable]
        public virtual string DocumentStatusID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        [Filterable]
        public virtual string ProcessType
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual InteractionRecordDto InteractionRecord
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ChannelDto Channel
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual DocumentStatusDto DocumentStatus
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual InteractionRecordReasonDto ActivityReason
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ExternalReferenceDto> ExternalReferences
        {
            get;
            set;
        }

        = new List<ExternalReferenceDto>();
    }
}