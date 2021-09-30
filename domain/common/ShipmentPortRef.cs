using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class ShipmentPortRef : DomainData
	{
		private int shipmentPortId;
		private string shipmentPortDescription;
		private ShipmentCountryRef shipmentCountry;
		private int opsKey;
        private string officialCode;

		public ShipmentPortRef()
		{
		}

		public int ShipmentPortId
		{
			get { return shipmentPortId; }
			set { shipmentPortId = value; }
		}

		public string ShipmentPortDescription
		{
			get { return shipmentPortDescription; }
			set { shipmentPortDescription = value; }
		}

		public ShipmentCountryRef ShipmentCountry
		{
			get { return shipmentCountry; }
			set { shipmentCountry = value; }
		}

		public int OPSKey
		{
			get { return opsKey; }
			set { opsKey = value; }
		}

        public string OfficialCode
        {
            get { return officialCode; }
            set { officialCode = value; }
        }
	}
}
