using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class SplitShipmentDetailDef : DomainData
	{
		private int splitShipmentDetailId;
		private int splitShipmentId;
		private int shipmentDetailId;
		private SizeOptionRef sizeOption;
		private decimal sellingPrice;
		private decimal netFOBPrice;
		private decimal supplierGarmentPrice;
		private int orderQuantity;
		private int poQuantity;
		private int shippedQuantity;
		private decimal opaUpcharge;
		private decimal reducedSellingPrice;
		private decimal reducedNetFOBPrice;
		private decimal reducedSupplierGmtPrice;

		public SplitShipmentDetailDef()
		{
			sellingPrice = 0;
			netFOBPrice = 0;
			supplierGarmentPrice = 0;
			orderQuantity = 0;
			poQuantity = 0;
			shippedQuantity = 0;
		}

		public int SplitShipmentDetailId
		{
			get { return splitShipmentDetailId; }
			set { splitShipmentDetailId = value; }
		}

		public int SplitShipmentId
		{
			get { return splitShipmentId; }
			set { splitShipmentId = value; }
		}

		public int ShipmentDetailId
		{
			get { return shipmentDetailId; }
			set { shipmentDetailId = value; }
		}

		public SizeOptionRef SizeOption
		{
			get { return sizeOption; }
			set { sizeOption = value; }
		}

		public decimal SellingPrice
		{
			get { return sellingPrice; }
			set { sellingPrice = value; }
		}

		public decimal NetFOBPrice
		{
			get { return netFOBPrice; }
			set { netFOBPrice = value; }
		}

		public decimal SupplierGarmentPrice
		{
			get { return supplierGarmentPrice; }
			set { supplierGarmentPrice = value; }
		}

		public int OrderQuantity
		{
			get { return orderQuantity; }
			set { orderQuantity = value; }
		}

		public int POQuantity
		{
			get { return poQuantity; }
			set { poQuantity = value; }
		}

		public int ShippedQuantity
		{
			get { return shippedQuantity; }
			set { shippedQuantity = value; }
		}

		public decimal OrderAmount
		{
			get { return orderQuantity * sellingPrice; }
		}

        public decimal OrderAmountAfterDiscount
        {
            get { return orderQuantity * reducedSellingPrice; }
        }

        public decimal POAmount
		{
			get { return poQuantity * sellingPrice; }
		}

        public decimal POAmountAfterDiscount
        {
            get { return poQuantity * reducedSellingPrice; }
        }

        public decimal PONetFOBAmount
		{
			get { return poQuantity * netFOBPrice; }
		}

        public decimal PONetFOBAmountAfterDiscount
        {
            get { return poQuantity * reducedNetFOBPrice; }
        }

        public decimal ShippedAmount
		{
			get { return shippedQuantity * sellingPrice; }
		}

        public decimal ShippedAmountAfterDiscount
        {
            get { return shippedQuantity * reducedSellingPrice; }
        }

        public decimal NetFOBAmount
		{
			get { return orderQuantity * netFOBPrice; }
		}

        public decimal NetFOBAmountAfterDiscount
        {
            get { return orderQuantity * reducedNetFOBPrice; }
        }

        public decimal ShippedNetFOBAmount
        {
            get { return shippedQuantity * netFOBPrice; }
        }

        public decimal ShippedNetFOBAmountAfterDiscount
        {
            get { return shippedQuantity * reducedNetFOBPrice; }
        }

        public decimal SupplierGarmentAmount
		{
			get { return orderQuantity * supplierGarmentPrice; }
		}

        public decimal SupplierGarmentAmountAfterDiscount
        {
            get { return orderQuantity * reducedSupplierGmtPrice; }
        }

        public decimal ShippedSupplierGarmentAmount
        {
            get { return shippedQuantity * supplierGarmentPrice; }
        }

        public decimal ShippedSupplierGarmentAmountAfterDiscount
        {
            get { return shippedQuantity * reducedSupplierGmtPrice; }
        }

        public decimal OPAUpcharge 
        { 
            get { return opaUpcharge; } 
            set { opaUpcharge = value; } 
        }

		public decimal OPAUpchargeAmount
		{
			get { return orderQuantity * opaUpcharge; }
		}

		public decimal ShippedOPAUpchargeAmount
		{
			get { return shippedQuantity * opaUpcharge; }
		}

		public decimal ReducedSellingPrice 
        { 
            get { return reducedSellingPrice; } 
            set { reducedSellingPrice = value; } 
        }

		public decimal ReducedNetFOBPrice 
        { 
            get { return reducedNetFOBPrice; } 
            set { reducedNetFOBPrice = value; } 
        }

		public decimal ReducedSupplierGmtPrice 
        { 
            get { return reducedSupplierGmtPrice; } 
            set { reducedSupplierGmtPrice = value; } 
        }
		
	}
}
