using System;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    public class NUKSalesDef : DomainData 
    {
        public NUKSalesDef()
        {
        }

        public string ContractNo { get; set; }
        public int DeliveryNo { get; set; }
        public string Currency { get; set; }
        public decimal NSLValue { get; set; }
        public string NSLInvoiceNo { get; set; }
        public DateTime OkToMoveDate { get; set; }
        public int ShipmentId { get; set; }
        public int FiscalYear { get; set; }
        public int Period { get; set; }
    }
}
