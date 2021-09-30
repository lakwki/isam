using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSOrderCopyDef : DomainData
    {
        private int orderRefId;
        private string contractNo;
        private string deliveryNo;
        private string itemNo;
        private string itemDesc;
        private string transportMode;
        private string countryOfOrigin;
        private string departCountry;
        private DateTime exFactoryDate;
        private DateTime inWarehouseDate;
        private string supplierCode;
        private string supplierName;
        private string hangBox;
        private string buyingTerms;
        private string finalDest;
        private string currency;
        private int nextFreightPercent;
        private int supplierFreightPercent;
        private string arrivalPort;
        private string franchisePartnerCode;
        private string refurb;
        private string fileNo;
        private DateTime importDate;
        private string lastSentLoadingPort;
        private string lastSentOfficeCode;
        private string lastSentQuota;
        private string lastSentDocType;
        private string lastSentOriginCountry;
        private bool isValid;

        public ILSOrderCopyDef()
		{

		}

        public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

		public string ContractNo 
        { 
            get { return contractNo; } 
            set { contractNo = value; } 
        }

        public string DeliveryNo 
        { 
            get { return deliveryNo; } 
            set { deliveryNo = value; } 
        }

		public string ItemNo 
        { 
            get { return itemNo; } 
            set { itemNo = value; } 
        }

        public string ItemDesc 
        { 
            get { return itemDesc; } 
            set { itemDesc = value; } 
        }

        public string TransportMode 
        { 
            get { return transportMode; } 
            set { transportMode = value; } 
        }

        public string CountryOfOrigin 
        { 
            get { return countryOfOrigin; } 
            set { countryOfOrigin = value; } 
        }

        public string DepartCountry 
        { 
            get { return departCountry; } 
            set { departCountry = value; } 
        }

        public DateTime ExFactoryDate 
        { 
            get { return exFactoryDate; } 
            set { exFactoryDate = value; } 
        }

        public DateTime InWarehouseDate 
        { 
            get { return inWarehouseDate; } 
            set { inWarehouseDate = value; } 
        }

        public string SupplierCode 
        { 
            get { return supplierCode; } 
            set { supplierCode = value; } 
        }

        public string SupplierName 
        { 
            get { return supplierName; } 
            set { supplierName = value; } 
        }

        public string HangBox 
        { 
            get { return hangBox; } 
            set { hangBox = value; } 
        }

        public string BuyingTerms 
        { 
            get { return buyingTerms; } 
            set { buyingTerms = value; } 
        }

        public string FinalDestination 
        { 
            get { return finalDest; } 
            set { finalDest = value; } 
        }

        public string Currency 
        { 
            get { return currency; } 
            set { currency = value; } 
        }

        public int NextFreightPercent 
        { 
            get { return nextFreightPercent; } 
            set { nextFreightPercent = value; } 
        }

        public int SupplierFreightPercent 
        { 
            get { return supplierFreightPercent; } 
            set { supplierFreightPercent = value; } 
        }

        public string ArrivalPort 
        { 
            get { return arrivalPort; } 
            set { arrivalPort = value; } 
        }

        public string FranchisePartnerCode 
        { 
            get { return franchisePartnerCode; } 
            set { franchisePartnerCode = value; } 
        }

        public string Refurb 
        { 
            get { return refurb; } 
            set { refurb = value; } 
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

        public string LastSentLoadingPort 
        { 
            get { return lastSentLoadingPort; } 
            set { lastSentLoadingPort = value; } 
        }

        public string LastSentOfficeCode 
        { 
            get { return lastSentOfficeCode; } 
            set { lastSentOfficeCode = value; } 
        }

        public string LastSentQuota 
        { 
            get { return lastSentQuota; } 
            set { lastSentQuota = value; } 
        }

        public string LastSentDocType 
        { 
            get { return lastSentDocType; } 
            set { lastSentDocType = value; } 
        }

        public string LastSentOriginCountry
        {
            get { return lastSentOriginCountry; }
            set { lastSentOriginCountry = value; }
        }

        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }
    }
}
