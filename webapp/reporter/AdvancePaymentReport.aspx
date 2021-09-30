<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="AdvancePaymentReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.AdvancePaymentReport" Theme="DefaultTheme"  %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Advance Payment Report</asp:Panel> 
<br/>
<table>
    <tr><td class="FieldLabel4">Office</td><td style="width: 319px"><cc2:smartdropdownlist id="ddlOffice" runat="server"></cc2:smartdropdownlist></td></tr>
    <tr><td class="FieldLabel4">Supplier</td><td style="width: 319px"><uc1:UclSmartSelection  ID="txt_Supplier" runat="server" width="300px"/></td></tr>
    <tr><td class="FieldLabel4">Payment Status</td><td style="width: 319px"><asp:DropDownList runat="server" ID="ddlStatus"/></td></tr>
    <tr><td class="FieldLabel4">Payment Date</td>
        <td style="width: 319px">
            <cc2:SmartCalendar ID="txt_PaymentDateFrom" runat="server" FromDateControl="txt_PaymentDateFrom" ToDateControl="txt_PaymentDateTo" />&nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_PaymentDateTo" runat="server" FromDateControl="txt_PaymentDateFrom" ToDateControl="txt_PaymentDateTo" />
        </td>
    </tr>
    <tr><td class="FieldLabel4">Deduction Date</td>
        <td style="width: 319px">
            <cc2:SmartCalendar ID="txt_DeductionDateFrom" runat="server" FromDateControl="txt_DeductionDateFrom" ToDateControl="txt_DeductionDateTo" />&nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_DeductionDateTo" runat="server" FromDateControl="txt_DeductionDateFrom" ToDateControl="txt_DeductionDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Version</td><td style="width: 319px">
        <asp:RadioButtonList ID="rbVersion" runat="server" RepeatDirection="Horizontal">
            <%--<asp:ListItem Value="0">Normal</asp:ListItem>--%>
            <asp:ListItem Value="1" Selected="True">Management</asp:ListItem>
        </asp:RadioButtonList>
        </td>
    </tr>
</table>
<br/>
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  />
<asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" Visible="false"/>
</asp:Content>
