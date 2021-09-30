<%@ Page Title="Non-Trade Role Maintenance" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="NTRoleEdit.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NTRoleEdit" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Non-Trade Role Edit</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">

    function showProgressBarX() {
        var left = window.screen.availWidth / 3 - 100;
        var top = window.screen.availHeight / 3 - 100
        waitDialog = window.showModelessDialog("../common/ProgressBarX.htm", window, "dialogHeight: 30px; dialogWidth: 200px; center:yes; edge: sunken; center: Yes; help: No; resizable: yes; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
    }

</script>
<table>
    <tr>
        <td colspan="2"><b><asp:validationsummary id="vs" runat="server"></asp:validationsummary></b></td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Role Id</td>
        <td><asp:Label runat="server" ID="lbl_RoleId" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Role Name</td>
        <td><asp:Label runat="server" ID="lbl_RoleName" /></td>
    </tr>
</table>
<br />
<TABLE cellSpacing="0" cellPadding="2" border="0">
	<TR>
		<TD>User:</TD>
		<TD width="100%">
			<uc1:UclSmartSelection ID="txtUser" runat="server" /></TD>
	</TR>

	<TR>
		<TD>Company:</TD>
		<TD width="100%">
			<cc2:smartdropdownlist id="ddl_Company" runat="server" Width="200px"></cc2:smartdropdownlist></TD>
	</TR>
	<TR>
		<TD>Office:</TD>
		<TD width="100%">
			<cc2:smartdropdownlist id="ddl_Office" runat="server" Width="200px"></cc2:smartdropdownlist></TD>
	</TR>
    <tr>
        <td></td>
        <td><asp:button id="lnkAdd" runat="server" CausesValidation="True" Text="Add" 
                onclick="lnkAdd_Click"></asp:button>
            <asp:CustomValidator ID="valCustom" runat="server" Display="None" 
                onservervalidate="valCustom_ServerValidate"></asp:CustomValidator>
        </td>
    </tr>
</TABLE>
<br />

<asp:GridView ID="gv_User" runat="server" AutoGenerateColumns="false" 
        onrowcommand="gv_User_RowCommand" onrowdatabound="gv_User_RowDataBound" 
        onrowdeleting="gv_User_RowDeleting" AllowSorting="True" 
        onsorting="gv_User_Sorting">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:ImageButton ID="lnk_Delete" runat="server" ImageUrl="~/images/icon_delete.gif" CommandName="Delete"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="User" SortExpression="User" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_User" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Company" SortExpression="Company" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_Company" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Office" SortExpression="Office" HeaderStyle-ForeColor="DarkBlue">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>   

</asp:Content>
