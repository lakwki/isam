<%@ Page Language="C#" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="EditFabric.aspx.cs" Inherits="com.next.isam.webapp.account.EditFabric" Theme="DefaultTheme" Async="true" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Advance Payment (<%# paymentType.Name %>)</title>
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet" />
    <script src="../common/common.js" type="text/javascript"></script>
    <script type="text/javascript">
        window.focus();
        var paymentId = '<%# vwPaymentId %>';
        var popup;
        var toClose; // for popup
        var updating = false;
        var totalPaymentAmt = <%# TotalAmount %>;
        var totalActualValue = <%# TotalActualValue %>;
        var balDiff = <%# BalDiff %>;

        function openContractPage() {
            if ( !(typeof(popup) == 'undefined' || popup == null) && !popup.closed ) {
                popup.focus();
                return false;
            } else {
                popup = null;
                __doPostBack('btn_OpenContract');
                return true;
            }
        }

        function btnConfirmContractCallback() { //btnRefreshAdditionalYarns
            updating = true;
            __doPostBack('btn_RefreshContract');
            window.focus();
            if (typeof (popup) !== 'undefined' && popup != null) {
                popup.close();
                popup = null;
            }
        }
        function contractCloseCallback(){
            if(!updating){
                __doPostBack('btnClosedContract', '');
                window.focus();
                if (typeof (popup) !== 'undefined' && popup != null) {
                    popup.close();
                    popup = null;
                }
            }
        }

        window.onload = function () {
            if (typeof (contractListOpen) === "function") { // Only when popup opening
                popup = contractListOpen(popup);
            }
        }

        window.onunload = function (e) {
            if (typeof (popup) !== 'undefined' && popup != null) { // Popup no long exists
                toClose = true;
                if (!popup.closed) {
                    popup.close();
                    popup = null;
                }
            }
        }

    </script>
    <style type="text/css">
        tr.row td input {
            text-align: center;
        }

        input.Invalid {
            border-color: rgb(255, 0, 0);
        }

        tr.row td {
            text-align: center;
            background-color: rgb(255, 255, 255) !important;
        }

        tr.row.WorkflowCancelled td {
            background-color: rgb(221, 221, 221) !important;
        }

        tr.row.Settled td {
            background-color: rgb(252, 213, 180) !important;
        }

        #txb_Remark.TextArea {
            width: 300px !important;
        }

        #p01 {
            color: #005AB5;
            font-size: medium;
        }

        .beta table, .beta th, .beta td {
            border: 1px solid black;
        }

        .gridHeaderNew {
            font-size: x-small;
            font-weight: bold;
        }

        .repeaterHeaderNew {
            text-align: center;
            height: 30px;
            background-color: #F0F0F0;
        }

        tr.repeaterHeaderNew td {
            border-bottom: 1px solid #808080;
            border-right: 1px solid #808080;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server" method="POST">
        <h1 id="p01">Advance Payment (<%= paymentType.Name %>)</h1>
        <table>
            <tr>
                <td class="FieldLabel2" style="width: 120px;">Advanced Payment No</td>
                <td>
                    <asp:Label ID="lbl_PaymentNo" runat="server"></asp:Label>&nbsp;<asp:Label ID="lbl_FLRefNo" runat="server" Visible="False"></asp:Label></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Office</td>
                <td>
                    <asp:Label ID="lbl_Office" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Supplier Name</td>
                <td>
                    <asp:Label ID="lbl_Vendor" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Currency</td>
                <td>
                    <asp:Label ID="lbl_Currency" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="FieldLabel2">
                    <asp:Label runat="server" ID="lblTotalPaymentAmt" Text="Total Payment Amt" /></td>
                <td>
                    <asp:Label ID="lbl_TotalPaymentAmt" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="FieldLabel2">
                    <asp:Label runat="server" ID="lblTotalAmt" Text="Total Amt (Included Interest Amt)" /></td>
                <td>
                    <asp:Label ID="lbl_TotalAmt" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Interest Amt</td>
                <td>
                    <asp:TextBox ID="txt_InterestChargedAmt" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Interest Rate %</td>
                <td>
                    <asp:TextBox runat="server" ID="txt_InterestRate" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Payment Date</td>
                <td>
                    <cc2:SmartCalendar ID="uclPaymentDate" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel2" style="width: 120px;">Payable To</td>
                <td>
                    <asp:Label ID="lbl_PayableTo" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel2">Initiated By</td>
                <td>
                    <asp:Label ID="lbl_InitiatedBy" runat="server"></asp:Label>&nbsp;</td>
            </tr>
            <tr>
                <td class="FieldLabel2">Created By</td>
                <td>
                    <asp:Label ID="lbl_CreatedBy" runat="server"></asp:Label>&nbsp;</td>
            </tr>
            <tr>
                <td class="FieldLabel2" style="vertical-align: top; background-position: top;">Remark</td>
                <td class="style4">
                    <asp:TextBox ID="txb_Remark" runat="server" TextMode="MultiLine" Rows="4" Columns="400" CssClass="TextArea"></asp:TextBox>
                    <asp:HiddenField ID="hf_Remark" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel2" style="vertical-align: top; background-position: top;">Variance of total amount</td>
                <td>
                    <asp:Label ID="lbl_totalAmtVariance" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel2" style="vertical-align: top; background-position: top;">Balance of Adv. Payment</td>
                <td>
                    <asp:Label ID="lbl_balDiff" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel2" style="vertical-align: top; background-position: top;">Balance of Adv. Payment (No Recovery Plan)</td>
                <td>
                    <asp:Label ID="lbl_noRecoveryPlanBalance" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <hr />
        <table>
            <tr>
                <td style="width: 120px;">
                    <strong>Contract Details</strong>
                    <br />
                    <asp:Button ID="btn_AddContract" runat="server" Text="Add Contract" OnClientClick="openContractPage();" SkinID="MButton" PostBackUrl="javascript:void(0);" />
                </td>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btn_OpenContract" runat="server" Visible="false" OnClick="btn_OpenContract_Click" />
                    <asp:Button ID="btn_RefreshContract" runat="server" Visible="false" OnClick="btn_RefreshContract_Click" ValidationGroup="Open" />
                    <asp:Button ID="btnClosedContract" runat="server" CausesValidation="False" Text="" Visible="False" OnClick="btnClosedContract_Click" />

                </td>
            </tr>
        </table>
        <asp:GridView ID="gv_ContractDetails" runat="server" AutoGenerateColumns="False" EnableModelValidation="True"
            OnRowDataBound="gv_ContractDetails_RowDataBound" ShowFooter="True">
            <HeaderStyle CssClass="gridHeaderNew" />
            <Columns>
                <asp:TemplateField ShowHeader="True" HeaderText="Seq" HeaderStyle-Width="20px">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Seq" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="ContractNo" HeaderText="Contract No" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="Contract No" HeaderStyle-Width="100px">
                    <ItemTemplate>
                        <asp:LinkButton ID="btn_ContractNo" runat="server" ToolTip="Shipment Detail" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="ItemNo" HeaderText="Item No" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="Item No">
                    <ItemTemplate>
                        <asp:Label ID="lbl_ItemNo" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="ProductTeam" HeaderText="Product Team" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="Product Team">
                    <ItemTemplate>
                        <asp:Label ID="lbl_ProductTeam" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="PaymentTeam" HeaderText="Payment Term" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="Payment Term">
                    <ItemTemplate>
                        <asp:Label ID="lbl_PaymentTerm" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="LCBillRefNo" HeaderText="L/C Bill Ref No" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="L/C Bill Ref No">
                    <ItemTemplate>
                        <asp:Label ID="lbl_LCBillRefNo" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" HeaderText="L/C Settlement Date">
                    <ItemTemplate>
                        <asp:Label ID="lbl_LCSettlementDate" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="ExpectedValue" HeaderText="Expected Value" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="Expected Value">
                    <ItemTemplate>
                        <asp:Label ID="lbl_ExpectedValue" runat="server" />
                        <asp:TextBox ID="txt_ExpectedValue" runat="server" Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="ActualValue" HeaderText="Actual Value" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="Actual Deduction Value">
                    <ItemTemplate>
                        <asp:Label ID="lbl_ActualValue" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="WorkflowStatus" HeaderText="Workflow Status" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="Workflow Status">
                    <ItemTemplate>
                        <asp:Label ID="lbl_WorkflowStatus" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="SettlementDate" HeaderText="SettlementDate" ReadOnly="True" />--%>
                <asp:TemplateField ShowHeader="True" HeaderText="Deduction Date">
                    <ItemTemplate>
                        <asp:Label ID="lbl_SettlementDate" runat="server" />
                        <cc2:SmartCalendar ID="txt_SettlementDate" runat="server" Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:CommandField ShowDeleteButton="True" ButtonType="Image" CancelImageUrl="~/images/icon_delete.gif" />--%>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:ImageButton ID="ibtn_Delete" runat="server" Visible="false"
                            ImageUrl="~/images/icon_delete.gif" ToolTip="Delete"
                            OnClientClick="return confirm('Confirm to delete?');" OnCommand="ibtn_Delete_Command" />
                        <asp:Label ID="lbl_NewRecord" runat="server" Text="New" Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>No result found.</EmptyDataTemplate>
        </asp:GridView>
        <br />
        <br />
        <asp:Repeater ID="rep_BalanceDetail" runat="server" OnItemDataBound="rep_BalanceDetail_DataBound">
            <HeaderTemplate>
                <table style="width: 650px;" cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="5">
                            <strong>Balance Settlement<br />
                                <br />
                            </strong>
                        </td>
                    </tr>
                    <tr class="repeaterHeaderNew">
                        <td style="width: 10px;">
                            <asp:ImageButton ID="btn_AddBalance" runat="server" ImageUrl="~/images/icon_s_add.gif" OnClick="btn_AddBalance_Click" CausesValidation="false" />
                        </td>
                        <td style="width: 20px;"><b>Seq</b></td>
                        <td><b>Expected Date</b></td>
                        <td><b>Expected Amt<b></td>
                        <td><b>Actual Deduction Amt<b></td>
                        <td><b>Remark<b></td>
                        <td><b>Deduction Date</b></td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="repeaterHeaderNew" style="background-color: White;">
                    <td style="width: 10px;">
                        <asp:ImageButton ID="btn_RemoveBalance" runat="server" ImageUrl="~/images/icon_remove.gif" CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' OnClick="btn_RemoveBalance_Click" CausesValidation="false" />
                    </td>
                    <td style="width: 20px;">
                        <asp:Label ID="lbl_Seq" runat="server" />
                    </td>
                    <td style="width: 180px;">
                        <cc2:SmartCalendar ID="txtExpectedDate" runat="server" />
                    </td>
                    <td style="width: 140px;">
                        <asp:TextBox runat="server" ID="txtExpectedAmt" Text='<%# DataBinder.Eval(Container, "DataItem.ExpectedAmount")%>' /></td>
                    <td style="width: 140px;">
                        <asp:TextBox runat="server" ID="txtDeductionAmt" Text='<%# DataBinder.Eval(Container, "DataItem.PaymentAmount")%>' /></td>
                    <td style="width: 140px;">
                        <asp:TextBox runat="server" ID="txtRemark" Text='<%# DataBinder.Eval(Container, "DataItem.Remark")%>' /></td>
                    <td style="width: 180px;">
                        <cc2:SmartCalendar ID="txtDeductionDate" runat="server" ReadOnly="false" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr id="trEmpty">
                    <td colspan="5">
                        <asp:Label ID="lblNoData" runat="server" Text="No records found." Visible="false" />
                    </td>
                </tr>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <br />
        <table>
            <tr>
                <td>
                    <asp:Button ID="btn_Save" runat="server" Text="Save" UseSubmitBehavior="True" OnClick="btn_Save_Click" Enabled="False" SkinID="MButton" CausesValidation="True" ValidationGroup="Submit" />&nbsp;
                </td>
                <td>
                    <asp:Button runat="server" ID="btnAttachment" Text="View Attachment" OnClick="btnAttachment_Click" SkinID="LButton" />&nbsp;&nbsp;&nbsp;
                    <asp:Button runat="server" ID="btnLog" Text="View Log" CausesValidation="false" SkinID="LButton" />
                </td>
            </tr>
        </table>
        <asp:CustomValidator ID="cv_Submit" runat="server" ValidationGroup="Submit" OnServerValidate="cv_Submit_ServerValidate"></asp:CustomValidator>
        <asp:CustomValidator ID="cv_ContractDuplication" runat="server" ErrorMessage="<br/>Duplication existing" OnServerValidate="cv_ContractDuplication_ServerValidate" ValidationGroup="Open"></asp:CustomValidator>
        <asp:CustomValidator ID="cv_ExpectedValue" runat="server" ErrorMessage="<br/>Invalid Expected Value" OnServerValidate="cv_ExpectedValue_ServerValidate" ValidationGroup="Refresh"></asp:CustomValidator>
    </form>
</body>
</html>
