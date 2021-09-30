using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class InvoiceDef : DomainData
	{
		private int shipmentId;
		private string invoicePrefix;
		private int invoiceSeqNo;
        private int invoiceYear;
        private int sequenceNo;
        private DateTime invoiceDate;
        private DateTime invoiceUploadDate;
        private UserRef invoiceUploadUser;
        private DateTime invoicePrintDate;
        private UserRef invoicePrintUser;
        private bool isReadyToSendInvoice;
        private DateTime invoiceSentDate;
        private string supplierInvoiceNo;
        private string shippingRemark;
        private decimal nslCommissionAmt;
        private decimal invoiceBuyExchangeRate;
        private decimal invoiceSellExchangeRate;
        private DateTime arDate;
        private decimal arExchangeRate;
        private decimal arAmt;
        private string arRefNo;
        private DateTime apDate;
        private decimal apExchangeRate;
        private decimal apAmt;
        private string apRefNo;
        private decimal lcAmt;
        private string lcNo;
        private string lcBillRefNo;
        private DateTime lcIssueDate;
        private DateTime lcExpiryDate;
        private int isLCPaymentChecked;
        private DateTime lcPaymentCheckedDate;
        private decimal exportLicenceFee;
        private decimal quotaCharge;
        private string itemDesc1;
        private string itemDesc2;
        private string itemDesc3;
        private string itemDesc4;
        private string itemDesc5;
        private string itemColour;
        private ShipmentCountryRef shipFromCountry;
        private CustomerDestinationDef customerDestination;
        private int piecesPerDeliveryUnit;
        private PackingMethodRef packingMethod;
        private DateTime shippingDocReceiptDate;
        private DateTime accountDocReceiptDate;
        private bool isSelfBilledOrder;
        private int containerType;
        private decimal courierChargeToNUK;
        private string courierChargeToNUKDebitNoteNo;
        private string qccRemark;
        private bool isSyncToFactory; // iNetGarment
        private string bookingSONo;
        private DateTime bookingDate;
        private int bookingQty;
        private DateTime bookingAtWarehouseDate;
        private DateTime actualAtWarehouseDate;
        private DateTime ilsActualAtWarehouseDate;
        private CurrencyRef importDutyCurrency;
        private decimal importDutyCalculatedAmt;
        private decimal importDutyActualAmt;
        private int isImportDutyChecked;
        private DateTime importDutyCheckedDate;

        private CurrencyRef inputVATCurrency;
        private decimal inputVATCalculatedAmt;
        private decimal inputVATActualAmt;
        private int isInputVATChecked;
        private DateTime inputVATCheckedDate;

        private CurrencyRef outputVATCurrency;
        private decimal outputVATCalculatedAmt;
        private decimal outputVATActualAmt;
        private int isOutputVATChecked;
        private DateTime outputVATCheckedDate;

        private string dfDebitNoteNo;
        private decimal dfDocumentationCharge;
        private decimal  dfTransportationCharge;

        private string invoiceRemark;
		private int status;

        private DateTime salesScanDate;
        private decimal salesScanAmt;
        private DateTime purchaseScanDate;
        private decimal purchaseScanAmt;
        private UserRef purchaseScanBy;
        private int eInvoiceBatchId;
        private bool isILSQtyUploadAllowed;

        private DateTime nslCommissionSettlementDate;
        private decimal nslCommissionSettlementExchangeRate;
        private decimal nslCommissionSettlementAmt;
        private string nslCommissionRefNo;

        private decimal choiceOrderTotalShippedAmt;
        private decimal choiceOrderTotalShippedSupplierGmtAmt;
        private decimal choiceOrderNSLCommissionAmt;

        private bool isUploadDMSDocument;
        private DateTime lastSendDMSDocumentDate;
        private DateTime shippingDocCheckedOn;
        private decimal shippingCheckedTotalNetAmount;
        private UserRef shippingDocCheckedBy;

		public InvoiceDef()	{ }

		public int ShipmentId {	
            get { return shipmentId; } 
            set { shipmentId = value; } 
        }

        public string InvoicePrefix 
        { 
            get { return invoicePrefix; } 
            set { invoicePrefix = value; } 
        }

        public int InvoiceSeqNo 
        { 
            get { return invoiceSeqNo; } 
            set { invoiceSeqNo = value; } 
        }

        public int InvoiceYear 
        { 
            get { return invoiceYear; } 
            set { invoiceYear = value; } 
        }

        public string InvoiceNo 
        { 
            get { return this.formatInvoiceNo(invoicePrefix, invoiceSeqNo, invoiceYear); }
        }

        public int SequenceNo
        {
            get { return sequenceNo; }
            set { sequenceNo = value; }
        }

        public DateTime InvoiceDate 
        { 
            get { return invoiceDate; } 
            set { invoiceDate = value; } 
        }

        public DateTime InvoiceUploadDate 
        { 
            get { return invoiceUploadDate; } 
            set { invoiceUploadDate = value; } 
        }

        public UserRef InvoiceUploadUser 
        { 
            get { return invoiceUploadUser; } 
            set { invoiceUploadUser = value; } 
        }

        public DateTime InvoicePrintDate 
        { 
            get { return invoicePrintDate; } 
            set { invoicePrintDate = value; } 
        }

        public UserRef InvoicePrintUser 
        { 
            get { return invoicePrintUser; } 
            set { invoicePrintUser = value; } 
        }

        public bool IsReadyToSendInvoice
        {
            get { return isReadyToSendInvoice; }
            set { isReadyToSendInvoice = value; }
        }

        public DateTime InvoiceSentDate
        {
            get { return invoiceSentDate; }
            set { invoiceSentDate = value; }
        }

        public string SupplierInvoiceNo 
        { 
            get { return supplierInvoiceNo; } 
            set { supplierInvoiceNo = value; } 
        }

        public string ShippingRemark 
        { 
            get { return shippingRemark; } 
            set { shippingRemark = value; } 
        }

        public decimal NSLCommissionAmt 
        { 
            get { return nslCommissionAmt; } 
            set { nslCommissionAmt = value; } 
        }

        public decimal InvoiceBuyExchangeRate
        {
            get { return invoiceBuyExchangeRate; }
            set { invoiceBuyExchangeRate = value; }
        }

        public decimal InvoiceSellExchangeRate
        {
            get { return invoiceSellExchangeRate; }
            set { invoiceSellExchangeRate = value; }
        }

        public DateTime ARDate 
        { 
            get { return arDate; } 
            set { arDate = value; } 
        }

        public decimal ARExchangeRate 
        { 
            get { return arExchangeRate; } 
            set { arExchangeRate = value; } 
        }

        public decimal ARAmt 
        { 
            get { return arAmt; } 
            set { arAmt = value; } 
        }

        public string ARRefNo 
        { 
            get { return arRefNo; } 
            set { arRefNo = value; } 
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

        public decimal APAmt 
        { 
            get { return apAmt; } 
            set { apAmt = value; } 
        }

        public string APRefNo 
        { 
            get { return apRefNo; } 
            set { apRefNo = value; } 
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

        public string LCBillRefNo
        {
            get { return lcBillRefNo; }
            set { lcBillRefNo = value; }
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

        public decimal ExportLicenceFee 
        { 
            get { return exportLicenceFee; } 
            set { exportLicenceFee = value; } 
        }

        public decimal QuotaCharge 
        { 
            get { return quotaCharge; } 
            set { quotaCharge = value; } 
        }

        public string ItemDesc1 
        { 
            get { return itemDesc1; } 
            set { itemDesc1 = value; } 
        }

        public string ItemDesc2 
        { 
            get { return itemDesc2; } 
            set { itemDesc2 = value; } 
        }

        public string ItemDesc3 
        { 
            get { return itemDesc3; } 
            set { itemDesc3 = value; } 
        }

        public string ItemDesc4 
        { 
            get { return itemDesc4; } 
            set { itemDesc4 = value; } 
        }

        public string ItemDesc5 
        { 
            get { return itemDesc5; } 
            set { itemDesc5 = value; } 
        }

        public string ItemColour 
        { 
            get { return itemColour; } 
            set { itemColour = value; } 
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

        public bool IsSelfBilledOrder 
        { 
            get { return isSelfBilledOrder; } 
            set { isSelfBilledOrder = value; } 
        }

        public int ContainerType 
        { 
            get { return containerType; } 
            set { containerType = value; } 
        }

        public decimal CourierChargeToNUK 
        { 
            get { return courierChargeToNUK; } 
            set { courierChargeToNUK = value; } 
        }

        public string CourierChargeToNUKDebitNoteNo 
        { 
            get { return courierChargeToNUKDebitNoteNo; } 
            set { courierChargeToNUKDebitNoteNo = value; } 
        }

        public string QCCRemark
        {
            get { return qccRemark; }
            set { qccRemark = value; }
        }

        public bool IsSyncToFactory 
        { 
            get { return isSyncToFactory; } 
            set { isSyncToFactory = value; } 
        }

        public string BookingSONo 
        { 
            get { return bookingSONo; } 
            set { bookingSONo = value; } 
        }

        public DateTime BookingDate 
        { 
            get { return bookingDate; } 
            set { bookingDate = value; } 
        }

        public int BookingQty 
        { 
            get { return bookingQty; } 
            set { bookingQty = value; } 
        }

        public DateTime BookingAtWarehouseDate 
        { 
            get { return bookingAtWarehouseDate; } 
            set { bookingAtWarehouseDate = value; } 
        }

        public DateTime ActualAtWarehouseDate 
        { 
            get { return actualAtWarehouseDate; } 
            set { actualAtWarehouseDate = value; } 
        }

        public DateTime ILSActualAtWarehouseDate
        {
            get { return ilsActualAtWarehouseDate; }
            set { ilsActualAtWarehouseDate = value; }
        }

        public CurrencyRef ImportDutyCurrency 
        { 
            get { return importDutyCurrency; } 
            set { importDutyCurrency = value; } 
        }

        public decimal ImportDutyCalculatedAmt 
        { 
            get { return importDutyCalculatedAmt; } 
            set { importDutyCalculatedAmt = value; } 
        }

        public decimal ImportDutyActualAmt 
        { 
            get { return importDutyActualAmt; } 
            set { importDutyActualAmt = value; } 
        }

        public int IsImportDutyChecked 
        { 
            get { return isImportDutyChecked; } 
            set { isImportDutyChecked = value; } 
        }

        public DateTime ImportDutyCheckedDate 
        { 
            get { return importDutyCheckedDate; } 
            set { importDutyCheckedDate = value; } 
        }

        public CurrencyRef InputVATCurrency 
        { 
            get { return inputVATCurrency; } 
            set { inputVATCurrency = value; } 
        }

        public decimal InputVATCalculatedAmt 
        { 
            get { return inputVATCalculatedAmt; } 
            set { inputVATCalculatedAmt = value; } 
        }

        public decimal InputVATActualAmt 
        { 
            get { return inputVATActualAmt; } 
            set { inputVATActualAmt = value; } 
        }

        public int IsInputVATChecked 
        { 
            get { return isInputVATChecked; } 
            set { isInputVATChecked = value; } 
        }

        public DateTime InputVATCheckedDate 
        { 
            get { return inputVATCheckedDate; } 
            set { inputVATCheckedDate = value; } 
        }

        public CurrencyRef OutputVATCurrency 
        { 
            get { return outputVATCurrency; } 
            set { outputVATCurrency = value; } 
        }

        public decimal OutputVATCalculatedAmt 
        { 
            get { return outputVATCalculatedAmt; } 
            set { outputVATCalculatedAmt = value; } 
        }

        public decimal OutputVATActualAmt 
        { 
            get { return outputVATActualAmt; } 
            set { outputVATActualAmt = value; } 
        }

        public int IsOutputVATChecked 
        { 
            get { return isOutputVATChecked; } 
            set { isOutputVATChecked = value; } 
        }

        public DateTime OutputVATCheckedDate 
        { 
            get { return outputVATCheckedDate; } 
            set { outputVATCheckedDate = value; } 
        }

        public string InvoiceRemark 
        { 
            get { return invoiceRemark; } 
            set { invoiceRemark = value; } 
        }

		public int Status 
        {	
            get { return status; } 
            set { status = value; } 
        }

        private string formatInvoiceNo(string invoiceNoPrefix, int invoiceSeqNo, int invoiceYear)
        {
            if (invoicePrefix != String.Empty)
                return invoiceNoPrefix + "/" + invoiceSeqNo.ToString().PadLeft(5, '0') + "/" + invoiceYear.ToString();
            else
                return String.Empty;
        }

        public ShipmentCountryRef ShipFromCountry
        {
            get { return shipFromCountry; }
            set { shipFromCountry = value; }
        }
        
        public CustomerDestinationDef CustomerDestination
        {
            get { return customerDestination; }
            set { customerDestination = value; }
        }

        public PackingMethodRef PackingMethod
        {
            get { return packingMethod; }
            set { packingMethod = value; }
        }

        public int PiecesPerDeliveryUnit
        {
            get { return piecesPerDeliveryUnit; }
            set { piecesPerDeliveryUnit = value; }
        }

        public string DirectFranchiseDebitNoteNo
        {
            get { return dfDebitNoteNo; }
            set { dfDebitNoteNo = value; }
        }

        public decimal DirectFranchiseDocumentCharge
        {
            get { return dfDocumentationCharge ; }
            set { dfDocumentationCharge = value; }
        }

        public decimal DirectFranchiseTransportationCharge
        {
            get { return dfTransportationCharge; }
            set { dfTransportationCharge = value; }
        }

        public DateTime SalesScanDate
        {
            get { return salesScanDate; }
            set { salesScanDate = value; }
        }

        public decimal SalesScanAmount
        {
            get { return salesScanAmt; }
            set { salesScanAmt = value; }
        }

        public DateTime PurchaseScanDate
        {
            get { return purchaseScanDate; }
            set { purchaseScanDate = value; }
        }

        public decimal PurchaseScanAmount
        {
            get { return purchaseScanAmt; }
            set { purchaseScanAmt = value; }
        }

        public UserRef PurchaseScanBy
        {
            get { return purchaseScanBy; }
            set { purchaseScanBy = value; }
        }

        public int EInvoiceBatchId
        {
            get { return eInvoiceBatchId; }
            set { eInvoiceBatchId = value; }
        }

        public bool IsILSQtyUploadAllowed
        {
            get { return isILSQtyUploadAllowed; }
            set { isILSQtyUploadAllowed = value; }

        }

        public DateTime NSLCommissionSettlementDate
        {
            get { return nslCommissionSettlementDate; }
            set { nslCommissionSettlementDate = value; }
        }

        public decimal NSLCommissionSettlementExchangeRate
        {
            get { return nslCommissionSettlementExchangeRate; }
            set { nslCommissionSettlementExchangeRate = value; }
        }

        public decimal NSLCommissionSettlementAmt
        {
            get { return nslCommissionSettlementAmt; }
            set { nslCommissionSettlementAmt = value; }
        }

        public string NSLCommissionRefNo
        {
            get { return nslCommissionRefNo; }
            set { nslCommissionRefNo = value; }
        }

        public decimal ChoiceOrderTotalShippedAmount
        {
            get { return choiceOrderTotalShippedAmt; }
            set { choiceOrderTotalShippedAmt = value; }
        }

        public decimal ChoiceOrderTotalShippedSupplierGarmentAmount
        {
            get { return choiceOrderTotalShippedSupplierGmtAmt; }
            set { choiceOrderTotalShippedSupplierGmtAmt = value; }
        }

        public decimal ChoiceOrderNSLCommissionAmount
        {
            get { return choiceOrderNSLCommissionAmt; }
            set { choiceOrderNSLCommissionAmt = value; }
        }

        public bool IsUploadDMSDocument
        {
            get { return isUploadDMSDocument; }
            set { isUploadDMSDocument = value; }
        }

        public DateTime LastSendDMSDocumentDate
        {
            get { return lastSendDMSDocumentDate; }
            set { lastSendDMSDocumentDate = value; }
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


        private bool isValidInvoiceNo(string invoiceNo)
        {
            if (invoiceNo.Length != 14)
                return false;

            if (invoiceNo.Substring(3, 1) != "/" || invoiceNo.Substring(9, 1) != "/")
                return false;

            for (int i = 4; i <= 8; i++)
            {
                if (!Char.IsNumber(invoiceNo[i]))
                    return false;
            }
            for (int i = 10; i <= 13; i++)
            {
                if (!Char.IsNumber(invoiceNo[i]))
                    return false;
            }

            return true;
        }

	}
}
