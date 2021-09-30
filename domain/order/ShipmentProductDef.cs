using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;

namespace com.next.isam.domain.order
{
    [Serializable()]

    public class ShipmentProductDef : DomainData
    {
        public int ProductId { get; set; }
        public string ItemNo { get; set; }
        public string ItemDesc { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public string Desc4 { get; set; }
        public string Desc5 { get; set; }
        public int DeptId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int ProductTeamId { get; set; }
        public string ProductTeamCode { get; set; }
        public string ProductTeamName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int OfficeId { get; set; }
        public string OfficeCode { get; set; }
        public string OfficeName { get; set; }
        public int ContractId { get; set; }
        public int ShipmentId { get; set; }
        public int TermOfPurchaseId { get; set; }
        public int HandlingOfficeId { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
