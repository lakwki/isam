<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKDiscountClaimEdit.aspx.cs" Inherits="com.next.isam.webapp.claim.UKDiscountClaimEdit" Title="Next Discount Update" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">UK Discount Claim - Edit</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" language="javascript">
        function chkHasUKDN_OnChange(a) {
            var ckbHasDN = document.getElementById("<%=chk_HasUKDN.ClientID %>");
            var updUKDN = document.getElementById("<%=upd_UKDebitNote.ClientID %>");
            var txtDNNo = document.getElementById("<%=txt_UKDNNo.ClientID %>");
            var txtDNDate = document.getElementById("<%=txt_UKDNDate.ClientID %>_txt_UKDNDate_textbox")
            var txtDNRcvDate = document.getElementById("<%=txt_UKDNReceivedDate.ClientID %>_txt_UKDNReceivedDate_textbox")
            var imgDNDate = getFirstTag("div_UKDNDate", "img");
            var imgDNRcvDate = getFirstTag("div_UKDNReceivedDate", "img");

            if (!ckbHasDN.disabled) {
                if (ckbHasDN.checked) {
                    txtDNNo.disabled = false;
                    txtDNDate.disabled = false;
                    txtDNRcvDate.disabled = false;
                    if (updUKDN != null)
                        updUKDN.disabled = false;
                }
                else {
                    txtDNNo.value = "";
                    txtDNDate.value = "";
                    txtDNRcvDate.value = "";
                    txtDNNo.disabled = true;
                    txtDNDate.disabled = true;
                    txtDNRcvDate.disabled = true;
                    if (updUKDN != null)
                        updUKDN.disabled = true;
                }
            }
            else {
                txtDNDate.disabled = txtDNDate.readOnly;
                txtDNRcvDate.disabled = txtDNRcvDate.readOnly;
            }
            if (imgDNDate != null) {
                imgDNDate.disabled = txtDNDate.disabled;
                imgDNDate.style.cursor = (imgDNDate.disabled ? "auto" : "hand");
            }
            if (imgDNRcvDate != null) {
                imgDNRcvDate.disabled = txtDNRcvDate.disabled;
                imgDNRcvDate.style.cursor = (imgDNRcvDate.disabled ? "auto" : "hand");
            }
        }

        function getRefundResult(result) {
            //result : the string includes ReceivedDate, Amount & Remark (the first character is the delimiter)
            //alert(result);
            oResult = document.getElementById("<%=hid_RefundResult.ClientID %>");
            oResult.value = (result == undefined ? "" : result);
        }

        function getFirstTag(parentName, tag) {
            var obj = document.getElementById(parentName);
            var returnObj = null;
            var element = obj.getElementsByTagName(tag);
            for (i = 0; i < element.length; i++) {
                returnObj = element[i];
                break;
            }
            return returnObj;
        }

        function confirmToSave() {
            return confirm("Confirm that the NEXT Claim information is correct.")
        }

        function showProgressBarX() {
            var left = window.screen.availWidth / 3 - 100;
            var top = window.screen.availHeight / 3 - 100

            waitDialog = window.showModelessDialog("../common/ProgressBarX.htm", window, "dialogHeight: 30px; dialogWidth: 250px; center:yes; edge: sunken; center: Yes; help: No; resizable: No; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
        }
    </script>

    <table width="800px">
        <tr>
            <td colspan="3"><b>
                <asp:ValidationSummary ID="vs" runat="server"></asp:ValidationSummary>
            </b></td>
        </tr>
        <tr id="tr_UKClaimMainInfo">
            <td colspan="2">
                <table>
                    <tr>
                        <td colspan="4">
                            <table id="tb_MainClaimInfo" style="margin-left: 10px;" cellpadding="5" cellspacing="0">
                                <tr style="display: none;">
                                    <td style="width: 161px"></td>
                                    <td width="160px"></td>
                                    <td width="100px"></td>
                                    <td width="160px"></td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:CheckBox ID="chk_HasUKDN" runat="server" Text="Next Debit Note Available" OnClick="chkHasUKDN_OnChange();" OnChange="chkHasUKDN_OnChange();" Checked="true" /></td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 161px">Next D/N No</td>
                                    <td>
                                        <asp:TextBox ID="txt_UKDNNo" runat="server" />
                                    </td>
                                    <td class="FieldLabel2">Status</td>
                                    <td>
                                        <div style="border: thin solid silver; width: 150px; height: 18px; display: none;">
                                            <b>
                                                <asp:Label ID="lbl_Status" runat="server" Style="position: relative; top: 2px; left: 2px;"></asp:Label></b>
                                        </div>
                                        <asp:TextBox ID="txt_Status" runat="server" Style="font-weight: 600; width: 150px;" ReadOnly="true" TabIndex="-1" />
                                        <asp:HiddenField ID="hid_Status" runat="server" Value="" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 161px">Next D/N Date</td>
                                    <td>
                                        <div id="div_UKDNDate" style="display: block;">
                                            <cc2:SmartCalendar ID="txt_UKDNDate" runat="server" />
                                        </div>
                                    </td>
                                    <td class="FieldLabel2">Next D/N Received Date</td>
                                    <td>
                                        <div id="div_UKDNReceivedDate" style="display: block;">
                                            <cc2:SmartCalendar ID="txt_UKDNReceivedDate" runat="server" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <hr />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:CheckBox ID="chk_AppliedDiscount" Text="Applied Discount" runat="server" Enabled="false" />&nbsp;&nbsp;  
                                        <asp:Button ID="btn_Send_Alert" runat="server" Text="Send Alert to PIC" SkinID="XLButton" CausesValidation="false" OnClick="btn_Send_Alert_Click" />
                                    </td>
                                </tr>

                                <tr>
                                    <td class="FieldLabel2" style="width: 161px">Contract No</td>
                                    <td>
                                        <asp:TextBox ID="txt_ContractNo" runat="server" Style="width: 140px;"
                                            AutoPostBack="True" OnTextChanged="txt_ContractNo_TextChanged" />
                                    </td>
                                    <td class="FieldLabel2">Item No</td>
                                    <td>
                                        <asp:TextBox ID="txt_ItemNo" runat="server" OnTextChanged="txt_ItemNo_OnTextChanged" AutoPostBack="true" />
                                        <asp:HiddenField ID="hid_ItemNo" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 161px">Supplier</td>
                                    <td colspan="3">
                                        <cc2:SmartDropDownList ID="ddl_Supplier" runat="server" Enabled="false"
                                            Width="425px" OnSelectedIndexChanged="ddl_Supplier_SelectedIndexChanged" AutoPostBack="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 161px">Office / H. Office / Term / PT</td>
                                    <td colspan="3" valign="top">
                                        <table>
                                            <tr>
                                                <td>
                                                    <cc2:SmartDropDownList ID="ddl_Office" runat="server" Width="55px" Enabled="false" />&nbsp;&nbsp;
                                                </td>
                                                <td>
                                                    <cc2:SmartDropDownList ID="ddl_HandlingOffice" runat="server" Width="55px" />&nbsp;&nbsp;
                                                </td>
                                                <td>
                                                    <cc2:SmartDropDownList ID="ddl_TermOfPurchase" runat="server" Width="80px" Enabled="false" />&nbsp;&nbsp;
                                                </td>
                                                <td>
                                                    <uc1:UclSmartSelection ID="uclProductTeam" runat="server" Enabled="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 161px">Qty</td>
                                    <td>
                                        <asp:TextBox ID="txt_Qty" runat="server" />
                                    </td>
                                    <td class="FieldLabel2">Amount</td>
                                    <td>
                                        <cc2:SmartDropDownList ID="ddl_Currency" runat="server" Width="45px" />&nbsp;
                                    <asp:TextBox ID="txt_Amt" runat="server" Style="width: 88px;" />
                                    </td>
                                </tr>

                                <tr>
                                    <td class="FieldLabel2" style="width: 161px">Comments</td>
                                    <td colspan="3">
                                        <asp:TextBox ID="txt_Desc" runat="server" Style="width: 420px;" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2">Payment Office</td>
                                    <td>
                                        <cc2:SmartDropDownList ID="ddl_PaymentOffice" runat="server" Width="120px" />&nbsp;&nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width: 161px" />
                                    <td colspan="3">
                                        <asp:TextBox TextMode="MultiLine" Rows="4" ID="txt_RemarkHistory" runat="server" Style="width: 420px; background-color: lightgrey" ReadOnly="true" /></td>
                                </tr>
                                <tr id="tr_DNCopyUpload" runat="server">
                                    <td class="FieldLabel2">D/N Copy</td>
                                    <td colspan="3">
                                        <asp:FileUpload ID="upd_UKDebitNote" runat="server" Width="430px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <hr />
                                    </td>
                                </tr>
                                <%--    <tr id="row_Refund" runat="server">
                                    <td class="FieldLabel2">UK Discount Claim Refund</td>
                                    <td>
                                        <asp:Button ID="btn_Add" runat="server" Text="Add" SkinID="LButton" CausesValidation="false" OnClick="btn_Add_Click" />
                                        <asp:HiddenField ID="hid_RefundResult" runat="server" Value="" />
                                    </td>
                                </tr>--%>
                              </table>
                            <asp:Panel ID="Panel4" runat="server" SkinID="sectionHeader1" Width="700px" Height="28">
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <div style="padding-top: 2px; font-size: 9pt;">&nbsp;UK Discount Claim Refund&nbsp;&nbsp;</div>
                                        </td>
                                        <td>
                                            <div style="padding-top: 1px;">
                                                &nbsp;&nbsp;
                                                <asp:Button ID="btn_Add" runat="server" Text="Add" OnClick="btn_Add_Click" CausesValidation="false" />
                                                <asp:HiddenField ID="hid_RefundResult" runat="server" Value="" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>   
                        </td>
                    </tr>
                </table>
            </td>
        </tr>


    </table>


    <asp:GridView ID="gv_Refund" runat="server" AutoGenerateColumns="false"
        OnRowCommand="gv_Refund_RowCommand" OnRowDataBound="gv_Refund_RowDataBound" RowStyle-VerticalAlign="Top">
        <Columns>

            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditRefund" runat="server" ImageUrl="~/images/icon_edit.gif" ToolTip="Edit Claim Refund" CommandName="EditUKClaimRefund" />
                    <asp:ImageButton ID="imgDeleteRefund" runat="server" ImageUrl="~/images/icon_delete.gif" ToolTip="Delete Next Claim Refund" CommandName="DeleteUKClaimRefund" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Received Date" ItemStyle-Width="90px" ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Style="position: absolute; width: 520px; text-align: left;">
                        <table>
                            <tr>
                                <td height="15px"></td>
                            </tr>
                            <tr>
                                <td><b>Remark : </b></td>
                                <td>&nbsp;</td>
                                <td style="width: 500px; border: 1px solid Silver; background-color: White; z-index: 10; text-align: left;">
                                    <asp:Label ID="lbl_Remark" runat="server" Style="padding-left: 5px;" />
                                </td>
                            </tr>
                        </table>
                    </asp:Label><asp:Label ID="lbl_ReceivedDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amount" ItemStyle-Width="90px">
                <ItemTemplate>
                    <asp:Label ID="lbl_Amount" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <asp:CustomValidator ID="valCustom" runat="server" 
        Display="None" onservervalidate="valCustom_ServerValidate"></asp:CustomValidator><asp:Button ID="btn_Save" runat="server" Text="Save" OnClick="btn_Save_Click" OnClientClick="if (!confirmToSave()) return false;showProgressBarX();" CausesValidation="false" />&nbsp;&nbsp; <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" OnClick="btn_Cancel_Click" CausesValidation="false" />&nbsp;&nbsp; <asp:Button ID="btn_ViewLog" runat="server" Text="View Log" SkinID="MButton" CausesValidation="false" />&nbsp;&nbsp; <asp:Button ID="btn_ViewAttachment" runat="server" Text="View Attachment" SkinID="LButton" CausesValidation="false" />&nbsp;&nbsp; <asp:Button ID="btn_New" runat="server" Text="New UK Discount Claim" SkinID="XLButton" OnClick="btn_New_Click" />&nbsp;&nbsp; <script language="javascript" type="text/javascript">
        //initSmartCalendar("txt_UKDNDate");
        //initSmartCalendar("txt_UKDNReceivedDate");
        chkHasUKDN_OnChange()
    </script><br />
    <br />
</asp:Content>
