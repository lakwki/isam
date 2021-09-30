using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
	[Serializable()]
	public class ILSOrderCopyOriginDef : DomainData
	{
        private int orderRefId;
        private string orderRef;
        private int shipmentId;
		private string originContract;
		private string officeCode;
		private string portCode;
		private string requiredDocs;
		private string quotaCats;
        private string originCountry;

		public ILSOrderCopyOriginDef()
		{
		}

        public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

		public string OrderRef 
        { 
            get { return orderRef;	} 
            set {	orderRef = value; }	
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public string OriginContract 
        { 
            get { return originContract; } 
            set { originContract = value; } 
        }

		public string OfficeCode 
        { 
            get { return officeCode; } 
            set { officeCode = value; } 
        }

		public string PortCode 
        { 
            get { return portCode; } 
            set { portCode = value; } 
        }

		public string RequiredDocs 
        { 
            get { return requiredDocs; } 
            set { requiredDocs = value; } 
        }

		public string QuotaCats 
        { 
            get { return quotaCats; } 
            set { quotaCats = value; } 
        }

        public string OriginCountry
        {
            get { return originCountry; }
            set { originCountry = value; }
        }
	}
}
