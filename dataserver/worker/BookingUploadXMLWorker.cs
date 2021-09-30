using System;
using System.Collections;
using System.Data;

using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.common.datafactory.model;
using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.types;

namespace com.next.isam.dataserver.worker
{
	public class BookingUploadXMLWorker : Worker
	{
		private static BookingUploadXMLWorker _instance;

		public BookingUploadXMLWorker()
		{
		}

		public static BookingUploadXMLWorker Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new BookingUploadXMLWorker();
				}
				return _instance;
			}
		}
		
		public void deleteEBookingBatch(string batchNo)
		{
			IDataSetAdapter ad = getDataSetAdapter("EBookingApt", "DeleteEBookingBatch");
			ad.SelectCommand.Parameters["@batchNo"].Value = batchNo;
			ad.SelectCommand.ExecuteNonQuery();
		}

		public void insertEBookingBatch(string sql)
		{
			IDataSetAdapter ad = getDataSetAdapter("EBookingApt", "InsertEBookingBatch");
			ad.SelectCommand.DbCommand.CommandText = sql;
			ad.SelectCommand.DbCommand.CommandTimeout = 10000;
			ad.SelectCommand.ExecuteNonQuery();
		}

		public void importEBookingBatch(string batchNo)
		{
			IDataSetAdapter ad = getDataSetAdapter("EBookingApt", "ImportEBookingBatch");
			ad.SelectCommand.Parameters["@batchNo"].Value = batchNo;
			ad.SelectCommand.ExecuteNonQuery();
		}
	}
}