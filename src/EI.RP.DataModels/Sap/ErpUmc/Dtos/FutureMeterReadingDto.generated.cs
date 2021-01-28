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
    public partial class FutureMeterReadingDto : ODataDtoModel
    {
        public override string CollectionName() => "FutureMeterReadings";
        public override object[] UniqueId()
        {
            return new object[]{DeviceID, MeterReadingDate, };
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
        [StringLength(2)]
        public virtual string MeterReadingReasonID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string MeterReadingCategoryID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime MeterReadingDate
        {
            get => _MeterReadingDate;
            set => _MeterReadingDate = CompensateSapDateTimeBug(value);
        }

        private DateTime _MeterReadingDate;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string SerialNumber
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual MeterReadingCategoryDto MeterReadingCategory
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
    }
}