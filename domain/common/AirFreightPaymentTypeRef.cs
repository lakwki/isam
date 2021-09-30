using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class AirFreightPaymentTypeRef : DomainData
	{
		private int airFreightPaymentTypeId;
		private string airFreightPaymentTypeDescription;

        public enum typeId
        {
            PrePaidBy = 1,
            Collect = 2            
        }

		public AirFreightPaymentTypeRef()
		{
		}

		public int AirFreightPaymentTypeId
		{
			get { return airFreightPaymentTypeId; }
			set { airFreightPaymentTypeId = value; }
		}

		public string AirFreightPaymentTypeDescription
		{
			get { return airFreightPaymentTypeDescription; }
			set { airFreightPaymentTypeDescription = value; }
		}
	
	}
}
