using System;
using System.Collections.Generic;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimType : DomainData
    {
        public static UKClaimType REWORK = new UKClaimType(Code.Rework);
        public static UKClaimType REJECT = new UKClaimType(Code.Reject);
        public static UKClaimType MFRN = new UKClaimType(Code.MFRN);
        public static UKClaimType CFS = new UKClaimType(Code.CFS);
        public static UKClaimType SAFTETY_ISSUE = new UKClaimType(Code.SafetyIssue);
        public static UKClaimType AUDIT_FEE = new UKClaimType(Code.AuditFee);
        public static UKClaimType FABRIC_TEST = new UKClaimType(Code.FabricTest);
        public static UKClaimType PENALTY_CHARGE = new UKClaimType(Code.PenaltyCharge);
        public static UKClaimType BILL_IN_ADVANCE = new UKClaimType(Code.BillInAdvance);
        public static UKClaimType QCC = new UKClaimType(Code.QCC);
        public static UKClaimType CHB = new UKClaimType(Code.CHB);
        public static UKClaimType GB_TEST = new UKClaimType(Code.GBTest);
        public static UKClaimType FIRA_TEST = new UKClaimType(Code.FIRATest);
        public static UKClaimType OTHERS = new UKClaimType(Code.OTHERS);

        private Code _code;

        public enum Code
        {
            Rework = 1,
            Reject = 2,
            MFRN = 3,
            CFS = 4,
            SafetyIssue = 5,
            AuditFee = 6,
            FabricTest = 7,
            PenaltyCharge = 8,
            BillInAdvance = 9,
            QCC = 10,
            CHB = 11,
            GBTest = 12,
            FIRATest = 13,
            OTHERS = 14
        }

        public UKClaimType()
        {
        }

        private UKClaimType(Code code)
        {
            this._code = code;
        }

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_code.GetHashCode());
            }
            set { value = 1; }
        }

        public string Name
        {
            get
            {
                switch (_code)
                {
                    case Code.Rework:
                        return "Rework";
                    case Code.Reject:
                        return "Reject";
                    case Code.MFRN:
                        return "MFRN";
                    case Code.CFS:
                        return "CFS";
                    case Code.SafetyIssue:
                        return "Safety Issue";
                    case Code.AuditFee:
                        return "Audit Fee";
                    case Code.FabricTest:
                        return "Fabric Test";
                    case Code.PenaltyCharge:
                        return "Penalty Charge";
                    case Code.BillInAdvance:
                        return "Bill In Advance";
                    case Code.QCC:
                        return "QCC";
                    case Code.CHB:
                        return "CHB";
                    case Code.GBTest:
                        return "GB Testing Charge";
                    case Code.FIRATest:
                        return "FIRA Test";
                    case Code.OTHERS:
                        return "Temp";
                    default:
                        return "ERROR";
                }
            }
            set { value = string.Empty; }
        }

        public string T5Code
        {
            get
            {
                switch (_code)
                {
                    case Code.Rework:
                        return "RFS";
                    case Code.Reject:
                        return "RFS";
                    case Code.MFRN:
                        return "DN";
                    case Code.CFS:
                        return "CFS";
                    case Code.SafetyIssue:
                        return "SAF";
                    case Code.AuditFee:
                        return "AUD";
                    case Code.FabricTest:
                        return "DN";
                    case Code.PenaltyCharge:
                        return "DN";
                    case Code.BillInAdvance:
                        return string.Empty;
                    case Code.QCC:
                        return "QCC";
                    case Code.CHB:
                        return "CHB";
                    case Code.GBTest:
                        return "GBTEST";
                    case Code.FIRATest:
                        return "FIRA";
                    case Code.OTHERS:
                        return "DN";
                    default:
                        return "ERROR";
                }
            }
        }

        public string DMSDescription
        {
            get
            {
                switch (_code)
                {
                    case Code.Rework:
                        return "Rework";
                    case Code.Reject:
                        return "Reject";
                    case Code.MFRN:
                        return "MFRN";
                    case Code.CFS:
                        return "CFS";
                    case Code.SafetyIssue:
                        return "Safety Issue";
                    case Code.AuditFee:
                        return "Audit Fee";
                    case Code.FabricTest:
                        return "Fabric Test";
                    case Code.PenaltyCharge:
                        return "Penalty Charge";
                    case Code.BillInAdvance:
                        return "Bill In Advance";
                    case Code.QCC:
                        return "QCC";
                    case Code.CHB:
                        return "CHB";
                    case Code.GBTest:
                        return "GB Testing Charge";
                    case Code.FIRATest:
                        return "FIRA Test";
                    case Code.OTHERS:
                        return "Temp";
                    default:
                        return "ERROR";
                }
            }
        }

        public static bool isTechTeamRelated(int id)
        {
            if (id == Code.Rework.GetHashCode() 
                || id == Code.Reject.GetHashCode() 
                || id == Code.MFRN.GetHashCode() 
                || id == Code.CFS.GetHashCode() 
                || id == Code.SafetyIssue.GetHashCode()
                || id == Code.CHB.GetHashCode()
                || id == Code.QCC.GetHashCode()
                || id == Code.FabricTest.GetHashCode()
                || id == Code.PenaltyCharge.GetHashCode()
                || id == Code.FIRATest.GetHashCode()) 
                return true;
            return false;
        }

        public static UKClaimType getType(int id)
        {
            if (id == Code.Rework.GetHashCode()) return UKClaimType.REWORK;
            else if (id == Code.Reject.GetHashCode()) return UKClaimType.REJECT;
            else if (id == Code.MFRN.GetHashCode()) return UKClaimType.MFRN;
            else if (id == Code.CFS.GetHashCode()) return UKClaimType.CFS;
            else if (id == Code.SafetyIssue.GetHashCode()) return UKClaimType.SAFTETY_ISSUE;
            else if (id == Code.AuditFee.GetHashCode()) return UKClaimType.AUDIT_FEE;
            else if (id == Code.FabricTest.GetHashCode()) return UKClaimType.FABRIC_TEST;
            else if (id == Code.PenaltyCharge.GetHashCode()) return UKClaimType.PENALTY_CHARGE;
            else if (id == Code.BillInAdvance.GetHashCode()) return UKClaimType.BILL_IN_ADVANCE;
            else if (id == Code.QCC.GetHashCode()) return UKClaimType.QCC;
            else if (id == Code.CHB.GetHashCode()) return UKClaimType.CHB;
            else if (id == Code.GBTest.GetHashCode()) return UKClaimType.GB_TEST;
            else if (id == Code.FIRATest.GetHashCode()) return UKClaimType.FIRA_TEST;
            else if (id == Code.OTHERS.GetHashCode()) return UKClaimType.OTHERS;
            else return null;
        }

        public static UKClaimType getType(string textFromQAIS)
        {
            if (textFromQAIS == "Rework") return UKClaimType.REWORK;
            else if (textFromQAIS == "Reject") return UKClaimType.REJECT;
            else if (textFromQAIS == "MFRN") return UKClaimType.MFRN;
            else if (textFromQAIS == "CFS") return UKClaimType.CFS;
            else if (textFromQAIS == "Safety") return UKClaimType.SAFTETY_ISSUE;
            else if (textFromQAIS == "QCC") return UKClaimType.QCC;
            else if (textFromQAIS == "CHB") return UKClaimType.CHB;
            else if (textFromQAIS == "PenaltyCharge") return UKClaimType.PENALTY_CHARGE;
            else if (textFromQAIS == "FIRATest") return UKClaimType.FIRA_TEST;
            else return null;
        }

        public static List<UKClaimType> getCollectionValues()
        {
            List<UKClaimType> list = new List<UKClaimType>();
            list.Add(UKClaimType.REWORK);
            list.Add(UKClaimType.REJECT);
            list.Add(UKClaimType.MFRN);
            list.Add(UKClaimType.CFS);
            list.Add(UKClaimType.SAFTETY_ISSUE);
            list.Add(UKClaimType.AUDIT_FEE);
            list.Add(UKClaimType.FABRIC_TEST);
            list.Add(UKClaimType.PENALTY_CHARGE);
            list.Add(UKClaimType.BILL_IN_ADVANCE);
            list.Add(UKClaimType.QCC);
            list.Add(UKClaimType.CHB);
            list.Add(UKClaimType.GB_TEST);
            list.Add(UKClaimType.FIRA_TEST);
            list.Add(UKClaimType.OTHERS);
            return list;
        }

        public static UKClaimType getClaimType(string prefix, string text)
        {
            if (prefix == "DN") return UKClaimType.MFRN;
            else if (prefix == "F" && (text == "Safety Issue" || text == "Metal Contamination")) return UKClaimType.SAFTETY_ISSUE;
            else if (prefix == "F" && text == "QC Fail") return UKClaimType.QCC;
            else if (prefix == "F" && text == "Fine (Percentage)") return UKClaimType.FABRIC_TEST;
            else if (prefix == "FRA" && text == "FIRA") return UKClaimType.FIRA_TEST;
            else if (prefix == "CFS" && text == "Supplier Compliance") return UKClaimType.CFS;
            else if ((prefix == "DFS" || prefix == "PFS" || prefix == "RFS") && text == "Rejected Stock") return UKClaimType.REJECT;
            else if ((prefix == "DFS" || prefix == "PFS" || prefix == "RFS") && text == "Rework") return UKClaimType.REWORK;
            else return null;
        }


    }
}
