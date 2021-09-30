using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class DomainILSPackingListDef : DomainData
    {
        private ILSPackingListDef packingList;
        private ArrayList packingListDetails;
        private ArrayList packingListCartonDetails;
        private ArrayList packingListMixedCartonDetails;
        private int nslDeliveryNo;

        public DomainILSPackingListDef()
        {
            packingList = new ILSPackingListDef();
            packingListDetails = new ArrayList();
            packingListCartonDetails = new ArrayList();
            packingListMixedCartonDetails = new ArrayList();
        }

        public ILSPackingListDef PackingList 
        { 
            get { return packingList; } 
            set { packingList = value; } 
        }

        public ArrayList PackingListDetails 
        { 
            get { return packingListDetails; } 
            set { packingListDetails = value; } 
        }

        public ArrayList PackingListCartonDetails 
        { 
            get { return packingListCartonDetails; } 
            set { packingListCartonDetails = value; } 
        }

        public ArrayList PackingListMixedCartonDetails
        {
            get { return packingListMixedCartonDetails; }
            set { packingListMixedCartonDetails = value; }
        }

        public int NSLDeliveryNo 
        { 
            get { return nslDeliveryNo; } 
            set { nslDeliveryNo = value; } 
        }
    }
}
