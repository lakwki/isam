using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class ClaimType : DomainData
    {
        public static ClaimType AUDIT_FEE = new ClaimType(Code.AuditFee);
        public static ClaimType SUPPLIER_COMPLIANCE = new ClaimType(Code.SupplierCompliance);
        public static ClaimType NO_SAMPLE = new ClaimType(Code.NoSample);
        public static ClaimType DIFFERENCE = new ClaimType(Code.Difference);
        public static ClaimType FAULT_RETURN = new ClaimType(Code.FaultReturn);
        public static ClaimType DISCOUNT = new ClaimType(Code.Discount);
        public static ClaimType PACKING_ERROR = new ClaimType(Code.PackingError);
        public static ClaimType REWORK_DISPOSAL_REJECT = new ClaimType(Code.ReworkDisposalReject);

        private Code _code;

        private enum Code
        {
            AuditFee = 1,
            SupplierCompliance = 2,
            NoSample = 3,
            Difference = 4,
            FaultReturn = 5,
            Discount = 6,
            PackingError = 7,
            ReworkDisposalReject = 8
        }

        private ClaimType(Code code)
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

        public string ShortCode
        {
            get
            {
                switch (_code)
                {
                    case Code.AuditFee:
                        return "AUD";
                    case Code.SupplierCompliance:
                        return "CFS";
                    case Code.NoSample:
                        return "CR";
                    case Code.Difference:
                        return "DIFF";
                    case Code.FaultReturn:
                        return "DN";
                    case Code.Discount:
                        return "DS";
                    case Code.PackingError:
                        return "PFS";
                    case Code.ReworkDisposalReject:
                        return "RFS";
                    default:
                        return "ERROR";
                }
            }
        }

        public string Description
        {
            get
            {
                switch (_code)
                {
                    case Code.AuditFee:
                        return "Audit Fee (COP)";
                    case Code.SupplierCompliance:
                        return "Supplier Compliance, Crushed And Cartons Over-Packed";
                    case Code.NoSample:
                        return "New Next Claim Type Which Don't Received Sample";
                    case Code.Difference:
                        return "Price Difference / Quantity Difference";
                    case Code.FaultReturn:
                        return "Manufacturer Fault Returns";
                    case Code.Discount:
                        return "Last & Gmt; Discount Claim";
                    case Code.PackingError:
                        return "Packing Errors";
                    case Code.ReworkDisposalReject:
                        return "Rework, Disposal, Rejects In Errors And Rework Administration";
                    default:
                        return "ERROR";
                }
            }
        }

        public static ClaimType getType(int id)
        {
            if (id == Code.AuditFee.GetHashCode()) return ClaimType.AUDIT_FEE;
            else if (id == Code.SupplierCompliance.GetHashCode()) return ClaimType.SUPPLIER_COMPLIANCE;
            else if (id == Code.NoSample.GetHashCode()) return ClaimType.NO_SAMPLE;
            else if (id == Code.Difference.GetHashCode()) return ClaimType.DIFFERENCE;
            else if (id == Code.FaultReturn.GetHashCode()) return ClaimType.FAULT_RETURN;
            else if (id == Code.Discount.GetHashCode()) return ClaimType.DISCOUNT;
            else if (id == Code.PackingError.GetHashCode()) return ClaimType.PACKING_ERROR;
            else if (id == Code.ReworkDisposalReject.GetHashCode()) return ClaimType.REWORK_DISPOSAL_REJECT;
            else return null;
        }

        public static ICollection getCollectionValue()
        {
            ArrayList ary = new ArrayList();
            ary.Add(ClaimType.AUDIT_FEE);
            ary.Add(ClaimType.SUPPLIER_COMPLIANCE);
            ary.Add(ClaimType.NO_SAMPLE);
            ary.Add(ClaimType.DIFFERENCE);
            ary.Add(ClaimType.FAULT_RETURN);
            ary.Add(ClaimType.DISCOUNT);
            ary.Add(ClaimType.PACKING_ERROR);
            ary.Add(ClaimType.REWORK_DISPOSAL_REJECT);
            return ary;
        }
    }
}
