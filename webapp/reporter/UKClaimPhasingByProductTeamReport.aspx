<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UKClaimPhasingByProductTeamReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.UKClaimPhasingByProductTeamReport" EnableViewState="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns:v="urn:schemas-microsoft-com:vml" xmlns:x="urn:schemas-microsoft-com:office:excel"
	xmlns:o="urn:schemas-microsoft-com:office:office">
    <head runat="server">
		<title>ISAM</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta content="Excel.Sheet" name="ProgId"/>
		<style type="text/css">
		    @page {margin: .2in .2in .2in .2in; mso-header-margin: .2in; mso-footer-margin: 2in; mso-page-orientation: landscape; }
	        TD.NegativeFormat{border-bottom-style:none; border-top-style:none; mso-style-parent:style0;mso-number-format:"_\(* \#\,\#\#0_\)\;_\(* \\\(\#\,\#\#0\\\)\;_\(* \0022-\0022_\)\;_\(\@_\)";text-align:right;}
            TD.Percent{border-bottom-style:none; border-top-style:none; mso-number-format:"0\.0%";text-align:right;}
            TD.Number{ border-bottom-style:none; border-top-style:none; mso-style-parent:style0; mso-number-format:"_\(* \#\,\#\#0_\)\;_\(* \\\(\#\,\#\#0\\\)\;_\(* \0022-\0022_\)\;_\(\@_\)";text-align:right;}
	        TD.locked { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: #ffffff; }
	        TD.lockedAlt { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: Lavender; }
	        TD.Header { FONT-WEIGHT: bold; Z-INDEX: 10; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; TEXT-ALIGN: center }
	        TD.Footer { FONT-WEIGHT: bold; Z-INDEX: 10; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; TEXT-ALIGN: center }
	        TD.lockedHeader { FONT-WEIGHT: bold; Z-INDEX: 99; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: LightSteelBlue; TEXT-ALIGN: center; height : 35px; }
		    TD.subTotal { background-color : #ffffff; font-style:italic; font-weight:bold;}
		    TD.subTotalAlt { background-color : Lavender; font-style:italic; font-weight:bold;}		    
		    TD.office { background-color:Gainsboro; font-size:larger; font-style:italic; font-weight:bold;}
		    TD.groupHeading { background-color:LightSteelBlue; font-size:larger; font-style:italic; font-weight:bold;}
	        TD.Desc{ border-bottom-style:none; border-right-style:none; border-top-style:none; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: #ffffff; TEXT-ALIGN: left }
	        TD.yearHeader { background-color:LightSteelBlue; font-size:larger; font-style:italic; font-weight:bold;}
	        TD.SpaceCol{border-style:none;}
	
	        .nonEditText { FONT-SIZE: 10px; FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif }
            .gridTitle {
	            FONT-WEIGHT: normal; FONT-SIZE: 13px; FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif; background-color: #bedede; color: #990000; TEXT-ALIGN: left
            }

            .gridItem {
	            FONT-WEIGHT: normal; FONT-SIZE: 13px; COLOR: #000000; FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif; BACKGROUND-COLOR: #ffffff;
            }
            .gridAltItem {
	            FONT-WEIGHT: normal; FONT-SIZE: 13px; COLOR: #000000; FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif; BACKGROUND-COLOR: whitesmoke;
            }
	        
		</style>

		<xml>
			<o:OfficeDocumentSettings>
				<o:Colors>
					<o:Color>
						<o:Index>23</o:Index>
						<o:RGB>#dddddd</o:RGB>
					</o:Color>
				</o:Colors>
			</o:OfficeDocumentSettings>
		</xml>
        <xml>
			<x:ExcelWorkbook>
				<x:ExcelWorksheets>
					<x:ExcelWorksheet>
						<x:Name>UKClaimPhasingReport</x:Name>
						<x:WorksheetOptions>
							<x:FitToPage />
							<x:Print>
								<x:FitHeight>100</x:FitHeight>
								<x:ValidPrinterInfo />
								<x:PaperSizeIndex>9</x:PaperSizeIndex>
								<x:HorizontalResolution>600</x:HorizontalResolution>
								<x:VerticalResolution>600</x:VerticalResolution>
							</x:Print>
							<x:Selected />
						</x:WorksheetOptions>
					</x:ExcelWorksheet>
				</x:ExcelWorksheets>
			</x:ExcelWorkbook>
		</xml>
    </head>
<body>
    <form id="form1" runat="server">
    <table id="Table0" cellspacing="0" cellpadding="0" border="1" style="border-style: none">
    <tr>
        <td id="tdReportHeaderRow1" runat="server" colspan="14" style="text-align:left; font-size:larger; border-style:none; vertical-align:top; font-weight:bold;">NEXT SOURCING</td>
        
        <td colspan="3" style="text-align:left; font-size:larger; border-style:none;">
            <asp:label id="lblPrintTime" runat="server" EnableViewState="False"></asp:label>
        </td>
    </tr>
    <tr>
        <td colspan="17" style="text-align:center; font-weight:bold; border-style:none;">
           Next Claim By Phasing Report - <asp:Label runat="server" ID="lbl_ReportType"></asp:Label>
        </td>
    </tr>
    <tr id="tr_ReportCode" runat="server">
        <td style="text-align:left; font-weight:bold; border-style:none;" colspan="17">Report Code : <asp:Label runat="server" ID="lbl_ReportCode" /></td>
    </tr>
    <tr>
        <td style="text-align:left; font-weight:bold; border-style:none;" colspan="17">Office : <asp:Label runat="server" ID="lbl_Office" /></td>
    </tr>
    <tr>
        <td style="text-align:left; font-weight:bold; border-style:none;" colspan="17">Period : <asp:Label runat="server" ID="lbl_Period" /></td>
    </tr>
    <tr>
        <td style="text-align:left; font-weight:bold; border-style:none;" colspan="8">Supplier : <asp:Label runat="server" ID="lbl_Supplier" /></td>
        <td style="text-align:right; color:green;border-style:none; font-size:smaller; font-weight:bold;" colspan="9">Note: Amount : Next D/N Amount (Status: SUBMITTED, SIGNED_OFF, DEBIT_NOTE_TO_SUPPLIER)</td>
    </tr>
    <tr>
        <td colspan="17" style="text-align:right; font-style:italic; border-style:none;"/>
    </tr>
    <tr style="height:80px;">
        <td class="lockedHeader" style="width:300px; text-align:center;">Product Team</td>
        <td class="lockedHeader" style="width:80px; text-align:center;">Currency</td>
        <td class="lockedHeader" style="width:120px; text-align:center;">Claim Type</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P1</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P2</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P3</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P4</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P5</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P6</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P7</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P8</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P9</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P10</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P11</td>
        <td class="lockedHeader" style="width:90px; text-align:center;">P12</td>
        <td class="lockedHeader" style="width:110px; text-align:center;">Total</td>
        <td class="lockedHeader" style="width:100px; text-align:center;">Latest Shipment Date</td>
    </tr>
    <asp:Repeater ID="repUKClaim" runat="server" 
            onitemdatabound="repUKClaim_ItemDataBound">
        <ItemTemplate>
        <tr id="trSubGroupBlankLine" runat="server" >
            <td colspan="17" >&nbsp;</td>
        </tr>
        <tr id="trOffice" runat="server">
            <td colspan="17" class="office" style="text-align:left;">Office : <asp:Label ID="lbl_office" runat="server" /></td>
        </tr>
       <tr id="trSubTotal" runat="server">
            <td class="subTotal" id="tdSubProductTeam" runat="server"><asp:Label ID="lbl_SubProductTeam" runat="server" /></td>
            <td class="subTotal" id="tdSubCurrency" runat="server"><asp:Label ID="lbl_SubCurrency" runat="server" /></td>
            <td class="subTotal" id="tdSubClaimType" runat="server" style="text-align:left; font-style:italic;"><asp:Label ID="lbl_SubClaimType" runat="server" /></td>
            <td class="subTotal" id="tdSubP01" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP01" runat="server" /></td>
            <td class="subTotal" id="tdSubP02" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP02" runat="server" /></td>
            <td class="subTotal" id="tdSubP03" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP03" runat="server" /></td>
            <td class="subTotal" id="tdSubP04" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP04" runat="server" /></td>
            <td class="subTotal" id="tdSubP05" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP05" runat="server" /></td>
            <td class="subTotal" id="tdSubP06" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP06" runat="server" /></td>
            <td class="subTotal" id="tdSubP07" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP07" runat="server" /></td>
            <td class="subTotal" id="tdSubP08" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP08" runat="server" /></td>
            <td class="subTotal" id="tdSubP09" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP09" runat="server" /></td>
            <td class="subTotal" id="tdSubP10" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP10" runat="server" /></td>
            <td class="subTotal" id="tdSubP11" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP11" runat="server" /></td>
            <td class="subTotal" id="tdSubP12" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP12" runat="server" /></td>
            <td class="subTotal" id="tdSubTotal" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubTotal" runat="server" /></td>
            <td class="subTotal" id="tdSubShipmentDate" runat="server"><asp:Label ID="lbl_SubShipmentDate" runat="server" /></td>
        </tr>
        <tr id="trNormal" runat="server">
            <td class="locked" id="tdProductTeam" runat="server" style="text-align:left;"><asp:Label ID="lbl_ProductTeam" runat="server" /></td>
            <td class="locked" id="tdCurrency" runat="server" style="text-align:left;"><asp:Label ID="lbl_Currency" runat="server" /></td>
            <td class="locked" id="tdClaimType" runat="server" style="text-align:left;"><asp:Label ID="lbl_ClaimType" runat="server" /></td>
            <td class="locked" id="tdP01" runat="server" style="text-align:center;"><asp:Label ID="lbl_P01" runat="server" /></td>
            <td class="locked" id="tdP02" runat="server" style="text-align:center;"><asp:Label ID="lbl_P02" runat="server" /></td>
            <td class="locked" id="tdP03" runat="server" style="text-align:center;"><asp:Label ID="lbl_P03" runat="server" /></td>
            <td class="locked" id="tdP04" runat="server" style="text-align:center;"><asp:Label ID="lbl_P04" runat="server" /></td>
            <td class="locked" id="tdP05" runat="server" style="text-align:center;"><asp:Label ID="lbl_P05" runat="server" /></td>
            <td class="locked" id="tdP06" runat="server" style="text-align:center;"><asp:Label ID="lbl_P06" runat="server" /></td>
            <td class="locked" id="tdP07" runat="server" style="text-align:center;"><asp:Label ID="lbl_P07" runat="server" /></td>
            <td class="locked" id="tdP08" runat="server" style="text-align:center;"><asp:Label ID="lbl_P08" runat="server" /></td>
            <td class="locked" id="tdP09" runat="server" style="text-align:center;"><asp:Label ID="lbl_P09" runat="server" /></td>
            <td class="locked" id="tdP10" runat="server" style="text-align:center;"><asp:Label ID="lbl_P10" runat="server" /></td>
            <td class="locked" id="tdP11" runat="server" style="text-align:center;"><asp:Label ID="lbl_P11" runat="server" /></td>
            <td class="locked" id="tdP12" runat="server" style="text-align:center;"><asp:Label ID="lbl_P12" runat="server" /></td>
            <td class="locked" id="tdTotal" runat="server" style="text-align:center;"><asp:Label ID="lbl_Total" runat="server" /></td>
            <td class="locked" id="tdShipmentDate" runat="server" style="text-align:center;"><asp:Label ID="lbl_ShipmentDate" runat="server" /></td>
        </tr>
        <tr id="trGroupFooter" runat="server">
            <td class="subTotal" id="tdGfDesc" runat="server" style="text-align:Right; font-style:italic;" colspan="3"><asp:Label ID="lbl_GfDesc" runat="server" /></td>
            <td class="subTotal" id="tdGfP01" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP01" runat="server" /></td>
            <td class="subTotal" id="tdGfP02" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP02" runat="server" /></td>
            <td class="subTotal" id="tdGfP03" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP03" runat="server" /></td>
            <td class="subTotal" id="tdGfP04" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP04" runat="server" /></td>
            <td class="subTotal" id="tdGfP05" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP05" runat="server" /></td>
            <td class="subTotal" id="tdGfP06" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP06" runat="server" /></td>
            <td class="subTotal" id="tdGfP07" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP07" runat="server" /></td>
            <td class="subTotal" id="tdGfP08" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP08" runat="server" /></td>
            <td class="subTotal" id="tdGfP09" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP09" runat="server" /></td>
            <td class="subTotal" id="tdGfP10" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP10" runat="server" /></td>
            <td class="subTotal" id="tdGfP11" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP11" runat="server" /></td>
            <td class="subTotal" id="tdGfP12" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP12" runat="server" /></td>
            <td class="subTotal" id="tdGfTotal" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfTotal" runat="server" /></td>
            <td class="subTotal" id="tdGfShipmentDate" runat="server"><asp:Label ID="lbl_GfShipmentDate" runat="server" /></td>
        </tr>
        <tr id="trBlankLine" runat="server" >
            <td colspan="17" >&nbsp;</td>
        </tr>

        </ItemTemplate>
        <FooterTemplate>
        <tr id="trFooter" runat="server">
            <td class="Footer" id="tdSubProductTeam" runat="server"><asp:Label ID="lbl_SubProductTeam" runat="server" /></td>
            <td class="Footer" id="tdSubCurrency" runat="server"><asp:Label ID="lbl_SubCurrency" runat="server" /></td>
            <td class="Footer" id="tdSubClaimType" runat="server" style="text-align:left;"></td>
            <td class="Footer" id="tdSubP01" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP01" runat="server" /></td>
            <td class="Footer" id="tdSubP02" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP02" runat="server" /></td>
            <td class="Footer" id="tdSubP03" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP03" runat="server" /></td>
            <td class="Footer" id="tdSubP04" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP04" runat="server" /></td>
            <td class="Footer" id="tdSubP05" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP05" runat="server" /></td>
            <td class="Footer" id="tdSubP06" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP06" runat="server" /></td>
            <td class="Footer" id="tdSubP07" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP07" runat="server" /></td>
            <td class="Footer" id="tdSubP08" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP08" runat="server" /></td>
            <td class="Footer" id="tdSubP09" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP09" runat="server" /></td>
            <td class="Footer" id="tdSubP10" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP10" runat="server" /></td>
            <td class="Footer" id="tdSubP11" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP11" runat="server" /></td>
            <td class="Footer" id="tdSubP12" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP12" runat="server" /></td>
            <td class="Footer" id="tdSubTotal" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubTotal" runat="server" /></td>
            <td class="Footer" id="tdSubShipmentDate" runat="server"><asp:Label ID="lbl_SubShipmentDate" runat="server" /></td>
        </tr>
        </FooterTemplate>
    </asp:Repeater>

    <asp:Repeater ID="repUKClaimSummary" runat="server" onitemdatabound="repUKClaimSummary_ItemDataBound">
        <ItemTemplate>
        <tr id="trSubTotal" runat="server">
            <td class="subTotal" id="tdSubGroup" runat="server" colspan="2"><asp:Label ID="lbl_SubGroup" runat="server" /></td>
            <td class="subTotal" id="tdSubCurrency" runat="server"><asp:Label ID="lbl_SubCurrency" runat="server" /></td>
            <td class="subTotal" id="tdSubP01" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP01" runat="server" /></td>
            <td class="subTotal" id="tdSubP02" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP02" runat="server" /></td>
            <td class="subTotal" id="tdSubP03" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP03" runat="server" /></td>
            <td class="subTotal" id="tdSubP04" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP04" runat="server" /></td>
            <td class="subTotal" id="tdSubP05" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP05" runat="server" /></td>
            <td class="subTotal" id="tdSubP06" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP06" runat="server" /></td>
            <td class="subTotal" id="tdSubP07" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP07" runat="server" /></td>
            <td class="subTotal" id="tdSubP08" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP08" runat="server" /></td>
            <td class="subTotal" id="tdSubP09" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP09" runat="server" /></td>
            <td class="subTotal" id="tdSubP10" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP10" runat="server" /></td>
            <td class="subTotal" id="tdSubP11" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP11" runat="server" /></td>
            <td class="subTotal" id="tdSubP12" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubP12" runat="server" /></td>
            <td class="subTotal" id="tdSubTotal" runat="server" style="text-align:center;"><asp:Label ID="lbl_SubTotal" runat="server" /></td>
        </tr>
        <tr id="trBlankHeading" runat="server">
            <td colspan="16"><asp:Label ID="lbl_BlankHeading" runat="server" /><br /><br /><br /></td>
        </tr>
        <tr id="trGroupHeading" runat="server">
            <td colspan="16" class="groupHeading" style="text-align:left;"><asp:Label ID="lbl_GroupHeading" runat="server" /></td>
        </tr>
        <tr id="trNormal" runat="server">
            <td class="locked" id="tdGroup" runat="server" style="text-align:left;" colspan="2"><asp:Label ID="lbl_Group" runat="server" /></td>
            <td class="locked" id="tdCurrency" runat="server" style="text-align:left;"><asp:Label ID="lbl_Currency" runat="server" /></td>
            <td class="locked" id="tdP01" runat="server" style="text-align:center;"><asp:Label ID="lbl_P01" runat="server" /></td>
            <td class="locked" id="tdP02" runat="server" style="text-align:center;"><asp:Label ID="lbl_P02" runat="server" /></td>
            <td class="locked" id="tdP03" runat="server" style="text-align:center;"><asp:Label ID="lbl_P03" runat="server" /></td>
            <td class="locked" id="tdP04" runat="server" style="text-align:center;"><asp:Label ID="lbl_P04" runat="server" /></td>
            <td class="locked" id="tdP05" runat="server" style="text-align:center;"><asp:Label ID="lbl_P05" runat="server" /></td>
            <td class="locked" id="tdP06" runat="server" style="text-align:center;"><asp:Label ID="lbl_P06" runat="server" /></td>
            <td class="locked" id="tdP07" runat="server" style="text-align:center;"><asp:Label ID="lbl_P07" runat="server" /></td>
            <td class="locked" id="tdP08" runat="server" style="text-align:center;"><asp:Label ID="lbl_P08" runat="server" /></td>
            <td class="locked" id="tdP09" runat="server" style="text-align:center;"><asp:Label ID="lbl_P09" runat="server" /></td>
            <td class="locked" id="tdP10" runat="server" style="text-align:center;"><asp:Label ID="lbl_P10" runat="server" /></td>
            <td class="locked" id="tdP11" runat="server" style="text-align:center;"><asp:Label ID="lbl_P11" runat="server" /></td>
            <td class="locked" id="tdP12" runat="server" style="text-align:center;"><asp:Label ID="lbl_P12" runat="server" /></td>
            <td class="locked" id="tdTotal" runat="server" style="text-align:center;"><asp:Label ID="lbl_Total" runat="server"/></td>
        </tr>
        <tr id="trGroupFooter" runat="server">
            <td class="subTotal" id="tdGfDesc" runat="server"><asp:Label ID="lbl_GfDesc" runat="server" colspan="2" /></td>
            <td class="subTotal" id="tdGfP01" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP01" runat="server" /></td>
            <td class="subTotal" id="tdGfP02" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP02" runat="server" /></td>
            <td class="subTotal" id="tdGfP03" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP03" runat="server" /></td>
            <td class="subTotal" id="tdGfP04" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP04" runat="server" /></td>
            <td class="subTotal" id="tdGfP05" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP05" runat="server" /></td>
            <td class="subTotal" id="tdGfP06" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP06" runat="server" /></td>
            <td class="subTotal" id="tdGfP07" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP07" runat="server" /></td>
            <td class="subTotal" id="tdGfP08" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP08" runat="server" /></td>
            <td class="subTotal" id="tdGfP09" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP09" runat="server" /></td>
            <td class="subTotal" id="tdGfP10" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP10" runat="server" /></td>
            <td class="subTotal" id="tdGfP11" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP11" runat="server" /></td>
            <td class="subTotal" id="tdGfP12" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfP12" runat="server" /></td>
            <td class="subTotal" id="tdGfTotal" runat="server" style="text-align:center;"><asp:Label ID="lbl_GfTotal" runat="server"/></td>
        </tr>
        <tr id="trSummary" runat="server">
            <td class="locked" id="tdSmyGroup" runat="server" style="text-align:left;" colspan="2"><asp:Label ID="lbl_SmyGroup" runat="server" /></td>
            <td class="locked" id="tdSmyCurrency" runat="server" style="text-align:left;"><asp:Label ID="lbl_SmyCurrency" runat="server" /></td>
            <td class="locked" id="tdSmyP01" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP01" runat="server" /></td>
            <td class="locked" id="tdSmyP02" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP02" runat="server" /></td>
            <td class="locked" id="tdSmyP03" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP03" runat="server" /></td>
            <td class="locked" id="tdSmyP04" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP04" runat="server" /></td>
            <td class="locked" id="tdSmyP05" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP05" runat="server" /></td>
            <td class="locked" id="tdSmyP06" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP06" runat="server" /></td>
            <td class="locked" id="tdSmyP07" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP07" runat="server" /></td>
            <td class="locked" id="tdSmyP08" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP08" runat="server" /></td>
            <td class="locked" id="tdSmyP09" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP09" runat="server" /></td>
            <td class="locked" id="tdSmyP10" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP10" runat="server" /></td>
            <td class="locked" id="tdSmyP11" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP11" runat="server" /></td>
            <td class="locked" id="tdSmyP12" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyP12" runat="server" /></td>
            <td class="locked" id="tdSmyTotal" runat="server" style="text-align:center;"><asp:Label ID="lbl_SmyTotal" runat="server"/></td>
       </tr>
        <tr id="trBlankLine" runat="server" >
            <td colspan="16"  >&nbsp;</td>
        </tr>

        </ItemTemplate>
     </asp:Repeater>


        <tr ID="trCurrentYearTopMargin" runat="server"><td style="border:none; height:50px;" colspan="16" /></tr>
        <tr ID="trCurrentYearHeading" runat="server"><td class="yearHeader" colspan="16">Current Year</td></tr>
        <tr>
            <td class="locked" style="text-align:left;"><b><asp:Label ID="lbl_CurrentYearItem" runat="server">Overall</asp:Label></b></td>
            <td class="locked" style="text-align:left;">USD</td>
            <td class="locked" style="text-align:left;">Rework</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_ReworkTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Reject</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_RejectTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">MFRN</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_MFRNTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">CFS</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CFSTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Safety Issue</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_SafetyTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Audit Fee</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_AuditTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Fabric Test</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FabricTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Penalty Charge</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_PenaltyTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">QCC</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_QCCTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">CHB</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_CHBTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">GB Testing Charge</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_GBTestTotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">FIRA Test</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRAP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_FIRATotal" runat="server" /></td>
        </tr>
        <tr>
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Others</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_OthersTotal" runat="server" /></td>
        </tr>


        <tr>
            <td class="subTotalAlt" colspan="2"/>
            <td class="subTotalAlt" style="text-align:left;">Grand-total</td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP1" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP2" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP3" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP4" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP5" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP6" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP7" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP8" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP9" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP10" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP11" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandP12" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandTotal" runat="server" /></td>
        </tr>
        <tr ID="trCurrentYearClaimRatioTopMargin" runat="server"><td style="border:none" colspan="16"></td></tr>
        <tr ID="trCurrentYearClaimRatioPaidAmtBySupplier" runat="server">
            <td class="locked" style="text-align:left;" ID="tdCurrentYearClaimRatioItem" runat="server"><asp:label ID="trCurrentYearClaimRatioItem" runat="server"> <b>Claim Ratio</b></asp:label></td>
            <td class="locked" style="text-align:left;" ID="tdCurrentYearClaimRatioCurrency" runat="server"><asp:label ID="trCurrentYearClaimRatioCurrency" runat="server">USD</asp:label></td>
            <td class="locked" style="text-align:left;">$ Paid By Supplier</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P1PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P2PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P3PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P4PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P5PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P6PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P7PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P8PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P9PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P10PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P11PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P12PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_TotalPaidBySupplierAmt" runat="server" /></td>
        </tr>
        <tr ID="trCurrentYearClaimRatioPaidPercentBySupplier" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">% Paid By Supplier</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P1PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P2PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P3PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P4PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P5PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P6PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P7PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P8PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P9PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P10PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P11PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P12PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_TotalPaidBySupplierPct" runat="server" /></td>
        </tr>
        <tr ID="trCurrentYearClaimRatioPaidAmtByNS" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">$ Paid By NS</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P1PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P2PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P3PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P4PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P5PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P6PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P7PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P8PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P9PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P10PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P11PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P12PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_TotalPaidByNSAmt" runat="server" /></td>
        </tr>
        <tr ID="trCurrentYearClaimRatioPaidPercentByNS" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">% Paid By NS</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P1PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P2PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P3PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P4PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P5PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P6PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P7PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P8PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P9PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P10PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P11PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_P12PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_TotalPaidByNSPct" runat="server" /></td>
        </tr>
        <tr ID="trCurrentYearClaimRatioGrandTotal" runat="server">
            <td class="subTotalAlt" colspan="2"/>
            <td class="subTotalAlt" style="text-align:left;">Grand-total</td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP1" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP2" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP3" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP4" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP5" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP6" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP7" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP8" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP9" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP10" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP11" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioP12" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_GrandRatioTotal" runat="server" /></td>
        </tr>

 
        <tr ID="trLastYearTopMargin" runat="server"><td style="border:none" colspan="16"/></tr>
        <tr ID="trLastYearHeading" runat="server"><td class="yearHeader" colspan="16">Last Year</td></tr>
        <tr ID="trLastYearRework" runat="server">
            <td class="locked" style="text-align:left;"><b>Overall</b></td>
            <td class="locked" style="text-align:left;">USD</td>
            <td class="locked" style="text-align:left;">Rework</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_ReworkTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearReject" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Reject</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_RejectTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearMFRN" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">MFRN</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_MFRNTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearCFS" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">CFS</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CFSTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearSafetyIssue" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Safety Issue</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_SafetyTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearAuditFee" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Audit Fee</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_AuditTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearFabricTest" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Fabric Test</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FabricTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearPenaltyCharge" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">Penalty</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_PenaltyTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearQCC" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">QCC</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_QCCTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearCHB" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">CHB</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_CHBTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearGBTest" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">GB Testing Charge</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_GBTestTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearFIRA" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">GB Testing Charge</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRAP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_FIRATotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearOthers" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">GB Testing Charge</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP1" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP2" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP3" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP4" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP5" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP6" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP7" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP8" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP9" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP10" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP11" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersP12" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_OthersTotal" runat="server" /></td>
        </tr>

        <tr ID="trLastYearGrandTotal" runat="server">
            <td class="subTotalAlt" colspan="2"/>
            <td class="subTotalAlt" style="text-align:left;">Grand-total</td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP1" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP2" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP3" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP4" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP5" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP6" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP7" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP8" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP9" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP10" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP11" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandP12" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandTotal" runat="server" /></td>
        </tr>
        <tr ID="trLastYearClaimRatioTopMargin" runat="server"><td style="border:none" colspan="16"></td></tr>
        <tr ID="trLastYearClaimRatioPaidAmtBySupplier" runat="server">
            <td class="locked" style="text-align:left;"><b>Claim Ratio</b></td>
            <td class="locked" style="text-align:left;">USD</td>
            <td class="locked" style="text-align:left;">$ Paid By Supplier</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P1PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P2PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P3PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P4PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P5PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P6PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P7PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P8PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P9PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P10PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P11PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P12PaidBySupplierAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_TotalPaidBySupplierAmt" runat="server" /></td>
        </tr>
        <tr ID="trLastYearClaimRatioPaidPercentBySupplier" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">% Paid By Supplier</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P1PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P2PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P3PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P4PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P5PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P6PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P7PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P8PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P9PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P10PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P11PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P12PaidBySupplierPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_TotalPaidBySupplierPct" runat="server" /></td>
        </tr>
        <tr ID="trLastYearClaimRatioPaidAmtByNS" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">$ Paid By NS</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P1PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P2PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P3PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P4PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P5PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P6PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P7PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P8PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P9PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P10PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P11PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P12PaidByNSAmt" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_TotalPaidByNSAmt" runat="server" /></td>
        </tr>
        <tr ID="trLastYearClaimRatioPaidPercentByNS" runat="server">
            <td class="locked" colspan="2"/>
            <td class="locked" style="text-align:left;">% Paid By NS</td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P1PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P2PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P3PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P4PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P5PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P6PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P7PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P8PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P9PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P10PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P11PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_P12PaidByNSPct" runat="server" /></td>
            <td class="locked" style="text-align:center;"><asp:Label ID="lbl_LY_TotalPaidByNSPct" runat="server" /></td>
        </tr>
        <tr ID="trLastYearClaimRatioGrandTotal" runat="server">
            <td class="subTotalAlt" colspan="2"/>
            <td class="subTotalAlt" style="text-align:left;">Grand-total</td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP1" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP2" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP3" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP4" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP5" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP6" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP7" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP8" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP9" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP10" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP11" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioP12" runat="server" /></td>
            <td class="subTotalAlt" style="text-align:center;"><asp:Label ID="lbl_LY_GrandRatioTotal" runat="server" /></td>
        </tr>

    </table>
    </form>
</body>
</html>