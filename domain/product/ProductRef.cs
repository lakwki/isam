using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.product
{
	/// <summary>
	/// Summary description for ProductRef.
	/// </summary>
	[Serializable()]
	public class ProductRef : DomainData
	{
		private int productId;
		private string itemNo;
		private string description1;
		private string description2;
		private string description3;
		private string description4;
		private string description5;
		private string shortDesc;
		private string colour;
		private string styleDesignRef;

		public ProductRef()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public int ProductId
		{
			get 
			{
				return productId;
			}
			set
			{
				productId = value;
			}
		}

		public string ItemNo
		{
			get 
			{
                if (itemNo != null) return itemNo; else return String.Empty;
			}
			set
			{
				itemNo = value;
			}
		}
	
		public string Description1
		{
			get 
			{
				return description1;
			}
			set
			{
				description1 = value;
			}
		}

		public string Description2
		{
			get 
			{
				return description2;
			}
			set
			{
				description2 = value;
			}
		}

		public string Description3
		{
			get 
			{
				return description3;
			}
			set
			{
				description3 = value;
			}
		}

		public string Description4
		{
			get 
			{
				return description4;
			}
			set
			{
				description4 = value;
			}
		}

		public string Description5
		{
			get 
			{
				return description5;
			}
			set
			{
				description5 = value;
			}
		}

		public string ShortDesc
		{
			get 
			{
				return shortDesc;
			}
			set
			{
				shortDesc = value;
			}
		}

		public string Colour
		{
			get 
			{
				return colour;
			}
			set
			{
				colour = value;
			}
		}

		public string StyleDesignRef
		{
			get 
			{
				return styleDesignRef;
			}
			set
			{
				styleDesignRef = value;
			}
		}

        public string MasterDescription1 { get; set; }
        public string MasterDescription2 { get; set; }
        public string MasterDescription3 { get; set; }
        public string MasterDescription4 { get; set; }
        public string MasterDescription5 { get; set; }
        public int ProductDesignStyleId { get; set; }
        public string RetailDescription { get; set; }
	}
}
