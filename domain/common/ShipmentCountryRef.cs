using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class ShipmentCountryRef : DomainData
	{
		private int shipmentCountryId;
		private string shipmentCountryDescription;
		private string opsKey;

		public enum Id
		{
			china = 1,
			srilanka = 2,
			hk = 3,
			uk = 15,
			turkey = 25,
		}

		public ShipmentCountryRef()
		{
		}

		public int ShipmentCountryId
		{
			get { return shipmentCountryId; }
			set { shipmentCountryId = value; }
		}

		public string ShipmentCountryDescription
		{
			get { return shipmentCountryDescription; }
			set { shipmentCountryDescription = value; }
		}

		public string OPSKey
		{
			get { return opsKey; }
			set { opsKey = value; }
		}
	}
}
