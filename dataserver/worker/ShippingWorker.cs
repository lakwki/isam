using System;
using System.Data;
using System.Collections;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.isam.dataserver.model.order;
using com.next.isam.dataserver.model.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.shipping;
using com.next.isam.domain.account;
using com.next.infra.util;
using com.next.isam.domain.common;

namespace com.next.isam.dataserver.worker
{
    public class ShippingWorker : Worker
    {
        private static ShippingWorker _instance;
        private GeneralWorker generalWorker;
        private CommonWorker commonWorker;
        private VendorWorker vendorWorker;

        protected ShippingWorker()
        {
            generalWorker = GeneralWorker.Instance;
            commonWorker = CommonWorker.Instance;
            vendorWorker = VendorWorker.Instance;
        }

        public static ShippingWorker Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ShippingWorker();
                return _instance;
            }
        }

        public static bool isValidInvoiceNo(string invoiceNo)
        {
            if (invoiceNo.Length != 14)
                return false;

            if (invoiceNo.Substring(3, 1) != "/" || invoiceNo.Substring(9, 1) != "/")
                return false;

            for (int i = 4; i <= 8; i++)
            {
                if (!Char.IsNumber(invoiceNo[i]))
                    return false;
            }

            for (int i = 10; i <= 13; i++)
            {
                if (!Char.IsNumber(invoiceNo[i]))
                    return false;
            }

            return true;
        }

        public void updateInvoice(InvoiceDef def, ActionHistoryType actionType, ArrayList amendmentList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ShipmentDef shipment = OrderSelectWorker.Instance.getShipmentByKey(def.ShipmentId);

                IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceByKey");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = def.ShipmentId;
                ad.PopulateCommands();

                InvoiceDs dataSet = new InvoiceDs();
                InvoiceDs.InvoiceRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.Invoice[0];
                    if (amendmentList != null)
                    {
                        if (!row.IsInvoicePrefixNull())
                        {

                            if ((row.InvoicePrefix != def.InvoicePrefix || row.InvoiceSeq != def.InvoiceSeqNo || row.InvoiceYear != def.InvoiceYear)
                                && def.InvoicePrefix == string.Empty)
                                throw new UnauthorizedAccessException("- Fail in updating invoice. Invoice No. cannot be blank.");

                            if (def.InvoicePrefix != String.Empty)
                            {
                                if (row.InvoicePrefix != def.InvoicePrefix || row.InvoiceSeq != def.InvoiceSeqNo || row.InvoiceYear != def.InvoiceYear)
                                    amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.INVOICE_NO, getInvoiceNo(row.InvoicePrefix, row.InvoiceSeq, row.InvoiceYear), def.InvoiceNo, userId));

                                def.ChoiceOrderNSLCommissionAmount = Math.Round(shipment.NSLCommissionPercentage / 100 * def.ChoiceOrderTotalShippedAmount, 2, MidpointRounding.AwayFromZero);

                                if (row.ChoiceOrderNSLCommissionAmt != def.ChoiceOrderNSLCommissionAmount)
                                    amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.CHOICE_ORDER_NSL_COMM_AMT, row.ChoiceOrderNSLCommissionAmt.ToString("#,###.00"), def.ChoiceOrderNSLCommissionAmount.ToString("#,###.00"), userId));

                                if (row.ChoiceOrderTotalShippedAmt != def.ChoiceOrderTotalShippedAmount)
                                    amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.CHOICE_ORDER_TOTAL_SHIPPED_AMT, row.ChoiceOrderTotalShippedAmt.ToString("#,###.00"), def.ChoiceOrderTotalShippedAmount.ToString("#,###.00"), userId));
                                if (row.ChoiceOrderTotalShippedSupplierGmtAmt != def.ChoiceOrderTotalShippedSupplierGarmentAmount)
                                    amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.CHOICE_ORDER_TOTAL_SHIPPED_SUPPLIER_GMT_AMT, row.ChoiceOrderTotalShippedSupplierGmtAmt.ToString("#,###.00"), def.ChoiceOrderTotalShippedSupplierGarmentAmount.ToString("#,###.00"), userId));

                                if (def.ShippingDocReceiptDate != (row.IsShippingDocReceiptDateNull() ? DateTime.MinValue : row.ShippingDocReceiptDate))
                                    amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.SHIPPING_DOCUMENT_RECEIPT_DATE,
                                        (DateTimeUtility.getDateString((row.IsShippingDocReceiptDateNull() ? DateTime.MinValue : row.ShippingDocReceiptDate))),
                                                    (DateTimeUtility.getDateString(def.ShippingDocReceiptDate)), userId));


                                if (row.InvoiceDate != def.InvoiceDate.Date)
                                {
                                    amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.INVOICE_DATE, DateTimeUtility.getDateString(row.IsInvoiceDateNull() ? DateTime.MinValue : row.InvoiceDate), DateTimeUtility.getDateString(def.InvoiceDate), userId));

                                    shipment.USExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, def.InvoiceDate.Date);
                                    def.InvoiceBuyExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, shipment.BuyCurrency.CurrencyId, def.InvoiceDate.Date);
                                    def.InvoiceSellExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, shipment.SellCurrency.CurrencyId, def.InvoiceDate.Date);
                                    OrderWorker.Instance.updateShipmentList(ConvertUtility.createArrayList(shipment));

                                    ArrayList splitShipments = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(shipment.ShipmentId);
                                    foreach (SplitShipmentDef splitShipmentDef in splitShipments)
                                    {
                                        splitShipmentDef.InvoiceBuyExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, splitShipmentDef.BuyCurrency.CurrencyId, def.InvoiceDate);
                                        OrderWorker.Instance.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipmentDef), userId);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (def.InvoiceDate != DateTime.MinValue && !def.IsSelfBilledOrder)
                            {
                                if (row.IsInvoicePrefixNull() && string.IsNullOrEmpty(def.InvoicePrefix))
                                    issueInvoice(def, true, userId);
                                else
                                    issueInvoice(def, false, userId);
                                if (def.ActualAtWarehouseDate == DateTime.MinValue && !def.InvoicePrefix.EndsWith("E"))
                                    def.ActualAtWarehouseDate = def.InvoiceDate;
                            }
                        }
                        if (!row.IsSupplierInvoiceNoNull())
                        {
                            if (row.SupplierInvoiceNo != def.SupplierInvoiceNo)
                                amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.SUPPLIER_INVOICE_NO, (row.IsSupplierInvoiceNoNull() ? string.Empty : row.SupplierInvoiceNo), def.SupplierInvoiceNo, userId));
                        }
                        else
                            if (!string.IsNullOrEmpty(def.SupplierInvoiceNo))
                            amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.SUPPLIER_INVOICE_NO, string.Empty, def.SupplierInvoiceNo, userId));

                        if (row.InputVATActualAmt != def.InputVATActualAmt)
                            amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.UT_INPUT_VAT, row.InputVATActualAmt.ToString("0.00"), def.InputVATActualAmt.ToString("0.00"), userId));
                        if (row.ImportDutyActualAmt != def.ImportDutyActualAmt)
                            amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.UT_IMPORT_DUTY, row.ImportDutyActualAmt.ToString("0.00"), def.ImportDutyActualAmt.ToString("0.00"), userId));

                        string toAWHDate = (def.ActualAtWarehouseDate == DateTime.MinValue ? string.Empty : def.ActualAtWarehouseDate.ToString("dd/MM/yyyy"));
                        string fromAWHDate = (row.IsActualAtWarehouseDateNull() ? string.Empty : row.ActualAtWarehouseDate.ToString("dd/MM/yyyy"));
                        if (fromAWHDate != toAWHDate)
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.BOOKING_UPLOAD, null,
                                    "Actual In-Warehouse Date : " + fromAWHDate + " -> " + toAWHDate, userId));

                        if ((row.IsLCBillRefNoNull() ? string.Empty : row.LCBillRefNo) != def.LCBillRefNo.Trim())
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null,
                                    "L/C Bill Ref. No. : " + (row.IsLCBillRefNoNull() ? string.Empty : row.LCBillRefNo) + " -> " + def.LCBillRefNo, userId));
                    }
                    if (!row.IsILSQtyUploadAllowed)
                        def.IsILSQtyUploadAllowed = false;

                    def.NSLCommissionAmt = Math.Round(shipment.NSLCommissionPercentage / 100 * shipment.TotalShippedAmount, 2, MidpointRounding.AwayFromZero);
                    def.ChoiceOrderNSLCommissionAmount = Math.Round(shipment.NSLCommissionPercentage / 100 * def.ChoiceOrderTotalShippedAmount, 2, MidpointRounding.AwayFromZero);

                    this.InvoiceMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                    recordsAffected = ad.Update(dataSet);
                }
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Invoice ERROR");
                ctx.VoteCommit();

                SynchronizationWorker.Instance.SyncNssShipment(def.ShipmentId);
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

        public void updateInvoiceNoSync(InvoiceDef def, ActionHistoryType actionType, ArrayList amendmentList, int userId)
        {   // Data will not Sync to NSS
            string source;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ShipmentDef shipment = OrderSelectWorker.Instance.getShipmentByKey(def.ShipmentId);

                IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceByKey");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = def.ShipmentId;
                ad.PopulateCommands();

                InvoiceDs dataSet = new InvoiceDs();
                InvoiceDs.InvoiceRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.Invoice[0];
                    if (amendmentList != null)
                    {
                        if (def.ShippingDocReceiptDate != DateTime.MinValue)
                        {
                            source = (row.IsShippingDocReceiptDateNull() ? "NULL" : row.ShippingDocReceiptDate.ToString("dd/MM/yyyy"));
                            if (row.IsShippingDocReceiptDateNull() || def.ShippingDocReceiptDate != (row.IsShippingDocReceiptDateNull() ? DateTime.MinValue : row.ShippingDocReceiptDate))
                                amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.SHIPPING_DOCUMENT_RECEIPT_DATE, source, def.ShippingDocReceiptDate.ToString("dd/MM/yyyy"), userId));
                        }
                        if (def.LCPaymentCheckedDate != DateTime.MinValue)
                        {
                            source = (row.IsLCPaymentCheckedDateNull() ? "NULL" : row.LCPaymentCheckedDate.ToString("dd/MM/yyyy"));
                            if (row.IsLCPaymentCheckedDateNull() || def.LCPaymentCheckedDate != (row.IsLCPaymentCheckedDateNull() ? DateTime.MinValue : row.LCPaymentCheckedDate))
                                amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.LC_PAYMENT_CHECKED_DATE, source, def.LCPaymentCheckedDate.ToString("dd/MM/yyyy"), userId));
                        }
                        if (row.IsLCPaymentChecked != def.IsLCPaymentChecked)
                            amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.IS_LC_PAYMENT_CHECKED, row.IsLCPaymentChecked.ToString(), def.IsLCPaymentChecked.ToString(), userId));

                    }
                    this.InvoiceMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                    recordsAffected = ad.Update(dataSet);
                }
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Invoice ERROR");
                ctx.VoteCommit();

                //SynchronizationWorker.Instance.SyncNssShipment(def.ShipmentId);
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


        public void fillNextInvoiceNo(InvoiceDef def, int userId)
        {
            int budgetYear;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (def.InvoiceDate == DateTime.MinValue)
                    throw new ApplicationException("Invalid invoice date, please assign invoice date before calling fillNextInvoiceNo()");

                AccountFinancialCalenderDef calenderDef = generalWorker.getAccountPeriodByDate(9, def.InvoiceDate);

                IDataSetAdapter ad = getDataSetAdapter("InvoiceNoParamApt", "GetLatestInvoiceNo");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = def.ShipmentId;
                //ad.SelectCommand.Parameters["@InvoiceDate"].Value = def.InvoiceDate;

                InvoiceNoParamDs dataSet = new InvoiceNoParamDs();
                ad.SelectCommand.DbCommand.CommandTimeout = 120;
                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected < 1)
                    throw new ApplicationException("Missing Invoice No. Parameter");

                InvoiceNoParamDs.InvoiceNoParamRow row = dataSet.InvoiceNoParam[0];

                if (row.SampleType == "1" || row.SampleType == "2")
                {
                    DateTime fiscalYearStartDate = DateTime.Parse("1/2/" + (DateTime.Today.Year - (DateTime.Today.Month < 2 ? 1 : 0)).ToString());
                    budgetYear = (DateTime.Today > fiscalYearStartDate && def.InvoiceDate < fiscalYearStartDate ? fiscalYearStartDate.Year : calenderDef.BudgetYear);
                }
                else
                    budgetYear = calenderDef.BudgetYear;

                ad = getDataSetAdapter("InvoiceNoParamApt", "GetInvoiceNoParamByKey");
                ad.SelectCommand.Parameters["@OfficeId"].Value = row.OfficeId;
                ad.SelectCommand.Parameters["@CustomerId"].Value = row.CustomerId;
                ad.SelectCommand.Parameters["@TradingAgencyId"].Value = row.TradingAgencyId;
                ad.SelectCommand.Parameters["@SampleType"].Value = row.SampleType;
                ad.SelectCommand.Parameters["@SpecialOrderTypeId"].Value = row.SpecialOrderTypeId;
                ad.SelectCommand.Parameters["@InvoiceYear"].Value = budgetYear;

                dataSet = new InvoiceNoParamDs();
                ad.SelectCommand.DbCommand.CommandTimeout = 120;
                ad.PopulateCommands();
                recordsAffected = ad.Fill(dataSet);

                row = dataSet.InvoiceNoParam[0];
                def.InvoicePrefix = row.InvoicePrefix;
                def.InvoiceSeqNo = row.InvoiceSeq;
                def.InvoiceYear = budgetYear;
                row.InvoiceSeq += 1;
                this.sealStamp(row, userId, Stamp.UPDATE);
                recordsAffected = ad.Update(dataSet);
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

        public void deleteShipmentDeduction(int paymentId, int shipmentId, int userId)
        {
            string paymentNo = string.Empty;
            AdvancePaymentDef apDef = AdvancePaymentWorker.Instance.getAdvancePaymentByKey(paymentId);
            if (apDef != null)
            {
                paymentNo = apDef.PaymentNo;
                ArrayList deleteList = this.getShipmentDeductionByLogicalKey(shipmentId, PaymentDeductionType.FABRIC_ADVANCE.Id, paymentNo);
                foreach (ShipmentDeductionDef def in deleteList)
                {
                    def.Status = 0;
                    this.updateShipmentDeduction(def, userId);
                }
            }
        }

        public void issueInvoice(InvoiceDef def, bool isNew, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (def.InvoiceDate == DateTime.MinValue)
                    throw new ApplicationException("Invalid invoice date, please assign invoice date before calling issueInvoice()");

                ShipmentDef shipment = OrderSelectWorker.Instance.getShipmentByKey(def.ShipmentId);

                if (isNew) fillNextInvoiceNo(def, userId);
                def.InvoiceUploadDate = DateTime.Now;
                def.InvoiceUploadUser = generalWorker.getUserByKey(userId);
                def.InvoiceSellExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, shipment.SellCurrency.CurrencyId, def.InvoiceDate);
                def.InvoiceBuyExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, shipment.BuyCurrency.CurrencyId, def.InvoiceDate);

                ArrayList splitShipments = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(def.ShipmentId);
                foreach (SplitShipmentDef splitShipmentDef in splitShipments)
                {
                    splitShipmentDef.InvoiceBuyExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, splitShipmentDef.BuyCurrency.CurrencyId, def.InvoiceDate);
                    OrderWorker.Instance.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipmentDef), userId);
                }

                shipment.USExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, def.InvoiceDate);
                shipment.EditLock = true;
                shipment.PaymentLock = true;
                shipment.WorkflowStatus = ContractWFS.INVOICED;
                OrderWorker.Instance.updateShipmentList(ConvertUtility.createArrayList(shipment), userId);

                ActionHistoryDef actionHistory = new ActionHistoryDef();
                actionHistory.ShipmentId = def.ShipmentId;
                actionHistory.ActionDate = DateTime.Now;
                actionHistory.ActionHistoryType = ActionHistoryType.SHIPPING_UPDATES;
                actionHistory.SplitShipmentId = 0;
                actionHistory.Remark = "Issued Invoice : " + getInvoiceNo(def.InvoicePrefix, def.InvoiceSeqNo, def.InvoiceYear);
                actionHistory.Status = 1;
                actionHistory.User = generalWorker.getUserByKey(userId);
                updateActionHistory(actionHistory);

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

        public void updateShipmentSummaryTotal(int shipmentId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentId);
                InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(shipmentId);

                shipmentDef.TotalShippedQuantity = 0;
                shipmentDef.TotalShippedAmount = 0;
                shipmentDef.TotalShippedAmountAfterDiscount = 0;
                shipmentDef.TotalShippedNetFOBAmount = 0;
                shipmentDef.TotalShippedNetFOBAmountAfterDiscount = 0;
                shipmentDef.TotalShippedSupplierGarmentAmount = 0;
                shipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount = 0;
                shipmentDef.TotalShippedOPAUpcharge = 0;
                shipmentDef.TotalShippedFreightCost = 0;
                shipmentDef.TotalShippedDutyCost = 0;
                shipmentDef.TotalShippedOtherCost = 0;

                ArrayList shipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(shipmentId);

                foreach (ShipmentDetailDef shipmentDetailDef in shipmentDetailList)
                {
                    shipmentDef.TotalShippedQuantity += shipmentDetailDef.ShippedQuantity;
                    shipmentDef.TotalShippedAmount += shipmentDetailDef.ShippedAmount;
                    shipmentDef.TotalShippedAmountAfterDiscount += shipmentDetailDef.ShippedAmountAfterDiscount;
                    shipmentDef.TotalShippedNetFOBAmount += shipmentDetailDef.ShippedNetFOBAmount;
                    shipmentDef.TotalShippedNetFOBAmountAfterDiscount += shipmentDetailDef.ShippedNetFOBAmountAfterDiscount;
                    shipmentDef.TotalShippedSupplierGarmentAmount += shipmentDetailDef.ShippedSupplierGarmentAmount;
                    shipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount += shipmentDetailDef.ShippedSupplierGarmentAmountAfterDiscount;
                    shipmentDef.TotalShippedOPAUpcharge += shipmentDetailDef.ShippedOPAUpchargeAmount;
                    shipmentDef.TotalShippedFreightCost += shipmentDetailDef.ShippedFreightCostAmount;
                    shipmentDef.TotalShippedDutyCost += shipmentDetailDef.ShippedDutyCostAmount;
                    shipmentDef.TotalShippedOtherCost += shipmentDetailDef.TotalShippedOtherCostAmount;
                }
                OrderWorker.Instance.updateShipmentList(ConvertUtility.createArrayList(shipmentDef), userId);

                if (shipmentDef.SplitCount > 0)
                {
                    ArrayList splitShipmentList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(shipmentId);
                    foreach (SplitShipmentDef splitShipmentDef in splitShipmentList)
                    {
                        this.updateSplitShipmentSummaryTotal(splitShipmentDef.SplitShipmentId, userId);
                    }
                }
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

        public void updateSplitShipmentSummaryTotal(int splitShipmentId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                SplitShipmentDef splitShipmentDef = OrderSelectWorker.Instance.getSplitShipmentByKey(splitShipmentId);

                splitShipmentDef.TotalShippedAmount = 0;
                splitShipmentDef.TotalShippedAmountAfterDiscount = 0;
                splitShipmentDef.TotalShippedNetFOBAmount = 0;
                splitShipmentDef.TotalShippedNetFOBAmountAfterDiscount = 0;
                splitShipmentDef.TotalShippedOPAUpcharge = 0;
                splitShipmentDef.TotalShippedQuantity = 0;
                splitShipmentDef.TotalShippedSupplierGarmentAmount = 0;
                splitShipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount = 0;

                ArrayList splitShipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentDetailBySplitShipmentId(splitShipmentDef.SplitShipmentId);
                foreach (SplitShipmentDetailDef splitShipmentDetailDef in splitShipmentDetailList)
                {
                    splitShipmentDef.TotalShippedQuantity += splitShipmentDetailDef.ShippedQuantity;
                    splitShipmentDef.TotalShippedAmount += splitShipmentDetailDef.ShippedAmount;
                    splitShipmentDef.TotalShippedAmountAfterDiscount += splitShipmentDetailDef.ShippedAmountAfterDiscount;
                    splitShipmentDef.TotalShippedNetFOBAmount += splitShipmentDetailDef.ShippedNetFOBAmount;
                    splitShipmentDef.TotalShippedNetFOBAmountAfterDiscount += splitShipmentDetailDef.ShippedNetFOBAmountAfterDiscount;
                    splitShipmentDef.TotalShippedSupplierGarmentAmount += splitShipmentDetailDef.ShippedSupplierGarmentAmount;
                    splitShipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount += splitShipmentDetailDef.ShippedSupplierGarmentAmountAfterDiscount;
                    splitShipmentDef.TotalShippedOPAUpcharge += splitShipmentDetailDef.ShippedOPAUpchargeAmount;
                }
                OrderWorker.Instance.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipmentDef), userId);
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

        public void updateDocument(DocumentDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("DocumentApt", "GetDocumentByKey");
                ad.SelectCommand.Parameters["@DocId"].Value = def.DocId;
                ad.PopulateCommands();

                DocumentDs dataSet = new DocumentDs();
                DocumentDs.DocumentRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.Document[0];
                    this.DocumentMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.Document.NewDocumentRow();
                    def.DocId = this.getMaxDocumentId() + 1;
                    this.DocumentMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    //sealStamp(row, userId, Stamp.UPDATE);
                    dataSet.Document.AddDocumentRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Document ERROR");
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

        public void updateShipmentDeduction(ShipmentDeductionDef def, int userId)
        {
            ArrayList amendmentList = new ArrayList();
            updateShipmentDeduction(def, ActionHistoryType.MISCELLANEOUS, amendmentList, userId);
            foreach (ActionHistoryDef actionHistory in amendmentList)
                updateActionHistory(actionHistory);
        }

        public void updateShipmentDeduction(ShipmentDeductionDef def, ActionHistoryType actionType, ArrayList amendmentList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ShipmentDeductionApt", "GetShipmentDeductionByKey");
                ad.SelectCommand.Parameters["@ShipmentDeductionId"].Value = def.ShipmentDeductionId;
                ad.PopulateCommands();

                ShipmentDeductionDs dataSet = new ShipmentDeductionDs();
                ShipmentDeductionDs.ShipmentDeductionRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                ActionHistoryDef log = null;
                bool goingToUpdate = false;
                if (recordsAffected > 0)
                {
                    row = dataSet.ShipmentDeduction[0];
                    PaymentDeductionType fromType = PaymentDeductionType.getType(row.DeductionType);
                    string fromDeduction = fromType.Name + (fromType.RequireDocumentNo ? " '" + (row.DocNo != null ? row.DocNo : "") + "'" : "");
                    decimal fromAmt = row.Amt;
                    int fromStatus = row.Status;

                    this.ShipmentDeductionMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                    goingToUpdate = true;

                    PaymentDeductionType toType = PaymentDeductionType.getType(row.DeductionType);
                    string toDeduction = toType.Name + (toType.RequireDocumentNo ? " '" + (row.DocNo != null ? row.DocNo : "") + "'" : "");
                    decimal toAmt = row.Amt;
                    int toStatus = row.Status;
                    if (fromStatus == 1)
                        if (toStatus == 0)
                            log = getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.SHIPMENT_DEDUCTION,
                                    fromDeduction + " $" + fromAmt.ToString(), "CANCELLED", userId);
                        else if (fromDeduction != toDeduction || fromAmt != toAmt)
                            log = getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.SHIPMENT_DEDUCTION,
                                    fromDeduction + (fromAmt != toAmt ? " $" + fromAmt.ToString() : ""),
                                    (fromDeduction != toDeduction ? toDeduction : "") + (fromAmt != toAmt ? " $" + toAmt.ToString() : ""),
                                    userId);
                }
                else if (def.Status == 1)
                {
                    row = dataSet.ShipmentDeduction.NewShipmentDeductionRow();
                    def.ShipmentDeductionId = this.getMaxShipmentDeductionId() + 1;
                    this.ShipmentDeductionMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.ShipmentDeduction.AddShipmentDeductionRow(row);
                    goingToUpdate = true;

                    PaymentDeductionType newType = PaymentDeductionType.getType(row.DeductionType);
                    log = getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.SHIPMENT_DEDUCTION, "New Deduction",
                                newType.Name + (newType.RequireDocumentNo ? " '" + row.DocNo + "' " : " ") + "$" + row.Amt.ToString(),
                                userId);
                }

                if (log != null && amendmentList != null)
                    amendmentList.Add(log);

                if (goingToUpdate)
                {
                    recordsAffected = ad.Update(dataSet);
                    if (recordsAffected < 1)
                        throw new DataAccessException("Update Shipment Deduction ERROR");
                }
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

        public void updateActionHistory(ActionHistoryDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (def.ActionHistoryId <= 0)
                    for (int i = 0; i < 10; i++)
                        if ((def.ActionHistoryId = createNewActionHistoryRecord()) > 0) break;

                IDataSetAdapter ad = getDataSetAdapter("ActionHistoryApt", "GetActionHistoryByKey");
                ad.SelectCommand.Parameters["@ActionHistoryId"].Value = def.ActionHistoryId;
                ad.PopulateCommands();

                ActionHistoryDs dataSet = new ActionHistoryDs();
                ActionHistoryDs.ActionHistoryRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ActionHistory[0];
                    this.ActionHistoryMapping(def, row);
                    recordsAffected = ad.Update(dataSet);
                }
                /*
                else
                {
                    row = dataSet.ActionHistory.NewActionHistoryRow();
                    def.ActionHistoryId = this.getMaxActionHistoryId() + 1;
                    this.ActionHistoryMapping(def, row);
                    dataSet.ActionHistory.AddActionHistoryRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                */
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Action History ERROR");
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

        public void resetShippedQty(int shipmentId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                ArrayList shipmentDetails = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(shipmentId);
                foreach (ShipmentDetailDef shipmentDetail in shipmentDetails)
                {
                    shipmentDetail.ShippedQuantity = 0;
                }
                if (shipmentDetails.Count > 0)
                    OrderWorker.Instance.updateShipmentDetailList(shipmentDetails, ActionHistoryType.PACKING_LIST_RESET, null, 99999);

                ArrayList splitShipmentDetails = (ArrayList)OrderSelectWorker.Instance.getUpdatableSplitShipmentDetailByShipmentId(shipmentId);
                foreach (SplitShipmentDetailDef splitShipmentDetail in splitShipmentDetails)
                {
                    splitShipmentDetail.ShippedQuantity = 0;
                }
                if (splitShipmentDetails.Count > 0)
                    OrderWorker.Instance.updateSplitShipmentDetailList(shipmentId, splitShipmentDetails, ActionHistoryType.PACKING_LIST_RESET, null, 99999);

                this.updateShipmentSummaryTotal(shipmentId, 99999);

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


        public InvoiceDef getInvoiceByKey(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceByKey");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            InvoiceDs dataSet = new InvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            InvoiceDef def = new InvoiceDef();
            this.InvoiceMapping(dataSet.Invoice[0], def);
            return def;
        }

        public ArrayList getInvoiceByEInvoiceBatchId(int eInvoiceBatchId)
        {
            IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceByEInvoiceBatchId");
            ad.SelectCommand.Parameters["@eInvoiceBatchId"].Value = eInvoiceBatchId;

            InvoiceDs dataSet = new InvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ArrayList list = new ArrayList();
            foreach (InvoiceDs.InvoiceRow row in dataSet.Invoice)
            {
                InvoiceDef def = new InvoiceDef();
                InvoiceMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        public InvoiceDef getInvoiceByInvoiceNo(string invoicePrefix, int invoiceSeq, int invoiceYear)
        {
            IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceByInvoiceNo");
            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@InvoiceSeq"].Value = invoiceSeq;
            ad.SelectCommand.Parameters["@InvoiceYear"].Value = invoiceYear;

            InvoiceDs dataSet = new InvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            InvoiceDef def = new InvoiceDef();
            this.InvoiceMapping(dataSet.Invoice[0], def);
            return def;
        }

        public ArrayList getInvoiceByInvoiceNo(string invoicePrefix, int invoiceSeq, int invoiceYear, int sequenceNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceByInvoiceNoAndSequenceNo");
            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@InvoiceSeq"].Value = invoiceSeq;
            ad.SelectCommand.Parameters["@InvoiceYear"].Value = invoiceYear;

            if (sequenceNo == -1)
                ad.SelectCommand.Parameters["@SequenceNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@SequenceNo"].Value = sequenceNo;

            InvoiceDs dataSet = new InvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ArrayList list = new ArrayList();
            foreach (InvoiceDs.InvoiceRow row in dataSet.Invoice)
            {
                InvoiceDef def = new InvoiceDef();
                InvoiceMapping(row, def);
                list.Add(def);
            }

            return list;
        }

        public ArrayList getShipmentList(string contractNo, string invoiceNo, string itemNo, TypeCollector officeIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetShipmentList");
            if (contractNo == null || contractNo == String.Empty)
                ad.SelectCommand.Parameters["@ContractNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;

            if (invoiceNo == null || invoiceNo == String.Empty)
            {
                ad.SelectCommand.Parameters["@InvoicePrefix"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@InvoiceSeq"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@InvoiceYear"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@InvoicePrefix"].Value = ShippingWorker.getInvoicePrefix(invoiceNo);
                ad.SelectCommand.Parameters["@InvoiceSeq"].Value = ShippingWorker.getInvoiceSeq(invoiceNo);
                ad.SelectCommand.Parameters["@InvoiceYear"].Value = ShippingWorker.getInvoiceYear(invoiceNo);
            }

            if (itemNo == null || itemNo == String.Empty)
                ad.SelectCommand.Parameters["@ItemNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;

            if (officeIdList != null)
                ad.SelectCommand.CustomParameters["@OfficeCodeList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            else
                ad.SelectCommand.CustomParameters["@OfficeCodeList"] = CustomDataParameter.parse(false, ConvertUtility.createArrayList(-1));

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();
            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public ArrayList getShipmentList(string contractNo, int deliveryNo, string itemNo, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo,
                    int invoiceYear, int vendorId, int customerId, string supplierInvoiceNoFrom, string supplierInvoiceNoTo, TypeCollector officeList,
                    DateTime invoiceDateFrom, DateTime invoiceDateTo, int productTeamId, string orderType, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo,
                    DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo, DateTime ilsActualAtWHDateFrom, DateTime ilsActualAtWHDateTo,
                    DateTime invoiceSentDateFrom, DateTime invoiceSentDateTo,
                     int oprTypeId, int customerDestinationId, int countryOfOriginId, int termOfPurchaseId, string docNo, int invoiceUploadUserId,
                    TypeCollector workflowStatusList, TypeCollector shipmentMethodList,
                    int splitOnly, int szOrderOnly, int sampleOnly, int ldpOrder, int withQCCharge, int isReprocessGoods, int isGBTestRequired, int isQccInspection, int isTradingAF)
        {

            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetShipmentAdvanceSearchList");

            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@InvoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@InvoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@InvoiceYear"].Value = invoiceYear;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@CustomerId"].Value = customerId;
            ad.SelectCommand.Parameters["@SupplierInvoiceNoFrom"].Value = supplierInvoiceNoFrom;
            ad.SelectCommand.Parameters["@SupplierInvoiceNoTo"].Value = supplierInvoiceNoTo;

            ad.SelectCommand.CustomParameters["@OfficeCodeList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);

            if (invoiceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
            if (invoiceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;

            ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
            if (orderType.Trim() == "")
                ad.SelectCommand.Parameters["@OrderType"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@OrderType"].Value = orderType;
            if (invoiceUploadDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceUploadDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
            if (invoiceUploadDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceUploadDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceUploadDateTo"].Value = invoiceUploadDateTo;
            ad.SelectCommand.Parameters["@OPRTypeId"].Value = oprTypeId;
            ad.SelectCommand.Parameters["@CustomerDestinationId"].Value = customerDestinationId;
            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;
            if (docNo.Trim() == "")
                ad.SelectCommand.Parameters["@DocNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DocNo"].Value = docNo;
            ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);
            ad.SelectCommand.CustomParameters["@ShipmentMethodList"] = CustomDataParameter.parse(shipmentMethodList.IsInclusive, shipmentMethodList.Values);

            ad.SelectCommand.Parameters["@SplitOnly"].Value = splitOnly;
            ad.SelectCommand.Parameters["@SZOrderOnly"].Value = szOrderOnly;

            if (customerAtWHDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@CustomerAtWarehouseDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@CustomerAtWarehouseDateFrom"].Value = customerAtWHDateFrom;
            if (customerAtWHDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@CustomerAtWarehouseDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@CustomerAtWarehouseDateTo"].Value = customerAtWHDateTo;

            if (ilsActualAtWHDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ILSActualAtWarehouseDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ILSActualAtWarehouseDateFrom"].Value = ilsActualAtWHDateFrom;
            if (ilsActualAtWHDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ILSActualAtWarehouseDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ILSActualAtWarehouseDateTo"].Value = ilsActualAtWHDateTo;

            ad.SelectCommand.Parameters["@CountryOfOriginId"].Value = countryOfOriginId;

            ad.SelectCommand.Parameters["@IsSample"].Value = sampleOnly;
            ad.SelectCommand.Parameters["@IsLDPOrder"].Value = ldpOrder;
            ad.SelectCommand.Parameters["@WithQCCharge"].Value = withQCCharge;
            ad.SelectCommand.Parameters["@IsReprocessGoods"].Value = isReprocessGoods;
            ad.SelectCommand.Parameters["@IsGBTestRequired"].Value = isGBTestRequired;
            ad.SelectCommand.Parameters["@IsQccInspection"].Value = isQccInspection;
            ad.SelectCommand.Parameters["@IsTradingAF"].Value = isTradingAF;

            ad.SelectCommand.Parameters["@InvoiceUploadUserId"].Value = invoiceUploadUserId;
            if (invoiceSentDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceSentDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceSentDateFrom"].Value = invoiceSentDateFrom;
            if (invoiceSentDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceSentDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceSentDateTo"].Value = invoiceSentDateTo;

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }



        public ArrayList getShipmentList(string contractNo, int deliveryNo, string itemNo, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo,
                    int invoiceYear, int vendorId, TypeCollector customerList, string supplierInvoiceNoFrom, string supplierInvoiceNoTo, TypeCollector officeList,
                    DateTime invoiceDateFrom, DateTime invoiceDateTo, int productTeamId, string orderType, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo,
                    DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo, DateTime ilsActualAtWHDateFrom, DateTime ilsActualAtWHDateTo,
                    DateTime invoiceSentDateFrom, DateTime invoiceSentDateTo,
                     int oprTypeId, int customerDestinationId, int countryOfOriginId, int termOfPurchaseId, string docNo, int invoiceUploadUserId,
                    TypeCollector workflowStatusList, TypeCollector shipmentMethodList,
                    int splitOnly, int szOrderOnly, int sampleOnly, int ldpOrder, int withQCCharge,
                    string lcNoFrom, string lcNoTo, DateTime ActualAtWHDateFrom, DateTime ActualAtWHDateTo, int ShippingDocumentReceiptStatus, int LcPaymentCheckStatus,
                    TypeCollector shippingUserIdList, string SortingOrder, int userId)
        {

            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetShipmentMassUpdateList");

            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@InvoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@InvoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@InvoiceYear"].Value = invoiceYear;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            //ad.SelectCommand.Parameters["@CustomerId"].Value = customerId;
            ad.SelectCommand.Parameters["@SupplierInvoiceNoFrom"].Value = supplierInvoiceNoFrom;
            ad.SelectCommand.Parameters["@SupplierInvoiceNoTo"].Value = supplierInvoiceNoTo;

            ad.SelectCommand.CustomParameters["@OfficeCodeList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);
            ad.SelectCommand.CustomParameters["@ShippingUserIdList"] = CustomDataParameter.parse(shippingUserIdList.IsInclusive, shippingUserIdList.Values);

            if (invoiceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
            if (invoiceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;

            ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
            if (orderType.Trim() == "")
                ad.SelectCommand.Parameters["@OrderType"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@OrderType"].Value = orderType;
            if (invoiceUploadDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceUploadDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
            if (invoiceUploadDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceUploadDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceUploadDateTo"].Value = invoiceUploadDateTo;
            ad.SelectCommand.Parameters["@OPRTypeId"].Value = oprTypeId;
            ad.SelectCommand.Parameters["@CustomerDestinationId"].Value = customerDestinationId;
            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;
            if (docNo.Trim() == "")
                ad.SelectCommand.Parameters["@DocNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DocNo"].Value = docNo;
            if (customerAtWHDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@CustomerAtWarehouseDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@CustomerAtWarehouseDateFrom"].Value = customerAtWHDateFrom;
            if (customerAtWHDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@CustomerAtWarehouseDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@CustomerAtWarehouseDateTo"].Value = customerAtWHDateTo;

            ad.SelectCommand.Parameters["@CountryOfOriginId"].Value = countryOfOriginId;

            ad.SelectCommand.Parameters["@SplitOnly"].Value = splitOnly;
            ad.SelectCommand.Parameters["@SZOrderOnly"].Value = szOrderOnly;
            ad.SelectCommand.Parameters["@IsSample"].Value = sampleOnly;
            ad.SelectCommand.Parameters["@IsLDPOrder"].Value = ldpOrder;
            ad.SelectCommand.Parameters["@WithQCCharge"].Value = withQCCharge;
            ad.SelectCommand.Parameters["@InvoiceUploadUserId"].Value = invoiceUploadUserId;
            if (invoiceSentDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceSentDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceSentDateFrom"].Value = invoiceSentDateFrom;
            if (invoiceSentDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceSentDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceSentDateTo"].Value = invoiceSentDateTo;

            ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);
            ad.SelectCommand.CustomParameters["@ShipmentMethodList"] = CustomDataParameter.parse(shipmentMethodList.IsInclusive, shipmentMethodList.Values);
            ad.SelectCommand.CustomParameters["@CustomerList"] = CustomDataParameter.parse(customerList.IsInclusive, customerList.Values);

            if (ilsActualAtWHDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ILSActualAtWarehouseDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ILSActualAtWarehouseDateFrom"].Value = ilsActualAtWHDateFrom;
            if (ilsActualAtWHDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ILSActualAtWarehouseDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ILSActualAtWarehouseDateTo"].Value = ilsActualAtWHDateTo;

            ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom;
            ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo;

            if (ActualAtWHDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ActualAtWarehouseDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ActualAtWarehouseDateFrom"].Value = ActualAtWHDateFrom;
            if (ActualAtWHDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ActualAtWarehouseDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ActualAtWarehouseDateTo"].Value = ActualAtWHDateTo;

            ad.SelectCommand.Parameters["@ShippingDocumentReceiptStatus"].Value = ShippingDocumentReceiptStatus;
            ad.SelectCommand.Parameters["@LCPaymentCheckStatus"].Value = LcPaymentCheckStatus;

            ad.SelectCommand.Parameters["@UserId"].Value = userId;

            if (SortingOrder != "")
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY " + SortingOrder;

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        /// <summary>
        /// 2017-12-12, Alan
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public ArrayList getShipmentListByAdvancePaymentId(int paymentId)
        {
            ArrayList list = new ArrayList();
            if (paymentId <= 0)
            {
                return list;
            }
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetShipmentListByAdvancePaymentVendor");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getInvoiceList(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, TypeCollector officeIdList, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, int termOfPurchaseId, int workflowStatusId,
            int tradingAgencyId, DateTime purchaseScanDateFrom, DateTime purchaseScanDateTo, int purchaseScanStatus, string orderType, int currencyId,
            DateTime salesScanDateFrom, DateTime salesScanDateTo, DateTime submittedOnFrom, DateTime submittedOnTo, string batchNo, int invoiceBatchStatus)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetInvoiceList");

            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@InvoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@InvoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@InvoiceYear"].Value = invoiceYear;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            if (officeIdList != null)
                ad.SelectCommand.CustomParameters["@OfficeCodeList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);

            if (invoiceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
            if (invoiceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;

            if (invoiceUploadDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceUploadDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
            if (invoiceUploadDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceUploadDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceUploadDateTo"].Value = invoiceUploadDateTo;
            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@WorkflowStatusId"].Value = workflowStatusId;
            ad.SelectCommand.Parameters["@TradingAgencyId"].Value = tradingAgencyId;
            if (purchaseScanDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PurchaseScanDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PurchaseScanDateFrom"].Value = purchaseScanDateFrom;
            if (purchaseScanDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PurchaseScanDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PurchaseScanDateTo"].Value = purchaseScanDateTo;

            ad.SelectCommand.Parameters["@PurchaseScanStatus"].Value = purchaseScanStatus;


            ad.SelectCommand.Parameters["@OrderType"].Value = orderType;
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;
            if (salesScanDateFrom != DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@SalesScanDateFrom"].Value = salesScanDateFrom;
                ad.SelectCommand.Parameters["@SalesScanDateTo"].Value = salesScanDateTo;
            }
            else
            {
                ad.SelectCommand.Parameters["@SalesScanDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@SalesScanDateTo"].Value = DBNull.Value;
            }

            if (submittedOnFrom != DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@SubmittedOnFrom"].Value = submittedOnFrom;
                ad.SelectCommand.Parameters["@SubmittedOnTo"].Value = submittedOnTo;
            }
            else
            {
                ad.SelectCommand.Parameters["@SubmittedOnFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@SubmittedOnTo"].Value = DBNull.Value;
            }

            ad.SelectCommand.Parameters["@BatchNo"].Value = batchNo;
            ad.SelectCommand.Parameters["@InvoiceBatchStatus"].Value = invoiceBatchStatus;

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getInvoiceListForInvoiceBatch(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, int workflowStatusId, int tradingAgencyId, string orderType, int currencyId,
            DateTime salesScanDateFrom, DateTime salesScanDateTo, DateTime submittedOnFrom, DateTime submittedOnTo, string batchNo, int invoiceBatchStatus)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetInvoiceListForInvoiceBatch");

            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@InvoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@InvoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@InvoiceYear"].Value = invoiceYear;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            if (invoiceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
            if (invoiceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;

            ad.SelectCommand.Parameters["@WorkflowStatusId"].Value = workflowStatusId;
            ad.SelectCommand.Parameters["@TradingAgencyId"].Value = tradingAgencyId;



            ad.SelectCommand.Parameters["@OrderType"].Value = orderType;
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;
            if (salesScanDateFrom != DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@SalesScanDateFrom"].Value = salesScanDateFrom;
                ad.SelectCommand.Parameters["@SalesScanDateTo"].Value = salesScanDateTo;
            }
            else
            {
                ad.SelectCommand.Parameters["@SalesScanDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@SalesScanDateTo"].Value = DBNull.Value;
            }

            if (submittedOnFrom != DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@SubmittedOnFrom"].Value = submittedOnFrom;
                ad.SelectCommand.Parameters["@SubmittedOnTo"].Value = submittedOnTo;
            }
            else
            {
                ad.SelectCommand.Parameters["@SubmittedOnFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@SubmittedOnTo"].Value = DBNull.Value;
            }

            ad.SelectCommand.Parameters["@BatchNo"].Value = batchNo;
            ad.SelectCommand.Parameters["@InvoiceBatchStatus"].Value = invoiceBatchStatus;

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public ArrayList GetInvoiceListByLcBillRefNo(string lcBillRefNo, int workflowStatusId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetInvoiceListByLcBillRefNo");

            ad.SelectCommand.Parameters["@WorkflowStatusId"].Value = workflowStatusId;
            ad.SelectCommand.Parameters["@LCBillRefNo"].Value = lcBillRefNo;

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getInvoiceSentList()
        {
            IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceSentList");

            InvoiceDs dataSet = new InvoiceDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (InvoiceDs.InvoiceRow row in dataSet.Invoice)
            {
                InvoiceDef def = new InvoiceDef();
                InvoiceMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getInvoiceListForDMSUpload()
        {
            IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceListForDMSUpload");

            InvoiceDs dataSet = new InvoiceDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (InvoiceDs.InvoiceRow row in dataSet.Invoice)
            {
                InvoiceDef def = new InvoiceDef();
                InvoiceMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public DateTime getLastShipmentDate(string itemNo, string contractNo, int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetLastShipmentDate");
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId.ToString();
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
                return (DateTime)dataSet.Tables[0].Rows[0][0];
            else
                return DateTime.MinValue;
        }

        public VendorOrderSummaryRef getVendorOrderSummary(int vendorId)
        {
            VendorOrderSummaryRef def = new VendorOrderSummaryRef();
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetLastShipmentDateByVendor");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
                def.LastShipmentDate = (DateTime)dataSet.Tables[0].Rows[0][0];

            ad = getDataSetAdapter("ContractShipmentApt", "GetFutureOrderInfoByVendor");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
            {
                if (!Convert.IsDBNull(dataSet.Tables[0].Rows[0][1]))
                    def.NextShipmentDate = (DateTime)dataSet.Tables[0].Rows[0][1];
                def.FutureOrderCount = (int)dataSet.Tables[0].Rows[0][2];
                def.FutureOrderTotalSalesInUSD = (decimal)dataSet.Tables[0].Rows[0][0];
            }
            return def;
        }

        public GenericOrderSummaryRef getItemOrderSummary(string itemNo, DateTime fromDate, DateTime toDate)
        {
            GenericOrderSummaryRef def = new GenericOrderSummaryRef();
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetShippedSummaryByItemNo");
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
            ad.SelectCommand.Parameters["@ToDate"].Value = toDate;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
            {
                def.OrderTotalQty = (int)dataSet.Tables[0].Rows[0][0];
                def.OrderTotalSalesInUSD = (decimal)dataSet.Tables[0].Rows[0][1];
            }
            return def;
        }

        public ArrayList getLast10Shipment(string itemNo, string contractNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetLast10Shipment");
            if (itemNo == null || itemNo == String.Empty)
                ad.SelectCommand.Parameters["@ItemNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;

            if (contractNo == null || contractNo == string.Empty)
                ad.SelectCommand.Parameters["@ContractNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();
            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getLatest10UKProductGroupByItemNo(string itemNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetLatest10UKProductGroupByItemNo");
            if (itemNo == null || itemNo == String.Empty)
                ad.SelectCommand.Parameters["@ItemNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();
            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getInvoiceListByInvoiceNo(string invoiceNo, int sequenceNo, TypeCollector officeIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetInvoiceListByInvoiceNo");

            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = ShippingWorker.getInvoicePrefix(invoiceNo);
            ad.SelectCommand.Parameters["@InvoiceSeq"].Value = ShippingWorker.getInvoiceSeq(invoiceNo);
            ad.SelectCommand.Parameters["@InvoiceYear"].Value = ShippingWorker.getInvoiceYear(invoiceNo);

            if (sequenceNo == -1)
                ad.SelectCommand.Parameters["@SequenceNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@SequenceNo"].Value = sequenceNo;

            if (officeIdList != null)
                ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            else
                ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(false, ConvertUtility.createArrayList(-1));

            ContractShipmentDs dataSet = new ContractShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();
            foreach (ContractShipmentDs.ContractShipmentRow row in dataSet.ContractShipment)
            {
                ContractShipmentListJDef def = new ContractShipmentListJDef();
                ContractShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ContractShipmentListJDef getInvoiceByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetInvoiceByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ContractShipmentDs dataSet = new ContractShipmentDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            ContractShipmentListJDef def = new ContractShipmentListJDef();

            if (recordsAffected > 0)
            {
                ContractShipmentMapping(dataSet.ContractShipment.Rows[0], def);
            }
            else
                return null;

            return def;
        }

        public ContractShipmentListJDef getSplitShipmentByPONo(string contractNo, string splitSuffix, int deliveryNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetSplitShipmentByPONo");
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@SplitSuffix"].Value = splitSuffix;
            ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;

            ContractShipmentDs dataSet = new ContractShipmentDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            ContractShipmentListJDef def = new ContractShipmentListJDef();

            if (recordsAffected > 0)
            {
                ContractShipmentMapping(dataSet.ContractShipment.Rows[0], def);
            }
            else
                return null;

            return def;
        }

        public ArrayList getLcShipmentByLcBillRefNo(string lcBillRefNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetLcShipmentByLcBillRefNo");
            ad.SelectCommand.Parameters["@lcBillRefNo"].Value = lcBillRefNo;

            ContractShipmentDs dataSet = new ContractShipmentDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();
            if (recordsAffected > 0)
            {
                foreach (ContractShipmentListJDef def in dataSet.ContractShipment.Rows)
                {
                    list.Add(def);
                }
            }
            else
                return null;

            return list;
        }

        public ArrayList getShipmentProductByItemNo(string itemNo, string contractNo, int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentProductApt", "GetShipmentProductByItemNo");
            /*
            ad.SelectCommand.MailSQL = true;
            */
            if (itemNo == null || itemNo == String.Empty)
                ad.SelectCommand.Parameters["@ItemNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            if (contractNo == null || contractNo == String.Empty)
                ad.SelectCommand.Parameters["@ContractNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            ShipmentProductDs dataSet = new ShipmentProductDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1)
            {
                dataSet = new ShipmentProductDs();
                ad.SelectCommand.Parameters["@ContractNo"].Value = DBNull.Value;
                recordsAffected = ad.Fill(dataSet);
            }

            ArrayList list = new ArrayList();
            foreach (ShipmentProductDs.ShipmentProductRow row in dataSet.ShipmentProduct)
            {
                ShipmentProductDef def = new ShipmentProductDef();
                ShipmentProductMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getShipmentProductByVendorId(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentProductApt", "GetShipmentProductByVendorId");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            ShipmentProductDs dataSet = new ShipmentProductDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();
            foreach (ShipmentProductDs.ShipmentProductRow row in dataSet.ShipmentProduct)
            {
                ShipmentProductDef def = new ShipmentProductDef();
                ShipmentProductMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public static string getInvoiceNo(string invoicePrefix, int invoiceSeq, int invoiceYear)
        {
            if (invoicePrefix != String.Empty && invoiceSeq != 0 && invoiceYear != 0)
                return invoicePrefix + "/" + invoiceSeq.ToString().PadLeft(5, '0') + "/" + invoiceYear.ToString();
            else
                return String.Empty;
        }

        public static string getInvoicePrefix(string invoiceNo)
        {
            string[] separater = { "/" };
            return invoiceNo.Split(separater, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        public static int getInvoiceSeq(string invoiceNo)
        {
            string[] separater = { "/" };
            int seq = 0;
            if (invoiceNo.Split(separater, StringSplitOptions.RemoveEmptyEntries).Length >= 2)
                seq = Convert.ToInt32(invoiceNo.Split(separater, StringSplitOptions.RemoveEmptyEntries)[1]);
            return seq;
        }

        public static int getInvoiceYear(string invoiceNo)
        {
            return Convert.ToInt32(invoiceNo.Substring(invoiceNo.LastIndexOf('/') + 1, 4));
        }

        public ArrayList getDocumentListByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("DocumentApt", "GetDocumentListByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            DocumentDs dataSet = new DocumentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (DocumentDs.DocumentRow row in dataSet.Document)
            {
                DocumentDef def = new DocumentDef();
                DocumentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getShipmentDeductionList(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentDeductionApt", "GetShipmentDeductionList");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ShipmentDeductionDs dataSet = new ShipmentDeductionDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ShipmentDeductionDs.ShipmentDeductionRow row in dataSet.ShipmentDeduction)
            {
                ShipmentDeductionDef def = new ShipmentDeductionDef();
                ShipmentDeductionMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getShipmentDeductionByKey(int shipmentDeductionId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentDeductionApt", "GetShipmentDeductionByKey");
            ad.SelectCommand.Parameters["@ShipmentDeductionId"].Value = shipmentDeductionId;
            ShipmentDeductionDs dataSet = new ShipmentDeductionDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ShipmentDeductionDs.ShipmentDeductionRow row in dataSet.ShipmentDeduction)
            {
                ShipmentDeductionDef def = new ShipmentDeductionDef();
                ShipmentDeductionMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getShipmentDeductionByLogicalKey(int shipmentId, int deductionTypeId, string docNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentDeductionApt", "GetShipmentDeductionByLogicalKey");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ad.SelectCommand.Parameters["@DeductionTypeId"].Value = deductionTypeId;
            ad.SelectCommand.Parameters["@DocNo"].Value = docNo;

            ShipmentDeductionDs dataSet = new ShipmentDeductionDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ShipmentDeductionDs.ShipmentDeductionRow row in dataSet.ShipmentDeduction)
            {
                ShipmentDeductionDef def = new ShipmentDeductionDef();
                ShipmentDeductionMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getAllShipmentDeductionByLogicalKey(int shipmentId, int deductionTypeId, string docNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentDeductionApt", "GetAllShipmentDeductionByLogicalKey");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ad.SelectCommand.Parameters["@DeductionTypeId"].Value = deductionTypeId;
            ad.SelectCommand.Parameters["@DocNo"].Value = docNo;

            ShipmentDeductionDs dataSet = new ShipmentDeductionDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ShipmentDeductionDs.ShipmentDeductionRow row in dataSet.ShipmentDeduction)
            {
                ShipmentDeductionDef def = new ShipmentDeductionDef();
                ShipmentDeductionMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public decimal calcShipmentDeductionTotal(ArrayList deductions)
        {
            decimal total = 0;
            foreach (ShipmentDeductionDef d in deductions)
                if (d.Status == 1)
                    total += d.Amount * d.DeductionType.Factor;
            return total;
        }

        public DataSet getNSSDiscrepancyDataSet(DateTime startDate)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSSDiscrepancyApt", "GetNSSDiscrepancy");
            ad.SelectCommand.Parameters["@StartDate"].Value = startDate;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public ArrayList getActionHistoryByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ActionHistoryApt", "GetActionHistoryByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ActionHistoryDs dataSet = new ActionHistoryDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ActionHistoryDs.ActionHistoryRow row in dataSet.ActionHistory)
            {
                ActionHistoryDef def = new ActionHistoryDef();
                ActionHistoryMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getShipmentToDMSList(int officeId, DateTime docDateFrom,
            DateTime docDateTo, int paymentTermId, int vendorId, int checkStatus, string contractNo, string itemNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentToDMSApt", "GetShipmentToDMSList");

            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@PaymentTermId"].Value = paymentTermId;

            if (docDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DocReceiptDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DocReceiptDateFrom"].Value = docDateFrom;
            if (docDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DocReceiptDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DocReceiptDateTo"].Value = docDateTo;
            ad.SelectCommand.Parameters["@CheckStatus"].Value = checkStatus;
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;

            ShipmentToDMSDs dataSet = new ShipmentToDMSDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ShipmentToDMSDs.ShipmentToDMSRow row in dataSet.ShipmentToDMS)
            {
                ShipmentToDMSDef def = new ShipmentToDMSDef();
                def.ShipmentId = row.ShipmentId;
                def.SupplierName = row.VendorName;
                def.ContractNo = row.ContractNo + "-" + row.DeliveryNo.ToString();
                def.InvoiceNo = row.InvoiceNo;
                if (!row.IsSupplierInvoiceNoNull())
                    def.SupplierInvoiceNo = row.SupplierInvoiceNo;
                else
                    def.SupplierInvoiceNo = String.Empty;
                def.CurrencyCode = CurrencyId.getName(row.CurrencyId);
                def.QACommissionPercent = row.QACommissionPercent;
                def.VendorPaymentDiscountPercent = row.VendorPaymentDiscountPercent;
                def.QACommissionAmount = Math.Round(row.TotalShippedSupplierGmtAmtAfterDiscount * row.QACommissionPercent / 100, 2, MidpointRounding.AwayFromZero);
                def.VendorPaymentDiscountAmount = Math.Round(row.TotalShippedSupplierGmtAmtAfterDiscount * row.VendorPaymentDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);
                def.LabTestIncome = row.LabTestIncome * row.TotalShippedQty;
                def.NetAmount = row.TotalShippedSupplierGmtAmtAfterDiscount - def.VendorPaymentDiscountAmount - def.QACommissionAmount - def.LabTestIncome;
                def.TotalShippedQty = row.TotalShippedQty;
                def.ItemNo = row.ItemNo;

                if (!row.IsShippingDocReceiptDateNull())
                    def.ShippingDocReceiptDate = row.ShippingDocReceiptDate;
                else
                    def.ShippingDocReceiptDate = DateTime.MinValue;
                def.VendorId = row.VendorId;
                def.InvoiceDate = row.InvoiceDate;
                def.CustomerCode = row.CustomerCode;
                def.SplitCount = row.SplitCount;
                def.IsUploadDMSDocument = row.IsUploadDMSDocument;
                def.ShippingDocCheckedBy = null;
                if (!row.IsShippingDocCheckedByNull()) def.ShippingDocCheckedBy = generalWorker.getUserByKey(row.ShippingDocCheckedBy);
                if (!row.IsShippingDocCheckedOnNull())
                    def.ShippingDocCheckedDate = row.ShippingDocCheckedOn;
                else
                    def.ShippingDocCheckedDate = DateTime.MinValue;

                list.Add(def);
            }
            return list;
        }

        public ArrayList getActionHistoryByShipmentIdAndType(int shipmentId, int actionHistoryTypeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ActionHistoryApt", "GetActionHistoryByShipmentIdAndType");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ad.SelectCommand.Parameters["@ActionHistoryTypeId"].Value = actionHistoryTypeId;
            ActionHistoryDs dataSet = new ActionHistoryDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ActionHistoryDs.ActionHistoryRow row in dataSet.ActionHistory)
            {
                ActionHistoryDef def = new ActionHistoryDef();
                ActionHistoryMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getActionHistoryByTypeAndDay(int actionHistoryTypeId, int day)
        {
            IDataSetAdapter ad = getDataSetAdapter("ActionHistoryApt", "GetActionHistoryByTypeAndActionDate");
            DateTime dayBefore = DateTime.Today.AddDays(day);
            ad.SelectCommand.Parameters["@ActionHistoryTypeId"].Value = actionHistoryTypeId;
            ad.SelectCommand.Parameters["@Year"].Value = dayBefore.Year;
            ad.SelectCommand.Parameters["@Month"].Value = dayBefore.Month;
            ad.SelectCommand.Parameters["@Day"].Value = dayBefore.Day;
            ActionHistoryDs dataSet = new ActionHistoryDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ActionHistoryDs.ActionHistoryRow row in dataSet.ActionHistory)
            {
                ActionHistoryDef def = new ActionHistoryDef();
                ActionHistoryMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getActionHistoryByShipmentIdAndTypeAndDay(int shipmentId, int actionHistoryTypeId, int dayFrom)
        {
            IDataSetAdapter ad = getDataSetAdapter("ActionHistoryApt", "GetActionHistoryByShipmentAndTypeAndActionDateRange");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ad.SelectCommand.Parameters["@ActionHistoryTypeId"].Value = actionHistoryTypeId;
            ad.SelectCommand.Parameters["@dayFrom"].Value = DateTime.Today.AddDays(dayFrom);
            ad.SelectCommand.Parameters["@dayTo"].Value = DateTime.Now;

            ActionHistoryDs dataSet = new ActionHistoryDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ActionHistoryDs.ActionHistoryRow row in dataSet.ActionHistory)
            {
                ActionHistoryDef def = new ActionHistoryDef();
                ActionHistoryMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public DocumentDef getDocumentByKey(int docId)
        {
            IDataSetAdapter ad = getDataSetAdapter("DocumentApt", "GetDocumentByKey");
            ad.SelectCommand.Parameters["@DocId"].Value = docId;
            DocumentDs dataSet = new DocumentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            DocumentDef def = null;

            if (dataSet.Document.Rows.Count > 0)
            {
                def = new DocumentDef();
                DocumentMapping(dataSet.Document.Rows[0], def);
            }

            return def;
        }

        public ArrayList getDocumentListByContractNoAndDeliveryNo(string contractNo, int deliveryNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("DocumentApt", "GetDocumentListByContractNoAndDeliveryNo");
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;
            DocumentDs dataSet = new DocumentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (DocumentDs.DocumentRow row in dataSet.Document)
            {
                DocumentDef def = new DocumentDef();
                DocumentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        private int getMaxActionHistoryId()
        {
            IDataSetAdapter ad = getDataSetAdapter("ActionHistoryApt", "GetMaxActionHistoryId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }


        private int createNewActionHistoryRecord()
        {
            try
            {
                IDataSetAdapter ad = getDataSetAdapter("ActionHistoryApt", "GetActionHistoryByKey");
                ad.PopulateCommands();

                ActionHistoryDs dataSet = new ActionHistoryDs();
                ActionHistoryDs.ActionHistoryRow row = null;
                row = dataSet.ActionHistory.NewActionHistoryRow();

                row.ActionHistoryId = getMaxActionHistoryId() + 1;
                row.ShipmentId = -1;
                row.SplitShipmentId = -1;
                row.ActionHistoryTypeId = -1;
                row.ActionDate = DateTime.Today;
                row.Status = -1;
                dataSet.ActionHistory.AddActionHistoryRow(row);
                return (ad.Update(dataSet) == 1 ? row.ActionHistoryId : -1);
            }
            catch //(Exception e)
            {
                return -1;
            }
        }


        private int getMaxDocumentId()
        {
            IDataSetAdapter ad = getDataSetAdapter("DocumentApt", "GetMaxDocumentId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        private ActionHistoryDef getNewActionHistoryDef(int shipmentId, int splitShipmentId, ActionHistoryType actionType, AmendmentType amendType, string sourceValue, string targetValue, int userId)
        {
            string s = amendType.Description + " : " + sourceValue + " -> " + targetValue;
            return new ActionHistoryDef(shipmentId, splitShipmentId, actionType, amendType, s, userId);
        }

        private ActionHistoryDef getNewActionHistoryDef(int shipmentId, int splitShipmentId, ActionHistoryType actionType, string amendmentDesc, string sourceValue, string targetValue, int userId)
        {
            string s = amendmentDesc + " : " + sourceValue + " -> " + targetValue;
            return new ActionHistoryDef(shipmentId, splitShipmentId, actionType, null, s, userId);
        }

        public ArrayList getILSManifestDetailShipment(string containerNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSManifestDetailShipmentApt", "GetILSManifestDetailShipmentList");
            if (containerNo == null || containerNo == String.Empty)
                ad.SelectCommand.Parameters["@ContainerNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ContainerNo"].Value = containerNo;

            ILSManifestDetailShipmentDs dataSet = new ILSManifestDetailShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();
            foreach (ILSManifestDetailShipmentDs.ILSManifestDetailShipmentRow row in dataSet.ILSManifestDetailShipment)
            {
                ILSManifestDetailShipmentDef def = new ILSManifestDetailShipmentDef();
                ILSManifestDetailShipmentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getShipmentDetailUpdateLogList(int shipmentId, int splitShipmentId)
        {
            ArrayList list = new ArrayList();

            if (splitShipmentId == 0)
            {
                ArrayList shipmentDetails = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(shipmentId);
                foreach (ShipmentDetailDef shipmentDetail in shipmentDetails)
                {
                    ShipmentDetailUpdateLogDef def = new ShipmentDetailUpdateLogDef();
                    def.SizeOptionId = shipmentDetail.SizeOption.SizeOptionId;
                    def.SellingPrice = shipmentDetail.SellingPrice;
                    def.NetFOBPrice = shipmentDetail.ReducedNetFOBPrice;
                    def.SupplierGarmentPrice = shipmentDetail.ReducedSupplierGmtPrice;
                    def.ShippedQuantity = shipmentDetail.ShippedQuantity;
                    def.IsRevised = false;
                    list.Add(def);
                }
            }
            else
            {
                ArrayList splitShipmentDetails = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentDetailBySplitShipmentId(splitShipmentId);
                foreach (SplitShipmentDetailDef splitShipmentDetail in splitShipmentDetails)
                {
                    ShipmentDetailUpdateLogDef def = new ShipmentDetailUpdateLogDef();
                    def.SizeOptionId = splitShipmentDetail.SizeOption.SizeOptionId;
                    def.SellingPrice = splitShipmentDetail.SellingPrice;
                    def.NetFOBPrice = splitShipmentDetail.ReducedNetFOBPrice;
                    def.SupplierGarmentPrice = splitShipmentDetail.ReducedSupplierGmtPrice;
                    def.ShippedQuantity = splitShipmentDetail.ShippedQuantity;
                    def.IsRevised = false;
                    list.Add(def);
                }
            }
            return list;
        }

        public decimal getFutureOrderAmtByVendorId(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetFutureOrderInfoByVendor");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            decimal futureorder = 0;
            if (recordsAffected > 0)
            {
                futureorder = (decimal)dataSet.Tables[0].Rows[0][0];
            }
            return futureorder;
        }

        public int getFutureOrderCountByVendorId(int vendorId, int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetFutureOrderCountByVendor");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            int futureOrderCount = 0;
            if (recordsAffected > 0)
            {
                futureOrderCount = (int)dataSet.Tables[0].Rows[0][0];
            }
            return futureOrderCount;
        }

        public string getVendorNSLDocumentCount(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetVendorNSLDocumentCount");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            string s = string.Empty;
            if (recordsAffected > 0)
            {
                s = (string)dataSet.Tables[0].Rows[0][0];
            }
            return s;
        }

        public string getOSAdvancePaymentInstalmentAmt(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetOSAdvancePaymentInstalmentAmt");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            string s = string.Empty;
            if (recordsAffected > 0)
            {
                s = (string)dataSet.Tables[0].Rows[0][0];
            }
            return s;
        }

        public string getOSNextClaimAmtByVendorId(int vendorId, int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetOSNextClaimAmtByVendorId");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            string s = string.Empty;
            if (recordsAffected > 0)
            {
                s = (string)dataSet.Tables[0].Rows[0][0];
            }
            return s;
        }

        public decimal getOutstandingPaymentAmtByVendorId(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractShipmentApt", "GetOutstandingPaymentAmtByVendorId");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            decimal APOS = 0;
            if (recordsAffected > 0)
            {
                APOS = (decimal)dataSet.Tables[0].Rows[0][0];
            }
            return APOS;
        }


        #region Mapping Functions

        private void ContractShipmentMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ContractShipmentDs.ContractShipmentRow) &&
                target.GetType() == typeof(ContractShipmentListJDef))
            {
                ContractShipmentDs.ContractShipmentRow row = (ContractShipmentDs.ContractShipmentRow)source;
                ContractShipmentListJDef def = (ContractShipmentListJDef)target;

                def.ShipmentId = row.ShipmentId;
                if (!row.IsInvoicePrefixNull())
                    def.InvoicePrefix = row.InvoicePrefix;
                else
                    def.InvoicePrefix = string.Empty;
                if (!row.IsInvoiceSeqNull())
                    def.InvoiceSeq = row.InvoiceSeq;
                else
                    def.InvoiceSeq = 0;
                if (!row.IsInvoiceYearNull())
                    def.InvoiceYear = row.InvoiceYear;
                else
                    def.InvoiceYear = 0;
                if (!row.IsSequenceNoNull())
                {
                    def.SequenceNo = row.SequenceNo;
                }
                else
                    def.SequenceNo = int.MinValue;

                def.ContractNo = row.ContractNo;
                def.DeliveryNo = row.DeliveryNo;
                def.Vendor = vendorWorker.getVendorByKey(row.VendorId);
                def.ProductId = row.ProductId;
                def.ItemNo = row.ItemNo;
                def.CustomerAgreedAtWarehouseDate = row.CustomerAtWarehouseDate;
                def.SellCurrency = generalWorker.getCurrencyByKey(row.SellCurrencyId);
                def.BuyCurrency = generalWorker.getCurrencyByKey(row.BuyCurrencyId);
                def.TotalPOQuantity = row.TotalPOQty;
                def.TotalShippedQuantity = row.TotalShippedQty;
                def.TotalShippedAmount = row.TotalShippedAmt;

                def.TotalOrderQuantity = row.TotalOrderQty;
                def.TotalOrderAmount = row.TotalOrderAmt;
                def.InvoiceNo = getInvoiceNo(def.InvoicePrefix, def.InvoiceSeq, def.InvoiceYear);
                def.CustomerDestination = commonWorker.getCustomerDestinationByKey(row.CustomerDestinationId);
                def.Customer = commonWorker.getCustomerByKey(row.CustomerId);
                def.InvoiceAmount = row.TotalShippedAmt;
                if (!row.IsInvoiceUploadUserIdNull())
                    def.ShippingUser = generalWorker.getUserByKey(row.InvoiceUploadUserId);
                def.WorkflowStatus = ContractWFS.getType(row.WorkflowStatusId);
                def.IsNextMfgOrder = row.IsNextMfgOrder;
                def.IsDualSourcingOrder = row.IsDualSourcingOrder;
                def.IsUKDiscount = row.IsUKDiscount;
                def.WithOPRFabric = row.WithOPRFabric;

                if (!row.IsInvoiceDateNull())
                    def.InvoiceDate = row.InvoiceDate;
                else
                    def.InvoiceDate = DateTime.MinValue;
                if (ILSUploadWorker.Instance.getILSManifestDetailListByShipmentId(row.ShipmentId).Count > 0)
                    def.isConfirmedToShip = true;
                else
                    def.isConfirmedToShip = false;
                def.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
                def.EditLock = row.EditLock;
                def.PaymentLock = row.PaymentLock;
                def.IsMockShopSample = row.IsMockShopSample;
                def.IsPressSample = row.IsPressSample;
                def.IsStudioSample = row.IsStudioSample;

                def.TradingAgency = commonWorker.getTradingAgencyByKey(row.TradingAgencyId);
                def.ProductTeam = generalWorker.getProductCodeRefByKey(row.ProductTeamId);
                def.Season = generalWorker.getSeasonByKey(row.SeasonId);
                if (!row.IsSupplierInvoiceNoNull())
                    def.SupplierInvoiceNo = row.SupplierInvoiceNo;
                else
                    def.SupplierInvoiceNo = string.Empty;

                def.TotalShippedSupplierGarmentAmount = row.TotalShippedSupplierGmtAmt;
                def.TotalShippedSupplierGarmentAmountAfterDiscount = row.TotalShippedSupplierGmtAmtAfterDiscount;
                def.TotalShippedNetFOBAmtAfterDiscount = row.TotalShippedNetFOBAmtAfterDiscount;
                def.NSLCommissionPercent = row.NSLCommissionPercent;
                def.QACommissionPercent = row.QACommissionPercent;
                def.LabTestIncome = row.LabTestIncome;
                def.VendorPaymentDiscountPercent = row.VendorPaymentDiscountPercent;
                if (row.IsQACommissionAmtNull())
                    def.QACommissionAmount = 0;
                else
                    def.QACommissionAmount = row.QACommissionAmt;
                if (row.IsVendorPaymentDiscountAmtNull())
                    def.VendorPaymentDiscountAmount = 0;
                else
                    def.VendorPaymentDiscountAmount = row.VendorPaymentDiscountAmt;
                if (row.IsLabTestIncomeAmtNull())
                    def.LabTestIncomeAmount = 0;
                else
                    def.LabTestIncomeAmount = row.LabTestIncomeAmt;

                if (!row.IsPurchaseScanDateNull())
                    def.PurchaseScanDate = row.PurchaseScanDate;
                else
                    def.PurchaseScanDate = DateTime.MinValue;

                if (!row.IsPurchaseScanByNull())
                    def.PurchaseScanBy = generalWorker.getUserByKey(row.PurchaseScanBy);
                else
                    def.PurchaseScanBy = null;

                if (!row.IsPaymentTermIdNull())
                    def.PaymentTerm = generalWorker.getPaymentTermByKey(row.PaymentTermId);

                def.ARAmount = row.ARAmt;

                if (!row.IsARDateNull())
                    def.ARDate = row.ARDate;
                else
                    def.ARDate = DateTime.MinValue;

                if (!row.IsARRefNoNull())
                    def.ARRefNo = row.ARRefNo;
                else
                    def.ARRefNo = string.Empty;

                def.APAmount = row.APAmt;

                if (!row.IsAPDateNull())
                    def.APDate = row.APDate;
                else
                    def.APDate = DateTime.MinValue;

                if (!row.IsAPRefNoNull())
                    def.APRefNo = row.APRefNo;
                else
                    def.APRefNo = string.Empty;

                if (!row.IsSalesScanDateNull())
                    def.SalesScanDate = row.SalesScanDate;
                else
                    def.SalesScanDate = DateTime.MinValue;

                if (!row.IsEInvoiceBatchIdNull())
                    def.EInoviceBatchId = row.EInvoiceBatchId;
                else
                    def.EInoviceBatchId = 0;

                def.IsSelfBilledOrder = row.IsSelfBilledOrder;

                def.SplitCount = row.SplitCount;

                def.IsLDPOrder = row.IsLDPOrder;
                def.WithQCCharge = row.WithQCCharge;

                if (row.IsNSLCommissionSettlementDateNull())
                    def.NSLCommissionSettlementDate = DateTime.MinValue;
                else
                    def.NSLCommissionSettlementDate = row.NSLCommissionSettlementDate;
                def.NSLCommissionSettlementAmount = row.NSLCommissionSettlementAmt;
                if (row.IsNSLCommissionRefNoNull())
                    def.NSLCommissionSettlementRefNo = string.Empty;
                else
                    def.NSLCommissionSettlementRefNo = row.NSLCommissionRefNo;

                def.NSLCommissionAmount = row.NSLCommissionAmt;
                def.RejectPaymentReasonId = (row.IsRejectPaymentReasonIdNull() ? 0 : row.RejectPaymentReasonId);
                def.ShippingDocWFS = ShippingDocWFS.getType(row.DMSWorkflowStatusId);

                //if (row.IsPiecesPerDeliveryUnitNull())
                //    def.PiecesPerDeliveryUnit = 0;
                //else
                //    def.PiecesPerDeliveryUnit = row.PiecesPerDeliveryUnit;
                if (row.IsShippingDocReceiptDateNull())
                    def.ShippingDocReceiptDate = DateTime.MinValue;
                else
                    def.ShippingDocReceiptDate = row.ShippingDocReceiptDate;

                def.IsLCPaymentChecked = (row.IsLCPaymentChecked == 1);
                def.IsUploadDMSDocument = row.IsUploadDMSDocument;
                def.ShipmentMethod = commonWorker.getShipmentMethodByKey(row.ShipmentMethodId);
                def.SpecialOrderTypeId = row.SpecialOrderTypeId;
                def.IsChinaGBTestRequired = row.IsChinaGBTestRequired;
                def.TermOfPurchaseId = row.TermOfPurchaseId;
            }
        }

        private void InvoiceMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(InvoiceDs.InvoiceRow) &&
                target.GetType() == typeof(InvoiceDef))
            {
                InvoiceDs.InvoiceRow row = (InvoiceDs.InvoiceRow)source;
                InvoiceDef def = (InvoiceDef)target;

                def.ShipmentId = row.ShipmentId;
                if (!row.IsInvoicePrefixNull())
                    def.InvoicePrefix = row.InvoicePrefix;
                else
                    def.InvoicePrefix = String.Empty;
                if (!row.IsInvoiceSeqNull())
                    def.InvoiceSeqNo = row.InvoiceSeq;
                else
                    def.InvoiceSeqNo = 0;
                if (!row.IsInvoiceYearNull())
                    def.InvoiceYear = row.InvoiceYear;
                else
                    def.InvoiceYear = 0;
                if (!row.IsSequenceNoNull())
                    def.SequenceNo = row.SequenceNo;
                else
                    def.SequenceNo = int.MinValue;
                if (!row.IsInvoiceDateNull())
                    def.InvoiceDate = row.InvoiceDate;
                else
                    def.InvoiceDate = DateTime.MinValue;
                if (!row.IsInvoiceUploadDateNull())
                    def.InvoiceUploadDate = row.InvoiceUploadDate;
                else
                    def.InvoiceUploadDate = DateTime.MinValue;
                if (!row.IsInvoiceUploadUserIdNull())
                    def.InvoiceUploadUser = generalWorker.getUserByKey(row.InvoiceUploadUserId);
                if (!row.IsInvoicePrintDateNull())
                    def.InvoicePrintDate = row.InvoicePrintDate;
                else
                    def.InvoicePrintDate = DateTime.MinValue;
                if (!row.IsInvoicePrintUserIdNull())
                    def.InvoicePrintUser = generalWorker.getUserByKey(row.InvoicePrintUserId);

                def.IsReadyToSendInvoice = row.IsReadyToSendInvoice;
                if (!row.IsInvoiceSentDateNull())
                    def.InvoiceSentDate = row.InvoiceSentDate;
                else
                    def.InvoiceSentDate = DateTime.MinValue;

                if (!row.IsSupplierInvoiceNoNull())
                    def.SupplierInvoiceNo = row.SupplierInvoiceNo.ToUpper();
                else
                    def.SupplierInvoiceNo = String.Empty;
                if (!row.IsShippingRemarkNull())
                    def.ShippingRemark = row.ShippingRemark;
                else
                    def.ShippingRemark = String.Empty;
                def.NSLCommissionAmt = row.NSLCommissionAmt;
                def.InvoiceSellExchangeRate = row.InvoiceSellExchangeRate;
                def.InvoiceBuyExchangeRate = row.InvoiceBuyExchangeRate;
                if (!row.IsARDateNull())
                    def.ARDate = row.ARDate;
                else
                    def.ARDate = DateTime.MinValue;
                def.ARExchangeRate = row.ARExchangeRate;
                def.ARAmt = row.ARAmt;
                if (!row.IsARRefNoNull())
                    def.ARRefNo = row.ARRefNo;
                else
                    def.ARRefNo = String.Empty;
                if (!row.IsAPDateNull())
                    def.APDate = row.APDate;
                else
                    def.APDate = DateTime.MinValue;
                def.APExchangeRate = row.APExchangeRate;
                def.APAmt = row.APAmt;
                if (!row.IsAPRefNoNull())
                    def.APRefNo = row.APRefNo;
                else
                    def.APRefNo = String.Empty;
                def.LCAmount = row.LCAmt;
                if (!row.IsLCNoNull())
                    def.LCNo = row.LCNo;
                else
                    def.LCNo = String.Empty;
                if (!row.IsLCBillRefNoNull())
                    def.LCBillRefNo = row.LCBillRefNo;
                else
                    def.LCBillRefNo = String.Empty;
                if (!row.IsLCIssueDateNull())
                    def.LCIssueDate = row.LCIssueDate;
                else
                    def.LCIssueDate = DateTime.MinValue;
                if (!row.IsLCExpiryDateNull())
                    def.LCExpiryDate = row.LCExpiryDate;
                else
                    def.LCExpiryDate = DateTime.MinValue;
                def.IsLCPaymentChecked = row.IsLCPaymentChecked;
                if (!row.IsLCPaymentCheckedDateNull())
                    def.LCPaymentCheckedDate = row.LCPaymentCheckedDate;
                else
                    def.LCPaymentCheckedDate = DateTime.MinValue;
                def.ExportLicenceFee = row.ExportLicenceFee;
                def.QuotaCharge = row.QuotaCharge;
                if (!row.IsItemDesc1Null())
                    def.ItemDesc1 = row.ItemDesc1;
                else
                    def.ItemDesc1 = String.Empty;
                if (!row.IsItemDesc2Null())
                    def.ItemDesc2 = row.ItemDesc2;
                else
                    def.ItemDesc2 = String.Empty;
                if (!row.IsItemDesc3Null())
                    def.ItemDesc3 = row.ItemDesc3;
                else
                    def.ItemDesc3 = String.Empty;
                if (!row.IsItemDesc4Null())
                    def.ItemDesc4 = row.ItemDesc4;
                else
                    def.ItemDesc4 = String.Empty;
                if (!row.IsItemDesc5Null())
                    def.ItemDesc5 = row.ItemDesc5;
                else
                    def.ItemDesc5 = String.Empty;
                if (!row.IsItemColourNull())
                    def.ItemColour = row.ItemColour;
                else
                    def.ItemColour = String.Empty;
                if (!row.IsShipFromCountryIdNull())
                    def.ShipFromCountry = commonWorker.getShipmentCountryByKey(row.ShipFromCountryId);
                if (!row.IsCustomerDestinationIdNull())
                    def.CustomerDestination = commonWorker.getCustomerDestinationByKey(row.CustomerDestinationId);
                def.PackingMethod = commonWorker.getPackingMethodByKey(row.PackingMethodId);
                def.PiecesPerDeliveryUnit = row.PiecesPerDeliveryUnit;

                if (!row.IsShippingDocReceiptDateNull())
                    def.ShippingDocReceiptDate = row.ShippingDocReceiptDate;
                else
                    def.ShippingDocReceiptDate = DateTime.MinValue;
                if (!row.IsAccountDocReceiptDateNull())
                    def.AccountDocReceiptDate = row.AccountDocReceiptDate;
                else
                    def.AccountDocReceiptDate = DateTime.MinValue;
                def.IsSelfBilledOrder = row.IsSelfBilledOrder;
                def.CourierChargeToNUK = row.CourierChargeToNUK;
                if (!row.IsCourierChargeToNUKDebitNoteNoNull())
                    def.CourierChargeToNUKDebitNoteNo = row.CourierChargeToNUKDebitNoteNo;
                else
                    def.CourierChargeToNUKDebitNoteNo = String.Empty;
                if (!row.IsQccRemarkNull())
                    def.QCCRemark = row.QccRemark;
                else
                    def.QCCRemark = String.Empty;
                def.IsSyncToFactory = row.IsSyncToFactory;
                if (!row.IsBookingSONoNull())
                    def.BookingSONo = row.BookingSONo;
                else
                    def.BookingSONo = String.Empty;
                if (!row.IsBookingDateNull())
                    def.BookingDate = row.BookingDate;
                else
                    def.BookingDate = DateTime.MinValue;
                def.BookingQty = row.BookingQty;
                if (!row.IsBookingAtWarehouseDateNull())
                    def.BookingAtWarehouseDate = row.BookingAtWarehouseDate;
                else
                    def.BookingAtWarehouseDate = DateTime.MinValue;
                if (!row.IsActualAtWarehouseDateNull())
                    def.ActualAtWarehouseDate = row.ActualAtWarehouseDate;
                else
                    def.ActualAtWarehouseDate = DateTime.MinValue;
                def.ImportDutyActualAmt = row.ImportDutyActualAmt;
                def.ImportDutyCalculatedAmt = row.ImportDutyCalculatedAmt;
                def.IsImportDutyChecked = row.IsImportDutyChecked;
                if (!row.IsImportDutyCurrencyIdNull())
                    def.ImportDutyCurrency = generalWorker.getCurrencyByKey(row.ImportDutyCurrencyId);
                if (!row.IsImportDutyCheckedDateNull())
                    def.ImportDutyCheckedDate = row.ImportDutyCheckedDate;
                else
                    def.ImportDutyCheckedDate = DateTime.MinValue;

                def.InputVATActualAmt = row.InputVATActualAmt;
                def.InputVATCalculatedAmt = row.InputVATCalculatedAmt;
                def.IsInputVATChecked = row.IsInputVATChecked;
                if (!row.IsInputVATCurrencyIdNull())
                    def.InputVATCurrency = generalWorker.getCurrencyByKey(row.InputVATCurrencyId);
                if (!row.IsInputVATCheckedDateNull())
                    def.InputVATCheckedDate = row.InputVATCheckedDate;
                else
                    def.InputVATCheckedDate = DateTime.MinValue;

                def.OutputVATActualAmt = row.OutputVATActualAmt;
                def.OutputVATCalculatedAmt = row.OutputVATCalculatedAmt;
                def.IsOutputVATChecked = row.IsOutputVATChecked;
                if (!row.IsOutputVATCurrencyIdNull())
                    def.OutputVATCurrency = generalWorker.getCurrencyByKey(row.OutputVATCurrencyId);
                if (!row.IsOutputVATCheckedDateNull())
                    def.OutputVATCheckedDate = row.OutputVATCheckedDate;
                else
                    def.OutputVATCheckedDate = DateTime.MinValue;
                if (!row.IsInvoiceRemarkNull())
                    def.InvoiceRemark = row.InvoiceRemark;
                else
                    def.InvoiceRemark = String.Empty;
                if (!row.IsDFDebitNoteNoNull())
                    def.DirectFranchiseDebitNoteNo = row.DFDebitNoteNo;
                else
                    def.DirectFranchiseDebitNoteNo = String.Empty;
                def.DirectFranchiseDocumentCharge = row.DFDocumentationCharge;
                def.DirectFranchiseTransportationCharge = row.DFTransportationCharge;
                if (!row.IsSalesScanDateNull())
                    def.SalesScanDate = row.SalesScanDate;
                def.SalesScanAmount = row.SalesScanAmt;
                if (!row.IsPurchaseScanDateNull())
                    def.PurchaseScanDate = row.PurchaseScanDate;
                if (!row.IsPurchaseScanByNull())
                    def.PurchaseScanBy = generalWorker.getUserByKey(row.PurchaseScanBy);
                def.PurchaseScanAmount = row.PurchaseScanAmt;
                if (!row.IsEInvoiceBatchIdNull())
                    def.EInvoiceBatchId = row.EInvoiceBatchId;
                def.IsILSQtyUploadAllowed = row.IsILSQtyUploadAllowed;
                if (!row.IsILSActualAtWarehouseDateNull())
                    def.ILSActualAtWarehouseDate = row.ILSActualAtWarehouseDate;
                else
                    def.ILSActualAtWarehouseDate = DateTime.MinValue;
                if (!row.IsNSLCommissionSettlementDateNull())
                    def.NSLCommissionSettlementDate = row.NSLCommissionSettlementDate;
                else
                    def.NSLCommissionSettlementDate = DateTime.MinValue;
                def.NSLCommissionSettlementExchangeRate = row.NSLCommissionSettlementExchangeRate;
                def.NSLCommissionSettlementAmt = row.NSLCommissionSettlementAmt;
                if (!row.IsNSLCommissionRefNoNull())
                    def.NSLCommissionRefNo = row.NSLCommissionRefNo;
                else
                    def.NSLCommissionRefNo = String.Empty;
                def.ChoiceOrderTotalShippedAmount = row.ChoiceOrderTotalShippedAmt;
                def.ChoiceOrderTotalShippedSupplierGarmentAmount = row.ChoiceOrderTotalShippedSupplierGmtAmt;
                def.ChoiceOrderNSLCommissionAmount = row.ChoiceOrderNSLCommissionAmt;
                def.IsUploadDMSDocument = row.IsUploadDMSDocument;
                if (!row.IsLastSendDMSDocumentDateNull())
                    def.LastSendDMSDocumentDate = row.LastSendDMSDocumentDate;
                else
                    def.LastSendDMSDocumentDate = DateTime.MinValue;
                if (!row.IsShippingDocCheckedByNull())
                    def.ShippingDocCheckedBy = generalWorker.getUserByKey(row.ShippingDocCheckedBy);
                def.ShippingCheckedTotalNetAmount = row.ShippingCheckedTotalNetAmount;
                if (!row.IsShippingDocCheckedOnNull())
                    def.ShippingDocCheckedOn = row.ShippingDocCheckedOn;
                else
                    def.ShippingDocCheckedOn = DateTime.MinValue;
            }
            else if (source.GetType() == typeof(InvoiceDef) &&
                target.GetType() == typeof(InvoiceDs.InvoiceRow))
            {
                InvoiceDef def = (InvoiceDef)source;
                InvoiceDs.InvoiceRow row = (InvoiceDs.InvoiceRow)target;

                row.ShipmentId = def.ShipmentId;
                if (def.InvoicePrefix != String.Empty)
                    row.InvoicePrefix = def.InvoicePrefix;
                else
                    row.SetInvoicePrefixNull();
                if (def.InvoiceSeqNo != 0)
                    row.InvoiceSeq = def.InvoiceSeqNo;
                else
                    row.SetInvoiceSeqNull();
                if (def.InvoiceYear != 0)
                    row.InvoiceYear = def.InvoiceYear;
                else
                    row.SetInvoiceYearNull();
                if (def.SequenceNo != int.MinValue)
                    row.SequenceNo = def.SequenceNo;
                else
                    row.SetSequenceNoNull();
                if (def.InvoiceDate != DateTime.MinValue)
                    row.InvoiceDate = def.InvoiceDate;
                else
                    row.SetInvoiceDateNull();
                if (def.InvoiceUploadDate != DateTime.MinValue)
                    row.InvoiceUploadDate = def.InvoiceUploadDate;
                else
                    row.SetInvoiceUploadDateNull();
                if (def.InvoiceUploadUser != null)
                    row.InvoiceUploadUserId = def.InvoiceUploadUser.UserId;
                else
                    row.SetInvoiceUploadUserIdNull();
                if (def.InvoicePrintDate != DateTime.MinValue)
                    row.InvoicePrintDate = def.InvoicePrintDate;
                else
                    row.SetInvoicePrintDateNull();
                if (def.InvoicePrintUser != null)
                    row.InvoicePrintUserId = def.InvoicePrintUser.UserId;
                else
                    row.SetInvoicePrintUserIdNull();
                if (def.InvoiceSentDate != DateTime.MinValue)
                    row.InvoiceSentDate = def.InvoiceSentDate;
                else
                    row.SetInvoiceSentDateNull();
                row.IsReadyToSendInvoice = def.IsReadyToSendInvoice;
                if (def.SupplierInvoiceNo != String.Empty)
                    row.SupplierInvoiceNo = def.SupplierInvoiceNo.ToUpper();
                else
                    row.SetSupplierInvoiceNoNull();
                if (def.ShippingRemark != String.Empty)
                    row.ShippingRemark = def.ShippingRemark;
                else
                    row.SetShippingRemarkNull();
                row.NSLCommissionAmt = def.NSLCommissionAmt;
                row.InvoiceSellExchangeRate = def.InvoiceSellExchangeRate;
                row.InvoiceBuyExchangeRate = def.InvoiceBuyExchangeRate;
                if (def.ARDate != DateTime.MinValue)
                    row.ARDate = def.ARDate;
                else
                    row.SetARDateNull();
                row.ARExchangeRate = def.ARExchangeRate;
                row.ARAmt = def.ARAmt;
                if (def.ARRefNo != String.Empty)
                    row.ARRefNo = def.ARRefNo;
                else
                    row.SetARRefNoNull();
                if (def.APDate != DateTime.MinValue)
                    row.APDate = def.APDate;
                else
                    row.SetAPDateNull();
                row.APExchangeRate = def.APExchangeRate;
                row.APAmt = def.APAmt;
                if (def.APRefNo != String.Empty)
                    row.APRefNo = def.APRefNo;
                else
                    row.SetAPRefNoNull();
                row.LCAmt = def.LCAmount;
                if (def.LCNo.Trim() != String.Empty)
                    row.LCNo = def.LCNo;
                else
                    row.SetLCNoNull();
                if (def.LCBillRefNo != String.Empty)
                    row.LCBillRefNo = def.LCBillRefNo;
                else
                    row.SetLCBillRefNoNull();
                if (def.LCIssueDate != DateTime.MinValue)
                    row.LCIssueDate = def.LCIssueDate;
                else
                    row.SetLCIssueDateNull();
                if (def.LCExpiryDate != DateTime.MinValue)
                    row.LCExpiryDate = def.LCExpiryDate;
                else
                    row.SetLCExpiryDateNull();
                row.IsLCPaymentChecked = def.IsLCPaymentChecked;
                if (def.LCPaymentCheckedDate != DateTime.MinValue)
                    row.LCPaymentCheckedDate = def.LCPaymentCheckedDate;
                else
                    row.SetLCPaymentCheckedDateNull();
                row.ExportLicenceFee = def.ExportLicenceFee;
                row.QuotaCharge = def.QuotaCharge;
                if (def.ItemDesc1 != String.Empty)
                    row.ItemDesc1 = def.ItemDesc1;
                else
                    row.SetItemDesc1Null();
                if (def.ItemDesc2 != String.Empty)
                    row.ItemDesc2 = def.ItemDesc2;
                else
                    row.SetItemDesc2Null();
                if (def.ItemDesc3 != String.Empty)
                    row.ItemDesc3 = def.ItemDesc3;
                else
                    row.SetItemDesc3Null();
                if (def.ItemDesc4 != String.Empty)
                    row.ItemDesc4 = def.ItemDesc4;
                else
                    row.SetItemDesc4Null();
                if (def.ItemDesc5 != String.Empty)
                    row.ItemDesc5 = def.ItemDesc5;
                else
                    row.SetItemDesc5Null();
                if (def.ItemColour != String.Empty)
                    row.ItemColour = def.ItemColour;
                else
                    row.SetItemColourNull();
                if (def.ShipFromCountry != null)
                    row.ShipFromCountryId = def.ShipFromCountry.ShipmentCountryId;
                else
                    row.SetShipFromCountryIdNull();
                if (def.CustomerDestination != null)
                    if (def.CustomerDestination.CustomerDestinationId == int.MinValue)
                        row.SetCustomerDestinationIdNull();
                    else
                        row.CustomerDestinationId = def.CustomerDestination.CustomerDestinationId;
                else
                    row.SetCustomerDestinationIdNull();
                row.PackingMethodId = def.PackingMethod.PackingMethodId;
                row.PiecesPerDeliveryUnit = def.PiecesPerDeliveryUnit;

                if (def.ShippingDocReceiptDate != DateTime.MinValue)
                    row.ShippingDocReceiptDate = def.ShippingDocReceiptDate;
                else
                    row.SetShippingDocReceiptDateNull();
                if (def.AccountDocReceiptDate != DateTime.MinValue)
                    row.AccountDocReceiptDate = def.AccountDocReceiptDate;
                else
                    row.SetAccountDocReceiptDateNull();
                row.IsSelfBilledOrder = def.IsSelfBilledOrder;
                row.CourierChargeToNUK = def.CourierChargeToNUK;
                if (def.CourierChargeToNUKDebitNoteNo != String.Empty)
                    row.CourierChargeToNUKDebitNoteNo = def.CourierChargeToNUKDebitNoteNo;
                else
                    row.SetCourierChargeToNUKDebitNoteNoNull();
                if (def.QCCRemark.Trim() != String.Empty)
                    row.QccRemark = def.QCCRemark;
                else
                    row.SetQccRemarkNull();
                row.IsSyncToFactory = def.IsSyncToFactory;
                if (def.BookingSONo != String.Empty)
                    row.BookingSONo = def.BookingSONo;
                else
                    row.SetBookingSONoNull();
                if (def.BookingDate != DateTime.MinValue)
                    row.BookingDate = def.BookingDate;
                else
                    row.SetBookingDateNull();
                row.BookingQty = def.BookingQty;
                if (def.BookingAtWarehouseDate != DateTime.MinValue)
                    row.BookingAtWarehouseDate = def.BookingAtWarehouseDate;
                else
                    row.SetBookingAtWarehouseDateNull();
                if (def.ActualAtWarehouseDate != DateTime.MinValue)
                    row.ActualAtWarehouseDate = def.ActualAtWarehouseDate;
                else
                    row.SetActualAtWarehouseDateNull();
                if (def.ImportDutyCurrency != null)
                    row.ImportDutyCurrencyId = def.ImportDutyCurrency.CurrencyId;
                else
                    row.SetImportDutyCurrencyIdNull();
                row.ImportDutyCalculatedAmt = def.ImportDutyCalculatedAmt;
                row.ImportDutyActualAmt = def.ImportDutyActualAmt;
                row.IsImportDutyChecked = def.IsImportDutyChecked;
                if (def.ImportDutyCheckedDate != DateTime.MinValue)
                    row.ImportDutyCheckedDate = def.ImportDutyCheckedDate;
                else
                    row.SetImportDutyCheckedDateNull();
                if (def.InputVATCurrency != null)
                    row.InputVATCurrencyId = def.InputVATCurrency.CurrencyId;
                else
                    row.SetInputVATCurrencyIdNull();
                row.InputVATCalculatedAmt = def.InputVATCalculatedAmt;
                row.InputVATActualAmt = def.InputVATActualAmt;
                row.IsInputVATChecked = def.IsInputVATChecked;
                if (def.InputVATCheckedDate != DateTime.MinValue)
                    row.InputVATCheckedDate = def.InputVATCheckedDate;
                else
                    row.SetInputVATCheckedDateNull();
                if (def.OutputVATCurrency != null)
                    row.OutputVATCurrencyId = def.OutputVATCurrency.CurrencyId;
                else
                    row.SetOutputVATCurrencyIdNull();
                row.OutputVATCalculatedAmt = def.OutputVATCalculatedAmt;
                row.OutputVATActualAmt = def.OutputVATActualAmt;
                row.IsOutputVATChecked = def.IsOutputVATChecked;
                if (def.OutputVATCheckedDate != DateTime.MinValue)
                    row.OutputVATCheckedDate = def.OutputVATCheckedDate;
                else
                    row.SetOutputVATCheckedDateNull();
                if (def.InvoiceRemark != String.Empty)
                    row.InvoiceRemark = def.InvoiceRemark;
                else
                    row.SetInvoiceRemarkNull();
                if (def.DirectFranchiseDebitNoteNo != String.Empty)
                    row.DFDebitNoteNo = def.DirectFranchiseDebitNoteNo;
                else
                    row.SetDFDebitNoteNoNull();
                row.DFDocumentationCharge = def.DirectFranchiseDocumentCharge;
                row.DFTransportationCharge = def.DirectFranchiseTransportationCharge;
                if (def.SalesScanDate != DateTime.MinValue)
                    row.SalesScanDate = def.SalesScanDate;
                else
                    row.SetSalesScanDateNull();
                row.SalesScanAmt = def.SalesScanAmount;
                if (def.PurchaseScanDate != DateTime.MinValue)
                    row.PurchaseScanDate = def.PurchaseScanDate;
                else
                    row.SetPurchaseScanDateNull();
                row.PurchaseScanAmt = def.PurchaseScanAmount;
                if (def.PurchaseScanBy != null)
                    row.PurchaseScanBy = def.PurchaseScanBy.UserId;
                else
                    row.SetPurchaseScanByNull();
                row.EInvoiceBatchId = def.EInvoiceBatchId;
                row.IsILSQtyUploadAllowed = def.IsILSQtyUploadAllowed;
                if (def.ILSActualAtWarehouseDate != DateTime.MinValue)
                    row.ILSActualAtWarehouseDate = def.ILSActualAtWarehouseDate;
                else
                    row.SetILSActualAtWarehouseDateNull();
                if (def.NSLCommissionSettlementDate != DateTime.MinValue)
                    row.NSLCommissionSettlementDate = def.NSLCommissionSettlementDate;
                else
                    row.SetNSLCommissionSettlementDateNull();
                row.NSLCommissionSettlementExchangeRate = def.NSLCommissionSettlementExchangeRate;
                row.NSLCommissionSettlementAmt = def.NSLCommissionSettlementAmt;
                if (def.NSLCommissionRefNo != String.Empty)
                    row.NSLCommissionRefNo = def.NSLCommissionRefNo;
                else
                    row.SetNSLCommissionRefNoNull();
                row.ChoiceOrderTotalShippedAmt = def.ChoiceOrderTotalShippedAmount;
                row.ChoiceOrderTotalShippedSupplierGmtAmt = def.ChoiceOrderTotalShippedSupplierGarmentAmount;
                row.ChoiceOrderNSLCommissionAmt = def.ChoiceOrderNSLCommissionAmount;
                row.IsUploadDMSDocument = def.IsUploadDMSDocument;
                if (def.LastSendDMSDocumentDate == DateTime.MinValue)
                    row.SetLastSendDMSDocumentDateNull();
                else
                    row.LastSendDMSDocumentDate = def.LastSendDMSDocumentDate;
                if (def.ShippingDocCheckedBy != null)
                    row.ShippingDocCheckedBy = def.ShippingDocCheckedBy.UserId;
                else
                    row.SetShippingDocCheckedByNull();
                if (def.ShippingDocCheckedOn == DateTime.MinValue)
                    row.SetShippingDocCheckedOnNull();
                else
                    row.ShippingDocCheckedOn = def.ShippingDocCheckedOn;
                row.ShippingCheckedTotalNetAmount = def.ShippingCheckedTotalNetAmount;
            }
        }

        private void DocumentMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(DocumentDs.DocumentRow) &&
                target.GetType() == typeof(DocumentDef))
            {
                DocumentDs.DocumentRow row = (DocumentDs.DocumentRow)source;
                DocumentDef def = (DocumentDef)target;

                def.DocId = row.DocId;
                def.ShipmentId = row.ShipmentId;
                def.DocumentType = DocumentType.getType(row.DocTypeId);
                def.DocumentNo = row.DocNo;
                if (!row.IsCountryIdNull())
                    def.Country = generalWorker.getCountryOfOriginByKey(row.CountryId);
                if (!row.IsIssueDateNull())
                    def.IssueDate = row.IssueDate;
                else
                    def.IssueDate = DateTime.MinValue;
                if (!row.IsExpiryDateNull())
                    def.ExpiryDate = row.ExpiryDate;
                else
                    def.ExpiryDate = DateTime.MinValue;
                if (!row.IsQuotaCategoryGroupIdNull())
                    def.QuotaCategory = commonWorker.getQuotaCategoryByKey(row.QuotaCategoryGroupId);
                def.Weight = row.Weight;
                def.Qty = row.Qty;
                if (!row.IsUnitIdNull())
                    def.Unit = commonWorker.getPackingUnitByKey(row.UnitId);
                def.OrderQty = row.OrderQty;
                if (!row.IsOrderUnitIdNull())
                    def.OrderUnit = commonWorker.getPackingUnitByKey(row.OrderUnitId);
                def.POQty = row.POQty;
                if (!row.IsPOUnitIdNull())
                    def.POUnit = commonWorker.getPackingUnitByKey(row.POUnitId);
                if (!row.IsDespatchToUKDateNull())
                    def.DespatchToUKDate = row.DespatchToUKDate;
                else
                    def.DespatchToUKDate = DateTime.MinValue;
                if (!row.IsDespatchAWBNoNull())
                    def.DespatchAWBNo = row.DespatchAWBNo;
                else
                    def.DespatchAWBNo = String.Empty;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(DocumentDef) &&
                target.GetType() == typeof(DocumentDs.DocumentRow))
            {
                DocumentDef def = (DocumentDef)source;
                DocumentDs.DocumentRow row = (DocumentDs.DocumentRow)target;

                row.DocId = def.DocId;
                row.ShipmentId = def.ShipmentId;
                row.DocNo = def.DocumentNo;
                row.DocTypeId = def.DocumentType.Id;
                if (def.Country != null)
                    row.CountryId = def.Country.CountryOfOriginId;
                else
                    row.SetCountryIdNull();
                if (def.IssueDate != DateTime.MinValue)
                    row.IssueDate = def.IssueDate;
                else
                    row.SetIssueDateNull();
                if (def.ExpiryDate != DateTime.MinValue)
                    row.ExpiryDate = def.ExpiryDate;
                else
                    row.SetExpiryDateNull();
                if (def.QuotaCategory != null)
                    row.QuotaCategoryGroupId = def.QuotaCategory.QuotaCategoryId;
                else
                    row.SetQuotaCategoryGroupIdNull();
                row.Weight = def.Weight;
                row.Qty = def.Qty;
                if (def.Unit != null)
                    row.UnitId = def.Unit.PackingUnitId;
                else
                    row.SetUnitIdNull();
                row.OrderQty = def.OrderQty;
                if (def.OrderUnit != null)
                    row.OrderUnitId = def.OrderUnit.PackingUnitId;
                else
                    row.SetOrderUnitIdNull();
                row.POQty = def.POQty;
                if (def.POUnit != null)
                    row.POUnitId = def.POUnit.PackingUnitId;
                else
                    row.SetPOUnitIdNull();
                if (def.DespatchToUKDate != DateTime.MinValue)
                    row.DespatchToUKDate = def.DespatchToUKDate;
                else
                    row.SetDespatchToUKDateNull();
                if (def.DespatchAWBNo != String.Empty)
                    row.DespatchAWBNo = def.DespatchAWBNo;
                else
                    row.SetDespatchAWBNoNull();
                row.Status = def.Status;
            }
        }

        private void ActionHistoryMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ActionHistoryDs.ActionHistoryRow) &&
                target.GetType() == typeof(ActionHistoryDef))
            {
                ActionHistoryDs.ActionHistoryRow row = (ActionHistoryDs.ActionHistoryRow)source;
                ActionHistoryDef def = (ActionHistoryDef)target;

                def.ActionHistoryId = row.ActionHistoryId;
                def.ShipmentId = row.ShipmentId;
                def.SplitShipmentId = row.SplitShipmentId;
                def.ActionHistoryType = ActionHistoryType.getType(row.ActionHistoryTypeId);
                if (!row.IsAmendmentTypeIdNull())
                    def.AmendmentType = AmendmentType.getType(row.AmendmentTypeId);
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = String.Empty;
                def.ActionDate = row.ActionDate;
                def.Status = row.Status;
                if (!row.IsUserIdNull())
                    def.User = generalWorker.getUserByKey(row.UserId);
            }
            else if (source.GetType() == typeof(ActionHistoryDef) &&
                target.GetType() == typeof(ActionHistoryDs.ActionHistoryRow))
            {
                ActionHistoryDef def = (ActionHistoryDef)source;
                ActionHistoryDs.ActionHistoryRow row = (ActionHistoryDs.ActionHistoryRow)target;

                row.ActionHistoryId = def.ActionHistoryId;
                row.ShipmentId = def.ShipmentId;
                row.SplitShipmentId = def.SplitShipmentId;
                row.ActionHistoryTypeId = def.ActionHistoryType.Id;
                if (def.AmendmentType != null)
                    row.AmendmentTypeId = def.AmendmentType.Id;
                else
                    row.SetAmendmentTypeIdNull();
                if (def.Remark != String.Empty)
                    if (def.Remark.Length > 200)
                        row.Remark = def.Remark.Substring(1, 197) + "...";
                    else
                        row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
                row.ActionDate = def.ActionDate;
                row.Status = def.Status;
                if (def.User == null)
                    row.SetUserIdNull();
                else
                    row.UserId = def.User.UserId;
            }
        }

        private void ILSManifestDetailShipmentMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSManifestDetailShipmentDs.ILSManifestDetailShipmentRow) &&
                target.GetType() == typeof(ILSManifestDetailShipmentDef))
            {
                ILSManifestDetailShipmentDs.ILSManifestDetailShipmentRow row = (ILSManifestDetailShipmentDs.ILSManifestDetailShipmentRow)source;
                ILSManifestDetailShipmentDef def = (ILSManifestDetailShipmentDef)target;

                def.VoyageNo = row.VoyageNo;
                def.VesselName = row.VesselName;
                def.ContainerNo = row.ContainerNo;
                def.ContractNo = row.ContractNo;
                def.DeliveryNo = row.DeliveryNo == null || row.DeliveryNo == "" ? int.MinValue : int.Parse(row.DeliveryNo);
                if (!row.IsInWarehouseDateNull())
                    def.InWarehouseDate = row.InWarehouseDate;
                def.TotalVolume = row.TotalVolume;

                if (!row.IsShipmentIdNull())
                {
                    if (!row.IsTotalCartonsNull())
                        def.TotalCartons = row.TotalCartons;
                    if (!row.IsContainerPositionNull())
                        def.ContainerPosition = row.ContainerPosition;

                    def.ShipmentId = row.ShipmentId;
                    if (!row.IsItemNoNull())
                        def.ItemNo = row.ItemNo;
                    if (!row.IsVendorIdNull())
                        def.Vendor = vendorWorker.getVendorByKey(row.VendorId);
                    if (!row.IsPackingMethodIdNull())
                        def.PackingMethod = commonWorker.getPackingMethodByKey(row.PackingMethodId);
                    if (!row.IsTotalShippedQtyNull())
                        def.TotalShippedQty = row.TotalShippedQty;
                    if (!row.IsBookingSONoNull())
                        def.BookingSONo = row.BookingSONo;
                    if (!row.IsInvoicePrefixNull())
                        def.InvoicePrefix = row.InvoicePrefix;
                    if (!row.IsInvoiceSeqNull())
                        def.InvoiceSeq = row.InvoiceSeq;
                    if (!row.IsInvoiceYearNull())
                        def.InvoiceYear = row.InvoiceYear;
                    def.InvoiceNo = getInvoiceNo(def.InvoicePrefix, def.InvoiceSeq, def.InvoiceYear);
                    if (!row.IsInvoiceUploadUserIdNull())
                        def.InvoiceUploadUser = generalWorker.getUserByKey(row.InvoiceUploadUserId);
                    if (!row.IsCustomerIdNull())
                        def.CustomerId = row.CustomerId;
                    if (!row.IsCustomerDestinationIdNull())
                        def.CustomerDestinationId = row.CustomerDestinationId;
                    if (!row.IsDestinationCodeNull())
                        def.DestinationCode = row.DestinationCode;
                }
            }
        }

        private void ShipmentProductMapping(Object source, Object target)
        {

            if (source.GetType() == typeof(ShipmentProductDs.ShipmentProductRow) &&
                target.GetType() == typeof(ShipmentProductDef))
            {
                ShipmentProductDs.ShipmentProductRow row = (ShipmentProductDs.ShipmentProductRow)source;
                ShipmentProductDef def = (ShipmentProductDef)target;

                def.ProductId = row.ProductId;
                def.ItemNo = row.ItemNo;
                def.ItemDesc = (!row.IsItemDescNull() ? row.ItemDesc : string.Empty);
                def.Desc1 = (!row.IsDesc1Null() ? row.Desc1 : string.Empty);
                def.Desc2 = (!row.IsDesc2Null() ? row.Desc2 : string.Empty);
                def.Desc3 = (!row.IsDesc3Null() ? row.Desc3 : string.Empty);
                def.Desc4 = (!row.IsDesc4Null() ? row.Desc4 : string.Empty);
                def.Desc5 = (!row.IsDesc5Null() ? row.Desc5 : string.Empty);
                def.DeptId = (!row.IsDeptIdNull() ? row.DeptId : int.MinValue);
                def.DepartmentCode = (!row.IsDepartmentCodeNull() ? row.DepartmentCode : string.Empty);
                def.DepartmentName = (!row.IsDepartmentNameNull() ? row.DepartmentName : string.Empty);
                def.ProductTeamId = (!row.IsProductTeamIdNull() ? row.ProductTeamId : int.MinValue);
                def.ProductTeamCode = (!row.IsProductTeamCodeNull() ? row.ProductTeamCode : string.Empty);
                def.ProductTeamName = (!row.IsProductTeamNameNull() ? row.ProductTeamName : string.Empty);
                def.VendorId = (!row.IsVendorIdNull() ? row.VendorId : int.MinValue);
                def.VendorName = (!row.IsVendorNameNull() ? row.VendorName : string.Empty);
                def.OfficeId = (!row.IsOfficeIdNull() ? row.OfficeId : int.MinValue);
                def.OfficeCode = (!row.IsOfficeCodeNull() ? row.OfficeCode : string.Empty);
                def.OfficeName = (!row.IsOfficeNameNull() ? row.OfficeName : string.Empty);
                def.ContractId = (!row.IsContractIdNull() ? row.ContractId : int.MinValue);
                def.ShipmentId = (!row.IsShipmentIdNull() ? row.ShipmentId : int.MinValue);
                def.DeliveryDate = (!row.IsDeliveryDateNull() ? row.DeliveryDate : DateTime.MinValue);
                def.TermOfPurchaseId = row.TermOfPurchaseId;
                def.HandlingOfficeId = row.HandlingOfficeId;
            }


        }

        private int getMaxShipmentDeductionId()
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentDeductionApt", "GetMaxShipmentDeductionId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        internal void ShipmentDeductionMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ShipmentDeductionDs.ShipmentDeductionRow) &&
                target.GetType() == typeof(ShipmentDeductionDef))
            {
                ShipmentDeductionDs.ShipmentDeductionRow row = (ShipmentDeductionDs.ShipmentDeductionRow)source;
                ShipmentDeductionDef def = (ShipmentDeductionDef)target;

                def.ShipmentDeductionId = row.ShipmentDeductionId;
                def.ShipmentId = row.ShipmentId;
                def.DeductionType = PaymentDeductionType.getType(row.DeductionType);
                def.DocumentNo = row.DocNo;
                def.Amount = row.Amt;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(ShipmentDeductionDef) &&
                target.GetType() == typeof(ShipmentDeductionDs.ShipmentDeductionRow))
            {
                ShipmentDeductionDef def = (ShipmentDeductionDef)source;
                ShipmentDeductionDs.ShipmentDeductionRow row = (ShipmentDeductionDs.ShipmentDeductionRow)target;

                row.ShipmentDeductionId = def.ShipmentDeductionId;
                row.ShipmentId = def.ShipmentId;
                row.DeductionType = def.DeductionType.Id;
                row.DocNo = def.DocumentNo;
                row.Amt = def.Amount;
                if (def.Remark.Trim() != string.Empty)
                    row.Remark = def.Remark.Trim();
                else
                    row.SetRemarkNull();
                row.Status = def.Status;
            }
        }

        #endregion

        public bool isChoiceActualAmountAutoUpdate(int customerId, int officeId)
        {
            bool isAutoUpdate = false;
            if (customerId == CustomerDef.Id.choice.GetHashCode())
                isAutoUpdate = (officeId == OfficeId.SH.Id || officeId == OfficeId.DG.Id);
            return isAutoUpdate;
        }

        public bool insertShipmentAttribute(int shipmentId)
        {
            bool inserted = false;
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ShipmentAttributeApt", "GetShipmentAttributeByKey");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
                ad.PopulateCommands();

                ShipmentAttributeDs ds = new ShipmentAttributeDs();
                ShipmentAttributeDs.ShipmentAttributeRow row = null;
                int recordsAffected = ad.Fill(ds);
                if (recordsAffected == 0)
                {   // insert new 
                    row = ds.ShipmentAttribute.NewShipmentAttributeRow();
                    row.ShipmentId = shipmentId;
                    row.FCR = DateTime.Now;
                    row.CreatedOn = DateTime.Now;
                    ds.ShipmentAttribute.AddShipmentAttributeRow(row);
                    if (ad.Update(ds) > 0)
                        inserted = true;
                    else
                        throw new DataAccessException("Fail to insert ShipmentAttribute, ShipmentId : " + shipmentId.ToString());
                }
                ctx.VoteCommit();
                return inserted;
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
    }
}
