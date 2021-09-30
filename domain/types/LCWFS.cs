using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;


namespace com.next.isam.domain.types
{
    [Serializable()]
    public class LCWFS : DomainData
    {
        public static LCWFS NEW = new LCWFS(Code.New);
        public static LCWFS APPROVED = new LCWFS(Code.Approved);
        public static LCWFS APPLIED = new LCWFS(Code.Applied);
        public static LCWFS COMPLETED = new LCWFS(Code.Completed);
        public static LCWFS REJECTED = new LCWFS(Code.Rejected);
        public static LCWFS LC_CANCELLED = new LCWFS(Code.LC_Cancelled);

        private Code _code;

        private enum Code
        {
            New = 1,
            Approved = 2,
            Applied = 3,
            Completed = 4,
            Rejected = 5,
            LC_Cancelled = 6
        }

        private LCWFS(Code code)
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
                        return "NEW";
                    case Code.Approved:
                        return "APPROVED";
                    case Code.Applied:
                        return "APPLIED";
                    case Code.Completed:
                        return "COMPLETED";
                    case Code.Rejected:
                        return "REJECTED";
                    case Code.LC_Cancelled:
                        return "LC CANCELLED";
                    default:
                        return "ERROR";
                }
            }
        }

        public static LCWFS getType(int id)
        {
            if (id == Code.New.GetHashCode()) return LCWFS.NEW;
            else if (id == Code.Approved.GetHashCode()) return LCWFS.APPROVED;
            else if (id == Code.Applied.GetHashCode()) return LCWFS.APPLIED;
            else if (id == Code.Approved.GetHashCode()) return LCWFS.APPROVED;
            else if (id == Code.Completed.GetHashCode()) return LCWFS.COMPLETED;
            else if (id == Code.Rejected.GetHashCode()) return LCWFS.REJECTED;
            else if (id == Code.LC_Cancelled.GetHashCode()) return LCWFS.LC_CANCELLED;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(LCWFS.NEW);
            list.Add(LCWFS.APPROVED);
            list.Add(LCWFS.APPLIED);
            list.Add(LCWFS.COMPLETED);
            list.Add(LCWFS.REJECTED);
            list.Add(LCWFS.LC_CANCELLED);
            return list;
        }
    }
}
