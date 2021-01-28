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
    [CrudOperations(CanAdd = false, CanUpdate = true, CanDelete = false, CanQuery = true)]
    public partial class AccountDto : ODataDtoModel
    {
        public override string CollectionName() => "Accounts";
        public override object[] UniqueId()
        {
            return new object[]{AccountID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string AccountTypeID
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
        [StringLength(4)]
        public virtual string AccountTitleID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string FirstName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string LastName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string MiddleName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string SecondName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string Sex
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Name1
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Name2
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Name3
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string Name4
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string GroupName1
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(40)]
        public virtual string GroupName2
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(80)]
        public virtual string FullName
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string CorrespondenceLanguage
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string CorrespondenceLanguageISO
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(1)]
        public virtual string Language
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string LanguageISO
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual List<AccountContactDto> AccountContacts
        {
            get;
            set;
        }

        = new List<AccountContactDto>();
        [Sortable]
        [Filterable]
        public virtual AccountTypeDto AccountType
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<AccountAlertDto> AccountAlerts
        {
            get;
            set;
        }

        = new List<AccountAlertDto>();
        [Sortable]
        [Filterable]
        public virtual AccountAddressDto StandardAccountAddress
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<CommunicationPreferenceDto> CommunicationPreferences
        {
            get;
            set;
        }

        = new List<CommunicationPreferenceDto>();
        [Sortable]
        [Filterable]
        public virtual List<CorrespondenceDto> Correspondences
        {
            get;
            set;
        }

        = new List<CorrespondenceDto>();
        [Sortable]
        [Filterable]
        public virtual List<ServiceNotificationDto> ServiceNotifications
        {
            get;
            set;
        }

        = new List<ServiceNotificationDto>();
        [Sortable]
        [Filterable]
        public virtual List<ContractAccountDto> ContractAccounts
        {
            get;
            set;
        }

        = new List<ContractAccountDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressDto> AccountAddresses
        {
            get;
            set;
        }

        = new List<AccountAddressDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountRelationshipDto> AccountRelationships
        {
            get;
            set;
        }

        = new List<AccountRelationshipDto>();
        [Sortable]
        [Filterable]
        public virtual AccountBalanceDto AccountBalance
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<PaymentCardDto> PaymentCards
        {
            get;
            set;
        }

        = new List<PaymentCardDto>();
        [Sortable]
        [Filterable]
        public virtual List<ServiceOrderDto> ServiceOrders
        {
            get;
            set;
        }

        = new List<ServiceOrderDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressIndependentEmailDto> AccountAddressIndependentEmails
        {
            get;
            set;
        }

        = new List<AccountAddressIndependentEmailDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressIndependentMobilePhoneDto> AccountAddressIndependentMobilePhones
        {
            get;
            set;
        }

        = new List<AccountAddressIndependentMobilePhoneDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressIndependentPhoneDto> AccountAddressIndependentPhones
        {
            get;
            set;
        }

        = new List<AccountAddressIndependentPhoneDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressIndependentFaxDto> AccountAddressIndependentFaxes
        {
            get;
            set;
        }

        = new List<AccountAddressIndependentFaxDto>();
        [Sortable]
        [Filterable]
        public virtual List<PaymentDocumentDto> PaymentDocuments
        {
            get;
            set;
        }

        = new List<PaymentDocumentDto>();
        [Sortable]
        [Filterable]
        public virtual List<InvoiceDto> Invoices
        {
            get;
            set;
        }

        = new List<InvoiceDto>();
        [Sortable]
        [Filterable]
        public virtual List<BankAccountDto> BankAccounts
        {
            get;
            set;
        }

        = new List<BankAccountDto>();
        [Sortable]
        [Filterable]
        public virtual AccountSexDto AccountSex
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual AccountTitleDto AccountTitle
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ESAProductBalanceDto> AccountESAProductBalance
        {
            get;
            set;
        }

        = new List<ESAProductBalanceDto>();
    }
}