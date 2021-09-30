<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="CarbonFootprintReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.CarbonFootprintReport" Title="Carbon Footprint Report" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Carbon Footprint Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="800px">
    <tr>
        <td colspan="2"><b><asp:validationsummary id="vs" runat="server"></asp:validationsummary></b></td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Office</td>
        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="200px"></cc2:smartdropdownlist></td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Fiscal Year</td>
        <td><cc2:smartdropdownlist id="ddl_FiscalYear" runat="server" Width="200px"></cc2:smartdropdownlist></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Period</td>
        <td><cc2:SmartDropDownList  ID="ddl_Period" runat="server" width="70px" /></td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Submit" runat="server" Text="Print" 
                SkinID="LButton" onclick="btn_Submit_Click"/>
            <asp:CustomValidator ID="valCustom" runat="server" Display="None" 
                onservervalidate="valCustom_ServerValidate" ></asp:CustomValidator>
        </td>
    </tr>
</table>

</asp:Content>
