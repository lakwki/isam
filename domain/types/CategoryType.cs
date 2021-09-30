using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class CategoryType : DomainData
    {
        public static CategoryType ACTUAL = new CategoryType(Code.Actual);
        public static CategoryType ACCRUAL = new CategoryType(Code.Accrual);
        public static CategoryType REALIZED = new CategoryType(Code.Realized);
        public static CategoryType DAILY = new CategoryType(Code.Daily);
        public static CategoryType REVERSAL = new CategoryType(Code.Reversal);
        public static CategoryType CONSOLIDATED = new CategoryType(Code.Consolidated);

        private Code _code;

        private enum Code
        {
            Actual = 1,
            Accrual = 2,
            Realized = 3,
            Daily = 4,
            Reversal = 5,
            Consolidated = 6
        }

        private CategoryType(Code code)
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
                    case Code.Actual:
                        return "ACTUAL";
                    case Code.Accrual:
                        return "ACCRUAL";
                    case Code.Realized:
                        return "REALIZED";
                    case Code.Daily:
                        return "DAILY";
                    case Code.Reversal:
                        return "REVERSAL";
                    case Code.Consolidated:
                        return "CONSOLIDATED";
                    default:
                        return "ERROR";
                }
            }
        }

        public static CategoryType getType(int id)
        {
            if (id == Code.Actual.GetHashCode()) return CategoryType.ACTUAL;
            else if (id == Code.Accrual.GetHashCode()) return CategoryType.ACCRUAL;
            else if (id == Code.Realized.GetHashCode()) return CategoryType.REALIZED;
            else if (id == Code.Daily.GetHashCode()) return CategoryType.DAILY;
            else if (id == Code.Reversal.GetHashCode()) return CategoryType.REVERSAL;
            else if (id == Code.Consolidated.GetHashCode()) return CategoryType.CONSOLIDATED;
            else return null;
        }

        public static ICollection getCollectionValues()
        {
            ArrayList ary = new ArrayList();
            ary.Add(CategoryType.ACTUAL);
            ary.Add(CategoryType.ACCRUAL);
            ary.Add(CategoryType.REALIZED);
            ary.Add(CategoryType.DAILY);
            ary.Add(CategoryType.REVERSAL);
            ary.Add(CategoryType.CONSOLIDATED);
            return ary;
        }

    }
}
