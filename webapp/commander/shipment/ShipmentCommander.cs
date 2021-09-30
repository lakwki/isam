using System;
using System.Web;
using System.Collections;
using com.next.infra.web;
using com.next.infra.util;
using com.next.common.domain.types;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.order;
using com.next.isam.appserver.ils;
using com.next.isam.domain.order;
using com.next.isam.domain.product;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.invoice;
using com.next.isam.reporter.accounts;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.types;
using com.next.isam.domain.shipping;
using com.next.common.domain;

namespace com.next.isam.webapp.commander.shipment
{
    public class ShipmentCommander : ICommand
    {
        public ShipmentCommander()
        {
        }

        public void execute(HttpContext context)
        {
            string contractNo;
            string invoiceNo;
            string itemNo;
            int deliveryNo;
            string invoicePrefix;
            int invoiceSeqFrom;
            int invoiceSeqTo;
            int invoiceYear;
            int vendorId;
            int customerId;
            string supplierInvoiceNoFrom;
            string supplierInvoiceNoTo;
            DateTime invoiceDateFrom;
            DateTime invoiceDateTo;
            int productTeamId;
            string orderType;
            DateTime invoiceUploadDateFrom;
            DateTime invoiceUploadDateTo;
            int oprTypeId;
            int customerDestinationId;
            int termOfPurchaseId;
            string docNo;
            int workflowStatusId;
            ArrayList officeList;


            ShipmentManager shipmentManager = ShipmentManager.Instance;
            OrderManager orderManager = OrderManager.Instance;
            OrderWorker orderWorker = OrderWorker.Instance;
            Action action = (Action)context.Items[WebParamNames.COMMAND_ACTION];
            int userId = WebHelper.getLogonUserId(context);

            if (action == Action.GetShipmentList)
            {
                contractNo = context.Items[Param.contractNo] == null ? "" : context.Items[Param.contractNo].ToString();
                invoiceNo = context.Items[Param.invoiceNo] == null ? "" : context.Items[Param.invoiceNo].ToString();
                itemNo = context.Items[Param.itemNo] == null ? "" : context.Items[Param.itemNo].ToString();
                officeList = context.Items[Param.officeList] == null ? null : (ArrayList)context.Items[Param.officeList];

                ArrayList shipmentList = shipmentManager.getShipmentList(contractNo, invoiceNo, itemNo, officeList);
                context.Items.Add(Param.shipmentList, shipmentList);
            }
            else if (action == Action.GetShipmentToDMSList)
            {
                DateTime docDateFrom = context.Items[Param.shippingDocReceiptDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.shippingDocReceiptDateFrom]);
                DateTime docDateTo = context.Items[Param.shippingDocReceiptDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.shippingDocReceiptDateTo]);
                int officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                int paymentTermId = context.Items[Param.paymentTermId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.paymentTermId]);
                vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.vendorId]);
                int checkStatus = context.Items[Param.shippingDocCheckStatus] == null ? 0 : Convert.ToInt32(context.Items[Param.shippingDocCheckStatus]);
                contractNo = context.Items[Param.contractNo] == null ? String.Empty : context.Items[Param.contractNo].ToString();
                itemNo = context.Items[Param.itemNo] == null ? String.Empty : context.Items[Param.itemNo].ToString();

                ArrayList list = ShipmentManager.Instance.getShipmentToDMSList(officeId, docDateFrom, docDateTo,
                paymentTermId, vendorId, checkStatus, contractNo, itemNo);
                context.Items.Add(Param.shipmentToDMSList, list);
            }
            else if (action == Action.GetShipmentListForAdvancedSearch)
            {
                contractNo = context.Items[Param.contractNo] == null ? "" : context.Items[Param.contractNo].ToString();
                deliveryNo = context.Items[Param.deliveryNo] == null ? -1 : Convert.ToInt32(context.Items[Param.deliveryNo]);
                itemNo = context.Items[Param.itemNo] == null ? "" : context.Items[Param.itemNo].ToString();
                invoicePrefix = context.Items[Param.invoicePrefix] == null ? "" : context.Items[Param.invoicePrefix].ToString();
                invoiceSeqFrom = context.Items[Param.invoiceSeqFrom] == null ? -1 : Convert.ToInt32(context.Items[Param.invoiceSeqFrom]);
                invoiceSeqTo = context.Items[Param.invoiceSeqTo] == null ? -1 : Convert.ToInt32(context.Items[Param.invoiceSeqTo]);
                invoiceYear = context.Items[Param.invoiceYear] == null ? -1 : Convert.ToInt32(context.Items[Param.invoiceYear]);
                vendorId = context.Items[Param.vendorId] == null ? -1 : Convert.ToInt32(context.Items[Param.vendorId]);
                customerId = context.Items[Param.customerId] == null ? -1 : Convert.ToInt32(context.Items[Param.customerId]);
                supplierInvoiceNoFrom = context.Items[Param.supplierInvoiceNoFrom] == null ? "" : context.Items[Param.supplierInvoiceNoFrom].ToString();
                supplierInvoiceNoTo = context.Items[Param.supplierInvoiceNoTo] == null ? "" : context.Items[Param.supplierInvoiceNoTo].ToString();
                TypeCollector officeIdList = context.Items[Param.officeList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.officeList];
                invoiceDateFrom = context.Items[Param.invoiceDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateFrom]);
                invoiceDateTo = context.Items[Param.invoiceDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateTo]);
                productTeamId = context.Items[Param.productTeamId] == null ? -1 : Convert.ToInt32(context.Items[Param.productTeamId]);
                orderType = context.Items[Param.orderType] == null ? "" : context.Items[Param.orderType].ToString();
                invoiceUploadDateFrom = context.Items[Param.invoiceUploadDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceUploadDateFrom]);
                invoiceUploadDateTo = context.Items[Param.invoiceUploadDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceUploadDateTo]);
                oprTypeId = context.Items[Param.oprTypeId] == null ? -1 : Convert.ToInt32(context.Items[Param.oprTypeId]);
                customerDestinationId = context.Items[Param.customerDestinationId] == null ? -1 : Convert.ToInt32(context.Items[Param.customerDestinationId]);
                termOfPurchaseId = context.Items[Param.termOfPurchaseId] == null ? -1 : Convert.ToInt32(context.Items[Param.termOfPurchaseId]);
                docNo = context.Items[Param.docNo] == null ? "" : context.Items[Param.docNo].ToString();
                //workflowStatusId = context.Items[Param.workflowStatusId] == null ? -1 :  Convert.ToInt32(context.Items[Param.workflowStatusId]);
                TypeCollector workflowStatusList = context.Items[Param.workflowStatusList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.workflowStatusList];
                TypeCollector shipmentMethodList = context.Items[Param.shipmentMethodList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.shipmentMethodList];
                int splitOnly = context.Items[Param.splitOnly] == null ? 0 : Convert.ToInt32(context.Items[Param.splitOnly]);
                int szOrderOnly = context.Items[Param.szOrderOnly] == null ? 0 : Convert.ToInt32(context.Items[Param.szOrderOnly]);
                int sampleOnly = context.Items[Param.sampleOnly] == null ? 0 : Convert.ToInt32(context.Items[Param.sampleOnly]);
                int ldpOrder = context.Items[Param.ldpOrder] == null ? 0 : Convert.ToInt32(context.Items[Param.ldpOrder]);
                int withQCCharge = context.Items[Param.withQCCharge] == null ? 0 : Convert.ToInt32(context.Items[Param.withQCCharge]);
                int isReprocessGoods = context.Items[Param.isReprocessGoods] == null ? 0 : Convert.ToInt32(context.Items[Param.isReprocessGoods]);
                DateTime atWHDateFrom = context.Items[Param.customerAgreedAtWHDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.customerAgreedAtWHDateFrom]);
                DateTime atWHDateTo = context.Items[Param.customerAgreedAtWHDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.customerAgreedAtWHDateTo]);
                DateTime ilsInWHDateFrom = context.Items[Param.ILSActualAtWHDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.ILSActualAtWHDateFrom]);
                DateTime ilsInWHDateTo = context.Items[Param.ILSActualAtWHDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.ILSActualAtWHDateTo]);
                DateTime invoiceSentDateFrom = context.Items[Param.invoiceSentDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceSentDateFrom]);
                DateTime invoiceSentDateTo = context.Items[Param.invoiceSentDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceSentDateTo]);
                int countryOfOriginId = context.Items[Param.countryOfOriginId] == null ? -1 : Convert.ToInt32(context.Items[Param.countryOfOriginId]);
                int invoiceUploadUserId = context.Items[Param.invoiceUploadUserId] == null ? -1 : Convert.ToInt32(context.Items[Param.invoiceUploadUserId]);
                int isGBTestRequired = context.Items[Param.isChinaGBTestRequired] == null ? 0 : Convert.ToInt32(context.Items[Param.isChinaGBTestRequired]);
                int isQccInspection = context.Items[Param.isQccInspection] == null ? 0 : Convert.ToInt32(context.Items[Param.isQccInspection]);
                int isTradingAirFreight = context.Items[Param.isTradingAirFreight] == null ? 0 : Convert.ToInt32(context.Items[Param.isTradingAirFreight]);

                ArrayList shipmentList = shipmentManager.getShipmentList (contractNo, deliveryNo, itemNo, invoicePrefix, invoiceSeqFrom, invoiceSeqTo,
                    invoiceYear, vendorId, customerId, supplierInvoiceNoFrom, supplierInvoiceNoTo, officeIdList, invoiceDateFrom, invoiceDateTo,
                    productTeamId, orderType, invoiceUploadDateFrom, invoiceUploadDateTo, atWHDateFrom, atWHDateTo, ilsInWHDateFrom, ilsInWHDateTo,
                    invoiceSentDateFrom, invoiceSentDateTo, oprTypeId, customerDestinationId, countryOfOriginId,termOfPurchaseId, docNo, invoiceUploadUserId, 
                    workflowStatusList, shipmentMethodList, splitOnly, szOrderOnly, sampleOnly, ldpOrder, withQCCharge, isReprocessGoods, isGBTestRequired, isQccInspection, isTradingAirFreight);

                context.Items.Add(Param.shipmentList, shipmentList);
            }
            else if (action == Action.UpdateShipment)
            {
                InvoiceDef invoiceDef = (InvoiceDef)context.Items[Param.invoiceDef];
                ArrayList documents = (ArrayList)context.Items[Param.documents];
                ArrayList paymentDeduction = (ArrayList)context.Items[Param.paymentDeduction];
                ArrayList shipmentDetails = (ArrayList)context.Items[Param.shipmentDetails];
                //int rejectReasonId = (int)context.Items[Param.RejectPaymentReasonId];
                //ArrayList splitShipments = (ArrayList)context.Items[Param.splitShipments];
                //ArrayList validatedSplitShipments = (ArrayList)context.Items[Param.validatedSplitShipments];
                ShipmentDef shipmentDef = (ShipmentDef)context.Items[Param.shipmentDef];
                ArrayList splitShipments = (ArrayList)context.Items[Param.splitShipments];

                if (invoiceDef != null)
                {
                    //shipmentManager.updateShipment(invoiceDef, shipmentDetails, documents, shipmentDef, splitShipments, userId);
                    shipmentManager.updateShipment(invoiceDef, shipmentDetails, documents, shipmentDef, splitShipments, paymentDeduction, userId);
                    if (invoiceDef.InvoiceDate != DateTime.MinValue && invoiceDef.InvoiceSentDate == DateTime.MinValue)
                        shipmentManager.SendInvoiceWithBeneficiaryCert(invoiceDef.ShipmentId, userId);
                }
            }
            else if (action == Action.UpdateSplitShipment)
            {
                SplitShipmentDef splitShipment = (SplitShipmentDef)context.Items[Param.splitShipment];
                ArrayList splitShipmentDetails = (ArrayList)context.Items[Param.splitShipmentDetails];

                shipmentManager.updateSplitShipment(splitShipment, splitShipmentDetails, userId);
            }
            else if (action == Action.PrintInvoice)
            {
                ArrayList shipmentIdList = (ArrayList)context.Items[Param.shipmentList];
                /*
                shipmentIdList = new ArrayList();
                shipmentIdList.Add(1041675);
                */

                bool isSuccess = shipmentManager.printInvoice(shipmentIdList, userId);
                if (isSuccess)
                    ReportHelper.export(InvoiceReportManager.Instance.getInvoiceReport(shipmentIdList, userId), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "invoice");
            }
            else if (action == Action.PrintDebitNote)
            {
                int shipmentId = (int)context.Items[Param.shipmentId];
                string noteType = (string)context.Items[Param.debitNoteType];
                /*
                ArrayList shipmentIdList = new ArrayList();
                
                shipmentIdList = new ArrayList();
                shipmentIdList.Add(1231815);
                shipmentIdList.Add(1231938);
                shipmentIdList.Add(1231939);
                shipmentIdList.Add(1234065);
                shipmentIdList.Add(1234066);
                shipmentIdList.Add(1238068);
                shipmentIdList.Add(1238070);
                shipmentIdList.Add(1238071);
                shipmentIdList.Add(1239165);
                shipmentIdList.Add(1240696);
                shipmentIdList.Add(1240697);
                shipmentIdList.Add(1240698);
                shipmentIdList.Add(1243147);
                shipmentIdList.Add(1243148);
                shipmentIdList.Add(1249025);
                shipmentIdList.Add(1254591);
                shipmentIdList.Add(1254592);
                shipmentIdList.Add(1255330);



                ReportHelper.export(AccountReportManager.Instance.getCommissionDebitNote(shipmentIdList), HttpContext.Current.Response,
                                   CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, noteType);
                */

                ReportHelper.export(AccountReportManager.Instance.getCommissionDebitNote(shipmentId), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, noteType);
            }
            else if (action == Action.GetSplitShipmentInvoice)
            {
                string poNo = context.Items[Param.poNo].ToString();

                ContractShipmentListJDef def = shipmentManager.getSplitShipmentByPONo(poNo.Substring(0, poNo.IndexOf("-") - 1), poNo.Substring(poNo.IndexOf("-") - 1, 1),
                    Convert.ToInt32(poNo.Substring(poNo.IndexOf("-") + 1, 1)));

                ArrayList list = new ArrayList();
                if (def != null)
                {
                    list.Add(def);
                }
                context.Items.Add(Param.shipmentList, list);
            }
            else if (action == Action.GetLCShipmentInvoice)
            {
                string lcBillRefNo = context.Items[Param.lcBillRefNo].ToString();

                ArrayList shipmentList = shipmentManager.getLcShipmentByLcBillRefNo(lcBillRefNo);
                context.Items.Add(Param.shipmentList, shipmentList);

            }
            else if (action == Action.GetManifestList)
            {
                string voyageNo = context.Items[Param.voyageNo] == null ? "" : context.Items[Param.voyageNo].ToString();
                string vesselName = context.Items[Param.vesselName] == null ? "" : context.Items[Param.vesselName].ToString();
                DateTime departureDate = context.Items[Param.departureDate] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.departureDate]);
                contractNo = context.Items[Param.contractNo] == null ? "" : context.Items[Param.contractNo].ToString();
                string departurePort = context.Items[Param.departurePort] == null ? "" : context.Items[Param.departurePort].ToString();

                ArrayList manifestList = ILSUploadManager.Instance.getManifestList(voyageNo, departureDate, contractNo, departurePort, vesselName);
                context.Items.Add(Param.manifestList, manifestList);
            }
            else if (action == Action.GetLast10Shipment)
            {
                itemNo = context.Items[Param.itemNo] == null ? string.Empty : context.Items[Param.itemNo].ToString();
                contractNo = context.Items[Param.contractNo] == null ? string.Empty : context.Items[Param.contractNo].ToString();

                ArrayList list = ShipmentManager.Instance.getLast10Shipment(itemNo, contractNo);
                context.Items.Add(Param.shipmentList, list);
            }
            else if (action == Action.GetShipmentProductByItemNo)
            {
                itemNo = context.Items[Param.itemNo] == null ? string.Empty : context.Items[Param.itemNo].ToString();
                contractNo = context.Items[Param.contractNo] == null ? string.Empty : context.Items[Param.contractNo].ToString();
                vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : int.Parse(context.Items[Param.vendorId].ToString());

                ArrayList list = ShipmentManager.Instance.GetShipmentProductByItemNo(itemNo, contractNo, vendorId);
                context.Items.Add(Param.shipmentList, list);
            }
            else if (action == Action.UpdateInvoiceForMultipleShipment)
            {
                InvoiceDef invoiceDef = (InvoiceDef)context.Items[Param.invoiceDef];
                ArrayList shipmentDetails = (ArrayList)context.Items[Param.shipmentDetails];
                ArrayList contracts = new ArrayList();
                contracts.Add((ContractDef)context.Items[Param.contractDef]);

                if (invoiceDef != null)
                    shipmentManager.updateShipment(invoiceDef, shipmentDetails, null, userId);
                if (contracts != null)
                    orderWorker.updateContractList(contracts);
            }
            else if (action == Action.SendEzibuyInvoice)
            {
                ArrayList shipmentIdList = (ArrayList)context.Items[Param.shipmentList];

                int count = shipmentManager.prepareToSendEzibuyInvoice(shipmentIdList, userId);
            }
            else if (action == Action.UpdateProduct)
            {
                ProductDef product = (ProductDef)context.Items[Param.product];
                int shipmentId = (int)context.Items[Param.shipmentId];
                if (product != null)
                    shipmentManager.updateProduct(shipmentId, product, userId);
            }
            else if (action == Action.GetShipmentListForMassUpdate)
            {
                contractNo = context.Items[Param.contractNo] == null ? "" : context.Items[Param.contractNo].ToString();
                deliveryNo = context.Items[Param.deliveryNo] == null ? -1 : Convert.ToInt32(context.Items[Param.deliveryNo]);
                itemNo = context.Items[Param.itemNo] == null ? "" : context.Items[Param.itemNo].ToString();
                invoicePrefix = context.Items[Param.invoicePrefix] == null ? "" : context.Items[Param.invoicePrefix].ToString();
                invoiceSeqFrom = context.Items[Param.invoiceSeqFrom] == null ? -1 : Convert.ToInt32(context.Items[Param.invoiceSeqFrom]);
                invoiceSeqTo = context.Items[Param.invoiceSeqTo] == null ? -1 : Convert.ToInt32(context.Items[Param.invoiceSeqTo]);
                invoiceYear = context.Items[Param.invoiceYear] == null ? -1 : Convert.ToInt32(context.Items[Param.invoiceYear]);
                vendorId = context.Items[Param.vendorId] == null ? -1 : Convert.ToInt32(context.Items[Param.vendorId]);

                //customerId = context.Items[Param.customerId] == null ? -1 : Convert.ToInt32(context.Items[Param.customerId]);
                TypeCollector customerIdList = context.Items[Param.customerList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.customerList];

                supplierInvoiceNoFrom = context.Items[Param.supplierInvoiceNoFrom] == null ? "" : context.Items[Param.supplierInvoiceNoFrom].ToString();
                supplierInvoiceNoTo = context.Items[Param.supplierInvoiceNoTo] == null ? "" : context.Items[Param.supplierInvoiceNoTo].ToString();
                TypeCollector officeIdList = context.Items[Param.officeList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.officeList];
                invoiceDateFrom = context.Items[Param.invoiceDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateFrom]);
                invoiceDateTo = context.Items[Param.invoiceDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateTo]);
                productTeamId = context.Items[Param.productTeamId] == null ? -1 : Convert.ToInt32(context.Items[Param.productTeamId]);
                orderType = context.Items[Param.orderType] == null ? "" : context.Items[Param.orderType].ToString();
                invoiceUploadDateFrom = context.Items[Param.invoiceUploadDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceUploadDateFrom]);
                invoiceUploadDateTo = context.Items[Param.invoiceUploadDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceUploadDateTo]);
                oprTypeId = context.Items[Param.oprTypeId] == null ? -1 : Convert.ToInt32(context.Items[Param.oprTypeId]);
                customerDestinationId = context.Items[Param.customerDestinationId] == null ? -1 : Convert.ToInt32(context.Items[Param.customerDestinationId]);
                termOfPurchaseId = context.Items[Param.termOfPurchaseId] == null ? -1 : Convert.ToInt32(context.Items[Param.termOfPurchaseId]);
                docNo = context.Items[Param.docNo] == null ? "" : context.Items[Param.docNo].ToString();
                //workflowStatusId = context.Items[Param.workflowStatusId] == null ? -1 :  Convert.ToInt32(context.Items[Param.workflowStatusId]);
                TypeCollector workflowStatusList = context.Items[Param.workflowStatusList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.workflowStatusList];
                TypeCollector shipmentMethodList = context.Items[Param.shipmentMethodList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.shipmentMethodList];
                int splitOnly = context.Items[Param.splitOnly] == null ? 0 : Convert.ToInt32(context.Items[Param.splitOnly]);
                int szOrderOnly = context.Items[Param.szOrderOnly] == null ? 0 : Convert.ToInt32(context.Items[Param.szOrderOnly]);
                int sampleOnly = context.Items[Param.sampleOnly] == null ? 0 : Convert.ToInt32(context.Items[Param.sampleOnly]);
                int ldpOrder = context.Items[Param.ldpOrder] == null ? 0 : Convert.ToInt32(context.Items[Param.ldpOrder]);
                int withQCCharge = context.Items[Param.withQCCharge] == null ? 0 : Convert.ToInt32(context.Items[Param.withQCCharge]);
                DateTime atWHDateFrom = context.Items[Param.customerAgreedAtWHDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.customerAgreedAtWHDateFrom]);
                DateTime atWHDateTo = context.Items[Param.customerAgreedAtWHDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.customerAgreedAtWHDateTo]);
                DateTime ilsInWHDateFrom = context.Items[Param.ILSActualAtWHDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.ILSActualAtWHDateFrom]);
                DateTime ilsInWHDateTo = context.Items[Param.ILSActualAtWHDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.ILSActualAtWHDateTo]);
                DateTime invoiceSentDateFrom = context.Items[Param.invoiceSentDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceSentDateFrom]);
                DateTime invoiceSentDateTo = context.Items[Param.invoiceSentDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceSentDateTo]);
                int countryOfOriginId = context.Items[Param.countryOfOriginId] == null ? -1 : Convert.ToInt32(context.Items[Param.countryOfOriginId]);
                int invoiceUploadUserId = context.Items[Param.invoiceUploadUserId] == null ? -1 : Convert.ToInt32(context.Items[Param.invoiceUploadUserId]);
                string lcNoFrom = context.Items[Param.lcNoFrom] == null ? string.Empty : context.Items[Param.lcNoFrom].ToString();
                string lcNoTo = context.Items[Param.lcNoTo] == null ? string.Empty : context.Items[Param.lcNoTo].ToString();
                DateTime ActualAtWHDateFrom = context.Items[Param.actualAtWHDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.actualAtWHDateFrom]);
                DateTime ActualAtWHDateTo = context.Items[Param.actualAtWHDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.actualAtWHDateTo]);
                int ShippingDocumentReceiptStatus = context.Items[Param.ShippingDocumentReceiptStatus] == null ? -1 : Convert.ToInt32(context.Items[Param.ShippingDocumentReceiptStatus]);
                int LcPaymentCheckStatus = context.Items[Param.LcPaymentCheckStatus] == null ? -1 : Convert.ToInt32(context.Items[Param.LcPaymentCheckStatus]);
                TypeCollector ShippingUserIdList = context.Items[Param.shippingUserList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.shippingUserList];
                string SortingOrder = context.Items[Param.sortingOrder] == null ? string.Empty : context.Items[Param.sortingOrder].ToString();

                ArrayList shipmentList = shipmentManager.getShipmentList(contractNo, deliveryNo, itemNo, invoicePrefix, invoiceSeqFrom, invoiceSeqTo,
                    invoiceYear, vendorId, customerIdList, supplierInvoiceNoFrom, supplierInvoiceNoTo, officeIdList, invoiceDateFrom, invoiceDateTo,
                    productTeamId, orderType, invoiceUploadDateFrom, invoiceUploadDateTo, atWHDateFrom, atWHDateTo, ilsInWHDateFrom, ilsInWHDateTo,
                    invoiceSentDateFrom, invoiceSentDateTo, oprTypeId, customerDestinationId, countryOfOriginId,
                    termOfPurchaseId, docNo, invoiceUploadUserId, workflowStatusList, shipmentMethodList, splitOnly, szOrderOnly, sampleOnly, ldpOrder, withQCCharge,
                    lcNoFrom, lcNoTo, ActualAtWHDateFrom, ActualAtWHDateTo, ShippingDocumentReceiptStatus, LcPaymentCheckStatus, ShippingUserIdList, SortingOrder, userId);

                context.Items.Add(Param.shipmentList, shipmentList);
            }

            else if (action == Action.ShipmentListMassUpdate)
            {

                ArrayList shipmentList = (ArrayList)context.Items[Param.shipmentList];
                DateTime lcPaymentCheckDate = context.Items[Param.lcPaymentCheckDate] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.lcPaymentCheckDate]);
                DateTime shippingDocumentReceiptDate = context.Items[Param.shippingDocumentReceiptDate] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.shippingDocumentReceiptDate]);
                //bool UpdateShippingDocStatus = (bool)context.Items[Param.UpdateShippingDocStatus];
                //shipmentManager.ShipmentListMassUpdate(shipmentList, lcPaymentCheckDate, shippingDocumentReceiptDate, UpdateShippingDocStatus, userId);
                string lcBillRefNo = (context.Items[Param.lcBillRefNo] == null ? string.Empty : (String)context.Items[Param.lcBillRefNo]);
                shipmentManager.ShipmentListMassUpdate(shipmentList, lcPaymentCheckDate, shippingDocumentReceiptDate, lcBillRefNo, userId);

            }
            else if (action == Action.UpdateShippingDocumentCheckStatus)
            {
                ArrayList list = (ArrayList)context.Items[Param.shipmentIdList];
                shipmentManager.updateShippingDocumentCheckStatus(list, userId);
            }
        }

        public enum Action
        {
            GetShipmentList,
            GetShipmentListForAdvancedSearch,
            UpdateShipment,
            UpdateSplitShipment,
            PrintInvoice,
            PrintDebitNote,
            GetSplitShipmentInvoice,
            GetLCShipmentInvoice,
            GetManifestList,
            GetLast10Shipment,
            UpdateInvoiceForMultipleShipment,
            SendEzibuyInvoice,
            UpdateProduct,
            GetShipmentListForMassUpdate,
            ShipmentListMassUpdate,
            GetShipmentToDMSList,
            UpdateShippingDocumentCheckStatus,
            GetShipmentProductByItemNo
        }

        public enum Param
        {
            contractNo,
            itemNo,
            invoiceNo,
            deliveryNo,
            invoicePrefix,
            invoiceSeqFrom,
            invoiceSeqTo,
            invoiceYear,
            vendorId,
            customerId,
            supplierInvoiceNoFrom,
            supplierInvoiceNoTo,
            officeId,
            invoiceDateFrom,
            invoiceDateTo,
            productTeamId,
            orderType,
            invoiceUploadDateFrom,
            invoiceUploadDateTo,
            invoiceSentDateFrom,
            invoiceSentDateTo,
            oprTypeId,
            customerDestinationId,
            termOfPurchaseId,
            docNo,
            workflowStatusId,
            shipmentMethodList,
            workflowStatusList,
            officeList,
            shipmentId,
            poNo,
            splitOnly,
            szOrderOnly,
            sampleOnly,
            ldpOrder,
            withQCCharge,
            isReprocessGoods,
            voyageNo,
            vesselName,
            departureDate,
            departurePort,
            customerAgreedAtWHDateFrom,
            customerAgreedAtWHDateTo,
            ILSActualAtWHDateFrom,
            ILSActualAtWHDateTo,
            countryOfOriginId,
            invoiceUploadUserId,

            shipmentDetails,
            invoiceDef,
            shipmentDef,
            splitShipment,
            splitShipments,
            //validatedSplitShipments,
            splitShipmentDetails,
            documents,
            shipmentList,
            shipmentIdList,
            manifestList,
            contractDef,
            product,

            lcNoFrom,
            lcNoTo,
            customerList,
            ShippingDocumentReceiptStatus,
            LcPaymentCheckStatus,
            actualAtWHDateFrom,
            actualAtWHDateTo,
            shippingDocumentReceiptDate,
            lcPaymentCheckDate,
            lcBillRefNo,
            shippingUserList,
            sortingOrder,

            UpdateShippingDocStatus,

            //ShippingDocWFSId,
            //RejectPaymentReasonId

            shippingDocReceiptDateFrom,
            shippingDocReceiptDateTo,
            shippingDocCheckStatus,
            shipmentToDMSList,
            paymentTermId,
            isChinaGBTestRequired,
            isQccInspection,
            isTradingAirFreight,
            debitNoteType,
            paymentDeduction
        }
    }
}
