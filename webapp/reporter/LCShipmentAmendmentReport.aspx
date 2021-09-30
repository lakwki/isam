<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="LCShipmentAmendmentReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.LCShipmentAmendmentReport" Title="L/C Shipment Amendment Report"%>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel2" runat="server" SkinID="sectionHeader_Report">L/C Shipment Amendment Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
/*
    function copyLCBatchNo() {
        document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value;
    }

    function copyLCNo() {
        document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoFrom").value;
    }


    function isFormValid() {
        if (
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoTo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCApplicationNoFrom_txt_LCApplicationNoFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCApplicationNoTo_txt_LCApplicationNoTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateFrom_txt_LCIssueDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateTo_txt_LCIssueDateTo_textbox").value == ""
            ) {
            alert("Please enter search criteria on one of below search criteria.\r\n"
                + " - LC No.\r\n"
                + " - LC Batch No.\r\n"
                + " - LC Application No.\r\n"
                + " - LC Issue Date.\r\n"
                );
        }

        if (((document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value != "")
                 && !isBatchNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value))
               || ((document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value != "")
                 && !isBatchNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value))) {
            alert("Invalid LC Batch no.");
            return false;
        }

        if (((document.getElementById("ctl00_ContentPlaceHolder1_txt_LCApplicationNoFrom").value != "")
                 && !isApplicationNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCApplicationNoFrom").value))
               || ((document.getElementById("ctl00_ContentPlaceHolder1_txt_LCApplicationNoTo").value != "")
                 && !isApplicationNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCApplicationNoTo").value))) {
            alert("Invalid LC Application no.");
            return false;
        }


        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateFrom_txt_LCIssueDateFrom_textbox").value == ""
                && document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateTo_txt_LCIssueDateTo_textbox").value != "")
            || (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateFrom_txt_LCIssueDateFrom_textbox").value != ""
                && document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateTo_txt_LCIssueDateTo_textbox").value == "")
           ) {
            alert("Please enter date range in pair.");
            return false;
        }

        return true;
    }

    function isBatchNoValid(lcBatchNo) {
        var no = -1;
        var batchNo = lcBatchNo.trim().toUpperCase();
        no = batchNo.replace("LCB", "");
        if (lcBatchNo.trim() == "" || ((parseInt(no) > 0 && parseInt(no) <= 999999)))
            return true;
        else
            return false;

    }

    function isApplicationNoValid(appNo) {
        var no = appNo.trim().toUpperCase();
        if (appNo.trim() == "" || ((parseInt(no) > 0 && parseInt(no) <= 999999)))
            return true;
        else
            return false;
    }
 */

    function isFormValid() {
        return true;
    }

</script>

<div>
<table >
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC No.</td>
        <td>
            <asp:TextBox ID="txt_LCNoFrom" runat='server' value='' /> 
            &nbsp;To&nbsp;
            <asp:TextBox ID="txt_LCNoTo" runat="server" value='' />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC Batch No.</td>
        <td>
            <asp:TextBox ID="txt_LCBatchNoFrom" runat='server' value=''  /> 
            &nbsp;To&nbsp;
            <asp:TextBox ID="txt_LCBatchNoTo" runat="server" value=''/>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC Application No.</td>
        <td>
            <asp:TextBox ID="txt_LCApplicationNoFrom" runat='server' value=''  /> 
            &nbsp;To&nbsp;
            <asp:TextBox ID="txt_LCApplicationNoTo" runat="server" value=''/>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">LC Issue Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_LCIssueDateFrom" runat="server" FromDateControl="txt_LCIssueDateFrom" ToDateControl="txt_LCIssueDateTo" />
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_LCIssueDateTo" runat="server" FromDateControl="txt_LCIssueDateFrom" ToDateControl="txt_LCIssueDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:135px;">PO At-Warehouse Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_SupplierAwhDateFrom" runat="server" FromDateControl="txt_SupplierAwhDateFrom" ToDateControl="txt_SupplierAwhDateTo" />
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_SupplierAwhDateTo" runat="server" FromDateControl="txt_SupplierAwhDateTo" ToDateControl="txt_SupplierAwhDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Office Group</td>
        <td><cc2:SmartDropDownList  ID="ddl_OfficeGroup" runat="server" width="70px" AutoPostBack="True" onSelectedIndexChanged='ddl_OfficeGroup_SelectedIndexChanged'/></td>
    </tr>
    <tr id="tr_HandlingOffice" runat="server" style="display:none;">
        <td class="FieldLabel4">Handling Office</td>
        <td><cc2:SmartDropDownList  ID="ddl_HandlingOffice" runat="server" width="70px" AutoPostBack="false" /></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Preview" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Preview_Click" OnClientClick="return isFormValid();" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
         </td>
    </tr>

</table>

    </div>


</asp:Content>