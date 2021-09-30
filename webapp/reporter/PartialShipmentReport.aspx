<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="PartialShipmentReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.PartialShipmentReport" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<%@ Register Src="~/usercontrol/UclSortingOrder.ascx" TagName="UclSortingOrder" TagPrefix="uso1" %>
<%@ Register Src="~/usercontrol/UclOfficeProductTeamSelection.ascx" TagName="UclOfficeProductTeamSelection" TagPrefix="uc2" %>
<%@ Register Src="~/usercontrol/UclMultipleSelection.ascx" TagName="UclMultipleSelection" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_reports_shipment_forecast.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Shipment Forecast Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function isFormValid() {

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value == "") &&
            (document.getElementById("ctl00_ContentPlaceHolder1_txt_BookInWHDateFrom_txt_BookInWHDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_BookInWHDateTo_txt_BookInWHDateTo_textbox").value == "")) {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Customer At-Warehouse Date\r\n" +
                "- Book In-Warehouse Date\r\n");
            return false;
        }
        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value == "")) {
            alert("Invalid At-Warehouse Date.");
            return false;
        }
        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_BookInWHDateFrom_txt_BookInWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_BookInWHDateTo_txt_BookInWHDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_BookInWHDateFrom_txt_BookInWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_BookInWHDateTo_txt_BookInWHDateTo_textbox").value == "")) {
            alert("Invalid Book In-Warehouse Date.");
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

</script>
<asp:UpdatePanel ID="updatePanel3" runat="server" >
<ContentTemplate >
<table>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Customer At-Warehouse Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_AtWHDateFrom" runat="server" FromDateControl="txt_AtWHDateFrom" ToDateControl="txt_AtWHDateTo" /> to 
            <cc2:SmartCalendar ID="txt_AtWHDateTo" runat="server"  FromDateControl="txt_AtWHDateFrom" ToDateControl="txt_AtWHDateTo" />
        </td>        
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Book In-Warehouse Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_BookInWHDateFrom" runat="server" FromDateControl="txt_BookInWHDateFrom" ToDateControl="txt_BookInWHDateTo" /> to 
            <cc2:SmartCalendar ID="txt_BookInWHDateTo" runat="server"  FromDateControl="txt_BookInWHDateFrom" ToDateControl="txt_BookInWHDateTo" />
        </td>        
    </tr>
    <tr>
        <td colspan="3">
            <uc2:UclOfficeProductTeamSelection ID="uclOfficeProductTeamSelection" runat="server" />
        </td>
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
        <td class="FieldLabel4">Supplier</td>
        <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4_T"  rowspan="6" style="vertical-align:top;">Order Type</td>
        <td>
        <asp:DropDownList ID="ddl_SZOrder" runat="server" SkinID="LargeDDL">
                <asp:ListItem Value="-1">ALL (NSL SZ ORDER + NON-NSL SZ ORDER)</asp:ListItem>
                <asp:ListItem Value="0">NON-NSL SZ ORDER</asp:ListItem>
                <asp:ListItem Value="1">NSL SZ ORDER</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
        <asp:DropDownList ID="ddl_DualSource" runat="server" SkinID="LargeDDL">
                <asp:ListItem Value="-1">ALL (DUAL SOURCING + NON-DUAL SOURCING)</asp:ListItem>
                <asp:ListItem Value="0">NON-DUAL SOURCING</asp:ListItem>
                <asp:ListItem Value="1">DUAL SOURCING</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddl_LDPOrder" runat="server" SkinID="LargeDDL" >
                <asp:ListItem Value="-1">ALL (LDP + NON-LDP ORDER)</asp:ListItem>
                <asp:ListItem Value="1">LDP ORDER</asp:ListItem>
                <asp:ListItem Value="0">NON-LDP ORDER</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinId="LargeDDL">
                <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS/STUDIO SAMPLE ORDER)" Value="-1" />
                <asp:ListItem Text="MAINLINE ORDER" Selected="True" Value="0" />
                <asp:ListItem Text="MOCK SHOP/PRESS/STUDIO SAMPLE ORDER" Value="1" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddl_DFOrder" runat="server" SkinID="LargeDDL" >
                <asp:ListItem Value="-1">ALL (DF + NON-DF ORDER)</asp:ListItem>
                <asp:ListItem Value="1">DF ORDER</asp:ListItem>
                <asp:ListItem Value="0">NON-DF ORDER</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddl_UTOrder" runat="server" SkinID="LargeDDL" >
                <asp:ListItem Value="-1">ALL (UT + NON-UT ORDER)</asp:ListItem>
                <asp:ListItem Value="1">UT ORDER</asp:ListItem>
                <asp:ListItem Value="0">NON-UT ORDER</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>

    <tr>
        <td class="FieldLabel4">Purchase Term</td>
        <td><cc2:SmartDropDownList ID="ddl_termOfPurchase" runat="server" Width="180px" /></td>
    </tr>
    <tr style="display:none;">
        <td class="FieldLabel4">Country of Origin</td>
        <td><cc2:SmartDropDownList ID="ddl_CO" runat="server" Width="180px" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Season</td>
        <td><cc2:SmartDropDownList ID="ddl_Season" runat="server" Width="180px" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4_T" style="vertical-align:top;">Country of Origin</td>
        <td>        
            <uc3:UclMultipleSelection ID="uclCountryOfOrigin" runat="server"/>
        </td>
    </tr>
    <tr style="display:none;">
        <td class="FieldLabel4">Loading Port</td>
        <td><cc2:SmartDropDownList ID="ddl_ShipmentPort" runat="server" Width="180px" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4_T" style="vertical-align:top;">Loading Port</td>
        <td><uc3:UclMultipleSelection ID="uclLoadingPort" runat="server" /></td>
    </tr>
    <tr style="display:none;">
        <td class="FieldLabel4">Destination</td>
        <td><cc2:SmartDropDownList ID="ddl_Destination" runat="server" Width="180px" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4_T" style="vertical-align:top;">Destination</td>
        <td><uc3:UclMultipleSelection ID="uclDestination" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">OPR Type</td>
        <td><cc2:SmartDropDownList ID="ddl_OPRFabricType" runat ="server" Width="180px"/></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Packing Method</td>
        <td><cc2:SmartDropDownList ID="ddl_PackingMethod" runat="server" Width="180px" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Payment Term</td>
        <td><cc2:SmartDropDownList ID="ddl_PaymentTerm" runat="server" Width="180px" /></td>
    </tr>
    <tr>
    <td class="FieldLabel4_T" valign="top">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="550" RepeatLayout="Table" RepeatColumns="4">
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
    <asp:CheckBoxList ID="cbl_ShipmentMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="550" RepeatLayout="Table"  >
        </asp:CheckBoxList>
    </td>
    <td>
        <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_ShipmentMethod'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_ShipmentMethod'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4" style="vertical-align:top; background-position : top;">Sort By</td>
    <td class="CellWithBorder">
        <uso1:UclSortingOrder ID="ucl_SortingOrder1" runat="server" />       
    </td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td colspan="2">   
    </td>
</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
<asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
</asp:Content>
