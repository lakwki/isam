using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class AdjustmentType : DomainData
    {
        public static AdjustmentType MOCK_SHOP = new AdjustmentType(Code.MockShop);
        public static AdjustmentType SALES_ADJUSTMENT = new AdjustmentType(Code.SalesAdjustment);
        public static AdjustmentType PURCHASE_ADJUSTMENT = new AdjustmentType(Code.PurchaseAdjustment);
        public static AdjustmentType UKCLAIM = new AdjustmentType(Code.UKClaim);
        public static AdjustmentType UT_DN = new AdjustmentType(Code.UTDebitNote);
        public static AdjustmentType QA_COMMISSION = new AdjustmentType(Code.QACommission);
        public static AdjustmentType STUDIO_SAMPLE = new AdjustmentType(Code.StudioSample);
        public static AdjustmentType ILS_PRICE_DIFF = new AdjustmentType(Code.ILSPriceDiff);
        public static AdjustmentType ADVANCE_PAYMENT_INTEREST = new AdjustmentType(Code.AdvancePaymentInterest);
        public static AdjustmentType OTHER_CHARGE = new AdjustmentType(Code.OtherCharge);
        /*
        public static AdjustmentType UKCLAIM_REFUND = new AdjustmentType(Code.UKClaimRefund);
        */

        private Code _code;

        private enum Code
        {
            MockShop = 0,
            SalesAdjustment = 1,
            PurchaseAdjustment = 2,
            UKClaim = 3,
            UTDebitNote = 4,
            QACommission = 5,
            StudioSample = 6,
            ILSPriceDiff = 7,
            AdvancePaymentInterest = 8,
            OtherCharge = 9

            /*
            UKClaimRefund = 4
            */
        }

        private AdjustmentType(Code code)
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
                    case Code.MockShop:
                        return "Mock Shop";
                    case Code.SalesAdjustment:
                        return "Sales Adjustment";
                    case Code.PurchaseAdjustment:
                        return "Purchase Adjustment";
                    case Code.UKClaim:
                        return "Next Claim";
                    case Code.UTDebitNote:
                        return "UT Debit Note";
                    case Code.QACommission:
                        return "QA Commission";
                    case Code.StudioSample:
                        return "Studio Sample";
                    case Code.ILSPriceDiff:
                        return "ILS Price Difference";
                    case Code.AdvancePaymentInterest:
                        return "Advance Payment Interest";
                    case Code.OtherCharge:
                        return "Other Charge";
                    /*
                    case Code.UKClaimRefund:
                        return "Next Claim Refund";
                    */
                    default:
                        return "ERROR";
                }
            }
        }

        public static AdjustmentType getType(int id)
        {
            if (id == Code.MockShop.GetHashCode()) return AdjustmentType.MOCK_SHOP;
            else if (id == Code.SalesAdjustment.GetHashCode()) return AdjustmentType.SALES_ADJUSTMENT;
            else if (id == Code.PurchaseAdjustment.GetHashCode()) return AdjustmentType.PURCHASE_ADJUSTMENT;
            else if (id == Code.UKClaim.GetHashCode()) return AdjustmentType.UKCLAIM;
            else if (id == Code.UTDebitNote.GetHashCode()) return AdjustmentType.UT_DN;
            else if (id == Code.QACommission.GetHashCode()) return AdjustmentType.QA_COMMISSION;
            else if (id == Code.StudioSample.GetHashCode()) return AdjustmentType.STUDIO_SAMPLE;
            else if (id == Code.ILSPriceDiff.GetHashCode()) return AdjustmentType.ILS_PRICE_DIFF;
            else if (id == Code.AdvancePaymentInterest.GetHashCode()) return AdjustmentType.ADVANCE_PAYMENT_INTEREST;
            else if (id == Code.OtherCharge.GetHashCode()) return AdjustmentType.OTHER_CHARGE;
            /*
            else if (id == Code.UKClaimRefund.GetHashCode()) return AdjustmentType.UKCLAIM_REFUND;
            */
            else return null;
        }

    }
}
