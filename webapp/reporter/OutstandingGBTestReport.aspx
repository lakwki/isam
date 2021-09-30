<%@ Page Title="ISAM - Outstanding GB Test Result Report" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="OutstandingGBTestReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.OutstandingGBTestReport" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc1" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Outstanding GB Test Result Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table style="width:800px;">
    <tr>
        <td class="FieldLabel4" style="width:22%;">Office Group</td>
        <td><cc1:SmartDropDownList  ID="ddl_Office" runat="server"  Width="80px" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Invoice Date</td>
        <td>
            <cc1:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />&nbsp;To&nbsp;<cc1:SmartCalendar 
                id="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Customer At Warehouse Date</td>
        <td>
            <cc1:SmartCalendar ID="txt_CustAWHDateFrom" runat="server" FromDateControl="txt_CustAWHDateFrom" ToDateControl="txt_CustAWHDateTo" />&nbsp;To&nbsp;<cc1:SmartCalendar 
                id="txt_CustAWHDateTo" runat="server" FromDateControl="txt_CustAWHDateFrom" ToDateControl="txt_CustAWHDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Supplier</td>
        <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="vertical-align :top;">Shipment Method</td>
        <td class="CellWithBorder" style=" width:500px;">
            <asp:CheckBoxList ID="cbl_ShipmentMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table"  >
            </asp:CheckBoxList>            
        </td>
        <td>
            <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_ShipmentMethod'); return false;" />&nbsp;
            <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_ShipmentMethod'); return false;" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="vertical-align :top ;">Payment Term</td>
        <td class="CellWithBorder">
            <asp:CheckBoxList ID="cbl_PaymentTerm" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table"  >
            </asp:CheckBoxList>                        
        </td>
        <td>
            <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_PaymentTerm'); return false;" />&nbsp;
            <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_PaymentTerm'); return false;" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="vertical-align :top ;">Payment Status</td>
        <td class="CellWithBorder">
            <asp:CheckBoxList ID="cbl_PaymentStatus" runat="server"  TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" >
                <asp:ListItem Text="Paid" Value="1" Selected="True"  />
                <asp:ListItem Text="Not Yet Paid" Value="0" Selected="True"  />
            </asp:CheckBoxList>                         
        </td>
        <td>
            <asp:ImageButton ID="btn_Clear3" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_PaymentStatus'); return false;" />&nbsp;
            <asp:ImageButton ID="btn_All3" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_PaymentStatus'); return false;" />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>    
    <tr>
        <td colspan="3">
            <asp:Button ID="btn_Preview" runat="server" Text="Print Preview" SkinID="LButton" OnClick="btn_Preview_Click" />&nbsp;&nbsp;
            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinId="LButton" OnClick="btn_Export_Click" />
        </td>
    </tr>
</table>
</asp:Content>
