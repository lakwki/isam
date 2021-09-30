using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class BIAActionType : DomainData
    {
        public static BIAActionType NS_PROVISION = new BIAActionType(Code.NS_Provision);
        public static BIAActionType NS_COST = new BIAActionType(Code.NS_Cost);
        public static BIAActionType SUPPLIER_RECHARGE = new BIAActionType(Code.Supplier_Recharge);
        public static BIAActionType SUPPLIER_REFUND = new BIAActionType(Code.Supplier_Refund);

        private Code _code;

        private enum Code
        {
            NS_Provision = 1,
            NS_Cost = 2,
            Supplier_Recharge = 3,
            Supplier_Refund = 4
        }

        private BIAActionType(Code code)
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
                    case Code.NS_Provision:
                        return "NS Provision";
                    case Code.NS_Cost:
                        return "NS Cost";
                    case Code.Supplier_Recharge:
                        return "Supplier Recharge";
                    case Code.Supplier_Refund:
                        return "Supplier Refund";
                    default:
                        return "ERROR";
                }
            }
        }

        public static BIAActionType getType(int id)
        {
            if (id == Code.NS_Provision.GetHashCode()) return BIAActionType.NS_PROVISION;
            else if (id == Code.NS_Cost.GetHashCode()) return BIAActionType.NS_COST;
            else if (id == Code.Supplier_Recharge.GetHashCode()) return BIAActionType.SUPPLIER_RECHARGE;
            else if (id == Code.Supplier_Refund.GetHashCode()) return BIAActionType.SUPPLIER_REFUND;
            else return null;
        }

    }
}
