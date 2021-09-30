using System;
using System.IO;
using System.Data;

using System.Collections;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;

using com.next.infra.util;
using com.next.infra.configuration.appserver;
using com.next.common.datafactory.worker;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.types;
using com.next.isam.dataserver.model.account;
using com.next.isam.domain.account;

namespace com.next.isam.dataserver.worker
{
	public class eBankingWorker : Worker
	{
		private static eBankingWorker _instance;
		private CommonWorker _commonWorker;
	
		private ArrayList _columnMapping;
		
		public eBankingWorker()	
		{
			_commonWorker = CommonWorker.Instance;
		}
		
		public static eBankingWorker Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new eBankingWorker();
				}
				return _instance;
			}
		}

        public string getFileNameByPath(string path, bool withFileExtension)
        {
            string[] fileSubString = path.Split('\\');
            string fileName = fileSubString[fileSubString.Length - 1].ToString();
            return (withFileExtension) ? fileName : (fileName.Substring(0, fileName.LastIndexOf(".")));
        }



        private int getMaxConvertPaymentFileRequestId()
        {
            IDataSetAdapter ad = getDataSetAdapter("ConvertPaymentFileRequestApt", "GetMaxConvertPaymentFileRequestId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }



        public void updateConvertPaymentFileRequest(ConvertPaymentFileRequestDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ConvertPaymentFileRequestApt", "GetConvertPaymentFileRequestByKey");
                ad.SelectCommand.Parameters["@RequestId"].Value = def.RequestId;
                ad.PopulateCommands();

                ConvertPaymentFileRequestDs dataSet = new ConvertPaymentFileRequestDs();
                ConvertPaymentFileRequestDs.ConvertPaymentFileRequestRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ConvertPaymentFileRequest[0];
                    this.ConvertPaymentFileRequestMapping(def, row);
                }
                else
                {
                    row = dataSet.ConvertPaymentFileRequest.NewConvertPaymentFileRequestRow();
                    def.RequestId = this.getMaxConvertPaymentFileRequestId() + 1;
                    this.ConvertPaymentFileRequestMapping(def, row);
                    dataSet.ConvertPaymentFileRequest.AddConvertPaymentFileRequestRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Convert Payment File Request ERROR");
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

        private void MailAfterFileGen(int logonUserId, string bankname, string filepath, string sourcefile)
        {
            UserRef userRef = CommonUtil.getUserByKey(logonUserId);
            NoticeHelper.sendeBankingFile(userRef, bankname, filepath, sourcefile);
        }

        #region Convert Payment File

        public int ExportBankFile(int userId, ConvertPaymentFileRequestDef request, string destinationPath)
        {
            // Get column mapping by bank first
            //if (_columnMapping == null)
            string bankName = request.Bank == 1? "HSBC" : "SCB";
            string chargeMethod;
            if (request.ChargeMethod == 1)
                chargeMethod = "SHA";
            else if (request.ChargeMethod == 2)
                chargeMethod = "BEN";
            else if (request.ChargeMethod == 3)
                chargeMethod = "OUR";
            else
                chargeMethod = string.Empty;

            _columnMapping = getMappingColumnsByBank(bankName);

            if (this.hasError(userId, bankName, request.FileName))
                return -1;

            // generate file content
            StreamReader sourceFile = File.OpenText(request.FileName);
            string folderstr = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_TEMP_Folder");
            string tmpFileName = folderstr + "tempbody.csv";

            if (File.Exists(tmpFileName))
                File.Delete(tmpFileName);

            StreamWriter srBody = File.CreateText(tmpFileName);

            int reccnt = 0;
            decimal invamt = 0;
            string line;

            if (bankName == "SCB")
            {
                Write2File(bankName, "H", "", "", ref reccnt, ref invamt, srBody);
                srBody.Write(srBody.NewLine);
            }

            while ((line = sourceFile.ReadLine()) != null)
            {
                Write2File(bankName, "", line, chargeMethod, ref reccnt, ref invamt, srBody);
                srBody.Write(srBody.NewLine);
                //reccnt++;
            }

            if (bankName == "SCB")
                Write2File(bankName, "T", "", chargeMethod, ref reccnt, ref invamt, srBody);

            srBody.Close();

            if (bankName == "SCB")
            {
                File.Copy(tmpFileName, destinationPath, true);
            }
            else
            {
                // Export File Header
                StreamWriter srhead = File.CreateText(destinationPath);

                if (bankName == "HSBC")
                    Write2File(bankName, "IFH", "", chargeMethod, ref reccnt, ref invamt, srhead);

                // Open the file to read from.
                StreamReader sr = File.OpenText(tmpFileName);

                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    if (bankName == "HSBC")
                    {
                        srhead.Write(srhead.NewLine + s);
                    }
                }

                sr.Close();
                srhead.Close();

            }
            File.Delete(tmpFileName);
            sourceFile.Close();

            // email notification

            MailAfterFileGen(userId, bankName, destinationPath, request.FileName);

            request.FileName = destinationPath;
            request.Status = RequestStatus.PROCESSED.StatusId;
            updateConvertPaymentFileRequest(request);

            return 1;

            // log function
            //filelog(userid, bankname, destinationpath);		
        }

        private void Write2File
            (string bankname,
                string rectype,
                string sourceline,
                string chargemethod,
                ref int cnt,
                ref decimal invamt,
                StreamWriter sr
            )
        {
            string[] elements = sourceline.Split(',');

            if (rectype == "")
                rectype = elements[0].Trim();

            ArrayList mappinglist = getColumnsFromList(bankname, rectype);

            string tmpout = "";

            if (bankname == "HSBC")
                cnt++;
            else if (bankname == "SCB")
            {
                if (elements[0].ToString().Trim().ToUpper() == "P")
                    cnt++;
            }

            for (int i = 0; i < mappinglist.Count; i++)
            {
                eBankingColumnMappingDef currow = (eBankingColumnMappingDef)mappinglist[i];

                string datestr = DateTimeUtility.getDateString(DateTime.Today);
                string timestr = DateTime.Now.ToString("HH:mm:ss");

                // Writing Logic
                // Default Value always be empty
                // If there is other indicator the val will be overwrited.
                string val = ((string)currow.DefaultValue).Trim();


                if (rectype == "IFH")
                {
                    int fileidx = i + 1;

                    switch (fileidx)
                    {
                        case 6:// File Reference 
                            val = "HSBC_" + DateTimeUtility.getDateString(DateTime.Today) +
                                "_" + timestr;
                            break;
                        case 7:// File Creation Date
                            val = DateTime.Today.ToString("yyyy/MM/dd");
                            break;
                        case 8:// File Creation Time
                            val = timestr;
                            break;
                        case 11:
                            val = cnt.ToString();
                            break;
                        default:
                            break;
                    }
                }
                else if (rectype == "T" && bankname == "SCB")
                {
                    switch (i)
                    {
                        case 1:
                            val = cnt.ToString();
                            break;
                        case 2:
                            val = invamt.ToString();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (currow.PaymentFileIndex != -1)
                    {

                        val = elements[currow.PaymentFileIndex].Trim();
                        if (currow.FieldName == "Customer Reference" || currow.FieldName == "Reference Line 1")
                        {
                            val += "-GP";
                        }

                        // Change Value Date Format Only Apply on HSBC value date
                        if (currow.FieldName == "Value Date(YYYYMMDD)")
                        {
                            string[] datematrix = val.Split('/');
                            val = datematrix[2].ToString() + datematrix[1].ToString() + datematrix[0].ToString();
                        }

                        if (currow.BankName == "SCB" && currow.RecordType == "P" && currow.Seq == 39)
                        {
                            invamt = invamt + Convert.ToDecimal(val);
                        }
                    }
                    else if (currow.BankName == "HSBC" && currow.RecordType == "SECPTY" && currow.Seq == 48)
                    {
                        val = chargemethod;
                    }
                }

                // Check Length of the data
                if (val.Length > currow.FieldLengh)
                    val = val.Substring(0, currow.FieldLengh);

                if (tmpout == "")
                    tmpout = val;
                else
                {
                    stampwithme(ref tmpout, val);
                }
            }

            if (tmpout != "")
            {
                sr.Write(tmpout);
            }
            //sr.WriteLine (tmpout);
        }

        private void stampwithme(ref string envolope, string stamp)
        {
            envolope = envolope + "," + stamp;
        }


        private bool hasError(int userId, string bankName, string sourcePath)
        {
            int rowNo = 0;
            string line;
            string errorMessage = "";
            bool isPass = false;

            ArrayList errorList = new ArrayList();

            string fileType = sourcePath.Substring(sourcePath.Length - 4, 4);
            string[] fileSubstring = sourcePath.Split('\\');
            string fileName = fileSubstring[fileSubstring.Length - 1].ToString();

            if (fileType.ToUpper() != ".CSV")
                errorList.Add(NoticeHelper.getErrorHtml("Only CSV file format is allowed here!", -1));
            else
            {
                if (!File.Exists(sourcePath))
                    errorList.Add(NoticeHelper.getErrorHtml("The file does not exist!", -1));
                else
                {
                    StreamReader sourceFile = File.OpenText(sourcePath);
                    ArrayList lineList = new ArrayList();
                    while ((line = sourceFile.ReadLine()) != null)
                    {
                        lineList.Add(line);
                    }
                    sourceFile.Close();

                    for (int i = 0; i < lineList.Count; i++)
                    {
                        this.checkLine(fileName, bankName, lineList[i].ToString(), rowNo, errorList);
                        rowNo++;
                    }
                }
            }

            if (errorList.Count > 0)
            {
                for (int i = 0; i < errorList.Count; i++)
                {
                    errorMessage += (errorList[i] + "\n");
                }

                NoticeHelper.sendeBankingFileError(CommonUtil.getUserByKey(userId), bankName, errorMessage, fileName);

                isPass = true;
            }

            return isPass;
        }

        private void checkLine(string filename, string bankname, string sourceline, int rowNo, ArrayList errorList)
        {
            string rownumstr = rowNo.ToString();

            string[] elements = sourceline.Split(',');
            int elementscnt = elements.Length;

            ArrayList mappinglist = null;
            string rectype = "";

            if (elements[0].Trim() == "")
            {
                //errorlist.Add("Record Type in line-" + rownumstr+ " is missing! Remaining data in this row will be skipped!");
                errorList.Add(NoticeHelper.getErrorHtml("Record Type is missing !", rowNo));
                return;
            }
            else
                rectype = elements[0].Trim();

            mappinglist = getColumnsFromList(bankname, rectype);

            for (int i = 1; i < elementscnt; i++)
            {
                eBankingColumnMappingDef currow = (eBankingColumnMappingDef)mappinglist[i];

                string val = "";

                if (currow.PaymentFileIndex != -1)
                {
                    int column = currow.PaymentFileIndex;

                    // Error Case 1. Missing Columns
                    if (elements.Length <= currow.PaymentFileIndex)
                    {
                        errorList.Add(NoticeHelper.getErrorHtml("Column " + currow.FieldName + "is missing in the datafile!", rowNo));
                        continue;
                    }


                    val = elements[currow.PaymentFileIndex];
                    int checkdata = currow.DataCheck;
                    if (checkdata == 1)
                    {
                        // Error Case 2. Missing content in the Mandatory fields
                        if ((val == null || val == "") && (currow.ErrorMessage != null || currow.ErrorMessage != ""))
                        {
                            errorList.Add(NoticeHelper.getErrorHtml(currow.ErrorMessage, rowNo));
                            continue;
                        }

                        // Error Case 3. Include invalid charactors
                        int colidx = i + 1;
                        if (val.IndexOf(',') > -1)
                            //errorlist.Add("Line-" + rownumstr+ " (" + colidx.ToString()+ ") : contains invalid char , !");
                            errorList.Add(NoticeHelper.getErrorHtml("Column " + column.ToString() + " has invalid char \",\"!", rowNo));

                        if (val.IndexOf('?') > -1)
                            //errorlist.Add("Line-" + rownumstr+ " (" + colidx.ToString()+ ") : contains invalid char ? !");
                            errorList.Add(NoticeHelper.getErrorHtml("Column " + column.ToString() + " has invalid char \"?\"!", rowNo));

                        if (val.IndexOf('#') > -1)
                            //errorlist.Add("Line-" + rownumstr+ " (" + colidx.ToString()+ ") : contains invalid char # !");
                            errorList.Add(NoticeHelper.getErrorHtml("Column " + column.ToString() + " has invalid char \"#\"!", rowNo));
                    }
                }
            }
        }


        #endregion

        #region COLUMN MAPPING
        public ArrayList getMappingColumnsByBank(string bankname)		
		{
			IDataSetAdapter ad = getDataSetAdapter("ColumnMappingApt", "GetColumneMappingByBankCmd");
			ad.SelectCommand.Parameters["@BankName"].Value = bankname;
			
			eBankingColumnMappingDs ds = new eBankingColumnMappingDs();
			int recordsAffected = ad.Fill(ds);

			if (recordsAffected < 1) return null;

			ArrayList list = new ArrayList();
			
			foreach(eBankingColumnMappingDs.eBankingColumnMappingRow row in ds.eBankingColumnMapping)
			{
				eBankingColumnMappingDef def = new eBankingColumnMappingDef();
				eBankingColumnMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		private ArrayList getColumnsFromList(string bankname, string recordtype)
		{
			ArrayList list = new ArrayList();

			int cnt = this._columnMapping.Count;
			if (cnt > 0)
			{
				for(int i = 0 ; i < cnt; i ++)
				{
					eBankingColumnMappingDef def = (eBankingColumnMappingDef)_columnMapping[i];
					if(def.RecordType == recordtype)
						list.Add(def);
				}
			}
			return list;
		}

		internal void eBankingColumnMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(eBankingColumnMappingDs.eBankingColumnMappingRow)
				&& target.GetType() == typeof(eBankingColumnMappingDef))
			{
				eBankingColumnMappingDs.eBankingColumnMappingRow row = 
					(eBankingColumnMappingDs.eBankingColumnMappingRow)source;
				eBankingColumnMappingDef def = (eBankingColumnMappingDef)target;
				
				def.ColumnMappingID = row.Id;
				def.BankName = row.BankName;
				def.RecordType = row.RecordType;
				def.Seq = row.Seq;
				def.FieldLengh  = row.FieldLength;
				def.FieldName = row.FieldName;
				
				int idx = -1;
				
				if(!row.IsPaymentFileIndexNull())
					idx = row.PaymentFileIndex;
				def.PaymentFileIndex = idx;	

				def.DefaultValue = String.Empty;	
				if(!row.IsDefaultValueNull())
					def.DefaultValue = row.DefaultValue;
				
				int dataCheck = 0;
				if(!row.IsDataCheckNull())
					dataCheck = row.DataCheck;
				def.DataCheck = idx;	

				def.ErrorMessage = String.Empty;	
				if(!row.IsErrorMessageNull())
					def.ErrorMessage = row.ErrorMessage;
			}
		}

        private void ConvertPaymentFileRequestMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ConvertPaymentFileRequestDs.ConvertPaymentFileRequestRow) &&
                target.GetType() == typeof(ConvertPaymentFileRequestDef))
            {
                ConvertPaymentFileRequestDs.ConvertPaymentFileRequestRow row = (ConvertPaymentFileRequestDs.ConvertPaymentFileRequestRow)source;
                ConvertPaymentFileRequestDef def = (ConvertPaymentFileRequestDef)target;

                def.RequestId = row.RequestId;
                def.FileName = row.FileName;
                def.Bank = row.Bank;
                if (!row.IsChargeMethodNull())
                    def.ChargeMethod = row.ChargeMethod;
                else
                    def.ChargeMethod = 0;
                def.SubmitBy = GeneralWorker.Instance.getUserByKey(row.SubmitBy);
                def.SubmittedDate = row.SubmittedDate;

                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(ConvertPaymentFileRequestDef) &&
                target.GetType() == typeof(ConvertPaymentFileRequestDs.ConvertPaymentFileRequestRow))
            {
                ConvertPaymentFileRequestDef def = (ConvertPaymentFileRequestDef)source;
                ConvertPaymentFileRequestDs.ConvertPaymentFileRequestRow row = (ConvertPaymentFileRequestDs.ConvertPaymentFileRequestRow)target;

                row.RequestId = def.RequestId;
                row.FileName = def.FileName;
                row.Bank = def.Bank;
                row.ChargeMethod = def.ChargeMethod;
                row.SubmitBy = def.SubmitBy.UserId;
                row.SubmittedDate = def.SubmittedDate;

                row.Status = def.Status;
            }
        }

		
		#endregion



	}
}
