using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class ShipmentMethodRef : DomainData
	{
		private int shipmentMethodId;
		private string shipmentMethodDescription;
		private string opsKey;

        public enum Method
        {
            AIR = 1,
            SEA = 2,
            SEAorAIR = 3,
            TRUCK = 4,
            ECOAIR = 5,
            TRAIN = 6

        }

		public ShipmentMethodRef()
		{
		}

		public int ShipmentMethodId
		{
			get { return shipmentMethodId; }
			set { shipmentMethodId = value; }
		}

		public string ShipmentMethodDescription
		{
			get { return shipmentMethodDescription; }
			set { shipmentMethodDescription = value; }
		}

		public string OPSKey
		{
			get { return opsKey; }
			set { opsKey = value; }
		}

        public static int getShipmentMethodId(string opsKey)
        {
            if (opsKey == "A")
                return Method.AIR.GetHashCode();
            else if (opsKey == "S")
                return Method.SEA.GetHashCode();
            else if (opsKey == "X")
                return Method.SEAorAIR.GetHashCode();
            else if (opsKey == "L")
                return Method.TRUCK.GetHashCode();
            else if (opsKey == "E")
                return Method.ECOAIR.GetHashCode();
            else if (opsKey == "T")
                return Method.TRAIN.GetHashCode();

            return -1;
        }

        public static int getShipmentMethodIdByDescription(string shipmentMethodDesc)
        {
            if (shipmentMethodDesc == "A" || shipmentMethodDesc == "AIR")
                return Method.AIR.GetHashCode();
            else if (shipmentMethodDesc == "S" || shipmentMethodDesc == "SEA")
                return Method.SEA.GetHashCode();
            else if (shipmentMethodDesc == "X" || shipmentMethodDesc == "SEA/AIR" || shipmentMethodDesc == "SEAorAIR")
                return Method.SEAorAIR.GetHashCode();
            else if (shipmentMethodDesc == "L" || shipmentMethodDesc == "LAND" || shipmentMethodDesc == "TRUCK")
                return Method.TRUCK.GetHashCode();
            else if (shipmentMethodDesc == "E" || shipmentMethodDesc == "ECOAIR")
                return Method.ECOAIR.GetHashCode();
            else if (shipmentMethodDesc == "T" || shipmentMethodDesc == "TRAIN")
                return Method.TRAIN.GetHashCode();

            return -1;
        }

	}
}
