using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;

namespace EI.RP.DomainServices.Queries.User.PhoneMetaData
{
    internal class PhoneMetadataResolverQueryHandler: QueryHandler<PhoneMetadataResolverQuery>
    {
        protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(PhoneMetadataResolverQuery query)
        {
            var phoneNumber = query.PhoneNumber;
            string phoneMetaDataType;
            AreaCodeConfig areaCodeConfigValue = null;
            phoneNumber = phoneNumber.Replace("+", "00");
            if (phoneNumber.StartsWith("00353"))
            {
                int len = phoneNumber.Length - 5;
                phoneNumber = phoneNumber.Substring(5, len);
            }

            string areaCode = phoneNumber.Substring(0, 3);
            areaCodeConfigValue = AreaCodesDictionary.FirstOrDefault(x => x.Key == areaCode).Value;
            if (areaCodeConfigValue!=null)
            {
                phoneMetaDataType = PhoneMetadataType.AllValues
                    .FirstOrDefault(x => x == areaCodeConfigValue.Type && areaCodeConfigValue.Length == phoneNumber.Length)?.ToString();
            }
            else
            {
                areaCode = phoneNumber.Substring(0, 2);
                if (phoneNumber.Length.ToString() == "9" && areaCode == "01")
                {
                    areaCodeConfigValue = AreaCodesDictionary.FirstOrDefault(x => x.Key == areaCode).Value;
                    if (areaCodeConfigValue != null)
                    {
                        phoneMetaDataType = PhoneMetadataType.AllValues
                            .FirstOrDefault(x => x == areaCodeConfigValue.Type && areaCodeConfigValue.Length == phoneNumber.Length)?.ToString();
                    }
                    else
                    {
                        phoneMetaDataType = PhoneMetadataType.Invalid;
                    }
                }
                else if (phoneNumber.StartsWith("0044") || phoneNumber.StartsWith("+44"))
                {
                    if (phoneNumber.Length == 13 || phoneNumber.Length == 14)
                    {
                        phoneMetaDataType = PhoneMetadataType.LandLine;
                    }
                    else
                    {
                        phoneMetaDataType = PhoneMetadataType.Invalid;
                    }
                }
                else if (phoneNumber.StartsWith("00"))
                {
                    if (phoneNumber.Length >= 8 && phoneNumber.Length <= 21)
                    {
                        phoneMetaDataType = PhoneMetadataType.LandLine;
                    }
                    else
                    {
                        phoneMetaDataType = PhoneMetadataType.Invalid;
                    }
                }
                else
                {
                    phoneMetaDataType = PhoneMetadataType.Invalid;
                }
            }

            var result=new Ei.Rp.DomainModels.User.PhoneMetaData();
            result.ContactNumberType = phoneMetaDataType ?? PhoneMetadataType.Invalid;
            return result.ToOneItemArray().Cast<TQueryResult>();
        }

        private static readonly Dictionary<string, AreaCodeConfig> AreaCodesDictionary =
            new Dictionary<string, AreaCodeConfig>
            {
                {"083", new AreaCodeConfig() {AreaCode = "083", Length = 10, Type = PhoneMetadataType.Mobile}},
                {"085", new AreaCodeConfig() {AreaCode = "085", Length = 10, Type = PhoneMetadataType.Mobile}},
                {"086", new AreaCodeConfig() {AreaCode = "086", Length = 10, Type = PhoneMetadataType.Mobile}},
                {"087", new AreaCodeConfig() {AreaCode = "087", Length = 10, Type = PhoneMetadataType.Mobile}},
                {"089", new AreaCodeConfig() {AreaCode = "089", Length = 10, Type = PhoneMetadataType.Mobile}},


                {"01", new AreaCodeConfig() {AreaCode = "01", Length = 9, Type = PhoneMetadataType.LandLine}},
                {"021", new AreaCodeConfig() {AreaCode = "021", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"022", new AreaCodeConfig() {AreaCode = "022", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"023", new AreaCodeConfig() {AreaCode = "023", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"024", new AreaCodeConfig() {AreaCode = "024", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"025", new AreaCodeConfig() {AreaCode = "025", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"026", new AreaCodeConfig() {AreaCode = "026", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"027", new AreaCodeConfig() {AreaCode = "027", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"028", new AreaCodeConfig() {AreaCode = "028", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"029", new AreaCodeConfig() {AreaCode = "029", Length = 8, Type = PhoneMetadataType.LandLine}},


                {"040", new AreaCodeConfig() {AreaCode = "040", Length = 9, Type = PhoneMetadataType.LandLine}},
                {"041", new AreaCodeConfig() {AreaCode = "041", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"042", new AreaCodeConfig() {AreaCode = "042", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"043", new AreaCodeConfig() {AreaCode = "043", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"044", new AreaCodeConfig() {AreaCode = "044", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"045", new AreaCodeConfig() {AreaCode = "045", Length = 9, Type = PhoneMetadataType.LandLine}},
                {"046", new AreaCodeConfig() {AreaCode = "046", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"047", new AreaCodeConfig() {AreaCode = "047", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"049", new AreaCodeConfig() {AreaCode = "049", Length = 10, Type = PhoneMetadataType.LandLine}},

                {"050", new AreaCodeConfig() {AreaCode = "050", Length = 9, Type = PhoneMetadataType.LandLine}},
                {"051", new AreaCodeConfig() {AreaCode = "051", Length = 9, Type = PhoneMetadataType.LandLine}},
                {"052", new AreaCodeConfig() {AreaCode = "052", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"053", new AreaCodeConfig() {AreaCode = "053", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"056", new AreaCodeConfig() {AreaCode = "056", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"057", new AreaCodeConfig() {AreaCode = "057", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"058", new AreaCodeConfig() {AreaCode = "058", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"059", new AreaCodeConfig() {AreaCode = "059", Length = 10, Type = PhoneMetadataType.LandLine}},

                {"061", new AreaCodeConfig() {AreaCode = "061", Length = 9, Type = PhoneMetadataType.LandLine}},
                {"062", new AreaCodeConfig() {AreaCode = "062", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"063", new AreaCodeConfig() {AreaCode = "063", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"064", new AreaCodeConfig() {AreaCode = "064", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"065", new AreaCodeConfig() {AreaCode = "065", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"066", new AreaCodeConfig() {AreaCode = "066", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"067", new AreaCodeConfig() {AreaCode = "067", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"068", new AreaCodeConfig() {AreaCode = "068", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"069", new AreaCodeConfig() {AreaCode = "069", Length = 8, Type = PhoneMetadataType.LandLine}},

                {"071", new AreaCodeConfig() {AreaCode = "071", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"072", new AreaCodeConfig() {AreaCode = "072", Length = 9, Type = PhoneMetadataType.LandLine}},
                {"074", new AreaCodeConfig() {AreaCode = "074", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"077", new AreaCodeConfig() {AreaCode = "077", Length = 9, Type = PhoneMetadataType.LandLine}},

                {"081", new AreaCodeConfig() {AreaCode = "081", Length = 10, Type = PhoneMetadataType.LandLine}},

                {"090", new AreaCodeConfig() {AreaCode = "090", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"091", new AreaCodeConfig() {AreaCode = "091", Length = 9, Type = PhoneMetadataType.LandLine}},
                {"093", new AreaCodeConfig() {AreaCode = "093", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"094", new AreaCodeConfig() {AreaCode = "094", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"095", new AreaCodeConfig() {AreaCode = "095", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"096", new AreaCodeConfig() {AreaCode = "096", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"097", new AreaCodeConfig() {AreaCode = "097", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"098_10", new AreaCodeConfig() {AreaCode = "098_10", Length = 10, Type = PhoneMetadataType.LandLine}},
                {"098_8", new AreaCodeConfig() {AreaCode = "098_8", Length = 8, Type = PhoneMetadataType.LandLine}},
                {"099", new AreaCodeConfig() {AreaCode = "099", Length = 8, Type = PhoneMetadataType.LandLine}}
            };

        private class AreaCodeConfig
        {
            public string AreaCode { get; set; }
            public PhoneMetadataType Type { get; set; }
            public int Length { get; set; }
        }

        protected override Type[] ValidQueryResultTypes { get; } = {typeof(Ei.Rp.DomainModels.User.PhoneMetaData)};
    }
}
