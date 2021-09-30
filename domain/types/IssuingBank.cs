using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;



namespace com.next.isam.domain.types
{
    [Serializable()]

    public class IssuingBank : DomainData
    {
        public static IssuingBank HSBC = new IssuingBank(Code.HSBC);
        public static IssuingBank SCB = new IssuingBank(Code.SCB);
        public static int DefaultBankId = SCB.Id; //HSBC.Id;
        public static IssuingBank DefaultBankCode = SCB;

        private Code _code;

        private enum Code
        {
            HSBC = 1,
            SCB = 2
        }

        private IssuingBank(Code code)
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

        public string ShortName
        {
            get
            {
                switch (_code)
                {
                    case Code.HSBC:
                        return "HSBC";
                    case Code.SCB:
                        return "SCB";
                    default:
                        return "ERROR";
                }
            }
        }

        public string Name
        {
            get
            {
                switch (_code)
                {
                    case Code.HSBC:
                        return "HONG KONG BANK";
                    case Code.SCB:
                        return "CHARTERED BANK";
                    default:
                        return "ERROR";
                }
            }
        }

        public string FullName
        {
            get
            {
                switch (_code)
                {
                    case Code.HSBC:
                        return "HONGKONG AND SHANGHAI BANKING CORP. LTD.";
                    case Code.SCB:
                        return "STANDARD CHARTERED BANK";
                    default:
                        return "ERROR";
                }
            }
        }

        public static IssuingBank getType(int id)
        {
            if (id == Code.HSBC.GetHashCode()) return IssuingBank.HSBC;
            else if (id == Code.SCB.GetHashCode()) return IssuingBank.SCB;
            else return null;
        }


        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(IssuingBank.HSBC);
            list.Add(IssuingBank.SCB);
            return list;
        }

    }
}
