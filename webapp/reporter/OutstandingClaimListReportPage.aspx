<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="OutstandingClaimListReportPage.aspx.cs" Inherits="com.next.isam.webapp.reporter.OutstandingClaimListReportPage" Title="O/S Next Claim Report" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Outstanding Next Claim List Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="800px">
    <tr>
        <td colspan="2"><b><asp:validationsummary id="vs" runat="server"></asp:validationsummary></b></td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Office Group</td>
        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="200px" OnSelectedIndexChanged="ddl_Office_SelectedIndexChanged" AutoPostBack="true"></cc2:smartdropdownlist></td>
    </tr>
    <tr id="tr_HandlingOffice" runat="server" style="display:none;">
        <td class="FieldLabel4">Handling Office</td>
        <td><cc2:SmartDropDownList  ID="ddl_HandlingOffice" runat="server" width="70px" AutoPostBack="false" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Supplier</td>
        <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Order Type</td>
        <td><cc2:SmartDropDownList ID="ddl_OrderType" runat="server" width="125px"/></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Cut Off Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_CutOffDate" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Report Option</td>
        <td>
            <asp:RadioButton runat="server" ID="radNormal" Text="Normal" GroupName="ReportOptionGroup" />&nbsp;
            <asp:RadioButton runat="server" ID="radOffice" Text="Office Summary (No Details)" GroupName="ReportOptionGroup" />&nbsp;
            <asp:RadioButton runat="server" ID="radSupplier" Text="Supplier Summary (No Details)" GroupName="ReportOptionGroup" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Next Claim Option</td>
        <td>
            <asp:RadioButton runat="server" ID="radNCAll" Text="All" GroupName="NCGroup" />&nbsp;
            <asp:RadioButton runat="server" ID="radNCClaim" Text="Claim" GroupName="NCGroup" />&nbsp;
            <asp:RadioButton runat="server" ID="radNCRefund" Text="Refund" GroupName="NCGroup" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Status</td>
        <td>
            <asp:CheckBox runat="server" ID="ckbExcludeCancel" />&nbsp;Exclude Cancelled & Pending For Cancellation
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">GBP Rate</td>
        <td>
            <asp:TextBox runat="server" ID="txtGBP" Text="0" style="width:140px;"/>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">EUR Rate</td>
        <td>
            <asp:TextBox runat="server" ID="txtEUR" Text="0" style="width:140px;"/>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4" valign="top">Monthly Accrual Amt</td>
        <td>
            USD&nbsp;<asp:TextBox runat="server" ID="txtUSDAccrual" Text="0" style="width:140px;"/><br />
            GBP&nbsp;<asp:TextBox runat="server" ID="txtGBPAccrual" Text="0" style="width:140px;"/><br />
            EUR&nbsp;<asp:TextBox runat="server" ID="txtEURAccrual" Text="0" style="width:140px;"/>
        </td>
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
