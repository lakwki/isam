<%@ Page Title="Non-Trade Expense - Vendor" Language="C#" AutoEventWireup="true" CodeBehind="NonTradeVendor.aspx.cs" Theme="DefaultTheme" Inherits="com.next.isam.webapp.nontrade.NonTradeVendor" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Non-Trade Expense</title>
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
    <script src="../common/common.js" type="text/javascript" ></script>
    <script type="text/javascript" >

        function disableButton(senderId) {
            if (senderId != "btn_Save" && document.getElementById("btn_Save"))
                document.getElementById("btn_Save").disabled = true;
            if (senderId != "btn_Submit" &&  document.getElementById("btn_Submit"))
                document.getElementById("btn_Submit").disabled = true;
            if (senderId != "btn_Cancel" && document.getElementById("btn_Cancel"))
                document.getElementById("btn_Cancel").disabled = true;
            if (senderId != "btn_Approve" && document.getElementById("btn_Approve"))
                document.getElementById("btn_Approve").disabled = true;
            if (senderId != "btn_Reject" && document.getElementById("btn_Reject"))
                document.getElementById("btn_Reject").disabled = true;
            if (senderId != "btn_Close" && document.getElementById("btn_Close"))
                document.getElementById("btn_Close").disabled = true;
            if (senderId != "btn_CancelVendor" && document.getElementById("btn_CancelVendor"))
                document.getElementById("btn_CancelVendor").disabled = true;
        }

        var eventName = "";
        function processEvent(currentEvent) {
            if (eventName != "" && eventName == currentEvent)
                return false;
            else {
                eventName = currentEvent;
                return true;
            }
        }

        function updateConsumptionUnit() {
            var providerType = parseInt(document.getElementById("ddl_UtilityProviderType").value);
            var conunit = parseInt(document.getElementById("ddl_ConsumptionUnit").value);
            document.getElementById("ddl_ConsumptionUnit").disabled = false;
            if (providerType == 1)
                document.getElementById("ddl_ConsumptionUnit").value='1';
            else if (providerType == 2)
                document.getElementById("ddl_ConsumptionUnit").value='3';
            else if (providerType == 3)
                document.getElementById("ddl_ConsumptionUnit").value='7';
            else if (providerType == 4 && conunit != 5 && conunit != 6 && conunit != 8)
                document.getElementById("ddl_ConsumptionUnit").value='5';
            else if (providerType == 5 || providerType == -1)
                document.getElementById("ddl_ConsumptionUnit").value='-1';

            if (providerType != 4 && providerType != -1)
                document.getElementById("ddl_ConsumptionUnit").disabled = true;
        }

    </script>
</head>
<body>
<form id="form1" runat="server" >
<asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
<asp:panel ID="pnl_ShipmentHeading" runat="server"  SkinID="sectionHeader1" >
<table><tr><td style="height:22px; font-weight:bold ; width:100%">Non-Trade Vendor</td>

</tr></table>
</asp:panel> 
<table cellspacing="2" cellpadding="2">        
        <tr>
            <td>
                <table width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td  colspan="9">
                            &nbsp;<asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2" style="width:128px">&nbsp;Vendor Id&nbsp;</td>
                        <td>&nbsp;</td>
                        <td >
                           <asp:TextBox ID="txt_vendorId" runat="server" ReadOnly="true" CssClass="readOnlyField"  />
                        </td>                        
                        <td class="FieldLabel2" style="width:80px;">&nbsp;Status</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_Status" runat="server" ReadOnly="true" CssClass="readOnlyField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Vendor Name</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:TextBox ID="txt_VendorName" runat="server"  SkinID="TextBox300" MaxLength="200"  OnTextChanged="txt_VendorName_OnChanged" AutoPostBack="true" /><asp:RequiredFieldValidator
                                ID="val_VendorName" runat="server" ErrorMessage="Please enter Vendor Name." Display="None"  ControlToValidate="txt_VendorName" ></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Other Name</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:TextBox ID="txt_OtherName" runat="server" MaxLength="250"  /></td>                       
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Office</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_Office" runat="server" OnSelectedIndexChanged="Office_SelectedIndexChanged" AutoPostBack="true" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Address</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:TextBox ID ="txt_Address" runat="server" SkinID="TextBox300" MaxLength="400" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Country</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_Country" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Telephone No.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_phoneNo" runat="server" MaxLength="50"  /></td>
                        <td class="FieldLabel2">&nbsp;Fax No.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_FaxNo" runat="server" MaxLength="50" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Contact Person</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_ContactPerson" runat="server" MaxLength="100" /></td>
                        <td class="FieldLabel2">&nbsp;Email</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox style="width:300px;" ID="txt_Email" runat="server" MaxLength="500"  /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Bank Account No.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_BankAccountNo" runat="server" MaxLength="100"  /></td>
                        <td class="FieldLabel2">&nbsp;E-Advice Email</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox style="width:300px;" ID="txt_EAdviceEmail" runat="server" MaxLength="500"  /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Invoice Required Fields</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:CheckBox ID="ckb_InvoiceNoReq" runat="server" Text="Invoice No."  />
                        <asp:CheckBox ID="ckb_CustomerNoReq" runat="server" Text="Customer No."  />
                        <asp:CustomValidator ID="val_Vendor" ControlToValidate="txt_FaxNo" ValidateEmptyText="true"  OnServerValidate="val_Vendor_Validate"
                                runat="server" ErrorMessage="CustomValidator" Display="None" ></asp:CustomValidator></td>
                    </tr>
                   <tr>
                        <td class="FieldLabel2">&nbsp;Default Expense Type</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_DefaultExpenseType" runat="server" OnSelectedIndexChanged="DefaultExpenseType_SelectedIndexChanged" AutoPostBack="true" /></td>                        
                    </tr>  
                    <tr>
                        <td class="FieldLabel2">&nbsp;Service Provider Type</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_UtilityProviderType" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Default Consumption Unit</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_ConsumptionUnit" runat="server" Enabled="false"/></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Default Currency</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_DefaultCurrency" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Default Company</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_DefaultCompany" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Default Payment Method</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_DefaultPaymentTerm" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Payment Term Days</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:TextBox ID="txt_PaymentTermDays" runat="server" MaxLength="3" />&nbsp;Days</td>
                    </tr>
                    <tr id="tr_VendorType" runat="server">
                        <td class="FieldLabel2">&nbsp;Vendor Type</td>
                        <td>&nbsp;</td>
                        <td colspan="4">
                            <asp:RadioButton GroupName="VendorType" ID="rad_NonTradeVendor" runat="server" Text="Non-Trade Vendor" OnCheckedChanged="rad_NonTradeVendor_OnCheckedChanged" AutoPostBack="true"/>
                            <asp:RadioButton GroupName="VendorType" ID="rad_BulkVendor" runat="server" Text="Bulk Vendor" oncheckedchanged="rad_BulkVendor_OnCheckedChanged" AutoPostBack="true"/>
                        </td>
                    </tr>
                   <tr id="tr_SimilarSupplier" runat="server">
                        <td class="FieldLabel2">&nbsp;Similar Vendor</td>
                        <td>&nbsp;</td>
                        <td colspan="4">
                            <cc2:SmartDropDownList ID="ddl_SimilarVendor" runat="server" OnSelectedIndexChanged="SimilarVendor_SelectedIndexChanged" AutoPostBack="true"  Width="420px"/>
                            <br />
                        </td>
                    </tr>  

                    <tr>
                        <td class="FieldLabel2">&nbsp;Epicor Supplier Id</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_EpicorSupplierId" runat="server"  MaxLength="8" /></td>                       
                        <td class="FieldLabel2" style="display:none;">&nbsp;SUN Account Code</td>
                        <td style="display:none;">&nbsp;</td>
                        <td ><asp:TextBox ID="txt_SUNAccCode" runat="server" MaxLength="8" ReadOnly="true" style="display:none;"/></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Remark</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:TextBox ID="txt_Remark" runat="server" MaxLength="200" SkinID="TextBox300" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Vendor Document</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:LinkButton runat="server" ID="lnkFile" Text="Dummy" 
                                onclick="lnkFile_Click" />&nbsp;<asp:FileUpload ID="updVendorDoc" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                    <td colspan="6">
                         <asp:UpdatePanel ID="up_VendorExpenseTypeMapping" runat="server" >                        
                            <ContentTemplate >  
                            <table width="550px" style="border: 1px solid #AAAAAA; " cellpadding="0" cellspacing="0">
                                <tr>
                                    <td class="RowHeader" colspan="3" style="height:35px;">Available Expense Type <cc2:SmartDropDownList ID="ddl_ExpenseType" runat="server" />&nbsp;
                                    <asp:Button ID="btn_AddExpenseType" runat="server" Text="add" OnClick="btn_AddExpenseType_Click" CausesValidation="false"   /></td>
                                </tr>
                                <asp:Repeater ID="rep_ExpenseTypeMapping" runat="server" OnItemCommand="ExpenseTypeMapping_ItemCommand">
                                    <HeaderTemplate >
                                        <tr>
                                            <td style="width:400px; border-top:1px solid #AAAAAA;" class="RowHeader">Name</td>
                                            <td class="RowHeader" style="border-top: 1px solid #AAAAAA;">Epicor COA</td>
                                        </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate >
                                        <tr>
                                            <td style="height:20px;" ><asp:ImageButton ID="btn_remove" runat="server" ImageUrl="../images/icon_remove.gif" 
                                                CommandName="remove" CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" style="padding:1px;" CausesValidation="false"  />&nbsp;
                                                <%# DataBinder.Eval(Container, "DataItem.ExpenseType.ExpenseType")%>
                                                </td>
                                                <td style="text-align :center ;"><%# DataBinder.Eval(Container, "DataItem.ExpenseType.EpicorCode")%></td>
                                        </tr>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <tr>
                                            <td style="background-color:#F4FDFF; height: 20px; vertical-align : middle ;"><asp:ImageButton ID="btn_remove" runat="server" ImageUrl="../images/icon_remove.gif" 
                                                CommandName="remove" CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" style="padding:1px;" CausesValidation="false"  />&nbsp;
                                                <%# DataBinder.Eval(Container, "DataItem.ExpenseType.ExpenseType")%>
                                                </td>
                                                <td style="text-align :center ;"><%# DataBinder.Eval(Container, "DataItem.ExpenseType.EpicorCode")%></td>
                                        </tr>
                                    </AlternatingItemTemplate>
                                    <SeparatorTemplate>
                                        <tr>
                                            <td style="height:1px; font-size:1px; border-bottom : 1px solid #AAAAAA;" colspan="5">&nbsp;</td>
                                        </tr>
                                    </SeparatorTemplate>
                                </asp:Repeater> 
                            </table>   
                            </ContentTemplate>
                         </asp:UpdatePanel>                         
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                    </tr>       
                    <tr>
                        <td colspan="6">
                            <asp:Button ID="btn_Save" runat="server" Text="Save" OnClick="Save_OnClick" OnClientClick="disableButton(this.id); return processEvent('save');" />&nbsp;
                            <asp:Button ID="btn_Submit" runat="server" Text="Submit" OnClick="Submit_OnClick" Visible="false" OnClientClick="disableButton(this.id);return processEvent('submit'); "  />&nbsp;
                            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" OnClick="Cancel_OnClick"   CausesValidation="false" OnClientClick="disableButton(this.id);return processEvent('cancel');" />&nbsp;
                           <asp:Button ID="btn_Approve" runat="server" Text="Approve" Visible="false" OnClick="Approve_OnClick" OnClientClick="disableButton(this.id);return processEvent('approve');" />&nbsp;
                           <asp:Button ID="btn_Reject" runat="server" Text="Reject" Visible="false" OnClick="Reject_OnClick" OnClientClick="disableButton(this.id);return processEvent('reject');" />&nbsp;
                           <asp:Button ID="btn_CancelVendor" runat="server" Text="Cancel Vendor" 
                                Visible="false" 
                                OnClientClick="disableButton(this.id);return processEvent('cancelVendor');" 
                                onclick="btn_CancelVendor_Click" SkinID="LButton"/>&nbsp;
                           <asp:Button ID="btn_Close" runat="server" Text="Close" OnClientClick="window.close(this.id);" />

                        </td>
                    </tr>
                </table>
            
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <table width="480px" style="border: 1px solid #AAAAAA;" cellspacing="0" id="tb_AmendRequest" runat="server" visible="false"  >
                    <tr>
                        <td class="RowHeader" style="height:18px;">Send Amendment Request to Accounts</td>
                    </tr>
                    <tr>
                        <td><asp:TextBox ID="txt_AmendRequest" runat="server"  SkinID="XLTextBox" Rows="5"  TextMode="MultiLine" MaxLength="150"   onKeyUp="checkMultiLineTextboxMaxLength(this,150)" onChange="checkMultiLineTextboxMaxLength(this,150)" /></td>
                    </tr>
                    <tr>
                        <td><asp:Button ID="btn_Send" runat="server" Text="Send Request" SkinID="MButton" OnClick="btn_SendRequest_Click"  /></td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr><td>&nbsp;</td></tr>
        <tr>
            <td >
            <table style="border: 1px solid #AAAAAA;" cellpadding="0" cellspacing="0">
                <tr>
                    <td>

                <asp:GridView ID="gv_ActionHistory" runat="server" OnRowDataBound="ActionHistoryDataBound" Width="600px">
                    <Columns>
                        <asp:TemplateField HeaderText="Action Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="120px">
                            <ItemTemplate >
                                <%# DataBinder.Eval(Container, "DataItem.ActionOn")%>                                
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action By" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ActionBy" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Detail" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <%# DataBinder.Eval(Container, "DataItem.Description")%>       
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                    </td>
                </tr>
            </table>

            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>
    </form>
    <script type="text/javascript" >
        updateConsumptionUnit();
    </script>
</body>
</html>
