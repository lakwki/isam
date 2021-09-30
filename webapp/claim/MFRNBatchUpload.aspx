<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="MFRNBatchUpload.aspx.cs" Inherits="com.next.isam.webapp.claim.MFRNBatchUpload" Title="Next Claim - MFRN Batch Upload" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Next Claim MFRN Upload</asp:Panel>
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
        <b><asp:validationsummary id="vs" runat="server"></asp:validationsummary></b>
    </p>
<table width="800px">
    <tr>
        <td class="FieldLabel2">Claim Month:</td>
        <td>
            <asp:TextBox ID="txtMonth" runat="server" />&nbsp;(YYYYMM)</td>
    </tr>
    <tr>
        <td class="FieldLabel2">MFRN Docs - Zip File:</td>
        <td>
            <asp:FileUpload ID="uplZipFile" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2">MFRN Listing - Excel File:</td>
        <td>
            <asp:FileUpload ID="uplExcelFile" runat="server" /></td>
    </tr>

    <tr>
        <td>&nbsp;</td>
        <td>
            1.&nbsp;<asp:Button runat="server" ID="btnUploadZip" Text="Upload Zip Contents" 
                CssClass="btn" CausesValidation="false" onclick="btnUploadZip_Click" SkinId="LButton"  />
            2.&nbsp;<asp:Button runat="server" ID="btnLoadExcel" Text="Load Excel File" 
                CssClass="btn" CausesValidation="false" onclick="btnLoadExcel_Click" SkinId="LButton" OnClientClick="showProgressBarX();" />&nbsp;&nbsp;
            3.&nbsp;<asp:Button runat="server" ID="btnConfirm" Text="Confirm" 
                CssClass="btn" CausesValidation="false" SkinId="LButton" 
                onclick="btnConfirm_Click" OnClientClick="if (confirm('Are you sure to proceed?')) showProgressBarX(); else return false;" />&nbsp;&nbsp;
        </td>
    </tr>
</table>
<p />
<p id="pnlHeader" runat="server" visible="false">
    <font color="green"><b>Excel Contents - MFRN Listing</b></font>
</p>
<asp:GridView ID="gv_UKClaim" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_UKClaim_RowDataBound" >
    <Columns>
       <asp:TemplateField HeaderText="D/N No.">
            <ItemTemplate>
                <asp:Label ID="lbl_DNNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="D/N Date">
            <ItemTemplate>
                <asp:Label ID="lbl_DNDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Received Date">
            <ItemTemplate>
                <asp:Label ID="lbl_ReceivedDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Currency">
            <ItemTemplate>
                <asp:Label ID="lbl_Currency" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Item No">
            <ItemTemplate>
                <asp:Label ID="lbl_ItemNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qty">
            <ItemTemplate>
                <asp:Label ID="lbl_Qty" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Amount">
            <ItemTemplate>
                <asp:Label ID="lbl_Amount" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Vendor">
            <ItemTemplate>
                <asp:Label ID="lbl_Vendor" runat="server" />
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
        <asp:TemplateField HeaderText="Payment Office">
            <ItemTemplate>
                <asp:Label ID="lbl_PaymentOffice" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Product Team">
            <ItemTemplate>
                <asp:Label ID="lbl_ProductTeam" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>

       <asp:TemplateField HeaderText="Attachment">
            <ItemTemplate>
                <asp:Label ID="lbl_Attachment" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Note">
            <ItemTemplate>
                <asp:Label ID="lbl_Note" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <br />&nbsp;&nbsp; <span style="color:Red;"><h3>* There is NO record found</h3></span>
    </EmptyDataTemplate>

</asp:GridView>   
<br />

</asp:Content>
