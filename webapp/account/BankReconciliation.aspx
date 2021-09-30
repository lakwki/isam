<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="BankReconciliation.aspx.cs" Inherits="com.next.isam.webapp.account.BankReconciliation" Title="Payment Record Update" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Payment Record Update</asp:Panel>     
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table width="800px">
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel2">Bank</td>
        <td><asp:RadioButton ID="radHSBC" Text="HSBC"  Checked="true" GroupName="bank" runat="server" />
            <asp:RadioButton ID="radSCB" Text="SCB" GroupName="bank" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Source File</td>
        <td>
            <asp:FileUpload ID="FileUpload1" Width="400" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Target Database</td>
        <td>
            <asp:DropDownList ID="ddl_SunDB" runat="server" />            
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Process" runat="server" Text="Process" OnClick="btn_Process_Click" />&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_ListPending" SkinID="LButton"  runat="server" Text="List Pending Jobs" OnClick="btn_ListPending_Click" />&nbsp;&nbsp;&nbsp;            
        </td>
    </tr>
</table>
<br />
<asp:GridView ID="gv_PendingJob" runat="server" AutoGenerateColumns="false" OnRowDeleting="BankReconRowDelete" >
    <Columns>
        <asp:TemplateField >
            <ItemTemplate>
                <asp:LinkButton ID="lnk_Cancel" runat="server" Text="Cancel" CommandName ="Delete" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="File Name" >
            <ItemTemplate>
                <asp:Label ID="lbl_FileName" runat="server" Text='<%# Eval("FileName") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="User Name" >
            <ItemTemplate >
                <asp:Label ID="lbl_UserName" runat="server" Text='<%# Eval("SubmitUser.DisplayName") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Upload Date" >
            <ItemTemplate >
                <asp:Label ID="lbl_UploadDate" runat="server" Text='<%# Eval("SubmitDate","{0:d}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Finish Status" >
            <ItemTemplate >
                <asp:Label ID="lbl_FinishStatus" runat="server" Text='<%# Eval("Status") %>' />                
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Bank Name">
            <ItemTemplate>
                <asp:Label ID="lbl_BankName" runat="server" Text='<%# Eval("Bank") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Target SunAccount DB" >
            <ItemTemplate >
                <asp:Label ID="lbl_TargetDB" runat="server" Text='<%# Eval("SunDB") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate >No pending job.</EmptyDataTemplate>
</asp:GridView>
</asp:Content>
