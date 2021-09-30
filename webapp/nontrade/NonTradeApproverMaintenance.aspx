<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" EnableEventValidation="false"  Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="NonTradeApproverMaintenance.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NonTradeApproverMaintenance" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Accounts">Non-Trade Expense</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<span class="header2" style="margin:10px;">Non-Trade Approver Maintenance</span><br />
<table style="margin: 10px;" cellspacing="0" cellpadding="0">
<tr>
    <td class="FieldLabel2">&nbsp;Office</td>
    <td>&nbsp;</td>
    <td><cc2:SmartDropDownList ID="ddl_Office" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel2">&nbsp;Approver</td>
    <td></td>
    <td><uc1:UclSmartSelection ID="txt_Approver" runat="server" AutoPostBack="true" OnSelectionChanged="txt_Approver_change" /></td>
</tr>
<tr id="row_department" runat="server" visible="false" >
    <td class="FieldLabel2">&nbsp;User Department</td>
    <td></td>
    <td><asp:TextBox ID="txt_Department" runat="server" SkinID="XLTextBox" CssClass="readOnlyField" onfocus="this.blur();"  /></td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td><asp:Button ID="btn_add" runat="server" Text="Add" OnClick="AddApprover_Click" /></td>
</tr>
</table>
<table style="border: 1px solid #999999; margin :10px;" cellspacing="0" cellpadding="0">
<tr><td>
<asp:GridView ID="gv_Approver" runat="server" OnRowCommand="ApproverRowCommand">
    <Columns >
        <asp:TemplateField HeaderText="Office" ItemStyle-Width="80px">
            <ItemTemplate >
                <%# DataBinder.Eval(Container, "DataItem.Office.OfficeCode")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Approver" ItemStyle-Width="300px" ItemStyle-HorizontalAlign="Left" >
            <ItemTemplate>
                <asp:ImageButton ID="btn_Delete" runat="server" CommandArgument='<%#  DataBinder.Eval(Container, "RowIndex") %>' ImageUrl="../images/icon_remove.gif" ImageAlign="Middle"
                    OnClientClick="return confirm('Are you sure to remove this approver?');" CommandName="removeApprover" CausesValidation="false"  />
                <%# DataBinder.Eval(Container, "DataItem.Approver.DisplayName")%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</td></tr>
</table>

</asp:Content>