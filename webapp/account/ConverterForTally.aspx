<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ConverterForTally.aspx.cs" Inherits="com.next.isam.webapp.account.ConverterForTally" Title="Convert Sun Interface File For Tally"%>

<asp:Content ID="Content3" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Converter For Tally</asp:Panel> 
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script language="javascript">
    function validateInput() {
        if (document.all.ctl00_ContentPlaceHolder1_ddl_FileType.value == "0") {
            alert("Please select the file type first.");
            return false;
        }
        else if (document.all.ctl00_ContentPlaceHolder1_updFile.value == "") {
        alert("Please select the upload file.");
        return false;
        }
        return true;
    }

    function isSelectedFileType() {
        if (document.all.ctl00_ContentPlaceHolder1_ddl_FileType.value == "0") {
            alert("Please select the file type first.");
            return false;
        }
        return true;
    }

</script>

<table width="800px" border='0'>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr style='display:none;'>
        <td class="FieldLabel2">File Type</td>
        <td>
            <asp:DropDownList ID="ddl_FileType" runat="server"  Style="Width:300px;" >            
                <asp:ListItem Text="Please Select" Value="0" />
                <asp:ListItem Text="Sun Interface file for Tally" Value="1" Selected="True"/>
            </asp:DropDownList>
        </td>
    </tr>

    <tr>
        <td class="FieldLabel2">Source File&nbsp;&nbsp;</td>
        <td>
            <input id="updFile" type="file" name="updFile" runat="server" style='width:500px;' />
			<asp:customvalidator id="valFile" runat="server" Enabled="False" ErrorMessage="CustomValidator" ></asp:customvalidator>
			
	    </td>
    </tr>
   <tr>
        <td colspan="2">
            <asp:Button ID='btnProcess' runat='server' text='Convert' onclick="btnProcess_Click" /> <!--OnClientClick='return validateInput();' />-->
            <asp:Button ID="btnListHistory" SkinID="LButton"  runat="server" Text="List History" OnClick="btnListHistory_Click" onClientClick='return isSelectedFileType();'/>&nbsp;&nbsp;&nbsp;
        </td>
    </tr>
</table>
<br />

<asp:GridView ID="gv_History" runat="server" AutoGenerateColumns="false" 
     OnRowDataBound="gv_History_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderText="File Name" >
            <ItemTemplate>
                <asp:Label ID="lbl_FileName" runat="server" Text='' Width="300" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText='Sequence No.'>
            <ItemTemplate>
                <asp:Label ID="lbl_Sequence" runat="server" Text='' Width="50" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="User Name" >
            <ItemTemplate >
                <asp:Label ID="lbl_UserName" runat="server" Text='' width="120px" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Upload Time" >
            <ItemTemplate >
                <asp:Label ID="lbl_UploadTime" runat="server" Text=''  Width="120px"/>
            </ItemTemplate>
        </asp:TemplateField>
</Columns>
    
    <EmptyDataTemplate >No Record</EmptyDataTemplate>
</asp:GridView>
</asp:Content>

