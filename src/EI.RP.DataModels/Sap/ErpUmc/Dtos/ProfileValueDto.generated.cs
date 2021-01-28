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
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class ProfileValueDto : ODataDtoModel
    {
        public override string CollectionName() => "ProfileValues";
        public override object[] UniqueId()
        {
            return new object[]{ProfileHeaderID, Timestamp, OutputInterval, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        [Filterable]
        public virtual string ProfileHeaderID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [Filterable]
        public virtual DateTime Timestamp
        {
            get => _Timestamp;
            set => _Timestamp = CompensateSapDateTimeBug(value);
        }

        private DateTime _Timestamp;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        [Filterable]
        public virtual string OutputInterval
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        [Filterable]
        public virtual string ContractID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        [Filterable]
        public virtual string DeviceID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        [Filterable]
        public virtual string RegisterID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        [Filterable]
        public virtual string ProfileTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string ProfileValueTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        [Filterable]
        public virtual string UnitOfMeasure
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        [Filterable]
        public virtual string Currency
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        public virtual string TimeZone
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal Value
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ProfileValueTypeDto ProfileValueType
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual ProfileTypeDto ProfileType
        {
            get;
            set;
        }
    }
}