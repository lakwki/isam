using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimDCNoteDetailDef : DomainData
    {
        private int dcNoteDetailId;
        private int dcNoteId;
        private int claimId;
        private int claimRefundId;
        private int currencyId;
        private decimal amt;
        private decimal rechargeableAmt;
        private string lineRemark;

        public int DCNoteDetailId
        {
            get { return dcNoteDetailId; }
            set { dcNoteDetailId = value; }
        }

        public int ClaimId
        {
            get { return claimId; }
            set { claimId = value; }
        }

        public int ClaimRefundId
        {
            get { return claimRefundId; }
            set { claimRefundId = value; }
        }

        public int DCNoteId
        {
            get { return dcNoteId; }
            set { dcNoteId = value; }
        }


        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public decimal Amount
        {
            get { return amt; }
            set { amt = value; }
        }

        public decimal RechargeableAmount
        {
            get { return rechargeableAmt; }
            set { rechargeableAmt = value; }
        }

        public string LineRemark
        {
            get { return lineRemark; }
            set { lineRemark = value; }
        }

    }
}
