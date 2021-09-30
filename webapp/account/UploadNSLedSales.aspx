<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UploadNSLedSales.aspx.cs" Inherits="com.next.isam.webapp.account.UploadNSLedSales" Title="Upload NS-LED Sales Excel file"%>

<asp:Content ID="Content3" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Upload NS-LED Sales Excel file</asp:Panel> 
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script language="javascript">
        //function resetUploadStatus() {
        //    document.all.ctl00_ContentPlaceHolder1_ddl_CustomerType.value = "";
        //}
        function validateInput() {
            //if (document.all.ctl00_ContentPlaceHolder1_ddl_CustomerType.value == "") {
            //    alert("Please select the upload file type.");
            //    return false;
            //}
            //else
                if (document.all.ctl00_ContentPlaceHolder1_updFile.value == "") {
                alert("Please select the upload file.");
                return false;
            }
            return true;
        }
    </script>

    <table id="tabMain" runat="server" width="800px" border='0'>
        <tr>
            <td>&nbsp;</td>
        </tr>
     <%--   <tr style='display:none;'>
            <td class="FieldLabel2">File Type</td>
            <td>
                <asp:DropDownList ID="ddl_FileType" runat="server"  Style="Width:300px;" >            
                    <asp:ListItem Text="Please Select" Value="0" />
                    <asp:ListItem Text="NS-LED Weekly Sales file" Value="1" Selected="True"/>
                </asp:DropDownList>
            </td>
        </tr>--%>
        
        <%--<tr>
            <td class="FieldLabel2">Customer Type&nbsp;&nbsp;</td>
            <td>
                <asp:DropDownList ID="ddl_CustomerType" runat="server"  Style="Width:200px;" >            
                    <asp:ListItem Text="Please Select" Value="" Selected />
                    <asp:ListItem Text="Directory" Value="D" />
                    <asp:ListItem Text="Retail" Value="R"  />
                </asp:DropDownList>
            </td>
        </tr>--%>
        <tr>
            <td class="FieldLabel2">Source File&nbsp;&nbsp;</td>
            <td>
                <input id="updFile"  type="file" name="upd_File" runat="server" style='width:500px;' text="TEXT"  />
			    <asp:customvalidator id="valFile" runat="server" Enabled="False" ErrorMessage="CustomValidator" ></asp:customvalidator>
	        </td>
        </tr>
       <tr>
            <td colspan="2">
                <asp:Button ID='btnProcess' runat='server' text='Upload' onclick="btnProcess_Click" OnClientClick='return validateInput();' /> &nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnListHistory" SkinID="LButton"  runat="server" Text="List History" OnClick="btnListHistory_Click" />
            </td>
        </tr>
    </table>
    <br />
    <asp:GridView ID="gv_History" runat="server" AutoGenerateColumns="false" OnRowDataBound="gv_History_RowDataBound">
        <Columns>
            <asp:TemplateField  HeaderText='Office'>
                <ItemTemplate>
                    <asp:Label ID="lbl_Office" runat="server" Text='' Width="10" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="File Name" >
                <ItemTemplate>
                    <asp:Label ID="lbl_FileName" runat="server" Text='' Width="300" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Upload by" >
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
