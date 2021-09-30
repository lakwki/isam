<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="LCAppRequest_Ship.aspx.cs" Inherits="com.next.isam.webapp.shipping.LCAppRequest_Ship" Title="L/C Application Request (Shipping)" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--    
    <img src="../images/banner_shipping_lc.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Shipping">L/C Application Request</asp:Panel> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">

     	//function cbxOfficeOnChange()
		//{
		//	if (typeof( getTxtName("ctl00_ContentPlaceHolder1_uclProductTeam") ) != 'undefined' )
		//	{
		//		var txtName = getTxtName("ctl00_ContentPlaceHolder1_uclProductTeam");
		//		var txtOldName = getTxtOldName("ctl00_ContentPlaceHolder1_uclProductTeam");
		//		var txtId = getTxtId("ctl00_ContentPlaceHolder1_uclProductTeam");
		//		var cbxList = getCbxList("ctl00_ContentPlaceHolder1_uclProductTeam");

		//		txtName.value="";
		//		txtOldName.value="";
		//		txtId.value="";
		//		if (cbxList.length > 0)	cbxList.remove(0);
		//	}
		//return;	
		//}
		
        function CopyTextToAll(obj, tagName)
        {
           var SupplierId;
           var oSupplierId;
           
           oSupplierId = document.all(obj.id.replace(tagName,"lbl_SupplierId"));
           SupplierId = oSupplierId.innerText;
           var nodeList = document.getElementsByTagName("input");
           for (i = 0; i <nodeList.length ; i++)
           {
                if (nodeList[i].type == "text" )
                    if (nodeList[i].name.indexOf(tagName) != -1)
                    {
                        if (nodeList[i].name>obj.name)
                        {
                            oSupplierId=document.all(nodeList[i].id.replace(tagName,"lbl_SupplierId"));
                            if (oSupplierId.innerText==SupplierId)
                                nodeList[i].value = obj.value;
                        }        
                    }
           }                      
        }

        function CopySelectValueToAll(obj, tagName)
        {
           var SupplierId;
           var oSupplierId;
           
           oSupplierId = document.all(obj.id.replace(tagName,"lbl_SupplierId"));
           SupplierId = oSupplierId.innerText;
           
           var nodeList = document.getElementsByTagName("select");
           for (i = 0; i <nodeList.length ; i++)
           {
                if (nodeList[i].type == "select-one" )
                    if (nodeList[i].name.indexOf(tagName) != -1)
                        if (nodeList[i].name>obj.name)
                        {
                            oSupplierId=document.all(nodeList[i].id.replace(tagName,"lbl_SupplierId"));
                            if (oSupplierId.innerText==SupplierId)
                                nodeList[i].value = obj.value;
                        }
           }                      
        }
		
		function checkAppNo(obj)
		{
		    if (obj.value.trim()!="" && isNaN(parseInt(obj.value)))
		    {
		        alert ("Invalid L/C Application No.");
		        return false;
		    }
		    return true;
		}


		var activeGridPanel;
		var gridPanelDimension = { height: 300, width: 700 };
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
		    elementTop = scrollWindow.top + panelTop + 10;
		    elementLeft = scrollWindow.left + panelLeft - gridPanelDimension.width / 2;
		    return { left: elementLeft, top: elementTop }
		}

		    function setPanel(display, button) {
		        //debugger;
		        var panel;
		        var nodes;
		        if (button != undefined) {
		            var buttonBoundary = button.getBoundingClientRect();
		            var panelBoundary = setPanelBoundary(buttonBoundary.left, buttonBoundary.bottom);
		            nodes = button.parentElement.getElementsByTagName("DIV");
		            if (nodes.length > 0) {
		                //debugger;
		                panel = nodes[0];
		                nodes = panel.getElementsByTagName("TR");
		                var panelHeight = ((nodes.length <= 10 ? nodes.length : 10) + 4) * 16;
		                panel.style.left = panelBoundary.left;
		                panel.style.top = panelBoundary.top;
		                panel.style.width = gridPanelDimension.width;
		                panel.style.height = panelHeight;   // gridPanelDimension.height;

		                var scrollWindow = scrollWindowDimension()
		                var scrollY = panelBoundary.top + panelHeight - scrollWindow.bottom;
		                window.scrollBy(0, (scrollY > 0 ? scrollY : 0));
		            }
		        }
		        if (panel == undefined)
		            panel = activeGridPanel;

		        if (panel != undefined)
		            if (display != undefined) {
		                if (display.toUpperCase() == "ON") {
		                    if (activeGridPanel != undefined) {
		                        activeGridPanel.style.display = "none";
		                        activeGridPanel = undefined;
		                    }
		                    if (panel != undefined)
		                        panel.style.display = "block";
		                    activeGridPanel = panel;
		                }
		                else if (display.toUpperCase() == "OFF") {
		                    if (panel != undefined)
		                        panel.style.display = "none";
		                    activeGridPanel = undefined;
		                }
		            }
		    }

		function hidePanel(obj) {
            // hide the active panel
		    setPanel("OFF");
		}

 /*
        // --- Move LC shipment info panel
		var enableMoveOfInfoPanel = false;
		var panelOffsetX = undefined;
		var panelOffsetY = undefined;
		var isMoving = false;
		var isOutside = true;
		function dragPanel(panel) {
		    if (!enableMoveOfInfoPanel) return;
		    //debugger;
		    showMouseEvent(panel, 'DragPanel');
		    isOutside = false;
		    panelOffsetX = window.event.clientX - parseInt(panel.style.left);
            panelOffsetY = window.event.clientY - parseInt(panel.style.top);
            panel.style.borderStyle = "outset";
            panel.style.cursor = "move";
            panel.style.borderBottomWidth = "8";
            panel.style.borderRightWidth = "8";
        }

        function outPanel(panel) {
            //debugger;
            if (!enableMoveOfInfoPanel) return;
            var x = window.event.clientX;
            var y = window.event.clientY;
            var w = parseInt(panel.style.width);
            var h = parseInt(panel.style.height);
            var rect = panel.getBoundingClientRect();
            var l = rect.left;
            var t = rect.top;

            showMouseEvent(panel, 'OutPanel (' + x.toString() + ',' + y.toString() + ') - (' + l.toString() + ',' + t.toString() + w.toString() + ',' + h.toString() + ')');
            if (!((x >= l && x <= (l + w)) && (y >= t && y <= (t + h)))) {
                //alert('Out : ' + l.toString() + '<=' + x.toString() + '<=' + (l + w).toString() + ' , ' + t.toString() + '<=' + y.toString() + '<=' + (t + h).toString());
                //debugger;
                isOutside = true;
                if (panelOffsetX != undefined && panelOffsetY != undefined) {
                    dropPanel(panel);
                }
            }
        }

        function resetPanel(panel) {
            if (!enableMoveOfInfoPanel) return;
            showMouseEvent(panel, 'ResetPanel');
            isOutside = true;
		    panelOffsetX = undefined;
		    panelOffsetY = undefined;
		    panel.style.borderStyle = "inset";
		    panel.style.cursor = "auto";
		    panel.style.borderBottomWidth = "1";
		    panel.style.borderRightWidth = "1";
		}

		function dropPanel(panel) {
		    if (!enableMoveOfInfoPanel) return;
		    showMouseEvent(panel, 'DropPanel');

		    if (panelOffsetX != undefined && panelOffsetY != undefined) {
		        //alert(panel.style.border);
		        //debugger;
		        panel.style.left = window.event.clientX - panelOffsetX  ;
		        panel.style.top = window.event.clientY - panelOffsetY ;

		        resetPanel(panel);
		    }
		}

		function movePanel(panel) {
		    if (!enableMoveOfInfoPanel) return;
		    if (!isMoving) {
		        isMoving = true;
		        showMouseEvent(panel,'MovePanel');
		        if (!isOutside) {
		            var cursorX = window.event.clientX;
		            var cursorY = window.event.clientY;
		            if (panelOffsetX != undefined && panelOffsetY != undefined) {
		                panel.style.left = cursorX - panelOffsetX;
		                panel.style.top = cursorY - panelOffsetY;
		            }
		        }
		        isMoving = false;
		    }
		}

		var eventCount = 0;
		function showMouseEvent(panel, msg) {
		    if (!enableMoveOfInfoPanel) return;
		    var nodes = panel.parentElement.getElementsByTagName("INPUT");
		    var txt = nodes[0];
		    var val = txt.value + "| ("+eventCount.toString()+") " + msg;
		    txt.value = val.substring((val.length > 100 ? val.length - 100 : 0), val.length);
		    eventCount++;
		}
*/
</script>

    <table width="800px" cellspacing="0" cellpadding="2">
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan='3'>
            <asp:Button ID="btn_LCNewApplicationSummary" runat="server" Text="L/C New Application Summary" skinid="XXLButton" onclick="btn_LCNewApplicationSummary_Click" />
        </td>
    </tr>
    <tr>
        <td colspan='6'>
            <hr />
        </td>
    </tr>
        <tr>
        <td class="FieldLabel2">Office</td>
        <td>
            <cc1:SmartDropDownList ID='ddl_Office' runat='server' width="130" AutoPostBack="True" onselectedindexchanged="ddl_Office_SelectedIndexChanged"/>
        </td>
        <td class="FieldLabel2">Department</td>
        <td>
            <cc1:SmartDropDownList ID='ddl_Department' runat='server' width="130" AutoPostBack="True" onselectedindexchanged="ddl_Department_SelectedIndexChanged"/>
        </td>
        <td class="FieldLabel2">Product Team</td>
        <td>
            <cc1:SmartDropDownList ID='ddl_ProductTeam' runat='server' width="200"/>
        </td>
    </tr>

    <tr>
        <td class="FieldLabel2">Supplier</td>
        <td colspan="5">
        <uc1:UclSmartSelection  ID="txt_SupplierName" runat="server" width="300px"/>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">L/C Application Date</td>
        <td colspan="3">
            <cc1:smartcalendar id="txt_LCAppDateFrom"  runat="server" Width="120px" FromDateControl="txt_LCAppDateFrom" ToDateControl="txt_LCAppDateTo"></cc1:smartcalendar>
            &nbsp;&nbsp;to&nbsp;
			<cc1:smartcalendar id="txt_LCAppDateTo" runat="server" Width="120px"  FromDateControl="txt_LCAppDateFrom" ToDateControl="txt_LCAppDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">L/C Application No.</td>
        <td colspan="3">
            <asp:TextBox ID ="txt_LCAppNoFrom" runat="server"  Skinid="DateTextBox" onchange='return(checkAppNo(this));' />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<asp:TextBox ID="txt_LCAppNoTo" runat="server" SkinID="DateTextBox" onchange='return(checkAppNo(this));' />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Status</td>
        <td colspan="3">
            <cc1:SmartDropDownList ID='ddl_LcApplicationStatus' runat='server' width="100" OnSelectedIndexChanged='ddl_LcApplicationStatus_SelectedIndexChanged'  AutoPostBack="true"/>
        </td>
    </tr>
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td colspan="3">
            <asp:Button ID="btn_Search" runat="server" Text="Search" onclick="btn_Search_Click" />
        </td>
    </tr>
</table>
<br /><br />
<asp:Panel ID="pnl_SearchResult" runat="server" Visible="false">
    <asp:Button ID="btn_Approve" runat="server" Text="Approve" onclick="btn_Approve_Click" />&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Reject" runat="server" Text="Reject" onclick="btn_Reject_Click" />&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Apply" runat="server" Text="Apply" onclick="btn_Apply_Click" />&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_UpdateLCInfo" runat="server" Text="Update"  ToolTip="Update the L/C Information of the selected shipment"   onclick="btn_UpdateLCInfo_Click" />&nbsp;&nbsp;&nbsp;
    <br /><br />
    <asp:LinkButton ID="btn_SelectAll" runat="server" Text="Select All" OnClientClick="CheckAll('ckb_LC'); return false;" />&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="btn_DeselectAll" runat="server" Text="Deselect All" OnClientClick="UncheckAll('ckb_LC'); return false;" />

    <asp:UpdatePanel ID="up_LC" runat="server" >
        <ContentTemplate>
            <asp:GridView ID="gv_LC" runat="server" AutoGenerateColumns="false" OnSorting="gv_LC_OnSort" AllowSorting="True" onrowdatabound="gv_LC_RowDataBound">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox ID="ckb_LC" runat="server" Checked="true" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="L/C Application No." SortExpression="LCApplication.LCApplicationNo" HeaderStyle-ForeColor="CornflowerBlue">
                        <ItemTemplate>
                            <asp:Label ID="lbl_ApplicationNo" Text='<%# Eval("LCApplication.LCApplicationNo") %>'  runat="server"  />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Office Code" ItemStyle-Width="20px">
                        <ItemTemplate >
                            <asp:Label ID="lbl_OfficeCode" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Supplier">
                        <ItemTemplate >
                            <asp:Label ID="lbl_Supplier" runat="server"  />
                            <asp:Label ID="lbl_SupplierId" runat="server" style="display:none;"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                        <div>
                            <asp:LinkButton ID="lnk_LCInfoOfOtherDly" runat="server" Text="!"  style="color:Red; font-size:medium;" visible="false" 
                                onClientClick="setPanel('on',this);return false;" ToolTip="L/C of other delivery" />
                                  
                            <asp:Panel id="div_LCInfoOfOtherDly" runat="server" 
                                    style="position:absolute; overflow:auto; left:0px; top:0px; width:0px; height:0px; z-index: 2; background-color: #FFFFCC; border:inset 1px #404040;  display:none;">
                                <label class="header2" style="float:left;" >&nbsp;L/C Shipment</label>
                                
                                <input type='text' id="txt_msg" value="" style="width:500px; text-align:right; display:none;"  visible="false"/>

                                <a onclick="hidePanel(this)" style="cursor:pointer;" ><img src="../images/close.png" alt="" style="float:right;" /></a>
                                <table width="98%">
                                     
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:GridView ID="gv_LCInfoOfOtherDly" runat="server" OnRowDataBound="LCInfoOfOtherDlyRowDataBound"  AutoGenerateColumns="false"  BorderColor="Silver" CssClass="TableWithGridLines">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="L/C<br>Batch No.">
                                                    <HeaderStyle BorderColor="Silver" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_LcBatchNo" runat="server" Text='' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Contract No.">
                                                    <HeaderStyle BorderColor="Silver" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_ContractNo" runat="server" Text='' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Dly<br>No.">
                                                    <HeaderStyle BorderColor="Silver" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_DlyNo" runat="server" Text='' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="L/C<br>Application<br>PO Qty">
                                                    <HeaderStyle BorderColor="Silver" />
                                                        <ItemTemplate >
                                                            <asp:Label ID="lbl_LcPoQty" runat="server" Text=''  />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ISAM<br>PO Qty">
                                                    <HeaderStyle BorderColor="Silver" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_PoQty" runat="server" Text=''  />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="L/C No.">
                                                    <HeaderStyle BorderColor="Silver" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_LcNo" runat="server" Text='' />
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
                                                    <asp:TemplateField HeaderText="L/C Status">
                                                    <HeaderStyle BorderColor="Silver" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_LcStatus" runat="server" Text='' />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="90" />
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                        <td></td>
                                     </tr>
                                    <tr><td>&nbsp;</td></tr>
                                </table>
                            </asp:Panel>
                        </div>
                        </ItemTemplate>
                    </asp:TemplateField>        
                    <asp:TemplateField HeaderText="Contract No." SortExpression="ContractNo" HeaderStyle-ForeColor="CornflowerBlue">
                        <ItemTemplate>
                            <asp:Label ID="lbl_ContractNo"   runat="server"  />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Dly No.">
                        <ItemTemplate>
                            <asp:Label ID="lbl_DlyNo"   runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>        
                    <asp:TemplateField HeaderText="PO Qty.">
                        <ItemTemplate>
                            <asp:Label ID="lbl_POQty" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Purchase Term">
                        <ItemTemplate>
                            <asp:Label ID="lbl_PurchaseTerm"   runat="server"  />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Purchase Location">
                        <ItemTemplate>
                            <asp:Label ID="lbl_PurchaseLocation"   runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>        
                    <asp:TemplateField HeaderText="Ccy">
                        <ItemTemplate >
                            <asp:Label ID="lbl_Ccy" runat="server" />
                            <asp:Label ID="lbl_CurrencyId" runat="server" style="display:none;" />
                        </ItemTemplate>
                    </asp:TemplateField>        
                    <asp:TemplateField HeaderText="PO Scheduled Delivery Date"  SortExpression="PoDeliveryDate" HeaderStyle-ForeColor="CornflowerBlue">
                        <ItemTemplate >
                            <asp:Label ID="lbl_PODeliveryDate" runat="server" />
                        </ItemTemplate>
                        <ItemStyle Width="80px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="PO Amount">
                        <ItemTemplate>
                            <asp:Label ID="lbl_POAmt"   runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Issuing Bank">
                        <ItemTemplate>
                        <asp:Label ID="lbl_IssuingBank" runat="server" Text='' enabled='false'/>
                        <asp:DropDownList ID="ddl_IssuingBank" runat="server" SkinID="SmallDDL" onchange='CopySelectValueToAll(this,"ddl_IssuingBank")'>
                        </asp:DropDownList> 
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Grouping">
                        <ItemTemplate >
                            <asp:Textbox ID="txt_Group" SkinID="TextBox_50" runat="server"  Enabled='true' onchange='CopyTextToAll(this,"txt_Group")' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="L/C Batch No.">
                        <ItemTemplate >
                            <asp:Label ID="lbl_LCBatchNo" runat="server" />
                            <asp:TextBox ID="txt_LCBatchNo" runat="server" style="display:none;"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate >
                            <asp:Label ID="lbl_WorkflowStatus" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <span style="font-family: Arial; font-size: medium; font-weight: bold; background-color: #FFFFCC;">No record found.</span>
                </EmptyDataTemplate>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>

<div style='display:none;'>
    <asp:Panel ID='pnl_DebugArea' runat='server' >
        <asp:TextBox ID='txt_DebugMessage' runat='server' Text='' style='display:none;' Enabled='false'></asp:TextBox>
        <span id='ClientDebugMessageArea' style=''></span>
    </asp:Panel>
</div>

</asp:Content>
