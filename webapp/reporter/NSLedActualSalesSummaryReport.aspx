<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="NSLedActualSalesSummaryReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.NSLedActualSalesSummaryReport" Theme="DefaultTheme"  %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../usercontrol/UclOfficeSelection.ascx" tagname="UclOfficeSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">NS-LED Actual Sales Summary Report</asp:Panel> 
<br/>
<table>
    <tr>
        <td class="FieldLabel2" style="width:110px;">Dept</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_Dept" runat="server"></cc2:SmartDropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:110px;">&nbsp;Period as of&nbsp;</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_Year" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_Year_SelectedIndexChanged"></cc2:SmartDropDownList>
            <cc2:SmartDropDownList ID="ddl_Week" runat="server" AutoPostBack="True"></cc2:SmartDropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:110px;">&nbsp;Phasing&nbsp;</td>
        <td>
            Starting from&nbsp;<cc2:SmartDropDownList ID="ddl_Phase" runat="server" AutoPostBack="True"></cc2:SmartDropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:110px;"></td>
        <td>
            <asp:CheckBox ID="ckb_StillSelling" runat="server" Checked="true" Text="Still Selling" />
        </td>
    </tr>
</table>
<br/>
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  />
<asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" Visible="false"/>
</asp:Content>
