using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class DomainILSInvoiceDef : DomainData
    {
        private ILSInvoiceDef invoice;
        private ArrayList invoiceDetails;

        public DomainILSInvoiceDef()
        {
            invoice = new ILSInvoiceDef();
            invoiceDetails = new ArrayList();
        }

        public ILSInvoiceDef Invoice 
        { 
            get { return invoice; } 
            set { invoice = value; } 
        }

        public ArrayList InvoiceDetails 
        { 
            get { return invoiceDetails; } 
            set { invoiceDetails = value; } 
        } 
    }
}
