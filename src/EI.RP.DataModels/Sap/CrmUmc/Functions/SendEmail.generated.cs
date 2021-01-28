using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;

namespace EI.RP.DataModels.Sap.CrmUmc.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class SendEmailFunction : ODataFunction<SendEmailFunction.QueryObject, BooleanResultDto>
    {
        public SendEmailFunction(params Expression<Func<BooleanResultDto, object>>[] expands): base("SendEmail", expands)
        {
        }

        public override bool ReturnsComplexType() => true;
        public override bool ReturnsCollection() => false;
        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        public class QueryObject
        {
            [StringLength(10)]
            public virtual string AccountID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(30)]
            public virtual string MailFormID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(250)]
            public virtual string SenderEmail
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(250)]
            public virtual string ReplyToEmail
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(250)]
            public virtual string CCEmail1
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string ActivityID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(255)]
            public virtual string Attribute1Name
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string Attribute1Value
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(255)]
            public virtual string Attribute2Name
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string Attribute2Value
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(255)]
            public virtual string Attribute3Name
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string Attribute3Value
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(255)]
            public virtual string Attribute4Name
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string Attribute4Value
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(255)]
            public virtual string Attribute5Name
            {
                get;
                set;
            }

            = string.Empty;
            public virtual string Attribute5Value
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}