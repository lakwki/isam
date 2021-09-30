<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="LCApplicationSummaryReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.LCApplicationSummaryReport" Title='L/C Application Summary Report'  Theme="DefaultTheme" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<!--
<img src="../images/banner_reports_lcapplication_summary.gif" runat="server" id="imgHeaderText" />
-->
<!--
<table width="100%" cellpadding=0 cellspacing=0>
<tr>
    <td class='tableHeader' style='font-size:135%;color:#ff9900;font-stretch:narrower;height:25px;' width='70px' >&nbsp;&nbsp;Report&nbsp;&nbsp;</td>
    <td class="tableHeader" style=' font-size:80%;color:Black;font-weight:bold;' >L/C Application Summary Report</td>
</tr>
</table>
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">L/C Application Summary Report</asp:Panel>


<script type="text/javascript">

    function isFormValid() {

        if (
               
               (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppDateFrom_txt_LCAppDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppDateTo_txt_LCAppDateTo_textbox").value == "")
            && (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppNoFrom").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppNoTo").value == "")
            && (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoFrom").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoTo").value == "")
            )
            {
            alert("Please enter search criteria on one of below search criteria : "
                + "\r\n - L/C Application Date Period ; "
                + "\r\n - L/C Application Number Range ; "
                + "\r\n - L/C Number Range  "
                + "\r\n"
                );
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_Customer') == 0) {
            alert("Please select at least one customer.");
            return false;
        }

        if (!isDate(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppDateFrom_txt_LCAppDateFrom_textbox"))) return false;
        if (!isDate(document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppDateTo_txt_LCAppDateTo_textbox"))) return false;
        
        return true;
    }


    function isAppNo(obj) {
        if (obj.value.trim() != "" && isNaN(parseInt(obj.value))) {
            alert("Invalid L/C Application No.");
            obj.select();
            return false;
        }
        return true;
    }

    function isDate(obj) {
        var DMY_Date

        DMY_Date = obj.value.trim();
        if (DMY_Date != "") {
            if (!isDateValid(DMY_Date))
            {
                alert("Invalid Date Format (DD/MM/YYYY)");
                obj.select();
                return false;
            }
        }
        return true;
    }
    
</script>

<table >
<tr>
<td>&nbsp;</td>
</tr>

        <tr>
            <td class="FieldLabel4_T" style="width:100px;" valign="top">Customer</td>
            <td style="border: 1px solid #C0C0C0">
                <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4">
                </asp:CheckBoxList>
            </td>
            <td>
                <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
                <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4">Office Group</td>
            <td><cc2:SmartDropDownList  ID="ddl_OfficeGroup" runat="server"  Width="70px" AutoPostBack="True"  OnSelectedIndexChanged="ddl_OfficeGroup_SelectedIndexChanged" /></td>
        </tr>
        <tr id="tr_HandlingOffice" runat="server" style="display:none;">
            <td class="FieldLabel4">Handling Office</td>
            <td><cc2:SmartDropDownList  ID="ddl_HandlingOffice" runat="server" width="70px" AutoPostBack="false" /></td>
        </tr>
        <tr>
            <td class="FieldLabel4">Supplier</td>
            <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width:120px;">L/C Application Date</td>
            <td>
                <cc2:SmartCalendar ID="txt_LCAppDateFrom" runat="server" FromDateControl="txt_LCAppDateFrom" ToDateControl="txt_LCAppDateTo" />
                &nbsp;To&nbsp;
                <cc2:SmartCalendar ID="txt_LCAppDateTo"   runat="server" FromDateControl="txt_LCAppDateFrom" ToDateControl="txt_LCAppDateTo" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4">LC Application No.</td>
            <td>
                <asp:TextBox ID="txt_LCAppNoFrom" runat='server' onchange='return isAppNo(this)' value='' /> 
                &nbsp;To&nbsp;
                <asp:TextBox ID="txt_LCAppNoTo" runat="server" onchange='return isAppNo(this)' value=''/>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4">LC No</td>
            <td>
                <asp:TextBox ID="txt_LCNoFrom" runat='server' value='' /> 
                &nbsp;To&nbsp;
                <asp:TextBox ID="txt_LCNoTo" runat="server" value='' />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4">LC Details</td>
            <td><cc2:SmartDropDownList  ID="ddl_LCDetail" runat="server"  Width="90px"/></td>
        </tr>

        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btn_Preview" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Preview_Click" OnClientClick="return isFormValid();" />
                <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
            </td>
        </tr>
</table>
</asp:Content>