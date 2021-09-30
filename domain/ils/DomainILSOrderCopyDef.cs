using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class DomainILSOrderCopyDef : DomainData
    {
        private ILSOrderCopyDef ordercopy;
        private ArrayList orderCopyDetails;

        public DomainILSOrderCopyDef()
        {
            ordercopy = new ILSOrderCopyDef();
            orderCopyDetails = new ArrayList();
        }

        public ILSOrderCopyDef OrderCopy 
        { 
            get { return ordercopy; } 
            set { ordercopy = value; } 
        }

        public ArrayList OrderCopyDetails 
        { 
            get { return orderCopyDetails; } 
            set { orderCopyDetails = value; } 
        } 
    }
}
