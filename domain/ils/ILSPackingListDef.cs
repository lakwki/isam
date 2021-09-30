using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSPackingListDef : DomainData
    {
        private int orderRefId;
        private string contractNo;
        private string deliveryNo;
        private string itemNo;
        private string itemDesc;
        private string transportMode;
        private string countryOfOrigin;
        private string departPort;
        private DateTime exFactoryDate;
        private DateTime inWarehouseDate;
        private string supplierCode;
        private string supplierName;
        private string hangBox;
        private string buyingTerms;
        private string finalDest;
        private string prepaidFreightCost;
        private DateTime handoverDate;
        private string vendorLoaded;
        private string arrivalPort;
        private string franchisePartnerCode;
        private string refurb;
        private int totalPieces;
        private int totalCartons;
        private decimal totalGrossWeight;
        private decimal totalNetWeight;
        private decimal totalVolume;
        private string fileNo;
        private DateTime importDate;
        private bool isUploaded;
        private int nslDeliveryNo;

        public ILSPackingListDef()
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

        public string DepartPort 
        { 
            get { return departPort; } 
            set { departPort = value; } 
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

        public string PrepaidFreightCost 
        { 
            get { return prepaidFreightCost; } 
            set { prepaidFreightCost = value; } 
        }

        public DateTime HandoverDate 
        { 
            get { return handoverDate; } 
            set { handoverDate = value; } 
        }

        public string VendorLoaded 
        { 
            get { return vendorLoaded; } 
            set { vendorLoaded = value; } 
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

        public int TotalPieces 
        { 
            get { return totalPieces; } 
            set { totalPieces = value; } 
        }

        public int TotalCartons 
        { 
            get { return totalCartons; } 
            set { totalCartons = value; } 
        }

        public decimal TotalGrossWeight 
        { 
            get { return totalGrossWeight; } 
            set { totalGrossWeight = value; } 
        }

        public decimal TotalNetWeight 
        { 
            get { return totalNetWeight; } 
            set { totalNetWeight = value; } 
        }

        public decimal TotalVolume 
        { 
            get { return totalVolume; } 
            set { totalVolume = value; } 
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

        public bool IsUploaded
        {
            get { return isUploaded; }
            set { isUploaded = value; }
        }

        public int NSLDeliveryNo
        {
            get { return nslDeliveryNo; }
            set { nslDeliveryNo = value; }
        }
    }
}
