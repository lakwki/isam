<%@ Page Title="ISAM - Release Lock Summary" Theme="DefaultTheme"  Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ReleaseLockSummary.aspx.cs" Inherits="com.next.isam.webapp.reporter.ReleaseLockSummary" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_workplace.gif" alt="" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Release Lock Summary</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript">
    function isFormValid() {

        if (document.getElementById("<%= txt_ReleaseDateFrom.ClientID %>_txt_ReleaseDateFrom_textbox").value == ""
            || document.getElementById("<%= txt_ReleaseDateTo.ClientID %>_txt_ReleaseDateTo_textbox").value == "")
        {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Release Date Range.\r\n");
            return false;
        }
        return true;
    }
</script>

<table width="600px">
<!--
    <tr>
        <td colspan="2" class="tableHeader">Release Lock Summary</td>
    </tr>
-->
    <tr>
        <td >&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4">Office Group</td>
        <td><cc2:SmartDropDownList ID="ddl_OfficeGroup" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Handling Office</td>
        <td><asp:DropDownList ID="ddl_HandlingOffice" runat="server" >
            <asp:ListItem Text="-- ALL --" Value="-1" />
            <asp:ListItem Text="DG" Value="17" />
            <asp:ListItem Text="HK" Value="1" />
            <asp:ListItem Text="SH" Value="2" />
            <asp:ListItem Text="VN" Value="16" />
        </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4" >Release Lock Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_ReleaseDateFrom" runat="server" FromDateControl ="txt_ReleaseDateFrom" ToDateControl="txt_ReleaseDateTo" /> to <cc2:SmartCalendar 
                id="txt_ReleaseDateTo" runat="server" FromDateControl="txt_ReleaseDateFrom" ToDateControl="txt_ReleaseDateTo" />
        </td>        
    </tr>
    <tr>
        <td class="FieldLabel4">Order Type</td>
        <td>
            <asp:DropDownList ID="ddl_OrderType" runat="server"  SkinID="LDDL" >
                <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS/STUDIO SAMPLE ORDER)" Value="-1" Selected="True" />
                <asp:ListItem Text="MAINLINE ORDER" Value="0" />
                <asp:ListItem Text="MOCK SHOP/PRESS/STUDIO SAMPLE ORDER" Value="1" />
                <asp:ListItem Text="MOCK SHOP ORDER" Value="2" />
                <asp:ListItem Text="PRESS SAMPLE ORDER" Value="3" />
                <asp:ListItem Text="STUDIO SAMPLE ORDER" Value="4" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Customer</td>
        <td class="CellWithBorder" style="width:400px;">
            <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="4" Width="400px">
            </asp:CheckBoxList>
        </td>    
        <td>
            <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
            <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4" >Trading Agency</td>
        <td class="CellWithBorder"><asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="4" Width="400px">
            </asp:CheckBoxList></td>
        <td>
            <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" />&nbsp;
            <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_TradingAgency'); return false;" />
        </td>
    </tr>
    <tr>
        <td></td>
        <td class="CellWithBorder">
        <asp:RadioButtonList ID="rad_RequireType" runat="server"  RepeatDirection="Horizontal">
            <asp:ListItem Text="All&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;" Value="All" Selected="True" />
            <asp:ListItem Text="Reversing Entries&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Required" Value="ReversingEntry"/>
            <asp:ListItem Text="D/C Notes&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Required" Value="DCNote" />
            <asp:ListItem Text="ILS Temp A/C&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Required " Value="ILSTempAC" />
        </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Preview" runat="server" Text="Print Preview" SkinID="LButton" 
                onclick="btn_Preview_Click"  OnClientClick="return isFormValid();"/>&nbsp;&nbsp;
            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinId="LButton" 
                onclick="btn_Export_Click"  OnClientClick="return isFormValid();"/>
        </td>
    </tr>
</table>
</asp:Content>
