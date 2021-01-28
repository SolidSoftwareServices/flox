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
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class InteractionRecordDto : ODataDtoModel
    {
        public override string CollectionName() => "InteractionRecords";
        public override object[] UniqueId()
        {
            return new object[]{InteractionRecordID, };
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
        public virtual string InteractionRecordReasonID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        public virtual string InteractionRecordCategory1
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        public virtual string InteractionRecordCategory2
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(22)]
        public virtual string InteractionRecordCategory1GUID
        {
            get;
            set;
        }

        [StringLength(22)]
        public virtual string InteractionRecordCategory2GUID
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string ChannelID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
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
        public virtual bool IncomingFlag
        {
            get;
            set;
        }

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
        [StringLength(1)]
        public virtual string Direction
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual InteractionRecordReasonDto InteractionRecordReason
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual InteractionRecordCategoryDto InteractionRecordCategoryLevel1
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual InteractionRecordCategoryDto InteractionRecordCategoryLevel2
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<AttachmentDto> Attachments
        {
            get;
            set;
        }

        = new List<AttachmentDto>();
        [Sortable]
        [Filterable]
        public virtual DocumentStatusDto DocumentStatus
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
        public virtual List<BusinessActivityDto> BusinessActivities
        {
            get;
            set;
        }

        = new List<BusinessActivityDto>();
    }
}