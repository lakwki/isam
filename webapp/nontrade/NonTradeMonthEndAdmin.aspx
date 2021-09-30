<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="NonTradeMonthEndAdmin.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NonTradeMonthEndAdmin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Accounts">Non-Trade Expense</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<span class="header2" style="margin:10px;">Non-Trade Accounts Month End Admin</span><br />
<div id="div_alert" runat="server" visible="false"  style="margin:10px; color :Red;">Exchange Rate for fiscal year <asp:Label ID="lbl_FiscalYear" runat="server" /> period <asp:Label ID="lbl_Period" runat="server" /> is not setup properly.<br />
Please contact system administrator.
</div>
<table style="border: 1px solid #B0B0B0; margin: 10px;" cellspacing="0" cellpadding="0">
<tr>
    <td>
<asp:GridView ID="gv_NTMonthEnd" runat="server" OnRowDataBound="MonthEndStatusDataBound" OnRowCommand="MonthEndRowCommand" >
    <Columns >
        <asp:TemplateField HeaderText="Office" ItemStyle-Width ="150px" HeaderStyle-Height="20px">
            <ItemTemplate ><%# Eval("Office.Description") %></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Fiscal Year" ItemStyle-Width="80px">
            <ItemTemplate >
                <%# Eval("FiscalYear") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Period" ItemStyle-Width="80px">
            <ItemTemplate >
                <%# Eval("Period") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px">
            <ItemTemplate >
                <%# Eval("StatusDescription") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  ItemStyle-Width="100px">
            <ItemTemplate >
                <asp:Button ID="btn_ChangeStatus" runat="server" CommandName="ChangeStatus" CommandArgument='<%# Eval("Office.OfficeId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
 </td>
</tr>
</table>
</asp:Content>
