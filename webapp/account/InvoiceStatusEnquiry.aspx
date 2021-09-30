<%@ Page Language="C#" Theme="DefaultTheme" MaintainScrollPositionOnPostback="true"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="InvoiceStatusEnquiry.aspx.cs" Inherits="com.next.isam.webapp.account.InvoiceStatusEnquiry" Title="Payment Document Enquiry" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<%@ Register Src="~/usercontrol/UclSortingOrder.ascx" TagName="UclSortingOrder" TagPrefix="uso" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript">
    function copyNSLInvoiceNo() {
        document.all.<%= txt_InvoiceNoFrom.ClientID %>.value = document.all.<%= txt_InvoiceNoFrom.ClientID %>.value.toUpperCase();
        document.all.<%= txt_InvoiceNoTo.ClientID %>.value = document.all.<%= txt_InvoiceNoFrom.ClientID %>.value;
    }
    
    function isValidSearch() {
        if (document.all.<%= txt_InvoiceDateFrom.ClientID %>_txt_InvoiceDateFrom_textbox.value != '' || 
            document.all.<%= txt_InvoiceNoFrom.ClientID %>.value != '' ||
            document.all.<%= txt_SubDocDateFrom.ClientID %>_txt_SubDocDateFrom_textbox.value != '' ||
            document.all.<%= txt_InterfaceDateFrom.ClientID %>_txt_InterfaceDateFrom_textbox.value != '' ||
            document.all.<%= txt_ContractNo.ClientID %>.value != '')
            return true;
        else
        {
            alert('Please input at least the [Invoice Date / Accounts Doc. Receipt Date / Epicor Interface Date / Invoice No / Contract No]');
            return false;
        }
    }

    function isValidTrigger() {
        if (document.all.<%= ddl_Office.ClientID %>.value == '-1')
        {
            alert('Please select Office');
            return false;
        }
        else
        {
            return confirm('Are you sure to process?');
        }
    }

    function openHoldPaymentBySupplierWindow(vendorId) {
        window.open('HoldPaymentSupplier.aspx?vendorId=' +  vendorId, '', 'height=400,width=400,center=yes,resizable=no,status=no,scroll=no');
    }
    
    function runSearch()
    {
        document.all.<%= btn_Search.ClientID %>.click();
    }
    
    function getRejectReason() {
        var id = window.showModalDialog('GetRejectReason.aspx', '', 'dialogWidth=300px;dialogHeight=150px;resizable=no;status=no;scroll=no');
        var obj = document.getElementById('<%= hid_RejectReasonId.ClientID %>');
        if (id==null || id<0) {
            // not select any reason
            obj.value = '-1'
            alert ('Rejection aborted!');
            return false;
        }
        else {
            obj.value = id.toString();
            return true;
        }
    }
    
    function openAttachments(o, sid)
    {
        resetHighlightEntry(o);
        highlightEntry(o);
        window.open('AttachmentList.aspx?shipmentId=' + sid ,'attachmentlist','scrollbars=1,status=0,width=400,height=300');
    }       
</script>
<table width="800px">
    <tr>
        <td class="tableHeader">Payment Document Enquiry</td>
    </tr>
</table>
<table width="800px">
    <tr>
        <td colspan="2">
            <table>  
            <tr>
                <td colspan="4">
                    <table style="margin-left:10px;" cellpadding="5" cellspacing="0" >
                        <tr>
                            <td class="FieldLabel2">Invoice Date</td>
                            <td>
                                <cc2:SmartCalendar ID="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" runat="server" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_InvoiceDateTo" FromDateControl="txt_InvoiceDateFrom" runat="server" />
                            </td>
                            <td class="FieldLabel2">Office</td>
                            <td>
                                <cc2:SmartDropDownList ID="ddl_Office" runat="server" width="125px"/>
                            </td>              
                        </tr>
                    
                        <tr>
                            <td class="FieldLabel2">Invoice No.</td>
                            <td><asp:TextBox ID="txt_InvoiceNoFrom" runat="server" onblur="copyNSLInvoiceNo();" /> To <asp:TextBox ID="txt_InvoiceNoTo" runat="server" /></td>
                            <td class="FieldLabel2">Handling Office</td>
                            <td><cc2:SmartDropDownList ID="ddl_HandlingOffice" runat="server" width="125px" /></td>                            
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Accounts Doc. Receipt Date</td>
                            <td>
                                <cc2:SmartCalendar ID="txt_SubDocDateFrom" ToDateControl="txt_SubDocDateTo" runat="server" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_SubDocDateTo" FromDateControl="txt_SubDocDateFrom" runat="server" />
                            </td>
                            <td class="FieldLabel2" >Trading Agency</td>
                            <td>
                                <cc2:SmartDropDownList ID="ddl_TradingAgency" runat="server" width="125px"/>
                            </td>                                      
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Epicor Interface Date</td>
                            <td>
                                <cc2:SmartCalendar ID="txt_InterfaceDateFrom" ToDateControl="txt_InterfaceDateTo" runat="server" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_InterfaceDateTo" FromDateControl="txt_InterfaceDateFrom" runat="server" />
                            </td>
                            <td class="FieldLabel2" >Payment Term</td>
                            <td>
                                <cc2:SmartDropDownList ID="ddl_PaymentTerm" runat="server" width="125px"/>
                            </td>                                      
                        </tr>
                        <tr>
                            <td class="FieldLabel2">Supplier</td>
                            <td>
                                <uc1:UclSmartSelection  ID="txt_SupplierName" runat="server" width="300px"/>
                            </td>
                            <td class="FieldLabel2" >Contract No.</td>
                            <td>
                                <asp:TextBox ID="txt_ContractNo" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" class="CellWithBorder">
                                <table border="0">
                                    <tr>
                                        <td class="FieldLabel2">Status</td>
                                        <td>
                                            <asp:CheckBox runat="server" ID="chkNotReady" Text="Not Ready To Check"/>&nbsp;&nbsp;&nbsp;
                                            <asp:CheckBox runat="server" ID="chkReady" Text="Document Ready To Check"/>&nbsp;&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:CheckBox runat="server" ID="chkAccepted" Text="Accepted"/>&nbsp;&nbsp;&nbsp;
                                            <asp:CheckBox runat="server" ID="chkRejected" Text="Rejected"/>&nbsp;&nbsp;&nbsp;
                                            <asp:CheckBox runat="server" ID="chkReviewed" Text="Reviewed"/>&nbsp;&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4"><asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" />&nbsp;&nbsp;
                                <asp:Button ID="btn_Reset" runat="server" Text="Reset" OnClick="btn_Reset_Click" />&nbsp;&nbsp;
                                <asp:Button ID="btn_Excel" runat="server" Text="Export To Excel" SkinID="LButton" onclick="btn_Excel_Click" OnClientClick="return isValidSearch();" />&nbsp;&nbsp;
                                <asp:Button ID="btn_ExcelToEpicor" runat="server" Text="Export To Epicor" SkinID="LButton" onclick="btn_ExcelToEpicor_Click" OnClientClick="return isValidSearch();" ToolTip="Extract the reviewed and not yet pay Invoice" />&nbsp;&nbsp;
                                <asp:Button ID="btn_TriggerDMSComplete" runat="server" 
                                    Text="Trigger DMS Complete" SkinID="XLButton" 
                                    ToolTip="It tells the system to run a process so that daily purchase interface can include reviewed shipments" 
                                    onclick="btn_TriggerDMSComplete_Click" OnClientClick="return isValidTrigger();"/>
                            </td>
                        </tr>
                   </table>            
                </td>
            </tr>
            </table>        
        </td>
    </tr>
</table>

<br />
<asp:Panel ID="pnl_Result" runat="server" Visible="false" >
<asp:Label ID="lbl_Warning" runat="server" Visible="false" style="color:#ff9900; font-weight :bolder;" Text="Only the first 500 records are displayed" /><br />
<br />
&nbsp;&nbsp;
<asp:ImageButton id="imgSelectAll" runat="server" ImageUrl="../images/icon_selectall.jpg" AlternateText="Select All" OnClientClick="CheckAll('ckb_inv'); return false;" />
&nbsp;&nbsp;
<asp:ImageButton id="imgDeSelectAll" runat="server" ImageUrl="../images/icon_deselectall.jpg" AlternateText="De-Select All" OnClientClick="UncheckAll('ckb_inv'); return false;" />
&nbsp;&nbsp;
<asp:HiddenField ID="hid_RejectReasonId" runat="server" Value="" />
<asp:Button ID="btn_Reject" runat="server" Text="Reject" onclientclick="return getRejectReason();" 
        onclick="btn_Reject_Click" />&nbsp;&nbsp;
<asp:Button ID="btn_Accept" runat="server" Text="Accept" 
        onclick="btn_Accept_Click" />
&nbsp;&nbsp;
<asp:Button ID="btn_Hold" runat="server" Text="Hold" onclick="btn_Hold_Click" Visible="false"/>
&nbsp;&nbsp;
<asp:Button ID="btn_Unhold" runat="server" Text="Unhold" Visible="false" onclick="btn_Unhold_Click" />         
<asp:GridView ID="gv_Inv" runat="server" AutoGenerateColumns ="false" 
        OnRowDataBound="InvDataBound" onrowcommand="gv_Inv_RowCommand" 
        AllowSorting="True" onsorting="gv_Inv_Sorting">
    <Columns >
        <asp:TemplateField HeaderText="">
            <ItemTemplate >
                <asp:CheckBox ID="ckb_inv" runat="server" />&nbsp;
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supplier Name" SortExpression="SupplierName" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate >
                <asp:LinkButton ID="lnk_SupplierName" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="">
            <ItemTemplate >
                <asp:ImageButton runat="server" CommandName="OpenAttachment" ID="imgOpen" ImageUrl="~/images/icon_edit.gif" />&nbsp;
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Prod Team" runat="server">
            <ItemTemplate >
                <asp:Label ID="lbl_ProdTeamCode" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Prod Team Desc" runat="server" Visible="false">
            <ItemTemplate >
                <asp:Label ID="lbl_ProdTeamDesc" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Contract No." SortExpression="ContractNo" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate >
                <asp:Label ID="lbl_ContractNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" >
            <ItemTemplate>
                <asp:Image ID="img_GBTestRequired" ImageUrl="~/images/icon_Test.png" Visible="false" ToolTip="GB Test Required" runat="server" />
                <asp:Image ID="img_GBTestPassed" ImageUrl="~/images/icon_Pass.png" Visible="false" ToolTip="GB Test Pass" runat="server" />
                <asp:Image ID="img_GBTestFailedRelease" ImageUrl="~/images/icon_Fail.png" Visible="false" ToolTip="GB Test Fail (Release Payment)" runat="server" />
                <asp:Image ID="img_GBTestFailedHold" ImageUrl="~/images/icon_Fail_orange.png" Visible="false" ToolTip="GB Test Fail (Hold Payment)" runat="server" />
                <asp:Image ID="img_GBTestFailedCannotRelease" ImageUrl="~/images/icon_Fail_red.png" Visible="false" ToolTip="GB Test Fail (Cannot Release Payment)" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Item No." SortExpression="ItemNo" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate >
                <asp:Label ID="lbl_ItemNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Invoice No." SortExpression="InvoiceNo" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_invNo" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Invoice Date" SortExpression="InvoiceDate" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_invDate" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Supplier Invoice No." SortExpression="SupplierInvoiceNo" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_SupplierInvoiceNo" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="Currency">
            <ItemTemplate>
                <asp:Label ID="lbl_Currency" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Net Amt">
            <ItemTemplate >
                <asp:Label ID="lbl_NetAmt" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Net Amt (USD)" SortExpression="NetAmtInUSD" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate >
                <asp:Label ID="lbl_NetAmtInUSD" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate >
                <asp:Label ID="lbl_PaymentStatus" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText="Lock Applied">
            <ItemTemplate>
                <asp:Label ID="lbl_LockReleased" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText="Hold Payment?">
            <ItemTemplate>
                <asp:Label ID="lbl_IsPaymentHold" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText="Remark">
            <ItemTemplate>
                <asp:Label ID="lbl_PaymentHoldRemark" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="LG Due Date">
            <ItemTemplate>
                <asp:Label ID="lbl_LGDueDate" runat ="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText="Deductions" >
            <ItemTemplate>
                <asp:Label ID="lbl_DeductionAmt" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>    
    </asp:GridView>
</asp:Panel>
</asp:Content>
