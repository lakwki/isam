<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="InvoiceListSummaryReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.InvoiceListSummaryReport" Title="Invoice Report" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<%@ Register Src="~/usercontrol/UclSortingOrder.ascx" TagName="UclSortingOrder" TagPrefix="uso1" %>
<%@ Register Src="~/usercontrol/UclOfficeProductTeamSelection.ascx" TagName="UclOfficeProductTeamSelection" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <!--
<img src="../images/banner_reports_invoice_list.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Invoice List Summary (BD)</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        //function copyInvoiceNo() {
        //    document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value.toUpperCase();
        //    document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value;
        //}

        function isFormValid() {
            //if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value == "" &&
            //    document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value == "" &&
            //    ((document.getElementById("ctl00_ContentPlaceHolder1_rad_InvoiceDate") != null && document.getElementById("ctl00_ContentPlaceHolder1_rad_InvoiceDate").checked) &&
            //    document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
            //    document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "") &&
            //    document.getElementById("ctl00_ContentPlaceHolder1_txt_PurchaseExtractDateFrom_txt_PurchaseExtractDateFrom_textbox").value == "" &&
            //    document.getElementById("ctl00_ContentPlaceHolder1_txt_PurchaseExtractDateTo_txt_PurchaseExtractDateTo_textbox").value == "") {
            //    alert("Please enter search criteria on one of below search criteria.\r\n- Invoice No.\r\n" +
            //    "- Invoice Date\r\n- Invoice Application Date\r\n-Purchase Extract Date\r\n- Fiscal Period");
            //    return false;
            //}
            //if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value != "" &&
            //(!isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value) ||
            //!isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value))) {
            //    alert("Invalid invoice no.");
            //    return false;
            //}

            if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox") != null &&
                ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == ""))) {
                alert("Invalid Invoice Date.");
                return false;
            }

            //if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_PurchaseExtractDateFrom_txt_PurchaseExtractDateFrom_textbox").value != "" &&
            //    document.getElementById("ctl00_ContentPlaceHolder1_txt_PurchaseExtractDateTo_txt_PurchaseExtractDateTo_textbox").value == "") ||
            //    (document.getElementById("ctl00_ContentPlaceHolder1_txt_PurchaseExtractDateFrom_txt_PurchaseExtractDateFrom_textbox").value == "" &&
            //    document.getElementById("ctl00_ContentPlaceHolder1_txt_PurchaseExtractDateTo_txt_PurchaseExtractDateTo_textbox").value != "")) {
            //    alert("Invalid Purchase Extract Date");
            //    return false;
            //}
            //if ((document.getElementById("ctl00_ContentPlaceHolder1_rad_FiscalPeriod") == null || document.getElementById("ctl00_ContentPlaceHolder1_rad_FiscalPeriod").checked) &&
            //document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodFrom").value > document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodTo").value) {
            //    alert("Invalid fiscal period.");
            //    return false;
            //}
            //if (GetCheckBoxSelectedCount('cbl_Customer') == 0) {
            //    alert("Please select one of the customers.");
            //    return false;
            //}

            //if (GetCheckBoxSelectedCount('cbl_TradingAgency') == 0) {
            //    alert("Please select one of the trading agencies");
            //    return false;
            //}

            //if (GetCheckBoxSelectedCount('cbl_ShipmentMethod') == 0) {
            //    alert("Please select one of the shipment method");
            //    return false;
            //}

            return true;
        }

    </script>
    <table>
        <tr>
            <td class="FieldLabel2">Upload File</td>
            <td>
                <asp:FileUpload ID="uplFile" runat="server" />
                <asp:Button runat="server" ID="btnLoadExcel" Text="Upload" CssClass="btn" CausesValidation="false" OnClick="btn_upload" SkinID="LButton" AutoPostBack="True" />
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="updatePanel3" runat="server" Visible="false">
        <ContentTemplate>

            <table>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanel2" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td class="FieldLabel4" style="width: 150px;">Uploaded L/C Bill Ref No. List</td>
                                        <td style="width: 500px">
                                            <asp:Label ID="txt_lcNumber" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                        <td class="FieldLabel4" style="width: 150px;">Version</td>
                                        <td>
                                            <asp:RadioButtonList ID="rad_Version" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rad_Version_CheckedChanged">
                                                <asp:ListItem Text="Epicor" Value="EPICOR" />
                                                <asp:ListItem Text="Sun" Value="SUN" Selected="True" />
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td class="FieldLabel4" style="width: 150px; vertical-align: top; background-position: top;">Invoice Date</td>
                                        <td>
                                            <table>
                                                <tr id="row_InvoiceDateRange" runat="server">
                                                    <%--<td>
                                                        <asp:RadioButton ID="rad_InvoiceDate" runat="server" GroupName="rad_SelectCriteria" Checked="true" AutoPostBack="true" Text="By Range : " OnCheckedChanged="rad_InvoiceDate_CheckedChanged" /></td>--%>
                                                    <td>
                                                        <cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo"/>
                                                        &nbsp;To&nbsp;<cc2:SmartCalendar
                                                            ID="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />
                                                    </td>
                                                </tr>
                                                <%--<tr>
                                                    <td id="col_Period" runat="server">
                                                        <asp:RadioButton ID="rad_FiscalPeriod" runat="server" GroupName="rad_SelectCriteria" AutoPostBack="true" Text="By Period : " OnCheckedChanged="rad_FiscalPeriod_CheckedChanged" /></td>
                                                    <td>Year&nbsp;&nbsp;<asp:DropDownList ID="ddl_Year" runat="server" SkinID="SmallDDL" Enabled="false" />&nbsp;&nbsp;&nbsp;
                                                        Period From&nbsp;<asp:DropDownList ID="ddl_PeriodFrom" runat="server" SkinID="SmallDDL"
                                                            OnSelectedIndexChanged="ddl_PeriodFrom_SelectedIndexChanged" AutoPostBack="True" Enabled="false">
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
                                                        <asp:DropDownList ID="ddl_PeriodTo" runat="server" SkinID="SmallDDL" Enabled="false">
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
                                                </tr>--%>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <%--<asp:AsyncPostBackTrigger ControlID="rad_InvoiceDate" EventName="CheckedChanged" />
                                <asp:AsyncPostBackTrigger ControlID="rad_FiscalPeriod" EventName="CheckedChanged" />
                                <asp:AsyncPostBackTrigger ControlID="rad_Version" EventName="SelectedIndexChanged" />--%>
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>

                <%--<tr>
                    <td class="FieldLabel4" style="width: 150px;">Invoice No</td>
                    <td>
                        <asp:TextBox ID="txt_InvoiceNoFrom" runat="server" onblur="copyInvoiceNo();" />&nbsp;To&nbsp;<asp:TextBox ID="txt_InvoiceNoTo" runat="server" /></td>
                </tr>

                <tr>
                    <td class="FieldLabel4">Purchase Extract Date</td>
                    <td>
                        <cc2:SmartCalendar ID="txt_PurchaseExtractDateFrom" runat="server" FromDateControl="txt_PurchaseExtractDateFrom" ToDateControl="txt_PurchaseExtractDateTo" />
                        &nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_PurchaseExtractDateTo" runat="server" FromDateControl="txt_PurchaseExtractDateFrom" ToDateControl="txt_PurchaseExtractDateTo" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Departure Date</td>
                    <td>
                        <cc2:SmartCalendar ID="txt_DepartDateFrom" runat="server" FromDateControl="txt_DepartDateFrom" ToDateControl="txt_DepartDateTo" />
                        &nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_DepartDateTo" runat="server" FromDateControl="txt_DepartDateFrom" ToDateControl="txt_DepartDateTo" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Voyage No</td>
                    <td>
                        <asp:TextBox ID="txt_VoyageNo" runat="server" /></td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Base Currency</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_BaseCurrency" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <uc2:UclOfficeProductTeamSelection ID="uclOfficeProductTeamSelection" runat="server" />
                        <asp:DropDownList ID="ddl_Office" runat="server" Style="display: none;" title="Add for compatiable to UclSmartSelection">
                            <asp:ListItem Selected="True" Value="-1">All</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Handling Office</td>
                    <td>
                        <asp:DropDownList ID="ddl_HandlingOffice" runat="server">
                            <asp:ListItem Text="-- ALL --" Value="-1" />
                            <asp:ListItem Text="DG" Value="17" />
                            <asp:ListItem Text="HK" Value="1" />
                            <asp:ListItem Text="SH" Value="2" />
                            <asp:ListItem Text="VN" Value="16" />
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="FieldLabel4" rowspan="4">Order Type</td>
                    <td>
                        <asp:DropDownList ID="ddl_SZOrder" runat="server" SkinID="LargeDDL">
                            <asp:ListItem Text="ALL (SZ + NON-SZ ORDER)" Value="-1" Selected="True" />
                            <asp:ListItem Text="SZ ORDER" Value="1" />
                            <asp:ListItem Text="NON-SZ ORDER" Value="0" />
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddl_UTOrder" runat="server" SkinID="LargeDDL">
                            <asp:ListItem Text="ALL (UT + NON-UT ORDER)" Value="-1" Selected="True" />
                            <asp:ListItem Text="UT ORDER" Value="1" />
                            <asp:ListItem Text="NON-UT ORDER" Value="0" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddl_LDPOrder" runat="server" SkinID="LargeDDL">
                            <asp:ListItem Text="ALL (LDP + NON-LDP ORDER)" Value="-1" Selected="True" />
                            <asp:ListItem Text="LDP ORDER" Value="1" />
                            <asp:ListItem Text="NON-LDP ORDER" Value="0" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinID="LargeDDL">
                            <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS/STUDIO SAMPLE ORDER)" Value="-1" />
                            <asp:ListItem Text="MAINLINE ORDER" Selected="True" Value="0" />
                            <asp:ListItem Text="MOCK SHOP/PRESS/STUDIO SAMPLE ORDER" Value="1" />
                        </asp:DropDownList>
                    </td>
                </tr>

                <tr>
                    <td class="FieldLabel4">Supplier</td>
                    <td>
                        <uc1:UclSmartSelection ID="txt_Supplier" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Purchase Term</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_TermOfPurchase" runat="server" /></td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Season</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_Season" runat="server" /></td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Destination</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_CustomerDestination" runat="server" /></td>
                </tr>
                <tr>
                    <td class="FieldLabel4">OPR Type</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_OPRType" runat="server" /></td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Customer</td>
                    <td class="CellWithBorder">
                        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="550" RepeatLayout="Table" RepeatColumns="4">
                        </asp:CheckBoxList>
                    </td>
                    <td>
                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_Customer'); return false;" /></td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Trading Agency</td>
                    <td class="CellWithBorder">
                        <asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="550" RepeatLayout="Table" RepeatColumns="4">
                        </asp:CheckBoxList></td>
                    <td>
                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" /></td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Shipment Method</td>
                    <td class="CellWithBorder">
                        <asp:CheckBoxList ID="cbl_ShipmentMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table">
                        </asp:CheckBoxList>
                    </td>
                    <td>
                        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/images/icon_clear.gif" OnClientClick="UncheckAll('cbl_ShipmentMethod'); return false;" /></td>
                </tr>
                <tr style="display: none;">
                    <td class="FieldLabel4">Account Doc. Receipted</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_AccDocReceipt" runat="server">
                            <asp:ListItem Value="-1" Text="--All--"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Receipted"></asp:ListItem>
                            <asp:ListItem Value="0" Text="Not Yet Receipt"></asp:ListItem>
                        </cc2:SmartDropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Payment Term</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_PaymentTerm" runat="server">
                            <asp:ListItem Value="-1" Text="--All--"></asp:ListItem>
                            <asp:ListItem Value="1" Text="O/A"></asp:ListItem>
                            <asp:ListItem Value="2" Text="L/C"></asp:ListItem>
                        </cc2:SmartDropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Payment Status</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_PaymentStatus" runat="server">
                            <asp:ListItem Value="-1" Text="--All--"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Paid"></asp:ListItem>
                            <asp:ListItem Value="0" Text="Not Yet Pay"></asp:ListItem>
                        </cc2:SmartDropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4">L/C Payment Checked Status</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_LCPaymentChecked" runat="server">
                            <asp:ListItem Value="-1" Text="--All--"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                            <asp:ListItem Value="0" Text="No"></asp:ListItem>
                        </cc2:SmartDropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4">Shipping Doc. Receipt Date</td>
                    <td>
                        <cc2:SmartDropDownList ID="ddl_ShippingDocReceiptDate" runat="server">
                            <asp:ListItem Value="-1" Text="--All--"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                            <asp:ListItem Value="0" Text="No"></asp:ListItem>
                        </cc2:SmartDropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>--%>
            </table>
            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
