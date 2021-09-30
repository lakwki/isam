using System; 
using System.Data;
using System.Collections;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.infra.persistency.dataaccess;

namespace com.next.isam.dataserver.worker
{
    public class ReportWorker : Worker 
    {
        private static ReportWorker _instance;
  		private GeneralWorker generalWorker;
		private CommonWorker commonWorker;
		private VendorWorker vendorWorker;

		protected ReportWorker()
		{
			generalWorker = GeneralWorker.Instance;
			commonWorker = CommonWorker.Instance;
			vendorWorker = VendorWorker.Instance;
		}

		public static ReportWorker Instance
		{
			get 
			{
				if (_instance == null)
				    _instance = new ReportWorker();
				return _instance;
			}
		}

        public ArrayList GetPaymentReferenceCodeList_tobeRemoved()
        {
            int i;
            ArrayList list = new ArrayList();

            IDataSetAdapter ad = getDataSetAdapter("BankAccountApt", "getPaymentReferenceCodeList");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            for (i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                list.Add(dataSet.Tables[0].Rows[i][0].ToString());
            }

            return list;
        }


        public DataSet getCustomerReceiptReportByCriteria_tobeRemoved(DateTime receiptDateFrom, DateTime receiptDateTo, int fiscalYear, int periodFrom, int periodTo,
            int baseCurrencyId, ArrayList customerIdList, ArrayList currencyIdList, 
            //ArrayList ReceiptReferenceNoList,
            string receiptReferenceNo, ArrayList officeIdList, ArrayList seasonIdList, 
            int departmentId, ArrayList productTeamIdList,  int vendorId,
            //int productTeamId, 
            ArrayList tradingAgencyIdList, ArrayList purchaseTermIdList, ArrayList paymentTermIdList)
        {
            int i;

            IDataSetAdapter ad = getDataSetAdapter("AccountReportApt", "getCustomerReceiptReportByCriteria");

            if (receiptDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@receiptDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@receiptDateFrom"].Value = receiptDateFrom;

            if (receiptDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@receiptDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@receiptDateTo"].Value = receiptDateTo;

            if (fiscalYear == int.MinValue)
                ad.SelectCommand.Parameters["@fiscalYear"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@fiscalYear"].Value = fiscalYear;

            if (periodFrom == int.MinValue)
                ad.SelectCommand.Parameters["@periodFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;

            if (periodTo == int.MinValue)
                ad.SelectCommand.Parameters["@periodTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;

            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;

            TypeCollector customerIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < customerIdList.Count; i++) customerIdCollector.append(Convert.ToInt32(customerIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdCollector.IsInclusive, customerIdCollector.Values);

            TypeCollector currencyIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < currencyIdList.Count; i++) currencyIdCollector.append(Convert.ToInt32(currencyIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@currencyIdList"] = CustomDataParameter.parse(currencyIdCollector.IsInclusive, currencyIdCollector.Values);

            if (receiptReferenceNo == "")
                ad.SelectCommand.Parameters["@receiveRefCode"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@receiveRefCode"].Value = receiptReferenceNo;

            TypeCollector officeIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < officeIdList.Count; i++) officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);

            TypeCollector seasonIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < seasonIdList.Count; i++) seasonIdCollector.append(Convert.ToInt32(seasonIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@seasonIdList"] = CustomDataParameter.parse(seasonIdCollector.IsInclusive, seasonIdCollector.Values);

            //if (productTeamId == -1)
            //    ad.SelectCommand.Parameters["@productTeamId"].Value = DBNull.Value;
            //else
            //    ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            TypeCollector ProductTeamCollector = TypeCollector.Inclusive;
            for (i = 0; i < productTeamIdList.Count; i++) ProductTeamCollector.append(Convert.ToInt32(productTeamIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@productTeamIdList"] = CustomDataParameter.parse(ProductTeamCollector.IsInclusive, ProductTeamCollector.Values);


            TypeCollector purchaseTermIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < purchaseTermIdList.Count; i++) purchaseTermIdCollector.append(Convert.ToInt32(purchaseTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@purchaseTermIdList"] = CustomDataParameter.parse(purchaseTermIdCollector.IsInclusive, purchaseTermIdCollector.Values);

            TypeCollector tradingAgencyIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < tradingAgencyIdList.Count; i++) tradingAgencyIdCollector.append(Convert.ToInt32(tradingAgencyIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@tradingAgencyIdList"] = CustomDataParameter.parse(tradingAgencyIdCollector.IsInclusive, tradingAgencyIdCollector.Values);

            TypeCollector paymentTermIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < paymentTermIdList.Count; i++) paymentTermIdCollector.append(Convert.ToInt32(paymentTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@paymentTermIdList"] = CustomDataParameter.parse(paymentTermIdCollector.IsInclusive, paymentTermIdCollector.Values);

            ad.SelectCommand.Parameters["@departmentId"].Value = departmentId;
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            
            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);
            
            return ds;
        }


        public DataSet getSupplierPaymentReportByCriteria_tobeRemoved(DateTime PaymentDateFrom, DateTime PaymentDateTo, int fiscalYear, int periodFrom, int periodTo,
                int baseCurrencyId, ArrayList customerIdList, ArrayList currencyIdList, string PaymentReferenceNo, ArrayList officeIdList, ArrayList seasonIdList,
                ArrayList productTeamIdList, ArrayList tradingAgencyIdList, ArrayList purchaseTermIdList, ArrayList paymentTermIdList, int departmentId, int vendorId)
        {
            int i;

            IDataSetAdapter ad = getDataSetAdapter("AccountReportApt", "getSupplierPaymentByCriteria");

            if (PaymentDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = PaymentDateFrom;

            if (PaymentDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = PaymentDateTo;

            if (fiscalYear == int.MinValue)
                ad.SelectCommand.Parameters["@fiscalYear"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@fiscalYear"].Value = fiscalYear;

            if (periodFrom == int.MinValue)
                ad.SelectCommand.Parameters["@periodFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;

            if (periodTo == int.MinValue)
                ad.SelectCommand.Parameters["@periodTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;

            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;

            ad.SelectCommand.Parameters["@departmentId"].Value = departmentId;
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;

            TypeCollector customerIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < customerIdList.Count; i++) customerIdCollector.append(Convert.ToInt32(customerIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdCollector.IsInclusive, customerIdCollector.Values);

            TypeCollector currencyIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < currencyIdList.Count; i++) currencyIdCollector.append(Convert.ToInt32(currencyIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@currencyIdList"] = CustomDataParameter.parse(currencyIdCollector.IsInclusive, currencyIdCollector.Values);

            if (PaymentReferenceNo == "")
                ad.SelectCommand.Parameters["@paymentRefCode"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@paymentRefCode"].Value = PaymentReferenceNo;

            TypeCollector officeIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < officeIdList.Count; i++) officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);

            TypeCollector seasonIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < seasonIdList.Count; i++) seasonIdCollector.append(Convert.ToInt32(seasonIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@seasonIdList"] = CustomDataParameter.parse(seasonIdCollector.IsInclusive, seasonIdCollector.Values);

            //if (productTeamId == -1)
            //    ad.SelectCommand.Parameters["@productTeamId"].Value = DBNull.Value;
            //else
            //    ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            TypeCollector ProductTeamCollector = TypeCollector.Inclusive;
            for (i = 0; i < productTeamIdList.Count; i++) ProductTeamCollector.append(Convert.ToInt32(productTeamIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@productTeamIdList"] = CustomDataParameter.parse(ProductTeamCollector.IsInclusive, ProductTeamCollector.Values);


            TypeCollector purchaseTermIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < purchaseTermIdList.Count; i++) purchaseTermIdCollector.append(Convert.ToInt32(purchaseTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@purchaseTermIdList"] = CustomDataParameter.parse(purchaseTermIdCollector.IsInclusive, purchaseTermIdCollector.Values);

            TypeCollector tradingAgencyIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < tradingAgencyIdList.Count; i++) tradingAgencyIdCollector.append(Convert.ToInt32(tradingAgencyIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@tradingAgencyIdList"] = CustomDataParameter.parse(tradingAgencyIdCollector.IsInclusive, tradingAgencyIdCollector.Values);

            TypeCollector paymentTermIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < paymentTermIdList.Count; i++) paymentTermIdCollector.append(Convert.ToInt32(paymentTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@paymentTermIdList"] = CustomDataParameter.parse(paymentTermIdCollector.IsInclusive, paymentTermIdCollector.Values);

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public DataSet getLCBatchContolReport_tobeRemoved(int lcBatchNoFrom, int lcBatchNoTo, DateTime lcBatchDateFrom, DateTime lcBatchDateTo,
                    DateTime lcIssueDateFrom, DateTime lcIssueDateTo, DateTime lcExpiryDateFrom, DateTime lcExpiryDateTo,
                    int coId, ArrayList officeIdList, int vendorId, int lcApplicationNoFrom, int lcApplicationNoTo, DateTime lcApplicationDateFrom, DateTime lcApplicationDateTo,
                    string lcNoFrom, string lcNoTo)
        {
            int i;

            IDataSetAdapter ad = getDataSetAdapter("LCReportApt", "GetLCBatchControlReport");

            TypeCollector officeIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < officeIdList.Count; i++) officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);

            if (vendorId == int.MinValue)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            if (coId == int.MinValue)
                ad.SelectCommand.Parameters["CoId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["CoId"].Value = coId;

            //TypeCollector coIdCollector = TypeCollector.Inclusive;
            //for (i = 0; i < coIdList.Count; i++) coIdCollector.append(Convert.ToInt32(coIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@COIdList"] = CustomDataParameter.parse(coIdCollector.IsInclusive, coIdCollector.Values);

            if (lcBatchNoFrom == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = lcBatchNoFrom;

            if (lcBatchNoTo == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = lcBatchNoTo;


            if (lcBatchDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@LCBatchDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchDateFrom"].Value = lcBatchDateFrom;

            if (lcBatchDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@LCBatchDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchDateTo"].Value = lcBatchDateTo;


            if (lcIssueDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = lcIssueDateFrom;

            if (lcIssueDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = lcIssueDateTo;


            if (lcExpiryDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcExpiryDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcExpiryDateFrom"].Value = lcExpiryDateFrom;

            if (lcExpiryDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcExpiryDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcExpiryDateTo"].Value = lcExpiryDateTo;

            ad.SelectCommand.Parameters["@LCApplicationNoFrom"].Value = lcApplicationNoFrom ;
            ad.SelectCommand.Parameters["@LCApplicationNoTo"].Value = lcApplicationNoTo ;

            if (lcApplicationDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@LCApplicationDateFrom"].Value = DBNull.Value ;
                ad.SelectCommand.Parameters["@LCApplicationDateTo"].Value = DBNull.Value ;
            }
            else
            {
                ad.SelectCommand.Parameters["@LCApplicationDateFrom"].Value = lcApplicationDateFrom;
                ad.SelectCommand.Parameters["@LCApplicationDateTo"].Value = lcApplicationDateTo;
            }
            
            if (lcNoFrom == "")
            {
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@LCNoTo"].Value = DBNull.Value ;
            }
            else{
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom ;
                ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo ;
            }          
          
            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public DataSet getLCStatusReport_tobeRemoved(int lcBatchNoFrom, int lcBatchNoTo, string lcNoFrom, string lcNoTo,
                    DateTime lcIssueDateFrom, DateTime lcIssueDateTo, DateTime lcExpiryDateFrom, DateTime lcExpiryDateTo,
                    DateTime lcPaymentCheckDateFrom, DateTime lcPaymentCheckDateTo, 
                    int coId, ArrayList officeIdList, ArrayList productTeamIdList, int vendorId)
        {
            int i;

            IDataSetAdapter ad = getDataSetAdapter("LCReportApt", "GetLCStatusReport");

            TypeCollector officeIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < officeIdList.Count; i++) officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);

            TypeCollector productTeamIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < productTeamIdList.Count; i++) productTeamIdCollector.append(Convert.ToInt32(productTeamIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@ProductTeamIdList"] = CustomDataParameter.parse(productTeamIdCollector.IsInclusive, productTeamIdCollector.Values);

            if (vendorId == int.MinValue)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            if (coId == int.MinValue)
                ad.SelectCommand.Parameters["CoId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["CoId"].Value = coId;

            //TypeCollector coIdCollector = TypeCollector.Inclusive;
            //for (i = 0; i < coIdList.Count; i++) coIdCollector.append(Convert.ToInt32(coIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@COIdList"] = CustomDataParameter.parse(coIdCollector.IsInclusive, coIdCollector.Values);

            if (lcBatchNoFrom == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = lcBatchNoFrom;

            if (lcBatchNoTo == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = lcBatchNoTo;


            if (lcNoFrom == "")
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom;

            if (lcNoTo == "")
                ad.SelectCommand.Parameters["@LCNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo;


            if (lcIssueDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = lcIssueDateFrom;

            if (lcIssueDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = lcIssueDateTo;


            if (lcExpiryDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcExpiryDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcExpiryDateFrom"].Value = lcExpiryDateFrom;

            if (lcExpiryDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcExpiryDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcExpiryDateTo"].Value = lcExpiryDateTo;

            if (lcPaymentCheckDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcPaymentCheckDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcPaymentCheckDateFrom"].Value = lcPaymentCheckDateFrom;

            if (lcPaymentCheckDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcPaymentCheckDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcPaymentCheckDateTo"].Value = lcPaymentCheckDateTo;


            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }



        public DataSet getLCNewApplicationAllocation_tobeRemoved()
        {

            IDataSetAdapter ad = getDataSetAdapter("LCReportApt", "GetLCNewApplicationAllocationReport");

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }


    }
}
