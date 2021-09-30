<%@ Page Language="C#" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" Theme="DefaultTheme" CodeBehind="SplitShipmentDetail.aspx.cs" Inherits="com.next.isam.webapp.shipping.SplitShipmentDetail" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Split Shipment Detail</title>
    <script src="../common/common.js" type="text/javascript" ></script>   
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
    <style type="text/css">
        .style1
        {
            height: 27px;
        }
    </style>
</head>
<script type="text/javascript">
    window.focus();


    function isShipDocCheckValid(obj, objName) {
        var prefix = obj.id.replace(objName, "");
        var oVendorNetAmt = document.getElementById(prefix + "lbl_NetAmtToSupp");
        var oShipDocCheck = document.getElementById(prefix + "ckb_ShipDocCheck");
        var oShipDocCheckAmt = document.getElementById(prefix + "txt_ShipDocCheckAmount");

        var oSuppInvNo = document.getElementById(prefix + "txt_SuppInvNo");
        //var oNoOfCarton = document.getElementById(prefix + "txt_InvPiece");
        var oShipDocRecDate = document.getElementById(prefix + "txt_ShipRecvDocDate");

        if (oShipDocCheck.checked && (oSuppInvNo.value == "" || oShipDocRecDate.value == "")) {
            alert("Please input the following items before tick “Shipping Doc. Check Amt” checkbox:\n - Supplier Invoice No.;\n - Shipping Doc. Receipt Date.");
            oShipDocCheck.checked = false;
            return false;
        }
        else {
            if (oShipDocCheck.checked) {
                oShipDocCheckAmt.value = oVendorNetAmt.innerText;
            }
            else {
                oShipDocCheckAmt.value = "";
            }
        }
        return true;
    }


    function confirmIfTotalShippedQtyZero(allowZeroQty) {
        var totalQty = 0, qty = 0;
        var anyShippedQty = false;
        var invNo = '', invDate = '';

        //if (document.all.hid_WorkflowStatus.value == "INVOICED") {
            var nodeList = document.getElementsByTagName('input');
            for (i = 0, anyInvalid = false; i < nodeList.length && !anyInvalid; i++)
                if (nodeList[i].name.indexOf("gv_Options") > 0 && nodeList[i].name.indexOf("txt_ShipQty") > 0) {
                    anyShippedQty = true;
                    str = nodeList[i].value.replace(",","").trim();
                    qty = parseInt(str, 10);
                    if (qty >= 0 && str.replace(qty.toString(), "") == "")
                        totalQty += qty;
                    else
                        anyInvalid = true;
                }

                if (anyInvalid) {
                    alert('Invalid shipped quantity.');
                    setSaveMode('OFF');
                    return false;
                }
                else
                    if (anyShippedQty && totalQty == 0) {
                        if (allowZeroQty) {
                            if (confirm('Total Shipped Quantity is zero. Confirm to save?'))
                                return true;
                            else {
                                setSaveMode('OFF');
                                return false;
                            }
                        }
                        else {
                            alert('Total Shipped Quantity cannot be zero.');
                            setSaveMode('OFF');
                            return false;
                        }
                    }
        //}
        return true;
    }

    // Save Action Control
    function enableButtons(flag) {
        document.getElementById('btn_Save').disabled = !flag;
        document.getElementById('btn_Cancel').disabled = !flag;
        document.getElementById('btn_Close').disabled = !flag;
    }

    function setSaveMode(state) {
        var saveButton = document.getElementById('btn_Save');
        switch (state) {
            case 'OFF':
                enableButtons(true);
                saveButton.clicked = false;
                break;
            case 'ON':
                enableButtons(false);
                saveButton.clicked = true;
                saveButton.disabled = false; // Allow to process the save action in server side
                break;
            case 'FREEZE':
                enableButtons(false);   // Do not allow to process the server side action
                saveButton.clicked = true;
                break;
        }
    }

    function allowToSave() {
        var saveButton = document.getElementById('btn_Save');
        if (saveButton.clicked == null || saveButton.clicked == false) { // Have not click on the SAVE button
            setSaveMode('ON');
            return true;
        }
        else // Reclick on SAVE button
            setSaveMode('FREEZE');
        return false;
    }


    function suppressSpecialKey(evt) {
        evt = evt || window.event;
        var target = evt.target || evt.srcElement;
        if (evt.type == "keydown")
            switch (evt.keyCode) {
            case 8: // BackSpace
                var isEditable = !target.disabled && !(target.readOnly == null ? false : target.readOnly);
                if (!(isEditable && /input|textarea/i.test(target.nodeName) && /text|password/i.test(target.type))) {
                    // not in an active input area, block the key
                    evt.keyCode = 0;
                    evt.returnValue = false;
                }
                break;
            case 116: // 'F5' - Refresh Page
                evt.keyCode = 0;
                evt.returnValue = false;
                break;
        }
    }
    document.onkeydown = suppressSpecialKey;
    document.onkeypress = suppressSpecialKey;

</script>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <table width="800px" cellspacing="0" cellpadding="2" id="tableView" runat="server">
            <tr>
                <td colspan="6"><asp:panel runat="server" SkinID="sectionHeader1" Height="20px"><span style="vertical-align:Middle;font-weight:bold; font-size:larger;">Split Shipment Detail</span></asp:panel></td>
            </tr>
        </table>
        <br />
        <table ID="tb_SplitShipmentInfo" class="TableWithGridLines" cellpadding ="2" cellspacing="0"  >
            <tr>
                <td class="FieldLabel2" width="150px">Contract No.</td>
                <td><asp:Label ID="lbl_ContractNo" runat="server" /></td>
                <td class="FieldLabel2">Delivery No.</td>
                <td><asp:Label ID="lbl_DeliveryNo"  runat="server" /></td>
                <td class="FieldLabel2">Require Payment</td>
                <td><asp:Label ID="lbl_RequirePayment"  runat="server" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Supplier</td>
                <td colspan="3"><asp:Label ID="lbl_Vendor"   runat="server"  /></td>
                <td class="FieldLabel2">Country of Origin</td>
                <td><asp:Label ID="lbl_CO" runat="server"  /></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Payment Term</td>
                <td><asp:Label ID="lbl_PaymentTerm" runat="server" /></td>
                <td class="FieldLabel2">PO At-warehouse Date</td>
                <td><asp:Label ID="lbl_POAtWHDate"  runat="server" /></td>
                <td class="FieldLabel2">Actual At-warehouse Date</td>
                <td><asp:Label ID="lbl_ActualAtWHDate" runat="server" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Packing Description</td>
                <td><asp:Label ID="lbl_piece"  runat="server"   />&nbsp;/&nbsp;
                    <asp:Label ID="lbl_Pack"  runat="server" /></td>
                <td class="FieldLabel2">Total PO Qty</td>
                <td><asp:Label ID="lbl_TotalPOQty" runat="server"  /></td>
                <td class="FieldLabel2">Total Shipped Qty</td>
                <td><asp:Label ID="lbl_TotalShipQty" runat="server" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Currency</td>
                <td><asp:Label ID="lbl_currency" runat="server" /></td>
                <td class="FieldLabel2">Total PO FOB Amt</td>
                <td><asp:Label ID="lbl_TotalPOFOBAmt" runat="server"  /></td>
                <td class="FieldLabel2">Total PO CMT Amt</td>
                <td><asp:Label ID="lbl_TotalPOCMTAmt" runat="server" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Quota Cat. No.</td>
                <td><asp:Label ID="lbl_QuotaCat" runat="server"  /></td>
                <td class="FieldLabel2">Total Shipped FOB Amt</td>
                <td><asp:Label ID="lbl_ttlFOBAmt"  runat="server" /></td>
                <td class="FieldLabel2">Total Shipped CMT Amt</td>
                <td><asp:Label ID="lbl_ttlShipCMTAmt"  runat="server" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2">Notes from Mer.</td>
                <td colspan="5"><asp:Label ID="lbl_MerToShipNote"  runat="server" /></td>            
            </tr>
        </table>  

        <asp:Panel ID="pnl_ItemHeader_old" runat="server" CssClass="header2" style="display:none;">
           <div style="padding:5px;vertical-align: middle;">             
                <div style="float: left; vertical-align: middle;">
                    <asp:ImageButton ID="ImageButton1_old" runat="server" ImageUrl="../images/expand.jpg" 
                    OnClientClick="div_ItemCollapsed.style.visibility = div_ItemCollapsed.style.visibility == 'visible'? 'hidden' : 'visible';" 
                    AlternateText="(Show Details...)"/>
                </div>
                <div style="float: left;">&nbsp;Item Information&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>
                <div style="float:left;visibility:visible;" id="div_ItemCollapsed_old">                        
                <asp:Label ID="lbl_ItemNoHeader_old" runat="server" Text="Item No." SkinID="FieldLabel" />
                    <asp:Label ID="Label26" runat="server" Font-Bold="false"   />
                  </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnl_ItemHeader" runat="server" skinid="sectionHeader1" Width="800px" height="23" >
            <div style="padding:2px; vertical-align: middle;">             
                <div style="float: left; vertical-align: middle;">
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/summy_reverse.jpg" 
                        OnClientClick="div_ItemCollapsed.style.visibility = div_ItemCollapsed.style.visibility == 'visible'? 'hidden' : 'visible';" 
                        AlternateText="(Show Details...)"/>
                </div>
                <div style="float: left; font-size:9pt; vertical-align:middle;">&nbsp;&nbsp;Item Information&nbsp;&nbsp;&nbsp;&nbsp;</div>
                <div style="float:left;visibility:visible;" id="div_ItemCollapsed">                        
                    <asp:Label ID="lbl_ItemNoHeader" runat="server" Text="Item No." SkinID="FieldLabel" />
                    <asp:Label ID="Label2" runat="server" Font-Bold="false"   />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnl_ItemDetail" runat="server" width="800px" style="padding-left:19px;">
            <table cellpadding="5" cellspacing="0" class="TableWithGridLines"
                style="width:600px; margin-left:00px;">
                <tr>
                    <td class="FieldLabel2" style="width:100px;">Season/Phase</td>
                    <td >
                        <asp:Label ID="lbl_Season" runat="server" />&nbsp;,&nbsp;<asp:Label ID="lbl_Phase" runat="server"    />
                    </td>
                    <td class="FieldLabel2" >Split Item Description</td>
                    <td><asp:Label ID="lbl_Description" runat="server"  /></td>
                </tr>
                <tr>
                    <td class="FieldLabel2">Item No.</td>
                    <td>
                        <asp:Label ID="lbl_ItemNo" runat="server" />
                    </td>
                    <td class="FieldLabel2">Colour</td>
                    <td>
                        <asp:Label ID="lbl_Color" runat="server"  />
                        <asp:TextBox ID="txt_Color" runat="server" Visible="false" SkinID="TextBoxLarge" />
                    </td>                    
                </tr>
                <tr>
                    <td style="vertical-align:middle;" align="left" class="FieldLabel2_H2">Description</td>
                    <td colspan="3">
                        <asp:Label ID="lbl_Desc1" runat="server"/>
                        <asp:Label ID="lbl_Desc2" runat="server"/>
                        <asp:Label ID="lbl_Desc3" runat="server"/>
                        <asp:Label ID="lbl_Desc4" runat="server"/>
                        <asp:Label ID="lbl_Desc5" runat="server"/>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <cc1:CollapsiblePanelExtender ID="cpe_Item" runat="Server"
            TargetControlID="pnl_ItemDetail"
            ExpandControlID="ImageButton1"
            CollapseControlID="ImageButton1" 
            Collapsed="true"                    
            ImageControlID="ImageButton1"    
            ExpandedText="(Hide Details...)"
            CollapsedText="(Show Details...)"
            ExpandedImage="~/images/summy.jpg"
            CollapsedImage="~/images/summy_reverse.jpg"
            SuppressPostBack="true" />

        <br />     
        <asp:HiddenField ID="hid_WorkflowStatus" runat="server" Value="" />
        <cc1:TabContainer ID="TabContainer1" runat="server" Width="800px"  CssClass="ajax__tab_revamp">
            <cc1:TabPanel ID="tab_Option" runat="server" Width="100%">
                <HeaderTemplate>Size Option</HeaderTemplate>
                <ContentTemplate>
                    <asp:GridView ID="gv_Options" runat="server"  OnRowDataBound="OptionRowDataBound"
                     AutoGenerateColumns="False" ShowFooter="True"  >
                    <Columns>
                        <asp:TemplateField HeaderText="Option No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_OptionNo" Text='<%# Eval("SizeOption.SizeOptionNo") %>' runat="server" />
                            </ItemTemplate>
                            <ControlStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Size">
                            <ItemTemplate>
                                <asp:Label ID="lbl_SizeDesc" Width="50" Text='<%# Eval("SizeOption.SizeDescription") %>' runat="server" />
                            </ItemTemplate>
                            <ControlStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="PO Qty">
                            <ItemTemplate>
                                <asp:Label ID="lbl_POQty" runat="server" Width="50" Text='<%# Eval("POQuantity") %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lbl_TotalFooter" runat="server" Text="Total" Font-Bold="true"   />
                            </FooterTemplate>
                            <ControlStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipped Qty">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ShipQty" runat="server" Width="50" Text='<%# Eval("ShippedQuantity","{0:#,##0}") %>' ForeColor="Blue" EnableTheming="False" Font-Size="8pt" />
                            </ItemTemplate>
                            <FooterTemplate >
                                <asp:Label ID="lbl_TotalShippedQty" runat="server" ForeColor="Blue" Font-Bold="true" EnableTheming="False" Font-Size="8pt" />
                            </FooterTemplate>
                            <ControlStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipped Qty">
                            <ItemTemplate>
                                <asp:TextBox ID="txt_ShipQty" runat="server" Text='<%# Eval("ShippedQuantity") %>' />
                            </ItemTemplate>
                            <FooterTemplate >
                                <asp:Label ID="lbl_TotalShippedQty2" runat="server" ForeColor="Blue" Font-Bold="true" EnableTheming="False" Font-Size="8pt" />
                            </FooterTemplate>
                            <ControlStyle Width="50px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Selling Price">
                            <ItemTemplate>
                                <asp:Label ID="lbl_NtoCSellPrice" runat="server" Width="50" Text='<%# Eval("ReducedSellingPrice") %>' />
                            </ItemTemplate>
                            <ControlStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Selling Amt">
                            <ItemTemplate>
                                <asp:Label ID="lbl_SellingAmt" runat="server" Width="50" />
                            </ItemTemplate>
                            <ControlStyle Width="80px" />
                            <FooterTemplate>
                                <asp:Label ID="lbl_SellAmtTotal" runat="server"   />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Price">
                            <ItemTemplate>
                                <asp:Label ID="lbl_FtyPrice" runat="server" Width="50" Text='<%# Eval("ReducedSupplierGmtPrice") %>' />
                            </ItemTemplate>
                            <ControlStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Amt">
                            <ItemTemplate>
                                <asp:Label ID="lbl_OptionFactoryAmt" runat="server" Width="50" />
                            </ItemTemplate>
                            <ControlStyle Width="80px" />
                            <FooterTemplate>
                                <asp:Label ID="lbl_FactoryAmtTotal" runat="server"  />
                            </FooterTemplate>
                        </asp:TemplateField>
                        </Columns>
                    <EmptyDataTemplate>
                        No option information.
                        </EmptyDataTemplate>
                    </asp:GridView>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tab_Payment" runat="server"  Width="100%">
                <HeaderTemplate>Payment Information</HeaderTemplate>
                <ContentTemplate>
                <table width="100%">
                    <tr>
                        <td ID="td_LCInfo" style="width:50%;">
                            <table width="100%" cellpadding="5" cellspacing="0">
                            <tr>
                                <td class="header2" colspan="2">L/C Information</td>
                            </tr>
                            <tr>
                                <td><asp:CustomValidator ID="val_LCInfo" runat="server" OnServerValidate="val_LCInfo_Validate" ErrorMessage="" /></td>
                            </tr>
                            </table>
                            <table class="TableWithGridLines" cellpadding="2" cellspacing="0">
                            <tr>
                                <td class="FieldLabel2" style="width:180px;">Supplier Invoice No.</td>
                                <td style="width :100px;"><asp:Label ID="lbl_SuppInvNo" runat="server" />
                                        <asp:TextBox ID="txt_SuppInvNo" runat="server" Visible="false" />
                                </td>
                            </tr>
                            <tr id="row_QACom" runat="server" >
                                <td class="FieldLabel2">QA Commission</td>
                                <td><asp:Label ID="lbl_QACommPercent" runat="server" />%,&nbsp;
                                    <asp:Label ID="lbl_QACommAmt" runat="server" /></td>
                            </tr>
                            <tr id="row_TRDiscount" runat="server" >
                                <td class="FieldLabel2">Vendor Payment Discount</td>
                                <td>
                                    <asp:Label ID="lbl_DiscountPercent" runat="server" />%,&nbsp;
                                    <asp:Label ID="lbl_DiscountAmt" runat="server" />
                                </td>
                            </tr>
                            <tr id="row_LabTest" runat="server">
                                <td class="FieldLabel2">Lab Test Income</td>
                                <td>
                                    <asp:Label ID="lbl_LabTestIncome" runat="server" />,&nbsp;
                                    <asp:Label ID="lbl_LabTestIncomeAmt" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Net Amount to Supplier</td>
                                <td><asp:Label ID="lbl_NetAmtToSupp" runat="server"  /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">L/C Batch No.</td>
                                <td><asp:Label ID="lbl_LCBatchNo" runat="server"  /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">L/C Batch Submission Date</td>
                                <td><asp:Label ID="lbl_LCBatchSubmitDate" runat="server"  /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">L/C No.</td>
                                <td><asp:Label ID="lbl_LCNo" runat="server"  />
                                    <asp:TextBox ID="txt_LCNo" runat="server" Visible ="false"  />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">L/C Issue Date</td>
                                <td>
                                    <asp:Label ID="lbl_LCIssueDate" runat="server" />
                                    <asp:TextBox ID="txt_LCIssueDate" runat="server" Visible ="false" onblur="formatDateString(this);" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">L/C Expiry Date and Amount</td>
                                <td><asp:Label ID="lbl_LCExpiryDate" runat="server" />
                                    <asp:TextBox ID="txt_LCExpiryDate" runat="server" Visible="false" SkinID="DateTextBox" onblur="formatDateString(this);" /><asp:ImageButton 
                                    id="btn_cal_LCExpiryDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex="9999" />
                                    <cc1:CalendarExtender ID="ce_LCExpiryDate" runat="server" Format="dd/MM/yyyy" PopupButtonID="btn_cal_LCExpiryDate"
                                        FirstDayOfWeek="Sunday" TargetControlID="txt_LCExpiryDate">
                                    </cc1:CalendarExtender>,&nbsp;
                                    <asp:Label ID="lbl_LCAmt" runat="server" />
                                    <asp:TextBox ID="txt_LCAmt" runat="server" Visible="false" SkinID="DateTextBox" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">L/C Payment Checked</td>
                                <td><asp:CheckBox ID="ckb_LCPaymentChecked" runat="server" Enabled="false" />
                                    <asp:Label ID="lbl_LCPaymentCheckedDate" runat="server" />
                                    <asp:TextBox ID="txt_LCPaymentCheckedDate" runat="server" SkinID="DateTextBox" Visible="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">L/C Application No.</td>
                                <td><asp:Label ID="lbl_LCAppNo" runat="server" /></td>
                            </tr>
                            </table>
                        </td>
                        <td ID="td_AccountPaymentInfo" style="width:50%; vertical-align :top;">
                            <table width="100%" cellpadding="5" cellspacing="0">
                            <tr>
                                <td class="header2" colspan="2">Account Payment Information</td>
                            </tr>
                            <tr>
                                <td><asp:CustomValidator ID="val_PaymentInfo" runat="server" OnServerValidate="val_PaymentInfo_Validate" ErrorMessage="" /></td>
                            </tr>
                            </table>
                            <table class="TableWithGridLines" cellpadding="2" cellspacing="0">                                
                            <tr>
                                <td class="FieldLabel2">Shipping Doc Receipt Date</td>
                                <td style="width:100px;"><asp:Label ID="lbl_ShipRecvDocDate" runat="server" />
                                    <asp:TextBox ID="txt_ShipRecvDocDate" runat="server" Visible="false" SkinID="DateTextBox" onblur="formatDateString(this);" />
                                    <asp:CheckBox ID="ckb_ShipRecvDocDate" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Account Doc Received Date</td>
                                <td style="width:100px;"><asp:Label ID="lbl_ACRecvDocDate" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2" style="width:180px;">Epicor Interfaced Date</td>
                                <td><asp:Label ID="lbl_SUNInterfaceDate" runat="server"  /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Bank Reference No.</td>
                                <td><asp:Label ID="lbl_BankRefNo" runat="server"  /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Settlement Amount</td>
                                <td><asp:Label ID="lbl_SettlementAmt" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Settlement Date</td>
                                <td><asp:Label ID="lbl_SettlementDate" runat="server"    /></td>
                            </tr>
                            <tr id="tr_ShipDocCheck" runat="server">
                                <td class="FieldLabel2" >Shipping Doc. Check Amount</td>
                                <td style="width:110px;"><asp:Label ID="Label1" runat="server" />
                                    <asp:CheckBox ID="ckb_ShipDocCheck" runat="server" OnCheckedChanged="ckb_ShipDocCheck_CheckedChanged"/>
                                    <asp:Label ID="lbl_ShipDocCheckAmount" runat="server" style="display:none;" />
                                    <asp:TextBox ID="txt_ShipDocCheckAmount" runat="server" style="border:none;width:50px; color:Black;"  ReadOnly="true" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2" runat="server">LG Due Date</td>
                                <td style="width: 110px;" runat="server">
                                    <asp:Label ID="lbl_LGDueDate" runat="server" />
                                </td>
                            </tr>

                            </table>
                        </td>
                    </tr>
                </table>
                </ContentTemplate>
            </cc1:TabPanel>
        </cc1:TabContainer>

        <br />
        <br />
        <asp:Button ID="btn_Save" runat="server" Text="Save" Visible="false" onclick="btn_Save_Click"/>
        <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" Visible ="false" OnClick="btn_Cancel_Click" />
        <asp:Button ID="btn_Edit" runat="server" Text="Edit" onclick="btn_Edit_Click" Visible="false" />&nbsp;&nbsp;&nbsp;        
        <asp:Button ID="btn_Close" runat="server" Text="Close" OnClientClick="window.close();" />
    </div>
    </form>
</body>
</html>
