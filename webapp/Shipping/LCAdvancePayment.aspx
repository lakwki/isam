<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="LCAdvancePayment.aspx.cs" Inherits="com.next.isam.webapp.shipping.LCAdvancePayment" Title="L/C Advance Payment" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc3" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <!--
    <img src="../images/banner_shipping_lc_mer.gif" runat="server" id="imgHeaderText" alt="workplace"/>
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Shipping">L/C Advance Payment</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        function isDate(DMY_Date) {
            var sDate;
            var sYear, sMonth, sDay;
            var nP1, nP2

            if (DMY_Date == "") {
                return true;
            }
            else {
                nP1 = DMY_Date.indexOf("/", 0);
                if (nP1 >= 0) {
                    nP2 = DMY_Date.indexOf("/", nP1 + 1);
                    if (nP2 >= 0)
                        nP3 = DMY_Date.indexOf("/", nP2 + 1);
                    else
                        nP3 = -1;
                }
                else
                    nP2 = -1;

                if (nP1 >= 0 && nP2 >= 0 && nP3 == -1) {
                    sDay = DMY_Date.substr(0, nP1);
                    sMonth = DMY_Date.substr(nP1 + 1, nP2 - nP1 - 1);
                    sYear = DMY_Date.substr(nP2 + 1, DMY_Date.length - nP2);
                    //alert (sYear + "-" + sMonth + "-" + sDay);
                    if (sMonth.valueOf() >= 1 && sMonth.valueOf() <= 12 && sDay.valueOf() >= 1)
                        if (sMonth.valueOf() != 2)
                            if (sMonth.valueOf() == 4 || sMonth.valueOf() == 6 || sMonth.valueOf() == 9 || sMonth.valueOf() == 11)
                                return (sDay.valueOf() <= 30);
                            else
                                return (sDay.valueOf() <= 31);
                        else
                            if (sYear.valueOf() % 4 == 0)
                                return (sDay.valueOf() <= 29);
                            else
                                return (sDay.valueOf() <= 28);
                }
                return false;
            }
        }

        function inputValidation() {
            var sFromAwhDate;
            var sToAwhDate;
            var sFromIssueDate;
            var sToIssueDate;
            var sFromLCNo;
            var sToLCNo;
            var sFromLCBatchNo;
            var sToLCBatchNo;
            var sFromAdvancePaymentNo;
            var sToAdvancePaymentNo;
            var sContractNo;
            var sFromNslRefNo, sToNslRefNo;

            sFromAwhDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value);
            sToAwhDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value);
            sFromIssueDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateFrom_txt_LCIssueDateFrom_textbox").value);
            sToIssueDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCIssueDateTo_txt_LCIssueDateTo_textbox").value);
            sFromLCNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoFrom").value);
            sToLCNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCNoTo").value);
            sFromLCBatchNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoFrom").value);
            sToLCBatchNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCBatchNoTo").value);
            sFromAdvancePaymentNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_AdvancePaymentNoFrom").value);
            sToAdvancePaymentNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_AdvancePaymentNoTo").value);
            sContractNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_ContractNo").value);
            sItemNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_ItemNo").value);
            sFromNslRefNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_NSLRefNoFrom").value);
            sToNslRefNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_NSLRefNoTo").value);
            sFromApprovalDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_ApprovalDateFrom_txt_ApprovalDateFrom_textbox").value);
            sToApprovalDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_ApprovalDateTo_txt_ApprovalDateTo_textbox").value);
            
            if ((sFromAwhDate == "" || sToAwhDate == "") && (sFromIssueDate == "" || sToIssueDate == "")
                && (sFromLCNo == "" || sToLCNo == "") && (sFromLCBatchNo == "" || sToLCBatchNo == "")
                && (sFromAdvancePaymentNo == "" || sToAdvancePaymentNo == "") && (sFromNslRefNo == "" || sToNslRefNo == "")
                && sContractNo == "" && sItemNo == "" && (sFromApprovalDate == "" || sToApprovalDate == "")) {
                alert("Please input any of the followings:\n\r - At-Warehouse Date\n\r - Item No.\n\r - Contract No.\n\r - L/C Issue Date\n\r - L/C No.\n\r - L/C Batch No.\n\r - Advance Payment No.\n\r - NSL Ref. No.\n\r - L/C Advance Payment Approval Date");
                return false;
            }
            else
                if (!isDate(sFromAwhDate) || !isDate(sToAwhDate) || !isDate(sFromIssueDate) || !isDate(sToIssueDate)) {
                    // sDate = new Date(sFromDate);
                    alert("Invalid Date Format.");
                    return false;
                }
                else {
                    return true;
                }
            return false;
        }


        function checkAppNo(obj) {
            var input = "";
            var val = 0;

            input = obj.value.trim();
            val = parseInt(input);
            if (input != "") {
                if (isNaN(val)) {
                    alert("Invalid format");
                    return false;
                }
                else {
                    if (parseInt(input).toString() != input) {
                        alert("Invalid format");
                        return false;
                    }
                }
            }
            return true;
        }

        function SubmitApplication(obj) {
            obj.disabled = true;
            document.all.ctl00_ContentPlaceHolder1_btn_Apply.click();
            return true;
        }

        function WhoAmI(obj) {
            alert(obj.id);
            return true;
        }
    </script>

    <table width="800px" cellspacing="0" cellpadding="2">
        <col width="120" />
        <col width="70" />
        <col width="80" />
        <col width="150" />
        <col width="250" />
        <col />
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2">&nbsp;<span>P.O. Scheduled<br />
                &nbsp;At-warehouse Date</span></td>
            <td colspan="4">
                <cc3:SmartCalendar ID="txt_AtWHDateFrom" runat="server"
                    ToDateControl="txt_AtWHDateTo" RequiredFieldEnabled="False"
                    RequiredFieldText="Please Input At Warehouse Date here" />
                &nbsp;&nbsp;to&nbsp;
			    <cc3:SmartCalendar ID="txt_AtWHDateTo" runat="server"
                    FromDateControl="txt_AtWHDateFrom" RequiredFieldEnabled="False"
                    RequiredFieldText="Please Input At Warehouse Date here" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">&nbsp;Supplier</td>
            <td colspan="4">
                <uc1:UclSmartSelection ID="txt_VendorName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">&nbsp;Item No</td>
            <td>
                <span>
                    <asp:TextBox ID="txt_ItemNo" runat="server" /></span>
            </td>
            <td class="FieldLabel2">Contract No.</td>
            <td>
                <span>
                    <asp:TextBox ID="txt_ContractNo" runat="server" /></span>
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="FieldLabel2">&nbsp;Office</td>
            <td>
                <cc3:SmartDropDownList ID='ddl_Office' runat='server' Width="150"
                    OnSelectedIndexChanged="ddl_Office_SelectedIndexChanged" AutoPostBack="True" />
            </td>
            <td class="FieldLabel2">Product Team</td>
            <td>
                <cc3:SmartDropDownList ID='ddl_ProductTeam' runat='server' Width="250" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="FieldLabel2">L/C Issue Date</td>
            <td colspan="3">
                <cc3:SmartCalendar ID="txt_LCIssueDateFrom" runat="server" Width="120px" FromDateControl="txt_LCIssueDateFrom"
                    ToDateControl="txt_LCIssueDateTo" />
                &nbsp;&nbsp;to&nbsp;
			    <cc3:SmartCalendar ID="txt_LCIssueDateTo" runat="server" Width="120px" FromDateControl="txt_LCIssueDateFrom"
                    ToDateControl="txt_LCIssueDateTo" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">L/C No.</td>
            <td colspan="3">
                <asp:TextBox ID="txt_LCNoFrom" runat="server"  SkinID="MTextBox" />&nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;&nbsp;&nbsp;
		    	<asp:TextBox ID="txt_LCNoTo" runat="server"  SkinID="MTextBox" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">L/C Batch No.</td>
            <td colspan="3">
                <asp:TextBox ID="txt_LCBatchNoFrom" runat="server" SkinID="MTextBox" onchange='return(checkAppNo(this));' />&nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;&nbsp;&nbsp;
		    	<asp:TextBox ID="txt_LCBatchNoTo" runat="server" SkinID="MTextBox" onchange='return(checkAppNo(this));' />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Advance Payment No.</td>
            <td colspan="3">
                <asp:TextBox ID="txt_AdvancePaymentNoFrom" runat="server" SkinID="MTextBox"  />&nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;&nbsp;&nbsp;
		    	<asp:TextBox ID="txt_AdvancePaymentNoTo" runat="server" SkinID="MTextBox"  />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">NSL Ref. No.</td>
            <td colspan="3">
                <asp:TextBox ID="txt_NSLRefNoFrom" runat="server" SkinID="MTextBox" />&nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;&nbsp;&nbsp;
		    	<asp:TextBox ID="txt_NSLRefNoTo" runat="server" SkinID="MTextBox"  />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">L/C Advance Payment Approval/Rejected/Pending Date</td>
            <td colspan="3">
                <cc3:SmartCalendar ID="txt_ApprovalDateFrom" runat="server" Width="120px" FromDateControl="txt_ApprovalDateFrom"
                    ToDateControl="txt_ApprovalDateTo" />
                &nbsp;&nbsp;to&nbsp;
			    <cc3:SmartCalendar ID="txt_ApprovalDateTo" runat="server" Width="120px" FromDateControl="txt_ApprovalDateFrom"
                    ToDateControl="txt_ApprovalDateTo" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">C19 order</td>
            <td colspan="3">
                <asp:CheckBox runat="server" ID="ckb_C19" Checked="true" Text="C19" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:CheckBox runat="server" ID="ckb_NotC19" Checked="true" Text="Not C19" />
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>
    <table cellspacing="10">
        <tr>
            <td colspan="1">
                <asp:Button ID="btn_Search" runat="server" Text="Search"
                    OnClick="btn_Search_Click" OnClientClick='return inputValidation();' />
            </td>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="btn_Print" runat="server" Text="Print" OnClick="btn_Print_Click" Style="height: 26px" />
            </td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <asp:Label ID="lbl_RowCount" runat="server" Style="color: #ff9900; font-weight: bolder;" Text="" /><br />
    <br />
    &nbsp;&nbsp;
    <asp:Panel ID="pnl_SearchResult" runat="server" Visible="false">
        <asp:UpdatePanel ID="up_LC" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gv_LC" runat="server" AutoGenerateColumns="false" OnRowDataBound="gv_LC_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Contract No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ContractNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dly No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DlyNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date">
                            <ItemTemplate>
                                <asp:Label ID="lbl_InvoiceDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Supplier" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delivery Date">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DeliveryDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="PO Qty">
                            <ItemTemplate>
                                <asp:Label ID="lbl_POQty" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipped Qty">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ShippedQty" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Amount">
                            <ItemTemplate>
                                <asp:Label ID="lbl_InvoiceAmount" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contract Status">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ContractStatus" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Paid (Y/N)" HeaderStyle-Width="20px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_InvoicePaid" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="L/C Application No." HeaderStyle-Width="30px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_LCAppNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="L/C No." HeaderStyle-Width="110px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_LCNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="L/C Issue Date">
                            <ItemTemplate>
                                <asp:Label ID="lbl_LCIssueDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="L/C Expired Date">
                            <ItemTemplate>
                                <asp:Label ID="lbl_LCExpiredDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="With Payment Deduction in L/C (Y/N)">
                            <ItemTemplate>
                                <asp:Label ID="lbl_IsAnyPaymentDeductionInLC" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Payment Deduction Amt in L/C" HeaderStyle-Width="40px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_PaymentDeductionAmtInLC" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Payment Deduction Amount" HeaderStyle-Width="40px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_PaymentDeductionAmt" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Advance Payment No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_AdvancePaymentNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Advance Payment Approval/ Rejected/ Pending Date">
                            <ItemTemplate>
                                <asp:Label ID="lbl_AdvPayActionDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NSL Ref. No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_NSLRefNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Advance Payment Status (D/P/A/R)" HeaderStyle-Width="20px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_AdvancePaymentStatus" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Payment Term">
                            <ItemTemplate>
                                <asp:Label ID="lbl_PaymentTerm" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <span style="font-family: Arial; font-size: medium; font-weight: bold; background-color: #FFFFCC;">No record found.</span>
                    </EmptyDataTemplate>

                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </asp:Panel>
</asp:Content>
