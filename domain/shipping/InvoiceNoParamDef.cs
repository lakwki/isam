using System;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class InvoiceNoParamDef
    {
        private int officeId;
        private int customerId;
        private string tradingAgencyId;
        private string invoicePrefix;
        private int invoiceSeq;

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public string TradingAgencyId
        {
            get { return tradingAgencyId; }
            set { tradingAgencyId = value; }
        }

        public string InvoicePrefix
        {
            get { return invoicePrefix; }
            set { invoicePrefix = value; }
        }

        public int InvoiceSeq
        {
            get { return invoiceSeq; }
            set { invoiceSeq = value; }
        }

        public string SampleType { get; set; }
        
    }
}
