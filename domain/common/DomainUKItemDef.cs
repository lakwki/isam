using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class DomainUKItemDef : DomainData
    {
        private UKItemRef ukItem;
        private ArrayList ukItemPartDetails;

        public DomainUKItemDef()
        {
            ukItem = new UKItemRef();
            ukItemPartDetails = new ArrayList();
        }

        public UKItemRef UKItem 
        {
            get { return ukItem; }
            set { ukItem = value; } 
        }

        public ArrayList UKItemPartDetails 
        {
            get { return ukItemPartDetails; }
            set { ukItemPartDetails = value; } 
        } 
    }
}
