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
using com.next.isam.domain.ils;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.shipping;
using com.next.common.domain;
using com.next.infra.web;

namespace com.next.isam.webapp.shipping
{
    public partial class Manifest : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private ArrayList vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (ArrayList)ViewState["SearchResult"];
            }
        }

        private string sortExpression
        {
            get
            {
                return (string)ViewState["SortExpression"];
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }
        private SortDirection sortDirection
        {
            get { return (SortDirection)ViewState["SortDirection"]; }
            set { ViewState["SortDirection"] = value; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //bool includePrintFunction;
            //string inputParameter;

            if (!Page.IsPostBack)
            {
                ddl_DepartLoc.bindList(WebUtil.getShipmentPortList(), "ShipmentPortDescription", "OfficialCode", "", "--ALL--", GeneralCriteria.ALLSTRING);

                //inputParameter = Request.Params["ReportPrinting"];
                //if (!string.IsNullOrEmpty(inputParameter))
                //    includePrintFunction = (inputParameter.Trim().ToUpper() == "TRUE");
                //else
                //    includePrintFunction = false;
            }
        }

        protected void lnk_SelectAll_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gv_AWB.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox ckb = (CheckBox)row.Cells[0].Controls[1];
                    ckb.Checked = true;
                }
            }

        }



        protected void btn_Search_Click(object sender, EventArgs e)
        {
            pnl_Result.Visible = true;

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetManifestList);

            Context.Items.Add(ShipmentCommander.Param.contractNo, txt_ContractNo.Text.Trim());
            Context.Items.Add(ShipmentCommander.Param.voyageNo, txt_VoyageNo.Text.Trim());
            Context.Items.Add(ShipmentCommander.Param.vesselName, txt_VesselName.Text.Trim());

            Context.Items.Add(ShipmentCommander.Param.departurePort, ddl_DepartLoc.SelectedValue);
            if (txt_DepartDate.Text.Trim() == string.Empty)
                Context.Items.Add(ShipmentCommander.Param.departureDate, DateTime.MinValue);
            else
                Context.Items.Add(ShipmentCommander.Param.departureDate, DateTime.ParseExact(txt_DepartDate.Text.Trim(), "dd/MM/yyyy", null));

            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[ShipmentCommander.Param.manifestList];

            this.vwSearchResult = list;

            gv_AWB.DataSource = list;
            gv_AWB.DataBind();
        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            vwSearchResult = null;
            gv_AWB.DataSource = vwSearchResult;
            gv_AWB.DataBind();

            txt_DepartDate.Text = "";
            txt_ContractNo.Text = "";
            txt_VoyageNo.Text = "";
            ddl_DepartLoc.SelectedIndex = -1;
            txt_VesselName.Text = "";
        }

        protected ContainerManifestReport genReport(string outputFormat)
        {
            string voyageNo, departurePort;
            DateTime departureDate;
            string ContractNo;
            string vesselName;

            voyageNo = txt_VoyageNo.Text;
            departurePort = ddl_DepartLoc.SelectedValue;
            if (txt_DepartDate.Text.Trim() == string.Empty)
                departureDate = DateTime.MinValue;
            else
                departureDate = DateTime.ParseExact(txt_DepartDate.Text.Trim(), "dd/MM/yyyy", null);
            ContractNo = txt_ContractNo.Text;
            vesselName = txt_VesselName.Text;

            return ShippingReportManager.Instance.getContainerManifestReport(voyageNo, departureDate, departurePort, vesselName, ContractNo, this.LogonUserId, outputFormat);
        }

        protected void btn_Print_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport("REPORT"), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Container Manifest Report");

        }


        protected void btn_ExportList_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport("TABULAR"), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "Container Manifest Report");
        }

        protected void btn_ExportReport_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport("REPORT"), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.Excel, "Container Manifest Report");
        }

        protected void ManifestDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ILSManifestDef manifest = (ILSManifestDef)vwSearchResult[e.Row.RowIndex];

                ImageButton lnk = (ImageButton)e.Row.Cells[0].Controls[1];
                lnk.Attributes.Add("onclick", "window.open('AWBDetail.aspx?ContainerNo=" + manifest.ContainerNo + "','ManifestDetail','width=700,height=500,scrollbars=1,resizable=1,status=1');return false;");                
            }
        }

        protected void gvAWBOnSort(object sender, GridViewSortEventArgs e)
        {
            if (sortExpression == e.SortExpression)
            {
                sortDirection = (sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending);
            }
            else
            {
                sortExpression = e.SortExpression;
                sortDirection = SortDirection.Ascending;
            }

            ILSManifestDef.ILSManifestComparer.CompareType compareType ;

            if (sortExpression == "DepartDate")
                compareType = ILSManifestDef.ILSManifestComparer.CompareType.DepartDate;
            else if (sortExpression == "VoyageNo")
                compareType = ILSManifestDef.ILSManifestComparer.CompareType.VoyageNo;
            else 
                compareType = ILSManifestDef.ILSManifestComparer.CompareType.ContainerNo;            

            vwSearchResult.Sort(new ILSManifestDef.ILSManifestComparer(compareType, sortDirection));
            gv_AWB.DataSource = vwSearchResult;
            gv_AWB.DataBind();
        }


    }
}
