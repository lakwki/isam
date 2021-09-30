<%@ Page Title="ISAM - Next Claim Summary Report" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKClaimSummaryReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.UKClaimSummaryReport" %>
 <%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
    <%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel runat="server" SkinID="sectionHeader_Report">Next Claim Summary Report</asp:Panel>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function isFormValid() {

        //if (document.getElementById("ctl00_ContentPlaceHolder1_txt_IssueDateFrom_txt_IssueDateFrom_textbox").value == "" &&
        //     document.getElementById("ctl00_ContentPlaceHolder1_txt_IssueDateTo_txt_IssueDateTo_textbox").value == "" && 
        //     document.getElementById("ctl00_ContentPlaceHolder1_uclSupplier_txtName").value == "") {
        //    alert("Please enter search criteria on one of below search criteria.\r\n" +
        //        "- Issue Date\r\n- Supplier");
        //    return false;
        //}
        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_IssueDateFrom_txt_IssueDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_IssueDateTo_txt_IssueDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_IssueDateFrom_txt_IssueDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_IssueDateTo_txt_IssueDateTo_textbox").value == "")) {
            alert("Invalid Next Debit Note Date");
            return false;
        }
    }
    
</script>
<table>
    <tr>
        <td>&nbsp;</td>
    </tr>
                     
            <tr>
            <td class="FieldLabel4_T" style="width:150px; vertical-align:top;">Next D/N Date</td>
                <td colspan="2">
                    <asp:UpdatePanel ID="updatePanel2" runat="server">
                        <ContentTemplate >            
                        <table>
                            <tr>
                                <td></td>
                                <td><asp:RadioButton ID="rad_IssueDate" runat="server" GroupName="rad_SelectCriteria" AutoPostBack="true" Checked="true" Text="Date Range : &nbsp;" OnCheckedChanged="rad_IssueDate_CheckedChanged" /></td>
                                <td>
                                    <cc2:SmartCalendar ID="txt_IssueDateFrom" runat="server" FromDateControl="txt_IssueDateFrom" ToDateControl="txt_IssueDateTo" /> to 
                                    <cc2:SmartCalendar ID="txt_IssueDateTo" runat="server" FromDateControl="txt_IssueDateFrom" ToDateControl="txt_IssueDateTo" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td><asp:RadioButton ID="rad_FiscalPeriod" runat="server" GroupName="rad_SelectCriteria" AutoPostBack="true" Text="Fiscal Period : " OnCheckedChanged="rad_FiscalPeriod_CheckedChanged" /></td>
                                <td>
                                    &nbsp;Year&nbsp;&nbsp;<cc2:SmartDropDownList  ID="ddl_Year" runat="server" SkinId="SmallDDL" Enabled="false" />&nbsp;&nbsp;&nbsp;
                                    Period From&nbsp;&nbsp;<asp:DropDownList id="ddl_PeriodFrom" runat="server"  width="30px"
                                        SkinID="SmallDDL" Enabled="false" 
                                        onselectedindexchanged="ddl_PeriodFrom_SelectedIndexChanged" AutoPostBack="True">
                                    <asp:ListItem Text="1" Value="1"  selected/>
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
                                    <asp:DropDownList ID="ddl_PeriodTo" runat="server" SkinID="SmallDDL" Enabled="false" width="30px">
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
                                            <asp:ListItem Text="12" Value="12" selected/>
                                    </asp:DropDownList> 
                                </td>
                            </tr>
                        </table>
                  </ContentTemplate>
                  <Triggers >
                     <asp:AsyncPostBackTrigger ControlID="rad_IssueDate" 
                        EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="rad_FiscalPeriod" 
                        EventName="CheckedChanged" />
                  </Triggers>
                </asp:UpdatePanel> 
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
        <td class="FieldLabel4">Claim Reason</td>
        <td><cc2:SmartDropDownList ID="ddl_ClaimReason" runat="server" /></td>
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
