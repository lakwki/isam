<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="reports.aspx.cs" Inherits="com.next.isam.webapp.reporter.reports" Title="Reports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinId="sectionHeader_Report"></asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<script type="text/javascript">
	/***********************************************
	* Image w/ description tooltip- By Dynamic Web Coding (www.dyn-web.com)
	* Copyright 2002-2007 by Sharon Paine
	* Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
	***********************************************/

	/* IMPORTANT: Put script after tooltip div or 
	put tooltip div just before </BODY>. */

	var dom = (document.getElementById) ? true : false;
	var ns5 = (!document.all && dom || window.opera) ? true : false;
	var ie5 = ((navigator.userAgent.indexOf("MSIE") > -1) && dom) ? true : false;
	var ie4 = (document.all && !dom) ? true : false;
	var nodyn = (!ns5 && !ie4 && !ie5 && !dom) ? true : false;

	var origWidth, origHeight;

	// avoid error of passing event object in older browsers
	if (nodyn) { event = "nope" }

	///////////////////////  CUSTOMIZE HERE   ////////////////////
	// settings for tooltip 
	// Do you want tip to move when mouse moves over link?
	var tipFollowMouse = true;
	// Be sure to set tipWidth wide enough for widest image
	var tipWidth = 640;
	var offX = 15; // how far from mouse to show tip
	var offY = 15;
	var tipFontFamily = "Verdana, arial, helvetica, sans-serif";
	var tipFontSize = "9pt";
	// set default text color and background color for tooltip here
	// individual tooltips can have their own (set in messages arrays)
	// but don't have to
	var tipFontColor = "#000000";
	var tipBgColor = "#DDECFF";
	var tipBorderColor = "#000000";
	var tipBorderWidth = 1;
	var tipBorderStyle = "ridge";
	var tipPadding = 4;

	// tooltip content goes here (image, description, optional bgColor, optional textcolor)
	var messages = new Array();
	// multi-dimensional arrays containing: 
	// image and text for tooltip
	// optional: bgColor and color to be sent to tooltip
	messages[0] = new Array('../images/Report_LCAppSummary.jpg', 'L/C Application Summary', "#FFFFFF", "blue");
	messages[1] = new Array('../images/Report_LCAppControl.jpg', 'L/C Application Control', "#FFFFFF", "blue");
	messages[2] = new Array('../images/Report_LCStatus.jpg', 'L/C Status', "#FFFFFF", "blue");
	messages[3] = new Array('../images/Report_OutstandingLC.jpg', 'Outstanding L/C', "#FFFFFF", "blue");
	messages[4] = new Array('../images/Report_WeeklyShipment.jpg', 'Weekly Shipment', "#FFFFFF", "blue");
	messages[5] = new Array('../images/Report_ShipmentForecast.jpg', 'Shipment Forecast', "#FFFFFF", "blue");
	messages[6] = new Array('../images/Report_ShipmentForecastEzibuy.jpg', 'Shipment Forecast (Ezibuy)', "#FFFFFF", "blue");
	messages[7] = new Array('../images/Report_SalesActualAccrual.jpg', 'Sales (Actual & Accrual)', "#FFFFFF", "blue");
	messages[8] = new Array('../images/Report_InvoiceList.jpg', 'Invoice List', "#FFFFFF", "blue");
	messages[9] = new Array('../images/Report_OutstandingBooking.jpg', 'Outstanding Booking', "#FFFFFF", "blue");
    messages[10] = new Array('../images/Report_ShipmentCOmmission.jpg', 'Shipment and Commission', "#FFFFFF", "blue");
    messages[11] = new Array('../images/Report_ShipmentCommissionMockShop.jpg', 'Shipment and Commission (Mock Shop)', "#FFFFFF", "blue");
    messages[12] = new Array('../images/Report_ShipmentCommissionChoice.jpg', 'Shipment and Commission (CHOICE)', "#FFFFFF", "blue");
    messages[13] = new Array('../images/Report_ActualSalesSummary.jpg', 'Actual Sales Summary', "#FFFFFF", "blue");
    messages[14] = new Array('../images/Report_OtherCostSummary.jpg', 'Other Cost Summary', "#FFFFFF", "blue");
    messages[15] = new Array('../images/Report_SupplierPayment.jpg', 'Supplier Payment', "#FFFFFF", "blue");
    messages[16] = new Array('../images/Report_CustomerReceipt.jpg', 'Customer Receipt', "#FFFFFF", "blue");
    messages[17] = new Array('../images/Report_AccountsReceivePayForecast.jpg', 'Accounts Receivable and Payable Forecast', "#FFFFFF", "blue");
    messages[18] = new Array('../images/Report_ActiveSupplier.jpg', 'Active Supplier', "#FFFFFF", "blue");
    messages[19] = new Array('../images/Report_OutstandingTradePayable.jpg', 'Outstanding Trade Payable', "#FFFFFF", "blue");
    messages[20] = new Array('../images/Report_OutstandingTradeReceivable.jpg', 'Outstanding Trade Receivable', "#FFFFFF", "blue");
    messages[21] = new Array('../images/Report_OutstandingPayDocToAccount.jpg', 'Outstanding Payment Document to A/C', "#FFFFFF", "blue");
    messages[22] = new Array('../images/Report_ReleaseLockSummary.jpg', 'Release Lock Summary', "#FFFFFF", "blue");
    messages[23] = new Array('../images/Report_StwDateDiscrepancy.jpg', 'STW Date Discrepancy', "#FFFFFF", "blue");
    messages[24] = new Array('../images/Report_SupplierOrderStatus.jpg', 'Supplier Order Status', "#FFFFFF", "blue");
    messages[25] = new Array('../images/Report_SalesAccrualArchive.jpg', 'Sales Accrual Archive', "#FFFFFF", "blue");
    messages[26] = new Array('../images/Report_MonthEndSummary.jpg', 'Month End Summary', "#FFFFFF", "blue");
    messages[27] = new Array('../images/Report_NslSzOrder.jpg', 'NSL (SZ) Order', "#FFFFFF", "blue");
    messages[28] = new Array('../images/Report_UKClaimSummary.jpg', 'Next Claim Summary', "#FFFFFF", "blue");
    messages[29] = new Array('../images/Report_UKClaimPhasingByOffice.jpg', 'Next Claim By Phasing By Office', "#FFFFFF", "blue");
    messages[30] = new Array('../images/Report_UKClaimPhasingByReason.jpg', 'Next Claim By Phasing By Reason', "#FFFFFF", "blue");
    messages[31] = new Array('../images/Report_UKClaimPhasingBySupplier.jpg', 'Next Claim By Phasing By Supplier', "#FFFFFF", "blue");
    messages[32] = new Array('../images/Report_UKClaimPhasingByProduct.jpg', 'Next Claim By Phasing By Product Team', "#FFFFFF", "blue");
    messages[33] = new Array('../images/Report_OutstandingUKClaim.jpg', 'Outstanding Next Claim', "#FFFFFF", "blue");
    messages[34] = new Array('../images/Report_UKClaimAuditLog.jpg', 'Next Claim Audit Log', "#FFFFFF", "blue");
    messages[35] = new Array('../images/Report_NonTradeExpenseStatement.jpg', 'Non-Trade Expense Statement List', "#FFFFFF", "blue");
    messages[36] = new Array('../images/Report_MFRNQtyAnalysisReport.jpg', 'Next Claim MFRN Qty Analysis Report', "#FFFFFF", "blue");
    messages[37] = new Array('../images/Report_EpicorInterfaceLogReport.jpg', 'Epicor Interface Log Report', "#FFFFFF", "blue");
    messages[38] = new Array('../images/Report_LCShipmentAmendment.jpg', 'L/C Shipment Amenedment Report', "#FFFFFF", "blue");
    messages[39] = new Array('../images/Report_OutstandingGBTestResultReport.jpg', 'Outstanding GB Test Result Report', "#FFFFFF", "blue");
    messages[40] = new Array('../images/Report_AF.png', 'Trading A/F Report', "#FFFFFF", "blue");
    messages[41] = new Array('../images/Report_UTDiscrepancy.png', 'UT Discrepancy Report', "#FFFFFF", "blue");
    messages[42] = new Array('../images/Report_AdvancePaymentReport.png', 'Advance Payment Report', "#FFFFFF", "blue");
    messages[43] = new Array('../images/Report_ShipmentCommissionStudioSample.jpg', 'Shipment and Commission (Studio Sample)', "#FFFFFF", "blue");
    messages[44] = new Array('../images/Report_LGHoldPaymentReport.png', 'LG Hold Payment Report', "#FFFFFF", "blue");
    messages[45] = new Array('../images/POListByOfficeSupplier.png', 'PO List By Office Supplier Report', "#FFFFFF", "blue");
    messages[46] = new Array('../images/Report_OutstandingUKDiscountClaim.jpg', 'Outstanding Next Discount Claim', "#FFFFFF", "blue");
    messages[47] = new Array('../images/Report_FutureOrderSummaryBySupplierReport.png', 'Future Order Summary By Supplier', "#FFFFFF", "blue");
    messages[48] = new Array('../images/Report_InvoiceListSummary.jpg', 'Invoice List Summary (BD)', "#FFFFFF", "blue");
    messages[49] = new Array('../images/Report_NSLedProfitability.jpg', 'NS-Led Profitability Report', "#FFFFFF", "blue");
    messages[50] = new Array('../images/Report_NSLedSalesInfo.jpg', 'NS-Led Sales Info Report', "#FFFFFF", "blue");
    messages[51] = new Array('../images/Report_NSLedSellThruHistory.jpg', 'NS-Led Sell Thru History Report', "#FFFFFF", "blue");
    messages[52] = new Array('../images/Report_OrdersForLCCancellation.jpg', 'Orders For LC Cancellation Report', "#FFFFFF", "blue");
    messages[53] = new Array('../images/Report_CarbonFootprintReport.jpg', 'Carbon Footprint Report', "#FFFFFF", "blue");
    messages[54] = new Array('../images/Report_NSLedActualSalesSummary.JPG', 'NS-Led Actual Sales Summary Report', "#FFFFFF", "blue");
    
	////////////////////  END OF CUSTOMIZATION AREA  ///////////////////

	// preload images that are to appear in tooltip
	// from arrays above
	if (document.images) {
		var theImgs = new Array();
		for (var i = 0; i < messages.length; i++) {
		    theImgs[i] = new Image();
		    theImgs[i].src = messages[i][0];
		}
	}

	// to layout image and text, 2-row table, image centered in top cell
	// these go in var tip in doTooltip function
	// startStr goes before image, midStr goes between image and text
	//var startStr = '<table width="' + tipWidth + '"><tr><td align="center" width="100%"><img src="';
	//var midStr = '" border="0"></td></tr><tr><td valign="top">';
	//var endStr = '</td></tr></table>';

	var startStr = '<table width="' + tipWidth + '"><tr><td valign="top">';
	var midStr = '</td></tr><tr><td align="center" width="100%"><img src="';
	var endStr = '" border="0"></td></tr></table>';

	function getTipString(image, title, fontColor, width) {
	    //return '<table width="' + tipWidth + '"><tr><td valign="top">';
	    return '<table ' + (width == undefined ? '' : 'width="' + width + '"') + '>'
            + '<tr><td valign="top">'
            + '<span style="font-family:' + tipFontFamily + '; font-size:' + tipFontSize + '; color:' + (fontColor == undefined ? "black" : fontColor) + ';">' + (title == undefined ? "" : title) + '</span>'
            + '</td></tr>'
            + '<tr><td align="center" ><img src="' + (image == undefined ? "" : image) + '" border="0"></td></tr>' 
            + '</table>';
	}

	////////////////////////////////////////////////////////////
	//  initTip	- initialization for tooltip.
	//		Global variables for tooltip. 
	//		Set styles
	//		Set up mousemove capture if tipFollowMouse set true.
	////////////////////////////////////////////////////////////
	var tooltip, tipcss;
	function initTip() {
		if (nodyn) return;
		tooltip = (ie4) ? document.all['tipDiv'] : (ie5 || ns5) ? document.getElementById('tipDiv') : null;
		tipcss = tooltip.style;
		if (ie4 || ie5 || ns5) {	// ns4 would lose all this on rewrites
		    //tipcss.width = tipWidth + "px";
		    tipcss.fontFamily = tipFontFamily;
		    tipcss.fontSize = tipFontSize;
		    tipcss.color = tipFontColor;
		    tipcss.backgroundColor = tipBgColor;
		    tipcss.borderColor = tipBorderColor;
		    tipcss.borderWidth = tipBorderWidth + "px";
		    tipcss.padding = tipPadding + "px";
		    tipcss.borderStyle = tipBorderStyle;
		}
		if (tooltip && tipFollowMouse) {
		    document.onmousemove = trackMouse;
		}
	}

	window.onload = initTip;

	/////////////////////////////////////////////////
	//  doTooltip function
	//			Assembles content for tooltip and writes 
	//			it to tipDiv
	/////////////////////////////////////////////////
	var t1, t2; // for setTimeouts
	var tipOn = false; // check if over tooltip link
	function doTooltip(evt, num) {
	    if (!tooltip) return;
	    if (t1) clearTimeout(t1); if (t2) clearTimeout(t2);
	    tipOn = true;
	    // set colors if included in messages array
	    if (messages[num][2]) var curBgColor = messages[num][2];
	    else curBgColor = tipBgColor;
	    if (messages[num][3]) var curFontColor = messages[num][3];
	    else curFontColor = tipFontColor;
	    if (ie4 || ie5 || ns5) {
	        //var tip = startStr + messages[num][0] + midStr + '<span style="font-family:' + tipFontFamily + '; font-size:' + tipFontSize + '; color:' + curFontColor + ';">' + messages[num][1] + '</span>' + endStr;
	        //var tip = startStr + '<span style="font-family:' + tipFontFamily + '; font-size:' + tipFontSize + '; color:' + curFontColor + ';">' + messages[num][1] + '</span>' + midStr + messages[num][0] + endStr;
	        tip = getTipString(messages[num][0], messages[num][1], curFontColor);

	        tipcss.backgroundColor = curBgColor;
	        tooltip.innerHTML = tip;
	    }
	    if (!tipFollowMouse) positionTip(evt);
	    else t1 = setTimeout("tipcss.visibility='visible'", 100);
	}

	var mouseX, mouseY;
	function trackMouse(evt) {
		standardbody = (document.compatMode == "CSS1Compat") ? document.documentElement : document.body //create reference to common "body" across doctypes
		mouseX = (ns5) ? evt.pageX : window.event.clientX + standardbody.scrollLeft;
		mouseY = (ns5) ? evt.pageY : window.event.clientY + standardbody.scrollTop;
		if (tipOn) positionTip(evt);
	}

	/////////////////////////////////////////////////////////////
	//  positionTip function
	//		If tipFollowMouse set false, so trackMouse function
	//		not being used, get position of mouseover event.
	//		Calculations use mouseover event position, 
	//		offset amounts and tooltip width to position
	//		tooltip within window.
	/////////////////////////////////////////////////////////////
	function positionTip(evt) {
		if (!tipFollowMouse) {
		    mouseX = (ns5) ? evt.pageX : window.event.clientX + standardbody.scrollLeft;
		    mouseY = (ns5) ? evt.pageY : window.event.clientY + standardbody.scrollTop;
		}
		// tooltip width and height
		var tpWd = (ie4 || ie5) ? tooltip.clientWidth : tooltip.offsetWidth;
		var tpHt = (ie4 || ie5) ? tooltip.clientHeight : tooltip.offsetHeight;
		// document area in view (subtract scrollbar width for ns)
		var winWd = (ns5) ? window.innerWidth - 20 + window.pageXOffset : standardbody.clientWidth + standardbody.scrollLeft;
		var winHt = (ns5) ? window.innerHeight - 20 + window.pageYOffset : standardbody.clientHeight + standardbody.scrollTop;
		// check mouse position against tip and window dimensions
		// and position the tooltip
		if ((mouseX + offX + tpWd) > winWd)
		//tipcss.left = mouseX-(tpWd+offX)+"px";
		     tipcss.left = winWd - (tpWd + offX) + "px";
		//tipcss.left = "300px";
		else
		    tipcss.left = mouseX + offX + "px";
		    //tipcss.left = "300px";
		if ((mouseY + offY + tpHt) > winHt)
		    tipcss.top = winHt - (tpHt + offY) + "px";
		else 
            tipcss.top = mouseY + offY + "px";
		if (!tipFollowMouse) t1 = setTimeout("tipcss.visibility='visible'", 100);
	}

	function hideTip() {
		if (!tooltip) return;
		t2 = setTimeout("tipcss.visibility='hidden'", 100);
		tipOn = false;
	}

	document.write('<div id="tipDiv" style="position:absolute; visibility:hidden; z-index:100"></div>')


	var UkClaimSeq = 0;
	function ukClaimPhasingSeq() {
	    if (UkClaimSeq < 29 || UkClaimSeq > 31)
	        UkClaimSeq = 29;
	    else
	        UkClaimSeq++;
	    return UkClaimSeq;
	}
</script>


<table cellspacing="0" cellpadding="0" width="100%" border="0">
    <col width="200px" /><col width="25" /><col /><col  />
    <tbody runat="server" GroupName="L/C Reports" visible="false">
        <tr id="row_LCApplicationSummaryReport" runat="server" visible="false" style="border-top:1px solid grey;">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 0)" onmouseout="hideTip()" /></td>
            <td><a href="LCApplicationSummaryReport.aspx" target="_self">L/C Application Summary Report</a></td>
        </tr>
        <tr id="row_LCBatchControl" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 1)" onmouseout="hideTip()" /></td>
            <td><a href="LCBatchControlReport.aspx" target="_self">L/C Application Control Report</a></td>
        </tr>
        <tr id="row_LCStatus" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 2)" onmouseout="hideTip()" /></td>
            <td><a href="LCStatusReport.aspx" target="_self">L/C Status Report</a></td>
        </tr>
        <tr id="row_OutstandingLC" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 3)" onmouseout="hideTip()" /></td>
            <td><a href="OutstandingLCReport.aspx" target="_self">Outstanding L/C Report</a></td>
        </tr>
        <tr id="row_LCShipmentAmendment" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 38)" onmouseout="hideTip()" /></td>
            <td><a href="LCShipmentAmendmentReport.aspx" target="_self">L/C Shipment Amendment Report</a></td>
        </tr>
        <tr id="row_OrdersForLCCancellationReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 52)" onmouseout="hideTip()" /></td>
            <td><a href="OrdersForLCCancellationReport.aspx" target="_self">Orders For LC Cancellation Report</a></td>
        </tr>
    </tbody>
    <tbody runat="server" GroupName="Shipment Reports" visible="false">
        <tr id="row_WeeklyShipment" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 4)" onmouseout="hideTip()" /></td>
            <td> <a href="WeeklyShipmentReport.aspx" target="_self">Weekly Shipment Report</a></td>
        </tr>
        <tr id="row_PartialShipment" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 5)" onmouseout="hideTip()" /></td>
            <td><a href="PartialShipmentReport.aspx" target="_self">Shipment Forecast Report</a></td>
        </tr>
        <tr id="row_EziBuyPartialShipment" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 6)" onmouseout="hideTip()" /></td>
            <td><a href="EziBuyPartialShipmentReport.aspx" target="_self">Shipment Forecast Report (EziBuy)</a></td>
        </tr>
        <tr id="row_CTSSTW" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 7)" onmouseout="hideTip()" /></td>
            <td><a href="CTSSTWReport.aspx" target="_self">Sales Report (Actual & Accrual)</a></td>
        </tr>
        <tr id="row_InvoiceList" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 8)" onmouseout="hideTip()" /></td>
            <td><a href="InvoiceListReport.aspx" target="_self">Invoice List</a></td>
        </tr>
        <tr id="row_OutstandingBooking" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 9)" onmouseout="hideTip()" /></td>
            <td><a href="OutstandingBookingReport.aspx" target="_self">Outstanding Booking Report</a></td>
        </tr>
        <tr id="row_OutstandingGBTestResult" runat="server" visible ="false" >
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 39)" onmouseout="hideTip()" /></td>
            <td><a href="OutstandingGBTestReport.aspx" target="_self">Outstanding GB Test Result Report</a></td>
        </tr>
        <tr id="row_TradingAFReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 40)" onmouseout="hideTip()" /></td>
            <td><a href="TradingAFReport.aspx" target="_self">Trading A/F Report</a></td>
        </tr>
        <tr id="row_UTDiscrepancyReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 41)" onmouseout="hideTip()" /></td>
            <td><a href="UTSizePriceDiscrepancyReport.aspx" target="_self">UT Size Option Price Discrepancy Report</a></td>
        </tr>
        <tr id="row_InvoiceListSummary" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 48)" onmouseout="hideTip()" /></td>
            <td><a href="InvoiceListSummaryReport.aspx" target="_self">Invoice List Summary (BD)</a></td>
        </tr>
    </tbody>
    <tbody runat="server" GroupName="Accounts Sales Reports" visible="false">
        <tr id="row_ShipmentCommission" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 10)" onmouseout="hideTip()" /></td>
            <td><a href="ShipmentCommissionReport.aspx" target="_self">Shipment and Commission Statement</a></td>
        </tr>
        <tr id="row_ShipmentCommissionMockShop" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 11)" onmouseout="hideTip()" /></td>
            <td><a href="ShipmentCommissionMockShopReport.aspx" target="_self">Shipment and Commission Statement (Mock Shop)</a></td>
        </tr>
        <tr id="row_ShipmentCommissionStudioSample" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 43)" onmouseout="hideTip()" /></td>
            <td><a href="ShipmentCommissionStudioSampleReport.aspx" target="_self">Shipment and Commission Statement (Studio Sample)</a></td>
        </tr>
        <tr id="row_ShipmentCommissionChoice" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 12)" onmouseout="hideTip()" /></td>
            <td><a href="ShipmentCommissionChoiceReport.aspx" target="_self">Shipment and Commission Statement (Choice Order)</a></td>
        </tr>
        <tr id="row_ActualSalesSummary" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 13)" onmouseout="hideTip()" /></td>
            <td><a href="ActualSalesSummary.aspx" target="_self">Actual Sales Summary</a></td>
        </tr>
        <tr id="row_OtherCostSummaryReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 14)" onmouseout="hideTip()" /></td>
            <td><a href="OtherCostSummaryReport.aspx" target="_self">Other Cost Summary Report</a></td>
        </tr>
    </tbody>
    <tbody runat="server" GroupName="Accounts AR and AP Reports" visible="false">
        <tr id="row_SupplierPayment" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 15)" onmouseout="hideTip()" /></td>
            <td><a href="SupplierPaymentReport.aspx" target="_self">Supplier Payment Report</a></td>
        </tr>
        <tr id="row_CustomerReceipt" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 16)" onmouseout="hideTip()" /></td>
            <td><a href="CustomerReceiptReport.aspx" target="_self">Customer Receipt Report</a></td>
        </tr>
        <tr id="row_ReceivablePayableForecastReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 17)" onmouseout="hideTip()" /></td>
            <td><a href="ReceivablePayableForecastReport.aspx" target="_self">Accounts Receivable and Payable Forecast Report</a></td>
        </tr>
        <tr id="row_ActiveSupplierReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 18)" onmouseout="hideTip()" /></td>
            <td><a href="ActiveSupplierReport.aspx" target="_self">Active Supplier Report</a></td>
        </tr>
        <tr id="row_OutstandingAP" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 19)" onmouseout="hideTip()" /></td>
            <td><a href="OutstandingTradePayableRpt.aspx" target="_self">Outstanding Trade Payable Report</a></td>
        </tr>
        <tr id="row_OutstandingAR" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 20)" onmouseout="hideTip()" /></td>
            <td><a href="OutstandingTradeReceivableRpt.aspx" target="_self">Outstanding Trade Receivable Report</a></td>
        </tr>
        <tr id="row_OutstandingPaymentReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 21)" onmouseout="hideTip()" /></td>
            <td><a href="OutstandingPaymentReport.aspx" target="_self">Outstanding Payment Document to A/C Report</a></td>
        </tr>
        <tr id="row_AdvancePaymentReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 42)" onmouseout="hideTip()" /></td>
            <td><a href="AdvancePaymentReport.aspx" target="_self">Advance Payment Report</a></td>
        </tr>
        <tr id="row_LGHoldPaymentReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 44)" onmouseout="hideTip()" /></td>
            <td><a href="LGHoldPaymentReport.aspx" target="_self">LG Hold Payment Report</a></td>
        </tr>
        <tr id="row_POListByOfficeSupplierReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 45)" onmouseout="hideTip()" /></td>
            <td><a href="POListByOfficeSupplierReport.aspx" target="_self">P/O List By Office/Supplier Report</a></td>
        </tr>
        <tr id="row_FutureOrderSummaryBySupplierReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 47)" onmouseout="hideTip()" /></td>
            <td><a href="FutureOrderSummaryBySupplierReport.aspx" target="_self">Future Order Summary By Supplier Report</a></td>
        </tr>
    </tbody>
    <tbody runat="server" GroupName="Release Lock Reports" visible="false">
        <tr id="row_ReleaseLockSummary" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 22)" onmouseout="hideTip()" /></td>
            <td><a href="ReleaseLockSummary.aspx" target="_self">Release Lock Summary</a></td>
        </tr>
    </tbody>
    <tbody runat="server" GroupName="ILS Discrepancy Reports" visible="false">
        <tr id="row_StwDateDiscrepancyReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 23)" onmouseout="hideTip()" /></td>
            <td><a href="StwDateDiscrepancyReport.aspx" target="_self">STW Date Discrepancy Report</a></td>
        </tr>
    </tbody>
    <tbody runat="server" GroupName="Other Reports" visible="false">
        <tr id="row_SupplierOrderStatusReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 24)" onmouseout="hideTip()" /></td>
            <td><a href="SupplierOrderStatusReport.aspx" target="_self">Supplier Order Status Enquiry</a></td>
        </tr>
        <tr id="row_SalesAccrualArchiveReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 25)" onmouseout="hideTip()" /></td>
            <td><a href="SalesAccrualArchiveReport.aspx" target="_self">Sales Accrual Archive Report</a></td>
        </tr>
        <tr id="row_MonthEndSummaryReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 26)" onmouseout="hideTip()" /></td>
            <td><a href="MonthEndSummaryReport.aspx" target="_self">Month End Summary Report</a></td>
        </tr>
        <tr id="row_EpicorInterfaceLogReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 36)" onmouseout="hideTip()" /></td>
            <td><a href="EpicorInterfaceLogReport.aspx" target="_self">Epicor Interface Log Report</a></td>
        </tr>
    </tbody>
    <tbody runat="server" GroupName="NSL (SZ) Reports" visible="false">
        <tr id="row_NSLSZOrderReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 27)" onmouseout="hideTip()" /></td>
            <td><a href="NSLSZOrderReport.aspx" target="_self">NSL (SZ) Order Report</a></td>
        </tr>
    </tbody>
    <tbody runat="server" GroupName="Next Claim Reports" visible="false">
        <tr id="row_UKClaimSummaryReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 28)" onmouseout="hideTip()" /></td>
            <td><a href="UKClaimSummaryReport.aspx" target="_self">Next Claim Summary Report</a></td>
        </tr>
        <tr id="row_UKClaimPhasingReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, ukClaimPhasingSeq())" onmouseout="hideTip()" /></td>
            <td><a href="UKClaimPhasingReportPage.aspx" target="_self">Next Claim By Phasing Report</a></td>
        </tr>
        <tr id="row_OSUKClaimListReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 33)" onmouseout="hideTip()" /></td>
            <td><a href="OutstandingClaimListReportPage.aspx" target="_self">Outstanding Next Claim List Report</a></td>
        </tr>
        <tr id="row_OSUKDiscountListReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 46)" onmouseout="hideTip()" /></td>
            <td><a href="OutstandingDiscountListReportPage.aspx" target="_self">Outstanding UK Discount Claim List Report</a></td>
        </tr>
        <tr id="row_UKClaimAuditLogReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 34)" onmouseout="hideTip()" /></td>
            <td><a href="UKClaimAuditLogReport.aspx" target="_self">Next Claim Audit Log Report</a></td>
        </tr>
        <tr id="row_MFRNQtyAnalysisReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 36)" onmouseout="hideTip()" /></td>
            <td><a href="MFRNQtyAnalysisReportPage.aspx" target="_self">Next Claim MFRN Qty Analysis Report</a></td>
        </tr>
     </tbody>
    <tbody runat="server" GroupName="Non-Trade Expense Reports" visible="false">
        <tr id="row_NonTradeExpenseStatementList" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 35)" onmouseout="hideTip()" /></td>
            <td><a href="NonTradeExpenseStatementList.aspx" target="_self">Non-Trade Expense Statement List</a></td>
        </tr>
        <tr id="row_CarbonFootprintReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 53)" onmouseout="hideTip()" /></td>
            <td><a href="CarbonFootprintReport.aspx" target="_self">Carbon Footprint Report</a></td>
        </tr>
     </tbody>
     <tbody id="Tbody1" runat="server" GroupName="NS-LED Reports" visible="false">
        <tr id="row_NSLEDProfitabilityReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 49)" onmouseout="hideTip()" /></td>
            <td><a href="NSLedProfitabilitiesReport.aspx" target="_self">NS-LED Profitabilities Report</a></td>
        </tr>
        <tr id="row_NSLedSalesInfoReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 50)" onmouseout="hideTip()" /></td>
            <td><a href="NSLedSalesInfoReport.aspx" target="_self">NS-LED Sales Info Report</a></td>
        </tr>
        <tr id="row_NSLedSalesInfoReport_Finance" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 50)" onmouseout="hideTip()" /></td>
            <td><a href="NSLedSalesInfoReport_Finance.aspx" target="_self">NS-LED Sales Info Report (Finance)</a></td>
        </tr>
        <tr id="row_NSLedSellThruHistoryReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 51)" onmouseout="hideTip()" /></td>
            <td><a href="NSLedSellThruHistory.aspx" target="_self">NS-LED Sell Thru History Report</a></td>
        </tr>
        <tr id="row_NSLedActualSalesSummaryReport" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 54)" onmouseout="hideTip()" /></td>
            <td><a href="NSLedActualSalesSummaryReport.aspx" target="_self">NS-LED Actual Sales Summary Report</a></td>
        </tr>
        <tr id="row_NSLedSalesInfoReport_FinanceAdj" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 50)" onmouseout="hideTip()" /></td>
            <td><a href="NSLedSalesInfoReport_FinanceAdj.aspx" target="_self">NS-LED Sales Info Report (Finance With Adjustment - Not Yet Complete)</a></td>
        </tr>

        <tr id="row_NSLedSalesInfoReport_Finance_TBC" runat="server" visible="false">
            <td>&nbsp;</td>
            <td><img src="../images/btn_view.gif" alt="" onmouseover="doTooltip(event, 50)" onmouseout="hideTip()" /></td>
            <td><a href="NSLedSalesInfoReport_Finance_TBC.aspx" target="_self">NS-LED Sales Info Report (Finance TBC)</a></td>
        </tr>
        
      </tbody>

</table>


</asp:Content>
