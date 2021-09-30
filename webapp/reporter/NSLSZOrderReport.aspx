<%@ Page Title="NSL (SZ) Order Report" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="NSLSZOrderReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.NSLSZOrderReport" %>
 <%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
    <%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">NSL (SZ) Order Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function copyInvoiceNo() {
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value.toUpperCase();
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value;
    }

    function isFormValid() {
        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value == "") {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- At Warehouse Date\r\n- Invoice Date \r\n- Invoice Upload Date\r\n- Invoice Number");
            return false;
        }
        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value != "" &&
            (!isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value) ||
            !isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value))) {
            alert("Invalid invoice no.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_InvoiceUploadDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_InvoiceUploadDateTo_textbox").value != "")) {
            alert("Invalid Invoice Upload Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "")) {
            alert("Invalid Invoice Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value != "")) {
            alert("Invalid At Warehouse Date");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_Customer') == 0) {
            alert("Please select one of the customers.");
            return false;
        }

        if (GetCheckBoxSelectedCount('cbl_ShipmentMethod') == 0) {
            alert("Please select one of the shipment method.");
            return false;
        }

        return true;
    }
</script>
<table>    
<!--
    <tr>
        <td colspan="2" class="tableHeader">NSL (SZ) Order Report</td>
    </tr>
-->
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4">At Warehouse Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_AtWHDateFrom" runat="server" FromDateControl="txt_AtWHDateFrom" 
                ToDateControl="txt_AtWHDateTo" /> to <cc2:SmartCalendar ID="txt_AtWHDateTo" runat="server" FromDateControl="txt_AtWHDateFrom"
                ToDateControl="txt_AtWHDateTo" />
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
        <td class="FieldLabel4" style="width:130px;">Invoice Upload Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_InvoiceUploadDateFrom" runat="server" FromDateControl="txt_InvoiceUploadDateFrom"
                ToDateControl="txt_InvoiceUploadDateTo" /> to <cc2:SmartCalendar ID="txt_InvoiceUploadDateTo" runat="server" FromDateControl="txt_InvoiceUploadDateFrom"
                ToDateControl="txt_InvoiceUploadDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Invoice Number</td>
        <td><asp:TextBox ID="txt_InvoiceNoFrom" runat="server" onblur="copyInvoiceNo();" />&nbsp;To&nbsp;<asp:TextBox ID="txt_InvoiceNoTo" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Office</td>
        <td><cc2:SmartDropDownList ID="ddl_Office" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Product Team</td>
        <td><uc1:UclSmartSelection ID="uclProductTeam" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4_T" rowspan="5" style="vertical-align:top ;">Order Type</td>
        <td>
        <asp:DropDownList ID="ddl_ShipmentStatus" runat="server" SkinID="LargeDDL" >
                <asp:ListItem Text="ALL (SHIPPED + NON-SHIPPED)" Value="-1" Selected="True" />
                <asp:ListItem Text="SHIPPED" Value="1" />
                <asp:ListItem Text="NON-SHIPPED" Value="0" />
            </asp:DropDownList>            
        </td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddl_SZOrder" runat="server" SkinID="LargeDDL" >
                <asp:ListItem Text="ALL (SZ + UT ORDER)" Value="-1" Selected="True" />
                <asp:ListItem Text="SZ ORDER" Value ="1" />
                <asp:ListItem Text="UT ORDER" Value="2" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td><asp:DropDownList ID="ddl_DualSourcingOrder" runat="server" SkinID="LargeDDL">
            <asp:ListItem Text ="ALL (DUAL SOURCING+ NON-DUAL SOURCING)" Value="-1" Selected="True" />
            <asp:ListItem Text="DUAL SOURCING ORDER" Value ="1" />
            <asp:ListItem Text="NON-DUAL SOURCING ORDER" Value="0" />
        </asp:DropDownList></td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddl_OPROrder" runat="server" SkinID="LargeDDL">
                <asp:ListItem Text="ALL (OPR + NON-OPR ORDER)" Value="-1" Selected="True" />
                <asp:ListItem Text="OPR ORDER" Value="1" />
                <asp:ListItem Text="NON-OPR ORDER" Value="0" />
            </asp:DropDownList>
        </td>        
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddl_LDPOrder" runat="server" SkinID="LargeDDL">
                <asp:ListItem  Text="ALL (LDP + NON-LDP ORDER)" Value="-1" Selected="True" />
                <asp:ListItem Text="LDP ORDER" Value="1" />
                <asp:ListItem Text="NON-LDP ORDER" Value="0" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Destination</td>
        <td><cc2:SmartDropDownList ID="ddl_Destination" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Loading Port</td>
        <td><cc2:SmartDropDownList ID="ddl_LoadingPort" runat ="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Packing Method</td>
        <td><cc2:SmartDropDownList ID="ddl_PackingMethod" runat="server" /></td>
    </tr>
    <tr>
    <td class="FieldLabel4">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>    
    <td><asp:ImageButton ID="btn_clear1" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_Customer'); return false;" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Shipment Method</td>
    <td class="CellWithBorder">
    <asp:CheckBoxList ID="cbl_ShipmentMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400" RepeatLayout="Table"  >
        </asp:CheckBoxList>
    </td>
    <td><asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_ShipmentMethod'); return false;" /></td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
            <asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  OnClientClick="return isFormValid();" />
        </td>
    </tr>
</table>
</asp:Content>
