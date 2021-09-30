using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class NTWFS : DomainData
    {
        public static NTWFS DRAFT = new NTWFS(Code.Draft);
        public static NTWFS PENDING_FOR_APPROVAL = new NTWFS(Code.PendingForApproval);
        public static NTWFS DEPARTMENT_APPROVED = new NTWFS(Code.DepartmentApproved);
        public static NTWFS DEPARTMENT_REJECTED = new NTWFS(Code.DepartmentRejected);
        public static NTWFS ACCOUNTS_RECEIVED = new NTWFS(Code.AccountsReceived);
        public static NTWFS ACCOUNTS_REJECTED = new NTWFS(Code.AccountsRejected);
        public static NTWFS ACCOUNTS_APPROVED = new NTWFS(Code.AccountsApproved);
        public static NTWFS ACCOUNTS_EVALUATING = new NTWFS(Code.AccountsEvaluating);
        public static NTWFS SETTLED = new NTWFS(Code.Settled);
        public static NTWFS CANCELLED = new NTWFS(Code.Cancelled);

        private Code _code;

        private enum Code
        {
            Draft = 0,
            PendingForApproval = 1,
            DepartmentApproved = 2,
            DepartmentRejected = 3,
            AccountsReceived = 4,
            AccountsRejected = 5,
            AccountsApproved = 6,
            AccountsEvaluating = 7,
            Settled = 8,
            Cancelled = 9
        }

        private NTWFS(Code code)
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
                    case Code.Draft:
                        return "DRAFT";
                    case Code.PendingForApproval:
                        return "PENDING FOR APPROVAL";
                    case Code.DepartmentApproved:
                        return "DEPARTMENT APPROVED";
                    case Code.DepartmentRejected:
                        return "DEPARTMENT REJECTED";
                    case Code.AccountsReceived:
                        return "ACCOUNTS RECEIVED";
                    case Code.AccountsRejected:
                        return "ACCOUNTS REJECTED";
                    case Code.AccountsApproved:
                        return "ACCOUNTS APPROVED";
                    case Code.AccountsEvaluating:
                        return "ACCOUNTS EVALUATING";
                    case Code.Settled:
                        return "SETTLED";
                    case Code.Cancelled:
                        return "CANCELLED";
                    default:
                        return "UNDEFINED";
                }
            }
        }

        public static NTWFS getType(int id)
        {
            if (id == Code.Draft.GetHashCode()) return NTWFS.DRAFT;
            else if (id == Code.PendingForApproval.GetHashCode()) return NTWFS.PENDING_FOR_APPROVAL;
            else if (id == Code.DepartmentApproved.GetHashCode()) return NTWFS.DEPARTMENT_APPROVED;
            else if (id == Code.DepartmentRejected.GetHashCode()) return NTWFS.DEPARTMENT_REJECTED;
            else if (id == Code.AccountsReceived.GetHashCode()) return NTWFS.ACCOUNTS_RECEIVED;
            else if (id == Code.AccountsRejected.GetHashCode()) return NTWFS.ACCOUNTS_REJECTED;
            else if (id == Code.AccountsApproved.GetHashCode()) return NTWFS.ACCOUNTS_APPROVED;
            else if (id == Code.AccountsEvaluating.GetHashCode()) return NTWFS.ACCOUNTS_EVALUATING;
            else if (id == Code.Settled.GetHashCode()) return NTWFS.SETTLED;
            else if (id == Code.Cancelled.GetHashCode()) return NTWFS.CANCELLED;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(NTWFS.DRAFT);
            list.Add(NTWFS.PENDING_FOR_APPROVAL);
            list.Add(NTWFS.DEPARTMENT_APPROVED);
            list.Add(NTWFS.DEPARTMENT_REJECTED);
            list.Add(NTWFS.ACCOUNTS_RECEIVED);
            list.Add(NTWFS.ACCOUNTS_REJECTED);
            list.Add(NTWFS.ACCOUNTS_APPROVED);
            list.Add(NTWFS.ACCOUNTS_EVALUATING);
            list.Add(NTWFS.SETTLED);
            list.Add(NTWFS.CANCELLED);
            return list;
        }
    }
}
