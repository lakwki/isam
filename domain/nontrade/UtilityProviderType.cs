using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class UtilityProviderType : DomainData
    {
        public static UtilityProviderType ELECTRICITY = new UtilityProviderType(Code.Electricity);
        public static UtilityProviderType WATER = new UtilityProviderType(Code.Water);
        public static UtilityProviderType GAS = new UtilityProviderType(Code.Gas);
        public static UtilityProviderType CAR_FUEL = new UtilityProviderType(Code.CarFuel);
        public static UtilityProviderType PARKING = new UtilityProviderType(Code.Parking);

        private Code _code;

        private enum Code
        {
            Electricity = 1,
            Water = 2,
            Gas = 3,
            CarFuel = 4,
            Parking = 5

        }

        private UtilityProviderType(Code code)
        {
            this._code = code;
        }

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_code.GetHashCode());
            }
        }

        public string Name
        {
            get
            {
                switch (_code)
                {
                    case Code.Electricity:
                        return "Electricity";
                    case Code.Water:
                        return "Water";
                    case Code.Gas:
                        return "Gas";
                    case Code.CarFuel:
                        return "Car Fuel";
                    case Code.Parking:
                        return "Parking";
                    default:
                        return "ERROR";
                }
            }
        }

        public static UtilityProviderType getType(int id)
        {
            if (id == Code.Electricity.GetHashCode()) return UtilityProviderType.ELECTRICITY;
            else if (id == Code.Water.GetHashCode()) return UtilityProviderType.WATER;
            else if (id == Code.Gas.GetHashCode()) return UtilityProviderType.GAS;
            else if (id == Code.CarFuel.GetHashCode()) return UtilityProviderType.CAR_FUEL;
            else if (id == Code.Parking.GetHashCode()) return UtilityProviderType.PARKING;
            else return null;
        }

        public static ICollection getCollectionValues()
        {
            ArrayList ary = new ArrayList();
            ary.Add(UtilityProviderType.ELECTRICITY);
            ary.Add(UtilityProviderType.WATER);
            ary.Add(UtilityProviderType.GAS);
            ary.Add(UtilityProviderType.CAR_FUEL);
            ary.Add(UtilityProviderType.PARKING);
            return ary;
        }

    }
}
