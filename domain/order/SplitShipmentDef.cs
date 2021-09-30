using System;
using com.next.common.domain;
using com.next.isam.domain.product;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.industry.vendor;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class SplitShipmentDef : DomainData
	{
		private int splitShipmentId;
		private int shipmentId;
		private string splitSuffix;
		private int productId;
        private ProductDef product;
		private int piecesPerPack;
		private PackingUnitRef packingUnit;
		private VendorRef vendor;
		private DateTime supplierAgreedAtWarehouseDate;
		private PaymentTermRef paymentTerm;
		private QuotaCategoryGroupRef quotaCategoryGroup;
		private CountryOfOriginRef countryOfOrigin;
		private CurrencyRef sellCurrency;
		private CurrencyRef buyCurrency;
        private decimal invoiceBuyExchangeRate;
		private decimal poExchangeRate;
		private string mockShopSampleRemark;
		private string otherPOTermRemark;
        private string shippingRemark;
        private string supplierInvoiceNo;
		private int totalOrderQuantity;
		private decimal totalOrderAmount;
        private decimal totalOrderAmountAfterDiscount;
		private int totalPOQuantity;
		private decimal totalPOAmount;
        private decimal totalPOAmountAfterDiscount;
		private decimal totalPONetFOBAmount;
        private decimal totalPONetFOBAmountAfterDiscount;
        private decimal totalPOSupplierGarmentAmount;
        private decimal totalPOSupplierGarmentAmountAfterDiscount;
        private decimal totalNetFOBAmount;
        private decimal totalNetFOBAmountAfterDiscount;
        private decimal totalSupplierGarmentAmount;
        private decimal totalSupplierGarmentAmountAfterDiscount;
        private decimal totalOPAUpcharge;
		private int totalShippedQuantity;
		private decimal totalShippedAmount;
        private decimal totalShippedAmountAfterDiscount;
		private decimal totalShippedNetFOBAmount;
        private decimal totalShippedNetFOBAmountAfterDiscount;
		private decimal totalShippedSupplierGarmentAmount;
        private decimal totalShippedSupplierGarmentAmountAfterDiscount;
		private decimal totalShippedOPAUpcharge;
		private decimal vendorPaymentDiscountPercent;
		private int isVirtualSetSplit;
		private int isKnitwearComponent;
		private int isFobOrder;
		private decimal qaCommissionPercent;
        private string colour;
        private decimal lcAmt;
        private string lcNo;
        private DateTime lcIssueDate;
        private DateTime lcExpiryDate;
        private int isLCPaymentChecked;
        private DateTime lcPaymentCheckedDate;
        private bool paymentLock;
        private DateTime shippingDocReceiptDate;
        private DateTime accountDocReceiptDate;
        private string apRefNo;
        private decimal apAmt;
        private DateTime apDate;
        private decimal apExchangeRate;
        private bool isILSQtyUploadAllowed;
        private ShippingDocWFS shippingDocWFS;
        private DateTime docReviewedOn;
        private int docReviewedBy;
        private int rejectPaymentReasonId;
        private DateTime shippingDocCheckedOn;
        private decimal shippingCheckedTotalNetAmount;
        private UserRef shippingDocCheckedBy;
        private decimal labTestIncome;

		public SplitShipmentDef()
		{
		}

		public int SplitShipmentId
		{
			get { return splitShipmentId; }
			set { splitShipmentId = value; }
		}

		public int ShipmentId
		{
			get { return shipmentId; }
			set { shipmentId = value; }
		}

		public string SplitSuffix
		{
			get { return splitSuffix; }
			set { splitSuffix = value; }
		}

		public int ProductId
		{
			get { return productId; }
			set { productId = value; }
		}

        public ProductDef Product
        {
            get { return product; }
            set { product = value; }
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

		public VendorRef Vendor
		{
			get { return vendor; }
			set { vendor = value; }
		}

    	public DateTime SupplierAgreedAtWarehouseDate
		{
			get { return supplierAgreedAtWarehouseDate; }
			set { supplierAgreedAtWarehouseDate = value; }
		}

		public PaymentTermRef PaymentTerm
		{
			get { return paymentTerm; }
			set { paymentTerm = value; }
		}

		public QuotaCategoryGroupRef QuotaCategoryGroup
		{
			get { return quotaCategoryGroup; }
			set { quotaCategoryGroup = value; }
		}

		public CountryOfOriginRef CountryOfOrigin
		{
			get { return countryOfOrigin; }
			set { countryOfOrigin = value; }
		}

		public CurrencyRef SellCurrency
		{
			get { return sellCurrency; }
			set { sellCurrency = value; }
		}

		public CurrencyRef BuyCurrency
		{
			get { return buyCurrency; }
			set { buyCurrency = value; }
		}

        public decimal InvoiceBuyExchangeRate
        {
            get { return invoiceBuyExchangeRate; }
            set { invoiceBuyExchangeRate = value; }
        }

        public decimal POExchangeRate
		{
			get { return poExchangeRate; }
			set { poExchangeRate = value; }
		}

		public string MockShopSampleRemark
		{
			get { return mockShopSampleRemark; }
			set { mockShopSampleRemark = value; }
		}

		public string OtherPOTermRemark
		{
			get { return otherPOTermRemark; }
			set { otherPOTermRemark = value; }
		}

		public string ShippingRemark
		{
			get { return shippingRemark; }
			set { shippingRemark = value; }
		}

        public string SupplierInvoiceNo
        {
            get { return supplierInvoiceNo; }
            set { supplierInvoiceNo = value; }
        }

        public int TotalOrderQuantity
		{
			get { return totalOrderQuantity; }
			set { totalOrderQuantity = value; }
		}

		public decimal TotalOrderAmount
		{
			get { return totalOrderAmount; }
			set { totalOrderAmount = value; }
		}

        public decimal TotalOrderAmountAfterDiscount
        {
            get { return totalOrderAmountAfterDiscount; }
            set { totalOrderAmountAfterDiscount = value; }
        }

        public int TotalPOQuantity
		{
			get { return totalPOQuantity; }
			set { totalPOQuantity = value; }
		}

		public decimal TotalPOAmount
		{
			get { return totalPOAmount; }
			set { totalPOAmount = value; }
		}

        public decimal TotalPOAmountAfterDiscount
        {
            get { return totalPOAmountAfterDiscount; }
            set { totalPOAmountAfterDiscount = value; }
        }

        public decimal TotalPONetFOBAmount
		{
			get { return totalPONetFOBAmount; }
			set { totalPONetFOBAmount = value; }
		}

        public decimal TotalPONetFOBAmountAfterDiscount
        {
            get { return totalPONetFOBAmountAfterDiscount; }
            set { totalPONetFOBAmountAfterDiscount = value; }
        }

        public decimal TotalPOSupplierGarmentAmount
        {
            get { return totalPOSupplierGarmentAmount; }
            set { totalPOSupplierGarmentAmount = value; }
        }

        public decimal TotalPOSupplierGarmentAmountAfterDiscount
        {
            get { return totalPOSupplierGarmentAmountAfterDiscount; }
            set { totalPOSupplierGarmentAmountAfterDiscount = value; }
        }

        public decimal TotalNetFOBAmount
        {
            get { return totalNetFOBAmount; }
            set { totalNetFOBAmount = value; }
        }

        public decimal TotalNetFOBAmountAfterDiscount
        {
            get { return totalNetFOBAmountAfterDiscount; }
            set { totalNetFOBAmountAfterDiscount = value; }
        }

        public decimal TotalSupplierGarmentAmount
        {
            get { return totalSupplierGarmentAmount; }
            set { totalSupplierGarmentAmount = value; }
        }

        public decimal TotalSupplierGarmentAmountAfterDiscount
        {
            get { return totalSupplierGarmentAmountAfterDiscount; }
            set { totalSupplierGarmentAmountAfterDiscount = value; }
        }

        public decimal TotalOPAUpcharge 
        { 
            get { return totalOPAUpcharge; } 
            set { totalOPAUpcharge = value; } 
        }

        public int TotalShippedQuantity
		{
			get { return totalShippedQuantity; }
			set { totalShippedQuantity = value; }
		}

		public decimal TotalShippedAmount
		{
			get { return totalShippedAmount; }
			set { totalShippedAmount = value; }
		}

        public decimal TotalShippedAmountAfterDiscount
        {
            get { return totalShippedAmountAfterDiscount; }
            set { totalShippedAmountAfterDiscount = value; }
        }

        public decimal TotalShippedNetFOBAmount
		{
			get { return totalShippedNetFOBAmount; }
			set { totalShippedNetFOBAmount = value; }
		}

        public decimal TotalShippedNetFOBAmountAfterDiscount
        {
            get { return totalShippedNetFOBAmountAfterDiscount; }
            set { totalShippedNetFOBAmountAfterDiscount = value; }
        }

        public decimal TotalShippedSupplierGarmentAmount
		{
			get { return totalShippedSupplierGarmentAmount; }
			set { totalShippedSupplierGarmentAmount = value; }
		}

        public decimal TotalShippedSupplierGarmentAmountAfterDiscount
        {
            get { return totalShippedSupplierGarmentAmountAfterDiscount; }
            set { totalShippedSupplierGarmentAmountAfterDiscount = value; }
        }

        public decimal TotalShippedOPAUpcharge 
        { 
            get { return totalShippedOPAUpcharge; } 
            set { totalShippedOPAUpcharge = value; } 
        }

		public decimal VendorPaymentDiscountPercent 
        { 
            get { return vendorPaymentDiscountPercent; } 
            set { vendorPaymentDiscountPercent = value; } 
        }

		public int IsVirtualSetSplit 
        { 
            get { return isVirtualSetSplit; } 
            set { isVirtualSetSplit = value; } 
        }

		public int IsKnitwearComponent 
        { 
            get { return isKnitwearComponent; } 
            set { isKnitwearComponent = value; } 
        }

		public int IsFobOrder 
        { 
            get { return isFobOrder; } 
            set { isFobOrder = value; } 
        }

		public decimal QACommissionPercent 
        { 
            get { return qaCommissionPercent; } 
            set { qaCommissionPercent = value; } 
        }

        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public decimal LCAmount
        {
            get { return lcAmt; }
            set { lcAmt = value; }
        }

        public string LCNo
        {
            get { return lcNo; }
            set { lcNo = value; }
        }

        public DateTime LCIssueDate
        {
            get { return lcIssueDate; }
            set { lcIssueDate = value; }
        }

        public DateTime LCExpiryDate
        {
            get { return lcExpiryDate; }
            set { lcExpiryDate = value; }
        }

        public int IsLCPaymentChecked
        {
            get { return isLCPaymentChecked; }
            set { isLCPaymentChecked = value; }
        }

        public DateTime LCPaymentCheckedDate
        {
            get { return lcPaymentCheckedDate; }
            set { lcPaymentCheckedDate = value; }
        }

        public bool PaymentLock
        {
            get { return paymentLock; }
            set { paymentLock = value; }
        }

        public DateTime ShippingDocReceiptDate
        {
            get { return shippingDocReceiptDate; }
            set { shippingDocReceiptDate = value; }
        }

        public DateTime AccountDocReceiptDate
        {
            get { return accountDocReceiptDate; }
            set { accountDocReceiptDate = value; }
        }

        public string APRefNo
        {
            get { return apRefNo; }
            set { apRefNo = value; }
        }

        public decimal APAmt
        {
            get { return apAmt; }
            set { apAmt = value; }
        }

        public DateTime APDate
        {
            get { return apDate; }
            set { apDate = value; }
        }

        public decimal APExchangeRate
        {
            get { return apExchangeRate; }
            set { apExchangeRate = value; }
        }

        public bool IsILSQtyUploadAllowed
        {
            get { return isILSQtyUploadAllowed; }
            set { isILSQtyUploadAllowed = value; }

        }

        public ShippingDocWFS ShippingDocWFS
        {
            get { return shippingDocWFS; }
            set { shippingDocWFS = value; }
        }

        public DateTime DocumentReviewedOn
        {
            get { return docReviewedOn; }
            set { docReviewedOn = value; }
        }

        public int DocumentReviewedBy
        {
            get { return docReviewedBy; }
            set { docReviewedBy = value; }
        }

        public int RejectPaymentReasonId
        {
            get { return rejectPaymentReasonId; }
            set { rejectPaymentReasonId = value; }
        }

        public DateTime ShippingDocCheckedOn
        {
            get { return shippingDocCheckedOn; }
            set { shippingDocCheckedOn = value; }
        }

        public UserRef ShippingDocCheckedBy
        {
            get { return shippingDocCheckedBy; }
            set { shippingDocCheckedBy = value; }
        }

        public decimal ShippingCheckedTotalNetAmount
        {
            get { return shippingCheckedTotalNetAmount; }
            set { shippingCheckedTotalNetAmount = value; }
        }

        public decimal LabTestIncome
        {
            get { return labTestIncome; }
            set { labTestIncome = value; }
        }

    }
}
