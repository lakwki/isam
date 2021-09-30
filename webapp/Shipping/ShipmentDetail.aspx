<%@ Page Language="C#" MaintainScrollPositionOnPostback="true" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="ShipmentDetail.aspx.cs" Inherits="com.next.isam.webapp.shipping.ShipmentDetail" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Shipment Detail</title>
<link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
<script src="../common/common.js" type="text/javascript">

</script>

<script type="text/javascript">
    window.focus();

    

    var argShipmentId = "0";
    var argDefaultReceiptDate = "";
    var argShipmentIdList = "";


    //function getArguments() {
    //alert(window.dialogArguments);
    var args = window.dialogArguments;
    var arg="";
    if (args != null) {
        arg = args.toLowerCase().split("&");
        var i;
        for (i = 0; i < arg.length; i++) {
            if (arg[i].indexOf("shipmentid") >= 0)
                argShipmentId = arg[i].replace("shipmentid", "").replace("=", "").replace("  ", "");
            if (arg[i].indexOf("defaultreceiptdate") >= 0)
                argDefaultReceiptDate = arg[i].replace("defaultreceiptdate", "").replace("=", "").replace("  ", "");
            if (arg[i].indexOf("shipmentidlist") >= 0)
                argShipmentIdList = arg[i].replace("shipmentidlist", "").replace("=", "").replace("  ", "");
        }
    }

    function getShipmentId(obj) {
        obj.value = argShipmentId;
        obj.text = argShipmentId;
    }

    //}


    function setItemDescAsMaster(btn, btnId) {
        var prefix;
        var i;
        var desc;

        prefix = btn.id.replace(btnId, "");
        for (i = 1; i <= 5; i++) {
            desc = document.all.item(prefix + "txt_Desc" + i.toString()).value;
            document.all.item(prefix + "lbl_MasterItemDesc" + i.toString()).innerText = desc;
            document.all.item(prefix + "lbl_MasterItemDesc" + i.toString()).style.display = (desc == "" ? "none" : "block");
        }
        btn.title = "The Item Description will be set as a Master Copy when you click the 'SAVE' button";
        //document.all.item(prefix + "btn_copyItemDescFromMaster").enabled = true;
        //document.all.item(prefix + "btn_copyItemDescFromMaster").ToolTip = "Copy Item Description From Master Description";
        document.all.item(prefix + "ckb_setMaster").checked = true;
    }


    function updateItemDescFromMaster(btn, btnId) {
        var prefix;
        var i;

        prefix = btn.id.replace(btnId, "");
        for (i = 1; i <= 5; i++) {
            document.all.item(prefix + "txt_Desc" + i.toString()).value = document.all.item(prefix + "lbl_MasterItemDesc" + i.toString()).innerText;
        }
    }


    function confirmIfTotalShippedQtyZero(allowZeroQty) {
        var totalQty = 0, qty = 0;
        var anyShippedQty = false;
        var invNo = '', invDate = '';

        if (document.all.tcContract_tabInvoice_lbl_InvDate != null)
            invDate = document.all.tcContract_tabInvoice_lbl_InvDate.innerText;
        if (document.all.tcContract_tabInvoice_txt_InvDate != null)
            invDate = document.all.tcContract_tabInvoice_txt_InvDate.value;
        invNo = document.all.tcContract_tabInvoice_lbl_InvNo.innerText;
        if (invNo != '' || invDate != '') {
            var nodeList = document.getElementsByTagName('input');
            for (i = 0, anyInvalid = false; i < nodeList.length && !anyInvalid; i++) {
                if (nodeList[i].name.indexOf("gv_Options") > 0 && nodeList[i].name.indexOf("txt_ShipQty") > 0) {
                    anyShippedQty = true;
                    str = nodeList[i].value.replace(",", "").trim();
                    qty = parseInt(str, 10);
                    if (qty >= 0 && str.replace(qty.toString(), "") == "")
                        totalQty += qty;
                    else
                        anyInvalid = true;

                }
            }
            if (anyInvalid) {
                alert('Invalid shipped quantity.');
                setSaveMode('OFF');
                return false;
            }
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
        }
        return true;
    }


   function showDiscrepancyAlertLog()
   {
        logLayer = document.getElementById("div_DiscrepancyAlertLog");
        logLayer.style.display = "block";
                      
        logLayer.style.left = event.clientX + document.documentElement.scrollLeft ;
        logLayer.style.top = event.clientY + document.documentElement.scrollTop - 150;        
   }
   
   function hideDiscrepancyAlertLog()
   {
        document.getElementById("div_DiscrepancyAlertLog").style.display = "none";
   }

   function isShipDocCheckValid(obj, objName) {
        var prefix = obj.id.replace(objName, "");
        var oVendorNetAmt = document.getElementById(prefix + "lbl_NetAmt");
        var oShipDocCheck = document.getElementById(prefix + "ckb_ShipDocCheck");
        var oShipDocCheckAmt = document.getElementById(prefix + "txt_ShipDocCheckAmount");
        
        var oSuppInvNo = document.getElementById(prefix + "txt_VendorInvoiceNo");
        var oNoOfCarton = document.getElementById(prefix + "txt_InvPiece");
        var oShipDocRecDate = document.getElementById(prefix + "txt_ShipRecDocDate");

        if (oShipDocCheck.checked && (oSuppInvNo.value == "" || oNoOfCarton.value == "" || oShipDocRecDate.value == "")) {
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

    // Save Action Control
   function enableButtons(flag) {
       if (document.getElementById('btn_Save') != null)
           document.getElementById('btn_Save').disabled = !flag;
       if (document.getElementById('btn_Print') != null)
           document.getElementById('btn_Print').disabled = !flag;
       if (document.getElementById('btn_PrintDN') != null)
           document.getElementById('btn_PrintDN').disabled = !flag;
       if (document.getElementById('lnk_Prev') != null)
           document.getElementById('lnk_Prev').disabled = !flag;
       if (document.getElementById('lnk_Next') != null)
           document.getElementById('lnk_Next').disabled = !flag;
       if (document.getElementById('btn_Cancel') != null)
           document.getElementById('btn_Cancel').disabled = !flag;
       if (document.getElementById('btn_Close') != null)
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
           case 116: // 'F5' - Refresh Page, block the key
               evt.keyCode = 0;
               evt.returnValue = false;
               break;
       }
   }

   function filterInput(isInternalITIPAddress) {
       document.onkeydown = suppressSpecialKey;
       document.onkeypress = suppressSpecialKey;
   }

   var gridPanelDimension = { height: 200, width: 650 };
   function scrollWindowDimension() {
       var windowTop = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop;
       var windowLeft = document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft;
       var windowRight = (document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body.clientWidth) + document.documentElement.scrollLeft;
       var windowBottom = (document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight) + document.documentElement.scrollTop;
       return { top: windowTop, left: windowLeft, right: windowRight, bottom: windowBottom, height: windowBottom - windowTop, width: windowRight - windowLeft }
   }

   function setPanelBoundary(panelLeft, panelTop) {
       var elementLeft, elementTop; //x and y
       var scrollWindow = scrollWindowDimension();
       elementTop = scrollWindow.top + panelTop;
       elementLeft = scrollWindow.left + panelLeft;
       return { left: elementLeft, top: elementTop, right: elementLeft + gridPanelDimension.width, bottom: elementTop + gridPanelDimension.height }
   }

   function setPanel(display, button) {
       //debugger;
       var panel;
       var nodes;
       nodes = document.getElementsByTagName("DIV");
       var i;
       for (i = 0; i < nodes.length; i++)
           if (nodes[i].id.indexOf("div_LCInfoOfOtherDly") != -1) {
               panel = nodes[i];
               break;
           }

       if (panel != undefined)
           if (display != undefined) {
               if (display.toUpperCase() == "ON") {
                   if (panel != undefined)
                       panel.style.display = "block";
                   if (button != undefined) {
                       var buttonBoundary = button.getBoundingClientRect();
                       var panelBoundary = setPanelBoundary(buttonBoundary.right - 155, buttonBoundary.top - 25);

                       //debugger;

                       //img_LcOtherDelivery
                       nodes = panel.getElementsByTagName("TR");
                       var panelHeight = ((nodes.length <= 10 ? nodes.length : 10) + 5) * 16;
                       panel.style.left = panelBoundary.left;
                       panel.style.top = panelBoundary.top;
                       panel.style.width = gridPanelDimension.width;
                       panel.style.height = panelHeight;   // gridPanelDimension.height;

                       var scrollWindow = scrollWindowDimension()
                       var scrollY = panelBoundary.top + panelHeight - scrollWindow.bottom;
                       if (scrollY > 0)
                           window.scrollBy(0, scrollY);
                   }
               }
               else if (display.toUpperCase() == "OFF") {
                   if (panel != undefined)
                       panel.style.display = "none";
               }
           }

   }

   function requireDeductionDocNo(deductionTypeValue) {
       return deductionTypeValue.endsWith(".0");
   }

   function getDeductionRowObjects(tableRow) {
       ddlType = tableRow[1].getElementsByTagName("SELECT")[0];
       txtDocNo = tableRow[2].getElementsByTagName("INPUT")[0];
       txtAmt = tableRow[4].getElementsByTagName("INPUT")[0];
       txtRemark = tableRow[5].getElementsByTagName("INPUT")[0];
       return { ddlType: ddlType, txtDocNo: txtDocNo, txtAmt: txtAmt, txtRemark: txtRemark };
   }

   function deductionType_OnChange() {
       var me = event.target || event.srcElement;
       var deduction = getDeductionRowObjects(me.parentNode.parentNode.cells);
       // set visibility of deduction objects
       var myOption = deduction.ddlType[deduction.ddlType.selectedIndex];
       var displayOn = (deduction.txtDocNo.style.display != "none");
       deduction.txtDocNo.style.display = (requireDeductionDocNo(myOption.value) ? "" : "none");
       if (displayOn && deduction.txtDocNo.style.display == "none")
           deduction.txtDocNo.value = "";

       displayOn = (deduction.txtAmt.style.display != "none");
       deduction.txtAmt.style.display = (myOption.value != "0" ? "" : "none");
       deduction.txtAmt.readOnly = (myOption.value == "0" || myOption.text == "Remark");
       if (myOption.text == "Remark")
           deduction.txtAmt.value = "";

       if (displayOn && deduction.txtAmt.style.display == "none")
           deduction.txtAmt.value = "";

       displayOn = (deduction.txtRemark.style.display != "none");
       deduction.txtRemark.style.display = (myOption.value != "0" ? "" : "none");
       if (displayOn && deduction.txtRemark.style.display == "none")
           deduction.txtRemark.value = "";

       return;
   }

/*
   function getDeductionArray() {
       var i, j, inputs, selects;
       var ddlType, txtDocNo, txtAmt;
       var div = document.getElementById("tcContract_TabDeduction_gv_PaymentDeductionUpdate");
       var deductions = [];
       if (div != null) {
           var rows = div.getElementsByTagName("TR")
           // Create deduction array
           for (j = 0; j < rows.length; j++) {
               if ((selects = rows[j].getElementsByTagName("SELECT")).length > 0) {
                   ddlType = selects[0];
                   inputs = rows[j].getElementsByTagName("INPUT");
                   for (i = 0; i < inputs.length; i++) {
                       if (inputs[i].id.indexOf("txt_DocNo") != -1) txtDocNo = inputs[i];
                       else if (inputs[i].id.indexOf("txt_Amount") != -1) txtAmt = inputs[i];
                       //else if (inputs[i].id.indexOf("lnk_Import") != -1) lnkImport = inputs[i];
                   }
                   deductions.push({ddlType: ddlType, txtDocNo: txtDocNo, txtAmt: txtAmt });  //,lnkImport);
               }
           }
       }
       return deductions;
   }
*/

</script>

    </head>

<body>
    <form id="frm_ShipmentDetail" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="pnl_parameters">
        <asp:HiddenField ID="hid_Parameter_ShipmentId" runat="server" Value="217005" />
        <asp:HiddenField ID="hid_Parameter_DefaultReceiptDate" runat="server" Value="1/1/2011" />
        <asp:HiddenField ID="hid_Parameter_ShipmentIdList" runat="server" Value="217005" />
     </div>

    <div style="padding-top:10px;padding-left:3px;">
        
        <asp:panel ID="pnl_ShipmentHeading" runat="server"  SkinID="sectionHeader1" >
            <table width="100%" cellspacing="0" cellpadding="0" border="0" >
                <tr height="22px">
                    <td style="width:80px;vertical-align:middle;" >&nbsp;NSL PO No.</td>
                    <td colspan="3">
                        <asp:TextBox ID="txt_PONo" CssClass="nonEditableTextbox" SkinID="MTextBox"  runat="server" ReadOnly="true" />
                        <asp:Image ID="img_dualSourcing" ImageUrl="~/images/icon_DualSourcing.gif" ToolTip="Dual Sourcing" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_SZOrder" ToolTip="NSL(SZ) Order" ImageUrl="~/images/icon_SZOrder.gif" Visible="false" runat="server" ImageAlign="Middle"  />
                        <asp:Image ID="img_discount" ToolTip="Discount Order" ImageUrl="~/images/icon_DiscountOrder.gif" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_OPRFabric" ToolTip="OPR Order" ImageUrl="~/images/icon_OPRFabricOrder.gif" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_releaseLock" ToolTip="Lock Released" ImageUrl="~/images/icon_ReleaseLock.gif" Visible="false" runat="server"  ImageAlign="Middle" />
                        <asp:Image ID="img_UTurn" ToolTip="U-Turn Order" imageUrl="~/images/icon_UTurn.gif" Visible="false" runat="server" ImageAlign="Middle"  />
                        <asp:Image ID="img_MockShop" ToolTip="Mock Shop Sample" ImageUrl="~/images/icon_MockShop.gif" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_PressSample" ToolTip="Press Sample" ImageUrl="~/images/icon_PressSample.gif" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_StudioSample" ToolTip="Studio Sample" ImageUrl="~/images/icon_StudioSample.gif" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_LDP" ToolTip="LDP Order" ImageUrl="~/images/icon_LDP.gif" Visible ="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_WithQCCharge" ToolTip="With QC Charge" ImageUrl="~/images/icon_QCCharge.gif" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_AirShipment" ToolTip="Air Freight" imageUrl="~/images/icon_Air.gif" Visible="false" runat="server" ImageAlign="Middle"  />
                        <asp:Image ID="img_AirFreightWithPay" ToolTip="Air Freight with pay" imageUrl="~/images/icon_air_Purple.gif" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_TradingAF" ToolTip="Trading Air Freight" imageUrl="~/images/icon_air_Red.gif" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_ReprocessGoods" ToolTip="Reprocess Goods" imageUrl="~/images/icon_ReprocessGoods.gif" Visible="false" runat="server" ImageAlign="Middle"  />
                        <asp:Image ID="img_GBTestRequired" ToolTip="China GB Test Required" ImageUrl="~/images/icon_Test.png" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_GBTestPass" ToolTip="China GB Test Pass" ImageUrl="~/images/icon_pass.png" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_GBTestFailedRelease" ToolTip="China GB Test Fail (Release Payment)"  ImageUrl="~/images/icon_Fail.png" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_GBTestFailedHold" ToolTip="China GB Test Fail (Hold Payment)" ImageUrl="~/images/icon_Fail_orange.png" Visible="false" runat="server" ImageAlign="Middle" />
                        <asp:Image ID="img_GBTestFailedCannotRelease" ToolTip="China GB Test Fail (Cannot Release Payment)" ImageUrl="~/images/icon_Fail_red.png" Visible="false" runat="server" ImageAlign="Middle" />
                        <!---<asp:Image ID="img_QCCInspection" ToolTip="QCC Inspection" ImageUrl="~/images/icon_QCCInspection.png" Visible="false" runat="server" ImageAlign="Middle" />--->
                    </td>
                    <td></td>         
                    <td style="text-align:right;">
                        <asp:TextBox ID="txt_Status" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="TextBox_160"/>
                    </td>
                </tr>
            </table>
        </asp:panel> 
    </div>

    <table id="tb_ShipmentDetail" width="760px">
        <tr>
            <td colspan="2" style="width: 49%;">
                <span class="FieldLabel2_W2">Contract/Dly No.</span>
                <asp:TextBox ID="txt_ContractNo" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"
                    SkinID="MTextBox" />
                <asp:TextBox ID="txt_NSLDlyNo" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"
                    SkinID="XXSTextBox" />
                <span class="FieldLabel2">Cust</span>
                <asp:TextBox ID="txt_NUKDlyNo" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"
                    SkinID="XXSTextBox" />
                <span class="FieldLabel2">&nbsp;&nbsp;Office</span>
                <asp:TextBox ID="txt_Office" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"
                    SkinID="XSTextBox" />
            </td>
            <td colspan="2" style="width:51%;">
                <span class="FieldLabel2_W3R">Product Team</span>
                <asp:TextBox ID="txt_Team" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="TextBox_160" />
                <asp:TextBox ID="txt_Mer" CssClass="nonEditableTextbox" runat="server" SkinID="MTextBox" ReadOnly="true" />

            </td>
        </tr>
        <tr id="tr_InboundDelNo" runat="server" visible="false">
            <td colspan="2">
                <span class="FieldLabel2_W2">Inbound Dly No.</span>
                <asp:TextBox ID="txt_InboundDelNo" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" />
            </td>
            <td colspan="2">
                <span class="FieldLabel2_W3R">BDCM Type</span>
                <asp:TextBox Id="txt_BDCMType"  runat="server" ReadOnly="true" SkinId="TextBox_160" CssClass="nonEditableTextbox" />
                <span class="FieldLabel2_W1R" style="width:63px;" >Qty/CTN</span><asp:TextBox ID="txt_QtyPerCarton" 
                runat="server" ReadOnly="true" SkinID="SmallTextBox" CssClass="nonEditableTextbox" />
            </td>
        </tr>
        <tr id="tr_Forwarder" runat="server" visible="false">
            <td colspan="2">
                <span class="FieldLabel2_W2">Forwarder</span>
                <asp:TextBox ID="txt_Forwarder" CssClass="nonEditableTextbox" SkinID="TextBox_250" runat="server" ReadOnly="true" />
            </td>
            <td colspan="2">
                <span class="FieldLabel2_W3R">Sch.DC Dly Date</span>
                <asp:TextBox Id="txt_SchduleDlyDate"  runat="server"  SkinID="MTextBox" ReadOnly="true" CssClass="nonEditableTextbox" />
                <span class="FieldLabel2_W1R" style="width:100px;" >Promotion Start</span>
                <asp:TextBox ID="txt_PromotionStart" runat="server"  SkinID="DateTextBox" ReadOnly="true" CssClass="nonEditableTextbox" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <span class="FieldLabel2_W2">Item No.</span>
                <asp:TextBox ID="txt_ItemNo" CssClass="nonEditableTextbox" runat="server"  ReadOnly="true"  SkinID="DateTextBox" />
                <span class="FieldLabel2" >&nbsp;Season/Phase</span>
                <asp:TextBox ID="txt_Season" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox" />
                <asp:TextBox ID="txt_Phase" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox" />

            </td>
            <td colspan="2">
                <span class="FieldLabel2_W3R">Customer</span>
                <asp:TextBox ID="txt_Customer" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="MTextBox" />
                <span class="FieldLabel2_W1R" style="width:125px;">Trading Agency</span>
                <asp:TextBox ID="txt_TradingAgency"  runat="server" ReadOnly="true" SkinID="SmallTextBox" CssClass="nonEditableTextbox" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <span class="FieldLabel2_W2">Supplier</span>
                <asp:TextBox ID="txt_VendorName" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="TextBoxLarge"  />
                <asp:TextBox ID="txt_VendorCode" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinId="TextBox_50"   />
            </td>
            <td colspan="2">
                <span class="FieldLabel2_W3R">Purchase Term</span>
                <asp:TextBox ID="txt_PurchaseTerm" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox" />
                <asp:TextBox ID="txt_PurchaseCountry" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox"  />
                &nbsp;<span class="FieldLabel2_W1R" style="width:125px;">Pcs per Pack</span>
                <asp:TextBox ID="txt_PiecePerPack" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="SmallTextBox"  /><br />
            </td>
        </tr>
        <tr id="row_VMVendor" runat="server" visible="false">
            <td colspan="2">                    
                <span class="FieldLabel2_W2">VM Supplier</span>
                <asp:TextBox ID="txt_VMVendorName" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="TextBoxLarge"  />
                <asp:TextBox ID="txt_VMVendorCode" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinId="TextBox_50"   />
            </td>
        </tr>
        <tr id="row_Factory" runat="server" visible="false">
            <td colspan="2">
                <span class="FieldLabel2_W2">Factory</span>
                <asp:TextBox ID="txt_FactoryName" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="TextBox_250"  />
            </td>
            <td colspan="2">&nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table cellpadding="1" cellspacing="0" width="97%;" >
                    <tr style="background-color :#EAEAEA;border-color: #B5C3CE;border-style:solid ;border-width:1px; ">
                        <td style="border:none; background-color:transparent;width:15%;">
                            <span class="FieldLabel2_W2">Quota Cat.</span>
                        </td>
                        <td style="border-right:none;text-align:left;">
                            &nbsp;&nbsp;NSS&nbsp;<asp:TextBox ID="txt_NSSQuotaCat" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox"  />
                        </td>
                        <td style="border-left:none; text-align:right;">
                            Cust&nbsp;<asp:TextBox ID="txt_NUKQuotaCat" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox" />&nbsp;
                        </td>
                    </tr>
                </table>

            </td>
            <td colspan="2">
                 <table cellpadding="1" cellspacing="0" width="100%;" >
                    <tr style="background-color :#EAEAEA;border-color: #B5C3CE;border-style:solid ;border-width:1px; ">
                        <td style="border:none; background-color:transparent;width:15%;">
                            <span class="FieldLabel2_W3R">Packing Method</span> 
                        </td>
                        <td style="border-right:none;text-align:left;">
                            &nbsp;&nbsp;NSS&nbsp;<asp:TextBox ID="txt_NSSPackMethod" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox" />
                        </td>
                        <td style="border-left:none; text-align:right;">
                            Cust&nbsp;<asp:TextBox ID="txt_NUKPackMethod" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox"/>&nbsp;
                        </td>
                    </tr>
                </table>

            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table cellpadding="1" cellspacing="0" width="97%;" >
                    <tr style="background-color :#EAEAEA;border-color: #B5C3CE;border-style:solid ;border-width:1px; ">
                        <td style="border:none; background-color:transparent;width:15%;">
                            <span class="FieldLabel2_W2">Refurb</span>
                        </td>
                        <td style="border-right:none;text-align:left;">
                            &nbsp;&nbsp;NSS&nbsp;<asp:TextBox ID="txt_NSSRefurbishment" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox"/>
                        </td>
                        <td style="border-left:none; text-align:right;">
                            &nbsp;Cust&nbsp;<asp:TextBox ID="txt_NUKRefurbishment" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox"/>&nbsp;
                        </td>
                    </tr>
                </table>
            </td>
            <td colspan="2">
                <table cellpadding="1" cellspacing="0" width="100%;" >
                    <tr style="background-color :#EAEAEA;border-color: #B5C3CE;border-style:solid ;border-width:1px; ">
                        <td style="border:none; background-color:transparent;width:15%;">
                            <span class="FieldLabel2_W3R" >Ship from</span>
                        </td>
                        <td style="border-right:none;text-align:left;">
                            &nbsp;&nbsp;NSS&nbsp;<asp:TextBox ID="txt_NSSShipFromCountry" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox"/>
                        </td>
                        <td style="border-left:none; text-align:right;">
                            Cust&nbsp;<asp:TextBox ID="txt_NUKShipFromCountry" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox" />&nbsp;
                        </td>
                    </tr>
                </table>

            </td>
        </tr>
        <tr>
            <td colspan="2">
                <span class="FieldLabel2_W2">C/O</span>
                <asp:TextBox ID="txt_CO" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="TextBox_95"  />
                <span class = "FieldLabel2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OPR Type</span>
                <asp:TextBox ID="txt_OPRType" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox" /><br />
            </td>
            <td colspan="2">
                <table cellpadding="1" cellspacing="0" width="100%;" >
                    <tr style="background-color :#EAEAEA;border-color: #B5C3CE;border-style:solid ;border-width:1px; ">
                        <td  style="border:none; background-color:transparent;width:15%;">
                            <span class="FieldLabel2_W3R">Destination</span>
                        </td>
                        <td style="border-right:none;text-align:left;">
                            &nbsp;&nbsp;NSS&nbsp;<asp:TextBox ID="txt_NSSDestination" runat="server" ReadOnly="true" CssClass="nonEditableTextbox" SkinID="DateTextBox" />  
                        </td>
                        <td style="border-left:none; text-align:right;">
                            Cust&nbsp;<asp:TextBox ID="txt_NUKDestination" runat="server" ReadOnly="true" CssClass="nonEditableTextbox" SkinID="DateTextBox" />&nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="vertical-align:top ;">
                <span class="FieldLabel2_W2">Payment Term</span>
                <asp:TextBox ID="txt_PaymentTerm" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="TextBox_95" />
               <span class=" FieldLabel2">&nbsp;&nbsp;&nbsp;Design Source</span>
               <asp:TextBox ID="txt_DesignSource" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox" />
            </td>
            <td colspan="2" style="vertical-align:top; ">
                <table  cellpadding="1" cellspacing="0" width="100%;" >
                    <tr style="background-color :#EAEAEA;border-color: #B5C3CE;border-style:solid ;border-width:1px; ">
                        <td  style="border:none; background-color:transparent;width:15%;">
                        <span class="FieldLabel2_W3R">Shipment Mode</span>
                        </td>
                        <td style="border-right:none;text-align:left;">
                            &nbsp;&nbsp;NSS&nbsp;<asp:TextBox ID="txt_NSSShipMode" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox" />
                            <asp:TextBox ID="txt_AirFreigthPaymentType" runat ="server" CssClass="nonEditableTextbox" ReadOnly="true" SkinID="SmallTextBox" />
                            
                        </td>
                        <td style="border-left:none; text-align:right;">
                            Cust&nbsp;<asp:TextBox ID="txt_NUKShipMode" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="DateTextBox" />&nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <span class="FieldLabel2_W2">GSP Scheme</span>
                <asp:TextBox ID="txt_GSPScheme" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="TextBox_95" />
            </td>
        </tr>
        <tr id="tr_AirFreightPayment" runat="server" visible="true">
            <td colspan="3">
                &nbsp;
            </td>
            <td style="width:39%; vertical-align:top; margin-left:10;">
               <table cellspacing="0"  style="margin-left:0px; width:285px; font-size:7pt; background-color :#EAEAEA; border-left : solid 1px #B5C3CE; border-right : solid 1px #B5C3CE;">
                  <tr style="display:none;">
                    <td>
                        <asp:CheckBox ID="ckb_IsTradingAF" runat="server"  Enabled="false" Visible="false"/>&nbsp;<!-- todo:add Trading A/F label on the left side-->
                    </td>
                  </tr>
                  <tr>
                    <td style="border-right-width:1px; border-right-style:solid ; border-right-color: #B5C3CE; width:185px;">
                        NUK-<asp:TextBox ID="txt_NUKAirFreightPaymentPercent" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox"  />%
                        NSL-<asp:TextBox ID="txt_NSLAirFreightPaymentPercent" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox"  />%
                        FTY-<asp:TextBox ID="txt_FTYAirFreightPaymentPercent" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox" />%<br />

                        Other (<asp:TextBox ID="txt_OtherAirFreightPaymentRemark" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox" />)-
                        <asp:TextBox ID="txt_OtherAirFreightPaymentPercent" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox" />%
                    </td>
                    <td>
                        NUK-<asp:TextBox ID="txt_NextFreightPercent" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox" />%<br />
                        NSL+FTY-<asp:TextBox ID="txt_SupplierFreightPercent" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox"  />%
                    </td>
                  </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="vertical-align:top;">
                <table border="0" cellspacing="0" cellpadding="0" >
                  <tr>
                    <td><span class="FieldLabel2_W3R" style="padding-top:10px;">LDP Order&nbsp;</span><asp:CheckBox ID="ckb_LDPOrder" runat="server" Enabled="false"/></td>
                    <td><span class="FieldLabel2_W1R" style="padding-top:10px;">With QC Charge&nbsp;</span><asp:CheckBox ID="ckb_WithQCCharge" runat="server" Enabled="false" /></td>
                    <td><span class="FieldLabel2_W3R" runat="server" id="lbl_BizDesign" style="padding-top:10px;">Biz Design&nbsp;<asp:CheckBox ID="ckb_IsBizOrder" runat="server" Enabled="false" /></span></td>
                  </tr>
                    <tr>
                    <td colspan="3"><span class="FieldLabel2_W3R" style="padding-top:10px;">China GB Test&nbsp;</span><asp:CheckBox ID="ckb_GBTest" runat="server" Enabled="false" /><asp:TextBox ID="txt_GBTestResult" runat="server" SkinID="MTextBox" Visible="false"  CssClass="nonEditableTextbox" ReadOnly="true" /></td>

                    </tr>
                </table>
            </td>
            <td id="tdAirFreightReason" runat="server" colspan="2">
                <table cellpadding="1" cellspacing="0" width="100%;" >
                    <tr>
                        <td style="width:15%;text-align:right;">
                            <span class="FieldLabel2_H2" style="width:90px;">&nbsp;Trading A/F&nbsp;<br/>&nbsp;Reason&nbsp;</span>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txt_AirFreightReason" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XLTextBox" style="height:30px; width:280px;" 
                                TextMode="multiline" Columns="35" Rows="6" />
                        </td>
                    </tr>
                    <tr style="background-color :#EAEAEA;border-color: #B5C3CE;border-style:solid ;border-width:1px; ">
                        <td  style="border:none; background-color:transparent;width:15%;">
                            <span class="FieldLabel2_W3R">Trading A/F Cost</span>
                        </td>
                        <td style="border-right:none;text-align:left;">
                            &nbsp;Estimate&nbsp;<asp:TextBox ID="txt_TradingAFEstimateCost" runat="server" ReadOnly="true" CssClass="nonEditableTextbox" SkinID="DateTextBox" />  
                        </td>
                        <td style="border-left:none; text-align:right;">
                            &nbsp;Actual&nbsp;<asp:TextBox ID="txt_TradingAFActualCode" runat="server" ReadOnly="true" CssClass="nonEditableTextbox" SkinID="DateTextBox" />&nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>           
    <table id="tb_ShipmentDetail2" style="background-color :#EAEAEA; border-width:1px; border-style:solid ; border-color: #B5C3CE;" width="750px">
        <tr>
            <td class="FieldLabel2_W2">Mock&nbsp;Shop&nbsp;Sample</td>
            <td>
                <asp:TextBox ID="txt_MockShopSample" runat="server" CssClass="nonEditableTextbox" ReadOnly="true" SkinID="XLTextBox" />
            </td>
            <td rowspan="2" >
                <span class="FieldLabel3">Fumigation Cert Req'd</span>
                <asp:TextBox ID="txt_FuCert" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XXSTextBox" /> 
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2_W2">Notes from Mer.</td>
            <td><asp:TextBox ID="txt_NoteFromMer" CssClass="nonEditableTextbox" runat="server" ReadOnly="true" SkinID="XLTextBox" /></td>
        </tr>
    </table>

    <table>  
    <tr>
        <td colspan="4">
            <asp:Panel ID="pnl_SummaryHeader" runat="server" skinid="sectionHeader1" Width="750px" height="23" >
                <div style="padding:2px; vertical-align: middle;">             
                    <div style="float: left; vertical-align: middle;">
                        <asp:ImageButton ID="Image1" runat="server" ImageUrl="~/images/summy_reverse.jpg" AlternateText="(Hide Details...)"/>
                    </div>
                    <div style="float: left; font-size:9pt; vertical-align:middle;">&nbsp;&nbsp;Summary</div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnl_SummaryDetail" runat="server">
                <table style="text-align:center; margin-left:20px;" class="TableWithGridLines"
                    cellpadding="5" cellspacing="0" >
                    <tr class="FieldLabel">
                        <td rowspan="2"></td>
                        <td colspan="3" style="border-left: solid thin #A0A0A0;">Order Detail</td>
                        <td colspan="3" style="border-left: solid thin #A0A0A0;">Actual Shipment Detail</td>
                    </tr>
                    <tr class="FieldLabel">
                        <td style="width:85px;border-left: solid thin #A0A0A0;">Cust</td>
                        <td  style="width:85px;">Cust-to-NSL</td>
                        <td style="width:85px;">NSL-to-Supp</td>
                        <td style="width:85px;border-left: solid thin #A0A0A0;">Cust</td>
                        <td style="width:85px;">Cust-to-NSL</td>
                        <td style="width:85px;">NSL-to-Supp</td>
                    </tr>
                    <tr>
                        <td class="FieldLabel"><asp:Label ID="lbl_AtWHDate" runat="server" >At-Warehouse Date</asp:Label></td>
                        <td style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKAtWHDate" runat="server"  /></td>
                        <td><asp:Label ID="lbl_CtoNSLAtWHDate" runat="server" /></td>
                        <td><asp:Label ID="lbl_NSLtoSuppAtWHDate" runat="server" /></td>
                        <td style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKActualAtWHDate" runat="server" /></td>
                        <td><asp:Label ID="lbl_NSLActualAtWHDate" runat="server"  /></td>
                        <td><asp:Label ID="lbl_SuppActualAtWHDate" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">Currency</td>
                        <td runat="server" id="td_NUKCcy" style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKCcy" runat="server"  /></td>
                        <td><asp:Label ID="lbl_CtoNSLCcy" runat="server"  /></td>
                        <td><asp:Label ID="lbl_NSLtoSuppCcy" runat="server"  /></td>
                        <td ID="td_NUKActualCcy" runat="server" style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKActualCcy" runat="server"  /></td>
                        <td><asp:Label ID="lbl_NSLActualCcy" runat="server"  /></td>
                        <td><asp:Label ID="lbl_SupplierActualCcy" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">Quantity</td>
                        <td style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKQty" runat="server"  /></td>
                        <td><asp:Label ID="lbl_CtoNSLQty" runat="server"   /></td>
                        <td><asp:Label ID="lbl_NSLtoSuppQty" runat="server"  /></td>
                        <td style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKActualQty" runat="server"  /></td>
                        <td><asp:Label ID="lbl_NSLActualQty" runat="server"  /></td>
                        <td><asp:Label ID="lbl_SupplierActualQty" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel">Sales Amount</td>
                        <td style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKSalesAmt" runat="server"  /></td>
                        <td><asp:Label ID="lbl_CtoNSLSalesAmt" runat="server" /></td>
                        <td>--</td>
                        <td style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKActualSalesAmt" runat="server" /></td>
                        <td><asp:Label ID="lbl_NSLActualSalesAmt" runat="server" /></td>
                        <td>--</td>
                    </tr>
                    <tr id='row_SupplierAmount' runat='server'>
                        <td class="FieldLabel">Supplier Amount</td>
                        <td style="border-left: solid thin #A0A0A0;">--</td>
                        <td>--</td>
                        <td><asp:Label ID="lbl_NSLtoSuppAmt" runat="server" /></td>
                        <td style="border-left: solid thin #A0A0A0;"><asp:Label ID="lbl_NUKActualSuppAmt" runat="server" /></td>
                        <td>--</td>
                        <td><asp:Label ID="lbl_SupplierActualSuppAmt" runat="server" /></td>
                    </tr>
                </table>
            
            </asp:Panel>
                <cc1:CollapsiblePanelExtender ID="cpe_Summary" runat="Server"
                    TargetControlID="pnl_SummaryDetail"
                    ExpandControlID="Image1"
                    CollapseControlID="Image1" 
                    Collapsed="false"                    
                    ImageControlID="Image1"    
                    ExpandedText="(Hide Details...)"
                    CollapsedText="(Show Details...)"
                    ExpandedImage="~/images/summy.jpg"
                    CollapsedImage="~/images/summy_reverse.jpg"
                    SuppressPostBack="true" />

        </td>
    </tr>
    </table>
    
    <table width="800px">
        <tr id="tr_ShipmentDetailTabPage">
            <td colspan="4">
                <cc1:TabContainer ID="tcContract" runat="server" ActiveTabIndex="0" CssClass="ajax__tab_revamp"
                    Width="100%" ScrollBars="Vertical">
                    <cc1:TabPanel ID="tabDocInfo" runat="server" ScrollBars="None">
                        <HeaderTemplate>Document</HeaderTemplate>
                        <ContentTemplate>
                            <table>
                                <tr>
                                    <!-- <td class="header2">Custom Document</td> -->
                                    <td>
                                        <asp:Panel ID="Panel1" runat="server" skinid="sectionHeader2"  Height="22"  >
                                            <div style="float:left;padding:5px;">
                                                <img src="../images/pt3.gif" alt=""/>
                                            </div>
                                            <div style="float:left;padding:3px;vertical-align:middle;" class="header2">Custom Document</div>
                                        </asp:Panel>
                                    </td>             
                                </tr>
                                <tr>
                                    <td>                                 
                                        <asp:GridView ID="gv_DocInfo"  OnRowDataBound="DocInfoDataBound" runat="server" AutoGenerateColumns="False" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Doc. Type">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_DocType" runat="server"   Text='<%# Eval("DocumentType.ShortName") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Doc. No.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_DocNo" runat="server"   Text='<%# Eval("DocumentNo") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Country">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Country" runat="server"   Text='<%# Eval("Country.Name") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Issue Date">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_IssueDate"   Text='<%# Eval("IssueDate","{0:d}") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Expiry Date">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_ExpiryDate"  Text='<%# Eval("ExpiryDate","{0:d}") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Quota Cat.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_QuotaCat"   Text='<%# Eval("QuotaCategory.OPSKey") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Weight (kg)">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Weight"   Text='<%# Eval("Weight") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Qty on Doc.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_QtyOnDoc"  runat="server" Text='<%# Eval("Qty","{0:#,##0}") %>' />
                                                    <asp:Label ID="lbl_QtyOnDocUnit" runat="server" Text='<%# Eval("Unit.OPSKey") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Qty for Order">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_QtyForOrder"   Text='<%# Eval("OrderQty","{0:#,##0}") %>' runat="server" />
                                                    <asp:Label ID="lbl_QtyForOrderUnit" runat="server" Text='<%# Eval("OrderUnit.OPSKey") %>' />
                                                </ItemTemplate>                                        
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Eqv. PO Qty">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_EqvPOQty"   Text='<%# Eval("POQty","{0:#,##0}") %>' runat="server" />
                                                    <asp:Label ID="lbl_EqvPOQtyUnit" runat="server" Text='<%# Eval("POUnit.OPSKey") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Doc Despatch Date to UK">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_UploadDate"   Text='<%# Eval("DespatchToUKDate","{0:d}") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Airway Bill No.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_AWBNo"   Text='<%# Eval("DespatchAWBNo") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            </Columns>
                                        <EmptyDataTemplate>
                                            No document imported.
                                            </EmptyDataTemplate>
                                            </asp:GridView>
                                        <asp:GridView ID="gv_DocInfoUpdate"  OnRowUpdating="CusDocRowUpdating" OnRowDataBound="DocInfoUpdateDataBound"
                                            OnRowCommand="CusDocRowCommand" runat="server" AutoGenerateColumns="False" OnRowCancelingEdit="CusDocRowCancelEdit"
                                            OnRowDeleting="CusDocRowDelete" Visible="false">
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:ImageButton ID="lnk_Add" runat="server" ToolTip="Add" ImageUrl="~/images/icon_newrequest.gif" CommandName="Add" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="lnk_Copy" ImageUrl="~/images/copy.png" runat="server" ToolTip="Copy" CommandName="Copy"  />
                                                    <asp:ImageButton ID="lnk_Move" runat="server" ToolTip="Move" CommandName="Move" ImageUrl="~/images/icon_move.jpg" />
                                                    
                                                    <asp:ImageButton ID="lnk_Delete" runat="server" ToolTip="Delete" ImageUrl="~/images/icon_delete.gif" OnClientClick="return confirm('Are you sure to delete?');" CommandName="Delete" />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Doc. Type">
                                                <ItemTemplate>
                                                <cc2:SmartDropDownList ID="ddl_DocType" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Doc. No.">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_DocNo" runat="server" Text='<%# Eval("DocumentNo") %>' />
                                                </ItemTemplate>                                        
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Country">
                                                <ItemTemplate>
                                                    <cc2:SmartDropDownList ID="ddl_DocCountry" runat="server" />
                                                </ItemTemplate>                                                                                                                            
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Issue Date">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_IssueDate" SkinID="DateTextBox" runat="server"  Text='<%# Eval("IssueDate","{0:d}") %>' onblur="formatDateString(this);" /><asp:ImageButton 
                                                        id="btn_cal_IssueDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex="9999" />
                                                    <cc1:CalendarExtender ID="ce_IssueDate" TargetControlID="txt_IssueDate" runat="server" Format="dd/MM/yyyy"
                                                        FirstDayOfWeek="Sunday" PopupButtonID="btn_cal_IssueDate"  />
                                                </ItemTemplate>
                                                    
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Expiry Date">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_ExpiryDate" SkinID="DateTextBox" runat="server" Text='<%# Eval("ExpiryDate","{0:d}") %>' onblur="formatDateString(this);" /><asp:ImageButton 
                                                        id="btn_cal_ExpiryDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex="9999" />
                                                    <cc1:CalendarExtender ID="ce_ExpiryDate" TargetControlID="txt_ExpiryDate" runat="server" Format="dd/MM/yyyy"
                                                        FirstDayOfWeek="Sunday" PopupButtonID="btn_cal_ExpiryDate" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Quota Cat.">
                                                <ItemTemplate>
                                                    <cc2:SmartDropDownList ID="ddl_QuotaCat" runat="server" />
                                                </ItemTemplate>                                        
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Weight (kg)">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_Weight" SkinID="SmallTextBox" Text='<%# Eval("Weight") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Qty on Doc.">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_QtyOnDoc"  SkinID="SmallTextBox" Text='<%# Eval("Qty") %>' runat="server" />
                                                    <cc2:SmartDropDownList ID="ddl_QtyOnDocUnit" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Qty for Order">
                                                <ItemTemplate>
                                                   <asp:TextBox ID="txt_QtyForOrder"  SkinID="SmallTextBox" Text='<%# Eval("OrderQty") %>' runat="server" />
                                                    <cc2:SmartDropDownList ID="ddl_QtyForOrderUnit" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Eqv. PO Qty">
                                                <ItemTemplate>
                                                   <asp:TextBox ID="txt_EqvPOQty"  SkinID="SmallTextBox" runat="server" Text='<%# Eval("POQty") %>' />
                                                    <cc2:SmartDropDownList ID="ddl_QtyOnPOUnit" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Doc Despatch Date to UK">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_DespatchDate" SkinID="DateTextBox" runat="server" Text='<%# Eval("DespatchToUKDate","{0:d}") %>' 
                                                    onblur="formatDateString(this);" /><asp:ImageButton ID="btn_cal_DespatchDate" runat="server" ImageAlign ="Middle" ImageUrl="~/images/calendar.gif" TabIndex="9999" />
                                                    <cc1:CalendarExtender ID="ce_DespatchDate" runat="server" TargetControlID="txt_DespatchDate" Format="dd/MM/yyyy"
                                                        FirstDayOfWeek="Sunday" PopupButtonID="btn_cal_DespatchDate" />                                            
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Airway Bill No.">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_AWBNo" SkinID="DateTextBox" runat="server" Text='<%# Eval("DespatchAWBNo") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            </Columns>
                                        <EmptyDataTemplate>
                                            No document imported.
                                            </EmptyDataTemplate>
                                            </asp:GridView>

                                        <br />
                                            <asp:Button ID="btn_NewDoc" runat="server" Text="New" OnClick="btn_NewDoc_Click"  Visible="false" />                         
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <!-- <td class="header2">UK Import Document</td> -->
                                    <td >
                                        <asp:Panel ID="Panel2" runat="server" skinid="sectionHeader2"  Height="22" >
                                            <div style="float:left;padding:5px;" ><img src="../images/pt3.gif" alt="" /></div>
                                            <div style="float:left;padding:3px;vertical-align:middle;" class="header2" >UK Import Document</div>
                                        </asp:Panel>
                                    </td>

                                </tr>
                                <tr>
                                    <td>                            
                                    <asp:GridView ID="gv_ILSDocInfo" runat="server" AutoGenerateColumns="False" OnRowDataBound="ILSDocDataBound" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Doc. Type">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_DocType" Text='<%# Eval("DocumentType") %>'   runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Doc. No.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_DocNo" Text='<%# Eval("DocumentNo") %>'   runat="server" />
                                                </ItemTemplate>                                        
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Country">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Country" Text='<%# Eval("DocumentCountry") %>'   runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Issue Date">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_IssueDate" Text='<%# Eval("StartDate","{0:d}") %>'   runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Expiry Date">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_ExpiryDate" Text='<%# Eval("ExpiryDate","{0:d}") %>'   runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Quota Cat.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_QuotaCat" Text='<%# Eval("QuotaCategory") %>'   runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Weight">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Weight" Text='<%# Eval("Weight") %>'   runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Pieces">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Pieces" Text='<%# Eval("Pieces") %>'   runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ILS-to-NSS Upload Date">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_UploadDate" Text='<%# Eval("ImportDate", "{0:d}") %>'   runat="server" />
                                                </ItemTemplate>                                        
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            No document uploaded.
                                        </EmptyDataTemplate>
                                        </asp:GridView>

                                    </td>
                                </tr>
                                
                            </table>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabBooking" runat="server" HeaderText="Booking" ScrollBars="None"  CssClass="ajax__tab_plain2" class="ajax__tab_revamp" SkinID="ajax__tab_revamp" >
                        <HeaderTemplate>
                            <asp:Image id="img_tbHeader_Booking" runat="server" ImageUrl="~/images/icon_sp.gif" Visible="false" />Booking
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table  cellpadding="5" cellspacing="0" id="tableBookingView" runat="server">
                            <tr>
                                <td>
                                </td>
                                <td style="vertical-align:top;">
                                    <table  cellpadding="5" cellspacing="0">
                                        <tr>
                                            <td>
                                                <table cellpadding="5" cellspacing="0">
                                                    <tr>
                                                        <!-- <td class="header2">Booking Information</td> -->
                                                        <asp:Panel runat="server" skinid="sectionHeader2"  Height="22" >
                                                            <div style="float:left;padding:3px;vertical-align:middle;" class="header2" >Booking Information</div>
                                                        </asp:Panel>
                                                    </tr>
                                                    <tr>
                                                        <td><asp:CustomValidator ID="val_BookingInfo" OnServerValidate="val_BookingInfo_Validate" Enabled="false" runat="server" ErrorMessage="" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="margin-left:10px;">
                                                   <table class="TableWithGridLines"  cellspacing="0" cellpadding="2">
                                                    <tr>
                                                        <td class="FieldLabel2">Lot Ship Order No.</td>
                                                        <td colspan="3"><asp:Label ID="lbl_LotShipOrderNo" runat="server" />
                                                            <asp:TextBox ID="txt_LotShipOrderNo" runat="server" Visible="false" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2">Booking Date</td>
                                                        <td style="width:100px;"><asp:Label ID="lbl_BookingDate" runat="server" />
                                                            <asp:TextBox ID="txt_BookingDate" runat="server" Visible="false" SkinID="DateTextBox" onblur="formatDateString(this);" /><asp:ImageButton 
                                                            id="btn_cal_BookingDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex ="9999" /> 
                                                            <cc1:CalendarExtender ID="ce_BookingDate" runat="server" FirstDayOfWeek="Sunday" 
                                                                Format="dd/MM/yyyy" TargetControlID="txt_BookingDate" PopupButtonID="btn_cal_BookingDate"  />
                                                        </td>
                                                        <td class="FieldLabel2">Booking Quantity</td>
                                                        <td style="width:100px;"><asp:Label ID="lbl_BookingQty" runat="server"  />
                                                            <asp:TextBox ID="txt_BookingQty" runat="server" Visible="false" />
                                                       </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2">Booked In-Warehouse Date</td>
                                                        <td colspan="3"><asp:Label ID="lbl_BkInWHDate" runat="server" />
                                                            <asp:TextBox ID="txt_BkInWHDate" runat="server" Visible="false" onblur="formatDateString(this);" SkinID="DateTextBox" /><asp:ImageButton 
                                                            id="btn_cal_BkInWHDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex="9999" />
                                                            <cc1:CalendarExtender ID="ce_BkInWHDate" runat="server" FirstDayOfWeek="Sunday" 
                                                                Format="dd/MM/yyyy" TargetControlID="txt_BkInWHDate" PopupButtonID="btn_cal_BkInWHDate" />

                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2">Actual In-Warehouse Date</td>
                                                        <td><asp:Label ID="lbl_ActualInWHDate" runat="server"  />
                                                            <asp:TextBox ID="txt_ActualInWHDate" runat="server" Visible="false" SkinID="DateTextBox" onblur="formatDateString(this);" /><asp:ImageButton 
                                                                id="btn_cal_ActualInWHDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex="9999" />
                                                            <cc1:CalendarExtender ID="ce_ActualInWHDate" runat="server" FirstDayOfWeek="Sunday" 
                                                                Format="dd/MM/yyyy" TargetControlID="txt_ActualInWHDate" PopupButtonID="btn_cal_ActualInWHDate" />
                                                        </td>
                                                        <td class="FieldLabel2">ILS In-Warehouse Date</td>
                                                        <td><asp:Label ID="ILSWHDate" runat="server"  />
                                                            <asp:ImageButton ID="btn_ILSWHDateInfo" runat="server" ImageUrl="~/images/help.bmp" OnClientClick="return false;" />
                                                           <div id="flyout6" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                                                        <div id="div_ILSWHDateIInfo" class="animationLayer" style="width:200px; height:50px; opacity: 0; filter: progid:DXImageTransform.Microsoft.Alpha(opacity=0); ">
                                                         <a onclick="div_ILSWHDateIInfo.style.visibility='hidden';" style="cursor:pointer;float:right ;" ><img src="../images/close.png" alt="" /></a>
                                                                    ILS in-warehouse date is equivalent to ILS arrived at partner date.
                                                            </div>
                                                <cc1:AnimationExtender id="ae_ILSWHDateInfo" runat="server" TargetControlID="btn_ILSWHDateInfo">
                                                    <Animations>                
                                                        <OnClick>
                                                            <Sequence>
                                                                <EnableAction Enabled="false" />
                                                                
                                                               <ScriptAction Script="Cover($get('tcContract_tabBooking_btn_ILSWHDateInfo'), $get('flyout6'));" />
                                                                <StyleAction AnimationTarget="flyout6" Attribute="display" Value="block"/>
                                                                
                                                                <Parallel AnimationTarget="flyout6" Duration=".3" Fps="25">
                                                                    <Move Horizontal="-10" Vertical="10" />
                                                                    <Resize Width="200" Height="50" />
                                                                    <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                                                                </Parallel>
                                                                
                                                                <ScriptAction Script="Cover($get('flyout6'), $get('div_ILSWHDateIInfo'), true);" />
                                                                <StyleAction AnimationTarget="div_ILSWHDateIInfo" Attribute="display" Value="block" />
                                                                <ScriptAction Script="div_ILSWHDateIInfo.style.visibility='visible';" />
                                                                <FadeIn AnimationTarget="div_ILSWHDateIInfo" Duration=".2"/>
                                                                <StyleAction AnimationTarget="flyout6" Attribute="display" Value="none"/>
                                                                
                                                                 <Parallel AnimationTarget="div_ILSWHDateIInfo" Duration=".5">
                                                                    <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                                                                    <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                                                                </Parallel>
                                                                <Parallel AnimationTarget="div_ILSWHDateIInfo" Duration=".5">
                                                                    <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                                                                    <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                                                                </Parallel>
                                                                <EnableAction Enabled="true" />
                                                            </Sequence>
                                                        </OnClick>
                                                    </Animations>
                                                </cc1:AnimationExtender>

                                                        </td>
                                                    </tr>
                                                </table>

                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table  style="border:1px solid #C0C0C0; height:50px;" width="100%">
                                                    <tr>
                                                        <!-- <td class="header2">Notes to QCC</td> -->
                                                        <asp:Panel runat="server" skinid="sectionHeader2"  Height="22" >
                                                            <div style="float:left;padding:3px;vertical-align:middle;" class="header2" >Notes to QCC</div>
                                                        </asp:Panel>

                                                    </tr>
                                                    <tr>
                                                        <td><asp:Label ID="lbl_NotesToQCC" runat="server"   />
                                                            <asp:TextBox ID="txt_NotesToQCC" SkinID="XLTextBox" runat="server" Visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            </table>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabOptions" runat="server" ScrollBars="None" ImageUrl="~/images/indexbtn2b.jpg" CssClass="LightBlue" >
                        <HeaderTemplate><asp:Image id="img_tbHeader_Options" runat="server" ImageUrl="~/images/icon_sp.gif" Visible="false" />Size Option</HeaderTemplate>
                        <ContentTemplate> 
                            <asp:CheckBox ID="ckb_IsUKDiscount" runat="server" Enabled="false" Checked="true" /><span style="font-size:8pt;">Is UK Discount</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <span id="div_UKDiscountReason" runat="server" style="font-size:8pt;">Reason&nbsp;&nbsp;<asp:TextBox ID="txt_UKDiscountReason" ReadOnly="true" SkinID="XLTextBox" CssClass="nonEditableTextbox"  runat="server" /></span>
                            <br />
                            <asp:CustomValidator ID="val_Options" OnServerValidate="val_Options_Validate" runat="server" Enabled="false" ErrorMessage="" />
                            <br />
        <%--                   <asp:UpdatePanel runat="server" ID="up_Options">
                           <ContentTemplate>
        --%>                        
                            <asp:GridView ID="gv_Options" runat="server" OnRowDataBound="OptionRowDataBound" AutoGenerateColumns="False"  ShowFooter="True">
                                <Columns>
                                    <asp:TemplateField HeaderText="Option No.">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_OptionNo" Text='<%# Eval("SizeOption.SizeOptionNo") %>' runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Width="45px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Colour" Visible ="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_Colour" Text='<%# Eval("Colour") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Size">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_SizeDesc" Width="50" Text='<%# Eval("SizeOption.SizeDescription") %>' runat="server" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lbl_TotalFooter" runat="server" Text="Total" Font-Bold="true"   />
                                        </FooterTemplate>
                                        <ItemStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Order Qty" >
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_OrderQty" runat="server" Text='<%# Eval("OrderQuantity", "{0:#,##0}") %>' Width="50" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lbl_TotalOrderQty" runat="server" />
                                        </FooterTemplate>
                                        <ItemStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PO Qty">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_POQty" runat="server" Text='<%# Eval("POQuantity","{0:#,##0}") %>'  Width="50" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lbl_TotalPOQty" runat="server" />
                                        </FooterTemplate>
                                        <ItemStyle Width="50px" />
                                    </asp:TemplateField>                            
                                    <asp:TemplateField HeaderText="Shipped Qty">
                                        <ItemTemplate>
                                            <asp:Label ID ="lbl_ShipQty" runat="server" Text='<%# Eval("ShippedQuantity","{0:#,##0}") %>' Width="50"   EnableTheming="False" ForeColor="Blue" Font-Size="8pt" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lbl_TotalShippedQty" runat="server" EnableTheming="false" ForeColor="Blue" Font-Size="8pt" Font-Bold="true" />
                                        </FooterTemplate>
                                        <ControlStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Shipped Qty">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_ShipQty" runat="server" Text='<%# Eval("ShippedQuantity") %>'  Width="50" />
                                        </ItemTemplate>
                                        <ControlStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField  HeaderText="Ratio Pack">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_RatioPack" runat="server" Text='<%# Eval("RatioPack") %>' />
                                         </ItemTemplate>
                                         <ItemStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Selling Price">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_NtoCSellPrice" runat="server" Text='<%# Eval("SellingPrice") %>' Width="50" />
                                        </ItemTemplate>
                                        <ItemStyle Width="50px" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Selling Amt">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_SellingAmt" runat="server" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lbl_SellAmtTotal" runat="server" />
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Right" />
                                        <FooterStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier Price">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_FtyPrice" runat="server" Text='<%# Eval("ReducedSupplierGmtPrice") %>' Width="50" />
                                        </ItemTemplate>
                                        <ItemStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier Amt">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_OptionFactoryAmt" runat="server" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lbl_FactoryAmtTotal" runat="server"  />
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Right" />
                                        <FooterStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier Price (CNY)">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_FtyPriceCNY" runat="server" Text='' Width="50" />
                                        </ItemTemplate>
                                        <ItemStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier Amt (CNY)">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_OptionFactoryAmtCNY" runat="server" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lbl_FactoryAmtTotalCNY" runat="server"  />
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Right" />
                                        <FooterStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Other Cost Amt">
                                    <HeaderTemplate>
                                           <asp:ImageButton ID="btn_OtherCostBreakDown" OnClientClick="return false;" Visible="false"  runat="server" ImageUrl="~/images/moreinfo.JPG" />&nbsp;Other Cost Amt
                                                   <div id="flyout4" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>        
                                                    <!-- Info panel to be displayed as a flyout when the button is clicked -->
                                                    <div id="div_OtherCostBreakDown" class="animationLayer" style="width:600px; height:250px;overflow-x:scroll;overflow-y:scroll;">
                                                        <div style="text-align:left;">
                                                        <table width="96%">
                                                            <tr>
                                                                <td><span class="header2" >Other Cost</span></td>
                                                                <td><a onclick="div_OtherCostBreakDown.style.visibility='hidden';" style="cursor:pointer;float:right ;" >
                                                                <img src="../images/close.png" alt="close" /></a>
                                                              </td>
                                                            </tr>
                                                        </table>
                                                        </div>
                                                        <asp:GridView ID="gv_OtherCostSummary" runat="server" AutoGenerateColumns="false">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Other Cost Type" ItemStyle-VerticalAlign ="Top" >
                                                                    <ItemTemplate >
                                                                        <asp:Label ID="lbl_OtherCostType" runat="server" Text='<%# Eval("OtherCostType") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="150px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Supplier" ItemStyle-VerticalAlign ="Top">
                                                                    <ItemTemplate >
                                                                        <asp:Label ID="lbl_OtherCostSupplier" runat="server" Text='<%# Eval("Supplier") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="150px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Detail" ItemStyle-VerticalAlign ="Top">
                                                                    <ItemTemplate>
                                                                        <asp:GridView ID="gv_OtherCostDetail" runat="server" AutoGenerateColumns="false" EnableTheming="false" CellPadding="3" BorderStyle="Solid"
                                                                            ShowFooter="true">
                                                                            <Columns >
                                                                                <asp:TemplateField HeaderText="Option">
                                                                                <ItemTemplate>
                                                                                    <asp:Label id="lbl_SizeOption" runat="server" Text='<%# Eval("Option") %>' />
                                                                                </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Colour" Visible="false" >
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lbl_Colour" runat="server" Text='<%# Eval("Colour") %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Qty">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lbl_Qty" runat="server" Text='<%# Eval("TotalShippedQty") %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Cost">
                                                                                    <ItemTemplate >
                                                                                        <asp:Label ID="lbl_Cost" runat="server" Text='<%# Eval("OtherCostAmt") %>' />
                                                                                    </ItemTemplate>
                                                                                    <FooterTemplate >
                                                                                        <b>Total :</b>
                                                                                    </FooterTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Total Cost">
                                                                                    <ItemTemplate >
                                                                                        <asp:Label ID="lbl_Amt" runat="server" Text='<%# Eval("OtherCostTotalAmt") %>' />
                                                                                    </ItemTemplate>
                                                                                    <FooterTemplate >
                                                                                        <asp:Label ID="lbl_TotalOtherCost" runat="server" />
                                                                                    </FooterTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                            <HeaderStyle ForeColor="Black" Font-Bold="true" Font-Size="8pt" />
                                                                            <RowStyle ForeColor="Black" Font-Bold="false" Font-Size="8pt" />
                                                                            <FooterStyle ForeColor="Black" Font-Bold="true" Font-Size="8pt" />
                                                                        </asp:GridView> 
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                
                                                            </Columns>
                                                            <HeaderStyle Font-Bold="true" Font-Size="8pt" />
                                                                <RowStyle Font-Size="8pt" />
                                                        </asp:GridView>                     
                                                                <br />
                                                        </div>

                                                 <cc1:AnimationExtender id="ae_OtherCostBreakDown" runat="server" TargetControlID="btn_OtherCostBreakDown">
                                                    <Animations>
                                                        <OnClick>
                                                            <Sequence>
                                                                <%-- Disable the button so it can't be clicked again --%>
                                                                <EnableAction Enabled="false" />
                                                                
                                                                <%-- Position the wire frame on top of the button and show it --%>
                                                                <ScriptAction Script="Cover($get('tcContract_tabOptions_gv_Options_ctl01_btn_OtherCostBreakDown'), $get('flyout4'));" />
                                                                <StyleAction AnimationTarget="flyout4" Attribute="display" Value="block"/>
                                                                
                                                                <%-- Move the wire frame from the button's bounds to the info panel's bounds --%>
                                                                <Parallel AnimationTarget="flyout4" Duration=".3" Fps="25">
                                                                    <Move Horizontal="-500" Vertical="-250" />
                                                                    <Resize Width="600" Height="250" />
                                                                    <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                                                                </Parallel>
                                                                
                                                                <%-- Move the info panel on top of the wire frame, fade it in, and hide the frame --%>
                                                                <ScriptAction Script="Cover($get('flyout4'), $get('div_OtherCostBreakDown'), true);" />
                                                                <StyleAction AnimationTarget="div_OtherCostBreakDown" Attribute="display" Value="block" />
                                                                <ScriptAction Script="div_OtherCostBreakDown.style.visibility='visible';" />
                                                                <FadeIn AnimationTarget="div_OtherCostBreakDown" Duration=".2"/>
                                                                <StyleAction AnimationTarget="flyout4" Attribute="display" Value="none"/>
                                                                
                                                                <%-- Flash the text/border red and fade in the "close" button --%>
                                                                <Parallel AnimationTarget="div_OtherCostBreakDown" Duration=".5">
                                                                    <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                                                                    <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                                                                </Parallel>
                                                                <Parallel AnimationTarget="div_OtherCostBreakDown" Duration=".5">
                                                                    <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                                                                    <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                                                                </Parallel>
                                                                <EnableAction Enabled="true" />
                                                            </Sequence>
                                                        </OnClick>
                                                    </Animations>
                                                </cc1:AnimationExtender>

                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_OptionOtherCostAmt" runat="server" Text ='<%# Eval("TotalShippedOtherCost") %>' Width="50" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lbl_OtherCostAmtTotal" runat="server"    />
                                        </FooterTemplate>
                                        <ControlStyle Width="50px" />
                                        <ItemStyle HorizontalAlign="Right" />
                                        <FooterStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="NSL-to-NUK<br> Price After Discount">                                    
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_NSL2NUKDiscount" Text='<%# Eval("ReducedSellingPrice") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Mer. GSP">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_MercGSP" runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Width="20" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Shipping GSP" >
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_ShipGSP" runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Width="20" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Shipping GSP">
                                        <ItemTemplate>
                                            <cc2:SmartDropDownList ID="ddl_ShippingGSP" TabIndex="100" runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Width="20" />
                                    </asp:TemplateField>--%>
                                    </Columns>
                                <EmptyDataTemplate>
                                    No option information.
                                    </EmptyDataTemplate>
                               </asp:GridView>
                               <asp:LinkButton ID="lnk_ILSPackingList" OnClientClick="return false;" runat="server" Text="[ ILS P/L ]" TabIndex="300" />
                            <asp:LinkButton ID="lnk_ILSInvoice" OnClientClick="return false;"  runat="server" Text="[ ILS Invoice ]" TabIndex="300" />

                            <div id="flyoutPackingList" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                            <div id="div_PackingList" class="animationLayer" style="width:550px; height:200px;overflow-y:scroll; ">
                            <span style="font-weight:bold; font-size: small; float:left ;">ILS Packing List</span>
                            <a onclick="div_PackingList.style.visibility='hidden';" style="cursor:pointer;" ><img src="../images/close.png" alt="" style="float:right;" /></a>
                                <table width="95%">
                                    <tr>
                                        <td><asp:GridView ID="gv_PackingList" OnRowDataBound="ILSPackingListRowDataBound"  runat="server" AutoGenerateColumns="false" ShowFooter="true">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Option No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_ILSOptionNo" runat="server" Text='<%# Eval("OptionNo") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Size">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_ILSSize" runat="server" Text='<%# Eval("optionDescription") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="NUK Shipped Qty" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_ILSQty" runat="server" Text='<%# Eval("Qty","{0:#,##0}") %>' />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lbl_NUKQtyTotal" runat="server" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            </asp:GridView></td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                    </tr>
                                </table>
                            </div>
                            <cc1:AnimationExtender id="AnimationExtender1" runat="server" TargetControlID="lnk_ILSPackingList">
                                <Animations>
                                    <OnClick>
                                        <Sequence>
                                            <%-- Disable the button so it can't be clicked again --%>
                                            <EnableAction Enabled="false" />
                                            
                                            <%-- Position the wire frame on top of the button and show it --%>
                                            <ScriptAction Script="Cover($get('tcContract_tabOptions_lnk_ILSInvoice'), $get('flyoutPackingList'));" />
                                            <StyleAction AnimationTarget="flyoutPackingList" Attribute="display" Value="block"/>
                                            
                                            <%-- Move the wire frame from the button's bounds to the info panel's bounds --%>
                                            <Parallel AnimationTarget="flyoutPackingList" Duration=".3" Fps="25">
                                                <Move Horizontal="20" Vertical="0" />
                                                <Resize Width="550" Height="200" />
                                                <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                                            </Parallel>
                                            
                                            <%-- Move the info panel on top of the wire frame, fade it in, and hide the frame --%>
                                            <ScriptAction Script="Cover($get('flyoutPackingList'), $get('div_PackingList'), true);" />
                                            <StyleAction AnimationTarget="div_PackingList" Attribute="display" Value="block"/>
                                            <ScriptAction Script="div_PackingList.style.visibility='visible';" />
                                            <FadeIn AnimationTarget="div_PackingList" Duration=".2"/>
                                            <StyleAction AnimationTarget="flyoutPackingList" Attribute="display" Value="none"/>
                                            
                                            <%-- Flash the text/border red and fade in the "close" button --%>
                                            <Parallel AnimationTarget="div_PackingList" Duration=".5">
                                                <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                                                <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                                            </Parallel>
                                            <Parallel AnimationTarget="div_PackingList" Duration=".5">
                                                <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                                                <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                                            </Parallel>
                                        </Sequence>
                                    </OnClick>
                                </Animations>
                            </cc1:AnimationExtender>

                            <div id="flyout3" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                            <!-- Info panel to be displayed as a flyout when the button is clicked -->
                            <div id="div_Option" class="animationLayer" style="width:550px; height:200px;overflow-y:scroll; ">
                            <span style="font-weight:bold ; font-size:small; float: left ;">ILS Invoice</span>
                            <a onclick="div_Option.style.visibility='hidden';" style="cursor:pointer;" ><img src="../images/close.png" alt="" style="float:right;" /></a>
                                <table width="95%">
                                    <tr>
                                        <td><asp:GridView ID="gv_ILSOption"  runat="server" OnRowDataBound="ILSOptionRowDataBound" AutoGenerateColumns="false" ShowFooter="true">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Option No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_ILSOptionNo" runat="server" Text='<%# Eval("OptionNo") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Size">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_ILSSize" runat="server"  />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="NUK Shipped Qty" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_ILSQty" runat="server" Text='<%# Eval("Qty","{0:#,##0}") %>' />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lbl_NUKQtyTotal" runat="server" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="NUK Selling Price">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_NUKSellPrice" runat="server" Text='<%# Eval("Price", "{0:#,##0.00}") %>' />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lbl_Total" runat="server" Text="Total" Font-Bold="true" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="NUK Sales Amount">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_NUKSalesAmt" runat="server" />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lbl_NUKSalesAmtTotal" runat="server" />
                                                    </FooterTemplate>
                                                    <ItemStyle HorizontalAlign="Right" />
                                                    <FooterStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                            </Columns>
                                            </asp:GridView></td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                    </tr>
                                </table>
                            </div>
                            <cc1:AnimationExtender id="ae_Option" runat="server" TargetControlID="lnk_ILSInvoice">
                                <Animations>
                                    <OnClick>
                                        <Sequence>
                                            <%-- Disable the button so it can't be clicked again --%>
                                            <EnableAction Enabled="false" />
                                            
                                            <%-- Position the wire frame on top of the button and show it --%>
                                            <ScriptAction Script="Cover($get('tcContract_tabOptions_lnk_ShowILSOption'), $get('flyout3'));" />
                                            <StyleAction AnimationTarget="flyout3" Attribute="display" Value="block"/>
                                            
                                            <%-- Move the wire frame from the button's bounds to the info panel's bounds --%>
                                            <Parallel AnimationTarget="flyout3" Duration=".3" Fps="25">
                                                <Move Horizontal="20" Vertical="0" />
                                                <Resize Width="550" Height="200" />
                                                <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                                            </Parallel>
                                            
                                            <%-- Move the info panel on top of the wire frame, fade it in, and hide the frame --%>
                                            <ScriptAction Script="Cover($get('flyout3'), $get('div_Option'), true);" />
                                            <StyleAction AnimationTarget="div_Option" Attribute="display" Value="block"/>
                                            <ScriptAction Script="div_Option.style.visibility='visible';" />
                                            <FadeIn AnimationTarget="div_Option" Duration=".2"/>
                                            <StyleAction AnimationTarget="flyout3" Attribute="display" Value="none"/>
                                            
                                            <%-- Flash the text/border red and fade in the "close" button --%>
                                            <Parallel AnimationTarget="div_Option" Duration=".5">
                                                <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                                                <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                                            </Parallel>
                                            <Parallel AnimationTarget="div_Option" Duration=".5">
                                                <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                                                <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                                            </Parallel>
                                        </Sequence>
                                    </OnClick>
                                </Animations>
                            </cc1:AnimationExtender>
                    <%--                    </ContentTemplate>                   
                           </asp:UpdatePanel>
        --%>                
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabInvoice" runat="server" ScrollBars="None">
                        <HeaderTemplate>
                            <asp:Image id="img_tbHeader_Invoice" runat="server" ImageUrl="~/images/icon_sp.gif" Visible="false" />Invoice
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table cellpadding="2" cellspacing="2" id="tableInvView" border="0" >
                                <tr>
                                    <td id="td_InvoiceBlock" width="460px">
                                    <table cellpadding="5" cellspacing="0" width="460px">
                                        <tr>
                                            <!-- <td class="header2">Invoice</td> -->
                                            <td>
                                            <asp:Panel ID="Panel3" runat="server" skinid="sectionHeader2"  Height="22" Width="460px">
                                                <div style="float:left;padding:3px;vertical-align:middle;" class="header2" >Invoice</div>
                                            </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><asp:CustomValidator ID="val_Invoice" OnServerValidate="val_Invoice_Validate" runat="server" Enabled="false" ErrorMessage=""></asp:CustomValidator></td>
                                        </tr>
                                    </table>
                                    <table class="TableWithGridLines" width="100%" cellspacing="0" cellpadding="2">
                                        <tr>
                                            <td  style="width:100px;" class="FieldLabel2">Invoice No.</td>
                                            <td style="width:100px;"><asp:Label ID="lbl_InvNo" runat="server" />
                                            </td>
                                            <td class="FieldLabel2">Invoice Date</td>
                                            <td style="width:100px;"><asp:Label ID="lbl_InvDate" runat="server" />
                                                <asp:TextBox ID="txt_InvDate" runat="server" SkinID="DateTextBox"  Visible="False" onblur="formatDateString(this);" /><asp:ImageButton 
                                                id="btn_cal_InvDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex="9999" />
                                                <cc1:CalendarExtender ID="ce_InvDate" FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" PopupButtonID="btn_cal_InvDate"
                                                    TargetControlID="txt_InvDate" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="FieldLabel2">Ship From</td>
                                            <td><asp:Label ID="lbl_ShipFrom" runat="server"  />
                                                <cc2:SmartDropDownList ID="ddl_ShipFrom" Visible="false" runat="server" />
                                            </td>
                                            <td class="FieldLabel2">Ship To</td>
                                            <td><asp:Label ID="lbl_ShipTo" runat="server"  />
                                                <cc2:SmartDropDownList ID="ddl_ShipTo" Visible="false" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="FieldLabel2_H2">
                                                <asp:Label runat="server" id="lbl_SupplierInvoiceNoCaption">Supplier Invoice No.</asp:Label>
                                                <asp:HyperLink ID="hpl_SupplierInvoiceNo" runat="server" Text="Supplier Invoice No."/>
                                            </td>
                                            <td colspan="3"><asp:Label ID="lbl_VendorInvoiceNo" runat="server"   />
                                                <asp:TextBox ID="txt_VendorInvoiceNo" runat="server" Visible="False" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="FieldLabel2">Shipping Remark</td>
                                            <td colspan="3"><asp:Label ID="lbl_ShipRemark" runat="server" />
                                                <asp:TextBox ID="txt_ShipRemark" SkinID="TextBox300" runat="server" Visible="False" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="FieldLabel2_H2">No. of Cartons/Hangers</td>
                                            <td colspan="3"><asp:Label ID="lbl_InvPiece" runat="server"    />
                                                <asp:TextBox ID="txt_InvPiece" runat="server" Visible="False" 
                                                    SkinID="SmallTextBox" /> &nbsp;/&nbsp;
                                                <asp:Label ID="lbl_InvPack" runat="server"   />
                                                <asp:DropDownList ID="ddl_InvPack" Visible="false" runat="server" >                                             
                                                    <asp:ListItem Text="HANGER" Value="1" />
                                                    <asp:ListItem Text="CARTON" Value ="2" />
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="FieldLabel2">Item Colour</td>
                                            <td colspan="3">
                                                <asp:Label ID="lbl_ItemColor" runat="server" />
                                                <asp:TextBox ID="txt_ItemColor" MaxLength="50" runat="server" Visible="false" SkinID="TextBox300" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="FieldLabel2_H3">Item Description</td>
                                            <td colspan="3" align='left'>
                                                <table border="0" cellspacing="0" cellpadding="0">
                                                  <tr>
                                                    <td style='border-style:none;'>
                                                            <asp:Label ID="lbl_Desc1" runat="server" />
                                                            <asp:TextBox id="txt_Desc1" runat="server" MaxLength="100" Visible="false" SkinID="TextBox300" /><br />
                                                            <asp:Label ID="lbl_Desc2" runat="server" />
                                                            <asp:TextBox id="txt_Desc2" runat="server" MaxLength="100" Visible="false" SkinID="TextBox300" /><br />
                                                            <asp:Label ID="lbl_Desc3" runat="server" />
                                                            <asp:TextBox id="txt_Desc3" runat="server" MaxLength="100" Visible="false" SkinID="TextBox300" /><br />
                                                            <asp:Label ID="lbl_Desc4" runat="server" />
                                                            <asp:TextBox id="txt_Desc4" runat="server" MaxLength="100" Visible="false" SkinID="TextBox300" /><br />
                                                            <asp:Label ID="lbl_Desc5" runat="server" />
                                                            <asp:TextBox ID="txt_Desc5" runat ="server" MaxLength="100" Visible="false" SkinID="TextBox300" />
                                                    </td>
                                                    <td style='border-style:none;' valign='bottom'>
                                                        <asp:panel id='pnl_ItemMasterIcon' runat='server' style="float:left; display:none;">
                                                            <table>
                                                                <tr>
                                                                    <td style='border-style:none' >
                                                                        <asp:ImageButton ID="btn_copyItemDescFromMaster" runat="server" ToolTip="Copy Item Description From Master Copy" ImageUrl="../images/icon_copy_16.jpg" ImageAlign="Middle" 
                                                                            onMouseOver="div_MasterDesc.style.display = 'block'; div_MasterDesc.style.top = event.clientY + document.documentElement.scrollTop+10;
                                                                            div_MasterDesc.style.left = event.clientX + document.documentElement.scrollLeft +10;" 
                                                                            onMouseOut = "div_MasterDesc.style.display = 'none';"
                                                                            OnClientClick='updateItemDescFromMaster(this,"btn_copyItemDescFromMaster");return false;' />
                                                                    </td>
                                                                    <td style='border-style:none;'>
                                                                        <asp:ImageButton ID="btn_setItemDescriptionAsMaster" runat='server'  ToolTip="Set Item Description As Master" ImageUrl="../images/icon_Paste_16.jpg" ImageAlign="Middle" 
                                                                            OnClientClick='setItemDescAsMaster(this,"btn_setItemDescriptionAsMaster");return false;' 
                                                                            OnClick='btn_setItemDescriptionAsMaster_click' PostBackUrl='' />  
                                                                        <asp:CheckBox ID='ckb_setMaster' runat='server' Checked='false' style='display:none;'/>                                        
                                                                    </td>
                                                                         
                                                                </tr>
                                                            </table>
                                                            <div id="div_MasterDesc" style="position:absolute;  top:10px;left:100px; background-color : #FFFFCC; border : solid 1px #CCCCCC;display:none;" >
                                                                <table style='border-style:none;'>
                                                                    <tr><td style='border-style:none'><asp:Label ID='lbl_MasterItemDesc1' runat='server'  Text='' BorderStyle='None' /></td></tr>
                                                                    <tr><td style='border-style:none'><asp:Label ID='lbl_MasterItemDesc2' runat='server'  Text='' BorderStyle='None' /></td></tr>
                                                                    <tr><td style='border-style:none'><asp:Label ID='lbl_MasterItemDesc3' runat='server'  Text='' BorderStyle='None' /></td></tr>
                                                                    <tr><td style='border-style:none'><asp:Label ID='lbl_MasterItemDesc4' runat='server'  Text='' BorderStyle='None' /></td></tr>
                                                                    <tr><td style='border-style:none'><asp:Label ID='lbl_MasterItemDesc5' runat='server'  Text='' BorderStyle='None' /></td></tr>
                                                                </table>
                                                            </div>
                                                        </asp:panel>
                                                    </td>
                                                  </tr>
                                                </table>    
                                            </td>
                                        </tr>
                                        <tr>                                            
                                            <td class="FieldLabel2" runat="server">Retail Description</td>
                                            <td colspan="3"><asp:Label ID="lbl_RetailDesc" runat="server" /><asp:TextBox ID="txt_RetailDesc" runat ="server" MaxLength="100" Visible="False" SkinID="TextBox300" /></td>
                                        </tr>
                                        <tr id='tr_UploadDoc' runat="server">
                                            <td class="FieldLabel2">Uploaded Doc.</td>
                                            <td colspan="3">
                                                <asp:ImageButton ID="img_UploadedDoc" runat='server' ImageUrl="../images/icon_edit.gif"  ImageAlign="Middle" />  
                                            </td>
                                        </tr>
                                    </table>
                                    </td>
                                    <td id="td_AmountBlock" style="vertical-align:top;" runat="server">
                                    <table cellpadding="5" cellspacing="0">
                                        <tr>
                                            <!-- <td class="header2">Amount</td> -->
                                            <td>
                                            <asp:Panel ID="Panel4" runat="server" skinid="sectionHeader2"  Height="22" width="250">
                                                <div style="float:left;padding:3px;vertical-align:middle;" class="header2" >Amount</div>
                                            </asp:Panel>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td><asp:CustomValidator ID="val_Amount" OnServerValidate="val_Amount_Validate" runat="server" Enabled="false" ErrorMessage="" /></td>
                                        </tr>
                                    </table>
                                    <table class="TableWithGridLines"  cellspacing="0" cellpadding="2">
                                        <tr>
                                            <td colspan="2" class="FieldLabel2">Export Licence Fee</td>
                                            <td><asp:Label ID="lbl_ExLicenceFee" runat="server" />
                                                <asp:TextBox ID="txt_ExLicenceFee" SkinID="DateTextBox"  runat="server" Visible="False" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="FieldLabel2" >Quota Charge</td>
                                            <td><asp:Label ID="lbl_QuotaCharge" runat="server"  />
                                                <asp:TextBox ID="txt_QuotaCharge" SkinID="DateTextBox" runat="server" Visible="False" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="FieldLabel2">Total Duty</td>
                                            <td><asp:Label ID="lbl_TotalDuty" runat="server"  />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="FieldLabel2" style="border-right-width : 0;">Total Amount</td>
                                            <td class="FieldLabel2" style="border-left-width : 0; text-align : right ;"><asp:Label ID="lbl_SellCurrency1" runat="server"  /></td>
                                            <td><asp:Label ID="lbl_TotalAmount" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="FieldLabel2" style="border-right-width:0;">
                                            <asp:label ID="lbl_InvCommTitle" runat="server"  Text="Customer-to-NSL Commission" />
                                            </td>
                                            <td class="FieldLabel2" style="border-left-width:0; text-align: right ;"><asp:Label ID="lbl_SellCurrency2" runat="server"  /></td>
                                            <td><asp:Label ID="lbl_InvCommPercent" runat="server"  />%,&nbsp;<asp:Label ID="lbl_InvCommAmt" runat="server"    />
                                            </td>
                                        </tr>
                                        <tr id="row_QAComm" runat="server" >
                                            <td class="FieldLabel2" style="border-right-width : 0;"><asp:Label ID="lbl_SplitShipmentQAComm" runat="server" Text="Split Shipment " Visible="false" />QA Commission</td>
                                            <td class="FieldLabel2" style="border-left-width: 0; text-align : right ;"><asp:Label ID="lbl_BuyCurrency" runat="server" /></td>
                                            <td>
                                                <asp:Label ID="lbl_QACommPercent" runat="server"  />
                                                <asp:Label ID="lbl_QACommAmt" runat="server"  />                                        
                                            </td>
                                        </tr>
                                        <tr id="row_TRDiscount" runat="server">
                                            <td class="FieldLabel2" style="border-right-width: 0;">Vendor Payment Discount</td>
                                            <td class="FieldLabel2" style="border-left-width:0; text-align : right ;"><asp:Label ID="lbl_BuyCurrency2" runat="server" /></td>
                                            <td><asp:Label ID="lbl_DiscountPercent" runat="server"  />
                                                <asp:Label ID="lbl_DiscountAmt" runat="server"  /></td>
                                        </tr>
                                        <tr id="row_LabTest" runat="server">
                                            <td class="FieldLabel2" style="border-right-width: 0;"><asp:Label ID="lbl_SplitShipmentLabTest" runat="server" Text="Split Shipment " Visible="false" />Lab Test Income</td>
                                            <td class="FieldLabel2" style="border-left-width:0; text-align : right ;"><asp:Label ID="lbl_BuyCurrency5" runat="server" /></td>
                                            <td>
                                                <asp:Label ID="lbl_LabTestIncome" runat="server"  />
                                                <asp:Label ID="lbl_LabTestIncomeAmt" runat="server"  />
                                            </td>
                                         </tr>
                                        <!---
                                        <tr id="Tr1" runat="server">
                                            <td class="FieldLabel" style="border-right-width: 0;">Additional Bank Charges</td>
                                            <td class="FieldLabel" style="border-left-width:0; text-align : right ;"><asp:Label ID="lbl_BuyCurrency4" runat="server" /></td>
                                            <td><asp:Label ID="lbl_BankChargesPercent" runat="server"  />
                                                <asp:Label ID="lbl_BankChargesAmt" runat="server"  /></td>
                                        </tr>
                                        --->
                                        <tr id="row_PaymentDeduction" runat="server">
                                            <td class="FieldLabel2" style="border-right-width: 0;">Payment Deduction</td>
                                            <td class="FieldLabel2" style="border-left-width: 0; text-align : right ;"><asp:Label ID="lbl_BuyCurrency6" runat ="server" /></td>
                                            <td><asp:Label ID="lbl_PaymentDeduction" runat="server" /></td>
                                        </tr>
                                        <tr id="row_NetAmountToSupplier" runat="server">
                                            <td class="FieldLabel2" style="border-right-width: 0;">Net Amount to Supplier</td>
                                            <td class="FieldLabel2" style="border-left-width: 0; text-align : right ;"><asp:Label ID="lbl_BuyCurrency3" runat ="server" /></td>
                                            <td><asp:Label ID="lbl_NetAmt" runat="server" /></td>
                                        </tr>
                                        <tr id="row_ShipDocCheck" runat="server">
                                            <td class="FieldLabel2" colspan="2" style="width:110px;">Shipping Doc. Check Amount</td>
                                            <td style="width:110px;"><asp:Label ID="Label1" runat="server" />
                                                <asp:CheckBox ID="ckb_ShipDocCheck" runat="server" OnCheckedChanged="ckb_ShipDocCheck_CheckedChanged"/>
                                                <asp:Label ID="lbl_ShipDocCheckAmount" runat="server" style="display:none;" />
                                                <asp:TextBox ID="txt_ShipDocCheckAmount" runat="server" style="border:none;width:60px; color:Black;"  ReadOnly="true" />
                                            </td>
                                        </tr>
                                    </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td id="td_PaymentInfoBlock" >
                                        <table>
                                          <tr>
                                            <td>                              
                                                <table cellpadding="5" cellspacing="0">
                                                    <tr>
                                                        <!-- <td class="header2">Payment Information</td> -->
                                                        <asp:Panel ID="Panel5" runat="server" skinid="sectionHeader2"  Height="22" Width="460px">
                                                            <div style="float:left;padding:3px;vertical-align:middle;" class="header2" >Payment Information</div>
                                                        </asp:Panel>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CustomValidator ID="val_PaymentInfo" OnServerValidate="val_PaymentInfo_Validate" runat="server" Enabled="false" ErrorMessage="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table cellspacing="0" cellpadding="2" class="TableWithGridLines">
                                                    <tr>
                                                        <td class="FieldLabel2_H2" style="width:110px;">Shipping Doc. Receipt Date</td>
                                                        <td style="width:110px;"><asp:Label ID="lbl_ShipRecDocDate" runat="server" />
                                                            <asp:TextBox ID="txt_ShipRecDocDate" SkinID="DateTextBox"  runat="server" Visible="false" onblur="formatDateString(this);" />
                                                            <asp:CheckBox ID="ckb_ShipRecDocDate" runat="server" />
                                                        </td>
                                                        <td class="FieldLabel2_H2"style="width:110px;" >Account Doc. <br /> Receipt Date</td>
                                                        <td style="width:100px;"><asp:Label ID="lbl_ACRecDocDate" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2_H2">L/C Batch No.</td>
                                                        <td><asp:Label ID="lbl_LCBatchNo" runat="server" /></td>                                    
                                                        <td class="FieldLabel2_H2">L/C Batch Submission Date</td>
                                                        <td><asp:Label ID="lbl_AppDate" runat="server"  /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2">L/C No.</td>
                                                        <td><asp:Label ID="lbl_LCNo" runat="server" />
                                                            <asp:TextBox ID="txt_LCNo" runat="server" Visible="false" SkinID="DateTextBox" MaxLength="15" style="width:90px;"/>
                                                        </td>
                                                        <td class="FieldLabel2">L/C Issue Date</td>
                                                        <td><asp:Label ID="lbl_IssueDate" runat="server"  />        
                                                            <asp:TextBox ID="txt_IssueDate" runat="server" Visible="false" SkinID="DateTextBox" onblur="formatDateString(this);" /><asp:ImageButton 
                                                            id="btn_cal_LCIssueDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" TabIndex="9999" />                      
                                                            <cc1:CalendarExtender ID="ce_IssueDate" runat="server" FirstDayOfWeek="Sunday" Format="dd/MM/yyyy"
                                                                TargetControlID="txt_IssueDate" PopupButtonID ="btn_cal_LCIssueDate" />
                                                        </td>                                    
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2_H2">L/C Payment Checked</td>
                                                        <td><asp:CheckBox ID="ckb_PaymentChecked" Enabled="False" runat="server" />&nbsp;
                                                            <asp:Label ID="lbl_PaymentCheckDate" runat="server" />
                                                            <asp:TextBox ID="txt_PaymentCheckDate" runat="server" SkinId="DateTextBox"  Visible="False" onblur="formatDateString(this);" />
                                                        </td>                
                                                        <td class="FieldLabel2_H2">L/C Expiry Date & Amount</td>                                                    
                                                        <td>
                                                            <asp:Label ID="lbl_LCExpiryDate" runat="server" />&nbsp;&nbsp;<asp:Label ID="lbl_LCAmount" runat="server" />
                                                            <asp:TextBox ID="txt_LCExpiryDate" runat="server" Visible="false" SkinID="DateTextBox" onblur="formatDateString(this);" /><asp:ImageButton
                                                            id="btn_cal_LCExpiryDate" runat="server" ImageAlign="Middle" Visible="false"  ImageUrl="~/images/calendar.gif" TabIndex="9999" />
                                                            <cc1:CalendarExtender ID="ce_LCExpiryDate" runat="server" FirstDayOfWeek="Sunday" Format="dd/MM/yyyy"
                                                            TargetControlID="txt_LCExpiryDate" PopupButtonID="btn_cal_LCExpiryDate" /><asp:TextBox 
                                                            ID="txt_LCAmount" runat="server" SkinID="SmallTextBox" visible="false"  />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2">L/C Application No.</td>
                                                        <td><asp:Label ID="lbl_LCAppNo" runat="server" /></td>
                                                        <td class="FieldLabel2">L/C Bill Ref. No.</td>
                                                        <td><asp:Label ID="lbl_LCBillRefNo" runat="server" />
                                                            <asp:TextBox ID="txt_LCBillRefNo" runat="server" Visible="false" SkinID="MTextBox" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2">Contract's Delivery L/C Status</td>
                                                        <td>
                                                            <asp:ImageButton ID="img_LcOtherDelivery" ToolTip="L/C status of other delivery" ImageUrl="../images/icon_LC.gif" Visible="false" runat="server" ImageAlign="Middle" 
                                                                 onClientClick="setPanel('ON',this);return false;" />
                                                            <asp:Panel id="div_LCInfoOfOtherDly" runat="server" 
                                                                        style="position:absolute; overflow:auto; z-index: 2; background-color: #FFFFCC; border:inset 1px #404040;  display:none;">
                                                                    <label class="header2"  style="float:left">&nbsp;Contract Deliveries' L/C Status</label>
                                                                    <a onclick="setPanel('OFF')" style="cursor:pointer;" ><img src="../images/close.png" alt="" style="float:right;" /></a>
                                                                    
                                                                    <table width="98%">
                                                                        <tr>
                                                                        <td style="border:none;">&nbsp;</td>
                                                                            <td style="border:none;">
                                                                                <asp:GridView ID="gv_LCInfoOfOtherDly" runat="server" BorderColor="Silver" CssClass="TableWithGridLines"
                                                                                    OnRowDataBound="LCInfoOfOtherDlyRowDataBound"  AutoGenerateColumns="false">
                                                                                    <Columns>
                                                                                        <asp:TemplateField HeaderText="Dly<br>No.">
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lbl_DlyNo" runat="server" Text='<%# Eval("shipment.DeliveryNo") %>' />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="PO Qty">
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lbl_PoQty" runat="server" Text='<%# Eval("shipment.TotalPOQuantity","{0:#,##0}") %>'  />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="L/C<br>Application No.">
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lbl_LcApplicationNo" runat="server" Text='<%# Eval("lcApplication.LcApplicationNo") %>' />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="L/C<br>Batch No.">
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lbl_LcBatchNo" runat="server" Text='' />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="Original<br>L/C Batch<br>PO Qty">
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate >
                                                                                                <asp:Label ID="lbl_LcPoQty" runat="server" Text='<%# Eval("lcApplication.TotalPOQuantity","{0:#,##0}") %>'  />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="L/C No.">
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lbl_LcNo" runat="server" Text='<%# Eval("invoice.LCNo") %>'/>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="90" />
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="L/C Issued<br>Date" >
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate >
                                                                                                <asp:Label ID="lbl_LcIssuedDate" runat="server" Text='' />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="L/C Expiry<br>Date" >
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate >
                                                                                                <asp:Label ID="lbl_LcExpiryDate" runat="server" Text=''  BorderColor="Silver"/>
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="L/C Amount">
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lbl_LcAmount" runat="server" Text='' />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                         <asp:TemplateField HeaderText="PO Status">
                                                                                        <HeaderStyle BorderColor="Silver" />
                                                                                            <ItemTemplate>
                                                                                                <asp:Label ID="lbl_PoStatus" runat="server" Text='' />
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                   </Columns>
                                                                                </asp:GridView>
                                                                            </td>
                                                                         </tr>
                                                                    </table>
                                                                </asp:Panel>
                                                        </td>
                                                        <td class="FieldLabel2">L/C Cancellation Date</td>
                                                        <td><asp:Label ID="lbl_LcCancelDate" runat="server"  style="color:Red; font-weight:bold;" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2">With Payment Deduction on L/C</td>
                                                        <td><asp:Label ID="lbl_DeductionOnLC" runat="server" /></td>
                                                        <td class="FieldLabel2">Payment Deduction Amount in L/C</td>
                                                        <td><asp:Label ID="lbl_DeductionAmtInLC" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="FieldLabel2">LG Due Date</td>
                                                        <td><asp:Label ID="lbl_LGDueDate" runat="server" /></td>
                                                    </tr>
                                                    <tr id="row_EzibuyInvSent" runat="server" Visible="false" >
                                                        <td class="FieldLabel2">Invoice Sent Date</td>
                                                        <td><asp:Label ID="lbl_InvSentDate" runat="server"  /></td>
                                                    </tr>
                                                    <tr id="tr_ChoicePaymentSeperator" runat="server" visible="false">
                                                        <td style="border:none;"></td>
                                                    </tr>
                                                    <tr id="tr_ChoicePaymentRow" runat="server" visible="false">
                                                        <td class="FieldLabel2_H2">Choice Order Actual Sales Amount</td>
                                                        <td>
                                                            <asp:TextBox ID="txt_ChoiceActSalesAmt" runat="server" SkinId="DateTextBox" Visible="False" />&nbsp;
                                                            <asp:Label ID="lbl_ChoiceActSalesAmt" runat="server" />
                                                        </td>                                    
                                                        <td class="FieldLabel2_H2">Choice Order Actual<br /> Purchase Amount</td>
                                                        <td>
                                                            <asp:TextBox ID="txt_ChoiceActPurchaseAmt" runat="server" SkinId="DateTextBox" Visible="False" />&nbsp;
                                                            <asp:Label ID="lbl_ChoiceActPurchaseAmt" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr id="tr_choiceNslCommRow" runat="server" Visible="false">
                                                        <td class="FieldLabel2_H2">Choice to NSL Commission</td>
                                                        <td> 
                                                            &nbsp;
                                                            <asp:Label ID="lbl_ChoiceNslCommPercent" runat="server" />%,
                                                            &nbsp;
                                                            <asp:Label ID="lbl_ChoiceNslCommAmt" runat="server" />
                                                        </td> 
                                                    </tr>
                                                </table>
                                            </td>
                                          </tr>
                                        </table>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="2"><asp:CustomValidator ID="val_CourierCharge" OnServerValidate="val_CourierCharge_Validate" runat="server" Enabled="false" ErrorMessage="" /></td>
                                </tr>
                                <tr id="row_mockShop" runat="server" visible ="false"> 
                                    <td colspan="2">
                                        <table cellpadding="5" cellspacing="0" width="100%">
                                            <tr>
                                                <td class="header2">Mock Shop / Press Sample Courier Charges</td>
                                            </tr>
                                        </table>
                                        <table class="TableWithGridLines"  cellspacing="0" cellpadding="2">
                                            <tr>
                                                <td class="FieldLabel2_H2" style="width:100px;">Courier Charge</td>
                                                <td style="width:100px;"><asp:Label ID="lbl_CourierCharge" runat="server" 
                                                        Visible="False" /><asp:TextBox ID="txt_CourierCharge" runat="server" Visible="false" />
                                                </td>
                                                <td class="FieldLabel2_H2" style="width:100px;">NSL-to-NUK Debit Note No.</td>                                        
                                                <td style="width:100px;"><asp:Label ID="lbl_DebitNoteNo" runat="server" />
                                                    <asp:TextBox ID="txt_DebitNoteNo" runat="server" Visible="false" />
                                                </td>                                        
                                            </tr>
                                        </table>

                                    </td>
                                </tr>
                                <tr >
                                    <td>&nbsp;</td>
                                </tr>
                                <tr runat="server" id="row_DirectFranchise" visible="false">
                                    <td id="Td2" colspan="2" runat="server">
                                        <table cellpadding="5" cellspacing="0" width="100%">
                                            <tr>
                                                <td class="header2">Direct Franchise</td>
                                            </tr>
                                            <tr>
                                                <td><asp:CustomValidator ID="val_DF" OnServerValidate="val_DF_Validate" runat="server" Enabled="false" ErrorMessage="" /></td>
                                            </tr>
                                        </table>
                                        <table class="TableWithGridLines"  cellspacing="0" cellpadding="2">
                                            <tr>
                                                <td class="FieldLabel2_H2" style="width:100px;">Debit Note No.</td>
                                                <td style="width:100px;"><asp:Label ID="lbl_DFDebitNoteNo" runat="server" />
                                                    <asp:TextBox ID="txt_DFDebitNoteNo" runat="server" Visible="False" />
                                                </td>
                                                <td class="FieldLabel2_H2" style="width:100px;">Documentation Charge</td>
                                                <td style="width:100px;"><asp:Label ID="lbl_DocCharge" runat="server" />
                                                    <asp:TextBox ID="txt_DocCharge" runat="server" Visible="False" />
                                                </td>
                                                <td class="FieldLabel2_H2" style="width:100px;">Transportation Charge</td>
                                                <td style="width:100px;"><asp:Label ID="lbl_TransportCharge" runat="server" />
                                                    <asp:TextBox ID="txt_TransportCharge" runat="server" Visible="False" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr >
                                    <td>&nbsp;</td>
                                </tr>
                                <tr id="row_uturn" runat="server" visible="false">
                                    <td colspan="2">
                                        <table cellpadding="5" cellspacing="0" width="100%">
                                            <tr>
                                                <td class="header2" style="text-align:left ;">U-Turn Order / NEXT-HEMPEL Order</td>                                        
                                            </tr>
                                            <tr>
                                                <td><asp:CustomValidator ID="val_UTurn" OnServerValidate="val_UTurn_Validate" runat="server" Enabled="false" ErrorMessage="" /></td>
                                            </tr>
                                        </table>
                                        <table class="TableWithGridLines"  cellspacing="0" cellpadding="2" style="text-align:center;" >
                                            <tr>
                                                <td></td>
                                                <td class="FieldLabel">Actual Amount</td>
                                                <td class="FieldLabel">Record Checked</td>
                                                <td class="FieldLabel" colspan="2">Calculated Amount</td>
                                            </tr>
                                            <tr>
                                                <td class="FieldLabel" style="width:100px;">Import Duty</td>
                                                <td><asp:Label ID="lbl_ImportDutyActualAmt" runat="server" />
                                                    <asp:TextBox ID="txt_ImportDutyActualAmt" runat="server" Visible="False" />
                                                </td>
                                                <td><asp:CheckBox ID="ckb_ImportDutyRecordChecked" runat="server" 
                                                        Enabled="False" />
                                                    <asp:Label ID="lbl_ImportDutyRecordCheckedDate" runat="server" />
                                                    <asp:TextBox ID="txt_ImportDutyRecordCheckedDate" SkinID="DateTextBox" 
                                                        runat="server" Visible="False" />                                           
                                                </td>
                                                <td><asp:Label ID="lbl_ImportDutyCurrency" runat="server" /></td>
                                                <td><asp:Label ID="lbl_ImportDutyCalcAmt" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="FieldLabel">Input VAT</td>
                                                <td><asp:Label ID="lbl_InputVATActualAmt" runat="server" />
                                                    <asp:TextBox ID="txt_InputVATActualAmt" runat="server" Visible="False" />
                                                </td>
                                                <td><asp:CheckBox ID="ckb_InputVATRecordChecked" runat="server" 
                                                        Enabled="False" />
                                                    <asp:Label ID="lbl_InputVATRecordCheckedDate" runat="server"  />
                                                    <asp:TextBox ID="txt_InputVATRecordCheckedDate" SkinID="DateTextBox" 
                                                        runat="server" Visible="False" />                                            
                                                </td>
                                                <td><asp:Label ID="lbl_InputVATCurrency" runat="server" /></td>                                          
                                                <td><asp:Label ID="lbl_InputVATCalcAmt" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td class="FieldLabel">Output VAT</td>
                                                <td><asp:Label ID="lbl_OutputVATActualAmt" runat="server" />
                                                    <asp:TextBox ID="txt_OutputVATActualAmt" runat="server" Visible="False" />
                                                </td>
                                                <td><asp:CheckBox ID="ckb_OutputVATRecordChecked" runat="server" 
                                                        Enabled="False" />
                                                    <asp:Label ID="lbl_OutputVATRecordCheckedDate" runat="server"  />
                                                    <asp:TextBox ID="txt_OutputVATRecordCheckedDate" SkinID="DateTextBox" 
                                                        runat="server" Visible="False" />                                            
                                                </td>
                                                <td style="border-right-style:none; border-right-width:0px; border-right-color:White;">
                                                <asp:Label ID="lbl_OutputVATCurrency" runat="server" /></td>
                                                <td><asp:Label ID="lbl_OutputVATCalcAmt" runat="server"  /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabInvRemark" runat="server" ScrollBars="None">
                        <HeaderTemplate>Invoice Remark</HeaderTemplate>
                        <ContentTemplate>
                        <table >
                            <tr id="tr_EditInvRmk" runat="server" visible="false">
                                <td>
                                    <span style="font-weight:bold;">Invoice Remark Template</span>
                                    <asp:DropDownList id="ddl_InvTemplate" runat="server">
                                        <asp:ListItem Text="Spare Parts" Value="spareparts" />
                                        <asp:ListItem Text="Dangerous Goods" Value="dangerousgoods" />
                                        <asp:ListItem Text="Jewelry" Value="jewelry" />
                                        <asp:ListItem Text="Upholstery" Value="upholstery" />
                                    </asp:DropDownList>
                                    <asp:Button ID="btn_SelectTemplate" runat="server" Text="Submit" 
                                    onclick="btn_Submit_Click" OnClientClick="if (document.getElementById('tcContract_tabInvRemark_ddl_InvTemplate').value =='spareparts') window.showModalDialog('InvoiceRemarkGenerator.aspx','','status: Yes;dialogWidth:700px;dialogHeight: 500px;scrollbars:Yes;resizable:Yes;')" />
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txt_InvRemark" ReadOnly="true"  runat="server" TextMode="MultiLine" SkinID="XLInvoiceTextBox" Rows="10" Width="800px"></asp:TextBox> 
                                </td>
                            </tr>
                        
                        </table>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabAccount" runat="server" ScrollBars="None">
                        <HeaderTemplate>Account</HeaderTemplate>
                        <ContentTemplate>
                            <table cellpadding="2" cellspacing="0" class="TableWithGridLines" style="text-align:center;">
                            <tr>
                                <td rowspan="2">
                                      <asp:ImageButton ID="btn_ReversingEntry" OnClientClick="return false;" runat="server" ImageUrl="~/images/moreinfo.JPG" ToolTip="Show reversing entries" />&nbsp;<asp:Label ID="lbl_ReversingEntry"
                                        runat="server" Text="Reversing Entry" />
                                      <div id="flyout5" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                
                                    <!-- Info panel to be displayed as a flyout when the button is clicked -->

                                    <div id="div_ReversingEntry" class="animationLayer" style="width:400px; height:150px; ">
                                        <span class="header2" style="float:left ;">Reversing Entry Log</span><a onclick="div_ReversingEntry.style.visibility='hidden';" style="cursor:pointer;float:right ;" ><img src="../images/close.png" alt="" style="margin-bottom:5px;" /></a>
                                        <div style="width:100%; overflow-y:scroll;height:80%;">
                                            <asp:GridView ID="gv_ReversingEntry" runat="server" AutoGenerateColumns="false" OnRowDataBound="gv_ReversingEntry_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-Width="200px">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_type" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Scanned Amount" ItemStyle-Width="100px">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_Amount" runat="server" Text='<%# Eval("FullOtherAmount") %>' />
                                                        </ItemTemplate>                                
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Epicor Interfaced Date" ItemStyle-Width="100px">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_InterfacedDate" runat="server" Text='' /> <!--Text='<%# Eval("CreatedOn","{0:d}") %>' />-->
                                                        </ItemTemplate>                                
                                                    </asp:TemplateField>                            
                                                </Columns>
                                                <RowStyle Font-Bold="false" />
                                            </asp:GridView>                    
                                        </div>
                                    </div>

                                    <cc1:AnimationExtender id="ae_ReversingEntry" runat="server" TargetControlID="btn_ReversingEntry">
                                        <Animations>
                                            <OnClick>
                                                <Sequence>
                                                    <%-- Disable the button so it can't be clicked again --%>
                                                    <EnableAction Enabled="false" />
                                                                
                                                    <%-- Position the wire frame on top of the button and show it --%>
                                                    <ScriptAction Script="Cover($get('tcContract_tabAccount_btn_ReversingEntry'), $get('flyout5'));" />
                                                    <StyleAction AnimationTarget="flyout5" Attribute="display" Value="block"/>
                                                                
                                                    <%-- Move the wire frame from the button's bounds to the info panel's bounds --%>
                                                    <Parallel AnimationTarget="flyout5" Duration=".3" Fps="25">
                                                        <Move Horizontal="10" Vertical="10" />
                                                        <Resize Width="400" Height="150" />
                                                        <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                                                    </Parallel>
                                                                
                                                    <%-- Move the info panel on top of the wire frame, fade it in, and hide the frame --%>
                                                    <ScriptAction Script="Cover($get('flyout5'), $get('div_ReversingEntry'), true);" />
                                                    <StyleAction AnimationTarget="div_ReversingEntry" Attribute="display" Value="block" />
                                                    <ScriptAction Script="div_ReversingEntry.style.visibility='visible';" />
                                                    <FadeIn AnimationTarget="div_ReversingEntry" Duration=".2"/>
                                                    <StyleAction AnimationTarget="flyout5" Attribute="display" Value="none"/>
                                                                
                                                    <%-- Flash the text/border red and fade in the "close" button --%>
                                                    <Parallel AnimationTarget="div_ReversingEntry" Duration=".5">
                                                        <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                                                        <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                                                    </Parallel>
                                                    <Parallel AnimationTarget="div_ReversingEntry" Duration=".5">
                                                        <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                                                        <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                                                    </Parallel>
                                                    <EnableAction Enabled="true" />
                                                </Sequence>
                                            </OnClick>
                                        </Animations>
                                    </cc1:AnimationExtender>
                                </td>
                                <td class="RowHeader" colspan="3">Epicor Interfacing Information</td>
                                <td class="RowHeader" colspan="2">NSL-to-Customer *</td>
                                <td class="RowHeader" colspan="3">Settlement Information</td>
                            </tr>
                            <tr>
                                <td class="FieldLabel" style="width:100px;">Invoice Scan Date</td>
                                <td class="FieldLabel" style="width:100px;" >Interfaced Amount</td>
                                <td class="FieldLabel" style="width:100px;">Epicor Interfaced Date</td>
                                <td class="FieldLabel" style="width:100px;">eInvoice Batch No.</td>
                                <td class="FieldLabel" style="width:100px;">eInvoice Submission Date</td>
                                <td class="FieldLabel" style="width:100px;">Settlement Date</td>
                                <td class="FieldLabel" style="width:100px;">Amount</td>
                                <td class="FieldLabel" style="width:100px;">Bank Reference No.</td>
                            </tr>
                            <tr >
                                <td class="FieldLabel" style="width:100px;">Cost of Good Sold</td>
                                <td ><asp:Label ID="lbl_InvScanDate_COGS" runat="server" /></td>
                                <td ><asp:Label ID="lbl_InterfacedAmt_COGS" runat="server" /></td>
                                <td ><asp:Label ID="lbl_InterfaceDate_COGS" runat="server" /></td>
                                <td >--</td>
                                <td>--</td>
                                <td><asp:Label ID="lbl_SettleDate_COGS" runat="server" /></td>
                                <td><asp:Label ID="lbl_SettleAmount_COGS" runat="server" /></td>
                                <td><asp:Label ID="lbl_RefNo_COGS" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">Sales</td>
                                <td><asp:Label ID="lbl_InvScanDate_Sales" runat="server" /></td>
                                <td><asp:Label ID="lbl_InterfacedAmt_Sales" runat="server" /></td>
                                <td><asp:Label ID="lbl_InterfaceDate_Sales" runat="server" /></td>
                                <td><asp:Label ID="lbl_eInvBatchNo_Sales" runat="server" /></td>
                                <td><asp:Label ID="lbl_eInvSubmitDate" runat="server" /></td>
                                <td><asp:Label ID="lbl_SettleDate_Sales" runat="server" /></td>
                                <td><asp:Label ID="lbl_SettleAmount_Sales" runat="server" /></td>
                                <td><asp:Label ID="lbl_RefNo_Sales" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">Sales Commission</td>
                                <td>--</td>
                                <td><asp:Label ID="lbl_InterfaceAmt_SalesCom" runat="server" /></td>
                                <td><asp:Label ID="lbl_InterfaceDate_SalesCom" runat="server" /></td>
                                <td>--</td>
                                <td>--</td>
                                <td><asp:Label ID="lbl_SettleDate_SalesComm" runat="server" /></td>
                                <td><asp:Label ID="lbl_SettleAmount_SalesComm" runat="server" /></td>
                                <td><asp:Label ID="lbl_RefNo_SalesComm" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">Account Payable</td>
                                <td>--</td>
                                <td><asp:Label ID="lbl_InterfaceAmt_AP" runat="server" /></td>
                                <td><asp:Label ID="lbl_InterfaceDate_AP" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">Account Receivable</td>
                                <td>--</td>
                                <td><asp:Label ID="lbl_InterfaceAmt_AR" runat="server" /></td>
                                <td><asp:Label ID="lbl_InterfaceDate_AR" runat="server" /></td>
                            </tr>
                            <tr>                        
                                <td id="Td3" colspan="4" style="padding: 0 0 0 0;" runat="server">
                                    <asp:GridView ID="gv_OtherCost" runat="server" AutoGenerateColumns="False" 
                                        Width="100%" OnRowDataBound="OtherCostDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Other Cost" ItemStyle-Width="200px" >
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_OtherCostTypeId" runat="server" Text='<%# Eval("OtherCostTypeId") %>' visible="false" />
                                                    <asp:Label ID="lbl_OtherCost" runat="server" Text='<%# Eval("OtherCostTypeDescription") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Amount" ItemStyle-Width="90px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Amount" runat="server" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Right" />
                                            </asp:TemplateField>
                                            <asp:TemplateField  HeaderText="Date" ItemStyle-Width="90px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_OtherCostDate" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                            </table>
                            <span style="font-size:8pt;">* eInvoice for non-self-bill order only.</span>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabAuditLog" runat="server" ScrollBars="None">
                        <HeaderTemplate>Audit Log</HeaderTemplate>
                        <ContentTemplate>
                            <!-- <span class="header2">Audit Log</span><br /><br /> -->
                            <asp:Panel ID="Panel6" runat="server" skinid="sectionHeader2"  Height="22" Width="650">
                                <div style="float:left;padding:3px;vertical-align:middle;" class="header2" >Audit Log</div>
                            </asp:Panel>
                            
                            <asp:GridView ID="gv_AuditLog" runat="server" Width="650px" OnRowDataBound="AuditLogRowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="Action Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_ActionDate" Text='<%# Eval("ActionDate") %>' runat="server" SkinID="SmallLabel" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action At">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_ActionType"  runat="server" SkinID="SmallLabel"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Description">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_Description"  runat="server" SkinID="SmallLabel"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Detail">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_ActionDetail" Text='<%# Eval("Remark") %>' runat="server" SkinID="SmallLabel"/>
                                            <asp:Image ID="img_ReadDmsDoc" ImageUrl="~/images/icon_edit.gif" Visible="false" runat="server" style="cursor:hand;" ToolTip="Read DMS Doc."/>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_User" runat="server" SkinID="SmallLabel"/>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Split Shipment">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_SplitShipment" runat="server" SkinID="SmallLabel"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                                <EmptyDataTemplate>
                                    No history.
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabSplitShipment" runat="server" Height="500" ScrollBars="Horizontal">
                        <HeaderTemplate>
                           <span style="color:Red ;font-weight:bold;">Split Shipment</span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table width="90%">
                                <tr>
                                    <td>
                            <asp:GridView ID="gv_SplitShipment" runat="server" OnRowDataBound="SplitShipmentDataBound" AutoGenerateColumns="false">
                                <Columns>                            
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnView" runat="server" ToolTip="View Detail" CommandName="cmdView" ImageUrl="~/images/btn_view.gif" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contract No."  ControlStyle-Width="90">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_SplitPONo" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier" ControlStyle-Width="150">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_Vendor" Text='<%# Eval("Vendor.Name") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Actual Shipped Qty" >
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_ShippedQty" Text='<%# Eval("TotalShippedQuantity") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Net Amount">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_NetAmt" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="QA Comm.">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_QAComm" Text='<%# Eval("QACommissionPercent") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier Invoice No.">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_SuppInvNo" Text='<%# Eval("SupplierInvoiceNo") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Doc Receipt Date">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_DocReceiptDate" Text='<%# Eval("ShippingDocReceiptDate", "{0:d}") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Split Item Description">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_description" Text='<%# Eval("Product.ShortDesc") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier At-WH Date" >
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_SchAtWHDate" Text='<%# Eval("SupplierAgreedAtWarehouseDate","{0:d}") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Require Payment" >
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_RequirePayment" runat="server" Text='<%# Convert.ToInt32(Eval("IsVirtualSetSplit")) == 1? "NO" : "YES" %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <label id="lblNoSplitShipment"> No split shipment. </label>
                                </EmptyDataTemplate>
                                        </asp:GridView>
                            </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabOtherDel" runat="server" HeaderText="Other Delivery" >
                        <HeaderTemplate>Other Delivery</HeaderTemplate>
                        <ContentTemplate>
                            <asp:GridView ID="gv_OtherDelivery" runat="server" OnRowDataBound="OtherDeliveryDataBound" 
                                OnRowCommand="OtherDeliveryRowCommand" AutoGenerateColumns="false" >
                            <Columns>
                                <asp:TemplateField HeaderText="Dly No.">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnk_DlyNo" runat="server" Text='<%# Eval("DeliveryNo") %>' 
                                            CommandArgument='<%# Eval("ShipmentId") %>' CommandName="OtherDelivery" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Supplier">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_Supplier" runat="server" Text='<%# Eval("Vendor.Name") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cus. AWH Date" >
                                    <ItemTemplate >
                                        <asp:Label ID="lbl_CustAtWHDate" runat="server" Text='<%# Eval("CustomerAgreedAtWarehouseDate", "{0:d}") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Qty">
                                    <ItemTemplate >
                                        <asp:Label ID="lbl_OrderQty" runat="server" Text='<%# Eval("TotalOrderQuantity","{0:#,##0}") %>'  />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipped Qty">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_ShippedQty" runat="server" Text='<%# Eval("TotalShippedQuantity","{0:#,##0}") %>'  />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ccy">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_Currency" runat="server" Text='<%# Eval("SellCurrency.CurrencyCode") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Amt">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_OrderAmt" runat="server" Text='<%# Eval("TotalOrderAmount","{0:#,##0.00}") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sales Amt">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_SalesAmt" runat="server" Text='<%# Eval("InvoiceAmount","{0:#,##0.00}") %>'  />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Invoice No.">
                                    <ItemTemplate >
                                        <asp:Label ID="lbl_InvoiceNo" runat="server"  Text='<%# Eval("InvoiceNo") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Invoice Date">
                                    <ItemTemplate >
                                        <asp:Label ID="lbl_InvoiceDate" runat="server"  Text='<%# Eval("InvoiceDate", "{0:d}") %>'  />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" >
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_Status" runat="server" Text='<%# Eval("WorkflowStatus.Name") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <label id="lblNoOtherDelivery">No other delivery.</label>
                            </EmptyDataTemplate>

                        </asp:GridView>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="tabManifest" runat="server" HeaderText="Manifest" ScrollBars="None">
                        <HeaderTemplate>Manifest</HeaderTemplate>
                        <ContentTemplate>
                            <asp:GridView id="gv_Manifest" runat ="server" OnRowDataBound="ManifestDataBound"  AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Voyage No.">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_VoyageNo"  runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText ="Vessel Name / AWB No.">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_VesselName" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Container No.">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbl_ContainerNo" Font-Size="8pt" runat="server"   Text='<%# Eval("ContainerNo") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Partner Container No.">
                                        <ItemTemplate >
                                            <asp:Label ID="lbl_PartnerContainerNo" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CTS Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_CTSDate" runat="server" Text='<%# Eval("ConfirmedToShipDate","{0:d}") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Departure Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_DepartureDate" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Departure Port">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_DeparturePort" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Arrival Port">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_ArrivalPort" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                </Columns>
                                <EmptyDataTemplate>
                                    No manifest record.
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="TabDeduction" runat="server" HeaderText="Payment Deduction" ScrollBars="None" >
                        <HeaderTemplate>
                            <span id="tcDeductionTitle" runat="server">
                                <asp:Image id="img_tbHeader_Deduction" runat="server" ImageUrl="~/images/icon_sp.gif" Visible="false" />Deduction
                            </span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table>
                                <tr>
                                    <!-- <td class="header2">Payment Deduction</td> -->
                                    <td>
                                        <asp:Panel ID="Panel7" runat="server" skinid="sectionHeader2"  Height="22"  >
                                            <div style="float:left;padding:5px;">
                                                <img src="../images/pt3.gif" alt=""/>
                                            </div>
                                            <div style="float:left;padding:3px;vertical-align:middle;" class="header2">Payment Deduction</div>
                                        </asp:Panel>
                                    </td>             
                                </tr>
                                <tr>
                                    <td>
                                        <asp:CustomValidator ID="val_PaymentDeduction" OnServerValidate="val_PaymentDeduction_Validate" runat="server" Enabled="true" ErrorMessage=""></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr style=" vertical-align:top;">
                                    <td>              
                                        <asp:GridView ID="gv_PaymentDeduction"  OnRowDataBound="PaymentDeductionDataBound" runat="server" AutoGenerateColumns="False" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Deduction Type">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_DeductionType" runat="server"   Text='<%# Eval("DeductionType.Name") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Document No.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_DocNo" runat="server"   Text='<%# Eval("DocumentNo") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="C19 Info.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_C19Info" runat="server"   Text='' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Amount"  runat="server" Text='<%# Eval("Amount","{0:#,##0.00}") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Remark">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Remark"  runat="server" Text='<%# Eval("Remark") %>'/>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" Width="300px" />
                                            </asp:TemplateField>
                                            </Columns>
                                        <EmptyDataTemplate>
                                            No deduction record.
                                            </EmptyDataTemplate>
                                            </asp:GridView>
                                        <asp:GridView ID="gv_PaymentDeductionUpdate"  OnRowUpdating="PaymentDeductionRowUpdating" OnRowDataBound="PaymentDeductionUpdateDataBound"
                                            OnRowCommand="PaymentDeductionRowCommand" runat="server" AutoGenerateColumns="False" OnRowCancelingEdit="PaymentDeductionRowCancelEdit"
                                            OnRowDeleting="PaymentDeductionRowDelete" Visible="false">
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:ImageButton ID="lnk_AddPaymentDeduction" runat="server" ToolTip="Add" ImageUrl="~/images/icon_newrequest.gif" CommandName="Add" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="lnk_DeletePaymentDeduction" runat="server" ToolTip="Delete" ImageUrl="~/images/icon_delete.gif" OnClientClick="return confirm('Are you sure to delete?');" CommandName="Delete" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Deduction Type">
                                                <ItemTemplate>
                                                    <cc2:SmartDropDownList ID="ddl_DeductionType" runat="server" PrevSelectedIndex="" 
                                                     OnFocus="this.PrevSelectedIndex = this.selectedIndex;" onchange="deductionType_OnChange()"
                                                    />
                                                    <asp:TextBox ID="txt_DeductionType" runat="server" Text='<%# Eval("DeductionType.Name") %>' ReadOnly="true"   style=" width:100px;" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Doc. No.">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_DocNo" runat="server" MaxLength="20" Text='<%# Eval("DocumentNo") %>' />
                                                </ItemTemplate>                                        
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="C19 Info.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_C19Info" runat="server"   Text='' />
                                                </ItemTemplate>                                        
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_Amount"  runat="server" Text='<%# Eval("Amount") %>' style=" width:65px;text-align:right;" />
                                                    <asp:ImageButton ID="lnk_Import" ImageUrl="~/images/Icon_Excel.jpg" runat="server" ToolTip="Import"  OnClientClick="importDeductionAmount(this);return false;"  Visible="false" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" Width="70px" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Remark">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txt_Remark"  runat="server" Text='<%# Eval("Remark") %>' style=" width:300px;text-align:left;"/>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             </Columns>
                                        <EmptyDataTemplate>
                                            No Deduction record.
                                            </EmptyDataTemplate>
                                            </asp:GridView>
                                        <br />
                                            <asp:Button ID="btn_NewPaymentDeduction" runat="server" Text="New" OnClick="btn_NewPaymentDeduction_Click"  Visible="false" />                         
                                    </td>
                               </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </cc1:TabPanel>

                </cc1:TabContainer>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr id="tr_ButtonArea">
            <td colspan="4">        
                <asp:LinkButton ID="lnk_Prev"   Text="<< Previous" Title="View the Previous Shipment" TabIndex="501" runat="server"/>&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Edit"   Text="Edit"    Visible="false" OnClick="btn_Edit_Click"   TabIndex="502" runat="server"/>
                <asp:Button ID="btn_Save"   Text="Save"    Visible="false" OnClick="btn_Save_Click"   TabIndex="503" runat="server" />&nbsp;
                <asp:Button ID="btn_Print"  Text="Print"   Visible="false" OnClick="btn_Print_Click"  TabIndex="504" runat="server"/>&nbsp;
                <asp:Button ID="btn_PrintDN"    Text=""   Visible="false" OnClick="btn_PrintDN_Click"  TabIndex="505" runat="server" SkinID="XLButton"/>
                <asp:Button ID="btn_Send"   Text="Send"    Visible="false" OnClick="btn_Send_Click"   TabIndex="506" runat="server"/>
                <asp:Button ID="btn_DiscrepancyAlert" Text="Discrepancy Alert" SkinID="LButton" Visible="false" OnClick="btn_DiscrepancyAlert_Click" TabIndex="507" runat="server" onmouseover="showDiscrepancyAlertLog()" onmouseout="hideDiscrepancyAlertLog()" />
                <asp:Button ID="btn_Cancel" Text="Cancel"  Visible="false" OnClick="btn_Cancel_Click" TabIndex="508" runat="server"/>&nbsp;
                <asp:Button ID="btn_Close"  Text="Close"   Visible="true"  TabIndex="509" runat="server"/>&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnk_Next"   Text="Next >>" Title="View the Next Shipment" TabIndex="510" runat="server"/>
            </td>
        </tr>
</table>

        <div id="div_DiscrepancyAlertLog" class="infoLayer" style="display: none; width: 300px; height: 150px;z-index:100;position:absolute;">
        <span style="font-weight :bold ; text-decoration :underline; height: 35px;">Discrepancy Message Delivered Log</span>
        
                            <asp:GridView ID="gv_DiscrepancyAlertLog" runat="server" Width="280px" OnRowDataBound="DiscrepancyLogRowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="Delivered By">
                                        <ItemTemplate><asp:Label ID="lbl_DeliveredBy" runat="server" /></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date" ItemStyle-Width="80px">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_ActionDate" Text='<%# Eval("ActionDate" , "{0:d}") %>' runat="server"  />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No history.
                                </EmptyDataTemplate>
                            </asp:GridView>
        </div>
    </form>
</body>
<script type="text/javascript">
    initTabHeader();

    function initTabHeader() {
        if (document.getElementById("lblNoSplitShipment")) 
            document.all.__tab_tcContract_tabSplitShipment.style.display = "none";
        else 
            document.all.__tab_tcContract_tabSplitShipment.style.display = "";

        if (document.getElementById("lblNoOtherDelivery")) 
            document.all.__tab_tcContract_tabOtherDel.style.display = "none";
        else 
            document.all.__tab_tcContract_tabOtherDel.style.display = "";
        
    }

</script>
</html>

