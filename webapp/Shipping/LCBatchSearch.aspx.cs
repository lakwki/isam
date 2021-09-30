using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module ;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.shipping;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.shipping;
using com.next.infra.util;

namespace com.next.isam.webapp.shipping
{
    public partial class LCBatchSearch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private bool HasAccessRightsTo_View;
        private bool HasAccessRightsTo_Extract;
        private bool HasAccessRightsTo_Edit;

        protected void Page_Load(object sender, EventArgs e)
        {
            int nUserId;
            
            HasAccessRightsTo_View = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcMaintenance.Id, ISAMModule.lcMaintenance.View);
            HasAccessRightsTo_Extract = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcMaintenance.Id, ISAMModule.lcMaintenance.Extract);
            HasAccessRightsTo_Edit = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcMaintenance.Id, ISAMModule.lcMaintenance.Edit);

            //********** For Testing **************************
            //if (this.LogonUserId == 574)
            //    HasAccessRightsTo_View = true;
            //********** For Testing **************************
            nUserId = this.LogonUserId;

            if (!Page.IsPostBack)
            {
                ArrayList userOfficeList = new ArrayList();
                userOfficeList = CommonUtil.getOfficeListByUserId(nUserId, GeneralCriteria.ALL);
                this.ddl_Office.bindList(userOfficeList, "Description", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
                if (userOfficeList.Count == 1) this.ddl_Office.SelectedIndex = 1;

                txt_SupplierName.setWidth(300);
                txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                btn_Search.Enabled = (HasAccessRightsTo_View || HasAccessRightsTo_Extract || HasAccessRightsTo_Edit);
                btn_Extract.Enabled = HasAccessRightsTo_Extract;
            }
        }

        ArrayList vwLCBatchSummaryList
        {
            get { return (ArrayList)ViewState["LCBatchSummary"]; }
            set { ViewState["LCBatchSummary"] = value; }
        }


        protected DataSet LoadDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("LCBatchNo");
            dt.Columns.Add("Office");
            dt.Columns.Add("Supplier");
            dt.Columns.Add("NoOfShip");
            dt.Columns.Add("ccy");
            dt.Columns.Add("POAmt");
            dt.Columns.Add("AppDate");
            dt.Columns.Add("AppliedBy");
            dt.Columns.Add("Status");

            DataRow dr = dt.NewRow();

            dr[0] = "LCB090001";
            dr[1] = "HK";
            dr[2] = "GLOBAL IDEA (INTERNATIONAL) Co. LTD";
            dr[3] = "18";
            dr[4] = "USD";
            dr[5] = "30,000.00";
            dr[6] = "18/11/2008";
            dr[7] = "May Cheung";
            dr[8] = "Applied";
            
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "LCB090001";
            dr[1] = "HK";
            dr[2] = "GLOBAL IDEA (INTERNATIONAL) Co. LTD";
            dr[3] = "18";
            dr[4] = "USD";
            dr[5] = "30,000.00";
            dr[6] = "18/11/2008";
            dr[7] = "May Cheung";
            dr[8] = "Applied";

            dt.Rows.Add(dr);

            ds.Tables.Add(dt);

            return ds;
        }


        protected void btn_Search_Click(object sender, EventArgs e)
        {
            gv_LCBatch.DataSource = getLCBatchSummaryList();
            gv_LCBatch.DataBind();
        }

        protected void btn_Extract_Click(object sender, EventArgs e)
        {
            //TypeCollector LCBatchIdList = TypeCollector.Inclusive;
            ArrayList LCBatchIdList = new ArrayList();
            foreach (GridViewRow r in gv_LCBatch.Rows)
                if (r.RowType == DataControlRowType.DataRow)
                {
                    if (((CheckBox)r.FindControl("ckb_LCBatch")).Checked)
                    {
                        //LCBatchIdList.append(int.Parse(((Label)r.FindControl("lbl_LCBatchId")).Text));
                        LCBatchIdList.Add (int.Parse(((Label)r.FindControl("lbl_LCBatchId")).Text));
                    }
                }
            LCManager.Instance.extractLCBatchDetail(LCBatchIdList, this.LogonUserId);
        }


        protected ArrayList getLCBatchSummaryList()
        {
            int VendorId;
            string sLCBatchNoFrom, sLCBatchNoTo;
            int nLCBatchNoFrom, nLCBatchNoTo;
            DateTime dLCAppliedDateFrom, dLCAppliedDateTo;
            string sLCNoFrom, sLCNoTo;
            int i;

            ArrayList LCBatchSummary = new ArrayList();

            if (this.txt_SupplierName.KeyTextBox.Text == "")
                VendorId = -1;
            else
                VendorId = this.txt_SupplierName.VendorId;

            TypeCollector OfficeIdList = TypeCollector.Inclusive;
            if (this.ddl_Office.SelectedValue == "-1")
                for (i = 1; i < this.ddl_Office.Items.Count; i++)
                    OfficeIdList.append(int.Parse(this.ddl_Office.Items[i].Value));
            else
                OfficeIdList.append(int.Parse(this.ddl_Office.SelectedValue));


            dLCAppliedDateFrom = (this.txt_AppliedDateFrom.DateTime);
            //if (dLCAppliedDateFrom == DateTime.MinValue) dLCAppliedDateFrom = DBNull.Value;
            dLCAppliedDateTo = (this.txt_AppliedDateTo.DateTime);
            //if (dLCAppliedDateTo == DateTime.MinValue) dLCAppliedDateTo = DBNull.Value;

            sLCBatchNoFrom = (this.txt_BatchNoFrom.Text.ToUpper()).Replace("LCB","");
            if (sLCBatchNoFrom == "") 
                nLCBatchNoFrom = -1;
            else
                if (int.TryParse(sLCBatchNoFrom, out nLCBatchNoFrom) == true)
                    nLCBatchNoFrom = int.Parse(sLCBatchNoFrom);

            sLCBatchNoTo = (this.txt_BatchNoTo.Text.ToUpper()).Replace("LCB","");
            if (sLCBatchNoTo == "")
                nLCBatchNoTo = -1;
            else
                if (int.TryParse(sLCBatchNoTo, out nLCBatchNoTo) == true) 
                    nLCBatchNoTo = int.Parse(sLCBatchNoTo);

            sLCNoFrom = this.txt_LCNoFrom.Text;
            sLCNoTo = this.txt_LCNoTo.Text;
            
            
            this.vwLCBatchSummaryList = LCWorker.Instance.getLCBatchSummaryByCriteria(OfficeIdList, VendorId, nLCBatchNoFrom, nLCBatchNoTo, dLCAppliedDateFrom, dLCAppliedDateTo, sLCNoFrom, sLCNoTo);
            return this.vwLCBatchSummaryList;

        }

        protected ArrayList getLCBatchDetailList()
        {
            return null;
        }

        protected void gv_LCBatch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string sLCBatchNo;
            string sParameters;
            string sAppliedBy;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LCBatchSummaryRef LCSummary = (LCBatchSummaryRef)vwLCBatchSummaryList[e.Row.RowIndex];
                sLCBatchNo = LCSummary.LCBatch.LCBatchNo.ToString();
                sLCBatchNo = "LCB" + sLCBatchNo.PadLeft(6, char.Parse("0"));
                sAppliedBy = GeneralWorker.Instance.getUserByKey(LCSummary.LCBatch.CreatedBy).DisplayName;

                ((CheckBox)e.Row.FindControl("ckb_LCBatch")).Enabled = true;
                //((CheckBox)e.Row.FindControl("ckb_LCBatch")).Text = LCSummary.LCBatch.LCBatchId.ToString();
                ((Label)e.Row.FindControl("lbl_LCBatchId")).Text=LCSummary.LCBatch.LCBatchId.ToString();
                ((LinkButton)e.Row.FindControl("lnk_LCBatchNo")).Text = sLCBatchNo;
                ((Label)e.Row.FindControl("lbl_Office")).Text = LCSummary.Office.OfficeCode;
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = LCSummary.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_NoOfShip")).Text = LCSummary.NoOfShipment.ToString("N0");
                ((Label)e.Row.FindControl("lbl_Currency")).Text = LCSummary.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_POAmt")).Text = LCSummary.TotalPOAmount.ToString("N02");
                ((Label)e.Row.FindControl("lbl_ApplicationDate")).Text = DateTimeUtility.getDateString(LCSummary.LCBatch.CreatedOn);
                ((Label)e.Row.FindControl("lbl_AppliedBy")).Text = sAppliedBy;
                ((Label)e.Row.FindControl("lbl_Status")).Text = LCSummary.WorkflowStatus.Name;

                sParameters = "LCBatchId=" + LCSummary.LCBatch.LCBatchId.ToString();
                sParameters = sParameters + "&UserId=" + this.LogonUserId;
                sParameters = sParameters + "&HasEditRights=" + (HasAccessRightsTo_Edit ? "YES" : "NO");
                //sParameters = sParameters + "&LCBatchNo=" + sLCBatchNo;
                //sParameters = sParameters + "&OfficeCode=" + LCSummary.Office.OfficeCode;
                //sParameters = sParameters + "&SupplierName=" + LCSummary.Vendor.Name;
                //sParameters = sParameters + "&CurrencyCode=" + LCSummary.Currency.CurrencyCode;
                //sParameters = sParameters + "&TotalPOAmount=" + LCSummary.TotalPOAmount.ToString();
                //sParameters = sParameters + "&ApplicationDate=" + DateTimeUtility.getDateString(LCSummary.LCBatch.CreatedOn);
                //sParameters = sParameters + "&AppliedBy=" + sAppliedBy;
                //sParameters = sParameters + "&Status=" + LCSummary.WorkflowStatus.Name;
                
                ((LinkButton)e.Row.FindControl("lnk_LCBatchNo")).OnClientClick = "javascript:window.open('LCBatchUpdate.aspx?" + sParameters + "', '_blank', 'width=700,height=400,scrollbars=1,resizable=1,status=1'); return false;";
                //((LinkButton)e.Row.FindControl("lnk_LCBatchNo")).OnClientClick = "vbscript: val=window.ShowModalDialog (""LCBatchUpdate.aspx?" + sParameters + ", \"center:1;dialogHeight:400pt;dialogWidth=700pt;status=yes;help=no;resizable=no;""); return false;";
            
            }


        }


 

    }
}
