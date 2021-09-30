using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
	[Serializable()]
	public class EpicorVendorDef : DomainData
	{
        public string Key { get; set; }
        public int VendorNum { get; set; }
		public string Company { get; set; }
		public string VendorID { get; set; }
		public string Name { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string Address3 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZIP { get; set; }
		public string Country { get; set; }

    }
}