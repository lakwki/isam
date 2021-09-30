using System;
using System.Collections;
using System.Data;

using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.common.datafactory.worker;

using com.next.isam.dataserver.model.account;
using com.next.isam.domain.account;
using com.next.isam.domain.types;


namespace com.next.isam.dataserver.worker
{
    public class PaymentAdviceWorker : Worker 
    {
        private static PaymentAdviceWorker _instance;

		public PaymentAdviceWorker()
		{
		}

		public static PaymentAdviceWorker Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new PaymentAdviceWorker();
				}
				return _instance;
			}
		}
		
		public void uploadPaymentAdviceFile(GenerateFileRequestDef request)
		{
			System.IO.StreamReader r = null;
			string s;
			string[] cols;
			int paymentAdviceId = 0;
            decimal amount = 0;
            PaymentAdviceDef paymentAdviceDef = null;
            PaymentAdviceDetailDef paymentAdviceDetailDef = null;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

				r = System.IO.File.OpenText(request.FileName);
				s = r.ReadLine();
				
				while (s != null)
				{	
					s = s.Trim();
                    if (s.Contains("'"))
                        s = s.Replace("'", "''");

					if (s != "")
					{
						cols = s.Split(";".ToCharArray());
						if (cols[0].Trim() == "HEADER")
						{

                            paymentAdviceDef = new PaymentAdviceDef(cols[1].Trim(), cols[2].Trim(), 
                                DateTime.ParseExact(cols[3], "dd/MM/yyyy", null), cols[4].Trim(), request.FileName, string.Empty, 1);

                            updatePaymentAdvice(paymentAdviceDef);
                            paymentAdviceId = paymentAdviceDef.PaymentAdviceId;
						}
						else if (cols[0].Trim() == "DETAILS")
						{
                            if (cols[4].IndexOf("(") != -1)
                            {
                                amount = Convert.ToDecimal(cols[4].Trim().Remove(0, 1).Replace(")", "")) * -1;
                            }
                            else
                                amount = Convert.ToDecimal(cols[4].Trim());

                            paymentAdviceDetailDef = new PaymentAdviceDetailDef(paymentAdviceId, cols[2].Trim(), cols[3].Trim(), amount, cols[5].Trim(), DateTime.ParseExact(cols[6], "dd/MM/yyyy", null));

                            updatePaymentAdviceDetail(paymentAdviceDetailDef);							
						}
					}
					s = r.ReadLine();
										
				}

                request.Status = RequestStatus.PROCESSED.StatusId;
                updateGenerateFileRequest(request);

				ctx.VoteCommit();
				r.Close();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
                r.Close();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}


        private int getMaxPaymentAdviceId()
        {
            IDataSetAdapter ad = getDataSetAdapter("PaymentAdviceApt", "GetMaxPaymentAdviceId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        private int getMaxGenerateFileRequestId()
        {
            IDataSetAdapter ad = getDataSetAdapter("GenerateFileRequestApt", "GetMaxGenerateFileRequestId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public ArrayList getGenerateFileRequestList()
        {
            IDataSetAdapter ad = getDataSetAdapter("GenerateFileRequestApt", "GetGenerateFileRequestList");
            
            GenerateFileRequestDs dataSet = new GenerateFileRequestDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (GenerateFileRequestDs.GenerateFileRequestRow row in dataSet.GenerateFileRequest)
            {
                GenerateFileRequestDef def = new GenerateFileRequestDef();
                GenerateFileRequestMapping(row, def);

                list.Add(def);
            }
            return list;
        }


        public ArrayList getPaymentAdviceList()
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("PaymentAdviceApt", "GetPaymentAdviceList");

            PaymentAdviceDs dataSet = new PaymentAdviceDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (PaymentAdviceDs.PaymentAdviceRow row in dataSet.PaymentAdvice)
            {
                PaymentAdviceDef def = new PaymentAdviceDef();
                PaymentAdviceMapping(row, def);
                list.Add(def);
            }

            return list;            
        }

        public ArrayList getPaymentAdviceByCriteria(DateTime uploadDate, string paymentMethod)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("PaymentAdviceApt", "GetPaymentAdviceByCriteria");
            ad.SelectCommand.Parameters["@UploadDate"].Value = uploadDate;
            ad.SelectCommand.Parameters["@PaymentMethod"].Value = paymentMethod;
            ad.PopulateCommands();

            PaymentAdviceDs dataSet = new PaymentAdviceDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (PaymentAdviceDs.PaymentAdviceRow row in dataSet.PaymentAdvice)
            {
                PaymentAdviceDef def = new PaymentAdviceDef();
                PaymentAdviceMapping(row, def);
                list.Add(def);
            }

            return list;            
        }

        public ArrayList getPaymentAdviceDetailList(int paymentAdviceId)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("PaymentAdviceDetailApt", "GetPaymentAdviceDetailByKey");
            ad.SelectCommand.Parameters["@PaymentAdviceId"].Value = paymentAdviceId;
            ad.PopulateCommands();

            PaymentAdviceDetailDs dataSet = new PaymentAdviceDetailDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (PaymentAdviceDetailDs.PaymentAdviceDetailRow row in dataSet.PaymentAdviceDetail)
            {
                PaymentAdviceDetailDef def = new PaymentAdviceDetailDef();
                PaymentAdviceDetailMapping(row, def);
                list.Add(def);
            }

            return list;
        }


        public void updatePaymentAdvice(PaymentAdviceDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("PaymentAdviceApt", "GetPaymentAdviceByKey");
                ad.SelectCommand.Parameters["@PaymentAdviceId"].Value = def.PaymentAdviceId;
                ad.PopulateCommands();

                PaymentAdviceDs dataSet = new PaymentAdviceDs();
                PaymentAdviceDs.PaymentAdviceRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.PaymentAdvice[0];
                    this.PaymentAdviceMapping(def, row);
                }
                else
                {
                    row = dataSet.PaymentAdvice.NewPaymentAdviceRow();
                    def.PaymentAdviceId = getMaxPaymentAdviceId();
                    def.PaymentAdviceId += 1;

                    this.PaymentAdviceMapping(def, row);
                    dataSet.PaymentAdvice.AddPaymentAdviceRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Payment Advice ERROR");
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

        public void updatePaymentAdviceDetail(PaymentAdviceDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                IDataSetAdapter ad = getDataSetAdapter("PaymentAdviceDetailApt", "GetPaymentAdviceDetailByKey");
                ad.PopulateCommands();
                PaymentAdviceDetailDs dataSet = new PaymentAdviceDetailDs();
                PaymentAdviceDetailDs.PaymentAdviceDetailRow row = null;

                row = dataSet.PaymentAdviceDetail.NewPaymentAdviceDetailRow();
                this.PaymentAdviceDetailMapping(def, row);

                dataSet.PaymentAdviceDetail.AddPaymentAdviceDetailRow(row);
                
                int recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Payment Advice ERROR");

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


        public void updateGenerateFileRequest(GenerateFileRequestDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("GenerateFileRequestApt", "GetGenerateFileRequestByKey");
                ad.SelectCommand.Parameters["@RequestId"].Value = def.RequestId;
                ad.PopulateCommands();

                GenerateFileRequestDs dataSet = new GenerateFileRequestDs();
                GenerateFileRequestDs.GenerateFileRequestRow row = null;                

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.GenerateFileRequest[0];
                    this.GenerateFileRequestMapping(def, row);                    
                }
                else
                {
                    row = dataSet.GenerateFileRequest.NewGenerateFileRequestRow();
                    def.RequestId = this.getMaxGenerateFileRequestId() + 1;
                    this.GenerateFileRequestMapping(def, row);
                    dataSet.GenerateFileRequest.AddGenerateFileRequestRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Bank Reconciliation Request ERROR");
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


        #region MAPPING

        internal void PaymentAdviceMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(PaymentAdviceDs.PaymentAdviceRow) &&
                target.GetType() == typeof(PaymentAdviceDef))
            {
                PaymentAdviceDs.PaymentAdviceRow row = (PaymentAdviceDs.PaymentAdviceRow)source;
                PaymentAdviceDef def = (PaymentAdviceDef)target;

                def.BankName = row.BankName;
                def.FileName = row.FileName;
                def.Mailed = row.Mailed;
                def.PayDate = row.PayDate;
                def.PaymentAdviceId = row.PaymentAdviceId;
                if (row.IsSUNSupplierIdNull())
                    def.SUNSupplierId = string.Empty;
                else
                    def.SUNSupplierId = row.SUNSupplierId;
                def.SupplierName = row.SupplierName;
                def.UploadDate = row.UploadDate;
                if (row.IsEpicorSupplierIdNull())
                    def.EpicorSupplierId = string.Empty;
                else
                    def.EpicorSupplierId = row.EpicorSupplierId;
                if (row.IsCompanyNull())
                    def.Company = string.Empty;
                else
                    def.Company = row.Company;
                if (row.IsCheckedNull())
                    def.IsChecked = 0;
                else
                    def.IsChecked = row.Checked;
            }
            else if (source.GetType() == typeof(PaymentAdviceDef) &&
                target.GetType() == typeof(PaymentAdviceDs.PaymentAdviceRow))
            {
                PaymentAdviceDef def = (PaymentAdviceDef)source;
                PaymentAdviceDs.PaymentAdviceRow row = (PaymentAdviceDs.PaymentAdviceRow)target;

                row.BankName = def.BankName;
                row.FileName = def.FileName;
                row.Mailed = def.Mailed;
                row.PayDate = def.PayDate;
                row.PaymentAdviceId = def.PaymentAdviceId;
                if (def.SUNSupplierId == string.Empty)
                    row.SetSUNSupplierIdNull();
                else
                    row.SUNSupplierId = def.SUNSupplierId;
                row.SupplierName = def.SupplierName;
                row.UploadDate = def.UploadDate;
                if (def.EpicorSupplierId == string.Empty)
                    row.SetEpicorSupplierIdNull();
                else
                    row.EpicorSupplierId = def.EpicorSupplierId;
                if (def.Company == string.Empty)
                    row.SetCompanyNull();
                else
                    row.Company = def.Company;
                row.Checked = def.IsChecked;
            }
        }

        

        internal void PaymentAdviceDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(PaymentAdviceDetailDs.PaymentAdviceDetailRow) &&
                target.GetType() == typeof(PaymentAdviceDetailDef))
            {
                PaymentAdviceDetailDs.PaymentAdviceDetailRow row = (PaymentAdviceDetailDs.PaymentAdviceDetailRow)source;
                PaymentAdviceDetailDef def = (PaymentAdviceDetailDef)target;

                def.Amount = row.Amount;
                def.Currency = row.Currency;
                def.ManufacturerInvoiceNo = row.ManufacturerInvoiceNo;
                def.PaymentAdviceId = row.PaymentAdviceId;
                def.PONo = row.PONo;
                if (row.IsTransactionDateNull())
                    def.TransactionDate = DateTime.MinValue ;
                else
                    def.TransactionDate = row.TransactionDate;
                if (row.IsRefNoNull())
                    def.RefNo = string.Empty;
                else
                    def.RefNo = row.RefNo;
            }
            else if (source.GetType() == typeof(PaymentAdviceDetailDef) &&
                target.GetType() == typeof(PaymentAdviceDetailDs.PaymentAdviceDetailRow))
            {
                PaymentAdviceDetailDef def = (PaymentAdviceDetailDef)source;
                PaymentAdviceDetailDs.PaymentAdviceDetailRow row = (PaymentAdviceDetailDs.PaymentAdviceDetailRow)target;

                row.Amount = def.Amount;
                row.Currency = def.Currency;
                row.ManufacturerInvoiceNo = def.ManufacturerInvoiceNo;
                row.PaymentAdviceId = def.PaymentAdviceId;
                row.PONo = def.PONo;
                if (def.TransactionDate == DateTime.MinValue)
                    row.SetTransactionDateNull();
                else
                    row.TransactionDate = def.TransactionDate;
                if (def.RefNo == string.Empty)
                    row.SetRefNoNull();
                else
                    row.RefNo = def.RefNo;  
            }
        }

        internal void GenerateFileRequestMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(GenerateFileRequestDs.GenerateFileRequestRow) &&
                target.GetType() == typeof(GenerateFileRequestDef))
            {
                GenerateFileRequestDs.GenerateFileRequestRow row = (GenerateFileRequestDs.GenerateFileRequestRow)source;
                GenerateFileRequestDef def = (GenerateFileRequestDef)target;

                def.RequestId = row.RequestId;
                def.FileName = row.FileName;
                def.FileType = row.FileType;
                def.SubmitUser = GeneralWorker.Instance.getUserByKey(row.SubmitUserId);
                def.SubmitDate = row.SubmitDate;
                def.Status = row.Status;                
            }
            else if (source.GetType() == typeof(GenerateFileRequestDef) &&
                target.GetType() == typeof(GenerateFileRequestDs.GenerateFileRequestRow))
            {
                GenerateFileRequestDef def = (GenerateFileRequestDef)source;
                GenerateFileRequestDs.GenerateFileRequestRow row = (GenerateFileRequestDs.GenerateFileRequestRow)target;

                row.RequestId = def.RequestId;
                row.FileType = def.FileType;
                row.FileName = def.FileName;
                row.SubmitUserId = def.SubmitUser.UserId;
                row.SubmitDate = def.SubmitDate;
                row.Status = def.Status;                
            }
        }

        #endregion
    }
}
