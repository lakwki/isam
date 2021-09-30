<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="NextClaimUpload.aspx.cs" Inherits="com.next.isam.webapp.claim.NextClaimUpload" Title="Next Claim - MFRN Batch Upload" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Next Claim Upload</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" language="javascript">
        function showProgressBarX() {
            var left = window.screen.availWidth / 3 - 100;
            var top = window.screen.availHeight / 3 - 100

            waitDialog = window.showModelessDialog("../common/ProgressBarX.htm", window, "dialogHeight: 30px; dialogWidth: 250px; center:yes; edge: sunken; center: Yes; help: No; resizable: No; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
        }
    </script>
    <p>
        <b>
            <asp:ValidationSummary ID="vs" runat="server"></asp:ValidationSummary>
        </b>
    </p>

    <asp:HiddenField ID="hid_DatetimePath" Value="" runat="server" />
    <table width="800px">
        <tr>
            <td class="FieldLabel2">Payment Office</td>
            <td>
                <cc2:SmartDropDownList ID="ddl_PaymentOffice" runat="server" Width="120px" /></td>
        </tr>
        <tr>
            <td class="FieldLabel2">Next D/N Rec'd Date</td>
            <td>
                <cc2:SmartCalendar ID="txt_DebitNoteDate" runat="server" AutoPostBack="true" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Upload PDF File</td>
            <td>
                <asp:FileUpload ID="uplPdfFile" runat="server" />&nbsp;&nbsp;
                <asp:Button runat="server" ID="btnUpload" Text="Upload"
                    CssClass="btn" CausesValidation="false" SkinID="LButton" OnClick="btnUpload_Click" />
            </td>
        </tr>
    </table>
    <p />
    <asp:Label runat="server" visible="false" id="lbl_FileSummary" Font-Bold="true" style="color: green;"></asp:Label><br /><br />
    <asp:GridView ID="gv_NextClaim" runat="server" AutoGenerateColumns="false"
        OnRowDataBound="gv_NextClaim_RowDataBound">
        <Columns>

            <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:CheckBox ID="chb_check" runat="server" Checked="true" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Page No.">
                <ItemTemplate>
                    <asp:Label ID="lbl_PageNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Office">
                <ItemTemplate>
                    <asp:Label ID="lbl_Office" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Handling Office">
                <ItemTemplate>
                    <asp:Label ID="lbl_HandlingOffice" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Claim Type">
                <ItemTemplate>
                    <asp:Label ID="lbl_ClaimType" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Next D/N No">
                <ItemTemplate>
                    <asp:Label ID="lbl_DNNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Next D/N Date">
                <ItemTemplate>
                    <asp:Label ID="lbl_DNDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Item / Contract No">
                <ItemTemplate>
                    <asp:Label ID="lbl_ItemNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Supplier">
                <ItemTemplate>
                    <asp:Label ID="lbl_Vendor" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Qty">
                <ItemTemplate>
                    <asp:Label ID="lbl_Qty" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Currency">
                <ItemTemplate>
                    <asp:Label ID="lbl_Currency" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Amount">
                <ItemTemplate>
                    <asp:Label ID="lbl_Amount" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Product Team">
                <ItemTemplate>
                    <asp:Label ID="lbl_ProductTeam" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <%--
            <asp:TemplateField HeaderText="Upload Success (Y/N)">
                <ItemTemplate>
                    <asp:Label ID="lbl_Attachment" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            --%>

            <asp:TemplateField HeaderText="Note">
                <ItemTemplate>
                    <asp:Label ID="lbl_Note" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <br />
            &nbsp;&nbsp; <span style="color: Red;">
                <h3>* There is NO record found</h3>
            </span>
        </EmptyDataTemplate>

    </asp:GridView>
    <br />
     &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button runat="server" ID="btnConfirm" Text="Confirm"
        CssClass="btn" SkinID="XLButton" Visible="false"
        OnClick="btnConfirm_Click" OnClientClick="if (confirm('Are you sure to proceed?')) showProgressBarX(); else return false;" />&nbsp;&nbsp;

</asp:Content>
