<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="FutureOrderSummaryBySupplierReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.FutureOrderSummaryBySupplierReport" Theme="DefaultTheme" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Future Order Summary By Supplier Report</asp:Panel>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
    <br />
    <table>
        <tr>
            <td class="FieldLabel4" style="width: 146px">Office</td>
            <td style="width: 319px">
                <cc2:SmartDropDownList ID="ddlOffice" runat="server"></cc2:SmartDropDownList></td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width: 146px">Supplier</td>
            <td style="width: 350px">
                <uc1:UclSmartSelection ID="txt_Supplier" runat="server" width="300px" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width:150px;">Customer At-Warehouse Date</td>
            <td><cc2:SmartCalendar ID="txt_DateFrom" FromDateControl="txt_DateFrom" ToDateControl="txt_DateTo" runat="server" />&nbsp;To&nbsp;
                    <cc2:SmartCalendar ID="txt_DateTo" FromDateControl="txt_DateFrom" ToDateControl="txt_DateTo" runat="server" />
                <asp:CustomValidator ID="val_Custom" runat="server" Display="None" 
                    ErrorMessage="CustomValidator" onservervalidate="val_Custom_ServerValidate"></asp:CustomValidator>
            </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="btn_Submit" runat="server" Text="Export" SkinID="LButton" OnClick="btn_Submit_Click" Visible="true" />
    <div>&nbsp;</div>
    <div>
    <b>Please note orders from (William Lamb & Brand International) will be excluded from the result</b>
    </div>
</asp:Content>
