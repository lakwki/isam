using System;
using com.next.common.domain;
using com.next.isam.domain.product;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.industry.vendor;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class ShipmentDef : DomainData
    {
        private int shipmentId;
        private int contractId;
        private int deliveryNo;
        private string nslPONo;
        private VendorRef vendor;
        private VendorRef vmVendor;
        private int factoryId;
        private int supplierAssignTypeId;
        private TermOfPurchaseRef termOfPurchase;
        private TermOfPurchaseRef originalTermOfPurchase;
        private PurchaseLocationRef purchaseLocation;
        private DateTime ukAtWarehouseDate;
        private DateTime customerAgreedAtWarehouseDate;
        private DateTime supplierAgreedAtWarehouseDate;
        private ShipmentMethodRef shipmentMethod;
        private AirFreightPaymentTypeRef airFreightPaymentType;
        private decimal nukAirFreightPaymentPercent;
        private decimal nslAirFreightPaymentPercent;
        private decimal ftyAirFreightPaymentPercent;
        private decimal nslszAirFreightPaymentPercent;
        private decimal otherAirFreightPaymentPercent;
        private string otherAirFreightPaymentRemark;
        private string airFreightPaymentRemark;
        private decimal agencyCommissionPercentage;
        private PaymentTermRef paymentTerm;
        private QuotaCategoryGroupRef quotaCategoryGroup;
        private CountryOfOriginRef countryOfOrigin;
        private ShipmentCountryRef shipmentCountry;
        private ShipmentPortRef shipmentPort;
        private CurrencyRef sellCurrency;
        private CurrencyRef buyCurrency;
        private decimal poExchangeRate;
        private decimal usExchangeRate;
        private decimal vatPercent;
        private decimal nslCommissionPercentage;
        private string mockShopSampleRemark;
        private string notesFromMerchandiser;
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
        private decimal totalOtherCost;
        private decimal totalOrderFreightCost;
        private decimal totalOrderDutyCost;
        private decimal totalOPAUpcharge;
        private int totalShippedQuantity;
        private decimal totalShippedAmount;
        private decimal totalShippedAmountAfterDiscount;
        private decimal totalShippedNetFOBAmount;
        private decimal totalShippedNetFOBAmountAfterDiscount;
        private decimal totalShippedSupplierGarmentAmount;
        private decimal totalShippedSupplierGarmentAmountAfterDiscount;
        private decimal totalShippedOtherCost;
        private decimal totalShippedFreightCost;
        private decimal totalShippedDutyCost;
        private decimal totalShippedOPAUpcharge;
        private int splitCount;
        private int isRepeatOrder;
        private int delayReasonTypes;
        private string delayReasonOther;
        private ContractWFS workflowStatus;
        private int specialOrderTypeId;
        private int isMockShopSample;
        private int isPressSample;
        private int isStudioSample;
        private int salesForecastSpecialGroupId;
        private decimal vendorPaymentDiscountPercent;
        private int customDocType;
        private int isVirtualSetSplit;
        private int thirdPartyAgencyId;
        private int isRatioPackOrder;
        private int isUKDiscount;
        private int ukDiscountReasonId;
        private int withOPRFabric;
        private int customerDestinationId;
        private decimal sellingUTSurchargePercent;
        private decimal fobUTSurchangePercent;
        private decimal importDutyPercent;
        private decimal quarterlyExchangeRate;
        private int isNSLVMTROrder;
        private decimal cmCost;
        private decimal qaCommissionPercent;
        private decimal gtCommissionPercent;
        /*
        private decimal additionalBankChargesPercent;
        */
        private string colour;
        private int withQCCharge;
        private bool editLock;
        private bool paymentLock;
        private ShippingDocWFS shippingDocWFS;
        private DateTime docReviewedOn;
        private int docReviewedBy;
        private int rejectPaymentReasonId;
        private decimal labTestIncome;
        private bool isChinaGBTestRequired;
        private bool isTradingAF = false;
        private decimal tradingAFActualCost = 0;
        private string tradingAFReason;
        private int tradingAFTypeId;
        private decimal tradingAFEstimationCost = 0;
        private string nslRefNo;
        private int gspFormTypeId;

        public ShipmentDef()
        {
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int ContractId
        {
            get { return contractId; }
            set { contractId = value; }
        }

        public int DeliveryNo
        {
            get { return deliveryNo; }
            set { deliveryNo = value; }
        }

        public string NSLPONo
        {
            get { return nslPONo; }
            set { nslPONo = value; }
        }

        public VendorRef Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }

        public VendorRef VMVendor
        {
            get { return vmVendor; }
            set { vmVendor = value; }
        }

        public int FactoryId
        {
            get { return factoryId; }
            set { factoryId = value; }
        }

        public int SupplierAssignTypeId
        {
            get { return supplierAssignTypeId; }
            set { supplierAssignTypeId = value; }
        }

        public TermOfPurchaseRef TermOfPurchase
        {
            get { return termOfPurchase; }
            set { termOfPurchase = value; }
        }

        public TermOfPurchaseRef OriginalTermOfPurchase
        {
            get { return originalTermOfPurchase; }
            set { originalTermOfPurchase = value; }
        }

        public PurchaseLocationRef PurchaseLocation
        {
            get { return purchaseLocation; }
            set { purchaseLocation = value; }
        }

        public DateTime UKAtWarehouseDate
        {
            get { return ukAtWarehouseDate; }
            set { ukAtWarehouseDate = value; }
        }

        public DateTime CustomerAgreedAtWarehouseDate
        {
            get { return customerAgreedAtWarehouseDate; }
            set { customerAgreedAtWarehouseDate = value; }
        }

        public DateTime SupplierAgreedAtWarehouseDate
        {
            get { return supplierAgreedAtWarehouseDate; }
            set { supplierAgreedAtWarehouseDate = value; }
        }

        public ShipmentMethodRef ShipmentMethod
        {
            get { return shipmentMethod; }
            set { shipmentMethod = value; }
        }

        public AirFreightPaymentTypeRef AirFreightPaymentType
        {
            get { return airFreightPaymentType; }
            set { airFreightPaymentType = value; }
        }

        public decimal NUKAirFreightPaymentPercent
        {
            get { return nukAirFreightPaymentPercent; }
            set { nukAirFreightPaymentPercent = value; }
        }

        public decimal NSLAirFreightPaymentPercent
        {
            get { return nslAirFreightPaymentPercent; }
            set { nslAirFreightPaymentPercent = value; }
        }

        public decimal FTYAirFreightPaymentPercent
        {
            get { return ftyAirFreightPaymentPercent; }
            set { ftyAirFreightPaymentPercent = value; }
        }

        public decimal NSLSZAirFreightPaymentPercent
        {
            get { return nslszAirFreightPaymentPercent; }
            set { nslszAirFreightPaymentPercent = value; }
        }

        public decimal OtherAirFreightPaymentPercent
        {
            get { return otherAirFreightPaymentPercent; }
            set { otherAirFreightPaymentPercent = value; }
        }

        public string OtherAirFreightPaymentRemark
        {
            get { return otherAirFreightPaymentRemark; }
            set { otherAirFreightPaymentRemark = value; }
        }

        public string AirFreightPaymentRemark
        {
            get { return airFreightPaymentRemark; }
            set { airFreightPaymentRemark = value; }
        }

        public decimal AgencyCommissionPercentage
        {
            get { return agencyCommissionPercentage; }
            set { agencyCommissionPercentage = value; }
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

        public ShipmentCountryRef ShipmentCountry
        {
            get { return shipmentCountry; }
            set { shipmentCountry = value; }
        }

        public ShipmentPortRef ShipmentPort
        {
            get { return shipmentPort; }
            set { shipmentPort = value; }
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

        public decimal POExchangeRate
        {
            get { return poExchangeRate; }
            set { poExchangeRate = value; }
        }

        public decimal USExchangeRate
        {
            get { return usExchangeRate; }
            set { usExchangeRate = value; }
        }

        public decimal VatPercent
        {
            get { return vatPercent; }
            set { vatPercent = value; }
        }

        public decimal NSLCommissionPercentage
        {
            get { return nslCommissionPercentage; }
            set { nslCommissionPercentage = value; }
        }

        public string MockShopSampleRemark
        {
            get { return mockShopSampleRemark; }
            set { mockShopSampleRemark = value; }
        }

        public string NotesFromMerchandiser
        {
            get { return notesFromMerchandiser; }
            set { notesFromMerchandiser = value; }
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

        public decimal TotalOtherCost
        {
            get { return totalOtherCost; }
            set { totalOtherCost = value; }
        }

        public decimal TotalOrderFreightCost
        {
            get { return totalOrderFreightCost; }
            set { totalOrderFreightCost = value; }
        }

        public decimal TotalOrderDutyCost
        {
            get { return totalOrderDutyCost; }
            set { totalOrderDutyCost = value; }
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

        public decimal TotalShippedOtherCost
        {
            get { return totalShippedOtherCost; }
            set { totalShippedOtherCost = value; }
        }

        public decimal TotalShippedFreightCost
        {
            get { return totalShippedFreightCost; }
            set { totalShippedFreightCost = value; }
        }

        public decimal TotalShippedDutyCost
        {
            get { return totalShippedDutyCost; }
            set { totalShippedDutyCost = value; }
        }

        public decimal TotalShippedOPAUpcharge
        {
            get { return totalShippedOPAUpcharge; }
            set { totalShippedOPAUpcharge = value; }
        }

        public int SplitCount
        {
            get { return splitCount; }
            set { splitCount = value; }
        }

        public int IsRepeatOrder
        {
            get { return isRepeatOrder; }
            set { isRepeatOrder = value; }
        }

        public int DelayReasonTypes
        {
            get { return delayReasonTypes; }
            set { delayReasonTypes = value; }
        }

        public string DelayReasonOther
        {
            get { return delayReasonOther; }
            set { delayReasonOther = value; }
        }

        public ContractWFS WorkflowStatus
        {
            get { return workflowStatus; }
            set { workflowStatus = value; }
        }

        public int SpecialOrderTypeId
        {
            get { return specialOrderTypeId; }
            set { specialOrderTypeId = value; }
        }

        public bool IsChinaGBTestRequired
        {
            get { return isChinaGBTestRequired; }
            set { isChinaGBTestRequired = value; }
        }

        public int IsMockShopSample
        {
            get { return isMockShopSample; }
            set { isMockShopSample = value; }
        }

        public int IsPressSample
        {
            get { return isPressSample; }
            set { isPressSample = value; }
        }

        public int IsStudioSample
        {
            get { return isStudioSample; }
            set { isStudioSample = value; }
        }

        public int SalesForecastSpecialGroupId
        {
            get { return salesForecastSpecialGroupId; }
            set { salesForecastSpecialGroupId = value; }
        }

        public decimal VendorPaymentDiscountPercent
        {
            get { return vendorPaymentDiscountPercent; }
            set { vendorPaymentDiscountPercent = value; }
        }

        public int CustomDocType
        {
            get { return customDocType; }
            set { customDocType = value; }
        }

        public int IsVirtualSetSplit
        {
            get { return isVirtualSetSplit; }
            set { isVirtualSetSplit = value; }
        }

        public int ThirdPartyAgencyId
        {
            get { return thirdPartyAgencyId; }
            set { thirdPartyAgencyId = value; }
        }

        public int IsRatioPackOrder
        {
            get { return isRatioPackOrder; }
            set { isRatioPackOrder = value; }
        }

        public int IsUKDiscount
        {
            get { return isUKDiscount; }
            set { isUKDiscount = value; }
        }

        public int UKDiscountReasonId
        {
            get { return ukDiscountReasonId; }
            set { ukDiscountReasonId = value; }
        }

        public int WithOPRFabric
        {
            get { return withOPRFabric; }
            set { withOPRFabric = value; }
        }

        public int CustomerDestinationId
        {
            get { return customerDestinationId; }
            set { customerDestinationId = value; }
        }

        public decimal SellingUTSurchargePercent
        {
            get { return sellingUTSurchargePercent; }
            set { sellingUTSurchargePercent = value; }
        }

        public decimal FobUTSurchangePercent
        {
            get { return fobUTSurchangePercent; }
            set { fobUTSurchangePercent = value; }
        }

        public decimal ImportDutyPercent
        {
            get { return importDutyPercent; }
            set { importDutyPercent = value; }
        }

        public decimal QuarterlyExchangeRate
        {
            get { return quarterlyExchangeRate; }
            set { quarterlyExchangeRate = value; }
        }

        public int IsNSLVMTROrder
        {
            get { return isNSLVMTROrder; }
            set { isNSLVMTROrder = value; }
        }

        public decimal QACommissionPercent
        {
            get { return qaCommissionPercent; }
            set { qaCommissionPercent = value; }
        }

        public decimal GTCommissionPercent
        {
            get { return gtCommissionPercent; }
            set { gtCommissionPercent = value; }
        }
        /*
        public decimal AdditionalBankChargesPercent
        {
            get { return additionalBankChargesPercent; }
            set { additionalBankChargesPercent = value; }
        }
        */
        public decimal CMCost
        {
            get { return cmCost; }
            set { cmCost = value; }
        }

        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public int WithQCCharge
        {
            get { return withQCCharge; }
            set { withQCCharge = value; }
        }

        public bool EditLock
        {
            get { return editLock; }
            set { editLock = value; }
        }

        public bool PaymentLock
        {
            get { return paymentLock; }
            set { paymentLock = value; }

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

        public decimal LabTestIncome
        {
            get { return labTestIncome; }
            set { labTestIncome = value; }
        }

        public bool IsTradingAirFreight
        {
            get { return isTradingAF; }
            set { isTradingAF = value; }
        }

        public decimal TradingAirFreightActualCost
        {
            get { return tradingAFActualCost; }
            set { tradingAFActualCost = value; }
        }

        public string TradingAirFreightReason
        {
            get { return tradingAFReason; }
            set { tradingAFReason = value; }
        }

        public int TradingAirFreightTypeId
        {
            get { return tradingAFTypeId; }
            set { tradingAFTypeId = value; }
        }

        public decimal TradingAirFreightEstimationCost
        {
            get { return tradingAFEstimationCost; }
            set { tradingAFEstimationCost = value; }
        }

        public string NSLRefNo
        {
            get { return nslRefNo; }
            set { nslRefNo = value; }
        }

        public int GSPFormTypeId
        {
            get { return gspFormTypeId; }
            set { gspFormTypeId = value; }
        }


    }
}
