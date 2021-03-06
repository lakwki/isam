<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ShippingDataUpload.aspx.cs" Inherits="com.next.isam.webapp.shipping.ShippingDataUpload" Title="Shipping Record Upload"%>

<asp:Content ID="Content3" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <table>
    <tr>
        <td><asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Shipping">Upload Shipping Data File</asp:Panel></td>
        <td><a href="FileUploadSample.xlsx" target="_blank"><img src="../images/Icon_Excel.jpg" alt="File Template" border="0" /></a>&nbsp;&nbsp;
        <a href="guideline_shipping_data_upload.pdf" target="_blank"><img src="../images/help.JPG" alt="Help" height="16px" border="0" /></a></td>
    </tr>
    </table>    
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
<!--
    <tr>
        <td class="tableHeader" colspan="2">Upload Shipping Data File</td>
    </tr>
-->
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel2">File Type</td>
        <td>
            <asp:DropDownList ID="ddl_FileType" runat="server"  Style="Width:300px;" >            
                <asp:ListItem Text="Please Select" Value="0" />
                <asp:ListItem Text="Daily Booking Information" Value="1" Enabled="false"/>
                <asp:ListItem Text="Supplier Invoice No & Shipping Document Receipt Date" Value="2" Selected />
            </asp:DropDownList>
        </td>
    </tr>

    <tr>
        <td class="FieldLabel2">Source File&nbsp;&nbsp;</td>
        <td>
            <INPUT id="updFile" type="file" name="updFile" runat="server" style='width:500px;' />
			<asp:customvalidator id="valFile" runat="server" Enabled="False" ErrorMessage="CustomValidator" ></asp:customvalidator>
			<!-- onservervalidate="valFile_ServerValidate" -->
			
	    </td>
    </tr>
   <tr>
        <td colspan="2">
            <asp:Button ID='btnProcess' runat='server' text='Process' onclick="btnProcess_Click" OnClientClick='return validateInput();' />
            <asp:Button ID="btnListPending" SkinID="LButton"  runat="server" Text="List Pending Job" OnClick="btnListPending_Click" onClientClick='return isSelectedFileType();'/>&nbsp;&nbsp;&nbsp;
<!--
            <asp:Button ID="btnCancel" runat="server" Text="Cancel Job(s)" />
            <asp:label id="lblFileName" runat="server" Visible="False"></asp:label>
-->
        </td>
    </tr>
</table>
<br />
<asp:GridView ID="gv_PendingJob" runat="server" AutoGenerateColumns="false" OnRowDeleting="UploadFileRowDelete" 
     OnRowDataBound="gv_PendingJob_RowDataBound"
    >
    <Columns>
        <asp:TemplateField >
            <ItemTemplate>
                <asp:LinkButton ID="lnk_Cancel" runat="server" Text="Cancel" ToolTip="Cancel and remove this job" CommandName ="Delete" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="File Name" >
            <ItemTemplate>
                <asp:Label ID="lbl_FileName" runat="server" Text='' Width="300" />
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
    
    <EmptyDataTemplate >No pending job.</EmptyDataTemplate>
</asp:GridView>
</asp:Content>

