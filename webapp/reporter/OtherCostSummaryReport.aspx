<%@ Page Language="C#"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="OtherCostSummaryReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.OtherCostSummaryReport"  Title='Other Cost Summary Report'  Theme="DefaultTheme"%>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!--
<table width="100%" cellpadding=0 cellspacing=0>
<tr>
    <td class='tableHeader' style='font-size:135%;color:#ff9900;font-stretch:narrower;height:25px;' width='70px' >&nbsp;&nbsp;Report&nbsp;&nbsp;</td>
    <td class="tableHeader" style=' font-size:80%;color:Black;font-weight:bold;' >Other Cost Summary Report</td>
</tr>
</table>
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Other Cost Summary Report</asp:Panel>

<script type="text/javascript">
    function copyInvoiceNo() {
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value.toUpperCase();
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value;
    }

    function isFormValid() {

        if ((document.getElementById("ctl00_ContentPlaceHolder1_rad_InvoiceDate").checked &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" && 
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "") &&
            (document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateFrom_txt_DeliveryDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateTo_txt_DeliveryDateTo_textbox").value == "")) 
        {
            alert("Please enter search criteria on one of below search criteria."
                    + "\r\n - Invoice Date"
                    + ";\r\n - Fiscal Period"
//                    + ";\r\n - Delivery Date"
                    + "."
                    );
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "")) 
        {
            alert("Invalid Invoice Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateFrom_txt_DeliveryDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateTo_txt_DeliveryDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateFrom_txt_DeliveryDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateTo_txt_DeliveryDateTo_textbox").value != "")) 
        {
            alert("Invalid Delivery Date");
            return false;
        }
        if (document.getElementById("ctl00_ContentPlaceHolder1_rad_FiscalPeriod").checked &&
            document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodFrom").value > document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodTo").value) {
            alert("Invalid fiscal period.");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_Office') == 0) {
            alert("Please select at least one Office.");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_TradingAgency') == 0) {
            alert("Please select at least one Trading Agency.");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_PurchaseTerm') == 0) {
            alert("Please select at least one Purchase Term.");
            return false;
        }


        if (!(document.getElementById("ctl00_ContentPlaceHolder1_cbx_GetActual").checked || document.getElementById("ctl00_ContentPlaceHolder1_cbx_GetRealized").checked ||
            document.getElementById("ctl00_ContentPlaceHolder1_cbx_GetAccrual").checked))
        {
            alert("Please select at least one cut off type.");
            return false;
        }
            
        return true;
    }

</script>


<asp:UpdatePanel ID="updatePanel3" runat="server">
    <ContentTemplate >
    
<table>
<tr>
    <td width='100px'>&nbsp;</td>
    <td >&nbsp;</td>
    <td >&nbsp;</td>
</tr>
<tr >
    <td colspan=3>
    <asp:UpdatePanel ID="updatePanel2" runat="server">
        <ContentTemplate >            
        <table>
            <tr>
                <td class="FieldLabel4_T" style="width:100px; vertical-align:top;" rowspan='3'>Range</td>
                <td style="width:100px;">
                    <asp:RadioButton ID="rad_InvoiceDate" runat="server" GroupName="rad_SelectCriteria"  Width='120' AutoPostBack="true" Checked="true" Text="By Invoice Date : " OnCheckedChanged="rad_InvoiceDate_CheckedChanged" />
                </td>
                <td>
                    <cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />
                    &nbsp;To&nbsp;
                    <cc2:SmartCalendar id="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />
                </td>
            <tr style='display:none;'>
                <td style="width:100px;">
                    <asp:RadioButton ID="rad_DeliveryDate" runat="server" GroupName="rad_SelectCriteria"  Width='120' AutoPostBack="true" Text="By Supplier At Warehouse Date : " OnCheckedChanged="rad_DeliveryDate_CheckedChanged" />
                </td>
                <td>
                    <cc2:SmartCalendar ID="txt_DeliveryDateFrom" runat="server" FromDateControl="txt_DeliveryDateFrom" ToDateControl="txt_DeliveryDateTo" />
                    &nbsp;To&nbsp;
                    <cc2:SmartCalendar id="txt_DeliveryDateTo" runat="server" FromDateControl="txt_DeliveryDateFrom" ToDateControl="txt_DeliveryDateTo" />
                </td>
            </tr>
            <tr>
                <td style="width:100px;"><asp:RadioButton ID="rad_FiscalPeriod" runat="server" GroupName="rad_SelectCriteria" AutoPostBack="true" Text="By Period : " OnCheckedChanged="rad_FiscalPeriod_CheckedChanged" /></td>
                <td>
                    Year&nbsp;&nbsp;<asp:DropDownList  ID="ddl_Year" runat="server" SkinId="SmallDDL" Enabled="false" />&nbsp;&nbsp;&nbsp;
                    Period From&nbsp;
                    <asp:DropDownList id="ddl_PeriodFrom"  onselectedindexchanged="ddl_PeriodFrom_SelectedIndexChanged" Enabled="false" SkinID="SmallDDL" runat="server" autopostback='true'  Width='0'>
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
                            <asp:ListItem Text="12" Value="12" />
                    </asp:DropDownList>
                    &nbsp;To&nbsp;
                    <asp:DropDownList ID="ddl_PeriodTo" Enabled="false" SkinID="SmallDDL" runat="server"  style='width:100;'>
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
                            <asp:ListItem Text="12" Value="12" />
                    </asp:DropDownList> 
                </td>
            </tr>
        </table>

        </ContentTemplate>
        <Triggers >
            <asp:AsyncPostBackTrigger ControlID="rad_InvoiceDate" EventName="CheckedChanged" />
            <asp:AsyncPostBackTrigger ControlID="rad_FiscalPeriod" EventName="CheckedChanged" />
        </Triggers>
        </asp:UpdatePanel> 
    </td>
</tr>

<tr style='display:none;'>
    <td class="FieldLabel4">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="400" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>    
     <td><asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_Customer'); return false;" /></td>
</tr>
<tr>
    <td class="FieldLabel4_T" valign="top">Office</td>
    <td class="CellWithBorder" style="text-align:right;">
        <asp:CheckBoxList ID="cbl_Office" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" 
                OnSelectedIndexChanged="cbl_Office_SelectedIndexChanged" AutoPostBack="true"
                RepeatLayout="Table"  RepeatColumns="4" style="text-align:left;" />
        <asp:LinkButton ID="lnk_SelectAllOffice" runat="server" Text="Select All" OnClientClick="CheckAll('cbl_Office');" autopostback="true"/>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="Lnk_DeselectAllOffice" runat="server" Text="Clear" OnClientClick="UncheckAll('cbl_Office');" autopostback="true"/>
        &nbsp;
    </td>
</tr>
<tr id="tr_HandlingOffice" runat="server" style="display:none;">
    <td class="FieldLabel4">Handling Office</td>
    <td><cc2:SmartDropDownList  ID="ddl_HandlingOffice" runat="server" width="70px" AutoPostBack="false" /></td>
</tr>

<tr>
    <td class="FieldLabel4_T" valign="top">Trading Agency</td>
    <td class="CellWithBorder" style="text-align:right;">
        <asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4" style="text-align:left"/>
        <asp:LinkButton ID="lnk_SelectAllTradingAgency" runat="server" Text="Select All" OnClientClick="CheckAll('cbl_TradingAgency');" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="lnk_DeSelectAllTradingAgency" runat="server" Text="Clear" OnClientClick="UncheckAll('cbl_TradingAgency');" />
        &nbsp;
    </td>
    <td style='display:none;'>
        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4_T" valign="top">Purchase Term</td>
    <td class="CellWithBorder" style="text-align:right;">
        <asp:CheckBoxList ID="cbl_PurchaseTerm" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4" style="text-align:left"/>
        <asp:LinkButton ID="lnk_SelectAllPurchaseTerm" runat="server" Text="Select All" OnClientClick="CheckAll('cbl_PurchaseTerm');" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="lnk_DeSelectAllPurchaseTerm" runat="server" Text="Clear" OnClientClick="UncheckAll('cbl_PurchaseTerm');" />
        &nbsp;
    </td>
    <td style='display:none;'>
        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_PurchaseTerm'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Country of Origin</td>
    <td><cc2:SmartDropDownList ID="ddl_CO" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Season</td>
    <td><cc2:SmartDropDownList ID="ddl_Season" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4"> Cutoff Type</td>
    <td>
        <div>
        &nbsp;
        <asp:CheckBox ID='cbx_GetActual' runat='server' Checked='true' Text='Actual' />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID='cbx_GetAccrual' runat='server' Checked='true' Text='Accrual' />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID='cbx_GetRealized' runat='server' Checked='true' Text='Realized' />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID='cbx_GetNotYetCut' runat='server' Checked='false' Text='Not Yet Cut' style='display:none;'/>
        </div>
    </td>    
</tr>
<tr style='display:block;'>
    <td class="FieldLabel4">Base Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_BaseCurrency" runat="server" /></td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>

<tr>
    <td>&nbsp;</td>
</tr>
</table>

</ContentTemplate>
</asp:UpdatePanel>
<asp:Button ID="btn_Preview" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Preview_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />

</asp:Content>
