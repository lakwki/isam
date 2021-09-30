<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKClaimReviewList.aspx.cs" Inherits="com.next.isam.webapp.claim.UKClaimReviewList" Title="Next Claim Review List" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Next Claim Review List</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="800px">
    <tr>
        <td><font color="green"><b>The followings Next Claim Debit Notes require you to map the claim request from QAIS</b></font></td>
    </tr>
    <tr>
        <td><br /><b>Please click the icon to open the details and do the mapping</b></td>
    </tr>
</table>
<h3><font color="navy">New Next Claims</font></h3>
<asp:GridView ID="gv_UKClaim" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_UKClaim_RowDataBound" 
        onrowcommand="gv_UKClaim_RowCommand" >
    <Columns>
       <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/images/icon_edit.gif" ToolTip="Review" CommandName="Review"/>
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
    </Columns>
    <EmptyDataTemplate>
        &nbsp;&nbsp; <span style="color:Red;"><h5>* There is NO pending new Next Claim Debit Note</h5></span>
    </EmptyDataTemplate>

</asp:GridView>   


<h3><font color="navy">Submitted Next Claims</font></h3>
<b>You can open the followings to re-map it in case it is incorrect.</b><br /><br />
<asp:GridView ID="gv_UKClaim_Submitted" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_UKClaim_Submitted_RowDataBound" 
        onrowcommand="gv_UKClaim_Submitted_RowCommand" >
    <Columns>
       <asp:TemplateField HeaderText="">
            <ItemTemplate>
                <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/images/icon_edit.gif" ToolTip="Review" CommandName="Review"/>
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
        &nbsp;&nbsp; <span style="color:Red;"><h5>* There is NO pending submitted Next Claim Debit Note</h5></span>
    </EmptyDataTemplate>

</asp:GridView>   

</asp:Content>