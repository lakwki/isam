using System;
using System.Collections.Generic;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class GenericDCNoteDef : DomainData
    {
        private int dcNoteId = -1;
        private string dcNoteNo;
        private DateTime dcNoteDate;
        private int issueTypeId;
        private int officeId;
        private int vendorId = -1;
        private int ntVendorId = -1;
        private string customerCode = string.Empty;
        private string glCode = string.Empty;
        private int originalCurrencyId;
        private decimal originalAmount;
        private int billingCurrencyId;
        private decimal amount;
        private string debitCreditIndicator;
        private string partyName;
        private string partyAddress1;
        private string partyAddress2;
        private string partyAddress3;
        private string partyAddress4;
        private string description;
        private string coa;
        private string remark = string.Empty;
        private bool isInterfaced = false;
        private int mailStatus = 0;
        private DateTime settlementDate = DateTime.MinValue;
        private int status = GeneralStatus.ACTIVE.Code;
        private int paymentOfficeId = -1;
        private string attn = string.Empty;

        public int DCNoteId
        {
            get { return dcNoteId; }
            set { dcNoteId = value; }
        }

        public string DCNoteNo
        {
            get { return dcNoteNo; }
            set { dcNoteNo = value; }
        }

        public DateTime DCNoteDate
        {
            get { return dcNoteDate; }
            set { dcNoteDate = value; }
        }

        public int IssueTypeId
        {
            get { return issueTypeId; }
            set { issueTypeId = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public int NTVendorId
        {
            get { return ntVendorId; }
            set { ntVendorId = value; }
        }

        public string CustomerCode
        {
            get { return customerCode; }
            set { customerCode = value; }
        }

        public string GLCode
        {
            get { return glCode; }
            set { glCode = value; }
        }

        public int OriginalCurrencyId
        {
            get { return originalCurrencyId; }
            set { originalCurrencyId = value; }
        }

        public decimal OriginalAmount
        {
            get { return originalAmount; }
            set { originalAmount = value; }
        }

        public int BillingCurrencyId
        {
            get { return billingCurrencyId; }
            set { billingCurrencyId = value; }
        }

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string DebitCreditIndicator
        {
            get { return debitCreditIndicator; }
            set { debitCreditIndicator = value; }
        }

        public string PartyName
        {
            get { return partyName; }
            set { partyName = value; }
        }

        public string PartyAddress1
        {
            get { return partyAddress1; }
            set { partyAddress1 = value; }
        }

        public string PartyAddress2
        {
            get { return partyAddress2; }
            set { partyAddress2 = value; }
        }

        public string PartyAddress3
        {
            get { return partyAddress3; }
            set { partyAddress3 = value; }
        }

        public string PartyAddress4
        {
            get { return partyAddress4; }
            set { partyAddress4 = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string COA
        {
            get { return coa; }
            set { coa = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public bool IsInterfaced
        {
            get { return isInterfaced; }
            set { isInterfaced = value; }
        }

        public int MailStatus
        {
            get { return mailStatus; }
            set { mailStatus = value; }
        }

        public DateTime SettlementDate
        {
            get { return settlementDate; }
            set { settlementDate = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public int PaymentOfficeId
        {
            get { return paymentOfficeId; }
            set { paymentOfficeId = value; }
        }

        public string Attn
        {
            get { return attn; }
            set { attn = value; }
        }

        public static  List<string> getGLCodeList()
        {
            List<string> list = new List<string>();
            list.Add("1301220"); //NEXT MFG (PVT) LTD - TESTING
            list.Add("1301120"); // NEXT RETAIL - NSL UK + HK
            list.Add("1301121"); // NEXT RETAIL - NSL UK + CA
            list.Add("1301122"); //	NEXT RETAIL - NSL UK + SH
            list.Add("1301123"); //	NEXT RETAIL - NSL UK + TH
            list.Add("1301124"); //	NEXT RETAIL - NSL UK + VN
            list.Add("1301125"); //	NEXT RETAIL - NSL UK + TK
            list.Add("1301126"); //	NEXT RETAIL - NSL UK + SL
            list.Add("1301127"); //	NEXT RETAIL - NSL UK + BD
            list.Add("1301128"); //	NEXT RETAIL - NSL UK + PK
            list.Add("1301129"); //	NEXT RETAIL - NSL UK + IN
            list.Add("1301130"); //	NEXT RETAIL - NSL UK + ND
            return list;
        }

    }

}
