using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class SplitOtherCostDef : DomainData
	{
		private int otherCostId;
		private int splitShipmentDetailId;
		private OtherCostTypeRef otherCostType;
		private CurrencyRef currency;
		private decimal poExchangeRate;
		private decimal otherCostAmount;

		public SplitOtherCostDef()
		{

		}

		public int OtherCostId
		{
			get { return otherCostId; }
			set { otherCostId = value; }
		}

		public int SplitShipmentDetailId
		{
			get { return splitShipmentDetailId; }
			set { splitShipmentDetailId = value; }
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

		public decimal POExchangeRate
		{
			get { return poExchangeRate; }
			set { poExchangeRate = value; }
		}

		public decimal OtherCostAmount
		{
			get { return otherCostAmount; }
			set { otherCostAmount = value; }
		}

	}
}
