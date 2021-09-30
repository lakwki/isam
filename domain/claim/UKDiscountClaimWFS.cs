using System;
using System.Collections.Generic;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKDiscountClaimWFS : DomainData
    {
        public static UKDiscountClaimWFS OUTSTANDING = new UKDiscountClaimWFS(Code.Outstanding);
        public static UKDiscountClaimWFS CLEARED = new UKDiscountClaimWFS(Code.Cleared);

        private Code _code;

        private enum Code
        {
            Outstanding = 0,
            Cleared = 1
        }

        public UKDiscountClaimWFS() { }

        private UKDiscountClaimWFS(Code code)
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
                    case Code.Outstanding:
                        return "OUTSTANDING";
                    case Code.Cleared:
                        return "CLEARED";
                    default:
                        return "ERROR";
                }
            }
        }

        public static UKDiscountClaimWFS getType(int id)
        {
            if (id == Code.Outstanding.GetHashCode()) return UKDiscountClaimWFS.OUTSTANDING;
            else if (id == Code.Cleared.GetHashCode()) return UKDiscountClaimWFS.CLEARED;
            else return null;
        }

        public static List<UKDiscountClaimWFS> getCollectionValues()
        {
            List<UKDiscountClaimWFS> list = new List<UKDiscountClaimWFS>();
            list.Add(UKDiscountClaimWFS.OUTSTANDING);
            list.Add(UKDiscountClaimWFS.CLEARED);
            return list;
        }
    }
}
