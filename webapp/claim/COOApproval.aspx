<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="COOApproval.aspx.cs" Inherits="com.next.isam.webapp.claim.COOApproval" Title="Next Claim COO Approval" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Next Claim COO Approval</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>
    <b><asp:validationsummary id="vs" runat="server"></asp:validationsummary></b>
</p>
<table width="800px">
    <tr>
        <td class="FieldLabel2">Vendor</td>
        <td>
            <uc1:uclsmartselection id="uclVendor" runat="server"></uc1:uclsmartselection>&nbsp;
            <asp:Button runat="server" ID="btnList" Text="   List   " CssClass="btn" 
                CausesValidation="false" onclick="btnList_Click"/>
        </td>
    </tr>
</table>
<p />
<p id="pnlHeader" runat="server" visible="false">
    <font color="green"><b>The following Next Claim (s) require Approval from COO</b></font>
</p>
<asp:GridView ID="gv_UKClaim" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_UKClaim_RowDataBound" >
    <Columns>
       <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:CheckBox ID="chkUKClaim" runat="server" Text="" Checked="true"/>
            </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Office">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Type">
            <ItemTemplate>
                <asp:Label ID="lbl_ClaimType" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Next D/N No">
            <ItemTemplate>
                <asp:Label ID="lbl_UKDebitNoteNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Next D/N Date">
            <ItemTemplate>
                <asp:Label ID="lbl_DebitNoteDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Vendor">
            <ItemTemplate>
                <asp:Label ID="lbl_Vendor" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Item/Contract No">
            <ItemTemplate>
                <asp:Label ID="lbl_ItemContractNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Amt">
            <ItemTemplate>
                <asp:Label ID="lbl_Amount" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Form No.">
            <ItemTemplate>
                <asp:Label ID="lbl_FormNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="NS Cost %">
            <ItemTemplate>
                <asp:Label ID="lbl_NSRechargePercent" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <br />&nbsp;&nbsp; <span style="color:Red;"><h3>* There is NO pending approval</h3></span>
    </EmptyDataTemplate>

</asp:GridView>   
<br />
<p id="pnlRefundHeader" runat="server" visible="false">
    <font color="green"><b>The following Next Claim Refund(s) require Approval from COO</b></font>
</p>
<asp:GridView ID="gv_UKClaimRefund" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_UKClaimRefund_RowDataBound">
    <Columns>
       <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:CheckBox ID="chkUKClaimRefund" runat="server" Text="" Checked="true"/>
            </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Office">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" />
            </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Type">
            <ItemTemplate>
                <asp:Label ID="lbl_ClaimType" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Next D/N No">
            <ItemTemplate>
                <asp:Label ID="lbl_UKDebitNoteNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Refund Received Date">
            <ItemTemplate>
                <asp:Label ID="lbl_RefundReceivedDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Vendor">
            <ItemTemplate>
                <asp:Label ID="lbl_Vendor" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Item/Contract No">
            <ItemTemplate>
                <asp:Label ID="lbl_ItemContractNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Amt">
            <ItemTemplate>
                <asp:Label ID="lbl_Amount" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Form No.">
            <ItemTemplate>
                <asp:Label ID="lbl_FormNo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="NS Cost %">
            <ItemTemplate>
                <asp:Label ID="lbl_NSRechargePercent" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <br />&nbsp;&nbsp; <span style="color:Red;"><h3>* There is NO pending approval</h3></span>
    </EmptyDataTemplate>

</asp:GridView>   
<br />
<p runat="server" id="pnlDoc" visible="false">
    <table style="margin-left:10px;" cellpadding="5" cellspacing="0" border="0">
    <tr>
        <td><b>Account Code</b></td>
        <td><asp:DropDownList runat="server" ID="ddl_AccountCode" />&nbsp;&nbsp;<asp:DropDownList runat="server" ID="ddl_T9" />&nbsp;&nbsp;<asp:DropDownList runat="server" ID="ddl_Segment7" /></td>
    </tr>
    <tr>
        <td><b>COO Sign-Off Doc</b></td>
        <td><asp:FileUpload runat="server" ID="updDoc" /></td>
    </tr>
    <tr>
        <td colspan="2"><asp:Button ID="btnConfirm" runat="server" Text="Confirm" onclick="btnConfirm_Click" CssClass="btn" CausesValidation="true"/></td>
    </tr>
    </table>
    <asp:CustomValidator ID="valCustom" runat="server" Display="None" 
        onservervalidate="valCustom_ServerValidate"></asp:CustomValidator>
</p>

</asp:Content>
