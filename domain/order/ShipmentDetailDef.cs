using System;
using System.Collections;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class ShipmentDetailDef : DomainData
	{
		private int shipmentDetailId;
		private int shipmentId;
		private SizeOptionRef sizeOption;
		private decimal sellingPrice;
		private decimal netFOBPrice;
		private decimal supplierGarmentPrice;
		private int orderQuantity;
		private int poQuantity;
		private int shippedQuantity;
		private decimal totalOtherCost;
		private decimal totalShippedOtherCost;
		private int gspFormTypeId;
		private int shippingGSPFormTypeId;
		private decimal opaUpcharge;
		private int ratioPack;
		private decimal retailSellingPrice;
		private decimal reducedSellingPrice;
		private decimal reducedNetFOBPrice;
		private decimal reducedSupplierGmtPrice;
		private decimal freightCost;
		private decimal dutyCost;
		private decimal fobUTSurchargeUSD;
		private decimal cmPriceUSD;
        private string colour;
        private ArrayList otherCost;

		public ShipmentDetailDef()
		{
			sellingPrice = 0;
			netFOBPrice = 0;
			supplierGarmentPrice = 0;
			orderQuantity = 0;
			poQuantity = 0;
			shippedQuantity = 0;
		}

		public int ShipmentDetailId
		{
			get { return shipmentDetailId; }
			set { shipmentDetailId = value; }
		}

		public int ShipmentId
		{
			get { return shipmentId; }
			set { shipmentId = value; }
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

		public decimal TotalOtherCost
		{
			get { return totalOtherCost; }
			set { totalOtherCost = value; }
		}

		public decimal TotalShippedOtherCost
		{
			get { return totalShippedOtherCost; }
			set { totalShippedOtherCost = value; }
		}

        public decimal TotalShippedOtherCostAmount
        {
            get
            {
                if (shippedQuantity != int.MinValue && totalOtherCost != decimal.MinValue)
                    return shippedQuantity * totalOtherCost;
                else
                    return 0;
            }
        }

		public decimal OrderAmount
		{
			get 
			{
				if (orderQuantity != int.MinValue && sellingPrice != decimal.MinValue)
					return orderQuantity * sellingPrice;
				else
					return 0;
			}
		}

        public decimal OrderAmountAfterDiscount
        {
            get
            {
                if (orderQuantity != int.MinValue && reducedSellingPrice != decimal.MinValue)
                    return orderQuantity * reducedSellingPrice;
                else
                    return 0;
            }
        }

        public decimal POAmount
		{
			get 
			{
				if (poQuantity != int.MinValue && sellingPrice != decimal.MinValue)
					return poQuantity * sellingPrice;
				else
					return 0;
			}
		}

        public decimal POAmountAfterDiscount
        {
            get
            {
                if (poQuantity != int.MinValue && reducedSellingPrice != decimal.MinValue)
                    return poQuantity * reducedSellingPrice;
                else
                    return 0;
            }
        }

        public decimal PONetFOBAmount
		{
			get 
			{
				if (poQuantity != int.MinValue && netFOBPrice != decimal.MinValue)
					return poQuantity * netFOBPrice;
				else
					return 0;
			}
		}

        public decimal PONetFOBAmountAfterDiscount
        {
            get
            {
                if (poQuantity != int.MinValue && reducedNetFOBPrice != decimal.MinValue)
                    return poQuantity * reducedNetFOBPrice;
                else
                    return 0;
            }
        }

        public decimal ShippedAmount
		{
			get 
			{
				if (shippedQuantity != int.MinValue && sellingPrice != decimal.MinValue)
					return shippedQuantity * sellingPrice;
				else
					return 0;
			}
		}

        public decimal ShippedAmountAfterDiscount
        {
            get
            {
                if (shippedQuantity != int.MinValue && reducedSellingPrice != decimal.MinValue)
                    return shippedQuantity * reducedSellingPrice;
                else
                    return 0;
            }
        }

        public decimal ShippedNetFOBAmount
		{
			get 
			{
				if (shippedQuantity != int.MinValue && netFOBPrice != decimal.MinValue)
					return shippedQuantity * netFOBPrice;
				else
					return 0;
			}
		}

        public decimal ShippedNetFOBAmountAfterDiscount
        {
            get
            {
                if (shippedQuantity != int.MinValue && reducedNetFOBPrice != decimal.MinValue)
                    return shippedQuantity * reducedNetFOBPrice;
                else
                    return 0;
            }
        }

        public decimal NetFOBAmount
		{
			get 
			{
				if (orderQuantity != int.MinValue && netFOBPrice != decimal.MinValue)
					return orderQuantity * netFOBPrice;
				else
					return 0;
			}
		}

        public decimal NetFOBAmountAfterDiscount
        {
            get
            {
                if (orderQuantity != int.MinValue && reducedNetFOBPrice != decimal.MinValue)
                    return orderQuantity * reducedNetFOBPrice;
                else
                    return 0;
            }
        }

        public decimal ShippedSupplierGarmentAmount
		{
			get 
			{
				if (shippedQuantity != int.MinValue && supplierGarmentPrice != decimal.MinValue)
					return shippedQuantity * supplierGarmentPrice;
				else
					return 0;
			}
		}

        public decimal ShippedSupplierGarmentAmountAfterDiscount
        {
            get
            {
                if (shippedQuantity != int.MinValue && reducedSupplierGmtPrice != decimal.MinValue)
                    return shippedQuantity * reducedSupplierGmtPrice;
                else
                    return 0;
            }
        }

        public decimal SupplierGarmentAmount
		{
			get 
			{
				if (orderQuantity != int.MinValue && supplierGarmentPrice != decimal.MinValue)
					return orderQuantity * supplierGarmentPrice;
				else
					return 0;
			}
		}

        public decimal SupplierGarmentAmountAfterDiscount
        {
            get
            {
                if (orderQuantity != int.MinValue && reducedSupplierGmtPrice != decimal.MinValue)
                    return orderQuantity * reducedSupplierGmtPrice;
                else
                    return 0;
            }
        }

        public decimal FreightCostAmount
		{
			get 
			{
				if (orderQuantity != int.MinValue && freightCost != decimal.MinValue)
					return orderQuantity * freightCost;
				else
					return 0;
			}
		}

        public decimal ShippedFreightCostAmount
        {
            get
            {
                if (shippedQuantity != int.MinValue && freightCost != decimal.MinValue)
                    return shippedQuantity * freightCost;
                else
                    return 0;
            }
        }

		public decimal DutyCostAmount
		{
			get 
			{
				if (orderQuantity != int.MinValue && dutyCost != decimal.MinValue)
					return orderQuantity * dutyCost;
				else
					return 0;
			}
		}

        public decimal ShippedDutyCostAmount
        {
            get
            {
                if (shippedQuantity != int.MinValue && dutyCost != decimal.MinValue)
                    return shippedQuantity * dutyCost;
                else
                    return 0;
            }
        }

        public int GSPFormTypeId
		{
			get { return gspFormTypeId; }
			set { gspFormTypeId = value; }
		}

		public int ShippingGSPFormTypeId
		{
			get { return shippingGSPFormTypeId; }
			set { shippingGSPFormTypeId = value; }
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
			get { return shippedQuantity * opaUpcharge;	}
		}

		public int RatioPack 
        { 
            get { return ratioPack; } 
            set { ratioPack = value; } 
        }

		public decimal RetailSellingPrice 
        { 
            get { return retailSellingPrice; } 
            set { retailSellingPrice = value; } 
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
		
		public decimal FreightCost { 
            get { return freightCost; } 
            set { freightCost = value; } 
        }

		public decimal DutyCost 
        { 
            get { return dutyCost; } 
            set { dutyCost = value; } 
        }

		public decimal FobUTSurchargeUSD 
        { 
            get { return fobUTSurchargeUSD; } 
            set { fobUTSurchargeUSD = value; } 
        }

		public decimal CMPriceUSD 
        { 
            get { return cmPriceUSD; } 
            set { cmPriceUSD = value; } 
        }

        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public ArrayList OtherCost
        {
            get { return otherCost; }
            set { otherCost = value; }
        }
	}
}
