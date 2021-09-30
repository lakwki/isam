<%@ Page Title="ISAM - LC Bank Maintenance" Theme="DefaultTheme"  Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="LCBankMaint.aspx.cs" Inherits="com.next.isam.webapp.myapplication.LCBankMaint" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_workplace.gif" alt="" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="panel1" runat="server" SkinID="sectionHeader_PersonalSettings">L/C Bank</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table >
    <tr>
<!--
        <td class="tableHeader"  style="width:600px;">LC Bank</td>
-->
    &nbsp;
    </tr>
    <tr>
        <td>
            <asp:Panel ID="pnl_Bank" runat="server" >
            <asp:Button ID="btn_CreateBank" runat="server" Text="Create Bank" SkinID="LButton" 
                    onclick="btn_CreateBank_Click"  /><br />
            <br />
             <asp:GridView ID="gv_Bank" runat="server" AutoGenerateColumns="false" OnRowEditing ="BankRowEdit" OnRowUpdating="BankRowSave"
                 OnRowCommand="BankRowCommand" OnRowDeleting="BankRowDelete" OnRowCancelingEdit="BankRowCancelEdit">
                <Columns >
                    <asp:TemplateField Visible = "false" >
                        <ItemTemplate>
                            <asp:Label ID="lbl_BankId" runat="server" Text='<%# Eval("BankId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField  HeaderText="Bank Name">
                        <ItemTemplate>
                            <asp:Label ID="lbl_BankName" runat="server" Text='<%# Eval("BankName") %>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txt_BankName" runat="server" SkinID="TextBox300"  Text='<%# Eval("BankName") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btn_Edit" ImageUrl="~/images/icon_edit.gif" CommandName="Edit" runat="server" />&nbsp;&nbsp;&nbsp;
                            <asp:ImageButton ID="btn_Delete" ImageUrl="~/images/icon_delete.gif" OnClientClick="return confirm('Are you sure to delete?');" runat ="server" CommandName="Delete" />&nbsp;&nbsp;&nbsp;
                        </ItemTemplate>
                        <EditItemTemplate >
                            <asp:ImageButton ID="btn_Save" ImageUrl="~/images/save.png" runat="server" CommandName="Update" />&nbsp;&nbsp;&nbsp;
                            <asp:ImageButton ID="btn_Cancel" ImageUrl="~/images/cancel.png" runat="server" CommandName="Cancel" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField >
                        <ItemTemplate>
                            <asp:LinkButton ID="btn_ShowBranch" runat="server" Text ="Show Branch" CommandName="ShowBranch" CommandArgument='<%# Eval("BankId") %>'  />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>        
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="pnl_NewBank" runat="server" Visible = "false" >
                <table>
                    <tr>
                        <td>Bank Name&nbsp;&nbsp;&nbsp;</td>
                        <td><asp:TextBox ID="txt_BankName" runat="server" MaxLength="100" SkinID="TextBox300"   /></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btn_Create" runat="server" Text="Create" 
                                onclick="btn_Create_Click" />
                            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" 
                                onclick="btn_Cancel_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="pnl_Branch" runat="server" Visible="false" >
                <table>
                    <tr>
                        <td><asp:Label ID="Label1" runat="server" SkinID="FieldLabel"  Text="Bank Name   :   "  />&nbsp;<asp:Label ID="lbl_BankName" runat="server" /></td>
                    </tr>                    
                    <tr>
                        <td style="text-align:right ;"><asp:Button ID="btn_CreateBranch" runat="server" 
                                SkinID="LButton"  Text="Create Branch" onclick="btn_CreateBranch_Click" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="gv_Branch" runat="server" AutoGenerateColumns="false" OnRowEditing ="BranchRowEdit" OnRowUpdating="BranchRowSave"
                                OnRowDeleting="BranchRowDelete" OnRowCancelingEdit="BranchRowCancelEdit" OnRowDataBound="BranchRowDataBound">
                                <Columns>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_BranchId" runat="server" Text='<%# Eval("BankBranchId") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Branch Name">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_BranchName" runat="server" Text='<%# Eval("BranchName") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate >
                                            <asp:TextBox ID="txt_BranchName" runat="server" Text='<%# Eval("BranchName") %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Address">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_Address1" runat="server" Text='<%# Eval("Address1") %>' /><br />
                                            <asp:Label ID="lbl_Address2" runat="server" Text='<%# Eval("Address2") %>' /><br />
                                            <asp:Label ID="lbl_Address3" runat="server" Text='<%# Eval("Address3") %>' /><br />
                                            <asp:Label ID="lbl_Address4" runat="server" Text='<%# Eval("Address4") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate >
                                            <asp:TextBox ID="txt_Address1" runat="server" Text='<%# Eval("Address1") %>' /><br />
                                            <asp:TextBox ID="txt_Address2" runat="server" Text='<%# Eval("Address2") %>' /><br />
                                            <asp:TextBox ID="txt_Address3" runat="server" Text='<%# Eval("Address3") %>' /><br />
                                            <asp:TextBox ID="txt_Address4" runat="server" Text='<%# Eval("Address4") %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Country">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_Country" runat="server" Text='<%# Eval("Country.Name") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <cc2:SmartDropDownList ID="ddl_country" runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contact Person">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_ContactPerson" runat="server" Text='<%# Eval("ContactPerson") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txt_ContactPerson" runat="server" Text='<%# Eval("ContactPerson") %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Phone No.">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_Phone" runat="server" Text='<%# Eval("Phone") %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate >
                                            <asp:TextBox ID="txt_Phone" runat="server" Text='<%# Eval("Phone") %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btn_Edit" ImageUrl="~/images/icon_edit.gif" CommandName="Edit" runat="server" />&nbsp;&nbsp;&nbsp;
                                            <asp:ImageButton ID="btn_Delete" ImageUrl="~/images/icon_delete.gif" OnClientClick="return confirm('Are you sure to delete?');" runat ="server" CommandName="Delete" />&nbsp;&nbsp;&nbsp;
                                        </ItemTemplate>
                                        <EditItemTemplate >
                                            <asp:ImageButton ID="btn_Save" ImageUrl="~/images/save.png" runat="server" CommandName="Update" />&nbsp;&nbsp;&nbsp;
                                            <asp:ImageButton ID="btn_Cancel" ImageUrl="~/images/cancel.png" runat="server" CommandName="Cancel" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <br />
                            <asp:Button ID="btn_Back" runat="server" Text="Back" onclick="btn_Back_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pnl_CreateBranch" runat="server" visible="false" >
                                <table >
                                    <tr>
                                        <td>Branch Name</td>
                                        <td><asp:TextBox ID="txt_BranchName" runat="server" MaxLength="100" SkinID="TextBox300" /></td>
                                    </tr>
                                    <tr>
                                        <td rowspan="4" style="vertical-align:top ;">Address</td>
                                        <td><asp:TextBox ID="txt_Address1" runat="server" MaxLength="100" SkinID="TextBox300" /></td>
                                    </tr>
                                    <tr>
                                        <td><asp:TextBox ID="txt_Address2" runat="server" MaxLength="100" SkinID="TextBox300" /></td>
                                    </tr>
                                    <tr>
                                        <td><asp:TextBox ID="txt_Address3" runat="server" MaxLength="100" SkinID="TextBox300" /></td>
                                    </tr>
                                    <tr>
                                        <td><asp:TextBox ID="txt_Address4" runat="server" MaxLength="100" SkinID="TextBox300" /></td>
                                    </tr>
                                    <tr>
                                        <td>Country</td>
                                        <td><cc2:SmartDropDownList ID="ddl_country" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td>Contact Person</td>
                                        <td><asp:TextBox ID="txt_ContactPerson" runat="server" MaxLength="100" SkinID="TextBox300" /></td>
                                    </tr>
                                    <tr>
                                        <td>Phone No.</td>
                                        <td><asp:TextBox ID="txt_Phone" runat="server" MaxLength="100" SkinId="TextBox300" /></td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="btn_SaveBranch" runat="server" Text="Save" 
                                                onclick="btn_SaveBranch_Click" />
                                            <asp:Button ID="btn_CancelCreate" runat="server" Text="Cancel" />
                                        </td>
                                    </tr>
                                </table>                                
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            
        </td>
    </tr>
</table>

</asp:Content>
