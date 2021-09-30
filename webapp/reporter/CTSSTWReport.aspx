<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="CTSSTWReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.CTSSTWReport" Theme="DefaultTheme"  %>
 <%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
    
    <%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
    <%@ Register Src="../usercontrol/UclSortingOrder.ascx" TagName="UclSortingOrder" TagPrefix="uso1" %>
    <%@ Register Src="~/usercontrol/UclOfficeProductTeamSelection.ascx" TagName="UclOfficeProductTeamSelection" TagPrefix="uc2"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_reports_sales_actual_accrual.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Sales Report (Actual Accrual)</asp:Panel>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type ="text/javascript" >
    function isFormValid() {
        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_STWDateFrom_txt_STWDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_STWDateTo_txt_STWDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_CTSDateFrom_txt_CTSDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_CTSDateTo_txt_CTSDateTo_textbox").value == "") {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- STW Date\r\n- CTS Date");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_STWDateFrom_txt_STWDateFrom_textbox").value != "" && document.getElementById("ctl00_ContentPlaceHolder1_txt_STWDateTo_txt_STWDateTo_textbox").value == "") ||
            (document.getElementById("ctl00_ContentPlaceHolder1_txt_STWDateTo_txt_STWDateTo_textbox").value != "" && document.getElementById("ctl00_ContentPlaceHolder1_txt_STWDateFrom_txt_STWDateFrom_textbox").value == "")) {
            alert("Invalid Stock-to-Warehouse Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_CTSDateFrom_txt_CTSDateFrom_textbox").value != "" && document.getElementById("ctl00_ContentPlaceHolder1_txt_CTSDateTo_txt_CTSDateTo_textbox").value == "") ||
             (document.getElementById("ctl00_ContentPlaceHolder1_txt_CTSDateTo_txt_CTSDateTo_textbox").value != "" && document.getElementById("ctl00_ContentPlaceHolder1_txt_CTSDateFrom_txt_CTSDateFrom_textbox").value == "")) {
            alert("Invalid CTS Date.");
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
<asp:UpdatePanel ID="updatePanel3" runat="server" >
<ContentTemplate >
<table>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td class="FieldLabel4" style="width:150px;">Stock-to-Warehouse Date</td>
    <td><cc2:SmartCalendar ID="txt_STWDateFrom" FromDateControl="txt_STWDateFrom" ToDateControl="txt_STWDateTo" runat="server" />&nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_STWDateTo" FromDateControl="txt_STWDateFrom" ToDateControl="txt_STWDateTo" runat="server" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">CTS Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_CTSDateFrom" FromDateControl="txt_CTSDateFrom" ToDateControl="txt_CTSDateTo" runat="server" />&nbsp;To&nbsp;
        <cc2:SmartCalendar ID="txt_CTSDateTo" FromDateControl="txt_CTSDateFrom" ToDateControl="txt_CTSDateTo" runat="server" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4_T" style="vertical-align:top;" rowspan="7">Order Type</td>
    <td><asp:DropDownList ID="ddl_Shipped" runat="server"  SkinID="LargeDDL">
                <asp:ListItem Value="-1">ALL (SHIPPED + NON-SHIPPED)</asp:ListItem>
                <asp:ListItem Value="0">NON-SHIPPED</asp:ListItem>
                <asp:ListItem Value="1">SHIPPED</asp:ListItem>
            </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>
         <asp:DropDownList ID="ddl_SelfBilled" runat="server" SkinID="LargeDDL">
                <asp:ListItem Value="-1">ALL (SELF BILLED + NON-SELF BILLED)</asp:ListItem>
                <asp:ListItem Value="0">NON-SELF BILLED</asp:ListItem>
                <asp:ListItem Value="1">SELF BILLED</asp:ListItem>
            </asp:DropDownList>
    </td>
</tr>
<tr>
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
        <asp:DropDownList ID="ddl_UTOrder" runat="server" SkinID="LargeDDL" >
            <asp:ListItem Text="ALL (UT + NON-UT ORDER)" Value="-1" Selected="True" />
            <asp:ListItem Text="UT ORDER" Value="1" />
            <asp:ListItem Text="NON-UT ORDER" Value="0" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ddl_LDPOrder" runat="server" SkinID="LargeDDL">
            <asp:ListItem Text="ALL (LDP + NON-LDP ORDER)" Value="-1" Selected="True" />
            <asp:ListItem Text="LDP ORDER" Value ="1" />
            <asp:ListItem Text="NON-LDP ORDER" Value ="0" />
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
    <td class="FieldLabel4">Packing Method</td>
    <td><cc2:SmartDropDownList ID="ddl_PackingMethod" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Country of Origin</td>
    <td><cc2:SmartDropDownList ID="ddl_CountryOfOrigin" runat="server" /></td>
</tr>
<tr>
    <td colspan="3">
    <table >
        <tr valign="top">
            <td>
                <uc2:UclOfficeProductTeamSelection ID="uclOfficeProductTeamSelection" runat="server" /> 
            </td>
            <td align="right">
                <asp:checkbox id="chk_ShowProductTeam" runat="server"/>
            </td>
            <td width="125px" title="It will take more time to generate the report if you check this box">
                Show the Product Team selection in the Report
            </td>
        </tr>
    </table>
    </td>
    <td>
        &nbsp;
       
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Handling Office</td>
    <td><asp:DropDownList ID="ddl_HandlingOffice" runat="server" >
        <asp:ListItem Text="-- ALL -- " Value="-1" />
        <asp:ListItem Text="DG" Value="17" />
        <asp:ListItem Text="HK" Value="1" />
        <asp:ListItem Text="SH" Value="2" />
        <asp:ListItem Text="VN" Value="16" />
    </asp:DropDownList></td>
</tr>
<tr>
    <td class="FieldLabel4">Season</td>
    <td><cc2:SmartDropDownList ID="ddl_Season" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Loading Port</td>
    <td><cc2:SmartDropDownList ID="ddl_ShipmentPort" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">OPR Type</td>
    <td><cc2:SmartDropDownList ID="ddl_OPRType" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Destination</td>
    <td><cc2:SmartDropDownList ID="ddl_Destination" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Purchase Term</td>
    <td><cc2:SmartDropDownList id="ddl_purchaseTerm" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td> 
    <td>
        <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Shipment Method</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_ShipmentMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table"  >
        </asp:CheckBoxList>
    </td>
    <td>
        <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_ShipmentMethod'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_ShipmentMethod'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Sort Order</td>
    <td class="CellWithBorder">
                <uso1:UclSortingOrder id="ucl_sortingOrder" runat="server" />
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
        <asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  OnClientClick="return isFormValid();" />
</asp:Content>
