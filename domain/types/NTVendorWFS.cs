using System;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class NTVendorWFS : DomainData 
    {

        public static NTVendorWFS DRAFT = new NTVendorWFS(Code.Draft);
        public static NTVendorWFS APPROVED = new NTVendorWFS(Code.Approved);
        public static NTVendorWFS PENDING = new NTVendorWFS(Code.Pending);
        public static NTVendorWFS REJECTED = new NTVendorWFS(Code.Rejected);
        public static NTVendorWFS CANCELLED = new NTVendorWFS(Code.Cancelled);

        private Code _code;

        private enum Code
        {
            Draft = 0,
            Approved = 1,
            Pending = 2,
            Rejected = 3,
            Cancelled = 4
        }

        private NTVendorWFS(Code code)
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
                    case Code.Draft :
                        return "DRAFT";                    
                    case Code.Approved :
                        return "APPROVED";
                    case Code.Pending:
                        return "PENDING";
                    case Code.Rejected :
                        return "REJECTED";
                    case Code.Cancelled:
                        return "CANCELLED";
                    default :
                        return "UNDEFINED";
                }
            }
        }


        public static NTVendorWFS getType(int id)
        {
            if (id == Code.Draft.GetHashCode()) return NTVendorWFS.DRAFT;
            else if (id == Code.Approved.GetHashCode()) return NTVendorWFS.APPROVED;
            else if (id == Code.Pending.GetHashCode()) return NTVendorWFS.PENDING;
            else if (id == Code.Rejected.GetHashCode()) return NTVendorWFS.REJECTED;
            else if (id == Code.Cancelled.GetHashCode()) return NTVendorWFS.CANCELLED;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(NTVendorWFS.DRAFT);
            list.Add(NTVendorWFS.PENDING);
            list.Add(NTVendorWFS.APPROVED);
            list.Add(NTVendorWFS.REJECTED);
            list.Add(NTVendorWFS.CANCELLED);
            return list;
        }
    }
}
