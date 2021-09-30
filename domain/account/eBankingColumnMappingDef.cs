using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
	[Serializable()]
	public class eBankingColumnMappingDef : DomainData
	{
		private int id;
		private string bankname;
		
		private string recordtype;

		private int seq;
		private int fieldlengh;
		private string fieldname;
		private int paymentfileindex;
		private string defaultvalue;
		private int datacheck;
		private string errormessage;

		public eBankingColumnMappingDef()
		{
		}

		public int ColumnMappingID{get {return id;} set { id = value; }}
		public string BankName{get {return bankname;} set{bankname = value;}}
		public string RecordType{get {return recordtype;} set { recordtype = value; }}
		public int Seq{get {return seq;} set { seq = value; }}
		public int FieldLengh{get {return fieldlengh;} set { fieldlengh = value; }}
		public string FieldName{get {return fieldname;} set { fieldname = value; }}
		public int PaymentFileIndex{get {return paymentfileindex;} set { paymentfileindex = value; }}
		public string DefaultValue{get {return defaultvalue;} set { defaultvalue = value; }}
		public int DataCheck{get {return datacheck;} set { datacheck = value; }}
		public string ErrorMessage{get {return errormessage;} set { errormessage = value; }}
	}
}
