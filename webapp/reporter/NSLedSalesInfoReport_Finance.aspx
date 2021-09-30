<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="NSLedSalesInfoReport_Finance.aspx.cs" Inherits="com.next.isam.webapp.reporter.NSLedSalesInfoReport_Finance" Theme="DefaultTheme"  %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../usercontrol/UclOfficeSelection.ascx" tagname="UclOfficeSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">NS-LED Weekly Sales Information Report (Finance)</asp:Panel> 
<br/>
<table>
    <tr>
        <td style="width: 80%;" colspan="2"><uc1:UclOfficeSelection ID="uclOfficeSelection" runat="server"/></td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:110px;">&nbsp;Week&nbsp;</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_Year" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_Year_SelectedIndexChanged"></cc2:SmartDropDownList>
            <cc2:SmartDropDownList ID="ddl_Week" runat="server" AutoPostBack="True"></cc2:SmartDropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:110px;">&nbsp;Duty&nbsp;</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_Duty" runat="server"></cc2:SmartDropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:110px;">&nbsp;Phasing&nbsp;</td>
        <td>
            Starting from&nbsp;<cc2:SmartDropDownList ID="ddl_Phase" runat="server" AutoPostBack="True"></cc2:SmartDropDownList>&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="ckb_UnknownSeason" runat="server" Checked="true" Text="Unknown Season" />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:CheckBox ID="ckb_StillSelling" runat="server" Checked="true" Text="Still Selling"/>&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="ckb_NotYetLaunched" runat="server" Checked="true" Text="Not Yet Launched"/>&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="ckb_EndOfLife" runat="server" Checked="true" Text="End of Life" />&nbsp;&nbsp;&nbsp;&nbsp;
        </td>
    </tr>
</table>
<br/>
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  />
<asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" Visible="false"/>
</asp:Content>
