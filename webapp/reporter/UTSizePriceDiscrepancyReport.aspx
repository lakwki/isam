<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="UTSizePriceDiscrepancyReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.UTSizePriceDiscrepancyReport" %>
<%@ Register Src="~/usercontrol/UclOfficeSelection.ascx" TagName="UclOfficeSelection" TagPrefix="uc2" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">UT Size Option Price Discrepancy Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:UpdatePanel ID="updatePanel3" runat="server">
<ContentTemplate >
<table>
<tr>
<td colspan="2">&nbsp;</td>
</tr>
<tr>
    <td colspan="2">
        <uc2:UclOfficeSelection ID="uclOfficeSelection" runat="server" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Customer At-Warehouse Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_CustomerAtWHDateFrom" runat="server" FromDateControl="txt_CustomerAtWHDateFrom" ToDateControl="txt_CustomerAtWHDateTo" /> To 
        <cc2:SmartCalendar ID="txt_CustomerAtWHDateTo" runat="server" FromDateControl="txt_CustomerAtWHDateFrom" ToDateControl="txt_CustomerAtWHDateTo" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Invoice Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" /> To 
        <cc2:SmartCalendar ID="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />        
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Supplier</td>
    <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
       <asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
       <asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  OnClientClick="return isFormValid();" />
</asp:Content>
