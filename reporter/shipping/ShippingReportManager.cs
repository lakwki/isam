using System;
using System.Data;
using System.Linq;
using System.Collections;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.common;
using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.ils;
using com.next.isam.domain.types;
using com.next.isam.reporter.dataserver;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.infra.util;
using System.Collections.Generic;


namespace com.next.isam.reporter.shipping
{
    public class ShippingReportManager
    {
        private static ShippingReportManager _instance;
        private CommonWorker commonWorker;
        private const string AllSampleOrderText = "Mock Shop/Press/Studio Sample";

        public ShippingReportManager()
        {
            commonWorker = CommonWorker.Instance;
        }

        public static ShippingReportManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShippingReportManager();
                }
                return _instance;
            }
        }

        public CTSSTWReport getCTSSTWReport(DateTime CTSDateFrom, DateTime CTSDateTo, DateTime STWDateFrom, DateTime STWDateTo,
            int dualSourcingOrder, int selfBilledOrder, int nslSZOrder, int utOrder, int isLDPOrder, int isSampleOrder, TypeCollector workflowStatusList,
            TypeCollector shipmentMethodList
            , TypeCollector customerList, int packingMethodId, int countryOfOriginId, TypeCollector officeIdList,
            TypeCollector productTeamList, int handlingOfficeId, int seasonId, int shipmentPortId, int oprType, TypeCollector customerDestination, 
            int termOfPurchaseId, bool showProductTeam, string sortingField, int userId)
        {
            CTSSTWReport report = new CTSSTWReport();
            CTSSTWReportDs dataset;
            

            dataset = ReporterWorker.Instance.getCTSSTWReport(CTSDateFrom, CTSDateTo, STWDateFrom, STWDateTo, dualSourcingOrder, selfBilledOrder,
                nslSZOrder, utOrder, isLDPOrder, isSampleOrder, workflowStatusList, shipmentMethodList, customerList, packingMethodId, countryOfOriginId, officeIdList,
                productTeamList, handlingOfficeId, seasonId, shipmentPortId, oprType, customerDestination, termOfPurchaseId, sortingField);

            report.SetDataSource(dataset);
            
            report.SetParameterValue("STWDateFrom", DateTimeUtility.getDateString(STWDateFrom));
            report.SetParameterValue("STWDateTo", DateTimeUtility.getDateString(STWDateTo));
            report.SetParameterValue("CTSDateFrom", DateTimeUtility.getDateString(CTSDateFrom));
            report.SetParameterValue("CTSDateTo", DateTimeUtility.getDateString(CTSDateTo));

            string orderType = "";
            if (workflowStatusList.Values.Count != 3)
            {
                foreach (object obj in workflowStatusList.Values)
                {
                    if (orderType != "")
                        orderType += ", ";
                    orderType += ContractWFS.getShortName(Convert.ToInt32(obj));
                }
            }
            if (selfBilledOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                if (selfBilledOrder == 1)
                    orderType += "Self-Billed";
                else
                    orderType += "Non-Self-Billed";
            }

            if (dualSourcingOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                if (dualSourcingOrder == 1)
                    orderType += "Dual Sourcing";
                else
                    orderType += "Non-Dual Sourcing";
            }
            if (nslSZOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                if (nslSZOrder == 1)
                    orderType += "NSL SZ Order";
                else
                    orderType += "Non-NSL SZ Order";
            }
            if (isLDPOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                if (isLDPOrder == 1)
                    orderType += "LDP Order";
                else
                    orderType += "Non-LDP Order";
            }
            if (isSampleOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isSampleOrder == 1 ? AllSampleOrderText : "Mainline";
            }

            report.SetParameterValue("orderType", orderType == "" ? "ALL" : orderType);
            report.SetParameterValue("PackingMethod", packingMethodId == -1 ? "ALL" : commonWorker.getPackingMethodByKey(packingMethodId).PackingMethodDescription);
            //report.SetParameterValue("Office", officeId == -1 ? "ALL" : GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode);
            report.SetParameterValue("CountryOfOrigin", countryOfOriginId == -1 ? "ALL" : GeneralWorker.Instance.getCountryOfOriginByKey(countryOfOriginId).Name);
            report.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "ALL" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
            if (dataset.CTSSTWReport.Rows.Count != 0)
            {
                report.SetParameterValue("Season", seasonId == -1 ? "ALL" : GeneralWorker.Instance.getSeasonByKey(seasonId).Code);                
                report.SetParameterValue("ShipmentPort", shipmentPortId == -1 ? "ALL" : dataset.CTSSTWReport.Rows[0]["ShipmentPort"].ToString());
                report.SetParameterValue("OPRType", oprType == -1 ? "ALL" : dataset.CTSSTWReport.Rows[0]["OPRType"].ToString());
            }
            else
            {
                report.SetParameterValue("Season", seasonId == -1 ? "ALL" : GeneralWorker.Instance.getSeasonByKey(seasonId).Code);                
                report.SetParameterValue("ShipmentPort", shipmentPortId == -1 ? "ALL" : commonWorker.getShipmentPortByKey(shipmentPortId).ShipmentPortDescription);
                report.SetParameterValue("OPRType", oprType == -1 ? "ALL" : OPRFabricType.getName(oprType));
            }

            string office = "";
            foreach (object id in officeIdList.Values)
            {
                office += GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).Description + ", ";
            }
            if (office != "")
            {
                office = office.Remove(office.LastIndexOf(", "));
            }
            report.SetParameterValue("Office", office);
            string productTeam = "";

            if (showProductTeam)
            {
                foreach (object productTeamId in productTeamList.Values)
                {
                    ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                    if (!productTeam.Contains(pc.Code + ", "))
                        productTeam += pc.Code + ", ";
                }
                if (productTeam != "")
                    productTeam = productTeam.Remove(productTeam.LastIndexOf(", "));
            }
            report.SetParameterValue("ProductTeam", productTeam);

            if (shipmentMethodList.Values.Count == 5)
                report.SetParameterValue("ShipmentMethod", "ALL");
            else
            {
                string shipmentMethod = "";
                foreach (object obj in shipmentMethodList.Values)
                {
                    shipmentMethod += commonWorker.getShipmentMethodByKey(Convert.ToInt32(obj)).ShipmentMethodDescription + ", ";
                }
                shipmentMethod = shipmentMethod.Remove(shipmentMethod.LastIndexOf(", "));
                report.SetParameterValue("ShipmentMethod", shipmentMethod);
            }
            if (customerList.Values.Count == 9)
                report.SetParameterValue("Customer", "ALL");
            else
            {
                string cust = "";
                foreach (object obj in customerList.Values)
                {
                    cust += commonWorker.getCustomerByKey(Convert.ToInt32(obj)).CustomerCode + ", ";
                }
                cust = cust.Remove(cust.LastIndexOf(", "));
                report.SetParameterValue("Customer", cust);
            }
            
            if (customerDestination.Values.Count == 1)
            {
                foreach (object obj in customerDestination.Values)
                {
                    report.SetParameterValue("Destination", commonWorker.getCustomerDestinationByKey(Convert.ToInt32(obj)).DestinationDesc);
                }
            }
            else
                report.SetParameterValue("Destination", "ALL");

            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

             return report;
        }


        public WeeklyShipmentReport getWeeklyShipmentReport(DateTime stockToWHDateFrom, DateTime stockToWHDateTo,
            DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo, DateTime bookingDateFrom, DateTime bookingDateTo,
            DateTime bookedAtWHDateFrom, DateTime bookedAtWHDateTo, DateTime departDateFrom, DateTime departDateTo, string voyageNo, TypeCollector officeIdList, TypeCollector productTeamList, int handlingOfficeId, TypeCollector customerIdList,
            TypeCollector countryOfOrigin, TypeCollector customerDestination, TypeCollector shipmentPort, TypeCollector shipmentMethod, TypeCollector packingMethod, int vendorId,
            TypeCollector termOfPurchase, TypeCollector oprFabricType, int isNextMfgorder, int isDualSourcingOrder, int isLDPOrder, int isSampleOrder,
            string lcNoFrom, string lcNoTo, int paymentTermId, TypeCollector workflowStatus,
            string sortField, int userId)
        {
            WeeklyShipmentReport report = new WeeklyShipmentReport();
            WeeklyShipmentReportDs dataset;

            if (lcNoFrom.Trim() == "" || lcNoTo.Trim() == "")
                lcNoFrom = lcNoTo = lcNoFrom.Trim() + lcNoTo.Trim();
            dataset = ReporterWorker.Instance.getWeeklyShipmentReport(stockToWHDateFrom, stockToWHDateTo, customerAtWHDateFrom, customerAtWHDateTo,
                                bookingDateFrom, bookingDateTo, bookedAtWHDateFrom, bookedAtWHDateTo, departDateFrom, departDateTo, voyageNo, officeIdList, productTeamList, handlingOfficeId, customerIdList,
                                countryOfOrigin, customerDestination, shipmentPort, shipmentMethod, packingMethod, vendorId,
                                termOfPurchase, oprFabricType, isNextMfgorder, isDualSourcingOrder, isLDPOrder, isSampleOrder,
                                lcNoFrom, lcNoTo, paymentTermId, workflowStatus,
                                sortField);


            report.SetDataSource(dataset);

            report.SetParameterValue("StockToWHDateFrom", DateTimeUtility.getDateString(stockToWHDateFrom));
            report.SetParameterValue("StockToWHDateTo", DateTimeUtility.getDateString(stockToWHDateTo));
            report.SetParameterValue("CustomerAtWHDateFrom", DateTimeUtility.getDateString(customerAtWHDateFrom));
            report.SetParameterValue("CustomerAtWHDateTo", DateTimeUtility.getDateString(customerAtWHDateTo));
            report.SetParameterValue("BookingDateFrom", DateTimeUtility.getDateString(bookingDateFrom));
            report.SetParameterValue("BookingDateTo", DateTimeUtility.getDateString(bookingDateTo));
            report.SetParameterValue("BookedAtWHDateFrom", DateTimeUtility.getDateString(bookedAtWHDateFrom));
            report.SetParameterValue("BookedAtWHDateTo", DateTimeUtility.getDateString(bookedAtWHDateTo));
            report.SetParameterValue("DepartDateFrom", DateTimeUtility.getDateString(departDateFrom));
            report.SetParameterValue("DepartDateTo", DateTimeUtility.getDateString(departDateTo));
            report.SetParameterValue("VoyageNo", voyageNo == "" ? "ALL" : voyageNo);

            string orderType = "";
            
            if (isDualSourcingOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                if (isDualSourcingOrder == 1)
                    orderType += "Dual Sourcing";
                else
                    orderType += "Non-Dual Sourcing";
            }
            if (isNextMfgorder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                if (isNextMfgorder == 1)
                    orderType += "NSL SZ Order";
                else
                    orderType += "Non-NSL SZ Order";
            }
            if (isLDPOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                if (isLDPOrder == 1)
                    orderType += "LDP Order";
                else
                    orderType += "Non-LDP Order";
            }
            if (isSampleOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += (isSampleOrder == 1 ? AllSampleOrderText : "Mainline") + " Order";
            }

            #region Trial Version
            /*
            if (isSampleOrder == -1 && isNextMfgorder == -1 && isDualSourcingOrder == -1 && isLDPOrder == -1)
                orderType = "ALL";
            else
                if (isSampleOrder >1 && isNextMfgorder >1 && isDualSourcingOrder >1 && isLDPOrder >1)
                orderType = "N/A";
            else
            {
                orderType += (isSampleOrder == 0 ? ", Mainline Order" : (isSampleOrder == 1 ? ", Mock Shop/Press Sample Order" : (isSampleOrder == -1 ? ", Mainline Order, Mock Shop/Press Sample Order" : ", (Not Mainline nor Mock Shop/Press Sample Order)")));
                orderType += (isNextMfgorder == 0 ? ", NSL SZ Order" : (isNextMfgorder == 1 ? ", Non-NSL SZ Order" : ((isNextMfgorder == -1 ? ", NSL SZ Order, Non-NSL SZ Order" : ", (Not NSL SZ nor Non-NSL SZ Order)"))));
                orderType += (isDualSourcingOrder == 0 ? ", Dual Sourcing" : (isDualSourcingOrder == 1 ? ", Non-Dual Sourcing" : (isDualSourcingOrder == -1 ? ", Dual Sourcing, Non-Dual Sourcing" : ", (Not Dual Sourcing nor Non-Dual Sourcing Order)")));
                orderType += (isLDPOrder == 0 ? ", LDP Order" : (isLDPOrder == 1 ? ", Non-LDP Order" : (isLDPOrder == -1 ? ", LDP Order, Non-LDP Order" : ", (Not LDP Order nor Non-LDP Order)")));
                orderType = (orderType == "" ? "" : orderType.Substring(2));
            }
            */
            #endregion

            int keyId;
            string desc;
            ArrayList list;
            desc = string.Empty;
            list=(ArrayList)packingMethod.Values;
            for (int i = 0; i < packingMethod.Values.Count&&desc!="ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc+(desc == string.Empty ? "" : ", ") + commonWorker.getPackingMethodByKey(keyId).PackingMethodDescription);
            report.SetParameterValue("PackingMethod", desc);

            desc = string.Empty;
            list = (ArrayList)countryOfOrigin.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc+(desc == string.Empty ? "" : ", ") + GeneralWorker.Instance.getCountryOfOriginByKey(keyId).Name);
            report.SetParameterValue("CountryOfOrigin", desc);

            desc = string.Empty;
            list = (ArrayList)customerDestination.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getCustomerDestinationByKey(keyId).DestinationDesc);
            report.SetParameterValue("CustomerDestination", desc);

            desc = string.Empty;
            list = (ArrayList)shipmentPort.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getShipmentPortByKey(keyId).ShipmentPortDescription);
            report.SetParameterValue("ShipmentPort", desc);

            report.SetParameterValue("Vendor", vendorId == -1 ? "ALL" : VendorWorker.Instance.getVendorByKey(vendorId).Name);

            desc = string.Empty;
            list = (ArrayList)termOfPurchase.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getTermOfPurchaseByKey(keyId).TermOfPurchaseDescription);
            report.SetParameterValue("TermOfPurchase", desc);

            desc = string.Empty;
            list = (ArrayList)oprFabricType.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + OPRFabricType.getName(keyId));
            report.SetParameterValue("OPRFabric", desc);

            report.SetParameterValue("OrderType", orderType == "" ? "ALL" : orderType);

            desc = string.Empty;
            list = (ArrayList)shipmentMethod.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getShipmentMethodByKey(keyId).ShipmentMethodDescription);
            report.SetParameterValue("ShipmentMethod", desc);

            desc = string.Empty;
            list = (ArrayList)customerIdList.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getCustomerByKey(keyId).CustomerCode);
            report.SetParameterValue("Customer", desc);

            desc = string.Empty;
            list = (ArrayList)workflowStatus.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + ContractWFS.getType(keyId).Name);
            report.SetParameterValue("ShipmentStatus", desc);



            string office = "";
            foreach (object id in officeIdList.Values)
            {
                office += GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).Description + ", ";
            }
            if (office != "")
            {
                office = office.Remove(office.LastIndexOf(", "));
            }
            report.SetParameterValue("Office", office);
            report.SetParameterValue("HandlingOffice", (handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).Description));

            string productTeam = "";
            foreach (object productTeamId in productTeamList.Values)
            {
                ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                if (!productTeam.Contains(pc.Code + ", "))
                    productTeam += pc.Code + ", ";
            }
            if (productTeam != "")
                productTeam = productTeam.Remove(productTeam.LastIndexOf(", "));
            report.SetParameterValue("ProductTeam", productTeam);

            /*
            if (shipmentMethodList.Values.Count == 5)
                report.SetParameterValue("ShipmentMethod", "ALL");
            else
            {
                string shipmentMethod = "";
                foreach (object obj in shipmentMethodList.Values)
                {
                    shipmentMethod += commonWorker.getShipmentMethodByKey(Convert.ToInt32(obj)).ShipmentMethodDescription + ", ";
                }
                shipmentMethod = shipmentMethod.Remove(shipmentMethod.LastIndexOf(", "));
                report.SetParameterValue("ShipmentMethod", shipmentMethod);
            }
            if (customerIdList.Values.Count == 9)
                report.SetParameterValue("Customer", "ALL");
            else
            {
                string cust = "";
                foreach (object obj in customerIdList.Values)
                {
                    cust += commonWorker.getCustomerByKey(Convert.ToInt32(obj)).CustomerCode + ", ";
                }
                cust = cust.Remove(cust.LastIndexOf(", "));
                report.SetParameterValue("Customer", cust);
            }
            */
            report.SetParameterValue("LCNo", lcNoFrom + (lcNoFrom == lcNoTo ? "" : " to " + lcNoTo));
            report.SetParameterValue("PaymentTerm", (paymentTermId == -1 ? "ALL" : (paymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode() ? "OPEN ACCOUNT" : (paymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode() ? "L/C AT SIGHT" : "N/A"))));

            /*
            string statusDesc = "";
            bool selectAll = false;
            //foreach (int id in workflowStatus.Values)
            
            for (int i = workflowStatus.Values.Count - 1; i >= 0; i--)
            {
                int id = ((ArrayList)workflowStatus.Values)[i].GetHashCode();
                if (id == -1)
                {
                    selectAll = true;
                    ((ArrayList)workflowStatus.Values).RemoveAt(i);
                }
                else
                {
                    string wfs = ContractWFS.getShortName(id);
                    if (wfs != null)
                        statusDesc += (statusDesc == "" ? "" : ", ") + wfs;
                }
            }
            if (selectAll)
                statusDesc = "ALL";

            report.SetParameterValue("ShipmentStatus", statusDesc);
            */
            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            return report;
        }


        public OutstandingBookingReport getOutstandingBookingReport(string officeName, TypeCollector officeIdList, TypeCollector productTeamList, int handlingOfficeId,
            int countryOfOriginId, TypeCollector shipmentMethodList, int packingMethodId, DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo,
            TypeCollector customerList, int vendorId, int termOfPurchaseId, int shipmentPortId, int isSampleOrder, int userId)
        {
            OutstandingBookingReport report = new OutstandingBookingReport();
            OutstandingBookingReportDs dataset;

            dataset = ReporterWorker.Instance.getOutstandingBookingReport(officeIdList, productTeamList, handlingOfficeId,
                    countryOfOriginId, shipmentMethodList, packingMethodId, customerAtWHDateFrom, customerAtWHDateTo,
                    customerList, vendorId, termOfPurchaseId, shipmentPortId, isSampleOrder);

            report.SetDataSource(dataset);

            //report.SetParameterValue("Office", officeId == -1 ? "All" : GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode);
            report.SetParameterValue("Office", officeName);
            report.SetParameterValue("HandlingOffice", (handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).Description));

            report.SetParameterValue("CustomerAtWHDateFrom", DateTimeUtility.getDateString(customerAtWHDateFrom));
            report.SetParameterValue("CustomerAtWHDateTo", DateTimeUtility.getDateString(customerAtWHDateTo));
            report.SetParameterValue("PackingMethod", packingMethodId == -1 ? "All" : commonWorker.getPackingMethodByKey(packingMethodId).PackingMethodDescription);
            report.SetParameterValue("CountryOfOrigin", countryOfOriginId == -1 ? "All" : GeneralWorker.Instance.getCountryOfOriginByKey(countryOfOriginId).Name);
            report.SetParameterValue("Vendor", vendorId == -1 ? "All" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
            report.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "All" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
            report.SetParameterValue("LoadingPort", shipmentPortId == -1 ? "All" : commonWorker.getShipmentPortByKey(shipmentPortId).ShipmentPortDescription);
            //report.SetParameterValue("ProductTeam", productTeamId == -1 ? "" : GeneralWorker.Instance.getProductCodeDefByKey(productTeamId).Code);

            if (productTeamList.Values.Count == 1)
            {
                foreach (object productTeamId in productTeamList.Values)
                {
                    ProductCodeDef pc =  GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32( productTeamId));
                    report.SetParameterValue("ProductTeam", pc.CodeDescription);
                }
            }
            else
                report.SetParameterValue("ProductTeam", "All");

            if (shipmentMethodList.Values.Count == 5)
                report.SetParameterValue("ShipmentMethod", "All");
            else
            {
                string shipmentMethod = "";
                foreach (object obj in shipmentMethodList.Values)
                {
                    shipmentMethod += commonWorker.getShipmentMethodByKey(Convert.ToInt32(obj)).ShipmentMethodDescription + ", ";
                }
                shipmentMethod = shipmentMethod.Remove(shipmentMethod.LastIndexOf(", "));
                report.SetParameterValue("ShipmentMethod", shipmentMethod);
            }
            if (customerList.Values.Count == 9)
                report.SetParameterValue("Customer", "All");
            else
            {
                string cust = "";
                foreach (object obj in customerList.Values)
                {
                    cust += commonWorker.getCustomerByKey(Convert.ToInt32(obj)).CustomerCode + ", ";
                }
                cust = cust.Remove(cust.LastIndexOf(", "));
                report.SetParameterValue("Customer", cust);
            }
            string orderType = "";
            if (isSampleOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += (isSampleOrder == 1 ? AllSampleOrderText : "Mainline") + " Order";
            }
            if (orderType == "")
                orderType = "All";
            report.SetParameterValue("OrderType", orderType);

            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return report;
        }


        public PartialShipmentReport getPartialShipmentReport(DateTime atWHDateFrom, DateTime atWHDateTo, TypeCollector officeIdList, TypeCollector productTeamList, int handlingOfficeId,
            int vendorId, int isNSLSZOrder, int isDualSourcingOrder, int isLDPOrder, int isSampleOrder, int isDFOrder, int isUTOrder, int termOfPurchaseId, int seasonId, 
            TypeCollector countryOfOriginIdList, TypeCollector shipmentPortIdList,  TypeCollector customerDestinationIdList, int oprFabricType,
            int packingMethodId, TypeCollector shipmentMethodList, TypeCollector customerIdList, DateTime bookInWHDateFrom, DateTime bookInWHDateTo, int paymentTermId, string sortField, int userId)
        {
            PartialShipmentReport report = new PartialShipmentReport();
            PartialShipmentReportDs dataset;

            dataset = ReporterWorker.Instance.getPartialShipmentReport(atWHDateFrom, atWHDateTo, officeIdList, productTeamList, handlingOfficeId,
                vendorId, isNSLSZOrder, isDualSourcingOrder, isLDPOrder, isSampleOrder, isDFOrder, isUTOrder, termOfPurchaseId, seasonId,
                countryOfOriginIdList, shipmentPortIdList, customerDestinationIdList, oprFabricType,
                packingMethodId, shipmentMethodList, customerIdList, bookInWHDateFrom, bookInWHDateTo, paymentTermId, sortField);
            
            report.SetDataSource(dataset);
            
            report.SetParameterValue("AtWHDateFrom", DateTimeUtility.getDateString(atWHDateFrom));
            report.SetParameterValue("AtWHDateTo", DateTimeUtility.getDateString(atWHDateTo));
            report.SetParameterValue("BookInWHDateFrom", DateTimeUtility.getDateString(bookInWHDateFrom));
            report.SetParameterValue("BookInWHDateTo", DateTimeUtility.getDateString(bookInWHDateTo));
            //report.SetParameterValue("CO", countryOfOriginId == -1 ? "ALL" : GeneralWorker.Instance.getCountryOfOriginByKey(countryOfOriginId).Name);
            //report.SetParameterValue("CO", coIdList.Values.Count == 1 ? "ALL" : coNameList);
            string name = string.Empty;
            CountryOfOriginRef co;
            foreach (object obj in countryOfOriginIdList.Values)
                if ((co = GeneralWorker.Instance.getCountryOfOriginByKey(Convert.ToInt32(obj))) != null)
                    name += (name == string.Empty ? string.Empty : ", ") + co.Name;
            report.SetParameterValue("CO", (countryOfOriginIdList.Values.Count == 0) ? "ALL" : name);

            name = string.Empty;
            ShipmentPortRef port;
            foreach (object obj in shipmentPortIdList.Values)
                if ((port = commonWorker.getShipmentPortByKey(Convert.ToInt32(obj))) != null)
                    name += (name == string.Empty ? string.Empty : ", ") + port.ShipmentPortDescription;
            report.SetParameterValue("LoadingPort", (shipmentPortIdList.Values.Count == 0) ? "ALL" : name);

            name = string.Empty;
            CustomerDestinationDef dest;
            foreach (object obj in customerDestinationIdList.Values)
                if ((dest = commonWorker.getCustomerDestinationByKey(Convert.ToInt32(obj))) != null)
                    name += (name == string.Empty ? string.Empty : ", ") + dest.DestinationDesc;
            report.SetParameterValue("Destination", (customerDestinationIdList.Values.Count == 0) ? "ALL" : name);


            report.SetParameterValue("Supplier", vendorId == -1 ? "ALL" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
            report.SetParameterValue("PurchaseTerm", termOfPurchaseId == -1 ? "ALL" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
            report.SetParameterValue("Season", seasonId == -1 ? "ALL" : GeneralWorker.Instance.getSeasonByKey(seasonId).Code);
            //report.SetParameterValue("LoadingPort", shipmentPortId == -1 ? "ALL" : commonWorker.getShipmentPortByKey(shipmentPortId).ShipmentPortDescription);
            //report.SetParameterValue("Destination", customerDestinationId == -1 ? "ALL" : commonWorker.getCustomerDestinationByKey(customerDestinationId).DestinationDesc);
            report.SetParameterValue("OPRType", oprFabricType == -1 ? "ALL" : OPRFabricType.getName(oprFabricType));
            report.SetParameterValue("PackingMethod", packingMethodId == -1 ? "ALL" : commonWorker.getPackingMethodByKey(packingMethodId).PackingMethodDescription);
            report.SetParameterValue("PaymentTerm", paymentTermId == -1 ? "ALL" : commonWorker.getPaymentTermByKey(paymentTermId).PaymentTermDescription);

            string office = "";
            foreach (object id in officeIdList.Values)
            {
                office += GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).Description.Replace(" Office","") + ", ";
            }
            if (office != "")
            {
                office = office.Remove(office.LastIndexOf(", "));
            }
            report.SetParameterValue("Office", office);
            report.SetParameterValue("HandlingOffice", (handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).Description));

            string productTeam = "";
            foreach (object productTeamId in productTeamList.Values)
            {
                ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                if (!productTeam.Contains(pc.Code + ", "))
                    productTeam += pc.Code + ", ";
            }
            if (productTeam != "")
                productTeam = productTeam.Remove(productTeam.LastIndexOf(", "));
            report.SetParameterValue("ProductTeam", productTeam);


            string orderType = "";
            if (isNSLSZOrder != -1)
                orderType += (isNSLSZOrder == 1 ? ", NSL (SZ) Order" : ", Non-NSL (SZ) Order");
            if (isDualSourcingOrder != -1)
                orderType += (isDualSourcingOrder == 1 ? ", Dual Sourcing Order" : ", Non-Dual Sourcing Order");
            if (isLDPOrder != -1)
                orderType += (isLDPOrder == 1 ? ", LDP Order " : ", Non-LDP Order");
            if (isSampleOrder != -1)
                orderType += ", " + (isSampleOrder == 1 ? AllSampleOrderText : "Mainline") + " Order";
            if (isDFOrder != -1)
                orderType += (isDFOrder == 1 ? ", DF Order " : ", Non-DF Order");
            if (isUTOrder != -1)
                orderType += (isUTOrder == 1 ? ", UT Order " : ", Non-UT Order");
            report.SetParameterValue("OrderType", orderType == "" ? "ALL" : orderType.Substring(1, orderType.Length - 1).Trim());

            if (shipmentMethodList.Values.Count == 5)
                report.SetParameterValue("ShipmentMethod", "ALL");
            else
            {
                string shipmentMethod = "";
                foreach (object obj in shipmentMethodList.Values)
                {
                    shipmentMethod += commonWorker.getShipmentMethodByKey(Convert.ToInt32(obj)).ShipmentMethodDescription + ", ";
                }
                shipmentMethod = shipmentMethod.Remove(shipmentMethod.LastIndexOf(", "));
                report.SetParameterValue("ShipmentMethod", shipmentMethod);
            }
            if (customerIdList.Values.Count == 9)
                report.SetParameterValue("Customer", "ALL");
            else
            {
                string cust = "";
                foreach (object obj in customerIdList.Values)
                {
                    cust += commonWorker.getCustomerByKey(Convert.ToInt32(obj)).CustomerCode + ", ";
                }
                cust = cust.Remove(cust.LastIndexOf(", "));
                report.SetParameterValue("Customer", cust);
            }

            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return report;
        }

        public EziBuyPartialShipmentReport getEziBuyPartialShipmentReport(DateTime atWHDateFrom, DateTime atWHDateTo, TypeCollector officeIdList, TypeCollector productTeamList,
            int vendorId, int isNSLSZOrder, int isDualSourcingOrder, int isLDPOrder, int isSampleOrder, int termOfPurchaseId, int countryOfOriginId, int seasonId, int shipmentPortId, int customerDestinationId, int phaseId,
            int packingMethodId, TypeCollector shipmentMethodList, DateTime bookInWHDateFrom, DateTime bookInWHDateTo, string sortField, int userId)
        {
            EziBuyPartialShipmentReport report = new EziBuyPartialShipmentReport();
            EziBuyPartialShipmentReportDs dataset;

            dataset = ReporterWorker.Instance.getEziBuyPartialShipmentReport(atWHDateFrom, atWHDateTo, officeIdList, productTeamList,
            vendorId, isNSLSZOrder, isDualSourcingOrder, isLDPOrder, isSampleOrder, termOfPurchaseId, countryOfOriginId, seasonId, shipmentPortId, customerDestinationId, phaseId,
            packingMethodId, shipmentMethodList, bookInWHDateFrom, bookInWHDateTo, sortField);

            report.SetDataSource(dataset);

            report.SetParameterValue("AtWHDateFrom", DateTimeUtility.getDateString(atWHDateFrom));
            report.SetParameterValue("AtWHDateTo", DateTimeUtility.getDateString(atWHDateTo));
            report.SetParameterValue("BookInWHDateFrom", DateTimeUtility.getDateString(bookInWHDateFrom));
            report.SetParameterValue("BookInWHDateTo", DateTimeUtility.getDateString(bookInWHDateTo));
            report.SetParameterValue("CO", countryOfOriginId == -1 ? "ALL" : GeneralWorker.Instance.getCountryOfOriginByKey(countryOfOriginId).Name);
            report.SetParameterValue("Supplier", vendorId == -1 ? "ALL" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
            report.SetParameterValue("PurchaseTerm", termOfPurchaseId == -1 ? "ALL" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
            report.SetParameterValue("Season", seasonId == -1 ? "ALL" : GeneralWorker.Instance.getSeasonByKey(seasonId).Code);
            report.SetParameterValue("LoadingPort", shipmentPortId == -1 ? "ALL" : commonWorker.getShipmentPortByKey(shipmentPortId).ShipmentPortDescription);
            report.SetParameterValue("Destination", customerDestinationId == -1 ? "ALL" : commonWorker.getCustomerDestinationByKey(customerDestinationId).DestinationDesc);
            report.SetParameterValue("Phase", phaseId == -1 ? "ALL" : phaseId.ToString());
            report.SetParameterValue("PackingMethod", packingMethodId == -1 ? "ALL" : commonWorker.getPackingMethodByKey(packingMethodId).PackingMethodDescription);

            string office = "";
            foreach (object id in officeIdList.Values)
            {
                office += GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).Description.Replace(" Office", "") + ", ";
            }
            if (office != "")
            {
                office = office.Remove(office.LastIndexOf(", "));
            }
            report.SetParameterValue("Office", office);
            string productTeam = "";
            foreach (object productTeamId in productTeamList.Values)
            {
                ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                if (!productTeam.Contains(pc.Code + ", "))
                    productTeam += pc.Code + ", ";
            }
            if (productTeam != "")
                productTeam = productTeam.Remove(productTeam.LastIndexOf(", "));
            report.SetParameterValue("ProductTeam", productTeam);


            string orderType = "";
            if (isNSLSZOrder != -1)
                orderType += (isNSLSZOrder == 1 ? ", NSL (SZ) Order" : ", Non-NSL (SZ) Order");
            if (isDualSourcingOrder != -1)
                orderType += (isDualSourcingOrder == 1 ? ", Dual Sourcing Order" : ", Non-Dual Sourcing Order");
            if (isLDPOrder != -1)
                orderType += (isLDPOrder == 1 ? ", LDP Order " : ", Non-LDP Order");
            if (isSampleOrder != -1)
                orderType += ", " + (isSampleOrder == 1 ? AllSampleOrderText : "Mainline") + " Order";
            report.SetParameterValue("OrderType", orderType == "" ? "ALL" : orderType.Substring(1, orderType.Length - 1).Trim());

            if (shipmentMethodList.Values.Count == 5)
                report.SetParameterValue("ShipmentMethod", "ALL");
            else
            {
                string shipmentMethod = "";
                foreach (object obj in shipmentMethodList.Values)
                {
                    shipmentMethod += commonWorker.getShipmentMethodByKey(Convert.ToInt32(obj)).ShipmentMethodDescription + ", ";
                }
                shipmentMethod = shipmentMethod.Remove(shipmentMethod.LastIndexOf(", "));
                report.SetParameterValue("ShipmentMethod", shipmentMethod);
            }
            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return report;
        }


        public OutstandingPaymentReport getOutstandingPaymentReport(DateTime shipReceiptDateFrom, DateTime shipReceiptDateTo, 
            DateTime STWDateFrom,DateTime STWDateTo, DateTime invoiceDateFrom, DateTime invoiceDateTo, int vendorId, string officeName, string shippingusersName, TypeCollector officeIdList,
            TypeCollector productTeamList, int termOfPurchaseId, int paymentTermId, ArrayList shippingusersId, int isSampleOrder, int isUTOrder, int isUploadDMS, int printUser)        
        {
            string orderType = "";

            OutstandingPaymentReport report = new OutstandingPaymentReport();
            OutstandingPaymentReportDs dataset;

            dataset = ReporterWorker.Instance.getOutstandingPaymentReport(shipReceiptDateFrom, shipReceiptDateTo, STWDateFrom, STWDateTo, invoiceDateFrom, invoiceDateTo,
                vendorId, officeIdList, productTeamList, termOfPurchaseId, paymentTermId, shippingusersId, isSampleOrder, isUTOrder, isUploadDMS);

            report.SetDataSource(dataset);

            report.SetParameterValue("ShipReceiptDateFrom", DateTimeUtility.getDateString(shipReceiptDateFrom));
            report.SetParameterValue("ShipReceiptDateTo", DateTimeUtility.getDateString(shipReceiptDateTo));
            report.SetParameterValue("STWDateFrom", DateTimeUtility.getDateString(STWDateFrom));
            report.SetParameterValue("STWDateTo", DateTimeUtility.getDateString(STWDateTo));
            report.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(invoiceDateFrom));
            report.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(invoiceDateTo));
            //report.SetParameterValue("Office", officeId == -1 ? "ALL" : GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode);            
            report.SetParameterValue("Office", officeName);
            report.SetParameterValue("Supplier", vendorId == -1 ? "ALL" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
            report.SetParameterValue("PurchaseTerm", termOfPurchaseId == -1 ? "ALL" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
            report.SetParameterValue("PaymentTerm", paymentTermId == -1 ? "ALL" : commonWorker.getPaymentTermByKey(paymentTermId).PaymentTermDescription);
            report.SetParameterValue("ShippingUser", shippingusersName);
            string isUploadDMSString = "";
            if(isUploadDMS == -1)
            {
                isUploadDMSString ="ALL";
            }
            else if (isUploadDMS == 1)
            {
                isUploadDMSString = "YES";
            }
            else if (isUploadDMS == 0)
            {
                isUploadDMSString = "NO";
            }
            report.SetParameterValue("IsUploadDMS", isUploadDMSString);
            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(printUser).DisplayName);
            if (isSampleOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += (isSampleOrder == 1 ? AllSampleOrderText : "Mainline") + " Order";
            }
            if (isUTOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isUTOrder == 1 ? "UT Order" : "Non-UT Order";
            }
            if (orderType == "")
                orderType = "ALL";
            report.SetParameterValue("OrderType", orderType);

            return report;
        }

        public NSLSZOrderReport getNSLSZOrderReport(DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo, DateTime invoiceDateFrom, DateTime invoiceDateTo,
            DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, TypeCollector officeIdList,
            TypeCollector productTeamList, int shipmentPortId, int customerDestinationId, int packingMethodId, TypeCollector shipmentMethodList, TypeCollector customerIdList,
            TypeCollector shipmentStatusList, int isSZUTOrder, int isDualSourcing, int isOPROrder, int isLDPOrder, int userId)
        {
            NSLSZOrderReport report = new NSLSZOrderReport();
            NSLSZOrderReportDs dataset;

            dataset = ReporterWorker.Instance.getNSLSZOrderReport(customerAtWHDateFrom, customerAtWHDateTo, invoiceDateFrom, invoiceDateTo,
                    invoiceUploadDateFrom, invoiceUploadDateTo, invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, officeIdList,
                    productTeamList, shipmentPortId, customerDestinationId, packingMethodId, shipmentMethodList, customerIdList,
                    shipmentStatusList, isSZUTOrder, isDualSourcing, isOPROrder, isLDPOrder);
            

            report.SetDataSource(dataset);

            report.SetParameterValue("CustomerAtWHDateFrom", DateTimeUtility.getDateString(customerAtWHDateFrom));
            report.SetParameterValue("CustomerAtWHDateTo", DateTimeUtility.getDateString(customerAtWHDateTo));
            report.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(invoiceDateFrom));
            report.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(invoiceDateTo));
            report.SetParameterValue("InvoiceUploadDateFrom", DateTimeUtility.getDateString(invoiceUploadDateFrom));
            report.SetParameterValue("InvoiceUploadDateTo", DateTimeUtility.getDateString(invoiceUploadDateTo));
            report.SetParameterValue("InvoiceNoFrom", invoicePrefix == "" ? "" : ShippingWorker.getInvoiceNo(invoicePrefix, invoiceSeqFrom, invoiceYear));
            report.SetParameterValue("InvoiceNoTo", invoicePrefix == "" ? "" : ShippingWorker.getInvoiceNo(invoicePrefix, invoiceSeqTo, invoiceYear));
            report.SetParameterValue("Office", officeId == -1 ? "" : GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode);
            report.SetParameterValue("LoadingPort", shipmentPortId == -1 ? "" : commonWorker.getShipmentPortByKey(shipmentPortId).ShipmentPortDescription);
            report.SetParameterValue("Destination", customerDestinationId == -1 ? "" : commonWorker.getCustomerDestinationByKey(customerDestinationId).DestinationDesc);
            report.SetParameterValue("PackingMethod", packingMethodId == -1 ? "" : commonWorker.getPackingMethodByKey(packingMethodId).PackingMethodDescription);

            string orderType = "";
            if (shipmentStatusList.Values.Count != 3)
            {
                foreach (object obj in shipmentStatusList.Values)
                {
                    orderType += ContractWFS.getShortName(Convert.ToInt32(obj)) + ", ";
                }
            }

            if (isSZUTOrder != -1)
            {
                if (isSZUTOrder == 1)
                    orderType += "SZ Order, ";
                else
                    orderType += "UT Order, ";
            }

            if (isDualSourcing != -1)
            {
                if (isDualSourcing == 1)
                    orderType += "Dual Sourcing Order, ";
                else
                    orderType += "Non-Dual Sourcing Order, ";
            }

            if (isOPROrder != -1)
            {
                if (isOPROrder == 1)
                    orderType += "OPR Order, ";
                else
                    orderType += "Non-OPR Order, ";
            }
            if (isLDPOrder != -1)
            {
                if (isLDPOrder == 1)
                    orderType += "LDP Order";
                else
                    orderType += "Non-LDP Order";
            }
            if (orderType.EndsWith(", "))
                orderType = orderType.Remove(orderType.LastIndexOf(", "));
            report.SetParameterValue("OrderType", orderType);
            if (productTeamList.Values.Count == 1)
            {
                foreach (object productTeamId in productTeamList.Values)
                {
                    ProductCodeDef pc =  GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32( productTeamId));
                    report.SetParameterValue("ProductTeam", pc.CodeDescription);
                }
            }
            else
                report.SetParameterValue("ProductTeam", "");

            if (shipmentMethodList.Values.Count == 5)
                report.SetParameterValue("ShipmentMethod", "");
            else
            {
                string shipmentMethod = "";
                foreach (object obj in shipmentMethodList.Values)
                {
                    shipmentMethod += commonWorker.getShipmentMethodByKey(Convert.ToInt32(obj)).ShipmentMethodDescription + ", ";
                }
                shipmentMethod = shipmentMethod.Remove(shipmentMethod.LastIndexOf(", "));
                report.SetParameterValue("ShipmentMethod", shipmentMethod);
            }
            if (customerIdList.Values.Count == 9)
                report.SetParameterValue("Customer", "");
            else
            {
                string cust = "";
                foreach (object obj in customerIdList.Values)
                {
                    cust += commonWorker.getCustomerByKey(Convert.ToInt32(obj)).CustomerCode + ", ";
                }
                cust = cust.Remove(cust.LastIndexOf(", "));
                report.SetParameterValue("Customer", cust);
            }

            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return report;
        }

        public STWDateDiscrepancyRpt getSTWDateDiscrepancyReport(int budgetYear, int periodFrom, int periodTo, DateTime invoiceDateFrom, DateTime invoiceDateTo, 
            DateTime ilsActualStwDateFrom, DateTime ilsActualStwDateTo, DateTime actualStwDateFrom, DateTime actualStwDateTo, int CountryOfOriginId, int ShipmentPortId, 
            TypeCollector officeIdList, string officeString, TypeCollector productTeamList, string productTeamString, 
            TypeCollector customerIdList, string customerString, TypeCollector tradingAgencyList, string tradingAgencyString,
            int baseCurrencyId, int userId, string sortingField, string exportType)
        {
            STWDateDiscrepancyRpt rpt = new STWDateDiscrepancyRpt();

            STWDateDiscrepancyDs ds = ReporterWorker.Instance.getSTWDateDiscrepancyByCriteria(budgetYear, periodFrom, periodTo, 
                invoiceDateFrom, invoiceDateTo, ilsActualStwDateFrom,ilsActualStwDateTo, actualStwDateFrom, actualStwDateTo, 
                CountryOfOriginId, ShipmentPortId, officeIdList, productTeamList, customerIdList, tradingAgencyList, baseCurrencyId, sortingField);

            rpt.SetDataSource(ds.Tables[0]);

            rpt.SetParameterValue("BudgetYear", budgetYear);
            rpt.SetParameterValue("PeriodFrom", periodFrom);
            rpt.SetParameterValue("PeriodTo", periodTo);
            rpt.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(invoiceDateFrom));
            rpt.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(invoiceDateTo));
            rpt.SetParameterValue("IlsActualStwDateFrom", DateTimeUtility.getDateString(ilsActualStwDateFrom));
            rpt.SetParameterValue("IlsActualStwDateTo", DateTimeUtility.getDateString(ilsActualStwDateTo));
            rpt.SetParameterValue("ActualStwDateFrom", DateTimeUtility.getDateString(actualStwDateFrom));
            rpt.SetParameterValue("ActualStwDateTo", DateTimeUtility.getDateString(actualStwDateTo));

            rpt.SetParameterValue("Office", officeString);
            rpt.SetParameterValue("ProductTeam", productTeamString);
            rpt.SetParameterValue("Customer", customerString);
            rpt.SetParameterValue("TradingAgency", tradingAgencyString);
            rpt.SetParameterValue("ShipmentPort", (ShipmentPortId == -1 ? "ALL" : commonWorker.getShipmentPortByKey(ShipmentPortId).ShipmentPortDescription));
            rpt.SetParameterValue("CountryOfOrigin", (CountryOfOriginId == -1 ? "ALL" : GeneralWorker.Instance.getCountryOfOriginByKey(CountryOfOriginId).Name));
            rpt.SetParameterValue("ExportType", exportType);

            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return rpt;
        }

        public EziBuyOSPaymentList getEziBuyOSPaymentReport()
        {   
            //report generation thru handler
            EziBuyOSPaymentList rpt = new EziBuyOSPaymentList();
            EziBuyOSPaymentDs ds = ReporterWorker.Instance.getEziBuyAllOSPaymentReport("");
            rpt.SetDataSource(ds.Tables[0]);
            rpt.SetParameterValue("ReportFormat", "EMAIL");
            rpt.SetParameterValue("DateFormat", "DMY");
            return rpt;
        }

        public EziBuyOSPaymentList getEziBuyOSPaymentReport(string Consignee)
        {   
            //report generation thru handler
            // Consignee: 'GT'  - Golbal Textile -> phase 2 or before
            //          : 'EB'  - Ezibuy         -> Phase 3 onward
            //          : EMPTY                  -> All season & Phase

            EziBuyOSPaymentList rpt = new EziBuyOSPaymentList();
            EziBuyOSPaymentDs ds = ReporterWorker.Instance.getEziBuyAllOSPaymentReport(Consignee.ToUpper());
            rpt.SetDataSource(ds.Tables[0]);
            rpt.SetParameterValue("ReportFormat", "EMAIL");
            rpt.SetParameterValue("DateFormat", "DMY");
            return rpt;
        }

        public EziBuyOSPaymentList getEziBuyOSPaymentList(int UserId, TypeCollector OfficeIdList, string DepartmentCode)
        {   
            // report generation thru button clicking 
            EziBuyOSPaymentList rpt = new EziBuyOSPaymentList();
            EziBuyOSPaymentDs ds = ReporterWorker.Instance.getEziBuyAllOSPaymentList(UserId,OfficeIdList,DepartmentCode);
            rpt.SetDataSource(ds.Tables[0]);
            rpt.SetParameterValue("ReportFormat", "FULL");
            rpt.SetParameterValue("DateFormat", "DMY");
            return rpt;
        }

        public ContainerManifestReport getContainerManifestReport(string voyageNo, DateTime departDate, string departPort, string vesselName, string contractNo, int userId, string outputFormat)
        {
            ContainerManifestReport rpt = new ContainerManifestReport();
            ContainerManifestReportDs ds = ReporterWorker.Instance.getContainerManifest(voyageNo, departDate, departPort, vesselName, contractNo);
            rpt.SetDataSource(ds.Tables[0]);
            rpt.SetParameterValue("VoyageNo", voyageNo);
            rpt.SetParameterValue("DepartureDate", DateTimeUtility.getDateString(departDate)); 
            rpt.SetParameterValue("DeparturePort", departPort);
            rpt.SetParameterValue("VesselName", vesselName.ToUpper());
            rpt.SetParameterValue("ContractNo", contractNo);
            rpt.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            rpt.SetParameterValue("OutputFormat", outputFormat);
            return rpt;
        }

        public EziBuyOSPaymentList getEziBuyOSPaymentReport_Test(string DateFormat)
        {   
            //report generation for testing
            EziBuyOSPaymentList rpt = new EziBuyOSPaymentList();

            EziBuyOSPaymentDs ds = ReporterWorker.Instance.getEziBuyAllOSPaymentReport("");
            rpt.SetDataSource(ds.Tables[0]);
            rpt.SetParameterValue("ReportFormat", "EMAIL");
            rpt.SetParameterValue("DateFormat", DateFormat);
            return rpt;
        }

        public OutstandingGBTestReport getOutstandingGBTestReport(TypeCollector officeIdList, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime customerAWHDateFrom,
            DateTime customerAWHDateTo, int vendorId, TypeCollector shipmentMethodIdList, TypeCollector paymentTermIdList, int paymentStatus, int userId)
        {
            OutstandingGBTestReport rpt = new OutstandingGBTestReport();
            OutstandingGBTestReportDs ds = ReporterWorker.Instance.getOutstandingGBTestReport(officeIdList, invoiceDateFrom, invoiceDateTo, customerAWHDateFrom, customerAWHDateTo,
                vendorId, shipmentMethodIdList, paymentTermIdList, paymentStatus);
            rpt.SetDataSource(ds.Tables[0]);

            string officeList = string.Empty ;
            foreach (int officeId in officeIdList.Values)
            {
                officeList += (officeList == string.Empty ? "" : ", ") + GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode;
            }

            rpt.SetParameterValue("Office", officeList);
            rpt.SetParameterValue("InvoiceDateFrom", invoiceDateFrom == DateTime.MinValue ? "" : DateTimeUtility.getDateString(invoiceDateFrom));
            rpt.SetParameterValue("InvoiceDateTo", invoiceDateTo == DateTime.MinValue ? "" : DateTimeUtility.getDateString(invoiceDateTo));
            rpt.SetParameterValue("CustomerAWHDateFrom", customerAWHDateFrom == DateTime.MinValue ? "" : DateTimeUtility.getDateString(customerAWHDateFrom));
            rpt.SetParameterValue("CustomerAWHDateTo", customerAWHDateTo == DateTime.MinValue ? "" : DateTimeUtility.getDateString(customerAWHDateTo));
            rpt.SetParameterValue("Vendor", vendorId == -1 ? "" : VendorWorker.Instance.getVendorByKey(vendorId).Name);

            string desc = string.Empty;
            ArrayList list = (ArrayList)shipmentMethodIdList.Values;
            int keyId;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getShipmentMethodByKey(keyId).ShipmentMethodDescription);
            rpt.SetParameterValue("ShipmentMethod", desc == "ALL" ? "" : desc);

            desc = string.Empty;
            list = (ArrayList)paymentTermIdList.Values;
            for (int i = 0; i < list.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getPaymentTermByKey(keyId).PaymentTermDescription);
            rpt.SetParameterValue("PaymentTerm", desc == "ALL" ? "" : desc);

            if (paymentStatus == 1)
                rpt.SetParameterValue("PaymentStatus", "Paid");
            else if (paymentStatus == 0)
                rpt.SetParameterValue("PaymentStatus", "Not Yet Paid");
            else
                rpt.SetParameterValue("PaymentStatus", "");

            rpt.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return rpt;

        }

        public TradingAFReport getTradingAFReport(TypeCollector officeList, DateTime customerAtWearHouseFrom, DateTime customerAtWearHouseTo, DateTime invoiceFrom, DateTime invoiceTo, int supplierId, TypeCollector countrylist, TypeCollector portlist, TypeCollector shipmentlist, int userId)
        {

            TradingAFReport report = new TradingAFReport();
            TradingAFReportDs dataset;

           dataset = ReporterWorker.Instance.getTradingAFReport(officeList, customerAtWearHouseFrom, customerAtWearHouseTo, invoiceFrom, invoiceTo, supplierId, countrylist, portlist, shipmentlist);
           
           report.SetDataSource(dataset);

           string DateRange = "";
           string From = "";
           string To = "";
           if (customerAtWearHouseFrom != DateTime.MinValue) 
           {
               From = DateTimeUtility.getDateString(customerAtWearHouseFrom);
           }
           if (customerAtWearHouseTo != DateTime.MinValue) 
           {
               To = DateTimeUtility.getDateString(customerAtWearHouseTo);
           }
           if (From == "" && To == "") 
           {
               DateRange = "ALL";
           }
           else if (From != "" && To!="") 
           {
               DateRange = From + " to " + To;
           }
           else if (From != "" && To == "") 
           {
               DateRange = From + " to " + From;
           }
           report.SetParameterValue("CustomerAtWHDateRange", DateRange);

           DateRange = "";
           From = "";
           To = "";
           if (invoiceFrom != DateTime.MinValue)
           {
               From = DateTimeUtility.getDateString(invoiceFrom);
           }
           if (invoiceTo != DateTime.MinValue)
           {
               To = DateTimeUtility.getDateString(invoiceTo);
           }
           if (From == "" && To == "")
           {
               DateRange = "ALL";
           }
           else if (From != "" && To != "")
           {
               DateRange = From + " to " + To;
           }
           else if (From != "" && To == "")
           {
               DateRange = From + " to " + From;
           }
           report.SetParameterValue("InvoiceDateRange", DateRange);

           int keyId;
           string desc;
           ArrayList list;

           desc = string.Empty;
           list = (ArrayList)countrylist.Values;
           for (int i = 0; i < list.Count && desc != "ALL"; i++)
               desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + GeneralWorker.Instance.getCountryOfOriginByKey(keyId).Name);
           report.SetParameterValue("CountryOfOrigin", desc);

           desc = string.Empty;
           list = (ArrayList)portlist.Values;
           for (int i = 0; i < list.Count && desc != "ALL"; i++)
               desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getShipmentPortByKey(keyId).ShipmentPortDescription);
           report.SetParameterValue("LoadingPort", desc);

           report.SetParameterValue("Supplier", supplierId == -1 ? "ALL" : VendorWorker.Instance.getVendorByKey(supplierId).Name);

           desc = string.Empty;
           list = (ArrayList)shipmentlist.Values;
           for (int i = 0; i < list.Count && desc != "ALL"; i++)
               desc = ((keyId = (int)list[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + commonWorker.getShipmentMethodByKey(keyId).ShipmentMethodDescription);
           report.SetParameterValue("ShipmentMethod", desc);

           string office = "";
           foreach (object id in officeList.Values)
           {
               office += GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).Description + ", ";
           }
           if (office != "")
           {
               office = office.Remove(office.LastIndexOf(", "));
           }
           report.SetParameterValue("Office", office);

           report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
           return report;
        }

        public void generateUTForecastReport()
        {
            UTForcastDs.UTForcastRow r = null;
            string UTFORECAST_FOLDER = WebConfig.getValue("appSettings", "UTFORECAST_FOLDER");

            try
            {
                DateTime now = DateTime.Today;
                
                //now = new DateTime(2017,07,15);

                DateTime nowPlus30 = now.AddDays(31);
                UTForcastReport rpt = new UTForcastReport();
                UTForcastDs ds = new UTForcastDs();

                ds = ReporterWorker.Instance.getUTForcastReport(now, nowPlus30);

                rpt.SetDataSource(ds);

                string from = now.Day + "_" + now.Month + "_" + now.Year;

                string to = nowPlus30.Day + "_" + nowPlus30.Month + "_" + nowPlus30.Year;

                string fileName = UTFORECAST_FOLDER + "UTForecast_" + now.ToString("yyyyMMdd") + ".xls";
                rpt.SetParameterValue("PrintDate", from);
                rpt.SetParameterValue("WarehouseFrom", from + " To " + to);
                rpt.SetParameterValue("HandlingOffice", "ALL");
                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, fileName);
                rpt.Close();

                NoticeHelper.sendUTForecastReport(fileName, from, to);

                ds.UTForcast.Clear();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public UTDiscrepancyReport getUTDiscrepancyReport(TypeCollector officeList, DateTime customerAtWearHouseFrom, DateTime customerAtWearHouseTo, DateTime invoiceFrom, DateTime invoiceTo, int supplierId, int userId)
        {

            UTDiscrepancyReport report = new UTDiscrepancyReport();
            UTDiscrepancyDs dataset;

            dataset = ReporterWorker.Instance.getUTDiscrepancyReport(officeList, customerAtWearHouseFrom, customerAtWearHouseTo, invoiceFrom, invoiceTo, supplierId);

            report.SetDataSource(dataset);

            string DateRange = "";
            string From = "";
            string To = "";
            if (customerAtWearHouseFrom != DateTime.MinValue)
            {
                From = DateTimeUtility.getDateString(customerAtWearHouseFrom);
            }
            if (customerAtWearHouseTo != DateTime.MinValue)
            {
                To = DateTimeUtility.getDateString(customerAtWearHouseTo);
            }
            if (From == "" && To == "")
            {
                DateRange = "ALL";
            }
            else if (From != "" && To != "")
            {
                DateRange = From + " to " + To;
            }
            else if (From != "" && To == "")
            {
                DateRange = From + " to " + From;
            }
            report.SetParameterValue("custatdate", DateRange);

            DateRange = "";
            From = "";
            To = "";
            if (invoiceFrom != DateTime.MinValue)
            {
                From = DateTimeUtility.getDateString(invoiceFrom);
            }
            if (invoiceTo != DateTime.MinValue)
            {
                To = DateTimeUtility.getDateString(invoiceTo);
            }
            if (From == "" && To == "")
            {
                DateRange = "ALL";
            }
            else if (From != "" && To != "")
            {
                DateRange = From + " to " + To;
            }
            else if (From != "" && To == "")
            {
                DateRange = From + " to " + From;
            }
            report.SetParameterValue("invoicedate", DateRange);

            report.SetParameterValue("supplier", supplierId == -1 ? "ALL" : VendorWorker.Instance.getVendorByKey(supplierId).Name);

            string office = "";
            foreach (object id in officeList.Values)
            {
                office += GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).Description + ", ";
            }
            if (office != "")
            {
                office = office.Remove(office.LastIndexOf(", "));
            }
            report.SetParameterValue("Office", office);

            report.SetParameterValue("printuser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            return report;
        }

    }
}