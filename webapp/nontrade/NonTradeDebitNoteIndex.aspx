<%@ Page Title="Non-Trade Expense - Debit Note" Language="C#" Theme="DefaultTheme"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="NonTradeDebitNoteIndex.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NonTradeDebitNoteIndex" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinId="sectionHeader_Accounts">Non-Trade Expense Debit Note</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table>
    <tr runat="server" id="tr_DebitNoteRecharge">
        <td>-&nbsp;<a href="NonTradeRecharge.aspx">Generate Non-Trade Debit Note</a></td>
    </tr>
    <tr runat="server" id="tr_DebitNoteSearch">
        <td>-&nbsp;<a href="DebitNoteSearch.aspx">Non-Trade Debit Note Search Engine</a></td>
    </tr>
</table>
</asp:Content>
