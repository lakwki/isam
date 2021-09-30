<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="GenLogoXMLFile.aspx.cs" Inherits="com.next.isam.webapp.account.GenLogoXMLFile" Title="Generate Logo XML File" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Generate Logo XML File</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:LinkButton ID="userGuide" runat="server" Text="Click here for the user guide" 
                    onclick="userGuide_Click" />
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr id="trFiscalYear">
            <td class="FieldLabel2" style="width: 100px">Fiscal Year</td>
            <td>
                <cc2:SmartDropDownList ID="ddl_Year" runat="server" Width="250px" AutoPostBack="True"></cc2:SmartDropDownList>
            </td>
        </tr>
        <tr id="trPeriod">
            <td class="FieldLabel2" style="width: 100px">Period</td>
            <td>
                <cc2:SmartDropDownList ID="ddl_Period" runat="server" Width="250px" AutoPostBack="True">
                    <asp:ListItem Text="Period 1" Value="1" />
                    <asp:ListItem Text="Period 2" Value="2" />
                    <asp:ListItem Text="Period 3" Value="3" />
                    <asp:ListItem Text="Period 4" Value="4" />
                    <asp:ListItem Text="Period 5" Value="5" />
                    <asp:ListItem Text="Period 6" Value="6" />
                    <asp:ListItem Text="Period 7" Value="7" />
                    <asp:ListItem Text="Period 8" Value="8" />
                    <asp:ListItem Text="Period 9" Value="9" />
                    <asp:ListItem Text="Period 10" Value="10" />
                    <asp:ListItem Text="Period 11" Value="11" />
                    <asp:ListItem Text="Period 12" Value="12" />
                </cc2:SmartDropDownList>
            </td>
        </tr>
        <tr>
            <td style="width: 100px">&nbsp;</td>
            <td>
                <asp:Button ID="btn_Process" runat="server" Text="Submit"
                    OnClick="btn_Process_Click" />
                &nbsp;&nbsp;
            <asp:Label ID="errorMsg" runat="server" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:GridView ID="gv_LogoInterfaceRequest" runat="server" EnableModelValidation="True" OnRowDataBound="gv_LogoInterfaceRequest_RowDataBound" AllowPaging="True" OnPageIndexChanging="gv_LogoInterfaceRequest_PageIndexChanging" PageSize="15">
                    <PagerSettings Mode="Numeric" Position="TopAndBottom" />
                    <PagerStyle Font-Bold="true" HorizontalAlign="Right" />
                    <Columns>
                        <asp:TemplateField HeaderText="Request Id">
                            <ItemTemplate>
                                <asp:Label ID="lbl_RequestId" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Year/<br>Period">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Period" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Requester" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lbl_User" runat="server" />
                            </ItemTemplate>

                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Completed Time">
                            <ItemTemplate>
                                <asp:Label ID="lbl_CompletedTime" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Interface File" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:ImageButton CommandArgument='<%#  DataBinder.Eval(Container, "RowIndex") %>'
                                    OnClick="btn_file_Click"
                                    ID="btn_file" runat="server" ImageUrl="../images/btn_view.JPG" />
                            </ItemTemplate>

                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>
