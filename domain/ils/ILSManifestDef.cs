using System;
using System.Collections;
using System.Web.UI.WebControls;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSManifestDef : DomainData
    {
        private string containerNo;
        private int legId;
        private string voyageNo;
        private string vesselName;
        private string partnerContainerNo;
        private string transportMode;
        private string departPort;
        private DateTime departDate;
        private string arrivalPort;
        private DateTime arrivalDate;
        private bool isTranshipment;
        private int totalContracts;
        private decimal totalVolume;
        private int totalPieces;
        private int totalCartons;
        private string fileNo;
        private DateTime importDate;

        public ILSManifestDef()
		{

		}

		public string ContainerNo 
        { 
            get { return containerNo; } 
            set { containerNo = value; } 
        }

        public int LegId 
        { 
            get { return legId; } 
            set { legId = value; } 
        }

		public string VoyageNo 
        { 
            get { return voyageNo; } 
            set { voyageNo = value; } 
        }

		public string VesselName 
        { 
            get { return vesselName; } 
            set { vesselName = value; } 
        }

        public string PartnerContainerNo 
        { 
            get { return partnerContainerNo; } 
            set { partnerContainerNo = value; } 
        }

        public string TransportMode 
        { 
            get { return transportMode; } 
            set { transportMode = value; } 
        }

        public string DepartPort 
        { 
            get { return departPort; } 
            set { departPort = value; } 
        }

        public DateTime DepartDate 
        { 
            get { return departDate; } 
            set { departDate = value; } 
        }

        public string ArrivalPort 
        { 
            get { return arrivalPort; } 
            set { arrivalPort = value; } 
        }

        public DateTime ArrivalDate 
        { 
            get { return arrivalDate; } 
            set { arrivalDate = value; } 
        }

        public bool IsTranshipment 
        { 
            get { return isTranshipment; } 
            set { isTranshipment = value; } 
        }

        public int TotalContracts 
        { 
            get { return totalContracts; } 
            set { totalContracts = value; } 
        }

        public decimal TotalVolume 
        { 
            get { return totalVolume; } 
            set { totalVolume = value; } 
        }

        public int TotalPieces 
        { 
            get { return totalPieces; } 
            set { totalPieces = value; } 
        }

        public int TotalCartons 
        { 
            get { return totalContracts; } 
            set { totalContracts = value; } 
        }

        public string FileNo 
        { 
            get { return fileNo; } 
            set { fileNo = value; } 
        }

        public DateTime ImportDate 
        { 
            get { return importDate; } 
            set { importDate = value; } 
        }

        public class ILSManifestComparer : IComparer
        {
            public enum CompareType
            {
                ContainerNo = 1,
                DepartDate = 2,
                VoyageNo = 3                
            }

            private CompareType compareType;
            private SortDirection direction;

            public ILSManifestComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(object x, object y)
            {
                ILSManifestDef manifestX = (ILSManifestDef)x;
                ILSManifestDef manifestY = (ILSManifestDef)y;

                if (compareType.GetHashCode() == CompareType.ContainerNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return manifestX.ContainerNo.CompareTo(manifestY.ContainerNo);
                    else
                        return manifestY.ContainerNo.CompareTo(manifestX.ContainerNo);
                }
                else if (compareType.GetHashCode() == CompareType.VoyageNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return manifestX.VoyageNo.CompareTo(manifestY.VoyageNo);
                    else
                        return manifestY.VoyageNo.CompareTo(manifestX.VoyageNo);
                }                                
                else if (compareType.GetHashCode() == CompareType.DepartDate.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return manifestX.DepartDate.CompareTo(manifestY.DepartDate);
                    else
                        return manifestY.DepartDate.CompareTo(manifestX.DepartDate);
                }
                return 0;
            }
        }

    }
}
