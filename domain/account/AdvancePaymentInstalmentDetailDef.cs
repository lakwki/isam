using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class AdvancePaymentInstalmentDetailDef : DomainData
    {
        private int paymentId;
        private DateTime paymentDate;
        private decimal expectedAmt;
        private decimal paymentAmt;
        private DateTime settlementDate;
        private string remark;
        private decimal interestRate;
        private decimal interestAmt;
        private decimal remainingTotalAmt;
        private DateTime interestFromDate;
        private DateTime interestToDate;
        private DateTime dcNoteDate;
        private string dcNoteNo;
        private int status;
        private int mailStatus;
        bool isDCNoteInterfaced = false;

        public AdvancePaymentInstalmentDetailDef() 
        {
            paymentAmt = 0;
            paymentDate = DateTime.MinValue;
            settlementDate = DateTime.MinValue;
            Remark = "";
            isDCNoteInterfaced = false;
            interestRate = 0;
            dcNoteDate = DateTime.MinValue;
            dcNoteNo = string.Empty;
            remainingTotalAmt = 0;
            interestFromDate = DateTime.MinValue;
            interestToDate = DateTime.MinValue;
            interestAmt = 0;
            status = 1;
            mailStatus = 0;
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

        public decimal PaymentAmount
        {
            get { return paymentAmt; }
            set { paymentAmt = value; }
        }

        public decimal ExpectedAmount
        {
            get { return expectedAmt; }
            set { expectedAmt = value; }
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

        public bool IsInterestCharge
        {
            get { return (remark.ToLower().IndexOf("interest charge") != -1); }
        }

        public decimal InterestRate
        {
            get { return interestRate; }
            set { interestRate = value; }
        }

        public decimal InterestAmt
        {
            get { return interestAmt; }
            set { interestAmt = value; }
        }

        public decimal RemainingTotalAmt
        {
            get { return remainingTotalAmt; }
            set { remainingTotalAmt = value; }
        }

        public DateTime InterestFromDate
        {
            get { return interestFromDate; }
            set { interestFromDate = value; }
        }

        public DateTime InterestToDate
        {
            get { return interestToDate; }
            set { interestToDate = value; }
        }

        public DateTime DCNoteDate
        {
            get { return dcNoteDate; }
            set { dcNoteDate = value; }
        }

        public string DCNoteNo
        {
            get { return dcNoteNo; }
            set { dcNoteNo = value; }
        }

        public bool IsDCNoteInterfaced
        {
            get { return isDCNoteInterfaced; }
            set { isDCNoteInterfaced = value; }
        }

        public int MailStatus
        {
            get { return mailStatus; }
            set { mailStatus = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }



    }
}
