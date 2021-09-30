using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.industry.vendor;
using com.next.common.datafactory.worker.industry;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class AdjustmentNoteDef : DomainData 
    {
        private int adjustmentNoteId;
        private string adjustmentNoteNo;
        private int officeId;
        private AdjustmentType adjustmentType;
        private string debitCreditIndcator;
        private DateTime issueDate;
        private int currencyId;
        private int revisedCurrencyId = -1;
        private int vendorId;
        private string partyName;
        private string partyAddress1;
        private string partyAddress2;
        private string partyAddress3;
        private string partyAddress4;
        private decimal amount;
        private int mailStatus;
        private DateTime createdOn;

        public int AdjustmentNoteId
        {
            get { return adjustmentNoteId; }
            set { adjustmentNoteId = value; }
        }

        public string AdjustmentNoteNo
        {
            get { return adjustmentNoteNo; }
            set { adjustmentNoteNo = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public AdjustmentType AdjustmentType
        {
            get { return adjustmentType; }
            set { adjustmentType = value; }
        }

        public string DebitCreditIndicator
        {
            get { return debitCreditIndcator; }
            set { debitCreditIndcator = value; }
        }

        public DateTime IssueDate
        {
            get { return issueDate; }
            set { issueDate = value; }
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

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public int MailStatus
        {
            get { return mailStatus; }
            set { mailStatus = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        public static void fillPartyInfo(AdjustmentNoteDef def, int customerId)
        {
            if (def.AdjustmentType.Id == AdjustmentType.SALES_ADJUSTMENT.Id)
            {
                if (customerId == CustomerDef.Id.ezibuy.GetHashCode())
                {
                    def.PartyName = "EZIBUY";
                    def.PartyAddress1 = "PO BOX 46-241,";
                    def.PartyAddress2 = "HERNE BAY,";
                    def.PartyAddress3 = "AUCKLAND,";
                    def.PartyAddress4 = "NEW ZEALAND";
                }
                if (customerId == CustomerDef.Id.smithbrooks.GetHashCode())
                {
                    def.PartyName = "Smith & Brooks";
                    def.PartyAddress1 = "UNIT A, BROOK PARK EAST";
                    def.PartyAddress2 = "MEADOW LANE, SHIREBROOK, MANSFIELD";
                    def.PartyAddress3 = "NOTTINGHAMSHIRE";
                    def.PartyAddress4 = "NG20 8RY";
                }
                else
                {
                    def.PartyName = "NEXT COMMERICAL TRADING (SHANGHAI) CO., LTD.";
                    def.PartyAddress1 = string.Empty;
                    def.PartyAddress2 = string.Empty;
                    def.PartyAddress3 = string.Empty;
                    def.PartyAddress4 = string.Empty;

                }
            }
            if (def.AdjustmentType.Id == AdjustmentType.PURCHASE_ADJUSTMENT.Id)
            {
                VendorRef vendor = VendorWorker.Instance.getVendorByKey(def.VendorId);
                def.PartyName = vendor.Name;
                def.PartyAddress1 = vendor.Address0;
                def.PartyAddress2 = vendor.Address1;
                def.PartyAddress3 = vendor.Address2;
                def.PartyAddress4 = vendor.Address3;
            }
        }

    }
}
