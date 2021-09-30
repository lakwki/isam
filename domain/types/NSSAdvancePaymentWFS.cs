using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class NSSAdvancePaymentWFS : DomainData
    {
        public static NSSAdvancePaymentWFS DRAFT = new NSSAdvancePaymentWFS(Code.Draft);
        public static NSSAdvancePaymentWFS PENDING_FOR_APPROVAL = new NSSAdvancePaymentWFS(Code.PendingForApproval);
        public static NSSAdvancePaymentWFS APPROVED = new NSSAdvancePaymentWFS(Code.Approved);
        public static NSSAdvancePaymentWFS REJECTED = new NSSAdvancePaymentWFS(Code.Rejected);    
        private Code _code;

        private enum Code
        {
            Draft = 0,
            PendingForApproval = 1,
            Approved = 2,
            Rejected = 3
        }

        private NSSAdvancePaymentWFS(Code code)
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
                    case Code.Approved:
                        return "APPROVED";
                    case Code.Rejected:
                        return "REJECTED";
                    default:
                        return "UNDEFINED";
                }
            }
        }

        public string Prefix
        {
            get
            {
                switch (_code)
                {
                    case Code.Draft:
                        return "D";
                    case Code.PendingForApproval:
                        return "P";
                    case Code.Approved:
                        return "A";
                    case Code.Rejected:
                        return "R";
                    default:
                        return "U";
                }
            }
        }

        public static NSSAdvancePaymentWFS getType(int id)
        {
            if (id == Code.Draft.GetHashCode()) return NSSAdvancePaymentWFS.DRAFT;
            else if (id == Code.PendingForApproval.GetHashCode()) return NSSAdvancePaymentWFS.PENDING_FOR_APPROVAL;
            else if (id == Code.Approved.GetHashCode()) return NSSAdvancePaymentWFS.APPROVED;
            else if (id == Code.Rejected.GetHashCode()) return NSSAdvancePaymentWFS.REJECTED;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(NSSAdvancePaymentWFS.DRAFT);
            list.Add(NSSAdvancePaymentWFS.PENDING_FOR_APPROVAL);
            list.Add(NSSAdvancePaymentWFS.APPROVED);
            list.Add(NSSAdvancePaymentWFS.REJECTED);
            return list;
        }

    }
}
