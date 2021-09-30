using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class AdvancePaymentWFS : DomainData
    {
        public static AdvancePaymentWFS DRAFT = new AdvancePaymentWFS(Code.Draft);
        public static AdvancePaymentWFS PENDING_FOR_APPROVAL = new AdvancePaymentWFS(Code.PendingForApproval);
        public static AdvancePaymentWFS DEPARTMENT_APPROVED = new AdvancePaymentWFS(Code.DepartmentApproved);
        public static AdvancePaymentWFS DEPARTMENT_REJECTED = new AdvancePaymentWFS(Code.DepartmentRejected);
        public static AdvancePaymentWFS ACCOUNTS_RECEIVED = new AdvancePaymentWFS(Code.AccountsReceived);
        public static AdvancePaymentWFS ACCOUNTS_REJECTED = new AdvancePaymentWFS(Code.AccountsRejected);
        public static AdvancePaymentWFS ACCOUNTS_APPROVED = new AdvancePaymentWFS(Code.AccountsApproved);
        public static AdvancePaymentWFS ACCOUNTS_EVALUATING = new AdvancePaymentWFS(Code.AccountsEvaluating);
        public static AdvancePaymentWFS SETTLED = new AdvancePaymentWFS(Code.Settled);
        public static AdvancePaymentWFS CANCELLED = new AdvancePaymentWFS(Code.Cancelled);

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

        private AdvancePaymentWFS(Code code)
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

        public static AdvancePaymentWFS getType(int id)
        {
            if (id == Code.Draft.GetHashCode()) return AdvancePaymentWFS.DRAFT;
            else if (id == Code.PendingForApproval.GetHashCode()) return AdvancePaymentWFS.PENDING_FOR_APPROVAL;
            else if (id == Code.DepartmentApproved.GetHashCode()) return AdvancePaymentWFS.DEPARTMENT_APPROVED;
            else if (id == Code.DepartmentRejected.GetHashCode()) return AdvancePaymentWFS.DEPARTMENT_REJECTED;
            else if (id == Code.AccountsReceived.GetHashCode()) return AdvancePaymentWFS.ACCOUNTS_RECEIVED;
            else if (id == Code.AccountsRejected.GetHashCode()) return AdvancePaymentWFS.ACCOUNTS_REJECTED;
            else if (id == Code.AccountsApproved.GetHashCode()) return AdvancePaymentWFS.ACCOUNTS_APPROVED;
            else if (id == Code.AccountsEvaluating.GetHashCode()) return AdvancePaymentWFS.ACCOUNTS_EVALUATING;
            else if (id == Code.Settled.GetHashCode()) return AdvancePaymentWFS.SETTLED;
            else if (id == Code.Cancelled.GetHashCode()) return AdvancePaymentWFS.CANCELLED;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(AdvancePaymentWFS.DRAFT);
            list.Add(AdvancePaymentWFS.PENDING_FOR_APPROVAL);
            list.Add(AdvancePaymentWFS.DEPARTMENT_APPROVED);
            list.Add(AdvancePaymentWFS.DEPARTMENT_REJECTED);
            list.Add(AdvancePaymentWFS.ACCOUNTS_RECEIVED);
            list.Add(AdvancePaymentWFS.ACCOUNTS_REJECTED);
            list.Add(AdvancePaymentWFS.ACCOUNTS_APPROVED);
            list.Add(AdvancePaymentWFS.ACCOUNTS_EVALUATING);
            list.Add(AdvancePaymentWFS.SETTLED);
            list.Add(AdvancePaymentWFS.CANCELLED);
            return list;
        }

    }
}
