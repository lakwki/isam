using System;
using com.next.common.domain;
using System.Collections;
using System.Collections.Generic;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimSettlemtType : DomainData
    {
        public static UKClaimSettlemtType DEFAULT = new UKClaimSettlemtType(Code.Default);
        public static UKClaimSettlemtType PROVISION = new UKClaimSettlemtType(Code.Provision);
        public static UKClaimSettlemtType SUPPLIER = new UKClaimSettlemtType(Code.Supplier);

        private Code _code;

        private enum Code
        {
            Default = 0,
            Provision = 1,
            Supplier = 2
        }
        public UKClaimSettlemtType() { }

        private UKClaimSettlemtType(Code code)
        {
            this._code = code;
        }

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_code.GetHashCode());
            }
        }

        public string Name
        {
            get
            {
                switch (_code)
                {
                    case Code.Default:
                        return "Default";
                    case Code.Provision:
                        return "NS Provision";
                    case Code.Supplier:
                        return "Supplier";
                    default:
                        return "ERROR";
                }
            }
        }

        public static UKClaimSettlemtType getType(int id)
        {
            if (id == Code.Default.GetHashCode()) return UKClaimSettlemtType.DEFAULT;
            else if (id == Code.Provision.GetHashCode()) return UKClaimSettlemtType.PROVISION;
            else if (id == Code.Supplier.GetHashCode()) return UKClaimSettlemtType.SUPPLIER;
            else return null;
        }

        public static List<UKClaimSettlemtType> getCollectionValues()
        {
            List<UKClaimSettlemtType> list = new List<UKClaimSettlemtType>();
            list.Add(UKClaimSettlemtType.DEFAULT);
            list.Add(UKClaimSettlemtType.PROVISION);
            list.Add(UKClaimSettlemtType.SUPPLIER);
            return list;
        }


    }
}
