<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKDiscountClaimSearch.aspx.cs" Inherits="com.next.isam.webapp.claim.UKDiscountClaimSearch" Title="Search Page For UK Discount Claim" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">UK Discount Claim Portal</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        
        function isValidSearch() {
            if (document.all.<%= txt_DateFrom.ClientID %>_txt_DateFrom_textbox.value != '' || 
                document.all.<%= txt_UKDebitNoteNo.ClientID %>.value != '' ||
                document.all.<%= txt_ReceivedDateFrom.ClientID %>_txt_ReceivedDateFrom_textbox.value != '' ||
                document.all.<%= txt_ItemNo.ClientID %>.value != '')  
            {
                return true;
            }
        else
        {
            alert('Please input at least the [Next Debit Note No. / Next Debit Note Date / Next Debit Note Received Date / Item No.]');
            return false;
        }        
        }  

        function updateHandlingOffice() {
            if (parseInt(document.all.<%= ddl_Office.ClientID %>.value) != 17)
                document.all.<%= ddl_HandlingOffice.ClientID %>.value = document.all.<%= ddl_Office.ClientID %>.value;
        else
        {
            if (parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != -1 && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 17 && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 1 && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 2 && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 16)
            document.all.<%= ddl_HandlingOffice.ClientID %>.value = "-1";
        }
        }

        function ToggleMailSelection(o, controlName) {

            var nodeList = document.getElementsByTagName("input");

            for (i = 0; i < nodeList.length; i++) {
                if (nodeList[i].type == "checkbox")
                    if (nodeList[i].name != o.name && !(nodeList[i].disabled) && nodeList[i].name.indexOf(controlName) != -1)
                        nodeList[i].checked = o.checked;
            }
        }       
    
    </script>
    <table width="800px">
        <tr>
            <td colspan="2">
                <table>
                    <tr>
                        <td colspan="4" style="width: 604px">
                            <table style="margin-left: 10px;" cellpadding="5" cellspacing="0">
                                <tr>
                                    <td class="FieldLabel2" style="width: 139px">Office /
                                        <asp:Label runat="server" Text="H.Office" ToolTip="Handling Office" ID="lblHandlingOffice" /></td>
                                    <td>
                                        <cc2:SmartDropDownList ID="ddl_Office" runat="server" Width="60px" />&nbsp;
                                        <cc2:SmartDropDownList ID="ddl_HandlingOffice" runat="server" Width="60px" />&nbsp;
                                    </td>
                                    <td class="FieldLabel2" style="width: 139px">Next D/N No</td>
                                    <td>
                                        <asp:TextBox ID="txt_UKDebitNoteNo" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 139px">Next D/N Date</td>
                                    <td colspan="3">
                                        <cc2:SmartCalendar ID="txt_DateFrom" ToDateControl="txt_DateTo" runat="server" />
                                        &nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_DateTo" FromDateControl="txt_DateFrom" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 139px">Next D/N Rec'd Date</td>
                                    <td colspan="3">
                                        <cc2:SmartCalendar ID="txt_ReceivedDateFrom" ToDateControl="txt_ReceivedDateTo" runat="server" />
                                        &nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_ReceivedDateTo" FromDateControl="txt_ReceivedDateFrom" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 139px">Item No</td>
                                    <td>
                                        <asp:TextBox ID="txt_ItemNo" runat="server" />
                                    </td>
                                    <td class="FieldLabel2">Contract No</td>
                                    <td style="width: 129px">
                                        <asp:TextBox ID="txt_ContractNo" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2" style="width: 139px">Supplier</td>
                                    <td colspan="3">
                                        <uc1:UclSmartSelection ID="txt_Supplier" runat="server" width="300px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2_T" valign="top" style="width: 139px">Filters:</td>
                                    <td colspan="3">
                                        <asp:CheckBox ID="chkNextDNNo" Text="NEXT D/N NO." runat="server" />&nbsp;&nbsp;
                                        <asp:CheckBox ID="chkAppliedUKDiscount" Text="APPLIED UK DISCOUNT" runat="server" />
                                    </td>

                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:Button ID="btn_Search" runat="server" Text="Search"
                                            OnClick="btn_Search_Click" />&nbsp;
                                        <asp:Button ID="btn_Reset" runat="server" Text="Reset"
                                            OnClick="btn_Reset_Click" />&nbsp;
                                        <asp:Button ID="btn_Excel" runat="server" Text="Export To Excel" SkinID="LButton"
                                            OnClick="btn_Excel_Click" Visible="true"/>&nbsp;
                                        <asp:Button ID="btn_Create" runat="server" Text="New"
                                            OnClick="btn_Create_Click" />&nbsp;
                                        <asp:Button runat="server" Text="Mail D/N(s)" ID="btnMail" SkinID="LButton"
                                            OnClick="btnMail_Click" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:GridView ID="gv_UKClaim" runat="server" AutoGenerateColumns="false"
        OnRowCommand="gv_UKClaim_RowCommand" OnRowDataBound="gv_UKClaim_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="" Visible="false">
                <HeaderTemplate>
                    <input id="chkAll" onclick="javascript:ToggleMailSelection(this, 'chkMail');" type="checkbox" title="Select All or Clear All" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkMail" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/images/icon_edit.gif" ToolTip="Edit" CommandName="EditUKDiscountClaim" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/images/icon_delete.gif" ToolTip="Delete" CommandName="DeleteUKDiscountClaim" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Office">
                <ItemTemplate>
                    <asp:Label ID="lbl_Office" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Next D/N No">
                <ItemTemplate>
                    <asp:Label ID="lbl_UKDebitNoteNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Next D/N Date">
                <ItemTemplate>
                    <asp:Label ID="lbl_DebitNoteDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Vendor">
                <ItemTemplate>
                    <asp:Label ID="lbl_Vendor" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Product Team">
                <ItemTemplate>
                    <asp:Label ID="lbl_ProductTeam" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Item/Contract No">
                <ItemTemplate>
                    <asp:Label ID="lbl_ItemContractNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Currency">
                <ItemTemplate>
                    <asp:Label ID="lbl_Currency" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amt">
                <ItemTemplate>
                    <asp:Label ID="lbl_Amount" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Applied UK Discount">
                <ItemTemplate>
                    <asp:CheckBox ID="chkAppliedUKDiscount" runat="server" Enabled="false"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:Button ID="btnSendAlertToPIC" Text="Send Alert to PIC" SkinID="XLButton" runat="server" CommandName="SendAlertToPic"  />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>
</asp:Content>
