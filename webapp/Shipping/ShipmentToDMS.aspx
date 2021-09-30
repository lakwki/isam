<%@ Page Language="C#" Theme="DefaultTheme" MaintainScrollPositionOnPostback="true"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ShipmentToDMS.aspx.cs" Inherits="com.next.isam.webapp.shipping.ShipmentToDMS" Title="Supplier Shipping Document Reconciliation" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<%@ Register Src="~/usercontrol/UclSortingOrder.ascx" TagName="UclSortingOrder" TagPrefix="uso" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript">
    function isValidSearch() {
        if (document.all.<%= txt_DocReceiptDateFrom.ClientID %>_txt_DocReceiptDateFrom_textbox.value != '' || 
            document.all.<%= txt_ContractNo.ClientID %>.value != '' ||
            document.all.<%= txt_ItemNo.ClientID %>.value != '')
            return true;
        else
        {
            alert('Please input at least the [Shipping Doc. Receipt Date / Contract No / Item No]');
            return false;
        }
    }

    function openAttachments(o, sid)
    {
        resetHighlightEntry(o);
        highlightEntry(o);
        window.open('../account/AttachmentList.aspx?shipmentId=' + sid ,'attachmentlist','status=1,width=400,height=300');
    }       
</script>
<table width="800px">
    <tr>
        <td class="tableHeader">Shipping Document Reconciliation</td>
    </tr>
</table>
<table width="800px">
    <tr>
        <td colspan="2">
            <table>  
            <tr>
                <td colspan="4">
                    <table style="margin-left:10px;" cellpadding="5" cellspacing="0" >
                        <tr>
                            <td class="FieldLabel2">Shipping Doc. Receipt Date</td>
                            <td>
                                <cc2:SmartCalendar ID="txt_DocReceiptDateFrom" ToDateControl="txt_DocReceiptDateTo" runat="server" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_DocReceiptDateTo" FromDateControl="txt_DocReceiptDateFrom" runat="server" />
                            </td>
                            <td class="FieldLabel2">Office</td>
                            <td>
                                <cc2:SmartDropDownList ID="ddl_Office" runat="server" width="125px"/>
                            </td>              
                        </tr>
                        <tr>
                            <td class="FieldLabel2" >Contract No.</td>
                            <td>
                                <asp:TextBox ID="txt_ContractNo" runat="server"/>
                            </td>
                            <td class="FieldLabel2">Item No.</td>
                            <td>
                                <asp:TextBox ID="txt_ItemNo" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Supplier</td>
                            <td>
                                <uc1:UclSmartSelection  ID="txt_SupplierName" runat="server" width="300px"/>
                            </td>
                            <td class="FieldLabel2" >Payment Term</td>
                            <td>
                                <cc2:SmartDropDownList ID="ddl_PaymentTerm" runat="server" width="125px"/>
                            </td>          
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Supplier Doc. Check Status</td>
                            <td colspan="3">
                                <asp:RadioButton ID="radChecked" GroupName="CheckStatus" runat="server" Text="Checked" />&nbsp;<asp:RadioButton ID="radUnchecked" GroupName="CheckStatus" runat="server" Text=" Not Yet Checked" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4"><asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" />&nbsp;&nbsp;
                                <asp:Button ID="btn_Reset" runat="server" Text="Reset" OnClick="btn_Reset_Click" />
                            </td>
                        </tr>
                   </table>            
                </td>
            </tr>
            </table>        
        </td>
    </tr>
</table>

<br />
<asp:Panel ID="pnl_Result" runat="server" Visible="false" >
<asp:Label ID="lbl_Warning" runat="server" Visible="false" style="color:#ff9900; font-weight :bolder;" Text="Only the first 300 records are displayed" /><br />
<br />
&nbsp;&nbsp;
<asp:ImageButton id="imgSelectAll" runat="server" ImageUrl="../images/icon_selectall.jpg" AlternateText="Select All" OnClientClick="CheckAll('ckb_inv'); return false;" />
&nbsp;&nbsp;
<asp:ImageButton id="imgDeSelectAll" runat="server" ImageUrl="../images/icon_deselectall.jpg" AlternateText="De-Select All" OnClientClick="UncheckAll('ckb_inv'); return false;" />
&nbsp;&nbsp;
<asp:Button ID="btn_Accept" runat="server" Text="Accept" onclick="btn_Accept_Click" />
<asp:GridView ID="gv_Inv" runat="server" AutoGenerateColumns ="false" 
        OnRowDataBound="InvDataBound" onrowcommand="gv_Inv_RowCommand" 
        AllowSorting="True" onsorting="gv_Inv_Sorting">
    <Columns >
        <asp:TemplateField HeaderText="">
            <ItemTemplate >
                <asp:CheckBox ID="ckb_inv" runat="server" />&nbsp;
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supplier Name" SortExpression="SupplierName" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate >
                <asp:Label ID="lbl_SupplierName" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="">
            <ItemTemplate >
                <asp:ImageButton runat="server" CommandName="OpenAttachment" ID="imgOpen" ImageUrl="~/images/icon_edit.gif" />&nbsp;
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Contract No." SortExpression="ContractNo" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate >
                <asp:Label ID="lbl_ContractNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Item No.">
            <ItemTemplate >
                <asp:Label ID="lbl_ItemNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Customer">
            <ItemTemplate >
                <asp:Label ID="lbl_Customer" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Invoice No." SortExpression="InvoiceNo" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_invNo" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Invoice Date" SortExpression="InvoiceDate" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_invDate" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Supplier Invoice No." SortExpression="SupplierInvoiceNo" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_SupplierInvoiceNo" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Currency">
            <ItemTemplate>
                <asp:Label ID="lbl_Currency" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="QA Comm.">
            <ItemTemplate >
                <asp:Label ID="lbl_QACommissionAmt" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Payment Discount">
            <ItemTemplate >
                <asp:Label ID="lbl_VendorPaymentDiscountAmt" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Lab Test Income">
            <ItemTemplate >
                <asp:Label ID="lbl_LabTestIncome" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Net Amt" SortExpression="NetAmt" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate >
                <asp:Label ID="lbl_NetAmt" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Shipped Qty">
            <ItemTemplate >
                <asp:Label ID="lbl_ShippedQty" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText="No. Of Split">
            <ItemTemplate>
                <asp:Label ID="lbl_SplitCount" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate >
                <asp:Label ID="lbl_Status" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>    
    </asp:GridView>
</asp:Panel>
</asp:Content>
