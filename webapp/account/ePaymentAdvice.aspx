<%@ Page Language="C#" Theme="DefaultTheme"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ePaymentAdvice.aspx.cs" Inherits="com.next.isam.webapp.account.ePaymentAdvice" Title="ePayment Advice" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">File Upload</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="700">
    <tr><td>&nbsp;</td></tr>
</table>
<table>    
    <tr>
        <td class="FieldLabel2" style="width:100px;">Upload File Type</td>
        <td><asp:DropDownList ID="ddl_fileType" runat="server" Width="800" >
            <asp:ListItem Text="Payment Advice" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Source File&nbsp;:</td>
        <td>
            <asp:FileUpload ID="FileUpload1" runat="server" Width="400" />
          </td>
    </tr>

    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Process" runat="server" Text="Process" OnClick="btn_Process_Click" />
            <asp:Button ID="btn_ShowFolder" runat="server" Text="Show Folder" 
                onclick="btn_ShowFolder_Click" SkinID="MButton"  />
            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" OnClick="btn_Cancel_Click" />
        </td>
    </tr>
</table>
<asp:Panel ID="pnl_Duplicate" runat="server" ForeColor="Red" Visible ="false" >
<h6>Duplicate entries are found in the file: </h6> 
<asp:Label ID="lbl_duplicateList" runat="server" ForeColor="Red" ></asp:Label>
</asp:Panel>
<br />
<asp:GridView ID="gv_FileUpload" runat="server" AutoGenerateColumns="false" >
    <Columns>
        <asp:TemplateField >
            <ItemTemplate >
                <asp:CheckBox ID="ckb_Cancel" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="File Name">
            <ItemTemplate >
                <asp:Label ID="lbl_FileName" runat="server" Text='<%# Eval("FileName") %>' Width="400px" />
            </ItemTemplate>
        </asp:TemplateField>
<%--        <asp:TemplateField HeaderText="File Type">
            <ItemTemplate >
                <asp:Label ID="lbl_FileType" runat="server" Text='<%# Eval("FileType") %>' />
            </ItemTemplate>
        </asp:TemplateField>--%>
        <asp:TemplateField HeaderText="Submitted By">
            <ItemTemplate >
                <asp:Label ID="lbl_SubmitBy" runat="server" Text='<%# Eval("SubmitUser.DisplayName") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Submitted Date">
            <ItemTemplate >
                <asp:Label ID="lbl_SubmitDate" runat="server" Text='<%# Eval("SubmitDate","{0:d}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

</asp:Content>
