using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.product
{
	/// <summary>
	/// Summary description for SplitProductDef.
	/// </summary>

	[Serializable()]
	public class SplitProductDef : DomainData
	{
		private int productId;
		private string itemNo;
		private string splitSuffix;
		private string description1;
		private string description2;
		private string description3;
		private string description4;
		private string description5;
		private string shortDesc;
		private string colour;
		private string styleDesignRef;
		private DesignSourceRef designSource;
		
		private int designerId;

		private ProductDef parentProduct;
		private string originalDescription1;
		private string originalDescription2;
		private string originalDescription3;
		private string originalDescription4;
		private string originalDescription5;
		private string originalColour;
		private string originalStyleDesignRef;
		private string originalShortDesc;
		private int status;

		public SplitProductDef()
		{
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
	
		public string SplitSuffix
		{
			get 
			{
				return splitSuffix;
			}
			set
			{
				splitSuffix = value;
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

		public DesignSourceRef DesignSource
		{
			get 
			{
				return designSource;
			}
			set
			{
				designSource = value;
			}
		}

		public int DesignerId
		{
			get 
			{
				return designerId;
			}
			set
			{
				designerId = value;
			}
		}

		public ProductDef ParentProduct
		{
			get 
			{
				return parentProduct;
			}
			set
			{
				parentProduct = value;
			}
		}

		public string OriginalDescription1
		{
			get 
			{
				return originalDescription1;
			}
			set
			{
				originalDescription1 = value;
			}
		}

		public string OriginalDescription2
		{
			get 
			{
				return originalDescription2;
			}
			set
			{
				originalDescription2 = value;
			}
		}

		public string OriginalDescription3
		{
			get 
			{
				return originalDescription3;
			}
			set
			{
				originalDescription3 = value;
			}
		}

		public string OriginalDescription4
		{
			get 
			{
				return originalDescription4;
			}
			set
			{
				originalDescription4 = value;
			}
		}

		public string OriginalDescription5
		{
			get 
			{
				return originalDescription5;
			}
			set
			{
				originalDescription5 = value;
			}
		}

		public string OriginalColour
		{
			get 
			{
				return originalColour;
			}
			set
			{
				originalColour = value;
			}
		}

		public string OriginalStyleDesignRef
		{
			get 
			{
				return originalStyleDesignRef;
			}
			set
			{
				originalStyleDesignRef = value;
			}
		}

		public string OriginalShortDesc
		{
			get 
			{
				return originalShortDesc;
			}
			set
			{
				originalShortDesc = value;
			}
		}



		public int Status 
		{
			get 
			{
				return status;
			} 
			set
			{
				status = value;
			}
		}
	}
}
