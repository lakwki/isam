<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme"  AutoEventWireup="true" CodeBehind="ReceivablePayableForecastReport.aspx.cs"  Inherits="com.next.isam.webapp.reporter.AccountsReceivablePayableForecast" Title="Accounts Receivable and Payable Forecast Report" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_reports_ar_ap_forecast.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Account Receivable And Payable Forecast Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript">
     
     function isFormValid()
     {
     
        if (
            (document.getElementById("ctl00_ContentPlaceHolder1_txt_ReportDate_txt_ReportDate_textbox").value == "")
            || (GetCheckBoxSelectedCount('cbl_Office') == 0) )
            {
                alert("Please enter both of the following report criteria:\r\n" 
                    + "- Report Date;\r\n" 
                    + "- Office;\r\n");            
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
    <td class="FieldLabel4" style="width:100px;">Report Date</td>
    <td><cc2:SmartCalendar ID="txt_ReportDate" runat="server" /></td>
</tr>

<tr>
    <td class="FieldLabel4_T" valign="top">Office</td>
    <td class="CellWithBorder" style="text-align:right;">
        <asp:CheckBoxList ID="cbl_Office" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4" style="text-align:left;" />
        <asp:LinkButton ID="lnk_SelectAll" runat="server" Text="Select All" OnClientClick="CheckAll('cbl_Office');" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="Lnk_DeselectAll" runat="server" Text="Deselect All" OnClientClick="UncheckAll('cbl_Office');" />
        &nbsp;&nbsp;
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Payment Term</td>
    <td>
        <cc2:SmartDropDownList  ID="ddl_PaymentTerm" runat="server" />
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
