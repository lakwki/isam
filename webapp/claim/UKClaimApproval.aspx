<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKClaimApproval.aspx.cs" Inherits="com.next.isam.webapp.claim.UKClaimApproval" Title="Next Claim Approval" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Next Claim COO Approval</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="800px">
    <tr>
        <td><font color="green"><b>The followings Next Claim requires Approval from the corresponding Department Head</b></font></td>
    </tr>
    <tr>
        <td>Please upload the signed document to the Next claim record when it is available</td>
    </tr>
</table>
<br /><br />
<asp:GridView ID="gv_UKClaim" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_UKClaim_RowDataBound" >
    <Columns>
       <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/images/icon_edit.gif" ToolTip="Upload Document"/>
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
</asp:Content>
