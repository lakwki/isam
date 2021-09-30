using System;
using com.next.common.domain;
using com.next.isam.domain.product;
using com.next.isam.domain.common;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class SplitShipmentRef : DomainData
	{
		private int splitShipmentId;
		private string splitSuffix;
        private SplitProductDef product;
		private int vendorId;
		private string supplierName;
		private DateTime supplierAgreedAtWarehouseDate;
		private int totalOrderQuantity;
		private decimal totalOrderAmount;
		private decimal totalNetFOBAmount;
		private decimal totalOPAUpcharge;
		private int isVirtualSetSplit;
		private int piecesPerPack;
		private int isKnitwearComponent;
		private int isFobOrder;

		public SplitShipmentRef()
		{

        }

		public int SplitShipmentId
		{
			get { return splitShipmentId; }
			set { splitShipmentId = value; }
		}

		public string SplitSuffix
		{
			get { return splitSuffix; }
			set { splitSuffix = value; }
		}

		public int VendorId
		{
			get { return vendorId; }
			set { vendorId = value; }
		}

		public SplitProductDef Product 
        {
            get {return product; } 
            set {product = value;}
        }

		public string SupplierName
		{
			get { return supplierName; }
			set { supplierName = value; }
		}

		public DateTime SupplierAgreedAtWarehouseDate
		{
			get { return supplierAgreedAtWarehouseDate;	}
			set { supplierAgreedAtWarehouseDate = value; }
		}

		public int TotalOrderQuantity
		{
			get { return totalOrderQuantity; }
			set { totalOrderQuantity = value; }
		}

		public decimal TotalOrderAmount
		{
			get { return totalOrderAmount; }
			set { totalOrderAmount = value; }
		}

		public decimal TotalNetFOBAmount
		{
			get { return totalNetFOBAmount; }
			set { totalNetFOBAmount = value; }
		}

		public decimal TotalOPAUpcharge
		{
			get { return totalOPAUpcharge; }
			set { totalOPAUpcharge = value; }
		}

		public int IsVirtualSetSplit
		{
			get { return isVirtualSetSplit; }
			set	{ isVirtualSetSplit = value; }
		}

		public int PiecesPerPack
		{
			get { return piecesPerPack; }
			set { piecesPerPack = value; }
		}

		public int IsKnitwearComponent 
        { 
            get { return isKnitwearComponent; } 
            set { isKnitwearComponent = value; } 
        }

		public int IsFobOrder 
        { 
            get { return isFobOrder; } 
            set { isFobOrder = value; } 
        }

	}
}
