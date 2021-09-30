using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;

using com.next.isam.dataserver.worker;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.infra.util;
using com.next.common.domain;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.common;
using com.next.isam.appserver.helper;

using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;

namespace com.next.isam.appserver.Shipping
{
    public class ShippingDataUploadManager
    {
        private static ShippingDataUploadManager _instance;
        private DataUploadWorker worker;
        private appserver.helper.TableHelper tableHelper;
        private MailBoxHelper mailboxHelper;
        private int adminUserId = 99999;

        public ShippingDataUploadManager()
        {
            worker = DataUploadWorker.Instance;
            tableHelper = appserver.helper.TableHelper.Instance;
            mailboxHelper = MailBoxHelper.Instance;
        }

        public static ShippingDataUploadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShippingDataUploadManager();
                }
                return _instance;
            }
        }

        private void releaseObject(object obj)
        {
            worker.releaseObject(obj);
        }

        #region Import functions

        public void importAllFile()
        {
            //MailHelper.sendGeneralMessage("Supplier Invoice No Upload Started", DateTime.Now.ToString());
            importSupplierInvoinceNoFiles();
            //MailHelper.sendGeneralMessage("Supplier Invoice No Upload End", DateTime.Now.ToString());
            //MailHelper.sendGeneralMessage("Daily Booking Upload Started", DateTime.Now.ToString());
            importDailyBookingFiles();
            //MailHelper.sendGeneralMessage("Daily Booking Upload End", DateTime.Now.ToString());
        }

        private void AddUploadLog(int shipmentId, AmendmentType amendmentType, string remark, int userId)
        {
            //ActionHistoryDef action = new ActionHistoryDef(shipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, amendType, remark);
            ShippingWorker.Instance.updateActionHistory(newUploadLog(shipmentId, remark, amendmentType, userId));
        }

        private ActionHistoryDef newUploadLog(int shipmentId, string remark)
        {
            return newUploadLog(shipmentId, remark, null, adminUserId);
        }

        private ActionHistoryDef newUploadLog(int shipmentId, string remark, AmendmentType amendmentType, int userId)
        {
            return new ActionHistoryDef(shipmentId, 0, ActionHistoryType.SHIPPING_UPLOAD, amendmentType, remark, userId);
        }

        #region Import Supplier Invoice No and Shipping Document Receipt Date

        public void importSupplierInvoinceNoFiles()
        {
            string uploadFolder;
            string archiveFolder;
            UploadFileRef uploadFile;
            int importStatus;

            try
            {
                uploadFolder = WebConfig.getValue("appSettings", "SUPPLIER_INVOICE_NO_UPLOAD_FOLDER");
                archiveFolder = uploadFolder + "Archive\\";
                ArrayList uploadFileList = FileUploadManager.Instance.getServerFileList(uploadFolder, UploadFileType.SupplierInvoiceNo.FileTypeId);

                //string serverFilePath = copyUploadFileToServer(updFile.Value, getUploadFilePrefix(1));
                foreach (string fileName in uploadFileList)
                {
                    uploadFile = worker.getUploadFileInfo(fileName);
                    //prefix = info.FilePrefix.Split(char.Parse("-"));
                    if (uploadFile.FileType == UploadFileType.SupplierInvoiceNo && uploadFile.FileName != string.Empty && uploadFile.UploadUser != null)
                    {
                        ArrayList attachmentList = new ArrayList();
                        ArrayList importResult = new ArrayList();
                        importStatus = importSupplierInvoiceNoExcelFile(uploadFolder + uploadFile.ServerFileName, uploadFile.UploadUser.UserId, importResult);

                        if (importStatus >= -1)
                        {
                            FileUploadManager.Instance.moveUploadFile(uploadFolder + uploadFile.ServerFileName, archiveFolder + uploadFile.ServerFileName);
                            FileUploadManager.Instance.copyUploadFile(archiveFolder + uploadFile.ServerFileName, uploadFolder + uploadFile.FileName);
                            attachmentList.Add(uploadFolder + uploadFile.FileName);
                        }
                        switch (importStatus)
                        {
                            case 1:     // upload without failure 
                                NoticeHelper.sendUploadSupplierInvoiceNoMail(attachmentList, uploadFile.UploadUser.UserId);
                                break;
                            case 0:     // upload with failure 
                                NoticeHelper.sendUploadSupplierInvoiceNoMail(attachmentList, uploadFile.UploadUser.UserId, tableHelper.generateHtmlTable(importResult));
                                break;
                            case -1:     // Invalid Format (All the invoice no (first column) is invalid)
                                NoticeHelper.sendUploadSupplierInvoiceNoFailMail(attachmentList, uploadFile.UploadUser.UserId, tableHelper.generateHtmlTable(importResult));
                                break;
                            default:    // Fail to upload Excel File(worksheet not found or other reaseon)
                                NoticeHelper.sendGeneralNoticeToAdmin("[ISAM]Shipping Data Upload Error", "Fail to upload the Excel file " + uploadFile.ServerFileName + ".", "Shipping Data Upload", attachmentList);
                                break;
                        }
                        if (importStatus >= -1)
                        {
                            FileUploadManager.Instance.removeUploadFile(uploadFolder + uploadFile.FileName);
                            ArrayList logList = worker.getFileUploadLogByCriteria(FileUploadLogDef.Type.ShippingDataUpload.GetHashCode(), fileName, -1);
                            if (logList.Count > 0)
                            {
                                FileUploadLogDef logDef = (FileUploadLogDef)logList[0];
                                logDef.IsUploaded = 1;
                                logDef.UploadedOn = DateTime.Now;
                                FileUploadManager.Instance.updateFileUploadLog(logDef);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public int importSupplierInvoiceNoExcelFile(string serverFilePath, int userId, ArrayList returnList)
        {   // return :  2  - Empty worksheet
            //           1  - Success (uploaded without error)
            //           0  - Partially success (uploaded with some error/warning)
            //          -1  - Invalid format (All the Invoice No are not valid)
            //          -2  - Cannot find the Excel / Worksheet
            //          -3  - Invalid file Path
            //          -4  - Unexpect status
            ArrayList supInvNoList;
            ArrayList failList = null;
            ArrayList detailRow, headerRow, dataStyle;
            int uploadCount = 0;
            int WarningCount = 0;
            int SkipCount = 0;
            int InvalidKeyCount = 0;
            bool InvalidFormat = false;
            int ErrorCount = 0;
            string failReason;
            ArrayList importStatus;
            int returnStatus = -4;

            try
            {
                if (serverFilePath != String.Empty)
                {
                    //supInvNoList = DataUploadWorker.Instance.getSupplierInvoiceNoWorksheet(serverFilePath);
                    supInvNoList = DataUploadWorker.Instance.getAllSupplierInvoiceNoWorksheet(serverFilePath);
                    if (supInvNoList != null)
                    {
                        foreach (object[] cells in supInvNoList)
                        {   // Check the format of import list (column count must be less than 3)
                            bool isBlankLine = true;
                            for (int i = 0; i < cells.Length; i++)
                                if (cells[i].ToString().Trim() != string.Empty)
                                    isBlankLine = false;
                            if (!isBlankLine)   // skip validation on blank line
                            {
                                if (cells.Length < 2)
                                    InvalidFormat = true;
                                else
                                    if (cells.Length > 3)
                                        for (int i = 3; i < cells.Length && !InvalidFormat; i++)
                                            if (cells[i].ToString().Trim() != string.Empty)
                                                InvalidFormat = true;
                                if (InvalidFormat)
                                    break;
                            }
                        }
                        if (!InvalidFormat)
                        {
                            object[] firstRow = (object[])supInvNoList[0];
                            if (worker.getInvoiceNoSegment(firstRow[0].ToString()) == null)
                                // not a invoice no. -> assume it is a column header row.
                                supInvNoList.Remove(firstRow);      // Remove the column header row

                            failList = new ArrayList();
                            foreach (object[] cells in supInvNoList)
                            {
                                failReason = string.Empty;
                                importStatus = importSupplierInvoiceNoRecord(cells, userId);
                                string[] rowStatus = ((string)importStatus[0]).Split(char.Parse("|"));
                                switch (rowStatus[0])
                                {
                                    case "SUCCESS":
                                        uploadCount++;
                                        break;
                                    case "SKIP":
                                        SkipCount++;
                                        break;
                                    case "WARNING":
                                        WarningCount++;
                                        failReason = "";
                                        for (int i = 1; i < importStatus.Count; i++)
                                        {   // get the column status & detail
                                            string[] colStatus = ((string)importStatus[i]).Split(char.Parse("|"));
                                            if (colStatus.Length > 1)
                                                failReason += (failReason == "" ? "" : ";<br>") + colStatus[1];
                                        }
                                        break;
                                    case "INVALID_KEY":   // "Invalid Invoice No.";
                                        InvalidKeyCount++;
                                        ErrorCount++;
                                        failReason = (rowStatus.Length > 1 ? rowStatus[1] : "Unknown Reason");
                                        break;
                                    case "ERROR":
                                        ErrorCount++;
                                        failReason = (rowStatus.Length > 1 ? rowStatus[1] : "Unknown Reason");
                                        break;
                                    default:
                                        ErrorCount++;
                                        failReason = "Unknown Reason";
                                        break;
                                }

                                if (rowStatus[0] != "SUCCESS" && rowStatus[0] != "SKIP") //(rowStatus < 0)
                                {  // Add record to failure list
                                    detailRow = new ArrayList();
                                    detailRow.Add("DATA");
                                    detailRow.Add(cells[0].ToString());
                                    detailRow.Add(cells[1].ToString());
                                    if (cells.Length > 2)
                                    {
                                        string cell = cells[2].ToString();
                                        DateTime importDate;
                                        //The default format of the date field we get from Excel is M/d/yyyy
                                        if (DateTime.TryParseExact(cell, "M/d/yyyy", CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out importDate))
                                            detailRow.Add(DateTimeUtility.getDateString(importDate));
                                        else
                                            detailRow.Add(cell);
                                    }
                                    else
                                        detailRow.Add("");
                                    detailRow.Add(failReason);
                                    failList.Add(detailRow);
                                }
                            }
                        }
                        if (supInvNoList.Count == SkipCount)
                        {
                            returnList = null;
                            returnStatus = 2;   // Empty worksheet
                        }
                        else

                            if (InvalidFormat || (InvalidKeyCount == supInvNoList.Count - SkipCount))
                            {   // Invalid Format (All the data type of the column in the list is invalid) - return a blank list with standard format
                                // standard upload format
                                detailRow = new ArrayList();
                                detailRow.Add("HEADER");
                                detailRow.Add("NSL Invocie No.");
                                detailRow.Add("Supplier Invoice No.");
                                detailRow.Add("Shipping Document Receipt Date");
                                returnList.Add(detailRow);

                                detailRow = new ArrayList();
                                detailRow.Add("DATA");
                                detailRow.Add("&nbsp;");
                                detailRow.Add("&nbsp;");
                                detailRow.Add("&nbsp;");
                                returnList.Add(detailRow);
                                returnList.Add(detailRow);

                                returnStatus = -1;      // Invalid format - import fail
                            }
                            else
                            {
                                if (ErrorCount == 0 && WarningCount == 0)
                                {   // Successfully import all record
                                    failList.Clear();
                                    returnStatus = 1;       // Import Success
                                }
                                else
                                {   // Partially import - return the Fail list
                                    // Fail list header
                                    headerRow = new ArrayList();
                                    headerRow.Add("HEADER");
                                    headerRow.Add("NSL Invoice No.");
                                    headerRow.Add("Supplier Invoice No.");
                                    headerRow.Add("Shipping Document Receipt Date");
                                    headerRow.Add("Fail Reason");

                                    dataStyle = new ArrayList();
                                    dataStyle.Add("STYLE");
                                    dataStyle.Add("STYLE='FONT-SIZE:12;TEXT-ALIGN:CENTER;'");
                                    dataStyle.Add("STYLE='FONT-SIZE:12;TEXT-ALIGN:LEFT;'");
                                    dataStyle.Add("STYLE='FONT-SIZE:12;TEXT-ALIGN:CENTER;'");
                                    dataStyle.Add("STYLE='FONT-SIZE:12;TEXT-ALIGN:LEFT;'");

                                    returnList.Add(headerRow);
                                    returnList.Add(dataStyle);
                                    foreach (ArrayList row in failList)
                                        returnList.Add(row);

                                    returnStatus = 0;       // Partially import
                                }
                            }
                    }
                    else
                    {   // file or worksheet not found
                        returnList = null;
                        returnStatus = -2;
                    }
                }
                else
                {   // Invalid path
                    returnList = null;
                    returnStatus = -3;
                }
            }
            catch (SystemException e)
            {
                throw (e);
            }
            return returnStatus;
        }
        private string removeControlCharacter(string str)
        {
            string outString = string.Empty;
            foreach (char ch in str)
                if (!char.IsControl(ch))
                    // No-Break Space (ASCII 160)-> normal Space (ASCII 32)
                    outString += (ch == Convert.ToChar(160) ? ' ' : ch);
            return outString;
        }

        public ArrayList importSupplierInvoiceNoRecord(object[] cells, int userId) //, ArrayList officeCodeList)
        {   // Update Invoice.SupplierInvoiceNo & Shipping Doc Receipt Date and add an action log
            // Cells Seq. : NSL InvoiceNo, Supplier InvoiceNo., Shipping Doc Receipt Date(optional)
            // Return : return array list of string; the string format is:
            //          1 st element    - <row status> | <status detail>
            //          other elements  - <column status> | <status detail>
            //
            object[] invoiceNo;
            InvoiceDef inv;
            ArrayList importStatus = new ArrayList();
            int emptyCount = 0;
            try
            {
                importStatus.Add("");   // Row status
                bool isBlankLine = true;
                for (int i = 0; i < cells.Length; i++)
                    if (cells[0].ToString().Trim() != string.Empty)
                        isBlankLine = false;
                if (isBlankLine)
                    importStatus[0] = "SKIP|Blank Line";
                else
                {
                    DateTime importDate;
                    string supInvNo = removeControlCharacter(string.IsNullOrEmpty(cells[1].ToString()) ? "" : cells[1].ToString());
                    if (cells.Length < 3)
                        importDate = DateTime.MinValue;
                    else
                        //The default format of the date field we get from Excel is M/d/yyyy
                        DateTime.TryParseExact(removeControlCharacter(cells[2].ToString()), "M/d/yyyy", CultureInfo.CreateSpecificCulture("en-US"), System.Globalization.DateTimeStyles.None, out importDate);

                    if ((invoiceNo = worker.getInvoiceNoSegment(removeControlCharacter(cells[0].ToString()))) != null)
                    {
                        if ((inv = ShippingWorker.Instance.getInvoiceByInvoiceNo(invoiceNo[0].ToString(), (int)invoiceNo[1], (int)invoiceNo[2])) != null)
                        {
                            ArrayList splitShipment = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(inv.ShipmentId);
                            bool isSplit = false;
                            if (splitShipment != null)
                                foreach (SplitShipmentDef spt in splitShipment)
                                    isSplit |= (spt.IsVirtualSetSplit == 0);

                            if (!isSplit)
                            {
                                string log = string.Empty;
                                int importCount = 0;

                                // Supplier Invoice No
                                if (string.IsNullOrEmpty(supInvNo))
                                {   // Add column status
                                    importStatus.Add("WARNING|Undefine Supplier Invoice No");
                                    emptyCount++;
                                }
                                else
                                    if (!string.IsNullOrEmpty(inv.SupplierInvoiceNo))
                                        importStatus.Add("WARNING|Supplier Invoice No. is already exist");    //importStatus = -2;   // Supplier Invoice No is already exist, no update
                                    else
                                    {
                                        log = "Upload Supplier Invoice No. : " + (string.IsNullOrEmpty(inv.SupplierInvoiceNo) ? "" : inv.SupplierInvoiceNo)
                                                    + " -> " + supInvNo;
                                        AddUploadLog(inv.ShipmentId, AmendmentType.SUPPLIER_INVOICE_NO, log, userId);
                                        inv.SupplierInvoiceNo = supInvNo;   // cells[1].ToString();
                                        importCount++;
                                    }

                                if (cells.Length >= 3)
                                {   // Shipping Doc Receipt Date
                                    if (removeControlCharacter(cells[2].ToString()).Trim() == "")
                                    {   // Do not show warning if Shipping Document Receipt Date is empty but Supplier Invoice No. is not empty
                                        //if (emptyCount > 0)
                                        //    importStatus.Add("WARNING|Undefine Shipping Document Receipt Date");
                                    }
                                    else
                                        if (inv.ShippingDocReceiptDate != DateTime.MinValue)
                                            importStatus.Add("WARNING|Shipping Document Receipt Date is already exist");
                                        else
                                            if (importDate == DateTime.MinValue)
                                                importStatus.Add("WARNING|Invalid Shipping Document Receipt Date");
                                            else
                                            {
                                                log = "Upload Shipping Doc. Receipt Date : " + DateTimeUtility.getDateString(inv.ShippingDocReceiptDate)
                                                            + " -> " + (DateTimeUtility.getDateString(importDate));
                                                AddUploadLog(inv.ShipmentId, AmendmentType.SHIPPING_DOCUMENT_RECEIPT_DATE, log, userId);
                                                inv.ShippingDocReceiptDate = importDate;
                                                importCount++;
                                            }
                                }
                                if (importCount > 0)
                                    ShippingWorker.Instance.updateInvoice(inv, null, null, userId);
                                importStatus[0] = (importStatus.Count > 1 ? "WARNING" : "SUCCESS");
                            }
                            else
                                importStatus[0] = "ERROR|It is a split shipment. Please update through ISAM";
                        }
                        else
                            importStatus[0] = "ERROR|Invoice cannot be found";
                    }
                    else
                        importStatus[0] = "INVALID_KEY|Invalid Invoice No.";
                }
            }
            catch (SystemException e)
            {
                throw (e);
            }
            return importStatus;
        }

        #endregion

        #region Import Daily Booking Files
        //---------------------------------------------------------------------------------------------------------------------
        const string shipmentBookingMailBox = "Shipment_Booking@NextSL.com.hk";
        const string shipmentBookingAccountName = "APP-ISAM";
        const string shipmentBookingAccountPassword = "Work4ever";
        const string shipmentBookingDomain = "NEXTDOMAIN";
        const int maxDataColumn = 13;
        const int colPort = 1, colContract = 3, colDlyNo = 5, colBookQty = 7, colBookDate = 8, colBookWhDate = 9, colSoNo = 10, colShipMode = 11, colCustomerCode = 12, colUpdateType = 13;
        const Excel.XlRgbColor colorInvalid = Excel.XlRgbColor.rgbRed;
        const Excel.XlRgbColor colorFieldDiscrepancy = Excel.XlRgbColor.rgbLawnGreen;
        const Excel.XlRgbColor colorFieldFormatError = Excel.XlRgbColor.rgbOrange;
        const Excel.XlRgbColor colorShipmentCancelled = Excel.XlRgbColor.rgbLightGray;
        const Excel.XlRgbColor colorShipmentInvoiced = Excel.XlRgbColor.rgbLightSkyBlue;
        const Excel.XlRgbColor colorShipmentWithBooking = Excel.XlRgbColor.rgbYellow;
        const Excel.XlRgbColor colorShipmentNotFound = Excel.XlRgbColor.rgbLawnGreen;
        string dailyBookingUploadFolder;
        ArrayList nextSourcingEMailDomains = null;

        public void importDailyBookingFiles()
        {
            dailyBookingUploadFolder = WebConfig.getValue("appSettings", "DAILY_BOOKING_UPLOAD_FOLDER");
            if (dailyBookingUploadFolder.EndsWith("\\"))
                dailyBookingUploadFolder = dailyBookingUploadFolder.Substring(0, dailyBookingUploadFolder.Length - 1);
            if (!string.IsNullOrEmpty(dailyBookingUploadFolder))
            {
                uploadDailyBookingFile();
                mailBoxHouseKeeping();
                attachmentFolderHouseKeeping();
            }
        }

        private void attachmentFolderHouseKeeping()
        {
            if (Directory.Exists(dailyBookingUploadFolder))
            {
                DirectoryInfo folder;
                string[] folderNames = Directory.GetDirectories(dailyBookingUploadFolder, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                if (folderNames != null)
                    foreach (string name in folderNames)
                    {
                        folder = new DirectoryInfo(name);
                        if ((DateTime.Now - folder.CreationTime).TotalDays > 14)
                            folder.Delete(true);
                    }
            }
        }

        private void uploadDailyBookingFile()
        {
            requestService();
            Shell32.FolderItem item = null;
            string action = "uploadDailyBookingFile()";
            try
            {
                action += formatAction("Getting eMail ");
                List<EmailMessage> list = getIncomingDailyBookingMessage();
                int eMailCount = 0;
                string timeString = DateTime.Now.ToString("yyyyMMddHHmmss");
                Folder backupFolder = getMailBoxFolder("Backup");
                foreach (EmailMessage eMail in list)
                {
                    Console.WriteLine(eMail.Subject);
                    action += formatAction("Loading Mail '" + eMail.Subject + "'");
                    eMail.Load();
                    eMailCount++;
                    if (!Directory.Exists(dailyBookingUploadFolder))
                    {
                        Directory.CreateDirectory(dailyBookingUploadFolder);
                    }

                    string messageFolder;
                    int c = 0;
                    while (Directory.Exists(messageFolder = dailyBookingUploadFolder + "\\" + timeString + "_" + eMailCount.ToString() + (c == 0 ? "" : "_" + c.ToString())))
                        c++;
                    Directory.CreateDirectory(messageFolder);

                    List<string> excelFileList = new List<string>();
                    int attachmentCount = 1;
                    foreach (FileAttachment attachment in eMail.Attachments)
                    {
                        action += formatAction("Loading attachment '" + attachment.Name + "'");
                        Console.WriteLine(attachment.Name);
                        string attachmentFolder;
                        while (Directory.Exists(attachmentFolder = messageFolder + "\\" + attachmentCount.ToString()))
                            attachmentCount++;
                        Directory.CreateDirectory(attachmentFolder);
                        attachment.Load(attachmentFolder + "\\" + attachment.Name);
                        //Shell32.FolderItems extractedFolder = null;

                        //if (att.ContentType == MediaTypeNames.Application.Zip)
                        string fileExtension = (attachment.Name.Contains(".") ? attachment.Name.Substring(attachment.Name.LastIndexOf('.') + 1) : string.Empty).Trim();
                        string fileName = (attachment.Name.Contains(".") ? attachment.Name.Substring(0, attachment.Name.LastIndexOf('.')) : attachment.Name);   //.Trim();
                        if (fileExtension.ToLower() == "zip")
                        {   // zip file
                            action += formatAction("extracting zip '" + attachment.FileName + "'");
                            excelFileList = FileUploadManager.Instance.extractFileFromZip(attachment.FileName, "Excel");
                        }
                        else if (fileExtension.ToLower() == "xls" || fileExtension.ToLower() == "xlsx")
                        {   // Excel files .xls or .xlsx
                            action += formatAction("add to Excel file list '" + fileName + "'");
                            item = FileUploadManager.Instance.getFolderItem(attachmentFolder, fileName + '.' + fileExtension);
                            if (item != null)
                                excelFileList.Add(item.Path);
                            releaseObject(item);
                            item = null;
                        }
                        else if (fileExtension.ToLower() == "jpg" || fileExtension.ToLower() == "gif" || fileExtension.ToLower() == "png" || fileExtension.ToLower() == "tiff" || fileExtension.ToLower() == "bmp")
                        {   // ignore the image file
                            action += formatAction("ignore the image file '" + fileName + "'");
                        }
                        else
                            MailHelper.sendGeneralMessage("Fail to Upload Daily Booking files",
                                "eMail Subject : " + eMail.Subject + "\n<br>"
                                + "Receive Time : " + eMail.DateTimeReceived.ToString() + "\n<br>"
                                + "Failure : Fail to upload '" + attachment.Name + "'\n<br>"
                                + "Reason : System does not support file type '" + fileExtension.ToLower() + "'\n");

                        //uploadBookingExcelFiles(replyExcelFileNameList);
                    }
                    action += formatAction("Calling uploadBookingExcelFiles()");
                    ArrayList uploadedExcelFileList = uploadBookingExcelFiles(excelFileList);
                    if (uploadedExcelFileList.Count > 0)
                    {
                        action += formatAction("replying eMail");
                        replyDailyBookingMailToNSUser(eMail, uploadedExcelFileList);
                    }
                    action += formatAction("completed");
                    eMail.IsRead = true;
                    if (backupFolder != null)
                        eMail.Move(backupFolder.Id);
                }
                sendDebugMsg(action, "uploadDailBookingFile");
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "uploadDailBookingFile : Fail in " + action);
                throw e;
            }
            finally
            {
                if (item != null)
                    releaseObject(item);
            }
        }

        private List<string> getShipmentBookingGroupEMailList()
        {
            
            List<string> emailList = new List<string>();
            emailList.Add(shipmentBookingMailBox.ToLower());
            SystemParameterRef sysPara = CommonWorker.Instance.getSystemParameterByName("HK_SHIPPING_EMAIL_COMPACT");
            if (sysPara != null)
            {
                string val = sysPara.ParameterValue.Replace(" ", "").ToLower().Replace("@nextsl.com.hk", "@nextsl.com.hk;");
                foreach (string addr in val.Split(';'))
                    emailList.Add(addr);
            }
            return emailList;
        }

        private void replyDailyBookingMailToNSUser(EmailMessage eMail, ArrayList attachmentList)
        {
            string action = "replyDailyBookingMailToNSUser()";
            string replyTo = string.Empty;
            string toRecipients = string.Empty;
            string ccRecipients = string.Empty;
            string recipient = string.Empty;
            string address = string.Empty;
            List<EmailAddress> allRecipients = new List<EmailAddress>();
            action += formatAction("getShipmentBookingGroupEMailList()");
            List<string> ShipmentBookingEmailList = getShipmentBookingGroupEMailList();

            // Reply Recipients
            allRecipients.AddRange(eMail.ToRecipients);
            allRecipients.AddRange(eMail.CcRecipients);
            foreach (EmailAddress mailAddr in allRecipients)
            {
                address = mailAddr.Address.Trim();
                address = (address.EndsWith(('"').ToString()) && address.StartsWith(('"').ToString()) ? address.Substring(1, address.Length - 2) : address);
                address = (address.EndsWith("'") && address.StartsWith("'") ? address.Substring(1, address.Length - 2) : address);
                recipient = (string.IsNullOrEmpty(mailAddr.Name) ? address : mailAddr.Name);
                if (eMail.ToRecipients.Contains(mailAddr))
                    toRecipients += (toRecipients == string.Empty ? "" : "; ") + recipient;
                else if (eMail.CcRecipients.Contains(mailAddr))
                    ccRecipients += (ccRecipients == string.Empty ? "" : "; ") + recipient;

                if (isNSEMailAddress(mailAddr) && !ShipmentBookingEmailList.Contains(address.ToLower()) && !(";" + replyTo.Replace(" ", ";").ToLower() + ";").Contains(";" + address.ToLower() + ";"))
                    replyTo += (replyTo == string.Empty ? "" : "; ") + address;
            }
            //if (!replyTo.Contains("ShippingD@NextSL.com.hk"))
            //    replyTo += (replyTo == string.Empty ? "" : "; ") + "ShippingD@NextSL.com.hk";  // HK Shipping department
            action += formatAction("replyTo = '" + replyTo + "'");

            // Reply Content
            string mailBody = "<hr>";
            mailBody += "<b>From: </b>" + (string.IsNullOrEmpty(eMail.From.Name) ? eMail.From.Address : eMail.From.Name.ToString() + "[mail to:" + eMail.From.Address + "]") + "<br>";
            mailBody += "<b>Sent: </b>" + eMail.DateTimeSent.DayOfWeek.ToString() + ", " + eMail.DateTimeSent.ToString("MMMM dd, yyyy hh:mm:ss") + "<br>";
            mailBody += "<b>To: </b>" + toRecipients + "<br>";
            if (!string.IsNullOrEmpty(ccRecipients))
                mailBody += "<b>Cc: </b>" + ccRecipients + "<br>";
            mailBody += "<b>Subject: </b>" + eMail.Subject + "<br><br>";
            mailBody += extractMailBodyText(eMail);
            action += formatAction("mailBody = '" + mailBody + "'");
            string subject = "RE: " + ("FW:RE:".Contains(eMail.Subject.Substring(0, 3)) ? eMail.Subject.Substring(3).Trim() : eMail.Subject.Trim());
            action += formatAction("subject = '" + subject + "'");

            NoticeHelper.sendDailyBookingUploadMail(subject, mailBody, replyTo, shipmentBookingMailBox, attachmentList);
            action += formatAction("return");
            sendDebugMsg(action, "replyDailBookingMailToNSUser");
        }

        private ArrayList getNextSourcingEMailDomains()
        {
            SystemParameterRef para = CommonWorker.Instance.getSystemParameterByName("NS_EMAIL_DOMAIN");
            ArrayList nsEMailDomains = new ArrayList();
            if (para != null)
            {
                Array domainNameList = para.ParameterValue.Replace(" ", "").Replace(",", ";").ToLower().Split(';');
                foreach (string domainName in domainNameList)
                    if (!string.IsNullOrEmpty(domainName))
                        nsEMailDomains.Add(domainName.Trim());
            }
            else
                nsEMailDomains.Add("nextsl");
            return nsEMailDomains;
        }

        private bool isNSEMailAddress(EmailAddress eMail)
        {
            bool isNSEmailAddress = false;
            string eMailAddress = eMail.Address;

            if (nextSourcingEMailDomains == null)
                nextSourcingEMailDomains = getNextSourcingEMailDomains();
            if (!string.IsNullOrEmpty(eMailAddress))
                if (eMailAddress.Contains("@"))
                {
                    string domain = eMailAddress.Substring(eMailAddress.IndexOf("@")).ToLower(); ;
                    foreach (string nsDomain in nextSourcingEMailDomains)
                        if (domain.Contains("@" + nsDomain + ".") || domain.Contains("." + nsDomain + '.'))
                        {
                            isNSEmailAddress = true;
                            break;
                        }
                }
            return isNSEmailAddress;
        }

        private void sendDebugMsg(string msg)
        {
            sendDebugMsg(msg, string.Empty);
        }

        private void sendDebugMsg(string msg, string functionName)
        {
            // Send Debug message if we defined in SystemParameter
            bool enabled = false;
            List<SystemParameterRef> debugItem = CommonWorker.Instance.getSystemParametersByName("SEND_DEBUG_MSG");
            if (debugItem.Count > 0)
            {
                enabled = (string.IsNullOrEmpty(functionName));
                foreach (SystemParameterRef item in debugItem)
                    if (!enabled && item.ParameterValue == functionName)
                        enabled = true;
            }
            if (enabled)
                MailHelper.sendGeneralMessage("ISAM - Debug Message" + (string.IsNullOrEmpty(functionName) ? string.Empty : " : " + functionName), msg);
            return;
        }

        private string formatAction(string msg)
        {
            return "<br> - " + DateTime.Now.ToString() + " : " + msg;
        }

        private ArrayList uploadBookingExcelFiles(List<string> excelFileList)
        {
            string action = "uploadBookingExcelFiles()";
            action += formatAction("Creating Excel application object");
            Excel.Application xlApp = new Excel.Application();
            //xlApp.Visible = false;
            ArrayList uploadFileList = new ArrayList();
            Excel.Workbook xlWorkBook = null;
            string uploadedFile = string.Empty;
            string uploadingFile = string.Empty;
            try
            {
                action += formatAction("Including " + excelFileList.Count.ToString() + " Excel file");
                if (excelFileList != null)
                {
                    xlApp.Visible = true;
                    xlApp.DisplayAlerts = false;
                    string name, extension, uploadedFolder;
                    foreach (string excelPath in excelFileList)
                    {
                        action += formatAction("handling Excel File '" + excelPath + "'");
                        uploadingFile = excelPath;
                        extension = excelPath.Substring(excelPath.LastIndexOf('.'));
                        name = excelPath.Substring(excelPath.LastIndexOf("\\") + 1).Replace(extension, string.Empty);
                        uploadedFolder = excelPath.Substring(0, excelPath.LastIndexOf("\\")) + "\\Uploaded";
                        
                        if (!Directory.Exists(uploadedFolder))
                            Directory.CreateDirectory(uploadedFolder);
                        uploadedFile = uploadedFolder + "\\" + name + extension;

                        xlApp.FileValidation = MsoFileValidationMode.msoFileValidationSkip; // Skip the validation while opening the Excel file
                        xlApp.AutomationSecurity = MsoAutomationSecurity. msoAutomationSecurityForceDisable;    // Force to disable the macro in Excel
                        action += formatAction("Opening work book '" + uploadingFile + "'");
                        xlWorkBook = xlApp.Workbooks.Open(excelPath, 0, false, Type.Missing, Type.Missing, Type.Missing, true, Excel.XlPlatform.xlWindows, Type.Missing, true, false, 0, false, false, Type.Missing);

                        if (xlWorkBook != null)
                        {
                            action += formatAction("Uploading '" + uploadingFile + "'");
                            string uploadStatus = uploadBookingWorkbook(xlWorkBook);
                            if (uploadStatus != string.Empty)
                                action += formatAction("Fail in uploading '" + uploadedFile + "' : " + uploadStatus);
                            action += formatAction("Saving as '" + uploadedFile + "'");
                            xlWorkBook.SaveAs(uploadedFile);
                            action += formatAction("Closing '" + uploadingFile + "'");
                            xlWorkBook.Close();

                            uploadFileList.Add(uploadedFile);
                            action += formatAction("Releasing '" + uploadingFile + "'");
                            releaseObject(xlWorkBook);
                            xlWorkBook = null;
                            action += formatAction("Completed '" + uploadingFile + "'");
                        }
                        else
                            NoticeHelper.sendGeneralMessage("UploadBookingExcelFiles - Cannot open file '" + uploadingFile + "'");
                    }
                }
                sendDebugMsg(action, "uploadBookingExcelFiles");
                return uploadFileList;
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "UploadBookingExcelFiles - Fail in " + action);
                throw e;
            }
            finally
            {
                if (xlWorkBook != null)
                {
                    xlWorkBook.Close();
                    releaseObject(xlWorkBook);
                }
                if (xlApp != null)
                    releaseObject(xlApp);
            }
        }

        private string uploadBookingWorkbook(Excel.Workbook workbook)
        {
            List<int> mandatoryColumnNos = new List<int> { colContract, colDlyNo };
            List<int> uploadColumnNos = new List<int> { colBookDate, colBookQty, colBookWhDate, colSoNo };
            Excel.Worksheet ws = null;
            Excel.Sheets xlWorkSheets = null;
            string worksheetType = string.Empty;
            string status = string.Empty;
            int updateCount = 0;

            if (workbook != null)
                xlWorkSheets = workbook.Worksheets;
            try
            {
                if (xlWorkSheets != null)
                {
                    for (int sheetNo = 1; sheetNo <= xlWorkSheets.Count; sheetNo++)
                    {
                        worksheetType = string.Empty;
                        ws = (Excel.Worksheet)xlWorkSheets.Item[sheetNo];
                        if (ws.Name.ToUpper().Contains("DAILY") || ws.Name.ToUpper().Contains("NEW BOOKING"))
                            worksheetType = "DAILY";
                        else
                            if (ws.Name.ToUpper().Contains("REVISE"))
                                worksheetType = "REVISE";

                        if (worksheetType != string.Empty)
                        {
                            int ignoreRowLimit = 30;
                            for (int r = 1, headerRow = 0, ignoreRowCount = 0; r <= ws.Rows.Count && ignoreRowCount < ignoreRowLimit; r++)
                            {
                                ignoreRowCount++;
                                Excel.Range dataRow = ws.get_Range(ws.Cells[r, 1], ws.Cells[r, maxDataColumn]);
                                if (headerRow == 0)
                                {
                                    //Detecting Header Row
                                    if (ws.Cells[r, colPort] != null && ws.Cells[r, colContract] != null && ws.Cells[r, colDlyNo] != null)
                                        if (((Excel.Range)ws.Cells[r, colPort]).Value != null && ((Excel.Range)ws.Cells[r, colContract]).Value != null && ((Excel.Range)ws.Cells[r, colDlyNo]).Value != null)
                                        {
                                            string port = ((Excel.Range)ws.Cells[r, colPort]).Value.ToString().ToUpper();
                                            string contractNo = ((Excel.Range)ws.Cells[r, colContract]).Value.ToString().ToUpper();
                                            string dlyNo = ((Excel.Range)ws.Cells[r, colDlyNo]).Value.ToString().ToUpper();
                                            if (port.Contains("PORT") && contractNo.Contains("CONTRACT") && (dlyNo.Contains("NO") && (dlyNo.Contains("DEL") || dlyNo.Contains("DLY"))))
                                                headerRow = r;
                                        }
                                }
                                else
                                {
                                    //Handling of the Data rows
                                    bool validRow = true, blankRow = true;
                                    foreach (Excel.Range cell in dataRow)
                                        if (cell.Column != maxDataColumn)
                                        {
                                            string val = (cell.Value == null ? string.Empty : cell.Value.ToString().Trim());
                                            blankRow &= (val == string.Empty);
                                            validRow &= !(mandatoryColumnNos.Contains(cell.Column) && (val == string.Empty));
                                        }

                                    if (!blankRow)
                                        if (validRow)
                                        {
                                            uploadBookingRecord(dataRow, worksheetType);
                                            updateCount++;
                                            ignoreRowCount = 0;
                                        }
                                        else
                                            dataRow.Interior.Color = colorInvalid;
                                }
                            }
                            status = (updateCount == 0 ? "Cannot find any booking record" : status);
                        }
                        else
                        {
                            status = "Cannot find the booking worksheet";
                        }
                    }
                    releaseObject(xlWorkSheets);
                    xlWorkSheets = null;
                }
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "UploadBookingWorkbook - Uploading " + (string.IsNullOrEmpty(workbook.Path) ? "NULL" : workbook.Path));
                throw e;
            }
            finally
            {
                if (xlWorkSheets != null)
                    releaseObject(xlWorkSheets);
            }
            return status;
        }

        private int uploadBookingRecord(Excel.Range dataRow, string worksheetType)
        {
            return uploadBookingRecord(dataRow, Type.Missing, worksheetType);
        }

        private int uploadBookingRecord(Excel.Range dataRow, object rowIndex, string worksheetType)
        {
            int updateCount = 0;
            int dlyNo = int.MinValue;
            int bookQty = int.MinValue;
            DateTime bookDate = DateTime.MinValue;
            DateTime bookWhDate = DateTime.MinValue;
            string bookSoNo = string.Empty;
            string shipMode = string.Empty, shipCode = string.Empty, shipType = string.Empty;
            string contractNo = string.Empty;
            string port = string.Empty;
            string actionType = string.Empty;  // New, Revise or Cancel
            string updateType = string.Empty;

            try
            {
                Excel.Range cellPort = (Excel.Range)dataRow.Cells[rowIndex, colPort];
                Excel.Range cellContractNo = (Excel.Range)dataRow.Cells[rowIndex, colContract];
                Excel.Range cellDlyNo = (Excel.Range)dataRow.Cells[rowIndex, colDlyNo];
                Excel.Range cellBookDate = (Excel.Range)dataRow.Cells[rowIndex, colBookDate];
                Excel.Range cellBookWhDate = (Excel.Range)dataRow.Cells[rowIndex, colBookWhDate];
                Excel.Range cellBookQty = (Excel.Range)dataRow.Cells[rowIndex, colBookQty];
                Excel.Range cellSoNo = (Excel.Range)dataRow.Cells[rowIndex, colSoNo];
                Excel.Range cellShipMode = (Excel.Range)dataRow.Cells[rowIndex, colShipMode];
                Excel.Range cellUpdateType = (Excel.Range)dataRow.Cells[rowIndex, colUpdateType];

                contractNo = getCellStringValue(cellContractNo);
                dlyNo = getCellIntValue(cellDlyNo);
                port = getCellStringValue(cellPort).ToUpper();
                bookQty = getCellIntValue(cellBookQty);
                bookDate = getCellDateTimeValue(cellBookDate);
                bookWhDate = getCellDateTimeValue(cellBookWhDate);
                bookSoNo = getCellStringValue(cellSoNo);
                shipMode = getCellStringValue(cellShipMode).ToUpper();
                updateType = getCellStringValue(cellUpdateType).ToUpper();

                bool toBeUpload = true;
                ShipmentDef shipment = null;
                InvoiceDef invoice = null;
                if (dlyNo <= 0 || contractNo == string.Empty)
                {
                    toBeUpload = false;
                    dataRow.Interior.Color = colorShipmentNotFound;
                }
                else
                    shipment = OrderSelectWorker.Instance.getShipmentByContractNoAndDlyNo(contractNo, dlyNo);

                if (shipment == null)
                {   // Missing Shipment
                    toBeUpload = false;
                    dataRow.Interior.Color = colorShipmentNotFound;
                }
                else
                {   // Check booking data discrepancy
                    bool forceToUploadForInvoicedShipment = false;
                    invoice = ShippingWorker.Instance.getInvoiceByKey(shipment.ShipmentId);
                    ContractWFS wfs = ContractWFS.getType(shipment.WorkflowStatus.Id);
                    bool isEmptyBookingInfo = (invoice.BookingQty == 0 && invoice.BookingDate == DateTime.MinValue && invoice.BookingAtWarehouseDate == DateTime.MinValue && string.IsNullOrEmpty(invoice.BookingSONo));
                    bool isSameBookingInfo = (invoice.BookingQty == bookQty && invoice.BookingDate == bookDate && invoice.BookingAtWarehouseDate == bookWhDate && invoice.BookingSONo.ToUpper() == bookSoNo.ToUpper());

                    // define worksheet row color if there is discrepancy
                    if (wfs == ContractWFS.CANCELLED)
                    {   // Cancelled shipment
                        toBeUpload = false;
                        dataRow.Interior.Color = colorShipmentCancelled;
                    }
                    else if (wfs == ContractWFS.INVOICED)
                    {   // Invoiced shipment
                        if (!isEmptyBookingInfo && !forceToUploadForInvoicedShipment)
                        {
                            toBeUpload = false;
                            dataRow.Interior.Color = colorShipmentInvoiced;
                        }
                    }
                    else if (isSameBookingInfo)
                    {   // No different in Booking data, no action
                        toBeUpload = false;
                    }
                    else if (!isEmptyBookingInfo)
                    {   // Booking info exists
                        toBeUpload = true;
                        dataRow.Interior.Color = colorShipmentWithBooking;
                    }


                    if (toBeUpload) // || isSameBookingInfo)
                    {
                        // Check cell discrepancy
                        if (shipment.ShipmentPort != null)
                            if (port != shipment.ShipmentPort.OfficialCode)
                                cellPort.Interior.Color = colorFieldDiscrepancy;    // Shipment Port Discrepancy

                        if (bookQty == int.MinValue)
                            cellBookQty.Interior.Color = colorFieldFormatError;
                        else
                            if (shipment.TotalOrderQuantity > (bookQty * 1.1) || shipment.TotalOrderQuantity < (bookQty * 0.9))
                                cellBookQty.Interior.Color = colorFieldDiscrepancy;     // Order Qty Discrepancy
                        if (bookDate == DateTime.MinValue)
                            cellBookDate.Interior.Color = colorFieldFormatError;    // Date format error
                        if (bookWhDate == DateTime.MinValue)
                            cellBookWhDate.Interior.Color = colorFieldFormatError;    // Date format error
                        if (string.IsNullOrEmpty(bookSoNo))
                            cellSoNo.Interior.Color = colorInvalid;
                        shipCode = string.Empty;
                        if (shipMode != null)
                        {
                            if (new List<string> { "A", "E", "X", "L", "S" }.Contains(shipMode)) shipCode = shipMode;
                            else if (shipMode.Contains("AIR")) shipCode = (shipMode.Contains("ECO") ? "E" : (shipMode.Contains("SEA") ? "X" : "A"));
                            else if (shipMode.Contains("SEA")) shipCode = "S";
                            else if (shipMode.Contains("LAND") || shipMode.Contains("TRUCK")) shipCode = "L";
                        }
                        if (shipment.ShipmentMethod != null)
                            if (shipCode != shipment.ShipmentMethod.OPSKey)
                                cellShipMode.Interior.Color = colorFieldDiscrepancy;    // Shipment Method Discrepancy

                        // Update Booking data and add action log
                        ArrayList amendmentList = new ArrayList();
                        if (worksheetType == "REVISE" && (updateType == string.Empty ? string.Empty : updateType.Substring(0, 1)) == "C")
                        {   // Revise by Cancel - clear booking data
                            if (invoice.BookingQty != 0)
                                amendmentList.Add(newUploadLog(invoice.ShipmentId, "Booking Quantity : " + invoice.BookingQty.ToString() + " -> 0"));
                            if (invoice.BookingDate != DateTime.MinValue)
                                amendmentList.Add(newUploadLog(invoice.ShipmentId, "Booking Date : " + invoice.BookingDate.ToString("dd/MM/yyyy") + " -> "));
                            if (invoice.BookingAtWarehouseDate != DateTime.MinValue)
                                amendmentList.Add(newUploadLog(invoice.ShipmentId, "Booking At-Warehouse Date : " + invoice.BookingAtWarehouseDate.ToString("dd/MM/yyyy") + " -> "));
                            if (!string.IsNullOrEmpty(invoice.BookingSONo))
                                amendmentList.Add(newUploadLog(invoice.ShipmentId, "Booking SO. No. : " + invoice.BookingSONo + " -> "));

                            invoice.BookingQty = 0;
                            invoice.BookingDate = DateTime.MinValue;
                            invoice.BookingAtWarehouseDate = DateTime.MinValue;
                            invoice.BookingSONo = null;
                        }
                        else
                        {   // Update booking data
                            if (invoice.BookingQty != bookQty)
                                amendmentList.Add(newUploadLog(invoice.ShipmentId, "Booking Quantity : " + invoice.BookingQty.ToString() + " -> " + bookQty.ToString()));
                            if (invoice.BookingDate != bookDate)
                                amendmentList.Add(newUploadLog(invoice.ShipmentId, "Booking Date : " + (invoice.BookingDate == DateTime.MinValue ? "" : invoice.BookingDate.ToString("dd/MM/yyyy")) + " -> " + bookDate.ToString("dd/MM/yyyy")));
                            if (invoice.BookingAtWarehouseDate != bookWhDate)
                                amendmentList.Add(newUploadLog(invoice.ShipmentId, "Booking At-Warehouse Date : " + (invoice.BookingAtWarehouseDate == DateTime.MinValue ? "" : invoice.BookingAtWarehouseDate.ToString("dd/MM/yyyy")) + " -> " + bookWhDate.ToString("dd/MM/yyyy")));
                            if (invoice.BookingSONo != bookSoNo)
                                amendmentList.Add(newUploadLog(invoice.ShipmentId, "Booking SO. No. : " + invoice.BookingSONo + " -> " + bookSoNo));

                            invoice.BookingQty = bookQty;
                            invoice.BookingDate = bookDate;
                            invoice.BookingAtWarehouseDate = bookWhDate;
                            invoice.BookingSONo = bookSoNo;
                        }
                        ShippingWorker.Instance.updateInvoice(invoice, ActionHistoryType.SHIPPING_UPLOAD, amendmentList, adminUserId);
                        foreach (ActionHistoryDef def in amendmentList)
                        {
                            ShippingWorker.Instance.updateActionHistory(def);
                            updateCount++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "UploadBookingRecord - Uploading " + worksheetType + " (RowIndex = " + rowIndex.ToString() + "; UpdateCount = " + updateCount.ToString() + ") "); 
            }
            return updateCount;
        }

        private int getCellIntValue(object excelWorksheetCell)
        {
            decimal value = getCellDecimalValue(excelWorksheetCell);
            return (value == decimal.MinValue ? int.MinValue : Convert.ToInt32(value));
        }

        private decimal getCellDecimalValue(object excelWorksheetCell)
        {
            return (decimal)getCellValue(excelWorksheetCell, typeof(decimal));
        }

        private DateTime getCellDateTimeValue(object excelWorksheetCell)
        {
            return (DateTime)getCellValue(excelWorksheetCell, typeof(DateTime));
        }

        private string getCellStringValue(object excelWorksheetCell)
        {
            return ((string)getCellValue(excelWorksheetCell, typeof(string))).Trim();
        }

        private object getCellValue(object excelWorksheetCell)
        {
            Excel.Range cell = (Excel.Range)excelWorksheetCell;
            return ((cell != null && cell.Value != null) ? cell.Value.GetType() : typeof(string));
        }

        private object getCellValue(object excelWorksheetCell, Type dataType)
        {
            int intValue;
            decimal decimalValue;
            double doubleValue;
            DateTime dateTimeValue;
            object returnValue;
            Excel.Range cell = (Excel.Range)excelWorksheetCell;
            string text = (cell == null ? string.Empty : (cell.Text == null ? string.Empty : cell.Text.ToString()));
            string value = (cell == null ? string.Empty : (cell.Value == null ? string.Empty : cell.Value.ToString()));
            if (dataType == typeof(int))
                returnValue = (int.TryParse(value, out intValue) ? intValue : int.MinValue);
            else if (dataType == typeof(double))
                returnValue = (double.TryParse(value, out doubleValue) ? doubleValue : double.MinValue);
            else if (dataType == typeof(decimal))
                returnValue = (decimal.TryParse(value, out decimalValue) ? decimalValue : decimal.MinValue);
            else if (dataType == typeof(DateTime))
                returnValue = (DateTime.TryParse(value, out dateTimeValue) ? dateTimeValue : DateTime.MinValue);
            else
                returnValue = text;
            return returnValue;
        }

        #endregion

        /*
        #region EWS - Exchange Web Service Functions
        private const int defaultNoOfSearchItem = int.MaxValue;
        private ExchangeService exchangeService = null;

        private ExchangeService requestService()
        {
            if (exchangeService == null)
            {
                exchangeService = requestService(shipmentBookingAccountName, shipmentBookingAccountPassword, shipmentBookingDomain);
                exchangeService.AutodiscoverUrl(shipmentBookingMailBox);
            }
            return exchangeService;
        }

        private ExchangeService requestService(string loginName, string password, string domainName)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
            service.Credentials = new WebCredentials(loginName, password, domainName);
            // Setting credentials is unnecessary when you connect from a computer that is logged on to the domain.
            //service.UseDefaultCredentials = true;
            return service;
        }

        private ArrayList getIncomingDailyBookingMessage()
        {   // get all message from inbox that is not sent from NSSAdmin
            FindItemsResults<Item> findResults = getMailBoxItem(shipmentBookingMailBox, "Inbox", null);
            ArrayList mailMessages = new ArrayList();
            if (findResults != null)
                foreach (EmailMessage msg in findResults.Items)
                    if (!(msg.From.Name.ToLower().Contains("nss") && msg.From.Name.ToLower().Contains("admin")))
                        mailMessages.Add(msg);
            return mailMessages;
        }

        private ArrayList getNSSAdminDailyBookingMessage()
        {   // get all the daily booking message that reply by NSS Admin in the Inbox
            SearchFilter.SearchFilterCollection searchFilterCollection = new SearchFilter.SearchFilterCollection(LogicalOperator.And);
            searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.ContainsSubstring(EmailMessageSchema.From, "NSSAdmin")));
            searchFilterCollection.Add(new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.ContainsSubstring(EmailMessageSchema.Body, shipmentBookingMailBox)));
            FindItemsResults<Item> findResults = getMailBoxItem(shipmentBookingMailBox, "Inbox", searchFilterCollection);
            ArrayList mailMessages = new ArrayList();
            if (findResults != null)
                foreach (EmailMessage msg in findResults.Items)
                    mailMessages.Add(msg);
            return mailMessages;
        }

        private FindItemsResults<Item> getMailBoxItem(string eMailAddress, string folderName, SearchFilter.SearchFilterCollection filterCollection)
        {
            return getMailBoxItem(eMailAddress, folderName, filterCollection, defaultNoOfSearchItem);
        }

        private FindItemsResults<Item> getMailBoxItem(string eMailAddress, string folderName, SearchFilter.SearchFilterCollection filterCollection, int noOfItems)
        {
            ItemView view = new ItemView(noOfItems);
            FolderView fView = new FolderView(noOfItems);
            FindItemsResults<Item> findResults = null;

            ExchangeService service = requestService();
            service.AutodiscoverUrl(eMailAddress);
            fView.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            fView.PropertySet.Add(FolderSchema.DisplayName);
            fView.Traversal = FolderTraversal.Deep;
            FindFoldersResults mailBox;
            if (string.IsNullOrEmpty(folderName))
                mailBox = service.FindFolders(new FolderId(WellKnownFolderName.Inbox, new Mailbox(eMailAddress)), fView);   // get the Inbox content by default
            else
                mailBox = service.FindFolders(new FolderId(WellKnownFolderName.Root, new Mailbox(eMailAddress)), new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName.Trim()), fView);
            if (mailBox.Folders.Count > 0)
                findResults = (filterCollection == null ? mailBox.Folders[0].FindItems(view) : mailBox.Folders[0].FindItems(filterCollection, view));
            return findResults;
        }

        private Folder getMailBoxFolder(string folderName)
        {
            FolderView fView = new FolderView(defaultNoOfSearchItem);
            ExchangeService service = requestService();
            fView.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            fView.PropertySet.Add(FolderSchema.DisplayName);
            fView.Traversal = FolderTraversal.Deep;
            FindFoldersResults result = service.FindFolders(new FolderId(WellKnownFolderName.Root, new Mailbox(shipmentBookingMailBox)), new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName), fView);
            return (result.Folders.Count > 0 ? result.Folders[0] : null);
        }

        private string extractMailBodyText(EmailMessage eMail)
        {
            string mailBody = eMail.Body.Text;
            string body = mailBody.ToLower();
            int start = -1, end = -1;
            if (body.Contains("<html>") || body.Contains("<html "))
            {
                end = body.LastIndexOf("</body>") - 1;
                start = (body.IndexOf("<body>"));
                if (start < 0)
                    start = body.IndexOf("<body ");
                if (start > 0 && end > start)
                    start = body.IndexOf(">", start) + 1;
                else
                    start = -1;
            }
            if (start < 0)
                body = mailBody;
            else
                body = mailBody.Substring(start, end - start + 1).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("<o:p></o:p>", string.Empty).Replace("<o:p>&nbsp;</o:p>", string.Empty);
            return body;
        }

        private void mailBoxHouseKeeping()
        {
            ExchangeService service = requestService();
            try
            {
                ArrayList list = getNSSAdminDailyBookingMessage();
                Folder finishedFolder = getMailBoxFolder("Finished");
                if (finishedFolder != null)
                    foreach (EmailMessage eMail in list)
                        eMail.Move(finishedFolder.Id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion
        */

        #region Mail Box functions (thru Exchange Web Service)
        private void requestService()
        {
            mailboxHelper.requestMailBoxService(shipmentBookingAccountName, shipmentBookingAccountPassword, shipmentBookingMailBox);
        }

        private List<EmailMessage> getIncomingDailyBookingMessage()
        {   // get all message from inbox that is not sent from NSSAdmin
            List<EmailMessage> inbox = mailboxHelper.getMailHeader("Inbox");
            return inbox.FindAll(x => x.Sender.Name.IndexOf("NSS Admin") < 0);
        }

        private List<EmailMessage> getNSSAdminDailyBookingMessage()
        {   // get all the daily booking message which is replied by NSS Admin in the inbox folder
            return mailboxHelper.getMailHeader("Inbox", GeneralCriteria.ALLSTRING, "NSS Admin", GeneralCriteria.ALLSTRING, shipmentBookingMailBox);
        }

        private Folder getMailBoxFolder(string folderName)
        {
            return mailboxHelper.getMailBoxFolder(folderName);
        }

        private string extractMailBodyText(EmailMessage eMail)
        {   
            return mailboxHelper.extractMailBodyText(eMail);
        }

        private void mailBoxHouseKeeping()
        {
            mailboxHelper.requestMailBoxService(shipmentBookingAccountName, shipmentBookingAccountPassword, shipmentBookingMailBox);
            mailboxHelper.moveMessage(getNSSAdminDailyBookingMessage(), "Finished");
        }

        /*
        private void replyDailyBookingMailToNSUser(EmailMessage msg, ArrayList attachmentList)
        {
            // Reply thru Exchange Web Service
            ResponseMessage reply = msg.CreateReply(false);
            //draftMsg.ToRecipients.Add(new EmailAddress("Cliff Wong", "cliff_wong@nextsl.com.hk", "SMTP"));
            //list<EmailAddress> address = extractNextSourcingEMail(draftMsg.ToRecipients.);
            //List<EmailAddress> l = extractNSEMailList("<Cliff Wong>cliff_wong@nextsl.com.hk; toby_lo@nextsl.com.hk, <Cindy> Cindy_Li@sh.nextsl.com; anton@nexts.com.lk;");
            string mailTo = string.Empty;
            foreach (EmailAddress addr in msg.ToRecipients)
                if (isNSEMailAddress(addr))
                    reply.ToRecipients.Add(addr);
                //mailTo += (mailTo == string.Empty ? "" : "; ") + addr.ToString();
            foreach (EmailAddress addr in msg.CcRecipients)
                if (isNSEMailAddress(addr))
                    reply.CcRecipients.Add(addr);
                //ccTo += (ccTo == string.Empty ? "" : "; ") + addr.ToString();
            //List<EmailAddress> l2 = extractNSEMailList(msg.ToRecipients);
            EmailAddress em = new EmailAddress("<Cliff Wong>cliff_wong@nextsl.com.hk");

            //reply.Body = "TO : " + mailTo + "\nCC : " + ccTo;
            EmailMessage draftMsg = reply.Save(WellKnownFolderName.Drafts);

            //draftMsg.Attachments.Clear();
            //foreach (Shell32.FolderItem item in excelFileList)
            foreach (string filePath in attachmentList)
            {
                draftMsg.Attachments.AddFileAttachment(filePath);
                //draftMsg.Attachments.AddFileAttachment(item.Path);
                //draftMsg.Update(ConflictResolutionMode.AlwaysOverwrite);
            }
            draftMsg.ToRecipients.Add(new EmailAddress("Cliff Wong", "cliff_wong@nextsl.com.hk", "SMTP"));
            draftMsg.CcRecipients.Add(new EmailAddress("Shipment Booking", shipmentBookingMailBox, "SMTP"));
            draftMsg.From = new EmailAddress("NSS Admin", "nssadmin@nextsl.com.hk", "SMTP");    // Sending on Behalf of NSS Admin
            draftMsg.Update(ConflictResolutionMode.AlwaysOverwrite);
            draftMsg.Send();
            //reply.SendAndSaveCopy();
            //draftMsg.Delete(DeleteMode.MoveToDeletedItems);
        }
        */

        #endregion

        public static int ReadAllBytesFromStream(Stream stream, byte[] buffer)
        {
            // Use this method is used to read all bytes from a stream.
            int offset = 0;
            int totalCount = 0;
            int bytesRead = -1;
            while (bytesRead != 0)
            {
                bytesRead = stream.Read(buffer, offset, 100);
                offset += bytesRead;
                totalCount += bytesRead;
            }
            return totalCount;
        }

        private static bool CompareData(byte[] buf1, int len1, byte[] buf2, int len2)
        {
            // Use this method to compare data from two different buffers.
            if (len1 != len2)
            {
                Console.WriteLine("Number of bytes in two buffer are different {0}:{1}", len1, len2);
                return false;
            }

            for (int i = 0; i < len1; i++)
            {
                if (buf1[i] != buf2[i])
                {
                    Console.WriteLine("byte {0} is different {1}|{2}", i, buf1[i], buf2[i]);
                    return false;
                }
            }
            Console.WriteLine("All bytes compare.");
            return true;
        }

        #endregion

    }
}