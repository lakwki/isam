<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Title="Non-Trade Expense Statement List" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="NonTradeExpenseStatementList.aspx.cs" Inherits="com.next.isam.webapp.reporter.NonTradeExpenseStatementList" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel runat="server" SkinID="sectionHeader_Report">Non-Trade Expense Statement List</asp:Panel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript" >

    function isFormValid() {
        if (
        Content2.valueOf(
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_DueDateFrom_txt_DueDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_DueDateTo_txt_DueDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_NSRefNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_NSRefNoTo").value == "") {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Invoice Date\r\n- NS Ref No\r\n- Due Date ");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_DueDateFrom_txt_DueDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_DueDateTo_txt_DueDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_DueDateFrom_txt_DueDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_DueDateTo_txt_DueDateTo_textbox").value != "")) {
            alert("Invalid Due Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "")) {
            alert("Invalid Invoice Date.");
            return false;
        }

        return true;
    }
</script>

<table width="800px" cellspacing="2" cellpadding="2">        
        <tr>
            <td>
                <table width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="5">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Office&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                            <cc2:SmartDropDownList ID="ddl_Office" runat="server" />                            
                        </td>
                        <td style="width:1px;">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Invoice Date&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                             <cc2:SmartCalendar ID="txt_InvoiceDateFrom" FromDateControl="txt_InvoiceDateFrom" 
                            runat="server" ToDateControl="txt_InvoiceDateTo" />
                            &nbsp;To&nbsp;
                            <cc2:SmartCalendar ID="txt_InvoiceDateTo" FromDateControl="txt_InvoiceDateFrom"
                                ToDateControl="txt_InvoiceDateTo" runat="server" />  
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;NS Ref. No.</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_NSRefNoFrom" runat="server" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txt_NSRefNoTo" runat="server" />
                        </td>
                    </tr>
                    <tr style='display:none;'>
                        <td class="FieldLabel2">&nbsp;Expense Type</td>
                        <td>&nbsp;</td>
                        <td>
                            <cc2:SmartDropDownList ID="ddl_ExpenseType" runat="server" />                            
                        </td>
                        <td></td>
                    </tr>
                    <tr>
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
                    <tr style='display:none;'>
                        <td class="FieldLabel2">&nbsp;Invoice No.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_InvoiceNoFrom" runat="server" onblur="ctl00_ContentPlaceHolder1_txt_InvoiceNoTo.value = this.value;" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txt_InvoiceNoTo" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Vendor</td>
                        <td>&nbsp;</td>
                        <td>
                            <uc1:UclSmartSelection ID="txt_SupplierName" runat="server" />
                        </td>
                    </tr>             
                    <tr style='display:none;'>
                        <td class="FieldLabel2">&nbsp;Customer No</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_CustomerNoFrom" runat="server" onblur="ctl00_ContentPlaceHolder1_txt_CustomerNoTo.value = this.value;" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txt_CustomerNoTo" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Status</td>
                        <td>&nbsp;</td>
                        <td>
                            <cc2:SmartDropDownList ID="ddl_Status" runat="server" />                            
                        </td>                   
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                    </tr>       
                    <tr>
                        <td colspan="5">
                            <asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton" OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
                            <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>