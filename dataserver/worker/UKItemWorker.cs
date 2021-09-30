using System;
using System.Data;
using System.Collections;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.common.datafactory.worker;
using com.next.isam.dataserver.model.nss;
using com.next.isam.domain.common;

namespace com.next.isam.dataserver.worker
{
    public class UKItemWorker : Worker
    {
		private static UKItemWorker _instance;
        private GeneralWorker generalWorker;

        public UKItemWorker()
		{
            generalWorker = GeneralWorker.Instance;
		}

        public static UKItemWorker Instance
		{
			get 
			{
				if (_instance == null)
				{
                    _instance = new UKItemWorker();
				}
				return _instance;
			}
		}


        public ArrayList getUKItemPartList(string itemNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKItemPartApt", "GetUKItemPartList");
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;

            UKItemPartDs dataSet = new UKItemPartDs();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();

            foreach (UKItemPartDs.UKItemPartRow row in dataSet.UKItemPart)
            {
                UKItemRef def = new UKItemRef();
                UKItemPartMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public void removeUKItemPartDetails(string itemNo)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("UKItemPartApt", "GetUKItemPartList");
                ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
                ad.PopulateCommands();

                UKItemPartDs dataSet = new UKItemPartDs();
                int recordsAffected = ad.Fill(dataSet);
                foreach (UKItemPartDs.UKItemPartRow row in dataSet.UKItemPart.Rows)
                {
                    row.Delete();
                }
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


        public void updateUKItem(UKItemRef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKItemDs dataSet = new UKItemDs();
                UKItemDs.UKItemRow row = null;

                IDataSetAdapter ad = getDataSetAdapter("UKItemApt", "GetUKItemByKey");                
                ad.SelectCommand.Parameters["@ItemNo"].Value = def.ItemNo;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKItem[0];
                    this.UKItemMapping(def, row);
                    row.ModifiedOn = DateTime.Now;
                }
                else
                {
                    row = dataSet.UKItem.NewUKItemRow();
                    this.UKItemMapping(def, row);
                    row.Createdon = DateTime.Now;
                    dataSet.UKItem.AddUKItemRow(row);
                }

                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("failed to update UKItem");

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


        public void updateUKItemPart(UKItemPartRef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKItemPartDs dataSet = new UKItemPartDs();
                UKItemPartDs.UKItemPartRow row = null;

                IDataSetAdapter ad = getDataSetAdapter("UKItemPartApt", "GetUKItemPartByKey");
                ad.SelectCommand.Parameters["@ItemNo"].Value = def.ItemNo;
                ad.SelectCommand.Parameters["@PartNo"].Value = def.PartNo;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKItemPart[0];
                    this.UKItemPartMapping(def, row);
                }
                else
                {
                    row = dataSet.UKItemPart.NewUKItemPartRow();
                    this.UKItemPartMapping(def, row);
                    row.Createdon = DateTime.Now;
                    dataSet.UKItemPart.AddUKItemPartRow(row);
                }

                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("failed to update UKItemPart");

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

        #region Mapping Functions


        private void UKItemPartMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKItemPartDs.UKItemPartRow) &&
                target.GetType() == typeof(UKItemPartRef))
            {
                UKItemPartDs.UKItemPartRow row = (UKItemPartDs.UKItemPartRow)source;
                UKItemPartRef def = (UKItemPartRef)target;

                def.ItemNo = row.ItemNo;
                def.PartNo = row.PartNo;
                def.Description = row.PartDesc;
                def.NameCode = row.PartNameCode;
                def.Qty = row.Qty;
                def.SupplierCode = row.SupplierCode;
                def.Comment = row.Comment;

            }
            else if (source.GetType() == typeof(UKItemPartRef) &&
                target.GetType() == typeof(UKItemPartDs.UKItemPartRow))
            {
                UKItemPartRef def = (UKItemPartRef)source;
                UKItemPartDs.UKItemPartRow row = (UKItemPartDs.UKItemPartRow)target;

                row.ItemNo = def.ItemNo;
                row.PartNo = def.PartNo;
                row.PartNameCode = def.NameCode;
                row.PartDesc = def.Description;
                row.SupplierCode = def.SupplierCode;
                row.Qty = def.Qty;
                row.Comment = def.Comment;
            }
        }

        private void UKItemMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKItemDs.UKItemRow) &&
                target.GetType() == typeof(UKItemRef))
            {
                UKItemDs.UKItemRow row = (UKItemDs.UKItemRow)source;
                UKItemRef def = (UKItemRef)target;

                def.ItemNo = row.ItemNo;
                def.Description = row.ItemDesc;
                def.SubGroup = row.SubGroup;
                def.SupplierCode = row.SupplierCode;
            }
            else if (source.GetType() == typeof(UKItemRef) &&
                target.GetType() == typeof(UKItemDs.UKItemRow))
            {
                UKItemRef def = (UKItemRef)source;
                UKItemDs.UKItemRow row = (UKItemDs.UKItemRow)target;

                row.ItemNo = def.ItemNo;
                row.ItemDesc = def.Description;
                row.SupplierCode = def.SupplierCode;
                row.SubGroup = def.SubGroup;
            }
        }





        #endregion

    }
}
