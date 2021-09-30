using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class AdvancePaymentBalanceSettlementDef : DomainData
    {
        private int paymentId;
        private DateTime paymentDate;
        private decimal expectedAmt;
        private decimal paymentAmt;
        private DateTime settlementDate;
        private string remark;
        private int status;

        public AdvancePaymentBalanceSettlementDef() 
        {
            paymentAmt = 0;
            paymentDate = DateTime.MinValue;
            Remark = "";
            status = 1;
        } 

        public int PaymentId
        {
            get { return paymentId; }
            set { paymentId = value; }
        }

        public DateTime PaymentDate
        {
            get { return paymentDate; }
            set { paymentDate = value; }
        }

        public decimal ExpectedAmount
        {
            get { return expectedAmt; }
            set { expectedAmt = value; }
        }

        public decimal PaymentAmount
        {
            get { return paymentAmt; }
            set { paymentAmt = value; }
        }

        public DateTime SettlementDate
        {
            get { return settlementDate; }
            set { settlementDate = value; }
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
