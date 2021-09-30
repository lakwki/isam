using System;
using System.Collections;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.types;
using System.Web.UI.WebControls;
using com.next.common.appserver;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NTUserRoleAccessDef : DomainData
    {
        public NTUserRoleAccessDef()
        {
        }

        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public int OfficeId { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }

        public class NTUserRoleAccessDefComparer : IComparer
        {
            public enum CompareType
            {
                User = 1,
                Office = 2,
                Company = 3
            }

            private CompareType compareType;
            private SortDirection direction;

            public NTUserRoleAccessDefComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(object x, object y)
            {
                NTUserRoleAccessDef defX = (NTUserRoleAccessDef)x;
                NTUserRoleAccessDef defY = (NTUserRoleAccessDef)y;

                if (compareType.GetHashCode() == CompareType.Office.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return (defX.OfficeId == -1 ? "ALL" : GeneralManager.Instance.getOfficeRefByKey(defX.OfficeId).OfficeCode).CompareTo((defY.OfficeId == -1 ? "ALL" : GeneralManager.Instance.getOfficeRefByKey(defY.OfficeId).OfficeCode));
                    else
                        return (defX.OfficeId == -1 ? "ALL" : GeneralManager.Instance.getOfficeRefByKey(defY.OfficeId).OfficeCode).CompareTo((defY.OfficeId == -1 ? "ALL" : GeneralManager.Instance.getOfficeRefByKey(defX.OfficeId).OfficeCode));
                }
                else if (compareType.GetHashCode() == CompareType.User.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return GeneralManager.Instance.getUserByKey(defX.UserId).DisplayName.CompareTo(GeneralManager.Instance.getUserByKey(defY.UserId).DisplayName);
                    else
                        return GeneralManager.Instance.getUserByKey(defY.UserId).DisplayName.CompareTo(GeneralManager.Instance.getUserByKey(defX.UserId).DisplayName);
                }
                else if (compareType.GetHashCode() == CompareType.Company.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return (defX.CompanyId == -1 ? "ALL" : GeneralManager.Instance.getCompanyByKey(defX.CompanyId).CompanyName).CompareTo((defY.CompanyId == -1 ? "ALL" : GeneralManager.Instance.getCompanyByKey(defY.CompanyId).CompanyName));
                    else
                        return (defX.CompanyId == -1 ? "ALL" : GeneralManager.Instance.getCompanyByKey(defY.CompanyId).CompanyName).CompareTo((defY.CompanyId == -1 ? "ALL" : GeneralManager.Instance.getCompanyByKey(defX.CompanyId).CompanyName));
                }
                return 0;
            }
        }

    }
}


