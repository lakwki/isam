using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Data;
using com.next.common.domain.types;
using com.next.infra.util;
using com.next.common.domain;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.common;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.account;
using com.next.common.web.commander;
using System.Text;
using com.next.isam.domain.common;
using com.next.isam.domain.product;
using System.Diagnostics;
using System.Linq;

namespace com.next.isam.appserver.account
{

    public class AccountDataUploadManager
    {
        private static AccountDataUploadManager _instance;
        private DataUploadWorker worker;
        private AccountWorker accountWorker;

        public AccountDataUploadManager()
        {
            worker = DataUploadWorker.Instance;
            accountWorker = AccountWorker.Instance;
        }

        public static AccountDataUploadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AccountDataUploadManager();
                }
                return _instance;
            }
        }

        #region Import Interface file and convert to XML for TALLY

        public int getSunAccountJournalInExcel(string serverFilePath, int userId, ArrayList journalList)
        {   // return :  >0 - Success (No. of Record)
            //           0  - No Record (nor non-empty worksheet)
            //          -1  - Invalid data format (Data type of columns are not valid)
            //          -2  - Cannot find the Excel / Worksheet
            //          -3  - Invalid file Path
            //          -4  - Unexpect status
            int status = -4;

            if (serverFilePath == String.Empty)
                status = -3; // Invalid path
            else
            {
                DataSet ds = worker.getFirstWorksheet(serverFilePath);
                if (ds == null)
                    status = 0;    // No Record
                else
                {
                    int transactionStart = -1;
                    JournalTransaction heading = new JournalTransaction();
                    DataRowCollection rows = ds.Tables[0].Rows;
                    for (int i = 0; i < rows.Count && transactionStart == -1; i++)
                    {
                        heading.getRawFromDataRow(rows[i]);
                        if (string.Equals(heading.AccountCode, "Account code", StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(heading.AccountName, "Name", StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(heading.TransactionReference, "Transaction Reference", StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(heading.TransactionDate, "Transaction Date", StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(heading.Description, "Description", StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(heading.OtherAmount, "Other Amount", StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(heading.BaseAmount, "Base Amount", StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(heading.JournalNo, "Journal No.", StringComparison.CurrentCultureIgnoreCase)
                            && string.Equals(heading.JournalLine, "Journal Line", StringComparison.CurrentCultureIgnoreCase)
                            )
                            transactionStart = (i >= rows.Count - 1 ? -1 : i + 1);
                    }

                    if (transactionStart == -1)
                        status = -1;    // Incorrect Data Format
                    else
                    {
                        // Get Work sheet Transaction
                        for (int r = transactionStart; r < rows.Count; r++)
                        {
                            if (!worker.isBlankRow(rows[r]))
                            {
                                int number;
                                JournalTransaction jt = new JournalTransaction();
                                jt.getFromDataRow(rows[r], ds.Locale);
                                if (int.TryParse(jt.AccountCode, out number) && int.TryParse(jt.JournalNo, out number))
                                    journalList.Add(jt);
                            }
                        }
                        status = journalList.Count;
                    }
                }
            }
            return status;
        }

        public string convertJournalToXML(ArrayList journalList, string fileName, int userId)
        {
            string refNo = string.Empty;
            int noOfTransaction = 0;

            journalList.Sort(new ArrayListHelper.Sorter("SortingKey"));
            for (int i = 0; i < journalList.Count; i++)
            {
                JournalTransaction jt = (JournalTransaction)journalList[i];
                if (refNo != jt.TransactionReference)
                {
                    refNo = jt.TransactionReference;
                    noOfTransaction++;
                }
            }
            return worker.generateJournalXML(journalList, fileName, noOfTransaction, userId);
        }

        public ArrayList getTallyUploadHistory()
        {
            return worker.getTallyUploadHistory();
        }

        public ArrayList getRecentTallyUploadHistory()
        {
            return worker.getRecentTallyUploadHistory();
        }

        #endregion

        public UploadFileRef getUploadFileInfo(string serverFileName)
        {
            return worker.getUploadFileInfo(serverFileName);
        }

        private DateTime getExcelDateTime(string excelCellValue)
        {
            DateTime dt = DateTime.MinValue;
            double d = double.MinValue;
            if (!DateTime.TryParse(excelCellValue, out dt))
                if (double.TryParse(excelCellValue, out d))
                    dt = DateTime.FromOADate(d);
            return dt;
        }

        private string getSheetName(string sheetName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in sheetName)
            {
                if (c != '\'' && c != '$')
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString();
        }

        #region Import NS-LED Sales Excel file
        public int getNSLedSalesFromExcel2020(string filePath, int userId, out string notFoundItemNo)
        {   // return : >0  - Success (No. of Record)
            //           0  - No Record (nor non-empty worksheet)
            //          -1  - Invalid data format (Data type of columns are not valid)
            //          -2  - Cannot find the Excel / Worksheet
            //          -3  - Invalid file Path
            //          -4  - Unexpect status
            //          -5  - Same date, office, invoice and type are uploaded before
            //          -6  - ex rate undefined
            //          -7  - duplicate invoice no.
            //          -8  - Item No not Exist
            //          -9  - Actual freight cpu missing
            //          -10 - Duty Percent missing
            //          -11 - Total line amt is not aligned with the cover sheet amt

            notFoundItemNo = string.Empty;
            if (string.IsNullOrEmpty(filePath))
            {
                return -3;
            }
            FileInfo fileInfo = new FileInfo(filePath);
            ArrayList sheetList = worker.getExcelWorksheetName(filePath);
            string dirSheetName = string.Empty;
            string retSheetName = string.Empty;
            string summarySheetName = string.Empty;

            foreach (string item in sheetList)
            {
                if (item.ToLower().Contains("online data") && !item.ToLower().Contains("wrong") && !item.ToLower().Contains(" x") && dirSheetName == string.Empty)
                {
                    dirSheetName = getSheetName(item);
                }
                else if (item.ToLower().Contains("retail sales") && !item.ToLower().Contains("wrong") && !item.ToLower().Contains(" x") && retSheetName == string.Empty)
                {
                    retSheetName = getSheetName(item);
                }
                else if (item.ToLower().Contains("summary") && !item.ToLower().Contains("wrong") && !item.ToLower().Contains(" x") && summarySheetName == string.Empty)
                {
                    summarySheetName = getSheetName(item);
                }
            }
            if (string.IsNullOrEmpty(dirSheetName.ToString()) && string.IsNullOrEmpty(retSheetName.ToString()))
            {
                return -2;
            }
            List<List<string>> invoiceData = worker.getWorkSheet(filePath, "Invoice");
            List<List<string>> dirSalesData = worker.getWorkSheet(filePath, dirSheetName);
            List<List<string>> retSalesData = worker.getWorkSheet(filePath, retSheetName);
            List<List<string>> summaryData = worker.getWorkSheet(filePath, summarySheetName);
            if (invoiceData == null || invoiceData.Count == 0 || ((dirSalesData.Count == 0 || dirSalesData == null) && (retSalesData.Count == 0 || retSalesData == null)))
            {
                return -2;
            }
            decimal usdExRate = 0m;
            foreach (List<string> col in summaryData)
            {
                int num = 0;
                foreach (string cellValue in col)
                {
                    if (cellValue.ToLower().Replace("''", string.Empty).Replace("s", string.Empty).Trim() == "dollar exchange rate" && !string.IsNullOrEmpty(col[num + 1].Trim()) && decimal.TryParse(col[num + 1], out usdExRate))
                    {
                        break;
                    }
                    else if (cellValue.ToLower().Replace("''", string.Empty).Replace("s", string.Empty).Trim() == "dollar exchange rate" && !string.IsNullOrEmpty(col[num + 2].Trim()) && decimal.TryParse(col[num + 2], out usdExRate))
                    {
                        break;
                    }
                    else if (cellValue.ToLower().Trim() == "start date:" && !string.IsNullOrEmpty(col[num + 13].Trim()) && decimal.TryParse(col[num + 13], out usdExRate)) // row 4
                    {
                        break;
                    }
                    else if (cellValue.ToLower().Trim() == "start date:" && !string.IsNullOrEmpty(col[num + 12].Trim()) && decimal.TryParse(col[num + 12], out usdExRate)) // row 4
                    {
                        break;
                    }

                    num++;
                }
                if (usdExRate != 0m)
                {
                    break;
                }
            }
            if (usdExRate == 0m)
            {
                return -6;
            }


            //check item no exist
            bool exist = false;
            int index = 0;
            int officeId = 0;
            string supplierCode = string.Empty;

            foreach (List<string> col in invoiceData)
            {
                if (col.Count >= 7)
                {
                    if (col[5].ToLower().Contains("supplier number"))
                    {
                        string text = col[7].Trim();
                        text = ((text.Length > 2) ? text.Substring(2) : string.Empty);
                        if (string.IsNullOrEmpty(text))
                        {
                            throw new Exception("Cannot get correct supplier number.");
                        }
                        officeId = IndustryUtil.getOfficeIdByUKSupplierCode(text);
                        supplierCode = text;
                    }
                }
            }

            List<string> itemNoList = new List<string>();
            foreach (List<string> temp in retSalesData)
            {
                if (temp.Count >= 18)
                {
                    if (temp.Contains("NSVE") || temp.Contains("Comm Value"))
                    {
                        index = temp.IndexOf("item_code");
                        exist = true;
                    }
                    else if (exist)
                    {
                        string itemNo = temp[index].Trim();
                        if (itemNo == string.Empty)
                        {
                            break;
                        }
                        itemNoList.Add(itemNo);
                    }
                }
            }
            exist = false;

            foreach (List<string> temp in dirSalesData)
            {
                if (temp.Count >= 18)
                {
                    if (temp.Contains("NSVE") || temp.Contains("Comm Value"))
                    {
                        index = temp.IndexOf("ItemNo");
                        exist = true;
                    }
                    else if (exist)
                    {
                        string itemNo = temp[index].Trim();
                        if (itemNo == string.Empty)
                        {
                            break;
                        }
                        itemNoList.Add(itemNo);
                    }
                }
            }

            int detailId = 1;
            int returnValue = 0;
            NSLedImportFileDef importFile = new NSLedImportFileDef();
            List<NSLedSalesDef> list = new List<NSLedSalesDef>();

            returnValue = uploadNSLedDNData(fileInfo, "D", invoiceData, usdExRate, importFile);
            if (returnValue > 0)
            {
                List<string> distinctItemNo = itemNoList.GroupBy(p => itemNoList).Select(g => g.First()).ToList();
                foreach (string itemNo in distinctItemNo)
                {
                    NSLedRangePlanDef rangePlan = AccountWorker.Instance.getNSLedRangePlan(officeId, itemNo, AccountWorker.Instance.getNSLedRangePlanSeasonIdByFiscalWeek(officeId, itemNo, importFile.FiscalYear, importFile.FiscalWeek));
                    if (rangePlan == null)
                    {
                        notFoundItemNo = itemNo;
                        return -8;
                    }
                    else if (rangePlan.ActualFreightUSD == 0 && itemNo != "872834")
                    {
                        notFoundItemNo = itemNo;
                        return -9;
                    }
                    else if (itemNo != "M05387" && itemNo != "M06398" && itemNo != "751329" && itemNo != "165879" && itemNo != "350696" && itemNo != "367800" 
                            && AccountWorker.Instance.getNSLedSelfBilledSupplierCodeByCriteria(supplierCode, officeId).Count > 0 && rangePlan.DutyPercent == 0)
                    {
                        notFoundItemNo = itemNo;
                        return -10;
                    }
                }

                if (dirSalesData != null && dirSalesData.Count > 0)
                {
                    returnValue = uploadNSLedDNLineData(fileInfo, "D", invoiceData, dirSalesData, usdExRate, importFile, list, detailId);
                }
                if (returnValue >= 0 && retSalesData != null && retSalesData.Count > 0)
                {
                    detailId = returnValue;
                    returnValue = uploadNSLedDNLineData(fileInfo, "R", invoiceData, retSalesData, usdExRate, importFile, list, detailId);
                }

                /*
                if (importFile.TotalNSCommissionAmt != list.Sum(x => x.NSCommAmtInUSD))
                    return -11;
                */

                if (returnValue > 0)
                {
                    if (returnValue > 0)
                    {
                        AccountManager.Instance.updateNSLedSales(importFile, list, userId);
                    }
                    ArrayList logList = worker.getFileUploadLogByCriteria(FileUploadLogDef.Type.NSLedNSLedSalesDataUpload.GetHashCode(), fileInfo.Name, -1);
                    if (logList.Count > 0)
                    {
                        FileUploadLogDef fileUploadLogDef = (FileUploadLogDef)logList[0];
                        fileUploadLogDef.IsUploaded = 1;
                        fileUploadLogDef.UploadedOn = DateTime.Now;
                        FileUploadManager.Instance.updateFileUploadLog(fileUploadLogDef);

                        MailHelper.sendGeneralMessage("NS-LED Weekly Invoice was uploaded successfully / " + importFile.TotalNSCommissionAmt.ToString() + " vs " + list.Sum(x => x.NSCommAmtInUSD).ToString(), OfficeId.getName(importFile.OfficeId) + " / " + importFile.Filename);
                    }
                }

            }

            return returnValue;
        }


        private int uploadNSLedDNData(FileInfo info, string customerType, List<List<string>> invoice, decimal usdExRate, NSLedImportFileDef importFile)
        {
            if (string.IsNullOrEmpty(importFile.InvoiceNo))
            {
                importFile.FileId = int.MinValue;
                importFile.Filename = info.Name;
                foreach (List<string> col in invoice)
                {
                    if (col.Count >= 7)
                    {
                        if (col[0] == "InvoiceNo." || col[0] == "Invoice No." || col[0] == "CreditNo." || col[0] == "Credit No." || col[0] == "Credit note No.")
                        {
                            importFile.InvoiceNo = col[1];
                            if (accountWorker.getNSLedImportFileByInvoiceNo(col[1]) != null)
                                return -7;

                        }
                        if (col[0] == "Currency")
                        {
                            importFile.CurrencyId = CommonUtil.getCurrencyByCurrencyCode(col[1].Trim()).CurrencyId;
                            if (importFile.CurrencyId == CurrencyId.GBP.Id && usdExRate <= 1.00m)
                            {
                                throw new Exception("Invalid USD Dollars Exchange Rate");
                            }
                        }
                        if (col[0].ToUpper() == "TOTAL COMMISSION")
                        {
                            importFile.TotalNSCommissionAmt = Math.Round(Convert.ToDecimal(col[7].Trim()), 2, MidpointRounding.AwayFromZero);
                        }
                        if (col[5].ToLower().Contains("tax point") && !string.IsNullOrEmpty(col[7]))
                        {
                            importFile.InvoiceDate = getExcelDateTime(col[7]);
                            PeriodWeekInfoDef fiscalWeekByDate = CommonUtil.getFiscalWeekByDate(importFile.InvoiceDate);
                            importFile.FiscalYear = fiscalWeekByDate.Year;
                            importFile.FiscalWeek = fiscalWeekByDate.YearWeek;
                            importFile.Period = fiscalWeekByDate.Period;
                        }
                        if (col[5].ToLower().Contains("supplier number"))
                        {
                            string text = col[7].Trim();
                            text = ((text.Length > 2) ? text.Substring(2) : string.Empty);
                            if (string.IsNullOrEmpty(text))
                            {
                                throw new Exception("Cannot get correct supplier number.");
                            }
                            int officeId = IndustryUtil.getOfficeIdByUKSupplierCode(text);
                            importFile.SupplierCode = text;
                            importFile.OfficeId = officeId;

                        }
                    }
                }
                if (string.IsNullOrEmpty(importFile.InvoiceNo) || importFile.CurrencyId <= 0 || string.IsNullOrEmpty(importFile.SupplierCode) || importFile.InvoiceDate == DateTime.MinValue || importFile.OfficeId == 0)
                {
                    return -1;
                }
                if (getNSLedUploadHistory(importFile.InvoiceNo, importFile.InvoiceDate.Date, importFile.OfficeId).Count > 0)
                {
                    return -5;
                }
                importFile.IsDutiable = (AccountWorker.Instance.getNSLedSelfBilledSupplierCodeByCriteria(importFile.SupplierCode, importFile.OfficeId).Count > 0);
            }
            return 1;
        }

        private int uploadNSLedDNLineData(FileInfo info, string customerType, List<List<string>> invoice, List<List<string>> invoiceDetail, decimal usdExRate, NSLedImportFileDef importFile, List<NSLedSalesDef> salesList, int detailId)
        {

            bool flag = false;
            int idx_itemNo = 0;
            int idx_ItemOption = 0;
            int idx_Qty = 0;
            int idx_Amt = 0;
            int idx_ReturnQty = 0;
            int idx_ReturnAmt = 0;
            int idx_CountryCode = 0;
            int idx_VAT = 0;
            int idx_NSVE = 0;
            int idx_CommRate = 0;

            foreach (List<string> col in invoiceDetail)
            {
                if (col.Count >= 18)
                {
                    if (col.Contains("NSVE") || col.Contains("Comm Value"))
                    {
                        idx_itemNo = col.IndexOf((customerType == "D") ? "ItemNo" : "item_code");
                        idx_ItemOption = col.IndexOf((customerType == "D") ? "ItemOption" : "itemoption_code");
                        idx_Qty = col.IndexOf((customerType == "D") ? "DespQty" : "Qty");
                        idx_Amt = col.IndexOf((customerType == "D") ? "DespatchesValue" : "RealValueVATInc");
                        idx_ReturnQty = col.IndexOf((customerType == "D") ? "RtnQty" : "Qty");
                        idx_ReturnAmt = col.IndexOf((customerType == "D") ? "Revised Returns Value" : "RealValueVATInc");
                        idx_CountryCode = col.IndexOf("CountryCode");
                        idx_VAT = col.IndexOf((customerType == "D") ? "VATRate" : "RealValueVATEx");
                        idx_NSVE = col.IndexOf((customerType == "D") ? "NSVE" : "RealValueVATEx");
                        idx_CommRate = col.IndexOf((customerType == "D") ? "Commission Rate" : "Comm Rate");
                        if (idx_CommRate == -1)
                        {
                            idx_CommRate = col.IndexOf((customerType == "D") ? "Commision Rate" : "Comm Rate");
                        }
                        flag = true;
                    }
                    else if (flag)
                    {
                        string itemNo = (col[idx_itemNo].Trim() + "        ").Substring(0, 6).Trim();
                        if (itemNo == string.Empty)
                        {
                            break;
                        }
                        NSLedSalesDef salesDef = new NSLedSalesDef();
                        salesDef.FileId = importFile.FileId;
                        salesDef.DetailId = detailId;
                        salesDef.ItemNo = itemNo;
                        ContractDef contractDef = OrderSelectWorker.Instance.getContractByItemNoAndCustomerId(salesDef.ItemNo, CustomerDef.Id.ns_led.GetHashCode(), -1, 1);
                        if (contractDef == null)
                        {
                            contractDef = OrderSelectWorker.Instance.getContractByItemNoAndCustomerId(salesDef.ItemNo, CustomerDef.Id.manu_led.GetHashCode(), -1, 1);
                        }
                        if (contractDef == null)
                        {
                            throw new Exception("Invalid NS-LED Item No #" + salesDef.ItemNo);
                        }
                        if (contractDef.Office.OfficeId != importFile.OfficeId)
                        {
                            if (salesDef.ItemNo != "367800" && salesDef.ItemNo != "350696" && salesDef.ItemNo != "486688")
                                throw new Exception("Invalid NS-LED Item No #" + salesDef.ItemNo + " (under different office)");
                        }
                        if (customerType == "D")
                        {
                            salesDef.SizeOptionNo = (col[idx_ItemOption].Trim() + "        ").Substring(6).Trim();
                        }
                        else if (customerType == "R")
                        {
                            salesDef.SizeOptionNo = (col[idx_ItemOption].Trim() + "  ").Substring(0, 2).Trim();
                            salesDef.SizeOptionNo = salesDef.SizeOptionNo.PadLeft(2, '0');
                        }
                        if (string.IsNullOrEmpty(salesDef.SizeOptionNo))
                        {
                            throw new Exception("Cannot get size option no from " + ((customerType == "D") ? "ItemOption" : "itemoption_code") + " column");
                        }
                        SizeOptionRef sizeOption = CommonWorker.Instance.getNSLedSizeOption(salesDef.ItemNo, salesDef.SizeOptionNo);
                        ProductDef product = ProductWorker.Instance.getProductByItemNo(salesDef.ItemNo);
                        salesDef.SizeDesc = ((sizeOption != null) ? sizeOption.SizeDescription : "N/A");
                        salesDef.ItemDesc = ((product == null) ? "N/A" : ((product.ShortDesc.Trim() == string.Empty) ? product.Description1 : product.ShortDesc));

                        int qty = int.Parse(col[idx_Qty]);
                        decimal amt = decimal.Parse(col[idx_Amt], NumberStyles.Any, CultureInfo.InvariantCulture);
                        int returnQty = int.Parse(col[idx_ReturnQty]);
                        decimal returnAmt = decimal.Parse(col[idx_ReturnAmt], NumberStyles.Any, CultureInfo.InvariantCulture);
                        decimal vat = decimal.Parse(col[idx_VAT], NumberStyles.Any, CultureInfo.InvariantCulture);
                        decimal nsve = decimal.Parse(col[idx_NSVE], NumberStyles.Any, CultureInfo.InvariantCulture);
                        decimal commRate = decimal.Parse(col[idx_CommRate].Trim().Replace("%", string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture);
                        vat = Math.Abs((customerType == "D") ? vat : ((vat == 0m) ? 0m : Math.Round((amt - vat) / vat * 100m, 0, MidpointRounding.AwayFromZero)));
                        salesDef.DespatchQty = ((customerType == "D") ? qty : ((qty > 0) ? qty : 0));
                        salesDef.DespatchAmt = ((customerType == "D") ? amt : ((amt > 0m) ? amt : 0m));
                        salesDef.ReturnQty = ((customerType == "D") ? (returnQty * -1) : ((returnQty < 0) ? returnQty : 0));
                        salesDef.ReturnAmt = ((customerType == "D") ? (returnAmt * -1m) : ((returnAmt < 0m) ? returnAmt : 0m));
                        salesDef.NetQty = salesDef.DespatchQty + salesDef.ReturnQty;
                        salesDef.NetAmt = salesDef.DespatchAmt + salesDef.ReturnAmt;
                        salesDef.CountryOfSale = ((customerType == "D") ? col[idx_CountryCode] : "GB");
                        salesDef.VATPercent = vat;
                        salesDef.VAT = ((customerType == "D") ? (salesDef.NetAmt - nsve) : (amt - nsve));
                        salesDef.NSVE = nsve;
                        salesDef.CommPercent = commRate * 100m;
                        salesDef.UKCommAmt = salesDef.NSVE * commRate;
                        salesDef.NSCommAmt = salesDef.NSVE - salesDef.UKCommAmt;
                        salesDef.USDExRate = usdExRate;
                        salesDef.UKCommAmtInUSD = Math.Round(salesDef.UKCommAmt * usdExRate, 2, MidpointRounding.AwayFromZero);
                        salesDef.NSCommAmtInUSD = Math.Round(salesDef.NSCommAmt * usdExRate, 2, MidpointRounding.AwayFromZero);
                        salesDef.Status = 1;
                        salesDef.CustomerType = customerType;
                        if (salesDef.DespatchQty != 0 || salesDef.ReturnQty != 0)
                        {
                            if (salesDef.UKCommAmtInUSD == 0m)
                            {
                                salesDef.UKCommAmtInUSD = 0.01m;
                            }
                            if (salesDef.NSCommAmtInUSD == 0m)
                            {
                                salesDef.NSCommAmtInUSD = 0.01m;
                            }
                            if (salesDef.UKCommAmt == 0m)
                            {
                                salesDef.UKCommAmt = 0.01m;
                            }
                            if (salesDef.NSCommAmt == 0m)
                            {
                                salesDef.NSCommAmt = 0.01m;
                            }
                            salesList.Add(salesDef);
                            Debug.WriteLine("Wrote Detail Line : " + detailId.ToString());
                            Debug.WriteLine(string.Join("\n", col.ToArray()));
                            detailId++;
                        }
                    }
                }
            }
            return detailId;
        }

        public ArrayList getNSLedUploadHistory(int uploadUserId, DateTime startDate, DateTime endDate)
        {
            ArrayList list = AccountWorker.Instance.getNSLedImportFileByCriteria(uploadUserId, startDate, endDate);
            list.Sort(new ArrayListHelper.Sorter("CreatedOn", false));
            return list;
        }

        public ArrayList getNSLedUploadHistory(string invoiceNo, DateTime invoiceDate, int officeId)
        {
            ArrayList list = AccountWorker.Instance.getNSLedImportFileByCriteria(invoiceNo, invoiceDate, officeId);
            list.Sort(new ArrayListHelper.Sorter("CreatedOn", false));
            return list;
        }

        #endregion

    }
}
