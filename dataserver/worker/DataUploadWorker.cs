using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Xml;
using System.IO;
using System.Text;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.isam.domain.account;
using com.next.isam.domain.common;
using com.next.isam.dataserver.model.account;
using com.next.isam.dataserver.model.common;


namespace com.next.isam.dataserver.worker
{
    public class UploadFileRef : DomainData
    {
        public UploadFileType FileType { get; set; }
        public string FilePrefix { get; set; }
        public string FileName { get; set; }
        public DateTime UploadTime { get; set; }
        public UserRef UploadUser { get; set; }
        public string ServerFileName
        {
            get { return ("[" + FilePrefix + "] " + FileName); }
        }
    }

    public class UploadFileType : DomainData
    {
        private TypeId _fileTypeId;
        public enum TypeId
        {
            Unknown = 0,
            DailyBooking = 1,
            SupplierInvoiceNo = 2,
            TallyInterfaceFile = 3,
            NSLedSales = 4
        }

        private UploadFileType(TypeId fileType)
        {
            this._fileTypeId = fileType;
        }


        public static UploadFileType SupplierInvoiceNo = new UploadFileType(TypeId.SupplierInvoiceNo);
        public static UploadFileType DailyBooking = new UploadFileType(TypeId.DailyBooking);
        public static UploadFileType TallyInterfaceFile = new UploadFileType(TypeId.TallyInterfaceFile);
        public static UploadFileType NSLedSales = new UploadFileType(TypeId.NSLedSales);
        public static UploadFileType Unknown = new UploadFileType(TypeId.Unknown);

        public int FileTypeId
        {
            get { return Convert.ToUInt16(_fileTypeId.GetHashCode()); }
        }

        public string Name
        {
            get
            {
                switch (_fileTypeId)
                {
                    case TypeId.SupplierInvoiceNo:
                        return "SupplierInvoiceNo";
                    case TypeId.DailyBooking:
                        return "DailyBooking";
                    case TypeId.TallyInterfaceFile:
                        return "TallyInterfaceFile";
                    case TypeId.NSLedSales:
                        return "NSLedSales";
                    case TypeId.Unknown:
                        return "Unknown";
                    default:
                        return "";
                }
            }
        }

        public static UploadFileType getFileType(int typeId)
        {
            if (typeId == TypeId.SupplierInvoiceNo.GetHashCode()) return SupplierInvoiceNo;
            else if (typeId == TypeId.DailyBooking.GetHashCode()) return DailyBooking;
            else if (typeId == TypeId.TallyInterfaceFile.GetHashCode()) return TallyInterfaceFile;
            else if (typeId == TypeId.NSLedSales.GetHashCode()) return NSLedSales;
            else if (typeId == TypeId.Unknown.GetHashCode()) return Unknown;
            else return null;
        }

        public static UploadFileType getFileType(string FileTypeName)
        {
            if (FileTypeName == TypeId.SupplierInvoiceNo.ToString()) return SupplierInvoiceNo;
            else if (FileTypeName == TypeId.DailyBooking.ToString()) return DailyBooking;
            else if (FileTypeName == TypeId.TallyInterfaceFile.ToString()) return TallyInterfaceFile;
            else if (FileTypeName == TypeId.NSLedSales.ToString()) return NSLedSales;
            else if (FileTypeName == TypeId.Unknown.ToString()) return Unknown;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(TypeId.SupplierInvoiceNo);
            list.Add(TypeId.DailyBooking);
            list.Add(TypeId.TallyInterfaceFile);
            list.Add(TypeId.NSLedSales);
            return list;
        }

        public static ArrayList getCollection()
        {
            ArrayList list = new ArrayList();
            list.Add(SupplierInvoiceNo);
            list.Add(DailyBooking);
            list.Add(TallyInterfaceFile);
            list.Add(NSLedSales);
            return list;
        }

    }

    public class ImportStatus : DomainData
    {
        private importStatusId _importStatusId;
        public enum importStatusId
        {
            Empty = 0,
            Success = 1,
            SuccessWithWarning = 2,
            InvalidFormat = 3,
            WorksheetNotFind = 4,
            InvalidFilePath = 5,
            NonSupportRecord = 6,
            UnexpectStatus = 7
        }

        private ImportStatus(importStatusId statusId)
        {
            this._importStatusId = statusId;
        }

        public static ImportStatus Empty = new ImportStatus(importStatusId.Empty);
        public static ImportStatus Success = new ImportStatus(importStatusId.Success);
        public static ImportStatus SuccessWithWarning = new ImportStatus(importStatusId.SuccessWithWarning);

        public int ImportStatusId
        {
            get { return Convert.ToUInt16(_importStatusId.GetHashCode()); }
        }

        public string Name
        {
            get
            {
                switch (_importStatusId)
                {
                    case importStatusId.Empty:
                        return "Empty";
                    case importStatusId.Success:
                        return "Success";
                    case importStatusId.SuccessWithWarning:
                        return "SuccessWithWarning";
                    default:
                        return "";
                }
            }
        }

        public static ImportStatus getImportStatus(int statusId)
        {
            if (statusId == importStatusId.Empty.GetHashCode()) return Empty;
            else if (statusId == importStatusId.Success.GetHashCode()) return Success;
            else if (statusId == importStatusId.SuccessWithWarning.GetHashCode()) return SuccessWithWarning;
            else return null;
        }

        public static ImportStatus getImportStatus(string statusName)
        {
            if (statusName == importStatusId.Empty.ToString()) return Empty;
            else if (statusName == importStatusId.Success.ToString()) return Success;
            else if (statusName == importStatusId.SuccessWithWarning.ToString()) return SuccessWithWarning;
            else return null;
        }
    }

    public class JournalTransaction : DomainData
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AllocationCode { get; set; }
        public string AccountingPeriod { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionDate { get; set; }
        public string Description { get; set; }
        public string ConversionCode { get; set; }
        public string OtherAmount { get; set; }
        public string BaseAmount { get; set; }
        public string JournalNo { get; set; }
        public string JournalLine { get; set; }
        public string OfficeCode { get; set; }
        public string DepartmentCode { get; set; }
        public string ProductCode { get; set; }
        public string StaffCode { get; set; }
        public string AreaCode { get; set; }
        public string SeasonCode { get; set; }
        public string Analysis { get; set; }
        public string ItemNo { get; set; }
        public string ReferenceNo { get; set; }
        public string JournalSource { get; set; }
        public string AssetCode { get; set; }
        public string RoughBook { get; set; }
        public string SortingKey { get; set; }

        public void getRawFromDataRow(DataRow dataRow)
        {
            getFromDataRow(dataRow, null);
        }

        public void getFromDataRow(DataRow dataRow, CultureInfo locale)
        {
            object[] array = dataRow.ItemArray;
            int maxIndex = array.Length - 1;
            if (maxIndex >= 0) this.AccountCode = array[0].ToString();
            if (maxIndex >= 1) this.AccountName = array[1].ToString();
            if (maxIndex >= 2) this.AllocationCode = array[2].ToString();
            if (maxIndex >= 3) this.AccountingPeriod = array[3].ToString();
            if (maxIndex >= 4) this.TransactionReference = array[4].ToString();
            if (maxIndex >= 5)
            {
                if (locale != null)
                {
                    DateTime date;
                    if (DateTime.TryParseExact(array[5].ToString(), "M/d/yyyy", locale, System.Globalization.DateTimeStyles.None, out date))
                        this.TransactionDate = date.ToString("yyyyMMdd");
                    else
                        this.TransactionDate = "";
                }
                else
                    this.TransactionDate = array[5].ToString();
            }
            if (maxIndex >= 6) this.Description = array[6].ToString();
            if (maxIndex >= 7) this.ConversionCode = array[7].ToString();
            if (maxIndex >= 8) this.OtherAmount = array[8].ToString();
            if (maxIndex >= 9) this.BaseAmount = array[9].ToString();
            if (maxIndex >= 10) this.JournalNo = array[10].ToString();
            if (maxIndex >= 11) this.JournalLine = array[11].ToString();
            if (maxIndex >= 12) this.OfficeCode = array[12].ToString();
            if (maxIndex >= 13) this.DepartmentCode = array[13].ToString();
            if (maxIndex >= 14) this.ProductCode = array[14].ToString();
            if (maxIndex >= 15) this.StaffCode = array[15].ToString();
            if (maxIndex >= 16) this.AreaCode = array[16].ToString();
            if (maxIndex >= 17) this.SeasonCode = array[17].ToString();
            if (maxIndex >= 18) this.Analysis = array[18].ToString();
            if (maxIndex >= 19) this.ItemNo = array[19].ToString();
            if (maxIndex >= 20) this.ReferenceNo = array[20].ToString();
            if (maxIndex >= 21) this.JournalSource = array[21].ToString();
            if (maxIndex >= 22) this.AssetCode = array[22].ToString();
            if (maxIndex >= 23) this.RoughBook = array[23].ToString();

            if (this.TransactionReference != null && this.JournalLine != null)
                this.SortingKey = this.TransactionReference + '-' + this.JournalNo.PadLeft(10, '0') + '-' + this.JournalLine.PadLeft(10, '0');
            else
                this.SortingKey = string.Empty;
        }

        public bool isVendorAccount()
        {
            int accountCode;
            if (int.TryParse(this.AccountCode, out accountCode))
                if (accountCode >= 1411701 && accountCode <= 1411999)
                    return true;
            return false;
        }
    }

    public class DataUploadWorker : Worker
    {
        public static DataUploadWorker _instance;

        public static DataUploadWorker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataUploadWorker();
                }
                return _instance;
            }
        }

        #region Basic Utilities

        private const int blankLineLimit = 100;

        protected string getUploadFileAuxName(int fileTypeId, int userId)
        {
            return UploadFileType.getFileType(fileTypeId).Name + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + userId.ToString();
        }

        public UploadFileRef generateNewUploadFileInfo(string clientFileName, int UploadFileTypeId, int UserId)
        {
            UploadFileRef info = new UploadFileRef();
            info.FileType = UploadFileType.getFileType(UploadFileTypeId);
            info.FileName = clientFileName.Trim();
            info.FilePrefix = getUploadFileAuxName(UploadFileTypeId, UserId);
            //info.ServerFileName = info.FilePrefix + " " + info.UploadFileName;
            info.UploadTime = DateTime.Now;
            info.UploadUser = GeneralWorker.Instance.getUserByKey(UserId);
            return info;
        }

        public string getIdentifiedUploadFileName(string fileFullPath)
        {
            string fileName = string.Empty;
            foreach (UploadFileType type in UploadFileType.getCollection())
            {
                string id = "[" + type.Name + "-";
                if (fileFullPath.Contains(id))
                {
                    fileName = fileFullPath.Substring(fileFullPath.IndexOf(id)).Trim();
                    break;
                }
            }
            return fileName;
        }

        public UploadFileRef getUploadFileInfo(string serverFileName)
        {
            string[] segment;
            int userId;
            UserRef uploader = null;
            DateTime uploadTime;
            string FileName;
            string Prefix;

            UploadFileRef info = new UploadFileRef();
            serverFileName = getIdentifiedUploadFileName(serverFileName);
            Prefix = serverFileName.Substring(1, serverFileName.IndexOf("]") - 1).Trim();
            FileName = serverFileName.Substring(serverFileName.IndexOf("]") + 1).Trim();
            segment = Prefix.Split(char.Parse("-"));
            if (segment.Length == 3)
            {
                string timeString = segment[1].Substring(0, 4) + '-' + segment[1].Substring(4, 2) + '-' + segment[1].Substring(6, 2) + " "
                    + segment[1].Substring(8, 2) + ':' + segment[1].Substring(10, 2) + ':' + segment[1].Substring(12, 2);
                info.FileType = UploadFileType.getFileType(segment[0]);
                info.FilePrefix = Prefix;
                info.FileName = FileName.Trim();
                info.UploadTime = (DateTime.TryParse(timeString, out uploadTime) ? uploadTime : DateTime.MinValue);
                if (int.TryParse(segment[2], out userId))
                    uploader = GeneralWorker.Instance.getUserByKey(userId);
                if (uploader == null)
                {
                    uploader = new UserRef();
                    uploader.UserId = 0;
                    uploader.DisplayName = "N/A";
                }
                info.UploadUser = uploader;
            }
            else
            {
                info.FileType = UploadFileType.Unknown;
                info.FileName = "";
                info.FilePrefix = "";
                info.UploadTime = DateTime.MinValue;
                info.UploadUser = null;
            }
            return info;
        }

        public string getUploadFileNamePattern(int fileTypeId)
        {
            return "[" + UploadFileType.getFileType(fileTypeId).Name + "-*-*]*.xls*";
        }

        public DataSet getWorksheet(string filePath, string worksheetName)
        {
            return getWorksheet(filePath, worksheetName, false);
        }

        public DataSet getWorksheet(string filePath, string worksheetName, bool firstRowIsHeader)
        {
            // firstRowIsHeader=true  => the first row in the worksheet is a header row, it will not be returned in the data row
            // firstRowIsHeader=false => the first row in the worksheet is not a header row, it will be returned in the data row
            DataSet ds = null;
            OleDbConnection conn = null;
            try
            {
                //String sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filePath + "; Extended Properties='Excel 8.0; HDR=" + (firstRowIsHeader?"YES":"NO") + "'";
                String sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + @filePath + ";Extended Properties='Excel 12.0; HDR=" + (firstRowIsHeader ? "YES" : "NO") + ";IMEX=1" + "'";
                conn = new OleDbConnection(sConnectionString);
                conn.Open();

                OleDbDataAdapter ad = new OleDbDataAdapter();
                string name = worksheetName;
                if (name.Contains("''") && name.Substring(0, 1) == "'" && name.Substring(name.Length - 1, 1) == "'")
                    name = name.Substring(1, name.Length - 2).Replace("''", "'");

                OleDbCommand cmdSelect = new OleDbCommand("SELECT * FROM [" + name + "]", conn);
                ad.SelectCommand = cmdSelect;

                ds = new DataSet();
                int recordAffected = ad.Fill(ds, "XLData");

                conn.Close();
                releaseObject(conn);
                conn = null;
            }
            catch (System.Data.OleDb.OleDbException e)
            {
                // Invalid file path or worksheet name
                ds = null;
                Console.WriteLine("ERROR " + e.Message);
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    releaseObject(conn);
                }
            }
            return ds;
        }

        public DataSet getFirstWorksheet(string filePath)
        {
            string worksheetName;
            return getFirstWorksheet(filePath, out worksheetName);
        }

        private DataSet getFirstWorksheet(string filePath, out string worksheetName)
        {   // Get the first non empty worksheet from the Excel file
            string name = string.Empty;
            DataSet ds = null;
            ArrayList nameList = getExcelWorksheetName(filePath);
            if (nameList != null)
                for (int i = 0; i < nameList.Count && ds == null; i++)
                {
                    if (isWorksheetEmpty(ds = getWorksheet(filePath, nameList[i].ToString())))
                        ds = null;
                    else
                        name = nameList[i].ToString();
                }
            worksheetName = name;
            return removeBlankRow(ds);
        }

        public ArrayList getExcelWorksheetName(string filePath)
        {
            ArrayList worksheets = null;
            OleDbConnection conn = null;
            try
            {
                //String sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filePath + "; Extended Properties='Excel 8.0;'";
                String sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 12.0;";
                conn = new OleDbConnection(sConnectionString);
                conn.Open();

                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt != null)
                {
                    worksheets = new ArrayList();
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = row["Table_Name"].ToString();
                        string type = row["Table_Type"].ToString();
                        if (!string.IsNullOrEmpty(name) && type == "TABLE")
                        {
                            string sheetName = string.Empty;
                            sheetName = name.Substring(0, name.IndexOf("$") + ((name.IndexOf("$'") >= 0) ? 2 : 1));
                            if (sheetName == name)
                                worksheets.Add(name);
                        }
                    }
                }
                conn.Close();
                releaseObject(conn);
                conn = null;
            }
            catch (System.Data.OleDb.OleDbException e)
            {
                // Invalid file path or worksheet name
                worksheets = null;
                Console.WriteLine("ERROR " + e.Message);
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    releaseObject(conn);
                }
            }
            return worksheets;
        }

        public bool isWorksheetArrayEmpty(ArrayList list)
        {
            bool isBlank = true;
            if (list != null)
                for (int r = 0; r < list.Count && r < blankLineLimit && isBlank; r++)
                    isBlank = isBlankRow((object[])list[r]);
            return isBlank;
        }

        public bool isWorksheetEmpty(DataSet worksheetDataSet)
        {
            bool isBlank = true;
            if (worksheetDataSet != null)
                for (int r = 0; r < worksheetDataSet.Tables[0].Rows.Count && r < blankLineLimit && isBlank; r++)
                    isBlank = isBlankRow(worksheetDataSet.Tables[0].Rows[r]);
            return isBlank;
        }

        public bool isBlankRow(object[] cells)
        {
            if (cells != null)
                for (int i = 0; i < cells.Length; i++)
                    if (!string.IsNullOrEmpty(cells[i].ToString()))
                        return false;
            return true;
        }

        public bool isBlankRow(DataRow row)
        {
            if (row != null)
                for (int i = 0; i < row.ItemArray.Length; i++)
                    if (!string.IsNullOrEmpty(row.ItemArray[i].ToString()))
                        return false;
            return true;
        }

        public DataSet removeBlankRow(DataSet worksheetDataSet)
        {
            if (worksheetDataSet != null)
            {
                DataSet ds = worksheetDataSet.Clone();
                int blankCount = 0;
                for (int r = 0; r < worksheetDataSet.Tables[0].Rows.Count && blankCount < blankLineLimit; r++)
                {
                    DataRow row = worksheetDataSet.Tables[0].Rows[r];
                    if (isBlankRow(row))
                        blankCount++;
                    else
                    {
                        ds.Tables[0].Rows.Add(row.ItemArray);
                        blankCount = 0;
                    }
                }
                return ds;
            }
            else
                return null;
        }

        private string xmlEncode(string value)
        {
            //return HttpUtility.HtmlEncode(value);
            //return XmlConvert.EncodeName(value);
            return System.Security.SecurityElement.Escape(value);
        }

        public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion


        #region OpenXML utilities
        public List<List<string>> getWorkSheet(string filePath, string worksheetName)
        {
            return com.next.infra.util.OpenXmlUtil.ReadExcel(filePath, worksheetName);
        }
        #endregion


        #region Supplier Invoice No Upload

        public object[] getInvoiceNoSegment(string invoiceNo)
        {
            string[] inv;

            inv = invoiceNo.Trim().Split(char.Parse("/"));
            if (inv.Length == 3)
            {
                int invSeq, invYear;
                string invPfx = inv[0].Trim().ToUpper();
                if (int.TryParse(inv[1], out invSeq) && int.TryParse(inv[2], out invYear))
                    if (invPfx.Length == 3 && invSeq > 0 && invSeq <= 99999 && invYear > 0 && invYear <= 9999)
                    {
                        object[] invoiceSegment = new object[3];
                        invoiceSegment[0] = invPfx;
                        invoiceSegment[1] = invSeq;
                        invoiceSegment[2] = invYear;
                        return invoiceSegment;
                    }
            }
            return null;
        }

        public ArrayList getAllSupplierInvoiceNoWorksheet(string filePath)
        {   // Get all the record from each unhidden worksheet in the Excel file
            ArrayList list = null;

            ArrayList sheetName = getExcelWorksheetName(filePath);
            if (sheetName != null)
            {
                list = new ArrayList();
                for (int i = 0; i < sheetName.Count; i++)
                {
                    DataSet ds;
                    if ((ds = getWorksheet(filePath, sheetName[i].ToString())) != null)
                    {
                        DataRowCollection rows = ds.Tables[0].Rows;
                        if (rows.Count > 0)
                        {
                            for (int j = (getInvoiceNoSegment(rows[0].ItemArray[0].ToString()) != null ? 0 : 1); j < rows.Count; j++)
                                list.Add(rows[j].ItemArray);
                        }
                    }
                }
                if (list.Count == 0)
                    list = null;
            }
            return list;
        }

        #endregion


        #region Tally Interface File

        public int getMaxTallyUploadSequenceNo()
        {
            IDataSetAdapter ad = getDataSetAdapter("TallyUploadHistoryApt", "GetMaxTallyUploadSequenceNo");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public string generateJournalXML(ArrayList journalList, string fileName, int noOfTransaction, int userId)
        {
            string xml = string.Empty;
            if (noOfTransaction > 0)
            {
                TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
                try
                {
                    int StartSequenceNo = -1;
                    ctx.Enter();
                    TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                    StartSequenceNo = getMaxTallyUploadSequenceNo() + 1;
                    TallyUploadHistoryDef def = new TallyUploadHistoryDef();
                    def.UploadFileName = fileName;
                    def.SequenceNoStart = StartSequenceNo;
                    def.SequenceNoEnd = StartSequenceNo + noOfTransaction - 1;
                    def.CreatedOn = DateTime.Now;
                    def.CreatedBy = userId;
                    updateTallyUploadHistory(def);
                    xml = convertToXML(journalList, fileName, noOfTransaction, StartSequenceNo, userId);
                    if (string.IsNullOrEmpty(xml))
                        ctx.VoteRollback();
                    else
                        ctx.VoteCommit();
                    return xml;
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR " + e.Message);
                    ctx.VoteRollback();
                    //throw e;
                    return null;
                }
                finally
                {
                    ctx.Exit();
                }
            }
            else
                return xml;
        }

        private string convertToXML(ArrayList journalList, string fileName, int noOfTransaction, int StartSeqNo, int userId)
        {
            string xml = string.Empty;
            Stream ms = new MemoryStream();
            using (XmlTextWriter w = new XmlTextWriter(ms, Encoding.UTF8))
            {
                w.WriteStartDocument();
                w.WriteWhitespace("\n");
                w.WriteComment("Next Sourcing Limited");
                w.WriteComment("Convert from SUN Account Interface file");
                w.WriteWhitespace("\n");
                w.WriteStartElement("ENVELOPE");
                w.WriteWhitespace("\n");
                w.WriteStartElement("HEADER");
                w.WriteElementString("TALLYREQUEST", "Import Data");
                w.WriteEndElement();

                w.WriteStartElement("BODY");
                w.WriteStartElement("IMPORTDATA");
                w.WriteStartElement("REQUESTDESC");

                w.WriteElementString("REPORTNAME", "Vouchers");

                w.WriteStartElement("STATICVARIABLES");
                w.WriteElementString("SVCURRENTCOMPANY", "Next Sourcing Services (India) Pvt.Ltd.(2009-10)");
                w.WriteEndElement();

                w.WriteStartElement("REQUESTDATA");

                int SequenceNo = StartSeqNo;
                for (int i = 0; i < journalList.Count; i++)
                {
                    JournalTransaction lastTransaction = (i > 0 ? (JournalTransaction)journalList[i - 1] : null);
                    JournalTransaction thisTransaction = (JournalTransaction)journalList[i];
                    JournalTransaction nextTransaction = (i < journalList.Count - 1 ? (JournalTransaction)journalList[i + 1] : null);

                    bool newTransaction = false;
                    if (lastTransaction == null)
                        newTransaction = true;
                    else
                        if (thisTransaction.TransactionReference != lastTransaction.TransactionReference)
                            newTransaction = true;

                    if (newTransaction)
                    {
                        JournalTransaction jt = getHeaderTransaction(journalList, thisTransaction.TransactionReference);
                        newTallyMessage(w, jt, SequenceNo++);
                    }
                    buildLedgerEntries(w, thisTransaction, journalList);

                    if (nextTransaction == null)
                        closeTallyMessage(w, thisTransaction);
                    else
                        if (thisTransaction.TransactionReference != nextTransaction.TransactionReference)
                            closeTallyMessage(w, thisTransaction);
                }
                w.WriteEndElement();    // Request Data
                w.WriteEndElement();    // Request Desc
                w.WriteEndElement();    // Import Data
                w.WriteEndElement();    // Body
                w.WriteEndDocument();
                w.Flush();

                ms.Seek(0, SeekOrigin.Begin);
                xml = (new StreamReader(ms)).ReadToEnd();
            }
            return xml;
        }

        private JournalTransaction getHeaderTransaction(ArrayList journalList, string transactionReference)
        {
            JournalTransaction vat = null;
            for (int j = 0; j < journalList.Count; j++)
            {
                JournalTransaction jt = (JournalTransaction)journalList[j];
                if (transactionReference == jt.TransactionReference)
                {
                    if (vat == null)
                        vat = jt;
                    if (jt.isVendorAccount())
                    {
                        vat = jt;
                        break;
                    }
                }
            }
            return vat;
        }

        private void newTallyMessage(XmlTextWriter w, JournalTransaction jt, int SequenceNo)
        {
            string SeqNoString = SequenceNo.ToString("00000000");
            w.WriteStartElement("TALLYMESSAGE");
            w.WriteAttributeString("xmlns:UDF", "TallyUDF");
            w.WriteStartElement("VOUCHER");
            w.WriteAttributeString("REMOTE", "f147c9c5-ee75-4e49-bad8-9fbfb242dfc6-" + SeqNoString);
            w.WriteAttributeString("VCHKEY", "f147c9c5-ee75-4e49-bad8-9fbfb242dfc6-0000a046:" + SeqNoString);
            w.WriteAttributeString("VCHTYPE", "Journal");
            w.WriteAttributeString("ACTION", "Create");
            w.WriteElementString("DATE", jt.TransactionDate);
            w.WriteElementString("AUDITEDON", "");
            w.WriteElementString("GUID", "f147c9c5-ee75-4e49-bad8-9fbfb242dfc6-" + SeqNoString);
            w.WriteElementString("NARRATION", xmlEncode(jt.isVendorAccount() ? xmlEncode(jt.Description) : string.Empty));
            w.WriteElementString("VOUCHERTYPENAME", "Journal");
            w.WriteElementString("VOUCHERNUMBER", xmlEncode(jt.TransactionReference));
            w.WriteElementString("PARTYLEDGERNAME", (jt.isVendorAccount() ? xmlEncode(jt.AccountCode + jt.SeasonCode) : string.Empty));
            w.WriteElementString("CSTFORMISSUETYPE", "");
            w.WriteElementString("CSTFORMRECVTYPE", "");
            w.WriteElementString("FBTPAYMENTTYPE", "");
            w.WriteElementString("VCHGSTCLASS", "");
            w.WriteElementString("ENTEREDBY", "SUNACCOUNT");
            w.WriteElementString("DIFFACTUALQTY", "");
            w.WriteElementString("AUDITED", "");
            w.WriteElementString("FORJOBCOSTING", "");
            w.WriteElementString("ISOPTIONAL", "");
            w.WriteElementString("EFFECTIVEDATE", jt.TransactionDate);
            w.WriteElementString("USEFORINTEREST", "");
            w.WriteElementString("USEFORGAINLOSS", "");
            w.WriteElementString("USEFORGODOWNTRANSFER", "");
            w.WriteElementString("USEFORCOMPOUND", "");
            w.WriteElementString("ALTERID", "");
            w.WriteElementString("EXCISEOPENING", "");
            w.WriteElementString("USEFORFINALPRODUCTION", "");
            w.WriteElementString("ISCANCELLED", "");
            w.WriteElementString("HASCASHFLOW", "");

            w.WriteElementString("ISPOSTDATED", "");
            w.WriteElementString("USETRACKINGNUMBER", "");
            w.WriteElementString("ISINVOICE", "");
            w.WriteElementString("MFGJOURNAL", "");
            w.WriteElementString("HASDISCOUNTS", "");
            w.WriteElementString("ASPAYSLIP", "");
            w.WriteElementString("ISCOSTCENTRE", "");
            w.WriteElementString("ISDELETED", "");
            w.WriteElementString("ASORIGINAL", "");

        }

        private void closeTallyMessage(XmlTextWriter w, JournalTransaction jt)
        {
            w.WriteStartElement("UDF:REFERENCEDATE.LIST");
            w.WriteAttributeString("DESC", "`ReferenceDate`");
            w.WriteAttributeString("ISLIST", "YES");
            w.WriteAttributeString("TYPE", "Date");

            w.WriteStartElement("UDF:REFERENCEDATE");
            w.WriteAttributeString("DESC", "`ReferenceDate`");
            w.WriteString(jt.TransactionDate);
            w.WriteEndElement();    // ReferenceDate
            w.WriteEndElement();    // ReferenceDate.List

            w.WriteEndElement();    // Voucher
            w.WriteEndElement();    // Tally Message
        }

        private void buildLedgerEntries(XmlTextWriter w, JournalTransaction jt, ArrayList journalList)
        {
            decimal otherAmount = decimal.Parse(jt.OtherAmount);

            w.WriteStartElement("ALLLEDGERENTRIES.LIST");
            w.WriteElementString("NARRATION", xmlEncode(jt.Description));
            w.WriteElementString("LEDGERNAME", xmlEncode(jt.AccountCode + jt.SeasonCode));
            w.WriteElementString("GSTCLASS", "");
            w.WriteElementString("ISDEEMEDPOSITIVE", (otherAmount > 0 ? "Yes" : "NO"));
            w.WriteElementString("LEDGERFROMITEM", "No");
            w.WriteElementString("REMOVEZEROENTRIES", "No");
            w.WriteElementString("ISPARTYLEDGER", (jt.isVendorAccount() ? "YES" : "No"));
            w.WriteElementString("AMOUNT", (otherAmount * (-1)).ToString("0.00"));

            if (jt.isVendorAccount())    // IsPartyLedger=Yes
            {
                for (int i = 0; i < journalList.Count; i++)
                {
                    JournalTransaction j = (JournalTransaction)journalList[i];
                    if (j.TransactionReference == jt.TransactionReference && j.JournalLine != jt.JournalLine && !string.IsNullOrEmpty(j.ReferenceNo))
                    {
                        w.WriteStartElement("BILLALLOCATIONS.LIST", "");
                        w.WriteElementString("NAME", xmlEncode(j.ReferenceNo));
                        w.WriteElementString("BILLTYPE", "New Ref");
                        w.WriteElementString("AMOUNT", decimal.Parse(j.OtherAmount).ToString("0.00"));
                        w.WriteEndElement();
                    }
                }
            }
            w.WriteEndElement();    //All Leder Entries
        }

        public void updateTallyUploadHistory(TallyUploadHistoryDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("TallyUploadHistoryApt", "GetTallyUploadHistoryByKey");
                ad.SelectCommand.Parameters["@TallyUploadHistoryId"].Value = def.TallyUploadHistoryId;
                ad.PopulateCommands();

                TallyUploadHistoryDs dataSet = new TallyUploadHistoryDs();
                TallyUploadHistoryDs.TallyUploadHistoryRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.TallyUploadHistory[0];
                    this.TallyUploadHistoryMapping(def, row);
                }
                else
                {
                    row = dataSet.TallyUploadHistory.NewTallyUploadHistoryRow();
                    def.TallyUploadHistoryId = this.getMaxTallyUploadHistoryId() + 1;
                    this.TallyUploadHistoryMapping(def, row);
                    dataSet.TallyUploadHistory.AddTallyUploadHistoryRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Tally Upload History ERROR");
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

        public ArrayList getTallyUploadHistory()
        {
            return getTallyUploadHistory("GetTallyUploadHistory");
        }

        public ArrayList getRecentTallyUploadHistory()
        {
            return getTallyUploadHistory("GetRecentTallyUploadHistory");
        }

        private ArrayList getTallyUploadHistory(string commandName)
        {
            ArrayList list = new ArrayList();

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("TallyUploadHistoryApt", commandName);
                TallyUploadHistoryDs ds = new TallyUploadHistoryDs();
                int recordsAffected = ad.Fill(ds);

                if (recordsAffected < 1) return null;

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    TallyUploadHistoryDef df = new TallyUploadHistoryDef();
                    TallyUploadHistoryMapping(ds.Tables[0].Rows[i], df);
                    list.Add(df);
                }
                ctx.VoteCommit();
                return list;
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

        private int getMaxTallyUploadHistoryId()
        {
            IDataSetAdapter ad = getDataSetAdapter("TallyUploadHistoryApt", "GetMaxTallyUploadHistoryId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        private void TallyUploadHistoryMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(TallyUploadHistoryDs.TallyUploadHistoryRow) &&
                target.GetType() == typeof(TallyUploadHistoryDef))
            {
                TallyUploadHistoryDs.TallyUploadHistoryRow row = (TallyUploadHistoryDs.TallyUploadHistoryRow)source;
                TallyUploadHistoryDef def = (TallyUploadHistoryDef)target;

                def.TallyUploadHistoryId = row.TallyUploadHistoryId;
                if (!row.IsUploadFileNameNull())
                    def.UploadFileName = row.UploadFileName;
                else
                    def.UploadFileName = string.Empty;
                if (!row.IsSequenceNoStartNull())
                    def.SequenceNoStart = row.SequenceNoStart;
                else
                    def.SequenceNoStart = 0;
                if (!row.IsSequenceNoEndNull())
                    def.SequenceNoEnd = row.SequenceNoEnd;
                else
                    def.SequenceNoEnd = 0;
                if (!row.IsCreatedByNull())
                    def.CreatedBy = row.CreatedBy;
                else
                    def.CreatedBy = 0;
                if (!row.IsCreatedOnNull())
                    def.CreatedOn = row.CreatedOn;
                else
                    def.CreatedOn = DateTime.MinValue;
            }
            else if (source.GetType() == typeof(TallyUploadHistoryDef) &&
                target.GetType() == typeof(TallyUploadHistoryDs.TallyUploadHistoryRow))
            {
                TallyUploadHistoryDef def = (TallyUploadHistoryDef)source;
                TallyUploadHistoryDs.TallyUploadHistoryRow row = (TallyUploadHistoryDs.TallyUploadHistoryRow)target;

                row.TallyUploadHistoryId = def.TallyUploadHistoryId;
                if (def.UploadFileName == string.Empty)
                    row.SetUploadFileNameNull();
                else
                    row.UploadFileName = def.UploadFileName;
                row.SequenceNoEnd = def.SequenceNoEnd;
                row.SequenceNoStart = def.SequenceNoStart;
                row.CreatedOn = def.CreatedOn;
                row.CreatedBy = def.CreatedBy;
            }
        }

        #endregion


        #region NS-LED Upload function
        public ArrayList getNSLedUploadHistory(int uploadUserId, DateTime startDate, DateTime endDate)
        {
            return AccountWorker.Instance.getNSLedImportFileByCriteria(uploadUserId, startDate, endDate);
        }

        #endregion

        #region FileUploadLog
        public FileUploadLogDef getFileUploadLogByKey(int recordId)
        {
            IDataSetAdapter ad = getDataSetAdapter("FileUploadLogApt", "GetFileUploadLogByKey");
            ad.SelectCommand.Parameters["@RecordId"].Value = recordId;

            FileUploadLogDs dataSet = new FileUploadLogDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            FileUploadLogDef def = new FileUploadLogDef();
            FileUploadLogMapping(dataSet.FileUploadLog[0], def);
            return def;
        }

        public ArrayList getFileUploadLogByCriteria(int fileTypeId, string fileName, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("FileUploadLogApt", "GetFileUploadLogByCriteria");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.Parameters["@FileTypeId"].Value = fileTypeId;
            ad.SelectCommand.Parameters["@FileName"].Value = fileName;

            FileUploadLogDs dataSet = new FileUploadLogDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (FileUploadLogDs.FileUploadLogRow row in dataSet.FileUploadLog)
            {
                FileUploadLogDef def = new FileUploadLogDef();
                FileUploadLogMapping(row, def);
                list.Add(def);
            }
            return list;

        }

        private int getMaxFileUploadLogId()
        {
            IDataSetAdapter ad = getDataSetAdapter("FileUploadLogApt", "GetMaxFileUploadLogId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }


        public void updateFileUploadLog(FileUploadLogDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("FileUploadLogApt", "GetFileUploadLogByKey");
                ad.SelectCommand.Parameters["@RecordId"].Value = def.RecordId;
                ad.PopulateCommands();

                FileUploadLogDs dataSet = new FileUploadLogDs();
                FileUploadLogDs.FileUploadLogRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.FileUploadLog[0];
                    this.FileUploadLogMapping(def, row);
                }
                else
                {
                    row = dataSet.FileUploadLog.NewFileUploadLogRow();
                    def.RecordId = this.getMaxFileUploadLogId() + 1;
                    this.FileUploadLogMapping(def, row);

                    dataSet.FileUploadLog.AddFileUploadLogRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update FileUploadLog ERROR");
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

        private void FileUploadLogMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(FileUploadLogDs.FileUploadLogRow) &&
                target.GetType() == typeof(FileUploadLogDef))
            {
                FileUploadLogDs.FileUploadLogRow row = (FileUploadLogDs.FileUploadLogRow)source;
                FileUploadLogDef def = (FileUploadLogDef)target;

                def.RecordId = row.RecordId;
                def.FileTypeId = row.FileTypeId;
                def.FileName = row.FileName;
                def.FilePath = row.FilePath;
                def.IsUploaded = row.IsUploaded;
                def.Status = row.Status;
                def.SubmittedBy = GeneralWorker.Instance.getUserByKey(row.SubmittedBy);
                def.SubmittedOn = row.SubmittedOn;
                if (row.IsUploadedOnNull())
                    def.UploadedOn = DateTime.MinValue;
                else
                    def.UploadedOn = row.UploadedOn;
            }
            else if (source.GetType() == typeof(FileUploadLogDef) &&
                target.GetType() == typeof(FileUploadLogDs.FileUploadLogRow))
            {
                FileUploadLogDs.FileUploadLogRow row = (FileUploadLogDs.FileUploadLogRow)target;
                FileUploadLogDef def = (FileUploadLogDef)source;

                row.RecordId = def.RecordId;
                row.FileTypeId = def.FileTypeId;
                row.FileName = def.FileName;
                row.FilePath = def.FilePath;
                row.IsUploaded = def.IsUploaded;
                row.Status = def.Status;
                row.SubmittedBy = def.SubmittedBy.UserId;
                row.SubmittedOn = def.SubmittedOn;
                if (def.UploadedOn == DateTime.MinValue)
                    row.SetUploadedOnNull();
                else
                    row.UploadedOn = def.UploadedOn;
            }
        }

        #endregion


    }
}
