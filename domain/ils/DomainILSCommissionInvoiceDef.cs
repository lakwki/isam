using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class DomainILSCommissionInvoiceDef : DomainData
    {
        private ILSCommissionInvoiceDef invoice;
        private ArrayList invoiceDetails;

        public DomainILSCommissionInvoiceDef()
        {
            invoice = new ILSCommissionInvoiceDef();
            invoiceDetails = new ArrayList();
        }

        public ILSCommissionInvoiceDef Invoice 
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
