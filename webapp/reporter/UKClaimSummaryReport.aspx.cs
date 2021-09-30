using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.common.appserver;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.isam.domain.claim;
using com.next.infra.util;
using com.next.infra.web;
using com.next.isam.appserver.claim;


namespace com.next.isam.webapp.reporter
{

    public partial class UKClaimSummaryReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControls();
            }
        }

        private class ClaimReasonRef : DomainData
        {
            public string ReasonDesc { get; set; }
            public string ReasonId { get; set; }
            //public List<int> ReasonIdList { get; set; }
            //public List<int> claimTypeIdList { get; set; }
        }


        private List<ListItem> getGroupedReason()
        {
            int i = 0;
            List<ListItem> reason = new List<ListItem>();
            List<UKClaimReasonDef> allReason = UKClaimManager.Instance.getUKClaimReasonList(GeneralCriteria.ALL);
            foreach (UKClaimReasonDef def in allReason)
            {
                for (i = 0; i < reason.Count; i++)
                {

                    if (reason[i].Text == def.ReasonDesc)
                    {
                        reason[i].Value += "," + def.ReasonId.ToString();
                        break;
                    }
                    else
                        if (String.Compare(reason[i].Text,def.ReasonDesc)>0)
                        {
                            ListItem itm = new ListItem();
                            itm.Text = def.ReasonDesc;
                            itm.Value = def.ReasonId.ToString();
                            reason.Insert(i, itm);
                            break;
                        }
                }
                if (i>=reason.Count)
                {
                    ListItem itm = new ListItem();
                    itm.Text = def.ReasonDesc;
                    itm.Value = def.ReasonId.ToString();
                    reason.Add(itm);
                }
            }
            return reason;
        }

        void initControls()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            this.ddl_Office.bindList(userOfficeList, "Description", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());                                    

            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            uclSupplier.setWidth(305);
            uclSupplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            ddl_ClaimType.bindList(UKClaimType.getCollectionValues(), "Name", "Id", "", "--All--", GeneralCriteria.ALL.ToString());
            
            ddl_ClaimReason.bindList(getGroupedReason(), "Text", "Value", "", "--All--", GeneralCriteria.ALL.ToString());
            
            AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
            for (int i = 3; i >= 0; i--)
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - i).ToString(), (calenderDef.BudgetYear - i).ToString()));
            this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());

        }

        protected ReportClass genReport()
        {
            DateTime issueDateFrom = DateTime.MinValue;
            DateTime issueDateTo = DateTime.MinValue;
            int fiscalYear = -1;
            int periodFrom = -1;
            int periodTo = -1;

            int vendorId = -1;
            //int paymentTermId = Convert.ToInt32(ddl_ClaimType.SelectedValue);

            if (rad_IssueDate.Checked && txt_IssueDateFrom.Text.Trim() != "")
            {
                issueDateFrom = DateTimeUtility.getDate(txt_IssueDateFrom.Text.Trim());
                issueDateTo = DateTimeUtility.getDate(txt_IssueDateTo.Text.Trim());
            }
            else if (rad_FiscalPeriod.Checked)
            {
                fiscalYear = Int32.Parse(ddl_Year.SelectedValue);
                periodFrom = Int32.Parse(ddl_PeriodFrom.SelectedValue);
                periodTo = Int32.Parse(ddl_PeriodTo.SelectedValue);
            }

            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (ddl_Office.SelectedValue != "-1")
                officeIdList.append(Convert.ToInt32(ddl_Office.SelectedValue));
            else
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef oRef in userOfficeList)
                    officeIdList.append(oRef.OfficeId);
            }
            
            //TypeCollector productTeamList = TypeCollector.Inclusive;
            //if (uclProductTeam.ProductCodeId != int.MinValue)
            //    productTeamList.append(uclProductTeam.ProductCodeId);
            //else
            //    foreach (int officeId in officeIdList.Values)
            //    {
            //        ArrayList codeList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, officeId, this.LogonUserId, GeneralCriteria.ALLSTRING);
            //        foreach (OfficeStructureRef osRef in codeList)
            //            productTeamList.append(osRef.OfficeStructureId);
            //    }
            int productTeamId = -1;
            if (uclProductTeam.ProductCodeId != int.MinValue)
                productTeamId = uclProductTeam.ProductCodeId;

            if (uclSupplier.VendorId != int.MinValue)
                vendorId = uclSupplier.VendorId;

            TypeCollector ClaimReasonList = TypeCollector.Inclusive;
            if (ddl_ClaimReason.SelectedValue != "-1")
            {
                string[] idList = (ddl_ClaimReason.SelectedValue).Split(",".ToCharArray());
                foreach (string id in idList)
                    ClaimReasonList.append(Convert.ToInt32(id));
            }
            else
                foreach (ListItem itm in ddl_ClaimReason.Items)
                {
                    string[] idList = itm.Value.Split(",".ToCharArray());
                    foreach (string id in idList)
                        ClaimReasonList.append(Convert.ToInt32(id));
                }

            TypeCollector claimTypeIdList = TypeCollector.Inclusive;
            if (int.Parse(ddl_ClaimType.SelectedValue) != -1)
                claimTypeIdList.append(Convert.ToInt32(ddl_ClaimType.SelectedValue));
            else
                foreach (ListItem itm in ddl_ClaimType.Items)
                    //if ((Convert.ToInt32(itm.Value) != UKClaimType.AUDIT_FEE.Id && Convert.ToInt32(itm.Value) != UKClaimType.OTHERS.Id && Convert.ToInt32(itm.Value) != UKClaimType.GB_TEST.Id) || (ddl_ClaimReason.SelectedValue == "-1")) -- Michael commented out on 11-Feb-2019
                        claimTypeIdList.append(Convert.ToInt32(itm.Value)); // do not include 'Audit Fee' and 'GB Test' if any claim reason is selected.
            //int claimTypeId = -1;
            //if (ddl_ClaimType.selectedText != "-1")
            //    claimTypeId = Convert.ToInt32(ddl_ClaimType.SelectedValue);


            string officeDesc = (ddl_Office.SelectedIndex == 0 ? "ALL" : ddl_Office.selectedText);
            //string productTeamDesc = (uclProductTeam.KeyTextBox.Text=="" ? "ALL" : uclProductTeam.KeyTextBox.Text);
            //string vendorDesc = (uclSupplier.KeyTextBox.Text == "" ? "ALL" : IndustryManager.Instance.getVenderRefByKey(Convert.ToInt32(uclSupplier.KeyTextBox.Text)).Name);
            string productTeamDesc = (productTeamId == -1 ? "ALL" : CommonUtil.getProductCodeByKey(productTeamId).CodeDescription);
            string vendorDesc = (vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);
            string claimTypeDesc = (ddl_ClaimType.SelectedIndex == 0 ? "ALL" : ddl_ClaimType.selectedText);
            string claimReasonDesc = (ddl_ClaimReason.SelectedIndex == 0 ? "ALL" : ddl_ClaimReason.selectedText);

            return AccountReportManager.Instance.getUKClaimSummaryReport(issueDateFrom, issueDateTo, fiscalYear, periodFrom, periodTo, officeIdList, officeDesc, productTeamId, productTeamDesc,
                vendorId, vendorDesc, claimTypeIdList, claimTypeDesc, int.Parse(ddl_ClaimReason.SelectedValue), claimReasonDesc, this.LogonUserId, string.Empty);
        }


        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UKClaimSummaryReport");

        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.Excel, "UKClaimSummaryReport");

        }



        protected void rad_FiscalPeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_FiscalPeriod.Checked)
            {
                txt_IssueDateFrom.Enabled = false;
                txt_IssueDateTo.Enabled = false;

                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;
                ddl_PeriodTo.Enabled = true;
            }
        }

        protected void rad_IssueDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_IssueDate.Checked)
            {
                txt_IssueDateFrom.Enabled = true;
                txt_IssueDateTo.Enabled = true;

                ddl_Year.Enabled = false;
                ddl_PeriodFrom.Enabled = false;
                ddl_PeriodTo.Enabled = false;
            }
        }


        protected void ddl_PeriodFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ddl_PeriodTo.SelectedIndex = ddl_PeriodFrom.SelectedIndex;
        }
    }
}
