using System;
using System.Collections;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using System.Web.UI.WebControls;
using com.next.common.domain.industry.vendor;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimLogDef : DomainData
    {
        public UKClaimLogDef()
        {

        }

        public UKClaimLogDef(int claimId, string logText, int userId, int fromStatusId, int toStatusId)
        {
            this.LogId = -1;
            this.ClaimId = claimId;
            this.LogText = logText;
            this.FromStatusId = fromStatusId;
            this.ToStatusId = toStatusId;
            this.UserId = userId;
            this.LogDate = DateTime.Now;
        }

        public int LogId { get; set; }
        public int ClaimId { get; set; }
        public string LogText { get; set; }
        public int FromStatusId { get; set; }
        public int ToStatusId { get; set; }
        public int UserId { get; set; }
        public DateTime LogDate { get; set; }


    }
}
