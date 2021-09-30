<%@ Page Title="ISAM - System Maintenance" Theme="DefaultTheme"  Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="SystemMaintenance.aspx.cs" Inherits="com.next.isam.webapp.myapplication.SystemMaintenance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="panel1" runat="server" SkinID="sectionHeader_PersonalSettings">System Maintenance</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table >
    <tr>
        <td><a href="LCBankMaint.aspx">- LC Bank Maintenance</a></td>
    </tr>
</table>
</asp:Content>
