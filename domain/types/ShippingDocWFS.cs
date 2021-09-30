using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;


namespace com.next.isam.domain.types
{
    [Serializable()]
    public class ShippingDocWFS : DomainData
    {
        public static ShippingDocWFS REJECTED = new ShippingDocWFS(Code.Rejected);
        public static ShippingDocWFS NOT_READY = new ShippingDocWFS(Code.NotReady);
        public static ShippingDocWFS READY = new ShippingDocWFS(Code.Ready);
        public static ShippingDocWFS ACCEPTED = new ShippingDocWFS(Code.Accepted);
        public static ShippingDocWFS REVIEWED = new ShippingDocWFS(Code.Reviewed);

        private Code _code;

        private enum Code
        {
            Rejected = 3,
            NotReady = 0,
            Ready = 1,
            Accepted = 2,
            Reviewed = 4
        }

        private ShippingDocWFS(Code code)
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
                    case Code.Rejected:
                        return "REJECTED";
                    case Code.NotReady:
                        return "NOT READY";
                    case Code.Ready:
                        return "READY";
                    case Code.Accepted:
                        return "ACCEPTED";
                    case Code.Reviewed:
                        return "REVIEWED";
                    default:
                        return "ERROR";
                }
            }
        }

        public static ShippingDocWFS getType(int id)
        {
            if (id == Code.Rejected.GetHashCode()) return ShippingDocWFS.REJECTED;
            else if (id == Code.NotReady.GetHashCode()) return ShippingDocWFS.NOT_READY;
            else if (id == Code.Ready.GetHashCode()) return ShippingDocWFS.READY;
            else if (id == Code.Accepted.GetHashCode()) return ShippingDocWFS.ACCEPTED;
            else if (id == Code.Reviewed.GetHashCode()) return ShippingDocWFS.REVIEWED;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(ShippingDocWFS.REJECTED);
            list.Add(ShippingDocWFS.NOT_READY);
            list.Add(ShippingDocWFS.READY);
            list.Add(ShippingDocWFS.ACCEPTED);
            list.Add(ShippingDocWFS.REVIEWED);
            return list;
        }
    }
}
