<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="DCNoteDiscrepancy.aspx.cs" Inherits="com.next.isam.webapp.account.DCNoteDiscrepancy" Title="ILS Discrepancy Debit / Credit Note" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:panel runat="server" SkinID="sectionHeader_Accounts">ILS Discrepancy Debit / Credit Note</asp:panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
    <tr>
        <td>&nbsp;</td>
    </tr>
</table>
<table>
    <tr>
        <td class="FieldLabel3">Office</td>
        <td><asp:DropDownList ID="ddl_Office" runat="server">
                <asp:ListItem Text="HK" Value="HK" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel3">A/R Settlement Date</td>
        <td>
            <cc1:SmartCalendar ID="txt_SettleDateFrom" ToDateControl="txt_SettleDateTo" runat="server" />&nbsp;To&nbsp;
            <cc1:SmartCalendar ID="txt_SettleDateTo" FromDateControl="txt_SettleDateFrom" runat="server" />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Submit" runat="server" Text="Submit" 
                onclick="btn_Submit_Click" />&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" />&nbsp;&nbsp;&nbsp;
        </td>
    </tr>
</table>
<br />
<asp:Panel ID="pnl_Result" runat="server" Visible="false" >
<asp:LinkButton ID="lbl_SelectAll" runat="server" OnClientClick="CheckAll('ckb_DC');return false;" Text="Select All" />&nbsp;&nbsp;&nbsp;
<asp:LinkButton ID="lblDeselectAll" runat="server" OnClientClick="UncheckAll('ckb_DC');return false;" Text="Deselect All" />
<asp:GridView ID="gv_DC" runat="server" AutoGenerateColumns="false">
    <Columns>
        <asp:TemplateField >
            <ItemTemplate >
                <asp:CheckBox ID="ckb_DC" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Office">
            <ItemTemplate >
                <asp:Label ID="lbl_Office" runat="server" Text='<%# Eval("Office") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Invoice No.">
            <ItemTemplate >
                <asp:Label ID="lbl_InvNo" runat="server" Text='<%# Eval("InvoiceNumber") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Invoice Date">
            <ItemTemplate >
                <asp:Label ID="lbl_InvDate" runat="server" Text='<%# Eval("InvoiceDate") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Contract No.">
            <ItemTemplate >
                <asp:Label ID="lbl_ContractNo" runat="server" Text='<%# Eval("ContractNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Dly No.">
            <ItemTemplate >
                <asp:Label ID="lbl_DlyNo" runat="server" Text='<%# Eval("DlyNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ccy">
            <ItemTemplate >
                <asp:Label ID="lbl_Ccy" runat="server" Text='<%# Eval("Ccy") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="A/R Date">
            <ItemTemplate >
                <asp:Label ID="lbl_ARDate" runat="server" Text='<%# Eval("ARDate") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="A/R Amount">
            <ItemTemplate >
                <asp:Label ID="lbl_ARAmt" runat="server" Text='<%# Eval("ARAmt") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Sales Amount">
            <ItemTemplate >
                <asp:Label ID="lbl_SamesAmt" runat="server" Text='<%# Eval("SalesAmt") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Diff.">
            <ItemTemplate>
                <asp:Label ID="lbl_diff" runat="server" Text='<%# Eval("Diff") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Debit/Credit Note No.">
            <ItemTemplate >
                <asp:Label ID="lbl_DCNo" runat="server" Text='<%# Eval("DCNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate >
                <asp:Label ID="lbl_Status" runat="server" Text='<%# Eval("Status") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Panel>
<br />
* Only self-billed invoice will be catered for ILS discrepancy debit/credit note.
</asp:Content>
