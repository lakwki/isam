using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class ConsumptionType : DomainData
    {
        public static ConsumptionType KILOWATT_PER_HOUR = new ConsumptionType(Code.KiloWattPerHour);
        public static ConsumptionType US_GALLON = new ConsumptionType(Code.USGallon);
        public static ConsumptionType CUBIC_METER = new ConsumptionType(Code.CubicMeter);
        public static ConsumptionType CUBIC_FEET = new ConsumptionType(Code.CubicFeet);
        public static ConsumptionType MILE = new ConsumptionType(Code.Mile);
        public static ConsumptionType KILOMETER = new ConsumptionType(Code.KiloMeter);
        public static ConsumptionType KILOGRAM = new ConsumptionType(Code.Kilogram);
        public static ConsumptionType LITRE = new ConsumptionType(Code.Litre);
        public static ConsumptionType KILOLITRE = new ConsumptionType(Code.KiloLitre);


        private Code _code;

        private enum Code
        {
            KiloWattPerHour = 1,
            USGallon = 2,
            CubicMeter = 3,
            CubicFeet = 4,
            Mile = 5,
            KiloMeter = 6,
            Kilogram = 7,
            Litre = 8,
            KiloLitre = 9
        }

        private ConsumptionType(Code code)
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
                    case Code.KiloWattPerHour:
                        return "KiloWatt Per Hour";
                    case Code.USGallon:
                        return "US Gallon";
                    case Code.CubicMeter:
                        return "Cubic Meter";
                    case Code.CubicFeet:
                        return "Cubic Feet";
                    case Code.Mile:
                        return "Mile";
                    case Code.KiloMeter:
                        return "Kilometer";
                    case Code.Kilogram:
                        return "Kilogram";
                    case Code.Litre:
                        return "Litre";
                    case Code.KiloLitre:
                        return "KiloLitre";
                    default:
                        return "ERROR";
                }
            }
        }

        public static ConsumptionType getType(int id)
        {
            if (id == Code.KiloWattPerHour.GetHashCode()) return ConsumptionType.KILOWATT_PER_HOUR;
            else if (id == Code.USGallon.GetHashCode()) return ConsumptionType.US_GALLON;
            else if (id == Code.CubicMeter.GetHashCode()) return ConsumptionType.CUBIC_METER;
            else if (id == Code.CubicFeet.GetHashCode()) return ConsumptionType.CUBIC_FEET;
            else if (id == Code.Mile.GetHashCode()) return ConsumptionType.MILE;
            else if (id == Code.KiloMeter.GetHashCode()) return ConsumptionType.KILOMETER;
            else if (id == Code.Kilogram.GetHashCode()) return ConsumptionType.KILOGRAM;
            else if (id == Code.Litre.GetHashCode()) return ConsumptionType.LITRE;
            else if (id == Code.KiloLitre.GetHashCode()) return ConsumptionType.KILOLITRE;
            else return null;
        }

        public static ICollection getCollectionValues()
        {
            ArrayList ary = new ArrayList();
            ary.Add(ConsumptionType.KILOWATT_PER_HOUR);
            //ary.Add(ConsumptionType.US_GALLON);
            ary.Add(ConsumptionType.CUBIC_METER);
            //ary.Add(ConsumptionType.CUBIC_FEET);
            ary.Add(ConsumptionType.MILE);
            ary.Add(ConsumptionType.KILOMETER);
            ary.Add(ConsumptionType.KILOGRAM);
            ary.Add(ConsumptionType.LITRE);
            ary.Add(ConsumptionType.KILOLITRE);
            return ary;
        }

        public static ICollection getCollectionValuesForMileage()
        {
            ArrayList ary = new ArrayList();
            ary.Add(ConsumptionType.MILE);
            ary.Add(ConsumptionType.KILOMETER);
            ary.Add(ConsumptionType.LITRE);
            return ary;
        }

        public static ICollection getCollectionValuesForUtilities()
        {
            ArrayList ary = new ArrayList();
            ary.Add(ConsumptionType.KILOWATT_PER_HOUR);
            //ary.Add(ConsumptionType.US_GALLON);
            ary.Add(ConsumptionType.CUBIC_METER);
            //ary.Add(ConsumptionType.CUBIC_FEET);
            ary.Add(ConsumptionType.KILOGRAM);
            ary.Add(ConsumptionType.LITRE);
            ary.Add(ConsumptionType.KILOLITRE);
            return ary;
        }

    }
}
