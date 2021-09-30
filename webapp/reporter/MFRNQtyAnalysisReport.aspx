<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MFRNQtyAnalysisReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.MFRNQtyAnalysisReport" EnableViewState="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<HTML xmlns:v="urn:schemas-microsoft-com:vml" xmlns:x="urn:schemas-microsoft-com:office:excel"
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
	        TD.lockedLeftGrand { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: Yellow; TEXT-ALIGN: left }
	        TD.lockedLeftM { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: Aqua; TEXT-ALIGN: left }
	        TD.lockedLeftPC { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR:Silver; TEXT-ALIGN: left }
	        TD.locked { VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; TEXT-ALIGN: center }
	        TD.Header { FONT-WEIGHT: bold; Z-INDEX: 10; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; TEXT-ALIGN: center }
	        TD.lockedHeader { FONT-WEIGHT: bold; Z-INDEX: 99; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: #bedede; TEXT-ALIGN: center }
	        TD.lockedHeaderLeft { FONT-WEIGHT: bold; Z-INDEX: 99; VERTICAL-ALIGN: middle; CURSOR: default; COLOR: #000000; POSITION: relative; BACKGROUND-COLOR: #bedede; TEXT-ALIGN: left }	        
	
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
						<x:Name>MFRNQtyAnalysisList</x:Name>
						<x:WorksheetOptions>
							<x:FitToPage />
							<x:Print>
								<x:FitHeight>100</x:FitHeight>
								<x:ValidPrinterInfo />
								<x:PaperSizeIndex>8</x:PaperSizeIndex>
								<x:HorizontalResolution>600</x:HorizontalResolution>
								<x:VerticalResolution>600</x:VerticalResolution>
							</x:Print>
							<x:Selected />
						</x:WorksheetOptions>
					</x:ExcelWorksheet>
				</x:ExcelWorksheets>
			</x:ExcelWorkbook>
            <x:ExcelName>
                <x:Name>Print_Titles</x:Name>
                <x:SheetIndex>1</x:SheetIndex>
                <x:Formula>=MFRNQtyAnalysis!$1:$3</x:Formula>
            </x:ExcelName>                
		</xml>
    </head>
<body>
    <form id="form1" runat="server">
    <table id="Table0" cellspacing="0" cellpadding="0" border="1" style="border-style: none">
    <tr>
        <td style="text-align:left; font-size:larger; border-style:none; vertical-align:top; font-weight:bold;" nowrap width="70">NEXT SOURCING</td>
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;font-size:larger;" width="50" nowrap>
            <asp:label id="lblPrintTime" runat="server" EnableViewState="False"></asp:label>
        </td>
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
        <td style="border-style:none;" width="50" />
    </tr>
    <tr>
        <td style="text-align:left; font-size:larger; border-style:none; vertical-align:top; font-weight:bold;" nowrap>No of Pieces per UK Debit Note re MFRN</td>
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="text-align:left; font-size:larger; border-style:none;" nowrap>
            <asp:label id="lblPrintUser" runat="server" EnableViewState="False"></asp:label>
        </td>
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
    </tr>

    <tr id="tr_ReportCode" runat="server">
        <td style="text-align:left; font-weight:bold; border-style:none;" nowrap><asp:Label runat="server" ID="lbl_ReportCode" /></td>
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
        <td style="border-style:none;" />
    </tr>

    <asp:Repeater ID="repUKClaim" runat="server" 
            onitemdatabound="repUKClaim_ItemDataBound" EnableViewState="false">
        <ItemTemplate>
        <tr id="trTotal" runat="server">
            <td class="lockedHeaderLeft"><b>Total</b></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN1Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN2Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN3Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN4Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN5Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN6Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN7Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN8Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN9Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN10Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN11Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN12Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN13Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN14Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN15Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN16Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN17Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN18Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN19Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN20Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN21Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN22Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN23Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN24Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN25Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN26Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN31Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN36Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN41Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN46Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN51Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblNALLTotal" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblNFTotal" /></td>
        </tr>

        <tr id="trEmpty" runat="server">
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
        </tr>

        <tr runat="server" id="trHeaderOne">
            <td style="border-style:none;"><b><asp:Label runat="server" ID="lblPeriod" /></b></td>
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:solid;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" nowrap>No of Pieces per UK Debit Note</td>
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:none;background-color:yellow;" />
            <td class="lockedHeader" style="border-bottom-style:solid;border-top-style:solid;border-left-style:none;border-right-style:solid;background-color:yellow;" />
        </tr>
        <tr runat="server" id="trHeaderTwo">
            <td class="lockedHeader">Office</td>
            <td class="lockedHeader">1</td>
            <td class="lockedHeader">2</td>
            <td class="lockedHeader">3</td>
            <td class="lockedHeader">4</td>
            <td class="lockedHeader">5</td>
            <td class="lockedHeader">6</td>
            <td class="lockedHeader">7</td>
            <td class="lockedHeader">8</td>
            <td class="lockedHeader">9</td>
            <td class="lockedHeader">10</td>
            <td class="lockedHeader">11</td>
            <td class="lockedHeader">12</td>
            <td class="lockedHeader">13</td>
            <td class="lockedHeader">14</td>
            <td class="lockedHeader">15</td>
            <td class="lockedHeader">16</td>
            <td class="lockedHeader">17</td>
            <td class="lockedHeader">18</td>
            <td class="lockedHeader">19</td>
            <td class="lockedHeader">20</td>
            <td class="lockedHeader">21</td>
            <td class="lockedHeader">22</td>
            <td class="lockedHeader">23</td>
            <td class="lockedHeader">24</td>
            <td class="lockedHeader">25</td>
            <td class="lockedHeader">26-30</td>
            <td class="lockedHeader">31-35</td>
            <td class="lockedHeader">36-40</td>
            <td class="lockedHeader">41-45</td>
            <td class="lockedHeader">46-50</td>
            <td class="lockedHeader">&gt;50</td>
            <td class="lockedHeader">Total</td>
            <td class="lockedHeader" style="mso-number-format:\@">1-25</td>
        </tr>

        <tr id="trNormal" runat="server">
            <td class="lockedLeft"><asp:Label ID="lbl_Office" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N1" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N2" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N3" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N4" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N5" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N6" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N7" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N8" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N9" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N10" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N11" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N12" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N13" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N14" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N15" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N16" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N17" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N18" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N19" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N20" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N21" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N22" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N23" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N24" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N25" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N26" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N31" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N36" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N41" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N46" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_N51" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_NALL" runat="server" /></td>
            <td class="lockedLeft"><asp:Label ID="lbl_NF" runat="server" /></td>
        </tr>

        </ItemTemplate>
        <FooterTemplate>
        <tr id="trFooter" runat="server">
            <td class="lockedHeaderLeft"><b>Total</b></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN1Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN2Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN3Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN4Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN5Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN6Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN7Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN8Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN9Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN10Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN11Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN12Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN13Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN14Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN15Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN16Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN17Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN18Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN19Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN20Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN21Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN22Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN23Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN24Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN25Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN26Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN31Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN36Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN41Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN46Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblN51Total" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblNALLTotal" /></td>
            <td class="lockedHeaderLeft"><asp:Label runat="server" ID="lblNFTotal" /></td>
        </tr>
        
        </FooterTemplate>
    </asp:Repeater>
        <tr>
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
        </tr>
        <tr>
            <td style="text-align:left; font-size:larger; border-style:none; vertical-align:top; font-weight:bold;" nowrap>Note : Excludes debit notes cancelled</td>
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
            <td style="border-style:none;" />
        </tr>
        </table>


    </form>
</body>
</html>
