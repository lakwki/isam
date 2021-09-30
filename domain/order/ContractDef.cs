using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class ContractDef : DomainData
	{
		private int contractId;
		private string contractNo;
		private string nslPONo;
		private int tradingAgencyId;
		private int supplierAssignTypeId;
		private int productId;
        private int deptId;
		private SeasonRef season;
		private OfficeRef office;
		private UserRef merchandiser;
		private ProductCodeRef productTeam;
		private int phaseId;
		private CustomerDef customer;
		private string bookingRefNo;
		private DateTime bookingReceivedDate;
		private int piecesPerPack;
		private PackingUnitRef packingUnit;
		private PackingMethodRef packingMethod;
		private int setSplitCount;
        private string ukSupplierCode;
		private int isVirtualSetSplit;
		private int isNextMfgOrder;
        private int isPOIssueToNextMfg;
		private int isDualSourcingOrder;
        private int isLDPOrder;
        private int isBizOrder;
        private int isShortGame;
        private string ukProductGroupCode;
        private int isEnvSustainable;

		public ContractDef()
		{
		}

		public int ContractId 
        { 
            get { return contractId; } 
            set { contractId = value; } 
        }
		
        public string ContractNo 
        { 
            get { return contractNo; } 
            set { contractNo = value; } 
        }

		public string NSLPONo 
        { 
            get { return nslPONo; } 
            set { nslPONo = value; } 
        }

		public int TradingAgencyId 
        { 
            get { return tradingAgencyId; } 
            set { tradingAgencyId = value; } 
        }

		public int SupplierAssignTypeId 
        { 
            get { return supplierAssignTypeId; } 
            set { supplierAssignTypeId = value; } 
        }

		public int ProductId 
        { 
            get { return productId; } 
            set { productId = value; } 
        }

        public int DeptId
        {
            get { return deptId; }
            set { deptId = value; }
        }

		public SeasonRef Season 
        { 
            get { return season; } 
            set { season = value; } 
        }

		public OfficeRef Office 
        { 
            get { return office; } 
            set { office = value; } 
        }

		public UserRef Merchandiser 
        { 
            get { return merchandiser; } 
            set { merchandiser = value; } 
        }

		public ProductCodeRef ProductTeam 
        { 
            get { return productTeam; } 
            set { productTeam = value; } 
        }

		public CustomerDef Customer 
        { 
            get { return customer; } 
            set { customer = value; } 
        }

		public int PhaseId 
        { 
            get { return phaseId; } 
            set { phaseId = value; } 
        }

		public string BookingRefNo 
        { 
            get { return bookingRefNo; } 
            set { bookingRefNo = value; } 
        }

		public DateTime BookingReceivedDate 
        { 
            get { return bookingReceivedDate; } 
            set { bookingReceivedDate = value; } 
        }

		public int PiecesPerPack 
        { 
            get { return piecesPerPack; } 
            set { piecesPerPack = value; } 
        }

		public PackingUnitRef PackingUnit 
        { 
            get { return packingUnit; } 
            set { packingUnit = value; } 
        }

		public PackingMethodRef PackingMethod 
        { 
            get { return packingMethod; } 
            set { packingMethod = value; } 
        }

		public int SetSplitCount 
        { 
            get { return setSplitCount; } 
            set { setSplitCount = value; } 
        }

        public string UKSupplierCode 
        { 
            get { return ukSupplierCode; } 
            set { ukSupplierCode = value; } 
        }

		public int IsVirtualSetSplit 
        { 
            get { return isVirtualSetSplit; } 
            set { isVirtualSetSplit = value; } 
        }

		public int IsNextMfgOrder 
        { 
            get { return isNextMfgOrder; } 
            set { isNextMfgOrder = value; } 
        }

        public int IsPOIssueToNextMfg 
        { 
            get { return isPOIssueToNextMfg; } 
            set { isPOIssueToNextMfg = value; } 
        }

		public int IsDualSourcingOrder 
        { 
            get { return isDualSourcingOrder; } 
            set { isDualSourcingOrder = value; } 
        }

        public int IsLDPOrder
        {
            get { return isLDPOrder; }
            set { isLDPOrder = value; }
        }

        public int IsBizOrder
        {
            get { return isBizOrder; }
            set { isBizOrder = value; }
        }

        public int IsShortGame
        {
            get { return isShortGame; }
            set { isShortGame = value; }
        }

        public string UKProductGroupCode
        {
            get { return ukProductGroupCode; }
            set { ukProductGroupCode = value; }
        }

        public int IsEnvSustainable
        {
            get { return isEnvSustainable; }
            set { isEnvSustainable = value; }
        }
	}
}
