<%@ Page Title="ISAM - Next Claim Audit Log Report" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKClaimAuditLogReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.UKClaimAuditLogReport" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel runat="server" SkinID="sectionHeader_Report">Next Claim Audit Log Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function isFormValid() {
        if ((document.all.<%= txt_IssueDateFrom.ClientID %>_txt_IssueDateFrom_textbox.value == '' && document.all.<%= txt_IssueDateTo.ClientID %>_txt_IssueDateTo_textbox.value != '') ||
            (document.all.<%= txt_IssueDateFrom.ClientID %>_txt_IssueDateFrom_textbox.value != '' && document.all.<%= txt_IssueDateTo.ClientID %>_txt_IssueDateTo_textbox.value == '')) 
        {
            alert('Invalid Next Debit Note Date');
            return false;
        }
    }
</script>
<table>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Next D/N Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_IssueDateFrom" runat="server" FromDateControl="txt_IssueDateFrom" 
                ToDateControl="txt_IssueDateTo" /> to <cc2:SmartCalendar ID="txt_IssueDateTo" runat="server" FromDateControl="txt_IssueDateFrom"
                ToDateControl="txt_IssueDateTo" />
        </td>
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
        <td class="FieldLabel4">Supplier</td>
        <td><uc1:UclSmartSelection ID="uclSupplier" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Claim Type</td>
        <td><cc2:SmartDropDownList ID="ddl_ClaimType" runat="server" /></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Preview" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Preview_Click" OnClientClick="return isFormValid();" />
            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  OnClientClick="return isFormValid();" />
        </td>
    </tr>
</table>
</asp:Content>
