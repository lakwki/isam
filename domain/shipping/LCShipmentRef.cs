using System;
using com.next.common.domain;
using com.next.common.domain.industry.vendor;
using com.next.common.domain.general;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.isam.domain.product;
using System.Collections;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace com.next.isam.domain.shipping
    {
    [Serializable()]
    public class LCShipmentRef : DomainData
    {
 
        public LCShipmentRef()
        {
        }

        public int ShipmentId  { get; set; }
        public int SplitShipmentId { get; set; }
        public int ContractId { get; set; }
        public string ContractNo { get; set; }
        public string SplitSuffix { get; set; }
        public int DeliveryNo { get; set; }

        public ProductDef Product { get; set; }
        public OfficeRef Office { get; set; }
        public CustomerDef Customer { get; set; }
        public CustomerDestinationDef CustomerDestination { get; set; }
        public DateTime CustomerAtWarehouseDate { get; set; }
        public DateTime SupplierAtWarehouseDate { get; set; }

        public VendorRef Vendor { get; set; }
        public BankBranchRef BankBranch { get; set; }

        public PaymentTermRef PaymentTerm { get; set; }
        public CountryOfOriginRef CountryOfOrigin { get; set; }
        public ShipmentMethodRef ShipmentMethod { get; set; }
        public ShipmentPortRef ShipmentPort { get; set; }
        public PackingMethodRef PackingMethod { get; set; }
        public PackingUnitRef PackingUnit { get; set; }
        public TermOfPurchaseRef TermOfPurchase { get; set; }
        public PurchaseLocationRef PurchaseLocation { get; set; }

        public CurrencyRef Currency { get; set; }
        public int TotalPOQuantity { get; set; }
        public decimal ShipmentTotalPOAmount { get; set; }  // Shipment total amount
        public decimal TotalPOAmt { get; set; } // LC Application total amount
        public decimal QACommissionPercent { get; set; }
        public decimal VendorPaymentDiscountPercent { get; set; }
        public decimal LabTestIncome { get; set; }
        public ContractWFS ShipmentWorkflowStatus { get; set; }


        public LCApplicationRef LCApplication { get; set; }
        public LCBatchRef LCBatch { get; set; }
        public UserRef LCApprover { get; set; }
        public DateTime LCApprovalDate { get; set; }
        public LCWFS WorkflowStatus { get; set; }

        public string LCNo { get; set; }
        public DateTime LCIssueDate { get; set; }
        public DateTime LCExpiryDate { get; set; }
        public decimal LCAmt { get; set; }

        public BankRef AdvisingBank { get; set; }
        
        public int ContractLCIssued { get; set; }
        public int OtherShipmentIdWithLCNo { get; set; }
        public int OtherSplitShipmentIdWithLCNo { get; set; }
        public DateTime LCCancellationDate { get; set; }
        public int Status { get; set; }

        public class ShipmentComparer : IComparer
        {
            public enum CompareType
            {
                ContractNo = 1,
                LcApplicationNo = 2,
                PoDeliveryDate = 3,
                ItemNo = 4
            }

            private CompareType compareType;
            private SortDirection direction;

            public ShipmentComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(object x, object y)
            {
                LCShipmentRef objX = (LCShipmentRef)x;
                LCShipmentRef objY = (LCShipmentRef)y;

                if (compareType.GetHashCode() == CompareType.ContractNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return objX.ContractNo.CompareTo(objY.ContractNo);
                    else
                        return objY.ContractNo.CompareTo(objX.ContractNo);
                }
               else if (compareType.GetHashCode() == CompareType.LcApplicationNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return objX.LCApplication.LCApplicationNo.CompareTo(objY.LCApplication.LCApplicationNo);
                    else
                        return objY.LCApplication.LCApplicationNo.CompareTo(objX.LCApplication.LCApplicationNo);
                }
                else if (compareType.GetHashCode() == CompareType.PoDeliveryDate.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return objX.SupplierAtWarehouseDate.CompareTo(objY.SupplierAtWarehouseDate);
                    else
                        return objY.SupplierAtWarehouseDate.CompareTo(objX.SupplierAtWarehouseDate);
                }
                else if (compareType.GetHashCode() == CompareType.ItemNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return objX.Product.ItemNo.CompareTo(objY.Product.ItemNo);
                    else
                        return objY.Product.ItemNo.CompareTo(objX.Product.ItemNo);
                }
                return 0;
            }
        }

        //public List<LCApplicationShipmentDetailDef> OtherDelivery { get; set; }
        
    }
}
