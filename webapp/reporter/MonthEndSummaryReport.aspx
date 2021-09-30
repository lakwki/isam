<%@ Page Language="C#"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="MonthEndSummaryReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.MonthEndSummaryReport"  Title='Month End Summary Report'  Theme="DefaultTheme"%>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!--
<table width="100%" cellpadding='0' cellspacing='0'>
<tr>
    <td class='tableHeader' style='font-size:135%;color:#ff9900;font-stretch:narrower;height:25px;' width='70px' >&nbsp;&nbsp;Report&nbsp;&nbsp;</td>
    <td class="tableHeader" style=' font-size:80%;color:Black;font-weight:bold;' >Month End Summary Report</td>
</tr>
</table>
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Month End Summary Report</asp:Panel>

<script type="text/javascript">

    function isFormValid() {

        if (GetCheckBoxSelectedCount('cbl_Office') == 0) {
            alert("Please select at least one Office.");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_TradingAgency') == 0) {
            alert("Please select at least one Trading Agency.");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_PurchaseTerm') == 0) {
            alert("Please select at least one Purchase Term.");
            return false;
        }
            
        return true;
    }

</script>


<asp:UpdatePanel ID="updatePanel3" runat="server">
    <ContentTemplate >
    
<table>
<tr>
    <td width='100px'>&nbsp;</td>
    <td >&nbsp;</td>
    <td >&nbsp;</td>
</tr>
<tr >
    <td colspan='3'>
    <asp:UpdatePanel ID="updatePanel2" runat="server">
        <ContentTemplate >            
        <table>
            <tr>
                <td class="FieldLabel4" style="width:100px;">
                    Fiscal Period
                </td>
                <td>
                    Year&nbsp;&nbsp;<asp:DropDownList  ID="ddl_Year" runat="server" SkinId="SmallDDL" />&nbsp;&nbsp;&nbsp;
                    Period &nbsp;
                    <asp:DropDownList id="ddl_PeriodNo"  SkinID="SmallDDL" runat="server" autopostback='true'  Width='20'>
                            <asp:ListItem Text="1" Value="1" />
                            <asp:ListItem Text="2" Value="2" />
                            <asp:ListItem Text="3" Value="3" />
                            <asp:ListItem Text="4" Value="4" />
                            <asp:ListItem Text="5" Value="5" />
                            <asp:ListItem Text="6" Value="6" />
                            <asp:ListItem Text="7" Value="7" />
                            <asp:ListItem Text="8" Value="8" />
                            <asp:ListItem Text="9" Value="9" />
                            <asp:ListItem Text="10" Value="10" />
                            <asp:ListItem Text="11" Value="11" />
                            <asp:ListItem Text="12" Value="12" />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel4">
                    Order Type
                </td>
                <td>
                    <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinId="LargeDDL">
                        <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS/STUDIO SAMPLE ORDER)" Value="-1" />
                        <asp:ListItem Text="MAINLINE ORDER" Selected="True" Value="0" />
                        <asp:ListItem Text="MAINLINE + PRESS SAMPLE ORDER" Value="4" />
                        <asp:ListItem Text="MOCK SHOP/PRESS/STUDIO SAMPLE ORDER" Value="1" />
                        <asp:ListItem Text="MOCK SHOP ORDER" Value="2" />
                        <asp:ListItem Text="PRESS SAMPLE ORDER" Value="3" />
                        <asp:ListItem Text="STUDIO SAMPLE ORDER" Value="5" />
                    </asp:DropDownList>
                </td>
            </tr>

        </table>

        </ContentTemplate>
        </asp:UpdatePanel> 
    </td>
</tr>

<tr style='display:none;'>
    <td class="FieldLabel4">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>    
     <td><asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_Customer'); return false;" /></td>
</tr>
<tr style='display:none;'>
    <td class="FieldLabel4">Office</td>
    <td class="CellWithBorder" style="text-align:right;">
        <asp:CheckBoxList ID="cbl_Office" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4" style="text-align:left;" />
        <asp:LinkButton ID="lnk_SelectAllOffice" runat="server" Text="Select All" OnClientClick="CheckAll('cbl_Office');" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="Lnk_DeselectAllOffice" runat="server" Text="Clear" OnClientClick="UncheckAll('cbl_Office');" />
        &nbsp;
    </td>
</tr>
<tr style='display:none;'>
    <td class="FieldLabel4">Trading Agency</td>
    <td class="CellWithBorder" style="text-align:right;">
        <asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4" style="text-align:left"/>
        <asp:LinkButton ID="lnk_SelectAllTradingAgency" runat="server" Text="Select All" OnClientClick="CheckAll('cbl_TradingAgency');" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="lnk_DeSelectAllTradingAgency" runat="server" Text="Clear" OnClientClick="UncheckAll('cbl_TradingAgency');" />
        &nbsp;
    </td>
    <td style='display:none;'>
        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" />
    </td>
</tr>
<tr style='display:none;'>
    <td class="FieldLabel4">Purchase Term</td>
    <td class="CellWithBorder" style="text-align:right;">
        <asp:CheckBoxList ID="cbl_PurchaseTerm" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4" style="text-align:left"/>
        <asp:LinkButton ID="lnk_SelectAllPurchaseTerm" runat="server" Text="Select All" OnClientClick="CheckAll('cbl_PurchaseTerm');" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="lnk_DeSelectAllPurchaseTerm" runat="server" Text="Clear" OnClientClick="UncheckAll('cbl_PurchaseTerm');" />
        &nbsp;
    </td>
    <td style='display:none;'>
        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_PurchaseTerm'); return false;" />
    </td>
</tr>
<tr style='display:none;'>
    <td class="FieldLabel4">Base Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_BaseCurrency" runat="server" /></td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>

<tr>
    <td>&nbsp;</td>
</tr>
</table>

</ContentTemplate>
</asp:UpdatePanel>
<asp:Button ID="btn_Preview" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Preview_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />

</asp:Content>
