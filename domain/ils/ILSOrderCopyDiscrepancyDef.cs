using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
	[Serializable()]
	public class ILSOrderCopyDiscrepancyDef : DomainData
	{
		private int orderRefId;
        private string orderRef;
		private string optionNo;
        private int shipmentId;
		private string currencyCode;
		private string transportMode;
		private string ilsTransportMode;
		private decimal sellingPrice;
		private decimal ilsSellingPrice;
		private int ilsNextPercent;
		private int nextPercent;
		private int ilsSupplierPercent;
		private int supplierPercent;

		public ILSOrderCopyDiscrepancyDef()
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

		public string OptionNo 
        { 
            get { return optionNo; } 
            set { optionNo = value; } 
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public string CurrencyCode 
        { 
            get { return currencyCode; } 
            set { currencyCode = value; }
        }

		public string TransportMode 
        { 
            get { return transportMode; } 
            set { transportMode = value; } 
        }

		public string ILSTransportMode 
        { 
            get { return ilsTransportMode; } 
            set { ilsTransportMode = value; }
        }

		public decimal SellingPrice 
        { 
            get { return sellingPrice; } 
            set { sellingPrice = value; } 
        }

		public decimal ILSSellingPrice 
        { 
            get { return ilsSellingPrice; } 
            set { ilsSellingPrice = value; } 
        }

		public int ILSNextPercent 
        { 
            get { return ilsNextPercent; } 
            set { ilsNextPercent = value; } 
        }

		public int NextPercent 
        { 
            get { return nextPercent; } 
            set { nextPercent = value; } 
        }

		public int ILSSupplierPercent 
        { 
            get { return ilsSupplierPercent; } 
            set { ilsSupplierPercent = value; } 
        }

		public int SupplierPercent 
        { 
            get { return supplierPercent; } 
            set { supplierPercent = value; } 
        }
	}
}
