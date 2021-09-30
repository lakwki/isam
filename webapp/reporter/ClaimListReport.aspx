<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClaimListReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.ClaimListReport" EnableViewState="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns:v="urn:schemas-microsoft-com:vml" xmlns:x="urn:schemas-microsoft-com:office:excel"
	xmlns:o="urn:schemas-microsoft-com:office:office">
    <head>
		<title>ISAM</title>
		<meta http-equiv="Content-Type" content="text/html;"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<meta content="Excel.Sheet" name="ProgId"/>
		<style type="text/css">
		    @page {margin: .2in .2in .2in .2in; mso-header-margin: .2in; mso-footer-margin: 2in; mso-page-orientation: landscape; }
	        TD.NegativeFormat{border-bottom-style:none; border-top-style:none; mso-style-parent:style0;mso-number-format:"_\(* \#\,\#\#0_\)\;_\(* \\\(\#\,\#\#0\\\)\;_\(* \0022-\0022_\)\;_\(\@_\)";text-align:right;}
            TD.Percent{border-bottom-style:none; border-top-style:none; mso-number-format:"0\.0%";text-align:right;}
            TD.Number{ border-bottom-style:none; border-top-style:none; mso-style-parent:style0; mso-number-format:"_\(* \#\,\#\#0_\)\;_\(* \\\(\#\,\#\#0\\\)\;_\(* \0022-\0022_\)\;_\(\@_\)";text-align:right;}

	        TD.ItemHeader { BORDER-RIGHT: #000000 1px solid; BORDER-TOP: #000000 1px solid; FONT-WEIGHT: bold; Z-INDEX: 10; VERTICAL-ALIGN: middle; BORDER-LEFT: #000000 1px solid; CURSOR: default; COLOR: #000000; BORDER-BOTTOM: #000000 1px solid; POSITION: relative; BACKGROUND-COLOR: #ffffff; TEXT-ALIGN: left }
	        TD.lockedLeft { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: #ffffff; TEXT-ALIGN: left }
	        TD.lockedLeftHighlight { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: #ffff00; TEXT-ALIGN: left }
	        TD.locked { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; TEXT-ALIGN: center }
	        TD.Header { FONT-WEIGHT: bold; Z-INDEX: 10; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; TEXT-ALIGN: center }
	        TD.lockedHeader { FONT-WEIGHT: bold; Z-INDEX: 99; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: #bedede; TEXT-ALIGN: center; solid: }
	
	        TD.Desc{ border-bottom-style:none; border-right-style:none; border-top-style:none; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: #ffffff; TEXT-ALIGN: left }
	
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
						<x:Name>UKClaimList</x:Name>
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
        <td id="tdReportHeaderRow1" runat="server" colspan="24" style="text-align:left; font-size:larger; border-style:none; vertical-align:top;">NEXT SOURCING LIMITED
        </td>
        
        <td colspan="2" style="text-align:left; font-size:larger; border-style:none;">
        <asp:label id="lblPrintTime" runat="server" EnableViewState="False"></asp:label>
        </td>
    </tr>
    <tr>
        <td colspan="25" style="text-align:center; font-weight:bold; border-style:none;">
           Next Claim List Report
        </td>
    </tr>
    <tr>
        <td colspan="25" style="text-align:right; font-style:italic; border-style:none;"/>
    </tr>
    <tr>
        <td class="lockedHeader" style="width:60px; text-align:left;" x:autofilter='all'>Office</td>
        <td class="lockedHeader" style="width:60px; text-align:left;" x:autofilter='all'>Handling Office</td>
        <td class="lockedHeader" style="width:60px; text-align:left;" x:autofilter='all'>Payment Office</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>Claim Type</td>
        <td class="lockedHeader" style="width:60px; text-align:left;" x:autofilter='all'>T5</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>Next D/N No.</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>Next D/N Date</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>Next D/N Rec'd Date</td>
        <td class="lockedHeader" style="width:80px; text-align:left;" x:autofilter='all'>Order Type</td>
        <td class="lockedHeader" style="width:300px; text-align:left;" x:autofilter='all'>Vendor</td>
        <td class="lockedHeader" style="width:300px; text-align:left;" x:autofilter='all'>Product Team</td>
        <td class="lockedHeader" style="width:70px; text-align:left;" x:autofilter='all'>Item No.</td>
        <td class="lockedHeader" style="width:80px; text-align:left;" x:autofilter='all'>Contract No.</td>
        <td class="lockedHeader" style="width:50px; text-align:left;" x:autofilter='all'>Ccy</td>
        <td class="lockedHeader" style="width:80px; text-align:left;" x:autofilter='all'>Amount</td>
        <td class="lockedHeader" style="width:80px; text-align:left;" x:autofilter='all'>Amount In USD</td>
        <td class="lockedHeader" style="width:70px; text-align:left;" x:autofilter='all'>Qty</td>
        <td class="lockedHeader" style="width:200px; text-align:center;" x:autofilter='all'>Status</td>
        <td class="lockedHeader" style="width:100px; text-align:center;" x:autofilter='all'>Form No.</td>
        <td class="lockedHeader" style="width:60px; text-align:center;" x:autofilter='all'>NS Cost %</td>
        <td class="lockedHeader" style="width:60px; text-align:left;" x:autofilter='all'>Vendor %</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>Ready For Settlement</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>Supplier D/N No.</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>Supplier D/N <br />Settlement Date</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>Supplier D/N <br />Date</td>
        <td class="lockedHeader" style="width:100px; text-align:left;" x:autofilter='all'>First Contract <br />Date</td>
    </tr>
    <asp:Repeater ID="repUKClaim" runat="server" 
            onitemdatabound="repUKClaim_ItemDataBound">
        <ItemTemplate>
        <tr>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_Office" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_HandlingOffice" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_PaymentOffice" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_ClaimType" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_T5" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_UKDNNo" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_UKDNDate" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_UKDNRecdDate" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_OrderType" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_Vendor" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_ProductTeam" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_ItemNo" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_ContractNo" runat="server" /></td>
            <td class="lockedLeft" style="text-align:left;"><asp:Label ID="lbl_Currency" runat="server" /></td>
            <td style="text-align:right;VERTICAL-ALIGN: middle"><asp:Label ID="lbl_Amount" runat="server" /></td>
            <td style="text-align:right;VERTICAL-ALIGN: middle"><asp:Label ID="lbl_AmountInUSD" runat="server" /></td>
            <td style="text-align:right;VERTICAL-ALIGN: middle"><asp:Label ID="lbl_Qty" runat="server" /></td>
            <td class="lockedLeft" style="text-align:center;"><asp:Label ID="lbl_Status" runat="server" /></td>
            <td class="lockedLeft" style="text-align:center;"><asp:Label ID="lbl_FormNo" runat="server" /></td>
            <td style="text-align:right;VERTICAL-ALIGN: middle"><asp:Label ID="lbl_NSRechargePercent" runat="server" /></td>
            <td style="text-align:right;VERTICAL-ALIGN: middle"><asp:Label ID="lbl_VendorRechargePercent" runat="server" /></td>
            <td class="lockedLeft" style="text-align:center;"><asp:Label ID="lbl_IsReadyForSettlement" runat="server" /></td>
            <td class="lockedLeft" style="text-align:center;"><asp:Label ID="lbl_SupplierDNNo" runat="server" /></td>
            <td class="lockedLeft" style="text-align:center;"><asp:Label ID="lbl_SupplierDNSettlementDate" runat="server" /></td>
            <td class="lockedLeft" style="text-align:center;"><asp:Label ID="lbl_SupplierDNDate" runat="server" /></td>
            <td class="lockedLeft" style="text-align:center;"><asp:Label ID="lbl_FirstContractDate" runat="server" /></td>
        </tr>
        </ItemTemplate>
    </asp:Repeater>
    </table>
    </form>
</body>
</html>