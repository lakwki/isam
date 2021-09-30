using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class EpicorGLJournalDetailDef : DomainData
    {
        public EpicorGLJournalDetailDef() { }
        public long RecordId { get; set; }
        public string Company { get; set; }
        public int FiscalYear { get; set; }
        public int FiscalPeriod { get; set; }
        public int JournalNum { get; set; }
        public int JournalLine { get; set; }
        public string Description { get; set; }
        public DateTime JEDate { get; set; }
        public string GroupID { get; set; }
        public decimal Number01 { get; set; }
        public string CurrencyCode { get; set; }
        public string SegValue1 { get; set; }
        public string SegValue2 { get; set; }
        public string SegValue3 { get; set; }
        public string SegValue4 { get; set; }
        public string SegValue5 { get; set; }
        public string SegValue6 { get; set; }
        public string SegValue7 { get; set; }
        public string SegValue8 { get; set; }
        public string SegValue9 { get; set; }
        public string SegValue10 { get; set; }
        public string DC { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal BookDebitAmount { get; set; }
        public decimal BookCreditAmount { get; set; }
        public string LegalNumber { get; set; }
        public string JournalCode { get; set; }
        public string SourceModule { get; set; }
        public string APInvoiceNum { get; set; }
        public DateTime APInvoiceDate { get; set; }
        public string Character05 { get; set; }
        public string Character01 { get; set; }
        public string CommentText { get; set; }
        public string LegalNumberID { get; set; }
        public string OriginalDescription { get; set; }
        public string ShortChar01 { get; set; }
        public string IsBank { get; set; }
        public string IsCash { get; set; }
    }
}
