using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class NTPaymentMethodRef : DomainData
    {
        public static NTPaymentMethodRef CHEQUE = new NTPaymentMethodRef(Code.cheque);
        public static NTPaymentMethodRef TT = new NTPaymentMethodRef(Code.tt);
        public static NTPaymentMethodRef CASH = new NTPaymentMethodRef(Code.cash);
        public static NTPaymentMethodRef LC = new NTPaymentMethodRef(Code.lc);
        public static NTPaymentMethodRef AUTOPAY = new NTPaymentMethodRef(Code.autopay);

        private Code _code;

        private enum Code
        {
            cheque = 1,
            tt = 2,
            cash = 3,
            lc = 4,
            autopay = 5
        }

        private NTPaymentMethodRef(Code code)
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
                    case Code.cheque:
                        return "Cheque";
                    case Code.tt:
                        return "TT";
                    case Code.cash :
                        return "Cash";
                    case Code.lc :
                        return "L/C";
                    case Code.autopay :
                        return "Autopay";
                    default:
                        return "UNDEFINED";
                }
            }
        }

        public string T6Code
        {
            get
            {
                switch (_code)
                {
                    case Code.cheque:
                        return "Q";
                    case Code.tt:
                        return "O";
                    case Code.cash:
                        return "C";
                    case Code.lc:
                        return "L";
                    case Code.autopay:
                        return "A";
                    default:
                        return string.Empty;
                }
            }
        }

        public int EpicorPaymentMethodId
        {
            get
            {
                switch (_code)
                {
                    case Code.cheque:
                        return 1;
                    case Code.tt:
                        return 11;
                    case Code.cash:
                        return 3;
                    case Code.lc:
                        return 4;
                    case Code.autopay:
                        return 5;
                    default:
                        return -1;
                }
            }
        }

        public static NTPaymentMethodRef getType(int id)
        {
            if (id == Code.cheque.GetHashCode()) return NTPaymentMethodRef.CHEQUE;
            else if (id == Code.tt.GetHashCode()) return NTPaymentMethodRef.TT;
            else if (id == Code.cash.GetHashCode()) return NTPaymentMethodRef.CASH;
            else if (id == Code.lc.GetHashCode()) return NTPaymentMethodRef.LC;
            else if (id == Code.autopay.GetHashCode()) return NTPaymentMethodRef.AUTOPAY;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            return getCollectionValues(-1);
        }

        public static ArrayList getCollectionValues(int officeId)
        {
            ArrayList list = new ArrayList();
            list.Add(NTPaymentMethodRef.CHEQUE);
            list.Add(NTPaymentMethodRef.TT);
            if (officeId != OfficeId.HK.Id && officeId != OfficeId.CA.Id && officeId != OfficeId.TH.Id && officeId != OfficeId.VN.Id)
                list.Add(NTPaymentMethodRef.CASH);
            return list;
        }
    }
}
