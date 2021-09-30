<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ReplacementInvoiceEnquiry.aspx.cs" Inherits="com.next.isam.webapp.account.ReplacementInvoiceEnquiry" Title="Replacement Invoice Enquiry" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc1" %>
    <asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_account_replace_invoice.gif" runat="server" id="imgHeaderText" />
<img src="../images/banner_workplace.gif" runat="server" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Replacement Invoice Enquiry</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td>
            <asp:ValidationSummary ID="valSummary" runat="server" Font-Bold="true"/>
        </td>
    </tr>
</table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <span class="FieldLabel2" style="width:120px;">ILS Invoice No</span>
            <asp:TextBox ID="txtILSInvoiceNo" runat="server"></asp:TextBox>&nbsp;&nbsp;<asp:Button 
                ID="btnQuery" runat="server" Text="Query" onclick="btnQuery_Click" /><asp:UpdateProgress
                    ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        <img  id="imgloader" runat="server" src="../images/ajax-loader.gif" alt="ajax loader"/>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            <br /><br />
            <b>Shipment</b> : <asp:Label ID="lblShipment" runat="server" ForeColor="Blue"></asp:Label>
            <br /><br />
            <b>Replacement Invoice No</b> : <asp:Label ID="lblReplacementInvoiceNo" runat="server" ForeColor="Blue"></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
<br />
<br />
<b>Function Description</b>: To lookup the replacement invoice number by using the invoice no from the A/R reimbursement file
</asp:Content>