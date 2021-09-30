<%@ Page Title="Non-Trade Expense - Debit Note" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="DebitNoteSearch.aspx.cs" Inherits="com.next.isam.webapp.nontrade.DebitNoteSearch" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Non-Trade Expense Recharge Debit Note Search Engine</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">

    function showProgressBarX() {
        var left = window.screen.availWidth / 3 - 100;
        var top = window.screen.availHeight / 3 - 100
        waitDialog = window.showModelessDialog("../common/ProgressBarX.htm", window, "dialogHeight: 30px; dialogWidth: 200px; center:yes; edge: sunken; center: Yes; help: No; resizable: yes; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
    }

    function openInvoiceWindow(invoiceId) {
        if (invoiceId == 0)
            window.open('NonTradeInvoice.aspx', 'NTInvoice', 'width=900,height=900, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();
        else
            window.open('NonTradeInvoice.aspx?InvoiceId=' + invoiceId, 'NTInvoice', 'width=900,height=900, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();

    }
    function CheckAll(oCheckbox) {
        var gv_Request = document.getElementById("<%=gv_Request.ClientID %>");
        for (i = 1; i < gv_Request.rows.length; i++) {
            var cb = gv_Request.rows[i].cells[0].getElementsByTagName("INPUT")[0];
            if (cb) {
                cb.checked = oCheckbox.checked;
            }
        }
    }

</script>
<table>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Office</td>
        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="200px"></cc2:smartdropdownlist></td>
    </tr>
    <tr runat="server">
        <td class="FieldLabel2">Issue Date</td>
        <td>
            <asp:RadioButton ID="radMonth" runat="server" GroupName="Date" Text="Month"/>&nbsp;
            <cc2:smartdropdownlist id="ddl_Year" runat="server" Width="80px"></cc2:smartdropdownlist>&nbsp;
            <cc2:smartdropdownlist id="ddl_Month" runat="server" Width="80px"/>
            <br />
            <asp:RadioButton ID="radPeriod" runat="server" GroupName="Date" Text="Period"/>&nbsp;
            <cc2:smartdropdownlist id="ddl_FiscalYear" runat="server" Width="80px"></cc2:smartdropdownlist>&nbsp;
            <cc2:smartdropdownlist id="ddl_Period" runat="server" Width="80px"/>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td>
            <asp:Button ID="btn_Search" runat="server" Text="Search" onclick="btn_Search_Click" />
        </td>
        <td>
            <asp:Button 
                ID="btn_SendMail" runat="server" Text="Send Mail" 
                onclick="btn_SendMail_Click" SkinID="LButton" Visible="false" />&nbsp;<asp:CheckBox ID="chkSupportingDoc" runat="server" Text="Include Supporting Document" Checked="false" Visible="false" />
        </td>
    </tr>
</table>
<br /><br />
<asp:GridView ID="gv_Request" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_Request_RowDataBound" 
        onrowcommand="gv_Request_RowCommand">
    <Columns>
        <asp:TemplateField HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" Visible="true">
            <HeaderTemplate>
                <%--<asp:CheckBox ID="chb_All" runat="server" />--%>
                <input id="chb_All" type="checkbox" onclick="CheckAll(this)" runat="server" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chb_Mail" runat="server" CommandName="SendEmail" Visible="true" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:LinkButton ID="lnk_Excel" runat="server" CommandName="OutputExcel"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="DR Note No." ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:LinkButton ID="lnk_DebitNoteNo" runat="server" CommandName="OutputPDF"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Office" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="IssueDate" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_IssueDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Issued To" ControlStyle-Width="200px" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_IssuedTo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Currency" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_Currency" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total Amt" ControlStyle-Width="100px" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_TotalAmt" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Invoice No." ControlStyle-Width="150px" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                    <asp:panel ID="pnl_Invoices" runat="server" ></asp:panel>
             </ItemTemplate>
        </asp:TemplateField>
            <asp:TemplateField HeaderText="Vendor Email" ItemStyle-VerticalAlign="Top" Visible="true">
                <ItemTemplate>
                    <asp:Label ID="lbl_VendorEmail" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
    </Columns>
</asp:GridView>   

</asp:Content>
