<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKClaimSearch.aspx.cs" Inherits="com.next.isam.webapp.claim.UKClaimSearch" Title="Search Page For Next Claim" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Next Claim Portal</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">

    function isValidSearch() {
        if (document.all.<%= txt_DateFrom.ClientID %>_txt_DateFrom_textbox.value != '' || 
            document.all.<%= txt_UKDebitNoteNo.ClientID %>.value != '' ||
            document.all.<%= txt_ReceivedDateFrom.ClientID %>_txt_ReceivedDateFrom_textbox.value != '' || 
            document.all.<%= txt_ItemNo.ClientID %>.value != '' ||
            document.all.<%= ddl_ClaimType.ClientID %>.value == '6')
            return true;
        else
        {
            alert('Please input at least the [Next Debit Note No. / Next Debit Note Date / Next Debit Note Received Date / Item No.]');
            return false;
        }        
    }  
    function ToggleSelection(isSelected) {

        var nodeList = document.getElementsByTagName("input");

        for (i = 0; i < nodeList.length; i++) {
            if (nodeList[i].type == "checkbox")
                if (!(nodeList[i].disabled))
                    nodeList[i].checked = isSelected;
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
                <td colspan="4">
                    <table style="margin-left:10px;" cellpadding="5" cellspacing="0" >
                        <tr>
                            <td class="FieldLabel2">Office / <asp:Label runat="server" Text="H.Office" ToolTip="Handling Office" ID="lblHandlingOffice" /></td>
                            <td>
                                <cc2:SmartDropDownList ID="ddl_Office" runat="server" width="60px"/>&nbsp;
                                <cc2:SmartDropDownList ID="ddl_HandlingOffice" runat="server" width="60px"/>&nbsp;
                            </td>    
                            <td class="FieldLabel2">Order Type</td>
                            <td>
                                <cc2:SmartDropDownList ID="ddl_OrderType" runat="server" width="125px"/>
                                <asp:ImageButton runat="server" ImageUrl="../images/icon_excel.jpg" 
                                    ID="btnOSReport" ToolTip="O/S Report" onclick="btnOSReport_Click" Visible="false"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel2" >Claim Type</td>
                            <td>
                                <cc2:SmartDropDownList ID="ddl_ClaimType" runat="server" width="125px"/>
                            </td>          
                            <td class="FieldLabel2">Next D/N No</td>
                            <td><asp:TextBox ID="txt_UKDebitNoteNo" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Next D/N Date</td>
                            <td colspan="3">
                                <cc2:SmartCalendar ID="txt_DateFrom" ToDateControl="txt_DateTo" runat="server" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_DateTo" FromDateControl="txt_DateFrom" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Next D/N Rec'd Date</td>
                            <td colspan="3">
                                <cc2:SmartCalendar ID="txt_ReceivedDateFrom" ToDateControl="txt_ReceivedDateTo" runat="server" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_ReceivedDateTo" FromDateControl="txt_ReceivedDateFrom" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Item No</td>
                            <td>
                                <asp:TextBox ID="txt_ItemNo" runat="server" />
                            </td>
                            <td class="FieldLabel2" >Contract No</td>
                            <td>
                                <asp:TextBox ID="txt_ContractNo" runat="server" />
                            </td>          
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Supplier</td>
                            <td colspan="3">
                                <uc1:uclsmartselection  ID="txt_Supplier" runat="server" width="300px"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel2_T" valign="top">Status</td>
                            <td colspan="3">
                                <asp:CheckBoxList runat="server" ID="cblStatus" RepeatDirection="Horizontal" RepeatColumns="3"/>
                                <img alt="SelectAll" src="../images/icon_select_all.png" title="Select All" onclick="ToggleSelection(true);" />&nbsp;&nbsp;&nbsp;
                                <img alt="DeSelectAll" src="../images/icon_clear_all.png" title="De-Select All" onclick="ToggleSelection(false);" />
                            </td>
                            
                        </tr>
                        <tr>
                            <td colspan="4"><asp:Button ID="btn_Search" runat="server" Text="Search" 
                                    onclick="btn_Search_Click" />&nbsp;
                                <asp:Button ID="btn_Reset" runat="server" Text="Reset" 
                                    onclick="btn_Reset_Click" />&nbsp;
                                <asp:Button ID="btn_Excel" runat="server" Text="Export To Excel" SkinID="LButton" 
                                    onclick="btn_Excel_Click"/>&nbsp;
                                <asp:Button ID="btn_Create" runat="server" Text="New" 
                                    onclick="btn_Create_Click"/>&nbsp;
                                <asp:Button runat="server" Text="Mail D/N(s)" id="btnMail" SkinID="LButton" 
                                    onclick="btnMail_Click"/>
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
        onrowcommand="gv_UKClaim_RowCommand" onrowdatabound="gv_UKClaim_RowDataBound" >
    <Columns>
       <asp:TemplateField HeaderText="">
            <HeaderTemplate>
                <input id="chkAll" onclick="javascript:ToggleMailSelection(this, 'chkMail');" type="checkbox" title="Select All or Clear All" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkMail" runat="server" />
            </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/images/icon_edit.gif" ToolTip="Edit" CommandName="EditUKClaim"/>
            </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/images/icon_delete.gif" ToolTip="Delete" CommandName="DeleteUKClaim"/>
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Office">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Type">
            <ItemTemplate>
                <asp:Label ID="lbl_ClaimType" runat="server" />
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
        <asp:TemplateField HeaderText="NS Cost %">
            <ItemTemplate>
                <asp:Label ID="lbl_NSPercent" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Vendor %">
            <ItemTemplate>
                <asp:Label ID="lbl_VendorPercent" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

       <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <asp:Label ID="lbl_WorkflowStatus" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</asp:GridView>   

</asp:Content>
