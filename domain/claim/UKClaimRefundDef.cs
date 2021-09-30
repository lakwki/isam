using System;
using System.Collections;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using System.Web.UI.WebControls;
using com.next.common.domain.industry.vendor;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimRefundDef : DomainData
    {
        public UKClaimRefundDef()
        {
            ClaimRefundId = -1;
            ClaimId = -1;
            ReceivedDate = DateTime.MinValue;
            Amount = 0;
            Remark = string.Empty;
            IsInterfaced = false;
            IsRechargeInterfaced = false;
            CreditNoteNo = string.Empty;
            CreditNoteDate = DateTime.MinValue;
            CreditNoteAmount = 0;
            Status = 1;
            this.IsReadyForSettlement = true;
            this.SettlementOption = UKClaimSettlemtType.DEFAULT;
            this.PnLAccountCode = string.Empty;
        }

        public int ClaimRefundId { get; set; }
        public int ClaimId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public bool IsInterfaced { get; set; }
        public bool IsRechargeInterfaced { get; set; }
        public string CreditNoteNo { get; set; }
        public DateTime CreditNoteDate { get; set; }
        public decimal CreditNoteAmount { get; set; }
        public int Status { get; set; }
        public bool IsReadyForSettlement { get; set; }
        public DateTime SettlementDate { get; set; }
        public UKClaimSettlemtType SettlementOption { get; set; }
        public string PnLAccountCode { get; set; }
    }
}
