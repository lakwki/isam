<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="OutstandingDiscountListReportPage.aspx.cs" Inherits="com.next.isam.webapp.reporter.OutstandingDiscountListReportPage" Title="Next Discount Claim Report" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Outstanding UK Discount Claim List Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="800px">
        <tr>
            <td colspan="2"><b>
                <asp:ValidationSummary ID="vs" runat="server"></asp:ValidationSummary>
            </b></td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width: 150px;">Office Group</td>
            <td>
                <cc2:SmartDropDownList ID="ddl_Office" runat="server" Width="200px" OnSelectedIndexChanged="ddl_Office_SelectedIndexChanged" AutoPostBack="true"></cc2:SmartDropDownList></td>
        </tr>
        <tr id="tr_HandlingOffice" runat="server" style="display: none;">
            <td class="FieldLabel4">Handling Office</td>
            <td>
                <cc2:SmartDropDownList ID="ddl_HandlingOffice" runat="server" Width="70px" AutoPostBack="false" /></td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width: 150px;">Supplier</td>
            <td>
                <uc1:UclSmartSelection ID="txt_Supplier" runat="server" />
            </td>
        </tr>
        <%--    <tr>
        <td class="FieldLabel4" style="width:150px;">Order Type</td>
        <td><cc2:SmartDropDownList ID="ddl_OrderType" runat="server" width="125px"/></td>
    </tr>--%>
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
            <td class="FieldLabel4">GBP Rate</td>
            <td>
                <asp:TextBox runat="server" ID="txtGBP" Text="0" Style="width: 140px;" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4">EUR Rate</td>
            <td>
                <asp:TextBox runat="server" ID="txtEUR" Text="0" Style="width: 140px;" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4">Monthly Accrual Amt</td>
            <td>USD&nbsp;<asp:TextBox runat="server" ID="txtUSDAccrual" Text="0" Style="width: 140px;" /><br />
            </td>
        </tr>
        <tr>
            <td></td>
            <td>GBP&nbsp;<asp:TextBox runat="server" ID="txtGBPAccrual" Text="0" Style="width: 140px;" /><br />
            </td>
        </tr>
        <tr>
            <td></td>
            <td>EUR&nbsp;<asp:TextBox runat="server" ID="txtEURAccrual" Text="0" Style="width: 140px;" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btn_Submit" runat="server" Text="Print"
                    SkinID="LButton" OnClick="btn_Submit_Click" />
                <asp:CustomValidator ID="valCustom" runat="server" Display="None"
                    OnServerValidate="valCustom_ServerValidate"></asp:CustomValidator>
            </td>
        </tr>
    </table>

</asp:Content>
