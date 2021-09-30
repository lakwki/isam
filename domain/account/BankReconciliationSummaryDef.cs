using System;

namespace com.next.isam.domain.account
{
	/// <summary>
	/// Summary description for PaymentToBankColumnDef.
	/// </summary>
	[Serializable()]
	public class BankReconciliationSummaryDef
	{

		private string transactionrefnum;
		private string journalnumber;
		private string currency;
		private string amount;
		private string sunupdatestatus;
		private string nssupdatestatus;
		private string filename; 

		public BankReconciliationSummaryDef()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string TransactionRefNum{get {return transactionrefnum;} set{transactionrefnum = value;}}
		public string JournalNumber{get {return journalnumber;} set{journalnumber = value;}}
		public string Currency{get {return currency;} set{currency = value;}}
		public string Amount{get {return amount;} set{amount = value;}}
		public string SunUpdateStatus{get {return sunupdatestatus;} set{sunupdatestatus = value;}}
		public string NssUpdateStatus{get {return nssupdatestatus;} set{nssupdatestatus = value;}}
		public string FileName{get {return filename;} set{filename = value;}}
	}
}
