<%@ Page Title="Actual Sales Summary by Product"  Theme="DefaultTheme"  Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ActualSalesSummary.aspx.cs" Inherits="com.next.isam.webapp.reporter.ActualSalesSummary" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_reports_actual_sales_summary.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Actual Sales Summary</asp:Panel> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function isFormValid() {

        if (
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value == "" &&
            (document.getElementById("ctl00_ContentPlaceHolder1_rad_InvoiceDate").checked &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "") &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value == "") {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Invoice Date\r\n- Invoice Application Date\r\n- Fiscal Period");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value != "")) {
            alert("Invalid Invoice Upload Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "")) {
            alert("Invalid Invoice Date.");
            return false;
        }

        if (document.getElementById("ctl00_ContentPlaceHolder1_rad_FiscalPeriod").checked &&
            document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodFrom").value > document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodTo").value) {
            alert("Invalid fiscal period.");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_Customer') == 0) {
            alert("Please select one of the customers.");
            return false;
        }

        if (GetCheckBoxSelectedCount('cbl_TradingAgency') == 0) {
            alert("Please select one of the trading agencies");
            return false;
        }
        return true;
    }     
</script>
<table >
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td class="FieldLabel4" style="width:150px;">Invoice Application Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_InvoiceUploadDateFrom" runat="server" FromDateControl="txt_InvoiceUploadDateFrom" ToDateControl="txt_InvoiceUploadDateTo" />&nbsp;To&nbsp;
        <cc2:SmartCalendar ID="txt_InvoiceUploadDateTo" runat="server" FromDateControl="txt_InvoiceUploadDateFrom" ToDateControl="txt_InvoiceUploadDateTo" />
    </td>
</tr>

<tr>
    <td style="width:100%;" colspan="3">
    <asp:UpdatePanel ID="updatePanel3" runat="server" >
        <ContentTemplate >               
        <table cellspacing="0" cellpadding="0" >
            <tr>
                <td class="FieldLabel4_T" style="width:155px; vertical-align:top ;" rowspan="3">Invoice Date</td>
                <td>&nbsp;<asp:RadioButton ID="rad_InvoiceDate" runat="server" GroupName="rad_SelectCriteria" AutoPostBack="true" Checked="true" Text="By Range : " OnCheckedChanged="rad_InvoiceDate_CheckedChanged" />
                    &nbsp;<cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />&nbsp;To&nbsp;
                    &nbsp;<cc2:SmartCalendar ID="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />
                </td>
            </tr>
            <tr>
                <td>&nbsp;<asp:RadioButton ID="rad_FiscalPeriod" runat="server" GroupName="rad_SelectCriteria" AutoPostBack="true" Text="By Period : " OnCheckedChanged="rad_FiscalPeriod_CheckedChanged" />
                &nbsp;Year&nbsp;&nbsp;&nbsp;<asp:DropDownList  ID="ddl_Year" runat="server" SkinId="SmallDDL" Enabled="false" />&nbsp;&nbsp;&nbsp;
        Period From&nbsp;&nbsp;<asp:DropDownList id="ddl_PeriodFrom" runat="server" SkinID="SmallDDL" 
                        Enabled="false" AutoPostBack="True" 
                        onselectedindexchanged="ddl_PeriodFrom_SelectedIndexChanged">
        <asp:ListItem Text="1" Value="1" />
        <asp:ListItem Text="2" Value="2" />
        <asp:ListItem Text="3" Value="3" />
        <asp:ListItem Text="4" Value="4" />
        <asp:ListItem Text="5" Value="5" />
        <asp:ListItem Text="6" Value="6" />
        <asp:ListItem Text="7" Value="7" />
        <asp:ListItem Text="8" Value="8" />
        <asp:ListItem Text="9" Value="9" />
        <asp:ListItem Text="10" Value="10" />
        <asp:ListItem Text="11" Value="11" />
        <asp:ListItem Text="12" Value="12" />
        </asp:DropDownList>  &nbsp;To&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="ddl_PeriodTo" runat="server" SkinID="SmallDDL" Enabled="false">
                <asp:ListItem Text="1" Value="1" />
                <asp:ListItem Text="2" Value="2" />
                <asp:ListItem Text="3" Value="3" />
                <asp:ListItem Text="4" Value="4" />
                <asp:ListItem Text="5" Value="5" />
                <asp:ListItem Text="6" Value="6" />
                <asp:ListItem Text="7" Value="7" />
                <asp:ListItem Text="8" Value="8" />
                <asp:ListItem Text="9" Value="9" />
                <asp:ListItem Text="10" Value="10" />
                <asp:ListItem Text="11" Value="11" />
                <asp:ListItem Text="12" Value="12" />
        </asp:DropDownList> 
                </td>
            </tr>
            <tr>
                <td>&nbsp;<asp:CheckBox ID="ckb_Actual" runat="server" Checked="true" Enabled="false" Text="Actual" />&nbsp;&nbsp;&nbsp;
                    &nbsp;<asp:CheckBox ID="ckb_Realized" runat="server" Checked ="true" Enabled="false" Text="Realized" />
                 </td>
            </tr>
            <tr>
                <td class="FieldLabel4">Office</td>
                <td>&nbsp;&nbsp;<cc2:SmartDropDownList  ID="ddl_Office" runat="server" OnSelectedIndexChanged="ddl_Office_SelectedIndexChanged" AutoPostBack="True" /></td>
            </tr>
            <tr><td style="height:5px;"></td></tr>
            <tr>
                <td class="FieldLabel4">Department</td>
                <td>&nbsp;&nbsp;<cc2:SmartDropDownList ID="ddl_Department" runat="server" /></td>
            </tr>        
        </table>
        </ContentTemplate>
        <Triggers >
                 <asp:AsyncPostBackTrigger ControlID="rad_InvoiceDate" 
                    EventName="CheckedChanged" />
                <asp:AsyncPostBackTrigger ControlID="rad_FiscalPeriod" 
                    EventName="CheckedChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddl_Office" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Product Team</td>
    <td><uc1:UclSmartSelection ID="uclProductTeam" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Base Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_BaseCurrency" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4_T" rowspan="4" style="vertical-align :top;">Order Type</td>
    <td><asp:DropDownList ID="ddl_SZOrder" runat="server" SkinID="LargeDDL" >
        <asp:ListItem Text="ALL (SZ + NON-SZ ORDER)" Value ="-1" Selected="True" />
        <asp:ListItem Text="SZ ORDER" Value="1" />
        <asp:ListItem Text="NON-SZ ORDER" Value="0" />
    </asp:DropDownList></td>
</tr>
<tr>
    <td><asp:DropDownList ID="ddl_UTOrder" runat="server" SkinID="LargeDDL">
        <asp:ListItem Text="ALL (UT + NON-UT ORDER)" Value="-1" Selected="True" />
        <asp:ListItem Text="UT ORDER" Value="1" />
        <asp:ListItem Text="NON-UT ORDER" Value="0" />
    </asp:DropDownList></td>
</tr>
<tr>
    <td><asp:DropDownList ID="ddl_OPROrder" runat="server" SkinID="LargeDDL" >
        <asp:ListItem Text="ALL (OPR + NON-OPR ORDER)" Value="-1" Selected="True" />
        <asp:ListItem Text="OPR ORDER" Value="1" />
        <asp:ListItem Text="NON-OPR ORDER" Value="0" />
    </asp:DropDownList></td>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ddl_LDPOrder" runat="server" SkinID="LargeDDL" >
            <asp:ListItem Text="ALL (LDP + NON-LDP ORDER)" Value="-1" Selected="True" />
            <asp:ListItem Text="LDP ORDER" Value="1" />
            <asp:ListItem Text="NON-LDP ORDER" Value="0" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Season</td>
    <td><cc2:SmartDropDownList ID="ddl_Season" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Country of Origin</td>
    <td><cc2:SmartDropDownList ID="ddl_CO" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Loading Port</td>
    <td><cc2:SmartDropDownList ID="ddl_LoadingPort" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Purchase Term</td>
    <td><cc2:SmartDropDownList ID="ddl_PurchaseTerm"  runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4_T" style="vertical-align:top ;">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400px" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>    
    <td>
        <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4" style="vertical-align :top;">Trading Agency</td>
    <td class="CellWithBorder"><asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" Width="400px" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList></td>
    <td>
        <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_TradingAgency'); return false;" />
    </td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td colspan="2">
        <asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
        <asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  OnClientClick="return isFormValid();" />
    </td>
</tr>
</table>
</asp:Content>
