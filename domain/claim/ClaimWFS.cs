using System;
using System.Collections.Generic;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class ClaimWFS : DomainData
    {
        public static ClaimWFS NEW = new ClaimWFS(Code.New);
        public static ClaimWFS SUBMITTED = new ClaimWFS(Code.Submitted);
        public static ClaimWFS REJECTED = new ClaimWFS(Code.Rejected);
        public static ClaimWFS USER_SIGNED_OFF = new ClaimWFS(Code.UserSignedOff);
        public static ClaimWFS COO_SIGNED_OFF = new ClaimWFS(Code.COOSignedOff);
        public static ClaimWFS DEBIT_NOTE_TO_SUPPLIER = new ClaimWFS(Code.DebitNoteToSupplier);
        public static ClaimWFS CANCELLED = new ClaimWFS(Code.Cancelled);

        private Code _code;

        private enum Code
        {
            New = 0,
            Submitted = 1,
            Rejected = 2,
            UserSignedOff = 3,
            COOSignedOff = 4,
            DebitNoteToSupplier = 5,
            Cancelled = 9
        }

        public ClaimWFS() { }

        private ClaimWFS(Code code)
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
                    case Code.New:
                        return "D/N RECEIVED";
                    case Code.Rejected:
                        return "REJECTED";
                    case Code.Submitted:
                        return "TO-BE-INVOICED";
                    case Code.UserSignedOff:
                        return "GM_SIGNED_OFF";
                    case Code.COOSignedOff:
                        return "ACCOUNTS_SIGNED_OFF";
                    case Code.DebitNoteToSupplier:
                        return "INVOICED";
                    case Code.Cancelled:
                        return "CANCELLED";
                    default:
                        return "ERROR";
                }
            }
        }

        public static ClaimWFS getType(int id)
        {
            if (id == Code.New.GetHashCode()) return ClaimWFS.NEW;
            else if (id == Code.Rejected.GetHashCode()) return ClaimWFS.REJECTED;
            else if (id == Code.Submitted.GetHashCode()) return ClaimWFS.SUBMITTED;
            else if (id == Code.UserSignedOff.GetHashCode()) return ClaimWFS.USER_SIGNED_OFF;
            else if (id == Code.COOSignedOff.GetHashCode()) return ClaimWFS.COO_SIGNED_OFF;
            else if (id == Code.DebitNoteToSupplier.GetHashCode()) return ClaimWFS.DEBIT_NOTE_TO_SUPPLIER;
            else if (id == Code.Cancelled.GetHashCode()) return ClaimWFS.CANCELLED;
            else return null;
        }

        public static List<ClaimWFS> getCollectionValues()
        {
            List<ClaimWFS> list = new List<ClaimWFS>();
            list.Add(ClaimWFS.REJECTED);
            list.Add(ClaimWFS.NEW);
            list.Add(ClaimWFS.SUBMITTED);
            list.Add(ClaimWFS.USER_SIGNED_OFF);
            list.Add(ClaimWFS.COO_SIGNED_OFF);
            list.Add(ClaimWFS.DEBIT_NOTE_TO_SUPPLIER);
            list.Add(ClaimWFS.CANCELLED);
            return list;
        }
    }
}
