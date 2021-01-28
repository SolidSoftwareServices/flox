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
    public partial class DeviceDto : ODataDtoModel
    {
        public override string CollectionName() => "Devices";
        public override object[] UniqueId()
        {
            return new object[]{DeviceID, };
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
        [StringLength(10)]
        public virtual string ContractID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string DivisionID
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
        [StringLength(8)]
        public virtual string FunctionClass
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string DeviceMaterial
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string DeviceLocation
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(30)]
        public virtual string DeviceDescription
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string InstallationID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual DivisionDto Division
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<RegisterToReadDto> IntervalRegistersToRead
        {
            get;
            set;
        }

        = new List<RegisterToReadDto>();
        [Sortable]
        [Filterable]
        public virtual ContractDto Contract
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<MeterReadingResultDto> MeterReadingResults
        {
            get;
            set;
        }

        = new List<MeterReadingResultDto>();
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
        public virtual List<ProfileValuesFileDto> ProfileValuesFiles
        {
            get;
            set;
        }

        = new List<ProfileValuesFileDto>();
        [Sortable]
        [Filterable]
        public virtual List<RegisterToReadDto> RegistersToRead
        {
            get;
            set;
        }

        = new List<RegisterToReadDto>();
        [Sortable]
        [Filterable]
        public virtual List<FutureMeterReadingDto> FutureMeterReadings
        {
            get;
            set;
        }

        = new List<FutureMeterReadingDto>();
        [Sortable]
        [Filterable]
        public virtual DeviceCategoryDto DeviceCategory
        {
            get;
            set;
        }
    }
}