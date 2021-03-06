using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class NSLedCostDataByWeekDef : DomainData
    {
        private decimal fobAmt = 0;
        private decimal vendorPaymentDiscountAmt = 0;
        private decimal qaCommissionAmt = 0;
        private decimal labTestIncomeAmt = 0;
        private decimal freightCostPerUnit = 0;
        private decimal dutyPercent = 0;
        private int qty = 0;
        private int adjRequired = 0;

        // for ns-led finance report consumption only
        public decimal FOBAmtInUSD 
        {
            get { return fobAmt; }
            set { fobAmt = value; }
        }

        public decimal VendorPaymentDiscountInUSD
        {
            get { return vendorPaymentDiscountAmt; }
            set { vendorPaymentDiscountAmt = value; }
        }

        public decimal QACommissionInUSD
        {
            get { return qaCommissionAmt; }
            set { qaCommissionAmt = value; }
        }

        public decimal LabTestIncomeInUSD
        {
            get { return labTestIncomeAmt; }
            set { labTestIncomeAmt = value; }
        }

        // generalize
        public decimal SettlementDiscountAmtInUSD 
        {
            get
            {
                if (vendorPaymentDiscountAmt > 0)
                    return vendorPaymentDiscountAmt;
                else if (qaCommissionAmt > 0)
                    return qaCommissionAmt;
                else
                    return labTestIncomeAmt;

            }
        }

        public decimal ActualFreightCostPerUnitInUSD
        {
            get { return freightCostPerUnit; }
            set { freightCostPerUnit = value; }
        }

        public decimal DutyPercent
        {
            get { return dutyPercent; }
            set { dutyPercent = value; }
        }

        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        public int AdjRequired
        {
            get { return adjRequired; }
            set { adjRequired = value; }
        }
    }
}
