using System;
using System.Diagnostics;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using com.next.infra.web;
using com.next.infra.util;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.account;
using com.next.isam.appserver.order;
using com.next.isam.appserver.claim;
using com.next.isam.domain.account;
using com.next.isam.domain.order;
using com.next.isam.domain.claim;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.types;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.accounts;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.reporter.dataserver;
using com.next.isam.dataserver.worker;
using com.next.common.datafactory.worker.industry;
using System.Xml;
using System.IO;
using com.next.common.datafactory.worker;
using System.Linq;
using System.Text;
using com.next.common.web.commander;
using com.next.common.domain.dms;
using com.next.isam.dataserver.model.account;

namespace com.next.isam.webapp.commander.account
{
    public class AccountCommander : ICommand
    {
        public AccountCommander()
        {
        }

        public void execute(HttpContext context)
        {
            string invoicePrefix;
            int invoiceSeqFrom;
            int invoiceSeqTo;
            int invoiceSeq;
            int invoiceYear;
            DateTime invoiceDateFrom;
            DateTime invoiceDateTo;
            DateTime invoiceUploadDateFrom;
            DateTime invoiceUploadDateTo;
            DateTime subDocDateFrom;
            DateTime subDocDateTo;
            DateTime debitNoteDateFrom;
            DateTime debitNoteDateTo;
            DateTime debitNoteReceivedDateFrom;
            DateTime debitNoteReceivedDateTo;
            DateTime interfaceDateFrom;
            DateTime interfaceDateTo;
            DateTime cutOffDate;
            string contractNo;
            string ukDebitNoteNo;
            string itemNo;

            int officeId;
            ArrayList officeList;
            int tradingAgencyId;
            int termOfPurchaseId;
            int paymentTermId;
            int vendorId;
            int claimTypeId;
            int companyId;

            string paymentNo;

            Action action = (Action)context.Items[WebParamNames.COMMAND_ACTION];
            int userId = WebHelper.getLogonUserId(context);

            if (action == Action.GetInvoiceList)
            {
                invoicePrefix = context.Items[Param.invoicePrefix] == null ? "" : context.Items[Param.invoicePrefix].ToString();
                invoiceSeqFrom = context.Items[Param.invoiceSeqFrom] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceSeqFrom]);
                invoiceSeqTo = context.Items[Param.invoiceSeqTo] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceSeqTo]);
                invoiceYear = context.Items[Param.invoiceYear] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceYear]);
                invoiceDateFrom = context.Items[Param.invoiceDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateFrom]);
                invoiceDateTo = context.Items[Param.invoiceDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateTo]);
                invoiceUploadDateFrom = context.Items[Param.invoiceUploadDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceUploadDateFrom]);
                invoiceUploadDateTo = context.Items[Param.invoiceUploadDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceUploadDateTo]);
                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                officeList = context.Items[Param.officeList] == null ? null : (ArrayList)context.Items[Param.officeList];
                tradingAgencyId = context.Items[Param.tradingAgencyId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.tradingAgencyId]);
                termOfPurchaseId = context.Items[Param.termOfPurchaseId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.termOfPurchaseId]);
                DateTime purchaseScanDateFrom = context.Items[Param.purchaseScanDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.purchaseScanDateFrom]);
                DateTime purchaseScanDateTo = context.Items[Param.purchaseScanDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.purchaseScanDateTo]);
                int purchaseScanStatus = context.Items[Param.purchaseScanStatus] == null ? -1 : Convert.ToInt32(context.Items[Param.purchaseScanStatus]);
                string orderType = context.Items[Param.orderType] == null ? GeneralCriteria.ALLSTRING : context.Items[Param.orderType].ToString();
                int workflowStatusId = context.Items[Param.workflowStatusId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.workflowStatusId]);
                int currencyId = context.Items[Param.currencyId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.currencyId]);
                DateTime salesScanDateFrom = context.Items[Param.salesScanDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.salesScanDateFrom]);
                DateTime salesScanDateTo = context.Items[Param.salesScanDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.salesScanDateTo]);
                DateTime submittedOnFrom = context.Items[Param.submittedOnFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.submittedOnFrom]);
                DateTime submittedOnTo = context.Items[Param.submittedOnTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.submittedOnTo]);
                string batchNo = context.Items[Param.batchNo] == null ? GeneralCriteria.ALLSTRING : context.Items[Param.batchNo].ToString();
                int invoiceBatchStatus = context.Items[Param.invoiceBatchStatus] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.invoiceBatchStatus]);
                string lcBillRefNo = context.Items[Param.lcBillRefNo] == null ? GeneralCriteria.ALLSTRING : context.Items[Param.lcBillRefNo].ToString();
                ArrayList invoiceList = ShipmentManager.Instance.getInvoiceList(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, officeList, invoiceDateFrom,
                    invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo, termOfPurchaseId, workflowStatusId, tradingAgencyId, purchaseScanDateFrom, purchaseScanDateTo,
                    purchaseScanStatus, orderType, currencyId, salesScanDateFrom, salesScanDateTo, submittedOnFrom, submittedOnTo,
                    batchNo, invoiceBatchStatus);
                context.Items.Add(Param.invoiceList, invoiceList);
            }
            if (action == Action.GetInvoiceListForInvoiceBatch)
            {
                invoicePrefix = context.Items[Param.invoicePrefix] == null ? "" : context.Items[Param.invoicePrefix].ToString();
                invoiceSeqFrom = context.Items[Param.invoiceSeqFrom] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceSeqFrom]);
                invoiceSeqTo = context.Items[Param.invoiceSeqTo] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceSeqTo]);
                invoiceYear = context.Items[Param.invoiceYear] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceYear]);
                invoiceDateFrom = context.Items[Param.invoiceDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateFrom]);
                invoiceDateTo = context.Items[Param.invoiceDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateTo]);
                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                officeList = context.Items[Param.officeList] == null ? null : (ArrayList)context.Items[Param.officeList];
                tradingAgencyId = context.Items[Param.tradingAgencyId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.tradingAgencyId]);
                string orderType = context.Items[Param.orderType] == null ? GeneralCriteria.ALLSTRING : context.Items[Param.orderType].ToString();
                int workflowStatusId = context.Items[Param.workflowStatusId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.workflowStatusId]);
                int currencyId = context.Items[Param.currencyId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.currencyId]);
                DateTime salesScanDateFrom = context.Items[Param.salesScanDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.salesScanDateFrom]);
                DateTime salesScanDateTo = context.Items[Param.salesScanDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.salesScanDateTo]);
                DateTime submittedOnFrom = context.Items[Param.submittedOnFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.submittedOnFrom]);
                DateTime submittedOnTo = context.Items[Param.submittedOnTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.submittedOnTo]);
                string batchNo = context.Items[Param.batchNo] == null ? GeneralCriteria.ALLSTRING : context.Items[Param.batchNo].ToString();
                int invoiceBatchStatus = context.Items[Param.invoiceBatchStatus] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.invoiceBatchStatus]);

                ArrayList invoiceList = ShipmentManager.Instance.getInvoiceListForInvoiceBatch(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId,
                    invoiceDateFrom, invoiceDateTo, workflowStatusId, tradingAgencyId, orderType, currencyId, salesScanDateFrom, salesScanDateTo, submittedOnFrom, submittedOnTo,
                    batchNo, invoiceBatchStatus);
                context.Items.Add(Param.invoiceList, invoiceList);
            }
            else if (action == Action.GetInvoiceListByLcBillRefNo)
            {
                int workflowStatusId = context.Items[Param.workflowStatusId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.workflowStatusId]);
                string lcBillRefNo = context.Items[Param.lcBillRefNo] == null ? GeneralCriteria.ALLSTRING : context.Items[Param.lcBillRefNo].ToString();
                ArrayList invoiceList = ShipmentManager.Instance.getInvoiceListByLcBillRefNo(lcBillRefNo, workflowStatusId);
                context.Items.Add(Param.invoiceList, invoiceList);
            }
            else if (action == Action.GetInvoiceListByInvoiceNo)
            {
                invoicePrefix = context.Items[Param.invoicePrefix] == null ? "" : context.Items[Param.invoicePrefix].ToString();
                invoiceSeq = context.Items[Param.invoiceSeq] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceSeq]);
                invoiceYear = context.Items[Param.invoiceYear] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceYear]);
                officeList = context.Items[Param.officeList] == null ? null : (ArrayList)context.Items[Param.officeList];
                int sequenceNo = -1;
                if (context.Items[Param.sequenceNo] != null)
                {
                    sequenceNo = Convert.ToInt32(context.Items[Param.sequenceNo]);
                }

                ArrayList invoiceList = ShipmentManager.Instance.getInvoiceListByInvoiceNo(ShipmentManager.getInvoiceNo(invoicePrefix, invoiceSeq, invoiceYear), sequenceNo, officeList);
                context.Items.Add(Param.invoiceList, invoiceList);
            }
            else if (action == Action.GenerateSunInterface)
            {
                SunInterfaceQueueDef def = (SunInterfaceQueueDef)context.Items[Param.sunInterfaceQueue];
                AccountManager.Instance.submitSunInterfaceRequest(def);
            }
            else if (action == Action.GenerateNTSunInterface)
            {
                NTSunInterfaceQueueDef def = (NTSunInterfaceQueueDef)context.Items[Param.ntSunInterfaceQueue];
                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeExpenseInvoice.GetHashCode())
                {
                    /*
                    ArrayList ccList = new ArrayList();

                    ArrayList companyList = NonTradeManager.Instance.getNTUserCompanyList(def.User.UserId, def.Office.OfficeId, NTRoleType.SECOND_APPROVER.Id);
                    if (companyList.Count > 0)
                    {
                        foreach (CompanyRef companyRef in companyList)
                        {
                            ArrayList userList = NonTradeManager.Instance.getNTUserList(NTRoleType.FIRST_APPROVER.Id, companyRef.CompanyId, def.Office.OfficeId);
                            foreach (UserRef u in userList)
                            {
                                if (!ccList.Contains(u.UserId))
                                {
                                    def.QueueId = -1;
                                    def.User = u;
                                    NonTradeManager.Instance.submitNTSunInterfaceRequest(def);

                                    ccList.Add(u.UserId);
                                }
                            }
                        }
                    }
                    else
                        NonTradeManager.Instance.submitNTSunInterfaceRequest(def);
                    */
                    NonTradeManager.Instance.submitNTSunInterfaceRequest(def);
                }
                else
                    NonTradeManager.Instance.submitNTSunInterfaceRequest(def);

            }
            else if (action == Action.GenerateSunInterfaceBatch)
            {
                officeId = (int)context.Items[Param.officeId];
                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int period = (int)context.Items[Param.period];
                ArrayList typeList = (ArrayList)context.Items[Param.otherCostSunInterfaceTypeList];
                AccountManager.Instance.submitSunInterfaceBatch(officeId, fiscalYear, period, typeList, userId);
            }
            else if (action == Action.RegisterInvoiceForPayment)
            {
                ArrayList list = (ArrayList)context.Items[Param.invoiceList];
                AccountManager.Instance.registerAP(list, userId);
            }
            else if (action == Action.UpdateARAP)
            {
                ArrayList list = (ArrayList)context.Items[Param.invoiceList];
                ArrayList updateFailedList = AccountManager.Instance.updateARAP(list, userId);

                context.Items.Add(Param.result, updateFailedList);
            }
            else if (action == Action.UpdateARAPDN)
            {
                ArrayList list = (ArrayList)context.Items[Param.invoiceDebitNoteList];
                ArrayList updateFailedList = AccountManager.Instance.updateARAPDN(list, userId);

                context.Items.Add(Param.result, updateFailedList);
            }
            else if (action == Action.updateNTInvoiceSettlement)
            {
                ArrayList list = (ArrayList)context.Items[Param.ntInvoiceList];
                ArrayList updateFailedList = NonTradeManager.Instance.updateNTInvoiceSettlement(list, userId);

                context.Items.Add(Param.result, updateFailedList);
            }
            else if (action == Action.CreateEInvoiceBatch)
            {
                ArrayList list = (ArrayList)context.Items[Param.invoiceList];
                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                AccountManager.Instance.createEInvoiceBatch(list, officeId, userId);
            }
            else if (action == Action.DeleteFromEInvoiceBatch)
            {
                ArrayList list = context.Items[Param.invoiceList] == null ? null : (ArrayList)context.Items[Param.invoiceList];
                AccountManager.Instance.removeFromEInvoiceBatch(list, userId);
            }
            else if (action == Action.RegisterAR)
            {
                ArrayList list = context.Items[Param.invoiceList] == null ? null : (ArrayList)context.Items[Param.invoiceList];
                AccountManager.Instance.registerAR(list, userId);
            }
            else if (action == Action.UpdateBankReconciliationRequest)
            {
                BankReconciliationRequestDef request = (BankReconciliationRequestDef)context.Items[Param.bankReconRequest];
                AccountManager.Instance.updateBankReconciliationRequest(request);
            }
            else if (action == Action.GetBankReconciliationRequestList)
            {
                ArrayList list = AccountManager.Instance.getBankReconciliationRequestList();
                context.Items.Add(Param.bankReconList, list);
            }
            else if (action == Action.ConvertPaymentFile)
            {
                ConvertPaymentFileRequestDef request = (ConvertPaymentFileRequestDef)context.Items[Param.convertPaymentFileRequest];
                EBankingManager.Instance.genPaymentImportFile(request, userId);
            }
            else if (action == Action.GeneratePaymentAdvice)
            {
                GenerateFileRequestDef request = (GenerateFileRequestDef)context.Items[Param.genPaymentAdviceRequest];
                PaymentAdviceManager.Instance.updateGenerateFileRequest(request);
            }
            else if (action == Action.GetFileGenerateRequestList)
            {
                ArrayList list = PaymentAdviceManager.Instance.getGenerateFileRequestList();
                context.Items.Add(Param.requestList, list);
            }
            else if (action == Action.PrintQADebitNote)
            {
                ArrayList list = (ArrayList)context.Items[Param.invoiceList];
                ReportHelper.export(AccountReportManager.Instance.getQADebitNote(list), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "QADebitNote");
            }
            else if (action == Action.GenerateUTDebitNote)
            {
                officeId = (int)context.Items[Param.officeId];
                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int period = (int)context.Items[Param.period];
                DateTime debitNoteDate = Convert.ToDateTime(context.Items[Param.debitNoteDate]);
                bool isDraft = (bool)context.Items[Param.isDraft];
                string format = context.Items[Param.format].ToString();

                ArrayList debitNoteList = null;
                ArrayList debitNoteShipmentList = null;

                ArrayList qaDCNoteList = null;
                ArrayList qaDCNoteShipmentList = null;

                AccountManager.Instance.generateUTDebitNote(officeId, fiscalYear, period, debitNoteDate, isDraft, userId,
                    out debitNoteList, out debitNoteShipmentList, out qaDCNoteList, out qaDCNoteShipmentList);

                UTDebitNote rpt = null;

                if (format == "pdf")
                {
                    string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + ".pdf";
                    rpt = AccountReportManager.Instance.getUTDebitNote(debitNoteList, debitNoteShipmentList, isDraft, fiscalYear, period);
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                    rpt.Close();
                    rpt.Dispose();

                    if (debitNoteList.Count > 0)
                    {
                        NoticeHelper.sendUTDebitNote(officeId, fiscalYear, period, userId, isDraft, outputFilePath);
                        exportUTQACommissionNote(qaDCNoteList, qaDCNoteShipmentList, officeId, fiscalYear, period, userId, isDraft, "pdf");
                    }
                }
                else
                {
                    string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + ".xls";
                    rpt = AccountReportManager.Instance.getUTDebitNote(debitNoteList, debitNoteShipmentList, isDraft, fiscalYear, period);
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelRecord, outputFilePath);
                    rpt.Close();
                    rpt.Dispose();

                    if (debitNoteList.Count > 0)
                    {
                        NoticeHelper.sendUTDebitNote(officeId, fiscalYear, period, userId, isDraft, outputFilePath);
                        exportUTQACommissionNote(qaDCNoteList, qaDCNoteShipmentList, officeId, fiscalYear, period, userId, isDraft, "xls");
                    }
                }
            }
            else if (action == Action.GenerateMockShopDebitNote)
            {
                officeId = (int)context.Items[Param.officeId];
                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int period = (int)context.Items[Param.period];
                DateTime debitNoteDate = Convert.ToDateTime(context.Items[Param.debitNoteDate]);
                bool isDraft = (bool)context.Items[Param.isDraft];
                string format = context.Items[Param.format].ToString();

                ArrayList debitNoteList = null;
                ArrayList debitNoteShipmentList = null;
                ArrayList debitNoteShipmentDetailList = null;

                AccountManager.Instance.generateMockShopDebitNote(officeId, fiscalYear, period, debitNoteDate, isDraft, userId,
                    out debitNoteList, out debitNoteShipmentList, out debitNoteShipmentDetailList);

                MockShopDebitNote rpt = null;

                if (format == "pdf")
                {
                    /*
                    ReportHelper.export(AccountReportManager.Instance.getMockShopDebitNote(debitNoteList, debitNoteShipmentList, debitNoteShipmentDetailList,
                        isDraft, fiscalYear, period), HttpContext.Current.Response,
                        CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "MockShopDebitNote");
                    */

                    string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + ".pdf";
                    rpt = AccountReportManager.Instance.getMockShopDebitNote(debitNoteList, debitNoteShipmentList, debitNoteShipmentDetailList,
                        isDraft, fiscalYear, period);
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                    rpt.Close();
                    rpt.Dispose();

                    NoticeHelper.sendMockShopReport(officeId, fiscalYear, period, userId, outputFilePath);
                }
                else
                {
                    /*
                    ReportHelper.export(AccountReportManager.Instance.getMockShopDebitNote(debitNoteList, debitNoteShipmentList, debitNoteShipmentDetailList,
                    isDraft, fiscalYear, period), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "MockShopDebitNote");
                    */

                    string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + ".xls";
                    rpt = AccountReportManager.Instance.getMockShopDebitNote(debitNoteList, debitNoteShipmentList, debitNoteShipmentDetailList,
                        isDraft, fiscalYear, period);
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelRecord, outputFilePath);
                    rpt.Close();
                    rpt.Dispose();

                    NoticeHelper.sendMockShopReport(officeId, fiscalYear, period, userId, outputFilePath);
                }

            }
            else if (action == Action.GenerateStudioDebitNote)
            {
                officeId = (int)context.Items[Param.officeId];
                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int period = (int)context.Items[Param.period];
                DateTime debitNoteDate = Convert.ToDateTime(context.Items[Param.debitNoteDate]);
                bool isDraft = (bool)context.Items[Param.isDraft];
                string format = context.Items[Param.format].ToString();

                ArrayList debitNoteList = null;
                ArrayList debitNoteShipmentList = null;
                ArrayList debitNoteShipmentDetailList = null;

                AccountManager.Instance.generateStudioDebitNote(officeId, fiscalYear, period, debitNoteDate, isDraft, userId,
                    out debitNoteList, out debitNoteShipmentList, out debitNoteShipmentDetailList);

                MockShopDebitNote rpt = null;

                if (format == "pdf")
                {
                    string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + ".pdf";
                    rpt = AccountReportManager.Instance.getStudioDebitNote(debitNoteList, debitNoteShipmentList, debitNoteShipmentDetailList,
                        isDraft, fiscalYear, period);
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                    rpt.Close();
                    rpt.Dispose();

                    NoticeHelper.sendStudioReport(officeId, fiscalYear, period, userId, outputFilePath);
                }
                else
                {
                    string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + ".xls";
                    rpt = AccountReportManager.Instance.getStudioDebitNote(debitNoteList, debitNoteShipmentList, debitNoteShipmentDetailList,
                        isDraft, fiscalYear, period);
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelRecord, outputFilePath);
                    rpt.Close();
                    rpt.Dispose();

                    NoticeHelper.sendStudioReport(officeId, fiscalYear, period, userId, outputFilePath);
                }

            }
            else if (action == Action.UpdateDMSWorkflowStatusId)
            {
                ArrayList list = (ArrayList)context.Items[Param.shipmentIdList];
                int dmsWorkflowStatusId = (int)context.Items[Param.dmsWorkflowStatusId];
                int rejectPaymentReasonId = (int)context.Items[Param.rejectPaymentReasonId];
                AccountManager.Instance.updateDMSWorkflowStatus(list, dmsWorkflowStatusId, rejectPaymentReasonId, userId);
            }
            else if (action == Action.HoldPayment)
            {
                /*
                ArrayList list = (ArrayList)context.Items[Param.shipmentIdList];
                AccountManager.Instance.holdPayment(list, isPaymentHold, userId);
                */
                vendorId = (int)context.Items[Param.vendorId];
                bool isPaymentHold = (bool)context.Items[Param.isPaymentHold];
                string remark = (string)context.Items[Param.remark];
                AccountManager.Instance.holdPayment(vendorId, isPaymentHold, remark, userId);
            }
            else if (action == Action.MarkDMSComplete)
            {
                officeId = (int)context.Items[Param.officeId];
                AccountManager.Instance.markDMSComplete(officeId, userId);
            }

            else if (action == Action.MarkDocumentAsReviewed)
            {
                AccountManager.Instance.markDMSDocumentAsReviewed((int)context.Items[Param.shipmentId], userId);
            }
            /*
            else if (action == Action.UpdateClaim)
            {
                ClaimDef claim = (ClaimDef)context.Items[Param.claim];
                AccountManager.Instance.updateClaim(claim, userId);
            }
            else if (action == Action.UpdateClaimDetail)
            {
                ArrayList list = (ArrayList)context.Items[Param.claimDetails];
                AccountManager.Instance.updateClaimDetail(list, userId);
            }
            else if (action == Action.UploadClaimDocument)
            {
                FileUploadDef file = (FileUploadDef)context.Items[Param.file];
                CommonManager.Instance.updateFileUpload(file, userId);
            }
            */
            else if (action == Action.GetPaymentStatusEnquiryList)
            {
                invoicePrefix = context.Items[Param.invoicePrefix] == null ? "" : context.Items[Param.invoicePrefix].ToString();
                invoiceSeqFrom = context.Items[Param.invoiceSeqFrom] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceSeqFrom]);
                invoiceSeqTo = context.Items[Param.invoiceSeqTo] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceSeqTo]);
                invoiceYear = context.Items[Param.invoiceYear] == null ? 0 : Convert.ToInt32(context.Items[Param.invoiceYear]);
                invoiceDateFrom = context.Items[Param.invoiceDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateFrom]);
                invoiceDateTo = context.Items[Param.invoiceDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.invoiceDateTo]);
                subDocDateFrom = context.Items[Param.subDocDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.subDocDateFrom]);
                subDocDateTo = context.Items[Param.subDocDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.subDocDateTo]);
                interfaceDateFrom = context.Items[Param.interfaceDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.interfaceDateFrom]);
                interfaceDateTo = context.Items[Param.interfaceDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.interfaceDateTo]);
                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                int handlingOfficeId = context.Items[Param.handlingOfficeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.handlingOfficeId]);
                tradingAgencyId = context.Items[Param.tradingAgencyId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.tradingAgencyId]);
                paymentTermId = context.Items[Param.paymentTermId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.paymentTermId]);
                vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.vendorId]);
                TypeCollector paymentStatusList = context.Items[Param.paymentStatusList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.paymentStatusList];
                contractNo = context.Items[Param.contractNo] == null ? String.Empty : context.Items[Param.contractNo].ToString();

                ArrayList paymentStatusEnquiryList = AccountManager.Instance.getPaymentStatusEnquiryList(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, handlingOfficeId, invoiceDateFrom, invoiceDateTo, subDocDateFrom, subDocDateTo,
                interfaceDateFrom, interfaceDateTo, paymentTermId, tradingAgencyId, vendorId, paymentStatusList, contractNo);
                context.Items.Add(Param.paymentStatusEnquiryList, paymentStatusEnquiryList);
            }
            else if (action == Action.GetUKClaimList)
            {
                ukDebitNoteNo = context.Items[Param.ukDebitNoteNo] == null ? string.Empty : context.Items[Param.ukDebitNoteNo].ToString();
                debitNoteDateFrom = context.Items[Param.debitNoteDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteDateFrom]);
                debitNoteDateTo = context.Items[Param.debitNoteDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteDateTo]);
                debitNoteReceivedDateFrom = context.Items[Param.debitNoteReceivedDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteReceivedDateFrom]);
                debitNoteReceivedDateTo = context.Items[Param.debitNoteReceivedDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteReceivedDateTo]);

                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                int handlingOfficeId = context.Items[Param.handlingOfficeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.handlingOfficeId]);
                TypeCollector workflowStatusList = (TypeCollector)context.Items[Param.workflowStatusList];

                claimTypeId = context.Items[Param.ukClaimTypeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.ukClaimTypeId]);
                vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.vendorId]);
                contractNo = context.Items[Param.contractNo] == null ? String.Empty : context.Items[Param.contractNo].ToString();
                itemNo = context.Items[Param.itemNo] == null ? String.Empty : context.Items[Param.itemNo].ToString();
                termOfPurchaseId = context.Items[Param.orderType] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.orderType].ToString());

                List<UKClaimDef> list = UKClaimManager.Instance.getUKClaimListByCriteria(claimTypeId, officeId, handlingOfficeId, ukDebitNoteNo, vendorId, itemNo, contractNo, string.Empty, debitNoteDateFrom, debitNoteDateTo, debitNoteReceivedDateFrom, debitNoteReceivedDateTo, workflowStatusList, termOfPurchaseId);
                context.Items.Add(Param.ukClaimList, list);
                List<UKClaimRefundDef> refundList = UKClaimManager.Instance.getUKClaimRefundListByCriteria(claimTypeId, officeId, handlingOfficeId, ukDebitNoteNo, vendorId, itemNo, contractNo, string.Empty, debitNoteDateFrom, debitNoteDateTo, debitNoteReceivedDateFrom, debitNoteReceivedDateTo, workflowStatusList, termOfPurchaseId);
                context.Items.Add(Param.claimRefundList, refundList);
            }
            else if (action == Action.GetUKDiscountClaimList)
            {
                ukDebitNoteNo = context.Items[Param.ukDebitNoteNo] == null ? string.Empty : context.Items[Param.ukDebitNoteNo].ToString();
                debitNoteDateFrom = context.Items[Param.debitNoteDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteDateFrom]);
                debitNoteDateTo = context.Items[Param.debitNoteDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteDateTo]);
                debitNoteReceivedDateFrom = context.Items[Param.debitNoteReceivedDateFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteReceivedDateFrom]);
                debitNoteReceivedDateTo = context.Items[Param.debitNoteReceivedDateTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteReceivedDateTo]);

                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                int handlingOfficeId = context.Items[Param.handlingOfficeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.handlingOfficeId]);

                bool nextDNNo = (bool)context.Items[Param.nextDNNo];
                bool appliedUKDiscount = (bool)context.Items[Param.appliedUKDiscount];

                vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.vendorId]);
                contractNo = context.Items[Param.contractNo] == null ? String.Empty : context.Items[Param.contractNo].ToString();
                itemNo = context.Items[Param.itemNo] == null ? String.Empty : context.Items[Param.itemNo].ToString();

                List<UKDiscountClaimDef> list = UKClaimManager.Instance.getUKDiscountClaimListByCriteria(officeId, handlingOfficeId, ukDebitNoteNo, vendorId, itemNo, contractNo, debitNoteDateFrom, debitNoteDateTo, debitNoteReceivedDateFrom, debitNoteReceivedDateTo, nextDNNo, appliedUKDiscount);
                context.Items.Add(Param.ukClaimList, list);
                List<UKDiscountClaimRefundDef> refundList = UKClaimManager.Instance.getUKDiscountClaimRefundListByCriteria(officeId, handlingOfficeId, ukDebitNoteNo, vendorId, itemNo, contractNo, debitNoteDateFrom, debitNoteDateTo, debitNoteReceivedDateFrom, debitNoteReceivedDateTo);
                context.Items.Add(Param.claimRefundList, refundList);
            }
            else if (action == Action.GetOSUKClaimList)
            {
                cutOffDate = context.Items[Param.cutoffDate] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.cutoffDate]);
                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.vendorId]);
                termOfPurchaseId = context.Items[Param.orderType] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.orderType]);
                int handlingOfficeId = context.Items[Param.handlingOfficeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.handlingOfficeId]);
                int ncOptionId = context.Items[Param.ncOptionId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.ncOptionId]);
                TypeCollector wfsExcludingList = (context.Items[Param.workflowStatusList] == null ? TypeCollector.Inclusive : (TypeCollector)context.Items[Param.workflowStatusList]);
                List<UKClaimDef> list = UKClaimManager.Instance.getOutstandingUKClaimList(officeId, vendorId, termOfPurchaseId, cutOffDate, handlingOfficeId, ncOptionId, wfsExcludingList);

                context.Items.Add(Param.ukClaimList, list);
            }
            else if (action == Action.GetOSUKDiscountClaimList)
            {
                cutOffDate = context.Items[Param.cutoffDate] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.cutoffDate]);
                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.vendorId]);
                termOfPurchaseId = context.Items[Param.orderType] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.orderType]);
                int handlingOfficeId = context.Items[Param.handlingOfficeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.handlingOfficeId]);

                List<UKDiscountClaimDef> list = UKClaimManager.Instance.getOutstandingUKDiscountClaimReport(officeId, vendorId, termOfPurchaseId, cutOffDate, handlingOfficeId);
                context.Items.Add(Param.ukClaimList, list);
            }
            else if (action == Action.GetMFRNQtyAnalysisList)
            {
                int fiscalYear = int.Parse(context.Items[Param.fiscalYear].ToString());
                int periodFrom = int.Parse(context.Items[Param.periodFrom].ToString());
                int periodTo = int.Parse(context.Items[Param.periodTo].ToString());

                UKClaimMFRNQtyAnalysisReportDs ds = ReporterWorker.Instance.getUKClaimMFRNQtyAnalysisReportList(fiscalYear, periodFrom, periodTo);
                context.Items.Add(Param.mfrnQtyAnalysisList, ds);
            }
            else if (action == Action.GetUKClaimPhasingList)
            {
                vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.vendorId]);
                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                int fiscalYear = context.Items[Param.fiscalYear] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.fiscalYear]);
                string[] period = (context.Items[Param.period] == null ? "0,0".Split(',') : context.Items[Param.period].ToString().Split(','));
                string docType = (context.Items[Param.docType] == null ? "" : context.Items[Param.docType].ToString());
                if (context.Items[Param.reportGroupType].ToString() == "Office")
                {

                }
                else if (context.Items[Param.reportGroupType].ToString() == "ProductTeam")
                {
                    List<UKClaimPhasingByProductTeamDef> list = UKClaimManager.Instance.getUKClaimPhasingByProductTeamReport(fiscalYear, Convert.ToInt32(period[0]), Convert.ToInt32(period[1]), officeId, vendorId);
                    context.Items.Add(Param.ukClaimPhasingList, list);
                }
                else
                {
                    List<UKClaimPhasingDef> list = UKClaimManager.Instance.getUKClaimPhasingReport(fiscalYear, Convert.ToInt32(period[0]), Convert.ToInt32(period[1]), officeId, vendorId, (docType == "0" ? 1 : 0));
                    context.Items.Add(Param.ukClaimPhasingList, list);
                }
            }
            else if (action == Action.GetUKClaimByKey)
            {
                int claimId = (int)context.Items[Param.claimId];
                UKClaimDef def = UKClaimManager.Instance.getUKClaimByKey(claimId);
                context.Items.Add(Param.claim, def);
                context.Items.Add(Param.action, "EDIT");
            }
            else if (action == Action.GetUKDiscountClaimByKey)
            {
                int claimId = (int)context.Items[Param.claimId];
                UKDiscountClaimDef def = UKClaimManager.Instance.getUKDiscountClaimByKey(claimId);
                context.Items.Add(Param.claim, def);
                context.Items.Add(Param.action, "EDIT");
            }
            else if (action == Action.GetUKClaimByGuid)
            {
                string guId = context.Items[Param.guid].ToString();
                UKClaimDef def = UKClaimManager.Instance.getUKClaimByGuid(guId);
                context.Items.Add(Param.claim, def);
                context.Items.Add(Param.action, "EDIT");
            }
            else if (action == Action.ReviewUKClaimDN)
            {
                string guId = context.Items[Param.guid].ToString();
                UKClaimDef def = UKClaimManager.Instance.getUKClaimByGuid(guId);
                context.Items.Add(Param.claim, def);
                context.Items.Add(Param.action, "REVIEW");
            }
            else if (action == Action.DeleteUKClaim)
            {
                int claimId = (int)context.Items[Param.claimId];
                UKClaimManager.Instance.deleteUKClaim(claimId, userId);
                /*
                int claimId = (int)context.Items[Param.claimId];
                UKClaimDef def = UKClaimManager.Instance.getUKClaimByKey(claimId);
                UKClaimManager.Instance.setClaimWorkflowStatus(claimId, ClaimWFS.CANCELLED.Id, userId);
                */
            }
            else if (action == Action.DeleteUKDiscountClaim)
            {
                int claimId = (int)context.Items[Param.claimId];
                UKClaimManager.Instance.deleteUKDiscountClaim(claimId, userId);
            }
            else if (action == Action.CreateUKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                context.Items.Add(Param.claim, def);
                context.Items.Add(Param.action, "CREATE");
            }
            else if (action == Action.CreateUKDiscountClaim)
            {
                UKDiscountClaimDef def = new UKDiscountClaimDef();
                context.Items.Add(Param.claim, def);
                context.Items.Add(Param.action, "CREATE");
            }
            else if (action == Action.UpdateUKClaim)
            {
                UKClaimDef def = (UKClaimDef)context.Items[Param.claim];
                UKClaimBIADiscrepancyDef discrepancyDef = (UKClaimBIADiscrepancyDef)context.Items[Param.claimBIADiscrepancy];
                UKClaimManager.Instance.updateUKClaimDef(def, userId);
                if (discrepancyDef != null)
                    UKClaimManager.Instance.updateUKClaimBIADiscrepancyDef(discrepancyDef, userId);
                /*
                bool sendNotification = (bool)context.Items[Param.SendNotification];
                if (sendNotification)
                    UKClaimManager.Instance.sendUKClaimReviewNotification(def);
                */
                context.Items.Add(Param.claimId, def.ClaimId.ToString());
                context.Items.Add(Param.action, "UPDATE");
            }
            else if (action == Action.UpdateUKDiscountClaim)
            {
                UKDiscountClaimDef def = (UKDiscountClaimDef)context.Items[Param.claim];
                UKClaimManager.Instance.updateUKDiscountClaimDef(def, userId);

                context.Items.Add(Param.claimId, def.ClaimId.ToString());
                context.Items.Add(Param.action, "UPDATE");
            }
            else if (action == Action.SendAlertToPic)
            {
                contractNo = context.Items[Param.contractNo] == null ? string.Empty : context.Items[Param.contractNo].ToString();
                ContractDef contractDef = OrderManager.Instance.getContractByContractNo(contractNo);
                ArrayList shipmentList = OrderManager.Instance.getShipmentListByContractNo(contractNo);

                foreach (ShipmentDef shipment in shipmentList)
                {
                    if (shipment.IsUKDiscount == 0)
                        NoticeHelper.sendUKDiscountClaimAlert(contractDef.Merchandiser.UserId, contractNo + "-" + shipment.DeliveryNo.ToString());
                }
            }
            else if (action == Action.getUKClaimRefundByKey)
            {
                int claimRefundId = (int)context.Items[Param.claimRefundId];
                UKClaimRefundDef def = UKClaimManager.Instance.getUKClaimRefundByKey(claimRefundId);
                context.Items.Add(Param.claimRefund, def);
            }
            else if (action == Action.getUKDiscountClaimRefundByKey)
            {
                int claimRefundId = (int)context.Items[Param.claimRefundId];
                UKDiscountClaimRefundDef def = UKClaimManager.Instance.getUKDiscountClaimRefundByKey(claimRefundId);
                context.Items.Add(Param.claimRefund, def);
            }
            else if (action == Action.UpdateUKClaimRefundDef)
            {
                UKClaimRefundDef refund = (UKClaimRefundDef)context.Items[Param.claimRefund];
                UKClaimManager.Instance.updateUKClaimRefundDef(refund, userId);
                context.Items.Add(Param.claimRefundId, refund.ClaimRefundId.ToString());
            }
            else if (action == Action.UpdateUKDiscountClaimRefundDef)
            {
                UKDiscountClaimRefundDef refund = (UKDiscountClaimRefundDef)context.Items[Param.claimRefund];
                UKClaimManager.Instance.updateUKDiscountClaimRefundDef(refund, userId);
                context.Items.Add(Param.claimRefundId, refund.ClaimRefundId.ToString());
            }
            else if (action == Action.DeleteUKClaimRefundDef)
            {
                UKClaimRefundDef refund = (UKClaimRefundDef)context.Items[Param.claimRefund];
                UKClaimManager.Instance.deleteUKClaimRefund(refund.ClaimRefundId, userId);
            }
            else if (action == Action.DeleteUKDiscountClaimRefundDef)
            {
                UKDiscountClaimRefundDef refund = (UKDiscountClaimRefundDef)context.Items[Param.claimRefund];
                UKClaimManager.Instance.deleteUKDiscountClaimRefund(refund.ClaimRefundId, userId);
            }
            else if (action == Action.GetUKClaimDCNoteByDCNoteNo)
            {
                string dcNoteNo = (string)context.Items[Param.dcNoteNo];
                UKClaimDCNoteDef def = UKClaimManager.Instance.getUKClaimDCNoteByDCNoteNo(dcNoteNo);
                context.Items.Add(Param.dcNote, def);
                /*
                context.Items.Add(Param.action, "EDIT");
                */
            }
            else if (action == Action.GetSupplierProductByItemNo)
            {
                itemNo = context.Items[Param.itemNo] == null ? string.Empty : context.Items[Param.itemNo].ToString();

                ArrayList list = AccountManager.Instance.GetSupplierProductByItemNo(itemNo);
                context.Items.Add(Param.supplierProductList, list);
            }
            else if (action == Action.MailUKClaimDN)
            {
                ArrayList list = (ArrayList)context.Items[Param.ukClaimIdList];
                UKClaimManager.Instance.mailUKClaimDN(list, userId);
            }
            else if (action == Action.UpdateNTInvoice)
            {
                NTInvoiceDef invoiceDef = (NTInvoiceDef)context.Items[Param.ntInvoice];
                ArrayList invoiceDetailList = (ArrayList)context.Items[Param.ntInvoiceDetailList];
                ArrayList rechargeList = context.Items[Param.ntRechargeDetailList] == null ? null : (ArrayList)context.Items[Param.ntRechargeDetailList];

                NonTradeManager.Instance.updateNTInvoice(invoiceDef, invoiceDetailList, rechargeList, userId);
            }
            else if (action == Action.GetNTInvoiceList)
            {
                TypeCollector officeIdList = (TypeCollector)context.Items[Param.officeList];
                int expenseTypeId = (int)context.Items[Param.expenseTypeId];
                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int periodFrom = (int)context.Items[Param.periodFrom];
                int periodTo = (int)context.Items[Param.periodTo];
                invoiceDateFrom = Convert.ToDateTime(context.Items[Param.invoiceDateFrom]);
                invoiceDateTo = Convert.ToDateTime(context.Items[Param.invoiceDateTo]);
                DateTime dueDateFrom = Convert.ToDateTime(context.Items[Param.dueDateFrom]);
                DateTime dueDateTo = Convert.ToDateTime(context.Items[Param.dueDateTo]);

                DateTime settlementDateFrom = DateTime.MinValue;
                if (context.Items[Param.settlementDateFrom] != null)
                    settlementDateFrom = Convert.ToDateTime(context.Items[Param.settlementDateFrom]);
                DateTime settlementDateTo = DateTime.MinValue;
                if (context.Items[Param.settlementDateTo] != null)
                    settlementDateTo = Convert.ToDateTime(context.Items[Param.settlementDateTo]);

                string invoiceNoFrom = context.Items[Param.invoiceNoFrom].ToString();
                string invoiceNoTo = context.Items[Param.invoiceNoTo].ToString();
                string customerNoFrom = context.Items[Param.customerNoFrom].ToString();
                string customerNoTo = context.Items[Param.customerNoTo].ToString();
                string nslRefNoFrom = context.Items[Param.nslRefNoFrom].ToString();
                string nslRefNoTo = context.Items[Param.nslRefNoTo].ToString();
                vendorId = (int)context.Items[Param.vendorId];
                int includePayByHK = 0;
                if (context.Items[Param.includePayByHK] != null)
                    includePayByHK = (int)context.Items[Param.includePayByHK];

                TypeCollector workflowStatusList;
                if (context.Items[Param.workflowStatusList] != null)
                    workflowStatusList = (TypeCollector)context.Items[Param.workflowStatusList];
                else
                    workflowStatusList = TypeCollector.Inclusive;

                DateTime paymentDateFrom = Convert.ToDateTime(context.Items[Param.paymentDateFrom]);
                DateTime paymentDateTo = Convert.ToDateTime(context.Items[Param.paymentDateTo]);
                int currencyId = context.Items[Param.currencyId] == null ? -1 : (int)context.Items[Param.currencyId];
                int paymentMethodId = context.Items[Param.paymentMethodId] == null ? -1 : (int)context.Items[Param.paymentMethodId];
                int departmentId = context.Items[Param.departmentId] == null ? -1 : (int)context.Items[Param.departmentId];
                int approverId = context.Items[Param.userId] == null ? -1 : (int)context.Items[Param.userId];
                int firstApproverId = context.Items[Param.approverId] == null ? -1 : (int)context.Items[Param.approverId];
                string dcIndicator = context.Items[Param.dcIndicator] == null ? string.Empty : context.Items[Param.dcIndicator].ToString();
                int submittedBy = -1;
                if (context.Items[Param.submittedBy] != null)
                    submittedBy = (int)context.Items[Param.submittedBy];
                ArrayList list = NonTradeManager.Instance.getNTInvoiceList(officeIdList, expenseTypeId, fiscalYear, periodFrom, periodTo, invoiceDateFrom, invoiceDateTo, dueDateFrom, dueDateTo, settlementDateFrom, settlementDateTo, invoiceNoFrom, invoiceNoTo, customerNoFrom, customerNoTo,
                    nslRefNoFrom, nslRefNoTo, vendorId, workflowStatusList, paymentDateFrom, paymentDateTo, currencyId, paymentMethodId, departmentId, approverId, includePayByHK, dcIndicator, firstApproverId, submittedBy);

                context.Items.Add(Param.invoiceList, list);
            }
            else if (action == Action.UpdateNTVendor)
            {
                NTVendorDef ntVendor = (NTVendorDef)context.Items[Param.ntVendor];
                ArrayList expenseTypeList = (ArrayList)context.Items[Param.expenseTypeList];

                NonTradeManager.Instance.updateNTVendor(ntVendor, expenseTypeList, userId);
            }
            else if (action == Action.GetNTVendorList)
            {
                officeId = (int)context.Items[Param.officeId];
                int expenseTypeId = (int)context.Items[Param.expenseTypeId];
                int ntVendorId = (int)context.Items[Param.ntVendorId];
                int workflowStatusId = (int)context.Items[Param.workflowStatusId];

                ArrayList list = NonTradeManager.Instance.getNTVendorList(officeId, ntVendorId, expenseTypeId, workflowStatusId);
                context.Items.Add(Param.ntVendorList, list);

            }
            else if (action == Action.GetNTInvoiceDetailListForRechargeDCNote)
            {
                officeId = (int)context.Items[Param.officeId];
                companyId = (int)context.Items[Param.companyId];
                ArrayList invoiceList = new ArrayList();

                ArrayList list = NonTradeManager.Instance.getNTInvoiceDetailListForRechargeDCNote(officeId, companyId, ref invoiceList);
                context.Items.Add(Param.ntRechargeDetailList, list);
                context.Items.Add(Param.invoiceList, invoiceList);
            }
            else if (action == Action.GetCurrentNTMonthEndStatusList)
            {
                officeList = (ArrayList)context.Items[Param.officeList];

                ArrayList statusList = NonTradeManager.Instance.getCurrentNTMonthEndStatus(officeList);
                context.Items.Add(Param.ntMonthEndStatusList, statusList);
            }
            else if (action == Action.UpdateNTMonthEndStatus)
            {
                NTMonthEndStatusDef ntMonthEndStatusDef = (NTMonthEndStatusDef)context.Items[Param.ntMonthEndStatus];

                NonTradeManager.Instance.updateMonthEndStatus(ntMonthEndStatusDef, userId);
            }
            /*
            else if (action == Action.UpdateUKClaimAndRefundList)
            {
                UKClaimDef claim = (UKClaimDef)context.Items[Param.claim];
                List<UKClaimRefundDef> refundList = (List<UKClaimRefundDef>)context.Items[Param.claimRefundList];
                bool sendNotification = (bool)context.Items[Param.SendNotification];

                UKClaimManager.Instance.updateUKClaimAndRefundList(claim, refundList, userId);
                if (sendNotification)
                    UKClaimManager.Instance.sendUKClaimReviewNotification(claim);

                context.Items.Add(Param.claimId, claim.ClaimId.ToString());
                //context.Items.Add(Param.claimRefundList, refundList);
                context.Items.Add(Param.action, "UPDATE");
            }
            */
            else if (action == Action.GetNTRoleByKey)
            {
                int roleId = (int)context.Items[Param.ntRoleId];
                NTRoleRef def = NonTradeManager.Instance.getNTRoleByKey(roleId);
                context.Items.Add(Param.ntRole, def);
            }
            else if (action == Action.DeleteNTUserRoleAccess)
            {
                NTUserRoleAccessDef def = (NTUserRoleAccessDef)context.Items[Param.ntUserRoleAccess];
                NonTradeManager.Instance.deleteNTUserRoleAccess(def, userId);
            }
            else if (action == Action.UpdateNTUserRoleAccess)
            {
                NTUserRoleAccessDef def = (NTUserRoleAccessDef)context.Items[Param.ntUserRoleAccess];
                NonTradeManager.Instance.updateNTUserRoleAccess(def, userId);
            }
            else if (action == Action.UpdateAdvancePaymentAndInstalment)
            {
                AdvancePaymentDef payment = (AdvancePaymentDef)context.Items[Param.advancePayment];
                List<AdvancePaymentInstalmentDetailDef> instalments = (List<AdvancePaymentInstalmentDetailDef>)context.Items[Param.instalments];
                List<AdvancePaymentActionHistoryDef> historys = (List<AdvancePaymentActionHistoryDef>)context.Items[Param.paymenthistory];
                AccountManager.Instance.updateAdvancePaymentAndInstalment(payment, instalments, historys, userId);
            }
            else if (action == Action.GetAdvancePaymentByCriteria)
            {
                officeId = context.Items[Param.officeId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.officeId]);
                vendorId = vendorId = context.Items[Param.vendorId] == null ? GeneralCriteria.ALL : Convert.ToInt32(context.Items[Param.vendorId]);
                paymentNo = context.Items[Param.paymentNo] == null ? "" : context.Items[Param.paymentNo].ToString();
                contractNo = context.Items[Param.contractNo] == null ? "" : context.Items[Param.contractNo].ToString();
                int paymentStatusId = int.Parse(context.Items[Param.advancePaymentSettlementStatus].ToString());
                string lcBillRefNo = context.Items[Param.lcBillRefNo] == null ? "" : context.Items[Param.lcBillRefNo].ToString();
                DateTime paymentDateFrom = Convert.ToDateTime(context.Items[Param.paymentDateFrom]);
                DateTime paymentDateTo = Convert.ToDateTime(context.Items[Param.paymentDateTo]);
                List<AdvancePaymentDef> list = AccountManager.Instance.getAdvancePaymentByCriteria(vendorId, officeId, paymentNo, contractNo, lcBillRefNo, paymentDateFrom, paymentDateTo, paymentStatusId);
                context.Items.Add(Param.advancePaymentList, list);
            }
            else if (action == Action.UpdateAdvancePaymentAndOrderDetail)
            {
                AdvancePaymentDef advancePaymentDef = (AdvancePaymentDef)context.Items[Param.advancePayment];
                List<AdvancePaymentOrderDetailDef> orderDetails = (List<AdvancePaymentOrderDetailDef>)context.Items[Param.advancePaymentOrderDetailList];
                List<AdvancePaymentBalanceSettlementDef> balanceSettlement = (List<AdvancePaymentBalanceSettlementDef>)context.Items[Param.advancePaymentBalanceSettlement];
                List<AdvancePaymentActionHistoryDef> historys = (List<AdvancePaymentActionHistoryDef>)context.Items[Param.paymenthistory];
                AccountManager.Instance.updateAdvancePaymentAndOrderDetail(advancePaymentDef, orderDetails, balanceSettlement, historys, userId);
            }
            else if (action == Action.DeleteAdvancePaymentOrderDetail)
            {
                int paymentId = (int)context.Items[Param.paymentId];
                int shipmentId = (int)context.Items[Param.shipmentId];
                AdvancePaymentActionHistoryDef history = (AdvancePaymentActionHistoryDef)context.Items[Param.paymenthistory];
                AccountManager.Instance.deleteAdvancePaymentOrderDetail(paymentId, shipmentId, history, userId);
            }
            else if (action == Action.GetTurkeyEpicorGLJournalDetailList)
            {

                bool success = false;

                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int period = (int)context.Items[Param.period];
                string filePath = string.Format("{0}_{1}_P{2}_{3}.xml", WebConfig.getValue("appSettings", "LOGO_XML_PATH"), fiscalYear.ToString(), period.ToString(), DateTime.Now.ToString("yyyyMMdd"));
                TypeCollector segValueList = TypeCollector.Inclusive;
                EpicorGLJournalDetailListDef list = AccountManager.Instance.getTurkeyEpicorGLJournalDetailList(fiscalYear, period, segValueList);
                EpicorGLExtractLogListDef extractedList = AccountManager.Instance.getTurkeyEpicorGLJournalRecordCount(fiscalYear, period);

                if (list != null)
                {
                    List<EpicorGLJournalDetailDef> newList = list.EpicorGLJournalDetailList.Where(l => !extractedList.EpicorGLExtractLogList.Any(l2 => l2.RecordId == l.RecordId)).ToList();//compare the list
                    if (newList.Count != 0) //non-match data found
                    {
                        context.Items.Add(Param.journal, newList);
                        success = exportLogoXMLfile(newList, filePath, fiscalYear, period);
                        if (success) // success generate xml file
                        {
                            ArrayList attachmentList = new ArrayList();
                            attachmentList.Add(filePath);
                            LogoInterfaceRequestDef request = new LogoInterfaceRequestDef(); //insert request record
                            request.Office = CommonUtil.getOfficeRefByKey(9);
                            request.FiscalYear = fiscalYear;
                            request.Period = period;
                            request.SubmitUser = CommonUtil.getUserByKey(userId);
                            request.SubmitTime = DateTime.Now;
                            AccountManager.Instance.updateLogoInterfaceRequest(request);

                            ArrayList qStruct = new ArrayList();
                            qStruct.Add(new QueryStructDef("DOCUMENTTYPE", "LOGO Interface"));
                            qStruct.Add(new QueryStructDef("Request Id", request.RequestId.ToString()));
                            qStruct.Add(new QueryStructDef("Fiscal Year", fiscalYear.ToString()));
                            qStruct.Add(new QueryStructDef("Period", period.ToString()));

                            string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Account\\LOGO Interface\\", request.RequestId.ToString(), "Logo Interface", qStruct, attachmentList);

                            if (msg != string.Empty)
                            {
                                return;
                            }

                            AccountManager.Instance.insertTurkeyEpicorGLJournalRecord(newList, fiscalYear, period, userId); // insert the record which is generated
                            NoticeHelper.sendLogoXMLFile(fiscalYear, period, userId, filePath);
                        }
                    }
                }
            }
            else if (action == Action.VoidDebitNote)
            {
                UKClaimDCNoteDef def = (UKClaimDCNoteDef)context.Items[Param.dcNote];
                DebitNoteVoidType voidType = (DebitNoteVoidType)context.Items[Param.voidType];
                int? newVendorId = (context.Items[Param.vendorId] != null) ? (int)context.Items[Param.vendorId] : (int?)null;
                UKClaimManager.Instance.voidUKClaimDN(def, voidType, newVendorId, userId);
            }
            else if (action == Action.GetLGHoldPaymentList)
            {
                officeId = (int)context.Items[Param.officeId];
                string lgNo = context.Items[Param.lgNo] == null ? "" : context.Items[Param.lgNo].ToString();
                vendorId = (int)context.Items[Param.vendorId];
                itemNo = context.Items[Param.itemNo] == null ? "" : context.Items[Param.itemNo].ToString();
                contractNo = context.Items[Param.contractNo] == null ? "" : context.Items[Param.contractNo].ToString();
                int paymentStatusId = int.Parse(context.Items[Param.advancePaymentSettlementStatus].ToString());

                LGHoldPaymentDs ds = AccountReportManager.Instance.getLGHoldPaymentList(officeId, lgNo, vendorId, itemNo, contractNo, paymentStatusId);
                context.Items.Add(Param.lgHoldPaymentList, ds);
            }
            else if (action == Action.GenerateILSDiffDCNote)
            {
                officeId = (int)context.Items[Param.officeId];
                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int period = (int)context.Items[Param.period];
                DateTime debitNoteDate = Convert.ToDateTime(context.Items[Param.debitNoteDate]);
                bool isDraft = (bool)context.Items[Param.isDraft];

                ArrayList debitNoteList = null;
                ArrayList debitNoteShipmentList = null;

                AccountManager.Instance.generateILSDiffDebitNote(officeId, fiscalYear, period, debitNoteDate, isDraft, userId,
                    out debitNoteList, out debitNoteShipmentList);

                ILSDiffDCNoteReport rpt = null;

                string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + ".pdf";
                rpt = AccountReportManager.Instance.getILSDiffDCNote(debitNoteList, debitNoteShipmentList, officeId, fiscalYear, period, isDraft);
                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                rpt.Close();
                rpt.Dispose();

                NoticeHelper.sendILSDiffDCNoteReport(officeId, fiscalYear, period, userId, outputFilePath);
            }
            else if (action == Action.GenerateAdvPaymentInstalmentInterestCharges)
            {
                officeId = (int)context.Items[Param.officeId];
                int paymentId = (int)context.Items[Param.paymentId];
                paymentNo = context.Items[Param.paymentNo].ToString();
                DateTime periodFrom = context.Items[Param.periodFrom] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.periodFrom]);
                DateTime periodTo = context.Items[Param.periodTo] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.periodTo]);
                DateTime dcNoteDate = context.Items[Param.debitNoteDate] == null ? DateTime.MinValue : Convert.ToDateTime(context.Items[Param.debitNoteDate]);
                decimal remainingTotal = (decimal)context.Items[Param.remainingTotal];
                decimal periodDays = (decimal)context.Items[Param.periodDays];
                decimal interestRate = (decimal)context.Items[Param.interestRate];
                decimal interestCharges = (decimal)context.Items[Param.interestCharges];
                bool isDebit = (bool)context.Items[Param.isDebit];
                string dcNoteNo = AccountWorker.Instance.fillNextAdjustmentNoteNo(AdjustmentType.ADVANCE_PAYMENT_INTEREST, dcNoteDate, officeId, isDebit ? "D" : "C");

                AdvancePaymentDef def = AccountManager.Instance.getAdvancePaymentByKey(paymentId);
                AdvancePaymentInstalmentDetailDef instalmentDef = new AdvancePaymentInstalmentDetailDef();
                instalmentDef.PaymentId = def.PaymentId;
                instalmentDef.PaymentDate = dcNoteDate;
                instalmentDef.ExpectedAmount = interestCharges;
                instalmentDef.PaymentAmount = interestCharges;
                instalmentDef.Remark = String.Format("Interest Charges ({0}{1}, {2}%, {3}-{4}, {5})", def.Currency.CurrencyCode, remainingTotal, interestRate,
                                                        periodFrom.ToString("dd/MM/yyyy"), periodTo.ToString("dd/MM/yyyy"), dcNoteNo);
                instalmentDef.InterestRate = interestRate;
                instalmentDef.InterestAmt = interestCharges;
                instalmentDef.RemainingTotalAmt = remainingTotal;
                instalmentDef.InterestFromDate = periodFrom;
                instalmentDef.InterestToDate = periodTo;
                instalmentDef.DCNoteDate = dcNoteDate;
                instalmentDef.DCNoteNo = dcNoteNo;
                instalmentDef.IsDCNoteInterfaced = false;
                instalmentDef.Status = 1;

                def.TotalAmount += interestCharges;
                def.InterestChargedAmt += interestCharges;
                AdvancePaymentWorker.Instance.updateAdvancePayment(def, userId);

                AdvancePaymentInstalmentDetailDef existingDef = AccountManager.Instance.getAdvancePaymentInstalmentDetailByKey(def.PaymentId, instalmentDef.PaymentDate);

                while (existingDef != null)
                {
                    if (existingDef.Status == GeneralStatus.ACTIVE.Code)
                    {
                        instalmentDef.PaymentDate = existingDef.PaymentDate.AddDays(1);
                        existingDef = AccountManager.Instance.getAdvancePaymentInstalmentDetailByKey(def.PaymentId, instalmentDef.PaymentDate);
                    }
                    else
                        existingDef = null;
                }

                AccountManager.Instance.updateAdvancePaymentInstalmentDetail(instalmentDef, userId);

                string outputFilePath = WebConfig.getValue("appSettings", "INSTALMENT_DOC_FOLDER") + dcNoteNo + ".pdf";
                AdvPaymentInstalmentInterestChargesReport rpt = AccountReportManager.Instance.getAdvancePaymentInterestChargeReport(def.PaymentId, instalmentDef.PaymentDate);
                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                File.Copy(outputFilePath,  WebConfig.getValue("appSettings", "APP_TMP_REPORT_FOLDER") + dcNoteNo + ".pdf", true);
                rpt.Close();
                rpt.Dispose();

                SunInterfaceQueueDef queueDef = new SunInterfaceQueueDef();
                queueDef.QueueId = -1;
                queueDef.OfficeGroup = CommonWorker.Instance.getReportOfficeGroupByKey(def.OfficeId);
                queueDef.FiscalYear = 0;
                queueDef.Period = 0;
                queueDef.PurchaseTerm = 0;
                queueDef.UTurn = 0;
                queueDef.CategoryType = CategoryType.DAILY;
                queueDef.SourceId = 2;
                queueDef.SubmitTime = DateTime.Now;
                queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.AdvancePaymentInterest.GetHashCode();
                queueDef.User = GeneralWorker.Instance.getUserByKey(userId);
                AccountWorker.Instance.updateSunInterfaceQueue(queueDef);

                ArrayList queryStructs = new ArrayList();
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Advance Payment"));
                queryStructs.Add(new QueryStructDef("Payment No", paymentNo));
                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                long docId = 0;
                ArrayList attachmentList = new ArrayList();
                attachmentList.Add(outputFilePath);

                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    docId = docInfoDef.DocumentID;
                }

                string msg = string.Empty;

                if (docId > 0)
                    msg = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                else
                    msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Account\\Advance Payment\\", paymentNo + " Instalment Doc", "Advance Payment", queryStructs, attachmentList);

                context.Items.Add(Param.dcNote, dcNoteNo);

                /*
                ReportHelper.export(rpt, HttpContext.Current.Response,
                                        CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, dcNoteNo);
                */
                /*
                NoticeHelper.sendAdvPaymentInstalmentInterestChargesReport(officeId, paymentNo, isDebit, userId, outputFilePath);
                */
            }
            else if (action == Action.GenerateOtherChargesDCNote)
            {
                officeId = (int)context.Items[Param.officeId];
                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int period = (int)context.Items[Param.period];
                List<GenericDCNoteDef> list = (List<GenericDCNoteDef>)context.Items[Param.genericDCNoteList];
                List<string> dcNoteNoList = new List<string>();
                string dcNoteNo = string.Empty;
                string debitCreditIndicator = "D";
                bool autoUpload = (bool)context.Items[Param.autoUpload];

                for (int i = 0; i < list.Count; i++)
                {
                    AccountManager.Instance.updateGenericDCNote(list[i], userId, out dcNoteNo);
                    debitCreditIndicator = list[i].DebitCreditIndicator;
                    officeId = list[i].OfficeId;
                    dcNoteNoList.Add(dcNoteNo);
                }
                GenericDCNoteReport report = null;

                string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + dcNoteNoList[0] + ".pdf";
                report = AccountReportManager.Instance.getGenericDCNoteReport(dcNoteNoList);
                report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                report.Close();
                report.Dispose();

                SunInterfaceQueueDef queueDef = new SunInterfaceQueueDef();
                queueDef.QueueId = -1;
                queueDef.OfficeGroup = CommonWorker.Instance.getReportOfficeGroupByKey(officeId);
                queueDef.FiscalYear = 0;
                queueDef.Period = 0;
                queueDef.PurchaseTerm = 0;
                queueDef.UTurn = 0;
                queueDef.CategoryType = CategoryType.DAILY;
                queueDef.SourceId = autoUpload ? 2 : 1;
                queueDef.SubmitTime = DateTime.Now;
                queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.OtherChargeDCNote.GetHashCode();
                queueDef.User = GeneralWorker.Instance.getUserByKey(userId);
                AccountWorker.Instance.updateSunInterfaceQueue(queueDef);

                context.Items.Add(Param.file, outputFilePath);

                /*
                NoticeHelper.sendGenericDCNoteReportToUser(officeId, fiscalYear, period, userId, debitCreditIndicator, outputFilePath);
                */
            }
            else if (action == Action.GenerateThirdPartyCustomerDNToUK)
            {
                officeId = (int)context.Items[Param.officeId];
                int fiscalYear = (int)context.Items[Param.fiscalYear];
                int period = (int)context.Items[Param.period];
                string supplierCode = context.Items[Param.supplierCode].ToString();
                ArrayList list = (ArrayList)context.Items[Param.shipmentIdList];

                foreach (int shipmentId in list)
                {
                    SAndBCommissionDebitNote report = null;
                    InvoiceDef invoice = ShipmentManager.Instance.getInvoiceByShipmentId(shipmentId);
                    string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + "Comm-DN-" + invoice.InvoiceNo.Replace("/", "-") + ".pdf";

                    report = AccountReportManager.Instance.getCommissionDebitNote(shipmentId);
                    report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                    report.Close();
                    report.Dispose();

                    NoticeHelper.sendThirdPartyCustomerDNToUK(officeId, supplierCode, fiscalYear, period, invoice.InvoiceNo + "C", userId, outputFilePath);
                }
            }

        }

        private bool exportLogoXMLfile(List<EpicorGLJournalDetailDef> list, string filePath, int fiscalYear, int period)
        {
            try
            {
                decimal totalDebitAmt = 0m;
                decimal totalCreditAmt = 0m;
                decimal totalBookDebitAmt = 0m;
                decimal totalBookCreditAmt = 0m;
                decimal diff = 0m;
                string groupID = string.Empty;
                string shortChar = string.Empty;
                string journalCode = string.Empty;
                DateTime apDate = DateTime.MinValue;
                DateTime journalDate = DateTime.MinValue;
                string character = string.Empty;
                string legalNumberId = string.Empty;
                string isBank = string.Empty;
                string isCash = string.Empty;
                string empty = string.Empty;
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.OmitXmlDeclaration = false;
                xmlWriterSettings.IndentChars = "\t";
                xmlWriterSettings.NewLineChars = "\r\n";
                xmlWriterSettings.Encoding = Encoding.GetEncoding("ISO-8859-9");
                using (FileStream output = new FileStream(filePath, FileMode.Create))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(output, xmlWriterSettings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("GL_VOUCHERS");
                        xmlWriter.WriteStartElement("GL_VOUCHER");
                        xmlWriter.WriteAttributeString("DBOP", "INS");
                        for (int i = 0; i < list.Count; i++)
                        {
                            bool isDebit = (list[i].DC == "D") ? true : false;
                            if (i == 0)
                            {
                                foreach (EpicorGLJournalDetailDef temp in list)
                                {
                                    bool isDebit4Cal = (temp.DC == "D") ? true : false;
                                    if (temp.GroupID == list[0].GroupID)
                                    {
                                        decimal tryAmt = 0m;
                                        decimal bookAmt = 0m;
                                        Debug.WriteLine(temp.BookDebitAmount.ToString() + "\t" + temp.BookCreditAmount.ToString() + "\t" + temp.DC);
                                        tryAmt = (isDebit4Cal ? temp.BookDebitAmount : temp.BookCreditAmount);
                                        bookAmt = (isDebit4Cal ? temp.BookDebitAmount : temp.BookCreditAmount);
                                        if (isDebit4Cal)
                                        {
                                            totalDebitAmt += tryAmt;
                                            totalBookDebitAmt += bookAmt;
                                        }
                                        if (!isDebit4Cal)
                                        {
                                            totalCreditAmt += tryAmt;
                                            totalBookCreditAmt += bookAmt;
                                        }
                                    }
                                }
                                diff = Math.Round(totalDebitAmt - totalCreditAmt, 2);
                                if (totalDebitAmt > totalCreditAmt)
                                {
                                    totalCreditAmt += Math.Abs(diff);
                                }
                                else if (totalDebitAmt < totalCreditAmt)
                                {
                                    totalDebitAmt += Math.Abs(diff);
                                }
                                xmlWriter.WriteElementString("TYPE", "4");
                                xmlWriter.WriteElementString("NUMBER", list[i].GroupID);
                                xmlWriter.WriteElementString("DATE", list[i].JEDate.ToString("dd.MM.yyyy"));
                                xmlWriter.WriteElementString("NOTES1", list[i].JEDate.ToString("dd.MM.yyyy"));
                                xmlWriter.WriteElementString("TOTAL_DEBIT", Math.Round(totalDebitAmt, 2).ToString());
                                xmlWriter.WriteElementString("TOTAL_CREDIT", Math.Round(totalCreditAmt, 2).ToString());
                                xmlWriter.WriteElementString("CREATED_BY", "1");
                                xmlWriter.WriteElementString("DATE_CREATED", DateTime.Now.ToString("dd.MM.yyyy"));
                                xmlWriter.WriteElementString("HOUR_CREATED", DateTime.Now.ToString("HH"));
                                xmlWriter.WriteElementString("MIN_CREATED", DateTime.Now.ToString("mm"));
                                xmlWriter.WriteElementString("SEC_CREATED", DateTime.Now.ToString("ss"));
                                xmlWriter.WriteElementString("CURRSEL_TOTALS", "1");
                                xmlWriter.WriteElementString("CURRSEL_DETAILS", "2");
                                xmlWriter.WriteElementString("RC_TOTAL_DEBIT", Math.Round(totalBookDebitAmt, 2).ToString());
                                xmlWriter.WriteElementString("RC_TOTAL_CREDIT", Math.Round(totalBookCreditAmt, 2).ToString());
                                xmlWriter.WriteStartElement("TRANSACTIONS");
                            }
                            else if (groupID != list[i].GroupID)
                            {
                                diff = 0m;
                                totalDebitAmt = 0m;
                                totalCreditAmt = 0m;
                                totalBookDebitAmt = 0m;
                                totalBookCreditAmt = 0m;
                                foreach (EpicorGLJournalDetailDef temp in list)
                                {
                                    bool isDebit4Cal = (temp.DC == "D") ? true : false;
                                    if (temp.GroupID == list[i].GroupID)
                                    {
                                        decimal tryAmt = 0m;
                                        decimal bookAmt = 0m;
                                        tryAmt = (isDebit4Cal ? temp.BookDebitAmount : temp.BookCreditAmount);
                                        bookAmt = (isDebit4Cal ? temp.BookDebitAmount : temp.BookCreditAmount);
                                        if (isDebit4Cal)
                                        {
                                            totalDebitAmt += tryAmt;
                                            totalBookDebitAmt += bookAmt;
                                        }
                                        if (!isDebit4Cal)
                                        {
                                            totalCreditAmt += tryAmt;
                                            totalBookCreditAmt += bookAmt;
                                        }
                                    }
                                }
                                diff = Math.Round(totalDebitAmt - totalCreditAmt, 2);
                                if (totalDebitAmt > totalCreditAmt)
                                {
                                    totalCreditAmt += Math.Abs(diff);
                                }
                                else if (totalDebitAmt < totalCreditAmt)
                                {
                                    totalDebitAmt += Math.Abs(diff);
                                }
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteStartElement("ORGLOGOID");
                                xmlWriter.WriteString(string.Empty);
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteStartElement("DEFNFLDSLIST");
                                xmlWriter.WriteString(string.Empty);
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteStartElement("GENFLDLIST");
                                xmlWriter.WriteString(string.Empty);
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteElementString("DOC_DATE", DateTime.Today.ToString("dd.MM.yyyy"));
                                writeEBookData(shortChar, journalCode, apDate, journalDate, character, legalNumberId, isBank, isCash, groupID, xmlWriter);
                                xmlWriter.WriteElementString("CROSS_FLAG", "0");
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteStartElement("GL_VOUCHER");
                                xmlWriter.WriteAttributeString("DBOP", "INS");
                                xmlWriter.WriteElementString("TYPE", "4");
                                xmlWriter.WriteElementString("NUMBER", list[i].GroupID);
                                xmlWriter.WriteElementString("DATE", list[i].JEDate.ToString("dd.MM.yyyy"));
                                xmlWriter.WriteElementString("NOTES1", list[i].JEDate.ToString("dd.MM.yyyy"));
                                xmlWriter.WriteElementString("TOTAL_DEBIT", Math.Round(totalDebitAmt, 2).ToString());
                                xmlWriter.WriteElementString("TOTAL_CREDIT", Math.Round(totalCreditAmt, 2).ToString());
                                xmlWriter.WriteElementString("CREATED_BY", "1");
                                xmlWriter.WriteElementString("DATE_CREATED", DateTime.Now.ToString("dd.MM.yyyy"));
                                xmlWriter.WriteElementString("HOUR_CREATED", DateTime.Now.ToString("HH"));
                                xmlWriter.WriteElementString("MIN_CREATED", DateTime.Now.ToString("mm"));
                                xmlWriter.WriteElementString("SEC_CREATED", DateTime.Now.ToString("ss"));
                                xmlWriter.WriteElementString("CURRSEL_TOTALS", "1");
                                xmlWriter.WriteElementString("CURRSEL_DETAILS", "2");
                                xmlWriter.WriteElementString("RC_TOTAL_DEBIT", Math.Round(totalBookDebitAmt, 2).ToString());
                                xmlWriter.WriteElementString("RC_TOTAL_CREDIT", Math.Round(totalBookCreditAmt, 2).ToString());
                                xmlWriter.WriteStartElement("TRANSACTIONS");
                            }
                            xmlWriter.WriteStartElement("TRANSACTION");
                            if (!isDebit)
                            {
                                xmlWriter.WriteElementString("SIGN", "1");
                            }
                            string logoCOA = GeneralWorker.Instance.getLogoCOA(list[i].SegValue1);
                            if (logoCOA != "UNDEFINED")
                            {
                                xmlWriter.WriteElementString("GL_CODE", logoCOA);
                                xmlWriter.WriteElementString("PARENT_GLCODE", logoCOA.Split('.')[0]);
                            }
                            else
                            {
                                xmlWriter.WriteElementString("GL_CODE", logoCOA + " - " + list[i].SegValue1);
                                xmlWriter.WriteElementString("PARENT_GLCODE", logoCOA);
                            }
                            if (list[i].CurrencyCode == "TRY")
                            {
                                decimal tryExRate = 0m;
                                decimal usdExRate = 0m;
                                decimal rc_exRate = 0m;
                                decimal exRate = 0m;
                                tryExRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.TRY.Id, list[i].JEDate);
                                usdExRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, list[i].JEDate);
                                exRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, GeneralWorker.Instance.getCurrencyByCurrencyCode(list[i].CurrencyCode).CurrencyId, list[i].JEDate);
                                rc_exRate = Math.Round(usdExRate / tryExRate, 4);
                                if (isDebit)
                                {
                                    xmlWriter.WriteElementString("DEBIT", Math.Round(list[i].BookDebitAmount, 2).ToString());
                                }
                                if (!isDebit)
                                {
                                    xmlWriter.WriteElementString("CREDIT", Math.Round(list[i].BookCreditAmount, 2).ToString());
                                }
                                xmlWriter.WriteElementString("DESCRIPTION", list[i].Description);
                                xmlWriter.WriteElementString("RC_XRATE", rc_exRate.ToString());
                                if (isDebit)
                                {
                                    xmlWriter.WriteElementString("RC_AMOUNT", Math.Round(list[i].DebitAmount * Math.Round(exRate / usdExRate, 4), 2).ToString());
                                }
                                if (!isDebit)
                                {
                                    xmlWriter.WriteElementString("RC_AMOUNT", Math.Round(list[i].CreditAmount * Math.Round(exRate / usdExRate, 4), 2).ToString());
                                }
                                xmlWriter.WriteElementString("TC_XRATE", "1");
                                xmlWriter.WriteElementString("TC_AMOUNT", isDebit ? Math.Round(list[i].BookDebitAmount, 2).ToString() : Math.Round(list[i].BookCreditAmount, 2).ToString());
                            }
                            else
                            {
                                decimal tryExRate = 0m;
                                decimal usdExRate = 0m;
                                decimal exRate = 0m;
                                decimal rc_exRate = 0m;
                                tryExRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.TRY.Id, list[i].JEDate);
                                usdExRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, list[i].JEDate);
                                exRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, GeneralWorker.Instance.getCurrencyByCurrencyCode(list[i].CurrencyCode).CurrencyId, list[i].JEDate);
                                rc_exRate = Math.Round(usdExRate / tryExRate, 4);
                                if (isDebit)
                                {
                                    xmlWriter.WriteElementString("DEBIT", Math.Round(list[i].BookDebitAmount, 2).ToString());
                                }
                                if (!isDebit)
                                {
                                    xmlWriter.WriteElementString("CREDIT", Math.Round(list[i].BookCreditAmount, 2).ToString());
                                }
                                xmlWriter.WriteElementString("DESCRIPTION", list[i].Description);
                                xmlWriter.WriteElementString("RC_XRATE", rc_exRate.ToString());
                                if (isDebit)
                                {
                                    xmlWriter.WriteElementString("RC_AMOUNT", Math.Round(list[i].DebitAmount * Math.Round(exRate / usdExRate, 4), 2).ToString());
                                }
                                if (!isDebit)
                                {
                                    xmlWriter.WriteElementString("RC_AMOUNT", Math.Round(list[i].CreditAmount * Math.Round(exRate / usdExRate, 4), 2).ToString());
                                }
                                xmlWriter.WriteElementString("TC_XRATE", Math.Round(exRate / tryExRate, 4).ToString());
                                xmlWriter.WriteElementString("TC_AMOUNT", isDebit ? Math.Round(list[i].DebitAmount, 2).ToString() : Math.Round(list[i].CreditAmount, 2).ToString());
                                xmlWriter.WriteElementString("CURR_TRANS", CurrencyId.getLogoCurrencyId(list[i].CurrencyCode).ToString());
                            }
                            xmlWriter.WriteElementString("QUANTITY", Math.Round(list[i].Number01, 0).ToString());
                            xmlWriter.WriteElementString("CURRSEL_TRANS", "2");
                            xmlWriter.WriteStartElement("DETLIST");
                            xmlWriter.WriteString(string.Empty);
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("DEFNFLDLIST");
                            xmlWriter.WriteString(string.Empty);
                            xmlWriter.WriteEndElement();
                            journalCode = list[i].JournalCode;
                            character = list[i].Character05.Trim();
                            legalNumberId = list[i].LegalNumberID.ToUpper();
                            isBank = list[i].IsBank;
                            isCash = list[i].IsCash;
                            journalDate = list[i].JEDate;
                            groupID = list[i].GroupID;
                            if (list[i].JournalCode == "PJ" && list[i].ShortChar01.Trim() != string.Empty)
                            {
                                shortChar = list[i].ShortChar01.Trim();
                                apDate = list[i].APInvoiceDate;
                            }
                            xmlWriter.WriteStartElement("GENFLDLIST");
                            xmlWriter.WriteString(string.Empty);
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteElementString("MONTH", list[i].JEDate.Month.ToString());
                            xmlWriter.WriteElementString("YEAR", list[i].JEDate.Year.ToString());
                            xmlWriter.WriteElementString("DOC_DATE", list[i].JEDate.ToString("dd.MM.yyyy"));
                            xmlWriter.WriteEndElement();
                            groupID = list[i].GroupID;
                        }
                        if (diff != 0m)
                        {
                            bool isDebit = diff < 0m;
                            DateTime endDate = GeneralWorker.Instance.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, period).EndDate;
                            xmlWriter.WriteStartElement("TRANSACTION");
                            if (!isDebit)
                            {
                                xmlWriter.WriteElementString("SIGN", "1");
                            }
                            xmlWriter.WriteElementString("GL_CODE", "740.01.003");
                            xmlWriter.WriteElementString("PARENT_GLCODE", "740");
                            if (isDebit)
                            {
                                xmlWriter.WriteElementString("DEBIT", Math.Abs(Math.Round(diff, 2)).ToString());
                            }
                            if (!isDebit)
                            {
                                xmlWriter.WriteElementString("CREDIT", Math.Abs(Math.Round(diff, 2)).ToString());
                            }
                            xmlWriter.WriteElementString("DESCRIPTION", "GAIN/LOSS ON EXCHANGE-OTHER");
                            xmlWriter.WriteElementString("RC_XRATE", "0");
                            xmlWriter.WriteElementString("RC_AMOUNT", "0");
                            xmlWriter.WriteElementString("TC_XRATE", "1");
                            xmlWriter.WriteElementString("TC_AMOUNT", Math.Abs(Math.Round(diff, 2)).ToString());
                            xmlWriter.WriteElementString("QUANTITY", "0");
                            xmlWriter.WriteElementString("CURRSEL_TRANS", "2");
                            xmlWriter.WriteStartElement("DETLIST");
                            xmlWriter.WriteString(string.Empty);
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("DEFNFLDLIST");
                            xmlWriter.WriteString(string.Empty);
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("GENFLDLIST");
                            xmlWriter.WriteString(string.Empty);
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteElementString("MONTH", endDate.Month.ToString());
                            xmlWriter.WriteElementString("YEAR", endDate.Year.ToString());
                            xmlWriter.WriteElementString("DOC_DATE", endDate.ToString("dd.MM.yyyy"));
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteStartElement("ORGLOGOID");
                        xmlWriter.WriteString(string.Empty);
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteStartElement("DEFNFLDSLIST");
                        xmlWriter.WriteString(string.Empty);
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteElementString("DOC_DATE", DateTime.Today.ToString("dd.MM.yyyy"));
                        writeEBookData(shortChar, journalCode, apDate, journalDate, character, legalNumberId, isBank, isCash, groupID, xmlWriter);
                        xmlWriter.WriteElementString("CROSS_FLAG", "0");
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                        xmlWriter.Close();
                    }
                }
                return true;
            }
            catch (Exception exp)
            {
                MailHelper.sendErrorAlert(exp, "michael lau");
                return false;
            }
        }

        private bool isBankPaymentCOA(string coa)
        {
            return coa == "1241014" || coa == "1241030" || coa == "1241040" || coa == "1241080" || coa == "1251082" || coa == "1281103" || coa == "1231042" || coa == "1231125" || coa == "1231039" || coa == "1231082";

        }

        private bool isCashPaymentCOA(string coa)
        {
            return coa == "1293010" || coa == "1293030" || coa == "1293040" || coa == "1293080" || coa == "1293990";
        }


        private void writeEBookData(string shortChar01, string journalCode, DateTime apDate, DateTime journalDate, string character05, string legalNumberId, string isBank, string isCash, string groupId, XmlWriter w)
        {
            if (journalCode == "PJ" && shortChar01 != string.Empty)
            {
                w.WriteElementString("EBOOK_DOCNR", shortChar01);
                w.WriteElementString("EBOOK_DOCDATE", apDate.ToString("dd.MM.yyyy"));
                w.WriteElementString("EBOOK_DOCTYPE", "2");
                w.WriteElementString("EBOOK_NOPAY", "1");
                return;
            }
            if (character05 != "Y" && journalCode != "CD")
            {
                w.WriteElementString("EBOOK_NODOCUMENT", "1");
                w.WriteElementString("EBOOK_DOCTYPE", "99");
                return;
            }
            if (legalNumberId.IndexOf("TEMSCLAIMS") != -1 && isBank != "Y" && isCash != "Y")
            {
                w.WriteElementString("EBOOK_DOCNR", groupId);
                w.WriteElementString("EBOOK_NOPAY", "1");
            }
            else
            {
                w.WriteElementString("EBOOK_DOCNR", groupId);
                if (isBank == "Y")
                {
                    w.WriteElementString("EBOOK_PAYTYPE", "BANKA");
                }
                else if (isCash == "Y")
                {
                    w.WriteElementString("EBOOK_PAYTYPE", "NAKIT");
                }
                else
                {
                    w.WriteElementString("EBOOK_NOPAY", "1");
                }
            }
            w.WriteElementString("EBOOK_DOCDATE", journalDate.ToString("dd.MM.yyyy"));
            w.WriteElementString("EBOOK_DOCTYPE", "6");
            if (legalNumberId.IndexOf("TEMSCLAIMS") != -1)
            {
                w.WriteElementString("EBOOK_EXPLAIN", "Masraf Formu");
            }
            else
            {
                w.WriteElementString("EBOOK_EXPLAIN", "Muhasebe Fişi");
            }
        }

        private void writeGroupId(EpicorGLJournalDetailDef def, XmlWriter w, bool isDebit)
        {
            if (def.LegalNumberID.ToUpper().IndexOf("TEMSCLAIMS") != -1 && def.OriginalDescription.Trim() != string.Empty)
            {
                w.WriteElementString("EBOOK_DOCNR", def.OriginalDescription.Split(' ')[0]);
            }
            else
            {
                w.WriteElementString("EBOOK_DOCNR", def.GroupID.Trim());
            }
        }

        private void exportUTQACommissionNote(ArrayList qaDCNoteList, ArrayList qaDCNoteShipmentList, int officeId, int fiscalYear, int period, int userId, bool isDraft, string filetype)
        {
            int shipmentstart = 0;
            foreach (QACommissionDNDef qaDN in qaDCNoteList)
            {
                ArrayList qaDCNoteVendor = new ArrayList();
                qaDCNoteVendor.Add(qaDN);
                ArrayList qaDCNoteShipmentListVendor = new ArrayList();
                int vendorpre = 0;
                for (int i = shipmentstart; i < qaDCNoteShipmentList.Count; i++)
                {
                    QACommissionDNShipmentDef qaShipment = (QACommissionDNShipmentDef)qaDCNoteShipmentList[i];
                    int vendortemp = qaShipment.VendorId;
                    shipmentstart = i;
                    if (vendorpre != 0 && vendorpre != vendortemp)
                    {
                        vendorpre = 0;
                        break;
                    }
                    else
                    {
                        qaDCNoteShipmentListVendor.Add(qaShipment);
                        vendorpre = qaShipment.VendorId;
                    }
                }
                string vendorName = VendorWorker.Instance.getVendorByKey(qaDN.VendorId).Name;
                string outputFilePath = string.Empty;
                UTQADebitNoteReport rptQA = null;

                if (filetype == "pdf")
                {
                    outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + "-" + qaDN.DNNo + ".pdf";
                    rptQA = AccountReportManager.Instance.getQACommissiomDebitNote(qaDCNoteVendor, qaDCNoteShipmentListVendor, isDraft, fiscalYear, period);
                    rptQA.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                    rptQA.Close();
                    rptQA.Dispose();
                }
                else
                {
                    outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + OfficeId.getStatus(officeId).Name.ToUpper() + "-" + fiscalYear.ToString() + "-" + period.ToString().PadLeft(2, '0') + "-" + qaDN.DNNo + ".xls";
                    rptQA = AccountReportManager.Instance.getQACommissiomDebitNote(qaDCNoteVendor, qaDCNoteShipmentListVendor, isDraft, fiscalYear, period);
                    rptQA.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelRecord, outputFilePath);
                    rptQA.Close();
                    rptQA.Dispose();
                }

                NoticeHelper.sendQACommissionDebitNote(officeId, fiscalYear, period, userId, isDraft, outputFilePath, qaDN.VendorId);
            }
        }

        public enum Action
        {
            GetInvoiceList,
            GetInvoiceListForInvoiceBatch,
            GetInvoiceListByLcBillRefNo,
            RegisterInvoiceForPayment,
            UpdateARAP,
            UpdateARAPDN,
            CreateEInvoiceBatch,
            DeleteFromEInvoiceBatch,
            RegisterAR,
            UpdateBankReconciliationRequest,
            GetBankReconciliationRequestList,
            ConvertPaymentFile,
            GeneratePaymentAdvice,
            GetFileGenerateRequestList,
            GetInvoiceListByInvoiceNo,
            GenerateSunInterface,
            GenerateNTSunInterface,
            GenerateSunInterfaceBatch,
            PrintQADebitNote,
            GenerateMockShopDebitNote,
            GenerateStudioDebitNote,
            GenerateUTDebitNote,
            UpdateClaim,
            UpdateClaimDetail,
            UploadClaimDocument,
            GetPaymentStatusEnquiryList,
            UpdateDMSWorkflowStatusId,
            HoldPayment,
            MarkDocumentAsReviewed,
            GetUKClaimList,
            GetOSUKClaimList,
            GetOSUKDiscountClaimList,
            GetMFRNQtyAnalysisList,
            GetUKClaimPhasingList,
            GetUKClaimByKey,
            GetUKClaimByGuid,
            DeleteUKClaim,
            CreateUKClaim,
            UpdateUKClaim,
            ReviewUKClaimDN,
            getUKClaimRefundByKey,
            UpdateUKClaimAndRefundList,
            UpdateUKClaimRefundDef,
            DeleteUKClaimRefundDef,
            MailUKClaimDN,
            GetSupplierProductByItemNo,
            UpdateNTInvoice,
            UpdateNTVendor,
            GetNTInvoiceList,
            GetNTVendorList,
            GetNTInvoiceDetailListForRechargeDCNote,
            GetUKClaimDCNoteByDCNoteNo,
            GetCurrentNTMonthEndStatusList,
            UpdateNTMonthEndStatus,
            updateNTInvoiceSettlement,
            GetNTRoleByKey,
            DeleteNTUserRoleAccess,
            UpdateNTUserRoleAccess,
            MarkDMSComplete,
            GetAdvancePaymentByCriteria,
            UpdateAdvancePaymentAndInstalment,
            UpdateAdvancePaymentAndOrderDetail,
            DeleteAdvancePaymentOrderDetail,
            GetTurkeyEpicorGLJournalDetailList,
            VoidDebitNote,
            GetLGHoldPaymentList,
            GenerateILSDiffDCNote,
            GenerateAdvPaymentInstalmentInterestCharges,
            GenerateOtherChargesDCNote,
            GenerateThirdPartyCustomerDNToUK,
            GetUKDiscountClaimList,
            GetUKDiscountClaimByKey,
            DeleteUKDiscountClaim,
            CreateUKDiscountClaim,
            UpdateUKDiscountClaim,
            getUKDiscountClaimRefundByKey,
            UpdateUKDiscountClaimRefundDef,
            DeleteUKDiscountClaimRefundDef,
            SendAlertToPic
        }

        public enum Param
        {
            invoicePrefix,
            invoiceSeqFrom,
            invoiceSeqTo,
            invoiceSeq,
            invoiceYear,
            sequenceNo,
            invoiceDateFrom,
            invoiceDateTo,
            invoiceUploadDateFrom,
            invoiceUploadDateTo,
            officeId,
            officeList,
            tradingAgencyId,
            termOfPurchaseId,
            purchaseScanDateFrom,
            purchaseScanDateTo,
            purchaseScanStatus,
            orderType,
            currencyId,
            salesScanDateFrom,
            salesScanDateTo,
            submittedOnFrom,
            submittedOnTo,
            batchNo,
            invoiceBatchStatus,
            workflowStatusId,
            bankReconRequest,
            bankReconList,
            convertPaymentFileRequest,
            genPaymentAdviceRequest,
            requestList,
            shipmentIdList,
            result,
            invoiceList,
            sunInterfaceQueue,
            ntSunInterfaceQueue,
            fiscalYear,
            period,
            debitNoteDate,
            isDraft,
            format,
            claim,
            claimBIADiscrepancy,
            claimDetails,
            file,
            subDocDateFrom,
            subDocDateTo,
            interfaceDateFrom,
            interfaceDateTo,
            paymentTermId,
            vendorId,
            paymentStatusList,
            paymentStatusEnquiryList,
            dmsWorkflowStatusId,
            rejectPaymentReasonId,
            contractNo,
            isPaymentHold,
            shipmentId,
            otherCostSunInterfaceTypeList,
            ukClaimList,
            ukClaimIdList,
            debitNoteDateFrom,
            debitNoteDateTo,
            debitNoteReceivedDateFrom,
            debitNoteReceivedDateTo,
            ukDebitNoteNo,
            itemNo,
            ukClaimTypeId,
            claimId,
            claimRequestList,
            docId,
            fileName,
            action,
            guid,
            SendNotification,
            toClaimStatusId,
            docType,
            claimRefund,
            claimRefundId,
            claimRefundList,
            isReviewed,
            supplierProductList,
            cutoffDate,
            ukClaimPhasingList,
            reportGroupType,
            workflowStatusList,
            gbpRate,
            eurRate,
            gbpAccrualAmt,
            eurAccrualAmt,
            usdAccrualAmt,
            rptOption,
            ncOptionId,
            userId,
            handlingOfficeId,
            handlingOfficeName,
            ntInvoice,
            ntInvoiceDetailList,
            ntRechargeDetailList,
            ntVendor,
            ntVendorId,
            expenseTypeId,
            dueDateFrom,
            dueDateTo,
            settlementDateFrom,
            settlementDateTo,
            approverId,
            invoiceNoFrom,
            invoiceNoTo,
            customerNoFrom,
            customerNoTo,
            nslRefNoFrom,
            nslRefNoTo,
            expenseTypeList,
            ntVendorList,
            mfrnQtyAnalysisList,
            periodFrom,
            periodTo,
            dcNoteNo,
            dcNote,
            invoiceDebitNoteList,
            ntMonthEndStatusList,
            ntMonthEndStatus,
            ntInvoiceList,
            paymentMethodId,
            paymentDateFrom,
            paymentDateTo,
            departmentId,
            includePayByHK,
            dcIndicator,
            lcBillRefNo,
            companyId,
            ntRole,
            ntRoleId,
            ntUserRoleAccess,
            instalments,
            paymenthistory,
            advancePaymentList,
            paymentNo,
            paymentId,
            advancePayment,
            advancePaymentOrderDetailList,
            advancePaymentDocAction,
            advancePaymentSettlementStatus,
            advancePaymentBalanceSettlement,
            journal,
            voidType,
            returnRowCount,
            submittedBy,
            lgHoldPaymentList,
            periodDays,
            remainingTotal,
            interestRate,
            interestCharges,
            isDebit,
            lgNo,
            genericDCNoteList,
            supplierCode,
            supplierIdList,
            autoUpload,
            nextDNNo,
            appliedUKDiscount,
            remark
        }

    }
}
