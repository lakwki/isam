function loadSmartCalendarRange(obj, to){								
	var date=document.all(obj).value;
	var dateTo=document.all(to).value;
	
	if (date=="" && document.all(obj).VisibleDate)
		date=document.all(obj).VisibleDate;

	if (dateTo=="" && document.all(to).VisibleDate)
		dateTo=document.all(to).VisibleDate						

	calendar_window=window.open("../common/SmartCalendarRange.aspx?target=" + obj + "&date=" + date + "&targetTo=" + to + "&dateTo=" + dateTo,"calendar_window","width=465,height=190");
	calendar_window.focus()
}

function showDMSProgressBar() {
    var left = window.screen.availWidth / 3 - 100;
    var top = window.screen.availHeight / 3 - 100
    waitDialog = window.showModelessDialog("../common/DMSProgressBar.htm", window, "dialogHeight: 30px; dialogWidth: 450px; center:yes; edge: sunken; center: Yes; help: No; resizable: No; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
}

function loadSmartCalendar(obj){	
	var date=document.all(obj).value;
	
	if (date=="" && document.all(obj).VisibleDate)
		date=document.all(obj).VisibleDate		
	
	calendar_window=window.open("../common/SmartCalendar.aspx?target=" + obj + "&date=" + date,"calendar_window","width=220,height=190");
	calendar_window.focus()
}

function SelectAllWithSameType(controlName, altText) {
    var nodeList = document.getElementsByTagName("input");
    for (i = 0; i < nodeList.length; i++) {
        if (nodeList[i].type == "checkbox")
            if (nodeList[i].alt == altText && nodeList[i].name.indexOf(controlName) != -1 && !(nodeList[i].disabled))
                nodeList[i].checked = true;
    }
}

function resetHighlightEntry(o) {
    var op = o.parentNode;
    var className = '';

    while (op.nodeName != 'TABLE')
        op = op.parentNode;
    var nodeList = op.getElementsByTagName("TR");
    for (i = 0; i < nodeList.length; i++) {
        nodeList[i].className = className;
    }   
}

function highlightEntry(o) {
    var op = o.parentNode;
    var className = '';
    if (o.type == "checkbox") {
        if (o.checked) className = 'over';
    }
    else
        className = 'over';

    while (op.nodeName != 'TR')
        op = op.parentNode;
    op.className = className;
}

function CheckAll(controlName) {

    var nodeList = document.getElementsByTagName("input");
    
   for (i = 0; i <nodeList.length ; i++)
   {
        if (nodeList[i].type == "checkbox" )
            if (nodeList[i].name.indexOf(controlName) != -1 && !(nodeList[i].disabled) )
                nodeList[i].checked = true;
   }
}

function UncheckAll(controlName)
{
   var nodeList = document.getElementsByTagName("input");
   for (i = 0; i <nodeList.length ; i++)
   {
        if (nodeList[i].type == "checkbox")
            if (nodeList[i].name.indexOf(controlName) != -1)            
                nodeList[i].checked = false;
   }
}

function GetCheckBoxSelectedCount(controlName)
{
   var nodeList = document.getElementsByTagName("input");
   var count = 0;
   for (i = 0; i <nodeList.length ; i++)
   {
        if (nodeList[i].type == "checkbox")
            if (nodeList[i].name.indexOf(controlName) != -1)            
                if (nodeList[i].checked)
                    count ++;
   }
   return count;
}

function getCurrentDate(ckb, textboxName)
{                
    var d = new Date();
    var obj = document.getElementById(textboxName);
    var day = d.getDate();
    var month = d.getMonth() + 1;
    if (day < 10)
    {
        day = "0" + day.toString();
    }
    
    if (month < 10)
        month = "0" + month.toString();
       
    if (obj != null)
    {
        if (ckb.checked)
            obj.value = day+"/"+month +"/"+ d.getFullYear();     
        else
            obj.value ="";       
    }
}

// Move an element directly on top of another element (and optionally
// make it the same size)
function Cover(bottom, top, ignoreSize) {
    var location = Sys.UI.DomElement.getLocation(bottom);
    top.style.position = 'absolute';
    top.style.top = location.y + 'px';
    top.style.left = location.x + 'px';
    if (!ignoreSize) {
        top.style.height = bottom.offsetHeight + 'px';
        top.style.width = bottom.offsetWidth + 'px';
    }
}

function isInvoiceNoValid(invoiceNo) {

    return (/[A-Za-z]{3}\/[0-9]{5}\/[0-9]{4}/.test(invoiceNo));

    /*
    if (invoiceNo.length != 14) {
        if (invoiceNo.indexOf('-') == -1 )
            return false;
    }
       
    if ( invoiceNo.substring(3,4) != "/" || invoiceNo.substring(9,10) != "/")
        return false

    for (i = 4; i <= 8; i++)
    {
        if (isNaN(parseInt(invoiceNo.charAt(i))))
            return false
    }        
    for (i = 10; i <= 13; i++)
    {
        if (isNaN(parseInt(invoiceNo.charAt(i))))
            return false;            
    }            
    
    return true;
    */
}

function formatDateString(obj) {
    var valid = true;
    var str = '', chr = '';
    var delimiter = '';
    var d, m, y;

    str = '';
    for (i = 0; i < obj.value.length; i++)
        if ((chr = obj.value.substring(i, i + 1)) != ' ')
        str += chr;
    obj.value = str;

    if (obj.value.length == 0)
        return;
    else if (obj.value.trim().length != 0 && obj.value.trim().length < 6)
        valid = false;
    else if (obj.value.indexOf('/') != -1 || obj.value.indexOf('-') != -1 || obj.value.indexOf('.') != -1) {
        var temp;
        if (obj.value.indexOf('/') != -1)
            delimiter = '/';
        else
            if (obj.value.indexOf('-') != -1)
            delimiter = '-';
        else
            delimiter = '.';

        temp = obj.value.split(delimiter);
        if (valid = (temp.length == 3)) {
            d = temp[0].trim();
            m = temp[1].trim();
            y = temp[2].trim();
        }
    }
    else {
        if (valid = (parseInt(obj.value, 10) > 0)) {
            d = obj.value.trim().substring(0, 2);
            m = obj.value.trim().substring(2, 4);
            y = obj.value.trim().substring(4);
        }
    }

    if (valid) {
        if (d == undefined || m == undefined || y == undefined)
            valid = false;
        else
            if (parseInt(d, 10) <= 0 || parseInt(m, 10) <= 0 || parseInt(y, 10) <= 0)
            valid = false;
        else {
            str = parseInt(d, 10).toString();
            str = d.replace('0' + str, '').replace(str, '');
            valid = valid & (str == '')

            str = parseInt(m, 10).toString();
            str = m.replace('0' + str, '').replace(str, '');
            valid = valid & (str == '')

            str = parseInt(y, 10).toString();
            str = y.replace('000' + str, '').replace('00' + str, '').replace('0' + str, '').replace(str, '');
            valid = valid & (str == '')
        }
    }
    if (!valid) {
        alert('Invalid date format.');
        return;
    }

    // Build the date string
    if (d.length == 1)
        d = "0" + d;
    if (m.length == 1)
        m = "0" + m;
    if (y.length == 2)
        y = "20" + y;

    obj.value = d + "/" + m + "/" + y;
    str = d + "/" + m + "/" + y;
    var delimChar = "/";

    var re = /\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/;
    if (re.test(str)) {
        //var delimChar = str.indexOf("/") != -1 ? "/" : "-";
        var delim1 = str.indexOf(delimChar);
        var delim2 = str.lastIndexOf(delimChar);
        day = parseInt(str.substring(0, delim1), 10);
        mo = parseInt(str.substring(delim1 + 1, delim2), 10);
        yr = parseInt(str.substring(delim2 + 1), 10);
        var testDate = new Date(yr, mo - 1, day);

        if (testDate.getDate() != day || testDate.getMonth() + 1 != mo || testDate.getFullYear() != yr) {
            alert("Incorrect date format.");
            return;
        }
    }
    else {
        alert("Incorrect date format.");
        return;
    }
}

function formatDateString_Trial(obj) {
    var valid = true;
    var str = '', chr = '', dateStr = '';
    var delimiter = '';
    var d, m, y;

    if (obj.value.trim() == "")
        return;
    //debug = temp[0];    
    // Remove space and invalid characters
    for (i = 0, str = ''; i < obj.value.length; i++) {
        if ((chr = obj.value.substring(i, i + 1)) != ' ') {
            if ((chr >= "0" && chr <= "9") || chr == "/" || chr == "-" || chr == ".")
                str += chr;
            else
                valid = false;
        }
    }
    dateStr = str;
alert('Filtered - [' + dateStr + ']');

    if (valid) {
        // determine the Day, Month and Year field    
        if (dateStr.length < 6)
            valid = false;
        else if (dateStr.indexOf('/') != -1 || dateStr.indexOf('-') != -1 || dateStr.indexOf('.') != -1) {
            var temp;
            if (dateStr.indexOf('/') != -1)
                delimiter = '/';
            else
                if (dateStr.indexOf('-') != -1)
                delimiter = '-';
            else
                delimiter = '.';

            // Remove invalid characters
            for (i = 0, str = ''; i < dateStr.length; i++) {
                chr = dateStr.substring(i, i + 1);
                if (chr == delimiter || (chr >= '0' && chr <= '9'))
                    //str += (delimiter != "." && chr == "." ? "" : chr);
                    str += chr;
                //else
                    //valid = false;
            }
            dateStr = str;

            if (valid) {
                temp = dateStr.split(delimiter);
                valid = (temp.length == 3);
                //if (valid = (temp.length == 3)) {
                //    d = temp[0];
                //    m = temp[1];
                //    y = temp[2];
                //}
                for (i = 0; i < temp.length; i++) {
                    for (j = 0, str = '', chr = ''; j < temp[i].length; j++) {
                        // remove the preceding floating point
                        if (chr == '') {
                            if ((chr = temp[i].substring(j, j + 1)) == '.')
                                chr = '';
                        }
                        else
                            chr = temp[i].substring(j, j + 1);
                        str += chr;
                    }
                    //str = temp[i];
                    if (i == temp.length - 1)
                        y = str;
                    else if (i == temp.length - 2)
                        m = str;
                    else
                        d = (d == null ? str : d + delimiter + str);
                }
            }
        }
        else {
            if (valid = (parseInt(dateStr, 10) > 0)) {
                d = dateStr.substring(0, 2);
                m = dateStr.substring(2, 4);
                y = dateStr.substring(4);
            }
        }
    }

alert('Extracted - [' + d + "|" + m + "|" + y + ']' + (valid ? 'Valid' : 'Invalid'));

    valid &= !(d == undefined || m == undefined || y == undefined);

    if (valid) {
        // Build the formatted date string
        if (true) {
            if (d.length == 1)
                d = "0" + d;
            if (m.length == 1)
                m = "0" + m;
            if (y.length == 2)
                y = "20" + y;
        }
        else {
            //d = (parseInt(d, 10) <= 9 ? "0" : "") + parseInt(d, 10).toString(10);
            d = (parseInt(d.replace('.', ''), 10) <= 9 ? "0" : "") + parseInt(d.replace('.', ''), 10).toString(10);
            m = (parseInt(m, 10) <= 9 ? "0" : "") + parseInt(m, 10).toString(10);
            y = ((parseInt(y, 10) <= 99 ? 2000 : 0) + parseInt(y, 10)).toString(10);
        }
        
        str = (d + "/" + m + "/" + y);
        var delimChar = "/";
alert('Formatted - [' + (d + "/" + m + "/" + y) + ']' + (valid ? 'Valid' : 'Invalid'));

        var re = /\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/;
        if (re.test(str)) {
            //var delimChar = str.indexOf("/") != -1 ? "/" : "-";
            var delim1 = str.indexOf(delimChar);
            var delim2 = str.lastIndexOf(delimChar);
            day = parseInt(str.substring(0, delim1), 10);
            mo = parseInt(str.substring(delim1 + 1, delim2), 10);
            yr = parseInt(str.substring(delim2 + 1), 10);
            var testDate = new Date(yr, mo - 1, day);

            if (testDate.getDate() != day || testDate.getMonth() + 1 != mo || testDate.getFullYear() != yr)
                valid = false;
            else {
                str = (day <= 9 ? "0" + day.toString(10) : day.toString(10));
                str += delimChar + (mo <= 9 ? "0" + mo.toString(10) : mo.toString(10));
                str += delimChar + (yr <= 99 ? (2000 + yr).toString(10) : yr.toString(10));
                //obj.value = str;
                alert('Ultimate Format : ' + str);
            }
        }
        else 
            valid = false;
    }

    if (valid)
        alert('Validated - [' + str + ']');
    else
        alert('Invalid - [' + d + '|' + m + '|' + y + ']');

    if (!valid) {
        alert('Incorrect date format.');
        //return;
    }
}

function isDateValid(str) {
    var re = /\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/;
    if (re.test(str)) {
        var delimChar = (str.indexOf("/") != -1) ? "/" : "-";
        var delim1 = str.indexOf(delimChar);
        var delim2 = str.lastIndexOf(delimChar);
        day = parseInt(str.substring(0, delim1), 10);
        mo = parseInt(str.substring(delim1 + 1, delim2), 10);
        yr = parseInt(str.substring(delim2 + 1), 10);
        var testDate = new Date(yr, mo - 1, day);

        if (testDate.getDate() != day || testDate.getMonth() + 1 != mo || testDate.getFullYear() != yr) {
            return false;
        }
    }
    else
        return false;

    return true;
}

function isBatchNoValid(lcBatchNo) {
    var batchNo;
    var re;

    re = /LCB/g;
    batchNo = lcBatchNo.toUpperCase().replace(re, "");
    return !isNaN(parseInt(batchNo));
}

function getDefaultDate(ckb, textboxName, defaultValue) {
    if (defaultValue != "")
        d = Date.parseLocale(defaultValue, 'dd/MM/yyyy');
    else
        d = new Date();

    var obj = document.getElementById(textboxName);
    var day = d.getDate();

    var month = d.getMonth() + 1;
    if (day < 10) {
        day = "0" + day.toString();
    }

    if (month < 10)
        month = "0" + month.toString();

    if (obj != null) {
        if (ckb.checked)
            obj.value = day + "/" + month + "/" + d.getFullYear();
        else
            obj.value = "";
    }
}

function checkMultiLineTextboxMaxLength(text, long) {
    var maxlength = new Number(long); // Change number to your max length.
    if (text.value.length > maxlength) {
        text.value = text.value.substring(0, maxlength);
        alert(" Only allow " + long + " characters.");
    }
}