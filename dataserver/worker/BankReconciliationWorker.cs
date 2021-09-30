using System;
using System.IO;
using System.Data;
using System.Collections;
using com.next.infra.persistency.dataaccess;
using com.next.infra.util;
using com.next.common.datafactory.worker;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.dataserver.model.account;
using com.next.isam.domain.account;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.infra.persistency.transactions;

namespace com.next.isam.dataserver.worker
{
    public class BankReconciliationWorker : Worker
    {
        private static BankReconciliationWorker _instance;
        private CommonWorker _commonWorker;
        private ShippingWorker _shippingWorker;
        private OrderWorker _orderWorker;
        private OrderSelectWorker _orderSelectWorker;
        private eBankingWorker _eBankingWorker;

        private ArrayList _columnMapping;
        private Hashtable _bankRefList;
        private string _archivePath;

        public BankReconciliationWorker()
        {
            _commonWorker = CommonWorker.Instance;
            _shippingWorker = ShippingWorker.Instance;
            _orderWorker = OrderWorker.Instance;
            _bankRefList = this.getBankRefList("OA");
            _orderSelectWorker = OrderSelectWorker.Instance;
            _eBankingWorker = eBankingWorker.Instance;
            _archivePath = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_RECON_Folder") + @"\archive\";
        }

        public static BankReconciliationWorker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BankReconciliationWorker();
                }
                return _instance;
            }
        }

        public void processBankReconciliation()
        {
            ArrayList requestList = AccountWorker.Instance.getBankReconciliationRequestList(GeneralCriteria.ALL);

            foreach (BankReconciliationRequestDef request in requestList)
            {
                string sourcePath = request.FileName;
                string bankName = request.Bank;
                string dbName = request.SunDB;
                int userId = request.SubmitUser.UserId;

                string tmpFile = sourcePath + ".tmp";

                if (File.Exists(tmpFile))
                {
                    return;
                }
                else
                {
                    File.Create(tmpFile).Close();
                }

                StreamReader sourceFile = File.OpenText(sourcePath);

                _columnMapping = _eBankingWorker.getMappingColumnsByBank(request.Bank);

                string line = String.Empty;
                string fileName = _eBankingWorker.getFileNameByPath(sourcePath, false);

                if (sourceFile.ReadLine() == null) return;

                ArrayList updatedList = new ArrayList();
                ArrayList errorList = new ArrayList();
                ArrayList chargeList = new ArrayList();
                ArrayList lineList = new ArrayList();

                while ((line = sourceFile.ReadLine()) != null)
                {
                    lineList.Add(line);
                }
                sourceFile.Close();
                File.Copy(sourcePath, _archivePath + fileName, true);
                File.Delete(sourcePath);

                Hashtable paymentList = new Hashtable();

                try
                {
                    // _bankRefHash = this.getBankRefList("OA");

                    for (int i = 0; i < lineList.Count; i++)
                    {
                        this.updateRecord(fileName, lineList[i].ToString(), dbName, bankName, updatedList, errorList, chargeList, paymentList, i + 1, request.SubmitUser.UserId);
                    }

                    // update the mapping for charge list and payment list
                    if (bankName != "SCB")
                        this.checkChargeList(chargeList, paymentList, errorList);

                    // mail the result
                    if (updatedList.Count > 0 || errorList.Count > 0)
                    {
                        // updatedList.Sort(1, updatedList.Count, null);
                        completeBankReconciliation(fileName, userId, bankName, updatedList, errorList, chargeList);
                    }

                    // return updatedList;
                    request.ProcessDate = DateTime.Now;
                    request.Status = RequestStatus.PROCESSED.StatusId;

                    AccountWorker.Instance.updateBankReconciliationRequest(request);                    

                    if (File.Exists(tmpFile))
                    {
                        File.Delete(tmpFile);
                    }
                }
                catch (Exception exc)
                {
                    if (File.Exists(tmpFile))
                    {
                        File.Delete(tmpFile);
                        //dataset.FileUploadQueue.RemoveFileUploadQueueRow(row);
                        request.Status = RequestStatus.REJECTED.StatusId;
                        AccountWorker.Instance.updateBankReconciliationRequest(request);
                        UserRef userRef = CommonUtil.getUserByKey(userId);
                        NoticeHelper.sendBankRecProcessErrorMessage(exc, userRef, fileName);
                    }
                    //NoticeHelper.sendErrorMessage(exc, 
                }
            }
        }

        private void updateRecord(string fileName, string sourceLine, string dbName, string bankName,
            ArrayList updatedList, ArrayList errorList, ArrayList chargeList, Hashtable paymentList, int lineNo, int userId)
        {
            if (bankName == "HSBC")
                this.updateHSBCRecord(fileName, sourceLine, dbName, bankName, updatedList, errorList, chargeList, paymentList, userId);
            else
            {
                this.updateSCBRecord(fileName, sourceLine, dbName, bankName, updatedList, errorList, chargeList, paymentList, lineNo, userId);
            }
        }

        private void updateSCBRecord(string fileName, string sourceLine, string dbName, string bankName,
            ArrayList updatedList, ArrayList errorList, ArrayList chargeList, Hashtable paymentList, int lineNo, int userId)
        {
            ArrayList columnList = getColumnsFromList(bankName, "RECON");

            string[] fields = sourceLine.Split(',');
            bool sunUpdate = false;
            bool nssUpdate = false;

            string ccy, accountNo, valueDate, amtStr, refNo, dcFlag;
            ccy = "";
            accountNo = "";
            valueDate = "";
            amtStr = "";
            refNo = "";
            dcFlag = "";

            accountNo = fields[1].ToString().ToUpper();
            ccy = fields[2].ToString().ToUpper();
            valueDate = fields[9].ToString();
            amtStr = fields[10].ToString();
            refNo = fields[16].ToString();
            dcFlag = fields[11].ToString();

            if (refNo.IndexOf("STS") == -1)
            {
                errorList.Add(NoticeHelper.getErrorHtml("Non-GP Payment! Process is skipped! ", lineNo));
                return;
            }
            refNo = refNo.Substring(1, refNo.IndexOf(" "));
            string[] refs = refNo.Split('-');

            if (refs.Length < 4)
            {
                errorList.Add(NoticeHelper.getErrorHtml("Non-GP Payment! Process is skipped! ", lineNo));
                return;
            }

            string sunAccountRefNo = refs[2].ToString().Trim();

            if (refs[3].ToString().Trim() != "GP")
            {
                errorList.Add(NoticeHelper.getErrorHtml("Non-GP Payment! Process is skipped! ", lineNo));
                return;
            }

            // if (sunAccountRefNo.Length != 10) return;

            try
            {
                // get the bank transaction
                SunSALFLDGTblDef bankDef = getSunAccountGPDebitDataDefByTransactionRef(sunAccountRefNo, dbName);

                decimal amount = Convert.ToDecimal(amtStr);

                bool error = false;
                if (Math.Abs(amount) - Math.Abs((decimal)bankDef.OTHER_AMT) != 0)
                {
                    errorList.Add(NoticeHelper.getErrorHtml("Bank amount is mismatched with SunDB!", lineNo));
                    error = true;
                }

                /*
                if (dcFlag != (string)bankdef.D_C.Trim())
                {
                    errorList.Add(this.getErrorHtml("DC Indicator is mismatched with SunDB!", lineNo));
                    error = true;
                }
                */

                if (ccy != (string)bankDef.CONV_CODE.Trim())
                {
                    errorList.Add(NoticeHelper.getErrorHtml("Currency is mismatched with SunDB!", lineNo));
                    error = true;
                }
                // any missmatch information will be filtered out
                if (error) return;

                // starting bank reconciliation
                // if all ok then update value date and bank's allocation
                int valDate = Convert.ToInt32((Convert.ToDateTime(valueDate)).ToString("yyyyMMdd"));

                IDataSetAdapter ad = getDataSetAdapter("SunSALFLDGApt", "GetBankTransactionCmd");
                string sql = "SELECT * FROM SALFLDG" + dbName + " WITH (NOLOCK) WHERE TREFERENCE = '" + sunAccountRefNo + "'";
                ad.SelectCommand.DbCommand.CommandText = sql;
                ad.PopulateCommands();
                SALFLDGDs ds = new SALFLDGDs();
                ad.SelectCommand.DbCommand.CommandTimeout = 300;
                int recordsAffected = ad.Fill(ds);

                foreach (SALFLDGDs.SALFLDGRow row in ds.Tables[0].Rows)
                {
                    row.TRANS_DATE = valDate;
                    if (row.ACCNT_CODE == bankDef.ACCNT_CODE)
                    {
                        row.ALLOCATION = "R";
                        row.ANAL_T9 = sunAccountRefNo.Trim();
                    }
                }

                int count = 0;

                while (count <= 3)
                {
                    try
                    {
                        ad.UpdateCommand.DbCommand.CommandTimeout = 60;
                        this.updateRecord(ad, ds);
                        break;
                    }
                    catch (Exception e)
                    {
                        if (count == 3)
                            NoticeHelper.sendErrorMessage(e, "Line is => " + sourceLine);
                    }
                    finally
                    {
                        count++;
                    }
                }
                sunUpdate = true;

                // add payment list 
                if (!paymentList.ContainsKey(sunAccountRefNo))
                    paymentList.Add(sunAccountRefNo, Convert.ToDecimal(amtStr));
                // end of update sun data				

                ArrayList list = getT9ByTransactionRef(sunAccountRefNo, dbName);

                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        string t9 = list[i].ToString().Trim();
                        float delAmt = this.updateAP(sourceLine, bankName, accountNo, t9, ccy, valueDate, errorList, lineNo, userId);
                        nssUpdate = true;
                        string delAmtStr = "";
                        if (delAmt == float.NegativeInfinity)
                        {
                            nssUpdate = false;
                            delAmtStr = "N/A";
                        }
                        else
                            delAmtStr = Math.Abs(delAmt).ToString();
                        updatedList.Add(this.getUpdateBankReconciliationSummaryDef(fileName, sunAccountRefNo, bankDef.JRNAL_NO.ToString(), ccy, delAmtStr, sunUpdate, nssUpdate, t9));
                    }
                }
                else
                {
                    errorList.Add(NoticeHelper.getErrorHtml("Missing T9 Code (Sales Invoice Number or Contract Number)!", lineNo));
                    updatedList.Add(this.getUpdateBankReconciliationSummaryDef(fileName, sunAccountRefNo, bankDef.JRNAL_NO.ToString(), ccy, amtStr, sunUpdate, nssUpdate, ""));
                }
            }            
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "Line is => " + sourceLine);
            }
        }

        private void updateHSBCRecord(string fileName, string sourceLine, string dbName, string bankName,
            ArrayList updatedList, ArrayList errorList, ArrayList chargeList, Hashtable paymentList, int userId)
        {
            ArrayList columnList = getColumnsFromList(bankName, "RECON");
            string[] fields = sourceLine.Split(',');

            bool sunUpdate = false;
            bool nssUpdate = false;

            // bank statement unique id
            int lineNo = Convert.ToInt32(fields[0].Replace("\"", ""));
            string ccyCode, accountNo, valueDate, fieldAmt, textLine1, textLine2, textLine4, bankCharge;

            ccyCode = "";
            accountNo = "";
            valueDate = "";
            fieldAmt = "";
            textLine1 = "";
            textLine2 = "";
            textLine4 = "";
            bankCharge = "0";

            // get the information first from statement file
            foreach (eBankingColumnMappingDef def in columnList)
            {
                int i = def.Seq;
                string field = fields[i - 1].Replace("\"", ""); // remove the quotation mark in the source file
                if (def.FieldName == "CCYCODE") ccyCode = field;
                if (def.FieldName == "ACCOUNTNO") accountNo = field;
                if (def.FieldName == "VAL_DATE") valueDate = field;
                if (def.FieldName == "FIELDAMOUN")
                {
                    fieldAmt = field;
                    bankCharge = def.DefaultValue;
                }
                if (def.FieldName == "TEXTLINE1") textLine1 = field;
                if (def.FieldName == "TEXTLINE2") textLine2 = field;
                if (def.FieldName == "TEXTLINE4") textLine4 = field;
            }

            // start the analyst of the statement line
            try
            {
                if (textLine1.ToUpper().IndexOf("-GP") == -1 || textLine1.ToUpper().IndexOf("-GP") != 10)
                {
                    errorList.Add(NoticeHelper.getErrorHtml("Non-GP payment! It is skipped from processing! " + "[" + textLine1 + "]", lineNo));
                    return;
                }
                else
                {
                    // key for data search
                    string payRefNo = textLine1.Substring(0, 10);

                    // get the bank transaction
                    SunSALFLDGTblDef bankDef = getSunAccountGPDebitDataDefByTransactionRef(payRefNo, dbName);

                    if (bankDef == null)
                    {
                        errorList.Add(NoticeHelper.getErrorHtml("No bank information in SunDB for the reconcilation process!", lineNo));

                        if (textLine4.IndexOf("CHARGES") != -1)
                        {
                            updatedList.Add(this.getUpdateBankReconciliationSummaryDef(fileName, payRefNo, "Missing", ccyCode, fieldAmt, sunUpdate, nssUpdate, ""));
                        }
                        return;
                    }

                    // step 1. charges row 
                    if (textLine4.IndexOf("CHARGES") != -1)
                    {
                        this.addChargeData(chargeList, valueDate, accountNo, fieldAmt, payRefNo, bankDef.JRNAL_NO.ToString(), bankDef.ANAL_T0, ccyCode);
                    }
                    else
                    {
                        decimal otherAmt = Convert.ToDecimal(fieldAmt);

                        // check bank transaction information
                        string dcFlag = "";
                        if (Convert.ToDecimal(fieldAmt) > 0)
                            dcFlag = "D";
                        else
                            dcFlag = "C";

                        bool error = false;

                        if (Math.Abs(otherAmt) - Math.Abs((decimal)bankDef.OTHER_AMT) != 0)
                        {
                            errorList.Add(NoticeHelper.getErrorHtml("Bank amount is mismatched with SunDB!", lineNo));
                            error = true;
                        }
                        if (dcFlag != (string)bankDef.D_C.Trim())
                        {
                            errorList.Add(NoticeHelper.getErrorHtml("DC Indicator is mismatched with SunDB!", lineNo));
                            error = true;
                        }
                        if (ccyCode != (string)bankDef.CONV_CODE.Trim())
                        {
                            errorList.Add(NoticeHelper.getErrorHtml("Currency is mismatched with SunDB!", lineNo));
                            error = true;
                        }

                        // any missmatch information will be filtered out
                        if (error)
                        {
                            if (chargeList.Count > 0)
                                chargeList.RemoveAt(chargeList.Count - 1);

                            updatedList.Add(this.getUpdateBankReconciliationSummaryDef(fileName, payRefNo, bankDef.JRNAL_NO.ToString(), ccyCode, fieldAmt, sunUpdate, nssUpdate, ""));
                            return;
                        }

                        // if all ok then update value date and bank's allocation
                        int valDate = Convert.ToInt32((Convert.ToDateTime(valueDate)).ToString("yyyyMMdd"));

                        IDataSetAdapter ad = getDataSetAdapter("SunSALFLDGApt", "GetBankTransactionCmd");
                        string sql = "SELECT * FROM SALFLDG" + dbName + " WITH (NOLOCK) WHERE TREFERENCE = '" + payRefNo + "'";
                        ad.SelectCommand.DbCommand.CommandText = sql;
                        ad.SelectCommand.DbCommand.CommandTimeout = 300;
                        ad.PopulateCommands();
                        SALFLDGDs ds = new SALFLDGDs();
                        int recordsAffected = ad.Fill(ds);

                        foreach (SALFLDGDs.SALFLDGRow row in ds.Tables[0].Rows)
                        {
                            row.TRANS_DATE = valDate;
                            if (row.ACCNT_CODE == bankDef.ACCNT_CODE)
                            {
                                row.ALLOCATION = "R";
                                if (textLine2.Substring(0, 2) == "TT")
                                    row.ANAL_T9 = textLine2.Substring(2).Trim();
                            }
                        }

                        int count = 0;

                        while (count <= 3)
                        {
                            try
                            {
                                ad.UpdateCommand.DbCommand.CommandTimeout = 60;
                                this.updateRecord(ad, ds);
                                break;
                            }
                            catch (Exception e)
                            {
                                if (count == 3)
                                    NoticeHelper.sendErrorMessage(e, "Line is => " + sourceLine);
                            }
                            finally
                            {
                                count++;
                            }
                        }
                        sunUpdate = true;

                        // add payment list 
                        if (!paymentList.ContainsKey(payRefNo))
                            paymentList.Add(payRefNo, Convert.ToDecimal(fieldAmt));

                        ArrayList list = getT9ByTransactionRef(payRefNo, dbName);

                        if (list != null && list.Count > 0)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                string t9 = list[i].ToString().Trim();
                                float delAmt = this.updateAP(sourceLine, bankName, accountNo, t9, ccyCode, valueDate, errorList, lineNo, userId);                                

                                nssUpdate = true;
                                string delAmtStr = "";
                                if (delAmt == float.NegativeInfinity)
                                {
                                    nssUpdate = false;
                                    delAmtStr = "N/A";
                                }
                                else
                                    delAmtStr = Math.Abs(delAmt).ToString();

                                updatedList.Add(this.getUpdateBankReconciliationSummaryDef(fileName, payRefNo, bankDef.JRNAL_NO.ToString(), ccyCode, delAmtStr, sunUpdate, nssUpdate, t9));
                            }
                        }
                        else
                        {
                            errorList.Add(NoticeHelper.getErrorHtml("Missing T9 Code (Sales Invoice Number or Contract Number)!", lineNo));
                            updatedList.Add(this.getUpdateBankReconciliationSummaryDef(fileName, payRefNo, bankDef.JRNAL_NO.ToString(), ccyCode, fieldAmt, sunUpdate, nssUpdate, ""));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "Line is => " + sourceLine);
            }
        }

        private float updateAP(string sourceLine, string bankName, string accountNo, string t9, string ccyCode, string payDate, ArrayList errorList, int lineNo, int userId)
        {
            // HKD : 2, GBP : 3, USD	
            System.DateTime pd = Convert.ToDateTime(payDate);
            int ccyId = 1;
            string[] temp = t9.Split('*');
            if (temp[0] == "" || temp[1] == "")
            {
                errorList.Add(NoticeHelper.getErrorHtml("Missing T9 Code : " + t9 + " !", lineNo));
                return 0;
            }
            string sunSupplierCode = temp[1];
            t9 = temp[0];
            bool isSplitFound = false;
            bool isMasterFound = false;

            switch (ccyCode.Trim())
            {
                case "GBP":
                    ccyId = 2;
                    break;
                case "USD":
                    ccyId = 3;
                    break;
                case "EUR":
                    ccyId = 12;
                    break;
                case "CNY":
                    ccyId = 4;
                    break;
            }

            string payRef = "";

            if (bankName == "HSBC") payRef = "HKB-";
            else if (bankName == "SCB") payRef = "SCB-";

            payRef += ccyCode.Trim();
            string newPayRef = "";

            if (this._bankRefList.Contains(accountNo))
            {
                newPayRef = this._bankRefList[accountNo].ToString();
            }

            payRef = newPayRef;

            string key = "";
            decimal exchangeRate = _commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, ccyId, pd);
            float payAmt = 0;
            int a;
            int sequenceNo = -1;
            InvoiceDef invoiceDef = null;

            // if t9 is invoice number
            if (t9.IndexOf("/") > 0 && (isInvoiceNo(t9) ||
                ((t9.Length == 11 && t9.Substring(t9.Length - 3, 1) == "/" ) ||
                (t9.Contains("-") && int.TryParse(t9.Substring(t9.IndexOf('-') + 1) , out sequenceNo)))
                && int.TryParse(t9.Substring(3, 5), out a)))
            {
                key = t9;
                if (t9.Length == 9)
                    key = "HK" + key;



                ArrayList list = null;
                if (isInvoiceNo(t9))
                {
                    if (t9.Contains("-"))
                         int.TryParse(t9.Substring(t9.IndexOf('-') + 1) , out sequenceNo);
                    list = _shippingWorker.getInvoiceByInvoiceNo(key.Substring(0, 3), Convert.ToInt32(key.Substring(4, 5)), Convert.ToInt32(key.Substring(10, 4)), sequenceNo);
                }
                else
                    list = _shippingWorker.getInvoiceByInvoiceNo(key.Substring(0, 3), Convert.ToInt32(key.Substring(3, 5)), Convert.ToInt32(key.Substring(9, 2)) + 2000, sequenceNo);

                if (list != null && list.Count > 0)
                    invoiceDef = (InvoiceDef) list[0];
                ArrayList splitShipmentList = null;

                if (invoiceDef != null)
                {
                    splitShipmentList  = (ArrayList) _orderSelectWorker.getSplitShipmentByShipmentId(invoiceDef.ShipmentId);

                    foreach (SplitShipmentDef splitShipment in splitShipmentList)
                    {
                        if (splitShipment.IsVirtualSetSplit == 1 || splitShipment.Vendor.SunAccountCode != sunSupplierCode)
                            continue;
                        splitShipment.APRefNo = payRef;
                        splitShipment.APDate = pd;
                        splitShipment.APExchangeRate = exchangeRate;                       
                        splitShipment.APAmt = splitShipment.TotalShippedSupplierGarmentAmountAfterDiscount - Math.Round(splitShipment.TotalShippedSupplierGarmentAmountAfterDiscount * splitShipment.QACommissionPercent / 100, 2, MidpointRounding.AwayFromZero);
                        if (splitShipment.VendorPaymentDiscountPercent != 0)
                            splitShipment.APAmt -= Math.Round(splitShipment.TotalShippedSupplierGarmentAmountAfterDiscount * splitShipment.VendorPaymentDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);
                        ShipmentDef shipmentDef = _orderSelectWorker.getShipmentByKey(invoiceDef.ShipmentId);                        
                        if (shipmentDef.LabTestIncome != 0)
                            splitShipment.APAmt -= Math.Round(splitShipment.TotalShippedQuantity * shipmentDef.LabTestIncome, 2, MidpointRounding.AwayFromZero);

                        payAmt = (float)splitShipment.APAmt;
                        _orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipment), userId);
                        
                        _shippingWorker.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, splitShipment.SplitShipmentId, ActionHistoryType.AR_AP_MAINTENANCE,
                            null, "Bank Reconciliation: Updated A/P Date & Reference No.", userId));

                        isSplitFound = true;
                        break;
                    }

                    if (!isSplitFound)
                    {
                        isMasterFound = true;
                        invoiceDef.APRefNo = payRef;
                        invoiceDef.APDate = pd;
                        invoiceDef.APExchangeRate = exchangeRate;
                        ShipmentDef shipmentDef = _orderSelectWorker.getShipmentByKey(invoiceDef.ShipmentId);                        
                        invoiceDef.APAmt = shipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount - Math.Round(shipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount * shipmentDef.QACommissionPercent / 100, 2, MidpointRounding.AwayFromZero);
                        if (shipmentDef.VendorPaymentDiscountPercent != 0)
                            invoiceDef.APAmt -= Math.Round(shipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount * shipmentDef.VendorPaymentDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);
                        if (shipmentDef.LabTestIncome != 0)
                            invoiceDef.APAmt -= Math.Round(shipmentDef.TotalShippedQuantity * shipmentDef.LabTestIncome, 2, MidpointRounding.AwayFromZero);

                        payAmt = (float)invoiceDef.APAmt;
                        _shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.AR_AP_MAINTENANCE, new ArrayList(), userId);

                        _shippingWorker.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, null, "Bank Reconciliation : Updated A/P Date & Reference No.", userId));
                    }
                }

                if (!isSplitFound && !isMasterFound)
                {
                    errorList.Add(NoticeHelper.getErrorHtml("No Record Found by this T9 Code : " + t9 + " !", lineNo));
                    return float.NegativeInfinity;
                }
                else
                {
                    if (splitShipmentList.Count != 0)
                    {
                        updateSplitInvoices(invoiceDef, splitShipmentList, payRef, pd, exchangeRate, userId);
                    }
                }
            }
            else // if the t9 is storing the contract number
            {
                // key = t9.Substring(0, t9.Length - 2);

                //YF8719827A01
                key = t9.Trim();
                int deliveryNo = 0;
                if (key.Length <= 3)
                {
                    errorList.Add(NoticeHelper.getErrorHtml("No Record Found by this T9 Code : " + t9 + " !", lineNo));
                    return float.NegativeInfinity;
                }
                if (!int.TryParse(key.Substring(key.Length - 2, 2), out deliveryNo))
                {
                    errorList.Add(NoticeHelper.getErrorHtml("No Record Found by this T9 Code : " + t9 + " !", lineNo));
                    return float.NegativeInfinity;
                }
                string splitSuffux = key.Substring(key.Length - 3, 1);
                string contractNo = key.Substring(0, key.Length - 3);
                
                SplitShipmentDef splitShipmentDef = _orderSelectWorker.getSplitShipmentByPONo(contractNo, splitSuffux, deliveryNo);

                if (splitShipmentDef != null)
                {
                    splitShipmentDef.APRefNo = payRef;
                    splitShipmentDef.APDate = pd;
                    splitShipmentDef.APExchangeRate = exchangeRate;
                    splitShipmentDef.APAmt = splitShipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount - Math.Round(splitShipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount * splitShipmentDef.QACommissionPercent / 100, 2, MidpointRounding.AwayFromZero);
                    //if (splitShipmentDef.VendorPaymentDiscountPercent != 0)
                    //{
                    //    splitShipmentDef.APAmt -= Math.Round(splitShipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount * splitShipmentDef.VendorPaymentDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);
                    //}
                    payAmt = (float)splitShipmentDef.APAmt;

                    _orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipmentDef), userId);
                    _shippingWorker.updateActionHistory(new ActionHistoryDef(splitShipmentDef.ShipmentId, splitShipmentDef.SplitShipmentId, ActionHistoryType.AR_AP_MAINTENANCE,
                        null, "Bank Reconciliation: Updated A/P Date & Reference No", userId));

                    //update invoice
                    this.updateSplitInvoices(_shippingWorker.getInvoiceByKey(splitShipmentDef.ShipmentId), 
                        (ArrayList) _orderSelectWorker.getSplitShipmentByShipmentId(splitShipmentDef.ShipmentId),
                        payRef, pd, exchangeRate, userId);
                }
                else                
                {
                    errorList.Add(NoticeHelper.getErrorHtml("No Record Found by this T9 Code : " + t9 + " !", lineNo));
                    return float.NegativeInfinity;
                }
            }
            return payAmt;
        }

        private void updateSplitInvoices(InvoiceDef invoiceDef, ArrayList splitShipmentList, string payRef, DateTime pd, decimal exchangeRate, int userId)
        {
            bool isUpdated = true;
            int splitCount = 0;
            decimal totalAPAmt = 0;
            foreach (SplitShipmentDef splitShipment in splitShipmentList)
            {
                if (splitShipment.IsVirtualSetSplit == 1)
                    continue;

                splitCount++;
                if (splitShipment.APDate == DateTime.MinValue)
                    isUpdated = false;
                else
                    totalAPAmt += splitShipment.APAmt;
            }

            if (splitCount > 0 && isUpdated)
            {
                invoiceDef.APDate = pd;
                invoiceDef.APRefNo = payRef;
                invoiceDef.APExchangeRate = exchangeRate;
                invoiceDef.APAmt = totalAPAmt;

                _shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.AR_AP_MAINTENANCE, new ArrayList(), userId);
            }
        }

        private BankReconciliationSummaryDef getUpdateBankReconciliationSummaryDef
            (string fileName, string txRef, string journalNo, string ccy, string amount, bool updateSun, bool updateNss, string nssKey)
        {
            BankReconciliationSummaryDef def = new BankReconciliationSummaryDef();
            def.TransactionRefNum = txRef;
            def.JournalNumber = journalNo;
            def.Currency = ccy;
            def.Amount = amount;
            def.SunUpdateStatus = (updateSun ? "Success" : "Fail");

            if (nssKey != null && nssKey != "")
                def.NssUpdateStatus = nssKey;
            else
                def.NssUpdateStatus = (updateNss ? "Success" : "Fail");
            def.FileName = fileName;
            return def;
        }

        private void checkChargeList(ArrayList chargeList, Hashtable paymentList, ArrayList errorList)
        {
            for (int i = 0; i < chargeList.Count; i++)
            {
                string chgStr = (string)chargeList[i];
                int pos = chgStr.IndexOf(",");

                string txRef = chgStr.Substring(0, pos);
                if (!paymentList.ContainsKey(txRef))
                {
                    errorList.Add(NoticeHelper.getErrorHtml("The charge with transaction reference <" + txRef + "> is missing corresponding payment data!", -99));
                    chargeList.RemoveAt(i);
                }
            }
        }

        private bool isInvoiceNo(string invoiceNo)
        {
            if (invoiceNo.Length != 14) {
                if (invoiceNo.IndexOf('-') == -1 )
                    return false;
            }
               
            if ( invoiceNo.Substring(3,1) != "/" || invoiceNo.Substring(9,1) != "/")
                return false;

            for ( int i = 4; i <= 8; i++)
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

        private ArrayList getT9ByTransactionRef(string txRef, string dbName)
        {
            IDataSetAdapter ad = getDataSetAdapter("SunSALFLDGApt", "GetBankTransactionCmd");

            string sql = "SELECT distinct LTRIM(RTRIM(c.ANAL_T9)) + '*' + LTRIM(RTRIM(a.ACCNT_CODE)) AS ANAL_T9" +
                " FROM 	SALFLDG" + dbName + " a WITH (NOLOCK) " +
                " INNER JOIN SSRFACC b WITH (NOLOCK) on a.ACCNT_CODE = b.ACCNT_CODE and b.SUN_DB = '" + dbName + "' and b.ACCNT_TYPE in ('C', 'T') " +
                " INNER JOIN SALFLDG" + dbName + " c WITH (NOLOCK) on c.D_C = 'C' and c.ACCNT_CODE = b.ACCNT_CODE and a.ALLOC_REF = c.ALLOC_REF " +
                " WHERE a.TREFERENCE = '" + txRef + "'" +
                " AND c.ALLOC_REF <> 0 AND a.ALLOC_REF = c.ALLOC_REF ";


            ad.SelectCommand.DbCommand.CommandText = sql;
            ad.SelectCommand.DbCommand.CommandTimeout = 300;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected < 1) return null;

            ArrayList list = new ArrayList();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                if (!row.IsNull("ANAL_T9"))
                    list.Add(row["ANAL_T9"].ToString());
            }
            return list;
        }


        private void addChargeData(ArrayList chargeList, string valueDate, string bankAccCode,
            string charge, string txRef, string journalNo, string officeCode, string ccy)
        {
            string s = txRef + "," + ccy + "," + charge + "," + valueDate + "," + bankAccCode + "," +
                journalNo + "," + officeCode;
            chargeList.Add(s);
        }

        private void completeBankReconciliation(string sourceFileName, int logonUserId, string bankName, ArrayList updateList, ArrayList errorList, ArrayList chargeList)
        {
            // create charge list file
            string fileName = "ChargeList.csv";
            string folderStr = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_TEMP_Folder");
            string filePath = folderStr + fileName;

            if (File.Exists(filePath))
                File.Delete(filePath);
            StreamWriter body = File.CreateText(filePath);

            string header = "TransactionReference,Currency,Charge,ValueDate,AccountCode,JournalNo,Office";
            body.WriteLine(header);

            for (int i = 0; i < chargeList.Count; i++)
            {
                string chgStr = (string)chargeList[i];
                body.WriteLine(chgStr);
            }
            body.Close();

            UserRef userRef = CommonUtil.getUserByKey(logonUserId);

            // error list 
            string errorString = "";
            if (errorList.Count > 0)
            {
                errorString = " Following errors are found : <br>";
                errorString += "<table cellSpacing=\"0\" cellPadding=\"3\" width=\"700px\" border=\"1\">";
                errorString += "<TR><TD COLSPAN=2 class=\"gridHeader\"> FileName : " + sourceFileName + " </TD></TR>";
                for (int i = 0; i < errorList.Count; i++)
                {
                    errorString += (string)errorList[i];
                }
                errorString += "</table>";
            }

            // update status list
            string updateListTable = this.getUpdateListHtml(sourceFileName, updateList);
            NoticeHelper.sendBankReconciliationMail(userRef, bankName, updateListTable, errorString, filePath);
        }

        private ArrayList getColumnsFromList(string bankName, string recordType)
        {
            ArrayList list = new ArrayList();
            int cnt = this._columnMapping.Count;
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    eBankingColumnMappingDef def = (eBankingColumnMappingDef)_columnMapping[i];
                    if (def.RecordType == recordType)
                        list.Add(def);
                }
            }
            return list;
        }

        private SunSALFLDGTblDef getSunAccountGPDebitDataDefByTransactionRef(string txRef, string dbName)
        {
            IDataSetAdapter ad = getDataSetAdapter("SunSALFLDGApt", "GetBankTransactionCmd");

            string sql = "SELECT distinct b.* FROM SALFLDG" + dbName + "  b WITH (NOLOCK) " +
                " INNER JOIN SSRFACC c WITH (NOLOCK) on b.ACCNT_CODE = c.ACCNT_CODE and c.SUN_DB = '" + dbName + "'" +
                " AND c.ACCNT_TYPE = 'B' " +
                " WHERE b.TREFERENCE = '" + txRef + "'";
            ad.SelectCommand.DbCommand.CommandText = sql;

            SALFLDGDs dataSet = new SALFLDGDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 300;
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            SunSALFLDGTblDef def = new SunSALFLDGTblDef();
            SunAccountTransactionMapping(dataSet.SALFLDG.Rows[0], def);
            return def;
        }

        private Hashtable getBankRefList(string paymentType)
        {
            Hashtable list = new Hashtable();
            ArrayList bankList = AccountWorker.Instance.getBankInfo();
            for (int i = 0; i < bankList.Count; i++)
            {
                BankAccountDef def = (BankAccountDef)bankList[i];
                if (def.PaymentType == paymentType)
                {
                    list.Add(def.BankAccountNo, def.PaymentReferenceCode);
                }
            }
            return list;
        }


        private  string getUpdateListHtml(string sourceFileName, ArrayList updateList)
        {
            string s =
                "<table cellSpacing=\"0\" cellPadding=\"3\" width=\"800px\" border=\"1\">" +
                "<TR><TD COLSPAN=6 class=\"gridHeader\"> Update status for bank statement file : " + sourceFileName + "</TD></TR>" +

                "<TR>" +
                "<TD class=\"colHeader\"> Trans Ref Number </TD>" +
                "<TD class=\"colHeader\"> Journal Number </TD>" +
                "<TD class=\"colHeader\"> Currency </TD>" +
                "<TD class=\"colHeader\"> Amount </TD>" +
                "<TD class=\"colHeader\"> Sun Update Status </TD>" +
                "<TD class=\"colHeader\" > NSS Update Status </TD>" +
                "</TR>";

            for (int i = 0; i < updateList.Count; i++)
            {
                BankReconciliationSummaryDef def = (BankReconciliationSummaryDef)updateList[i];
                s += this.getSuccessHtml(def.TransactionRefNum, def.JournalNumber, def.Currency, def.Amount, def.SunUpdateStatus, def.NssUpdateStatus);
            }
            s += "</table>";
            return s;

        }

        private  string getSuccessHtml(string txRef, string journalNo, string ccy, string amount, string updateSUN, string updateNSS)
        {
            string s = "";
            s =
                "<TR>" +
                "<TD class=\"dataCellStr\">" + txRef + "</TD>" +
                "<TD class=\"dataCellStr\">" + journalNo + "</TD>" +
                "<TD class=\"dataCellStr\">" + ccy + "</TD>" +
                "<TD class=\"dataCellNum\">" + amount + "</TD>" +
                "<TD class=\"dataCellStr\">" + updateSUN + "</TD>" +
                "<TD class=\"dataCellStr\">" + updateNSS + "</TD>" +
                "</TR>";
            return s;
        }

        #region COLUMN MAPPING

        internal void SunAccountTransactionMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(SALFLDGDs.SALFLDGRow)
                && target.GetType() == typeof(SunSALFLDGTblDef))
            {
                SALFLDGDs.SALFLDGRow row =
                    (SALFLDGDs.SALFLDGRow)source;
                SunSALFLDGTblDef def = (SunSALFLDGTblDef)target;

                def.ACCNT_CODE = row.ACCNT_CODE;
                def.ALLOCATION = row.ALLOCATION;
                def.AMOUNT = row.AMOUNT;
                def.CONV_CODE = row.CONV_CODE;
                def.CONV_RATE = row.CONV_RATE;
                def.D_C = row.D_C;
                def.DESCRIPTION = row.DESCRIPTN;
                def.JRNAL_LINE = row.JRNAL_LINE;
                def.JRNAL_NO = row.JRNAL_NO;
                def.JRNAL_SRCE = row.JRNAL_SRCE;
                def.JRNAL_TYPE = row.JRNAL_TYPE;
                def.OTHER_AMT = row.OTHER_AMT;
                def.PERIOD = row.PERIOD;
                def.TREFERENCE = row.TREFERENCE;
                def.ANAL_T0 = row.ANAL_T0;
                def.ANAL_T1 = row.ANAL_T1;
                def.ANAL_T2 = row.ANAL_T2;
                def.ANAL_T5 = row.ANAL_T5;
                def.ANAL_T9 = row.ANAL_T9;
            }
        }


        private void updateRecord(IDataSetAdapter ad, DataSet ds)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ad.Update(ds);
                ctx.VoteCommit();
                //ctx.VoteRollback(); // don't set commit here during testing
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

    }
}
