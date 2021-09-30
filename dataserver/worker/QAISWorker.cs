using System;
using com.next.common.datafactory.worker;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.isam.dataserver.model.qais;
using System.Collections.Generic;
using com.next.isam.domain.claim;

namespace com.next.isam.dataserver.worker
{
	public class QAISWorker : Worker
	{
        private static QAISWorker _instance;

		protected QAISWorker()
		{
		}

		public static QAISWorker Instance
		{
			get 
			{
				if (_instance == null)
                    _instance = new QAISWorker();
				return _instance;
			}
		}

        public void updateInspection(int contractId, int inspectionId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("InspectionApt", "GetInspectionByKey");
                ad.SelectCommand.Parameters["@ContractId"].Value = contractId;
                ad.SelectCommand.Parameters["@InspectionId"].Value = inspectionId;
                ad.PopulateCommands();

                InspectionDs dataSet = new InspectionDs();
                InspectionDs.InspectionRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.Inspection[0];
                    row.IsSentToILS = false;
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update QAIS Inspection ERROR");
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

        public List<UKClaimReasonDef> getUKClaimReasonList(int ClaimTypeId)
        {
            
            IDataSetAdapter ad = getDataSetAdapter("UKClaimReasonApt", "GetUKClaimReasonList");
            ad.SelectCommand.Parameters["@ClaimTypeId"].Value = ClaimTypeId;

            UKClaimReasonDs dataSet = new UKClaimReasonDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimReasonDef> list = new List<UKClaimReasonDef>();

            foreach (UKClaimReasonDs.ClaimReasonRow row in dataSet.ClaimReason)
            {
                UKClaimReasonDef def = new UKClaimReasonDef();
                UKClaimReasonMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        private void UKClaimReasonMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKClaimReasonDs.ClaimReasonRow) &&
                target.GetType() == typeof(UKClaimReasonDef))
            {
                UKClaimReasonDs.ClaimReasonRow row = (UKClaimReasonDs.ClaimReasonRow)source;
                UKClaimReasonDef def = (UKClaimReasonDef)target;
                def.ReasonId = row.ReasonId;
                def.ClaimTypeId = row.ClaimTypeId;
                def.ReasonDesc = row.ReasonDesc;
            }
            else if (source.GetType() == typeof(UKClaimReasonDef) &&
                target.GetType() == typeof(UKClaimReasonDs.ClaimReasonRow))
            {
                UKClaimReasonDs.ClaimReasonRow row = (UKClaimReasonDs.ClaimReasonRow)target;
                UKClaimReasonDef def = (UKClaimReasonDef)source;
                row.ReasonId = def.ReasonId;
                row.ClaimTypeId = def.ClaimTypeId;
                row.ReasonDesc = def.ReasonDesc;
            }

        }


	}
}
