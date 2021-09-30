<%@ Page Title="Outstanding Payment Document to A/C Report" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="OutstandingPaymentReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.OutstandingPaymentReport" %>
 <%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
    <%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Outstanding Payment Document To A/C Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function isFormValid() {

        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_ShipReceiptDateFrom_txt_ShipReceiptDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_ShipReceiptDateTo_txt_ShipReceiptDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateFrom_txt_StockToWHDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateTo_txt_StockToWHDateTo_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "") {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Date of Shipping Receipt\r\n- Stock at Warehouse Date\r\n- Invoice Date");
            return false;
        }
        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_ShipReceiptDateFrom_txt_ShipReceiptDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_ShipReceiptDateTo_txt_ShipReceiptDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_ShipReceiptDateFrom_txt_ShipReceiptDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_ShipReceiptDateTo_txt_ShipReceiptDateTo_textbox").value != "")) {
            alert("Invalid Shipping Receipt Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "")) {
            alert("Invalid Invoice Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateFrom_txt_StockToWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateTo_txt_StockToWHDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateFrom_txt_StockToWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateTo_txt_StockToWHDateTo_textbox").value != "")) {
            alert("Invalid Stock to Warehouse Date");
            return false;
        }

        if (GetCheckBoxSelectedCount('cbl_ShippingUser') == 0) {
            alert("Please select one of the shipping users.");
            return false;
        }
            
        return true;
    }

</script>
<table >
<!--
<tr>
    <td colspan="2" class="tableHeader">Outstanding Payment Document to A/C Report</td>
</tr>
-->
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td class="FieldLabel4" style="width:160px;">Date of Shipping Receipt</td>
    <td><cc2:SmartCalendar ID="txt_ShipReceiptDateFrom" runat="server" FromDateControl="txt_ShipReceiptDateFrom" 
        ToDateControl="txt_ShipReceiptDateTo" /> to <cc2:SmartCalendar ID="txt_ShipReceiptDateTo" runat="server"  FromDateControl="txt_ShipReceiptDateFrom"
        ToDateControl="txt_ShipReceiptDateTo" />
     </td>
</tr>
<tr>
    <td class="FieldLabel4">Stock to Warehouse Date</td>
    <td><cc2:SmartCalendar ID="txt_StockToWHDateFrom" runat="server" FromDateControl="txt_StockToWHDateFrom" 
        ToDateControl="txt_StockToWHDateTo" /> to <cc2:SmartCalendar ID="txt_StockToWHDateTo" runat="server" 
        FromDateControl="txt_StockToWHDateFrom" ToDateControl="txt_StockToWHDateTo" />
     </td>
</tr>
<tr>
    <td class="FieldLabel4">Invoice Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" /> to <cc2:SmartCalendar 
        id="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Office Group</td>
    <td><cc2:SmartDropDownList ID="ddl_OfficeGroup" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Supplier</td>
    <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Payment Term</td>
    <td><cc2:SmartDropDownList ID="ddl_PaymentTerm" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Purchase Term</td>
    <td><cc2:SmartDropDownList ID="ddl_PurchaseTerm" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4_T" style="width:150px; vertical-align:top;">Shipping User</td>
    <td style="border: 1px solid #C0C0C0">
        <asp:CheckBoxList ID="cbl_ShippingUser" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="800" RepeatLayout="Table" RepeatColumns="5">
        </asp:CheckBoxList>
    </td>
    <td>
        <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_ShippingUser'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_ShippingUser'); return false;" />
    </td>    
</tr>
<tr>
    <td class="FieldLabel4">Order Type<br/></td>
    <td>
        <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinId="LargeDDL">
            <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS SAMPLE ORDER)" Value="-1" />
            <asp:ListItem Text="MAINLINE ORDER" Selected="True" Value="0" />
            <asp:ListItem Text="MOCK SHOP/PRESS SAMPLE ORDER" Value="1" />
        </asp:DropDownList>
        <br/>
        <asp:DropDownList ID="ddl_UTOrder" runat="server" SkinID="LargeDDL" >
            <asp:ListItem Text="All (UT + Non-UT Order)" Value="-1" Selected="True" />
            <asp:ListItem Text="UT Order" Value="1" />
            <asp:ListItem Text="Non-UT Order" Value="0" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Document upload in DMS</td>
    <td>
        <asp:DropDownList ID="ddlDMS" runat="server">
            <asp:ListItem Text="All" Value="-1" />
            <asp:ListItem Text="Yes" Value="1" />
            <asp:ListItem Text="No" Value="0" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td colspan="2">
         
<asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
    </td>
</tr>
</table>
</asp:Content>
