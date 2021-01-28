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
    public partial class RegisterToReadDto : ODataDtoModel
    {
        public override string CollectionName() => "RegistersToRead";
        public override object[] UniqueId()
        {
            return new object[]{DeviceID, RegisterID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string DeviceID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string RegisterID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string RegisterTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string ReadingUnit
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string IntegerPlaces
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string DecimalPlaces
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual bool NoMeterReadingOrderFlag
        {
            get;
            set;
        }

        public virtual decimal? PreviousMeterReadingResult
        {
            get;
            set;
        }

        public virtual DateTime? PreviousMeterReadingDate
        {
            get => _PreviousMeterReadingDate;
            set => _PreviousMeterReadingDate = CompensateSapDateTimeBug(value);
        }

        private DateTime? _PreviousMeterReadingDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string PreviousMeterReadingReasonID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string PreviousMeterReadingCategoryID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string SerialNumber
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual decimal Multiplier
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual DeviceDto Device
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual MeterReadingReasonDto MeterReadingReason
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ProfileHeaderDto> ProfileHeaders
        {
            get;
            set;
        }

        = new List<ProfileHeaderDto>();
        [Sortable]
        [Filterable]
        public virtual List<ProfileValuesFileDto> ProfileValuesFiles
        {
            get;
            set;
        }

        = new List<ProfileValuesFileDto>();
        [Sortable]
        [Filterable]
        public virtual MeterReadingCategoryDto MeterReadingCategory
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ProfileValueDto> ProfileValues
        {
            get;
            set;
        }

        = new List<ProfileValueDto>();
        [Sortable]
        [Filterable]
        public virtual RegisterTypeDto RegisterType
        {
            get;
            set;
        }
    }
}