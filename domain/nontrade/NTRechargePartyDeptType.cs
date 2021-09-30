using System;
using System.Collections;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class NTRechargePartyDeptType : DomainData 
    {
        public static NTRechargePartyDeptType ACCOUNTS_DEPARTMENT = new NTRechargePartyDeptType(Code.AccountsDepartment);
        public static NTRechargePartyDeptType ACCOUNT_PAYABLE = new NTRechargePartyDeptType(Code.AccountPayable);
        public static NTRechargePartyDeptType BRAND_FINANCE = new NTRechargePartyDeptType(Code.BrandFinance);
        public static NTRechargePartyDeptType GROUP_FINANCE = new NTRechargePartyDeptType(Code.GroupFinance);

        private Code _code;

        private NTRechargePartyDeptType(Code code)
        {
            this._code = code;
        }

        private enum Code
        {
            AccountsDepartment = 1,
            AccountPayable = 2,
            BrandFinance = 3,
            GroupFinance = 4
        }

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_code.GetHashCode());
            }
        }

        public string Description
        {
            get
            {
                switch (_code)
                {
                    case Code.AccountsDepartment:
                        return "Accounts Department";
                    case Code.AccountPayable:
                        return "Account Payable";
                    case Code.BrandFinance:
                        return "Brand Finance";
                    case Code.GroupFinance:
                        return "Group Finance";
                    default:
                        return "UNDEFINED";
                }
            }
        }


        public static NTRechargePartyDeptType getType(int id)
        {
            if (id == Code.AccountsDepartment.GetHashCode()) return NTRechargePartyDeptType.ACCOUNTS_DEPARTMENT;
            else if (id == Code.AccountPayable.GetHashCode()) return NTRechargePartyDeptType.ACCOUNT_PAYABLE;
            else if (id == Code.BrandFinance.GetHashCode()) return NTRechargePartyDeptType.BRAND_FINANCE;
            else if (id == Code.GroupFinance.GetHashCode()) return NTRechargePartyDeptType.GROUP_FINANCE;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(NTRechargePartyDeptType.ACCOUNTS_DEPARTMENT);
            list.Add(NTRechargePartyDeptType.ACCOUNT_PAYABLE);
            list.Add(NTRechargePartyDeptType.BRAND_FINANCE);
            list.Add(NTRechargePartyDeptType.GROUP_FINANCE);

            return list;
        }

    }
}
