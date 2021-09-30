using System;
using System.Web.UI.WebControls;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.industry.vendor;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class ContractShipmentListJDef : DomainData
    {
        private int shipmentId;
        private int contractId;
        private int productId;
        private string itemNo;
        private string contractNo;
        private SeasonRef season;
        private OfficeRef office;
        private int productDepartmentId;
        private ProductCodeRef productTeam;
        private CustomerDef customer;
        private CustomerDestinationDef customerDestination;
        private int deliveryNo;
        private VendorRef vendor;

        private DateTime customerAgreedAtWarehouseDate;
        private int totalShippedQuantity;
        private decimal totalShippedAmount;
        private decimal totalOrderAmount;
        private int totalOrderQty;

        private int termOfPurchaseId;

        private CurrencyRef sellCurrency;
        private CurrencyRef buyCurrency;
        private int splitCount;

        private ShipmentMethodRef shipmentMethod;

        private bool editLock;
        private bool paymentLock;

        private int isNextMfgOrder;
        private int isDualSourcingOrder;
        private string invoicePrefix;
        private int invoiceSeq;
        private int invoiceYear;
        private int sequenceNo;

        private string invoiceNo;
        private DateTime invoiceDate;
        private decimal invoiceAmount;
        private int totalPOQuantity;
        private UserRef shippingUser;
        private ContractWFS workflowStatus;
        private int isUKDiscount;
        private int withOPRFabric;
        private bool isconfirmedToShip;
        private int isMockShopSample;
        private int isPressSample;
        private int isStudioSample;
        private int isLDPOrder;
        private int withQCCharge;

        private TradingAgencyDef tradingAgency;
        private string supplierInvoiceNo;
        private decimal totalShippedSupplierGarmentAmountAfterDiscount;
        private decimal totalShippedSupplierGarmentAmount;
        private decimal totalShippedNetFOBAmtAfterDiscount;
        private decimal nslCommissionPercent;
        private decimal nslCommissionAmt;
        private DateTime purchaseScanDate;
        private UserRef purchaseScanBy;

        private PaymentTermRef paymentTerm;
        private decimal arAmount;
        private DateTime arDate;
        private string arRefNo;
        private DateTime apDate;
        private string apRefNo;
        private decimal apAmount;
        private DateTime nslCommissionSettlementDate;
        private decimal nslCommissionSettlementAmt;
        private string nslCommissionRefNo;

        private DateTime salesScanDate;
        private int eInoviceBatchId;
        private bool isSelfBilledOrder;
        private decimal qaCommissionPercent;

        private int PiecesPerDlyUnit;
        private DateTime ShipDocRcptDate;
        private bool isLCPaymentChecked;
        private bool isUploadDMSDocument;

        public ContractShipmentListJDef()
        {
        }

        public string InvoicePrefix
        {
            get { return invoicePrefix; }
            set { invoicePrefix = value; }
        }

        public int InvoiceSeq
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

        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        public string ItemNo
        {
            get { return itemNo; }
            set { itemNo = value; }
        }

        public string ContractNo
        {
            get { return contractNo; }
            set { contractNo = value; }
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


        public int ProductDepartmentId
        {
            get { return productDepartmentId; }
            set { productDepartmentId = value; }
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

        public CustomerDestinationDef CustomerDestination
        {
            get { return customerDestination; }
            set { customerDestination = value; }
        }

        public int DeliveryNo
        {
            get { return deliveryNo; }
            set { deliveryNo = value; }
        }

        public VendorRef Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }

        public DateTime CustomerAgreedAtWarehouseDate
        {
            get { return customerAgreedAtWarehouseDate; }
            set { customerAgreedAtWarehouseDate = value; }
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

        public int TermOfPurchaseId
        {
            get { return termOfPurchaseId; }
            set { termOfPurchaseId = value; }
        }

        public int SplitCount
        {
            get { return splitCount; }
            set { splitCount = value; }
        }

        public ShipmentMethodRef ShipmentMethod
        {
            get { return shipmentMethod; }
            set { shipmentMethod = value; }
        }


        public int IsNextMfgOrder
        {
            get { return isNextMfgOrder; }
            set { isNextMfgOrder = value; }
        }

        public int IsDualSourcingOrder
        {
            get { return isDualSourcingOrder; }
            set { isDualSourcingOrder = value; }
        }

        public string InvoiceNo
        {
            get { return invoiceNo; }
            set { invoiceNo = value; }
        }

        public DateTime InvoiceDate
        {
            get { return invoiceDate; }
            set { invoiceDate = value; }
        }

        public int TotalPOQuantity
        {
            get { return totalPOQuantity; }
            set { totalPOQuantity = value; }
        }

        public decimal InvoiceAmount
        {
            get { return invoiceAmount; }
            set { invoiceAmount = value; }
        }

        public UserRef ShippingUser
        {
            get { return shippingUser; }
            set { shippingUser = value; }
        }

        public ContractWFS WorkflowStatus
        {
            get { return workflowStatus; }
            set { workflowStatus = value; }
        }

        public int IsUKDiscount
        {
            get { return isUKDiscount; }
            set { isUKDiscount = value; }
        }

        public int WithOPRFabric
        {
            get { return withOPRFabric; }
            set { withOPRFabric = value; }
        }


        public bool isConfirmedToShip
        {
            get { return isconfirmedToShip; }
            set { isconfirmedToShip = value; }
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

        public TradingAgencyDef TradingAgency
        {
            get { return tradingAgency; }
            set { tradingAgency = value; }
        }


        public string SupplierInvoiceNo
        {
            get { return supplierInvoiceNo; }
            set { supplierInvoiceNo = value; }
        }

        public decimal TotalShippedSupplierGarmentAmountAfterDiscount
        {
            get { return totalShippedSupplierGarmentAmountAfterDiscount; }
            set { totalShippedSupplierGarmentAmountAfterDiscount = value; }
        }

        public decimal TotalShippedSupplierGarmentAmount
        {
            get { return totalShippedSupplierGarmentAmount; }
            set { totalShippedSupplierGarmentAmount = value; }
        }

        public decimal TotalShippedNetFOBAmtAfterDiscount { get; set; }

        public decimal NSLCommissionPercent
        {
            get { return nslCommissionPercent; }
            set { nslCommissionPercent = value; }
        }

        public decimal NSLCommissionAmount
        {
            get { return nslCommissionAmt; }
            set { nslCommissionAmt = value; }
        }

        public DateTime PurchaseScanDate
        {
            get { return purchaseScanDate; }
            set { purchaseScanDate = value; }
        }

        public UserRef PurchaseScanBy
        {
            get { return purchaseScanBy; }
            set { purchaseScanBy = value; }
        }

        public PaymentTermRef PaymentTerm
        {
            get { return paymentTerm; }
            set { paymentTerm = value; }
        }

        public decimal ARAmount
        {
            get { return arAmount; }
            set { arAmount = value; }
        }

        public DateTime ARDate
        {
            get { return arDate; }
            set { arDate = value; }
        }

        public string ARRefNo
        {
            get { return arRefNo; }
            set { arRefNo = value; }
        }

        public decimal APAmount
        {
            get { return apAmount; }
            set { apAmount = value; }
        }

        public DateTime APDate
        {
            get { return apDate; }
            set { apDate = value; }
        }

        public string APRefNo
        {
            get { return apRefNo; }
            set { apRefNo = value; }
        }

        public DateTime NSLCommissionSettlementDate
        {
            get { return nslCommissionSettlementDate; }
            set { nslCommissionSettlementDate = value; }
        }

        public decimal NSLCommissionSettlementAmount
        {
            get { return nslCommissionSettlementAmt; }
            set { nslCommissionSettlementAmt = value; }
        }

        public string NSLCommissionSettlementRefNo
        {
            get { return nslCommissionRefNo; }
            set { nslCommissionRefNo = value; }
        }

        public DateTime SalesScanDate
        {
            get { return salesScanDate; }
            set { salesScanDate = value; }
        }

        public int EInoviceBatchId
        {
            get { return eInoviceBatchId; }
            set { eInoviceBatchId = value; }
        }

        public bool IsSelfBilledOrder
        {
            get { return isSelfBilledOrder; }
            set { isSelfBilledOrder = value; }
        }

        public decimal QACommissionPercent
        {
            get { return qaCommissionPercent; }
            set { qaCommissionPercent = value; }
        }

        public decimal VendorPaymentDiscountPercent { get; set; }
        public decimal LabTestIncome { get; set; }

        public decimal QACommissionAmount { get; set; }
        public decimal VendorPaymentDiscountAmount { get; set; }
        public decimal LabTestIncomeAmount { get; set; }

        public int IsLDPOrder
        {
            get { return isLDPOrder; }
            set { isLDPOrder = value; }
        }

        public int WithQCCharge
        {
            get { return withQCCharge; }
            set { withQCCharge = value; }
        }

        public decimal TotalOrderAmount
        {
            get { return totalOrderAmount; }
            set { totalOrderAmount = value; }
        }

        public int TotalOrderQuantity
        {
            get { return totalOrderQty; }
            set { totalOrderQty = value; }
        }

        //public int PiecesPerDeliveryUnit
        //{
        //    get { return PiecesPerDlyUnit; }
        //    set { PiecesPerDlyUnit = value; }
        //}

        public DateTime ShippingDocReceiptDate
        {
            get { return ShipDocRcptDate; }
            set { ShipDocRcptDate = value; }
        }

        public bool IsLCPaymentChecked
        {
            get { return isLCPaymentChecked; }
            set { isLCPaymentChecked = value; }
        }

        public bool IsUploadDMSDocument
        {
            get { return isUploadDMSDocument; }
            set { isUploadDMSDocument = value; }
        }

        public class ContractShipmentComparer : IComparer
        {
            public enum CompareType
            {
                ContractNo = 1,
                DlyNo = 2,
                Supplier = 3,
                ItemNo = 4,
                CustomerDlyDate = 5,
                SalesScanDate = 6,
                InvoiceNo = 7,
                ShippingUser = 8
            }

            private CompareType compareType;
            private SortDirection direction;

            public ContractShipmentComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(object x, object y)
            {
                ContractShipmentListJDef contractX = (ContractShipmentListJDef)x;
                ContractShipmentListJDef contractY = (ContractShipmentListJDef)y;

                if (compareType.GetHashCode() == CompareType.ContractNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return contractX.ContractNo.CompareTo(contractY.ContractNo);
                    else
                        return contractY.ContractNo.CompareTo(contractX.ContractNo);
                }
                else if (compareType.GetHashCode() == CompareType.DlyNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return contractX.DeliveryNo.CompareTo(contractY.DeliveryNo);
                    else
                        return contractY.DeliveryNo.CompareTo(contractX.DeliveryNo);
                }
                else if (compareType.GetHashCode() == CompareType.Supplier.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return contractX.Vendor.Name.CompareTo(contractY.Vendor.Name);
                    else
                        return contractY.Vendor.Name.CompareTo(contractX.Vendor.Name);
                }
                else if (compareType.GetHashCode() == CompareType.ItemNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return contractX.ItemNo.CompareTo(contractY.ItemNo);
                    else
                        return contractY.ItemNo.CompareTo(contractX.ItemNo);
                }
                else if (compareType.GetHashCode() == CompareType.CustomerDlyDate.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return contractX.CustomerAgreedAtWarehouseDate.CompareTo(contractY.CustomerAgreedAtWarehouseDate);
                    else
                        return contractY.CustomerAgreedAtWarehouseDate.CompareTo(contractX.CustomerAgreedAtWarehouseDate);
                }
                else if (compareType.GetHashCode() == CompareType.SalesScanDate.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return contractX.SalesScanDate.CompareTo(contractY.SalesScanDate);
                    else
                        return contractY.SalesScanDate.CompareTo(contractX.SalesScanDate);
                }
                else if (compareType.GetHashCode() == CompareType.InvoiceNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return contractX.InvoiceNo.CompareTo(contractY.InvoiceNo);
                    else
                        return contractY.InvoiceNo.CompareTo(contractX.InvoiceNo);
                }
                else if (compareType.GetHashCode() == CompareType.ShippingUser.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                    {
                        if (contractX.ShippingUser != null && contractY.ShippingUser != null)
                            return contractX.ShippingUser.DisplayName.CompareTo(contractY.ShippingUser.DisplayName);
                        else if (contractX.ShippingUser == null && contractY.ShippingUser == null)
                            return 0;
                        else if (contractX.ShippingUser == null)
                            return 1;
                        else
                            return -1;
                    }
                    else
                    {
                        if (contractX.ShippingUser != null && contractY.ShippingUser != null)
                            return contractY.ShippingUser.DisplayName.CompareTo(contractX.ShippingUser.DisplayName);
                        else if (contractX.ShippingUser == null && contractY.ShippingUser == null)
                            return 0;
                        else if (contractX.ShippingUser == null)
                            return -1;
                        else
                            return 1;
                    }
                }
                return 0;
            }
        }

        public ShippingDocWFS ShippingDocWFS { get; set; }
        public int RejectPaymentReasonId { get; set; }
        public int SpecialOrderTypeId { get; set; }
        public bool IsChinaGBTestRequired { get; set; }

    }
}
