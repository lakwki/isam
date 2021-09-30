using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class NTFuelType : DomainData
    {
        public static NTFuelType NOTAPPLICABLE = new NTFuelType(Code.NotApplicable);
        public static NTFuelType PETROL = new NTFuelType(Code.Petrol);
        public static NTFuelType DIESEL = new NTFuelType(Code.Diesel);

        private Code _code;

        private enum Code
        {
            NotApplicable = 0,
            Petrol = 1,
            Diesel = 2
        }

        private NTFuelType(Code code)
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
                    case Code.NotApplicable:
                        return "Not Applicable";
                    case Code.Petrol:
                        return "Petrol";
                    case Code.Diesel:
                        return "Diesel";
                    default:
                        return "Undefined";
                }
            }
        }

        public static NTFuelType getType(int id)
        {
            if (id == Code.NotApplicable.GetHashCode()) return NTFuelType.NOTAPPLICABLE;
            else if (id == Code.Petrol.GetHashCode()) return NTFuelType.PETROL;
            else if (id == Code.Diesel.GetHashCode()) return NTFuelType.DIESEL;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(NTFuelType.PETROL);
            list.Add(NTFuelType.DIESEL);
            return list;
        }

        public static ArrayList getCollectionValuesByOfficeId(int officeId)
        {
            ArrayList list = new ArrayList();
            list.Add(NTFuelType.PETROL);
            if (officeId == OfficeId.TR.Id || officeId == OfficeId.BD.Id || officeId == OfficeId.SL.Id || officeId == OfficeId.ND.Id || officeId == OfficeId.IND.Id)
                list.Add(NTFuelType.DIESEL);
            return list;
        }

    }
}
