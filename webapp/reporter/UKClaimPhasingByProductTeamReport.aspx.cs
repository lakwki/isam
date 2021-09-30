using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using com.next.infra.util;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.claim;
using com.next.common.web.commander;
using com.next.common.domain.types;
using com.next.isam.appserver.claim;

namespace com.next.isam.webapp.reporter
{
    public partial class UKClaimPhasingByProductTeamReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public struct MIME_Type
        {
            public const string Excel = "application/vnd.ms-excel";
            public const string Word = "application/vnd.ms-word";
            public const string PowerPoint = "application/vnd.ms-powerpoint";
        }

        private string productTeamName = string.Empty;
        private string currencyName = string.Empty;
        private int currencyId = 0;
        private int productTeamId = 0;
        private int officeId = 0;
        private int count;

        #region initialization
        private decimal ttlNSP01 = 0;
        private decimal ttlNSP02 = 0;
        private decimal ttlNSP03 = 0;
        private decimal ttlNSP04 = 0;
        private decimal ttlNSP05 = 0;
        private decimal ttlNSP06 = 0;
        private decimal ttlNSP07 = 0;
        private decimal ttlNSP08 = 0;
        private decimal ttlNSP09 = 0;
        private decimal ttlNSP10 = 0;
        private decimal ttlNSP11 = 0;
        private decimal ttlNSP12 = 0;
        private decimal ttlNSTotal = 0;

        private decimal ttlVendorP01 = 0;
        private decimal ttlVendorP02 = 0;
        private decimal ttlVendorP03 = 0;
        private decimal ttlVendorP04 = 0;
        private decimal ttlVendorP05 = 0;
        private decimal ttlVendorP06 = 0;
        private decimal ttlVendorP07 = 0;
        private decimal ttlVendorP08 = 0;
        private decimal ttlVendorP09 = 0;
        private decimal ttlVendorP10 = 0;
        private decimal ttlVendorP11 = 0;
        private decimal ttlVendorP12 = 0;
        private decimal ttlVendorTotal = 0;

        private decimal ttlLYNSP01 = 0;
        private decimal ttlLYNSP02 = 0;
        private decimal ttlLYNSP03 = 0;
        private decimal ttlLYNSP04 = 0;
        private decimal ttlLYNSP05 = 0;
        private decimal ttlLYNSP06 = 0;
        private decimal ttlLYNSP07 = 0;
        private decimal ttlLYNSP08 = 0;
        private decimal ttlLYNSP09 = 0;
        private decimal ttlLYNSP10 = 0;
        private decimal ttlLYNSP11 = 0;
        private decimal ttlLYNSP12 = 0;
        private decimal ttlLYNSTotal = 0;

        private decimal ttlLYVendorP01 = 0;
        private decimal ttlLYVendorP02 = 0;
        private decimal ttlLYVendorP03 = 0;
        private decimal ttlLYVendorP04 = 0;
        private decimal ttlLYVendorP05 = 0;
        private decimal ttlLYVendorP06 = 0;
        private decimal ttlLYVendorP07 = 0;
        private decimal ttlLYVendorP08 = 0;
        private decimal ttlLYVendorP09 = 0;
        private decimal ttlLYVendorP10 = 0;
        private decimal ttlLYVendorP11 = 0;
        private decimal ttlLYVendorP12 = 0;
        private decimal ttlLYVendorTotal = 0;

        private decimal ttlReworkP01 = 0;
        private decimal ttlReworkP02 = 0;
        private decimal ttlReworkP03 = 0;
        private decimal ttlReworkP04 = 0;
        private decimal ttlReworkP05 = 0;
        private decimal ttlReworkP06 = 0;
        private decimal ttlReworkP07 = 0;
        private decimal ttlReworkP08 = 0;
        private decimal ttlReworkP09 = 0;
        private decimal ttlReworkP10 = 0;
        private decimal ttlReworkP11 = 0;
        private decimal ttlReworkP12 = 0;
        private decimal ttlReworkTotal = 0;

        private decimal ttlLYReworkP01 = 0;
        private decimal ttlLYReworkP02 = 0;
        private decimal ttlLYReworkP03 = 0;
        private decimal ttlLYReworkP04 = 0;
        private decimal ttlLYReworkP05 = 0;
        private decimal ttlLYReworkP06 = 0;
        private decimal ttlLYReworkP07 = 0;
        private decimal ttlLYReworkP08 = 0;
        private decimal ttlLYReworkP09 = 0;
        private decimal ttlLYReworkP10 = 0;
        private decimal ttlLYReworkP11 = 0;
        private decimal ttlLYReworkP12 = 0;
        private decimal ttlLYReworkTotal = 0;

        private decimal ttlRejectP01 = 0;
        private decimal ttlRejectP02 = 0;
        private decimal ttlRejectP03 = 0;
        private decimal ttlRejectP04 = 0;
        private decimal ttlRejectP05 = 0;
        private decimal ttlRejectP06 = 0;
        private decimal ttlRejectP07 = 0;
        private decimal ttlRejectP08 = 0;
        private decimal ttlRejectP09 = 0;
        private decimal ttlRejectP10 = 0;
        private decimal ttlRejectP11 = 0;
        private decimal ttlRejectP12 = 0;
        private decimal ttlRejectTotal = 0;
        private decimal ttlLYRejectP01 = 0;
        private decimal ttlLYRejectP02 = 0;
        private decimal ttlLYRejectP03 = 0;
        private decimal ttlLYRejectP04 = 0;
        private decimal ttlLYRejectP05 = 0;
        private decimal ttlLYRejectP06 = 0;
        private decimal ttlLYRejectP07 = 0;
        private decimal ttlLYRejectP08 = 0;
        private decimal ttlLYRejectP09 = 0;
        private decimal ttlLYRejectP10 = 0;
        private decimal ttlLYRejectP11 = 0;
        private decimal ttlLYRejectP12 = 0;
        private decimal ttlLYRejectTotal = 0;

        private decimal ttlMFRNP01 = 0;
        private decimal ttlMFRNP02 = 0;
        private decimal ttlMFRNP03 = 0;
        private decimal ttlMFRNP04 = 0;
        private decimal ttlMFRNP05 = 0;
        private decimal ttlMFRNP06 = 0;
        private decimal ttlMFRNP07 = 0;
        private decimal ttlMFRNP08 = 0;
        private decimal ttlMFRNP09 = 0;
        private decimal ttlMFRNP10 = 0;
        private decimal ttlMFRNP11 = 0;
        private decimal ttlMFRNP12 = 0;
        private decimal ttlMFRNTotal = 0;
        private decimal ttlLYMFRNP01 = 0;
        private decimal ttlLYMFRNP02 = 0;
        private decimal ttlLYMFRNP03 = 0;
        private decimal ttlLYMFRNP04 = 0;
        private decimal ttlLYMFRNP05 = 0;
        private decimal ttlLYMFRNP06 = 0;
        private decimal ttlLYMFRNP07 = 0;
        private decimal ttlLYMFRNP08 = 0;
        private decimal ttlLYMFRNP09 = 0;
        private decimal ttlLYMFRNP10 = 0;
        private decimal ttlLYMFRNP11 = 0;
        private decimal ttlLYMFRNP12 = 0;
        private decimal ttlLYMFRNTotal = 0;

        private decimal ttlCFSP01 = 0;
        private decimal ttlCFSP02 = 0;
        private decimal ttlCFSP03 = 0;
        private decimal ttlCFSP04 = 0;
        private decimal ttlCFSP05 = 0;
        private decimal ttlCFSP06 = 0;
        private decimal ttlCFSP07 = 0;
        private decimal ttlCFSP08 = 0;
        private decimal ttlCFSP09 = 0;
        private decimal ttlCFSP10 = 0;
        private decimal ttlCFSP11 = 0;
        private decimal ttlCFSP12 = 0;
        private decimal ttlCFSTotal = 0;
        private decimal ttlLYCFSP01 = 0;
        private decimal ttlLYCFSP02 = 0;
        private decimal ttlLYCFSP03 = 0;
        private decimal ttlLYCFSP04 = 0;
        private decimal ttlLYCFSP05 = 0;
        private decimal ttlLYCFSP06 = 0;
        private decimal ttlLYCFSP07 = 0;
        private decimal ttlLYCFSP08 = 0;
        private decimal ttlLYCFSP09 = 0;
        private decimal ttlLYCFSP10 = 0;
        private decimal ttlLYCFSP11 = 0;
        private decimal ttlLYCFSP12 = 0;
        private decimal ttlLYCFSTotal = 0;

        private decimal ttlSafetyP01 = 0;
        private decimal ttlSafetyP02 = 0;
        private decimal ttlSafetyP03 = 0;
        private decimal ttlSafetyP04 = 0;
        private decimal ttlSafetyP05 = 0;
        private decimal ttlSafetyP06 = 0;
        private decimal ttlSafetyP07 = 0;
        private decimal ttlSafetyP08 = 0;
        private decimal ttlSafetyP09 = 0;
        private decimal ttlSafetyP10 = 0;
        private decimal ttlSafetyP11 = 0;
        private decimal ttlSafetyP12 = 0;
        private decimal ttlSafetyTotal = 0;
        private decimal ttlLYSafetyP01 = 0;
        private decimal ttlLYSafetyP02 = 0;
        private decimal ttlLYSafetyP03 = 0;
        private decimal ttlLYSafetyP04 = 0;
        private decimal ttlLYSafetyP05 = 0;
        private decimal ttlLYSafetyP06 = 0;
        private decimal ttlLYSafetyP07 = 0;
        private decimal ttlLYSafetyP08 = 0;
        private decimal ttlLYSafetyP09 = 0;
        private decimal ttlLYSafetyP10 = 0;
        private decimal ttlLYSafetyP11 = 0;
        private decimal ttlLYSafetyP12 = 0;
        private decimal ttlLYSafetyTotal = 0;

        private decimal ttlAuditP01 = 0;
        private decimal ttlAuditP02 = 0;
        private decimal ttlAuditP03 = 0;
        private decimal ttlAuditP04 = 0;
        private decimal ttlAuditP05 = 0;
        private decimal ttlAuditP06 = 0;
        private decimal ttlAuditP07 = 0;
        private decimal ttlAuditP08 = 0;
        private decimal ttlAuditP09 = 0;
        private decimal ttlAuditP10 = 0;
        private decimal ttlAuditP11 = 0;
        private decimal ttlAuditP12 = 0;
        private decimal ttlAuditTotal = 0;
        private decimal ttlLYAuditP01 = 0;
        private decimal ttlLYAuditP02 = 0;
        private decimal ttlLYAuditP03 = 0;
        private decimal ttlLYAuditP04 = 0;
        private decimal ttlLYAuditP05 = 0;
        private decimal ttlLYAuditP06 = 0;
        private decimal ttlLYAuditP07 = 0;
        private decimal ttlLYAuditP08 = 0;
        private decimal ttlLYAuditP09 = 0;
        private decimal ttlLYAuditP10 = 0;
        private decimal ttlLYAuditP11 = 0;
        private decimal ttlLYAuditP12 = 0;
        private decimal ttlLYAuditTotal = 0;

        private decimal ttlFabricP01 = 0;
        private decimal ttlFabricP02 = 0;
        private decimal ttlFabricP03 = 0;
        private decimal ttlFabricP04 = 0;
        private decimal ttlFabricP05 = 0;
        private decimal ttlFabricP06 = 0;
        private decimal ttlFabricP07 = 0;
        private decimal ttlFabricP08 = 0;
        private decimal ttlFabricP09 = 0;
        private decimal ttlFabricP10 = 0;
        private decimal ttlFabricP11 = 0;
        private decimal ttlFabricP12 = 0;
        private decimal ttlFabricTotal = 0;
        private decimal ttlLYFabricP01 = 0;
        private decimal ttlLYFabricP02 = 0;
        private decimal ttlLYFabricP03 = 0;
        private decimal ttlLYFabricP04 = 0;
        private decimal ttlLYFabricP05 = 0;
        private decimal ttlLYFabricP06 = 0;
        private decimal ttlLYFabricP07 = 0;
        private decimal ttlLYFabricP08 = 0;
        private decimal ttlLYFabricP09 = 0;
        private decimal ttlLYFabricP10 = 0;
        private decimal ttlLYFabricP11 = 0;
        private decimal ttlLYFabricP12 = 0;
        private decimal ttlLYFabricTotal = 0;

        private decimal ttlPenaltyP01 = 0;
        private decimal ttlPenaltyP02 = 0;
        private decimal ttlPenaltyP03 = 0;
        private decimal ttlPenaltyP04 = 0;
        private decimal ttlPenaltyP05 = 0;
        private decimal ttlPenaltyP06 = 0;
        private decimal ttlPenaltyP07 = 0;
        private decimal ttlPenaltyP08 = 0;
        private decimal ttlPenaltyP09 = 0;
        private decimal ttlPenaltyP10 = 0;
        private decimal ttlPenaltyP11 = 0;
        private decimal ttlPenaltyP12 = 0;
        private decimal ttlPenaltyTotal = 0;
        private decimal ttlLYPenaltyP01 = 0;
        private decimal ttlLYPenaltyP02 = 0;
        private decimal ttlLYPenaltyP03 = 0;
        private decimal ttlLYPenaltyP04 = 0;
        private decimal ttlLYPenaltyP05 = 0;
        private decimal ttlLYPenaltyP06 = 0;
        private decimal ttlLYPenaltyP07 = 0;
        private decimal ttlLYPenaltyP08 = 0;
        private decimal ttlLYPenaltyP09 = 0;
        private decimal ttlLYPenaltyP10 = 0;
        private decimal ttlLYPenaltyP11 = 0;
        private decimal ttlLYPenaltyP12 = 0;
        private decimal ttlLYPenaltyTotal = 0;

        private decimal ttlQCCP01 = 0;
        private decimal ttlQCCP02 = 0;
        private decimal ttlQCCP03 = 0;
        private decimal ttlQCCP04 = 0;
        private decimal ttlQCCP05 = 0;
        private decimal ttlQCCP06 = 0;
        private decimal ttlQCCP07 = 0;
        private decimal ttlQCCP08 = 0;
        private decimal ttlQCCP09 = 0;
        private decimal ttlQCCP10 = 0;
        private decimal ttlQCCP11 = 0;
        private decimal ttlQCCP12 = 0;
        private decimal ttlQCCTotal = 0;
        private decimal ttlLYQCCP01 = 0;
        private decimal ttlLYQCCP02 = 0;
        private decimal ttlLYQCCP03 = 0;
        private decimal ttlLYQCCP04 = 0;
        private decimal ttlLYQCCP05 = 0;
        private decimal ttlLYQCCP06 = 0;
        private decimal ttlLYQCCP07 = 0;
        private decimal ttlLYQCCP08 = 0;
        private decimal ttlLYQCCP09 = 0;
        private decimal ttlLYQCCP10 = 0;
        private decimal ttlLYQCCP11 = 0;
        private decimal ttlLYQCCP12 = 0;
        private decimal ttlLYQCCTotal = 0;

        private decimal ttlCHBP01 = 0;
        private decimal ttlCHBP02 = 0;
        private decimal ttlCHBP03 = 0;
        private decimal ttlCHBP04 = 0;
        private decimal ttlCHBP05 = 0;
        private decimal ttlCHBP06 = 0;
        private decimal ttlCHBP07 = 0;
        private decimal ttlCHBP08 = 0;
        private decimal ttlCHBP09 = 0;
        private decimal ttlCHBP10 = 0;
        private decimal ttlCHBP11 = 0;
        private decimal ttlCHBP12 = 0;
        private decimal ttlCHBTotal = 0;
        private decimal ttlLYCHBP01 = 0;
        private decimal ttlLYCHBP02 = 0;
        private decimal ttlLYCHBP03 = 0;
        private decimal ttlLYCHBP04 = 0;
        private decimal ttlLYCHBP05 = 0;
        private decimal ttlLYCHBP06 = 0;
        private decimal ttlLYCHBP07 = 0;
        private decimal ttlLYCHBP08 = 0;
        private decimal ttlLYCHBP09 = 0;
        private decimal ttlLYCHBP10 = 0;
        private decimal ttlLYCHBP11 = 0;
        private decimal ttlLYCHBP12 = 0;
        private decimal ttlLYCHBTotal = 0;

        private decimal ttlGBTestP01 = 0;
        private decimal ttlGBTestP02 = 0;
        private decimal ttlGBTestP03 = 0;
        private decimal ttlGBTestP04 = 0;
        private decimal ttlGBTestP05 = 0;
        private decimal ttlGBTestP06 = 0;
        private decimal ttlGBTestP07 = 0;
        private decimal ttlGBTestP08 = 0;
        private decimal ttlGBTestP09 = 0;
        private decimal ttlGBTestP10 = 0;
        private decimal ttlGBTestP11 = 0;
        private decimal ttlGBTestP12 = 0;
        private decimal ttlGBTestTotal = 0;
        private decimal ttlLYGBTestP01 = 0;
        private decimal ttlLYGBTestP02 = 0;
        private decimal ttlLYGBTestP03 = 0;
        private decimal ttlLYGBTestP04 = 0;
        private decimal ttlLYGBTestP05 = 0;
        private decimal ttlLYGBTestP06 = 0;
        private decimal ttlLYGBTestP07 = 0;
        private decimal ttlLYGBTestP08 = 0;
        private decimal ttlLYGBTestP09 = 0;
        private decimal ttlLYGBTestP10 = 0;
        private decimal ttlLYGBTestP11 = 0;
        private decimal ttlLYGBTestP12 = 0;
        private decimal ttlLYGBTestTotal = 0;

        private decimal ttlFIRAP01 = 0;
        private decimal ttlFIRAP02 = 0;
        private decimal ttlFIRAP03 = 0;
        private decimal ttlFIRAP04 = 0;
        private decimal ttlFIRAP05 = 0;
        private decimal ttlFIRAP06 = 0;
        private decimal ttlFIRAP07 = 0;
        private decimal ttlFIRAP08 = 0;
        private decimal ttlFIRAP09 = 0;
        private decimal ttlFIRAP10 = 0;
        private decimal ttlFIRAP11 = 0;
        private decimal ttlFIRAP12 = 0;
        private decimal ttlFIRATotal = 0;
        private decimal ttlLYFIRAP01 = 0;
        private decimal ttlLYFIRAP02 = 0;
        private decimal ttlLYFIRAP03 = 0;
        private decimal ttlLYFIRAP04 = 0;
        private decimal ttlLYFIRAP05 = 0;
        private decimal ttlLYFIRAP06 = 0;
        private decimal ttlLYFIRAP07 = 0;
        private decimal ttlLYFIRAP08 = 0;
        private decimal ttlLYFIRAP09 = 0;
        private decimal ttlLYFIRAP10 = 0;
        private decimal ttlLYFIRAP11 = 0;
        private decimal ttlLYFIRAP12 = 0;
        private decimal ttlLYFIRATotal = 0;


        private decimal ttlOthersP01 = 0;
        private decimal ttlOthersP02 = 0;
        private decimal ttlOthersP03 = 0;
        private decimal ttlOthersP04 = 0;
        private decimal ttlOthersP05 = 0;
        private decimal ttlOthersP06 = 0;
        private decimal ttlOthersP07 = 0;
        private decimal ttlOthersP08 = 0;
        private decimal ttlOthersP09 = 0;
        private decimal ttlOthersP10 = 0;
        private decimal ttlOthersP11 = 0;
        private decimal ttlOthersP12 = 0;
        private decimal ttlOthersTotal = 0;
        private decimal ttlLYOthersP01 = 0;
        private decimal ttlLYOthersP02 = 0;
        private decimal ttlLYOthersP03 = 0;
        private decimal ttlLYOthersP04 = 0;
        private decimal ttlLYOthersP05 = 0;
        private decimal ttlLYOthersP06 = 0;
        private decimal ttlLYOthersP07 = 0;
        private decimal ttlLYOthersP08 = 0;
        private decimal ttlLYOthersP09 = 0;
        private decimal ttlLYOthersP10 = 0;
        private decimal ttlLYOthersP11 = 0;
        private decimal ttlLYOthersP12 = 0;
        private decimal ttlLYOthersTotal = 0;

        private decimal ttlSubP01 = 0;
        private decimal ttlSubP02 = 0;
        private decimal ttlSubP03 = 0;
        private decimal ttlSubP04 = 0;
        private decimal ttlSubP05 = 0;
        private decimal ttlSubP06 = 0;
        private decimal ttlSubP07 = 0;
        private decimal ttlSubP08 = 0;
        private decimal ttlSubP09 = 0;
        private decimal ttlSubP10 = 0;
        private decimal ttlSubP11 = 0;
        private decimal ttlSubP12 = 0;
        private decimal ttlSubTotal = 0;

        private decimal ttlOfficeSubP01 = 0;
        private decimal ttlOfficeSubP02 = 0;
        private decimal ttlOfficeSubP03 = 0;
        private decimal ttlOfficeSubP04 = 0;
        private decimal ttlOfficeSubP05 = 0;
        private decimal ttlOfficeSubP06 = 0;
        private decimal ttlOfficeSubP07 = 0;
        private decimal ttlOfficeSubP08 = 0;
        private decimal ttlOfficeSubP09 = 0;
        private decimal ttlOfficeSubP10 = 0;
        private decimal ttlOfficeSubP11 = 0;
        private decimal ttlOfficeSubP12 = 0;
        private decimal ttlOfficeSubTotal = 0;

        private decimal ttlOfficeGrandP01 = 0;
        private decimal ttlOfficeGrandP02 = 0;
        private decimal ttlOfficeGrandP03 = 0;
        private decimal ttlOfficeGrandP04 = 0;
        private decimal ttlOfficeGrandP05 = 0;
        private decimal ttlOfficeGrandP06 = 0;
        private decimal ttlOfficeGrandP07 = 0;
        private decimal ttlOfficeGrandP08 = 0;
        private decimal ttlOfficeGrandP09 = 0;
        private decimal ttlOfficeGrandP10 = 0;
        private decimal ttlOfficeGrandP11 = 0;
        private decimal ttlOfficeGrandP12 = 0;
        private decimal ttlOfficeGrandTotal = 0;

        #endregion Initialization

        bool isAlternate = false;
        bool isFirst = true;
        bool isSummary = false;
        DateTime shipmentDate = DateTime.MinValue;
        bool isSingleVendor;

        string param_FiscalYear;
        string param_Period;
        string param_VendorId;
        string param_OfficeId;
        string param_ReportType;
        string param_UserId;
        string param_ReportCode;

        List<UKClaimPhasingByProductTeamDef> list = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.EnableViewState = false;
                Response.Clear();
                Response.Charset = "";

                Response.Buffer = true;
                Response.ContentType = MIME_Type.Excel;
                Response.AddHeader("Content-Disposition", "attachment;filename=UKClaimPhasingReport.xls");

                param_OfficeId = Request.Params["officeId"];
                param_Period = Request.Params["period"];
                param_FiscalYear = Request.Params["fiscalYear"];
                param_ReportType = Request.Params["docType"];
                param_VendorId = Request.Params["vendorId"];
                param_UserId = Request.Params["userId"];
                param_ReportCode = Request.Params["reportCode"];
 
                if (param_OfficeId == null && param_Period == null && param_FiscalYear == null && param_ReportType == null && param_VendorId == null)
                {
                    param_OfficeId = Context.Items[AccountCommander.Param.officeId].ToString();
                    param_Period = Context.Items[AccountCommander.Param.period].ToString();
                    param_FiscalYear = Context.Items[AccountCommander.Param.fiscalYear].ToString();
                    param_ReportType = Context.Items[AccountCommander.Param.docType].ToString();
                    param_VendorId = Context.Items[AccountCommander.Param.vendorId].ToString();
                    param_UserId = (this.LogonUserId == int.MinValue ? -1 : this.LogonUserId).ToString();
                    list = (List<UKClaimPhasingByProductTeamDef>)Context.Items[AccountCommander.Param.ukClaimPhasingList];
                }
                string[] period = (param_Period == null ? "0,0".Split(',') : param_Period.Split(','));
                int vendorId = int.Parse(param_VendorId);
                isSingleVendor = (vendorId != -1);
                isSummary = (param_ReportType == "2");
                if (list == null)
                    list = UKClaimManager.Instance.getUKClaimPhasingByProductTeamReport(Int32.Parse(param_FiscalYear), Int32.Parse(period[0]), Int32.Parse(period[1]), int.Parse(param_OfficeId), vendorId);

                if (string.IsNullOrEmpty(param_ReportCode))
                    tr_ReportCode.Style.Add("Display", "none");
                this.lbl_ReportCode.Text = param_ReportCode;
                this.lblPrintTime.Text = "Print Date: " + DateTime.Today.ToShortDateString() + "  " + DateTime.Now.ToShortTimeString();
                this.lblPrintTime.Text += "<br>Print By: " + (param_UserId == "-1" ? "" : CommonUtil.getUserByKey(Int32.Parse(param_UserId)).DisplayName);
                this.lbl_Office.Text = (param_OfficeId == "-1" ? "ALL" : OfficeId.getName(int.Parse(param_OfficeId)));
                this.lbl_Period.Text = "Year " + param_FiscalYear + "  P" + period[0] + " - P" + period[1];
                this.lbl_ReportType.Text = (isSummary ? "Summary" : "Detail");
                this.lbl_Supplier.Text = (vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);

                this.getReport();
            }
        }

        private void getReport()
        {
            //List<UKClaimPhasingByProductTeamDef> list = (List<UKClaimPhasingByProductTeamDef>)Context.Items[AccountCommander.Param.ukClaimPhasingList];

            this.vwSearchResult = list;

            this.repUKClaim.DataSource = list;
            this.repUKClaim.DataBind();

            #region Report Summary

            this.lbl_ReworkP1.Text = ttlReworkP01.ToString("#,##0");
            this.lbl_ReworkP2.Text = ttlReworkP02.ToString("#,##0");
            this.lbl_ReworkP3.Text = ttlReworkP03.ToString("#,##0");
            this.lbl_ReworkP4.Text = ttlReworkP04.ToString("#,##0");
            this.lbl_ReworkP5.Text = ttlReworkP05.ToString("#,##0");
            this.lbl_ReworkP6.Text = ttlReworkP06.ToString("#,##0");
            this.lbl_ReworkP7.Text = ttlReworkP07.ToString("#,##0");
            this.lbl_ReworkP8.Text = ttlReworkP08.ToString("#,##0");
            this.lbl_ReworkP9.Text = ttlReworkP09.ToString("#,##0");
            this.lbl_ReworkP10.Text = ttlReworkP10.ToString("#,##0");
            this.lbl_ReworkP11.Text = ttlReworkP11.ToString("#,##0");
            this.lbl_ReworkP12.Text = ttlReworkP12.ToString("#,##0");
            this.lbl_ReworkTotal.Text = ttlReworkTotal.ToString("#,##0");

            this.lbl_RejectP1.Text = ttlRejectP01.ToString("#,##0");
            this.lbl_RejectP2.Text = ttlRejectP02.ToString("#,##0");
            this.lbl_RejectP3.Text = ttlRejectP03.ToString("#,##0");
            this.lbl_RejectP4.Text = ttlRejectP04.ToString("#,##0");
            this.lbl_RejectP5.Text = ttlRejectP05.ToString("#,##0");
            this.lbl_RejectP6.Text = ttlRejectP06.ToString("#,##0");
            this.lbl_RejectP7.Text = ttlRejectP07.ToString("#,##0");
            this.lbl_RejectP8.Text = ttlRejectP08.ToString("#,##0");
            this.lbl_RejectP9.Text = ttlRejectP09.ToString("#,##0");
            this.lbl_RejectP10.Text = ttlRejectP10.ToString("#,##0");
            this.lbl_RejectP11.Text = ttlRejectP11.ToString("#,##0");
            this.lbl_RejectP12.Text = ttlRejectP12.ToString("#,##0");
            this.lbl_RejectTotal.Text = ttlRejectTotal.ToString("#,##0");

            this.lbl_MFRNP1.Text = ttlMFRNP01.ToString("#,##0");
            this.lbl_MFRNP2.Text = ttlMFRNP02.ToString("#,##0");
            this.lbl_MFRNP3.Text = ttlMFRNP03.ToString("#,##0");
            this.lbl_MFRNP4.Text = ttlMFRNP04.ToString("#,##0");
            this.lbl_MFRNP5.Text = ttlMFRNP05.ToString("#,##0");
            this.lbl_MFRNP6.Text = ttlMFRNP06.ToString("#,##0");
            this.lbl_MFRNP7.Text = ttlMFRNP07.ToString("#,##0");
            this.lbl_MFRNP8.Text = ttlMFRNP08.ToString("#,##0");
            this.lbl_MFRNP9.Text = ttlMFRNP09.ToString("#,##0");
            this.lbl_MFRNP10.Text = ttlMFRNP10.ToString("#,##0");
            this.lbl_MFRNP11.Text = ttlMFRNP11.ToString("#,##0");
            this.lbl_MFRNP12.Text = ttlMFRNP12.ToString("#,##0");
            this.lbl_MFRNTotal.Text = ttlMFRNTotal.ToString("#,##0");

            this.lbl_CFSP1.Text = ttlCFSP01.ToString("#,##0");
            this.lbl_CFSP2.Text = ttlCFSP02.ToString("#,##0");
            this.lbl_CFSP3.Text = ttlCFSP03.ToString("#,##0");
            this.lbl_CFSP4.Text = ttlCFSP04.ToString("#,##0");
            this.lbl_CFSP5.Text = ttlCFSP05.ToString("#,##0");
            this.lbl_CFSP6.Text = ttlCFSP06.ToString("#,##0");
            this.lbl_CFSP7.Text = ttlCFSP07.ToString("#,##0");
            this.lbl_CFSP8.Text = ttlCFSP08.ToString("#,##0");
            this.lbl_CFSP9.Text = ttlCFSP09.ToString("#,##0");
            this.lbl_CFSP10.Text = ttlCFSP10.ToString("#,##0");
            this.lbl_CFSP11.Text = ttlCFSP11.ToString("#,##0");
            this.lbl_CFSP12.Text = ttlCFSP12.ToString("#,##0");
            this.lbl_CFSTotal.Text = ttlCFSTotal.ToString("#,##0");

            this.lbl_SafetyP1.Text = ttlSafetyP01.ToString("#,##0");
            this.lbl_SafetyP2.Text = ttlSafetyP02.ToString("#,##0");
            this.lbl_SafetyP3.Text = ttlSafetyP03.ToString("#,##0");
            this.lbl_SafetyP4.Text = ttlSafetyP04.ToString("#,##0");
            this.lbl_SafetyP5.Text = ttlSafetyP05.ToString("#,##0");
            this.lbl_SafetyP6.Text = ttlSafetyP06.ToString("#,##0");
            this.lbl_SafetyP7.Text = ttlSafetyP07.ToString("#,##0");
            this.lbl_SafetyP8.Text = ttlSafetyP08.ToString("#,##0");
            this.lbl_SafetyP9.Text = ttlSafetyP09.ToString("#,##0");
            this.lbl_SafetyP10.Text = ttlSafetyP10.ToString("#,##0");
            this.lbl_SafetyP11.Text = ttlSafetyP11.ToString("#,##0");
            this.lbl_SafetyP12.Text = ttlSafetyP12.ToString("#,##0");
            this.lbl_SafetyTotal.Text = ttlSafetyTotal.ToString("#,##0");

            this.lbl_AuditP1.Text = ttlAuditP01.ToString("#,##0");
            this.lbl_AuditP2.Text = ttlAuditP02.ToString("#,##0");
            this.lbl_AuditP3.Text = ttlAuditP03.ToString("#,##0");
            this.lbl_AuditP4.Text = ttlAuditP04.ToString("#,##0");
            this.lbl_AuditP5.Text = ttlAuditP05.ToString("#,##0");
            this.lbl_AuditP6.Text = ttlAuditP06.ToString("#,##0");
            this.lbl_AuditP7.Text = ttlAuditP07.ToString("#,##0");
            this.lbl_AuditP8.Text = ttlAuditP08.ToString("#,##0");
            this.lbl_AuditP9.Text = ttlAuditP09.ToString("#,##0");
            this.lbl_AuditP10.Text = ttlAuditP10.ToString("#,##0");
            this.lbl_AuditP11.Text = ttlAuditP11.ToString("#,##0");
            this.lbl_AuditP12.Text = ttlAuditP12.ToString("#,##0");
            this.lbl_AuditTotal.Text = ttlAuditTotal.ToString("#,##0");

            this.lbl_FabricP1.Text = ttlFabricP01.ToString("#,##0");
            this.lbl_FabricP2.Text = ttlFabricP02.ToString("#,##0");
            this.lbl_FabricP3.Text = ttlFabricP03.ToString("#,##0");
            this.lbl_FabricP4.Text = ttlFabricP04.ToString("#,##0");
            this.lbl_FabricP5.Text = ttlFabricP05.ToString("#,##0");
            this.lbl_FabricP6.Text = ttlFabricP06.ToString("#,##0");
            this.lbl_FabricP7.Text = ttlFabricP07.ToString("#,##0");
            this.lbl_FabricP8.Text = ttlFabricP08.ToString("#,##0");
            this.lbl_FabricP9.Text = ttlFabricP09.ToString("#,##0");
            this.lbl_FabricP10.Text = ttlFabricP10.ToString("#,##0");
            this.lbl_FabricP11.Text = ttlFabricP11.ToString("#,##0");
            this.lbl_FabricP12.Text = ttlFabricP12.ToString("#,##0");
            this.lbl_FabricTotal.Text = ttlFabricTotal.ToString("#,##0");

            this.lbl_PenaltyP1.Text = ttlPenaltyP01.ToString("#,##0");
            this.lbl_PenaltyP2.Text = ttlPenaltyP02.ToString("#,##0");
            this.lbl_PenaltyP3.Text = ttlPenaltyP03.ToString("#,##0");
            this.lbl_PenaltyP4.Text = ttlPenaltyP04.ToString("#,##0");
            this.lbl_PenaltyP5.Text = ttlPenaltyP05.ToString("#,##0");
            this.lbl_PenaltyP6.Text = ttlPenaltyP06.ToString("#,##0");
            this.lbl_PenaltyP7.Text = ttlPenaltyP07.ToString("#,##0");
            this.lbl_PenaltyP8.Text = ttlPenaltyP08.ToString("#,##0");
            this.lbl_PenaltyP9.Text = ttlPenaltyP09.ToString("#,##0");
            this.lbl_PenaltyP10.Text = ttlPenaltyP10.ToString("#,##0");
            this.lbl_PenaltyP11.Text = ttlPenaltyP11.ToString("#,##0");
            this.lbl_PenaltyP12.Text = ttlPenaltyP12.ToString("#,##0");
            this.lbl_PenaltyTotal.Text = ttlPenaltyTotal.ToString("#,##0");

            this.lbl_QCCP1.Text = ttlQCCP01.ToString("#,##0");
            this.lbl_QCCP2.Text = ttlQCCP02.ToString("#,##0");
            this.lbl_QCCP3.Text = ttlQCCP03.ToString("#,##0");
            this.lbl_QCCP4.Text = ttlQCCP04.ToString("#,##0");
            this.lbl_QCCP5.Text = ttlQCCP05.ToString("#,##0");
            this.lbl_QCCP6.Text = ttlQCCP06.ToString("#,##0");
            this.lbl_QCCP7.Text = ttlQCCP07.ToString("#,##0");
            this.lbl_QCCP8.Text = ttlQCCP08.ToString("#,##0");
            this.lbl_QCCP9.Text = ttlQCCP09.ToString("#,##0");
            this.lbl_QCCP10.Text = ttlQCCP10.ToString("#,##0");
            this.lbl_QCCP11.Text = ttlQCCP11.ToString("#,##0");
            this.lbl_QCCP12.Text = ttlQCCP12.ToString("#,##0");
            this.lbl_QCCTotal.Text = ttlQCCTotal.ToString("#,##0");

            this.lbl_CHBP1.Text = ttlCHBP01.ToString("#,##0");
            this.lbl_CHBP2.Text = ttlCHBP02.ToString("#,##0");
            this.lbl_CHBP3.Text = ttlCHBP03.ToString("#,##0");
            this.lbl_CHBP4.Text = ttlCHBP04.ToString("#,##0");
            this.lbl_CHBP5.Text = ttlCHBP05.ToString("#,##0");
            this.lbl_CHBP6.Text = ttlCHBP06.ToString("#,##0");
            this.lbl_CHBP7.Text = ttlCHBP07.ToString("#,##0");
            this.lbl_CHBP8.Text = ttlCHBP08.ToString("#,##0");
            this.lbl_CHBP9.Text = ttlCHBP09.ToString("#,##0");
            this.lbl_CHBP10.Text = ttlCHBP10.ToString("#,##0");
            this.lbl_CHBP11.Text = ttlCHBP11.ToString("#,##0");
            this.lbl_CHBP12.Text = ttlCHBP12.ToString("#,##0");
            this.lbl_CHBTotal.Text = ttlCHBTotal.ToString("#,##0");

            this.lbl_GBTestP1.Text = ttlGBTestP01.ToString("#,##0");
            this.lbl_GBTestP2.Text = ttlGBTestP02.ToString("#,##0");
            this.lbl_GBTestP3.Text = ttlGBTestP03.ToString("#,##0");
            this.lbl_GBTestP4.Text = ttlGBTestP04.ToString("#,##0");
            this.lbl_GBTestP5.Text = ttlGBTestP05.ToString("#,##0");
            this.lbl_GBTestP6.Text = ttlGBTestP06.ToString("#,##0");
            this.lbl_GBTestP7.Text = ttlGBTestP07.ToString("#,##0");
            this.lbl_GBTestP8.Text = ttlGBTestP08.ToString("#,##0");
            this.lbl_GBTestP9.Text = ttlGBTestP09.ToString("#,##0");
            this.lbl_GBTestP10.Text = ttlGBTestP10.ToString("#,##0");
            this.lbl_GBTestP11.Text = ttlGBTestP11.ToString("#,##0");
            this.lbl_GBTestP12.Text = ttlGBTestP12.ToString("#,##0");
            this.lbl_GBTestTotal.Text = ttlGBTestTotal.ToString("#,##0");

            this.lbl_FIRAP1.Text = ttlFIRAP01.ToString("#,##0");
            this.lbl_FIRAP2.Text = ttlFIRAP02.ToString("#,##0");
            this.lbl_FIRAP3.Text = ttlFIRAP03.ToString("#,##0");
            this.lbl_FIRAP4.Text = ttlFIRAP04.ToString("#,##0");
            this.lbl_FIRAP5.Text = ttlFIRAP05.ToString("#,##0");
            this.lbl_FIRAP6.Text = ttlFIRAP06.ToString("#,##0");
            this.lbl_FIRAP7.Text = ttlFIRAP07.ToString("#,##0");
            this.lbl_FIRAP8.Text = ttlFIRAP08.ToString("#,##0");
            this.lbl_FIRAP9.Text = ttlFIRAP09.ToString("#,##0");
            this.lbl_FIRAP10.Text = ttlFIRAP10.ToString("#,##0");
            this.lbl_FIRAP11.Text = ttlFIRAP11.ToString("#,##0");
            this.lbl_FIRAP12.Text = ttlFIRAP12.ToString("#,##0");
            this.lbl_FIRATotal.Text = ttlFIRATotal.ToString("#,##0");

            this.lbl_OthersP1.Text = ttlOthersP01.ToString("#,##0");
            this.lbl_OthersP2.Text = ttlOthersP02.ToString("#,##0");
            this.lbl_OthersP3.Text = ttlOthersP03.ToString("#,##0");
            this.lbl_OthersP4.Text = ttlOthersP04.ToString("#,##0");
            this.lbl_OthersP5.Text = ttlOthersP05.ToString("#,##0");
            this.lbl_OthersP6.Text = ttlOthersP06.ToString("#,##0");
            this.lbl_OthersP7.Text = ttlOthersP07.ToString("#,##0");
            this.lbl_OthersP8.Text = ttlOthersP08.ToString("#,##0");
            this.lbl_OthersP9.Text = ttlOthersP09.ToString("#,##0");
            this.lbl_OthersP10.Text = ttlOthersP10.ToString("#,##0");
            this.lbl_OthersP11.Text = ttlOthersP11.ToString("#,##0");
            this.lbl_OthersP12.Text = ttlOthersP12.ToString("#,##0");
            this.lbl_OthersTotal.Text = ttlOthersTotal.ToString("#,##0");


            this.lbl_GrandP1.Text = (ttlReworkP01 + ttlRejectP01 + ttlMFRNP01 + ttlCFSP01 + ttlSafetyP01 + ttlAuditP01 + ttlFabricP01 + ttlPenaltyP01 + ttlQCCP01 + ttlCHBP01 + ttlGBTestP01 + ttlFIRAP01 + ttlOthersP01).ToString("#,##0");
            this.lbl_GrandP2.Text = (ttlReworkP02 + ttlRejectP02 + ttlMFRNP02 + ttlCFSP02 + ttlSafetyP02 + ttlAuditP02 + ttlFabricP02 + ttlPenaltyP02 + ttlQCCP02 + ttlCHBP02 + ttlGBTestP02 + ttlFIRAP02 + ttlOthersP02).ToString("#,##0");
            this.lbl_GrandP3.Text = (ttlReworkP03 + ttlRejectP03 + ttlMFRNP03 + ttlCFSP03 + ttlSafetyP03 + ttlAuditP03 + ttlFabricP03 + ttlPenaltyP03 + ttlQCCP03 + ttlCHBP03 + ttlGBTestP03 + ttlFIRAP03 + ttlOthersP03).ToString("#,##0");
            this.lbl_GrandP4.Text = (ttlReworkP04 + ttlRejectP04 + ttlMFRNP04 + ttlCFSP04 + ttlSafetyP04 + ttlAuditP04 + ttlFabricP04 + ttlPenaltyP04 + ttlQCCP04 + ttlCHBP04 + ttlGBTestP04 + ttlFIRAP04 + ttlOthersP04).ToString("#,##0");
            this.lbl_GrandP5.Text = (ttlReworkP05 + ttlRejectP05 + ttlMFRNP05 + ttlCFSP05 + ttlSafetyP05 + ttlAuditP05 + ttlFabricP05 + ttlPenaltyP05 + ttlQCCP05 + ttlCHBP05 + ttlGBTestP05 + ttlFIRAP05 + ttlOthersP05).ToString("#,##0");
            this.lbl_GrandP6.Text = (ttlReworkP06 + ttlRejectP06 + ttlMFRNP06 + ttlCFSP06 + ttlSafetyP06 + ttlAuditP06 + ttlFabricP06 + ttlPenaltyP06 + ttlQCCP06 + ttlCHBP06 + ttlGBTestP06 + ttlFIRAP06 + ttlOthersP06).ToString("#,##0");
            this.lbl_GrandP7.Text = (ttlReworkP07 + ttlRejectP07 + ttlMFRNP07 + ttlCFSP07 + ttlSafetyP07 + ttlAuditP07 + ttlFabricP07 + ttlPenaltyP07 + ttlQCCP07 + ttlCHBP07 + ttlGBTestP07 + ttlFIRAP07 + ttlOthersP07).ToString("#,##0");
            this.lbl_GrandP8.Text = (ttlReworkP08 + ttlRejectP08 + ttlMFRNP08 + ttlCFSP08 + ttlSafetyP08 + ttlAuditP08 + ttlFabricP08 + ttlPenaltyP08 + ttlQCCP08 + ttlCHBP08 + ttlGBTestP08 + ttlFIRAP08 + ttlOthersP08).ToString("#,##0");
            this.lbl_GrandP9.Text = (ttlReworkP09 + ttlRejectP09 + ttlMFRNP09 + ttlCFSP09 + ttlSafetyP09 + ttlAuditP09 + ttlFabricP09 + ttlPenaltyP09 + ttlQCCP09 + ttlCHBP09 + ttlGBTestP09 + ttlFIRAP09 + ttlOthersP09).ToString("#,##0");
            this.lbl_GrandP10.Text = (ttlReworkP10 + ttlRejectP10 + ttlMFRNP10 + ttlCFSP10 + ttlSafetyP10 + ttlAuditP10 + ttlFabricP10 + ttlPenaltyP10 + ttlQCCP10 + ttlCHBP10 + ttlGBTestP10 + ttlFIRAP10 + ttlOthersP10).ToString("#,##0");
            this.lbl_GrandP11.Text = (ttlReworkP11 + ttlRejectP11 + ttlMFRNP11 + ttlCFSP11 + ttlSafetyP11 + ttlAuditP11 + ttlFabricP11 + ttlPenaltyP11 + ttlQCCP11 + ttlCHBP11 + ttlGBTestP11 + ttlFIRAP11 + ttlOthersP11).ToString("#,##0");
            this.lbl_GrandP12.Text = (ttlReworkP12 + ttlRejectP12 + ttlMFRNP12 + ttlCFSP12 + ttlSafetyP12 + ttlAuditP12 + ttlFabricP12 + ttlPenaltyP12 + ttlQCCP12 + ttlCHBP12 + ttlGBTestP12 + ttlFIRAP12 + ttlOthersP12).ToString("#,##0");
            this.lbl_GrandTotal.Text = (ttlReworkTotal + ttlRejectTotal + ttlMFRNTotal + ttlCFSTotal + ttlSafetyTotal + ttlAuditTotal + ttlFabricTotal + ttlPenaltyTotal + ttlQCCTotal + ttlCHBTotal + ttlGBTestTotal + ttlFIRATotal + ttlOthersTotal).ToString("#,##0");

            this.lbl_LY_ReworkP1.Text = ttlLYReworkP01.ToString("#,##0");
            this.lbl_LY_ReworkP2.Text = ttlLYReworkP02.ToString("#,##0");
            this.lbl_LY_ReworkP3.Text = ttlLYReworkP03.ToString("#,##0");
            this.lbl_LY_ReworkP4.Text = ttlLYReworkP04.ToString("#,##0");
            this.lbl_LY_ReworkP5.Text = ttlLYReworkP05.ToString("#,##0");
            this.lbl_LY_ReworkP6.Text = ttlLYReworkP06.ToString("#,##0");
            this.lbl_LY_ReworkP7.Text = ttlLYReworkP07.ToString("#,##0");
            this.lbl_LY_ReworkP8.Text = ttlLYReworkP08.ToString("#,##0");
            this.lbl_LY_ReworkP9.Text = ttlLYReworkP09.ToString("#,##0");
            this.lbl_LY_ReworkP10.Text = ttlLYReworkP10.ToString("#,##0");
            this.lbl_LY_ReworkP11.Text = ttlLYReworkP11.ToString("#,##0");
            this.lbl_LY_ReworkP12.Text = ttlLYReworkP12.ToString("#,##0");
            this.lbl_LY_ReworkTotal.Text = ttlLYReworkTotal.ToString("#,##0");

            this.lbl_LY_RejectP1.Text = ttlLYRejectP01.ToString("#,##0");
            this.lbl_LY_RejectP2.Text = ttlLYRejectP02.ToString("#,##0");
            this.lbl_LY_RejectP3.Text = ttlLYRejectP03.ToString("#,##0");
            this.lbl_LY_RejectP4.Text = ttlLYRejectP04.ToString("#,##0");
            this.lbl_LY_RejectP5.Text = ttlLYRejectP05.ToString("#,##0");
            this.lbl_LY_RejectP6.Text = ttlLYRejectP06.ToString("#,##0");
            this.lbl_LY_RejectP7.Text = ttlLYRejectP07.ToString("#,##0");
            this.lbl_LY_RejectP8.Text = ttlLYRejectP08.ToString("#,##0");
            this.lbl_LY_RejectP9.Text = ttlLYRejectP09.ToString("#,##0");
            this.lbl_LY_RejectP10.Text = ttlLYRejectP10.ToString("#,##0");
            this.lbl_LY_RejectP11.Text = ttlLYRejectP11.ToString("#,##0");
            this.lbl_LY_RejectP12.Text = ttlLYRejectP12.ToString("#,##0");
            this.lbl_LY_RejectTotal.Text = ttlLYRejectTotal.ToString("#,##0");

            this.lbl_LY_MFRNP1.Text = ttlLYMFRNP01.ToString("#,##0");
            this.lbl_LY_MFRNP2.Text = ttlLYMFRNP02.ToString("#,##0");
            this.lbl_LY_MFRNP3.Text = ttlLYMFRNP03.ToString("#,##0");
            this.lbl_LY_MFRNP4.Text = ttlLYMFRNP04.ToString("#,##0");
            this.lbl_LY_MFRNP5.Text = ttlLYMFRNP05.ToString("#,##0");
            this.lbl_LY_MFRNP6.Text = ttlLYMFRNP06.ToString("#,##0");
            this.lbl_LY_MFRNP7.Text = ttlLYMFRNP07.ToString("#,##0");
            this.lbl_LY_MFRNP8.Text = ttlLYMFRNP08.ToString("#,##0");
            this.lbl_LY_MFRNP9.Text = ttlLYMFRNP09.ToString("#,##0");
            this.lbl_LY_MFRNP10.Text = ttlLYMFRNP10.ToString("#,##0");
            this.lbl_LY_MFRNP11.Text = ttlLYMFRNP11.ToString("#,##0");
            this.lbl_LY_MFRNP12.Text = ttlLYMFRNP12.ToString("#,##0");
            this.lbl_LY_MFRNTotal.Text = ttlLYMFRNTotal.ToString("#,##0");

            this.lbl_LY_CFSP1.Text = ttlLYCFSP01.ToString("#,##0");
            this.lbl_LY_CFSP2.Text = ttlLYCFSP02.ToString("#,##0");
            this.lbl_LY_CFSP3.Text = ttlLYCFSP03.ToString("#,##0");
            this.lbl_LY_CFSP4.Text = ttlLYCFSP04.ToString("#,##0");
            this.lbl_LY_CFSP5.Text = ttlLYCFSP05.ToString("#,##0");
            this.lbl_LY_CFSP6.Text = ttlLYCFSP06.ToString("#,##0");
            this.lbl_LY_CFSP7.Text = ttlLYCFSP07.ToString("#,##0");
            this.lbl_LY_CFSP8.Text = ttlLYCFSP08.ToString("#,##0");
            this.lbl_LY_CFSP9.Text = ttlLYCFSP09.ToString("#,##0");
            this.lbl_LY_CFSP10.Text = ttlLYCFSP10.ToString("#,##0");
            this.lbl_LY_CFSP11.Text = ttlLYCFSP11.ToString("#,##0");
            this.lbl_LY_CFSP12.Text = ttlLYCFSP12.ToString("#,##0");
            this.lbl_LY_CFSTotal.Text = ttlLYCFSTotal.ToString("#,##0");

            this.lbl_LY_SafetyP1.Text = ttlLYSafetyP01.ToString("#,##0");
            this.lbl_LY_SafetyP2.Text = ttlLYSafetyP02.ToString("#,##0");
            this.lbl_LY_SafetyP3.Text = ttlLYSafetyP03.ToString("#,##0");
            this.lbl_LY_SafetyP4.Text = ttlLYSafetyP04.ToString("#,##0");
            this.lbl_LY_SafetyP5.Text = ttlLYSafetyP05.ToString("#,##0");
            this.lbl_LY_SafetyP6.Text = ttlLYSafetyP06.ToString("#,##0");
            this.lbl_LY_SafetyP7.Text = ttlLYSafetyP07.ToString("#,##0");
            this.lbl_LY_SafetyP8.Text = ttlLYSafetyP08.ToString("#,##0");
            this.lbl_LY_SafetyP9.Text = ttlLYSafetyP09.ToString("#,##0");
            this.lbl_LY_SafetyP10.Text = ttlLYSafetyP10.ToString("#,##0");
            this.lbl_LY_SafetyP11.Text = ttlLYSafetyP11.ToString("#,##0");
            this.lbl_LY_SafetyP12.Text = ttlLYSafetyP12.ToString("#,##0");
            this.lbl_LY_SafetyTotal.Text = ttlLYSafetyTotal.ToString("#,##0");

            this.lbl_LY_AuditP1.Text = ttlLYAuditP01.ToString("#,##0");
            this.lbl_LY_AuditP2.Text = ttlLYAuditP02.ToString("#,##0");
            this.lbl_LY_AuditP3.Text = ttlLYAuditP03.ToString("#,##0");
            this.lbl_LY_AuditP4.Text = ttlLYAuditP04.ToString("#,##0");
            this.lbl_LY_AuditP5.Text = ttlLYAuditP05.ToString("#,##0");
            this.lbl_LY_AuditP6.Text = ttlLYAuditP06.ToString("#,##0");
            this.lbl_LY_AuditP7.Text = ttlLYAuditP07.ToString("#,##0");
            this.lbl_LY_AuditP8.Text = ttlLYAuditP08.ToString("#,##0");
            this.lbl_LY_AuditP9.Text = ttlLYAuditP09.ToString("#,##0");
            this.lbl_LY_AuditP10.Text = ttlLYAuditP10.ToString("#,##0");
            this.lbl_LY_AuditP11.Text = ttlLYAuditP11.ToString("#,##0");
            this.lbl_LY_AuditP12.Text = ttlLYAuditP12.ToString("#,##0");
            this.lbl_LY_AuditTotal.Text = ttlLYAuditTotal.ToString("#,##0");

            this.lbl_LY_FabricP1.Text = ttlLYFabricP01.ToString("#,##0");
            this.lbl_LY_FabricP2.Text = ttlLYFabricP02.ToString("#,##0");
            this.lbl_LY_FabricP3.Text = ttlLYFabricP03.ToString("#,##0");
            this.lbl_LY_FabricP4.Text = ttlLYFabricP04.ToString("#,##0");
            this.lbl_LY_FabricP5.Text = ttlLYFabricP05.ToString("#,##0");
            this.lbl_LY_FabricP6.Text = ttlLYFabricP06.ToString("#,##0");
            this.lbl_LY_FabricP7.Text = ttlLYFabricP07.ToString("#,##0");
            this.lbl_LY_FabricP8.Text = ttlLYFabricP08.ToString("#,##0");
            this.lbl_LY_FabricP9.Text = ttlLYFabricP09.ToString("#,##0");
            this.lbl_LY_FabricP10.Text = ttlLYFabricP10.ToString("#,##0");
            this.lbl_LY_FabricP11.Text = ttlLYFabricP11.ToString("#,##0");
            this.lbl_LY_FabricP12.Text = ttlLYFabricP12.ToString("#,##0");
            this.lbl_LY_FabricTotal.Text = ttlLYFabricTotal.ToString("#,##0");

            this.lbl_LY_PenaltyP1.Text = ttlLYPenaltyP01.ToString("#,##0");
            this.lbl_LY_PenaltyP2.Text = ttlLYPenaltyP02.ToString("#,##0");
            this.lbl_LY_PenaltyP3.Text = ttlLYPenaltyP03.ToString("#,##0");
            this.lbl_LY_PenaltyP4.Text = ttlLYPenaltyP04.ToString("#,##0");
            this.lbl_LY_PenaltyP5.Text = ttlLYPenaltyP05.ToString("#,##0");
            this.lbl_LY_PenaltyP6.Text = ttlLYPenaltyP06.ToString("#,##0");
            this.lbl_LY_PenaltyP7.Text = ttlLYPenaltyP07.ToString("#,##0");
            this.lbl_LY_PenaltyP8.Text = ttlLYPenaltyP08.ToString("#,##0");
            this.lbl_LY_PenaltyP9.Text = ttlLYPenaltyP09.ToString("#,##0");
            this.lbl_LY_PenaltyP10.Text = ttlLYPenaltyP10.ToString("#,##0");
            this.lbl_LY_PenaltyP11.Text = ttlLYPenaltyP11.ToString("#,##0");
            this.lbl_LY_PenaltyP12.Text = ttlLYPenaltyP12.ToString("#,##0");
            this.lbl_LY_PenaltyTotal.Text = ttlLYPenaltyTotal.ToString("#,##0");

            this.lbl_LY_QCCP1.Text = ttlLYQCCP01.ToString("#,##0");
            this.lbl_LY_QCCP2.Text = ttlLYQCCP02.ToString("#,##0");
            this.lbl_LY_QCCP3.Text = ttlLYQCCP03.ToString("#,##0");
            this.lbl_LY_QCCP4.Text = ttlLYQCCP04.ToString("#,##0");
            this.lbl_LY_QCCP5.Text = ttlLYQCCP05.ToString("#,##0");
            this.lbl_LY_QCCP6.Text = ttlLYQCCP06.ToString("#,##0");
            this.lbl_LY_QCCP7.Text = ttlLYQCCP07.ToString("#,##0");
            this.lbl_LY_QCCP8.Text = ttlLYQCCP08.ToString("#,##0");
            this.lbl_LY_QCCP9.Text = ttlLYQCCP09.ToString("#,##0");
            this.lbl_LY_QCCP10.Text = ttlLYQCCP10.ToString("#,##0");
            this.lbl_LY_QCCP11.Text = ttlLYQCCP11.ToString("#,##0");
            this.lbl_LY_QCCP12.Text = ttlLYQCCP12.ToString("#,##0");
            this.lbl_LY_QCCTotal.Text = ttlLYQCCTotal.ToString("#,##0");

            this.lbl_LY_CHBP1.Text = ttlLYCHBP01.ToString("#,##0");
            this.lbl_LY_CHBP2.Text = ttlLYCHBP02.ToString("#,##0");
            this.lbl_LY_CHBP3.Text = ttlLYCHBP03.ToString("#,##0");
            this.lbl_LY_CHBP4.Text = ttlLYCHBP04.ToString("#,##0");
            this.lbl_LY_CHBP5.Text = ttlLYCHBP05.ToString("#,##0");
            this.lbl_LY_CHBP6.Text = ttlLYCHBP06.ToString("#,##0");
            this.lbl_LY_CHBP7.Text = ttlLYCHBP07.ToString("#,##0");
            this.lbl_LY_CHBP8.Text = ttlLYCHBP08.ToString("#,##0");
            this.lbl_LY_CHBP9.Text = ttlLYCHBP09.ToString("#,##0");
            this.lbl_LY_CHBP10.Text = ttlLYCHBP10.ToString("#,##0");
            this.lbl_LY_CHBP11.Text = ttlLYCHBP11.ToString("#,##0");
            this.lbl_LY_CHBP12.Text = ttlLYCHBP12.ToString("#,##0");
            this.lbl_LY_CHBTotal.Text = ttlLYCHBTotal.ToString("#,##0");

            this.lbl_LY_GBTestP1.Text = ttlLYGBTestP01.ToString("#,##0");
            this.lbl_LY_GBTestP2.Text = ttlLYGBTestP02.ToString("#,##0");
            this.lbl_LY_GBTestP3.Text = ttlLYGBTestP03.ToString("#,##0");
            this.lbl_LY_GBTestP4.Text = ttlLYGBTestP04.ToString("#,##0");
            this.lbl_LY_GBTestP5.Text = ttlLYGBTestP05.ToString("#,##0");
            this.lbl_LY_GBTestP6.Text = ttlLYGBTestP06.ToString("#,##0");
            this.lbl_LY_GBTestP7.Text = ttlLYGBTestP07.ToString("#,##0");
            this.lbl_LY_GBTestP8.Text = ttlLYGBTestP08.ToString("#,##0");
            this.lbl_LY_GBTestP9.Text = ttlLYGBTestP09.ToString("#,##0");
            this.lbl_LY_GBTestP10.Text = ttlLYGBTestP10.ToString("#,##0");
            this.lbl_LY_GBTestP11.Text = ttlLYGBTestP11.ToString("#,##0");
            this.lbl_LY_GBTestP12.Text = ttlLYGBTestP12.ToString("#,##0");
            this.lbl_LY_GBTestTotal.Text = ttlLYGBTestTotal.ToString("#,##0");

            this.lbl_LY_FIRAP1.Text = ttlLYFIRAP01.ToString("#,##0");
            this.lbl_LY_FIRAP2.Text = ttlLYFIRAP02.ToString("#,##0");
            this.lbl_LY_FIRAP3.Text = ttlLYFIRAP03.ToString("#,##0");
            this.lbl_LY_FIRAP4.Text = ttlLYFIRAP04.ToString("#,##0");
            this.lbl_LY_FIRAP5.Text = ttlLYFIRAP05.ToString("#,##0");
            this.lbl_LY_FIRAP6.Text = ttlLYFIRAP06.ToString("#,##0");
            this.lbl_LY_FIRAP7.Text = ttlLYFIRAP07.ToString("#,##0");
            this.lbl_LY_FIRAP8.Text = ttlLYFIRAP08.ToString("#,##0");
            this.lbl_LY_FIRAP9.Text = ttlLYFIRAP09.ToString("#,##0");
            this.lbl_LY_FIRAP10.Text = ttlLYFIRAP10.ToString("#,##0");
            this.lbl_LY_FIRAP11.Text = ttlLYFIRAP11.ToString("#,##0");
            this.lbl_LY_FIRAP12.Text = ttlLYFIRAP12.ToString("#,##0");
            this.lbl_LY_FIRATotal.Text = ttlLYFIRATotal.ToString("#,##0");

            this.lbl_LY_OthersP1.Text = ttlLYOthersP01.ToString("#,##0");
            this.lbl_LY_OthersP2.Text = ttlLYOthersP02.ToString("#,##0");
            this.lbl_LY_OthersP3.Text = ttlLYOthersP03.ToString("#,##0");
            this.lbl_LY_OthersP4.Text = ttlLYOthersP04.ToString("#,##0");
            this.lbl_LY_OthersP5.Text = ttlLYOthersP05.ToString("#,##0");
            this.lbl_LY_OthersP6.Text = ttlLYOthersP06.ToString("#,##0");
            this.lbl_LY_OthersP7.Text = ttlLYOthersP07.ToString("#,##0");
            this.lbl_LY_OthersP8.Text = ttlLYOthersP08.ToString("#,##0");
            this.lbl_LY_OthersP9.Text = ttlLYOthersP09.ToString("#,##0");
            this.lbl_LY_OthersP10.Text = ttlLYOthersP10.ToString("#,##0");
            this.lbl_LY_OthersP11.Text = ttlLYOthersP11.ToString("#,##0");
            this.lbl_LY_OthersP12.Text = ttlLYOthersP12.ToString("#,##0");
            this.lbl_LY_OthersTotal.Text = ttlLYOthersTotal.ToString("#,##0");


            this.lbl_LY_GrandP1.Text = (ttlLYReworkP01 + ttlLYRejectP01 + ttlLYMFRNP01 + ttlLYCFSP01 + ttlLYSafetyP01 + ttlLYAuditP01 + ttlLYFabricP01 + ttlLYPenaltyP01 + ttlLYQCCP01 + ttlLYCHBP01 + ttlLYGBTestP01 + ttlLYFIRAP01 + ttlLYOthersP01).ToString("#,##0");
            this.lbl_LY_GrandP2.Text = (ttlLYReworkP02 + ttlLYRejectP02 + ttlLYMFRNP02 + ttlLYCFSP02 + ttlLYSafetyP02 + ttlLYAuditP02 + ttlLYFabricP02 + ttlLYPenaltyP02 + ttlLYQCCP02 + ttlLYCHBP02 + ttlLYGBTestP02 + ttlLYFIRAP02 + ttlLYOthersP02).ToString("#,##0");
            this.lbl_LY_GrandP3.Text = (ttlLYReworkP03 + ttlLYRejectP03 + ttlLYMFRNP03 + ttlLYCFSP03 + ttlLYSafetyP03 + ttlLYAuditP03 + ttlLYFabricP03 + ttlLYPenaltyP03 + ttlLYQCCP03 + ttlLYCHBP03 + ttlLYGBTestP03 + ttlLYFIRAP03 + ttlLYOthersP03).ToString("#,##0");
            this.lbl_LY_GrandP4.Text = (ttlLYReworkP04 + ttlLYRejectP04 + ttlLYMFRNP04 + ttlLYCFSP04 + ttlLYSafetyP04 + ttlLYAuditP04 + ttlLYFabricP04 + ttlLYPenaltyP04 + ttlLYQCCP04 + ttlLYCHBP04 + ttlLYGBTestP04 + ttlLYFIRAP04 + ttlLYOthersP04).ToString("#,##0");
            this.lbl_LY_GrandP5.Text = (ttlLYReworkP05 + ttlLYRejectP05 + ttlLYMFRNP05 + ttlLYCFSP05 + ttlLYSafetyP05 + ttlLYAuditP05 + ttlLYFabricP05 + ttlLYPenaltyP05 + ttlLYQCCP05 + ttlLYCHBP05 + ttlLYGBTestP05 + ttlLYFIRAP05 + ttlLYOthersP05).ToString("#,##0");
            this.lbl_LY_GrandP6.Text = (ttlLYReworkP06 + ttlLYRejectP06 + ttlLYMFRNP06 + ttlLYCFSP06 + ttlLYSafetyP06 + ttlLYAuditP06 + ttlLYFabricP06 + ttlLYPenaltyP06 + ttlLYQCCP06 + ttlLYCHBP06 + ttlLYGBTestP06 + ttlLYFIRAP06 + ttlLYOthersP06).ToString("#,##0");
            this.lbl_LY_GrandP7.Text = (ttlLYReworkP07 + ttlLYRejectP07 + ttlLYMFRNP07 + ttlLYCFSP07 + ttlLYSafetyP07 + ttlLYAuditP07 + ttlLYFabricP07 + ttlLYPenaltyP07 + ttlLYQCCP07 + ttlLYCHBP07 + ttlLYGBTestP07 + ttlLYFIRAP07 + ttlLYOthersP07).ToString("#,##0");
            this.lbl_LY_GrandP8.Text = (ttlLYReworkP08 + ttlLYRejectP08 + ttlLYMFRNP08 + ttlLYCFSP08 + ttlLYSafetyP08 + ttlLYAuditP08 + ttlLYFabricP08 + ttlLYPenaltyP08 + ttlLYQCCP08 + ttlLYCHBP08 + ttlLYGBTestP08 + ttlLYFIRAP08 + ttlLYOthersP08).ToString("#,##0");
            this.lbl_LY_GrandP9.Text = (ttlLYReworkP09 + ttlLYRejectP09 + ttlLYMFRNP09 + ttlLYCFSP09 + ttlLYSafetyP09 + ttlLYAuditP09 + ttlLYFabricP09 + ttlLYPenaltyP09 + ttlLYQCCP09 + ttlLYCHBP09 + ttlLYGBTestP09 + ttlLYFIRAP09 + ttlLYOthersP09).ToString("#,##0");
            this.lbl_LY_GrandP10.Text = (ttlLYReworkP10 + ttlLYRejectP10 + ttlLYMFRNP10 + ttlLYCFSP10 + ttlLYSafetyP10 + ttlLYAuditP10 + ttlLYFabricP10 + ttlLYPenaltyP10 + ttlLYQCCP10 + ttlLYCHBP10 + ttlLYGBTestP10 + ttlLYFIRAP10 + ttlLYOthersP10).ToString("#,##0");
            this.lbl_LY_GrandP11.Text = (ttlLYReworkP11 + ttlLYRejectP11 + ttlLYMFRNP11 + ttlLYCFSP11 + ttlLYSafetyP11 + ttlLYAuditP11 + ttlLYFabricP11 + ttlLYPenaltyP11 + ttlLYQCCP11 + ttlLYCHBP11 + ttlLYGBTestP11 + ttlLYFIRAP11 + ttlLYOthersP11).ToString("#,##0");
            this.lbl_LY_GrandP12.Text = (ttlLYReworkP12 + ttlLYRejectP12 + ttlLYMFRNP12 + ttlLYCFSP12 + ttlLYSafetyP12 + ttlLYAuditP12 + ttlLYFabricP12 + ttlLYPenaltyP12 + ttlLYQCCP12 + ttlLYCHBP12 + ttlLYGBTestP12 + ttlLYFIRAP12 + ttlLYOthersP12).ToString("#,##0");
            this.lbl_LY_GrandTotal.Text = (ttlLYReworkTotal + ttlLYRejectTotal + ttlLYMFRNTotal + ttlLYCFSTotal + ttlLYSafetyTotal + ttlLYAuditTotal + ttlLYFabricTotal + ttlLYPenaltyTotal + ttlLYQCCTotal + ttlLYCHBTotal + ttlLYGBTestTotal + ttlLYFIRATotal + ttlLYOthersTotal).ToString("#,##0");

            this.lbl_P1PaidBySupplierAmt.Text = ttlVendorP01.ToString("#,##0");
            this.lbl_P2PaidBySupplierAmt.Text = ttlVendorP02.ToString("#,##0");
            this.lbl_P3PaidBySupplierAmt.Text = ttlVendorP03.ToString("#,##0");
            this.lbl_P4PaidBySupplierAmt.Text = ttlVendorP04.ToString("#,##0");
            this.lbl_P5PaidBySupplierAmt.Text = ttlVendorP05.ToString("#,##0");
            this.lbl_P6PaidBySupplierAmt.Text = ttlVendorP06.ToString("#,##0");
            this.lbl_P7PaidBySupplierAmt.Text = ttlVendorP07.ToString("#,##0");
            this.lbl_P8PaidBySupplierAmt.Text = ttlVendorP08.ToString("#,##0");
            this.lbl_P9PaidBySupplierAmt.Text = ttlVendorP09.ToString("#,##0");
            this.lbl_P10PaidBySupplierAmt.Text = ttlVendorP10.ToString("#,##0");
            this.lbl_P11PaidBySupplierAmt.Text = ttlVendorP11.ToString("#,##0");
            this.lbl_P12PaidBySupplierAmt.Text = ttlVendorP12.ToString("#,##0");
            this.lbl_TotalPaidBySupplierAmt.Text = ttlVendorTotal.ToString("#,##0");

            this.lbl_P1PaidBySupplierPct.Text = (ttlReworkP01 + ttlRejectP01 + ttlMFRNP01 + ttlCFSP01 + ttlSafetyP01 + ttlAuditP01 + ttlFabricP01 + ttlPenaltyP01 + ttlQCCP01 + ttlCHBP01 + ttlGBTestP01 + ttlFIRAP01 + ttlOthersP01) == 0 ? "N/A" : (ttlVendorP01 / (ttlReworkP01 + ttlRejectP01 + ttlMFRNP01 + ttlCFSP01 + ttlSafetyP01 + ttlAuditP01 + ttlFabricP01 + ttlPenaltyP01 + ttlQCCP01 + ttlCHBP01 + ttlGBTestP01 + ttlFIRAP01 + ttlOthersP01) * 100).ToString("#,##0.00");
            this.lbl_P2PaidBySupplierPct.Text = (ttlReworkP02 + ttlRejectP02 + ttlMFRNP02 + ttlCFSP02 + ttlSafetyP02 + ttlAuditP02 + ttlFabricP02 + ttlPenaltyP02 + ttlQCCP02 + ttlCHBP02 + ttlGBTestP02 + ttlFIRAP02 + ttlOthersP02) == 0 ? "N/A" : (ttlVendorP02 / (ttlReworkP02 + ttlRejectP02 + ttlMFRNP02 + ttlCFSP02 + ttlSafetyP02 + ttlAuditP02 + ttlFabricP02 + ttlPenaltyP02 + ttlQCCP02 + ttlCHBP02 + ttlGBTestP02 + ttlFIRAP02 + ttlOthersP02) * 100).ToString("#,##0.00");
            this.lbl_P3PaidBySupplierPct.Text = (ttlReworkP03 + ttlRejectP03 + ttlMFRNP03 + ttlCFSP03 + ttlSafetyP03 + ttlAuditP03 + ttlFabricP03 + ttlPenaltyP03 + ttlQCCP03 + ttlCHBP03 + ttlGBTestP03 + ttlFIRAP03 + ttlOthersP03) == 0 ? "N/A" : (ttlVendorP03 / (ttlReworkP03 + ttlRejectP03 + ttlMFRNP03 + ttlCFSP03 + ttlSafetyP03 + ttlAuditP03 + ttlFabricP03 + ttlPenaltyP03 + ttlQCCP03 + ttlCHBP03 + ttlGBTestP03 + ttlFIRAP03 + ttlOthersP03) * 100).ToString("#,##0.00");
            this.lbl_P4PaidBySupplierPct.Text = (ttlReworkP04 + ttlRejectP04 + ttlMFRNP04 + ttlCFSP04 + ttlSafetyP04 + ttlAuditP04 + ttlFabricP04 + ttlPenaltyP04 + ttlQCCP04 + ttlCHBP04 + ttlGBTestP04 + ttlFIRAP04 + ttlOthersP04) == 0 ? "N/A" : (ttlVendorP04 / (ttlReworkP04 + ttlRejectP04 + ttlMFRNP04 + ttlCFSP04 + ttlSafetyP04 + ttlAuditP04 + ttlFabricP04 + ttlPenaltyP04 + ttlQCCP04 + ttlCHBP04 + ttlGBTestP04 + ttlFIRAP04 + ttlOthersP04) * 100).ToString("#,##0.00");
            this.lbl_P5PaidBySupplierPct.Text = (ttlReworkP05 + ttlRejectP05 + ttlMFRNP05 + ttlCFSP05 + ttlSafetyP05 + ttlAuditP05 + ttlFabricP05 + ttlPenaltyP05 + ttlQCCP05 + ttlCHBP05 + ttlGBTestP05 + ttlFIRAP05 + ttlOthersP05) == 0 ? "N/A" : (ttlVendorP05 / (ttlReworkP05 + ttlRejectP05 + ttlMFRNP05 + ttlCFSP05 + ttlSafetyP05 + ttlAuditP05 + ttlFabricP05 + ttlPenaltyP05 + ttlQCCP05 + ttlCHBP05 + ttlGBTestP05 + ttlFIRAP05 + ttlOthersP05) * 100).ToString("#,##0.00");
            this.lbl_P6PaidBySupplierPct.Text = (ttlReworkP06 + ttlRejectP06 + ttlMFRNP06 + ttlCFSP06 + ttlSafetyP06 + ttlAuditP06 + ttlFabricP06 + ttlPenaltyP06 + ttlQCCP06 + ttlCHBP06 + ttlGBTestP06 + ttlFIRAP06 + ttlOthersP06) == 0 ? "N/A" : (ttlVendorP06 / (ttlReworkP06 + ttlRejectP06 + ttlMFRNP06 + ttlCFSP06 + ttlSafetyP06 + ttlAuditP06 + ttlFabricP06 + ttlPenaltyP06 + ttlQCCP06 + ttlCHBP06 + ttlGBTestP06 + ttlFIRAP06 + ttlOthersP06) * 100).ToString("#,##0.00");
            this.lbl_P7PaidBySupplierPct.Text = (ttlReworkP07 + ttlRejectP07 + ttlMFRNP07 + ttlCFSP07 + ttlSafetyP07 + ttlAuditP07 + ttlFabricP07 + ttlPenaltyP07 + ttlQCCP07 + ttlCHBP07 + ttlGBTestP07 + ttlFIRAP07 + ttlOthersP07) == 0 ? "N/A" : (ttlVendorP07 / (ttlReworkP07 + ttlRejectP07 + ttlMFRNP07 + ttlCFSP07 + ttlSafetyP07 + ttlAuditP07 + ttlFabricP07 + ttlPenaltyP07 + ttlQCCP07 + ttlCHBP07 + ttlGBTestP07 + ttlFIRAP07 + ttlOthersP07) * 100).ToString("#,##0.00");
            this.lbl_P8PaidBySupplierPct.Text = (ttlReworkP08 + ttlRejectP08 + ttlMFRNP08 + ttlCFSP08 + ttlSafetyP08 + ttlAuditP08 + ttlFabricP08 + ttlPenaltyP08 + ttlQCCP08 + ttlCHBP08 + ttlGBTestP08 + ttlFIRAP08 + ttlOthersP08) == 0 ? "N/A" : (ttlVendorP08 / (ttlReworkP08 + ttlRejectP08 + ttlMFRNP08 + ttlCFSP08 + ttlSafetyP08 + ttlAuditP08 + ttlFabricP08 + ttlPenaltyP08 + ttlQCCP08 + ttlCHBP08 + ttlGBTestP08 + ttlFIRAP08 + ttlOthersP08) * 100).ToString("#,##0.00");
            this.lbl_P9PaidBySupplierPct.Text = (ttlReworkP09 + ttlRejectP09 + ttlMFRNP09 + ttlCFSP09 + ttlSafetyP09 + ttlAuditP09 + ttlFabricP09 + ttlPenaltyP09 + ttlQCCP09 + ttlCHBP09 + ttlGBTestP09 + ttlFIRAP09 + ttlOthersP09) == 0 ? "N/A" : (ttlVendorP09 / (ttlReworkP09 + ttlRejectP09 + ttlMFRNP09 + ttlCFSP09 + ttlSafetyP09 + ttlAuditP09 + ttlFabricP09 + ttlPenaltyP09 + ttlQCCP09 + ttlCHBP09 + ttlGBTestP09 + ttlFIRAP09 + ttlOthersP09) * 100).ToString("#,##0.00");
            this.lbl_P10PaidBySupplierPct.Text = (ttlReworkP10 + ttlRejectP10 + ttlMFRNP10 + ttlCFSP10 + ttlSafetyP10 + ttlAuditP10 + ttlFabricP10 + ttlPenaltyP10 + ttlQCCP10 + ttlCHBP10 + ttlGBTestP10 + ttlFIRAP10 + ttlOthersP10) == 0 ? "N/A" : (ttlVendorP10 / (ttlReworkP10 + ttlRejectP10 + ttlMFRNP10 + ttlCFSP10 + ttlSafetyP10 + ttlAuditP10 + ttlFabricP10 + ttlPenaltyP10 + ttlQCCP10 + ttlCHBP10 + ttlGBTestP10 + ttlFIRAP10 + ttlOthersP10) * 100).ToString("#,##0.00");
            this.lbl_P11PaidBySupplierPct.Text = (ttlReworkP11 + ttlRejectP11 + ttlMFRNP11 + ttlCFSP11 + ttlSafetyP11 + ttlAuditP11 + ttlFabricP11 + ttlPenaltyP11 + ttlQCCP11 + ttlCHBP11 + ttlGBTestP11 + ttlFIRAP11 + ttlOthersP11) == 0 ? "N/A" : (ttlVendorP11 / (ttlReworkP11 + ttlRejectP11 + ttlMFRNP11 + ttlCFSP11 + ttlSafetyP11 + ttlAuditP11 + ttlFabricP11 + ttlPenaltyP11 + ttlQCCP11 + ttlCHBP11 + ttlGBTestP11 + ttlFIRAP11 + ttlOthersP11) * 100).ToString("#,##0.00");
            this.lbl_P12PaidBySupplierPct.Text = (ttlReworkP12 + ttlRejectP12 + ttlMFRNP12 + ttlCFSP12 + ttlSafetyP12 + ttlAuditP12 + ttlFabricP12 + ttlPenaltyP12 + ttlQCCP12 + ttlCHBP12 + ttlGBTestP12 + ttlFIRAP12 + ttlOthersP12) == 0 ? "N/A" : (ttlVendorP12 / (ttlReworkP12 + ttlRejectP12 + ttlMFRNP12 + ttlCFSP12 + ttlSafetyP12 + ttlAuditP12 + ttlFabricP12 + ttlPenaltyP12 + ttlQCCP12 + ttlCHBP12 + ttlGBTestP12 + ttlFIRAP12 + ttlOthersP12) * 100).ToString("#,##0.00");
            this.lbl_TotalPaidBySupplierPct.Text = (ttlReworkTotal + ttlRejectTotal + ttlMFRNTotal + ttlCFSTotal + ttlSafetyTotal + ttlAuditTotal + ttlFabricTotal + ttlPenaltyTotal + ttlQCCTotal + ttlCHBTotal + ttlGBTestTotal + ttlFIRATotal + ttlOthersTotal) == 0 ? "N/A" : (ttlVendorTotal / (ttlReworkTotal + ttlRejectTotal + ttlMFRNTotal + ttlCFSTotal + ttlSafetyTotal + ttlAuditTotal + ttlFabricTotal + ttlPenaltyTotal + ttlQCCTotal + ttlCHBTotal + ttlGBTestTotal + ttlFIRATotal + ttlOthersTotal) * 100).ToString("#,##0.00");

            this.lbl_P1PaidByNSAmt.Text = ttlNSP01.ToString("#,##0");
            this.lbl_P2PaidByNSAmt.Text = ttlNSP02.ToString("#,##0");
            this.lbl_P3PaidByNSAmt.Text = ttlNSP03.ToString("#,##0");
            this.lbl_P4PaidByNSAmt.Text = ttlNSP04.ToString("#,##0");
            this.lbl_P5PaidByNSAmt.Text = ttlNSP05.ToString("#,##0");
            this.lbl_P6PaidByNSAmt.Text = ttlNSP06.ToString("#,##0");
            this.lbl_P7PaidByNSAmt.Text = ttlNSP07.ToString("#,##0");
            this.lbl_P8PaidByNSAmt.Text = ttlNSP08.ToString("#,##0");
            this.lbl_P9PaidByNSAmt.Text = ttlNSP09.ToString("#,##0");
            this.lbl_P10PaidByNSAmt.Text = ttlNSP10.ToString("#,##0");
            this.lbl_P11PaidByNSAmt.Text = ttlNSP11.ToString("#,##0");
            this.lbl_P12PaidByNSAmt.Text = ttlNSP12.ToString("#,##0");
            this.lbl_TotalPaidByNSAmt.Text = ttlNSTotal.ToString("#,##0");

            this.lbl_P1PaidByNSPct.Text = (ttlReworkP01 + ttlRejectP01 + ttlMFRNP01 + ttlCFSP01 + ttlSafetyP01 + ttlAuditP01 + ttlFabricP01 + ttlPenaltyP01 + ttlQCCP01 + ttlCHBP01 + ttlGBTestP01 + ttlFIRAP01 + ttlOthersP01) == 0 ? "N/A" : (ttlNSP01 / (ttlReworkP01 + ttlRejectP01 + ttlMFRNP01 + ttlCFSP01 + ttlSafetyP01 + ttlAuditP01 + ttlFabricP01 + ttlPenaltyP01 + ttlQCCP01 + ttlCHBP01 + ttlGBTestP01 + ttlFIRAP01 + ttlOthersP01) * 100).ToString("#,##0.00");
            this.lbl_P2PaidByNSPct.Text = (ttlReworkP02 + ttlRejectP02 + ttlMFRNP02 + ttlCFSP02 + ttlSafetyP02 + ttlAuditP02 + ttlFabricP02 + ttlPenaltyP02 + ttlQCCP02 + ttlCHBP02 + ttlGBTestP02 + ttlFIRAP02 + ttlOthersP02) == 0 ? "N/A" : (ttlNSP02 / (ttlReworkP02 + ttlRejectP02 + ttlMFRNP02 + ttlCFSP02 + ttlSafetyP02 + ttlAuditP02 + ttlFabricP02 + ttlPenaltyP02 + ttlQCCP02 + ttlCHBP02 + ttlGBTestP02 + ttlFIRAP02 + ttlOthersP02) * 100).ToString("#,##0.00");
            this.lbl_P3PaidByNSPct.Text = (ttlReworkP03 + ttlRejectP03 + ttlMFRNP03 + ttlCFSP03 + ttlSafetyP03 + ttlAuditP03 + ttlFabricP03 + ttlPenaltyP03 + ttlQCCP03 + ttlCHBP03 + ttlGBTestP03 + ttlFIRAP03 + ttlOthersP03) == 0 ? "N/A" : (ttlNSP03 / (ttlReworkP03 + ttlRejectP03 + ttlMFRNP03 + ttlCFSP03 + ttlSafetyP03 + ttlAuditP03 + ttlFabricP03 + ttlPenaltyP03 + ttlQCCP03 + ttlCHBP03 + ttlGBTestP03 + ttlFIRAP03 + ttlOthersP03) * 100).ToString("#,##0.00");
            this.lbl_P4PaidByNSPct.Text = (ttlReworkP04 + ttlRejectP04 + ttlMFRNP04 + ttlCFSP04 + ttlSafetyP04 + ttlAuditP04 + ttlFabricP04 + ttlPenaltyP04 + ttlQCCP04 + ttlCHBP04 + ttlGBTestP04 + ttlFIRAP04 + ttlOthersP04) == 0 ? "N/A" : (ttlNSP04 / (ttlReworkP04 + ttlRejectP04 + ttlMFRNP04 + ttlCFSP04 + ttlSafetyP04 + ttlAuditP04 + ttlFabricP04 + ttlPenaltyP04 + ttlQCCP04 + ttlCHBP04 + ttlGBTestP04 + ttlFIRAP04 + ttlOthersP04) * 100).ToString("#,##0.00");
            this.lbl_P5PaidByNSPct.Text = (ttlReworkP05 + ttlRejectP05 + ttlMFRNP05 + ttlCFSP05 + ttlSafetyP05 + ttlAuditP05 + ttlFabricP05 + ttlPenaltyP05 + ttlQCCP05 + ttlCHBP05 + ttlGBTestP05 + ttlFIRAP05 + ttlOthersP05) == 0 ? "N/A" : (ttlNSP05 / (ttlReworkP05 + ttlRejectP05 + ttlMFRNP05 + ttlCFSP05 + ttlSafetyP05 + ttlAuditP05 + ttlFabricP05 + ttlPenaltyP05 + ttlQCCP05 + ttlCHBP05 + ttlGBTestP05 + ttlFIRAP05 + ttlOthersP05) * 100).ToString("#,##0.00");
            this.lbl_P6PaidByNSPct.Text = (ttlReworkP06 + ttlRejectP06 + ttlMFRNP06 + ttlCFSP06 + ttlSafetyP06 + ttlAuditP06 + ttlFabricP06 + ttlPenaltyP06 + ttlQCCP06 + ttlCHBP06 + ttlGBTestP06 + ttlFIRAP06 + ttlOthersP06) == 0 ? "N/A" : (ttlNSP06 / (ttlReworkP06 + ttlRejectP06 + ttlMFRNP06 + ttlCFSP06 + ttlSafetyP06 + ttlAuditP06 + ttlFabricP06 + ttlPenaltyP06 + ttlQCCP06 + ttlCHBP06 + ttlGBTestP06 + ttlFIRAP06 + ttlOthersP06) * 100).ToString("#,##0.00");
            this.lbl_P7PaidByNSPct.Text = (ttlReworkP07 + ttlRejectP07 + ttlMFRNP07 + ttlCFSP07 + ttlSafetyP07 + ttlAuditP07 + ttlFabricP07 + ttlPenaltyP07 + ttlQCCP07 + ttlCHBP07 + ttlGBTestP07 + ttlFIRAP07 + ttlOthersP07) == 0 ? "N/A" : (ttlNSP07 / (ttlReworkP07 + ttlRejectP07 + ttlMFRNP07 + ttlCFSP07 + ttlSafetyP07 + ttlAuditP07 + ttlFabricP07 + ttlPenaltyP07 + ttlQCCP07 + ttlCHBP07 + ttlGBTestP07 + ttlFIRAP07 + ttlOthersP07) * 100).ToString("#,##0.00");
            this.lbl_P8PaidByNSPct.Text = (ttlReworkP08 + ttlRejectP08 + ttlMFRNP08 + ttlCFSP08 + ttlSafetyP08 + ttlAuditP08 + ttlFabricP08 + ttlPenaltyP08 + ttlQCCP08 + ttlCHBP08 + ttlGBTestP08 + ttlFIRAP08 + ttlOthersP08) == 0 ? "N/A" : (ttlNSP08 / (ttlReworkP08 + ttlRejectP08 + ttlMFRNP08 + ttlCFSP08 + ttlSafetyP08 + ttlAuditP08 + ttlFabricP08 + ttlPenaltyP08 + ttlQCCP08 + ttlCHBP08 + ttlGBTestP08 + ttlFIRAP08 + ttlOthersP08) * 100).ToString("#,##0.00");
            this.lbl_P9PaidByNSPct.Text = (ttlReworkP09 + ttlRejectP09 + ttlMFRNP09 + ttlCFSP09 + ttlSafetyP09 + ttlAuditP09 + ttlFabricP09 + ttlPenaltyP09 + ttlQCCP09 + ttlCHBP09 + ttlGBTestP09 + ttlFIRAP09 + ttlOthersP09) == 0 ? "N/A" : (ttlNSP09 / (ttlReworkP09 + ttlRejectP09 + ttlMFRNP09 + ttlCFSP09 + ttlSafetyP09 + ttlAuditP09 + ttlFabricP09 + ttlPenaltyP09 + ttlQCCP09 + ttlCHBP09 + ttlGBTestP09 + ttlFIRAP09 + ttlOthersP09) * 100).ToString("#,##0.00");
            this.lbl_P10PaidByNSPct.Text = (ttlReworkP10 + ttlRejectP10 + ttlMFRNP10 + ttlCFSP10 + ttlSafetyP10 + ttlAuditP10 + ttlFabricP10 + ttlPenaltyP10 + ttlQCCP10 + ttlCHBP10 + ttlGBTestP10 + ttlFIRAP10 + ttlOthersP10) == 0 ? "N/A" : (ttlNSP10 / (ttlReworkP10 + ttlRejectP10 + ttlMFRNP10 + ttlCFSP10 + ttlSafetyP10 + ttlAuditP10 + ttlFabricP10 + ttlPenaltyP10 + ttlQCCP10 + ttlCHBP10 + ttlGBTestP10 + ttlFIRAP10 + ttlOthersP10) * 100).ToString("#,##0.00");
            this.lbl_P11PaidByNSPct.Text = (ttlReworkP11 + ttlRejectP11 + ttlMFRNP11 + ttlCFSP11 + ttlSafetyP11 + ttlAuditP11 + ttlFabricP11 + ttlPenaltyP11 + ttlQCCP11 + ttlCHBP11 + ttlGBTestP11 + ttlFIRAP11 + ttlOthersP11) == 0 ? "N/A" : (ttlNSP11 / (ttlReworkP11 + ttlRejectP11 + ttlMFRNP11 + ttlCFSP11 + ttlSafetyP11 + ttlAuditP11 + ttlFabricP11 + ttlPenaltyP11 + ttlQCCP11 + ttlCHBP11 + ttlGBTestP11 + ttlFIRAP11 + ttlOthersP11) * 100).ToString("#,##0.00");
            this.lbl_P12PaidByNSPct.Text = (ttlReworkP12 + ttlRejectP12 + ttlMFRNP12 + ttlCFSP12 + ttlSafetyP12 + ttlAuditP12 + ttlFabricP12 + ttlPenaltyP12 + ttlQCCP12 + ttlCHBP12 + ttlGBTestP12 + ttlFIRAP12 + ttlOthersP12) == 0 ? "N/A" : (ttlNSP12 / (ttlReworkP12 + ttlRejectP12 + ttlMFRNP12 + ttlCFSP12 + ttlSafetyP12 + ttlAuditP12 + ttlFabricP12 + ttlPenaltyP12 + ttlQCCP12 + ttlCHBP12 + ttlGBTestP12 + ttlFIRAP12 + ttlOthersP12) * 100).ToString("#,##0.00");
            this.lbl_TotalPaidByNSPct.Text = (ttlReworkTotal + ttlRejectTotal + ttlMFRNTotal + ttlCFSTotal + ttlSafetyTotal + ttlAuditTotal + ttlFabricTotal + ttlPenaltyTotal + ttlQCCTotal + ttlCHBTotal + ttlGBTestTotal + ttlFIRATotal + ttlOthersTotal) == 0 ? "N/A" : (ttlNSTotal / (ttlReworkTotal + ttlRejectTotal + ttlMFRNTotal + ttlCFSTotal + ttlSafetyTotal + ttlAuditTotal + ttlFabricTotal + ttlPenaltyTotal + ttlQCCTotal + ttlCHBTotal + ttlGBTestTotal + ttlFIRATotal + ttlOthersTotal) * 100).ToString("#,##0.00");

            this.lbl_GrandRatioP1.Text = (ttlNSP01 + ttlVendorP01).ToString("#,##0");
            this.lbl_GrandRatioP2.Text = (ttlNSP02 + ttlVendorP02).ToString("#,##0");
            this.lbl_GrandRatioP3.Text = (ttlNSP03 + ttlVendorP03).ToString("#,##0");
            this.lbl_GrandRatioP4.Text = (ttlNSP04 + ttlVendorP04).ToString("#,##0");
            this.lbl_GrandRatioP5.Text = (ttlNSP05 + ttlVendorP05).ToString("#,##0");
            this.lbl_GrandRatioP6.Text = (ttlNSP06 + ttlVendorP06).ToString("#,##0");
            this.lbl_GrandRatioP7.Text = (ttlNSP07 + ttlVendorP07).ToString("#,##0");
            this.lbl_GrandRatioP8.Text = (ttlNSP08 + ttlVendorP08).ToString("#,##0");
            this.lbl_GrandRatioP9.Text = (ttlNSP09 + ttlVendorP09).ToString("#,##0");
            this.lbl_GrandRatioP10.Text = (ttlNSP10 + ttlVendorP10).ToString("#,##0");
            this.lbl_GrandRatioP11.Text = (ttlNSP11 + ttlVendorP11).ToString("#,##0");
            this.lbl_GrandRatioP12.Text = (ttlNSP12 + ttlVendorP12).ToString("#,##0");
            this.lbl_GrandRatioTotal.Text = (ttlNSTotal + ttlVendorTotal).ToString("#,##0");

            this.lbl_LY_P1PaidBySupplierAmt.Text = ttlLYVendorP01.ToString("#,##0");
            this.lbl_LY_P2PaidBySupplierAmt.Text = ttlLYVendorP02.ToString("#,##0");
            this.lbl_LY_P3PaidBySupplierAmt.Text = ttlLYVendorP03.ToString("#,##0");
            this.lbl_LY_P4PaidBySupplierAmt.Text = ttlLYVendorP04.ToString("#,##0");
            this.lbl_LY_P5PaidBySupplierAmt.Text = ttlLYVendorP05.ToString("#,##0");
            this.lbl_LY_P6PaidBySupplierAmt.Text = ttlLYVendorP06.ToString("#,##0");
            this.lbl_LY_P7PaidBySupplierAmt.Text = ttlLYVendorP07.ToString("#,##0");
            this.lbl_LY_P8PaidBySupplierAmt.Text = ttlLYVendorP08.ToString("#,##0");
            this.lbl_LY_P9PaidBySupplierAmt.Text = ttlLYVendorP09.ToString("#,##0");
            this.lbl_LY_P10PaidBySupplierAmt.Text = ttlLYVendorP10.ToString("#,##0");
            this.lbl_LY_P11PaidBySupplierAmt.Text = ttlLYVendorP11.ToString("#,##0");
            this.lbl_LY_P12PaidBySupplierAmt.Text = ttlLYVendorP12.ToString("#,##0");
            this.lbl_LY_TotalPaidBySupplierAmt.Text = ttlLYVendorTotal.ToString("#,##0");

            this.lbl_LY_P1PaidBySupplierPct.Text = (ttlLYReworkP01 + ttlLYRejectP01 + ttlLYMFRNP01 + ttlLYCFSP01 + ttlLYSafetyP01 + ttlLYAuditP01 + ttlLYFabricP01 + ttlLYPenaltyP01 + ttlLYQCCP01 + ttlLYCHBP01 + ttlLYGBTestP01 + ttlLYFIRAP01 + ttlLYOthersP01) == 0 ? "N/A" : (ttlLYVendorP01 / (ttlLYReworkP01 + ttlLYRejectP01 + ttlLYMFRNP01 + ttlLYCFSP01 + ttlLYSafetyP01 + ttlLYAuditP01 + ttlLYFabricP01 + ttlLYPenaltyP01 + ttlLYQCCP01 + ttlLYCHBP01 + ttlLYGBTestP01 + ttlLYFIRAP01 + ttlLYOthersP01) * 100).ToString("#,##0.00");
            this.lbl_LY_P2PaidBySupplierPct.Text = (ttlLYReworkP02 + ttlLYRejectP02 + ttlLYMFRNP02 + ttlLYCFSP02 + ttlLYSafetyP02 + ttlLYAuditP02 + ttlLYFabricP02 + ttlLYPenaltyP02 + ttlLYQCCP02 + ttlLYCHBP02 + ttlLYGBTestP02 + ttlLYFIRAP02 + ttlLYOthersP02) == 0 ? "N/A" : (ttlLYVendorP02 / (ttlLYReworkP02 + ttlLYRejectP02 + ttlLYMFRNP02 + ttlLYCFSP02 + ttlLYSafetyP02 + ttlLYAuditP02 + ttlLYFabricP02 + ttlLYPenaltyP02 + ttlLYQCCP02 + ttlLYCHBP02 + ttlLYGBTestP02 + ttlLYFIRAP02 + ttlLYOthersP02) * 100).ToString("#,##0.00");
            this.lbl_LY_P3PaidBySupplierPct.Text = (ttlLYReworkP03 + ttlLYRejectP03 + ttlLYMFRNP03 + ttlLYCFSP03 + ttlLYSafetyP03 + ttlLYAuditP03 + ttlLYFabricP03 + ttlLYPenaltyP03 + ttlLYQCCP03 + ttlLYCHBP03 + ttlLYGBTestP03 + ttlLYFIRAP03 + ttlLYOthersP03) == 0 ? "N/A" : (ttlLYVendorP03 / (ttlLYReworkP03 + ttlLYRejectP03 + ttlLYMFRNP03 + ttlLYCFSP03 + ttlLYSafetyP03 + ttlLYAuditP03 + ttlLYFabricP03 + ttlLYPenaltyP03 + ttlLYQCCP03 + ttlLYCHBP03 + ttlLYGBTestP03 + ttlLYFIRAP03 + ttlLYOthersP03) * 100).ToString("#,##0.00");
            this.lbl_LY_P4PaidBySupplierPct.Text = (ttlLYReworkP04 + ttlLYRejectP04 + ttlLYMFRNP04 + ttlLYCFSP04 + ttlLYSafetyP04 + ttlLYAuditP04 + ttlLYFabricP04 + ttlLYPenaltyP04 + ttlLYQCCP04 + ttlLYCHBP04 + ttlLYGBTestP04 + ttlLYFIRAP04 + ttlLYOthersP04) == 0 ? "N/A" : (ttlLYVendorP04 / (ttlLYReworkP04 + ttlLYRejectP04 + ttlLYMFRNP04 + ttlLYCFSP04 + ttlLYSafetyP04 + ttlLYAuditP04 + ttlLYFabricP04 + ttlLYPenaltyP04 + ttlLYQCCP04 + ttlLYCHBP04 + ttlLYGBTestP04 + ttlLYFIRAP04 + ttlLYOthersP04) * 100).ToString("#,##0.00");
            this.lbl_LY_P5PaidBySupplierPct.Text = (ttlLYReworkP05 + ttlLYRejectP05 + ttlLYMFRNP05 + ttlLYCFSP05 + ttlLYSafetyP05 + ttlLYAuditP05 + ttlLYFabricP05 + ttlLYPenaltyP05 + ttlLYQCCP05 + ttlLYCHBP05 + ttlLYGBTestP05 + ttlLYFIRAP05 + ttlLYOthersP05) == 0 ? "N/A" : (ttlLYVendorP05 / (ttlLYReworkP05 + ttlLYRejectP05 + ttlLYMFRNP05 + ttlLYCFSP05 + ttlLYSafetyP05 + ttlLYAuditP05 + ttlLYFabricP05 + ttlLYPenaltyP05 + ttlLYQCCP05 + ttlLYCHBP05 + ttlLYGBTestP05 + ttlLYFIRAP05 + ttlLYOthersP05) * 100).ToString("#,##0.00");
            this.lbl_LY_P6PaidBySupplierPct.Text = (ttlLYReworkP06 + ttlLYRejectP06 + ttlLYMFRNP06 + ttlLYCFSP06 + ttlLYSafetyP06 + ttlLYAuditP06 + ttlLYFabricP06 + ttlLYPenaltyP06 + ttlLYQCCP06 + ttlLYCHBP06 + ttlLYGBTestP06 + ttlLYFIRAP06 + ttlLYOthersP06) == 0 ? "N/A" : (ttlLYVendorP06 / (ttlLYReworkP06 + ttlLYRejectP06 + ttlLYMFRNP06 + ttlLYCFSP06 + ttlLYSafetyP06 + ttlLYAuditP06 + ttlLYFabricP06 + ttlLYPenaltyP06 + ttlLYQCCP06 + ttlLYCHBP06 + ttlLYGBTestP06 + ttlLYFIRAP06 + ttlLYOthersP06) * 100).ToString("#,##0.00");
            this.lbl_LY_P7PaidBySupplierPct.Text = (ttlLYReworkP07 + ttlLYRejectP07 + ttlLYMFRNP07 + ttlLYCFSP07 + ttlLYSafetyP07 + ttlLYAuditP07 + ttlLYFabricP07 + ttlLYPenaltyP07 + ttlLYQCCP07 + ttlLYCHBP07 + ttlLYGBTestP07 + ttlLYFIRAP07 + ttlLYOthersP07) == 0 ? "N/A" : (ttlLYVendorP07 / (ttlLYReworkP07 + ttlLYRejectP07 + ttlLYMFRNP07 + ttlLYCFSP07 + ttlLYSafetyP07 + ttlLYAuditP07 + ttlLYFabricP07 + ttlLYPenaltyP07 + ttlLYQCCP07 + ttlLYCHBP07 + ttlLYGBTestP07 + ttlLYFIRAP07 + ttlLYOthersP07) * 100).ToString("#,##0.00");
            this.lbl_LY_P8PaidBySupplierPct.Text = (ttlLYReworkP08 + ttlLYRejectP08 + ttlLYMFRNP08 + ttlLYCFSP08 + ttlLYSafetyP08 + ttlLYAuditP08 + ttlLYFabricP08 + ttlLYPenaltyP08 + ttlLYQCCP08 + ttlLYCHBP08 + ttlLYGBTestP08 + ttlLYFIRAP08 + ttlLYOthersP08) == 0 ? "N/A" : (ttlLYVendorP08 / (ttlLYReworkP08 + ttlLYRejectP08 + ttlLYMFRNP08 + ttlLYCFSP08 + ttlLYSafetyP08 + ttlLYAuditP08 + ttlLYFabricP08 + ttlLYPenaltyP08 + ttlLYQCCP08 + ttlLYCHBP08 + ttlLYGBTestP08 + ttlLYFIRAP08 + ttlLYOthersP08) * 100).ToString("#,##0.00");
            this.lbl_LY_P9PaidBySupplierPct.Text = (ttlLYReworkP09 + ttlLYRejectP09 + ttlLYMFRNP09 + ttlLYCFSP09 + ttlLYSafetyP09 + ttlLYAuditP09 + ttlLYFabricP09 + ttlLYPenaltyP09 + ttlLYQCCP09 + ttlLYCHBP09 + ttlLYGBTestP09 + ttlLYFIRAP09 + ttlLYOthersP09) == 0 ? "N/A" : (ttlLYVendorP09 / (ttlLYReworkP09 + ttlLYRejectP09 + ttlLYMFRNP09 + ttlLYCFSP09 + ttlLYSafetyP09 + ttlLYAuditP09 + ttlLYFabricP09 + ttlLYPenaltyP09 + ttlLYQCCP09 + ttlLYCHBP09 + ttlLYGBTestP09 + ttlLYFIRAP09 + ttlLYOthersP09) * 100).ToString("#,##0.00");
            this.lbl_LY_P10PaidBySupplierPct.Text = (ttlLYReworkP10 + ttlLYRejectP10 + ttlLYMFRNP10 + ttlLYCFSP10 + ttlLYSafetyP10 + ttlLYAuditP10 + ttlLYFabricP10 + ttlLYPenaltyP10 + ttlLYQCCP10 + ttlLYCHBP10 + ttlLYGBTestP10 + ttlLYFIRAP10 + ttlLYOthersP10) == 0 ? "N/A" : (ttlLYVendorP10 / (ttlLYReworkP10 + ttlLYRejectP10 + ttlLYMFRNP10 + ttlLYCFSP10 + ttlLYSafetyP10 + ttlLYAuditP10 + ttlLYFabricP10 + ttlLYPenaltyP10 + ttlLYQCCP10 + ttlLYCHBP10 + ttlLYGBTestP10 + ttlLYFIRAP10 + ttlLYOthersP10) * 100).ToString("#,##0.00");
            this.lbl_LY_P11PaidBySupplierPct.Text = (ttlLYReworkP11 + ttlLYRejectP11 + ttlLYMFRNP11 + ttlLYCFSP11 + ttlLYSafetyP11 + ttlLYAuditP11 + ttlLYFabricP11 + ttlLYPenaltyP11 + ttlLYQCCP11 + ttlLYCHBP11 + ttlLYGBTestP11 + ttlLYFIRAP11 + ttlLYOthersP11) == 0 ? "N/A" : (ttlLYVendorP11 / (ttlLYReworkP11 + ttlLYRejectP11 + ttlLYMFRNP11 + ttlLYCFSP11 + ttlLYSafetyP11 + ttlLYAuditP11 + ttlLYFabricP11 + ttlLYPenaltyP11 + ttlLYQCCP11 + ttlLYCHBP11 + ttlLYGBTestP11 + ttlLYFIRAP11 + ttlLYOthersP11) * 100).ToString("#,##0.00");
            this.lbl_LY_P12PaidBySupplierPct.Text = (ttlLYReworkP12 + ttlLYRejectP12 + ttlLYMFRNP12 + ttlLYCFSP12 + ttlLYSafetyP12 + ttlLYAuditP12 + ttlLYFabricP12 + ttlLYPenaltyP12 + ttlLYQCCP12 + ttlLYCHBP12 + ttlLYGBTestP12 + ttlLYFIRAP12 + ttlLYOthersP12) == 0 ? "N/A" : (ttlLYVendorP12 / (ttlLYReworkP12 + ttlLYRejectP12 + ttlLYMFRNP12 + ttlLYCFSP12 + ttlLYSafetyP12 + ttlLYAuditP12 + ttlLYFabricP12 + ttlLYPenaltyP12 + ttlLYQCCP12 + ttlLYCHBP12 + ttlLYGBTestP12 + ttlLYFIRAP12 + ttlLYOthersP12) * 100).ToString("#,##0.00");
            this.lbl_LY_TotalPaidBySupplierPct.Text = (ttlLYReworkTotal + ttlLYRejectTotal + ttlLYMFRNTotal + ttlLYCFSTotal + ttlLYSafetyTotal + ttlLYAuditTotal + ttlLYFabricTotal + ttlLYPenaltyTotal + ttlLYQCCTotal + ttlLYCHBTotal + ttlLYGBTestTotal + ttlLYFIRATotal + ttlLYOthersTotal) == 0 ? "N/A" : (ttlLYVendorTotal / (ttlLYReworkTotal + ttlLYRejectTotal + ttlLYMFRNTotal + ttlLYCFSTotal + ttlLYSafetyTotal + ttlLYAuditTotal + ttlLYFabricTotal + ttlLYPenaltyTotal + ttlLYQCCTotal + ttlLYCHBTotal + ttlLYGBTestTotal + ttlLYFIRATotal + ttlLYOthersTotal) * 100).ToString("#,##0.00");

            this.lbl_LY_P1PaidByNSAmt.Text = ttlLYNSP01.ToString("#,##0");
            this.lbl_LY_P2PaidByNSAmt.Text = ttlLYNSP02.ToString("#,##0");
            this.lbl_LY_P3PaidByNSAmt.Text = ttlLYNSP03.ToString("#,##0");
            this.lbl_LY_P4PaidByNSAmt.Text = ttlLYNSP04.ToString("#,##0");
            this.lbl_LY_P5PaidByNSAmt.Text = ttlLYNSP05.ToString("#,##0");
            this.lbl_LY_P6PaidByNSAmt.Text = ttlLYNSP06.ToString("#,##0");
            this.lbl_LY_P7PaidByNSAmt.Text = ttlLYNSP07.ToString("#,##0");
            this.lbl_LY_P8PaidByNSAmt.Text = ttlLYNSP08.ToString("#,##0");
            this.lbl_LY_P9PaidByNSAmt.Text = ttlLYNSP09.ToString("#,##0");
            this.lbl_LY_P10PaidByNSAmt.Text = ttlLYNSP10.ToString("#,##0");
            this.lbl_LY_P11PaidByNSAmt.Text = ttlLYNSP11.ToString("#,##0");
            this.lbl_LY_P12PaidByNSAmt.Text = ttlLYNSP12.ToString("#,##0");
            this.lbl_LY_TotalPaidByNSAmt.Text = ttlLYNSTotal.ToString("#,##0");

            this.lbl_LY_P1PaidByNSPct.Text = (ttlLYReworkP01 + ttlLYRejectP01 + ttlLYMFRNP01 + ttlLYCFSP01 + ttlLYSafetyP01 + ttlLYAuditP01 + ttlLYFabricP01 + ttlLYPenaltyP01 + ttlLYQCCP01 + ttlLYCHBP01 + ttlLYGBTestP01 + ttlLYFIRAP01 + ttlLYOthersP01) == 0 ? "N/A" : (ttlLYNSP01 / (ttlLYReworkP01 + ttlLYRejectP01 + ttlLYMFRNP01 + ttlLYCFSP01 + ttlLYSafetyP01 + ttlLYAuditP01 + ttlLYFabricP01 + ttlLYPenaltyP01 + ttlLYQCCP01 + ttlLYCHBP01 + ttlLYGBTestP01 + ttlLYFIRAP01 + ttlLYOthersP01) * 100).ToString("#,##0.00");
            this.lbl_LY_P2PaidByNSPct.Text = (ttlLYReworkP02 + ttlLYRejectP02 + ttlLYMFRNP02 + ttlLYCFSP02 + ttlLYSafetyP02 + ttlLYAuditP02 + ttlLYFabricP02 + ttlLYPenaltyP02 + ttlLYQCCP02 + ttlLYCHBP02 + ttlLYGBTestP02 + ttlLYFIRAP02 + ttlLYOthersP02) == 0 ? "N/A" : (ttlLYNSP02 / (ttlLYReworkP02 + ttlLYRejectP02 + ttlLYMFRNP02 + ttlLYCFSP02 + ttlLYSafetyP02 + ttlLYAuditP02 + ttlLYFabricP02 + ttlLYPenaltyP02 + ttlLYQCCP02 + ttlLYCHBP02 + ttlLYGBTestP02 + ttlLYFIRAP02 + ttlLYOthersP02) * 100).ToString("#,##0.00");
            this.lbl_LY_P3PaidByNSPct.Text = (ttlLYReworkP03 + ttlLYRejectP03 + ttlLYMFRNP03 + ttlLYCFSP03 + ttlLYSafetyP03 + ttlLYAuditP03 + ttlLYFabricP03 + ttlLYPenaltyP03 + ttlLYQCCP03 + ttlLYCHBP03 + ttlLYGBTestP03 + ttlLYFIRAP03 + ttlLYOthersP03) == 0 ? "N/A" : (ttlLYNSP03 / (ttlLYReworkP03 + ttlLYRejectP03 + ttlLYMFRNP03 + ttlLYCFSP03 + ttlLYSafetyP03 + ttlLYAuditP03 + ttlLYFabricP03 + ttlLYPenaltyP03 + ttlLYQCCP03 + ttlLYCHBP03 + ttlLYGBTestP03 + ttlLYFIRAP03 + ttlLYOthersP03) * 100).ToString("#,##0.00");
            this.lbl_LY_P4PaidByNSPct.Text = (ttlLYReworkP04 + ttlLYRejectP04 + ttlLYMFRNP04 + ttlLYCFSP04 + ttlLYSafetyP04 + ttlLYAuditP04 + ttlLYFabricP04 + ttlLYPenaltyP04 + ttlLYQCCP04 + ttlLYCHBP04 + ttlLYGBTestP04 + ttlLYFIRAP04 + ttlLYOthersP04) == 0 ? "N/A" : (ttlLYNSP04 / (ttlLYReworkP04 + ttlLYRejectP04 + ttlLYMFRNP04 + ttlLYCFSP04 + ttlLYSafetyP04 + ttlLYAuditP04 + ttlLYFabricP04 + ttlLYPenaltyP04 + ttlLYQCCP04 + ttlLYCHBP04 + ttlLYGBTestP04 + ttlLYFIRAP04 + ttlLYOthersP04) * 100).ToString("#,##0.00");
            this.lbl_LY_P5PaidByNSPct.Text = (ttlLYReworkP05 + ttlLYRejectP05 + ttlLYMFRNP05 + ttlLYCFSP05 + ttlLYSafetyP05 + ttlLYAuditP05 + ttlLYFabricP05 + ttlLYPenaltyP05 + ttlLYQCCP05 + ttlLYCHBP05 + ttlLYGBTestP05 + ttlLYFIRAP05 + ttlLYOthersP05) == 0 ? "N/A" : (ttlLYNSP05 / (ttlLYReworkP05 + ttlLYRejectP05 + ttlLYMFRNP05 + ttlLYCFSP05 + ttlLYSafetyP05 + ttlLYAuditP05 + ttlLYFabricP05 + ttlLYPenaltyP05 + ttlLYQCCP05 + ttlLYCHBP05 + ttlLYGBTestP05 + ttlLYFIRAP05 + ttlLYOthersP05) * 100).ToString("#,##0.00");
            this.lbl_LY_P6PaidByNSPct.Text = (ttlLYReworkP06 + ttlLYRejectP06 + ttlLYMFRNP06 + ttlLYCFSP06 + ttlLYSafetyP06 + ttlLYAuditP06 + ttlLYFabricP06 + ttlLYPenaltyP06 + ttlLYQCCP06 + ttlLYCHBP06 + ttlLYGBTestP06 + ttlLYFIRAP06 + ttlLYOthersP06) == 0 ? "N/A" : (ttlLYNSP06 / (ttlLYReworkP06 + ttlLYRejectP06 + ttlLYMFRNP06 + ttlLYCFSP06 + ttlLYSafetyP06 + ttlLYAuditP06 + ttlLYFabricP06 + ttlLYPenaltyP06 + ttlLYQCCP06 + ttlLYCHBP06 + ttlLYGBTestP06 + ttlLYFIRAP06 + ttlLYOthersP06) * 100).ToString("#,##0.00");
            this.lbl_LY_P7PaidByNSPct.Text = (ttlLYReworkP07 + ttlLYRejectP07 + ttlLYMFRNP07 + ttlLYCFSP07 + ttlLYSafetyP07 + ttlLYAuditP07 + ttlLYFabricP07 + ttlLYPenaltyP07 + ttlLYQCCP07 + ttlLYCHBP07 + ttlLYGBTestP07 + ttlLYFIRAP07 + ttlLYOthersP07) == 0 ? "N/A" : (ttlLYNSP07 / (ttlLYReworkP07 + ttlLYRejectP07 + ttlLYMFRNP07 + ttlLYCFSP07 + ttlLYSafetyP07 + ttlLYAuditP07 + ttlLYFabricP07 + ttlLYPenaltyP07 + ttlLYQCCP07 + ttlLYCHBP07 + ttlLYGBTestP07 + ttlLYFIRAP07 + ttlLYOthersP07) * 100).ToString("#,##0.00");
            this.lbl_LY_P8PaidByNSPct.Text = (ttlLYReworkP08 + ttlLYRejectP08 + ttlLYMFRNP08 + ttlLYCFSP08 + ttlLYSafetyP08 + ttlLYAuditP08 + ttlLYFabricP08 + ttlLYPenaltyP08 + ttlLYQCCP08 + ttlLYCHBP08 + ttlLYGBTestP08 + ttlLYFIRAP08 + ttlLYOthersP08) == 0 ? "N/A" : (ttlLYNSP08 / (ttlLYReworkP08 + ttlLYRejectP08 + ttlLYMFRNP08 + ttlLYCFSP08 + ttlLYSafetyP08 + ttlLYAuditP08 + ttlLYFabricP08 + ttlLYPenaltyP08 + ttlLYQCCP08 + ttlLYCHBP08 + ttlLYGBTestP08 + ttlLYFIRAP08 + ttlLYOthersP08) * 100).ToString("#,##0.00");
            this.lbl_LY_P9PaidByNSPct.Text = (ttlLYReworkP09 + ttlLYRejectP09 + ttlLYMFRNP09 + ttlLYCFSP09 + ttlLYSafetyP09 + ttlLYAuditP09 + ttlLYFabricP09 + ttlLYPenaltyP09 + ttlLYQCCP09 + ttlLYCHBP09 + ttlLYGBTestP09 + ttlLYFIRAP09 + ttlLYOthersP09) == 0 ? "N/A" : (ttlLYNSP09 / (ttlLYReworkP09 + ttlLYRejectP09 + ttlLYMFRNP09 + ttlLYCFSP09 + ttlLYSafetyP09 + ttlLYAuditP09 + ttlLYFabricP09 + ttlLYPenaltyP09 + ttlLYQCCP09 + ttlLYCHBP09 + ttlLYGBTestP09 + ttlLYFIRAP09 + ttlLYOthersP09) * 100).ToString("#,##0.00");
            this.lbl_LY_P10PaidByNSPct.Text = (ttlLYReworkP10 + ttlLYRejectP10 + ttlLYMFRNP10 + ttlLYCFSP10 + ttlLYSafetyP10 + ttlLYAuditP10 + ttlLYFabricP10 + ttlLYPenaltyP10 + ttlLYQCCP10 + ttlLYCHBP10 + ttlLYGBTestP10 + ttlLYFIRAP10 + ttlLYOthersP10) == 0 ? "N/A" : (ttlLYNSP10 / (ttlLYReworkP10 + ttlLYRejectP10 + ttlLYMFRNP10 + ttlLYCFSP10 + ttlLYSafetyP10 + ttlLYAuditP10 + ttlLYFabricP10 + ttlLYPenaltyP10 + ttlLYQCCP10 + ttlLYCHBP10 + ttlLYGBTestP10 + ttlLYFIRAP10 + ttlLYOthersP10) * 100).ToString("#,##0.00");
            this.lbl_LY_P11PaidByNSPct.Text = (ttlLYReworkP11 + ttlLYRejectP11 + ttlLYMFRNP11 + ttlLYCFSP11 + ttlLYSafetyP11 + ttlLYAuditP11 + ttlLYFabricP11 + ttlLYPenaltyP11 + ttlLYQCCP11 + ttlLYCHBP11 + ttlLYGBTestP11 + ttlLYFIRAP11 + ttlLYOthersP11) == 0 ? "N/A" : (ttlLYNSP11 / (ttlLYReworkP11 + ttlLYRejectP11 + ttlLYMFRNP11 + ttlLYCFSP11 + ttlLYSafetyP11 + ttlLYAuditP11 + ttlLYFabricP11 + ttlLYPenaltyP11 + ttlLYQCCP11 + ttlLYCHBP11 + ttlLYGBTestP11 + ttlLYFIRAP11 + ttlLYOthersP11) * 100).ToString("#,##0.00");
            this.lbl_LY_P12PaidByNSPct.Text = (ttlLYReworkP12 + ttlLYRejectP12 + ttlLYMFRNP12 + ttlLYCFSP12 + ttlLYSafetyP12 + ttlLYAuditP12 + ttlLYFabricP12 + ttlLYPenaltyP12 + ttlLYQCCP12 + ttlLYCHBP12 + ttlLYGBTestP12 + ttlLYFIRAP12 + ttlLYOthersP12) == 0 ? "N/A" : (ttlLYNSP12 / (ttlLYReworkP12 + ttlLYRejectP12 + ttlLYMFRNP12 + ttlLYCFSP12 + ttlLYSafetyP12 + ttlLYAuditP12 + ttlLYFabricP12 + ttlLYPenaltyP12 + ttlLYQCCP12 + ttlLYCHBP12 + ttlLYGBTestP12 + ttlLYFIRAP12 + ttlLYOthersP12) * 100).ToString("#,##0.00");
            this.lbl_LY_TotalPaidByNSPct.Text = (ttlLYReworkTotal + ttlLYRejectTotal + ttlLYMFRNTotal + ttlLYCFSTotal + ttlLYSafetyTotal + ttlLYAuditTotal + ttlLYFabricTotal + ttlLYPenaltyTotal + ttlLYQCCTotal + ttlLYCHBTotal + ttlLYGBTestTotal + ttlLYFIRATotal + ttlLYOthersTotal) == 0 ? "N/A" : (ttlLYNSTotal / (ttlLYReworkTotal + ttlLYRejectTotal + ttlLYMFRNTotal + ttlLYCFSTotal + ttlLYSafetyTotal + ttlLYAuditTotal + ttlLYFabricTotal + ttlLYPenaltyTotal + ttlLYQCCTotal + ttlLYCHBTotal + ttlLYGBTestTotal + ttlLYFIRATotal + ttlLYOthersTotal) * 100).ToString("#,##0.00");

            this.lbl_LY_GrandRatioP1.Text = (ttlLYNSP01 + ttlLYVendorP01).ToString("#,##0");
            this.lbl_LY_GrandRatioP2.Text = (ttlLYNSP02 + ttlLYVendorP02).ToString("#,##0");
            this.lbl_LY_GrandRatioP3.Text = (ttlLYNSP03 + ttlLYVendorP03).ToString("#,##0");
            this.lbl_LY_GrandRatioP4.Text = (ttlLYNSP04 + ttlLYVendorP04).ToString("#,##0");
            this.lbl_LY_GrandRatioP5.Text = (ttlLYNSP05 + ttlLYVendorP05).ToString("#,##0");
            this.lbl_LY_GrandRatioP6.Text = (ttlLYNSP06 + ttlLYVendorP06).ToString("#,##0");
            this.lbl_LY_GrandRatioP7.Text = (ttlLYNSP07 + ttlLYVendorP07).ToString("#,##0");
            this.lbl_LY_GrandRatioP8.Text = (ttlLYNSP08 + ttlLYVendorP08).ToString("#,##0");
            this.lbl_LY_GrandRatioP9.Text = (ttlLYNSP09 + ttlLYVendorP09).ToString("#,##0");
            this.lbl_LY_GrandRatioP10.Text = (ttlLYNSP10 + ttlLYVendorP10).ToString("#,##0");
            this.lbl_LY_GrandRatioP11.Text = (ttlLYNSP11 + ttlLYVendorP11).ToString("#,##0");
            this.lbl_LY_GrandRatioP12.Text = (ttlLYNSP12 + ttlLYVendorP12).ToString("#,##0");
            this.lbl_LY_GrandRatioTotal.Text = (ttlLYNSTotal + ttlLYVendorTotal).ToString("#,##0");
            #endregion Report Summary

            isFirst = true;
            ttlVendorP01 = 0;
            ttlVendorP02 = 0;
            ttlVendorP03 = 0;
            ttlVendorP04 = 0;
            ttlVendorP05 = 0;
            ttlVendorP06 = 0;
            ttlVendorP07 = 0;
            ttlVendorP08 = 0;
            ttlVendorP09 = 0;
            ttlVendorP10 = 0;
            ttlVendorP11 = 0;
            ttlVendorP12 = 0;
            ttlVendorTotal = 0;

            if (!isSingleVendor)
            {
                if (!isSummary)
                {
                    this.repUKClaimSummary.DataSource = list;
                    this.repUKClaimSummary.DataBind();
                }
            }
            else
            {
                // Show some of the figure in current year
                this.lbl_CurrentYearItem.Text = this.lbl_Supplier.Text;
                this.trCurrentYearTopMargin.Visible = false;
                this.trCurrentYearHeading.Visible = false;

                this.trCurrentYearClaimRatioTopMargin.Visible = false;
                this.trCurrentYearClaimRatioCurrency.Text = "";
                this.trCurrentYearClaimRatioItem.Text = "";
                this.tdCurrentYearClaimRatioItem.Style.Add(HtmlTextWriterStyle.BorderStyle, "none");
                this.tdCurrentYearClaimRatioCurrency.Style.Add(HtmlTextWriterStyle.BorderStyle, "none");
                this.trCurrentYearClaimRatioPaidAmtBySupplier.Visible = true;
                this.trCurrentYearClaimRatioPaidPercentBySupplier.Visible = false;
                this.trCurrentYearClaimRatioPaidAmtByNS.Visible = true;
                this.trCurrentYearClaimRatioPaidPercentByNS.Visible = false;
                this.trCurrentYearClaimRatioGrandTotal.Visible = false;

                // Disable Last Year Summary
                this.trLastYearTopMargin.Visible = false;
                this.trLastYearHeading.Visible = false;
                this.trLastYearRework.Visible = false;
                this.trLastYearReject.Visible = false;
                this.trLastYearMFRN.Visible = false;
                this.trLastYearCFS.Visible = false;
                this.trLastYearSafetyIssue.Visible = false;
                this.trLastYearAuditFee.Visible = false;
                this.trLastYearFabricTest.Visible = false;
                this.trLastYearPenaltyCharge.Visible = false;
                this.trLastYearQCC.Visible = false;
                this.trLastYearCHB.Visible = false;
                this.trLastYearFIRA.Visible = false;
                this.trLastYearOthers.Visible = false;
                this.trLastYearGrandTotal.Visible = false;

                this.trLastYearClaimRatioTopMargin.Visible = false;
                this.trLastYearClaimRatioPaidAmtBySupplier.Visible = false;
                this.trLastYearClaimRatioPaidPercentBySupplier.Visible = false;
                this.trLastYearClaimRatioPaidAmtByNS.Visible = false;
                this.trLastYearClaimRatioPaidPercentByNS.Visible = false;
                this.trLastYearClaimRatioGrandTotal.Visible = false;


            }

        }

        private List<UKClaimPhasingByProductTeamDef> vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (List<UKClaimPhasingByProductTeamDef>)ViewState["SearchResult"];
            }
        }

        protected void repUKClaimSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HtmlTableRow tr_SubTotal = (HtmlTableRow)e.Item.FindControl("trSubTotal");
            HtmlTableRow tr_GroupHeading = (HtmlTableRow)e.Item.FindControl("trGroupHeading");
            HtmlTableRow tr_GroupFooter = (HtmlTableRow)e.Item.FindControl("trGroupFooter");
            HtmlTableRow tr_Summary = (HtmlTableRow)e.Item.FindControl("trSummary");
            HtmlTableRow tr_BlankLine = (HtmlTableRow)e.Item.FindControl("trBlankLine");
            HtmlTableRow tr_BlankHeading = (HtmlTableRow)e.Item.FindControl("trBlankHeading");
            HtmlTableRow tr_Normal = (HtmlTableRow)e.Item.FindControl("trNormal");
            bool reportEnd = false;

            if (!WebHelper.isRepeaterFooter(e))
            {
                tr_GroupHeading.Visible = false;
                tr_GroupFooter.Visible = false;
                tr_BlankLine.Visible = false;
                tr_Summary.Visible = false;
                tr_SubTotal.Visible = false;
                tr_Normal.Visible = false;
                tr_BlankHeading.Visible = false;

                UKClaimPhasingByProductTeamDef def = (UKClaimPhasingByProductTeamDef)vwSearchResult[e.Item.ItemIndex];
                //((Label)e.Item.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
                if (isFirst)
                {
                    tr_BlankHeading.Visible = true;
                    tr_GroupHeading.Visible = true;
                    ((Label)e.Item.FindControl("lbl_GroupHeading")).Text = "Office Summary";
                    isFirst = false;
                }
                #region Calulate Total

                count += 1;
                ttlOfficeSubP01 += def.P01AmountInUSD;
                ttlOfficeSubP02 += def.P02AmountInUSD;
                ttlOfficeSubP03 += def.P03AmountInUSD;
                ttlOfficeSubP04 += def.P04AmountInUSD;
                ttlOfficeSubP05 += def.P05AmountInUSD;
                ttlOfficeSubP06 += def.P06AmountInUSD;
                ttlOfficeSubP07 += def.P07AmountInUSD;
                ttlOfficeSubP08 += def.P08AmountInUSD;
                ttlOfficeSubP09 += def.P09AmountInUSD;
                ttlOfficeSubP10 += def.P10AmountInUSD;
                ttlOfficeSubP11 += def.P11AmountInUSD;
                ttlOfficeSubP12 += def.P12AmountInUSD;
                ttlOfficeSubTotal += def.TotalAmountInUSD;

                ttlVendorP01 += def.P01VendorAmountInUSD;
                ttlVendorP02 += def.P02VendorAmountInUSD;
                ttlVendorP03 += def.P03VendorAmountInUSD;
                ttlVendorP04 += def.P04VendorAmountInUSD;
                ttlVendorP05 += def.P05VendorAmountInUSD;
                ttlVendorP06 += def.P06VendorAmountInUSD;
                ttlVendorP07 += def.P07VendorAmountInUSD;
                ttlVendorP08 += def.P08VendorAmountInUSD;
                ttlVendorP09 += def.P09VendorAmountInUSD;
                ttlVendorP10 += def.P10VendorAmountInUSD;
                ttlVendorP11 += def.P11VendorAmountInUSD;
                ttlVendorP12 += def.P12VendorAmountInUSD;
                ttlVendorTotal += def.TotalVendorAmountInUSD;

                #endregion Calculate Total


                //if (!isSummary)
                {   // Detail Report - Group Footer (After calculating total amount)
                    #region Office Group Total
                    bool groupEnd = true;
                    if ((e.Item.ItemIndex + 1) < vwSearchResult.Count)
                        groupEnd = (((UKClaimPhasingByProductTeamDef)vwSearchResult[e.Item.ItemIndex + 1]).OfficeId != def.OfficeId);
                    if ((e.Item.ItemIndex + 1) == vwSearchResult.Count)
                        reportEnd = true;

                    if (groupEnd)
                    {
                        //tr_Normal.Visible = true;
                        //tr_GroupFooter.Visible = true;
                        tr_Normal.Visible = true;
                        ((Label)e.Item.FindControl("lbl_Group")).Text = CommonUtil.getOfficeRefByKey(def.OfficeId).Description;
                        ((Label)e.Item.FindControl("lbl_Currency")).Text = "USD";
                        ((Label)e.Item.FindControl("lbl_P01")).Text = ttlOfficeSubP01.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P02")).Text = ttlOfficeSubP02.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P03")).Text = ttlOfficeSubP03.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P04")).Text = ttlOfficeSubP04.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P05")).Text = ttlOfficeSubP05.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P06")).Text = ttlOfficeSubP06.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P07")).Text = ttlOfficeSubP07.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P08")).Text = ttlOfficeSubP08.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P09")).Text = ttlOfficeSubP09.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P10")).Text = ttlOfficeSubP10.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P11")).Text = ttlOfficeSubP11.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_P12")).Text = ttlOfficeSubP12.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_Total")).Text = ttlOfficeSubTotal.ToString("#,##0");

                        ttlOfficeSubP01 = 0;
                        ttlOfficeSubP02 = 0;
                        ttlOfficeSubP03 = 0;
                        ttlOfficeSubP04 = 0;
                        ttlOfficeSubP05 = 0;
                        ttlOfficeSubP06 = 0;
                        ttlOfficeSubP07 = 0;
                        ttlOfficeSubP08 = 0;
                        ttlOfficeSubP09 = 0;
                        ttlOfficeSubP10 = 0;
                        ttlOfficeSubP11 = 0;
                        ttlOfficeSubP12 = 0;
                        ttlOfficeSubTotal = 0;
                    }

                    if (reportEnd)
                    {
                        //tr_Normal.Visible = true;
                        //tr_GroupFooter.Visible = true;
                        tr_Summary.Visible = true;

                        ((HtmlTableCell)e.Item.FindControl("tdSmyGroup")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyCurrency")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP01")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP02")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP03")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP04")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP05")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP06")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP07")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP08")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP09")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP10")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP11")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyP12")).Attributes["class"] = "subTotalAlt";
                        ((HtmlTableCell)e.Item.FindControl("tdSmyTotal")).Attributes["class"] = "subTotalAlt";

                        /*
                        ((Label)e.Item.FindControl("lbl_SmyCurrency")).Text = "Grand Total";
                        ((Label)e.Item.FindControl("lbl_SmyP01")).Text = ttlVendorP01.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP02")).Text = ttlVendorP02.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP03")).Text = ttlVendorP03.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP04")).Text = ttlVendorP04.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP05")).Text = ttlVendorP05.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP06")).Text = ttlVendorP06.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP07")).Text = ttlVendorP07.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP08")).Text = ttlVendorP08.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP09")).Text = ttlVendorP09.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP10")).Text = ttlVendorP10.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP11")).Text = ttlVendorP11.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP12")).Text = ttlVendorP12.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyTotal")).Text = ttlVendorTotal.ToString("#,##0");
                        */
                        ((Label)e.Item.FindControl("lbl_SmyCurrency")).Text = "Grand Total";
                        ((Label)e.Item.FindControl("lbl_SmyP01")).Text = ttlOfficeGrandP01.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP02")).Text = ttlOfficeGrandP02.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP03")).Text = ttlOfficeGrandP03.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP04")).Text = ttlOfficeGrandP04.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP05")).Text = ttlOfficeGrandP05.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP06")).Text = ttlOfficeGrandP06.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP07")).Text = ttlOfficeGrandP07.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP08")).Text = ttlOfficeGrandP08.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP09")).Text = ttlOfficeGrandP09.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP10")).Text = ttlOfficeGrandP10.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP11")).Text = ttlOfficeGrandP11.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyP12")).Text = ttlOfficeGrandP12.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SmyTotal")).Text = ttlOfficeGrandTotal.ToString("#,##0");

                    }
                    #endregion Office SubTotal
                }


            }

        }

        protected void repUKClaim_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HtmlTableRow tr_SubTotal = (HtmlTableRow)e.Item.FindControl("trSubTotal");
            HtmlTableRow tr_Office = (HtmlTableRow)e.Item.FindControl("trOffice");
            HtmlTableRow tr_GroupFooter = (HtmlTableRow)e.Item.FindControl("trGroupFooter");
            HtmlTableRow tr_BlankLine = (HtmlTableRow)e.Item.FindControl("trBlankLine");
            HtmlTableRow tr_SubGroupBlankLine = (HtmlTableRow)e.Item.FindControl("trSubGroupBlankLine");
            OfficeId currentOffice = null;

            if (WebHelper.isRepeaterFooter(e))
            {
                #region Report Footer
                HtmlTableRow tr_Footer = (HtmlTableRow)e.Item.FindControl("trFooter");
                if (isSummary)
                {
                    /*
                    ((HtmlTableCell)e.Item.FindControl("tdSubProductTeam")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubCurrency")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubClaimType")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP01")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP02")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP03")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP04")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP05")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP06")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP07")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP08")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP09")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP10")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP11")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubP12")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubTotal")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdSubShipmentDate")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);

                    ((Label)e.Item.FindControl("lbl_SubProductTeam")).Text = productTeamName;
                    ((Label)e.Item.FindControl("lbl_SubCurrency")).Text = currencyName;
                    ((Label)e.Item.FindControl("lbl_SubP01")).Text = ttlSubP01.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP02")).Text = ttlSubP02.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP03")).Text = ttlSubP03.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP04")).Text = ttlSubP04.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP05")).Text = ttlSubP05.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP06")).Text = ttlSubP06.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP07")).Text = ttlSubP07.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP08")).Text = ttlSubP08.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP09")).Text = ttlSubP09.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP10")).Text = ttlSubP10.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP11")).Text = ttlSubP11.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubP12")).Text = ttlSubP12.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_SubTotal")).Text = ttlSubTotal.ToString("#,##0");
                    if (shipmentDate == DateTime.MinValue)
                        ((Label)e.Item.FindControl("lbl_SubShipmentDate")).Text = "N/A";
                    else
                        ((Label)e.Item.FindControl("lbl_SubShipmentDate")).Text = DateTimeUtility.getDateString(shipmentDate);
                    */
                    tr_Footer.Visible = false;
                }
                else
                    tr_Footer.Visible = false;
                #endregion
            }
            else
            {
                UKClaimPhasingByProductTeamDef def = (UKClaimPhasingByProductTeamDef)vwSearchResult[e.Item.ItemIndex];
                UKClaimPhasingByProductTeamDef nextDef = (e.Item.ItemIndex + 1 < vwSearchResult.Count ? (UKClaimPhasingByProductTeamDef)vwSearchResult[e.Item.ItemIndex + 1] : null);
                ((Label)e.Item.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
                tr_Office.Visible = false;
                tr_GroupFooter.Visible = false;
                tr_BlankLine.Visible = false;
                tr_SubGroupBlankLine.Visible = false;

                currentOffice = OfficeId.getStatus(def.OfficeId);
                if (isFirst)
                {
                    productTeamId = def.ProductTeamId;
                    currencyId = def.CurrencyId;
                }

                if (def.OfficeId != officeId)
                {
                    tr_Office.Visible = true;
                    //tr_SubGroupBlankLine.Visible = (officeId > 0);
                }


                productTeamName = def.Name;
                currencyName = CurrencyId.getName(def.CurrencyId);
                shipmentDate = def.LatestShipmentDate;

                if (count == 0)
                {
                    ((Label)e.Item.FindControl("lbl_ProductTeam")).Text = def.Name;
                    ((Label)e.Item.FindControl("lbl_Currency")).Text = CurrencyId.getName(def.CurrencyId);
                }
                
                HtmlTableRow tr_Normal = (HtmlTableRow)e.Item.FindControl("trNormal");
                tr_Normal.Visible = false;

                if (!isSummary)
                {   // Detail Report - Group Header
                    #region ProductTeam/Currency SubTotal
                    tr_Normal.Visible = true;
                    ((HtmlTableCell)e.Item.FindControl("tdProductTeam")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdCurrency")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdClaimType")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP01")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP02")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP03")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP04")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP05")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP06")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP07")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP08")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP09")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP10")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP11")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdP12")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdTotal")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((HtmlTableCell)e.Item.FindControl("tdShipmentDate")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);

                    ((Label)e.Item.FindControl("lbl_ClaimType")).Text = UKClaimType.getType(def.ClaimTypeId).Name;
                    ((Label)e.Item.FindControl("lbl_P01")).Text = def.P01Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P02")).Text = def.P02Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P03")).Text = def.P03Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P04")).Text = def.P04Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P05")).Text = def.P05Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P06")).Text = def.P06Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P07")).Text = def.P07Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P08")).Text = def.P08Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P09")).Text = def.P09Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P10")).Text = def.P10Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P11")).Text = def.P11Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_P12")).Text = def.P12Amount.ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_Total")).Text = def.TotalAmount.ToString("#,##0");
                    if (def.LatestShipmentDate == DateTime.MinValue)
                        ((Label)e.Item.FindControl("lbl_ShipmentDate")).Text = "N/A";
                    else
                        ((Label)e.Item.FindControl("lbl_ShipmentDate")).Text = DateTimeUtility.getDateString(def.LatestShipmentDate);
                    #endregion ProductTeam/Currency SubTotal
                }

                #region Calulate Total

                #region ProductTeam/Currency & Office SubTotal
                ttlSubP01 += def.P01Amount;
                ttlSubP02 += def.P02Amount;
                ttlSubP03 += def.P03Amount;
                ttlSubP04 += def.P04Amount;
                ttlSubP05 += def.P05Amount;
                ttlSubP06 += def.P06Amount;
                ttlSubP07 += def.P07Amount;
                ttlSubP08 += def.P08Amount;
                ttlSubP09 += def.P09Amount;
                ttlSubP10 += def.P10Amount;
                ttlSubP11 += def.P11Amount;
                ttlSubP12 += def.P12Amount;
                ttlSubTotal += def.TotalAmountInUSD;

                ttlOfficeSubP01 += def.P01AmountInUSD;
                ttlOfficeSubP02 += def.P02AmountInUSD;
                ttlOfficeSubP03 += def.P03AmountInUSD;
                ttlOfficeSubP04 += def.P04AmountInUSD;
                ttlOfficeSubP05 += def.P05AmountInUSD;
                ttlOfficeSubP06 += def.P06AmountInUSD;
                ttlOfficeSubP07 += def.P07AmountInUSD;
                ttlOfficeSubP08 += def.P08AmountInUSD;
                ttlOfficeSubP09 += def.P09AmountInUSD;
                ttlOfficeSubP10 += def.P10AmountInUSD;
                ttlOfficeSubP11 += def.P11AmountInUSD;
                ttlOfficeSubP12 += def.P12AmountInUSD;
                ttlOfficeSubTotal += def.TotalAmountInUSD;
                #endregion SubTotal

                #region Office Grand Total
                ttlOfficeGrandP01 += def.P01AmountInUSD;
                ttlOfficeGrandP02 += def.P02AmountInUSD;
                ttlOfficeGrandP03 += def.P03AmountInUSD;
                ttlOfficeGrandP04 += def.P04AmountInUSD;
                ttlOfficeGrandP05 += def.P05AmountInUSD;
                ttlOfficeGrandP06 += def.P06AmountInUSD;
                ttlOfficeGrandP07 += def.P07AmountInUSD;
                ttlOfficeGrandP08 += def.P08AmountInUSD;
                ttlOfficeGrandP09 += def.P09AmountInUSD;
                ttlOfficeGrandP10 += def.P10AmountInUSD;
                ttlOfficeGrandP11 += def.P11AmountInUSD;
                ttlOfficeGrandP12 += def.P12AmountInUSD;
                ttlOfficeGrandTotal += def.TotalAmountInUSD;
                #endregion

                count += 1;

                #region Calculate claim type total
                if (def.ClaimTypeId == 1)
                {
                    ttlReworkP01 += def.P01AmountInUSD;
                    ttlReworkP02 += def.P02AmountInUSD;
                    ttlReworkP03 += def.P03AmountInUSD;
                    ttlReworkP04 += def.P04AmountInUSD;
                    ttlReworkP05 += def.P05AmountInUSD;
                    ttlReworkP06 += def.P06AmountInUSD;
                    ttlReworkP07 += def.P07AmountInUSD;
                    ttlReworkP08 += def.P08AmountInUSD;
                    ttlReworkP09 += def.P09AmountInUSD;
                    ttlReworkP10 += def.P10AmountInUSD;
                    ttlReworkP11 += def.P11AmountInUSD;
                    ttlReworkP12 += def.P12AmountInUSD;
                    ttlReworkTotal += def.TotalAmountInUSD;

                    ttlLYReworkP01 += def.LYP01AmountInUSD;
                    ttlLYReworkP02 += def.LYP02AmountInUSD;
                    ttlLYReworkP03 += def.LYP03AmountInUSD;
                    ttlLYReworkP04 += def.LYP04AmountInUSD;
                    ttlLYReworkP05 += def.LYP05AmountInUSD;
                    ttlLYReworkP06 += def.LYP06AmountInUSD;
                    ttlLYReworkP07 += def.LYP07AmountInUSD;
                    ttlLYReworkP08 += def.LYP08AmountInUSD;
                    ttlLYReworkP09 += def.LYP09AmountInUSD;
                    ttlLYReworkP10 += def.LYP10AmountInUSD;
                    ttlLYReworkP11 += def.LYP11AmountInUSD;
                    ttlLYReworkP12 += def.LYP12AmountInUSD;
                    ttlLYReworkTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 2)
                {
                    ttlRejectP01 += def.P01AmountInUSD;
                    ttlRejectP02 += def.P02AmountInUSD;
                    ttlRejectP03 += def.P03AmountInUSD;
                    ttlRejectP04 += def.P04AmountInUSD;
                    ttlRejectP05 += def.P05AmountInUSD;
                    ttlRejectP06 += def.P06AmountInUSD;
                    ttlRejectP07 += def.P07AmountInUSD;
                    ttlRejectP08 += def.P08AmountInUSD;
                    ttlRejectP09 += def.P09AmountInUSD;
                    ttlRejectP10 += def.P10AmountInUSD;
                    ttlRejectP11 += def.P11AmountInUSD;
                    ttlRejectP12 += def.P12AmountInUSD;
                    ttlRejectTotal += def.TotalAmountInUSD;

                    ttlLYRejectP01 += def.LYP01AmountInUSD;
                    ttlLYRejectP02 += def.LYP02AmountInUSD;
                    ttlLYRejectP03 += def.LYP03AmountInUSD;
                    ttlLYRejectP04 += def.LYP04AmountInUSD;
                    ttlLYRejectP05 += def.LYP05AmountInUSD;
                    ttlLYRejectP06 += def.LYP06AmountInUSD;
                    ttlLYRejectP07 += def.LYP07AmountInUSD;
                    ttlLYRejectP08 += def.LYP08AmountInUSD;
                    ttlLYRejectP09 += def.LYP09AmountInUSD;
                    ttlLYRejectP10 += def.LYP10AmountInUSD;
                    ttlLYRejectP11 += def.LYP11AmountInUSD;
                    ttlLYRejectP12 += def.LYP12AmountInUSD;
                    ttlLYRejectTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 3)
                {
                    ttlMFRNP01 += def.P01AmountInUSD;
                    ttlMFRNP02 += def.P02AmountInUSD;
                    ttlMFRNP03 += def.P03AmountInUSD;
                    ttlMFRNP04 += def.P04AmountInUSD;
                    ttlMFRNP05 += def.P05AmountInUSD;
                    ttlMFRNP06 += def.P06AmountInUSD;
                    ttlMFRNP07 += def.P07AmountInUSD;
                    ttlMFRNP08 += def.P08AmountInUSD;
                    ttlMFRNP09 += def.P09AmountInUSD;
                    ttlMFRNP10 += def.P10AmountInUSD;
                    ttlMFRNP11 += def.P11AmountInUSD;
                    ttlMFRNP12 += def.P12AmountInUSD;
                    ttlMFRNTotal += def.TotalAmountInUSD;

                    ttlLYMFRNP01 += def.LYP01AmountInUSD;
                    ttlLYMFRNP02 += def.LYP02AmountInUSD;
                    ttlLYMFRNP03 += def.LYP03AmountInUSD;
                    ttlLYMFRNP04 += def.LYP04AmountInUSD;
                    ttlLYMFRNP05 += def.LYP05AmountInUSD;
                    ttlLYMFRNP06 += def.LYP06AmountInUSD;
                    ttlLYMFRNP07 += def.LYP07AmountInUSD;
                    ttlLYMFRNP08 += def.LYP08AmountInUSD;
                    ttlLYMFRNP09 += def.LYP09AmountInUSD;
                    ttlLYMFRNP10 += def.LYP10AmountInUSD;
                    ttlLYMFRNP11 += def.LYP11AmountInUSD;
                    ttlLYMFRNP12 += def.LYP12AmountInUSD;
                    ttlLYMFRNTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 4)
                {
                    ttlCFSP01 += def.P01AmountInUSD;
                    ttlCFSP02 += def.P02AmountInUSD;
                    ttlCFSP03 += def.P03AmountInUSD;
                    ttlCFSP04 += def.P04AmountInUSD;
                    ttlCFSP05 += def.P05AmountInUSD;
                    ttlCFSP06 += def.P06AmountInUSD;
                    ttlCFSP07 += def.P07AmountInUSD;
                    ttlCFSP08 += def.P08AmountInUSD;
                    ttlCFSP09 += def.P09AmountInUSD;
                    ttlCFSP10 += def.P10AmountInUSD;
                    ttlCFSP11 += def.P11AmountInUSD;
                    ttlCFSP12 += def.P12AmountInUSD;
                    ttlCFSTotal += def.TotalAmountInUSD;

                    ttlLYCFSP01 += def.LYP01AmountInUSD;
                    ttlLYCFSP02 += def.LYP02AmountInUSD;
                    ttlLYCFSP03 += def.LYP03AmountInUSD;
                    ttlLYCFSP04 += def.LYP04AmountInUSD;
                    ttlLYCFSP05 += def.LYP05AmountInUSD;
                    ttlLYCFSP06 += def.LYP06AmountInUSD;
                    ttlLYCFSP07 += def.LYP07AmountInUSD;
                    ttlLYCFSP08 += def.LYP08AmountInUSD;
                    ttlLYCFSP09 += def.LYP09AmountInUSD;
                    ttlLYCFSP10 += def.LYP10AmountInUSD;
                    ttlLYCFSP11 += def.LYP11AmountInUSD;
                    ttlLYCFSP12 += def.LYP12AmountInUSD;
                    ttlLYCFSTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 5)
                {
                    ttlSafetyP01 += def.P01AmountInUSD;
                    ttlSafetyP02 += def.P02AmountInUSD;
                    ttlSafetyP03 += def.P03AmountInUSD;
                    ttlSafetyP04 += def.P04AmountInUSD;
                    ttlSafetyP05 += def.P05AmountInUSD;
                    ttlSafetyP06 += def.P06AmountInUSD;
                    ttlSafetyP07 += def.P07AmountInUSD;
                    ttlSafetyP08 += def.P08AmountInUSD;
                    ttlSafetyP09 += def.P09AmountInUSD;
                    ttlSafetyP10 += def.P10AmountInUSD;
                    ttlSafetyP11 += def.P11AmountInUSD;
                    ttlSafetyP12 += def.P12AmountInUSD;
                    ttlSafetyTotal += def.TotalAmountInUSD;

                    ttlLYSafetyP01 += def.LYP01AmountInUSD;
                    ttlLYSafetyP02 += def.LYP02AmountInUSD;
                    ttlLYSafetyP03 += def.LYP03AmountInUSD;
                    ttlLYSafetyP04 += def.LYP04AmountInUSD;
                    ttlLYSafetyP05 += def.LYP05AmountInUSD;
                    ttlLYSafetyP06 += def.LYP06AmountInUSD;
                    ttlLYSafetyP07 += def.LYP07AmountInUSD;
                    ttlLYSafetyP08 += def.LYP08AmountInUSD;
                    ttlLYSafetyP09 += def.LYP09AmountInUSD;
                    ttlLYSafetyP10 += def.LYP10AmountInUSD;
                    ttlLYSafetyP11 += def.LYP11AmountInUSD;
                    ttlLYSafetyP12 += def.LYP12AmountInUSD;
                    ttlLYSafetyTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 6)
                {
                    ttlAuditP01 += def.P01AmountInUSD;
                    ttlAuditP02 += def.P02AmountInUSD;
                    ttlAuditP03 += def.P03AmountInUSD;
                    ttlAuditP04 += def.P04AmountInUSD;
                    ttlAuditP05 += def.P05AmountInUSD;
                    ttlAuditP06 += def.P06AmountInUSD;
                    ttlAuditP07 += def.P07AmountInUSD;
                    ttlAuditP08 += def.P08AmountInUSD;
                    ttlAuditP09 += def.P09AmountInUSD;
                    ttlAuditP10 += def.P10AmountInUSD;
                    ttlAuditP11 += def.P11AmountInUSD;
                    ttlAuditP12 += def.P12AmountInUSD;
                    ttlAuditTotal += def.TotalAmountInUSD;

                    ttlLYAuditP01 += def.LYP01AmountInUSD;
                    ttlLYAuditP02 += def.LYP02AmountInUSD;
                    ttlLYAuditP03 += def.LYP03AmountInUSD;
                    ttlLYAuditP04 += def.LYP04AmountInUSD;
                    ttlLYAuditP05 += def.LYP05AmountInUSD;
                    ttlLYAuditP06 += def.LYP06AmountInUSD;
                    ttlLYAuditP07 += def.LYP07AmountInUSD;
                    ttlLYAuditP08 += def.LYP08AmountInUSD;
                    ttlLYAuditP09 += def.LYP09AmountInUSD;
                    ttlLYAuditP10 += def.LYP10AmountInUSD;
                    ttlLYAuditP11 += def.LYP11AmountInUSD;
                    ttlLYAuditP12 += def.LYP12AmountInUSD;
                    ttlLYAuditTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 7)
                {
                    ttlFabricP01 += def.P01AmountInUSD;
                    ttlFabricP02 += def.P02AmountInUSD;
                    ttlFabricP03 += def.P03AmountInUSD;
                    ttlFabricP04 += def.P04AmountInUSD;
                    ttlFabricP05 += def.P05AmountInUSD;
                    ttlFabricP06 += def.P06AmountInUSD;
                    ttlFabricP07 += def.P07AmountInUSD;
                    ttlFabricP08 += def.P08AmountInUSD;
                    ttlFabricP09 += def.P09AmountInUSD;
                    ttlFabricP10 += def.P10AmountInUSD;
                    ttlFabricP11 += def.P11AmountInUSD;
                    ttlFabricP12 += def.P12AmountInUSD;
                    ttlFabricTotal += def.TotalAmountInUSD;

                    ttlLYFabricP01 += def.LYP01AmountInUSD;
                    ttlLYFabricP02 += def.LYP02AmountInUSD;
                    ttlLYFabricP03 += def.LYP03AmountInUSD;
                    ttlLYFabricP04 += def.LYP04AmountInUSD;
                    ttlLYFabricP05 += def.LYP05AmountInUSD;
                    ttlLYFabricP06 += def.LYP06AmountInUSD;
                    ttlLYFabricP07 += def.LYP07AmountInUSD;
                    ttlLYFabricP08 += def.LYP08AmountInUSD;
                    ttlLYFabricP09 += def.LYP09AmountInUSD;
                    ttlLYFabricP10 += def.LYP10AmountInUSD;
                    ttlLYFabricP11 += def.LYP11AmountInUSD;
                    ttlLYFabricP12 += def.LYP12AmountInUSD;
                    ttlLYFabricTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 8)
                {
                    ttlPenaltyP01 += def.P01AmountInUSD;
                    ttlPenaltyP02 += def.P02AmountInUSD;
                    ttlPenaltyP03 += def.P03AmountInUSD;
                    ttlPenaltyP04 += def.P04AmountInUSD;
                    ttlPenaltyP05 += def.P05AmountInUSD;
                    ttlPenaltyP06 += def.P06AmountInUSD;
                    ttlPenaltyP07 += def.P07AmountInUSD;
                    ttlPenaltyP08 += def.P08AmountInUSD;
                    ttlPenaltyP09 += def.P09AmountInUSD;
                    ttlPenaltyP10 += def.P10AmountInUSD;
                    ttlPenaltyP11 += def.P11AmountInUSD;
                    ttlPenaltyP12 += def.P12AmountInUSD;
                    ttlPenaltyTotal += def.TotalAmountInUSD;

                    ttlLYPenaltyP01 += def.LYP01AmountInUSD;
                    ttlLYPenaltyP02 += def.LYP02AmountInUSD;
                    ttlLYPenaltyP03 += def.LYP03AmountInUSD;
                    ttlLYPenaltyP04 += def.LYP04AmountInUSD;
                    ttlLYPenaltyP05 += def.LYP05AmountInUSD;
                    ttlLYPenaltyP06 += def.LYP06AmountInUSD;
                    ttlLYPenaltyP07 += def.LYP07AmountInUSD;
                    ttlLYPenaltyP08 += def.LYP08AmountInUSD;
                    ttlLYPenaltyP09 += def.LYP09AmountInUSD;
                    ttlLYPenaltyP10 += def.LYP10AmountInUSD;
                    ttlLYPenaltyP11 += def.LYP11AmountInUSD;
                    ttlLYPenaltyP12 += def.LYP12AmountInUSD;
                    ttlLYPenaltyTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 10)
                {
                    ttlQCCP01 += def.P01AmountInUSD;
                    ttlQCCP02 += def.P02AmountInUSD;
                    ttlQCCP03 += def.P03AmountInUSD;
                    ttlQCCP04 += def.P04AmountInUSD;
                    ttlQCCP05 += def.P05AmountInUSD;
                    ttlQCCP06 += def.P06AmountInUSD;
                    ttlQCCP07 += def.P07AmountInUSD;
                    ttlQCCP08 += def.P08AmountInUSD;
                    ttlQCCP09 += def.P09AmountInUSD;
                    ttlQCCP10 += def.P10AmountInUSD;
                    ttlQCCP11 += def.P11AmountInUSD;
                    ttlQCCP12 += def.P12AmountInUSD;
                    ttlQCCTotal += def.TotalAmountInUSD;

                    ttlLYQCCP01 += def.LYP01AmountInUSD;
                    ttlLYQCCP02 += def.LYP02AmountInUSD;
                    ttlLYQCCP03 += def.LYP03AmountInUSD;
                    ttlLYQCCP04 += def.LYP04AmountInUSD;
                    ttlLYQCCP05 += def.LYP05AmountInUSD;
                    ttlLYQCCP06 += def.LYP06AmountInUSD;
                    ttlLYQCCP07 += def.LYP07AmountInUSD;
                    ttlLYQCCP08 += def.LYP08AmountInUSD;
                    ttlLYQCCP09 += def.LYP09AmountInUSD;
                    ttlLYQCCP10 += def.LYP10AmountInUSD;
                    ttlLYQCCP11 += def.LYP11AmountInUSD;
                    ttlLYQCCP12 += def.LYP12AmountInUSD;
                    ttlLYQCCTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 11)
                {
                    ttlCHBP01 += def.P01AmountInUSD;
                    ttlCHBP02 += def.P02AmountInUSD;
                    ttlCHBP03 += def.P03AmountInUSD;
                    ttlCHBP04 += def.P04AmountInUSD;
                    ttlCHBP05 += def.P05AmountInUSD;
                    ttlCHBP06 += def.P06AmountInUSD;
                    ttlCHBP07 += def.P07AmountInUSD;
                    ttlCHBP08 += def.P08AmountInUSD;
                    ttlCHBP09 += def.P09AmountInUSD;
                    ttlCHBP10 += def.P10AmountInUSD;
                    ttlCHBP11 += def.P11AmountInUSD;
                    ttlCHBP12 += def.P12AmountInUSD;
                    ttlCHBTotal += def.TotalAmountInUSD;

                    ttlLYCHBP01 += def.LYP01AmountInUSD;
                    ttlLYCHBP02 += def.LYP02AmountInUSD;
                    ttlLYCHBP03 += def.LYP03AmountInUSD;
                    ttlLYCHBP04 += def.LYP04AmountInUSD;
                    ttlLYCHBP05 += def.LYP05AmountInUSD;
                    ttlLYCHBP06 += def.LYP06AmountInUSD;
                    ttlLYCHBP07 += def.LYP07AmountInUSD;
                    ttlLYCHBP08 += def.LYP08AmountInUSD;
                    ttlLYCHBP09 += def.LYP09AmountInUSD;
                    ttlLYCHBP10 += def.LYP10AmountInUSD;
                    ttlLYCHBP11 += def.LYP11AmountInUSD;
                    ttlLYCHBP12 += def.LYP12AmountInUSD;
                    ttlLYCHBTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 12)
                {
                    ttlGBTestP01 += def.P01AmountInUSD;
                    ttlGBTestP02 += def.P02AmountInUSD;
                    ttlGBTestP03 += def.P03AmountInUSD;
                    ttlGBTestP04 += def.P04AmountInUSD;
                    ttlGBTestP05 += def.P05AmountInUSD;
                    ttlGBTestP06 += def.P06AmountInUSD;
                    ttlGBTestP07 += def.P07AmountInUSD;
                    ttlGBTestP08 += def.P08AmountInUSD;
                    ttlGBTestP09 += def.P09AmountInUSD;
                    ttlGBTestP10 += def.P10AmountInUSD;
                    ttlGBTestP11 += def.P11AmountInUSD;
                    ttlGBTestP12 += def.P12AmountInUSD;
                    ttlGBTestTotal += def.TotalAmountInUSD;

                    ttlLYGBTestP01 += def.LYP01AmountInUSD;
                    ttlLYGBTestP02 += def.LYP02AmountInUSD;
                    ttlLYGBTestP03 += def.LYP03AmountInUSD;
                    ttlLYGBTestP04 += def.LYP04AmountInUSD;
                    ttlLYGBTestP05 += def.LYP05AmountInUSD;
                    ttlLYGBTestP06 += def.LYP06AmountInUSD;
                    ttlLYGBTestP07 += def.LYP07AmountInUSD;
                    ttlLYGBTestP08 += def.LYP08AmountInUSD;
                    ttlLYGBTestP09 += def.LYP09AmountInUSD;
                    ttlLYGBTestP10 += def.LYP10AmountInUSD;
                    ttlLYGBTestP11 += def.LYP11AmountInUSD;
                    ttlLYGBTestP12 += def.LYP12AmountInUSD;
                    ttlLYGBTestTotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 13)
                {
                    ttlFIRAP01 += def.P01AmountInUSD;
                    ttlFIRAP02 += def.P02AmountInUSD;
                    ttlFIRAP03 += def.P03AmountInUSD;
                    ttlFIRAP04 += def.P04AmountInUSD;
                    ttlFIRAP05 += def.P05AmountInUSD;
                    ttlFIRAP06 += def.P06AmountInUSD;
                    ttlFIRAP07 += def.P07AmountInUSD;
                    ttlFIRAP08 += def.P08AmountInUSD;
                    ttlFIRAP09 += def.P09AmountInUSD;
                    ttlFIRAP10 += def.P10AmountInUSD;
                    ttlFIRAP11 += def.P11AmountInUSD;
                    ttlFIRAP12 += def.P12AmountInUSD;
                    ttlFIRATotal += def.TotalAmountInUSD;

                    ttlLYFIRAP01 += def.LYP01AmountInUSD;
                    ttlLYFIRAP02 += def.LYP02AmountInUSD;
                    ttlLYFIRAP03 += def.LYP03AmountInUSD;
                    ttlLYFIRAP04 += def.LYP04AmountInUSD;
                    ttlLYFIRAP05 += def.LYP05AmountInUSD;
                    ttlLYFIRAP06 += def.LYP06AmountInUSD;
                    ttlLYFIRAP07 += def.LYP07AmountInUSD;
                    ttlLYFIRAP08 += def.LYP08AmountInUSD;
                    ttlLYFIRAP09 += def.LYP09AmountInUSD;
                    ttlLYFIRAP10 += def.LYP10AmountInUSD;
                    ttlLYFIRAP11 += def.LYP11AmountInUSD;
                    ttlLYFIRAP12 += def.LYP12AmountInUSD;
                    ttlLYFIRATotal += def.LYTotalAmountInUSD;
                }

                if (def.ClaimTypeId == 14)
                {
                    ttlOthersP01 += def.P01AmountInUSD;
                    ttlOthersP02 += def.P02AmountInUSD;
                    ttlOthersP03 += def.P03AmountInUSD;
                    ttlOthersP04 += def.P04AmountInUSD;
                    ttlOthersP05 += def.P05AmountInUSD;
                    ttlOthersP06 += def.P06AmountInUSD;
                    ttlOthersP07 += def.P07AmountInUSD;
                    ttlOthersP08 += def.P08AmountInUSD;
                    ttlOthersP09 += def.P09AmountInUSD;
                    ttlOthersP10 += def.P10AmountInUSD;
                    ttlOthersP11 += def.P11AmountInUSD;
                    ttlOthersP12 += def.P12AmountInUSD;
                    ttlOthersTotal += def.TotalAmountInUSD;

                    ttlLYOthersP01 += def.LYP01AmountInUSD;
                    ttlLYOthersP02 += def.LYP02AmountInUSD;
                    ttlLYOthersP03 += def.LYP03AmountInUSD;
                    ttlLYOthersP04 += def.LYP04AmountInUSD;
                    ttlLYOthersP05 += def.LYP05AmountInUSD;
                    ttlLYOthersP06 += def.LYP06AmountInUSD;
                    ttlLYOthersP07 += def.LYP07AmountInUSD;
                    ttlLYOthersP08 += def.LYP08AmountInUSD;
                    ttlLYOthersP09 += def.LYP09AmountInUSD;
                    ttlLYOthersP10 += def.LYP10AmountInUSD;
                    ttlLYOthersP11 += def.LYP11AmountInUSD;
                    ttlLYOthersP12 += def.LYP12AmountInUSD;
                    ttlLYOthersTotal += def.LYTotalAmountInUSD;
                }


                #endregion Claim Type Total

                #region Calculate Yearly Summary

                ttlNSP01 += def.P01NSAmountInUSD;
                ttlNSP02 += def.P02NSAmountInUSD;
                ttlNSP03 += def.P03NSAmountInUSD;
                ttlNSP04 += def.P04NSAmountInUSD;
                ttlNSP05 += def.P05NSAmountInUSD;
                ttlNSP06 += def.P06NSAmountInUSD;
                ttlNSP07 += def.P07NSAmountInUSD;
                ttlNSP08 += def.P08NSAmountInUSD;
                ttlNSP09 += def.P09NSAmountInUSD;
                ttlNSP10 += def.P10NSAmountInUSD;
                ttlNSP11 += def.P11NSAmountInUSD;
                ttlNSP12 += def.P12NSAmountInUSD;
                ttlNSTotal += def.TotalNSAmountInUSD;

                ttlVendorP01 += def.P01VendorAmountInUSD;
                ttlVendorP02 += def.P02VendorAmountInUSD;
                ttlVendorP03 += def.P03VendorAmountInUSD;
                ttlVendorP04 += def.P04VendorAmountInUSD;
                ttlVendorP05 += def.P05VendorAmountInUSD;
                ttlVendorP06 += def.P06VendorAmountInUSD;
                ttlVendorP07 += def.P07VendorAmountInUSD;
                ttlVendorP08 += def.P08VendorAmountInUSD;
                ttlVendorP09 += def.P09VendorAmountInUSD;
                ttlVendorP10 += def.P10VendorAmountInUSD;
                ttlVendorP11 += def.P11VendorAmountInUSD;
                ttlVendorP12 += def.P12VendorAmountInUSD;
                ttlVendorTotal += def.TotalVendorAmountInUSD;

                ttlLYNSP01 += def.LYP01NSAmountInUSD;
                ttlLYNSP02 += def.LYP02NSAmountInUSD;
                ttlLYNSP03 += def.LYP03NSAmountInUSD;
                ttlLYNSP04 += def.LYP04NSAmountInUSD;
                ttlLYNSP05 += def.LYP05NSAmountInUSD;
                ttlLYNSP06 += def.LYP06NSAmountInUSD;
                ttlLYNSP07 += def.LYP07NSAmountInUSD;
                ttlLYNSP08 += def.LYP08NSAmountInUSD;
                ttlLYNSP09 += def.LYP09NSAmountInUSD;
                ttlLYNSP10 += def.LYP10NSAmountInUSD;
                ttlLYNSP11 += def.LYP11NSAmountInUSD;
                ttlLYNSP12 += def.LYP12NSAmountInUSD;
                ttlLYNSTotal += def.LYTotalNSAmountInUSD;

                ttlLYVendorP01 += def.LYP01VendorAmountInUSD;
                ttlLYVendorP02 += def.LYP02VendorAmountInUSD;
                ttlLYVendorP03 += def.LYP03VendorAmountInUSD;
                ttlLYVendorP04 += def.LYP04VendorAmountInUSD;
                ttlLYVendorP05 += def.LYP05VendorAmountInUSD;
                ttlLYVendorP06 += def.LYP06VendorAmountInUSD;
                ttlLYVendorP07 += def.LYP07VendorAmountInUSD;
                ttlLYVendorP08 += def.LYP08VendorAmountInUSD;
                ttlLYVendorP09 += def.LYP09VendorAmountInUSD;
                ttlLYVendorP10 += def.LYP10VendorAmountInUSD;
                ttlLYVendorP11 += def.LYP11VendorAmountInUSD;
                ttlLYVendorP12 += def.LYP12VendorAmountInUSD;
                ttlLYVendorTotal += def.LYTotalVendorAmountInUSD;
                #endregion Yearly Summary

                #endregion Calculate Total

                tr_SubTotal.Visible = false;

                bool subGroupEnd = true;
                if (nextDef != null)
                    subGroupEnd = (def.ProductTeamId != nextDef.ProductTeamId || def.CurrencyId != nextDef.CurrencyId);
                if (subGroupEnd)
                //if ((def.ProductTeamId != productTeamId || def.CurrencyId != currencyId))
                {
                    #region ProductTeam/Currency SubTotal
                    if (isSummary)
                    {
                        tr_SubTotal.Visible = true;

                        ((HtmlTableCell)e.Item.FindControl("tdSubProductTeam")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubCurrency")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubClaimType")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP01")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP02")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP03")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP04")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP05")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP06")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP07")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP08")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP09")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP10")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP11")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubP12")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubTotal")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdSubShipmentDate")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);

                        ((Label)e.Item.FindControl("lbl_SubProductTeam")).Text = productTeamName;
                        ((Label)e.Item.FindControl("lbl_SubCurrency")).Text = currencyName;
                        ((Label)e.Item.FindControl("lbl_SubP01")).Text = ttlSubP01.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP02")).Text = ttlSubP02.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP03")).Text = ttlSubP03.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP04")).Text = ttlSubP04.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP05")).Text = ttlSubP05.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP06")).Text = ttlSubP06.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP07")).Text = ttlSubP07.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP08")).Text = ttlSubP08.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP09")).Text = ttlSubP09.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP10")).Text = ttlSubP10.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP11")).Text = ttlSubP11.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubP12")).Text = ttlSubP12.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_SubTotal")).Text = ttlSubTotal.ToString("#,##0");
                        if (shipmentDate == DateTime.MinValue)
                            ((Label)e.Item.FindControl("lbl_SubShipmentDate")).Text = "N/A";
                        else
                            ((Label)e.Item.FindControl("lbl_SubShipmentDate")).Text = DateTimeUtility.getDateString(shipmentDate);
                    }


                    isAlternate = !isAlternate;
                    ttlSubP01 = 0;
                    ttlSubP02 = 0;
                    ttlSubP03 = 0;
                    ttlSubP04 = 0;
                    ttlSubP05 = 0;
                    ttlSubP06 = 0;
                    ttlSubP07 = 0;
                    ttlSubP08 = 0;
                    ttlSubP09 = 0;
                    ttlSubP10 = 0;
                    ttlSubP11 = 0;
                    ttlSubP12 = 0;
                    ttlSubTotal = 0;
                    count = 0;
                    #endregion 
                }

                //if (!isSummary)
                {   // Detail Report - Group Footer (After calculating total amount)
                     #region Office Group Total
                    bool groupEnd = true;
                    if (nextDef != null)
                        groupEnd = (def.OfficeId != nextDef.OfficeId);
                    if (groupEnd)
                    {
                        tr_BlankLine.Visible = true;
                        tr_GroupFooter.Visible = true;
                        ((Label)e.Item.FindControl("lbl_GfDesc")).Text = OfficeId.getName(def.OfficeId).ToString() + " Subtotal (USD):";
                        ((Label)e.Item.FindControl("lbl_GfP01")).Text = ttlOfficeSubP01.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP02")).Text = ttlOfficeSubP02.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP03")).Text = ttlOfficeSubP03.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP04")).Text = ttlOfficeSubP04.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP05")).Text = ttlOfficeSubP05.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP06")).Text = ttlOfficeSubP06.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP07")).Text = ttlOfficeSubP07.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP08")).Text = ttlOfficeSubP08.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP09")).Text = ttlOfficeSubP09.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP10")).Text = ttlOfficeSubP10.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP11")).Text = ttlOfficeSubP11.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfP12")).Text = ttlOfficeSubP12.ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_GfTotal")).Text = ttlOfficeSubTotal.ToString("#,##0");
                        //if (shipmentDate == DateTime.MinValue)
                        //    ((Label)e.Item.FindControl("lbl_GfShipmentDate")).Text = "N/A";
                        //else
                        //    ((Label)e.Item.FindControl("lbl_GfShipmentDate")).Text = DateTimeUtility.getDateString(shipmentDate);

                        ttlOfficeSubP01 = 0;
                        ttlOfficeSubP02 = 0;
                        ttlOfficeSubP03 = 0;
                        ttlOfficeSubP04 = 0;
                        ttlOfficeSubP05 = 0;
                        ttlOfficeSubP06 = 0;
                        ttlOfficeSubP07 = 0;
                        ttlOfficeSubP08 = 0;
                        ttlOfficeSubP09 = 0;
                        ttlOfficeSubP10 = 0;
                        ttlOfficeSubP11 = 0;
                        ttlOfficeSubP12 = 0;
                        ttlOfficeSubTotal = 0;
                    }
                    #endregion Office SubTotal

                }


                if (isSingleVendor)
                {
                    //((Label)e.Item.FindControl("lbl_CurrentYearItem")).Text = this.lbl_Supplier.Text;
                    HtmlTableRow tr_GroupHeading = (HtmlTableRow)e.Item.FindControl("trGroupHeading");
                    HtmlTableRow tr_Summary = (HtmlTableRow)e.Item.FindControl("trSummary");
                    HtmlTableRow tr_BlankHeading = (HtmlTableRow)e.Item.FindControl("trBlankHeading");
                    //HtmlTableRow tr_Normal = (HtmlTableRow)e.Item.FindControl("trNormal");
                    HtmlTableRow tr_CurrentYearHeading = (HtmlTableRow)e.Item.FindControl("trCurrentYearHeading");
                    tr_SubTotal.Visible = false;
                    tr_Normal.Visible = false;
                    tr_Office.Visible = false;
                    tr_GroupFooter.Visible = false;
                    tr_BlankLine.Visible = false;
                    //tr_Footer.Visible = false;

                    
                }



                productTeamId = def.ProductTeamId;
                currencyId = def.CurrencyId;
                officeId = def.OfficeId;
                isFirst = false;

            }

        }

    }
}