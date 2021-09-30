using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class AdjustmentDetailDef : DomainData
    {
        private int adjustmentDetailId;
        private int shipmentId;
        private int splitShipmentId;
        private AdjustmentType adjustmentType;
        private string debitCreditIndicator;
        private int vendorId;
        private CurrencyRef currency;
        private decimal latestAmount;
        private decimal settledAmount;
        private decimal adjustmentAmount;
        private int adjustmentNoteId;
        private bool isInterfaced;
        private decimal qaCommissionPercent;
        private decimal qaCommissionAmt;
        private decimal vendorPaymentDiscountPercent;
        private decimal vendorPaymentDiscountAmt;
        private decimal labTestIncome;


        public int AdjustmentDetailId
        {
            get { return adjustmentDetailId; }
            set { adjustmentDetailId = value; }
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

        public AdjustmentType AdjustmentType
        {
            get { return adjustmentType; }
            set { adjustmentType = value; }
        }

        public string DebitCreditIndicator
        {
            get { return debitCreditIndicator; }
            set { debitCreditIndicator = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public CurrencyRef Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public decimal LatestAmount
        {
            get { return latestAmount; }
            set { latestAmount = value; }
        }

        public decimal SettledAmount
        {
            get { return settledAmount; }
            set { settledAmount = value; }
        }

        public decimal AdjustmentAmount
        {
            get { return adjustmentAmount; }
            set { adjustmentAmount = value; }
        }

        public int AdjustmentNoteId
        {
            get { return adjustmentNoteId; }
            set { adjustmentNoteId = value; }
        }

        public bool IsInterfaced
        {
            get { return isInterfaced; }
            set { isInterfaced = value; }
        }

        public decimal QACommissionPercent
        {
            get { return qaCommissionPercent; }
            set { qaCommissionPercent = value; }
        }

        public decimal QACommissionAmount
        {
            get { return qaCommissionAmt; }
            set { qaCommissionAmt = value; }
        }

        public decimal VendorPaymentDiscountPercent
        {
            get { return vendorPaymentDiscountPercent; }
            set { vendorPaymentDiscountPercent = value; }
        }

        public decimal VendorPaymentDiscountAmount
        {
            get { return vendorPaymentDiscountAmt; }
            set { vendorPaymentDiscountAmt = value; }
        }

        public decimal LabTestIncome
        {
            get { return labTestIncome; }
            set { labTestIncome = value; }
        }

    }
}
