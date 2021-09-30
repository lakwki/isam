using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class PaymentDeductionType : DomainData
    {
        public static PaymentDeductionType NOT_AVAILIABLE = new PaymentDeductionType(Code.NotAvailable);
        public static PaymentDeductionType DEBIT_NOTE = new PaymentDeductionType(Code.DebitNote);
        public static PaymentDeductionType CREDIT_NOTE = new PaymentDeductionType(Code.CreditNote);
        public static PaymentDeductionType NEXT_CLAIM_DEBIT_NOTE = new PaymentDeductionType(Code.NextClaimDebitNote);
        public static PaymentDeductionType NEXT_CLAIM_CREDIT_NOTE = new PaymentDeductionType(Code.NextClaimCreditNote);
        public static PaymentDeductionType AIR_FREIGHT = new PaymentDeductionType(Code.AirFreight);
        public static PaymentDeductionType FABRIC_ADVANCE = new PaymentDeductionType(Code.FabricAdvance);
        public static PaymentDeductionType FABRIC_LIABILITY = new PaymentDeductionType(Code.FabricLiability);
        public static PaymentDeductionType FABRIC_UTILIZATION = new PaymentDeductionType(Code.FabricUtilization);
        public static PaymentDeductionType OTHERS_DEBIT = new PaymentDeductionType(Code.OthersDebit);
        public static PaymentDeductionType OTHERS_CREDIT = new PaymentDeductionType(Code.OthersCredit);
        public static PaymentDeductionType C19 = new PaymentDeductionType(Code.C19);
        public static PaymentDeductionType REMARK = new PaymentDeductionType(Code.Remark);

        private Code _code;

        private enum Code
        {
            NotAvailable = 0,
            DebitNote = 1,
            CreditNote = 2,
            NextClaimDebitNote = 3,
            NextClaimCreditNote = 4,
            AirFreight = 5,
            FabricAdvance = 6,
            FabricLiability = 7,
            FabricUtilization = 8,
            OthersDebit = 9,
            OthersCredit = 10,
            C19 = 11,
            Remark = 12
        }


        private PaymentDeductionType(Code code)
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
                    case Code.NotAvailable:
                        return "N/A";
                    case Code.DebitNote:
                        return "Debit Note";
                    case Code.CreditNote:
                        return "Credit Note";
                    case Code.NextClaimDebitNote:
                        return "Next Claim D/N";
                    case Code.NextClaimCreditNote:
                        return "Next Claim C/N";
                    case Code.AirFreight:
                        return "Air Freight";
                    case Code.FabricAdvance:
                        return "Fabric Advance";
                    case Code.FabricLiability:
                        return "Fabric Liability";
                    case Code.FabricUtilization:
                        return "Fabric Utilization";
                    case Code.OthersDebit:
                        return "Others - Debit";
                    case Code.OthersCredit:
                        return "Others - Credit";
                    case Code.C19:
                        return "C19";
                    case Code.Remark:
                        return "Remark";
                    default:
                        return "ERROR";
                }
            }
        }

        public static PaymentDeductionType getType(int id)
        {
            if (id == Code.NotAvailable.GetHashCode()) return PaymentDeductionType.NOT_AVAILIABLE;
            else if (id == Code.DebitNote.GetHashCode()) return PaymentDeductionType.DEBIT_NOTE;
            else if (id == Code.CreditNote.GetHashCode()) return PaymentDeductionType.CREDIT_NOTE;
            else if (id == Code.NextClaimDebitNote.GetHashCode()) return PaymentDeductionType.NEXT_CLAIM_DEBIT_NOTE;
            else if (id == Code.NextClaimCreditNote.GetHashCode()) return PaymentDeductionType.NEXT_CLAIM_CREDIT_NOTE;
            else if (id == Code.AirFreight.GetHashCode()) return PaymentDeductionType.AIR_FREIGHT;
            else if (id == Code.FabricAdvance.GetHashCode()) return PaymentDeductionType.FABRIC_ADVANCE;
            else if (id == Code.FabricLiability.GetHashCode()) return PaymentDeductionType.FABRIC_LIABILITY;
            else if (id == Code.FabricUtilization.GetHashCode()) return PaymentDeductionType.FABRIC_UTILIZATION;
            else if (id == Code.OthersDebit.GetHashCode()) return PaymentDeductionType.OTHERS_DEBIT;
            else if (id == Code.OthersCredit.GetHashCode()) return PaymentDeductionType.OTHERS_CREDIT;
            else if (id == Code.C19.GetHashCode()) return PaymentDeductionType.C19;
            else if (id == Code.Remark.GetHashCode()) return PaymentDeductionType.REMARK;
            else return null;
        }

        public int Factor
        {
            get
            {
                return ((_code == Code.NotAvailable || _code == Code.Remark) ? 0 : (_code == Code.CreditNote || _code == Code.NextClaimCreditNote || _code == Code.FabricLiability || _code == Code.OthersCredit ? -1 : 1));
            }
        }

        private static int getId(Code c)
        {
            return Convert.ToUInt16(c.GetHashCode());
        }

        public static ICollection GetDeductionTypeList()
        {
            ArrayList list = new ArrayList();
            list.Add(NOT_AVAILIABLE);
            list.Add(DEBIT_NOTE);
            list.Add(CREDIT_NOTE);
            list.Add(NEXT_CLAIM_DEBIT_NOTE);
            list.Add(NEXT_CLAIM_CREDIT_NOTE);
            list.Add(AIR_FREIGHT);
            list.Add(FABRIC_ADVANCE);
            list.Add(FABRIC_LIABILITY);
            list.Add(FABRIC_UTILIZATION);
            list.Add(OTHERS_DEBIT);
            list.Add(OTHERS_CREDIT);
            list.Add(C19);
            list.Add(REMARK);
            return list;
        }

        // extend properties
        public bool RequireDocumentNo
        {
            get { return !(_code == Code.NotAvailable || _code == Code.AirFreight || _code == Code.Remark); }
        }
    }
}
