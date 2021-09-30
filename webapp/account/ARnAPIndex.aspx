<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ARnAPIndex.aspx.cs" Inherits="com.next.isam.webapp.account.ARnAPIndex" Title="A/C Receivable and Payable" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinId="sectionHeader_Accounts"></asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="750">
        <tr id="row_InvReg" runat="server" visible ="false">
            <td>-&nbsp;&nbsp;<a href="SuppInvReg.aspx">Invoice Registration for Payment</a></td>
        </tr>
        <tr id="row_PaymentFileConversion" runat="server" visible ="false">
            <td>-&nbsp;&nbsp;<a href="PaymentFileConversion.aspx">Payment File Conversion</a></td>
        </tr>
        <tr id="row_BankRec" runat="server" visible="false">
            <td>-&nbsp;&nbsp;<a href="BankReconciliation.aspx">Payment Record Update</a></td>
        </tr>
        <tr id="row_eInvReg" runat="server" visible="false">
            <td>-&nbsp;&nbsp;<a href="ARShipmentReg.aspx">eInvoice (A/R Shipment Registration)</a></td>
        </tr>
        <tr id="row_CreateInvBatch" runat="server" visible="false">
            <td>-&nbsp;&nbsp;<a href="CreateInvBatch.aspx">eInvoice (Non self-billed shipment)</a></td>
        </tr>
        <tr id="row_ARAPMaint" runat="server" visible="false">
            <td>-&nbsp;&nbsp;<a href="PaymentMaintenance.aspx">A/R and A/P Settlement Maintenance</a></td>            
        </tr>
        <tr id="row_PaymentAdvice" runat="server" visible="false" >
            <td>-&nbsp;&nbsp;<a href="ePaymentAdvice.aspx">ePayment Advice</a></td>
        </tr>
        <tr id="row_PaymentStatusEnquiry" runat="server" visible="false" >
            <td>-&nbsp;&nbsp;<a href="InvoiceStatusEnquiry.aspx">Payment Document Enquiry</a></td>
        </tr>
    </table>
</asp:Content>
