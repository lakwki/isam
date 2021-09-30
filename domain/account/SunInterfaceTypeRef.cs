using System;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class SunInterfaceTypeRef : DomainData
    {
        private int sunInterfaceTypeId;
        private string description;

        public SunInterfaceTypeRef()
        {
        }

        public int SunInterfaceTypeId
        {
            get { return sunInterfaceTypeId; }
            set { sunInterfaceTypeId = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public enum Id
        {
            Sales = 1,
            SalesCommission = 2,
            Purchase = 3,
            AccountReceivable = 4,
            AccountPayable = 5,
            RepairCost = 6,
            OutsideLabTestCost = 7,
            FreightCost = 8,
            DutyCost = 9,
            PackageCost = 10,
            OtherFabricCost = 11,
            SellingPriceDiscount = 12,
            FactoryPriceDiscount = 13,
            PrintDevCost = 14,
            TransportationCost = 15,
            FragranceCost = 16,
            ToolingCost = 17,
            BudgetSales = 18,
            BudgetPurchase = 19,
            ARAdjustmentForNext = 20,
            APAdjustment = 21,
            DesignFee = 22,
            MockShop = 23,
            GTCommission = 24,
            ARAdjustmentForEziBuy = 25,
            MockShopSales = 26,
            MockShopSalesCommission = 27,
            ProvisionForFabricLiabilities = 28,
            SalesCommissionSettlement = 29,
            DevelopmentCost = 30,
            ILSSalesDiscrepancy = 31,
            ILSSalesCommissionDiscrepancy = 32,
            AirFreightCost = 33,
            TemporaryPayment = 34,
            KitDevelopmentCost = 35,
            FinanceCost = 36,
            UKClaim = 37,
            UKClaimRechargeToSupplier = 38,
            DevelopmentSampleCost = 39,
            SampleLengthCost = 40,
            FreightForBodycare = 41,
            CourierCostForSample = 42,
            MarginDifference = 43,
            Recovery = 44,
            NTN_Recovery = 45,
            CoverForQuenbyFabric = 46,
            ClaimsRecovery = 47,
            QCC = 48,
            MOB = 49,
            ForwardingCharge = 50,
            LicenseCost = 51,
            TradingAirFreight = 52,
            ClaimsRecovery_Specific = 53,
            NonTradeExpenseInvoice = 1001,
            NonTradeRechargeDebitNote = 1002,
            NonTradeBankPayment = 1003,
            NonTradeAccrual = 1004,
            NonTradePaymentOnBehalfDebitNote = 1005,
            CoatingCost = 54,
            QCCMyanmar = 55,
            QACommission = 56,
            UTContractDN = 57,
            FabricCostDiscount = 58,
            Slippage = 59,
            AdvancePayment = 60,
            SlippageReversal = 61,
            StudioSample = 62,
            StudioSampleSales = 63,
            StudioSampleSalesCommission = 64,
            NSLedSales_Duty = 65,
            AdvancePaymentInterest = 66,
            UKClaimRechargeToSupplier_ChangeCurrency = 67,
            APAdjustment_ChangeCurrency = 68,
            OtherChargeDCNote = 69,
            UKDiscountClaim = 70,
            UKDiscountClaimClearing = 71,
            CMPOrderProcessing = 72,
            CMPQAComm = 73,
            NSLedStockProvision = 74,
            NSLedSales_NonDuty = 75 
        }

        public static Id getEnum(int id)
        {
            return (SunInterfaceTypeRef.Id)Enum.ToObject(typeof(SunInterfaceTypeRef.Id), id);
        }

        public static string getDescription(int id)
        {
            if (id == Id.Sales.GetHashCode()) return "SALES";
            else if (id == Id.SalesCommission.GetHashCode()) return "SALES COMMISSION";
            else if (id == Id.Purchase.GetHashCode()) return "PURCHASE";
            else if (id == Id.AccountReceivable.GetHashCode()) return "ACCOUNT RECEIVABLE";
            else if (id == Id.AccountPayable.GetHashCode()) return "ACCOUNT PAYABLE";
            else if (id == Id.RepairCost.GetHashCode()) return "REPAIR COST";
            else if (id == Id.OutsideLabTestCost.GetHashCode()) return "LAB TEST CHARGE";
            else if (id == Id.FreightCost.GetHashCode()) return "FREIGHT COST";
            else if (id == Id.DutyCost.GetHashCode()) return "DUTY COST";
            else if (id == Id.OtherFabricCost.GetHashCode()) return "OTHER FABRIC COST";
            else if (id == Id.PackageCost.GetHashCode()) return "PACKAGE COST";
            else if (id == Id.SellingPriceDiscount.GetHashCode()) return "UK DISCOUNT (SALES)";
            else if (id == Id.FactoryPriceDiscount.GetHashCode()) return "FACTORY PRICE DISCOUNT";
            else if (id == Id.PrintDevCost.GetHashCode()) return "PRINT & DEVELOPMENT COST";
            else if (id == Id.TransportationCost.GetHashCode()) return "TRANSPORTATION COST";
            else if (id == Id.FragranceCost.GetHashCode()) return "FRAGRANCE COST";
            else if (id == Id.ToolingCost.GetHashCode()) return "TOOLING COST";
            else if (id == Id.BudgetSales.GetHashCode()) return "BUDGET SALES";
            else if (id == Id.BudgetPurchase.GetHashCode()) return "BUDGET PURCHASE";
            else if (id == Id.ARAdjustmentForNext.GetHashCode()) return "AR ADJUSTMENT FOR NEXT";
            else if (id == Id.APAdjustment.GetHashCode()) return "AP ADJUSTMENT";
            else if (id == Id.DesignFee.GetHashCode()) return "DESIGN FEE";
            else if (id == Id.MockShop.GetHashCode()) return "MOCK SHOP";
            else if (id == Id.GTCommission.GetHashCode()) return "GT COMMISSION";
            else if (id == Id.ARAdjustmentForEziBuy.GetHashCode()) return "AR ADJUSTMENT FOR EZIBUY";
            else if (id == Id.MockShopSales.GetHashCode()) return "SALES (MOCK SHOP)";
            else if (id == Id.MockShopSalesCommission.GetHashCode()) return "SALES COMMISSION (MOCK SHOP)";
            else if (id == Id.DevelopmentCost.GetHashCode()) return "DEVELOPMENT COST";
            else if (id == Id.SalesCommissionSettlement.GetHashCode()) return "SALES COMMISSION SETTLEMENT";
            else if (id == Id.ILSSalesDiscrepancy.GetHashCode()) return "ILS DISCREPANCY (SALES)";
            else if (id == Id.ILSSalesCommissionDiscrepancy.GetHashCode()) return "ILS DISCREPANCY (SALES COMMISSION)";
            else if (id == Id.ProvisionForFabricLiabilities.GetHashCode()) return "GENERAL PROVISION FOR FABRIC LIABILITIES";
            else if (id == Id.AirFreightCost.GetHashCode()) return "AIR FREIGHT COST";
            else if (id == Id.TemporaryPayment.GetHashCode()) return "TEMPORARY PAYMENT";
            else if (id == Id.KitDevelopmentCost.GetHashCode()) return "KIT DEVELOPMENT COST";
            else if (id == Id.FinanceCost.GetHashCode()) return "FINANCE COST";
            else if (id == Id.UKClaim.GetHashCode()) return "NEXT CLAIM";
            else if (id == Id.UKClaimRechargeToSupplier.GetHashCode()) return "NEXT CLAIM RECHARGE TO SUPPLIER";
            else if (id == Id.DevelopmentSampleCost.GetHashCode()) return "DEVELOPMENT SAMPLE COST";
            else if (id == Id.SampleLengthCost.GetHashCode()) return "SAMPLE LENGTH COST";
            else if (id == Id.FreightForBodycare.GetHashCode()) return "FREIGHT FOR BODYCARE";
            else if (id == Id.CourierCostForSample.GetHashCode()) return "COURIER COST FOR SAMPLE";
            else if (id == Id.MarginDifference.GetHashCode()) return "MARGIN DIFFERENCE";
            else if (id == Id.Recovery.GetHashCode()) return "RECOVERY";
            else if (id == Id.NTN_Recovery.GetHashCode()) return "NTN RECOVERY";
            else if (id == Id.CoverForQuenbyFabric.GetHashCode()) return "COVER FOR QUENBY FABRIC";
            else if (id == Id.ClaimsRecovery.GetHashCode()) return "CLAIMS RECOVERY";
            else if (id == Id.QCC.GetHashCode()) return "QCC";
            else if (id == Id.MOB.GetHashCode()) return "MOB";
            else if (id == Id.ForwardingCharge.GetHashCode()) return "FORWARDING CHARGE";
            else if (id == Id.LicenseCost.GetHashCode()) return "LICENSE COST";
            else if (id == Id.TradingAirFreight.GetHashCode()) return "TRADING AIR FREIGHT";
            else if (id == Id.ClaimsRecovery_Specific.GetHashCode()) return "CLAIMS RECOVERY (SPECIFIC)";
            else if (id == Id.NonTradeExpenseInvoice.GetHashCode()) return "NON-TRADE EXPENSE INVOICE";
            else if (id == Id.NonTradeRechargeDebitNote.GetHashCode()) return "NON-TRADE RECHARGE DEBIT NOTE";
            else if (id == Id.NonTradeBankPayment.GetHashCode()) return "NON-TRADE BANK PAYMENT";
            else if (id == Id.NonTradeAccrual.GetHashCode()) return "NON-TRADE ACCRUAL";
            else if (id == Id.NonTradePaymentOnBehalfDebitNote.GetHashCode()) return "NON-TRADE PAYMENT-ON-BEHALF DEBIT NOTE";
            else if (id == Id.CoatingCost.GetHashCode()) return "COATING COST";
            else if (id == Id.QCCMyanmar.GetHashCode()) return "QCC (MYANMAR)";
            else if (id == Id.QACommission.GetHashCode()) return "QA Commission";
            else if (id == Id.UTContractDN.GetHashCode()) return "UT CONTRACT DEBIT NOTE";
            else if (id == Id.FabricCostDiscount.GetHashCode()) return "FABRIC COST DISCOUNT";
            else if (id == Id.Slippage.GetHashCode()) return "SLIPPAGE";
            else if (id == Id.AdvancePayment.GetHashCode()) return "ADVANCE PAYMENT";
            else if (id == Id.SlippageReversal.GetHashCode()) return "SLIPPAGE-REVERSAL";
            else if (id == Id.StudioSample.GetHashCode()) return "STUDIO SAMPLE";
            else if (id == Id.StudioSampleSales.GetHashCode()) return "SALES (STUDIO SAMPLE)";
            else if (id == Id.StudioSampleSalesCommission.GetHashCode()) return "SALES COMMISSION (STUDIO SAMPLE)";
            else if (id == Id.NSLedSales_Duty.GetHashCode()) return "NS-LED SALES (DUTY)";
            else if (id == Id.AdvancePaymentInterest.GetHashCode()) return "ADVANCE PAYMENT INTEREST";
            else if (id == Id.UKClaimRechargeToSupplier_ChangeCurrency.GetHashCode()) return "NEXT CLAIM RECHARGE TO SUPPLIER (CHANGE CURRENCY)";
            else if (id == Id.APAdjustment_ChangeCurrency.GetHashCode()) return "AP ADJUSTMENT (CHANGE CURRENCY)";
            else if (id == Id.OtherChargeDCNote.GetHashCode()) return "OTHER CHARGE D/C NOTE";
            else if (id == Id.UKDiscountClaim.GetHashCode()) return "UK DISCOUNT CLAIM";
            else if (id == Id.UKDiscountClaimClearing.GetHashCode()) return "UK DISCOUNT CLAIM CLEARING";
            else if (id == Id.CMPOrderProcessing.GetHashCode()) return "CMP ORDER PROCESSING";
            else if (id == Id.CMPQAComm.GetHashCode()) return "CMP QA COMMISSION";
            else if (id == Id.NSLedStockProvision.GetHashCode()) return "NS-LED STOCK PROVISION";
            else if (id == Id.NSLedSales_NonDuty.GetHashCode()) return "NS-LED SALES (NON-DUTY)";

            else return "N/A";
        }

        public static bool isOtherCost(int id)
        {
            if (id == Id.RepairCost.GetHashCode()) return true;
            else if (id == Id.OutsideLabTestCost.GetHashCode()) return true;
            else if (id == Id.FreightCost.GetHashCode()) return true;
            else if (id == Id.DutyCost.GetHashCode()) return true;
            else if (id == Id.OtherFabricCost.GetHashCode()) return true;
            else if (id == Id.PackageCost.GetHashCode()) return true;
            else if (id == Id.SellingPriceDiscount.GetHashCode()) return true;
            else if (id == Id.FactoryPriceDiscount.GetHashCode()) return true;
            else if (id == Id.PrintDevCost.GetHashCode()) return true;
            else if (id == Id.TransportationCost.GetHashCode()) return true;
            else if (id == Id.FragranceCost.GetHashCode()) return true;
            else if (id == Id.ToolingCost.GetHashCode()) return true;
            else if (id == Id.DesignFee.GetHashCode()) return true;
            else if (id == Id.GTCommission.GetHashCode()) return true;
            else if (id == Id.DevelopmentCost.GetHashCode()) return true;
            else if (id == Id.ProvisionForFabricLiabilities.GetHashCode()) return true;
            else if (id == Id.AirFreightCost.GetHashCode()) return true;
            else if (id == Id.KitDevelopmentCost.GetHashCode()) return true;
            else if (id == Id.FinanceCost.GetHashCode()) return true;
            else if (id == Id.DevelopmentSampleCost.GetHashCode()) return true;
            else if (id == Id.SampleLengthCost.GetHashCode()) return true;
            else if (id == Id.FreightForBodycare.GetHashCode()) return true;
            else if (id == Id.CourierCostForSample.GetHashCode()) return true;
            else if (id == Id.MarginDifference.GetHashCode()) return true;
            else if (id == Id.Recovery.GetHashCode()) return true;
            else if (id == Id.NTN_Recovery.GetHashCode()) return true;
            else if (id == Id.CoverForQuenbyFabric.GetHashCode()) return true;
            else if (id == Id.ClaimsRecovery.GetHashCode()) return true;
            else if (id == Id.QCC.GetHashCode()) return true;
            else if (id == Id.MOB.GetHashCode()) return true;
            else if (id == Id.ForwardingCharge.GetHashCode()) return true;
            else if (id == Id.LicenseCost.GetHashCode()) return true;
            else if (id == Id.TradingAirFreight.GetHashCode()) return true;
            else if (id == Id.ClaimsRecovery_Specific.GetHashCode()) return true;
            else if (id == Id.CoatingCost.GetHashCode()) return true;
            else if (id == Id.QCCMyanmar.GetHashCode()) return true;
            else if (id == Id.FabricCostDiscount.GetHashCode()) return true;
            else if (id == Id.Slippage.GetHashCode()) return true;
            else if (id == Id.SlippageReversal.GetHashCode()) return true;
            else if (id == Id.CMPOrderProcessing.GetHashCode()) return true;
            else if (id == Id.CMPQAComm.GetHashCode()) return true;
            else return false;
        }

        public static bool isAutoPost(int id)
        {
            if (id == Id.Sales.GetHashCode()) return true;
            else if (id == Id.SalesCommission.GetHashCode()) return true;
            else if (id == Id.Purchase.GetHashCode()) return true;
            else if (id == Id.AccountReceivable.GetHashCode()) return false;
            else if (id == Id.AccountPayable.GetHashCode()) return false;
            else if (id == Id.RepairCost.GetHashCode()) return true;
            else if (id == Id.OutsideLabTestCost.GetHashCode()) return true;
            else if (id == Id.FreightCost.GetHashCode()) return true;
            else if (id == Id.DutyCost.GetHashCode()) return true;
            else if (id == Id.OtherFabricCost.GetHashCode()) return true;
            else if (id == Id.PackageCost.GetHashCode()) return true;
            else if (id == Id.SellingPriceDiscount.GetHashCode()) return true;
            else if (id == Id.FactoryPriceDiscount.GetHashCode()) return false;
            else if (id == Id.PrintDevCost.GetHashCode()) return true;
            else if (id == Id.TransportationCost.GetHashCode()) return true;
            else if (id == Id.FragranceCost.GetHashCode()) return true;
            else if (id == Id.ToolingCost.GetHashCode()) return true;
            else if (id == Id.BudgetSales.GetHashCode()) return false;
            else if (id == Id.BudgetPurchase.GetHashCode()) return false;
            else if (id == Id.ARAdjustmentForNext.GetHashCode()) return false;
            else if (id == Id.APAdjustment.GetHashCode()) return false;
            else if (id == Id.DesignFee.GetHashCode()) return true;
            else if (id == Id.MockShop.GetHashCode()) return true;
            else if (id == Id.GTCommission.GetHashCode()) return true;
            else if (id == Id.ARAdjustmentForEziBuy.GetHashCode()) return false;
            else if (id == Id.MockShopSales.GetHashCode()) return true;
            else if (id == Id.MockShopSalesCommission.GetHashCode()) return true;
            else if (id == Id.DevelopmentCost.GetHashCode()) return true;
            else if (id == Id.SalesCommissionSettlement.GetHashCode()) return false;
            else if (id == Id.ILSSalesDiscrepancy.GetHashCode()) return false;
            else if (id == Id.ILSSalesCommissionDiscrepancy.GetHashCode()) return false;
            else if (id == Id.ProvisionForFabricLiabilities.GetHashCode()) return true;
            else if (id == Id.AirFreightCost.GetHashCode()) return true;
            else if (id == Id.TemporaryPayment.GetHashCode()) return true;
            else if (id == Id.KitDevelopmentCost.GetHashCode()) return true;
            else if (id == Id.FinanceCost.GetHashCode()) return true;
            else if (id == Id.UKClaim.GetHashCode()) return false;
            else if (id == Id.UKClaimRechargeToSupplier.GetHashCode()) return false;
            else if (id == Id.DevelopmentSampleCost.GetHashCode()) return true;
            else if (id == Id.SampleLengthCost.GetHashCode()) return true;
            else if (id == Id.FreightForBodycare.GetHashCode()) return true;
            else if (id == Id.CourierCostForSample.GetHashCode()) return true;
            else if (id == Id.MarginDifference.GetHashCode()) return true;
            else if (id == Id.Recovery.GetHashCode()) return true;
            else if (id == Id.NTN_Recovery.GetHashCode()) return true;
            else if (id == Id.CoverForQuenbyFabric.GetHashCode()) return true;
            else if (id == Id.ClaimsRecovery.GetHashCode()) return true;
            else if (id == Id.QCC.GetHashCode()) return true;
            else if (id == Id.MOB.GetHashCode()) return true;
            else if (id == Id.ForwardingCharge.GetHashCode()) return true;
            else if (id == Id.LicenseCost.GetHashCode()) return true;
            else if (id == Id.TradingAirFreight.GetHashCode()) return true;
            else if (id == Id.ClaimsRecovery_Specific.GetHashCode()) return true;
            else if (id == Id.NonTradeExpenseInvoice.GetHashCode()) return false;
            else if (id == Id.NonTradeRechargeDebitNote.GetHashCode()) return false;
            else if (id == Id.NonTradeBankPayment.GetHashCode()) return false;
            else if (id == Id.NonTradeAccrual.GetHashCode()) return false;
            else if (id == Id.NonTradePaymentOnBehalfDebitNote.GetHashCode()) return false;
            else if (id == Id.CoatingCost.GetHashCode()) return true;
            else if (id == Id.QCCMyanmar.GetHashCode()) return true;
            else if (id == Id.QACommission.GetHashCode()) return true;
            else if (id == Id.UTContractDN.GetHashCode()) return true;
            else if (id == Id.FabricCostDiscount.GetHashCode()) return true;
            else if (id == Id.Slippage.GetHashCode()) return true;
            else if (id == Id.AdvancePayment.GetHashCode()) return false;
            else if (id == Id.SlippageReversal.GetHashCode()) return false;
            else if (id == Id.StudioSample.GetHashCode()) return true;
            else if (id == Id.StudioSampleSales.GetHashCode()) return true;
            else if (id == Id.StudioSampleSalesCommission.GetHashCode()) return true;
            else if (id == Id.AdvancePaymentInterest.GetHashCode()) return false;
            else if (id == Id.UKClaimRechargeToSupplier_ChangeCurrency.GetHashCode()) return false;
            else if (id == Id.APAdjustment_ChangeCurrency.GetHashCode()) return false;
            else if (id == Id.OtherChargeDCNote.GetHashCode()) return false;
            else if (id == Id.UKDiscountClaim.GetHashCode()) return false;
            else if (id == Id.UKDiscountClaimClearing.GetHashCode()) return false;
            else if (id == Id.CMPOrderProcessing.GetHashCode()) return true;
            else if (id == Id.CMPQAComm.GetHashCode()) return true;

            else return false;
        }

        public static bool isAPRelated(int id)
        {
            if (id == Id.Purchase.GetHashCode())
                return true;
            else
                return false;
        }

        public static bool isARRelated(int id)
        {
            if (id == Id.Sales.GetHashCode() || id == Id.SalesCommission.GetHashCode())
                return true;
            else
                return false;
        }

        public static bool isMockShopRelated(int id)
        {
            if (id == Id.MockShop.GetHashCode() || id == Id.MockShopSales.GetHashCode() || id == Id.MockShopSalesCommission.GetHashCode())
                return true;
            else
                return false;
        }

        public static string getActualDataCommandName(int id)
        {
            if (id == Id.Sales.GetHashCode()) return "GetActualSales";
            else if (id == Id.SalesCommission.GetHashCode()) return "GetActualSalesCommission";
            else if (id == Id.Purchase.GetHashCode()) return "GetActualPurchase";
            else if (id == Id.AccountReceivable.GetHashCode()) return "GetActualAR";
            /*
            else if (id == Id.AccountPayable.GetHashCode()) return "GetActualAP";
            */
            else if (id == Id.FreightCost.GetHashCode()) return "GetActualFreightCost";
            else if (id == Id.DutyCost.GetHashCode()) return "GetActualDutyCost";
            else if (id == Id.SellingPriceDiscount.GetHashCode()) return "GetActualUKDiscount";
            else if (id == Id.APAdjustment.GetHashCode()) return "GetActualAPAdjustment";
            else if (id == Id.ARAdjustmentForEziBuy.GetHashCode()) return "GetActualARAdjustmentForEziBuy";
            else if (id == Id.MockShop.GetHashCode()) return "GetActualMockShop";
            else if (id == Id.GTCommission.GetHashCode()) return "GetActualGTCommission";
            else if (id == Id.DevelopmentCost.GetHashCode()) return "GetActualDevelopmentCost";
            else if (id == Id.MockShopSales.GetHashCode()) return "GetActualMockShopSales";
            else if (id == Id.MockShopSalesCommission.GetHashCode()) return "GetActualMockShopSalesCommission";
            else if (id == Id.SalesCommissionSettlement.GetHashCode()) return "GetActualSalesCommissionSettlement";
            else if (id == Id.ProvisionForFabricLiabilities.GetHashCode()) return "GetActualProvisionForFabricLiabilities";
            else if (id == Id.ILSSalesDiscrepancy.GetHashCode()) return "GetActualILSSalesDiscrepancy";
            else if (id == Id.ILSSalesCommissionDiscrepancy.GetHashCode()) return "GetActualILSSalesCommissionDiscrepancy";
            else if (id == Id.TemporaryPayment.GetHashCode()) return "GetActualTemporaryPayment";
            else if (id == Id.KitDevelopmentCost.GetHashCode()) return "GetActualKitDevelopmentCost";
            else if (id == Id.UKClaim.GetHashCode()) return "GetActualUKClaim";
            else if (id == Id.UKClaimRechargeToSupplier.GetHashCode()) return "GetActualUKClaimRechargeToSupplier";
            else if (id == Id.NonTradeExpenseInvoice.GetHashCode()) return "GetActualNonTradeExpenseInvoice";
            else if (id == Id.NonTradeRechargeDebitNote.GetHashCode()) return "GetActualNonTradeRechargeDebitNote";
            else if (id == Id.NonTradeBankPayment.GetHashCode()) return "GetActualNonTradeBankPayment";
            else if (id == Id.NonTradeAccrual.GetHashCode()) return "GetActualNonTradeAccrual";
            else if (id == Id.NonTradePaymentOnBehalfDebitNote.GetHashCode()) return "GetActualNonTradePaymentOnBehalfDebitNote";
            else if (id == Id.QACommission.GetHashCode()) return "GetActualQACommission";
            else if (id == Id.UTContractDN.GetHashCode()) return "GetActualUTContractDN";
            /*
            else if (id == Id.PrintDevCost.GetHashCode()) return "GetActualPrintWashCost";
            else if (id == Id.DevelopmentSampleCost.GetHashCode()) return "GetActualDevelopmentSampleCost";
            else if (id == Id.Recovery.GetHashCode()) return "GetActualHomeHardRecovery";
            else if (id == Id.CoverForQuenbyFabric.GetHashCode()) return "GetActualCoverForQuenbyFabric";
            else if (id == Id.ClaimsRecovery.GetHashCode()) return "GetActualClaimsRecovery";
            else if (id == Id.CourierCostForSample.GetHashCode()) return "GetActualCourierCostForSample";
            else if (id == Id.MarginDifference.GetHashCode()) return "GetActualMarginDifference";
            else if (id == Id.ForwardingCharge.GetHashCode()) return "GetActualForwardingCharge";
            else if (id == Id.QCC.GetHashCode()) return "GetActualQCC";
            else if (id == Id.MOB.GetHashCode()) return "GetActualMOB";
            else if (id == Id.SampleLengthCost.GetHashCode()) return "GetActualSampleLengthCost";
            else if (id == Id.NTN_Recovery.GetHashCode()) return "GetActualNTNRecovery";
            else if (id == Id.ClaimsRecovery_Specific.GetHashCode()) return "GetActualClaimsRecoverySpecific";
            else if (id == Id.TradingAirFreight.GetHashCode()) return "GetActualTradingAirFreight";
            else if (id == Id.AirFreightCost.GetHashCode()) return "GetActualAirFreightCost";
            else if (id == Id.LicenseCost.GetHashCode()) return "GetActualLicenseCost";
            else if (id == Id.FreightForBodycare.GetHashCode()) return "GetActualFreightForBodycare";
            else if (id == Id.TransportationCost.GetHashCode()) return "GetActualTransportationCost";
            else if (id == Id.RepairCost.GetHashCode()) return "GetActualRepairCost";
            else if (id == Id.OutsideLabTestCost.GetHashCode()) return "GetActualLabTestCharge";
            else if (id == Id.FragranceCost.GetHashCode()) return "GetActualFragranceCost";
            else if (id == Id.DesignFee.GetHashCode()) return "GetActualDesignFee";
            else if (id == Id.OtherFabricCost.GetHashCode()) return "GetActualOtherFabricCost";
            else if (id == Id.ToolingCost.GetHashCode()) return "GetActualToolingCost";
            else if (id == Id.PackageCost.GetHashCode()) return "GetActualPackageCost";
            else if (id == Id.CoatingCost.GetHashCode()) return "GetActualCoatingCost";
            else if (id == Id.QCCMyanmar.GetHashCode()) return "GetActualQCCMyanmar";
            else if (id == Id.FabricCostDiscount.GetHashCode()) return "GetActualFabricCostDiscount";
            else if (id == Id.Slippage.GetHashCode()) return "GetActualSlippage";
            else if (id == Id.FinanceCost.GetHashCode()) return "GetActualFinanceCost";
            */
            else if (isOtherCost(id)) return "GetActualGenericOtherCost";

            else if (id == Id.StudioSample.GetHashCode()) return "GetActualStudioSample";
            else if (id == Id.StudioSampleSales.GetHashCode()) return "GetActualStudioSampleSales";
            else if (id == Id.StudioSampleSalesCommission.GetHashCode()) return "GetActualStudioSampleSalesCommission";
            else if (id == Id.NSLedSales_Duty.GetHashCode()) return "GetActualNSLedSales_Duty";
            else if (id == Id.NSLedSales_NonDuty.GetHashCode()) return "GetActualNSLedSales_NonDuty";
            else if (id == Id.UKClaimRechargeToSupplier_ChangeCurrency.GetHashCode()) return "GetActualUKClaimRechargeToSupplier_ChangeCurrency";
            else if (id == Id.APAdjustment_ChangeCurrency.GetHashCode()) return "GetActualAPAdjustment_ChangeCurrency";
            else if (id == Id.UKDiscountClaim.GetHashCode()) return "GetActualUKDiscountClaim";
            else if (id == Id.UKDiscountClaim.GetHashCode()) return "GetActualUKDiscountClaimClearing";

            else return "N/A";
        }

        public static string getAccrualDataCommandName(int id)
        {
            if (id == Id.Sales.GetHashCode()) return "GetAccrualSales";
            else if (id == Id.SalesCommission.GetHashCode()) return "GetAccrualSalesCommission";
            else if (id == Id.Purchase.GetHashCode()) return "GetAccrualPurchase";
            else if (id == Id.FreightCost.GetHashCode()) return "GetAccrualFreightCost";
            else if (id == Id.DutyCost.GetHashCode()) return "GetAccrualDutyCost";
            else if (id == Id.GTCommission.GetHashCode()) return "GetAccrualGTCommission";
            else if (id == Id.SellingPriceDiscount.GetHashCode()) return "GetAccrualUKDiscount";
            else if (id == Id.KitDevelopmentCost.GetHashCode()) return "GetAccrualKitDevelopmentCost";
            else if (id == Id.NonTradeExpenseInvoice.GetHashCode()) return "GetAccrualNonTradeExpenseInvoice";
            else if (id == Id.NonTradeAccrual.GetHashCode()) return "GetAccrualNonTradeAccrual";
            /*
            else if (id == Id.PrintDevCost.GetHashCode()) return "GetAccrualPrintWashCost";
            else if (id == Id.DevelopmentSampleCost.GetHashCode()) return "GetAccrualDevelopmentSampleCost";
            else if (id == Id.Recovery.GetHashCode()) return "GetAccrualHomeHardRecovery";
            else if (id == Id.CoverForQuenbyFabric.GetHashCode()) return "GetAccrualCoverForQuenbyFabric";
            else if (id == Id.ClaimsRecovery.GetHashCode()) return "GetAccrualClaimsRecovery";
            else if (id == Id.CourierCostForSample.GetHashCode()) return "GetAccrualCourierCostForSample";
            else if (id == Id.MarginDifference.GetHashCode()) return "GetAccrualMarginDifference";
            else if (id == Id.ForwardingCharge.GetHashCode()) return "GetAccrualForwardingCharge";
            else if (id == Id.QCC.GetHashCode()) return "GetAccrualQCC";
            else if (id == Id.MOB.GetHashCode()) return "GetAccrualMOB";
            else if (id == Id.SampleLengthCost.GetHashCode()) return "GetAccrualSampleLengthCost";
            else if (id == Id.NTN_Recovery.GetHashCode()) return "GetAccrualNTNRecovery";
            else if (id == Id.ClaimsRecovery_Specific.GetHashCode()) return "GetAccrualClaimsRecoverySpecific";
            else if (id == Id.TradingAirFreight.GetHashCode()) return "GetAccrualTradingAirFreight";
            else if (id == Id.AirFreightCost.GetHashCode()) return "GetAccrualAirFreightCost";
            else if (id == Id.LicenseCost.GetHashCode()) return "GetAccrualLicenseCost";
            else if (id == Id.FreightForBodycare.GetHashCode()) return "GetAccrualFreightForBodycare";
            else if (id == Id.TransportationCost.GetHashCode()) return "GetAccrualTransportationCost";
            else if (id == Id.RepairCost.GetHashCode()) return "GetAccrualRepairCost";
            else if (id == Id.OutsideLabTestCost.GetHashCode()) return "GetAccrualLabTestCharge";
            else if (id == Id.FragranceCost.GetHashCode()) return "GetAccrualFragranceCost";
            else if (id == Id.OtherFabricCost.GetHashCode()) return "GetAccrualOtherFabricCost";
            else if (id == Id.DesignFee.GetHashCode()) return "GetAccrualDesignFee";
            else if (id == Id.PackageCost.GetHashCode()) return "GetAccrualPackageCost";
            else if (id == Id.ToolingCost.GetHashCode()) return "GetAccrualToolingCost";
            else if (id == Id.FinanceCost.GetHashCode()) return "GetAccrualFinanceCost";
            else if (id == Id.CoatingCost.GetHashCode()) return "GetAccrualCoatingCost";
            else if (id == Id.QCCMyanmar.GetHashCode()) return "GetAccrualQCCMyanmar";
            else if (id == Id.FabricCostDiscount.GetHashCode()) return "GetAccrualFabricCostDiscount";
            else if (id == Id.Slippage.GetHashCode()) return "GetAccrualSlippage";
            */
            else if (isOtherCost(id)) return "GetAccrualGenericOtherCost";
            else return "N/A";
        }

        public static string getRealizedDataCommandName(int id)
        {
            if (id == Id.Sales.GetHashCode()) return "GetRealizedSales";
            else if (id == Id.SalesCommission.GetHashCode()) return "GetRealizedSalesCommission";
            else if (id == Id.Purchase.GetHashCode()) return "GetRealizedPurchase";
            else if (id == Id.FreightCost.GetHashCode()) return "GetRealizedFreightCost";
            else if (id == Id.DutyCost.GetHashCode()) return "GetRealizedDutyCost";
            else if (id == Id.GTCommission.GetHashCode()) return "GetRealizedGTCommission";
            else if (id == Id.SellingPriceDiscount.GetHashCode()) return "GetRealizedUKDiscount";
            else if (id == Id.DevelopmentCost.GetHashCode()) return "GetRealizedDevelopmentCost";
            else if (id == Id.ProvisionForFabricLiabilities.GetHashCode()) return "GetRealizedProvisionForFabricLiabilities";
            else if (id == Id.KitDevelopmentCost.GetHashCode()) return "GetRealizedKitDevelopmentCost";
            /*
            else if (id == Id.PrintDevCost.GetHashCode()) return "GetRealizedPrintWashCost";
            else if (id == Id.DevelopmentSampleCost.GetHashCode()) return "GetRealizedDevelopmentSampleCost";
            else if (id == Id.Recovery.GetHashCode()) return "GetRealizedHomeHardRecovery";
            else if (id == Id.CoverForQuenbyFabric.GetHashCode()) return "GetRealizedCoverForQuenbyFabric";
            else if (id == Id.ClaimsRecovery.GetHashCode()) return "GetRealizedClaimsRecovery";
            else if (id == Id.CourierCostForSample.GetHashCode()) return "GetRealizedCourierCostForSample";
            else if (id == Id.MarginDifference.GetHashCode()) return "GetRealizedMarginDifference";
            else if (id == Id.ForwardingCharge.GetHashCode()) return "GetRealizedForwardingCharge";
            else if (id == Id.QCC.GetHashCode()) return "GetRealizedQCC";
            else if (id == Id.MOB.GetHashCode()) return "GetRealizedMOB";
            else if (id == Id.SampleLengthCost.GetHashCode()) return "GetRealizedSampleLengthCost";
            else if (id == Id.NTN_Recovery.GetHashCode()) return "GetRealizedNTNRecovery";
            else if (id == Id.ClaimsRecovery_Specific.GetHashCode()) return "GetRealizedClaimsRecoverySpecific";
            else if (id == Id.TradingAirFreight.GetHashCode()) return "GetRealizedTradingAirFreight";
            else if (id == Id.AirFreightCost.GetHashCode()) return "GetRealizedAirFreightCost";
            else if (id == Id.LicenseCost.GetHashCode()) return "GetRealizedLicenseCost";
            else if (id == Id.FreightForBodycare.GetHashCode()) return "GetRealizedFreightForBodycare";
            else if (id == Id.TransportationCost.GetHashCode()) return "GetRealizedTransportationCost";
            else if (id == Id.RepairCost.GetHashCode()) return "GetRealizedRepairCost";
            else if (id == Id.OutsideLabTestCost.GetHashCode()) return "GetRealizedLabTestCharge";
            else if (id == Id.FragranceCost.GetHashCode()) return "GetRealizedFragranceCost";
            else if (id == Id.DesignFee.GetHashCode()) return "GetRealizedDesignFee";
            else if (id == Id.OtherFabricCost.GetHashCode()) return "GetRealizedOtherFabricCost";
            else if (id == Id.ToolingCost.GetHashCode()) return "GetRealizedToolingCost";
            else if (id == Id.PackageCost.GetHashCode()) return "GetRealizedPackageCost";
            else if (id == Id.FinanceCost.GetHashCode()) return "GetRealizedFinanceCost";
            else if (id == Id.CoatingCost.GetHashCode()) return "GetRealizedCoatingCost";
            else if (id == Id.QCCMyanmar.GetHashCode()) return "GetRealizedQCCMyanmar";
            else if (id == Id.FabricCostDiscount.GetHashCode()) return "GetRealizedFabricCostDiscount";
            else if (id == Id.Slippage.GetHashCode()) return "GetRealizedSlippage";
            */
            else if (isOtherCost(id)) return "GetRealizedGenericOtherCost";
            else return "N/A";
        }

        public static string getDailyDataCommandName(int id)
        {
            if (id == Id.Sales.GetHashCode()) return "GetDailySales";
            else if (id == Id.SalesCommission.GetHashCode()) return "GetDailySalesCommission";
            else if (id == Id.Purchase.GetHashCode()) return "GetDailyPurchase";
            else if (id == Id.AccountPayable.GetHashCode()) return "GetDailyAP";
            else if (id == Id.ARAdjustmentForNext.GetHashCode()) return "GetDailyARAdjustmentForNext";
            else if (id == Id.AdvancePayment.GetHashCode()) return "GetDailyAdvancePayment";
            else if (id == Id.AdvancePaymentInterest.GetHashCode()) return "GetDailyAdvancePaymentInterest";
            else if (id == Id.OtherChargeDCNote.GetHashCode()) return "GetDailyOtherChargeDCNote";
            /*
            else if (id == Id.ILSSalesDiscrepancy.GetHashCode()) return "GetDailyILSSalesDiscrepancy";
            else if (id == Id.ILSSalesCommissionDiscrepancy.GetHashCode()) return "GetDailyILSSalesCommissionDiscrepancy";
            */
            else return "N/A";
        }

        public static string getReversalDataCommandName(int id, bool isSplit)
        {
            if (!isSplit)
            {
                if (id == Id.Sales.GetHashCode()) return "GetReverseSalesByShipmentId";
                else if (id == Id.SalesCommission.GetHashCode()) return "GetReverseSalesCommissionByShipmentId";
                else if (id == Id.Purchase.GetHashCode()) return "GetReversePurchaseByShipmentId";
                else if (id == Id.FreightCost.GetHashCode()) return "GetReverseFreightCostByShipmentId";
                else if (id == Id.DutyCost.GetHashCode()) return "GetReverseDutyCostByShipmentId";
                else if (id == Id.SellingPriceDiscount.GetHashCode()) return "GetReverseUKDiscountByShipmentId";
                else if (id == Id.GTCommission.GetHashCode()) return "GetReverseGTCommissionByShipmentId";
                else if (id == Id.MockShopSales.GetHashCode()) return "GetReverseSalesByShipmentId";
                else if (id == Id.MockShopSalesCommission.GetHashCode()) return "GetReverseSalesCommissionByShipmentId";
                else if (id == Id.KitDevelopmentCost.GetHashCode()) return "GetReverseKitDevelopmentCostByShipmentId";
                else if (id == Id.ProvisionForFabricLiabilities.GetHashCode()) return "GetReverseProvisionForFabricLiabilitiesByShipmentId";
                else if (id == Id.TemporaryPayment.GetHashCode()) return "GetReverseTemporaryPaymentByShipmentId";
                /*
                else if (id == Id.PrintDevCost.GetHashCode()) return "GetReversePrintWashCostByShipmentId";
                else if (id == Id.DevelopmentSampleCost.GetHashCode()) return "GetReverseDevelopmentSampleCostByShipmentId";
                else if (id == Id.Recovery.GetHashCode()) return "GetReverseHomeHardRecoveryByShipmentId";
                else if (id == Id.CoverForQuenbyFabric.GetHashCode()) return "GetReverseCoverForQuenbyFabricByShipmentId";
                else if (id == Id.ClaimsRecovery.GetHashCode()) return "GetReverseClaimsRecoveryByShipmentId";
                else if (id == Id.CourierCostForSample.GetHashCode()) return "GetReverseCourierCostForSampleByShipmentId";
                else if (id == Id.MarginDifference.GetHashCode()) return "GetReverseMarginDifferenceByShipmentId";
                else if (id == Id.ForwardingCharge.GetHashCode()) return "GetReverseForwardingChargeByShipmentId";
                else if (id == Id.QCC.GetHashCode()) return "GetReverseQCCByShipmentId";
                else if (id == Id.MOB.GetHashCode()) return "GetReverseMOBByShipmentId";
                else if (id == Id.SampleLengthCost.GetHashCode()) return "GetReverseSampleLengthCostByShipmentId";
                else if (id == Id.NTN_Recovery.GetHashCode()) return "GetReverseNTNRecoveryByShipmentId";
                else if (id == Id.ClaimsRecovery_Specific.GetHashCode()) return "GetReverseClaimsRecoverySpecificByShipmentId";
                else if (id == Id.TradingAirFreight.GetHashCode()) return "GetReverseTradingAirFreightByShipmentId";
                else if (id == Id.AirFreightCost.GetHashCode()) return "GetReverseAirFreightCostByShipmentId";
                else if (id == Id.LicenseCost.GetHashCode()) return "GetReverseLicenseCostByShipmentId";
                else if (id == Id.FreightForBodycare.GetHashCode()) return "GetReverseFreightForBodycareByShipmentId";
                else if (id == Id.TransportationCost.GetHashCode()) return "GetReverseTransportationCostByShipmentId";
                else if (id == Id.RepairCost.GetHashCode()) return "GetReverseRepairCostByShipmentId";
                else if (id == Id.OutsideLabTestCost.GetHashCode()) return "GetReverseLabTestChargeByShipmentId";
                else if (id == Id.FragranceCost.GetHashCode()) return "GetReverseFragranceCostByShipmentId";
                else if (id == Id.DesignFee.GetHashCode()) return "GetReverseDesignFeeByShipmentId";
                else if (id == Id.OtherFabricCost.GetHashCode()) return "GetReverseOtherFabricCostByShipmentId";
                else if (id == Id.ToolingCost.GetHashCode()) return "GetReverseToolingCostByShipmentId";
                else if (id == Id.PackageCost.GetHashCode()) return "GetReversePackageCostByShipmentId";
                else if (id == Id.FinanceCost.GetHashCode()) return "GetReverseFinanceCostByShipmentId";
                else if (id == Id.CoatingCost.GetHashCode()) return "GetReverseCoatingCostByShipmentId";
                else if (id == Id.QCCMyanmar.GetHashCode()) return "GetReverseQCCMyanmarByShipmentId";
                else if (id == Id.FabricCostDiscount.GetHashCode()) return "GetReverseFabricCostDiscountByShipmentId";
                else if (id == Id.Slippage.GetHashCode()) return "GetReverseSlippageByShipmentId";
                */
                else if (isOtherCost(id)) return "GetReverseGenericOtherCostByShipmentId";
                else if (id == Id.StudioSampleSales.GetHashCode()) return "GetReverseSalesByShipmentId";
                else if (id == Id.StudioSampleSalesCommission.GetHashCode()) return "GetReverseSalesCommissionByShipmentId";
                else if (id == Id.NSLedStockProvision.GetHashCode()) return "GetReverseNSLedStockProvisionByShipmentId";

                else return "N/A";
            }
            else
            {
                if (id == Id.Purchase.GetHashCode()) return "GetReversePurchaseBySplitShipmentId";
                else return "N/A";
            }
        }



        public static string getConsolidatedDataCommandName(int id)
        {
            if (id == Id.Purchase.GetHashCode()) return "AVAILABLE";
            else return "N/A";
        }

        public static ICollection getNonTradeCollectionValues()
        {
            ArrayList ary = new ArrayList();
            ary.Add(Id.NonTradeExpenseInvoice.GetHashCode());
            /*
            ary.Add(Id.NonTradeRechargeDebitNote.GetHashCode());
            ary.Add(Id.NonTradeBankPayment.GetHashCode());
            */
            ary.Add(Id.NonTradeAccrual.GetHashCode());
            ary.Add(Id.NonTradeRechargeDebitNote.GetHashCode());

            return ary;
        }

        public static ICollection getCollectionValues()
        {
            ArrayList ary = new ArrayList();
            for (int i = 1; i <= 75; i++)
            {
                if (i != 13 && i != 18 && i != 19 && i != 21 && i != 23 && i != 25 && i != 30 && i != 9 && i != 26 && i != 27 && i != 46 && i != 31 && i != 32 && i != 56 && i != 56
                    && i != 62 && i != 63 && i != 64 && i != 67 && i != 68 && i != 69 && i != 74)
                    ary.Add(i);
            }
            return ary;
        }

        public static ArrayList getSunMacroTypeIdList()
        {
            ArrayList list = new ArrayList();
            list.Add(SunInterfaceTypeRef.Id.Sales.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.SalesCommission.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.Purchase.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.GTCommission.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.AirFreightCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.KitDevelopmentCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.FragranceCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.OutsideLabTestCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.PackageCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.ToolingCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.SellingPriceDiscount.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.OtherFabricCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.RepairCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.DesignFee.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.FinanceCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.TransportationCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.ProvisionForFabricLiabilities.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.TemporaryPayment.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.FreightForBodycare.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.SampleLengthCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.CourierCostForSample.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.MarginDifference.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.Recovery.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.NTN_Recovery.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.DevelopmentSampleCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.PrintDevCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.ClaimsRecovery.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.QCC.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.MOB.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.ForwardingCharge.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.LicenseCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.TradingAirFreight.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.ClaimsRecovery_Specific.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.CoatingCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.FreightCost.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.QCCMyanmar.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.FabricCostDiscount.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.Slippage.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.SlippageReversal.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.CMPOrderProcessing.GetHashCode());
            list.Add(SunInterfaceTypeRef.Id.CMPQAComm.GetHashCode());
            return list;
        }

    }
}
