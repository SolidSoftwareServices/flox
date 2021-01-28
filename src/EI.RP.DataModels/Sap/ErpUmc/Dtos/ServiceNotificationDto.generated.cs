using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.ErpUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = true, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class ServiceNotificationDto : ODataDtoModel
    {
        public override string CollectionName() => "ServiceNotifications";
        public override object[] UniqueId()
        {
            return new object[]{ServiceNotificationID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ServiceNotificationID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string DeviceID
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? MalfunctionStartDate
        {
            get => _MalfunctionStartDate;
            set => _MalfunctionStartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _MalfunctionStartDate;
        public virtual DateTime? MalfunctionEndDate
        {
            get => _MalfunctionEndDate;
            set => _MalfunctionEndDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _MalfunctionEndDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string ServicePriorityID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string ServiceNotificationTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Description
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(300)]
        public virtual string Note
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string SystemStatus
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ServiceOrderID
        {
            get;
            set;
        }

        = string.Empty;
        public virtual DateTime? ChangedOn
        {
            get => _ChangedOn;
            set => _ChangedOn = CompensateSapDateTimeBug(value);
        }

        private DateTime? _ChangedOn;
        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        public virtual string Status
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [Filterable]
        public virtual bool Active
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string CreatedBy
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
        public virtual ServiceNotificationTypeDto ServiceNotificationType
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ServiceOrderDto ServiceOrder
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ServicePriorityDto ServicePriority
        {
            get;
            set;
        }
    }
}