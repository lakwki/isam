<%@ Page Title="QA Commission Debit Note" Theme="DefaultTheme"  Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="QADebitNote.aspx.cs" Inherits="com.next.isam.webapp.account.QADebitNote" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">QA Commission Debit Note</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function copyNSLInvoiceNo() {
        document.getElementById("ctl00_ContentPlaceHolder1_txtNSLInvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txtNSLInvoiceNoFrom").value;
    }  
</script>
    <table width="800px">
    <tr>
        <td colspan="5">&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel2">Invoice No.</td>
        <td><asp:TextBox ID="txtNSLInvoiceNoFrom" runat="server" 
                                onblur = "copyNSLInvoiceNo();" MaxLength="14" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txtNSLInvoiceNoTo" runat="server" MaxLength="14" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Invoice Date</td>
        <td><cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />&nbsp;to&nbsp;<cc2:SmartCalendar 
                id="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Office</td>
        <td><cc2:SmartDropDownList ID="ddl_Office" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Supplier</td>
        <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Order Type</td>
        <td>
            <table>
                <tr>
                    <td><asp:RadioButton id="rbNonUT" runat="server" GroupName="orderType" Checked="true" Text="non UT order" /></td>
                    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                    <td><asp:RadioButton id="rbUTOnly" runat="server" GroupName="orderType" Text="UT order"/></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" />&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Reset" runat="server" Text="Reset" OnClick="btn_Reset_Click" />
        </td>
    </tr>
</table>
<br /><br /><br />
<asp:Panel ID="pnl_Result" runat="server" Visible="false" >
<asp:Label ID="lbl_Msg" style="color:#ff9900; font-weight :bolder;"  runat="server" /><br /><br />
<asp:Button ID="btn_Print" runat="server" Text="Print" OnClick="btn_Print_Click" />&nbsp;&nbsp;&nbsp;
<asp:LinkButton ID="lnk_SelectAll" OnClientClick="CheckAll('ckb_Inv');return false;" runat="server" Text="Select All" />&nbsp;
<asp:LinkButton ID="lnk_Deselect" OnClientClick="UncheckAll('ckb_Inv');return false;" runat="server" Text="Deselect All" /><br />

<asp:GridView id="gv_Invoice" runat="server" OnRowDataBound="gv_Invoice_RowDataBound">
    <Columns>
        <asp:TemplateField >
            <ItemTemplate >
                <asp:CheckBox ID="ckb_Inv" Checked="true"  runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText="Debit Notes No.">
            <ItemTemplate >
                <asp:Label ID="lbl_InvoiceNo" Text='<%# Eval("InvoiceNo") %>' runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supplier Name">
            <ItemTemplate >
                <asp:Label ID="lbl_Supplier" Text='<%# Eval("Vendor.Name") %>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Currency">
            <ItemTemplate >
                <asp:Label Id="lbl_Currency" Text='<%# Eval("BuyCurrency.CurrencyCode") %>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="DN Amount">
            <ItemTemplate >
                <asp:Label ID="lbl_Amount" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="DN Date">
            <ItemTemplate >
                <asp:Label ID="lbl_InvoiceDate" Text='<%# Eval("InvoiceDate", "{0:d}") %>' runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Panel>
</asp:Content>
