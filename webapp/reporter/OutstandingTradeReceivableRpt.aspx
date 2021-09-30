<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" Theme="DefaultTheme" CodeBehind="OutstandingTradeReceivableRpt.aspx.cs" Inherits="com.next.isam.webapp.reporter.OutstandingTradeReceivableRpt" Title="Outstanding Trade Receivable Report" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_reports_outstanding_receivable_rpt.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Outstanding Trade Receivable Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">   
     function isFormValid()
     {
     
        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value == "" &&
            (document.getElementById("ctl00_ContentPlaceHolder1_rad_InvoiceDate").checked &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "" &&                        
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == ""))
            {
                alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Invoice Date\r\n- Invoice Application Date\r\n- Fiscal Period");            
                return false;
            }
            if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "")) {
                alert("Invalid Invoice Date.");
                return false;
            }

            if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value != "")) {
                alert("Invalid Invoice Application Date");
                return false;
            }
            if (document.getElementById("ctl00_ContentPlaceHolder1_rad_FiscalPeriod").checked &&
            parseInt(document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodFrom").value) > parseInt(document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodTo").value)) {
                alert("Invalid fiscal period.");
                return false;
            }

            if (document.getElementById("ctl00_ContentPlaceHolder1_txt_CutOffDate_txt_CutOffDate_textbox").value == "") {
                alert("Please  enter cutoff date");
                return false;
            }

            if (GetCheckBoxSelectedCount('cbl_Customer') == 0)
            {
                alert("Please select one of the customers.");
                return false;
            }
            
            if (GetCheckBoxSelectedCount('cbl_TradingAgency') == 0)
            {
                alert("Please select one of the trading agencies");
                return false;
            }            

            return true;
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
<table >
<tr>
<td>&nbsp;</td>
</tr>
<tr>
    <td class="FieldLabel4">Version</td>
    <td>
        <asp:RadioButtonList ID="rad_ReportVersion" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rad_ReportVersion_SelectedChanged" AutoPostBack="true">
            <asp:ListItem Text="Epicor" Value="1" Selected="True" ></asp:ListItem>
            <asp:ListItem Text="SUN" Value="0"></asp:ListItem>
        </asp:RadioButtonList>

    </td>
</tr>
<tr>
    <td class="FieldLabel4">Customer Receipt Type</td>
    <td>
        <asp:RadioButtonList ID="radDataType" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Selected="True" Text="Both" Value="-1"></asp:ListItem>
            <asp:ListItem Text="Sales" Value="1"></asp:ListItem>
            <asp:ListItem Text="Sales Commission" Value="2"></asp:ListItem>
        </asp:RadioButtonList>
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Cutoff Date</td>
    <td><cc2:SmartCalendar ID="txt_CutOffDate" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Base Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_BaseCurrency" runat="server" /> </td>
</tr>
<tr id="tr_ExchangeRate" runat="server" style="display:block;">
    <td class="FieldLabel4">Exchange Rate</td>
    <td>&nbsp;Year&nbsp;&nbsp;<asp:DropDownList ID="ddl_ExchangeRateYear" runat="server" SkinID="SmallDDL" />&nbsp;&nbsp;&nbsp;
        Period&nbsp;&nbsp;<asp:DropDownList ID="ddl_ExchangeRatePeriod" runat="server" SkinID="SmallDDL" >
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
    <td class="FieldLabel4_T" style="width:150px; vertical-align:top;">Invoice Date</td>
    <td colspan="1">
    <asp:UpdatePanel ID="updatePanel2" runat="server" >
    <ContentTemplate >    
        <table cellspacing="0">
        <tr id="tr_InvoiceDate" runat="server" style="display:none;">
            <td>&nbsp;&nbsp;<asp:RadioButton ID="rad_InvoiceDate" runat="server" GroupName="rad_SelectCriteria" Text="By Range : " AutoPostBack="True" OnCheckedChanged="rad_InvoiceDate_CheckedChanged" /></td>
            <td>&nbsp;<cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />&nbsp;To&nbsp;<cc2:SmartCalendar 
                id="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" /></td>
        </tr>
        <tr>
            <td id="td_FiscalPeriod" runat="server" style="display:none">
                &nbsp;&nbsp;<asp:RadioButton ID="rad_FiscalPeriod" runat="server" GroupName="rad_SelectCriteria" Text="By Period : " Checked="true" OnCheckedChanged="rad_FiscalPeriod_CheckedChanged" AutoPostBack="True" />
            </td>
            <td>
                &nbsp;Year&nbsp;&nbsp;<asp:DropDownList  ID="ddl_Year" runat="server" SkinId="SmallDDL" Enabled="true"  />&nbsp;&nbsp;&nbsp;
                Period From&nbsp;&nbsp;<asp:DropDownList id="ddl_PeriodFrom" runat="server" 
                    SkinID="SmallDDL" Enabled="true" AutoPostBack="True" 
                    onselectedindexchanged="ddl_PeriodFrom_SelectedIndexChanged" >
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
                </asp:DropDownList>  &nbsp;To&nbsp;&nbsp;
                <asp:DropDownList ID="ddl_PeriodTo" runat="server" SkinID="SmallDDL" Enabled="true" >
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
    <td class="FieldLabel4" style="width :140px;">Invoice Application Date</td>
    <td><cc2:SmartCalendar ID="txt_InvoiceUploadDateFrom" runat="server" FromDateControl="txt_InvoiceUploadDateFrom"
        ToDateControl="txt_InvoiceUploadDateTo" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_InvoiceUploadDateTo" runat ="server" 
        FromDateControl="txt_InvoiceUploadDateFrom" ToDateControl="txt_InvoiceUploadDateTo" />
    </td>
</tr>

<tr>
    <td class="FieldLabel4">Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_Currency" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Receipt Reference No</td>
    <td><asp:DropDownList ID="ddl_ReceiptRefNo" runat="server" /> </td>
</tr>
<tr>
    <td class="FieldLabel4">Purchase Term</td>
    <td><cc2:smartdropdownlist ID="ddl_TermOfPurchase" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Office Group</td>
    <td><cc2:SmartDropDownList  ID="ddl_Office" runat="server" onChange="ddl_Office_OnChange(this)"/></td>    
</tr>
<tr>
    <td class="FieldLabel4">Handling Office</td>
    <td><asp:DropDownList ID="ddl_HandlingOffice" runat="server" >
        <asp:ListItem Text="-- ALL --" Value="-1" />
        <asp:ListItem Text="DG" Value="17" />
        <asp:ListItem Text="HK" Value="1" />
        <asp:ListItem Text="SH" Value="2" />
        <asp:ListItem Text="VN" Value="16" />
    </asp:DropDownList></td>
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
    <td class="FieldLabel4_T" rowspan="5" style="vertical-align:top ;">Order Type</td>
    <td><asp:DropDownList ID="ddl_SZOrder" runat="server" SkinID="LargeDDL" >
            <asp:ListItem Text="All (SZ + Non-SZ Order)" Value="-1" Selected="True" />
            <asp:ListItem Text="SZ Order" Value="1" />
            <asp:ListItem Text="Non-SZ Order" Value ="0" />
        </asp:DropDownList></td>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ddl_OrderType" runat="server" SkinID="LargeDDL" >
            <asp:ListItem Text="ALL (FOB + VM)" Value="" Selected="True"  />
            <asp:ListItem Text="FOB" Value="F" />
            <asp:ListItem Text="VM" Value="V" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ddl_UTOrder" runat="server" SkinID="LargeDDL" >
            <asp:ListItem Text="All (UT + Non-UT Order)" Value="-1" Selected="True" />
            <asp:ListItem Text="UT Order" Value="1" />
            <asp:ListItem Text="Non-UT Order" Value="0" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ddl_OPROrder" runat="server" SkinID="LargeDDL" >
            <asp:ListItem Text="All (OPR + Non-OPR Order)" Value ="-1" Selected="True" />
            <asp:ListItem Text="OPR Order" Value ="1" />
            <asp:ListItem Text="Non-OPR Order" Value="0" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinId="LargeDDL">
            <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS SAMPLE ORDER)" Value="-1" />
            <asp:ListItem Text="MAINLINE ORDER" Selected="True" Value="0" />
            <asp:ListItem Text="MOCK SHOP/PRESS SAMPLE ORDER" Value="1" />
            <asp:ListItem Text="MOCK SHOP ORDER" Value="2" />
            <asp:ListItem Text="PRESS SAMPLE ORDER" Value="3" />
        </asp:DropDownList>
    </td>
</tr>

<tr>
    <td class="FieldLabel4_T" style="vertical-align :top;">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>    
    <td>
        <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Trading Agency</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>
    <td>
        <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_TradingAgency'); return false;" />
    </td>
</tr>
<tr>
<td>&nbsp;</td>
</tr>
<tr>
<td colspan="2"><asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
</td>
</tr>
</table>

</asp:Content>
