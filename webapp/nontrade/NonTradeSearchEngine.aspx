<%@ Page Title="Non-Trade Expense - Invoice" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme"  AutoEventWireup="true" CodeBehind="NonTradeSearchEngine.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NonTradeSearchEngine" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Accounts">Non-Trade Expense Invoice Search Engine</asp:Panel>
<script type="text/javascript" >
    function openInvoiceWindow(invoiceId) {
        if (invoiceId == 0)
            window.open('NonTradeInvoice.aspx', 'NTInvoice', 'width=900,height=900, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();
        else
            window.open('NonTradeInvoice.aspx?InvoiceId=' + invoiceId, 'NTInvoice', 'width=900,height=900, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();

    }
    function ToggleSelection(isSelected) {

        var nodeList = document.getElementsByTagName("input");

        for (i = 0; i < nodeList.length; i++) {
            if (nodeList[i].type == "checkbox")
                if (!(nodeList[i].disabled))
                    nodeList[i].checked = isSelected;
        }
    }   

</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="div_monthEnd" runat="server" visible="false">
    <span style="color:#009933; font-weight :bold; margin : 10px; font-size : 12pt;"><asp:Label ID="lbl_Office" runat="server" EnableTheming="false"  /> office has started the month end process. Invoice cannot be modified and submitted at the moment.</span>
</div>
<table width="900px" cellspacing="2" cellpadding="2">        
        <tr>
            <td>
                <table width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td style="width:100px">&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                        <td style="width:100px">&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Office&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                            <cc2:SmartDropDownList ID="ddl_Office" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Office_SelectedIndexChanged" />                            
                        </td>
                        <td style="width:1px;">&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">Invoice Date</td>
                        <td>&nbsp;</td>
                        <td colspan="1"><asp:RadioButton ID="rad_InvoiceDate" runat="server" Checked="true" AutoPostBack="true" GroupName="rad_SelectCriteria" OnCheckedChanged="rad_InvoiceDate_CheckedChanged" Text="By Range : " />
                            &nbsp;
                            <cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />&nbsp;To&nbsp;
                            <cc2:SmartCalendar id="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>

                    <tr>
                        <td></td>
                        <td>&nbsp;</td>
                        <td colspan="1"><asp:RadioButton ID="rad_FiscalPeriod" runat="server" AutoPostBack="true" GroupName="rad_SelectCriteria" OnCheckedChanged="rad_FiscalPeriod_CheckedChanged" Text="By Period : " />
                            &nbsp;Year&nbsp;
                            <asp:DropDownList  ID="ddl_Year" runat="server" SkinId="XSDDL" Enabled ="false"  />&nbsp;&nbsp;&nbsp;Period&nbsp;
                            <asp:DropDownList id="ddl_PeriodFrom" runat="server" SkinID= "XSDDL" Enabled ="false" AutoPostBack="True" >
                                <asp:ListItem Text="1" Value="1"  />
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
                            &nbsp;To &nbsp;
                            <asp:DropDownList id="ddl_PeriodTo" runat="server" SkinID="XSDDL" Enabled ="false" AutoPostBack="True" >
                                <asp:ListItem Text="1" Value="1"  />
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
                        <td></td>
                    </tr>

                    <tr>
                        <td class="FieldLabel2">&nbsp;Expense Type</td>
                        <td>&nbsp;</td>
                        <td>
                            <cc2:SmartDropDownList ID="ddl_ExpenseType" runat="server" />
                        </td>
                        <td></td>
                        <td class="FieldLabel2">&nbsp;Due Date</td>
                        <td>&nbsp;</td>
                        <td>
                            <cc2:SmartCalendar ID="txt_DueDateFrom" FromDateControl="txt_DueDateFrom" 
                                ToDateControl="txt_DueDateTo" runat="server" />
                            &nbsp;To&nbsp;
                            <cc2:SmartCalendar ID="txt_DueDateTo" FromDateControl="txt_DueDateFrom"
                                ToDateControl="txt_DueDateTo" runat="server" />                        
                        </td>
                    </tr>                    
                    <tr>
                        <td class="FieldLabel2">&nbsp;Invoice No.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_InvoiceNoFrom" runat="server" onblur="ctl00_ContentPlaceHolder1_txt_InvoiceNoTo.value = this.value;" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txt_InvoiceNoTo" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2">&nbsp;Vendor</td>
                        <td>&nbsp;</td>
                        <td>
                            <uc1:UclSmartSelection ID="txt_SupplierName" runat="server" />
                        </td>
                    </tr>             
                    <tr>
                        <td class="FieldLabel2">&nbsp;Customer No</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_CustomerNoFrom" runat="server" onblur="ctl00_ContentPlaceHolder1_txt_CustomerNoTo.value = this.value;" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txt_CustomerNoTo" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2">&nbsp;Submitted By</td>
                        <td>&nbsp;</td>
                        <td>
                            <uc1:UclSmartSelection ID="txt_SubmittedBy" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;NSL Ref. No.</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_NSLRefNoFrom" runat="server" onblur="ctl00_ContentPlaceHolder1_txt_NSLRefNoTo.value = this.value;" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txt_NSLRefNoTo" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class=" FieldLabel2_T" style="vertical-align:top ;">Status</td>
                        <td>&nbsp;</td>
                        <td colspan="5">
                            <table><tr>
                                <td class="CellWithBorder">
                                    <asp:CheckBoxList ID="cbl_Status" runat="server" TextAlign="Right" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="4"/>
                                </td>    
                                <td valign="top">
                                    <img alt="SelectAll" src="../images/icon_select_all.png" title="select all" onclick="ToggleSelection(true);" />&nbsp;&nbsp;&nbsp;
                                    <img alt="DeSelectAll" src="../images/icon_clear_all.png" title="clear all" onclick="ToggleSelection(false);" />
                                </td>
                            </tr>
                            </table>
                        </td> 
                        <td>&nbsp;</td>
                    </tr>

                    <tr>
                        <td>&nbsp;</td>
                    </tr>       
                    <tr>
                        <td colspan="5">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="Search" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnReset" runat="server" Text="Reset" OnClick="Reset_OnClick"  />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnNew" runat="server" Text="New" OnClientClick="openInvoiceWindow(0); return false;" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnExport" runat="server" Text="Export"  OnClick="ExportDetail" />
                        </td>
                    </tr>
                </table>
            
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
            <asp:Panel ID="pnl_Result" runat="server" Visible="false" >
            <div id="div_Over200" runat="server" style="margin:10px; color:Orange; font-weight:bold;" visible="false">
                            There are more than 200 non-trade invoices matching your search criteria.<br />
                            Only the first 200 search result are shown.
             </div>          
            <table style="border: 1px solid #B0B0B0;" cellspacing="0" cellpadding="0">
                <tr>
                    <td>                    
                <asp:GridView ID="gv_Invoice" runat="server" OnRowDataBound="InvoiceRowDataBound" AllowSorting="true" OnSorting="InvoiceOnSort">
                    <Columns >
                        <asp:TemplateField Visible="false" >
                            <ItemTemplate >
                                <asp:CheckBox ID="ckb_Invoice" runat="server" />
                            </ItemTemplate>                                                       
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NSL Ref. No.">
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_NSLRefNo" runat="server" Text='<%# Eval("NSLInvoiceNo") %>' />
                            </ItemTemplate>
                            <ItemStyle width="90px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice No."  SortExpression="InvoiceNo" HeaderStyle-ForeColor="CornflowerBlue">                            
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_InvoiceNo" runat="server" Text='<%# Eval("InvoiceNo") %>' />
                            </ItemTemplate>                            
                            <ItemStyle HorizontalAlign="Left" Width="100px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer No.">
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_CustomerNo" runat="server" Text='<%# Eval("CustomerNo") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" Width="100px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date" ItemStyle-Width="70px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_InvoiceDate" runat="server" Text='<%# Eval("InvoiceDate", "{0:d}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Office" ItemStyle-Width="50px" SortExpression="Office" HeaderStyle-ForeColor="CornflowerBlue">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Office" runat="server" Text='<%# Eval("Office.OfficeCode") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor" SortExpression="Vendor" HeaderStyle-ForeColor="CornflowerBlue">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Vendor" runat="server" Text='<%# Eval("NTVendor.VendorName") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor A/C Code">
                            <ItemTemplate >
                                <asp:Label ID="lbl_VendorSunAccountCode" runat="server" Text='<%# Eval("NTVendor.SunAccountCode") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" Width="100px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Currency" ItemStyle-Width="60px">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Currency" runat="server" Text='<%# Eval("Currency.CurrencyCode") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total Amount" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Amount" runat="server" Text='<%# Eval("Amount") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total VAT" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_VAT" runat="server" Text='<%# Eval("TotalVAT") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Due Date" ItemStyle-Width="70px">
                            <ItemTemplate >
                                <asp:Label ID="lbl_DueDate" runat="server" Text='<%# Eval("DueDate", "{0:d}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Status" runat="server" Text='<%# Eval("WorkflowStatus.Name") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>               
                        <asp:TemplateField HeaderText="Submitted By" SortExpression="Requester" HeaderStyle-ForeColor="CornflowerBlue">
                            <ItemTemplate >
                                <asp:Label ID="lbl_CreatedBy" runat="server" Text='<%# Eval("SubmittedBy.DisplayName") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" Width="90px" />
                        </asp:TemplateField>          
                        <asp:TemplateField HeaderText="First Approver" SortExpression="FirstApprover" HeaderStyle-ForeColor="CornflowerBlue">
                            <ItemTemplate >
                                <asp:Label ID="lbl_FirstApprover" runat="server" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" Width="90px" />
                        </asp:TemplateField>         
                        <asp:TemplateField HeaderText="Debit Note">
                            <ItemTemplate>
                                <asp:Label ID="lbl_HasDebitNote" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>    
                        <asp:TemplateField HeaderText="Fixed Assets">
                            <ItemTemplate>
                                <asp:Label ID="lbl_IsFixedAsset" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>    
                    </Columns>                    
                    <EmptyDataTemplate>
                        No record found.
                    </EmptyDataTemplate>
                </asp:GridView>
                </td>
                </tr>
            </table>
            </asp:Panel>
            </td>
        </tr>
    </table>

<script type="text/javascript" >

    function setTextBoxWidth(objectId, width) {
        //debugger;
        input = document.getElementsByTagName("input");
        for (i = 0; i < input.length; i++)
            if ((input[i].id).indexOf(objectId) >= 0) {
                obj = input[i];
                break;
            }
        obj.style.width = width.toString() + "px";
    }

    setTextBoxWidth("txt_InvoiceDateFrom", 90);
    setTextBoxWidth("txt_InvoiceDateTo", 90);
    setTextBoxWidth("txt_DueDateFrom", 90);
    setTextBoxWidth("txt_DueDateTo", 90);

</script>

</asp:Content>

