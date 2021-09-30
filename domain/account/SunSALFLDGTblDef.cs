using System;

namespace com.next.isam.domain.account
{
	/// <summary>
	/// Summary description for SunSALFLDGTblDef.
	/// </summary>
	public class SunSALFLDGTblDef
	{
		private string accnt_code;
		private int period;
		private int jrnal_no;
		private int jrnal_line;
		private decimal amount;
		private string d_c;
		private string allocation;
		private string jrnal_type;
		private string jrnal_srce;
		private string treference;
		private string description;

		private string conv_code;
		private decimal conv_rate;
		private decimal other_amt;
		
		private string anal_t9;
		private string anal_t0;
		private string anal_t2;
		private string anal_t1;
		private string anal_t5;

		public SunSALFLDGTblDef()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string ACCNT_CODE {get{return accnt_code;} set { accnt_code = value;}}
		public int PERIOD {get{return period;} set { period = value;}}
		public int JRNAL_NO {get{return jrnal_no;} set { jrnal_no = value;}}
		public int JRNAL_LINE {get{return jrnal_line;} set { jrnal_line = value;}}
		public decimal AMOUNT {get{return amount;} set { amount = value;}}

		public string D_C {get{return d_c;} set { d_c = value;}}

		public string ALLOCATION {get{return allocation;} set { allocation = value;}}
		public string JRNAL_TYPE {get{return jrnal_type;} set { jrnal_type = value;}}
		public string JRNAL_SRCE {get{return jrnal_srce;} set { jrnal_srce = value;}}
		public string TREFERENCE {get{return treference;} set { treference = value;}}
		public string DESCRIPTION {get{return description;} set { description = value;}}
		public string CONV_CODE {get{return conv_code;} set { conv_code = value;}}

		public decimal CONV_RATE {get{return conv_rate;} set { conv_rate = value;}}
		public decimal OTHER_AMT {get{return other_amt;} set { other_amt = value;}}

		public string ANAL_T9 {get{return anal_t9;} set { anal_t9 = value;}}
		public string ANAL_T0 {get{return anal_t0;} set { anal_t0 = value;}}
		public string ANAL_T2 {get{return anal_t2;} set { anal_t2 = value;}}
		public string ANAL_T1 {get{return anal_t1;} set { anal_t1 = value;}}
		public string ANAL_T5 {get{return anal_t5;} set { anal_t5 = value;}}

	}
}
