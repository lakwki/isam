using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.isam.dataserver.model.nontrade;
using com.next.isam.dataserver.model.account;
using com.next.isam.dataserver.worker;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.infra.util;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.types;


namespace com.next.isam.dataserver.worker
{
    public class NonTradeWorker : Worker
    {
        private static NonTradeWorker _instance;
        private GeneralWorker generalWorker;
        private CommonWorker commonWorker;
        private AccountWorker accountWorker;

        protected NonTradeWorker()
        {
            generalWorker = GeneralWorker.Instance;
            commonWorker = CommonWorker.Instance;
            accountWorker = AccountWorker.Instance;
        }

        public static NonTradeWorker Instance
        {
            get
            {
                if (_instance == null)
                    _instance =  new NonTradeWorker();
                return _instance;
            }
        }

        public NTExpenseTypeRef getNTExpenseTypeByKey(int ExpenseTypeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTExpenseTypeApt", "GetNTExpenseTypeByKey");
            ad.SelectCommand.Parameters["@ExpenseTypeId"].Value = ExpenseTypeId.ToString();
            NTExpenseTypeDs ds = new NTExpenseTypeDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTExpenseTypeRef def = new NTExpenseTypeRef();
            NTExpenseTypeMapping(ds.NTExpenseType.Rows[0], def);

            return def;
        }

        public ArrayList getNTExpenseTypeByOfficeId(int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTExpenseTypeApt", "GetNTExpenseTypeByOfficeId");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            NTExpenseTypeDs dataSet = new NTExpenseTypeDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTExpenseTypeDs.NTExpenseTypeRow row in dataSet.NTExpenseType)
            {
                NTExpenseTypeRef def = new NTExpenseTypeRef();
                NTExpenseTypeMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        public ArrayList getNTExpenseTypeByNTVendorId(int NTVendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTExpenseTypeApt", "GetNTExpenseTypeByNTVendorId");
            ad.SelectCommand.Parameters["@NTVendorId"].Value = NTVendorId;

            NTExpenseTypeDs dataSet = new NTExpenseTypeDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTExpenseTypeDs.NTExpenseTypeRow row in dataSet.NTExpenseType)
            {
                NTExpenseTypeRef def = new NTExpenseTypeRef();
                NTExpenseTypeMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getNTExpenseTypeList()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTExpenseTypeApt", "GetNTExpenseTypeByKey");
            ad.SelectCommand.Parameters["@ExpenseTypeId"].Value = GeneralCriteria.ALL;
            NTExpenseTypeDs ds = new NTExpenseTypeDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            ArrayList lst = new ArrayList();
            for (int i = 0; i < ds.NTExpenseType.Rows.Count; i++)
            {
                NTExpenseTypeDs.NTExpenseTypeRow row = (NTExpenseTypeDs.NTExpenseTypeRow)(ds.NTExpenseType.Rows[i]);
                NTExpenseTypeRef def = new NTExpenseTypeRef();
                NTExpenseTypeMapping(row, def);
                lst.Add(def);
            }
            return lst;
        }

        public NTVendorDef getNTVendorByKey(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTVendorApt", "GetNTVendorByKey");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId.ToString();
            NTVendorDs ds = new NTVendorDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTVendorDef def = new NTVendorDef();
            NTVendorMapping(ds.NTVendor.Rows[0], def);
            return def;
        }

        public NTVendorDef getNTVendorByEPVendorCode(string code, bool enforceEmail)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTVendorApt", "GetNTVendorByEPVendorCode");
            ad.SelectCommand.Parameters["@EPVendorCode"].Value = code;
            ad.SelectCommand.Parameters["@EnforceEmail"].Value = enforceEmail ? 1 : 0;
            NTVendorDs ds = new NTVendorDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTVendorDef def = new NTVendorDef();
            NTVendorMapping(ds.NTVendor.Rows[0], def);
            return def;
        }

        public ArrayList getNTVendorByName(string name, int officeId, int workflowStatusId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTVendorApt", "GetNTVendorByName");
            ad.SelectCommand.Parameters["@VendorName"].Value = name;
            ad.SelectCommand.Parameters["@WorkflowStatusId"].Value = workflowStatusId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            NTVendorDs dataSet = new NTVendorDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTVendorDs.NTVendorRow row in dataSet.NTVendor)
            {
                NTVendorDef def = new NTVendorDef();
                NTVendorMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        public ArrayList getNTVendorList(int officeId, int ntVendorId, int expenseTypeId, int workflowStatusId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTVendorApt", "GetNTVendorList");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@NTVendorId"].Value = ntVendorId;
            ad.SelectCommand.Parameters["@ExpenseTypeId"].Value = expenseTypeId;
            ad.SelectCommand.Parameters["@WorkflowStatusId"].Value = workflowStatusId;

            NTVendorDs dataSet = new NTVendorDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTVendorDs.NTVendorRow row in dataSet.NTVendor)
            {
                NTVendorDef def = new NTVendorDef();
                NTVendorMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        public ArrayList getNTVendorExpenseTypeMappingByNTVendorId(int ntVendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTVendorExpenseTypeMappingApt", "GetNTVendorExpenseTypeMappingByNTVendorId");
            ad.SelectCommand.Parameters["@NTVendorId"].Value = ntVendorId;

            NTVendorExpenseTypeMappingDs dataSet = new NTVendorExpenseTypeMappingDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTVendorExpenseTypeMappingDs.NTVendorExpenseTypeMappingRow row in dataSet.NTVendorExpenseTypeMapping)
            {
                NTVendorExpenseTypeMappingDef def = new NTVendorExpenseTypeMappingDef();
                NTVendorExpenseTypeMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        public void updateNTVendor(NTVendorDef def, ArrayList amendmentList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTVendorApt", "GetNTVendorByKey");
                ad.SelectCommand.Parameters["@VendorId"].Value = def.NTVendorId;
                ad.PopulateCommands();

                NTVendorDs dataSet = new NTVendorDs();
                NTVendorDs.NTVendorRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTVendor[0];

                    if (row.VendorName != def.VendorName)
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Vendor Name", row.VendorName, def.VendorName, userId));
                    if (def.Address != (row.IsAddressNull() ? string.Empty : row.Address))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Address", row.IsAddressNull() ? string.Empty : row.Address, def.Address, userId));
                    if (def.SUNAccountCode != (row.IsSUNAccountCodeNull() ? string.Empty : row.SUNAccountCode))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "SUN Account Code", row.IsSUNAccountCodeNull() ? string.Empty : row.SUNAccountCode, def.SUNAccountCode, userId));
                    if ((def.PaymentMethod == null ? 0 : def.PaymentMethod.Id) != (row.IsPaymentMethodIdNull() ? 0 : row.PaymentMethodId))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Payment Method", row.IsPaymentMethodIdNull() ? string.Empty : NTPaymentMethodRef.getType(row.PaymentMethodId).Name, def.PaymentMethod.Name, userId));
                    if ((def.Country == null ? 0 : def.Country.CountryId) != (row.IsCountryIdNull() ? 0 : row.CountryId))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Country", row.IsCountryIdNull() ? string.Empty : generalWorker.getCountryByKey(row.CountryId).Name, def.Country.Name, userId));
                    if ((def.CompanyId == int.MinValue ? -1 : def.CompanyId) != (row.IsCompanyIdNull() ? -1 : row.CompanyId))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Company", row.IsCompanyIdNull() ? "N/A" : generalWorker.getCompanyByKey(row.CompanyId).CompanyName, (def.CompanyId == int.MinValue ? -1 : def.CompanyId) == -1 ? "N/A" : generalWorker.getCompanyByKey(def.CompanyId).CompanyName, userId));
                    if ((def.Currency == null ? 0 : def.Currency.CurrencyId) != (row.IsCurrencyIdNull() ? 0 : row.CurrencyId))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Currency", row.IsCurrencyIdNull() ? string.Empty : CurrencyId.getCommonName(row.CurrencyId), def.Currency.CurrencyCode, userId));
                    if ((def.ExpenseType == null ? 0 : def.ExpenseType.ExpenseTypeId) != (row.IsExpenseTypeIdNull() ? 0 : row.ExpenseTypeId))
                    //    amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Default Expense Type", row.IsExpenseTypeIdNull() ? string.Empty : getNTExpenseTypeByKey(row.ExpenseTypeId).ExpenseType, def.ExpenseType.ExpenseType, userId));
                    {
                        string rowExpenseType = string.Empty;
                        string defExpenseType = string.Empty;
                        NTExpenseTypeRef rowExpenseTypeRef = null;
                        if (!row.IsExpenseTypeIdNull())
                            if ((rowExpenseTypeRef = getNTExpenseTypeByKey(row.ExpenseTypeId)) != null)
                                rowExpenseType = rowExpenseTypeRef.ExpenseType + " (" + OfficeId.getName(rowExpenseTypeRef.OfficeId) + ")";
                        defExpenseType = def.ExpenseType.ExpenseType + " (" + OfficeId.getName(def.ExpenseType.OfficeId) + ")";
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Default Expense Type", rowExpenseType, defExpenseType, userId));
                    }
                    if (row.OfficeId != def.Office.OfficeId)
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Office", OfficeId.getName(row.OfficeId), def.Office.OfficeCode, userId));
                    if (def.Telephone != (row.IsTelephoneNull() ? string.Empty : row.Telephone))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Phone No.", (row.IsTelephoneNull() ? string.Empty : row.Telephone), def.Telephone, userId));
                    if (def.Fax != (row.IsFaxNull() ? string.Empty : row.Fax))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Fax No.", (row.IsFaxNull() ? string.Empty : row.Fax), def.Fax, userId));
                    if (def.PaymentTermDays != (row.IsPaymentTermDaysNull() ? 0 : row.PaymentTermDays))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Payment Term Days", row.IsPaymentTermDaysNull() ? string.Empty : row.PaymentTermDays.ToString(), def.PaymentTermDays.ToString(), userId));
                    if (def.ReviewedOn != (row.IsReviewedOnNull() ? DateTime.MinValue : row.ReviewedOn))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Reviewed", string.Empty, string.Empty, userId));
                    if (def.IsCustomerNoRequired != (row.IsIsCustomerNoRequiredNull() ? 0 : row.IsCustomerNoRequired))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Customer No. Required", (row.IsIsCustomerNoRequiredNull() ? "NO" : (row.IsCustomerNoRequired == 1 ? "YES" : "NO")), (def.IsCustomerNoRequired == 1 ? "YES" : "NO"), userId));
                    if (def.IsInvoiceNoRequired != (row.IsIsInvoiceNoRequiredNull() ? 0 : row.IsInvoiceNoRequired))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Customer No. Required", (row.IsIsInvoiceNoRequiredNull() ? "NO" : (row.IsInvoiceNoRequired == 1 ? "YES" : "NO")), (def.IsInvoiceNoRequired == 1 ? "YES" : "NO"), userId));
                    if (def.WorkflowStatus.Id != row.WorkflowStatusId)
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Vendor Status", NTVendorWFS.getType(row.WorkflowStatusId).Name, def.WorkflowStatus.Name, userId));

                    if (def.EPVendorCode != (row.IsEPVendorCodeNull() ? "" : row.EPVendorCode))
                        amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Epicor Vendor Code", (row.IsEPVendorCodeNull() ? string.Empty : row.EPVendorCode), def.EPVendorCode, userId));

                    this.NTVendorMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.NTVendor.NewNTVendorRow();
                    def.NTVendorId = this.getMaxNTVendorId() + 1;
                    this.NTVendorMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.NTVendor.AddNTVendorRow(row);
                    amendmentList.Add(getNewNTActionHistoryDef(-1, def.NTVendorId, "Non-trade vendor created", null, null, userId));
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Non-trade Vendor ERROR");
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



        public int getMaxNTInvoiceId()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetMaxNTInvoiceId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public void updateNTVendorExpenseTypeMapping(NTVendorExpenseTypeMappingDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTVendorExpenseTypeMappingApt", "GetNTVendorExpenseTypeMappingByKey");
                ad.SelectCommand.Parameters["@NTVendorId"].Value = def.NTVendorId;
                ad.SelectCommand.Parameters["@ExpenseTypeId"].Value = def.ExpenseType.ExpenseTypeId;
                ad.PopulateCommands();

                NTVendorExpenseTypeMappingDs dataSet = new NTVendorExpenseTypeMappingDs();
                NTVendorExpenseTypeMappingDs.NTVendorExpenseTypeMappingRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTVendorExpenseTypeMapping[0];
                    this.NTVendorExpenseTypeMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.NTVendorExpenseTypeMapping.NewNTVendorExpenseTypeMappingRow();
                    this.NTVendorExpenseTypeMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.NTVendorExpenseTypeMapping.AddNTVendorExpenseTypeMappingRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Non-trade Vendor ERROR");
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

        public string getNTInvoiceNo(string invoicePrefix)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetMaxInvoiceNo");
            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = invoicePrefix;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return invoicePrefix + "000001";
            else
            {
                string invoiceNo = dataSet.Tables[0].Rows[0][0].ToString();

                return invoicePrefix + (int.Parse(invoiceNo.Substring(4, 6)) + 1).ToString().PadLeft(6, '0');
            }
        }

        public int getMaxNTInvoiceDetailId()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetMaxNTInvoiceDetailId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);

        }

        public int getMaxNTRechargeDetailId()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTRechargeDetailApt", "GetMaxNTRechargeDetailId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);

        }

        public int getMaxNTVendorId()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTVendorApt", "GetMaxNTVendorId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public int getEpicorVendorCodeSequenceNo(string vendorType)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTVendorApt", "GetEpicorVendorCodeSequenceNo");
            ad.SelectCommand.Parameters["@Suffix"].Value = vendorType.ToUpper();
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public NTInvoiceDef getNTInvoiceByKey(int invoiceId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceByKey");
            ad.SelectCommand.Parameters["@InvoiceId"].Value = invoiceId.ToString();
            NTInvoiceDs ds = new NTInvoiceDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTInvoiceDef def = new NTInvoiceDef();
            NTInvoiceMapping(ds.NTInvoice.Rows[0], def);
            return def;
        }

        public NTInvoiceDef getNTInvoiceByRefNo(string refNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceByRefNo");
            ad.SelectCommand.Parameters["@RefNo"].Value = refNo.ToString();
            NTInvoiceDs ds = new NTInvoiceDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTInvoiceDef def = new NTInvoiceDef();
            NTInvoiceMapping(ds.NTInvoice.Rows[0], def);
            return def;
        }

        public NTInvoiceDef getNTInvoiceByProcurementRequestId(int procurementRequestId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceByProcurementRequestId");
            ad.SelectCommand.Parameters["@ProcurementRequestId"].Value = procurementRequestId.ToString();
            NTInvoiceDs ds = new NTInvoiceDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTInvoiceDef def = new NTInvoiceDef();
            NTInvoiceMapping(ds.NTInvoice.Rows[0], def);
            return def;
        }

        public ArrayList getNTInvoiceList(TypeCollector officeList, int expenseTypeId, int fiscalYear, int fiscalPeriodFrom, int fiscalPeriodTo, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo,
                DateTime settlementDateFrom, DateTime settlementDateTo, string invoiceNoFrom, string invoiceNoTo, string customerNoFrom, string customerNoTo, string nslRefNoFrom, string nslRefNoTo, int vendorId, TypeCollector workflowStatusIdList,
                DateTime paymentDateFrom, DateTime paymentDateTo, int currencyId, int paymentMethodId, int departmentId, int userId, int includePayByHK, string dcIndicator, int firstApproverid, int submittedBy)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceList");
            ad.SelectCommand.CustomParameters["@OfficeList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);
            ad.SelectCommand.Parameters["@ExpenseTypeId"].Value = expenseTypeId;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@FiscalPeriodFrom"].Value = fiscalPeriodFrom;
            ad.SelectCommand.Parameters["@FiscalPeriodTo"].Value = fiscalPeriodTo;
            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;
            }
            if (dueDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@DueDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@DueDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@DueDateFrom"].Value = dueDateFrom;
                ad.SelectCommand.Parameters["@DueDateTo"].Value = dueDateTo;
            }
            if (settlementDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@SettlementDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@SettlementDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@SettlementDateFrom"].Value = settlementDateFrom;
                ad.SelectCommand.Parameters["@SettlementDateTo"].Value = settlementDateTo;
            }
            ad.SelectCommand.Parameters["@InvoiceNoFrom"].Value = invoiceNoFrom;
            ad.SelectCommand.Parameters["@InvoiceNoTo"].Value = invoiceNoTo;
            ad.SelectCommand.Parameters["@CustomerNoFrom"].Value = customerNoFrom;
            ad.SelectCommand.Parameters["@CustomerNoTo"].Value = customerNoTo;
            ad.SelectCommand.Parameters["@NSLRefNoFrom"].Value = nslRefNoFrom;
            ad.SelectCommand.Parameters["@NSLRefNoTo"].Value = nslRefNoTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            /*
            ad.SelectCommand.Parameters["@WorkflowStatusId"].Value = workflowStatusId;
            */
            if (workflowStatusIdList.Values.Count > 0)
                ad.SelectCommand.CustomParameters["@WorkflowStatusIdList"] = CustomDataParameter.parse(workflowStatusIdList.IsInclusive, workflowStatusIdList.Values);
            else
            {
                int[] noId = { int.MinValue };
                ad.SelectCommand.CustomParameters["@WorkflowStatusIdList"] = CustomDataParameter.parse(workflowStatusIdList.IsInclusive, noId);
            }

            if (paymentDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = paymentDateFrom;
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = paymentDateTo;
            }
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;
            ad.SelectCommand.Parameters["@PaymentMethodId"].Value = paymentMethodId;
            ad.SelectCommand.Parameters["@DepartmentId"].Value = departmentId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.Parameters["@FirstApproverId"].Value = firstApproverid;
            ad.SelectCommand.Parameters["@IncludePayByHK"].Value = includePayByHK;
            if (dcIndicator == null)
                dcIndicator = string.Empty;
            ad.SelectCommand.Parameters["@DCIndicator"].Value = dcIndicator;
            ad.SelectCommand.Parameters["@SubmittedBy"].Value = submittedBy;
            ad.SelectCommand.MailSQL = true;
            NTInvoiceDs dataSet = new NTInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTInvoiceDs.NTInvoiceRow row in dataSet.NTInvoice)
            {
                NTInvoiceDef def = new NTInvoiceDef();
                NTInvoiceMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getNTRechargeDCNoteList(TypeCollector officeList, DateTime fromDate, DateTime toDate)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("NTRechargeDCNoteApt", "GetNTRechargeDCNoteList");
            ad.SelectCommand.CustomParameters["@OfficeList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);
            ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
            ad.SelectCommand.Parameters["@ToDate"].Value = toDate;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            NTRechargeDCNoteDs dataSet = new NTRechargeDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTRechargeDCNoteDs.NTRechargeDCNoteRow row in dataSet.NTRechargeDCNote)
            {
                NTRechargeDCNoteDef def = new NTRechargeDCNoteDef();
                NTRechargeDCNoteMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getNTRechargeDCNoteInvoiceList(TypeCollector officeList, DateTime fromDate, DateTime toDate)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("NTRechargeDCNoteApt", "GetNTRechargeDCNoteInvoiceList");
            ad.SelectCommand.CustomParameters["@OfficeList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);
            ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
            ad.SelectCommand.Parameters["@ToDate"].Value = toDate;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            NTRechargeDCNoteDs dataSet = new NTRechargeDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            Hashtable ht_dcNote = new Hashtable();
            NTRechargeDCNoteInvoiceRef currentRef;
            foreach (NTRechargeDCNoteDs.NTRechargeDCNoteRow row in dataSet.NTRechargeDCNote)
            {
                if (ht_dcNote.ContainsKey(row.RechargeDCNoteNo))
                    currentRef = (NTRechargeDCNoteInvoiceRef)ht_dcNote[row.RechargeDCNoteNo];
                else
                {
                    currentRef = new NTRechargeDCNoteInvoiceRef();
                    currentRef.DCNote = new NTRechargeDCNoteDef();
                    currentRef.InvoiceList = new List<NTInvoiceDef>();

                    NTRechargeDCNoteMapping(row, currentRef.DCNote);
                    ht_dcNote.Add(row.RechargeDCNoteNo, currentRef);
                    list.Add(currentRef);
                }
                NTInvoiceDef inv = getNTInvoiceByKey(row.InvoiceId);
                if (inv.InvoiceId == row.InvoiceId)
                    currentRef.InvoiceList.Add(inv);
            }
            return list;
        }

        public ArrayList getPendingForApprovalInvoiceList(DateTime submittedDate)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetPendingForApprovalInvoiceList");
            ad.SelectCommand.Parameters["@StartDate"].Value = submittedDate.Date;
            ad.SelectCommand.Parameters["@EndDate"].Value = submittedDate.Date.AddDays(1);

            NTInvoiceDs dataSet = new NTInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTInvoiceDs.NTInvoiceRow row in dataSet.NTInvoice)
            {
                NTInvoiceDef def = new NTInvoiceDef();
                NTInvoiceMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        public List<NTInvoiceDef> getIntercommRechargeInvoiceList(TypeCollector invoiceIdList, string officeCode)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetIntercommRechargeInvoiceList");
            ad.SelectCommand.CustomParameters["@InvoiceIdList"] = CustomDataParameter.parse(invoiceIdList.IsInclusive, invoiceIdList.Values);
            ad.SelectCommand.Parameters["@OfficeCode"].Value = officeCode;

            NTInvoiceDs dataSet = new NTInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            List<NTInvoiceDef> list = new List<NTInvoiceDef>();

            foreach (NTInvoiceDs.NTInvoiceRow row in dataSet.NTInvoice)
            {
                NTInvoiceDef def = new NTInvoiceDef();
                NTInvoiceMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getIntercommRechargeOfficeList(TypeCollector invoiceIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetIntercommRechargeOfficeList");
            ad.SelectCommand.CustomParameters["@InvoiceIdList"] = CustomDataParameter.parse(invoiceIdList.IsInclusive, invoiceIdList.Values);

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (DataRow row in dataSet.Tables[0].Rows)
                list.Add(row[0].ToString());
            return list;
        }

        public List<NTInvoiceDef> getDebitNoteRechargeInvoiceList(TypeCollector rechargeDCNoteIdList, string officeCode)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetDebitNoteRechargeInvoiceList");
            ad.SelectCommand.CustomParameters["@RechargeDCNoteIdList"] = CustomDataParameter.parse(rechargeDCNoteIdList.IsInclusive, rechargeDCNoteIdList.Values);
            ad.SelectCommand.Parameters["@OfficeCode"].Value = officeCode;

            NTInvoiceDs dataSet = new NTInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            List<NTInvoiceDef> list = new List<NTInvoiceDef>();

            foreach (NTInvoiceDs.NTInvoiceRow row in dataSet.NTInvoice)
            {
                NTInvoiceDef def = new NTInvoiceDef();
                NTInvoiceMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<NTInvoiceDef> getNTInvoiceListByRechargeDCNoteId(int rechargeDCNoteId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceListByRechargeDCNoteId");
            ad.SelectCommand.Parameters["@RechargeDCNoteId"].Value = rechargeDCNoteId;

            NTInvoiceDs dataSet = new NTInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            List<NTInvoiceDef> list = new List<NTInvoiceDef>();

            foreach (NTInvoiceDs.NTInvoiceRow row in dataSet.NTInvoice)
            {
                NTInvoiceDef def = new NTInvoiceDef();
                NTInvoiceMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getNTInvoiceListByVendorId(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceListByVendorId");
            ad.SelectCommand.Parameters["@NTVendorId"].Value = vendorId.ToString();

            NTInvoiceDs dataSet = new NTInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTInvoiceDs.NTInvoiceRow row in dataSet.NTInvoice)
            {
                NTInvoiceDef def = new NTInvoiceDef();
                NTInvoiceMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        public bool isNTInvoiceDuplicated(int invoiceId, string invoiceNo, string customerNo, DateTime invoiceDate, string epicorSupplierId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceListByInvoiceNoAndVendorAccountCode");
            ad.SelectCommand.Parameters["@InvoiceId"].Value = invoiceId;
            ad.SelectCommand.Parameters["@InvoiceNo"].Value = invoiceNo;
            ad.SelectCommand.Parameters["@CustomerNo"].Value = customerNo;
            ad.SelectCommand.Parameters["@EpicorSupplierId"].Value = epicorSupplierId;

            if (invoiceDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDate"].Value = invoiceDate;

            NTInvoiceDs dataSet = new NTInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1)
                return false;
            else
                return true;

        }

        public bool isNTInvoiceHasDebitNote(int invoiceId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetNTInvoiceDetailListForRechargeDCNoteByInvoiceId");
            ad.SelectCommand.Parameters["@InvoiceId"].Value = invoiceId;

            NTInvoiceDetailDs dataSet = new NTInvoiceDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1)
                return false;
            else
                return true;
        }

        public bool isNTInvoiceHasFixedAsset(int invoiceId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetNTInvoiceDetailListForFixedAssetByInvoiceId");
            ad.SelectCommand.Parameters["@InvoiceId"].Value = invoiceId;

            NTInvoiceDetailDs dataSet = new NTInvoiceDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1)
                return false;
            else
                return true;
        }


        public NTInvoiceDetailDef getNTInvoiceDetailByKey(int invoiceDetailId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetNTInvoiceDetailByKey");
            ad.SelectCommand.Parameters["@InvoiceDetailId"].Value = invoiceDetailId.ToString();
            NTInvoiceDetailDs ds = new NTInvoiceDetailDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTInvoiceDetailDef def = new NTInvoiceDetailDef();
            NTInvoiceDetailMapping(ds.NTInvoiceDetail.Rows[0], def);
            return def;
        }


        public ArrayList getNTInvoiceDetailListByInvoiceIdAndType(int invoiceId, TypeCollector invoiceDetailTypeList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetNTInvoiceDetailListByInvoiceIdAndType");
            ad.SelectCommand.Parameters["@InvoiceId"].Value = invoiceId.ToString();
            ad.SelectCommand.CustomParameters["@InvoiceDetailTypeList"] = CustomDataParameter.parse(invoiceDetailTypeList.IsInclusive, invoiceDetailTypeList.Values);

            NTInvoiceDetailDs ds = new NTInvoiceDetailDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            ArrayList lst = new ArrayList();
            for (int i = 0; i < recordsAffected; i++)
            {
                NTInvoiceDetailDs.NTInvoiceDetailRow row = ds.NTInvoiceDetail[i];
                NTInvoiceDetailDef def = new NTInvoiceDetailDef();
                NTInvoiceDetailMapping(row, def);
                lst.Add(def);
            }
            return lst;
        }

        public ArrayList getNTInvoiceDetailForRechargeDCNote(int officeId, int companyId, ref ArrayList invoiceList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetNTInvoiceDetailListForRechargeDCNote");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@CompanyId"].Value = companyId;

            NTInvoiceDetailDs ds = new NTInvoiceDetailDs();

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;

            ArrayList list = new ArrayList();

            foreach (NTInvoiceDetailDs.NTInvoiceDetailRow row in ds.NTInvoiceDetail)
            {
                NTInvoiceDetailDef def = new NTInvoiceDetailDef();
                NTInvoiceDetailMapping(row, def);
                list.Add(def);
                invoiceList.Add(getNTInvoiceByKey(def.InvoiceId));
            }

            return list;
        }


        public List<NTInvoiceDetailDef> getNTInvoiceDetailByRechargeDCNoteId(int rechargeDCNoteId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetNTInvoiceDetailListByRechargeDCNoteId");
            ad.SelectCommand.Parameters["@RechargeDCNoteId"].Value = rechargeDCNoteId;

            NTInvoiceDetailDs ds = new NTInvoiceDetailDs();
            List<NTInvoiceDetailDef> list = new List<NTInvoiceDetailDef>();

            int recordsAffected = ad.Fill(ds);

            foreach (NTInvoiceDetailDs.NTInvoiceDetailRow row in ds.NTInvoiceDetail)
            {
                NTInvoiceDetailDef def = new NTInvoiceDetailDef();
                NTInvoiceDetailMapping(row, def);
                list.Add(def);
            }

            return list;

        }


        public List<NTInvoiceDetailDef> getNTInvoiceTradingAFDetailByContractDeliveryNo(string contractNo, int deliveryNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetNTInvoiceTradingAFDetailListByContractDeliveryNo");
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;

            NTInvoiceDetailDs ds = new NTInvoiceDetailDs();
            List<NTInvoiceDetailDef> list = new List<NTInvoiceDetailDef>();

            int recordsAffected = ad.Fill(ds);

            foreach (NTInvoiceDetailDs.NTInvoiceDetailRow row in ds.NTInvoiceDetail)
            {
                NTInvoiceDetailDef def = new NTInvoiceDetailDef();
                NTInvoiceDetailMapping(row, def);
                list.Add(def);
            }

            return list;

        }


        public void updateNTInvoice(NTInvoiceDef invoice, ArrayList amendmentList, int userId, bool updateSetttlementOnly)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceByKey");
                ad.SelectCommand.Parameters["@InvoiceId"].Value = invoice.InvoiceId;
                ad.PopulateCommands();

                NTInvoiceDs dataSet = new NTInvoiceDs();
                NTInvoiceDs.NTInvoiceRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTInvoice[0];

                    if (!updateSetttlementOnly)
                    {
                        if (invoice.Office.OfficeId != row.OfficeId)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Office", OfficeId.getName(row.OfficeId), invoice.Office.OfficeCode, userId));
                        if (invoice.NTVendor.NTVendorId != row.NTVendorId)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Non-trade Vendor", getNTVendorByKey(row.NTVendorId).VendorName, invoice.NTVendor.VendorName, userId));
                        if (invoice.CustomerNo != (row.IsCustomerNoNull() ? string.Empty : row.CustomerNo))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Customer No.", (row.IsCustomerNoNull() ? string.Empty : row.CustomerNo), invoice.CustomerNo, userId));
                        if (invoice.InvoiceNo != (row.IsInvoiceNoNull() ? string.Empty : row.InvoiceNo))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Invoice No.", (row.IsInvoiceNoNull() ? string.Empty : row.InvoiceNo), invoice.InvoiceNo, userId));
                        if (invoice.InvoiceDate != (row.IsInvoiceDateNull() ? DateTime.MinValue : row.InvoiceDate))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Invoice Date", (row.IsInvoiceDateNull() ? string.Empty : DateTimeUtility.getDateString(row.InvoiceDate)), DateTimeUtility.getDateString(invoice.InvoiceDate), userId));
                        if (invoice.DueDate != (row.IsDueDateNull() ? DateTime.MinValue : row.DueDate))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Due Date", (row.IsDueDateNull() ? string.Empty : DateTimeUtility.getDateString(row.DueDate)), DateTimeUtility.getDateString(invoice.DueDate), userId));
                        if (invoice.PaymentFromDate.Month != row.PaymentFromDate.Month)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Payment Month From", row.PaymentFromDate.ToString("MMM"), invoice.PaymentFromDate.ToString("MMM"), userId));
                        if (invoice.PaymentFromDate.Year != row.PaymentFromDate.Year)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Payment Year From", row.PaymentFromDate.Year.ToString(), invoice.PaymentFromDate.Year.ToString(), userId));
                        if (invoice.PaymentToDate.Month != row.PaymentToDate.Month)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Payment Month To", row.PaymentToDate.ToString("MMM"), invoice.PaymentToDate.ToString("MMM"), userId));
                        if (invoice.PaymentToDate.Year != row.PaymentToDate.Year)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Payment Year To", row.PaymentToDate.Year.ToString(), invoice.PaymentToDate.Year.ToString(), userId));
                        if (invoice.InvoiceReceivedDate != row.InvoiceReceivedDate)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Invoice Received Date", DateTimeUtility.getDateString(row.InvoiceReceivedDate), DateTimeUtility.getDateString(invoice.InvoiceReceivedDate), userId));
                        if (invoice.Currency.CurrencyId != row.CurrencyId)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Currency", CurrencyId.getCommonName(row.CurrencyId), invoice.Currency.CurrencyCode, userId));
                        if (invoice.PaymentMethod.Id != row.PaymentMethodId)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Payment Method", NTPaymentMethodRef.getType(row.PaymentMethodId).Name, invoice.PaymentMethod.Name, userId));
                        if (invoice.Amount != row.Amount)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Total Invoice Amount", row.Amount.ToString(), invoice.Amount.ToString(), userId));

                        if (invoice.SettlementRefNo != (row.IsSettlementRefNoNull() ? string.Empty : row.SettlementRefNo))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Settlement Ref. No.", (row.IsSettlementRefNoNull() ? string.Empty : row.SettlementRefNo), invoice.SettlementRefNo, userId));

                        if (invoice.IsPayByHK != row.IsPayByHK)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Is Pay By HK", row.IsPayByHK.ToString(), invoice.IsPayByHK.ToString(), userId));
                        if (invoice.RejectReason != (row.IsRejectReasonNull() ? string.Empty : row.RejectReason))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Reject Reason", (row.IsRejectReasonNull() ? string.Empty : row.RejectReason), invoice.RejectReason, userId));
                        if (invoice.DCIndicator != row.DCIndicator)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Debit / Credit Note", row.DCIndicator, invoice.DCIndicator, userId));
                        if (invoice.UserRemark != (row.IsUserRemarkNull() ? string.Empty : row.UserRemark))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Remark", (row.IsUserRemarkNull() ? string.Empty : row.UserRemark), invoice.UserRemark, userId));
                        if (invoice.Company.Id != row.CompanyId)
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Business Entity", CompanyType.getType(row.CompanyId).CompanyName, invoice.Company.CompanyName, userId));

                        if (invoice.SUNInterfaceDate != (row.IsSUNInterfaceDateNull() ? DateTime.MinValue : row.SUNInterfaceDate))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Epicor Interface Date", (row.IsSUNInterfaceDateNull() ? string.Empty : DateTimeUtility.getDateString(row.SUNInterfaceDate)), (invoice.SUNInterfaceDate == DateTime.MinValue ? string.Empty : DateTimeUtility.getDateString(invoice.SUNInterfaceDate)), userId));

                        if (invoice.LogoInterfaceDate != (row.IsLogoInterfaceDateNull() ? DateTime.MinValue : row.LogoInterfaceDate))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Logo Interface Date", (row.IsLogoInterfaceDateNull() ? string.Empty : DateTimeUtility.getDateString(row.LogoInterfaceDate)), (invoice.LogoInterfaceDate == DateTime.MinValue ? string.Empty : DateTimeUtility.getDateString(invoice.LogoInterfaceDate)), userId));

                        if (invoice.ReleaseReason != (row.IsReleaseReasonNull() ? string.Empty : row.ReleaseReason))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Release Reason", (row.IsReleaseReasonNull() ? string.Empty : row.ReleaseReason), invoice.ReleaseReason, userId));
                        if (invoice.JournalNo != (row.IsJournalNoNull() ? string.Empty : row.JournalNo))
                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Journal No.", (row.IsJournalNoNull() ? string.Empty : row.JournalNo), invoice.JournalNo, userId));
                        if (invoice.ProcurementRequestId != (row.IsProcurementRequestIdNull() ? -1 : row.ProcurementRequestId))
                        {
                            APDS.APDSService svc = new APDS.APDSService();
                            APDS.ProcurementRequestDef requestDef;
                            string newRequestNo = string.Empty;
                            string oldRequestNo = string.Empty;
                            if (invoice.ProcurementRequestId != -1)
                            {
                                requestDef = svc.GetProcurementRequestByKey(invoice.ProcurementRequestId);
                                newRequestNo = requestDef.RequestNo;
                            }
                            if (!row.IsProcurementRequestIdNull())
                            {
                                requestDef = svc.GetProcurementRequestByKey(row.ProcurementRequestId);
                                oldRequestNo = requestDef.RequestNo;
                            }

                            amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Procurement Request No.", oldRequestNo, newRequestNo, userId));
                        }

                    }

                    // Update Settlement related fields
                    if (invoice.SettlementDate != (row.IsSettlementDateNull() ? DateTime.MinValue : row.SettlementDate))
                        amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Settlement Date", (row.IsSettlementDateNull() ? string.Empty : DateTimeUtility.getDateString(row.SettlementDate)), DateTimeUtility.getDateString(invoice.SettlementDate), userId));
                    if (invoice.SettlementAmount != (row.IsSettlementAmountNull() ? 0 : row.SettlementAmount))
                        amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Settlement Amount", (row.IsSettlementAmountNull() ? string.Empty : row.SettlementAmount.ToString()), invoice.SettlementAmount.ToString(), userId));


                    if (invoice.SettlementBankAccountId != (row.IsSettlementBankAccountIdNull() ? int.MinValue : row.SettlementBankAccountId))
                    {
                        NSLBankAccountDef def;
                        string AccountNo = string.Empty;
                        string NewAccountNo = string.Empty;
                        if ((def = getNSLBankAccountByKey(row.IsSettlementBankAccountIdNull() ? -1 : row.SettlementBankAccountId)) != null)
                            AccountNo = def.AccountNo;
                        if ((def = getNSLBankAccountByKey(invoice.SettlementBankAccountId)) != null)
                            NewAccountNo = def.AccountNo;
                        amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Settlement Bank Account No", AccountNo, NewAccountNo, userId));
                    }
                    if (invoice.UpdateSettlementUserId != (row.IsUpdateSettlementUserIdNull() ? int.MinValue : row.UpdateSettlementUserId))
                    {
                        UserRef user;
                        string orgUpdater = string.Empty;
                        string newUpdater = string.Empty;
                        if ((user = generalWorker.getUserByKey(row.IsUpdateSettlementUserIdNull() ? -1 : row.UpdateSettlementUserId)) != null)
                            orgUpdater = user.DisplayName;
                        if ((user = generalWorker.getUserByKey(invoice.UpdateSettlementUserId)) != null)
                            newUpdater = user.DisplayName;
                        amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Update Settlement By", orgUpdater, newUpdater, userId));
                    }
                    if (invoice.ChequeNo != (row.IsChequeNoNull() ? string.Empty : row.ChequeNo))
                        amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Cheque No.", (row.IsChequeNoNull() ? string.Empty : row.ChequeNo), invoice.ChequeNo, userId));


                    if (invoice.WorkflowStatus.Id != row.WorkflowStatusId)
                        amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Invoice Status", NTWFS.getType(row.WorkflowStatusId).Name, invoice.WorkflowStatus.Name, userId));

                    this.NTInvoiceMapping(invoice, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.NTInvoice.NewNTInvoiceRow();
                    invoice.InvoiceId = this.getMaxNTInvoiceId() + 1;
                    invoice.NSLInvoiceNo = getNTInvoiceNo("NS" + DateTime.Today.Year.ToString().Substring(2, 2));
                    this.NTInvoiceMapping(invoice, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.NTInvoice.AddNTInvoiceRow(row);
                    amendmentList.Add(getNewNTActionHistoryDef(invoice.InvoiceId, -1, "Non-trade invoice created", null, null, userId));
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Non-trade Invoice ERROR");
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

        public void updateNTInvoice(NTInvoiceDef invoice, ArrayList amendmentList, int userId)
        {
            updateNTInvoice(invoice, amendmentList, userId, false);
        }

        public void updateNTInvoiceSettlement(NTInvoiceDef invoice, ArrayList amendmentList, int userId)
        {
            updateNTInvoice(invoice, amendmentList, userId, true);
        }

        public int updateNTInvoiceDetail(NTInvoiceDetailDef invoiceDetailDef, ArrayList amendmentList, int userId)
        {
            return updateNTInvoiceDetailReturnOriginalDCNoteId(invoiceDetailDef, amendmentList, userId);
        }

        private int updateNTInvoiceDetailReturnOriginalDCNoteId(NTInvoiceDetailDef invoiceDetailDef, ArrayList amendmentList, int userId)
        {
            int dcNoteId = 0;
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailApt", "GetNTInvoiceDetailByKey");
                ad.SelectCommand.Parameters["@InvoiceDetailId"].Value = invoiceDetailDef.InvoiceDetailId;
                ad.PopulateCommands();

                NTInvoiceDetailDs dataSet = new NTInvoiceDetailDs();
                NTInvoiceDetailDs.NTInvoiceDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTInvoiceDetail[0];
                    if (row.IsRechargeDCNoteIdNull())
                        dcNoteId = 0;
                    else
                        dcNoteId = row.RechargeDCNoteId;
                    if (invoiceDetailDef.ExpenseType != null && invoiceDetailDef.ExpenseType.ExpenseTypeId != row.ExpenseTypeId)
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Expense Type", getNTExpenseTypeByKey(row.ExpenseTypeId).ExpenseType, invoiceDetailDef.ExpenseType.ExpenseType, userId));
                    if ((invoiceDetailDef.CostCenter == null ? 0 : invoiceDetailDef.CostCenter.CostCenterId) != (row.IsCostCenterIdNull() ? 0 : row.CostCenterId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Cost Centre", (row.IsCostCenterIdNull() ? string.Empty : generalWorker.getCostCenterByKey(row.CostCenterId).Description), (invoiceDetailDef.CostCenter == null ? string.Empty : invoiceDetailDef.CostCenter.Description), userId));
                    if ((invoiceDetailDef.Vendor == null ? 0 : invoiceDetailDef.Vendor.VendorId) != (row.IsVendorIdNull() ? 0 : row.VendorId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Recharge Vendor", (row.IsVendorIdNull() ? string.Empty : VendorWorker.Instance.getVendorByKey(row.VendorId).Name), (invoiceDetailDef.Vendor == null ? string.Empty : invoiceDetailDef.Vendor.Name), userId));
                    if ((invoiceDetailDef.Customer == null ? 0 : invoiceDetailDef.Customer.CustomerId) != (row.IsCustomerIdNull() ? 0 : row.CustomerId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Customer", (row.IsCustomerIdNull() ? string.Empty : CommonWorker.Instance.getCustomerByKey(row.CustomerId).CustomerCode), (invoiceDetailDef.Customer == null ? string.Empty : invoiceDetailDef.Customer.CustomerCode), userId));
                    if ((invoiceDetailDef.Department == null ? 0 : invoiceDetailDef.Department.DepartmentId) != (row.IsDepartmentIdNull() ? 0 : row.DepartmentId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Department", (row.IsDepartmentIdNull() ? string.Empty : generalWorker.getDepartmentByKey(row.DepartmentId).Description), (invoiceDetailDef.Department == null ? string.Empty : invoiceDetailDef.Department.Description), userId));
                    if ((invoiceDetailDef.ProductTeam == null ? 0 : invoiceDetailDef.ProductTeam.ProductCodeId) != (row.IsProductTeamIdNull() ? 0 : row.ProductTeamId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Product Team", (row.IsProductTeamIdNull() ? string.Empty : generalWorker.getProductCodeRefByKey(row.ProductTeamId).Code), (invoiceDetailDef.ProductTeam == null ? string.Empty : invoiceDetailDef.ProductTeam.Code), userId));
                    if ((invoiceDetailDef.Season == null ? 0 : invoiceDetailDef.Season.SeasonId) != (row.IsSeasonIdNull() ? 0 : row.SeasonId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Season", (row.IsSeasonIdNull() ? string.Empty : generalWorker.getSeasonByKey(row.SeasonId).Code), (invoiceDetailDef.Season == null ? string.Empty : invoiceDetailDef.Season.Code), userId));
                    if ((invoiceDetailDef.User == null ? 0 : invoiceDetailDef.User.UserId) != (row.IsUserIdNull() ? 0 : row.UserId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "User", (row.IsUserIdNull() ? string.Empty : (row.UserId == -1 ? "UNCLASSIFIED" : generalWorker.getUserByKey(row.UserId).DisplayName)), (invoiceDetailDef.User == null ? string.Empty : invoiceDetailDef.User.DisplayName), userId));
                    if ((invoiceDetailDef.RechargeCurrency == null ? 0 : invoiceDetailDef.RechargeCurrency.CurrencyId) != (row.IsRechargeCurrencyIdNull() ? 0 : row.RechargeCurrencyId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Recharge Currency", (row.IsRechargeCurrencyIdNull() ? string.Empty : CurrencyId.getCommonName(row.RechargeCurrencyId)), (invoiceDetailDef.RechargeCurrency == null ? string.Empty : invoiceDetailDef.RechargeCurrency.CurrencyCode), userId));
                    if (invoiceDetailDef.Amount != row.Amount)
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Invoice Detail Amount", row.Amount.ToString(), invoiceDetailDef.Amount.ToString(), userId));
                    if (invoiceDetailDef.Status != row.Status && invoiceDetailDef.Status == 0)
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, string.Format("Remove invoice detail {0}: {1} - {2}", (invoiceDetailDef.IsRecharge == 1 ? "(Recharge)" : ""), invoiceDetailDef.ExpenseType == null ? string.Empty : invoiceDetailDef.ExpenseType.ExpenseType, invoiceDetailDef.Amount.ToString()), null, null, userId));
                    if (invoiceDetailDef.ItemDescription1 != (row.IsItemDescription1Null() ? string.Empty : row.ItemDescription1))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Item Description 1", (row.IsItemDescription1Null() ? string.Empty : row.ItemDescription1), invoiceDetailDef.ItemDescription1, userId));
                    if (invoiceDetailDef.ItemDescription2 != (row.IsItemDescription2Null() ? string.Empty : row.ItemDescription2))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Item Description 2", (row.IsItemDescription2Null() ? string.Empty : row.ItemDescription2), invoiceDetailDef.ItemDescription2, userId));
                    if (invoiceDetailDef.ItemDescription3 != (row.IsItemDescription3Null() ? string.Empty : row.ItemDescription3))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Item Description 3", (row.IsItemDescription3Null() ? string.Empty : row.ItemDescription3), invoiceDetailDef.ItemDescription3, userId));
                    if (invoiceDetailDef.ItemDescription4 != (row.IsItemDescription4Null() ? string.Empty : row.ItemDescription4))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Item Description 4", (row.IsItemDescription4Null() ? string.Empty : row.ItemDescription4), invoiceDetailDef.ItemDescription4, userId));
                    if (invoiceDetailDef.ItemDescription5 != (row.IsItemDescription5Null() ? string.Empty : row.ItemDescription5))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Item Description 5", (row.IsItemDescription5Null() ? string.Empty : row.ItemDescription5), invoiceDetailDef.ItemDescription5, userId));
                    if (invoiceDetailDef.RechargeDCNoteId != (row.IsRechargeDCNoteIdNull() ? 0 : row.RechargeDCNoteId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Recharge debit note generated.", null, null, userId));
                    if (invoiceDetailDef.ItemNo != (row.IsItemNoNull() ? string.Empty : row.ItemNo))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Item No.", (row.IsItemNoNull() ? string.Empty : row.ItemNo), invoiceDetailDef.ItemNo, userId));
                    if (invoiceDetailDef.DevSampleCostTypeId != (row.IsDevSampleCostTypeIdNull() ? 0 : row.DevSampleCostTypeId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Dev Sample Cost Type", (row.IsDevSampleCostTypeIdNull() ? string.Empty : NTDevSampleCostType.getType(row.DevSampleCostTypeId).Description), (invoiceDetailDef.DevSampleCostTypeId <= 0 ? string.Empty : NTDevSampleCostType.getType(invoiceDetailDef.DevSampleCostTypeId).Description), userId));
                    if ((invoiceDetailDef.Company == null ? 0 : invoiceDetailDef.Company.Id) != (row.IsCompanyIdNull() ? 0 : row.CompanyId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Business Entity", (row.IsCompanyIdNull() ? null : CompanyType.getType(row.CompanyId).CompanyName), (invoiceDetailDef.Company == null ? string.Empty : invoiceDetailDef.Company.CompanyName), userId));
                    if ((invoiceDetailDef.RechargePartyDept == null ? 0 : invoiceDetailDef.RechargePartyDept.Id) != (row.IsRechargePartyDeptIdNull() ? 0 : row.RechargePartyDeptId))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Recharge Party Department", (row.IsRechargePartyDeptIdNull() ? string.Empty : NTRechargePartyDeptType.getType(row.RechargePartyDeptId).Description), (invoiceDetailDef.RechargePartyDept == null ? string.Empty : invoiceDetailDef.RechargePartyDept.Description), userId));
                    if (invoiceDetailDef.ContactPerson != (row.IsContactPersonNull() ? string.Empty : row.ContactPerson))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Contact Person", (row.IsContactPersonNull() ? string.Empty : row.ContactPerson), invoiceDetailDef.ContactPerson, userId));
                    if (invoiceDetailDef.RechargeContactPerson != (row.IsRechargeContactPersonNull() ? string.Empty : row.RechargeContactPerson))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Recharge Contact Person", (row.IsRechargeContactPersonNull() ? string.Empty : row.RechargeContactPerson), invoiceDetailDef.RechargeContactPerson, userId));
                    if ((invoiceDetailDef.SegmentValue7 == null ? 0 : invoiceDetailDef.SegmentValue7.SegmentValueId) != (row.IsSegmentValue7Null() ? 0 : row.SegmentValue7))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Segment Value 7", (row.IsSegmentValue7Null() ? string.Empty : getNTEpicorSegmentValueByKey(row.SegmentValue7).SegmentName), (invoiceDetailDef.SegmentValue7 == null ? string.Empty : invoiceDetailDef.SegmentValue7.SegmentName), userId));
                    if ((invoiceDetailDef.SegmentValue8 == null ? 0 : invoiceDetailDef.SegmentValue8.SegmentValueId) != (row.IsSegmentValue8Null() ? 0 : row.SegmentValue8))
                        amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, "Segment Value 8", (row.IsSegmentValue8Null() ? string.Empty : getNTEpicorSegmentValueByKey(row.SegmentValue8).SegmentName), (invoiceDetailDef.SegmentValue8 == null ? string.Empty : invoiceDetailDef.SegmentValue8.SegmentName), userId));


                    this.NTInvoiceDetailMapping(invoiceDetailDef, row);

                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.NTInvoiceDetail.NewNTInvoiceDetailRow();
                    dcNoteId = 0;
                    invoiceDetailDef.InvoiceDetailId = this.getMaxNTInvoiceDetailId() + 1;
                    this.NTInvoiceDetailMapping(invoiceDetailDef, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.NTInvoiceDetail.AddNTInvoiceDetailRow(row);

                    amendmentList.Add(getNewNTActionHistoryDef(invoiceDetailDef.InvoiceId, -1, string.Format("Add invoice detail {0}: {1} - {2}", (invoiceDetailDef.IsRecharge == 1 ? "(Recharge)" : ""), invoiceDetailDef.ExpenseType == null ? string.Empty : invoiceDetailDef.ExpenseType.ExpenseType, invoiceDetailDef.Amount.ToString()), null, null, userId));
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Non-trade Invoice ERROR");
                ctx.VoteCommit();
                return dcNoteId;
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


        public int getMaxNTRechargeDCNoteId()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTRechargeDCNoteApt", "GetMaxNTRechargeDCNoteId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);

        }

        public string fillNextRechargeDCNoteNo(DateTime issueDate, int officeId, string debitCreditIndicator)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                string noteNo = string.Empty;
                if (issueDate == DateTime.MinValue)
                    throw new ApplicationException("Invalid issue date");

                AccountFinancialCalenderDef calenderDef = generalWorker.getAccountPeriodByDate(9, issueDate);

                IDataSetAdapter ad = getDataSetAdapter("NTRechargeDCNoteNoParamApt", "GetRechargeDCNoteNoParamByKey");
                ad.SelectCommand.Parameters["@FiscalYear"].Value = calenderDef.BudgetYear;
                ad.SelectCommand.Parameters["@Period"].Value = calenderDef.Period;

                ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
                ad.SelectCommand.Parameters["@DebitCreditIndicator"].Value = debitCreditIndicator;

                NTRechargeDCNoteNoParamDs dataSet = new NTRechargeDCNoteNoParamDs();
                ad.SelectCommand.DbCommand.CommandTimeout = 120;
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected < 1)
                    throw new ApplicationException("Missing Recharge Note No. Param");

                NTRechargeDCNoteNoParamDs.NTRechargeDCNoteNoParamRow row = dataSet.NTRechargeDCNoteNoParam[0];

                noteNo = row.OfficePrefix + debitCreditIndicator + row.FiscalYear.ToString().Substring(2) + row.Period.ToString().PadLeft(2, '0') + row.SeqNo.ToString().PadLeft(3, '0') + "E";
                row.SeqNo += 1;
                recordsAffected = ad.Update(dataSet);
                ctx.VoteCommit();

                return noteNo;
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

        public NTRechargeDCNoteNoParamDs getRechargeDCNoteNoParam(DateTime issueDate, int officeId, string debitCreditIndicator)
        {
            try
            {
                if (issueDate == DateTime.MinValue)
                    throw new ApplicationException("Invalid issue date");

                AccountFinancialCalenderDef calenderDef = generalWorker.getAccountPeriodByDate(9, issueDate);

                IDataSetAdapter ad = getDataSetAdapter("NTRechargeDCNoteNoParamApt", "GetRechargeDCNoteNoParamByKey");
                ad.SelectCommand.Parameters["@FiscalYear"].Value = calenderDef.BudgetYear;
                ad.SelectCommand.Parameters["@Period"].Value = calenderDef.Period;

                ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
                ad.SelectCommand.Parameters["@DebitCreditIndicator"].Value = debitCreditIndicator;

                NTRechargeDCNoteNoParamDs dataSet = new NTRechargeDCNoteNoParamDs();
                ad.SelectCommand.DbCommand.CommandTimeout = 120;
                int recordsAffected = ad.Fill(dataSet);
                return dataSet;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                throw e;
            }
            finally
            {
            }
        }

        public NTRechargeDCNoteDef getNTRechargeDCNoteByDCNoteNo(string dcNoteNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTRechargeDCNoteApt", "GetNTRechargeDCNoteByDCNoteNo");
            ad.SelectCommand.Parameters["@DCNoteNo"].Value = dcNoteNo;
            NTRechargeDCNoteDs ds = new NTRechargeDCNoteDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTRechargeDCNoteDef def = new NTRechargeDCNoteDef();
            NTRechargeDCNoteMapping(ds.NTRechargeDCNote.Rows[0], def);
            return def;
        }

        public Hashtable getT0ListListFromSAFList(ArrayList safList)
        {
            Hashtable tbl = new Hashtable();
            foreach (SAFDef def in safList)
            {
                if (!tbl.ContainsKey(def.T0))
                    tbl.Add(def.T0, def.OfficeCode);
            }
            return tbl;
        }

        private Hashtable getSunDBListFromSAFList(ArrayList safList)
        {
            Hashtable tbl = new Hashtable();
            foreach (SAFDef def in safList)
            {
                string key = def.TargetDB + "-" + def.JournalType;
                if (!tbl.ContainsKey(key))
                    tbl.Add(key, key);
            }
            return tbl;
        }

        public ArrayList exportToSunInterfaceFileList(NTSunInterfaceQueueDef def, ArrayList safList, int sourceId)
        {
            string fileFolder = WebConfig.getValue("appSettings", "SUN_INTERFACE_OUTPUT_FOLDER");
            string macroFolder = WebConfig.getValue("appSettings", "SUN_INTERFACE_MACRO_FOLDER");
            string fileName = null; ;
            ArrayList returnedFileList = new ArrayList();
            int cnt = 1;

            if (sourceId == 1)
            {
                foreach (DictionaryEntry de in getT0ListListFromSAFList(safList))
                {
                    fileName = fileFolder + def.Office.OfficeCode + "_";

                    fileName += def.FiscalYear.ToString() + def.Period.ToString().PadLeft(2, '0') + "_" +
                                def.CategoryType.Name + "_" +
                                SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId) + "_" +
                                def.QueueId.ToString().PadLeft(10, '0') + "-" + de.Key + ".csv";

                    if (File.Exists(fileName)) File.Delete(fileName);

                    StreamWriter s = File.CreateText(fileName);

                    s.WriteLine("AccountCode,Filler1,AccountingPeriod,TransactionDate,Filler2,RecordType,JournalNo,JournalLineNo,baseamount,DCIndicator,AllocationIndicator,JournalType,JournalSource,TransactionReference,Filler3,Description,EntryDate,EntryPeriod,DueDate,Filler4,PaymentReference,PaymentDate,PaymentPeriod,AssetIndicator,AssetCode,AssetSubCode,ConversionCode,ConversionRate,OtherAmount,OtherAmountDecimal,ClearDownSequenceNo,Filler5,NextPeriodReversal,LossOrGain,RoughBookFlag,InUseFlag,T0,T1,T2,T3,T4,T5,T6,T7,T8,T9,PostingDate,UpdateOrderBalIndicator,AllocationInProgressMaker,JournalHoldReference,OperatorId,BudgetCheckAccount,Filler6,PaymentTerm,PayRefNo,OfficeCode,TradingAgency,AccruedSince");

                    int seqNo = 0;
                    string tmpRefPfx = string.Empty;

                    foreach (SAFDef safDef in safList)
                    {
                        if (de.Key.ToString() == safDef.T0)
                        {
                            if (tmpRefPfx != safDef.JournalType + safDef.PeriodString)
                                seqNo = accountWorker.getTransactionRefNo(safDef.JournalType + safDef.PeriodString);

                            safDef.TransactionRefSeqNo = seqNo;

                            tmpRefPfx = safDef.JournalType + safDef.PeriodString;

                            s.WriteLine(safDef.ToString());
                        }
                    }
                    s.Close();
                    File.Copy(fileName, fileFolder + "queue\\" + def.QueueId.ToString() + ".csv", true);
                    returnedFileList.Add(fileName);

                }


                /* spare copy nt */
                foreach (DictionaryEntry de in getSunDBListFromSAFList(safList))
                {
                    if (cnt == 1 || def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeBankPayment.GetHashCode())
                    {
                        fileName = macroFolder + "nt\\" + DateTime.Today.ToString("MMdd") + de.Key + "(NT)_" + def.QueueId.ToString() + ".ndf";

                        if (File.Exists(fileName)) File.Delete(fileName);

                        StreamWriter s = File.CreateText(fileName);

                        s.WriteLine("VERSION                         42601");

                        foreach (SAFDef safDef in safList)
                            if (de.Key.ToString() == safDef.TargetDB + "-" + safDef.JournalType) s.WriteLine(safDef.ToNDFString());
                        s.Close();
                    }
                    cnt++;
                }

            }
            else
            {
                foreach (DictionaryEntry de in getSunDBListFromSAFList(safList))
                {
                    int seqNo = 0;
                    string tmpRefPfx = string.Empty;

                    if (cnt == 1 || def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeBankPayment.GetHashCode())
                    {
                        fileName = macroFolder + "nt\\" + DateTime.Today.ToString("MMdd") + de.Key + "(NT)_" + def.QueueId.ToString() + ".ndf";

                        if (File.Exists(fileName)) File.Delete(fileName);

                        StreamWriter s = File.CreateText(fileName);

                        s.WriteLine("VERSION                         42601");

                        foreach (SAFDef safDef in safList)
                        {
                            if (de.Key.ToString() == safDef.TargetDB + "-" + safDef.JournalType)
                            {
                                if (tmpRefPfx != safDef.JournalType + safDef.PeriodString)
                                    seqNo = accountWorker.getTransactionRefNo(safDef.JournalType + safDef.PeriodString);

                                safDef.TransactionRefSeqNo = seqNo;

                                tmpRefPfx = safDef.JournalType + safDef.PeriodString;

                                s.WriteLine(safDef.ToNDFString());
                            }
                        }
                        s.Close();
                    }
                    cnt++;

                    returnedFileList.Add(fileName);
                }
            }

            return returnedFileList;
        }



        public void updateNTRechargeDCNote(NTRechargeDCNoteDef def, int officeId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTRechargeDCNoteApt", "GetNTRechargeDCNoteByKey");
                ad.SelectCommand.Parameters["@RechargeDCNoteId"].Value = def.RechargeDCNoteId;
                ad.PopulateCommands();

                NTRechargeDCNoteDs dataSet = new NTRechargeDCNoteDs();
                NTRechargeDCNoteDs.NTRechargeDCNoteRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTRechargeDCNote[0];
                    this.NTRechargeDCNoteMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.NTRechargeDCNote.NewNTRechargeDCNoteRow();
                    def.RechargeDCNoteId = this.getMaxNTRechargeDCNoteId() + 1;
                    def.RechargeDCNoteNo = fillNextRechargeDCNoteNo(DateTime.Today, officeId, def.DCIndicator);
                    this.NTRechargeDCNoteMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.NTRechargeDCNote.AddNTRechargeDCNoteRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Non-trade Recharge DC Note ERROR");
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

        public NTRechargeDCNoteDef getNTRechargeDCNoteByKey(int rechargeDCNoteId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTRechargeDCNoteApt", "GetNTRechargeDCNoteByKey");
            ad.SelectCommand.Parameters["@RechargeDCNoteId"].Value = rechargeDCNoteId.ToString();

            NTRechargeDCNoteDef def = null;
            NTRechargeDCNoteDs dataSet = new NTRechargeDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected > 0)
            {
                def = new NTRechargeDCNoteDef();
                NTRechargeDCNoteMapping(dataSet.NTRechargeDCNote.Rows[0], def);
            }
            return def;
        }

        //public ArrayList getNTRechargeDetailListByInvoiceId(int invoiceId)
        //{
        //    IDataSetAdapter ad = getDataSetAdapter("NTRechargeDetailApt", "GetNTRechargeDetailListByInvoiceId");
        //    ad.SelectCommand.Parameters["@InvoiceId"].Value = invoiceId.ToString();

        //    NTRechargeDetailDs dataSet = new NTRechargeDetailDs();
        //    int recordsAffected = ad.Fill(dataSet);

        //    ArrayList list = new ArrayList();

        //    foreach (NTRechargeDetailDs.NTRechargeDetailRow row in dataSet.NTRechargeDetail)
        //    {
        //        NTRechargeDetailDef def = new NTRechargeDetailDef();
        //        NTRechargeDetailMapping(row, def);
        //        list.Add(def);
        //    }
        //    return list;
        //}


        //public void UpdateNTRechargeDetail(NTRechargeDetailDef def, int userId)
        //{
        //    TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
        //    try
        //    {
        //        ctx.Enter();
        //        TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

        //        IDataSetAdapter ad = getDataSetAdapter("NTRechargeDetailApt", "GetNTRechargeDetailByKey");
        //        ad.SelectCommand.Parameters["@RechargeDetailId"].Value = def.RechargeDetailId;
        //        ad.PopulateCommands();

        //        NTRechargeDetailDs dataSet = new NTRechargeDetailDs();
        //        NTRechargeDetailDs.NTRechargeDetailRow row = null;

        //        int recordsAffected = ad.Fill(dataSet);
        //        if (recordsAffected > 0)
        //        {
        //            row = dataSet.NTRechargeDetail[0];
        //            this.NTRechargeDetailMapping(def, row);
        //            sealStamp(row, userId, Stamp.UPDATE);
        //        }
        //        else
        //        {
        //            row = dataSet.NTRechargeDetail.NewNTRechargeDetailRow();
        //            def.RechargeDetailId = this.getMaxNTRechargeDetailId() + 1;
        //            this.NTRechargeDetailMapping(def, row);
        //            sealStamp(row, userId, Stamp.INSERT);
        //            dataSet.NTRechargeDetail.AddNTRechargeDetailRow(row);
        //        }
        //        recordsAffected = ad.Update(dataSet);
        //        if (recordsAffected < 1)
        //            throw new DataAccessException("Update Non-trade Invoice ERROR");
        //        ctx.VoteCommit();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("ERROR " + e.Message);
        //        ctx.VoteRollback();
        //        throw e;
        //    }
        //    finally
        //    {
        //        ctx.Exit();
        //    }

        //}

        public ArrayList getCurrentMonthEndStatusList(TypeCollector officeList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTMonthEndStatusApt", "GetCurrentNTMonthEndStatusList");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);

            NTMonthEndStatusDs dataSet = new NTMonthEndStatusDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTMonthEndStatusDs.NTMonthEndStatusRow row in dataSet.NTMonthEndStatus)
            {
                NTMonthEndStatusDef def = new NTMonthEndStatusDef();
                NTMonthEndStatusMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<NTMonthEndStatusDef> getActiveNTMonthEndStatusList(int officeId, int fiscalYear, int period)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTMonthEndStatusApt", "GetActiveNTMonthEndStatusList");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@Period"].Value = period;

            NTMonthEndStatusDs ds = new NTMonthEndStatusDs();
            int recordsAffected = ad.Fill(ds);

            List<NTMonthEndStatusDef> list = new List<NTMonthEndStatusDef>();

            foreach (NTMonthEndStatusDs.NTMonthEndStatusRow row in ds.NTMonthEndStatus)
            {
                NTMonthEndStatusDef def = new NTMonthEndStatusDef();
                NTMonthEndStatusMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public NTMonthEndStatusDef getNTMonthEndStatusByOfficeIdAndPeriod(int officeId, int fiscalYear, int period)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTMonthEndStatusApt", "GetNTMonthEndStatusByOfficeIdAndPeriod");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@FiscalPeriod"].Value = period;

            NTMonthEndStatusDs ds = new NTMonthEndStatusDs();
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected < 1) return null;

            NTMonthEndStatusDef def = new NTMonthEndStatusDef();
            NTMonthEndStatusMapping(ds.NTMonthEndStatus.Rows[0], def);
            return def;
        }


        public void updateNTMonthEndStatus(NTMonthEndStatusDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTMonthEndStatusApt", "GetNTMonthEndStatusByKey");
                ad.SelectCommand.Parameters["@RecordId"].Value = def.RecordId;
                ad.PopulateCommands();

                NTMonthEndStatusDs dataSet = new NTMonthEndStatusDs();
                NTMonthEndStatusDs.NTMonthEndStatusRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTMonthEndStatus[0];
                    this.NTMonthEndStatusMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    throw new DataAccessException("Update Non-trade Month End Status ERROR");
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Non-trade Recharge DC Note ERROR");
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

        #region NT Approver
        public NTApproverDef getNTApproverByKey(int officeId, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTApproverApt", "GetNTApproverByKey");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;

            NTApproverDs ds = new NTApproverDs();
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected < 1) return null;

            NTApproverDef def = new NTApproverDef();
            NTApproverMapping(ds.NTApprover.Rows[0], def);
            return def;
        }

        public ArrayList getNTApproverListByOfficeId(TypeCollector officeIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTApproverApt", "GetNTApproverListByOfficeId");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);

            NTApproverDs dataSet = new NTApproverDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTApproverDs.NTApproverRow row in dataSet.NTApprover)
            {
                NTApproverDef def = new NTApproverDef();
                NTApproverMapping(row, def);
                list.Add(def);
            }
            return list;

        }


        public void updateNTApprover(NTApproverDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTApproverApt", "GetNTApproverByKey");
                ad.SelectCommand.Parameters["@OfficeId"].Value = def.Office.OfficeId;
                ad.SelectCommand.Parameters["@UserId"].Value = def.Approver.UserId;
                ad.PopulateCommands();

                NTApproverDs dataSet = new NTApproverDs();
                NTApproverDs.NTApproverRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTApprover[0];

                    this.NTApproverMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.NTApprover.NewNTApproverRow();
                    this.NTApproverMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.NTApprover.AddNTApproverRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Non-trade Approver ERROR");
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


        #endregion



        #region NTActionHistory

        public void updateNTActionHistory(int invoiceId, int ntVendorId, string description, int userId)
        {
            updateNTActionHistory(new NTActionHistoryDef(invoiceId, ntVendorId, description, userId));
        }

        public void updateNTActionHistory(NTActionHistoryDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (def.ActionHistoryId <= 0)
                    for (int i = 0; i < 10; i++)
                        if ((def.ActionHistoryId = createNewNTActionHistoryRecord()) > 0) break;

                IDataSetAdapter ad = getDataSetAdapter("NTActionHistoryApt", "GetNTActionHistoryByKey");
                ad.SelectCommand.Parameters["@ActionHistoryId"].Value = def.ActionHistoryId;
                ad.PopulateCommands();

                NTActionHistoryDs dataSet = new NTActionHistoryDs();
                NTActionHistoryDs.NTActionHistoryRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTActionHistory[0];
                    this.NTActionHistoryMapping(def, row);
                    if (def.ActionOn == null || def.ActionOn == DateTime.MinValue)
                        def.ActionOn = DateTime.Now;
                    recordsAffected = ad.Update(dataSet);
                }
                if (recordsAffected < 1)
                    throw new DataAccessException("Update NT Action History ERROR");
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

        private NTActionHistoryDef getNewNTActionHistoryDef(int invoiceId, int ntVendorId, string description, string sourceValue, string targetValue, int userId)
        {
            if (!string.IsNullOrEmpty(sourceValue) || !string.IsNullOrEmpty(targetValue))
                description += " : " + (sourceValue == null ? string.Empty : sourceValue) + " -> " + (targetValue == null ? string.Empty : targetValue);
            return new NTActionHistoryDef(invoiceId, ntVendorId, description, userId);
        }

        private int createNewNTActionHistoryRecord()
        {
            try
            {
                IDataSetAdapter ad = getDataSetAdapter("NTActionHistoryApt", "GetNTActionHistoryByKey");
                ad.PopulateCommands();

                NTActionHistoryDs dataSet = new NTActionHistoryDs();
                NTActionHistoryDs.NTActionHistoryRow row = null;
                row = dataSet.NTActionHistory.NewNTActionHistoryRow();

                row.ActionHistoryId = getMaxNTActionHistoryId() + 1;
                row.InvoiceId = -1;
                row.NTVendorId = -1;
                row.ActionBy = -1;
                row.ActionOn = DateTime.Now;
                row.Status = -1;
                dataSet.NTActionHistory.AddNTActionHistoryRow(row);
                return (ad.Update(dataSet) == 1 ? row.ActionHistoryId : -1);
            }
            catch //(Exception e)
            {
                return -1;
            }
        }

        public ArrayList getNTActionHistoryList(int invoiceId, int ntVendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTActionHistoryApt", "GetNTActionHistoryList");
            ad.SelectCommand.Parameters["@InvoiceId"].Value = invoiceId.ToString();
            ad.SelectCommand.Parameters["@NTVendorId"].Value = ntVendorId.ToString();

            NTActionHistoryDs ds = new NTActionHistoryDs();
            int recordsAffected = ad.Fill(ds);

            ArrayList list = new ArrayList();

            foreach (NTActionHistoryDs.NTActionHistoryRow row in ds.NTActionHistory)
            {
                NTActionHistoryDef def = new NTActionHistoryDef();
                NTActionHistoryMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        private int getMaxNTActionHistoryId()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTActionHistoryApt", "GetMaxNTActionHistoryId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public NTSunInterfaceQueueDef getNTSunInterfaceQueueByKey(int queueId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTSunInterfaceQueueApt", "GetNTSunInterfaceQueueByKey");
            ad.SelectCommand.Parameters["@QueueId"].Value = queueId;
            NTSunInterfaceQueueDs dataSet = new NTSunInterfaceQueueDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
            {
                NTSunInterfaceQueueDs.NTSunInterfaceQueueRow row = dataSet.NTSunInterfaceQueue[0];
                NTSunInterfaceQueueDef def = new NTSunInterfaceQueueDef();
                this.NTSunInterfaceQueueMapping(row, def);
                return def;
            }
            else
                return null;
        }

        public void updateNTSunInterfaceQueue(NTSunInterfaceQueueDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTSunInterfaceQueueApt", "GetNTSunInterfaceQueueByKey");
                ad.SelectCommand.Parameters["@QueueId"].Value = def.QueueId;
                ad.PopulateCommands();

                NTSunInterfaceQueueDs dataSet = new NTSunInterfaceQueueDs();
                NTSunInterfaceQueueDs.NTSunInterfaceQueueRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTSunInterfaceQueue[0];
                    this.NTSunInterfaceQueueMapping(def, row);
                }
                else
                {
                    row = dataSet.NTSunInterfaceQueue.NewNTSunInterfaceQueueRow();
                    def.QueueId = this.getMaxNTSunInterfaceQueueId() + 1;
                    this.NTSunInterfaceQueueMapping(def, row);
                    dataSet.NTSunInterfaceQueue.AddNTSunInterfaceQueueRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update NT Sun Interface Queue ERROR");
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

        public void updateNTUserRoleAccess(NTUserRoleAccessDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("NTUserRoleAccessApt", "GetNTUserRoleAccessByKey");
                ad.SelectCommand.Parameters["@OfficeId"].Value = def.OfficeId;
                ad.SelectCommand.Parameters["@CompanyId"].Value = def.CompanyId;
                ad.SelectCommand.Parameters["@UserId"].Value = def.UserId;
                ad.SelectCommand.Parameters["@RoleId"].Value = def.RoleId;
                ad.PopulateCommands();

                NTUserRoleAccesssDs dataSet = new NTUserRoleAccesssDs();
                NTUserRoleAccesssDs.NTUserRoleAccessRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.NTUserRoleAccess[0];
                    this.NTUserRoleAccessMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.NTUserRoleAccess.NewNTUserRoleAccessRow(); ;
                    this.NTUserRoleAccessMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.NTUserRoleAccess.AddNTUserRoleAccessRow(row); ;
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update NT UserRoleAccess ERROR");
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

        private int getMaxNTSunInterfaceQueueId()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTSunInterfaceQueueApt", "GetMaxNTSunInterfaceQueueId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public ArrayList getInterfaceData(NTSunInterfaceQueueDef queueDef)
        {
            string commandName = null;

            if (queueDef.CategoryType.Id == CategoryType.ACTUAL.Id)
                commandName = SunInterfaceTypeRef.getActualDataCommandName(queueDef.SunInterfaceTypeId);
            else if (queueDef.CategoryType.Id == CategoryType.ACCRUAL.Id)
                commandName = SunInterfaceTypeRef.getAccrualDataCommandName(queueDef.SunInterfaceTypeId);

            if (commandName == "N/A")
            {
                throw new ApplicationException("Sun Interface Data Command was not specified");
            }

            IDataSetAdapter ad = getDataSetAdapter("SunInterfaceLogApt", commandName);
            SunInterfaceLogDs dataSet = new SunInterfaceLogDs();
            ad.SelectCommand.MailSQL = true;
            ad.SelectCommand.Parameters["@OfficeGroupId"].Value = queueDef.Office.OfficeId;
            ad.SelectCommand.Parameters["@SunInterfaceTypeId"].Value = queueDef.SunInterfaceTypeId;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = queueDef.FiscalYear;
            ad.SelectCommand.Parameters["@Period"].Value = queueDef.Period;
            ad.SelectCommand.Parameters["@CategoryId"].Value = queueDef.CategoryType.Id;
            ad.SelectCommand.Parameters["@UserId"].Value = queueDef.User.UserId;

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (SunInterfaceLogDs.SunInterfaceLogRow row in dataSet.SunInterfaceLog)
            {
                SunInterfaceLogDef def = new SunInterfaceLogDef();
                accountWorker.SunInterfaceLogMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public ArrayList getNTSunInterfaceQueueList()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTSunInterfaceQueueApt", "GetNTSunInterfaceQueueList");
            NTSunInterfaceQueueDs dataSet = new NTSunInterfaceQueueDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTSunInterfaceQueueDs.NTSunInterfaceQueueRow row in dataSet.NTSunInterfaceQueue)
            {
                NTSunInterfaceQueueDef def = new NTSunInterfaceQueueDef();
                NTSunInterfaceQueueMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getNTSunInterfaceQueueList(int officeId, int fiscalYear, int period, int sunInterfaceTypeId, int categoryId, int sourceId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTSunInterfaceQueueApt", "GetNTSunInterfaceQueueByCriteria");
            NTSunInterfaceQueueDs dataSet = new NTSunInterfaceQueueDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@Period"].Value = period;
            ad.SelectCommand.Parameters["@CategoryId"].Value = categoryId;
            ad.SelectCommand.Parameters["@SunInterfaceTypeId"].Value = sunInterfaceTypeId;
            ad.SelectCommand.Parameters["@SourceId"].Value = sourceId;

            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTSunInterfaceQueueDs.NTSunInterfaceQueueRow row in dataSet.NTSunInterfaceQueue)
            {
                NTSunInterfaceQueueDef def = new NTSunInterfaceQueueDef();
                NTSunInterfaceQueueMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getRecentNTSunInterfaceQueueList()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTSunInterfaceQueueApt", "GetRecentNTSunInterfaceQueueList");
            NTSunInterfaceQueueDs dataSet = new NTSunInterfaceQueueDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTSunInterfaceQueueDs.NTSunInterfaceQueueRow row in dataSet.NTSunInterfaceQueue)
            {
                NTSunInterfaceQueueDef def = new NTSunInterfaceQueueDef();
                NTSunInterfaceQueueMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        #endregion

        #region Mapping functions

        private void NTExpenseTypeMapping(object source, object target)
        {
            if (source.GetType() == typeof(NTExpenseTypeDs.NTExpenseTypeRow) &&
                target.GetType() == typeof(NTExpenseTypeRef))
            {
                NTExpenseTypeDs.NTExpenseTypeRow row = (NTExpenseTypeDs.NTExpenseTypeRow)source;
                NTExpenseTypeRef def = (NTExpenseTypeRef)target;
                def.ExpenseTypeId = row.ExpenseTypeId;
                def.ExpenseType = row.ExpenseType;
                def.SUNAccountCode = row.SUNAccountCode;
                def.EpicorCode = row.IsEpicorCodeNull() ? string.Empty : row.EpicorCode;
                def.IsOfficeCode = row.IsOfficeCode;
                def.IsDepartmentCode = row.IsDepartmentCode;
                def.IsProductCode = row.IsProductCode;
                def.IsSeasonCode = row.IsSeasonCode;
                def.IsStaffCode = row.IsStaffCode;
                def.IsItemNo = row.IsItemNo;
                def.IsDevSampleCostType = row.IsDevSampleCostType;
                def.IsQtyRequired = row.IsQtyRequired;
                def.OfficeId = row.OfficeId;
                def.IsAllowAccrual = row.IsAllowAccrual;
                def.ItemDescriptionHints = row.IsItemDescriptionHintsNull() ? string.Empty : row.ItemDescriptionHints;
                def.SUNDescription = row.IsSUNDescriptionNull() ? string.Empty : row.SUNDescription;
                def.TallyCode = row.IsTallyCodeNull() ? string.Empty : row.TallyCode;
                def.NatureId = row.IsNatureIdNull() ? -1 : row.NatureId;
                def.Status = row.Status;
                def.isOtherCost = row.IsOtherCost;
                def.IsSegmentValue = row.IsSegmentValue;
            }
            else
                if (source.GetType() == typeof(NTExpenseTypeRef) &&
                target.GetType() == typeof(NTExpenseTypeDs.NTExpenseTypeRow))
                {
                    NTExpenseTypeDs.NTExpenseTypeRow row = (NTExpenseTypeDs.NTExpenseTypeRow)target;
                    NTExpenseTypeRef def = (NTExpenseTypeRef)source;
                    row.ExpenseTypeId = def.ExpenseTypeId;
                    row.ExpenseType = def.ExpenseType;
                    row.SUNAccountCode = def.SUNAccountCode;
                    row.IsOfficeCode = def.IsOfficeCode;
                    row.IsDepartmentCode = def.IsDepartmentCode;
                    row.IsProductCode = def.IsProductCode;
                    row.IsSeasonCode = def.IsSeasonCode;
                    row.IsStaffCode = def.IsStaffCode;
                    row.IsItemNo = def.IsItemNo;
                    row.IsDevSampleCostType = def.IsDevSampleCostType;
                    row.IsQtyRequired = def.IsQtyRequired;
                    row.OfficeId = def.OfficeId;
                    row.IsAllowAccrual = def.IsAllowAccrual;
                    row.ItemDescriptionHints = def.ItemDescriptionHints;
                    row.SUNDescription = def.SUNDescription;
                    if (def.TallyCode.Trim() == string.Empty)
                        row.SetTallyCodeNull();
                    else
                        row.TallyCode = def.TallyCode;
                    if (def.NatureId == -1)
                        row.SetNatureIdNull();
                    else
                        row.NatureId = def.NatureId;
                    row.Status = def.Status;
                    row.IsOtherCost = def.isOtherCost;
                    row.IsSegmentValue = def.IsSegmentValue;
                }

        }


        private void NTVendorMapping(object source, object target)
        {
            if (source.GetType() == typeof(NTVendorDs.NTVendorRow) &&
                target.GetType() == typeof(NTVendorDef))
            {
                NTVendorDs.NTVendorRow row = (NTVendorDs.NTVendorRow)source;
                NTVendorDef def = (NTVendorDef)target;
                def.NTVendorId = row.NTVendorId;
                def.VendorName = row.VendorName;
                def.OtherName = ((!row.IsOtherNameNull()) ? row.OtherName : string.Empty);
                def.ContactPerson = ((!row.IsContactPersonNull()) ? row.ContactPerson : string.Empty);
                def.Email = ((!row.IsEmailNull()) ? row.Email : string.Empty);
                def.BankAccountNo = ((!row.IsBankAccountNoNull()) ? row.BankAccountNo : string.Empty);
                def.Address = ((!row.IsAddressNull()) ? row.Address : string.Empty);
                def.SUNAccountCode = ((!row.IsSUNAccountCodeNull()) ? row.SUNAccountCode : string.Empty);
                def.PaymentMethod = NTPaymentMethodRef.getType(!row.IsPaymentMethodIdNull() ? row.PaymentMethodId : -1);
                def.Currency = generalWorker.getCurrencyByKey(!row.IsCurrencyIdNull() ? row.CurrencyId : -1);
                def.Country = generalWorker.getCountryByKey(!row.IsCountryIdNull() ? row.CountryId : -1);
                def.ExpenseType = getNTExpenseTypeByKey(!row.IsExpenseTypeIdNull() ? row.ExpenseTypeId : 0);
                def.Telephone = ((!row.IsTelephoneNull()) ? row.Telephone : string.Empty);
                def.Office = generalWorker.getOfficeRefByKey(!row.IsOfficeIdNull() ? row.OfficeId : -1);
                def.Fax = ((!row.IsFaxNull()) ? row.Fax : string.Empty);
                def.TallyCode = ((!row.IsTallyCodeNull()) ? row.TallyCode : string.Empty);
                def.PaymentTermDays = row.IsPaymentTermDaysNull() ? 0 : row.PaymentTermDays;
                def.Remark = row.IsRemarkNull() ? string.Empty : row.Remark;
                def.IsCustomerNoRequired = row.IsCustomerNoRequired;
                def.IsInvoiceNoRequired = row.IsInvoiceNoRequired;
                def.EPVendorCode = row.IsEPVendorCodeNull() ? string.Empty : row.EPVendorCode;
                def.VendorTypeId = row.IsVendorTypeIdNull() ? 0 : row.VendorTypeId;
                def.AmendmentDetail = row.IsAmendmentDetailNull() ? string.Empty : row.AmendmentDetail;
                def.ReviewedBy = row.IsReviewedByNull() ? null : generalWorker.getUserByKey(row.ReviewedBy);
                def.ReviewedOn = row.IsReviewedOnNull() ? DateTime.MinValue : row.ReviewedOn;
                def.ConsumptionUnitId = row.IsConsumptionUnitIdNull() ? -1 : row.ConsumptionUnitId;
                def.UtilityProviderTypeId = row.IsUtilityProviderTypeIdNull() ? -1 : row.UtilityProviderTypeId;
                def.CompanyId = row.IsCompanyIdNull() ? -1 : row.CompanyId;
                def.WorkflowStatus = NTVendorWFS.getType(row.WorkflowStatusId);
                def.CreatedBy = generalWorker.getUserByKey(row.CreatedBy);
                def.CreatedOn = row.CreatedOn;
                def.Status = row.Status;
                def.EAdviceEmail = ((!row.IsEAdviceEmailNull()) ? row.EAdviceEmail : string.Empty);
            }
            else if (source.GetType() == typeof(NTVendorDef) &&
                target.GetType() == typeof(NTVendorDs.NTVendorRow))
            {
                NTVendorDs.NTVendorRow row = (NTVendorDs.NTVendorRow)target;
                NTVendorDef def = (NTVendorDef)source;
                row.NTVendorId = (def.NTVendorId != int.MinValue ? def.NTVendorId : -1);
                row.VendorName = (!string.IsNullOrEmpty(def.VendorName) ? def.VendorName : string.Empty);
                row.OtherName = (!string.IsNullOrEmpty(def.OtherName) ? def.OtherName : string.Empty);
                row.ContactPerson = (!string.IsNullOrEmpty(def.ContactPerson) ? def.ContactPerson : string.Empty);
                row.Email = (!string.IsNullOrEmpty(def.Email) ? def.Email : string.Empty);
                row.BankAccountNo = (!string.IsNullOrEmpty(def.BankAccountNo) ? def.BankAccountNo : string.Empty);
                if (!string.IsNullOrEmpty(def.Address))
                    row.Address = def.Address;
                else
                    row.SetAddressNull();
                if (!string.IsNullOrEmpty(def.SUNAccountCode))
                    row.SUNAccountCode = def.SUNAccountCode;
                else
                    row.SetSUNAccountCodeNull();
                if (def.PaymentMethod != null)
                    row.PaymentMethodId = def.PaymentMethod.Id;
                else
                    row.SetPaymentMethodIdNull();
                row.PaymentTermDays = def.PaymentTermDays;
                if (def.Currency != null)
                    row.CurrencyId = def.Currency.CurrencyId;
                else
                    row.SetCurrencyIdNull();
                if (def.Country != null)
                    row.CountryId = def.Country.CountryId;
                else
                    row.SetCountryIdNull();
                if (def.ExpenseType != null)
                    row.ExpenseTypeId = def.ExpenseType.ExpenseTypeId;
                else
                    row.SetExpenseTypeIdNull();
                if (!string.IsNullOrEmpty(def.Telephone))
                    row.Telephone = def.Telephone;
                else
                    row.SetTelephoneNull();
                if (def.Office != null)
                    row.OfficeId = def.Office.OfficeId;
                else
                    row.SetOfficeIdNull();
                if (!string.IsNullOrEmpty(def.Fax))
                    row.Fax = def.Fax;
                else
                    row.SetFaxNull();
                if (!string.IsNullOrEmpty(def.TallyCode))
                    row.Fax = def.TallyCode;
                else
                    row.SetTallyCodeNull();
                row.Status = def.Status;
                row.IsInvoiceNoRequired = def.IsInvoiceNoRequired;
                row.IsCustomerNoRequired = def.IsCustomerNoRequired;
                if (string.IsNullOrEmpty(def.EPVendorCode))
                    row.SetEPVendorCodeNull();
                else
                    row.EPVendorCode = def.EPVendorCode;
                row.VendorTypeId = def.VendorTypeId;
                if (def.AmendmentDetail == string.Empty)
                    row.SetAmendmentDetailNull();
                else
                    row.AmendmentDetail = def.AmendmentDetail;
                if (def.ReviewedOn == DateTime.MinValue)
                    row.SetReviewedOnNull();
                else
                    row.ReviewedOn = def.ReviewedOn;
                if (def.ReviewedBy == null)
                    row.SetReviewedByNull();
                else
                    row.ReviewedBy = def.ReviewedBy.UserId;
                if (def.ConsumptionUnitId == -1)
                    row.SetConsumptionUnitIdNull();
                else
                    row.ConsumptionUnitId = def.ConsumptionUnitId;
                if (def.UtilityProviderTypeId == -1)
                    row.SetUtilityProviderTypeIdNull();
                else
                    row.UtilityProviderTypeId = def.UtilityProviderTypeId;

                if (def.CompanyId == -1)
                    row.SetCompanyIdNull();
                else
                    row.CompanyId = def.CompanyId;

                row.WorkflowStatusId = def.WorkflowStatus.Id;
                row.EAdviceEmail = (!string.IsNullOrEmpty(def.EAdviceEmail) ? def.EAdviceEmail : string.Empty);
            }
        }

        private void NTActionHistoryMapping(object source, object target)
        {
            if (source.GetType() == typeof(NTActionHistoryDef) &&
                target.GetType() == typeof(NTActionHistoryDs.NTActionHistoryRow))
            {
                NTActionHistoryDs.NTActionHistoryRow row = (NTActionHistoryDs.NTActionHistoryRow)target;
                NTActionHistoryDef def = (NTActionHistoryDef)source;
                row.ActionHistoryId = def.ActionHistoryId;
                if (def.InvoiceId > 0)
                    row.InvoiceId = def.InvoiceId;
                else
                    row.SetInvoiceIdNull();
                if (def.NTVendorId > 0)
                    row.NTVendorId = def.NTVendorId;
                else
                    row.SetNTVendorIdNull();
                row.Description = (def.Description != null ? (def.Description.Length > 200 ? def.Description.Substring(0, 200) : def.Description) : string.Empty);
                row.ActionBy = def.ActionBy;
                row.ActionOn = def.ActionOn;
                row.Status = def.Status;
            }
            else if (source.GetType() == typeof(NTActionHistoryDs.NTActionHistoryRow) &&
                target.GetType() == typeof(NTActionHistoryDef))
            {
                NTActionHistoryDs.NTActionHistoryRow row = (NTActionHistoryDs.NTActionHistoryRow)source;
                NTActionHistoryDef def = (NTActionHistoryDef)target;

                def.ActionHistoryId = row.ActionHistoryId;
                def.InvoiceId = row.IsInvoiceIdNull() ? -1 : row.InvoiceId;
                def.NTVendorId = row.IsNTVendorIdNull() ? -1 : row.NTVendorId;
                def.ActionBy = row.ActionBy;
                def.ActionOn = row.ActionOn;
                def.Description = row.Description;
                def.Status = row.Status;
            }
        }

        private void NTVendorExpenseTypeMapping(object source, object target)
        {
            if (source.GetType() == typeof(NTVendorExpenseTypeMappingDef) &&
                target.GetType() == typeof(NTVendorExpenseTypeMappingDs.NTVendorExpenseTypeMappingRow))
            {
                NTVendorExpenseTypeMappingDs.NTVendorExpenseTypeMappingRow row = (NTVendorExpenseTypeMappingDs.NTVendorExpenseTypeMappingRow)target;
                NTVendorExpenseTypeMappingDef def = (NTVendorExpenseTypeMappingDef)source;

                row.NTVendorId = def.NTVendorId;
                row.ExpenseTypeId = def.ExpenseType.ExpenseTypeId;
                row.Status = def.Status;
            }
            else if (source.GetType() == typeof(NTVendorExpenseTypeMappingDs.NTVendorExpenseTypeMappingRow) &&
                target.GetType() == typeof(NTVendorExpenseTypeMappingDef))
            {
                NTVendorExpenseTypeMappingDs.NTVendorExpenseTypeMappingRow row = (NTVendorExpenseTypeMappingDs.NTVendorExpenseTypeMappingRow)source;
                NTVendorExpenseTypeMappingDef def = (NTVendorExpenseTypeMappingDef)target;

                def.NTVendorId = row.NTVendorId;
                def.ExpenseType = getNTExpenseTypeByKey(row.ExpenseTypeId);
                def.Status = row.Status;
            }
        }


        private void NTInvoiceMapping(object source, object target)
        {
            if (source.GetType() == typeof(NTInvoiceDs.NTInvoiceRow) &&
                target.GetType() == typeof(NTInvoiceDef))
            {
                NTInvoiceDs.NTInvoiceRow row = (NTInvoiceDs.NTInvoiceRow)source;
                NTInvoiceDef def = (NTInvoiceDef)target;
                def.InvoiceId = row.InvoiceId;
                def.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
                def.Company = row.IsCompanyIdNull() ? null : CompanyType.getType(row.CompanyId);
                def.Dept = generalWorker.getDepartmentByKey(row.DeptId);
                def.NTVendor = getNTVendorByKey(row.NTVendorId);
                def.CustomerNo = (!row.IsCustomerNoNull() ? row.CustomerNo : string.Empty);
                def.InvoiceNo = (!row.IsInvoiceNoNull() ? row.InvoiceNo : string.Empty);
                def.InvoiceDate = row.InvoiceDate;
                def.NSLInvoiceNo = row.NSLInvoiceNo;
                def.DueDate = (!row.IsDueDateNull() ? row.DueDate : DateTime.MinValue);
                def.PaymentFromDate = row.PaymentFromDate;
                def.PaymentToDate = row.PaymentToDate;
                def.InvoiceReceivedDate = row.InvoiceReceivedDate;
                def.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                def.PaymentMethod = NTPaymentMethodRef.getType(row.PaymentMethodId);
                def.Amount = row.Amount;
                def.TotalVAT = row.TotalVAT;
                def.SettlementDate = (!row.IsSettlementDateNull() ? row.SettlementDate : DateTime.MinValue);
                def.SettlementAmount = (!row.IsSettlementAmountNull() ? row.SettlementAmount : 0);
                def.SettlementRefNo = (!row.IsSettlementRefNoNull() ? row.SettlementRefNo : string.Empty);
                if (!row.IsSettlementBankAccountIdNull())
                    def.SettlementBankAccountId = row.SettlementBankAccountId;
                else
                    def.SettlementBankAccountId = int.MinValue;
                if (!row.IsUpdateSettlementUserIdNull())
                    def.UpdateSettlementUserId = row.UpdateSettlementUserId;
                else
                    def.UpdateSettlementUserId = int.MinValue;
                if (!row.IsChequeNoNull())
                    def.ChequeNo = row.ChequeNo;
                else
                    def.ChequeNo = string.Empty;
                def.IsPayByHK = row.IsPayByHK;
                def.WorkflowStatus = NTWFS.getType(row.WorkflowStatusId);
                def.RejectReason = (!row.IsRejectReasonNull() ? row.RejectReason : string.Empty);
                def.DCIndicator = row.DCIndicator;
                def.IsSUNInterfaced = row.IsSUNInterfaced;
                def.IsSUNInterfacedForSettlement = row.IsSUNInterfacedForSettlement;
                def.FiscalYear = row.IsFiscalYearNull() ? 0 : row.FiscalYear;
                def.FiscalPeriod = row.IsFiscalPeriodNull() ? 0 : row.FiscalPeriod;
                def.SUNInterfaceDate = row.IsSUNInterfaceDateNull() ? DateTime.MinValue : row.SUNInterfaceDate;
                def.JournalNo = row.IsJournalNoNull() ? string.Empty : row.JournalNo;
                def.ReleaseReason = row.IsReleaseReasonNull() ? string.Empty : row.ReleaseReason;
                def.UserRemark = (!row.IsUserRemarkNull() ? row.UserRemark : string.Empty);
                def.SubmittedBy = (!row.IsSubmittedByNull() ? generalWorker.getUserByKey(row.SubmittedBy) : null);
                def.SubmittedOn = (!row.IsSubmittedOnNull() ? row.SubmittedOn : DateTime.MinValue);
                def.Approver = (!row.IsApproverIdNull() ? generalWorker.getUserByKey(row.ApproverId) : null);
                def.AccountFirstApproverId = (!row.IsAccountFirstApproverIdNull() ? row.AccountFirstApproverId : -1);
                def.AccountFirstApprovedOn = (!row.IsAccountFirstApprovedOnNull() ? row.AccountFirstApprovedOn : DateTime.MinValue);
                def.AccountSecondApproverId = (!row.IsAccountSecondApproverIdNull() ? row.AccountSecondApproverId : -1);
                def.AccountSecondApprovedOn = (!row.IsAccountSecondApprovedOnNull() ? row.AccountSecondApprovedOn : DateTime.MinValue);
                def.BankCharge = (!row.IsBankChargeNull() ? row.BankCharge : 0);
                def.CreatedBy = generalWorker.getUserByKey(row.CreatedBy);
                def.CreatedOn = row.CreatedOn;
                def.ModifiedBy = (!row.IsModifiedByNull() ? generalWorker.getUserByKey(row.ModifiedBy) : null);
                def.ModifiedOn = (!row.IsModifiedOnNull() ? row.ModifiedOn : DateTime.MinValue);
                def.Status = row.Status;
                def.ProcurementRequestId = (!row.IsProcurementRequestIdNull() ? row.ProcurementRequestId : -1);
                def.LogoInterfaceDate = row.IsLogoInterfaceDateNull() ? DateTime.MinValue : row.LogoInterfaceDate;
            }
            else if (source.GetType() == typeof(NTInvoiceDef) &&
                target.GetType() == typeof(NTInvoiceDs.NTInvoiceRow))
            {
                NTInvoiceDs.NTInvoiceRow row = (NTInvoiceDs.NTInvoiceRow)target;
                NTInvoiceDef def = (NTInvoiceDef)source;

                row.InvoiceId = def.InvoiceId;
                row.OfficeId = def.Office.OfficeId;
                if (def.Company != null)
                    row.CompanyId = def.Company.Id;
                else
                    row.SetCompanyIdNull();
                if (def.Dept != null)
                    row.DeptId = def.Dept.DepartmentId;
                else
                    row.SetDeptIdNull();
                row.NTVendorId = def.NTVendor.NTVendorId;
                if (def.CustomerNo != null)
                    row.CustomerNo = def.CustomerNo;
                else
                    row.SetCustomerNoNull();
                if (def.InvoiceNo != null)
                    row.InvoiceNo = def.InvoiceNo;
                else
                    row.SetInvoiceNoNull();
                row.InvoiceDate = def.InvoiceDate;
                row.NSLInvoiceNo = def.NSLInvoiceNo;
                if (def.DueDate != DateTime.MinValue)
                    row.DueDate = def.DueDate;
                else
                    row.SetDueDateNull();
                row.PaymentFromDate = def.PaymentFromDate;
                row.PaymentToDate = def.PaymentToDate;
                row.InvoiceReceivedDate = def.InvoiceReceivedDate;
                row.CurrencyId = def.Currency.CurrencyId;
                row.PaymentMethodId = def.PaymentMethod.Id;
                row.Amount = def.Amount;
                row.TotalVAT = def.TotalVAT;
                row.SettlementAmount = def.SettlementAmount;
                if (def.SettlementDate != DateTime.MinValue)
                    row.SettlementDate = def.SettlementDate;
                else
                    row.SetSettlementDateNull();
                if (def.SettlementRefNo == string.Empty)
                    row.SetSettlementRefNoNull();
                else
                    row.SettlementRefNo = def.SettlementRefNo;
                if (def.SettlementBankAccountId != int.MinValue)
                    row.SettlementBankAccountId = def.SettlementBankAccountId;
                else
                    row.SetSettlementBankAccountIdNull();
                if (def.UpdateSettlementUserId != int.MinValue)
                    row.UpdateSettlementUserId = def.UpdateSettlementUserId;
                else
                    row.SetUpdateSettlementUserIdNull();
                if (!string.IsNullOrEmpty(def.ChequeNo))
                    row.ChequeNo = def.ChequeNo;
                else
                    row.SetChequeNoNull();
                row.IsPayByHK = def.IsPayByHK;
                row.BankCharge = def.BankCharge;
                row.WorkflowStatusId = def.WorkflowStatus.Id;
                if (def.RejectReason != null)
                    row.RejectReason = def.RejectReason;
                else
                    row.SetRejectReasonNull();
                row.DCIndicator = def.DCIndicator;

                if (row.RowState == DataRowState.Detached || row.IsSUNInterfacedForSettlement != 1)
                    row.IsSUNInterfacedForSettlement = def.IsSUNInterfacedForSettlement;
                if (row.RowState == DataRowState.Detached || row.IsSUNInterfaced != 1)
                {
                    row.IsSUNInterfaced = def.IsSUNInterfaced;
                    if (def.FiscalYear == 0)
                        row.SetFiscalYearNull();
                    else
                        row.FiscalYear = def.FiscalYear;
                    if (def.FiscalPeriod == 0)
                        row.SetFiscalPeriodNull();
                    else
                        row.FiscalPeriod = def.FiscalPeriod;
                    if (def.SUNInterfaceDate == DateTime.MinValue)
                        row.SetSUNInterfaceDateNull();
                    else
                        row.SUNInterfaceDate = def.SUNInterfaceDate;
                }
                if (def.JournalNo == string.Empty)
                    row.SetJournalNoNull();
                else
                    row.JournalNo = def.JournalNo;
                if (def.ReleaseReason == string.Empty)
                    row.SetReleaseReasonNull();
                else
                    row.ReleaseReason = def.ReleaseReason;

                if (def.UserRemark == string.Empty)
                    row.SetUserRemarkNull();
                else
                    row.UserRemark = def.UserRemark;
                if (def.SubmittedBy == null)
                    row.SetSubmittedByNull();
                else
                    row.SubmittedBy = def.SubmittedBy.UserId;
                if (def.SubmittedOn == DateTime.MinValue)
                    row.SetSubmittedOnNull();
                else
                    row.SubmittedOn = def.SubmittedOn;
                if (def.Approver == null)
                    row.SetApproverIdNull();
                else
                    row.ApproverId = def.Approver.UserId;
                row.Status = def.Status;
                if (def.AccountFirstApproverId <= 0)
                    row.SetAccountFirstApproverIdNull();
                else
                    row.AccountFirstApproverId = def.AccountFirstApproverId;
                if (def.AccountFirstApprovedOn == DateTime.MinValue)
                    row.SetAccountFirstApprovedOnNull();
                else
                    row.AccountFirstApprovedOn = def.AccountFirstApprovedOn;
                if (def.AccountSecondApproverId <= 0)
                    row.SetAccountSecondApproverIdNull();
                else
                    row.AccountSecondApproverId = def.AccountSecondApproverId;
                if (def.AccountSecondApprovedOn == DateTime.MinValue)
                    row.SetAccountSecondApprovedOnNull();
                else
                    row.AccountSecondApprovedOn = def.AccountSecondApprovedOn;
                if (def.ProcurementRequestId == -1)
                    row.SetProcurementRequestIdNull();
                else
                    row.ProcurementRequestId = def.ProcurementRequestId;
                if (def.LogoInterfaceDate == DateTime.MinValue)
                    row.SetLogoInterfaceDateNull();
                else
                    row.LogoInterfaceDate = def.LogoInterfaceDate;
            }
        }

        private void NTInvoiceDetailMapping(object source, object target)
        {
            if (source.GetType() == typeof(NTInvoiceDetailDs.NTInvoiceDetailRow) &&
                target.GetType() == typeof(NTInvoiceDetailDef))
            {
                NTInvoiceDetailDs.NTInvoiceDetailRow row = (NTInvoiceDetailDs.NTInvoiceDetailRow)source;
                NTInvoiceDetailDef def = (NTInvoiceDetailDef)target;
                def.InvoiceId = row.InvoiceId;
                def.InvoiceDetailId = row.InvoiceDetailId;
                def.ExpenseType = row.IsExpenseTypeIdNull() ? null : this.getNTExpenseTypeByKey(row.ExpenseTypeId);
                def.InvoiceDetailType = NTInvoiceDetailType.getType(row.InvoiceDetailTypeId);
                def.CostCenter = row.IsCostCenterIdNull() ? null : generalWorker.getCostCenterByKey(row.CostCenterId);
                def.Vendor = row.IsVendorIdNull() ? null : VendorWorker.Instance.getVendorByKey(row.VendorId);
                def.Office = row.IsOfficeIdNull() ? null : generalWorker.getOfficeRefByKey(row.OfficeId);
                def.Company = row.IsCompanyIdNull() ? null : CompanyType.getType(row.CompanyId);
                def.Customer = row.IsCustomerIdNull() ? null : CommonWorker.Instance.getCustomerByKey(row.CustomerId);
                def.Department = row.IsDepartmentIdNull() ? null : generalWorker.getDepartmentByKey(row.DepartmentId);
                def.ProductTeam = row.IsProductTeamIdNull() ? null : generalWorker.getProductCodeRefByKey(row.ProductTeamId);
                def.Season = row.IsSeasonIdNull() ? null : generalWorker.getSeasonByKey(row.SeasonId);
                if (row.IsUserIdNull())
                    def.User = null;
                else if (row.UserId == -1)
                {
                    def.User = new UserRef();
                    def.User.UserId = -1;
                    def.User.DisplayName = "UNCLASSIFIED";
                }
                else
                    def.User = generalWorker.getUserByKey(row.UserId);
                if (row.IsItemNoNull())
                    def.ItemNo = string.Empty;
                else
                    def.ItemNo = row.ItemNo;
                def.DevSampleCostTypeId = row.IsDevSampleCostTypeIdNull() ? 0 : row.DevSampleCostTypeId;
                def.IsRecharge = row.IsRecharge;
                def.RechargeCurrency = row.IsRechargeCurrencyIdNull() ? null : generalWorker.getCurrencyByKey(row.RechargeCurrencyId);
                def.RechargeDCNoteId = row.IsRechargeDCNoteIdNull() ? 0 : row.RechargeDCNoteId;
                if (row.IsRechargePartyDeptIdNull())
                    def.RechargePartyDept = null;
                else
                    def.RechargePartyDept = NTRechargePartyDeptType.getType(row.RechargePartyDeptId);
                if (row.IsContactPersonNull())
                    def.ContactPerson = string.Empty;
                else
                    def.ContactPerson = row.ContactPerson;
                if (row.IsRechargeContactPersonNull())
                    def.RechargeContactPerson = string.Empty;
                else
                    def.RechargeContactPerson = row.RechargeContactPerson;
                def.Amount = row.Amount;
                def.VAT = row.VAT;
                def.ItemDescription1 = row.IsItemDescription1Null() ? string.Empty : row.ItemDescription1;
                def.ItemDescription2 = row.IsItemDescription2Null() ? string.Empty : row.ItemDescription2;
                def.ItemDescription3 = row.IsItemDescription3Null() ? string.Empty : row.ItemDescription3;
                def.ItemDescription4 = row.IsItemDescription4Null() ? string.Empty : row.ItemDescription4;
                def.ItemDescription5 = row.IsItemDescription5Null() ? string.Empty : row.ItemDescription5;
                def.Description = row.IsDescriptionNull() ? string.Empty : row.Description;
                def.IsPayByHK = row.IsPayByHK;
                def.NatureIdForAccrual = row.IsNatureIdForAccrualNull() ? 0 : row.NatureIdForAccrual;
                def.Quantity = row.IsQuantityNull() ? 0 : row.Quantity;
                if (row.IsNTVendorIdNull())
                    def.NTVendor = null;
                else
                    def.NTVendor = getNTVendorByKey(row.NTVendorId);
                def.Status = row.Status;

                if (row.IsSegmentValue7Null())
                    def.SegmentValue7 = null;
                else
                    def.SegmentValue7 = getNTEpicorSegmentValueByKey(row.SegmentValue7);

                if (row.IsSegmentValue8Null())
                    def.SegmentValue8 = null;
                else
                    def.SegmentValue8 = getNTEpicorSegmentValueByKey(row.SegmentValue8);
                if (!row.IsContractNoNull())
                    def.ContractNo = row.ContractNo;
                else
                    def.ContractNo = string.Empty;
                if (!row.IsDeliveryNoNull())
                    def.DeliveryNo = row.DeliveryNo;
                else
                    def.DeliveryNo = 0;

                if (!row.IsConsumptionUnitIdNull())
                    def.ConsumptionUnitId = row.ConsumptionUnitId;
                else
                    def.ConsumptionUnitId = -1;
                if (!row.IsNoOfUnitConsumedNull())
                    def.NoOfUnitConsumed = row.NoOfUnitConsumed;
                else
                    def.NoOfUnitConsumed = 0;
                if (!row.IsConsumptionUnitCostNull())
                    def.ConsumptionUnitCost = row.ConsumptionUnitCost;
                else
                    def.ConsumptionUnitCost = 0;
                if (!row.IsFuelTypeIdNull())
                    def.FuelTypeId = row.FuelTypeId;
                else
                    def.FuelTypeId = -1;
                if (!row.IsIntercommOfficeIdNull())
                    def.IntercommOfficeId = row.IntercommOfficeId;
                else
                    def.IntercommOfficeId = -1;
            }
            else if (source.GetType() == typeof(NTInvoiceDetailDef) &&
                target.GetType() == typeof(NTInvoiceDetailDs.NTInvoiceDetailRow))
            {
                NTInvoiceDetailDs.NTInvoiceDetailRow row = (NTInvoiceDetailDs.NTInvoiceDetailRow)target;
                NTInvoiceDetailDef def = (NTInvoiceDetailDef)source;

                row.InvoiceId = def.InvoiceId;
                row.InvoiceDetailId = def.InvoiceDetailId;
                if (def.ExpenseType == null)
                    row.SetExpenseTypeIdNull();
                else
                    row.ExpenseTypeId = def.ExpenseType.ExpenseTypeId;
                row.InvoiceDetailTypeId = def.InvoiceDetailType.Id;
                if (def.CostCenter == null)
                    row.SetCostCenterIdNull();
                else
                    row.CostCenterId = def.CostCenter.CostCenterId;
                if (def.Vendor == null)
                    row.SetVendorIdNull();
                else
                    row.VendorId = def.Vendor.VendorId;
                if (def.Office == null)
                    row.SetOfficeIdNull();
                else
                    row.OfficeId = def.Office.OfficeId;
                if (def.Company == null)
                    row.SetCompanyIdNull();
                else
                    row.CompanyId = def.Company.Id;
                if (def.Customer == null)
                    row.SetCustomerIdNull();
                else
                    row.CustomerId = def.Customer.CustomerId;
                if (def.Department == null)
                    row.SetDepartmentIdNull();
                else
                    row.DepartmentId = def.Department.DepartmentId;
                if (def.ProductTeam == null)
                    row.SetProductTeamIdNull();
                else
                    row.ProductTeamId = def.ProductTeam.ProductCodeId;
                if (def.Season == null)
                    row.SetSeasonIdNull();
                else
                    row.SeasonId = def.Season.SeasonId;
                if (def.User == null)
                    row.SetUserIdNull();
                else
                    row.UserId = def.User.UserId;
                if (def.ItemNo == string.Empty)
                    row.SetItemNoNull();
                else
                    row.ItemNo = def.ItemNo;
                if (def.DevSampleCostTypeId <= 0)
                    row.SetDevSampleCostTypeIdNull();
                else
                    row.DevSampleCostTypeId = def.DevSampleCostTypeId;
                row.IsRecharge = def.IsRecharge;
                if (def.RechargeCurrency == null)
                    row.SetRechargeCurrencyIdNull();
                else
                    row.RechargeCurrencyId = def.RechargeCurrency.CurrencyId;
                if (def.RechargeDCNoteId == 0)
                    row.SetRechargeDCNoteIdNull();
                else
                    row.RechargeDCNoteId = def.RechargeDCNoteId;
                if (def.RechargePartyDept == null)
                    row.SetRechargePartyDeptIdNull();
                else
                    row.RechargePartyDeptId = def.RechargePartyDept.Id;
                if (def.ContactPerson == string.Empty)
                    row.SetContactPersonNull();
                else
                    row.ContactPerson = def.ContactPerson;
                if (def.RechargeContactPerson == string.Empty)
                    row.SetRechargeContactPersonNull();
                else
                    row.RechargeContactPerson = def.RechargeContactPerson;
                row.Amount = def.Amount;
                row.VAT = def.VAT;
                if (def.ItemDescription1 == string.Empty)
                    row.SetItemDescription1Null();
                else
                    row.ItemDescription1 = def.ItemDescription1;
                if (def.ItemDescription2 == string.Empty)
                    row.SetItemDescription2Null();
                else
                    row.ItemDescription2 = def.ItemDescription2;
                if (def.ItemDescription3 == string.Empty)
                    row.SetItemDescription3Null();
                else
                    row.ItemDescription3 = def.ItemDescription3;
                if (def.ItemDescription4 == string.Empty)
                    row.SetItemDescription4Null();
                else
                    row.ItemDescription4 = def.ItemDescription4;
                if (def.ItemDescription5 == string.Empty)
                    row.SetItemDescription5Null();
                else
                    row.ItemDescription5 = def.ItemDescription5;
                if (def.Description == string.Empty)
                    row.SetDescriptionNull();
                else
                    row.Description = def.Description;
                row.IsPayByHK = def.IsPayByHK;
                if (def.NatureIdForAccrual == 0)
                    row.SetNatureIdForAccrualNull();
                else
                    row.NatureIdForAccrual = def.NatureIdForAccrual;
                if (def.Quantity == 0)
                    row.SetQuantityNull();
                else
                    row.Quantity = def.Quantity;
                if (def.NTVendor != null)
                    row.NTVendorId = def.NTVendor.NTVendorId;
                else
                    row.SetNTVendorIdNull();
                row.Status = def.Status;

                if (def.SegmentValue7 != null)
                    row.SegmentValue7 = def.SegmentValue7.SegmentValueId;
                else
                    row.SetSegmentValue7Null();

                if (def.SegmentValue8 != null)
                    row.SegmentValue8 = def.SegmentValue8.SegmentValueId;
                else
                    row.SetSegmentValue8Null();
                if (def.ContractNo != null && def.ContractNo.Trim() != string.Empty)
                    row.ContractNo = def.ContractNo;
                else
                    row.SetContractNoNull();
                row.DeliveryNo = def.DeliveryNo;

                if (def.ConsumptionUnitId > 0)
                    row.ConsumptionUnitId = def.ConsumptionUnitId;
                else
                    row.SetConsumptionUnitIdNull();

                row.NoOfUnitConsumed = def.NoOfUnitConsumed;
                row.ConsumptionUnitCost = def.ConsumptionUnitCost;

                if (def.FuelTypeId > 0)
                    row.FuelTypeId = def.FuelTypeId;
                else
                    row.SetFuelTypeIdNull();

                if (def.IntercommOfficeId == -1)
                    row.SetIntercommOfficeIdNull();
                else
                    row.IntercommOfficeId = def.IntercommOfficeId;
            }
        }

        private void NTRechargeDCNoteMapping(object source, object target)
        {
            if (source.GetType() == typeof(NTRechargeDCNoteDs.NTRechargeDCNoteRow) &&
                target.GetType() == typeof(NTRechargeDCNoteDef))
            {
                NTRechargeDCNoteDs.NTRechargeDCNoteRow row = (NTRechargeDCNoteDs.NTRechargeDCNoteRow)source;
                NTRechargeDCNoteDef def = (NTRechargeDCNoteDef)target;

                def.RechargeDCNoteId = row.RechargeDCNoteId;
                def.RechargeDCNoteNo = row.RechargeDCNoteNo;
                def.RechargeDCNoteDate = row.RechargeDCNoteDate;
                def.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
                def.Company = CompanyType.getType(row.CompanyId);
                def.DCIndicator = row.DCIndicator;
                if (row.IsToVendorIdNull())
                    def.ToVendorId = 0;
                else
                    def.ToVendorId = row.ToVendorId;
                if (row.IsToNTVendorIdNull())
                    def.ToNTVendorId = 0;
                else
                    def.ToNTVendorId = row.ToNTVendorId;
                if (row.IsToOfficeIdNull())
                    def.ToOfficeId = 0;
                else
                    def.ToOfficeId = row.ToOfficeId;
                if (row.IsToCompanyIdNull())
                    def.ToCompanyId = 0;
                else
                    def.ToCompanyId = row.ToCompanyId;
                if (row.IsToCustomerIdNull())
                    def.ToCustomerId = 0;
                else
                    def.ToCustomerId = row.ToCustomerId;
                def.RechargeCurrencyId = row.RechargeCurrencyId;
                def.RechargeAmount = row.RechargeAmount;
                def.FiscalYear = row.FiscalYear;
                def.FiscalPeriod = row.FiscalPeriod;
                def.IsSUNInterfaced = row.IsSUNInterfaced;
                if (row.IsSettlementDateNull())
                {
                    def.SettlementDate = DateTime.MinValue;
                    def.SettlementAmount = 0;
                    def.SettlementBankRefNo = string.Empty;
                }
                else
                {
                    def.SettlementDate = row.SettlementDate;
                    def.SettlementAmount = row.SettlementAmount;
                    def.SettlementBankRefNo = row.SettlementBankRefNo;
                }
                def.Status = row.Status;
                def.MailStatus = row.MailStatus;
            }
            else if (source.GetType() == typeof(NTRechargeDCNoteDef) &&
                target.GetType() == typeof(NTRechargeDCNoteDs.NTRechargeDCNoteRow))
            {
                NTRechargeDCNoteDs.NTRechargeDCNoteRow row = (NTRechargeDCNoteDs.NTRechargeDCNoteRow)target;
                NTRechargeDCNoteDef def = (NTRechargeDCNoteDef)source;

                row.RechargeDCNoteId = def.RechargeDCNoteId;
                row.RechargeDCNoteNo = def.RechargeDCNoteNo;
                row.RechargeDCNoteDate = def.RechargeDCNoteDate;
                row.OfficeId = def.Office.OfficeId;
                row.CompanyId = def.Company.Id;
                row.DCIndicator = def.DCIndicator;
                if (def.ToVendorId == 0)
                    row.SetToVendorIdNull();
                else
                    row.ToVendorId = def.ToVendorId;
                if (def.ToNTVendorId == 0)
                    row.SetToNTVendorIdNull();
                else
                    row.ToNTVendorId = def.ToNTVendorId;
                if (def.ToOfficeId == 0)
                    row.SetToOfficeIdNull();
                else
                    row.ToOfficeId = def.ToOfficeId;
                if (def.ToCompanyId == 0)
                    row.SetToCompanyIdNull();
                else
                    row.ToCompanyId = def.ToCompanyId;
                if (def.ToCustomerId == 0)
                    row.SetToCustomerIdNull();
                else
                    row.ToCustomerId = def.ToCustomerId;
                row.RechargeCurrencyId = def.RechargeCurrencyId;
                row.RechargeAmount = def.RechargeAmount;
                row.FiscalYear = def.FiscalYear;
                row.FiscalPeriod = def.FiscalPeriod;
                row.IsSUNInterfaced = def.IsSUNInterfaced;
                if (def.SettlementDate == DateTime.MinValue)
                    row.SetSettlementDateNull();
                else
                    row.SettlementDate = def.SettlementDate;
                row.SettlementAmount = def.SettlementAmount;
                if (def.SettlementBankRefNo == string.Empty)
                    row.SetSettlementBankRefNoNull();
                else
                    row.SettlementBankRefNo = def.SettlementBankRefNo;
                row.Status = def.Status;
                row.MailStatus = def.MailStatus;
            }
        }

        internal void NTSunInterfaceQueueMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(NTSunInterfaceQueueDs.NTSunInterfaceQueueRow) &&
                target.GetType() == typeof(NTSunInterfaceQueueDef))
            {
                NTSunInterfaceQueueDs.NTSunInterfaceQueueRow row = (NTSunInterfaceQueueDs.NTSunInterfaceQueueRow)source;
                NTSunInterfaceQueueDef def = (NTSunInterfaceQueueDef)target;

                def.QueueId = row.QueueId;
                def.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
                def.SunInterfaceTypeId = row.SunInterfaceTypeId;
                def.CategoryType = CategoryType.getType(row.CategoryId);
                def.FiscalYear = row.FiscalYear;
                def.Period = row.Period;
                def.User = generalWorker.getUserByKey(row.UserId);
                def.SourceId = row.SourceId;
                if (!row.IsJournalNoNull())
                    def.JournalNo = row.JournalNo;
                else
                    def.JournalNo = String.Empty;
                def.SubmitTime = row.SubmitTime;
                if (!row.IsCompletedTimeNull())
                    def.CompleteTime = row.CompletedTime;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(NTSunInterfaceQueueDef) &&
                target.GetType() == typeof(NTSunInterfaceQueueDs.NTSunInterfaceQueueRow))
            {
                NTSunInterfaceQueueDef def = (NTSunInterfaceQueueDef)source;
                NTSunInterfaceQueueDs.NTSunInterfaceQueueRow row = (NTSunInterfaceQueueDs.NTSunInterfaceQueueRow)target;

                row.QueueId = def.QueueId;
                row.OfficeId = def.Office.OfficeId;
                row.SunInterfaceTypeId = def.SunInterfaceTypeId;
                row.CategoryId = def.CategoryType.Id;
                row.FiscalYear = def.FiscalYear;
                row.Period = def.Period;
                row.UserId = def.User.UserId;
                row.SourceId = def.SourceId;
                if (def.JournalNo != String.Empty)
                    row.JournalNo = def.JournalNo;
                else
                    row.SetJournalNoNull();
                row.SubmitTime = def.SubmitTime;
                if (def.CompleteTime != DateTime.MinValue)
                    row.CompletedTime = def.CompleteTime;
                else
                    row.SetCompletedTimeNull();
                row.Status = def.Status;
            }
        }

        internal void NTUserRoleAccessMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(NTUserRoleAccesssDs.NTUserRoleAccessRow) &&
                target.GetType() == typeof(NTUserRoleAccessDef))
            {
                NTUserRoleAccesssDs.NTUserRoleAccessRow row = (NTUserRoleAccesssDs.NTUserRoleAccessRow)source;
                NTUserRoleAccessDef def = (NTUserRoleAccessDef)target;

                def.RoleId = row.RoleId;
                def.CompanyId = row.CompanyId;
                def.OfficeId = row.OfficeId;
                def.UserId = row.UserId;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(NTUserRoleAccessDef) &&
                target.GetType() == typeof(NTUserRoleAccesssDs.NTUserRoleAccessRow))
            {
                NTUserRoleAccessDef def = (NTUserRoleAccessDef)source;
                NTUserRoleAccesssDs.NTUserRoleAccessRow row = (NTUserRoleAccesssDs.NTUserRoleAccessRow)target;

                row.RoleId = def.RoleId;
                row.UserId = def.UserId;
                row.CompanyId = def.CompanyId;
                row.OfficeId = def.OfficeId;
                row.Status = def.Status;
            }
        }

        private void NTMonthEndStatusMapping(object source, object target)
        {
            if (source.GetType() == typeof(NTMonthEndStatusDs.NTMonthEndStatusRow) &&
                target.GetType() == typeof(NTMonthEndStatusDef))
            {
                NTMonthEndStatusDs.NTMonthEndStatusRow row = (NTMonthEndStatusDs.NTMonthEndStatusRow)source;
                NTMonthEndStatusDef def = (NTMonthEndStatusDef)target;

                def.RecordId = row.RecordId;
                def.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
                def.FiscalYear = row.FiscalYear;
                def.Period = row.FiscalPeriod;
                def.StartDate = row.StartDate;
                def.EndDate = row.EndDate;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(NTMonthEndStatusDef) &&
                target.GetType() == typeof(NTMonthEndStatusDs.NTMonthEndStatusRow))
            {
                NTMonthEndStatusDs.NTMonthEndStatusRow row = (NTMonthEndStatusDs.NTMonthEndStatusRow)target;
                NTMonthEndStatusDef def = (NTMonthEndStatusDef)source;

                row.RecordId = def.RecordId;
                row.OfficeId = def.Office.OfficeId;
                row.FiscalYear = def.FiscalYear;
                row.FiscalPeriod = def.Period;
                row.StartDate = def.StartDate;
                row.EndDate = def.EndDate;
                row.Status = def.Status;
            }
        }

        //private void NTRechargeDetailMapping(object source, object target)
        //{
        //    if (source.GetType() == typeof(NTRechargeDetailDs.NTRechargeDetailRow) &&
        //        target.GetType() == typeof(NTRechargeDetailDef))
        //    {
        //        NTRechargeDetailDs.NTRechargeDetailRow row = (NTRechargeDetailDs.NTRechargeDetailRow)source;
        //        NTRechargeDetailDef def = (NTRechargeDetailDef)target;
        //        def.RechargeDetailId = row.RechargeDetailId;
        //        def.InvoiceId = row.InvoiceId;
        //        def.RechargeType = NTInvoiceDetailType.getType(row.RechargeTypeId);
        //        def.RechargePartyId = row.RechargePartyId;                
        //        def.Amount = row.Amount;
        //        def.Status = (!row.IsStatusNull() ? row.Status : int.MinValue);
        //    }
        //    else if (source.GetType() == typeof(NTRechargeDetailDef) &&
        //        target.GetType() == typeof(NTRechargeDetailDs.NTRechargeDetailRow))
        //    {
        //        NTRechargeDetailDs.NTRechargeDetailRow row = (NTRechargeDetailDs.NTRechargeDetailRow)target;
        //        NTRechargeDetailDef def = (NTRechargeDetailDef)source;

        //        row.InvoiceId = def.InvoiceId;
        //        row.RechargeDetailId = def.RechargeDetailId;
        //        row.RechargeTypeId = def.RechargeType.Id;
        //        row.RechargePartyId = def.RechargePartyId;
        //        row.Amount = def.Amount;
        //        if (def.Status != null && def.Status != int.MinValue)
        //            row.Status = def.Status;
        //        else
        //            row.SetStatusNull();
        //    }

        //}

        private void NSLBankAccountMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(NSLBankAccountDs.NSLBankAccountRow) && target.GetType() == typeof(NSLBankAccountDef))
            {
                NSLBankAccountDs.NSLBankAccountRow row = (NSLBankAccountDs.NSLBankAccountRow)source;
                NSLBankAccountDef def = (NSLBankAccountDef)target;

                def.NSLBankAccountId = row.NSLBankAccountId;
                def.OfficeId = row.IsOfficeIdNull() ? int.MinValue : row.OfficeId;
                def.SUNAccountCode = row.IsSUNAccountCodeNull() ? string.Empty : row.SUNAccountCode;
                def.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                def.AccountNo = row.IsAccountNoNull() ? string.Empty : row.AccountNo;
                def.T0Code = row.IsT0CodeNull() ? string.Empty : row.T0Code;
                def.TallyCode = row.IsTallyCodeNull() ? string.Empty : row.TallyCode;
                if (row.IsBankOfficeIdNull())
                    def.BankOfficeId = -1;
                else
                    def.BankOfficeId = row.BankOfficeId;
                def.EpicorBankId = row.IsEpicorBankIdNull() ? string.Empty : row.EpicorBankId;
                def.Status = row.IsStatusNull() ? 0 : row.Status;
            }
        }

        private void NTApproverMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(NTApproverDs.NTApproverRow) &&
                target.GetType() == typeof(NTApproverDef))
            {
                NTApproverDs.NTApproverRow row = (NTApproverDs.NTApproverRow)source;
                NTApproverDef def = (NTApproverDef)target;

                def.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
                def.Approver = generalWorker.getUserByKey(row.NTApproverId);
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(NTApproverDef) &&
                target.GetType() == typeof(NTApproverDs.NTApproverRow))
            {
                NTApproverDs.NTApproverRow row = (NTApproverDs.NTApproverRow)target;
                NTApproverDef def = (NTApproverDef)source;

                row.OfficeId = def.Office.OfficeId;
                row.NTApproverId = def.Approver.UserId;
                row.Status = def.Status;
            }

        }

        private void NTDebitNoteMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(AdjustmentNoteDs.AdjustmentNoteRow) &&
                target.GetType() == typeof(AdjustmentNoteDef))
            {
                AdjustmentNoteDs.AdjustmentNoteRow row = (AdjustmentNoteDs.AdjustmentNoteRow)source;
                AdjustmentNoteDef def = (AdjustmentNoteDef)target;

                def.AdjustmentNoteId = row.AdjustmentNoteId;
                def.AdjustmentNoteNo = row.AdjustmentNoteNo;
                def.OfficeId = row.OfficeId;
                def.AdjustmentType = AdjustmentType.getType(row.AdjustmentTypeId);
                def.DebitCreditIndicator = row.DebitCreditIndicator;
                def.IssueDate = row.IssueDate;
                def.CurrencyId = row.CurrencyId;
                if (!row.IsVendorIdNull())
                    def.VendorId = row.VendorId;
                else
                    def.VendorId = -1;
                if (!row.IsPartyNameNull())
                    def.PartyName = row.PartyName;
                else
                    def.PartyName = String.Empty;
                if (!row.IsPartyAddress1Null())
                    def.PartyAddress1 = row.PartyAddress1;
                else
                    def.PartyAddress1 = String.Empty;
                if (!row.IsPartyAddress2Null())
                    def.PartyAddress2 = row.PartyAddress2;
                else
                    def.PartyAddress2 = String.Empty;
                if (!row.IsPartyAddress3Null())
                    def.PartyAddress3 = row.PartyAddress3;
                else
                    def.PartyAddress3 = String.Empty;
                if (!row.IsPartyAddress4Null())
                    def.PartyAddress4 = row.PartyAddress4;
                else
                    def.PartyAddress4 = String.Empty;
                def.Amount = row.Amount;
                if (!row.IsMailStatusNull())
                    def.MailStatus = row.MailStatus;
                else
                    def.MailStatus = 0;
            }
            else if (source.GetType() == typeof(AdjustmentNoteDef) &&
                target.GetType() == typeof(AdjustmentNoteDs.AdjustmentNoteRow))
            {
                AdjustmentNoteDef def = (AdjustmentNoteDef)source;
                AdjustmentNoteDs.AdjustmentNoteRow row = (AdjustmentNoteDs.AdjustmentNoteRow)target;

                row.AdjustmentNoteId = def.AdjustmentNoteId;
                row.AdjustmentNoteNo = def.AdjustmentNoteNo;
                row.OfficeId = def.OfficeId;
                row.AdjustmentTypeId = def.AdjustmentType.Id;
                row.DebitCreditIndicator = def.DebitCreditIndicator;
                row.IssueDate = def.IssueDate;
                row.CurrencyId = def.CurrencyId;
                if (def.VendorId != -1)
                    row.VendorId = def.VendorId;
                else
                    row.SetVendorIdNull();
                if (def.PartyName != String.Empty)
                    row.PartyName = def.PartyName;
                else
                    row.SetPartyNameNull();
                if (def.PartyAddress1 != String.Empty)
                    row.PartyAddress1 = def.PartyAddress1;
                else
                    row.SetPartyAddress1Null();
                if (def.PartyAddress2 != String.Empty)
                    row.PartyAddress2 = def.PartyAddress2;
                else
                    row.SetPartyAddress2Null();
                if (def.PartyAddress3 != String.Empty)
                    row.PartyAddress3 = def.PartyAddress3;
                else
                    row.SetPartyAddress3Null();
                if (def.PartyAddress4 != String.Empty)
                    row.PartyAddress4 = def.PartyAddress4;
                else
                    row.SetPartyAddress4Null();
                row.Amount = def.Amount;
                if (def.MailStatus != null && def.MailStatus != int.MinValue)
                    row.MailStatus = def.MailStatus;
                else
                    row.SetMailStatusNull();
            }
        }

        private void NTEpicorSegmentValueMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(NTEpicorSegmentValueDs.NTEpicorSegmentValueRow) && target.GetType() == typeof(NTEpicorSegmentValueRef))
            {
                NTEpicorSegmentValueDs.NTEpicorSegmentValueRow row = (NTEpicorSegmentValueDs.NTEpicorSegmentValueRow)source;
                NTEpicorSegmentValueRef def = (NTEpicorSegmentValueRef)target;

                def.SegmentValueId = row.SegmentValueId;
                def.SegmentField = row.SegmentField;
                def.SegmentValue = row.SegmentValue;
                def.SegmentName = row.SegmentName;                
                def.Status = row.Status;
            }
        }
        #endregion

        #region NT Settlement
        private bool isValidText(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            else
                for (int i = 0; i < str.Length; i++)
                    if (char.IsControl((char)str[i]))
                        return false;
            return true;
        }


        /*
        public ArrayList getUploadedNonTradeData(int userId, string sourcePath, TypeCollector OfficeIdList, out ArrayList ccyDiscrepancyList, out ArrayList arrUpdatedList, out bool isFileSplit)
        {
            StreamReader sourceFile = File.OpenText(sourcePath);
            string line = "";
            char[] delimiter = { ',' };
            ArrayList list = null;
            ArrayList temp = null;
            string[] fields;
            ShippingWorker shippingWorker = ShippingWorker.Instance;
            arrUpdatedList = new ArrayList();
            isFileSplit = false;

            ccyDiscrepancyList = new ArrayList();
            while (!sourceFile.EndOfStream)
            {
                if (list != null)
                    if (list.Count == 1000)
                    {
                        isFileSplit = true;
                        break;
                    }
                line = sourceFile.ReadLine();
                if (line.Replace(",", "").Trim() == "")
                    continue;
                fields = line.Split(delimiter);
                if (!isValidText(fields[0]))
                    break;


                temp = null;
                if (fields[0].Contains("/"))
                {   // get invoice by Account No. (Customer No + MM/YY)
                    string accountNo = fields[0].Trim();
                    string customerNo = string.Empty;
                    int yy = 0, mm = 0;
                    if (int.TryParse(accountNo.Substring(accountNo.Length - 2, 2), out yy)
                        && int.TryParse(accountNo.Substring(accountNo.Length - 4, 2), out mm))
                        if (accountNo.Substring(accountNo.Length - 5, 1) == "/" && mm > 0 && mm <= 12)
                        {
                            DateTime fromDate, toDate;
                            fromDate = DateTime.ParseExact(yy.ToString("00") + "-" + mm.ToString("00") + "-01", "yy-MM-dd", null);
                            if (mm == 12)
                                toDate = DateTime.ParseExact((yy + 1).ToString("00") + "-01-01", "yy-MM-dd", null).Subtract(TimeSpan.Parse("00:00:00.001"));
                            else
                                toDate = DateTime.ParseExact(yy.ToString("00") + "-" + (mm + 1).ToString("00") + "-01", "yy-MM-dd", null).Subtract(TimeSpan.Parse("00:00:00.001"));
                            customerNo = accountNo.Substring(0, accountNo.Length - 5);
                            temp = NonTradeWorker.Instance.getNTInvoiceList(OfficeIdList, -1, fromDate, toDate, DateTime.MinValue, DateTime.MinValue, "", "", customerNo, customerNo, "", "", -1, -1);
                        }
                }
                if (temp == null)
                {   // get invoice by Supplier Invoice no.
                    string invoiceNo;
                    invoiceNo = fields[0].Trim();
                    temp = NonTradeWorker.Instance.getNTInvoiceList(OfficeIdList, -1, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, invoiceNo, invoiceNo, "", "", "", "", -1, -1);
                }

                if (temp != null)
                {
                    if (list == null)
                        list = new ArrayList();
                    foreach (NTInvoiceDef def in temp)
                    {
                        if (def.SettlementDate != DateTime.MinValue)
                            arrUpdatedList.Add(def.NSLInvoiceNo);
                        decimal amt;
                        if (decimal.TryParse(fields[3], out amt))
                            def.SettlementAmount = amt;
                        else
                            def.SettlementAmount = 0;
                        DateTime dt;
                        if (DateTime.TryParse(fields[4], out dt))
                            def.SettlementDate = dt;
                        else
                            def.SettlementDate = DateTime.MinValue;
                        def.SettlementRefNo = fields[5].Trim();
                        if (fields[2].Trim() != def.Currency.CurrencyCode
                            && !(fields[2].Trim() == "GBP" && def.Currency.CurrencyCode == "GB£")
                            && !(fields[2].Trim() == "EUR" && def.Currency.CurrencyCode == "EU€"))
                            ccyDiscrepancyList.Add(def.NSLInvoiceNo);
                        list.Add(def);
                    }
                }
            }
            return list;
        }
        */

        public NSLBankAccountDef getNSLBankAccountByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSLBankAccountApt", "GetNSLBankAccountByKey");
            ad.SelectCommand.Parameters["@Key"].Value = key;
            NSLBankAccountDs ds = new NSLBankAccountDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NSLBankAccountDef def = new NSLBankAccountDef();
            NSLBankAccountMapping(ds.NSLBankAccount.Rows[0], def);
            return def;
        }

        /*
        public ArrayList getNSLBankAccountByOfficeId(TypeCollector officeIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSLBankAccountApt", "GetNSLBankAccountByOfficeId");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);

            NSLBankAccountDs dataSet = new NSLBankAccountDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NSLBankAccountDs.NSLBankAccountRow row in dataSet.NSLBankAccount)
            {
                NSLBankAccountDef def = new NSLBankAccountDef();
                NSLBankAccountMapping(row, def);
                list.Add(def);
            }
            return list;
        }
        */
        public ArrayList getNSLBankAccount(TypeCollector officeIdList, TypeCollector currencyIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSLBankAccountApt", "GetNSLBankAccount");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@CurrencyIdList"] = CustomDataParameter.parse(currencyIdList.IsInclusive, currencyIdList.Values);

            NSLBankAccountDs dataSet = new NSLBankAccountDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NSLBankAccountDs.NSLBankAccountRow row in dataSet.NSLBankAccount)
            {
                NSLBankAccountDef def = new NSLBankAccountDef();
                NSLBankAccountMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getNTInvoiceCurrencyList()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceApt", "GetNTInvoiceCurrency");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();
            if (recordsAffected > 0)
                foreach (DataRow row in dataSet.Tables[0].Rows)
                    list.Add(generalWorker.getCurrencyByKey(Convert.ToInt32(row[0])));
            return list;
        }

        #endregion


        #region NT Debit Note

        public OfficeRef getRechargeOffice(NTInvoiceDetailDef detailDef)
        {
            return getRechargeOffice(detailDef, getNTInvoiceByKey(detailDef.InvoiceId));
        }

        public OfficeRef getRechargeOffice(NTInvoiceDetailDef detailDef, NTInvoiceDef invoiceDef)
        {
            if (detailDef.IntercommOfficeId != -1 && detailDef.IntercommOfficeId != invoiceDef.Office.OfficeId)
                return generalWorker.getOfficeRefByKey(detailDef.IntercommOfficeId);
            else
                return (detailDef.Office.OfficeId != OfficeId.HK.Id && detailDef.IsPayByHK == 1 ? generalWorker.getOfficeRefByKey(OfficeId.HK.Id) : invoiceDef.Office);
        }

        public CompanyType getRechargeCompany(NTInvoiceDetailDef detailDef)
        {
            return getRechargeCompany(detailDef, getNTInvoiceByKey(detailDef.InvoiceId));
        }

        public CompanyType getRechargeCompany(NTInvoiceDetailDef detailDef, NTInvoiceDef invoiceDef)
        {
            return (detailDef.Office.OfficeId != OfficeId.HK.Id && detailDef.IsPayByHK == 1 ? CompanyType.NEXT_SOURCING : invoiceDef.Company);
        }

        public string getDCNoteNoGroupKey(NTInvoiceDef invoiceDef, NTInvoiceDetailDef detailDef)
        {
            string dept = "0";
            string contact = string.Empty;
            if (detailDef.RechargePartyDept != null)
                dept = detailDef.RechargePartyDept.Id.ToString();
            if (detailDef.RechargeContactPerson != null)
                contact = detailDef.RechargeContactPerson;
            string company = getRechargeCompany(detailDef, invoiceDef).ToString();
            string officeCode = getRechargeOffice(detailDef, invoiceDef).OfficeCode;
            return officeCode + company + detailDef.RechargeCurrency.CurrencyCode + dept + contact;
        }

        public int getDCNoteRechargeAmountSign(NTRechargeDCNoteDef dcNote, NTInvoiceDef invoiceDef)
        {
            int sign = 1;
            sign *= (dcNote.DCIndicator == "C" ? -1 : 1);
            sign *= (invoiceDef.DCIndicator == "C" ? -1 : 1);
            return sign;
        }

        public decimal calcRechargeAmount(NTRechargeDCNoteDef dcNote, NTInvoiceDetailDef detailDef, NTInvoiceDef invoiceDef)
        {
            return Math.Round(detailDef.Amount * getDCNoteRechargeAmountSign(dcNote, invoiceDef) * getRechargeExchangeRate(dcNote, detailDef, invoiceDef), 2, MidpointRounding.AwayFromZero);
        }

        public decimal getRechargeExchangeRate(NTRechargeDCNoteDef dcNoteDetail, NTInvoiceDetailDef detailDef, NTInvoiceDef invoiceDef)
        {
            DateTime date = DateTime.Today;
            if (dcNoteDetail != null)
                if (dcNoteDetail.RechargeDCNoteDate != null && dcNoteDetail.RechargeDCNoteDate != DateTime.MinValue)
                    date = dcNoteDetail.RechargeDCNoteDate;
            return CommonWorker.Instance.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, invoiceDef.Currency.CurrencyId, date) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, detailDef.RechargeCurrency.CurrencyId, date);
        }


        public NTRechargeDCNoteDef createNewDCNoteDef(string dcNoteNo, DateTime dcNoteDate, NTInvoiceDetailDef detailDef, NTInvoiceDef invoiceDef)
        {
            NTRechargeDCNoteDef dcNote = new NTRechargeDCNoteDef();
            dcNote.RechargeDCNoteNo = dcNoteNo;
            dcNote.RechargeDCNoteDate = dcNoteDate;
            dcNote.RechargeCurrencyId = detailDef.RechargeCurrency.CurrencyId;
            dcNote.Office = getRechargeOffice(detailDef, invoiceDef);
            dcNote.Company = getRechargeCompany(detailDef, invoiceDef);
            dcNote.DCIndicator = "D";//getRevisedDCIndicator(invoiceDef, detailDef); //invoiceDef.DCIndicator;
            if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.CUSTOMER.Id)
                dcNote.ToCustomerId = detailDef.Customer.CustomerId;
            else if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.OFFICE.Id) // && detailDef.Office.OfficeId == OfficeId.UK.Id)
            {
                dcNote.ToOfficeId = detailDef.Office.OfficeId;
                dcNote.ToCompanyId = detailDef.Company.Id;
            }
            else if (detailDef.InvoiceDetailType.Id == NTInvoiceDetailType.NT_VENDOR.Id)
            {
                dcNote.ToNTVendorId = detailDef.NTVendor.NTVendorId;
            }
            else
                dcNote.ToVendorId = detailDef.Vendor.VendorId;
            dcNote.IsSUNInterfaced = 0;
            dcNote.RechargeAmount = 0;

            ArrayList monthEndStatusList = getCurrentMonthEndStatusList(TypeCollector.createNew(invoiceDef.Office.OfficeId));
            NTMonthEndStatusDef monthEndDef = (NTMonthEndStatusDef)monthEndStatusList[0];
            dcNote.FiscalYear = monthEndDef.FiscalYear;
            dcNote.FiscalPeriod = monthEndDef.Period;

            return dcNote;
        }

        #endregion

        #region NT User Role Access

        public ArrayList getNTUserOfficeList(int userId, int roleId, int companyId)
        {
            ArrayList officeList = new ArrayList();
            ArrayList officeIdList = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("NTUserRoleAccessApt", "GetNTUserOfficeList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.Parameters["@RoleId"].Value = roleId;
            ad.SelectCommand.Parameters["@CompanyId"].Value = companyId;

            NTUserRoleAccesssDs ds = new NTUserRoleAccesssDs();
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected > 0)
            {
                foreach (NTUserRoleAccesssDs.NTUserRoleAccessRow row in ds.NTUserRoleAccess)
                {
                    if (row.OfficeId == -1)
                    {
                        // return all office
                        foreach (OfficeRef office in generalWorker.getOfficeListByUserId(userId, OfficeStructureType.PRODUCTCODE.Type))
                        {
                            if (!officeIdList.Contains(office.OfficeId))
                            {
                                officeIdList.Add(office.OfficeId);
                                officeList.Add(office);
                            }
                        }
                        break;
                    }
                    else

                        if (!officeIdList.Contains(row.OfficeId))
                        {
                            officeIdList.Add(row.OfficeId);
                            officeList.Add(generalWorker.getOfficeRefByKey(row.OfficeId));
                        }
                }
            }
            return officeList;
        }

        public ArrayList getNTUserCompanyList(int userId, int officeId, int roleId)
        {
            ArrayList companyList = new ArrayList();
            ArrayList companyIdList = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("NTUserRoleAccessApt", "GetNTUserCompanyList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.Parameters["@RoleId"].Value = roleId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            NTUserRoleAccesssDs ds = new NTUserRoleAccesssDs();
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected > 0)
            {
                foreach (NTUserRoleAccesssDs.NTUserRoleAccessRow row in ds.NTUserRoleAccess)
                {
                    if (row.CompanyId == -1)
                    {
                        // return all company
                        //foreach (CompanyRef company in generalWorker.getCompanyList(-1))
                        foreach (CompanyRef company in generalWorker.getCompanyList(officeId))
                        {
                            if (!companyIdList.Contains(company.CompanyId))
                            {
                                companyIdList.Add(company.CompanyId);
                                companyList.Add(company);
                            }
                        }
                        break;
                    }
                    else
                        if (!companyIdList.Contains(row.CompanyId))
                        {
                            companyIdList.Add(row.CompanyId);
                            companyList.Add(generalWorker.getCompanyByKey(row.CompanyId));
                        }
                }
            }
            return companyList;
        }

        public ArrayList getNTUserList(int roleId, int companyId, int officeId)
        {
            ArrayList userList = new ArrayList();
            ArrayList userIdList = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("NTUserRoleAccessApt", "GetNTUserList");
            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.Parameters["@RoleId"].Value = roleId;
            ad.SelectCommand.Parameters["@CompanyId"].Value = companyId;
            NTUserRoleAccesssDs ds = new NTUserRoleAccesssDs();
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected > 0)
            {
                foreach (NTUserRoleAccesssDs.NTUserRoleAccessRow row in ds.NTUserRoleAccess)
                {
                    if (!userIdList.Contains(row.UserId))
                    {
                        userIdList.Add(row.UserId);
                        userList.Add(generalWorker.getUserByKey(row.UserId));
                    }
                }
            }
            return userList;
        }

        public ArrayList getNTUserRoleAccessList(int roleId)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("NTUserRoleAccessApt", "GetNTUserList");
            ad.SelectCommand.Parameters["@officeId"].Value = -1;
            ad.SelectCommand.Parameters["@RoleId"].Value = roleId;
            ad.SelectCommand.Parameters["@CompanyId"].Value = -1;
            NTUserRoleAccesssDs ds = new NTUserRoleAccesssDs();
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected > 0)
            {
                foreach (NTUserRoleAccesssDs.NTUserRoleAccessRow row in ds.NTUserRoleAccess)
                {
                    NTUserRoleAccessDef def = new NTUserRoleAccessDef();
                    this.NTUserRoleAccessMapping(row, def);
                    list.Add(def);
                }
            }
            return list;
        }

        public NTUserRoleAccessDef getNTUserRoleAccessByKey(int roleId, int companyId, int officeId, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTUserRoleAccessApt", "GetNTUserRoleAccessByKey");
            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.Parameters["@RoleId"].Value = roleId;
            ad.SelectCommand.Parameters["@CompanyId"].Value = companyId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;

            NTUserRoleAccesssDs ds = new NTUserRoleAccesssDs();
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1) return null;
            NTUserRoleAccessDef def = new NTUserRoleAccessDef();
            NTUserRoleAccesssDs.NTUserRoleAccessRow row = (NTUserRoleAccesssDs.NTUserRoleAccessRow)ds.NTUserRoleAccess.Rows[0];

            this.NTUserRoleAccessMapping(row, def);

            return def;
        }

        public ArrayList getNTRoleList()
        {
            IDataSetAdapter ad = getDataSetAdapter("NTRoleApt", "GetNTRoleList");
            NTRoleDs dataSet = new NTRoleDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTRoleDs.NTRoleRow row in dataSet.NTRole)
            {
                NTRoleRef def = new NTRoleRef();
                def.RoleId = row.RoleId;
                def.RoleName = row.RoleName;
                list.Add(def);
            }
            return list;
        }

        public NTRoleRef getNTRoleByKey(int roleId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTRoleApt", "GetNTRoleByKey");
            ad.SelectCommand.Parameters["@RoleId"].Value = roleId;
            NTRoleDs dataSet = new NTRoleDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
            {
                NTRoleDs.NTRoleRow row = dataSet.NTRole[0];
                NTRoleRef def = new NTRoleRef();
                def.RoleId = row.RoleId;
                def.RoleName = row.RoleName;
                return def;
            }
            else
                return null;
        }

        /*
        public ArrayList getNTSuperUserList(int userId)
        {
            ArrayList userList = new ArrayList();
            ArrayList userIdList = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("NTUserRoleAccessApt", "GetNTSuperUserList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            NTUserRoleAccessDs ds = new NTUserRoleAccessDs();
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected > 0)
            {
                foreach (NTUserRoleAccessDs.NTUserRoleAccessRow row in ds.NTUserRoleAccess)
                {
                    if (!userIdList.Contains(row.UserId))
                    {
                        userIdList.Add(row.UserId);
                        userList.Add(generalWorker.getUserByKey(row.UserId));
                    }
                }
            }
            return userList;
        }

        public bool isNTSuperUser(int userId)
        {
            return (getNTSuperUserList(userId).Count > 0);
        }
        */

        #endregion

        public void deleteNTUserRoleAccess(NTUserRoleAccessDef def, int userId)
        {
            if (def == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                NTUserRoleAccesssDs dataSet = new NTUserRoleAccesssDs();
                NTUserRoleAccesssDs.NTUserRoleAccessRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("NTUserRoleAccessApt", "GetNTUserRoleAccessByKey");
                ad.SelectCommand.Parameters["@RoleId"].Value = def.RoleId;
                ad.SelectCommand.Parameters["@CompanyId"].Value = def.CompanyId;
                ad.SelectCommand.Parameters["@OfficeId"].Value = def.OfficeId;
                ad.SelectCommand.Parameters["@UserId"].Value = def.UserId;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.NTUserRoleAccess[0];
                    row.Status = 0;
                    sealStamp(dataSet.NTUserRoleAccess[0], userId, Stamp.UPDATE);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update NTUserRoleAccess ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }


        public ArrayList getNTExpenseNatureList(int ntVendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTExpenseNatureApt", "GetNTExpenseNatureList");
            ad.SelectCommand.Parameters["@NTVendorId"].Value = ntVendorId;

            NTExpenseNatureDs dataSet = new NTExpenseNatureDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTExpenseNatureDs.NTExpenseNatureRow row in dataSet.NTExpenseNature)
            {
                NTExpenseNatureRef def = new NTExpenseNatureRef();
                def.NatureId = row.NatureId;
                def.Description = row.NatureDesc;
                list.Add(def);
            }
            return list;
        }

        public NTExpenseNatureRef getNTExpenseNatureByKey(int id)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTExpenseNatureApt", "GetNTExpenseNatureByKey");
            ad.SelectCommand.Parameters["@NatureId"].Value = id;
            NTExpenseNatureDs dataSet = new NTExpenseNatureDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
            {
                NTExpenseNatureDs.NTExpenseNatureRow row = dataSet.NTExpenseNature[0];
                NTExpenseNatureRef def = new NTExpenseNatureRef();
                def.NatureId = row.NatureId;
                def.Description = row.NatureDesc;
                return def;
            }
            else
                return null;
        }


        public NTEpicorSegmentValueRef getNTEpicorSegmentValueByKey(int id)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTEpicorSegmentValueApt", "GetNTEpicorSegmentValueByKey");
            ad.SelectCommand.Parameters["@Key"].Value = id;
            NTEpicorSegmentValueDs dataSet = new NTEpicorSegmentValueDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
            {
                NTEpicorSegmentValueDs.NTEpicorSegmentValueRow row = dataSet.NTEpicorSegmentValue[0];
                NTEpicorSegmentValueRef def = new NTEpicorSegmentValueRef();
                NTEpicorSegmentValueMapping(row, def);
                return def;
            }
            else
                return null;
        }

        public ArrayList getNTEpicorSegmentValueListBySegmentField(int segmentField)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTEpicorSegmentValueApt", "GetNTEpicorSegmentValueBySegmentField");
            ad.SelectCommand.Parameters["@SegmentField"].Value = segmentField;

            NTEpicorSegmentValueDs dataSet = new NTEpicorSegmentValueDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (NTEpicorSegmentValueDs.NTEpicorSegmentValueRow row in dataSet.NTEpicorSegmentValue)
            {
                NTEpicorSegmentValueRef def = new NTEpicorSegmentValueRef();
                NTEpicorSegmentValueMapping(row, def);
                list.Add(def);
            }

            return list;

        }
    }
}
