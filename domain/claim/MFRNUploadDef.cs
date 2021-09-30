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
    public class MFRNUploadDef : DomainData
    {
        public MFRNUploadDef()
        {
            HasAttachment = false;
            VendorId = -1;
            OfficeId = -1;
            HandlingOfficeId = -1;
            ProductTeamId = -1;
            TermOfPurchaseId = -1;
            PaymentSupplierCode = string.Empty;
        }

        public string DNNo { get; set; }
        public DateTime DNDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string CurrencyCode { get; set; }
        public string ItemNo { get; set; }
        public int Qty { get; set; }
        public decimal Amount { get; set; }
        public bool HasAttachment { get; set; }
        public int VendorId { get; set; }
        public int OfficeId { get; set; }
        public int HandlingOfficeId { get; set; }
        public int ProductTeamId { get; set; }
        public int TermOfPurchaseId { get; set; }
        public string PaymentSupplierCode { get; set; }
    }
}
