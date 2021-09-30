using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class AmendmentType : DomainData
    {
        public static AmendmentType SELLING_PRICE = new AmendmentType(Code.SellingPrice);
        public static AmendmentType NET_FOB_PRICE = new AmendmentType(Code.NetFOBPrice);
        public static AmendmentType SUPPLIER_PRICE = new AmendmentType(Code.SupplierPrice);
        public static AmendmentType ACTUAL_QUANTITY = new AmendmentType(Code.ActualQty);
        public static AmendmentType OFFICE = new AmendmentType(Code.Office);
        public static AmendmentType TRADING_AGENCY = new AmendmentType(Code.TradingAgency);
        public static AmendmentType CUSTOMER = new AmendmentType(Code.Customer);
        public static AmendmentType PRODUCT_DEPARTMENT = new AmendmentType(Code.ProductDepartment);
        public static AmendmentType PRODUCT_TEAM = new AmendmentType(Code.ProductTeam);
        public static AmendmentType SEASON = new AmendmentType(Code.Season);
        public static AmendmentType ITEM = new AmendmentType(Code.Item);
        public static AmendmentType BUY_CURRENCY = new AmendmentType(Code.BuyCurrency);
        public static AmendmentType SELL_CURRENCY = new AmendmentType(Code.SellCurrency);
        public static AmendmentType VENDOR = new AmendmentType(Code.Vendor);
        public static AmendmentType DESTINATION = new AmendmentType(Code.Destination);
        public static AmendmentType TERM_OF_PURCHASE = new AmendmentType(Code.TermOfPurchase);
        public static AmendmentType PAYMENT_TERM = new AmendmentType(Code.PaymentTerm);
        public static AmendmentType SALES_COMMISSION_PERCENT = new AmendmentType(Code.SalesCommissionPercent);
        public static AmendmentType QA_COMMISSION_PERCENT = new AmendmentType(Code.QACommissionPercent);
        public static AmendmentType FREIGHT_COST = new AmendmentType(Code.FreightCost);
        public static AmendmentType DUTY_COST = new AmendmentType(Code.DutyCost);
        public static AmendmentType TRANSPORTATION_COST = new AmendmentType(Code.TransportationCost);
        public static AmendmentType UK_INVOICE_AMOUNT = new AmendmentType(Code.UKInvoiceAmt);
        public static AmendmentType SELLING_PRICE_DISCOUNT = new AmendmentType(Code.SellingPriceDiscount);
        public static AmendmentType PRINT_AND_DEVELOPMENT_COST = new AmendmentType(Code.PrintDevCost);
        public static AmendmentType OUTSIDE_LAB_TEST_COST = new AmendmentType(Code.OutsideLabTestCost);
        public static AmendmentType UT_INPUT_VAT = new AmendmentType(Code.UTInputVAT);
        public static AmendmentType UT_IMPORT_DUTY = new AmendmentType(Code.UTImportDuty);
        public static AmendmentType UK_SELLING_SURCHARGE_PERCENT = new AmendmentType(Code.UTSellingSurchargePercent);
        public static AmendmentType UT_FOB_SURCHARGE_AMOUNT = new AmendmentType(Code.UTFOBSurchargeAmt);
        public static AmendmentType REPAIR_COST = new AmendmentType(Code.RepairCost);
        public static AmendmentType PACKAGING_COST = new AmendmentType(Code.PackagingCost);
        public static AmendmentType OTHER_FABRIC_COST = new AmendmentType(Code.OtherFabricCost);
        public static AmendmentType FACTORY_PRICE_DISCOUNT = new AmendmentType(Code.FactoryPriceDiscount);
        public static AmendmentType SUPPLIER_INVOICE_NO = new AmendmentType(Code.SupplierInvoiceNo);
        public static AmendmentType INVOICE_NO = new AmendmentType(Code.InvoiceNo);
        public static AmendmentType REDUCED_SELLING_PRICE = new AmendmentType(Code.ReducedSellingPrice);
        public static AmendmentType REDUCED_NET_FOB_PRICE = new AmendmentType(Code.ReducedNetFOBPrice);
        public static AmendmentType REDUCED_SUPPLIER_PRICE = new AmendmentType(Code.ReducedSupplierPrice);
        public static AmendmentType INVOICE_DATE = new AmendmentType(Code.InvoiceDate);
        public static AmendmentType VENDOR_PAYMENT_DISCOUNT = new AmendmentType(Code.VendorPaymentDiscount);
        public static AmendmentType DESIGN_FEE = new AmendmentType(Code.DesignFee);
        public static AmendmentType AGENCY_COMMISSION = new AmendmentType(Code.AgencyCommission);
        public static AmendmentType SHIPPING_DOCUMENT_RECEIPT_DATE = new AmendmentType(Code.ShippingDocumentReceiptDate);
        public static AmendmentType LC_PAYMENT_CHECKED_DATE = new AmendmentType(Code.LcPaymentCheckedDate);
        public static AmendmentType IS_LC_PAYMENT_CHECKED = new AmendmentType(Code.IsLcPaymentChecked);
        public static AmendmentType GT_COMMISSION_PERCENT = new AmendmentType(Code.GTCommissionPercent);
        public static AmendmentType LC_Number = new AmendmentType(Code.LCNumber);
        public static AmendmentType FINANCE_COST = new AmendmentType(Code.FinanceCost);
        public static AmendmentType AIR_FREIGHT_COST = new AmendmentType(Code.AirFreightCost);
        public static AmendmentType CHOICE_ORDER_NSL_COMM_AMT = new AmendmentType(Code.ChoiceOrderNSLCommissionAmount);
        public static AmendmentType CHOICE_ORDER_TOTAL_SHIPPED_AMT = new AmendmentType(Code.ChoiceOrderTotalShippedAmount);
        public static AmendmentType CHOICE_ORDER_TOTAL_SHIPPED_SUPPLIER_GMT_AMT = new AmendmentType(Code.ChoiceOrderTotalShippedSupplierGarmentAmount);
        public static AmendmentType KIT_DEVELOPMENT_COST = new AmendmentType(Code.KitDevelopmentCost);
        public static AmendmentType LAB_TEST_INCOME = new AmendmentType(Code.LabTestIncome);
        public static AmendmentType SAMPLE_LENGTH_COST = new AmendmentType(Code.SampleLengthCost);
        public static AmendmentType FREIGHT_FOR_BODYCARE = new AmendmentType(Code.FreightForBodycare);
        public static AmendmentType COURIER_COST_FOR_SAMPLE = new AmendmentType(Code.CourierCostForSample);
        public static AmendmentType DEVELOPMENT_SAMPLE_COST = new AmendmentType(Code.DevelopmentSampleCost);
        public static AmendmentType MARGIN_DIFFERENCE = new AmendmentType(Code.MarginDifference);
        public static AmendmentType RECOVERY = new AmendmentType(Code.Recovery);
        public static AmendmentType NTN_RECOVERY = new AmendmentType(Code.NTNRecovery);
        public static AmendmentType COVER_FOR_QUENBY_FABRIC = new AmendmentType(Code.CoverForQuenbyFabric);
        public static AmendmentType CLAIMS_RECOVERY = new AmendmentType(Code.ClaimsRecovery);
        public static AmendmentType QCC = new AmendmentType(Code.QCC);
        public static AmendmentType MOB = new AmendmentType(Code.MOB);
        public static AmendmentType CHINA_GB_TEST_REQUIREMENT = new AmendmentType(Code.ChinaGBTestRequirement);
        public static AmendmentType FORWARDING_CHARGE = new AmendmentType(Code.ForwardingCharge);
        public static AmendmentType LICENSE_COST = new AmendmentType(Code.LicenseCost);
        public static AmendmentType TRADING_AIR_FREIGHT = new AmendmentType(Code.TradingAirFreight);
        public static AmendmentType SHORT_GAME_INDICATOR = new AmendmentType(Code.ShortGameIndicator);
        public static AmendmentType CLAIMS_RECOVERY_SPECIFIC = new AmendmentType(Code.ClaimsRecoverySpecific);
        public static AmendmentType COATING_COST = new AmendmentType(Code.CoatingCost);
        public static AmendmentType QCC_MYANMAR = new AmendmentType(Code.QCCMyanmar);
        public static AmendmentType FABRIC_COST_DISCOUNT = new AmendmentType(Code.FabricCostDiscount);
        public static AmendmentType SLIPPAGE = new AmendmentType(Code.Slippage);
        public static AmendmentType SHIPMENT_DEDUCTION = new AmendmentType(Code.ShipmentDeduction);
        public static AmendmentType GSPFORMTYPE = new AmendmentType(Code.GSPFromType);
        public static AmendmentType CMP_ORDER_PROCESSING = new AmendmentType(Code.CMPOrderProcessing);
        public static AmendmentType CMP_QA_COMM = new AmendmentType(Code.CMPQAComm);

        private Code _code;

        private enum Code
        {
            SellingPrice = 1,
            NetFOBPrice = 2,
            SupplierPrice = 3,
            ActualQty = 4,
            Office = 5,
            TradingAgency = 6,
            Customer = 7,
            ProductDepartment = 8,
            ProductTeam = 9,
            Season = 10,
            Item = 11,
            BuyCurrency = 12,
            SellCurrency = 13,
            Vendor = 14,
            Destination = 15,
            TermOfPurchase = 16,
            PaymentTerm = 17,
            SalesCommissionPercent = 18,
            QACommissionPercent = 19,
            FreightCost = 20,
            DutyCost = 21,
            TransportationCost = 22,
            UKInvoiceAmt = 23,
            SellingPriceDiscount = 24,
            PrintDevCost = 25,
            OutsideLabTestCost = 26,
            UTInputVAT = 27,
            UTImportDuty = 28,
            UTSellingSurchargePercent = 29,
            UTFOBSurchargeAmt = 30,
            RepairCost = 31,
            PackagingCost = 32,
            OtherFabricCost = 33,
            FactoryPriceDiscount = 34,
            SupplierInvoiceNo = 35,
            InvoiceNo = 36,
            ReducedSellingPrice = 37,
            ReducedNetFOBPrice = 38, 
            ReducedSupplierPrice = 39,
            InvoiceDate = 40,
            VendorPaymentDiscount = 41,
            DesignFee = 42,
            AgencyCommission = 43,
            ShippingDocumentReceiptDate = 44,
            LcPaymentCheckedDate = 45,
            IsLcPaymentChecked = 46,
            GTCommissionPercent = 47,
            LCNumber = 48,
            FinanceCost = 49,
            AirFreightCost = 50,
            ChoiceOrderNSLCommissionAmount = 51,
            ChoiceOrderTotalShippedAmount = 52,
            ChoiceOrderTotalShippedSupplierGarmentAmount = 53,
            KitDevelopmentCost = 54,
            LabTestIncome = 55,
            SampleLengthCost = 56,
            FreightForBodycare = 57,
            CourierCostForSample = 58,
            DevelopmentSampleCost = 59,
            MarginDifference = 60,
            Recovery = 61,
            NTNRecovery = 62,
            CoverForQuenbyFabric = 63,
            ClaimsRecovery = 64,
            QCC = 65,
            MOB = 66,
            ChinaGBTestRequirement = 67,
            ForwardingCharge = 68,
            LicenseCost = 69,
            TradingAirFreight = 70,
            ShortGameIndicator = 71,
            ClaimsRecoverySpecific = 72,
            CoatingCost = 73,
            QCCMyanmar = 74,
            FabricCostDiscount = 75,
            Slippage = 76,
            ShipmentDeduction = 77,
            GSPFromType = 78,
            CMPOrderProcessing = 79,
            CMPQAComm = 80
        }

        private AmendmentType(Code code)
        {
            this._code = code;
        }

        public int Id
        {
            get { return Convert.ToUInt16(_code.GetHashCode()); }
        }

        public string Description
        {
            get
            {
                switch (_code)
                {
                    case Code.SellingPrice:
                        return "Selling Price";
                    case Code.NetFOBPrice:
                        return "Net FOB Price";
                    case Code.SupplierPrice:
                        return "Supplier Price";
                    case Code.ActualQty:
                        return "Actual Quantity";
                    case Code.Office:
                        return "Office";
                    case Code.TradingAgency:
                        return "Trading Agency";
                    case Code.Customer:
                        return "Customer";
                    case Code.ProductDepartment:
                        return "Product Department";
                    case Code.ProductTeam:
                        return "Product Team";
                    case Code.Season:
                        return "Season";
                    case Code.Item:
                        return "Item";
                    case Code.BuyCurrency:
                        return "Buy Currency";
                    case Code.SellCurrency:
                        return "Sell Currency";
                    case Code.Vendor:
                        return "Vendor";
                    case Code.Destination:
                        return "Destination";
                    case Code.TermOfPurchase:
                        return "Term Of Purchase";
                    case Code.PaymentTerm:
                        return "Payment Term";
                    case Code.SalesCommissionPercent:
                        return "Sales Commission Percent";
                    case Code.QACommissionPercent:
                        return "QA Commission Percent";
                    case Code.FreightCost:
                        return "Freight Cost";
                    case Code.DutyCost:
                        return "Duty Cost";
                    case Code.TransportationCost:
                        return "Transportation Cost";
                    case Code.UKInvoiceAmt:
                        return "UK Invoice Amount";
                    case Code.SellingPriceDiscount:
                        return "Selling Price Discount";
                    case Code.PrintDevCost:
                        return "Print & Development Cost";
                    case Code.OutsideLabTestCost:
                        return "Outside Lab Test Cost";
                    case Code.UTInputVAT:
                        return "UT - Input VAT";
                    case Code.UTImportDuty:
                        return "UT - Import Duty";
                    case Code.UTSellingSurchargePercent:
                        return "UT - Selling Surcharge Percent";
                    case Code.UTFOBSurchargeAmt:
                        return "UT - FOB Surcharge Amount";
                    case Code.RepairCost:
                        return "Repair Cost";
                    case Code.PackagingCost:
                        return "Packaging Cost";
                    case Code.OtherFabricCost:
                        return "Other Fabric Cost";
                    case Code.FactoryPriceDiscount:
                        return "Factory Price Discount";
                    case Code.SupplierInvoiceNo:
                        return "Supplier Invoice No";
                    case Code.InvoiceNo:
                        return "Invoice No";
                    case Code.ReducedSellingPrice:
                        return "Reduced Selling Price";
                    case Code.ReducedNetFOBPrice:
                        return "Reduced Net FOB Price";
                    case Code.ReducedSupplierPrice:
                        return "Reduced Supplier Price";
                    case Code.InvoiceDate:
                        return "Invoice Date";
                    case Code.VendorPaymentDiscount:
                        return "Vendor Payment Discount";
                    case Code.DesignFee:
                        return "Design Fee";
                    case Code.AgencyCommission:
                        return "Agency Commission";
                    case Code.ShippingDocumentReceiptDate: 
                        return "Shipping Document Receipt Date";
                    case Code.LcPaymentCheckedDate:
                        return "LC Payment Checked Date";
                    case Code.IsLcPaymentChecked:
                        return "Is LC Payment Checked";
                    case Code.GTCommissionPercent:
                        return "GT Commission Percent";
                    case Code.LCNumber:
                        return "LC Number";
                    case Code.FinanceCost:
                        return "Finance Cost";
                    case Code.AirFreightCost:
                        return "Air Freight Cost";
                    case Code.ChoiceOrderNSLCommissionAmount:
                        return "Choice Order NSL Commission Amount";
                    case Code.ChoiceOrderTotalShippedAmount:
                        return "Choice Order Total Shipped Amount";
                    case Code.ChoiceOrderTotalShippedSupplierGarmentAmount:
                        return "Choice Order Total Shipped Supplier Garment Amount";
                    case Code.KitDevelopmentCost:
                        return "Kit Development Cost";
                    case Code.LabTestIncome:
                        return "Lab Test Income";
                    case Code.SampleLengthCost:
                        return "Sample Length Cost";
                    case Code.FreightForBodycare:
                        return "Freight For Bodycare";
                    case Code.CourierCostForSample:
                        return "Courier Cost For Sample";
                    case Code.DevelopmentSampleCost:
                        return "Development Sample Cost";
                    case Code.MarginDifference:
                        return "Margin Difference";
                    case Code.Recovery:
                        return "Recovery";
                    case Code.NTNRecovery:
                        return "NTN Recovery";
                    case Code.CoverForQuenbyFabric:
                        return "Cover For Quenby Fabric";
                    case Code.ClaimsRecovery:
                        return "Claims Recovery";
                    case Code.QCC:
                        return "QCC";
                    case Code.MOB:
                        return "MOB";
                    case Code.ChinaGBTestRequirement:
                        return "China GB Test Requirement";
                    case Code.ForwardingCharge:
                        return "Forwarding Charge";
                    case Code.LicenseCost:
                        return "License Cost";
                    case Code.TradingAirFreight:
                        return "Trading Air Freight";
                    case Code.ShortGameIndicator:
                        return "Short Game Indicator";
                    case Code.ClaimsRecoverySpecific:
                        return "Claims Recovery (Specific)";
                    case Code.CoatingCost:
                        return "Coating Cost";
                    case Code.QCCMyanmar:
                        return "QCC (Myanmar)";
                    case Code.FabricCostDiscount:
                        return "Fabric Cost Discount";
                    case Code.Slippage:
                        return "Margin Slippage";
                    case Code.ShipmentDeduction:
                        return "Shipment Deduction";
                    case Code.GSPFromType:
                        return "GSP Form Type";
                    case Code.CMPOrderProcessing:
                        return "CMP Order Processing";
                    case Code.CMPQAComm:
                        return "CMP QA Comm";
                    default:
                        return "ERROR";
                }
            }
        }

        public static AmendmentType getType(int id)
        {
            if (id == Code.SellingPrice.GetHashCode()) return AmendmentType.SELLING_PRICE;
            else if (id == Code.NetFOBPrice.GetHashCode()) return AmendmentType.NET_FOB_PRICE;
            else if (id == Code.SupplierPrice.GetHashCode()) return AmendmentType.SUPPLIER_PRICE;
            else if (id == Code.ActualQty.GetHashCode()) return AmendmentType.ACTUAL_QUANTITY;
            else if (id == Code.Office.GetHashCode()) return AmendmentType.OFFICE;
            else if (id == Code.TradingAgency.GetHashCode()) return AmendmentType.TRADING_AGENCY;
            else if (id == Code.Customer.GetHashCode()) return AmendmentType.CUSTOMER;
            else if (id == Code.ProductDepartment.GetHashCode()) return AmendmentType.PRODUCT_DEPARTMENT;
            else if (id == Code.ProductTeam.GetHashCode()) return AmendmentType.PRODUCT_TEAM;
            else if (id == Code.Season.GetHashCode()) return AmendmentType.SEASON;
            else if (id == Code.Item.GetHashCode()) return AmendmentType.ITEM;
            else if (id == Code.BuyCurrency.GetHashCode()) return AmendmentType.BUY_CURRENCY;
            else if (id == Code.SellCurrency.GetHashCode()) return AmendmentType.SELL_CURRENCY;
            else if (id == Code.Vendor.GetHashCode()) return AmendmentType.VENDOR;
            else if (id == Code.Destination.GetHashCode()) return AmendmentType.DESTINATION;
            else if (id == Code.TermOfPurchase.GetHashCode()) return AmendmentType.TERM_OF_PURCHASE;
            else if (id == Code.PaymentTerm.GetHashCode()) return AmendmentType.PAYMENT_TERM;
            else if (id == Code.SalesCommissionPercent.GetHashCode()) return AmendmentType.SALES_COMMISSION_PERCENT;
            else if (id == Code.QACommissionPercent.GetHashCode()) return AmendmentType.QA_COMMISSION_PERCENT;
            else if (id == Code.FreightCost.GetHashCode()) return AmendmentType.FREIGHT_COST;
            else if (id == Code.DutyCost.GetHashCode()) return AmendmentType.DUTY_COST;
            else if (id == Code.TransportationCost.GetHashCode()) return AmendmentType.TRANSPORTATION_COST;
            else if (id == Code.UKInvoiceAmt.GetHashCode()) return AmendmentType.UK_INVOICE_AMOUNT;
            else if (id == Code.SellingPriceDiscount.GetHashCode()) return AmendmentType.SELLING_PRICE_DISCOUNT;
            else if (id == Code.PrintDevCost.GetHashCode()) return AmendmentType.PRINT_AND_DEVELOPMENT_COST;
            else if (id == Code.OutsideLabTestCost.GetHashCode()) return AmendmentType.OUTSIDE_LAB_TEST_COST;
            else if (id == Code.UTInputVAT.GetHashCode()) return AmendmentType.UT_INPUT_VAT;
            else if (id == Code.UTImportDuty.GetHashCode()) return AmendmentType.UT_IMPORT_DUTY;
            else if (id == Code.UTSellingSurchargePercent.GetHashCode()) return AmendmentType.UK_SELLING_SURCHARGE_PERCENT;
            else if (id == Code.UTFOBSurchargeAmt.GetHashCode()) return AmendmentType.UT_FOB_SURCHARGE_AMOUNT;
            else if (id == Code.RepairCost.GetHashCode()) return AmendmentType.REPAIR_COST;
            else if (id == Code.PackagingCost.GetHashCode()) return AmendmentType.PACKAGING_COST;
            else if (id == Code.OtherFabricCost.GetHashCode()) return AmendmentType.OTHER_FABRIC_COST;
            else if (id == Code.FactoryPriceDiscount.GetHashCode()) return AmendmentType.FACTORY_PRICE_DISCOUNT;
            else if (id == Code.SupplierInvoiceNo.GetHashCode()) return AmendmentType.SUPPLIER_INVOICE_NO;
            else if (id == Code.InvoiceNo.GetHashCode()) return AmendmentType.INVOICE_NO;
            else if (id == Code.ReducedSellingPrice.GetHashCode()) return AmendmentType.REDUCED_SELLING_PRICE;
            else if (id == Code.ReducedNetFOBPrice.GetHashCode()) return AmendmentType.REDUCED_NET_FOB_PRICE;
            else if (id == Code.ReducedSupplierPrice.GetHashCode()) return AmendmentType.REDUCED_SUPPLIER_PRICE;
            else if (id == Code.InvoiceDate.GetHashCode()) return AmendmentType.INVOICE_DATE;
            else if (id == Code.VendorPaymentDiscount.GetHashCode()) return AmendmentType.VENDOR_PAYMENT_DISCOUNT;
            else if (id == Code.DesignFee.GetHashCode()) return AmendmentType.DESIGN_FEE;
            else if (id == Code.AgencyCommission.GetHashCode()) return AmendmentType.AGENCY_COMMISSION;
            else if (id == Code.ShippingDocumentReceiptDate.GetHashCode()) return AmendmentType.SHIPPING_DOCUMENT_RECEIPT_DATE;
            else if (id == Code.LcPaymentCheckedDate.GetHashCode()) return AmendmentType.LC_PAYMENT_CHECKED_DATE;
            else if (id == Code.IsLcPaymentChecked.GetHashCode()) return AmendmentType.IS_LC_PAYMENT_CHECKED;
            else if (id == Code.GTCommissionPercent.GetHashCode()) return AmendmentType.GT_COMMISSION_PERCENT;
            else if (id == Code.LCNumber.GetHashCode()) return AmendmentType.LC_Number;
            else if (id == Code.FinanceCost.GetHashCode()) return AmendmentType.FINANCE_COST;
            else if (id == Code.AirFreightCost.GetHashCode()) return AmendmentType.AIR_FREIGHT_COST;
            else if (id == Code.ChoiceOrderNSLCommissionAmount.GetHashCode()) return AmendmentType.CHOICE_ORDER_NSL_COMM_AMT;
            else if (id == Code.ChoiceOrderTotalShippedAmount.GetHashCode()) return AmendmentType.CHOICE_ORDER_TOTAL_SHIPPED_AMT;
            else if (id == Code.ChoiceOrderTotalShippedSupplierGarmentAmount.GetHashCode()) return AmendmentType.CHOICE_ORDER_TOTAL_SHIPPED_SUPPLIER_GMT_AMT;
            else if (id == Code.KitDevelopmentCost.GetHashCode()) return AmendmentType.KIT_DEVELOPMENT_COST;
            else if (id == Code.LabTestIncome.GetHashCode()) return AmendmentType.LAB_TEST_INCOME;
            else if (id == Code.SampleLengthCost.GetHashCode()) return AmendmentType.SAMPLE_LENGTH_COST;
            else if (id == Code.FreightForBodycare.GetHashCode()) return AmendmentType.FREIGHT_FOR_BODYCARE;
            else if (id == Code.CourierCostForSample.GetHashCode()) return AmendmentType.COURIER_COST_FOR_SAMPLE;
            else if (id == Code.DevelopmentSampleCost.GetHashCode()) return AmendmentType.DEVELOPMENT_SAMPLE_COST;
            else if (id == Code.MarginDifference.GetHashCode()) return AmendmentType.MARGIN_DIFFERENCE;
            else if (id == Code.Recovery.GetHashCode()) return AmendmentType.RECOVERY;
            else if (id == Code.NTNRecovery.GetHashCode()) return AmendmentType.NTN_RECOVERY;
            else if (id == Code.CoverForQuenbyFabric.GetHashCode()) return AmendmentType.COVER_FOR_QUENBY_FABRIC;
            else if (id == Code.ClaimsRecovery.GetHashCode()) return AmendmentType.CLAIMS_RECOVERY;
            else if (id == Code.QCC.GetHashCode()) return AmendmentType.QCC;
            else if (id == Code.MOB.GetHashCode()) return AmendmentType.MOB;
            else if (id == Code.ForwardingCharge.GetHashCode()) return AmendmentType.FORWARDING_CHARGE;
            else if (id == Code.ChinaGBTestRequirement.GetHashCode()) return AmendmentType.CHINA_GB_TEST_REQUIREMENT;
            else if (id == Code.LicenseCost.GetHashCode()) return AmendmentType.LICENSE_COST;
            else if (id == Code.TradingAirFreight.GetHashCode()) return AmendmentType.TRADING_AIR_FREIGHT;
            else if (id == Code.ShortGameIndicator.GetHashCode()) return AmendmentType.SHORT_GAME_INDICATOR;
            else if (id == Code.ClaimsRecoverySpecific.GetHashCode()) return AmendmentType.CLAIMS_RECOVERY_SPECIFIC;
            else if (id == Code.CoatingCost.GetHashCode()) return AmendmentType.COATING_COST;
            else if (id == Code.QCCMyanmar.GetHashCode()) return AmendmentType.QCC_MYANMAR;
            else if (id == Code.FabricCostDiscount.GetHashCode()) return AmendmentType.FABRIC_COST_DISCOUNT;
            else if (id == Code.Slippage.GetHashCode()) return AmendmentType.SLIPPAGE;
            else if (id == Code.ShipmentDeduction.GetHashCode()) return AmendmentType.SHIPMENT_DEDUCTION;
            else if (id == Code.GSPFromType.GetHashCode()) return AmendmentType.GSPFORMTYPE;
            else if (id == Code.CMPOrderProcessing.GetHashCode()) return AmendmentType.CMP_ORDER_PROCESSING;
            else if (id == Code.CMPQAComm.GetHashCode()) return AmendmentType.CMP_QA_COMM;
            else return null;
        }

        public static bool isOtherCost(int id)
        {
            if (id == Code.TransportationCost.GetHashCode()) return true;
            else if (id == Code.SellingPriceDiscount.GetHashCode()) return true;
            else if (id == Code.OutsideLabTestCost.GetHashCode()) return true;
            else if (id == Code.RepairCost.GetHashCode()) return true;
            else if (id == Code.PackagingCost.GetHashCode()) return true;
            else if (id == Code.OtherFabricCost.GetHashCode()) return true;
            else if (id == Code.DesignFee.GetHashCode()) return true;
            else if (id == Code.FinanceCost.GetHashCode()) return true;
            else if (id == Code.AirFreightCost.GetHashCode()) return true;
            else if (id == Code.KitDevelopmentCost.GetHashCode()) return true;
            else if (id == Code.SampleLengthCost.GetHashCode()) return true;
            else if (id == Code.FreightForBodycare.GetHashCode()) return true;
            else if (id == Code.CourierCostForSample.GetHashCode()) return true;
            else if (id == Code.DevelopmentSampleCost.GetHashCode()) return true;
            else if (id == Code.MarginDifference.GetHashCode()) return true;
            else if (id == Code.Recovery.GetHashCode()) return true;
            else if (id == Code.NTNRecovery.GetHashCode()) return true;
            else if (id == Code.CoverForQuenbyFabric.GetHashCode()) return true;
            else if (id == Code.ClaimsRecovery.GetHashCode()) return true;
            else if (id == Code.QCC.GetHashCode()) return true;
            else if (id == Code.MOB.GetHashCode()) return true;
            else if (id == Code.ForwardingCharge.GetHashCode()) return true;
            else if (id == Code.LicenseCost.GetHashCode()) return true;
            else if (id == Code.TradingAirFreight.GetHashCode()) return true;
            else if (id == Code.ClaimsRecoverySpecific.GetHashCode()) return true;
            else if (id == Code.DutyCost.GetHashCode()) return true;
            else if (id == Code.PrintDevCost.GetHashCode()) return true;
            else if (id == Code.CoatingCost.GetHashCode()) return true;
            else if (id == Code.QCCMyanmar.GetHashCode()) return true;
            else if (id == Code.FabricCostDiscount.GetHashCode()) return true;
            else if (id == Code.Slippage.GetHashCode()) return true;
            else if (id == Code.CMPOrderProcessing.GetHashCode()) return true;
            else if (id == Code.CMPQAComm.GetHashCode()) return true;
            else return false;
        }

        public static bool isSizeOptionAmendmentType(int id)
        {
            if (id == Code.SellingPrice.GetHashCode()) return true;
            else if (id == Code.NetFOBPrice.GetHashCode()) return true;
            else if (id == Code.SupplierPrice.GetHashCode()) return true;
            else if (id == Code.ActualQty.GetHashCode()) return true;
            else if (id == Code.ReducedSellingPrice.GetHashCode()) return true;
            else if (id == Code.ReducedNetFOBPrice.GetHashCode()) return true;
            else if (id == Code.ReducedSupplierPrice.GetHashCode()) return true;
            else return false;
        }

        public static bool isSalesMarginAffected(int id)
        {
            if (id == Code.SellingPrice.GetHashCode()) return true;
            else if (id == Code.NetFOBPrice.GetHashCode()) return true;
            else if (id == Code.SupplierPrice.GetHashCode()) return true;
            else if (id == Code.ReducedSellingPrice.GetHashCode()) return true;
            else if (id == Code.ReducedNetFOBPrice.GetHashCode()) return true;
            else if (id == Code.ReducedSupplierPrice.GetHashCode()) return true;
            else if (isOtherCost(id)) return true;
            else return false;
        }

    }
}
