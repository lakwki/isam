using System;
using com.next.common.domain;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKDiscountClaimRefundDef : DomainData
    {
        private int claimRefundId;
        private int claimId;
        private DateTime receivedDate;
        private decimal amount;
        private string remark;
        private int isInterfaced = 0;
        private int workflowStatusId = 0;
        private UKDiscountClaimWFS workflowStatus = UKDiscountClaimWFS.OUTSTANDING;
        private int status = 1;

        public int ClaimRefundId
        {
            get { return claimRefundId; }
            set { claimRefundId = value; }
        }

        public int ClaimId
        {
            get { return claimId; }
            set { claimId = value; }
        }

        public DateTime ReceivedDate
        {
            get { return receivedDate; }
            set { receivedDate = value; }
        }

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public int IsInterfaced
        {
            get { return isInterfaced; }
            set { isInterfaced = value; }
        }

        public int WorkflowStatusId
        {
            get { return workflowStatusId; }
            set { workflowStatusId = value; }
        }

        public UKDiscountClaimWFS WorkflowStatus
        {
            get { return workflowStatus; }
            set { workflowStatus = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
