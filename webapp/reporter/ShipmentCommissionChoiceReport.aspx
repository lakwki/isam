<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ShipmentCommissionChoiceReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.ShipmentCommissionChoiceReport" Title="Shipment and Commission Report (Choice)" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<script type="text/javascript">
    function isFormValid() {

        if (document.getElementById("<%= txt_InvoiceNoTo.ClientID %>").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value == "" &&
            (document.getElementById("ctl00_ContentPlaceHolder1_rad_InvoiceDate").checked &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "") &&
            (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value == "") &&
            (document.getElementById("ctl00_ContentPlaceHolder1_txt_ExtractDateFrom_txt_ExtractDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_ExtractDateTo_txt_ExtractDateTo_textbox").value == "")) {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Invoice No.\r\n- Invoice Date\r\n- Invoice Application Date\r\n- Fiscal Period\r\n- Purchase Extract Date");
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


        if (GetCheckBoxSelectedCount('cbl_TradingAgency') == 0) {
            alert("Please select one of the trading agencies");
            return false;
        }

        return true;
    }
    function copyInvoiceNo() {
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value.toUpperCase();
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value;
    }

    function ddl_Office_OnChange(obj) {
        var uclPrefix = "<%= uclProductTeam.ClientID %>";
        if (obj[obj.selectedIndex].text.indexOf("+") > 0) {
            clearSmartSelection(null, uclPrefix, null);
            disableSmartSelection(uclPrefix);
        }
        else {
            enableSmartSelection(uclPrefix);
        }
    }
    
</script>
<!--
<img src="../images/banner_reports_ship_comm_stmt_choice.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel runat="server" SkinID="sectionHeader_Report">Shipment And Commission Statement (Choice)</asp:Panel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table>
<tr>
<td>&nbsp;</td>
</tr>
<tr>
    <td colspan="2">
    <asp:UpdatePanel runat="server" ID="updatePanel2">
<ContentTemplate>
        <table width="100%">

<tr>
    <td class="FieldLabel4" style="width:150px;">Invoice Date</td>
    <td><asp:RadioButton ID="rad_InvoiceDate" runat="server" Checked="true" AutoPostBack="true" GroupName="rad_SelectCriteria" OnCheckedChanged="rad_InvoiceDate_CheckedChanged" Text="By Range : " />
    </td><td>
        <cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo"   />&nbsp;To&nbsp;<cc2:SmartCalendar 
        id="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo"  /></td>
</tr>

<tr>
    <td></td>
    <td><asp:RadioButton ID="rad_FiscalPeriod" runat="server" AutoPostBack="true" GroupName="rad_SelectCriteria" OnCheckedChanged="rad_FiscalPeriod_CheckedChanged" Text="By Period : " /></td>
    <td>
        Year&nbsp;&nbsp;<asp:DropDownList  ID="ddl_Year" runat="server" SkinId="SmallDDL" Enabled ="false"  />&nbsp;&nbsp;&nbsp;
        Period&nbsp;<asp:DropDownList id="ddl_PeriodFrom" runat="server" 
            SkinID="SmallDDL" Enabled ="false" AutoPostBack="True" >
        <asp:ListItem Text="1" Value="1"  />
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
    <td></td>
    <td colspan="2">
        <asp:CheckBox ID="ckb_Actual" runat="server" Checked="true" Enabled="false" Text="Actual" />&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID="ckb_Realized" runat="server" Checked ="true" Enabled="false" Text="Realized" />&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID="ckb_Accrual" runat="server" Checked="true" Enabled="false" Text="Accrual" />
    </td>
</tr>
        </table>
        </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="rad_InvoiceDate" 
            EventName="CheckedChanged" />
        <asp:AsyncPostBackTrigger ControlID="rad_FiscalPeriod" 
            EventName="CheckedChanged" />
    </Triggers>
</asp:UpdatePanel>
    </td>
</tr>
        <tr>
    <td class="FieldLabel4" style="width:150px;">Invoice No</td>
    <td><asp:TextBox ID="txt_InvoiceNoFrom" runat="server" onblur="copyInvoiceNo();" />&nbsp;To&nbsp;<asp:TextBox ID="txt_InvoiceNoTo" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Invoice Application Date</td>
    <td><cc2:SmartCalendar ID="txt_InvoiceUploadDateFrom" runat="server" FromDateControl="txt_InvoiceUploadDateFrom"
        ToDateControl="txt_InvoiceUploadDateTo"  />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_InvoiceUploadDateTo" runat ="server" 
        FromDateControl="txt_InvoiceUploadDateFrom" ToDateControl="txt_InvoiceUploadDateTo"  />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Purchase Extract Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_ExtractDateFrom" runat="server" FromDateControl="txt_ExtractDateFrom"
            ToDateControl="txt_ExtractDateTo" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_ExtractDateTo" runat="server" 
            FromDateControl="txt_ExtractDateFrom" ToDateControl ="txt_ExtractDateTo" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Base Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_BaseCurrency" runat="server" /> </td>
</tr>
<tr>
    <td colspan="2">
    <asp:UpdatePanel ID="updatePanel3" runat="server">
        <ContentTemplate>        
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td class="FieldLabel4" style="width:150px;">Office Group</td>
                <td>&nbsp;<cc2:SmartDropDownList  ID="ddl_Office" runat="server" OnSelectedIndexChanged="ddl_Office_SelectedIndexChanged" onChange="ddl_Office_OnChange(this)" AutoPostBack="True" /></td>
            </tr>
            <tr><td style="height:5px;"></td></tr>
            <tr>
                <td class="FieldLabel4">Department</td>
                <td>&nbsp;<cc2:SmartDropDownList ID="ddl_Department"  runat="server" /></td>
            </tr>            
        </table>
            </ContentTemplate> 
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddl_Office" EventName="SelectedIndexChanged" />
                    </Triggers>
        </asp:UpdatePanel>
    </td>
</tr>

<tr>
    <td class="FieldLabel4">Product Team</td>
    <td><uc1:uclsmartselection id="uclProductTeam" runat="server"></uc1:uclsmartselection></td>
</tr>
<tr>
    <td class="FieldLabel4">Supplier</td>
    <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Purchase Term</td>
    <td><cc2:SmartDropDownList ID="ddl_termOfPurchase" runat="server" /></td>
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
    <td class="FieldLabel4_T" style="vertical-align :top;">Trading Agency</td>
    <td class="CellWithBorder"><asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList></td>
        <td><asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Sort Order</td>
    <td>
        <asp:RadioButtonList ID="rad_SortField" runat="server"  RepeatDirection="Horizontal">
            <asp:ListItem Text="Invoice No" Value="InvoiceNo" Selected="True" />
            <asp:ListItem Text="Supplier" Value="VendorName" />
            <asp:ListItem Text="Invoice Date" Value="InvoiceDate" />
        </asp:RadioButtonList>
    </td>
</tr>
<tr>
<td>&nbsp;</td>
</tr>
<tr>
<td colspan="2">
</td>
</tr>
</table>

<asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
</asp:Content>
