using System;
using com.next.common.domain;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKDiscountClaimLogDef : DomainData
    {
        public UKDiscountClaimLogDef()
        {

        }

        public UKDiscountClaimLogDef(int claimId, string logText, int userId, int fromStatusId, int toStatusId)
        {
            this.LogId = -1;
            this.ClaimId = claimId;
            this.LogText = logText;
            this.FromStatusId = fromStatusId;
            this.ToStatusId = toStatusId;
            this.UserId = userId;
            this.LogDate = DateTime.Now;
        }

        private int logId;
        private int claimId;
        private string logText;
        private int fromStatusId;
        private int toStatusId;
        private int userId;
        private DateTime logDate;

        public int LogId
        {
            get { return logId; }
            set { logId = value; }
        }

        public int ClaimId
        {
            get { return claimId; }
            set { claimId = value; }
        }

        public string LogText
        {
            get { return logText; }
            set { logText = value; }
        }

        public int FromStatusId
        {
            get { return fromStatusId; }
            set { fromStatusId = value; }
        }

        public int ToStatusId
        {
            get { return toStatusId; }
            set { toStatusId = value; }
        }

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public DateTime LogDate
        {
            get { return logDate; }
            set { logDate = value; }
        }
        
    }
}
