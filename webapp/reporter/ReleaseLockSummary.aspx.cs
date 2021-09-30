using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.isam.webapp.commander;
using com.next.isam.appserver.common;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class ReleaseLockSummary : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }

        void initControl()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_OfficeGroup.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());

            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();
            foreach (ListItem item in cbl_TradingAgency.Items)
                item.Selected = true;
                        
        }

        protected ReportClass genReport()
        {

            DateTime releaseLockDateFrom = DateTime.MinValue;
            DateTime releaseLockDateTo = DateTime.MinValue;
            //int officeId = Convert.ToInt32(ddl_Office.SelectedValue);
            int orderType = Convert.ToInt32(ddl_OrderType.SelectedValue) ;
            //int hasReversingEntries = ckb_ReversingEntry.Checked ? 1 : 0;
            //int hasDCNotes = ckb_DCNote.Checked ? 1 : 0;
            //int hasILSTempAC = ckb_ILSTempAC.Checked ? 1 : 0;

            int hasReversingEntries = (rad_RequireType.SelectedValue == "ReversingEntry" ? 1 : 0);
            int hasDCNotes = (rad_RequireType.SelectedValue == "DCNote" ? 1 : 0);
            int hasILSTempAC = (rad_RequireType.SelectedValue == "ILSTempAC" ? 1 : 0);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);


            if (txt_ReleaseDateFrom.Text.Trim() != "")
            {
                releaseLockDateFrom = DateTimeUtility.getDate(txt_ReleaseDateFrom.Text.Trim());
                if (txt_ReleaseDateTo.Text.Trim() == "")
                    txt_ReleaseDateTo.Text = txt_ReleaseDateFrom.Text;
                releaseLockDateTo = DateTimeUtility.getDate(txt_ReleaseDateTo.Text.Trim());
            }

            TypeCollector customerCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
                if (item.Selected)
                    customerCollector.append(Convert.ToInt32(item.Value));
            if (customerCollector.Values.Count==0)
                customerCollector.append(-1);

            TypeCollector tradingAgencyCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_TradingAgency.Items)
                if (item.Selected)
                        tradingAgencyCollector.append(Convert.ToInt32(item.Value));
            if (tradingAgencyCollector.Values.Count == 0)
                tradingAgencyCollector.append(-1);
            /*
            int officeGroupId = -1;
            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeGroupId == -1)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef office in userOfficeList)
                    officeIdList.append(office.OfficeId);
            }
            else
            {
                if (ddl_OfficeGroup.selectedText.Contains("+"))
                {
                    ArrayList officeList = CommonManager.Instance.getOfficeListByReportOfficeGroupId(Convert.ToInt32(officeGroupId));
                    foreach (OfficeRef office in officeList)
                        officeIdList.append(office.OfficeId);
                }
                else
                    officeIdList.append(officeGroupId);
            }
            */
            TypeCollector productTeamList = TypeCollector.Inclusive;
            int officeGroupId = Convert.ToInt32(ddl_OfficeGroup.SelectedValue);
            ArrayList officeList = null;
            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeGroupId == -1)
                officeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            else
                officeList = CommonManager.Instance.getOfficeListByReportOfficeGroupId(Convert.ToInt32(officeGroupId));


            string officeName = string.Empty;
            foreach (OfficeRef office in officeList)
            {
                officeIdList.append(office.OfficeId);
                officeName += (officeName == string.Empty ? "" : ", ") + office.Description.Replace(" Office", "");
            }
            officeName = (officeGroupId == -1 ? "ALL" : officeName);

            //if (!officeIdList.contains(OfficeId.DG.Id))
            //    handlingOfficeId = -1;

            return AccountReportManager.Instance.getReleaseLockSummary(officeName, officeIdList, handlingOfficeId, releaseLockDateFrom, releaseLockDateTo, orderType, 
                customerCollector, tradingAgencyCollector, hasReversingEntries, hasDCNotes, hasILSTempAC, this.LogonUserId);            
        }
        
        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "ReleaseLockSummary");

        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "ReleaseLockSummary");

        }
    }
}
