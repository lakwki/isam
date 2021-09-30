using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.shipping;
using com.next.isam.domain.common;
using com.next.isam.domain.ils;
using com.next.isam.domain.order;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.isam.domain.product;
using com.next.isam.reporter.accounts;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.infra.persistency.transactions;
using com.next.infra.persistency.dataaccess;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.reporter.dataserver;
using com.next.infra.util;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using com.next.common.domain.industry.vendor;


namespace com.next.isam.appserver.account
{
    public class NonTradeManager
    {
        private static NonTradeManager _instance;

        private NonTradeWorker worker;
        private AccountWorker accountWorker;
        private GeneralWorker generalWorker;
        private CommonWorker commonWorker;

        public NonTradeManager()
        {
            worker = NonTradeWorker.Instance;
            accountWorker = AccountWorker.Instance;
            generalWorker = GeneralWorker.Instance;
            commonWorker = CommonWorker.Instance;
        }

        public static NonTradeManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NonTradeManager();
                return _instance;
            }
        }

        public ArrayList getNTOfficeList(int userId)
        {
            ArrayList officeList = null;

            if (userId == -1)
                officeList = generalWorker.getOfficeRefListByCriteria(-1, -1, GeneralStatus.ACTIVE.Code);
            else
                officeList = generalWorker.getOfficeListByUserId(userId, OfficeStructureType.PRODUCTCODE.Type);
            //officeList = getNTUserOfficeList(userId, GeneralCriteria.ALL, GeneralCriteria.ALL);

            ArrayList userOfficeList = new ArrayList();

            foreach (OfficeRef office in officeList)
            {
                if (office.OfficeCode == "FB" || office.OfficeCode == "PH" || office.OfficeCode == "MA")
                    continue;

                userOfficeList.Add(office);
            }

            if (userOfficeList.Count == 0)
            {
                UserRef user = generalWorker.getUserByKey(userId);
                userOfficeList.Add(user.Department.Office);
            }

            return userOfficeList;
        }

        public ArrayList getNTRechargeableOfficeList(int userId)
        {
            ArrayList officeList = null;

            if (userId == -1)
                officeList = generalWorker.getOfficeRefListByCriteria(-1, -1, GeneralStatus.ACTIVE.Code);
            else
                officeList = generalWorker.getOfficeListByUserId(userId, OfficeStructureType.PRODUCTCODE.Type);
            //officeList = getNTUserOfficeList(userId, GeneralCriteria.ALL, GeneralCriteria.ALL);

            ArrayList userOfficeList = new ArrayList();

            foreach (OfficeRef office in officeList)
            {
                //if (office.OfficeCode == "FB" || office.OfficeCode == "PH" || office.OfficeCode == "MA" || office.OfficeCode == "TH")
                if (office.OfficeCode == "FB" || office.OfficeCode == "PH" || office.OfficeCode == "MA")
                    continue;

                userOfficeList.Add(office);
            }

            if (userOfficeList.Count == 0)
            {
                UserRef user = generalWorker.getUserByKey(userId);
                userOfficeList.Add(user.Department.Office);
            }

            return userOfficeList;
        }


        public ArrayList getNTRoleList()
        {
            return worker.getNTRoleList();
        }

        public NTRoleRef getNTRoleByKey(int roleId)
        {
            return worker.getNTRoleByKey(roleId);
        }

        public ArrayList getNTActionHistoryList(int invoiceId, int ntVendorId)
        {
            return worker.getNTActionHistoryList(invoiceId, ntVendorId);
        }

        public NTExpenseTypeRef getNTExpenseTypeByKey(int NTExpenseTypeId)
        {
            return worker.getNTExpenseTypeByKey(NTExpenseTypeId);
        }

        public ArrayList getNTExpenseTypeList()
        {
            return worker.getNTExpenseTypeList();
        }

        public ArrayList getNTExpenseTypeListByOfficeId(int officeId)
        {
            return worker.getNTExpenseTypeByOfficeId(officeId);
        }

        public ArrayList getNTExpenseTypeByNTVendorId(int NTVendorId)
        {
            ArrayList list = worker.getNTExpenseTypeByNTVendorId(NTVendorId);

            if (list.Count == 0)
            {
                list = worker.getNTExpenseTypeList();
            }
            return list;
        }

        public NTVendorDef getNTVendorByKey(int vendorId)
        {
            return worker.getNTVendorByKey(vendorId);
        }

        public NTVendorDef getNTVendorByEPVendorCode(string code, bool enforceEmail)
        {
            return worker.getNTVendorByEPVendorCode(code, enforceEmail);
        }

        public void updateNTActionHistory(int invoiceId, int ntVendorId, string description, int userId)
        {
            worker.updateNTActionHistory(invoiceId, ntVendorId, description, userId);
        }

        public void updateNTActionHistory(NTActionHistoryDef def)
        {
            worker.updateNTActionHistory(def);
        }

        public void updateNTUserRoleAccess(NTUserRoleAccessDef def, int userId)
        {
            worker.updateNTUserRoleAccess(def, userId);
        }

        public DomainNTInvoiceDef getDomainNTInvoiceDef(int invoiceId)
        {
            DomainNTInvoiceDef domainDef = new DomainNTInvoiceDef();

            domainDef.NTInvoice = worker.getNTInvoiceByKey(invoiceId);
            domainDef.NTInvoiceDetailList = worker.getNTInvoiceDetailListByInvoiceIdAndType(invoiceId, TypeCollector.createNew(NTInvoiceDetailType.COSTCENTER.Id));
            domainDef.ActionHistoryList = worker.getNTActionHistoryList(invoiceId, -1);

            TypeCollector rechargeTypeList = TypeCollector.Inclusive;
            ArrayList rechargeType = NTInvoiceDetailType.getRechargeTypeCollectionValues();
            foreach (NTInvoiceDetailType type in rechargeType)
            {
                rechargeTypeList.append(type.Id);
            }
            domainDef.NTRechargeDetailList = worker.getNTInvoiceDetailListByInvoiceIdAndType(invoiceId, rechargeTypeList);
            return domainDef;
        }

        public ArrayList getNTVendorByName(string name, int officeId, int workflowStatusId)
        {
            return worker.getNTVendorByName(name, officeId, workflowStatusId);
        }

        public ArrayList getNTVendorList(int officeId, int ntVendorId, int expenseTypeId, int workflowStatusId)
        {
            return worker.getNTVendorList(officeId, ntVendorId, expenseTypeId, workflowStatusId);
        }

        public ArrayList getNTVendorExpenseTypeMappingByNTVendorId(int ntVendorId)
        {
            return worker.getNTVendorExpenseTypeMappingByNTVendorId(ntVendorId);
        }

        public ArrayList getNTInvoiceListByVendorId(int vendorId)
        {
            return worker.getNTInvoiceListByVendorId(vendorId); ;
        }

        public ArrayList getNTInvoiceList(TypeCollector officeList, int expenseTypeId, int fiscalYear, int fiscalPeriodFrom, int fiscalPeriodTo, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo,
                DateTime settlementDateFrom, DateTime settlementDateTo, string invoiceNoFrom, string invoiceNoTo, string customerNoFrom, string customerNoTo, string nslRefNoFrom, string nslRefNoTo, int vendorId, TypeCollector workflowStatusIdList,
                DateTime paymentDateFrom, DateTime paymentDateTo, int currencyId, int paymentMethodId, int departmentId, int userId, int includePayByHK, string dcIndicator, int firstApproverId, int submittedBy)
        {
            return worker.getNTInvoiceList(officeList, expenseTypeId, fiscalYear, fiscalPeriodFrom, fiscalPeriodTo, invoiceDateFrom, invoiceDateTo, dueDateFrom, dueDateTo, settlementDateFrom, settlementDateTo, invoiceNoFrom, invoiceNoTo, customerNoFrom, customerNoTo, nslRefNoFrom, nslRefNoTo, vendorId, workflowStatusIdList, paymentDateFrom, paymentDateTo, currencyId, paymentMethodId, departmentId, userId, includePayByHK, dcIndicator, firstApproverId, submittedBy);
        }

        public ReportClass GenNTInvoiceDetailExportList(TypeCollector invoiceIdList, TypeCollector officeList, int expenseTypeId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo,
                string invoiceNoFrom, string invoiceNoTo, string customerNoFrom, string customerNoTo, string nslRefNoFrom, string nslRefNoTo, int vendorId, TypeCollector workflowStatusIdList,
                int currencyId, int paymentMethodId, string dcIndicator)
        {
            NTInvoiceSettlementDs dataset = null;
            dataset = reporter.dataserver.ReporterWorker.Instance.GetNTInvoiceDetailForSettlement(invoiceIdList);
            if (dataset != null)
            {
                ReportClass rpt = new NTInvoiceDetailExportList();
                rpt.SetDataSource(dataset);
                rpt.SetParameterValue("ReportType", "SEARCH");
                string offices = string.Empty;
                foreach (int officeId in officeList.Values)
                    offices += (offices == string.Empty ? string.Empty : ", ") + OfficeId.getName(officeId);
                rpt.SetParameterValue("Office", offices);
                rpt.SetParameterValue("SupplierInvoiceAccountNo", string.Empty);
                rpt.SetParameterValue("DueDate", (dueDateFrom == DateTime.MinValue ? string.Empty : dueDateFrom.ToString("dd/MM/yyyy")) + (dueDateTo == DateTime.MinValue ? string.Empty : " - " + dueDateTo.ToString("dd/MM/yyyy")));
                rpt.SetParameterValue("Vendor", (vendorId > 0 ? NonTradeManager.Instance.getNTVendorByKey(vendorId).VendorName : string.Empty));
                rpt.SetParameterValue("PaymentMethod", string.Empty);
                rpt.SetParameterValue("Currency", string.Empty);
                rpt.SetParameterValue("InvoiceDate", (invoiceDateFrom == DateTime.MinValue ? string.Empty : invoiceDateFrom.ToString("dd/MM/yyyy")) + (invoiceDateTo == DateTime.MinValue ? string.Empty : " - " + invoiceDateTo.ToString("dd/MM/yyyy")));
                rpt.SetParameterValue("InvoiceNo", (invoiceNoFrom == string.Empty ? string.Empty : invoiceNoFrom) + (invoiceNoTo == string.Empty ? string.Empty : " - " + invoiceNoTo));
                rpt.SetParameterValue("CustomerNo", (customerNoFrom == string.Empty ? string.Empty : customerNoFrom) + (customerNoTo == string.Empty ? string.Empty : " - " + customerNoTo));
                rpt.SetParameterValue("NSLRefNo", (nslRefNoFrom == string.Empty ? string.Empty : nslRefNoFrom) + (nslRefNoTo == string.Empty ? string.Empty : " - " + nslRefNoTo));
                if (expenseTypeId == GeneralCriteria.ALL)
                    rpt.SetParameterValue("ExpenseType", "ALL");
                else
                    rpt.SetParameterValue("ExpenseType", getNTExpenseTypeByKey(expenseTypeId).Description);

                string status = string.Empty;
                foreach (int wfs in workflowStatusIdList.Values)
                    status += (status == string.Empty ? string.Empty : ",") + NTWFS.getType(wfs).Name;
                rpt.SetParameterValue("Status", status);
                rpt.SetParameterValue("DocumentType", (dcIndicator == "D" ? "Invoice" : (dcIndicator == "C" ? "Credit Note" : string.Empty)));
                return rpt;
            }
            else
                return null;
        }

        /*
        public ReportClass GenNTInvoiceDetailForSettlement(TypeCollector invoiceIdList, TypeCollector officeList, int expenseTypeId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo,
                string invoiceNoFrom, string invoiceNoTo, string customerNoFrom, string customerNoTo, string nslRefNoFrom, string nslRefNoTo, int vendorId, TypeCollector workflowStatusIdList,
                int currencyId, int paymentMethodId, DateTime paymentDateFrom, DateTime paymentDateTo, string accountNoFrom, string accountNoTo, string dcIndicator)
        {
            NTInvoiceSettlementDs dataset = null;
            dataset = reporter.dataserver.ReporterWorker.Instance.GetNTInvoiceDetailForSettlement(invoiceIdList);
            if (dataset != null)
            {
                ReportClass rpt = new NTInvoiceSettlementDetailList();
                rpt.SetDataSource(dataset);
                rpt.SetParameterValue("ReportType", "SETTLEMENT");
                string offices = string.Empty;
                foreach (int officeId in officeList.Values)
                    offices += (offices == string.Empty ? string.Empty : ", ") + OfficeId.getName(officeId);
                rpt.SetParameterValue("Office", offices);
                rpt.SetParameterValue("SupplierInvoiceAccountNo", accountNoFrom + (string.IsNullOrEmpty(accountNoTo) ? string.Empty : " - " + accountNoTo));
                rpt.SetParameterValue("DueDate", (dueDateFrom == DateTime.MinValue ? string.Empty : dueDateFrom.ToString("dd/MM/yyyy")) + (dueDateTo == DateTime.MinValue ? string.Empty : " - " +dueDateTo.ToString("dd/MM/yyyy")));
                rpt.SetParameterValue("Vendor", (vendorId > 0 ? NonTradeManager.Instance.getNTVendorByKey(vendorId).VendorName : string.Empty));
                rpt.SetParameterValue("PaymentMethod",  (paymentMethodId==-1?"ALL":NTPaymentMethodRef.getType(paymentMethodId).Name));
                rpt.SetParameterValue("Currency", (currencyId == GeneralCriteria.ALL ? "ALL" : CurrencyId.getName(currencyId)));
                rpt.SetParameterValue("InvoiceDate", (invoiceDateFrom == DateTime.MinValue ? string.Empty : invoiceDateFrom.ToString("dd/MM/yyyy") + " - " + invoiceDateTo.ToString("dd/MM/yyyy")));
                rpt.SetParameterValue("InvoiceNo", string.Empty);
                rpt.SetParameterValue("CustomerNo", string.Empty);
                rpt.SetParameterValue("NSLRefNo", string.Empty);
                rpt.SetParameterValue("ExpenseType", string.Empty);
                string status = string.Empty;
                foreach (int wfs in workflowStatusIdList.Values)
                    status += (status == string.Empty ? string.Empty : ",") + NTWFS.getType(wfs).Name;
                rpt.SetParameterValue("Status", status);
                rpt.SetParameterValue("DocumentType", (dcIndicator == string.Empty ? "ALL" : (dcIndicator == "D" ? "Invoice" : (dcIndicator == "C" ? "Credit Note" : string.Empty))));
                return rpt;
            }
            else
                return null;
        }
        */

        public NTInvoiceDef getNTInvoiceByKey(int invoiceId)
        {
            return NonTradeWorker.Instance.getNTInvoiceByKey(invoiceId);
        }

        public NTInvoiceDef getNTInvoiceByProcurementRequestId(int procurementRequestId)
        {
            return NonTradeWorker.Instance.getNTInvoiceByProcurementRequestId(procurementRequestId);
        }

        public bool isNTInvoiceDuplicated(int invoiceId, string invoiceNo, string customerNo, DateTime invoiceDate, string epicorSupplierId)
        {
            return worker.isNTInvoiceDuplicated(invoiceId, invoiceNo, customerNo, invoiceDate, epicorSupplierId);
        }

        public ArrayList getNTInvoiceDetailListForRechargeDCNote(int officeId, int companyId, ref ArrayList invoiceList)
        {
            return worker.getNTInvoiceDetailForRechargeDCNote(officeId, companyId, ref invoiceList);
        }

        public bool isNTInvoiceHasDebitNote(int invoiceId)
        {
            return worker.isNTInvoiceHasDebitNote(invoiceId);
        }

        public bool isNTInvoiceHasFixedAsset(int invoiceId)
        {
            return worker.isNTInvoiceHasFixedAsset(invoiceId);
        }

        public void sendNTInvoicePendingForApprovalNotification()
        {
            ArrayList invoiceList = worker.getPendingForApprovalInvoiceList(DateTime.Today.AddDays(-1));
            ArrayList invoiceApprovalList = new ArrayList();
            int approverId = 0;
            UserRef approvalUser;

            foreach (NTInvoiceDef invoiceDef in invoiceList)
            {
                if (approverId != 0 && approverId != invoiceDef.Approver.UserId)
                {
                    //send notification 
                    approvalUser = generalWorker.getUserByKey(approverId);
                    NoticeHelper.sendNTInvoicePendingForApprovalNotification(invoiceApprovalList, approvalUser);

                    approverId = invoiceDef.Approver.UserId;
                    invoiceApprovalList = new ArrayList();
                    invoiceApprovalList.Add(invoiceDef);
                }
                else
                {
                    approverId = invoiceDef.Approver.UserId;
                    invoiceApprovalList.Add(invoiceDef);
                }
            }

            if (approverId != 0)
            {
                approvalUser = generalWorker.getUserByKey(approverId);
                NoticeHelper.sendNTInvoicePendingForApprovalNotification(invoiceApprovalList, approvalUser);
            }
        }

        public void sendNTInvoiceRejectNotification(int invoiceId, int rejectedBy)
        {
            NTInvoiceDef invoiceDef = worker.getNTInvoiceByKey(invoiceId);

            NoticeHelper.sendNTInvoiceRejectNotification(invoiceDef, rejectedBy);
        }

        public void updateNTInvoice(NTInvoiceDef invoiceDef, ArrayList invoiceDetailList, ArrayList rechargeList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList = new ArrayList();

                if (invoiceDef.NTVendor.NTVendorId == 0)
                {
                    worker.updateNTVendor(invoiceDef.NTVendor, amendmentList, userId);
                    NoticeHelper.sendNewNTVendorNotification(invoiceDef.NTVendor, userId);
                }

                worker.updateNTInvoice(invoiceDef, amendmentList, userId);

                if (invoiceDetailList != null)
                {
                    foreach (NTInvoiceDetailDef detailDef in invoiceDetailList)
                    {
                        if (detailDef.InvoiceDetailId == 0)
                            detailDef.InvoiceId = invoiceDef.InvoiceId;

                        worker.updateNTInvoiceDetail(detailDef, amendmentList, userId);
                    }
                }

                if (rechargeList != null)
                {
                    foreach (NTInvoiceDetailDef rechargeDef in rechargeList)
                    {
                        if (rechargeDef.InvoiceDetailId == 0)
                            rechargeDef.InvoiceId = invoiceDef.InvoiceId;

                        worker.updateNTInvoiceDetail(rechargeDef, amendmentList, userId);
                    }
                }

                if (amendmentList.Count > 0)
                {
                    foreach (NTActionHistoryDef historyDef in amendmentList)
                    {
                        worker.updateNTActionHistory(historyDef);
                    }
                }

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public string getNewEpicorVendorCode(string vendorCodePrefix, string vendorType)
        {
            int seq = worker.getEpicorVendorCodeSequenceNo(vendorType);
            return vendorCodePrefix.ToUpper() + seq.ToString("00000.") + vendorType.ToUpper();
        }

        public void updateNTVendor(NTVendorDef ntVendorDef, ArrayList expenseTypeMappingList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList = new ArrayList();

                bool sendNotification = false;

                if (ntVendorDef.NTVendorId == 0 && ntVendorDef.WorkflowStatus.Id == NTVendorWFS.PENDING.Id)
                    sendNotification = true;
                else if (ntVendorDef.NTVendorId != 0)
                {
                    NTVendorDef v_original = worker.getNTVendorByKey(ntVendorDef.NTVendorId);
                    if (ntVendorDef.WorkflowStatus.Id != v_original.WorkflowStatus.Id && (ntVendorDef.WorkflowStatus.Id == NTVendorWFS.PENDING.Id || ntVendorDef.WorkflowStatus.Id == NTVendorWFS.APPROVED.Id || ntVendorDef.WorkflowStatus.Id == NTVendorWFS.REJECTED.Id || ntVendorDef.WorkflowStatus.Id == NTVendorWFS.CANCELLED.Id))
                        sendNotification = true;
                }

                worker.updateNTVendor(ntVendorDef, amendmentList, userId);

                if (expenseTypeMappingList != null)
                {
                    foreach (NTVendorExpenseTypeMappingDef mappingDef in expenseTypeMappingList)
                    {
                        OfficeRef office = GeneralWorker.Instance.getOfficeRefByKey(mappingDef.ExpenseType.OfficeId);
                        if (mappingDef.NTVendorId == 0 || mappingDef.Status != 0)
                        {
                            mappingDef.NTVendorId = ntVendorDef.NTVendorId;
                            amendmentList.Add(new NTActionHistoryDef(-1, mappingDef.NTVendorId, "Add Expense Type Mapping: " + mappingDef.ExpenseType.ExpenseType + " (" + office.OfficeCode + ")", userId));
                        }
                        else if (mappingDef.Status == 0)
                        {
                            amendmentList.Add(new NTActionHistoryDef(-1, mappingDef.NTVendorId, "Remove Expense Type Mapping: " + mappingDef.ExpenseType.ExpenseType + " (" + office.OfficeCode + ")", userId));
                        }
                        worker.updateNTVendorExpenseTypeMapping(mappingDef, userId);
                    }
                }

                if (amendmentList.Count > 0)
                {
                    foreach (NTActionHistoryDef historyDef in amendmentList)
                    {
                        worker.updateNTActionHistory(historyDef);
                    }
                }

                ctx.VoteCommit();

                //if (isNew)
                //    NoticeHelper.sendNewNTVendorNotification(ntVendorDef, userId);
                if (sendNotification)
                {
                    if (ntVendorDef.WorkflowStatus.Id == NTVendorWFS.PENDING.Id)
                        NoticeHelper.sendNewNTVendorNotification(ntVendorDef, userId);
                    else if (ntVendorDef.WorkflowStatus.Id == NTVendorWFS.APPROVED.Id)
                        NoticeHelper.sendNTVendorApprovalNotification(ntVendorDef, userId);
                    else if (ntVendorDef.WorkflowStatus.Id == NTVendorWFS.REJECTED.Id)
                        NoticeHelper.sendNTVendorRejectNotification(ntVendorDef, userId);
                    else if (ntVendorDef.WorkflowStatus.Id == NTVendorWFS.CANCELLED.Id)
                        NoticeHelper.sendNTVendorCancelledNotification(ntVendorDef, userId);
                }

            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        public void sendNTVendorAmendmentRequest(NTVendorDef def, string description, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.AmendmentDetail = description;
                updateNTVendor(def, null, userId);
                NTActionHistoryDef historyDef = new NTActionHistoryDef(-1, def.NTVendorId, "Amendment Request: " + description, userId);
                NonTradeManager.Instance.updateNTActionHistory(historyDef);

                NoticeHelper.sendNTVendorAmendmentRequest(def, description, userId);

            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        public ArrayList getCurrentNTMonthEndStatus(ArrayList officeList)
        {
            TypeCollector officeIdList = TypeCollector.Inclusive;

            foreach (OfficeRef office in officeList)
            {
                officeIdList.append(office.OfficeId);
            }

            return worker.getCurrentMonthEndStatusList(officeIdList);
        }

        public void updateMonthEndStatus(NTMonthEndStatusDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                worker.updateNTMonthEndStatus(def, userId);
                NTActionHistoryDef historyDef = new NTActionHistoryDef(-1, -1, string.Format("{0} YEAR {1} Period {2} {3}", def.Office.OfficeCode, def.FiscalYear, def.Period, def.StatusDescription), userId);
                worker.updateNTActionHistory(historyDef);

                //start the next period
                if (def.Status == NTMonthEndStatusDef.CLOSED)
                {
                    NTMonthEndStatusDef nextPeriodStatusDef;
                    if (def.Period < 12)
                        nextPeriodStatusDef = worker.getNTMonthEndStatusByOfficeIdAndPeriod(def.Office.OfficeId, def.FiscalYear, def.Period + 1);
                    else
                        nextPeriodStatusDef = worker.getNTMonthEndStatusByOfficeIdAndPeriod(def.Office.OfficeId, def.FiscalYear + 1, 1);

                    nextPeriodStatusDef.Status = NTMonthEndStatusDef.OPEN;
                    worker.updateNTMonthEndStatus(nextPeriodStatusDef, userId);

                    historyDef = new NTActionHistoryDef(-1, -1, string.Format("{0} YEAR {1} Period {2} {3}", nextPeriodStatusDef.Office.OfficeCode, nextPeriodStatusDef.FiscalYear, nextPeriodStatusDef.Period, nextPeriodStatusDef.StatusDescription), userId);
                    worker.updateNTActionHistory(historyDef);
                }

                ctx.VoteCommit();

            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        #region NT Recharge DC Note

        public ArrayList getNTRechargeDCNoteList(int reportOfficeGroupId, int dateType, int year, int periodMonth)
        {
            DateTime fromDate;
            DateTime toDate;
            if (dateType == 1)
            {
                fromDate = new DateTime(year, periodMonth, 1);
                toDate = (new DateTime(year, periodMonth, 1)).AddMonths(1);
            }
            else
            {
                AccountFinancialCalenderDef def = generalWorker.getAccountPeriodByYearPeriod(AppId.NSS.Code, year, periodMonth);
                fromDate = def.StartDate;
                toDate = def.EndDate.AddDays(1);
            }

            TypeCollector officeList = TypeCollector.createNew(reportOfficeGroupId);
            ArrayList olist = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(reportOfficeGroupId);
            foreach (OfficeRef oRef in olist)
            {
                officeList.append(oRef.OfficeId);
            }

            return worker.getNTRechargeDCNoteList(officeList, fromDate, toDate);
        }


        public ArrayList getNTRechargeDCNoteInvoiceList(int reportOfficeGroupId, int dateType, int year, int periodMonth)
        {
            DateTime fromDate;
            DateTime toDate;
            if (dateType == 1)
            {
                fromDate = new DateTime(year, periodMonth, 1);
                toDate = (new DateTime(year, periodMonth, 1)).AddMonths(1);
            }
            else
            {
                AccountFinancialCalenderDef def = generalWorker.getAccountPeriodByYearPeriod(AppId.NSS.Code, year, periodMonth);
                fromDate = def.StartDate;
                toDate = def.EndDate.AddDays(1);
            }

            TypeCollector officeList = TypeCollector.createNew(reportOfficeGroupId);
            ArrayList olist = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(reportOfficeGroupId);
            foreach (OfficeRef oRef in olist)
            {
                officeList.append(oRef.OfficeId);
            }

            return worker.getNTRechargeDCNoteInvoiceList(officeList, fromDate, toDate);
        }


        public NTRechargeDCNoteDef getNTRechargeDCNoteByKey(int rechargeDCNoteId)
        {
            return NonTradeWorker.Instance.getNTRechargeDCNoteByKey(rechargeDCNoteId);
        }

        public ArrayList generateZeroAmountNTRechargeDCNoteList(ArrayList ntRechargeDetailList, DateTime dcNoteDate)
        {
            try
            {
                Hashtable ht_dcNote = new Hashtable();
                ArrayList zeroAmountList = new ArrayList();
                Hashtable ht_invoice = new Hashtable();
                NTRechargeDCNoteDef dcNote;

                foreach (NTInvoiceDetailDef detailDef in ntRechargeDetailList)
                {
                    // sum the total recharge amount for each DC Note
                    NTInvoiceDef invoiceDef = worker.getNTInvoiceByKey(detailDef.InvoiceId);
                    string dcNoteNoIndex = worker.getDCNoteNoGroupKey(invoiceDef, detailDef);
                    if (ht_dcNote.ContainsKey(dcNoteNoIndex))
                        dcNote = (NTRechargeDCNoteDef)ht_dcNote[dcNoteNoIndex];
                    else
                    {   // New DC Note
                        dcNote = NonTradeWorker.Instance.createNewDCNoteDef(dcNoteNoIndex, dcNoteDate, detailDef, invoiceDef);
                        ht_dcNote.Add(dcNoteNoIndex, dcNote);
                        ht_invoice.Add(dcNoteNoIndex, new ArrayList());
                    }
                    dcNote.RechargeAmount += worker.calcRechargeAmount(dcNote, detailDef, invoiceDef);
                    ((ArrayList)ht_invoice[dcNoteNoIndex]).Add(invoiceDef);
                }

                foreach (string key in ht_dcNote.Keys)
                {
                    if (((NTRechargeDCNoteDef)ht_dcNote[key]).RechargeAmount == 0)
                    {
                        int i = 0;
                        //string[] lst = new string[((ArrayList)ht_invoice[key]).Count];
                        ArrayList lst = new ArrayList();
                        foreach (NTInvoiceDef inv in (ArrayList)ht_invoice[key])
                            if (!lst.Contains(inv.InvoiceNo))
                                lst.Add(inv.InvoiceNo);
                        zeroAmountList.Add(lst);
                    }
                }
                return zeroAmountList;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
            }

        }

        #endregion

        public void submitNTSunInterfaceRequest(NTSunInterfaceQueueDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeAccrual.GetHashCode())
                    def.SourceId = 1;
                worker.updateNTSunInterfaceQueue(def);

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public ArrayList getRecentNTSunInterfaceQueue()
        {
            return worker.getRecentNTSunInterfaceQueueList();
        }

        public List<NTMonthEndStatusDef> getActiveNTMonthEndStatusList(int officeId, int fiscalYear, int period)
        {
            return worker.getActiveNTMonthEndStatusList(officeId, fiscalYear, period);
        }

        public NTSunInterfaceQueueDef getNTSunInterfaceQueue(int officeId, int fiscalYear, int period, int sunInterfaceTypeId, int categoryId, int sourceId)
        {
            ArrayList list = worker.getNTSunInterfaceQueueList(officeId, fiscalYear, period, sunInterfaceTypeId, categoryId, sourceId);
            if (list.Count == 0)
                return null;
            else
                return (NTSunInterfaceQueueDef)list[0];
        }

        public NTSunInterfaceQueueDef getNTSunInterfaceQueueByKey(int queueId)
        {
            return worker.getNTSunInterfaceQueueByKey(queueId);
        }

        public void processQueues()
        {
            foreach (NTSunInterfaceQueueDef queueDef in worker.getNTSunInterfaceQueueList())
            {
                if (commonWorker.getSystemParameterByKey(220).ParameterValue == "Y" && 1== 0)
                    break;
                generateInterface(queueDef);
            }
        }

        private void generateInterface(NTSunInterfaceQueueDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList list = worker.getInterfaceData(def);
                ArrayList filteredList = new ArrayList();
                NonTradeEpicorInterfaceWorker.Instance.initialize(def.QueueId, def.SunInterfaceTypeId);

                foreach (SunInterfaceLogDef logDef in list)
                {
                    logDef.QueueId = def.QueueId;
                    logDef.CategoryId = def.CategoryType.Id;

                    if (logDef.BaseAmount == 0 && logDef.OtherAmount != 0)
                        NoticeHelper.sendGeneralMessage("Exchange Rate Not Defined While Generating NT Sun Interface", "QueueId: " + def.QueueId.ToString() + ", Amount = 0, Exchange Rate Not Defined?");
                    else
                    {

                        filteredList.Add(logDef);
                        /*
                        accountWorker.updateSunInterfaceLog(logDef);
                        */

                        if (logDef.CategoryId == CategoryType.ACTUAL.Id)
                        {
                            if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeExpenseInvoice.GetHashCode())
                                setNonTradeExpenseInvoiceInterfaceComplete(logDef, def.User.UserId);
                            else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeBankPayment.GetHashCode())
                                setNonTradeBankPaymentInterfaceComplete(logDef, def.User.UserId);
                            else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeRechargeDebitNote.GetHashCode() || logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradePaymentOnBehalfDebitNote.GetHashCode())
                                setNTRechargeDCNoteInterfaceComplete(logDef, def.User.UserId);
                            else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeAccrual.GetHashCode())
                                setNonTradeExpenseAccrualInterfaceComplete(logDef, def.User.UserId);
                        }
                    }
                }

                if (filteredList.Count > 0)
                {
                    ArrayList safList = accountWorker.convertToSAFList(filteredList, def.SunInterfaceTypeId, def.CategoryType.Id, def.Office.OfficeId);

                    if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeExpenseInvoice.GetHashCode())
                    {
                        TypeCollector invoiceIdList = TypeCollector.Inclusive;

                        foreach (SunInterfaceLogDef ld in filteredList)
                        {
                            invoiceIdList.append(int.Parse(ld.ReferenceNo));
                        }

                        foreach (string sOfficeId in NonTradeWorker.Instance.getIntercommRechargeOfficeList(invoiceIdList))
                        {
                            string officeCode = OfficeId.getName(int.Parse(sOfficeId));
                            string emailList = CommonWorker.Instance.getSystemParameterByName(officeCode + "_NT_RECHARGE_INTERFACE_EMAIL").ParameterValue;

                            string outFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");

                            foreach (NTInvoiceDef invoiceDef in NonTradeWorker.Instance.getIntercommRechargeInvoiceList(invoiceIdList, officeCode))
                            {

                                ArrayList queryStructs = new ArrayList();

                                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
                                queryStructs.Add(new QueryStructDef("NSL Invoice No", invoiceDef.NSLInvoiceNo));
                                queryStructs.Add(new QueryStructDef("Document Type", "Non-Trade-Invoice"));

                                ArrayList qList = DMSUtil.queryDocument(queryStructs);

                                DocumentInfoDef docDef = null;
                                foreach (DocumentInfoDef docInfoDef in qList)
                                {
                                    docDef = docInfoDef;
                                    foreach (AttachmentInfoDef attachInfoDef in docDef.AttachmentInfos)
                                    {
                                        string fileName = outFolder + "RECHARGE-" + invoiceDef.NSLInvoiceNo + "-" + attachInfoDef.FileName;
                                        File.WriteAllBytes(fileName, DMSUtil.getAttachment(attachInfoDef.AttachmentID));
                                        NoticeHelper.sendSunInterfaceSupportingMail(emailList, def, ConvertUtility.createArrayList(fileName));
                                        File.Delete(fileName);
                                    }
                                }
                            }

                        }


                    }

                }
                else
                {
                    if (def.SourceId > 0)
                        NoticeHelper.sendEmptySunInterfaceMail(def);
                }

                def.CompleteTime = DateTime.Now;
                def.Status = 1;
                worker.updateNTSunInterfaceQueue(def);

                ArrayList fileList = new ArrayList();
                List<string> files = NonTradeEpicorInterfaceWorker.Instance.GLInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);
                files = NonTradeEpicorInterfaceWorker.Instance.APInvoiceInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);
                files = NonTradeEpicorInterfaceWorker.Instance.ARInvoiceInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);
                files = NonTradeEpicorInterfaceWorker.Instance.PaymentInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);
                files = NonTradeEpicorInterfaceWorker.Instance.ReceiptInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);

                /*
                if (def.Office.OfficeId == OfficeId.TR.Id)
                {
                    files = NonTradeEpicorInterfaceWorker.Instance.GLInterfaceFile.ExportLogoXml(def.User.UserId);
                    fileList.AddRange(files);
                }
                */

                if (fileList.Count > 0)
                    NoticeHelper.sendEpicorInterfaceMail(def, fileList);

                NonTradeEpicorInterfaceWorker.Instance.dispose();

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void setNonTradeExpenseInvoiceInterfaceComplete(SunInterfaceLogDef logDef, int userId)
        {
            int invoiceId = int.Parse(logDef.ReferenceNo);
            NTInvoiceDef ntInvoiceDef = worker.getNTInvoiceByKey(invoiceId);
            AccountFinancialCalenderDef calDef = generalWorker.getAccountPeriodByYearPeriod(AppId.ISAM.Code, logDef.FiscalYear, logDef.Period);

            if (ntInvoiceDef != null)
            {
                /*TODO: TRADINGAF */
                ArrayList detailList = worker.getNTInvoiceDetailListByInvoiceIdAndType(invoiceId, NTInvoiceDetailType.getTypeCollector());
                foreach (NTInvoiceDetailDef dtl in detailList)
                {
                    if (dtl.ExpenseType != null && dtl.ExpenseType.SUNAccountCode == "1452028")
                    {
                        ShipmentDef shipment = OrderSelectWorker.Instance.getShipmentByContractNoAndDeliveryNo(dtl.ContractNo, dtl.DeliveryNo);
                        if (!shipment.IsTradingAirFreight)
                            shipment.IsTradingAirFreight = true;
                        shipment.TradingAirFreightActualCost += (dtl.Amount * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, ntInvoiceDef.Currency.CurrencyId, calDef.StartDate) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, calDef.StartDate));
                        OrderWorker.Instance.updateShipmentList(ConvertUtility.createArrayList(shipment), userId);
                        SynchronizationWorker.Instance.SyncNssShipment(shipment.ShipmentId);
                        NoticeHelper.sendGeneralMessage("Synchronized Trading A/F to NSS  (" + dtl.ContractNo + dtl.DeliveryNo.ToString() + ")");
                    }
                }

                ntInvoiceDef.IsSUNInterfaced = GeneralStatus.ACTIVE.Code;
                ntInvoiceDef.SUNInterfaceDate = DateTime.Today;
                ntInvoiceDef.FiscalYear = logDef.FiscalYear;
                ntInvoiceDef.FiscalPeriod = logDef.Period;
                worker.updateNTInvoice(ntInvoiceDef, new ArrayList(), userId);

                worker.updateNTActionHistory(ntInvoiceDef.InvoiceId, ntInvoiceDef.NTVendor.NTVendorId, "Non-Trade Expense Invoice Interface Generated - " + logDef.FiscalYear.ToString() + "/P" + logDef.Period.ToString() + " (QueueId = " + logDef.QueueId.ToString() + ")", userId);

            }
        }

        private void setNonTradeExpenseAccrualInterfaceComplete(SunInterfaceLogDef logDef, int userId)
        {
            NTInvoiceDef ntInvoiceDef = worker.getNTInvoiceByKey(int.Parse(logDef.ReferenceNo));
            if (ntInvoiceDef != null)
            {
                worker.updateNTActionHistory(ntInvoiceDef.InvoiceId, ntInvoiceDef.NTVendor.NTVendorId, "Non-Trade Expense Accrual Interface Generated - " + logDef.FiscalYear.ToString() + "/P" + logDef.Period.ToString() + " (QueueId = " + logDef.QueueId.ToString() + ")", userId);
            }
        }

        private void setNonTradeBankPaymentInterfaceComplete(SunInterfaceLogDef logDef, int userId)
        {
            NTInvoiceDef ntInvoiceDef = worker.getNTInvoiceByKey(int.Parse(logDef.ReferenceNo));
            if (ntInvoiceDef != null)
            {
                ntInvoiceDef.IsSUNInterfacedForSettlement = GeneralStatus.ACTIVE.Code;
                worker.updateNTInvoice(ntInvoiceDef, new ArrayList(), userId);

                worker.updateNTActionHistory(ntInvoiceDef.InvoiceId, ntInvoiceDef.NTVendor.NTVendorId, "Non-Trade Expense Invoice Settlement Interface Generated - " + logDef.FiscalYear.ToString() + "/P" + logDef.Period.ToString() + " (QueueId = " + logDef.QueueId.ToString() + ")", userId);
            }
        }

        private void setNTRechargeDCNoteInterfaceComplete(SunInterfaceLogDef logDef, int userId)
        {
            NTRechargeDCNoteDef ntDCNoteDef = worker.getNTRechargeDCNoteByDCNoteNo(logDef.DebitCreditNoteNo);
            if (ntDCNoteDef != null)
            {
                ntDCNoteDef.IsSUNInterfaced = GeneralStatus.ACTIVE.Code;
                worker.updateNTRechargeDCNote(ntDCNoteDef, ntDCNoteDef.Office.OfficeId, userId);

                List<NTInvoiceDetailDef> dcNoteDetailList = worker.getNTInvoiceDetailByRechargeDCNoteId(ntDCNoteDef.RechargeDCNoteId);
                foreach (NTInvoiceDetailDef dcNoteDetail in dcNoteDetailList)
                {
                    NTInvoiceDef ntInvoiceDef = worker.getNTInvoiceByKey(dcNoteDetail.InvoiceId);
                    worker.updateNTActionHistory(dcNoteDetail.InvoiceId, ntInvoiceDef.NTVendor.NTVendorId, "Non-Trade Recharge D/C Note Interface Generated - " + logDef.FiscalYear.ToString() + "/P" + logDef.Period.ToString() + " (QueueId = " + logDef.QueueId.ToString() + ")", userId);
                }
            }
        }

        public List<NTInvoiceDetailDef> getNTInvoiceTradingAFDetailByContractDeliveryNo(string contractNo, int deliveryNo)
        {
            return worker.getNTInvoiceTradingAFDetailByContractDeliveryNo(contractNo, deliveryNo);
        }

        public ArrayList updateNTInvoiceSettlement(ArrayList list, int userId)
        {   // Update settlement info to NTinvoice
            ArrayList updateFailedList = new ArrayList();
            DateTime max = DateTime.MinValue;
            bool isUpdated = false;
            int retryCount = 0;
            NTInvoiceDef def = null;

            foreach (Object obj in list)
            {
                ArrayList amendmentList = new ArrayList();
                isUpdated = false;
                retryCount = 0;

                while (isUpdated == false && retryCount < 3)
                {
                    TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
                    try
                    {
                        ctx.Enter();
                        TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                        def = (NTInvoiceDef)obj;
                        NonTradeWorker.Instance.updateNTInvoiceSettlement(def, amendmentList, userId);
                        foreach (NTActionHistoryDef historyDef in amendmentList)
                            worker.updateNTActionHistory(historyDef);

                        ctx.VoteCommit();
                        isUpdated = true;
                    }
                    catch (Exception e)
                    {
                        retryCount++;
                        ctx.VoteRollback();
                        if (def != null)
                            NoticeHelper.sendErrorMessage(e, "Non Trade settlement Update Failed - " + def.NSLInvoiceNo);
                        else
                            NoticeHelper.sendErrorMessage(e, "Non Trade settlement Update Failed");
                    }
                    finally
                    {
                        ctx.Exit();
                    }
                }

                if (!isUpdated)
                {
                    if (def != null)
                        updateFailedList.Add(def.NSLInvoiceNo);
                }
            }
            return updateFailedList;
        }

        public NSLBankAccountDef getNSLBankAccountByKey(int key)
        {
            return worker.getNSLBankAccountByKey(key);
        }

        public ArrayList getNSLBankAccount(TypeCollector officeIdList, TypeCollector currencyIdList)
        {
            return worker.getNSLBankAccount(officeIdList, currencyIdList);
        }

        public ArrayList getNTInvoiceCurrencyList()
        {
            return NonTradeWorker.Instance.getNTInvoiceCurrencyList();
        }

        public ArrayList getNTApproverListByOfficeId(ArrayList officeList)
        {
            TypeCollector officeIdList = TypeCollector.Inclusive;

            foreach (OfficeRef office in officeList)
            {
                officeIdList.append(office.OfficeId);
            }

            return worker.getNTApproverListByOfficeId(officeIdList);
        }

        public void updateNTApprover(NTApproverDef approverDef, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                worker.updateNTApprover(approverDef, userId);

                ctx.VoteCommit();

            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        public ArrayList getNTUserOfficeList(int userId, int roleId, int companyId)
        {
            return worker.getNTUserOfficeList(userId, roleId, companyId);
            /*
            return worker.getNTUserOfficeList(ntSuperUserRemap(userId), roleId, companyId);
            */
        }

        public ArrayList getNTUserOfficeIdList(int userId, int roleId, int companyId)
        {
            ArrayList officeIdList = new ArrayList();
            foreach (OfficeRef office in getNTUserOfficeList(userId, roleId, companyId))
                officeIdList.Add(office.OfficeId);
            return officeIdList;
        }

        /*
        public int ntSuperUserRemap(int userId)
        {
            return ((isNTSuperUser(userId) ? -1 : userId));
        }

        public bool isNTSuperUser(int userId)
        {
            return worker.isNTSuperUser(userId);
        }
        */

        public ArrayList getNTUserCompanyList(int userId, int officeId, int roleId)
        {
            return worker.getNTUserCompanyList(userId, officeId, roleId);
        }

        public ArrayList getNTUserList(int roleId, int companyId, int officeId)
        {
            return worker.getNTUserList(roleId, companyId, officeId);
        }

        public NTUserRoleAccessDef getNTUserRoleAccessByKey(int roleId, int companyId, int officeId, int userId)
        {
            return worker.getNTUserRoleAccessByKey(roleId, companyId, officeId, userId);
        }

        public ArrayList getNTUserList(int roleId, int officeId)
        {
            return worker.getNTUserList(roleId, -1, officeId);
        }

        public ArrayList getNTUserRoleAccessList(int roleId)
        {
            return worker.getNTUserRoleAccessList(roleId);
        }

        public void deleteNTUserRoleAccess(NTUserRoleAccessDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                worker.deleteNTUserRoleAccess(def, userId);

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        
        public string getFullItemDescriptionByInvoiceDetailId(NTInvoiceDetailDef detailDef)
        {
            if (detailDef.IsPayByHK == 0)
            {
                StringBuilder s = new StringBuilder();
                if (detailDef.ItemDescription1.Trim() != string.Empty) s.AppendLine(detailDef.ItemDescription1.Trim());
                if (detailDef.ItemDescription2.Trim() != string.Empty) s.AppendLine(detailDef.ItemDescription2.Trim());
                if (detailDef.ItemDescription3.Trim() != string.Empty) s.AppendLine(detailDef.ItemDescription3.Trim());
                if (detailDef.ItemDescription4.Trim() != string.Empty) s.AppendLine(detailDef.ItemDescription4.Trim());
                if (detailDef.ItemDescription5.Trim() != string.Empty) s.AppendLine(detailDef.ItemDescription5.Trim());
                return s.ToString();
            }
            else
            {
                StringBuilder s = new StringBuilder();
                int i = 1;
                NTInvoiceDef invoiceDef = worker.getNTInvoiceByKey(detailDef.InvoiceId);
                TypeCollector collector = TypeCollector.Inclusive;
                ArrayList detailList = null;

                collector.append(detailDef.InvoiceDetailType.Id);
                detailList = worker.getNTInvoiceDetailListByInvoiceIdAndType(detailDef.InvoiceId, collector);
                foreach(NTInvoiceDetailDef def in detailList)
                {
                    /*
                    if ((detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id && def.Office != null & def.Office.OfficeId == detailDef.Office.OfficeId)
                        || (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.CUSTOMER.Id && def.Customer != null & def.Customer.CustomerId == detailDef.Customer.CustomerId)
                        || (detailDef.InvoiceDetailType.Id != NTInvoiceDetailType.CUSTOMER.Id && detailDef.InvoiceDetailType.Id != NTInvoiceDetailType.OFFICE.Id && def.Vendor != null & def.Vendor.VendorId == detailDef.Vendor.VendorId))
                    */
                    if (def.Office != null & def.Office.OfficeId == detailDef.Office.OfficeId)

                    {
                        if (i != 1) s.AppendLine(string.Empty);
                        if (def.ItemDescription1.Trim() != string.Empty) s.AppendLine(def.ItemDescription1.Trim());
                        if (def.ItemDescription2.Trim() != string.Empty) s.AppendLine(def.ItemDescription2.Trim());
                        if (def.ItemDescription3.Trim() != string.Empty) s.AppendLine(def.ItemDescription3.Trim());
                        if (def.ItemDescription4.Trim() != string.Empty) s.AppendLine(def.ItemDescription4.Trim());
                        if (def.ItemDescription5.Trim() != string.Empty) s.AppendLine(def.ItemDescription5.Trim());

                        i++;
                    }
                }
                return s.ToString();
            }
        }

        public ArrayList getNTExpenseNatureList(int ntVendorId)
        {
            return worker.getNTExpenseNatureList(ntVendorId);
        }

        public NTExpenseNatureRef getNTExpenseNatureByKey(int id)
        {
            return worker.getNTExpenseNatureByKey(id);
        }

        public NTEpicorSegmentValueRef getNTEpicorSegmentValueByKey(int id)
        {
            return worker.getNTEpicorSegmentValueByKey(id);
        }

        public ArrayList getNTEpicorSegmentValueListBySegmentField(int segmentField)
        {
            return worker.getNTEpicorSegmentValueListBySegmentField(segmentField);
        }

        public void sendNTRechargeDCNoteCover(NTRechargeDCNoteInvoiceRef iDef, bool isIncludeSupportingDoc)
        {
            int rechargeDCNoteId;

            try
            {
                NTRechargeDCNoteDef def = iDef.DCNote; //this.getNTRechargeDCNoteByKey(dcNoteId);
                rechargeDCNoteId = def.RechargeDCNoteId;

                string attention = "";
                List<int> approverIdList = new List<int>();
                int rechargePartyDeptId = -1;

                var NTInvDetailDefList = NonTradeWorker.Instance.getNTInvoiceDetailByRechargeDCNoteId(rechargeDCNoteId);
                foreach (NTInvoiceDetailDef detail in NTInvDetailDefList)
                {
                    NTInvoiceDef inv = NonTradeWorker.Instance.getNTInvoiceByKey(detail.InvoiceId);
                    if (!approverIdList.Contains(inv.AccountFirstApproverId))
                        approverIdList.Add(inv.AccountFirstApproverId);
                    if (!approverIdList.Contains(inv.AccountSecondApproverId))
                        approverIdList.Add(inv.AccountSecondApproverId);

                    if (attention == "")
                        attention = detail.RechargeContactPerson.Trim();

                    if (detail.RechargePartyDept != null && rechargePartyDeptId == -1 && detail.RechargePartyDept.Id > 0)
                        rechargePartyDeptId = detail.RechargePartyDept.Id;
                }

                NTRechageDCNoteMailCase caseType = null;
                if (def.ToNTVendorId != 0) // Case 6
                {
                    caseType = NTRechageDCNoteMailCase.NTVendor;
                }
                else if (def.ToVendorId != 0) // Case 2
                {
                    caseType = NTRechageDCNoteMailCase.APSupplier;
                }
                else if (def.ToCustomerId == 2) // Case 1, 3, 4
                {
                    switch (rechargePartyDeptId)
                    {
                        case 4:
                            caseType = NTRechageDCNoteMailCase.GroupCentral; // Case 1
                            break;
                        case 2:
                            caseType = NTRechageDCNoteMailCase.Retail; // Case 3
                            break;
                        case 3:
                            caseType = NTRechageDCNoteMailCase.BrandFinance; // Case 4
                            break;
                    }
                }
                else if (def.ToCompanyId == 4 && def.ToOfficeId == 10) // Case 5
                {
                    caseType = NTRechageDCNoteMailCase.NSLUK;
                }


                if (caseType != null)
                {
                    TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
                    try
                    {
                        ctx.Enter();
                        TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                        if (attention == "")
                            attention = "Sir / Madam";

                        NTRechargeDCNote rpt = AccountReportManager.Instance.getNTRechargeDCNote(rechargeDCNoteId);
                        string outputFolder = WebConfig.getValue("appSettings", "NT_RECHARGE_DN_OUTPUT_Folder");
                        string fileName = outputFolder + def.RechargeDCNoteNo.Replace('/', '-') + ".pdf";
                        rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);
                        ArrayList attachmentList = new ArrayList();
                        attachmentList.Add(fileName);

                        if (isIncludeSupportingDoc)
                        {
                            attachmentList.AddRange(this.getAttachmentList(def));
                        }

                        DebitNoteToNUKParamDef paramDef = ReporterWorker.Instance.getDebitNoteToNUKParamByKey(def.Office.OfficeId, def.RechargeCurrencyId);
                        NoticeHelper.sendNTRechargeDCNoteCover(caseType, def, paramDef, attention, approverIdList, attachmentList);
                        def.MailStatus = 1;
                        worker.updateNTRechargeDCNote(def, def.Office.OfficeId, 99999);

                        ctx.VoteCommit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR " + e.Message);
                        ctx.VoteRollback();
                        throw e;
                    }
                    finally
                    {
                        ctx.Exit();
                    }

                }
                else
                {
                    // This iDef is not available for sending out NT Recharge Debit Note email
                    return;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        private string NTRechargeDCNoteMailCClist(int rechargeDCNoteId, NTRechageDCNoteMailCase type){
            string ccMail = "";
            // 1)
            List<int> approverIdList = new List<int>();
            foreach (NTInvoiceDetailDef detail in worker.getNTInvoiceDetailByRechargeDCNoteId(rechargeDCNoteId))
            {
                NTInvoiceDef inv = getNTInvoiceByKey(detail.InvoiceId);
                if (!approverIdList.Contains(inv.AccountFirstApproverId))
                    approverIdList.Add(inv.AccountFirstApproverId);
                if (!approverIdList.Contains(inv.AccountSecondApproverId))
                    approverIdList.Add(inv.AccountSecondApproverId);
            }            
            foreach (int approverId in approverIdList)
            {
                ccMail += (ccMail == "" ? "" : ";") + CommonUtil.getUserByKey(approverId).EmailAddress;
            }

            // 2)
            /*
            if (type == RechageDebitNoteMailCase.GroupCentral)
            {
                //lll.Add("Eric Leung <Eric_Leung@nextsl.com.hk>;Teresa Wong (Teresa_Wong@nextsl.com.hk); ");
                ccMail += "Eric_Leung@nextsl.com.hk;";
                ccMail += "Teresa_Wong@nextsl.com.hk;";
            }*/

            return ccMail;
        }

        private ArrayList getAttachmentList(NTRechargeDCNoteDef def)
        {
            string outputFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");
            ArrayList returnList = new ArrayList();
            List<NTInvoiceDef> invoiceList = worker.getNTInvoiceListByRechargeDCNoteId(def.RechargeDCNoteId);
            foreach (NTInvoiceDef invoiceDef in invoiceList)
            {
                ArrayList queryStructs = new ArrayList();
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
                queryStructs.Add(new QueryStructDef("Document Type", (invoiceDef.DCIndicator == "D" ? "Non-Trade-Invoice" : "Non-Trade-Refund")));
                queryStructs.Add(new QueryStructDef("NSL Invoice No", invoiceDef.NSLInvoiceNo));
                queryStructs.Add(new QueryStructDef("Invoice No", invoiceDef.InvoiceNo));
                queryStructs.Add(new QueryStructDef("Customer No", invoiceDef.CustomerNo));

                ArrayList qList = DMSUtil.queryDocument(queryStructs);

                DocumentInfoDef docDef = null;
                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    docDef = docInfoDef;
                    break;
                }

                if (docDef != null)
                {
                    int cnt = 1;
                    foreach (AttachmentInfoDef attachInfoDef in docDef.AttachmentInfos)
                    {
                        string fileName = string.Empty;
                        fileName = outputFolder + def.RechargeDCNoteNo + "-" + invoiceDef.NSLInvoiceNo + "-" + cnt.ToString() + ".pdf";
                        File.WriteAllBytes(fileName, DMSUtil.getAttachment(attachInfoDef.AttachmentID));
                        returnList.Add(fileName);
                        cnt += 1;
                    }
                }
            }
            return returnList;
        }

    }
}
