using System;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.common;


namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NTRechargeDCNoteDef :DomainData
    {
        public NTRechargeDCNoteDef()
        {
            Status = GeneralCriteria.TRUE;
            MailStatus = 0;
        }

        public int RechargeDCNoteId { get; set; }
        public string RechargeDCNoteNo { get; set; }
        public DateTime RechargeDCNoteDate { get; set; }
        public OfficeRef Office { get; set; }
        public CompanyType Company { get; set; }
        public string DCIndicator { get; set; }
        public int ToVendorId { get; set; }
        public int ToOfficeId { get; set; }
        public int ToCompanyId { get; set; }
        public int ToCustomerId { get; set; }
        public int ToNTVendorId { get; set; }
        public int RechargeCurrencyId { get; set; }
        public decimal RechargeAmount { get; set; }
        public int FiscalYear { get; set; }
        public int FiscalPeriod { get; set; }
        public int IsSUNInterfaced { get; set; }
        public DateTime SettlementDate { get; set; }
        public decimal SettlementAmount { get; set; }
        public string SettlementBankRefNo { get; set; }
        public int Status { get; set; }
        public int MailStatus { get; set; }

        public string DCIndicatorEng
        {
            get
            {
                switch (DCIndicator)
                {
                    case "D":
                        return "Debit";
                    case "C":
                        return "Credit";
                    default:
                        return "";
                }
            }
        }
    }
}
