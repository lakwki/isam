<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="OutstandingBookingReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.OutstandingBookingReport" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_reports_outstanding_booking.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Outstanding Booking Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function isFormValid() {

        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_POAtWHDateFrom_txt_POAtWHDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_POAtWHDateTo_txt_POAtWHDateTo_textbox").value == "") {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Customer At-Warehouse Date\r\n");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_POAtWHDateFrom_txt_POAtWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_POAtWHDateTo_txt_POAtWHDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_POAtWHDateFrom_txt_POAtWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_POAtWHDateTo_txt_POAtWHDateTo_textbox").value == "")) {
            alert("Invalid P.O. At-Warehouse Date.");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_Customer') == 0) {
            alert("Please select one of the customers.");
            return false;
        }

        if (GetCheckBoxSelectedCount('cbl_ShipmentMethod') == 0) {
            alert("Please select one of the shipment method");
            return false;
        }

        return true;
    }

    function ddl_Office_OnChange(obj) {
        var uclPrefix = "<%= uclProductTeam.ClientID %>";
        if (obj[obj.selectedIndex].text.indexOf("+") > 0) {
            clearSmartSelection(null, uclPrefix, null);
            disableSmartSelection(uclPrefix);
        }
        else {
            enableSmartSelection(uclPrefix);
        }
    }

</script>
<table >
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:160px;">Customer At-Warehouse Date</td>
        <td><cc2:SmartCalendar ID="txt_POAtWHDateFrom" runat="server" FromDateControl="txt_POAtWHDateFrom" ToDateControl="txt_POAtWHDateTo" />
            To <cc2:SmartCalendar ID="txt_POAtWHDateTo" runat="server" FromDateControl ="txt_POAtWHDateFrom" ToDateControl="txt_POAtWHDateTo" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Office Group</td>
        <td><cc2:SmartDropDownList ID="ddl_Office" runat="server" onchange="ddl_Office_OnChange(this);" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Handling Office</td>
        <td><asp:DropDownList ID="ddl_HandlingOffice" runat="server" >
            <asp:ListItem Text="-- ALL --" Value="-1" />
            <asp:ListItem Text="DG" Value="17" />
            <asp:ListItem Text="HK" Value="1" />
            <asp:ListItem Text="SH" Value="2" />
            <asp:ListItem Text="VN" Value="16" />
        </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Product Team</td>
        <td><uc1:UclSmartSelection ID="uclProductTeam" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Supplier</td>
        <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Country of Origin</td>
        <td><cc2:SmartDropDownList ID="ddl_CO" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Loading Port</td>
        <td><cc2:SmartDropDownList ID="ddl_ShipmentPort" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Packing Method</td>
        <td><cc2:SmartDropDownList ID="ddl_PackingMethod" runat="server" /></td>
    </tr>    
    <tr>
        <td class="FieldLabel4">Term of Purchase</td>
        <td><cc2:SmartDropDownList ID="ddl_TermOfPurchase" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Order Type</td>
        <td>
            <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinId="LargeDDL">
            <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS/STUDIO SAMPLE ORDER)" Value="-1" />
            <asp:ListItem Text="MAINLINE ORDER" Selected="True" Value="0" />
            <asp:ListItem Text="MOCK SHOP/PRESS/STUDIO SAMPLE ORDER" Value="1" />
        </asp:DropDownList>
</td>
    </tr>
    <tr>
        <td class="FieldLabel4_T" valign="top">Customer</td>
        <td class="CellWithBorder">
            <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400" RepeatLayout="Table" RepeatColumns="4">
            </asp:CheckBoxList>
        </td>    
         <td>
            <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
            <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
         </td>
    </tr>
    <tr>
        <td class="FieldLabel4_T" valign="top">Shipment Method</td>
        <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_ShipmentMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400" RepeatLayout="Table"  >
            </asp:CheckBoxList>
        </td>
        <td>
            <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_ShipmentMethod'); return false;" />&nbsp;
            <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_ShipmentMethod'); return false;" />
        </td>
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
