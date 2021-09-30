using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class AdvancePaymentOrderDetailDef : DomainData
    {
        private int paymentId;
        private int shipmentId;
        private decimal expectedValue;
        private decimal actualValue;
        private DateTime settlementDate;
        private bool isInitial;
        private string remark;
        private int status;

        public int PaymentId
        {
            get { return paymentId; }
            set { paymentId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public decimal ExpectedValue
        {
            get { return expectedValue; }
            set { expectedValue = value; }
        }

        public decimal ActualValue
        {
            get { return actualValue; }
            set { actualValue = value; }
        }

        public DateTime SettlementDate
        {
            get { return settlementDate; }
            set { settlementDate = value; }
        }

        public bool IsInitial
        {
            get { return isInitial; }
            set { isInitial = value; }            
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
