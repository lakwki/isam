<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="APAdjustment.aspx.cs" Inherits="com.next.isam.webapp.account.APAdjustment" Title="A/P Adjustment" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc1" %>
    <asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_account_ap_adj.gif" runat="server" id="imgHeaderText" />
<img src="../images/banner_workplace.gif" runat="server" id="img1" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Supplier Amount Amendment Debit/Credit Notes</asp:Panel>
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
<table>
    <tr>
        <td class="FieldLabel2">Office Group</td>
        <td><cc1:smartdropdownlist id="ddl_Office" runat="server" Width="200px"></cc1:smartdropdownlist>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Debit / Credit Note Date</td>
        <td>
            <cc1:SmartCalendar ID="txtIssueDate" runat="server" />
        </td>
    </tr>
    <tr>
        <td  class="FieldLabel2">Document Type</td>
        <td>
            <asp:RadioButton ID="radDraft" runat="server" GroupName="DocumentType" Text="Draft"/> 
            &nbsp;&nbsp;&nbsp;
            <asp:RadioButton ID="radOfficial" runat="server" GroupName="DocumentType" Text="Official"/> 
        </td>
    </tr>
    <tr>
        <td>
            <asp:CustomValidator ID="valIssueDate" runat="server" Display="None" 
                ErrorMessage="Issue Date is required for official copy" 
                onservervalidate="valIssueDate_ServerValidate"></asp:CustomValidator>
        </td>
        <td>
            <asp:Button ID="btn_Submit" runat="server" Text="Submit" onclick="btn_Submit_Click" />&nbsp;&nbsp;&nbsp;
        </td>
    </tr>
</table>
<br />
<br />
</asp:Content>