using System;
using com.next.common.domain;
using com.next.isam.domain.types;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class OtherCostTypeRef : DomainData
	{
		private int otherCostTypeId;
		private string otherCostTypeDescription;
		private int opsKey;
        private string sunAccountCode;
        private typeId _typeId;

        public static OtherCostTypeRef PACKAGING_COST = new OtherCostTypeRef(typeId.PackagingCost);
        public static OtherCostTypeRef IMPORT_QUOTA_CHARGE = new OtherCostTypeRef(typeId.ImportQuotaCharge);
        public static OtherCostTypeRef REPAIR_COST = new OtherCostTypeRef(typeId.RepairCost);
        public static OtherCostTypeRef OUTSIDE_LAB_TEST_COST = new OtherCostTypeRef(typeId.OutsideLabTestCost);
        public static OtherCostTypeRef OTHER_FABRIC_COST = new OtherCostTypeRef(typeId.OtherFabricCost);
        public static OtherCostTypeRef SELLING_PRICE_DISCOUNT = new OtherCostTypeRef(typeId.SellingPrxDiscount);
        public static OtherCostTypeRef FACTORY_PRICE_DISCOUNT = new OtherCostTypeRef(typeId.FtyPrxDiscount);
        public static OtherCostTypeRef PRINT_DEVELOPMENT_COST = new OtherCostTypeRef(typeId.PrintDevelopmentCost);
        public static OtherCostTypeRef FREIGHT_COST = new OtherCostTypeRef(typeId.FreightCost);
        public static OtherCostTypeRef DUTY_COST = new OtherCostTypeRef(typeId.DutyCost);
        public static OtherCostTypeRef TRANSPORTATION_COST = new OtherCostTypeRef(typeId.TransportationCost);
        public static OtherCostTypeRef FRAGRANCE_COST = new OtherCostTypeRef(typeId.FragranceCost);
        public static OtherCostTypeRef TOOLING_COST = new OtherCostTypeRef(typeId.ToolingCost);
        public static OtherCostTypeRef DESIGN_FEE = new OtherCostTypeRef(typeId.DesignFee);
        public static OtherCostTypeRef AGENCY_COMMISSION = new OtherCostTypeRef(typeId.AgencyCommission);
        public static OtherCostTypeRef INET_ACTUAL_MARGIN_DIFF = new OtherCostTypeRef(typeId.INetActualMarginDiff);
        public static OtherCostTypeRef FINANCE_COST = new OtherCostTypeRef(typeId.FinanceCost);
        public static OtherCostTypeRef AIR_FREIGHT_COST = new OtherCostTypeRef(typeId.AirFreightCost);
        public static OtherCostTypeRef KIT_DEVELOPMENT_COST = new OtherCostTypeRef(typeId.KitDevelopmentCost);
        public static OtherCostTypeRef REPEAT_DISCOUNT = new OtherCostTypeRef(typeId.RepeatDiscount);
        public static OtherCostTypeRef DEVELOPMENT_SAMPLE_COST = new OtherCostTypeRef(typeId.DevelopmentSampleCost);
        public static OtherCostTypeRef SAMPLE_LENGTH = new OtherCostTypeRef(typeId.SampleLength);
        public static OtherCostTypeRef FREIGHT_FOR_BODYCARE = new OtherCostTypeRef(typeId.FreightForBodycare);
        public static OtherCostTypeRef COURIER_COST_FOR_SAMPLE = new OtherCostTypeRef(typeId.CourierCostForSample);
        public static OtherCostTypeRef MARGIN_DIFFERENCE = new OtherCostTypeRef(typeId.MarginDifference);
        public static OtherCostTypeRef RECOVERY = new OtherCostTypeRef(typeId.Recovery);
        public static OtherCostTypeRef NTN_RECOVERY = new OtherCostTypeRef(typeId.NTNRecovery);
        public static OtherCostTypeRef COVER_FOR_QUENBY_FABRIC = new OtherCostTypeRef(typeId.CoverForQuenbyFabric);
        public static OtherCostTypeRef CLAIMS_RECOVERY = new OtherCostTypeRef(typeId.ClaimsRecovery);
        public static OtherCostTypeRef QCCENTRE = new OtherCostTypeRef(typeId.QCCentre);
        public static OtherCostTypeRef MAIL_ORDER_BOX = new OtherCostTypeRef(typeId.MailOrderBox);
        public static OtherCostTypeRef FORWARDER_CHARGE = new OtherCostTypeRef(typeId.ForwarderCharge);
        public static OtherCostTypeRef LICENSE_COST = new OtherCostTypeRef(typeId.LicenseCost);
        public static OtherCostTypeRef TRADING_AIR_FREIGHT = new OtherCostTypeRef(typeId.TradingAirFreight);
        public static OtherCostTypeRef CLAIMS_RECOVERY_SPECIFIC = new OtherCostTypeRef(typeId.ClaimsRecoverySpecific);
        public static OtherCostTypeRef COATING_COST = new OtherCostTypeRef(typeId.CoatingCost);
        public static OtherCostTypeRef QC_CENTRE_MYANMAR = new OtherCostTypeRef(typeId.QCCentreMyanmar);
        public static OtherCostTypeRef FABRIC_COST_DISCOUNT = new OtherCostTypeRef(typeId.FabricCostDiscount);
        public static OtherCostTypeRef SLIPPAGE = new OtherCostTypeRef(typeId.Slippage);
        public static OtherCostTypeRef CMP_ORDER_PROCESSING = new OtherCostTypeRef(typeId.CMPOrderProcessing);
        public static OtherCostTypeRef CMP_QA_COMM = new OtherCostTypeRef(typeId.CMPQAComm);


        public OtherCostTypeRef(typeId code)
        {
            this._typeId = code;
        }

		public enum typeId
		{
			PackagingCost = 1,
			ImportQuotaCharge = 2,
			RepairCost = 4, 
			OutsideLabTestCost = 5,
			OtherFabricCost = 6,
			SellingPrxDiscount = 7,
			FtyPrxDiscount = 8,
			PrintDevelopmentCost = 9,
			FreightCost = 10, 
			DutyCost = 11, 
            TransportationCost = 12,
            FragranceCost = 13,
            ToolingCost = 14,
            DesignFee = 15,
            AgencyCommission = 16,
            INetActualMarginDiff = 17,
            FinanceCost = 19,
            AirFreightCost = 20,
            KitDevelopmentCost = 21,
            RepeatDiscount = 23,
            DevelopmentSampleCost = 24,
            SampleLength = 25,
            FreightForBodycare = 26,
            CourierCostForSample = 27,
            MarginDifference = 28,
            Recovery = 29,
            NTNRecovery = 30,
            CoverForQuenbyFabric = 31,
            ClaimsRecovery = 32,
            QCCentre = 33,
            MailOrderBox = 34,
            ForwarderCharge = 35,
            LicenseCost = 36,
            TradingAirFreight = 37,
            ClaimsRecoverySpecific = 38,
            CoatingCost = 39,
            QCCentreMyanmar = 40,
            FabricCostDiscount = 41,
            Slippage = 42,
            CMPOrderProcessing = 43,
            CMPQAComm = 44
		}

		public OtherCostTypeRef()
		{
		}

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_typeId.GetHashCode());
            }
        }

		public int OtherCostTypeId
		{
			get { return otherCostTypeId; }
			set { otherCostTypeId = value; }
		}

		public string OtherCostTypeDescription
		{
			get { return otherCostTypeDescription; }
			set { otherCostTypeDescription = value; }
		}

		public int OPSKey
		{
			get { return opsKey; }
			set { opsKey = value; }
		}

        public string SunAccountCode
        {
            get { return sunAccountCode; }
            set { sunAccountCode = value; }
        }

        public static int getAmendmentTypeIdByOtherCostTypeId(int id)
        {
            if (id == typeId.TransportationCost.GetHashCode()) return AmendmentType.TRANSPORTATION_COST.Id;
            else if (id == typeId.SellingPrxDiscount.GetHashCode()) return AmendmentType.SELLING_PRICE_DISCOUNT.Id;
            else if (id == typeId.OutsideLabTestCost.GetHashCode()) return AmendmentType.OUTSIDE_LAB_TEST_COST.Id;
            else if (id == typeId.RepairCost.GetHashCode()) return AmendmentType.REPAIR_COST.Id;
            else if (id == typeId.PackagingCost.GetHashCode()) return AmendmentType.PACKAGING_COST.Id;
            else if (id == typeId.OtherFabricCost.GetHashCode()) return AmendmentType.OTHER_FABRIC_COST.Id;
            else if (id == typeId.DesignFee.GetHashCode()) return AmendmentType.DESIGN_FEE.Id;
            else if (id == typeId.FinanceCost.GetHashCode()) return AmendmentType.FINANCE_COST.Id;
            else if (id == typeId.AirFreightCost.GetHashCode()) return AmendmentType.AIR_FREIGHT_COST.Id;
            else if (id == typeId.KitDevelopmentCost.GetHashCode()) return AmendmentType.KIT_DEVELOPMENT_COST.Id;
            else if (id == typeId.SampleLength.GetHashCode()) return AmendmentType.SAMPLE_LENGTH_COST.Id;
            else if (id == typeId.FreightForBodycare.GetHashCode()) return AmendmentType.FREIGHT_FOR_BODYCARE.Id;
            else if (id == typeId.CourierCostForSample.GetHashCode()) return AmendmentType.COURIER_COST_FOR_SAMPLE.Id;
            else if (id == typeId.DevelopmentSampleCost.GetHashCode()) return AmendmentType.DEVELOPMENT_SAMPLE_COST.Id;
            else if (id == typeId.MarginDifference.GetHashCode()) return AmendmentType.MARGIN_DIFFERENCE.Id;
            else if (id == typeId.Recovery.GetHashCode()) return AmendmentType.RECOVERY.Id;
            else if (id == typeId.NTNRecovery.GetHashCode()) return AmendmentType.NTN_RECOVERY.Id;
            else if (id == typeId.CoverForQuenbyFabric.GetHashCode()) return AmendmentType.COVER_FOR_QUENBY_FABRIC.Id;
            else if (id == typeId.ClaimsRecovery.GetHashCode()) return AmendmentType.CLAIMS_RECOVERY.Id;
            else if (id == typeId.QCCentre.GetHashCode()) return AmendmentType.QCC.Id;
            else if (id == typeId.MailOrderBox.GetHashCode()) return AmendmentType.MOB.Id;
            else if (id == typeId.ForwarderCharge.GetHashCode()) return AmendmentType.FORWARDING_CHARGE.Id;
            else if (id == typeId.LicenseCost.GetHashCode()) return AmendmentType.LICENSE_COST.Id;
            else if (id == typeId.TradingAirFreight.GetHashCode()) return AmendmentType.TRADING_AIR_FREIGHT.Id;
            else if (id == typeId.ClaimsRecoverySpecific.GetHashCode()) return AmendmentType.CLAIMS_RECOVERY_SPECIFIC.Id;
            else if (id == typeId.FtyPrxDiscount.GetHashCode()) return AmendmentType.FACTORY_PRICE_DISCOUNT.Id;
            else if (id == typeId.DutyCost.GetHashCode()) return AmendmentType.DUTY_COST.Id;
            else if (id == typeId.PrintDevelopmentCost.GetHashCode()) return AmendmentType.PRINT_AND_DEVELOPMENT_COST.Id;
            else if (id == typeId.CoatingCost.GetHashCode()) return AmendmentType.COATING_COST.Id;
            else if (id == typeId.QCCentreMyanmar.GetHashCode()) return AmendmentType.QCC_MYANMAR.Id;
            else if (id == typeId.FabricCostDiscount.GetHashCode()) return AmendmentType.FABRIC_COST_DISCOUNT.Id;
            else if (id == typeId.Slippage.GetHashCode()) return AmendmentType.SLIPPAGE.Id;
            else if (id == typeId.CMPOrderProcessing.GetHashCode()) return AmendmentType.CMP_ORDER_PROCESSING.Id;
            else if (id == typeId.CMPQAComm.GetHashCode()) return AmendmentType.CMP_QA_COMM.Id;
            else return -1;
        }

    }
}
