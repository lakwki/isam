<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="SalesAccrualArchiveReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.SalesAccrualArchiveReport" Title="Sales AccrualArchiveReport" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_reports_accrual_archive.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Sales Accrual Archive Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="800px">
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Office</td>
        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="200px"></cc2:smartdropdownlist>
    </td>
    </tr>
    <tr runat="server" id="trFiscalYear">
        <td class="FieldLabel4">Fiscal Year</td>
        <td>
            <cc2:smartdropdownlist id="ddl_Year" runat="server" Width="200px"></cc2:smartdropdownlist>
        </td>
    </tr>
    <tr runat="server" id="trPeriod">
        <td class="FieldLabel4">Period</td>
        <td>
            <cc2:smartdropdownlist id="ddl_Period" runat="server" Width="200px">
                <asp:ListItem Text="Period 1" Value="1" Selected="True" />
                <asp:ListItem Text="Period 2" Value="2" />
                <asp:ListItem Text="Period 3" Value="3" />
                <asp:ListItem Text="Period 4" Value="4" />
                <asp:ListItem Text="Period 5" Value="5" />
                <asp:ListItem Text="Period 6" Value="6" />
                <asp:ListItem Text="Period 7" Value="7" />
                <asp:ListItem Text="Period 8" Value="8" />
                <asp:ListItem Text="Period 9" Value="9" />
                <asp:ListItem Text="Period 10" Value="10" />
                <asp:ListItem Text="Period 11" Value="11" />
                <asp:ListItem Text="Period 12" Value="12" />
            </cc2:smartdropdownlist>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Submit" runat="server" Text="Print Preview" 
                SkinID="LButton" onclick="btn_Submit_Click"/>
            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" 
                SkinID="LButton" onclick="btn_Export_Click"/>
        </td>
    </tr>
</table>

</asp:Content>
