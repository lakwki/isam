using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimDCNoteDef : DomainData 
    {
        private int dcNoteId;
        private string dcNoteNo;
        private DateTime dcNoteDate;
        private int officeId;
        private string debitCreditIndcator;
        private int currencyId;
        private int revisedCurrencyId = -1;
        private int vendorId;
        private string partyName;
        private string partyAddress1;
        private string partyAddress2;
        private string partyAddress3;
        private string partyAddress4;
        private string remark;
        private decimal totalAmt;
        private decimal settledAmt;
        private bool isCustom;
        private bool isInterfaced;
        private int mailStatus;
        private DateTime settlementDate;
        private int createUserId;
        private bool isVoid;

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

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public bool IsCustom
        {
            get { return isCustom; }
            set { isCustom = value; }
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

        public string DebitCreditIndicator
        {
            get { return debitCreditIndcator; }
            set { debitCreditIndcator = value; }
        }

        public DateTime DCNoteDate
        {
            get { return dcNoteDate; }
            set { dcNoteDate = value; }
        }

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public int RevisedCurrencyId
        {
            get { return revisedCurrencyId; }
            set { revisedCurrencyId = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
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

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public decimal TotalAmount
        {
            get { return totalAmt; }
            set { totalAmt = value; }
        }

        public decimal SettledAmount
        {
            get { return settledAmt; }
            set { settledAmt = value; }
        }

        public DateTime SettlementDate
        {
            get { return settlementDate; }
            set { settlementDate = value; }
        }

        public int CreateUserId
        {
            get { return createUserId; }
            set { createUserId = value; }
        }

        public bool IsVoid
        {
            get { return isVoid; }
            set { isVoid = value; }
        }

        public bool isVoidAsNSCost
        {
            get
            {
                if (remark.IndexOf("VOID-AS-NSCOST") != -1 && isCustom == false)
                    return true;
                else
                    return false;
            }
        }

        public bool isVoidAsNSProvision
        {
            get
            {
                if (remark.IndexOf("VOID-AS-NSPROVISION") != -1 && isCustom == false)
                    return true;
                else
                    return false;
            }
        }

        public bool isVoidAsSupplierCost
        {
            get
            {
                if (remark.IndexOf("VOID-AS-SUPPLIER-COST") != -1 && isCustom == false)
                    return true;
                else
                    return false;
            }
        }


    }
}
