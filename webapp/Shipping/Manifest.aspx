<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="Manifest.aspx.cs" Inherits="com.next.isam.webapp.shipping.Manifest" Title="AWB" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_shipping_manifest.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Shipping">Container Manifest</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function validateSearchCriteria() {
        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_VoyageNo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_DepartDate").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_ContractNo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_VesselName").value == "" ) {
            alert("Please enter search criteria on one of below search criteria.\r\n- Voyage Number\r\n" +
                "- Departure Date\r\n- Contract Number\r\n- Vessel Name");
            return false;
        }

        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_DepartDate").value != "" &&
            !isDateValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_DepartDate").value)) {
            alert("Invalid date format.");
           return false;
        }
        
        return true;
    }
</script>
    <table width="800px" cellspacing="2" cellpadding="2">
    <tr>
        <td class="FieldLabel2">Voyage Number</td>
        <td><asp:TextBox ID="txt_VoyageNo" runat="server" /></td>
        <td class="FieldLabel2">Departure Date</td>
        <td><asp:TextBox ID="txt_DepartDate" runat="server" onblur="formatDateString(this);" /><asp:Image ID="btn_DepartDate" runat="server" ImageUrl="~/images/calendar.gif"
                ImageAlign="Middle" />
             <cc1:CalendarExtender ID="ce_DepartDate" TargetControlID="txt_DepartDate" runat="server" Format="dd/MM/yyyy"
                                 PopupButtonID="btn_DepartDate"    FirstDayOfWeek="Sunday" />
        </td>
        <td class="FieldLabel2">Contract No.</td>
        <td><asp:TextBox ID="txt_ContractNo" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Departure Location</td>
        <td><cc2:SmartDropDownList ID="ddl_DepartLoc" runat="server" />
        </td>
        <td class="FieldLabel2">Vessel Name</td>
        <td><asp:TextBox ID="txt_VesselName" runat="server" /></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="6">
            <asp:Button ID="btn_Search" runat="server" Text="Search" 
                onclick="btn_Search_Click" OnClientClick="return validateSearchCriteria();" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Reset" runat="server" Text="Reset" 
                onclick="btn_Reset_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Print" runat="server" Text="Print Preview"  skinid="LButton"
                onclick="btn_Print_Click" OnClientClick="return validateSearchCriteria();" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel"  skinid="LButton"
                onclick="btn_ExportReport_Click" OnClientClick="return validateSearchCriteria();" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_ExportList" runat="server" Text="Export List to Excel" style="display:none;" skinid="LButton" visible="false"
                onclick="btn_ExportList_Click" OnClientClick="return validateSearchCriteria();" />&nbsp;&nbsp;&nbsp;&nbsp;
        </td>
    </tr>
</table>
<br /><br />
           

<asp:Panel ID="pnl_Result" runat="server" Visible ="false" >

    <asp:GridView ID="gv_AWB" runat="server" AutoGenerateColumns="false" OnRowDataBound="ManifestDataBound" 
    AllowSorting="True"  OnSorting="gvAWBOnSort">
        <Columns>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:ImageButton ID="btn_View"  runat="server" ImageUrl="~/images/btn_view.gif" ToolTip="View Details" />
                </ItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Departure Date" SortExpression="DepartDate" HeaderStyle-Font-Underline="true">
                <ItemTemplate>
                    <asp:Label ID="lbl_DepartDate" runat="server"   Text='<%# Eval("DepartDate","{0:d}") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Container" SortExpression="ContainerNo" HeaderStyle-Font-Underline="true">
                <ItemTemplate>
                    <asp:Label ID="lbl_Container" runat="server"   Text='<%# Eval("ContainerNo") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Partner Container">
                <ItemTemplate>
                    <asp:Label ID="lbl_PartnerContainer"   runat="server" Text='<%# Eval("PartnerContainerNo") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Departure Info">
                <ItemTemplate>
                    <asp:Label ID="lbl_DepartInfo" runat="server"   Text='<%# Eval("DepartPort") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Arrival Info">
                <ItemTemplate>
                    <asp:Label ID="lbl_ArriveInfo" runat="server"   Text='<%# Eval("ArrivalPort") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Arrival Date">
                <ItemTemplate>
                    <asp:Label ID="lbl_ArriveDate" runat="server"   Text='<%# Eval("ArrivalDate","{0:d}") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Voyage No." SortExpression="VoyageNo" HeaderStyle-Font-Underline="true">
                <ItemTemplate>
                    <asp:Label ID="lbl_VoyageNo" runat="server"   Text='<%# Eval("VoyageNo") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Vessel Name">
                <ItemTemplate>
                    <asp:Label ID="lbl_VesselName" runat="server"   Text='<%# Eval("VesselName") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            Record not found.
        </EmptyDataTemplate>
    </asp:GridView>    
    <br />
        <br />
        <br />    
</asp:Panel>
</asp:Content>
