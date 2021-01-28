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
    public partial class ServiceOrderDto : ODataDtoModel
    {
        public override string CollectionName() => "ServiceOrders";
        public override object[] UniqueId()
        {
            return new object[]{ServiceOrderID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ServiceOrderID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string ServiceOrderTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime StartDate
        {
            get => _StartDate;
            set => _StartDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _StartDate;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime EndDate
        {
            get => _EndDate;
            set => _EndDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _EndDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string ServicePriorityID
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
        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        public virtual string Status
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
        [StringLength(30)]
        public virtual string ConnectionObjectID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string CompanyCode
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(32)]
        public virtual string MeterRead
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(8)]
        public virtual string WorkCentre
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual ServiceNotificationDto ServiceNotification
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

        [Sortable]
        [Filterable]
        public virtual ServiceOrderTypeDto ServiceOrderType
        {
            get;
            set;
        }
    }
}