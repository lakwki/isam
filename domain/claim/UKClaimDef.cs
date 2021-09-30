using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using System.Web.UI.WebControls;
using com.next.common.domain.industry.vendor;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;
namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimDef : DomainData
    {
        public UKClaimDef()
        {
            this.ClaimId = -1;
            this.Type = UKClaimType.REWORK;
            this.ItemNo = string.Empty;
            this.ContractNo = string.Empty;
            this.OfficeId = -1;
            this.HandlingOfficeId = -1;
            this.Vendor = null;
            this.SZVendor = null;
            this.ProductTeamId = -1;
            this.TermOfPurchaseId = -1;
            this.Quantity = 0;
            this.Currency = GeneralWorker.Instance.getCurrencyByKey(CurrencyId.USD.Id);
            this.Amount = 0;
            this.HasUKDebitNote = true;
            this.UKDebitNoteNo = string.Empty;
            this.UKDebitNoteDate = DateTime.MinValue;
            this.UKDebitNoteReceivedDate = DateTime.Today;
            this.Remark = string.Empty;
            this.ClaimRequestId = -1;
            this.DebitNoteNo = string.Empty;
            this.DebitNoteDate = DateTime.MinValue;
            this.DebitNoteAmount = 0;
            this.IsInterfaced = false;
            this.IsRechargeInterfaced = false;
            this.IsReadyForSettlement = true;
            this.WorkflowStatus = ClaimWFS.NEW;
            this.WorkflowStatusId = 0;
            this.GUId = string.Empty;
            this.ClaimMonth = string.Empty;
            this.PnLAccountCode = string.Empty;
            this.SettlementOption = UKClaimSettlemtType.DEFAULT;
            this.DCNoteNoIssuedToSupplier = string.Empty;
            this.SupplierDNSettlementDate = DateTime.MinValue;
            this.SupplierDNDate = DateTime.MinValue;
            this.PaymentOfficeId = -1;
            this.StartPageForUpload = -1;
            this.NumOfPageForUpload = -1;
            this.FirstContractDate = DateTime.MinValue;

        }


        public int ClaimId { get; set; }
        public UKClaimType Type { get; set; }
        public string ClaimMonth { get; set; }
        public string ItemNo { get; set; }
        public string ContractNo { get; set; }
        public int OfficeId { get; set; }
        public int HandlingOfficeId { get; set; }
        public int ProductTeamId { get; set; }
        public int TermOfPurchaseId { get; set; }
        public VendorRef Vendor { get; set; }
        public VendorRef SZVendor { get; set; }
        public int Quantity { get; set; }
        public CurrencyRef Currency { get; set; }
        public decimal Amount { get; set; }
        public bool HasUKDebitNote { get; set; }
        public string UKDebitNoteNo { get; set; }
        public DateTime UKDebitNoteDate { get; set; }
        public DateTime UKDebitNoteReceivedDate { get; set; }
        public string Remark { get; set; }
        public int ClaimRequestId { get; set; }
        public string DebitNoteNo { get; set; }
        public DateTime DebitNoteDate { get; set; }
        public decimal DebitNoteAmount { get; set; }
        public bool IsInterfaced { get; set; }
        public bool IsRechargeInterfaced { get; set; }
        public bool IsReadyForSettlement { get; set; }
        public string PnLAccountCode { get; set; }
        public ClaimWFS WorkflowStatus { get; set; }
        public int WorkflowStatusId { get; set; }
        public string GUId { get; set; }
        public DateTime CreateDate { get; set; }
        public UKClaimSettlemtType SettlementOption { get; set; }
        public string DCNoteNoIssuedToSupplier { get; set; }
        public DateTime SupplierDNDate { get; set; }
        public DateTime SupplierDNSettlementDate { get; set; }
        public int PaymentOfficeId { get; set; }
        public int StartPageForUpload { get; set; } // for PDF Upload
        public int NumOfPageForUpload { get; set; } // for PDF Upload
        public DateTime FirstContractDate { get; set; } 

        public string T5Code
        {
            get
            {
                string s = string.Empty;

                if (this.UKDebitNoteNo != string.Empty)
                {
                    int n;
                    for (int i = 0; i <= this.UKDebitNoteNo.Length - 1; i++)
                    {
                        if (int.TryParse(this.UKDebitNoteNo.Substring(i, 1), out n))
                        {
                            s = this.UKDebitNoteNo.Substring(0, i);
                            break;
                        }
                    }
                }
                if (this.Type != null && (this.Type == UKClaimType.FABRIC_TEST || this.Type == UKClaimType.SAFTETY_ISSUE))
                    s = "DN";
                if (this.Type != null && (this.Type == UKClaimType.QCC))
                    s = "QCC";
                if (this.Type != null && (this.Type == UKClaimType.GB_TEST))
                    s = "CT";
                if (this.Type != null && (this.Type == UKClaimType.OTHERS))
                    s = "DN";
                if (this.Type != null && this.Type == UKClaimType.BILL_IN_ADVANCE)
                {
                    if (this.DebitNoteNo.Trim() != string.Empty)
                    {
                        QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
                        QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(int.Parse(this.DebitNoteNo.Trim()));
                        if (requestDef.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                            s = "DN";
                        else
                        {
                            int n;
                            for (int i = 0; i <= requestDef.FormNo.Length - 1; i++)
                            {
                                if (int.TryParse(requestDef.FormNo.Substring(i, 1), out n))
                                {
                                    s = requestDef.FormNo.Substring(0, i);
                                    break;
                                }
                            }

                        }
                    }
                    else
                        s = "DN";
                }

                if (s.ToUpper() == "F")
                    s = "DN";
                if (this.UKDebitNoteNo == "CR145800" || this.UKDebitNoteNo == "CR145799")
                    s = "DN";
                return s;
            }
        }

        public class UKClaimComparer : IComparer<UKClaimDef>
        {
            public enum CompareType
            {
                Office = 1,
                ClaimType = 2,
                UKDebitNoteNo = 3,
                SupplierName = 4,
                CurrencyId = 5
            }

            private CompareType compareType;
            private SortDirection direction;

            public UKClaimComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(UKClaimDef x, UKClaimDef y)
            {
                UKClaimDef defX = x;
                UKClaimDef defY = y;

                if (compareType.GetHashCode() == CompareType.Office.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return com.next.common.domain.types.OfficeId.getStatus(defX.OfficeId).Name.CompareTo(com.next.common.domain.types.OfficeId.getStatus(defY.OfficeId).Name);
                    else
                        return com.next.common.domain.types.OfficeId.getStatus(defY.OfficeId).Name.CompareTo(com.next.common.domain.types.OfficeId.getStatus(defX.OfficeId).Name);
                }
                else if (compareType.GetHashCode() == CompareType.SupplierName.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.Vendor.Name.CompareTo(defY.Vendor.Name);
                    else
                        return defY.Vendor.Name.CompareTo(defX.Vendor.Name);
                }
                else if (compareType.GetHashCode() == CompareType.ClaimType.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.Type.Name.CompareTo(defY.Type.Name);
                    else
                        return defY.Type.Name.CompareTo(defX.Type.Name);
                }
                else if (compareType.GetHashCode() == CompareType.UKDebitNoteNo.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.UKDebitNoteNo.CompareTo(defY.UKDebitNoteNo);
                    else
                        return defY.UKDebitNoteNo.CompareTo(defX.UKDebitNoteNo);
                }
                else if (compareType.GetHashCode() == CompareType.CurrencyId.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.Currency.CurrencyId.CompareTo(defY.Currency.CurrencyId);
                    else
                        return defY.Currency.CurrencyId.CompareTo(defX.Currency.CurrencyId);
                }
                return 0;
            }
        }


    }
}
