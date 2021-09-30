using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class UKItemRef : DomainData
	{
        private string itemNo;
        private string description;
        private string supplierCode;
        private string subGroup;

        public UKItemRef()
		{
		}

		public string ItemNo
		{
			get { return itemNo; }
            set { itemNo = value; }
		}

		public string Description
		{
			get { return description; }
			set { description = value;	}
		}

		public string SupplierCode
		{
			get { return supplierCode; }
            set { supplierCode = value; }
		}

        public string SubGroup
        {
            get { return subGroup; }
            set { subGroup = value; }
        }


	}
}
