using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class DebitNoteVoidType : DomainData
    {
        public static DebitNoteVoidType NS_PROVISION = new DebitNoteVoidType(Code.NSProvision);
        public static DebitNoteVoidType CHANGE_TO_OTHER_SUPPLIER = new DebitNoteVoidType(Code.ChangeToOtherSupplier);
        private Code _code;

        private enum Code
        {
            NSProvision = 1,
            ChangeToOtherSupplier = 2
        }

        private DebitNoteVoidType(Code code)
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
                    case Code.NSProvision:
                        return "NS Provision";
                    case Code.ChangeToOtherSupplier:
                        return "Change to other supplier";
                    default:
                        return "ERROR";
                }
            }
        }

        public string SelectedValue
        {
            get
            {
                switch (_code)
                {
                    case Code.NSProvision:
                        return "NSPROVISION";
                    case Code.ChangeToOtherSupplier:
                        return "SUPPLIER";
                    default:
                        return "ERROR";
                }
            }
        }

        public string getVoidTypeValueInDC(string debitCreditIndicator)
        {
            if (String.IsNullOrEmpty(debitCreditIndicator) || debitCreditIndicator.Length != 1)
                return "ERROR";
            debitCreditIndicator = debitCreditIndicator.Trim().ToUpper();
            if (!"DC".Contains(debitCreditIndicator.ToUpper()))
                return "ERROR";
            bool debitFlag = (debitCreditIndicator == "D"); // false => "C";
            switch (_code)
            {
                case Code.NSProvision:
                    return (debitFlag) ? "NSCOST" : "NSPROVISION";
                case Code.ChangeToOtherSupplier:
                    return "SUPPLIER-COST";
                    /*
                    return (debitFlag) ? "SUPPLIER-COST" : "ERROR";
                    */
                default:
                    return "ERROR";
            }
        }

        public static DebitNoteVoidType getType(int id)
        {
            if (id == Code.NSProvision.GetHashCode()) return DebitNoteVoidType.NS_PROVISION;
            else if (id == Code.ChangeToOtherSupplier.GetHashCode()) return DebitNoteVoidType.CHANGE_TO_OTHER_SUPPLIER;
            else return null;
        }
    }
}
