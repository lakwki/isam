using System;
using System.Collections;
using System.Collections.Generic;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.isam.domain.account;
using com.next.infra.util;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class SAFDef : DomainData, IComparable
    {
        private const int MAX_DESC_SIZE = 25;

        private string accountCode;
        private string filler1 = String.Empty;
        private int fiscalYear;
        private int period;
        private DateTime transactionDate;
        private decimal baseAmt;
        private string debitCreditIndicator;
        private string journalType;
        private string description;
        private string currencyCode;
        private decimal otherAmt;
        private string t0 = String.Empty;
        private string t1 = String.Empty;
        private string t2 = String.Empty;
        private string t3 = String.Empty;
        private string t4 = String.Empty;
        private string t5 = String.Empty;
        private string t6 = String.Empty;
        private string t7 = String.Empty;
        private string t8 = String.Empty;
        private string t9 = String.Empty;
        private string paymentTerm = String.Empty;
        private string refNo = String.Empty;
        private string officeCode = String.Empty;
        private string tradingAgency = String.Empty;
        private DateTime createDate;
        private int txRefSeqNo = 0;
        private bool isMockShopSample = false;
        private string accruedSince = String.Empty;
        private string targetDB = "1NS";
        private bool isDescOnly = false;

        public SAFDef()
        {
            createDate = DateTime.Now;
        }

        public string AccountCode
        {
            set { accountCode = value; }
            get { return accountCode; }
        }

        public string Filler1
        {
            set { filler1 = value; }
        }

        public int FiscalYear
        {
            set { fiscalYear = value; }
        }

        public int Period
        {
            set { period = value; }
        }

        public DateTime TransactionDate
        {
            set { transactionDate = value; }
        }

        public decimal BaseAmount
        {
            get { return baseAmt; }
            set { baseAmt = value; }
        }

        public string DebitCreditIndicator
        {
            get { return debitCreditIndicator; }
            set { debitCreditIndicator = value; }
        }

        public string JournalType
        {
            get { return journalType; }
            set { journalType = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string CurrencyCode
        {
            set { currencyCode = value; }
        }

        public decimal OtherAmount
        {
            get { return otherAmt; }
            set { otherAmt = value; }
        }

        public string T0
        {
            get { return t0; }
            set { t0 = value; }
        }

        public string T1
        {
            set { t1 = value; }
        }

        public string T2
        {
            set { t2 = value; }
        }

        public string T3
        {
            set { t3 = value; }
        }

        public string T4
        {
            set { t4 = value; }
        }

        public string T5
        {
            set { t5 = value; }
        }

        public string T6
        {
            set { t6 = value; }
            get { return t6; }
        }

        public string T7
        {
            set { t7 = value; }
        }

        public string T8
        {
            set { t8 = value; }
        }

        public string T9
        {
            get { return t9; }
            set { t9 = value; }
        }

        public string PeriodString
        {
            get
            {
                return fiscalYear.ToString().Substring(2) + period.ToString().PadLeft(2, '0');
            }
        }

        public int TransactionRefSeqNo
        {
            set { txRefSeqNo = value; }
        }

        public string PaymentTerm
        {
            set { paymentTerm = value; }
        }

        public string RefNo
        {
            get { return refNo; }
            set { refNo = value; }
        }

        public string OfficeCode
        {
            get { return officeCode; }
            set { officeCode = value; }
        }

        public string TradingAgency
        {
            set { tradingAgency = value; }
        }

        public bool IsMockShopSample
        {
            get { return isMockShopSample; }
            set { isMockShopSample = value; }
        }

        public string TargetDB
        {
            get { return targetDB; }
            set { targetDB = value; }
        }

        public bool IsDescOnly
        {
            get { return isDescOnly; }
            set { isDescOnly = value; }
        }

        public string AccruedSince
        {
            get
            {
                if (accruedSince != string.Empty)
                {
                    return "P" + accruedSince.Substring(4, 2) + "/" + accruedSince.Substring(0, 4);
                }
                return accruedSince;
            }
            set { accruedSince = value; }
        }

        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }

        public int CompareTo(SAFDef other)
        {
            return this.CreateDate.CompareTo(other.CreateDate);
        }

        public int CompareTo(Object other)
        {
            return 0;
        }

        public static void setDescOnlySAFDef(SAFDef def)
        {
            def.IsDescOnly = true;
            def.BaseAmount = 0.0001M;
            def.OtherAmount = 0.0001M;
            def.CreateDate = DateTime.Now;
        }

        public static void addDescArrayAsSAFDefs(ArrayList safList, SAFDef def, string[] descList)
        {
            if (descList.Length > 1)
            {
                for (int i = 1; i <= descList.Length - 1; i++)
                {
                    if (descList[i].Trim() != string.Empty)
                    {
                        SAFDef newDef = (SAFDef)def.Clone();
                        setDescOnlySAFDef(newDef);
                        newDef.Description = descList[i];
                        safList.Add(newDef);
                    }
                }
            }

        }

        /*
        public static void addOverSizeDescAsSAFDefs(ArrayList safList, SAFDef def)
        {
            decimal l = def.Description.Length;
            int t = (int)Math.Floor(l / MAX_DESC_SIZE);
            decimal r = l % 25;

            if (t >= 1)
            {
                for (int i = 2; i <= t; i++)
                {
                    SAFDef newDef = (SAFDef)def.Clone();
                    setDescOnlySAFDef(newDef);
                    newDef.Description = def.Description.Substring(t * MAX_DESC_SIZE, MAX_DESC_SIZE);
                    safList.Add(newDef);
                }

                if (r > 0)
                {
                    SAFDef newDef = (SAFDef)def.Clone();
                    setDescOnlySAFDef(newDef);
                    newDef.Description = def.Description.Substring(t * MAX_DESC_SIZE, (int)r);
                    safList.Add(newDef);
                }
            }
        }
        */

        public static void addOverSizeDescAsSAFDefs(ArrayList safList, SAFDef def)
        {
            List<string> list = StringUtility.chopWordsToListBySize(def.Description, MAX_DESC_SIZE);
            def.Description = list[0];
            safList[safList.Count - 1] = def;

            int i = 0;
            foreach (string s in list)
            {
                if (i > 0)
                {
                    SAFDef newDef = (SAFDef)def.Clone();
                    setDescOnlySAFDef(newDef);
                    newDef.Description = s;
                    safList.Add(newDef);
                }
                i++;
            }
        }


        public override string ToString()
        {
            System.Diagnostics.Debug.Print(t9);
            return
            accountCode + "," + // ACCOUNT CODE
            filler1 + "," + // FILLER1
            fiscalYear.ToString() + period.ToString().PadLeft(3, '0') + "," + // ACCOUNTING PERIOD
            DateTimeUtility.getDateString(transactionDate) + "," + // TRANSACTION DATE
            String.Empty + "," + // FILLER1
            "M" + "," + // RECORDTYPE
            String.Empty + "," + // JOURNAL NO
            String.Empty + "," + // JOURNAL LINE NO
            Math.Round(baseAmt, 4, MidpointRounding.AwayFromZero).ToString() + "," + // BASE AMOUNT
            debitCreditIndicator + "," + // DC INDICATOR
            String.Empty + "," + // ALLOCATION INDICATOR
            journalType + "," + // JOURNAL TYPE
            String.Empty + "," + // JOURNAL SOURCE
            journalType + this.PeriodString + txRefSeqNo.ToString().PadLeft(3, '0') + "," + // TRANSACTION REFERENCE
            String.Empty + "," + // FILLER3
            "=\"" + (description.ToUpper().Replace(',', ' ').Length <= 25 ? description.ToUpper().Replace(',', ' ') : description.ToUpper().Replace(',', ' ').Substring(0, 25)) + "\"," + // DESCRIPTION
            String.Empty + "," + // ENTRY DATE
            String.Empty + "," + // ENTRY PERIOD
            String.Empty + "," + // DUE DATE
            String.Empty + "," + // FILLER4
            String.Empty + "," + // PAYMENT REFERENCE
            String.Empty + "," + // PAYMENT DATE
            String.Empty + "," + // PAYMENT PERIOD
            String.Empty + "," + // ASSET INDICATOR
            String.Empty + "," + // ASSET CODE
            String.Empty + "," + // ASSET SUB CODE
            currencyCode + "," + // CONVERSION CODE
            "0" + "," + // CONVERSION RATE
            Math.Round(otherAmt, 4, MidpointRounding.AwayFromZero).ToString() + "," + // OTHER AMOUNT
            String.Empty + "," + // OTHER AMOUNT DECIMAL
            String.Empty + "," + // CLEAR DOWN SEQUENCE NO
            String.Empty + "," + // FILLER5
            String.Empty + "," + // NEXT PERIOD REVERSAL
            String.Empty + "," + // LOSS OR GAIN
            String.Empty + "," + // ROUGH BOOK FLAG
            String.Empty + "," + // IN USE FLAG
            "=\"" + t0 + "\"," + // T0
            "=\"" + t1 + "\"," + // T1
            "=\"" + t2 + "\"," + // T2
            "=\"" + t3 + "\"," + // T3
            "=\"" + t4 + "\"," + // T4
            "=\"" + t5 + "\"," + // T5
            "=\"" + t6 + "\"," + // T6
            "=\"" + t7 + "\"," + // T7
            "=\"" + t8 + "\"," + // T8
            "=\"" + (t9.Replace(',', ' ').Length <= 15 ? t9.Replace(',', ' ') : t9.Replace(',', ' ').Substring(0, 15)) + "\"," + // T9
            String.Empty + "," + // POSTING DATE
            String.Empty + "," + // UPDATE ORDER BALANCE INDICATOR
            String.Empty + "," + // ALLOCATION IN PROGRESS MARKER
            String.Empty + "," + // JOURNAL HOLD REFERENCE
            String.Empty + "," + // OPERATOR ID
            String.Empty + "," + // BUDGET CHECK ACCOUNT
            String.Empty + "," + // FILLER6
            paymentTerm + "," + // PAYMENT TERM
            refNo + "," + // PAYMENT REFERENCE NO
            officeCode + "," + // OFFICE CODE
            tradingAgency + "," + // TRADING AGENCY
            this.AccruedSince;
        }

        public string ToNDFString()
        {
            return
            accountCode.PadRight(10, ' ') + // ACCOUNT CODE
            String.Empty.PadRight(5, ' ') +  // FILLER1
            fiscalYear.ToString() + period.ToString().PadLeft(3, '0') + // ACCOUNTING PERIOD
            transactionDate.ToString("yyyyMMdd") +  // TRANSACTION DATE
            String.Empty.PadRight(2, ' ') + // FILLER1
            "M" + // RECORDTYPE
            String.Empty.PadRight(7, ' ') + // JOURNAL NO
            String.Empty.PadRight(7, ' ') + // JOURNAL LINE NO
            Math.Round(baseAmt, 2).ToString("0.000").Replace(".", "").PadLeft(18, '0') + // BASE AMOUNT
            debitCreditIndicator + // DC INDICATOR
            String.Empty.PadRight(1, ' ') + // ALLOCATION INDICATOR
            journalType.PadRight(5, ' ') + // JOURNAL TYPE
            "NSS".PadRight(5, ' ') + // JOURNAL SOURCE
            journalType + this.PeriodString + txRefSeqNo.ToString().PadLeft(3, '0') + // TRANSACTION REFERENCE
            String.Empty.PadRight(5, ' ') + // FILLER3
            (description.ToUpper().Replace(',', ' ').Length <= 25 ? description.ToUpper().Replace(',', ' ') : description.ToUpper().Replace(',', ' ').Substring(0, 25)).PadRight(25, ' ') + // DESCRIPTION
            String.Empty.PadRight(8, ' ') + // ENTRY DATE
            String.Empty.PadRight(7, ' ') + // ENTRY PERIOD
            String.Empty.PadRight(8, ' ') + // DUE DATE
            String.Empty.PadRight(6, ' ') + // FILLER4
            String.Empty.PadRight(9, ' ') + // PAYMENT REFERENCE
            String.Empty.PadRight(8, ' ') + // PAYMENT DATE
            String.Empty.PadRight(7, ' ') + // PAYMENT PERIOD
            String.Empty.PadRight(1, ' ') + // ASSET INDICATOR
            String.Empty.PadRight(10, ' ') + // ASSET CODE
            String.Empty.PadRight(5, ' ') + // ASSET SUB CODE
            currencyCode.PadRight(5, ' ') + // CONVERSION CODE
            String.Empty.PadRight(18, '0') + // CONVERSION RATE
            Math.Round(otherAmt, 2).ToString("0.000").Replace(".", "").PadLeft(18, '0') + // OTHER AMOUNT
            String.Empty.PadRight(1, ' ') + // OTHER AMOUNT DECIMAL
            String.Empty.PadRight(5, ' ') + // CLEAR DOWN SEQUENCE NO
            String.Empty.PadRight(4, ' ') + // FILLER5
            String.Empty.PadRight(1, ' ') + // NEXT PERIOD REVERSAL
            String.Empty.PadRight(1, ' ') + // LOSS OR GAIN
            String.Empty.PadRight(1, ' ') + // ROUGH BOOK FLAG
            String.Empty.PadRight(1, ' ') + // IN USE FLAG
            t0.PadRight(15, ' ').Substring(0, 15) + // T0
            t1.PadRight(15, ' ').Substring(0, 15) + // T1
            t2.PadRight(15, ' ').Substring(0, 15) + // T2
            t3.PadRight(15, ' ').Substring(0, 15) + // T3
            t4.PadRight(15, ' ').Substring(0, 15) + // T4
            t5.PadRight(15, ' ').Substring(0, 15) + // T5
            t6.PadRight(15, ' ').Substring(0, 15) + // T6
            t7.PadRight(15, ' ').Substring(0, 15) + // T7
            t8.PadRight(15, ' ').Substring(0, 15) + // T8
            t9.PadRight(15, ' ').Substring(0, 15) + // T9
            String.Empty.PadRight(8, ' ') + // POSTING DATE
            String.Empty.PadRight(1, ' ') + // UPDATE ORDER BALANCE INDICATOR
            String.Empty.PadRight(1, ' ') + // ALLOCATION IN PROGRESS MARKER
            String.Empty.PadRight(5, ' ') + // JOURNAL HOLD REFERENCE
            String.Empty.PadRight(3, ' ') + // OPERATOR ID
            String.Empty.PadRight(10, ' ') + // BUDGET CHECK ACCOUNT
            String.Empty.PadRight(93, ' '); // FILLER6
        }
    }
}
