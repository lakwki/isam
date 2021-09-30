using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class SunInterfaceLogDef : DomainData
    {
        private int sunInterfaceLogId;
        private int sunInterfaceTypeId;
        private int shipmentId;
        private int splitShipmentId;
        private int fiscalYear;
        private int period;
        private DateTime transactionDate;
        private string contractNo;
        private string nslPONo;
        private int deliveryNo;
        private int officeId;
        private int deptId;
        private int productTeamId;
        private int productId;
        private int seasonId;
        private int vendorId;
        private int customerId;
        private int termOfPurchaseId;
        private int customerDestinationId;
        private int tradingAgencyId;
        private int currencyId;
        private int paymentTermId;
        private int countryOfOriginId;
        private int designSourceId;
        private int withOPRFabric;
        private string invoicePrefix;
        private int invoiceSeq;
        private int invoiceYear;
        private int sequenceNo;
        private DateTime invoiceUploadDate;
        private decimal baseAmt;
        private decimal otherAmt;
        private int qty;
        private int piecesPerPack;
        private int packingUnitId;
        private string supplierInvoiceNo;
        private string refNo;
        private string dcNoteNo;
        private decimal baseTax;
        private decimal otherTax;
        private int setSplitCount;
        private decimal qaCommissionPercent;
        private decimal vendorPaymentDiscountPercent;
        private decimal labTestIncome;
        private decimal salesCommission;
        private decimal totalShippedAmt;
        private decimal totalShippedNetFOBAmt;
        private decimal totalShippedSupplierGmtAmt;
        private decimal utInputVATAmt;
        private decimal utOutputVATAmt;
        private decimal utImportDuty;
        private decimal utImportDutyInBaseCurrency;
        private decimal utTotalShippedAmt;
        private decimal utTotalShippedNetFOBAmt;
        private decimal utTotalShippedSupplierGmtAmt;
        private decimal utSalesCommission;
        private int isNextMfgOrder;
        private int isPOIssueToNextMfg;
        private int isMockShopSample;
        private int isStudioSample;
        private bool isSelfBilledOrder;
        private int categoryId;
        private decimal fullBaseAmt;
        private decimal fullOtherAmt;
        private decimal fullBaseTax;
        private decimal fullOtherTax;
        private int fullQty;
        private string ukSupplierCode;
        private bool isReversalEntry;
        private int queueId;
        private DateTime createdOn;

        public SunInterfaceLogDef() { }

        public int SunInterfaceLogId 
        { 
            get { return sunInterfaceLogId; } 
            set { sunInterfaceLogId = value; } 
        }

        public int SunInterfaceTypeId
        {
            get { return sunInterfaceTypeId; }
            set { sunInterfaceTypeId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int SplitShipmentId
        {
            get { return splitShipmentId; }
            set { splitShipmentId = value; }
        }

        public int FiscalYear
        {
            get { return fiscalYear; }
            set { fiscalYear = value; }
        }

        public int Period
        {
            get { return period; }
            set { period = value; }
        }

        public DateTime TransactionDate
        {
            get { return transactionDate; }
            set { transactionDate = value; }
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

        public int DeliveryNo
        {
            get { return deliveryNo; }
            set { deliveryNo = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int DeptId
        {
            get { return deptId; }
            set { deptId = value; }
        }

        public int ProductTeamId
        {
            get { return productTeamId; }
            set { productTeamId = value; }
        }

        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        public int SeasonId
        {
            get { return seasonId; }
            set { seasonId = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public int CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public int TermOfPurchaseId
        {
            get { return termOfPurchaseId; }
            set { termOfPurchaseId = value; }
        }

        public int CustomerDestinationId
        {
            get { return customerDestinationId; }
            set { customerDestinationId = value; }
        }

        public int TradingAgencyId
        {
            get { return tradingAgencyId; }
            set { tradingAgencyId = value; }
        }

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public int PaymentTermId
        {
            get { return paymentTermId; }
            set { paymentTermId = value; }
        }

        public int CountryOfOriginId
        {
            get { return countryOfOriginId; }
            set { countryOfOriginId = value; }
        }

        public int DesignSourceId
        {
            get { return designSourceId; }
            set { designSourceId = value; }
        }

        public int WithOPRFabric
        {
            get { return withOPRFabric; }
            set { withOPRFabric = value; }
        }

        public string InvoicePrefix
        {
            get { return invoicePrefix; }
            set { invoicePrefix = value; }
        }

        public int InvoiceSequence
        {
            get { return invoiceSeq; }
            set { invoiceSeq = value; }
        }

        public int InvoiceYear
        {
            get { return invoiceYear; }
            set { invoiceYear = value; }
        }

        public int SequenceNo
        {
            get { return sequenceNo; }
            set { sequenceNo = value; }
        }

        public DateTime InvoiceUploadDate
        {
            get { return invoiceUploadDate; }
            set { invoiceUploadDate = value; }
        }

        public decimal BaseAmount
        {
            get { return baseAmt; }
            set { baseAmt = value; }
        }

        public decimal OtherAmount
        {
            get { return otherAmt; }
            set { otherAmt = value; }
        }

        public int Quantity
        {
            get { return qty; }
            set { qty = value; }
        }

        public int PiecesPerPack
        {
            get { return piecesPerPack; }
            set { piecesPerPack = value; }
        }

        public int PackingUnitId
        {
            get { return packingUnitId; }
            set { packingUnitId = value; }
        }

        public string SupplierInvoiceNo
        {
            get { return supplierInvoiceNo; }
            set { supplierInvoiceNo = value; }
        }

        public string ReferenceNo
        {
            get { return refNo; }
            set { refNo = value; }
        }

        public string DebitCreditNoteNo
        {
            get { return dcNoteNo; }
            set { dcNoteNo = value; }
        }

        public decimal BaseTax
        {
            get { return baseTax; }
            set { baseTax = value; }
        }

        public decimal OtherTax
        {
            get { return otherTax; }
            set { otherTax = value; }
        }

        public int SetSplitCount
        {
            get { return setSplitCount; }
            set { setSplitCount = value; }
        }

        public decimal QACommissionPercent
        {
            get { return qaCommissionPercent; }
            set { qaCommissionPercent = value; }
        }

        public decimal VendorPaymentDiscountPercent
        {
            get { return vendorPaymentDiscountPercent; }
            set { vendorPaymentDiscountPercent = value; }
        }

        public decimal LabTestIncome
        {
            get { return labTestIncome; }
            set { labTestIncome = value; }
        }

        public decimal SalesCommission
        {
            get { return salesCommission; }
            set { salesCommission = value; }
        }

        public decimal TotalShippedAmount
        {
            get { return totalShippedAmt; }
            set { totalShippedAmt = value; }
        }

        public decimal TotalShippedNetFOBAmount
        {
            get { return totalShippedNetFOBAmt; }
            set { totalShippedNetFOBAmt = value; }
        }

        public decimal TotalShippedSupplierGmtAmount
        {
            get { return totalShippedSupplierGmtAmt; }
            set { totalShippedSupplierGmtAmt = value; }
        }

        public decimal UTInputVATAmount
        {
            get { return utInputVATAmt; }
            set { utInputVATAmt = value; }
        }

        public decimal UTOutputVATAmount
        {
            get { return utOutputVATAmt; }
            set { utOutputVATAmt = value; }
        }

        public decimal UTImportDuty
        {
            get { return utImportDuty; }
            set { utImportDuty = value; }
        }

        public decimal UTImportDutyInBaseCurrency
        {
            get { return utImportDutyInBaseCurrency; }
            set { utImportDutyInBaseCurrency = value; }
        }

        public decimal UTTotalShippedAmount
        {
            get { return utTotalShippedAmt; }
            set { utTotalShippedAmt = value; }
        }

        public decimal UTTotalShippedNetFOBAmount
        {
            get { return utTotalShippedNetFOBAmt; }
            set { utTotalShippedNetFOBAmt = value; }
        }

        public decimal UTTotalShippedSupplierGmtAmount
        {
            get { return utTotalShippedSupplierGmtAmt; }
            set { utTotalShippedSupplierGmtAmt = value; }
        }

        public decimal UTSalesCommission
        {
            get { return utSalesCommission; }
            set { utSalesCommission = value; }
        }

        public int IsNextManufacturingOrder
        {
            get { return isNextMfgOrder; }
            set { isNextMfgOrder = value; }
        }

        public int IsPOIssueToNextManufacturing
        {
            get { return isPOIssueToNextMfg; }
            set { isPOIssueToNextMfg = value; }
        }

        public int IsMockShopSample
        {
            get { return isMockShopSample; }
            set { isMockShopSample = value; }
        }

        public int IsStudioSample
        {
            get { return isStudioSample; }
            set { isStudioSample = value; }
        }

        public bool IsSelfBilledOrder
        {
            get { return isSelfBilledOrder; }
            set { isSelfBilledOrder = value; }
        }

        public int CategoryId
        {
            get { return categoryId; }
            set { categoryId = value; }
        }

        public decimal FullBaseAmount
        {
            get { return fullBaseAmt; }
            set { fullBaseAmt = value; }
        }

        public decimal FullOtherAmount
        {
            get { return fullOtherAmt; }
            set { fullOtherAmt = value; }
        }

        public decimal FullBaseTax
        {
            get { return fullBaseTax; }
            set { fullBaseTax = value; }
        }

        public decimal FullOtherTax
        {
            get { return fullOtherTax; }
            set { fullOtherTax = value; }
        }

        public int FullQuantity
        {
            get { return fullQty; }
            set { fullQty = value; }
        }

        public string UKSupplierCode
        {
            get { return ukSupplierCode; }
            set { ukSupplierCode = value; }
        }

        public bool IsReversalEntry
        {
            get { return isReversalEntry; }
            set { isReversalEntry = value; }
        }

        public int QueueId
        {
            get { return queueId; }
            set { queueId = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        public bool IsSampleOrder
        {
            get { return (isMockShopSample == 1 || isStudioSample == 1); }
        }

    }
}
