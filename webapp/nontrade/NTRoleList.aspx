<%@ Page Title="Non-Trade Role Maintenance" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="NTRoleList.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NTRoleList" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Non-Trade Role List</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">

    function showProgressBarX() {
        var left = window.screen.availWidth / 3 - 100;
        var top = window.screen.availHeight / 3 - 100
        waitDialog = window.showModelessDialog("../common/ProgressBarX.htm", window, "dialogHeight: 30px; dialogWidth: 200px; center:yes; edge: sunken; center: Yes; help: No; resizable: yes; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
    }

</script>
<br />
<asp:GridView ID="gv_NTRole" runat="server" AutoGenerateColumns="false" 
    onrowcommand="gv_NTRole_RowCommand" onrowdatabound="gv_NTRole_RowDataBound">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:ImageButton ID="lnk_Edit" runat="server" ImageUrl="~/images/icon_edit.gif" CommandName="Edit"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Role Id">
            <ItemTemplate>
                <asp:Label ID="lbl_RoleId" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Role Name">
            <ItemTemplate>
                <asp:Label ID="lbl_RoleName" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>   

</asp:Content>
