using System;
using System.Collections;
using com.next.common.domain;


namespace com.next.isam.domain.types
{
    [Serializable()]
    public class NextRetailDivision : DomainData 
    {

        public static NextRetailDivision BRAND_FINANCE = new NextRetailDivision(Code.BrandFinance);
        public static NextRetailDivision COP = new NextRetailDivision(Code.COP);
        public static NextRetailDivision PAT = new NextRetailDivision(Code.PAT);

        private Code _code;

        private enum Code
        {
            BrandFinance = 1,
            COP = 2,
            PAT = 3
        }

        private NextRetailDivision(Code code)
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

        public string Description
        {
            get
            {
                switch (_code)
                {
                    case Code.BrandFinance :
                        return "Brand Finance";                    
                    case Code.COP :
                        return "COP";
                    case Code.PAT:
                        return "PAT";
                    default:
                        return "UNDEFINED";
                }
            }
        }


        public static NextRetailDivision getType(int id)
        {
            if (id == Code.BrandFinance.GetHashCode()) return NextRetailDivision.BRAND_FINANCE;
            else if (id == Code.COP.GetHashCode()) return NextRetailDivision.COP;
            else if (id == Code.PAT.GetHashCode()) return NextRetailDivision.PAT;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(NextRetailDivision.BRAND_FINANCE);
            list.Add(NextRetailDivision.COP);
            list.Add(NextRetailDivision.PAT);
            return list;
        }
    }
}
