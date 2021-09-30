using System;
using System.Collections;
using com.next.common.domain.epicor;
using com.next.isam.domain.account;
using com.next.common.datafactory.worker;
using com.next.common.domain.types;
using com.next.isam.domain.types;
using com.next.isam.dataserver.model.common;
using com.next.isam.domain.common;
using com.next.infra.persistency.dataaccess;

namespace com.next.isam.dataserver.worker
{
    public class EpicorInterfaceWorker : Worker
    {
        private GeneralWorker generalWorker;
        public EpicorInterfaceFile GLInterfaceFile;
        public EpicorInterfaceFile ARInvoiceInterfaceFile;
        public EpicorInterfaceFile APInvoiceInterfaceFile;
        public EpicorInterfaceFile ReceiptInterfaceFile;
        public EpicorInterfaceFile PaymentInterfaceFile;

        public Hashtable SimpleTable;
        public Hashtable GLTable;

        private static EpicorInterfaceWorker _instance;

        protected EpicorInterfaceWorker()
        {
            generalWorker = GeneralWorker.Instance;
        }

        public static EpicorInterfaceWorker Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EpicorInterfaceWorker();
                return _instance;
            }
        }

        public void initialize(SunInterfaceQueueDef def)
        {
            SourceTypeEnum e;
            if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKClaim.GetHashCode()
                || def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKClaimRechargeToSupplier.GetHashCode()
                || def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKClaimRechargeToSupplier_ChangeCurrency.GetHashCode())
                e = SourceTypeEnum.NEXT_CLAIM;
            else
                e = SourceTypeEnum.ISAM;

            bool isAutoPost = SunInterfaceTypeRef.isAutoPost(def.SunInterfaceTypeId);

            if (def.OfficeGroup.OfficeGroupId == OfficeId.TH.Id && def.CategoryType == CategoryType.REVERSAL) // 2018-01-25 Merging TH to HK By Michael Lau
                isAutoPost = false;

            GLInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.GL, e, def.CategoryType == CategoryType.REVERSAL ? true : false, def.QueueId, isAutoPost);
            ARInvoiceInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.ARInvoice, e, def.CategoryType == CategoryType.REVERSAL ? true : false, def.QueueId, isAutoPost);
            APInvoiceInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.APInvoice, e, def.CategoryType == CategoryType.REVERSAL ? true : false, def.QueueId, isAutoPost);
            ReceiptInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.Receipt, e, false, def.QueueId, isAutoPost);
            PaymentInterfaceFile = new EpicorInterfaceFile(EpicorInterfaceTypeEnum.Payment, e, false, def.QueueId, isAutoPost);
            SimpleTable = new Hashtable();
            GLTable = new Hashtable();
        }

        public void addToSimpleTable(string key, decimal val)
        {
            if (!SimpleTable.ContainsKey(key))
                SimpleTable.Add(key, val);
            else
            {
                decimal curVal = (decimal)SimpleTable[key];
                val = curVal + val;
                SimpleTable[key] = val;
            }
        }

        public void addToGLTable(string key, decimal baseVal, decimal txVal)
        {
            if (!GLTable.ContainsKey(key))
                GLTable.Add(key, baseVal.ToString() + "|" + txVal.ToString());
            else
            {
                string curVal = (string)GLTable[key];
                decimal curBase = Convert.ToDecimal(curVal.Split('|')[0]);
                decimal curTx = Convert.ToDecimal(curVal.Split('|')[1]);
                GLTable[key] = (baseVal + curBase).ToString() + "|" + (txVal + curTx).ToString();
            }
        }

        public void finalize(int sunInterfaceTypeId)
        {
            if (sunInterfaceTypeId == SunInterfaceTypeRef.Id.DevelopmentSampleCost.GetHashCode() || sunInterfaceTypeId == SunInterfaceTypeRef.Id.Recovery.GetHashCode())
            {
                if (GLTable.Count > 0)
                {
                    foreach (DictionaryEntry itm in GLTable)
                    {
                        string[] values = itm.Key.ToString().Split('|');
                        int officeId = Convert.ToInt32(values[0]);
                        int currencyId = Convert.ToInt32(values[1]);
                        int fiscalYear = Convert.ToInt32(values[2]);
                        int period = Convert.ToInt32(values[3]);
                        bool isReversal = Convert.ToBoolean(values[4]);

                        GLInterfaceEntry entry = new GLInterfaceEntry();
                        entry.Company = generalWorker.getCompanyOfficeMappingByCriteria(CompanyType.NEXT_SOURCING.Id, OfficeId.HK.Id).EpicorCompanyId;
                        entry.GroupId = EpicorInterfaceWorker.Instance.GLInterfaceFile.getSourceString();
                        entry.GroupApplyDate = generalWorker.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, period).EndDate;
                        entry.ApplyDate = entry.GroupApplyDate;
                        entry.DocumentType = GLDocumentTypeEnum.ISAM_OTHER_COST;
                        entry.CurrencyCode = CurrencyId.getCommonName(currencyId);
                        entry.Amount = Convert.ToDecimal(itm.Value.ToString().Split('|')[1]);
                        entry.BaseAmount = Convert.ToDecimal(itm.Value.ToString().Split('|')[0]);
                        if (entry.BaseAmount > 0)
                            entry.DC = !isReversal ? DCFlag.Debit : DCFlag.Credit;
                        else
                            entry.DC = !isReversal ? DCFlag.Credit : DCFlag.Debit;

                        if (sunInterfaceTypeId == SunInterfaceTypeRef.Id.DevelopmentSampleCost.GetHashCode())
                            entry.LineDescription = "Development Sample Cost - China Key";
                        else
                            entry.LineDescription = "Recovery - China Key";
                        entry.Office = "HK1F";
                        entry.COA = officeId == OfficeId.CA.Id ? "1302010" : "1302012";
                        entry.Deferred = 1;
                        entry.IsRecordComplete = false;
                        EpicorInterfaceWorker.Instance.GLInterfaceFile.Entries.Add((IEpicorInterfaceEntry)entry);

                    }
                }
            }

            if (sunInterfaceTypeId == SunInterfaceTypeRef.Id.ProvisionForFabricLiabilities.GetHashCode())
            {
                if (GLTable.Count > 0)
                {
                    foreach (DictionaryEntry itm in GLTable)
                    {
                        string[] values = itm.Key.ToString().Split('|');
                        int officeId = Convert.ToInt32(values[0]);
                        int currencyId = Convert.ToInt32(values[1]);
                        int fiscalYear = Convert.ToInt32(values[2]);
                        int period = Convert.ToInt32(values[3]);
                        bool isReversal = Convert.ToBoolean(values[4]);

                        GLInterfaceEntry entry = new GLInterfaceEntry();
                        entry.Company = generalWorker.getCompanyOfficeMappingByCriteria(CompanyType.NEXT_SOURCING.Id, officeId).EpicorCompanyId;
                        entry.GroupId = EpicorInterfaceWorker.Instance.GLInterfaceFile.getSourceString();
                        entry.GroupApplyDate = generalWorker.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, period).EndDate;
                        entry.ApplyDate = entry.GroupApplyDate;
                        entry.DocumentType = GLDocumentTypeEnum.ISAM_OTHER_COST;
                        entry.CurrencyCode = CurrencyId.getCommonName(currencyId);
                        entry.DC = !isReversal ? DCFlag.Credit : DCFlag.Debit;
                        entry.Amount = Convert.ToDecimal(itm.Value.ToString().Split('|')[1]);
                        entry.BaseAmount = Convert.ToDecimal(itm.Value.ToString().Split('|')[0]);
                        entry.LineDescription = "P" + period.ToString().PadLeft(2, '0') + " FL prov-US$0.02/unit";
                        entry.Office = "SL1F";
                        entry.COA = generalWorker.getEpicorCOA("1415202");
                        entry.SegmentValue3 = "221";
                        entry.SegmentValue4 = "WCTRS";
                        entry.Deferred = 1;
                        entry.IsRecordComplete = false;
                        EpicorInterfaceWorker.Instance.GLInterfaceFile.Entries.Add((IEpicorInterfaceEntry)entry);

                    }
                }
            }

        }

        public void dispose()
        {
            GLInterfaceFile = null;
            ARInvoiceInterfaceFile = null;
            APInvoiceInterfaceFile = null;
            ReceiptInterfaceFile = null;
            PaymentInterfaceFile = null;
            SimpleTable = null;
        }

        public ArrayList getNullImportStatusList()
        {
            IDataSetAdapter ad = getDataSetAdapter("EpicorUploadImportLogApt", "GetNullImportStatusList");
            EpicorUploadImportLogDs dataSet = new EpicorUploadImportLogDs();

            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (EpicorUploadImportLogDs.EpicorUploadImportLogRow row in dataSet.EpicorUploadImportLog)
            {
                EpicorUploadImportLogDef def = new EpicorUploadImportLogDef();
                EpicorUploadImportLogMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        private void EpicorUploadImportLogMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(EpicorUploadImportLogDs.EpicorUploadImportLogRow) &&
                   target.GetType() == typeof(EpicorUploadImportLogDef))
            {
                EpicorUploadImportLogDs.EpicorUploadImportLogRow row = (EpicorUploadImportLogDs.EpicorUploadImportLogRow)source;
                EpicorUploadImportLogDef def = (EpicorUploadImportLogDef)target;

                def.EpicorUploadImportLogId = row.EpicorUploadImportLogId;
                def.EpicorCompany = row.EpicorCompany;
                def.InterfaceModule = row.InterfaceModule;
                if (!row.IsNSSourceTypeNull())
                    def.NSSourceType = row.NSSourceType;
                if (!row.IsDocumentTypeNull())
                    def.DocumentType = row.DocumentType;
                if (!row.IsGroupIdNull())
                    def.GroupId = row.GroupId;
                def.OriginalFileName = row.OriginalFileName;
                if (!row.IsFileNameNull())
                    def.FileName = row.FileName;
                if (!row.IsLogFilePathNull())
                    def.LogFilePath = row.LogFilePath;
                def.UploadedOn = row.UploadedOn;
                if (!row.IsCompletedOnNull())
                    def.CompletedOn = row.CompletedOn;
                if (!row.IsUploadStatusNull())
                    def.UploadStatus = row.UploadStatus;
                if (!row.IsUploadRejectReasonNull())
                    def.UploadRejectReason = row.UploadRejectReason;
                if (!row.IsImportStatusNull())
                    def.ImportStatus = row.ImportStatus;
                def.Status = row.Status;
                def.CreatedOn = row.CreatedOn;
                def.CreatedBy = row.CreatedBy;
                if (!row.IsProcessQueueNull())
                    def.ProcessQueue = row.ProcessQueue;
            }
            else if (source.GetType() == typeof(EpicorUploadImportLogDef) &&
              target.GetType() == typeof(EpicorUploadImportLogDs.EpicorUploadImportLogRow))
            {
                EpicorUploadImportLogDs.EpicorUploadImportLogRow row = (EpicorUploadImportLogDs.EpicorUploadImportLogRow)target;
                EpicorUploadImportLogDef def = (EpicorUploadImportLogDef)source;

                row.EpicorUploadImportLogId = def.EpicorUploadImportLogId;
                row.EpicorCompany = def.EpicorCompany;
                row.InterfaceModule = def.InterfaceModule;
                if (!string.IsNullOrEmpty(def.NSSourceType))
                    row.NSSourceType = def.NSSourceType;
                else
                    row.SetNSSourceTypeNull();

                if (!string.IsNullOrEmpty(def.DocumentType))
                    row.DocumentType = def.DocumentType;
                else
                    row.SetDocumentTypeNull();
                if (!string.IsNullOrEmpty(def.GroupId))
                    row.GroupId = def.GroupId;
                else
                    row.SetGroupIdNull();

                row.OriginalFileName = def.OriginalFileName;

                if (!string.IsNullOrEmpty(def.FileName))
                    row.FileName = def.FileName;
                else
                    row.SetFileNameNull();
                if (!string.IsNullOrEmpty(def.LogFilePath))
                    row.LogFilePath = def.LogFilePath;
                else
                    row.SetLogFilePathNull();
                row.UploadedOn = def.UploadedOn;
                if (def.CompletedOn != DateTime.MinValue)
                    row.CompletedOn = def.CompletedOn;
                else
                    row.SetCompletedOnNull();
                if (!string.IsNullOrEmpty(def.UploadStatus))
                    row.UploadStatus = def.UploadStatus;
                else
                    row.SetUploadStatusNull();
                if (!string.IsNullOrEmpty(def.UploadRejectReason))
                    row.UploadRejectReason = def.UploadRejectReason;
                else
                    row.SetUploadRejectReasonNull();
                if (!string.IsNullOrEmpty(def.ImportStatus))
                    row.ImportStatus = def.ImportStatus;
                else
                    row.SetImportStatusNull();
                row.Status = def.Status;
                row.CreatedOn = def.CreatedOn;
                row.CreatedBy = def.CreatedBy;
                row.ProcessQueue = def.ProcessQueue;
            }
        }


    }
}
