<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UclSmartSelectionAJAX.ascx.cs" Inherits="com.next.isam.webapp.webservices.UclSmartSelectionAJAX" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<script type="text/javascript">
    /*function pageLoad() {
        $addHandler($('ddlName'), 'change', setcontext);
    }
    function setContext(event) {
        $find('txtnames_AutoCompleteExtender').set_contextKey(event.target.value);
    }*/
</script>

<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js" type = "text/javascript"></script> 
<script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js" type = "text/javascript"></script> 
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel = "Stylesheet" type="text/css" /> 
<link href="common/AJAXSmartControlStyle.css" rel="stylesheet"/>


<script type = "text/javascript">
    $(function () {
        var webservice = '';
        var vendortype = '';
        var office = '-1';
        var workflowstatus = '-1';
        var department = '-1';
        var supplier = '';
        var listtype = document.getElementById('<%=hType.ClientID %>').value;

        if (listtype == 'getFabricVendorList') {
            webservice = 'getFabricVendorList';
        }
        else if (listtype == 'getNonClothingVendorList') {
            webservice = 'getNonClothingVendorList';
        }
        else if (listtype == 'getGarmentVendorList') {
            webservice = 'getGarmentVendorList';
        }
        else if (listtype == 'getSZVendorList') {
            webservice = 'getSZVendorList';
        }
        else if (listtype == 'getVendorListForRecharge') {
            vendortype = document.all.txt_VendorType.value;
            webservice = 'getVendorListForRecharge';
        }
        else if (listtype == 'getNTVendorList') {
            if (document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office') != null) {
                office = document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office').value;
            }
            if (document.all.txt_VendorStatus) {
                workflowstatus = document.all.txt_VendorStatus.value;
            }
            webservice = 'getNTVendorList';
        }
        else if (listtype == 'getContractNoList') {
            webservice = 'getContractNoList';
        }
        else if (listtype == 'getProductCodeList') {
            if (document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office') != null) {
                office = document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office').value;
            }
            if (document.getElementById('ctl00_ContentPlaceHolder1_ddl_Department') != null) {
                department = document.getElementById('ctl00_ContentPlaceHolder1_ddl_Department').value;
            }
            webservice = 'getProductCodeList';
        }
        else if (listtype == 'getArticleNoList') {
            if (typeof (getTxtId('uclFabricSupplier')) != 'undefined') {
                supplier = getTxtId('uclFabricSupplier').value;
            }
            webservice = 'getArticleNoList';
        }
        else if (listtype == 'getUserList') {
            if (document.all.txt_UserOfficeId) {
                office = document.all.txt_UserOfficeId.value;
            }
            webservice = 'getUserList';
        }

        var webserviceurl = '<%=ResolveUrl("SmartSelectionAJAXWebService.asmx/' + webservice + '") %>'
        $("[id$=txtItem]").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: webserviceurl,
                    data: "{ 'prefix': '" + request.term + "|" + vendortype + "|" + office + "|" + workflowstatus + "|" + department + "|" + supplier + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.d.length > 0) {
                            response($.map(data.d, function (item) {
                                return {
                                    label: item.split('^')[0],
                                    val: item.split('^')[1],
                                    display: item.split('^')[2],
                                    handleOffice: parseInt(item.split('^')[2]),
                                    construction: item.split('^')[3],
                                    composition: item.split('^')[4],
                                    width: item.split('^')[5],
                                    weight: item.split('^')[6],
                                    finish: item.split('^')[7]
                                }
                            }))
                        }
                        else {
                            //If no records found, set the default "No match found" item with value -1.
                            response([{ label: 'No results found.', val: -1}]);
                        }

                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            },
            select: function (e, i) {
                if (listtype == 'GarmentVendorList' || listtype == 'SZVendorList') {
                    callBackResultForGetGarmentSupplierContact(i.item.val, i.item.handleOffice);
                }
                else if (listtype == 'ArticleNoList') {
                    callBackResultForGetFabricInfoReference(i.item.construction, i.item.composition, i.item.width, i.item.weight, i.item.finish);
                }
                if (listtype == 'ArticleNoList') {
                    e.preventDefault();
                    $("[id$=txtItem]").val(i.item.display);
                }
                $("[id$=hCode]").val(i.item.val);
            },
            change: function (ev, ui) {
                $(this).val(ui.item.label);
                $("[id$=hCode]").val(ui.item.val);
                if (listtype == 'ArticleNoList') {
                    uclArticleNoOnClear();
                }
                else if (listtype == 'UserList') {
                    document.getElementById("ctl00_ContentPlaceHolder1_ddl_Office").value = "-1";
                }
            },
            minLength: 1
        });
    });

    function getTxtId(parentId) {
        return document.all[parentId + "_txtId"];
    }

    function uclArticleNoOnClear() {
        if (document.all.lblFabricFabricName)
            document.all.lblFabricFabricName.innerText = "";
        if (document.all.uclFabricSupplier_txtName)
            document.all.uclFabricSupplier_txtName.innerText = "";
        if (document.all.lblFabricConstruction)
            document.all.lblFabricConstruction.innerText = "";
        if (document.all.lblFabricComposition)
            document.all.lblFabricComposition.innerText = "";
        if (document.all.lblFabricWidth)
            document.all.lblFabricWidth.innerText = "";
        if (document.all.lblFabricBeforeWashWeight)
            document.all.lblFabricBeforeWashWeight.innerText = "";
        if (document.all.lblFabricFinish)
            document.all.lblFabricFinish.innerText = "";
    }

    function callBackResultForGetGarmentSupplierContact(vendor, handleOffice) {
        if (document.all.ctl00_ContentPlaceHolder1_chk_HasUKDN) {
            document.all.ctl00_ContentPlaceHolder1_ddl_Office.value = handleOffice;
            if (vendor == 353)
                document.all.ctl00_ContentPlaceHolder1_ddl_HandlingOffice.value = "17";
            else
                document.all.ctl00_ContentPlaceHolder1_ddl_HandlingOffice.value = handleOffice;
        }
    }

    function callBackResultForGetFabricInfoReference(construction, composition, width, weight, finish) {
        if (document.all.lblFabricConstruction)
            document.all.lblFabricConstruction.innerText = construction;
        if (document.all.lblFabricComposition)
            document.all.lblFabricComposition.innerText = composition;
        if (document.all.lblFabricWidth)
            document.all.lblFabricWidth.innerText = width;
        if (document.all.lblFabricBeforeWashWeight)
            document.all.lblFabricBeforeWashWeight.innerText = weight;
        if (document.all.lblFabricFinish)
            document.all.lblFabricFinish.innerText = finish;
    }

    function clearSmartSelection(selectionKey, parentId, customClientMethod) {
        var txtName = getTxtName(parentId);
        var txtOldName = getTxtOldName(parentId);
        var txtId = getTxtId(parentId);

        txtName.value = "";
        txtName.disabled = false;
        txtOldName.value = "";
        txtId.value = "";

        if (parentId == "uclArticleNo") {
            uclArticleNoOnClear();
        }

        txtName.fireEvent("onchange");
        txtId.fireEvent("onchange");

        if (selectionKey == "garmentVendor") {
            var contentPlaceHolder1_ddl_Office = document.getElementById("ctl00_ContentPlaceHolder1_ddl_Office");
            if (typeof (contentPlaceHolder1_ddl_Office) !== 'undefined' && contentPlaceHolder1_ddl_Office != null) {
                contentPlaceHolder1_ddl_Office.value = "-1";

            }
        }
    }

    function getTxtName(parentId) {
        //alert(parentId);
        return document.all[parentId + "_txtItem"];
    }

    function getTxtOldName(parentId) {
        //alert(parentId);
        return document.all[parentId + "_txtOldName"];
    }

    function getTxtId(parentId) {
        //alert(parentId);
        return document.all[parentId + "_hCode"];
    }

</script>

 <asp:TextBox ID="txtItem" runat="server" style="width:350px"/><asp:Label id="lblClear" runat="server"></asp:Label>
 <asp:HiddenField ID="hCode" runat="server" />
 <asp:HiddenField ID="hType" runat="server" />
 <asp:textbox id="txtOldName" runat="server" style="DISPLAY: none"></asp:textbox>
 <br/>
