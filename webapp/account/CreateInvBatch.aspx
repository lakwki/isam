<%@ Page Language="C#" Theme="DefaultTheme" MaintainScrollPositionOnPostback="true" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="CreateInvBatch.aspx.cs" Inherits="com.next.isam.webapp.account.CreateInvBatch" Title="Create Invoice Batch" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc1" %>
    <%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="swc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Create Invoice Batch</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
function UpdateCount(obj)
{
    if (obj.checked)
    {
        document.getElementById("txt_NoOfSelected").value = 1 + parseInt(document.getElementById("txt_NoOfSelected").value);                            
    }
    else
    {
        document.getElementById("txt_NoOfSelected").value =  parseInt(document.getElementById("txt_NoOfSelected").value) -1;        
    }
}

function Reset()
{
        document.getElementById("txt_NoOfSelected").value = 0;
}
function ShowMax()
{
        document.getElementById("txt_NoOfSelected").value = GetCheckBoxSelectedCount("ckb_Inv");
}

function isFormValid()
{
    if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNo").value != "" && !isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNo").value))
        return false;
        
    return true
}
</script>
<table width="100%">
    <tr>
        <td>&nbsp;</td>
    </tr>    
</table>
<table class="pnl">
    <tr>
        <td colspan="8" class="header2">Search Criteria</td>
    </tr>
    <tr>
        <td class="FieldLabel2">Office</td>
        <td><swc:SmartDropDownList ID="ddl_Office" runat="server" Width="50px"/></td>
        <td class="FieldLabel2" style="width:80px;">Trading Agency</td>
        <td><swc:SmartDropDownList ID="ddl_TradingAgency" runat="server" width="65px"/></td>
        <td class="FieldLabel2">Order Type</td>
        <td><swc:SmartDropDownList ID="ddl_OrderType" runat="server" Width="90px" /></td>
        <td class="FieldLabel2">Currency</td>
        <td><swc:SmartDropDownList ID="ddl_Currency" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:80px;">Registered Date</td>
        <td colspan="3"><swc:SmartCalendar id="txt_RegDateFrom" ToDateControl="txt_RegDateTo" runat="server" />&nbsp;To&nbsp;<swc:SmartCalendar 
                id="txt_RegDateTo" FromDateControl="txt_RegDateFrom" runat="server" />
        </td>
        <td class="FieldLabel2" style="width:80px;">Submitted Date</td>
        <td colspan="3"><swc:SmartCalendar ID="txt_SubmitDateFrom" ToDateControl="txt_SubmitDateTo" runat="server" />&nbsp;To&nbsp;<swc:SmartCalendar 
                id="txt_SubmitDateTo" FromDateControl="txt_SubmitDateTo" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Batch No.</td>
        <td colspan="3"><asp:TextBox ID="txt_BatchNo" MaxLength="20" runat="server" SkinID="TextBoxLarge"/></td>
        <td class="FieldLabel2">Invoice No.</td>
        <td colspan="3"><asp:TextBox ID="txt_InvoiceNo" MaxLength="14" runat="server" SkinID="TextBoxLarge"/></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Invoice Status</td>
        <td colspan="7"><asp:RadioButton ID="rad_All" runat="server" Text="All" GroupName="InvoiceStatus" Checked="true"  />&nbsp;
                <asp:RadioButton ID="rad_Registered" runat="server" Text="Registered" GroupName="InvoiceStatus" />&nbsp;
                <asp:RadioButton ID="rad_Sent" runat="server" Text="Sent to NUK" GroupName="InvoiceStatus" />&nbsp;
        </td>
    </tr>
    <tr>
        <td colspan="1"><asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" OnClientClick="if (!isFormValid()) {alert('Please enter a valid invoice number.');return false;}" /></td>
        <td colspan="7"><asp:Button ID="btn_Reset" runat="server" Text="Reset" OnClick ="btn_Reset_Click" /></td>
    </tr>
</table>
<br />

<table>
    <tr >
        <td class="FieldLabel2" style="width:110px;height:35px;">Total No. of Invoices</td>
        <td><asp:Textbox ID="txt_NoOfInv" runat="server" Text="0" SkinID="TextBox_50" ReadOnly="true" /></td>
        <td>&nbsp;&nbsp;&nbsp;</td>
        <td class="FieldLabel2" style="width:120px;height:35px;">No. of Invoices Selected</td>
        <td><input type="text" id="txt_NoOfSelected" value="0" readonly="readonly" 
                size="10" /></td>
    </tr>    
</table>

<br />
<asp:Panel ID="pnl_result" runat="server" Visible="false" >
<asp:Button ID="btn_Submit" runat="server" Text="Submit" OnClick="btn_Submit_Click" />&nbsp;&nbsp;&nbsp;
<asp:Button ID="btn_Print" runat ="server" Text="Print" />&nbsp;&nbsp;&nbsp;
<asp:Button ID="btn_Modify" runat="server" Text="Modify" OnClick="btn_Modify_Click" />&nbsp;&nbsp;&nbsp;
<asp:Button ID="btn_ReGen" runat="server" Text="Re-Generate" OnClick="btn_ReGen_Click" Visible ="false"  />&nbsp;&nbsp;&nbsp;
<asp:Button ID="btn_Cancel" runat="server" Text="Cancel" OnClick="btn_Cancel_Click" Visible ="false"  />&nbsp;&nbsp;&nbsp;
<br /><br />
<a href="#" onclick="CheckAll('ckb_Inv'); ShowMax(); return false;">Select All</a>&nbsp;&nbsp;&nbsp;
<asp:LinkButton ID="lnk_DeselectAll" runat="server" Text="Deselect All" OnClientClick="UncheckAll('ckb_Inv'); Reset(); return false;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<span style="font-weight:bolder;">Remark      :</span>&nbsp;&nbsp;&nbsp;
<span style="font-weight:bolder;color:Red;">M</span> - Mock Shop Sample &nbsp;&nbsp;&nbsp;
<span style="font-weight:bolder;color:Red;">P</span> - Press Sample
<asp:GridView ID="gv_Inv" runat="server" AutoGenerateColumns="false" AllowPaging="False" OnRowDataBound="InvDataBound" OnRowDeleting="InvoiceRowDelete">        
    <Columns>
        <asp:TemplateField  Visible="false">
            <ItemTemplate>
                <asp:LinkButton ID="lnk_Delete" runat="server" Text="Delete" CommandName="Delete" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField >
            <ItemTemplate>
                <asp:CheckBox ID="ckb_Inv" onclick="UpdateCount(this);" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Office">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" Text='<%# Eval("Office.OfficeCode") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Invoice No." >
            <ItemTemplate>
                <asp:Label ID="lbl_InvNo" runat="server" Text='<%# Eval("InvoiceNo") %>' />&nbsp;&nbsp;<asp:Label ID="lbl_Remark" 
                runat="server" SkinID="AlertLabel" Font-Bold="true" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Invoice Date">
            <ItemTemplate >
                <asp:Label ID="lbl_InvoiceDate" runat="server" Text='<%# Eval("InvoiceDate","{0:d}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Registered Date">
            <ItemTemplate >
                <asp:Label ID="lbl_ScannedDate" runat="server" Text='<%# Eval("SalesScanDate", "{0:d}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supplier">
            <ItemTemplate >
                <asp:Label ID="lbl_Supplier" runat="server" Text='<%# Eval("Vendor.Name") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supp. Inv. No.">
            <ItemTemplate >
                <asp:Label ID="lbl_SuppInvNo" runat="server" Text='<%# Eval("SupplierInvoiceNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Contract No.">
            <ItemTemplate >
                <asp:Label ID="lbl_ContractNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ccy">
            <ItemTemplate >
                <asp:Label ID="lbl_Ccy" runat="server" Text='<%# Eval("SellCurrency.CurrencyCode") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Inv. Amount">
            <ItemTemplate >
                <asp:Label ID="lbl_InvAmt" runat="server" Text='<%# Eval("InvoiceAmount", "{0:#,##0.00}") %>' />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Batch No.">
            <ItemTemplate >
                <asp:Label ID="lbl_BatchNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Submitted Date">
            <ItemTemplate >
                <asp:Label ID="lbl_SubmitDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate >
                <asp:Label ID="lbl_Status" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate >Record not found.</EmptyDataTemplate>
</asp:GridView>
</asp:Panel>
</asp:Content>
