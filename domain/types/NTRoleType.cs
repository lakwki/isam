using System;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class NTRoleType : DomainData 
    {

        public static NTRoleType FIRST_APPROVER = new NTRoleType(Code.FirstApprover);
        public static NTRoleType SECOND_APPROVER = new NTRoleType(Code.SecondApprover);
        public static NTRoleType MONTH_END_ADMIN = new NTRoleType(Code.MonthEndAdmin);
        public static NTRoleType DEPARTMENT_APPROVER_ADMIN = new NTRoleType(Code.DepartmentApproverAdmin);
        public static NTRoleType VENDOR_APPROVER = new NTRoleType(Code.VendorApprover);
        public static NTRoleType RECHARGE_DCNOTE_USER = new NTRoleType(Code.RechargeDCNoteUser);
        public static NTRoleType GENERAL_USER = new NTRoleType(Code.GeneralUser);
        public static NTRoleType VENDOR_ACCOUNT_EDIT_USER = new NTRoleType(Code.VendorAccountEditUser);
        public static NTRoleType SETTLEMENT_MAINTENANCE = new NTRoleType(Code.SettlementMaintenance);
        public static NTRoleType SUN_INTERFACE = new NTRoleType(Code.SunInterface);

        private Code _code;

        private enum Code
        {
            FirstApprover = 1,
            SecondApprover = 2,
            MonthEndAdmin = 3,
            DepartmentApproverAdmin = 4,
            VendorApprover = 5,
            RechargeDCNoteUser = 6,
            GeneralUser = 7,
            VendorAccountEditUser = 8,
            SettlementMaintenance = 9,
            SunInterface = 10
        }

        private NTRoleType(Code code)
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
                    case Code.FirstApprover :
                        return "FIRST APPROVER";                    
                    case Code.SecondApprover :
                        return "SECOND APPROVER";
                    case Code.MonthEndAdmin:
                        return "MONTH-END ADMIN";
                    case Code.DepartmentApproverAdmin :
                        return "DEPARTMENT APPROVER ADMIN";
                    case Code.VendorApprover:
                        return "VENDOR APPROVER";
                    case Code.RechargeDCNoteUser:
                        return "RECHARGE DCNOTE USER";
                    case Code.GeneralUser:
                        return "GENERAL USER";
                    case Code.VendorAccountEditUser:
                        return "VENDOR ACCOUNT USER";
                    case Code.SettlementMaintenance:
                        return "SETTLEMENT MAINTENANCE";
                    case Code.SunInterface:
                        return "SUN INTERFACE";
                    default:
                        return "UNDEFINED";
                }
            }
        }


        public static NTRoleType getType(int id)
        {
            if (id == Code.FirstApprover.GetHashCode()) return NTRoleType.FIRST_APPROVER;
            else if (id == Code.SecondApprover.GetHashCode()) return NTRoleType.SECOND_APPROVER;
            else if (id == Code.MonthEndAdmin.GetHashCode()) return NTRoleType.MONTH_END_ADMIN;
            else if (id == Code.DepartmentApproverAdmin.GetHashCode()) return NTRoleType.DEPARTMENT_APPROVER_ADMIN;
            else if (id == Code.VendorApprover.GetHashCode()) return NTRoleType.VENDOR_APPROVER;
            else if (id == Code.RechargeDCNoteUser.GetHashCode()) return NTRoleType.RECHARGE_DCNOTE_USER;
            else if (id == Code.GeneralUser.GetHashCode()) return NTRoleType.GENERAL_USER;
            else if (id == Code.VendorAccountEditUser.GetHashCode()) return NTRoleType.VENDOR_ACCOUNT_EDIT_USER;
            else if (id == Code.SettlementMaintenance.GetHashCode()) return NTRoleType.SETTLEMENT_MAINTENANCE;
            else if (id == Code.SunInterface.GetHashCode()) return NTRoleType.SUN_INTERFACE;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(NTRoleType.FIRST_APPROVER);
            list.Add(NTRoleType.SECOND_APPROVER);
            list.Add(NTRoleType.MONTH_END_ADMIN);
            list.Add(NTRoleType.DEPARTMENT_APPROVER_ADMIN);
            list.Add(NTRoleType.VENDOR_APPROVER);
            list.Add(NTRoleType.RECHARGE_DCNOTE_USER);
            list.Add(NTRoleType.GENERAL_USER);
            list.Add(NTRoleType.VENDOR_ACCOUNT_EDIT_USER);
            list.Add(NTRoleType.SETTLEMENT_MAINTENANCE);
            list.Add(NTRoleType.SUN_INTERFACE);
            return list;
        }
    }
}
