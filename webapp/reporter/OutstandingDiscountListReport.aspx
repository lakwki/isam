<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutstandingDiscountListReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.OutstandingDiscountListReport" EnableViewState="false" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns:v="urn:schemas-microsoft-com:vml" xmlns:x="urn:schemas-microsoft-com:office:excel"
xmlns:o="urn:schemas-microsoft-com:office:office">
<head>
    <title>ISAM</title>
    <meta http-equiv="Content-Type" content="text/html;" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <meta content="Excel.Sheet" name="ProgId" />
    <style type="text/css">
        @page {
            mso-footer-data: "Page &P of &N";
            margin: .2in .2in .5in .2in;
            mso-header-margin: .2in;
            mso-footer-margin: .2in;
            mso-page-orientation: landscape;
        }

        TD.NegativeFormat {
            border-bottom-style: none;
            border-top-style: none;
            mso-style-parent: style0;
            mso-number-format: "_\(* \#\,\#\#0_\)\;_\(* \\\(\#\,\#\#0\\\)\;_\(* \0022-\0022_\)\;_\(\@_\)";
            text-align: right;
        }

        TD.Percent {
            border-bottom-style: none;
            border-top-style: none;
            mso-number-format: "0\.0%";
            text-align: right;
        }

        TD.Number {
            border-bottom-style: none;
            border-top-style: none;
            mso-style-parent: style0;
            mso-number-format: "_\(* \#\,\#\#0_\)\;_\(* \\\(\#\,\#\#0\\\)\;_\(* \0022-\0022_\)\;_\(\@_\)";
            text-align: right;
        }

        TD.ItemHeader {
            BORDER-RIGHT: #000000 1px solid;
            BORDER-TOP: #000000 1px solid;
            FONT-WEIGHT: bold;
            Z-INDEX: 10;
            VERTICAL-ALIGN: middle;
            BORDER-LEFT: #000000 1px solid;
            CURSOR: default;
            COLOR: #000000;
            BORDER-BOTTOM: #000000 1px solid;
            POSITION: relative;
            BACKGROUND-COLOR: #ffffff;
            TEXT-ALIGN: left;
        }

        TD.lockedLeft {
            VERTICAL-ALIGN: middle;
            CURSOR: default;
            COLOR: #000000;
            POSITION: relative;
            BACKGROUND-COLOR: #ffffff;
            TEXT-ALIGN: left;
        }

        TD.lockedLeftGrand {
            VERTICAL-ALIGN: middle;
            CURSOR: default;
            COLOR: #000000;
            POSITION: relative;
            BACKGROUND-COLOR: Yellow;
            TEXT-ALIGN: left;
        }

        TD.lockedLeftM {
            VERTICAL-ALIGN: middle;
            CURSOR: default;
            COLOR: #000000;
            POSITION: relative;
            BACKGROUND-COLOR: Aqua;
            TEXT-ALIGN: left;
        }

        TD.lockedLeftPC {
            VERTICAL-ALIGN: middle;
            CURSOR: default;
            COLOR: #000000;
            POSITION: relative;
            BACKGROUND-COLOR: Silver;
            TEXT-ALIGN: left;
        }

        TD.locked {
            VERTICAL-ALIGN: middle;
            CURSOR: default;
            COLOR: #000000;
            POSITION: relative;
            TEXT-ALIGN: center;
        }

        TD.Header {
            FONT-WEIGHT: bold;
            Z-INDEX: 10;
            VERTICAL-ALIGN: middle;
            CURSOR: default;
            COLOR: #000000;
            POSITION: relative;
            TEXT-ALIGN: center;
        }

        TD.lockedHeader {
            FONT-WEIGHT: bold;
            Z-INDEX: 99;
            VERTICAL-ALIGN: middle;
            CURSOR: default;
            COLOR: #000000;
            POSITION: relative;
            BACKGROUND-COLOR: #bedede;
            TEXT-ALIGN: center;
        }

        TD.Desc {
            border-bottom-style: none;
            border-right-style: none;
            border-top-style: none;
            VERTICAL-ALIGN: middle;
            CURSOR: default;
            COLOR: #000000;
            POSITION: relative;
            BACKGROUND-COLOR: #ffffff;
            TEXT-ALIGN: left;
        }

        TD.SpaceCol {
            border-style: none;
        }

        .nonEditText {
            FONT-SIZE: 10px;
            FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif;
        }

        .gridTitle {
            FONT-WEIGHT: normal;
            FONT-SIZE: 13px;
            FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif;
            background-color: #bedede;
            color: #990000;
            TEXT-ALIGN: left;
        }

        .gridItem {
            FONT-WEIGHT: normal;
            FONT-SIZE: 13px;
            COLOR: #000000;
            FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif;
            BACKGROUND-COLOR: #ffffff;
        }

        .gridAltItem {
            FONT-WEIGHT: normal;
            FONT-SIZE: 13px;
            COLOR: #000000;
            FONT-FAMILY: Verdana, Geneva, Arial, Helvetica, sans-serif;
            BACKGROUND-COLOR: whitesmoke;
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
						<x:Name>OS_UKDiscountList</x:Name>
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
                <x:Formula>=OS_UKDiscountList!$1:$10</x:Formula>
            </x:ExcelName>                
		</xml>
</head>
<body>
    <form id="form1" runat="server">
        <table id="Table0" cellspacing="0" cellpadding="0" border="1" style="border-style: none">
            <tr>
                <td style="text-align: left; font-size: larger; border-style: none; vertical-align: top; font-weight: bold;" nowrap width="115">NEXT SOURCING</td>
                <td style="border-style: none;" width="115" />
                <td style="border-style: none;" width="95" />
                <td style="border-style: none;" width="115" />
                <td style="border-style: none;" width="230" />
                <td style="border-style: none;" width="90" />
                <td style="border-style: none;" width="90" />
                <td style="border-style: none;" width="95" />
                <td style="border-style: none;" width="45" />
                <td style="border-style: none;" width="45" />
                <td style="border-style: none;" width="100" />
                <td style="border-style: none;" width="100" />
                <td style="border-style: none;" width="10" />
                <td style="border-style: none;" width="85" />
                <td style="border-style: none;" width="85" />
                <td style="border-style: none;" width="85" />
                <td style="border-style: none;" width="85" />
                <td style="border-style: none;" width="85" />
                <td style="border-style: none;" width="10" />
                <td style="border-style: none;" width="105" />
                <td style="border-style: none;" width="90" />
                <td style="border-style: none;" width="105" />

                <td style="border-style: none; font-size: larger;" width="95" nowrap>
                    <asp:Label ID="lblPrintTime" runat="server" EnableViewState="False"></asp:Label>
                </td>

                <td style="border-style: none;" width="95" />
                <td style="border-style: none;" width="95" />
                <td style="border-style: none;" width="5" />
                <td style="border-style: none;" width="55" />
                <td style="border-style: none;" width="55" />
                <td style="border-style: none;" width="55" />
                <td style="border-style: none;" width="55" />
                <td style="border-style: none;" width="55" />
                <td style="border-style: none;" width="55" />
                <td style="border-style: none;" width="55" />
            </tr>
            <tr>
                <td style="text-align: left; font-size: larger; border-style: none; vertical-align: top; font-weight: bold;" nowrap>Outstanding Discount Claim List Report</td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />

                <td style="text-align: left; font-size: larger; border-style: none;" nowrap>
                    <asp:Label ID="lblPrintUser" runat="server" EnableViewState="False"></asp:Label>
                </td>

                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>

            <tr id="tr_ReportCode" runat="server">
                <td style="text-align: left; font-weight: bold; border-style: none;" nowrap>
                    <asp:Label runat="server" ID="lbl_ReportCode" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr>
                <td style="text-align: left; font-weight: bold; border-style: none;" nowrap>
                    <asp:Label runat="server" ID="lbl_Office" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <%--<tr id="tr_HandlingOffice" runat="server" style="display: none;">
                <td style="text-align: left; font-weight: bold; border-style: none;" nowrap>
                    <asp:Label runat="server" ID="lbl_HandlingOffice" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>--%>
            <%--        <tr>
                <td style="text-align: left; font-weight: bold; border-style: none;" nowrap>
                    <asp:Label runat="server" ID="lbl_OrderType" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>--%>
            <tr>
                <td style="text-align: left; font-weight: bold; border-style: none;" nowrap>
                    <asp:Label runat="server" ID="lblCutOffDate" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>

            <div runat="server" id="divDetailSection">
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none; text-align: center" nowrap>Aging Info (Per Next D/N Date)</td>
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
                <asp:Repeater ID="repUKClaim" runat="server"
                    OnItemDataBound="repUKClaim_ItemDataBound" EnableViewState="false">
                    <HeaderTemplate>
                        <tr>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Office</td>
                            <%--<td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Handling Office</td>--%>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Next D/N Date</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Next D/N Rec'd Date</td>
                            <%--<td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Claim Type</td>--%>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Debit Note No.</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Supplier</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Dept Code</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Item</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Contract</td>
                            <%--<td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Last Shipment Date</td>--%>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Ccy</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Qty</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Amount</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Amount In USD</td>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'></td>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>0 &lt; 30</td>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>31 &lt; 60</td>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>61 &lt; 90</td>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>91 &lt; 120</td>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>&gt; 120</td>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'></td>
                            <%--<td class="lockedHeader" style="text-align: center;" x:autofilter='all'>Comments From Technical</td>--%>
                            <%--<td class="lockedHeader" style="text-align: center;" x:autofilter='all'>Comments From A/C</td>--%>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>Status</td>
                            <%--<td class="lockedHeader" style="text-align: center;" x:autofilter='all'>T5 Code</td>--%>
                            <%--<td class="lockedHeader" style="text-align: center;" x:autofilter='all'>QAIS Status</td>--%>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>Form Issue Date</td>
                            <%--<td class="lockedHeader" style="text-align: center;" x:autofilter='all'>Record Create Date(QAIS)</td>--%>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>Apply discount in NSS</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>Future Order USD Amt</td>
                            <td class="lockedHeader" style="text-align: left;" x:autofilter='all'>AP O/S Amt</td>
                            <td class="lockedHeader" style="text-align: center;" x:autofilter='all'>
                                <asp:Label runat="server" ID="lbl_HandledByLabel" Text="Handled By" Visible="false" /></td>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="trSubTotal" runat="server">
                            <td class="lockedLeft"><b>Office Subtotal</b></td>
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_AmountInUSD" runat="server" /></td>
                            <td style="text-align: center;"></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging1" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging2" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging3" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging4" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging5" runat="server" /></td>
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                            <td class="lockedLeft" />
                        </tr>
                        <tr id="trNormal" runat="server">
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_Office" runat="server" /></td>
                            <%--               <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_HandlingOffice" runat="server" /></td>--%>
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_UKDNDate" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_UKDNReceivedDate" runat="server" /></td>
                            <%--    <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_ClaimType" runat="server" /></td>--%>
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_UKDNNo" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_Vendor" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_DeptCode" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_ItemNo" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_ContractNo" runat="server" /></td>
                            <%--       <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_LastShipmentDate" runat="server" /></td>--%>
                            <td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_Currency" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0">
                                <asp:Label ID="lbl_Qty" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Amount" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_AmountInUSD" runat="server" /></td>
                            <td style="text-align: center;"></td>
                            <td class="lockedLeft" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Aging1" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Aging2" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Aging3" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Aging4" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Aging5" runat="server" /></td>
                            <td style="text-align: center;"></td>
                            <%--<td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_CommentFromMer" runat="server" /></td>--%>
                            <%--<td class="lockedLeft" style="text-align: left;">
                                <asp:Label ID="lbl_Remark" runat="server" /></td>--%>
                            <td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_Status" runat="server" /></td>
                            <%--<td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_T5" runat="server" /></td>--%>
                            <%--<td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_QAISStatus" runat="server" /></td>--%>
                            <td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_FormIssueDate" runat="server" /></td>
                            <%--<td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_QAISCreateDate" runat="server" /></td>--%>
                            <td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_IsAuthorized" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_FutureOrder" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_APOS" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: center;">
                                <asp:Label ID="lbl_HandledBy" runat="server" /></td>
                        </tr>
                        <tr id="trMissing" runat="server" visible="false">
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_Office" runat="server" /></td>
                            <%--<td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_HandlingOffice" runat="server" /></td>--%>
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_UKDNDate" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_UKDNReceivedDate" runat="server" /></td>
                            <%--<td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_ClaimType" runat="server" /></td>--%>
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_UKDNNo" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_Vendor" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_DeptCode" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_ItemNo" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_ContractNo" runat="server" /></td>
                            <%--<td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_LastShipmentDate" runat="server" /></td>--%>
                            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_Currency" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; background-color: Aqua; mso-number-format: \#\,\#\#0">
                                <asp:Label ID="lbl_M_Qty" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; background-color: Aqua; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_M_Amount" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; background-color: Aqua; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_M_AmountInUSD" runat="server" /></td>
                            <td style="text-align: center; background-color: Aqua"></td>
                            <td class="lockedLeftM" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_M_Aging1" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_M_Aging2" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_M_Aging3" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_M_Aging4" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_M_Aging5" runat="server" /></td>
                            <td style="text-align: center; background-color: Aqua"></td>
                            <%--            <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_CommentFromMer" runat="server" /></td>--%>
                            <%--      <td class="lockedLeftM" style="text-align: left;">
                                <asp:Label ID="lbl_M_Remark" runat="server" /></td>--%>
                            <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_Status" runat="server" /></td>
                            <%--         <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_T5" runat="server" /></td>--%>
                            <%--       <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_QAISStatus" runat="server" /></td>--%>
                            <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_FormIssueDate" runat="server" /></td>
                            <%--                     <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_QAISCreateDate" runat="server" /></td>--%>
                            <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_IsAuthorized" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_FutureOrder" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_APOS" runat="server" /></td>
                            <td class="lockedLeftM" style="text-align: center;">
                                <asp:Label ID="lbl_M_HandledBy" runat="server" /></td>
                        </tr>
                        <tr id="trPendingCancellation" runat="server" visible="false">
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_Office" runat="server" /></td>
                            <%--          <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_HandlingOffice" runat="server" /></td>--%>
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_UKDNDate" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_UKDNReceivedDate" runat="server" /></td>
                            <%--           <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_ClaimType" runat="server" /></td>--%>
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_UKDNNo" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_Vendor" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_DeptCode" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_ItemNo" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_ContractNo" runat="server" /></td>
                            <%--            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_LastShipmentDate" runat="server" /></td>--%>
                            <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_Currency" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; background-color: Silver; mso-number-format: \#\,\#\#0">
                                <asp:Label ID="lbl_PC_Qty" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; background-color: Silver; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_PC_Amount" runat="server" /></td>
                            <td style="text-align: right; vertical-align: middle; background-color: Silver; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_PC_AmountInUSD" runat="server" /></td>
                            <td style="text-align: center; background-color: Silver"></td>
                            <td class="lockedLeftPC" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_PC_Aging1" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_PC_Aging2" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_PC_Aging3" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_PC_Aging4" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: right; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_PC_Aging5" runat="server" /></td>
                            <td style="text-align: center; background-color: Silver"></td>
                            <%-- <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_CommentFromMer" runat="server" /></td>--%>
                            <%--  <td class="lockedLeftPC" style="text-align: left;">
                                <asp:Label ID="lbl_PC_Remark" runat="server" /></td>--%>
                            <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_Status" runat="server" /></td>
                            <%--  <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_T5" runat="server" /></td>--%>
                            <%--                     <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_QAISStatus" runat="server" /></td>--%>
                            <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_FormIssueDate" runat="server" /></td>
                            <%--           <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_QAISCreateDate" runat="server" /></td>--%>
                            <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_IsAuthorized" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_FutureOrder" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_APOS" runat="server" /></td>
                            <td class="lockedLeftPC" style="text-align: center;">
                                <asp:Label ID="lbl_PC_HandledBy" runat="server" /></td>
                        </tr>

                    </ItemTemplate>
                    <FooterTemplate>
                        <tr id="trFooter" runat="server">
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid; text-align: right" nowrap><b>Office Subtotal : </b></td>
                            <td style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_AmountInUSD" runat="server" /></td>
                            <td style="text-align: center;"></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging1" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging2" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging3" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging4" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_Subtotal_lbl_Aging5" runat="server" /></td>
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
                <tr>
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid; text-align: right" nowrap><b>Grand Total : </b></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_TotalAmountInUSD" runat="server" /></td>
                    <td class="lockedLeftGrand" />
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_TotalAging1" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_TotalAging2" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_TotalAging3" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_TotalAging4" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_TotalAging5" runat="server" /></td>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
            <%--    <tr>
                    <td style="text-align: center; background-color: Aqua; border-style: none;"></td>
                    <td style="border-style: none;" nowrap>&nbsp;Indicates Tech Team has not created the claim in QAIS</td>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
                <tr>
                    <td style="text-align: center; background-color: Silver; border-style: none;"></td>
                    <td style="border-style: none;" nowrap>&nbsp;Indicates Tech Team has set the claim as PENDING FOR CANCELLATION in QAIS</td>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>--%>
            </div>
            <div runat="server" id="divOfficeSection">
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none; text-align: center" nowrap>Aging Info</td>
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td class="lockedHeader" style="text-align: left;">Office</td>
                    <td class="lockedHeader" style="text-align: left;">Amount In USD</td>
                    <td class="lockedHeader" style="text-align: center;"></td>
                    <td class="lockedHeader" style="text-align: center;">0 &lt; 30</td>
                    <td class="lockedHeader" style="text-align: center;">31 &lt; 60</td>
                    <td class="lockedHeader" style="text-align: center;">61 &lt; 90</td>
                    <td class="lockedHeader" style="text-align: center;">91 &lt; 120</td>
                    <td class="lockedHeader" style="text-align: center;">&gt; 120</td>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
                <asp:Repeater ID="repOfficeSummary" runat="server"
                    OnItemDataBound="repOfficeSummary_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td class="lockedLeft">
                                <asp:Label runat="server" ID="lbl_Summary_Office" /></td>
                            <td style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_OfficeTotal_AmountInUSD" runat="server" /></td>
                            <td style="text-align: center;"></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_OfficeTotal_Aging1" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_OfficeTotal_Aging2" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_OfficeTotal_Aging3" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_OfficeTotal_Aging4" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_OfficeTotal_Aging5" runat="server" /></td>
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td class="lockedLeftGrand"><b>Grand Total</b></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_OfficeGrandTotal_AmountInUSD" runat="server" /></td>
                    <td class="lockedLeftGrand"></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_OfficeGrandTotal_Aging1" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_OfficeGrandTotal_Aging2" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_OfficeGrandTotal_Aging3" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_OfficeGrandTotal_Aging4" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_OfficeGrandTotal_Aging5" runat="server" /></td>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
            </div>
            <div runat="server" id="divSupplierSection">
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none; text-align: center" nowrap>Aging Info</td>
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" nowrap>Vendor</td>
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                    <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                    <td class="lockedHeader" style="text-align: left;">Amount In USD</td>
                    <td class="lockedHeader" style="text-align: center;"></td>
                    <td class="lockedHeader" style="text-align: center;">0 &lt; 30</td>
                    <td class="lockedHeader" style="text-align: center;">31 &lt; 60</td>
                    <td class="lockedHeader" style="text-align: center;">61 &lt; 90</td>
                    <td class="lockedHeader" style="text-align: center;">91 &lt; 120</td>
                    <td class="lockedHeader" style="text-align: center;">&gt; 120</td>
                    <td style="border-style: none;" />
                    <td class="lockedHeader" style="text-align: center;">Future Order USD Amt</td>
                    <td class="lockedHeader" style="text-align: center;">A/P O/S Amt</td>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>

                <asp:Repeater ID="repVendorSummary" runat="server"
                    OnItemDataBound="repVendorSummary_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td class="lockedLeft" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" nowrap>
                                <asp:Label runat="server" ID="lbl_Vendor_HandledBy" /></td>
                            <td class="lockedLeft" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" nowrap>
                                <asp:Label runat="server" ID="lbl_Summary_Vendor" /></td>
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                            <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                            <td style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_VendorTotal_AmountInUSD" runat="server" /></td>
                            <td style="text-align: center;"></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_VendorTotal_Aging1" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_VendorTotal_Aging2" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_VendorTotal_Aging3" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_VendorTotal_Aging4" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_VendorTotal_Aging5" runat="server" /></td>
                            <td style="border-style: none;" />
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_VendorTotal_FutureOrderAmt" runat="server" /></td>
                            <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                                <asp:Label ID="lbl_VendorTotal_APOSAmt" runat="server" /></td>
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                            <td style="border-style: none;" />
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td class="lockedLeftGrand"><b>Grand Total</b></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_VendorGrandTotal_AmountInUSD" runat="server" /></td>
                    <td class="lockedLeftGrand"></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_VendorGrandTotal_Aging1" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_VendorGrandTotal_Aging2" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_VendorGrandTotal_Aging3" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_VendorGrandTotal_Aging4" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_VendorGrandTotal_Aging5" runat="server" /></td>
                    <td style="border-style: none;" />
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_VendorGrandTotal_FutureOrderAmt" runat="server" /></td>
                    <td class="lockedLeftGrand" style="text-align: right; vertical-align: middle; font-weight: bold; mso-number-format: \#\,\#\#0\.00">
                        <asp:Label ID="lbl_VendorGrandTotal_APOSAmt" runat="server" /></td>
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                    <td style="border-style: none;" />
                </tr>
            </div>
            <tr>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr id="row_RevaluationHeader" runat="server">
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" />
                <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td class="lockedHeader" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                <td class="lockedHeader">GBP</td>
                <td class="lockedHeader">EUR</td>
                <td class="lockedHeader">USD</td>
                <td class="lockedHeader">Base Amt</td>
                <td class="lockedHeader" />
                <td class="lockedHeader"></td>
                <td class="lockedHeader"></td>
                <td class="lockedHeader"></td>
                <td class="lockedHeader"></td>
                <td class="lockedHeader"></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr id="row_RevaluationTotal" runat="server">
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td class="lockedLeft" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" nowrap><b>Total Amt:</b></td>
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_GBP_Total" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_EUR_Total" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_USD_Total" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Total" runat="server" /></td>
                <td />
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging1" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging2" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging3" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging4" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging5" runat="server" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr id="row_Accrual" runat="server">
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td class="lockedLeft" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" nowrap><b>Monthly Accrual for UKDN:</b></td>
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_GBP_Accrual" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_EUR_Accrual" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_USD_Accrual" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Accrual" runat="server" /></td>
                <td />
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging1_Accrual" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging2_Accrual" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging3_Accrual" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging4_Accrual" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging5_Accrual" runat="server" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr id="row_AccrualTotal" runat="server">
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td class="lockedLeft" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;"></td>
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_GBP_AccrualTotal" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_EUR_AccrualTotal" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_USD_AccrualTotal" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_AccrualTotal" runat="server" /></td>
                <td />
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging1_AccrualTotal" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging2_AccrualTotal" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging3_AccrualTotal" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging4_AccrualTotal" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; font-weight: bold; color: Navy; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_Aging5_AccrualTotal" runat="server" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr id="row_RevaluationGBP" runat="server">
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td class="lockedLeft" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" nowrap>
                    <asp:Label runat="server" ID="lbl_Revalution_GBP" /></td>
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                <td />
                <td />
                <td />
                <td class="lockedLeft" style="text-align: right; vertical: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GBPTotal" runat="server" /></td>
                <td />
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GBPAging1" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GBPAging2" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GBPAging3" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GBPAging4" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GBPAging5" runat="server" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr id="row_RevaluationEUR" runat="server">
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td class="lockedLeft" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" nowrap>
                    <asp:Label runat="server" ID="lbl_Revalution_EUR" /></td>
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                <td />
                <td />
                <td />
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_EURTotal" runat="server" /></td>
                <td />
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_EURAging1" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_EURAging2" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_EURAging3" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_EURAging4" runat="server" /></td>
                <td class="lockedLeft" style="text-align: right; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_EURAging5" runat="server" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr id="row_RevaluationGrand" runat="server">
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: solid; border-right-style: none;" nowrap><b>Grand Total As Per Ledger</b></td>
                <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: none;" />
                <td class="lockedLeftGrand" style="border-bottom-style: solid; border-top-style: solid; border-left-style: none; border-right-style: solid;" />
                <td class="lockedLeftGrand" />
                <td class="lockedLeftGrand" />
                <td class="lockedLeftGrand" />
                <td class="lockedLeftGrand" style="text-align: right; font-weight: bold; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GrandTotal" runat="server" /></td>
                <td class="lockedLeftGrand" />
                <td class="lockedLeftGrand" style="text-align: right; font-weight: bold; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GrandAging1" runat="server" /></td>
                <td class="lockedLeftGrand" style="text-align: right; font-weight: bold; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GrandAging2" runat="server" /></td>
                <td class="lockedLeftGrand" style="text-align: right; font-weight: bold; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GrandAging3" runat="server" /></td>
                <td class="lockedLeftGrand" style="text-align: right; font-weight: bold; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GrandAging4" runat="server" /></td>
                <td class="lockedLeftGrand" style="text-align: right; font-weight: bold; vertical-align: middle; mso-number-format: \#\,\#\#0\.00">
                    <asp:Label ID="lbl_Currency_GrandAging5" runat="server" /></td>
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
                <td style="border-style: none;" />
            </tr>
            <tr />
            <tr />
            <tr>
                <td style="border-style: none;" nowrap>Prepared by:</td>
                <td style="border-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: none; border-left-style: none; border-right-style: none;"></td>
                <td style="border-bottom-style: solid; border-top-style: none; border-left-style: none; border-right-style: none;"></td>
            </tr>
            <tr />
            <tr />
            <tr>
                <td style="border-style: none;" nowrap>Checked by:</td>
                <td style="border-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: none; border-left-style: none; border-right-style: none;"></td>
                <td style="border-bottom-style: solid; border-top-style: none; border-left-style: none; border-right-style: none;"></td>
            </tr>
            <tr />
            <tr />
            <tr>
                <td style="border-style: none;" nowrap>Reviewed by:</td>
                <td style="border-style: none;" />
                <td style="border-bottom-style: solid; border-top-style: none; border-left-style: none; border-right-style: none;"></td>
                <td style="border-bottom-style: solid; border-top-style: none; border-left-style: none; border-right-style: none;"></td>
            </tr>
        </table>


    </form>
</body>
</html>
