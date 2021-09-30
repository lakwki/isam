using System;
using com.next.common.domain;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class ManifestDef : DomainData  
    {
        private DateTime ctsDate;
        private DateTime departureDate;
        private string voyageNo;
        private string containerNo;
        private string partnerContainerNo;
        private string vesselName;
        private string awbNo;

        public DateTime ConfirmedToShipDate
        {
            get { return ctsDate; }
            set { ctsDate = value; }
        }

        public DateTime DepartureDate
        {
            get { return departureDate; }
            set { departureDate = value; }
        }

        public string VoyageNo
        {
            get { return voyageNo; }
            set { voyageNo = value; }
        }

        public string ContainerNo
        {
            get { return containerNo; }
            set { containerNo = value; }
        }

        public string CarrierContainerNo
        {
            get { return partnerContainerNo; }
            set { partnerContainerNo = value; }
        }

        public string VesselName
        {
            get { return vesselName; }
            set { vesselName = value; }
        }

        public string AirwayBillNo
        {
            get { return awbNo; }
            set { awbNo = value; }
        }

    }
}
