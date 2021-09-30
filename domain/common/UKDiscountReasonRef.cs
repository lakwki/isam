using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class UKDiscountReasonRef : DomainData
	{
		private int ukDiscountReasonId;
		private string ukDiscountReason;

		public UKDiscountReasonRef()
		{
		}

		public int UKDiscountReasonId
		{
			get { return ukDiscountReasonId; }
			set { ukDiscountReasonId = value; }
		}

		public string UKDiscountReason
		{
			get { return ukDiscountReason; }
			set { ukDiscountReason = value; }
		}
	}
}

