<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKClaimPhasingReportPage.aspx.cs" Inherits="com.next.isam.webapp.reporter.UKClaimPhasingReportPage" Title="Next Claim By Phasing Report" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Next Claim By Phasing Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="800px">
    <tr>
        <td>&nbsp;</td>
    </tr>
                      
    <tr>
        <td class="FieldLabel4">Fiscal Period</td></td>
        <td>
            &nbsp;Year&nbsp;&nbsp;<cc2:smartdropdownlist id="ddl_Year" runat="server" Width="60px" SkinId="SmallDDL" />&nbsp;&nbsp;&nbsp;
            Period From&nbsp;&nbsp;<asp:DropDownList id="ddl_PeriodFrom" runat="server"  width="30px"
                SkinID="SmallDDL" 
                onselectedindexchanged="ddl_PeriodFrom_SelectedIndexChanged" AutoPostBack="True">
                    <asp:ListItem Text="1" Value="1" selected />
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
            </asp:DropDownList>  &nbsp;To&nbsp;&nbsp;
            <asp:DropDownList ID="ddl_PeriodTo" runat="server" SkinID="SmallDDL" width="30px">
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
                    <asp:ListItem Text="12" Value="12"  selected/>
            </asp:DropDownList> 
        </td>
    </tr>
    <tr runat="server" id="trPeriod">
        <td class="FieldLabel4">Supplier</td>
        <td colspan="3"><uc1:UclSmartSelection ID="uclSupplier" runat="server" /></td>

    </tr>
                        
    <tr>
        <td class="FieldLabel4_T" style="width:150px; vertical-align:top;">Group By</td>
        <td colspan="2">
            <asp:RadioButton ID="rad_GroupByOffice" runat="server" GroupName="ReportGroupBy" Text="By Office"  OnCheckedChanged="radGroupBy_CheckedChanged" AutoPostBack="true" Checked="true"/><br />
            <asp:RadioButton ID="rad_GroupByOfficeClaimType" runat="server" GroupName="ReportGroupBy" Text="By Office By Claim Type"  OnCheckedChanged="radGroupBy_CheckedChanged" AutoPostBack="true" /><br />
            <asp:RadioButton ID="rad_GroupByOfficeClaimReason" runat="server" GroupName="ReportGroupBy" Text="By Office By Claim Reason" OnCheckedChanged="radGroupBy_CheckedChanged" AutoPostBack="true"/><br />
            <asp:RadioButton ID="rad_GroupBySupplier" runat="server" GroupName="ReportGroupBy" Text="By Supplier" OnCheckedChanged="radGroupBy_CheckedChanged" AutoPostBack="true"/><br />
            <asp:RadioButton ID="rad_GroupByProductTeam" runat="server" GroupName="ReportGroupBy" Text="By Product Team"  OnCheckedChanged="radGroupBy_CheckedChanged" AutoPostBack="true"/>
        </td>
    </tr>
    <tr id="tr_Office" runat="server" style="display:none;">
        <td class="FieldLabel4" style="width:150px;">Office</td>
        <td colspan="3"><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="100px"></cc2:smartdropdownlist>
    </td>
    </tr>

    <tr id="tr_Summary" runat="server" style="display:none;">
        <td class="FieldLabel4" style="width:150px;">Report Type</td>
        <td colspan="2" id="td_ProductTeamReportType" runat="server">
            <asp:RadioButton ID="radDetail" runat="server" GroupName="ReportType" Text="Detail"  Checked="true"/>&nbsp;&nbsp;
            <asp:RadioButton ID="radSummary" runat="server" GroupName="ReportType" Text="Summary" />
        </td>
        <td colspan="2" id="td_SupplierReportType" runat="server">
            <asp:RadioButton ID="rad_SupplierByClaimType" runat="server" GroupName="SupplierReportType" Text="By Claim Type" Checked="true"/>&nbsp;&nbsp;
            <asp:RadioButton ID="rad_SupplierByClaimReason" runat="server" GroupName="SupplierReportType" Text="By Claim Reason" />&nbsp;&nbsp;
            <asp:RadioButton ID="rad_SupplierSummary" runat="server" GroupName="SupplierReportType" Text="Summary" />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <table>
            <tr>
            <td><asp:Button ID="btn_Submit" runat="server" Text="Print" SkinID="LButton" onclick="btn_Print_Click"/></td>
            <td><asp:Button ID="btn_Export" runat="server" Text="Export" SkinID="LButton" onclick="btn_Export_Click"/></td>
            </tr>
            </table>
        </td>
    </tr>
</table>

</asp:Content>
