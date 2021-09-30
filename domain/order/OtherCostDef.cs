using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class OtherCostDef : DomainData
	{
		private int otherCostId;
		private int shipmentDetailId;
		private OtherCostTypeRef otherCostType;
		private CurrencyRef currency;
		private int vendorId;
		private decimal poExchangeRate;
		private decimal otherCostAmount;

		public OtherCostDef()
		{
		}

		public int OtherCostId
		{
			get { return otherCostId; }
			set { otherCostId = value; }
		}

		public int ShipmentDetailId
		{
			get { return shipmentDetailId; }
			set { shipmentDetailId = value; }
		}

		public OtherCostTypeRef OtherCostType
		{
			get { return otherCostType; }
			set { otherCostType = value; }
		}

		public CurrencyRef Currency
		{
			get { return currency; }
			set { currency = value; }
		}

		public int VendorId
		{
			get { return vendorId; }
			set { vendorId = value; }
		}

		public decimal POExchangeRate
		{
			get { return poExchangeRate; }
			set	{ poExchangeRate = value; }
		}

		public decimal OtherCostAmount
		{
			get { return otherCostAmount; }
			set { otherCostAmount = value; }
		}
	}
}
