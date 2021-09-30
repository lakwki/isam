

	/**
	* DHTML email validation script. 
	*/
	function CheckControlValue_IsEmail(IsMandatory)
	{
		var oSrc = event.srcElement
		var SrcValue = oSrc.value
		//var SrcName = oSrc.name
		
		if ((SrcValue == "") && IsMandatory)
		{
			alert("This field is mandatory!")
			oSrc.focus()
			oSrc.select()
			return false
		}
		else if ((SrcValue == "") && !IsMandatory)
		{
			return true;
		}
		
		apos=SrcValue.indexOf("@"); 
		dotpos=SrcValue.lastIndexOf(".");
		lastpos=SrcValue.length-1;
		
		if (apos<1 || dotpos-apos<2 || lastpos-dotpos>3 || lastpos-dotpos<2) 
		{
			alert("Please input a valid email!")
			oSrc.focus()
			oSrc.select()
			return false
		}
		else 
		{
			return true;
		}
	} 


	/**
	* DHTML integer validation script. 
	*/
	function CheckControlValue_IsInteger(IsMandatory)
	{
		var oSrc = event.srcElement
		var SrcValue = oSrc.value
		//var SrcName = oSrc.name

		if ((SrcValue == "") && IsMandatory)
		{
			alert("This field is mandatory!")
			oSrc.focus()
			oSrc.select()
			return false
		}
		else if ((SrcValue == "") && !IsMandatory)
		{
			return true;
		}
		
		
		if (isNaN(SrcValue))
		{
			alert("Please input a valid integer!")
			oSrc.focus()
			oSrc.select()
			return false
		}
		else
		{	
			oSrc.value = String(Math.abs(parseInt(SrcValue)))
		}
		return true
		
	}


	/**
	* DHTML numeric validation script. 
	*/
	function CheckControlValue_IsNumeric(IsMandatory)
	{
		var ValidChars = "0123456789.";
		var Char;

		var oSrc = event.srcElement
		var SrcValue = oSrc.value
		//var SrcName = oSrc.name


		if ((SrcValue == "") && IsMandatory)
		{
			alert("This field is mandatory!")
			oSrc.focus()
			oSrc.select()
			return false
		}
		else if ((SrcValue == "") && !IsMandatory)
		{
			return true;
		}
		
		CheckControlValue_IsNumeric = true
		
		for (i = 0; i < SrcValue.length && CheckControlValue_IsNumeric == true; i++) 
		{ 
			Char = SrcValue.charAt(i); 
			if (ValidChars.indexOf(Char) == -1) 
			{
				alert("Please input a valid number!")
				oSrc.focus()
				oSrc.select()
				return false
			}
		}
		return true;
   
   }


 
 

	/**
	* DHTML date validation script. 
	*/
	// Declaring valid date character, minimum year and maximum year
	var dtCh= "/";
	var minYear=1900;
	var maxYear=2100;

	function isInteger(s){
		var i;
		for (i = 0; i < s.length; i++){   
			// Check that current character is number.
			var c = s.charAt(i);
			if (((c < "0") || (c > "9"))) return false;
		}
		// All characters are numbers.
		return true;
	}

	function stripCharsInBag(s, bag){
		var i;
		var returnString = "";
		// Search through string's characters one by one.
		// If character is not in bag, append to returnString.
		for (i = 0; i < s.length; i++){   
			var c = s.charAt(i);
			if (bag.indexOf(c) == -1) returnString += c;
		}
		return returnString;
	}

	function daysInFebruary (year){
		// February has 29 days in any year evenly divisible by four,
		// EXCEPT for centurial years which are not also divisible by 400.
		return (((year % 4 == 0) && ( (!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28 );
	}
	function DaysArray(n) {
		for (var i = 1; i <= n; i++) {
			this[i] = 31
			if (i==4 || i==6 || i==9 || i==11) {this[i] = 30}
			if (i==2) {this[i] = 29}
	} 
	return this
	}


	function CheckControlValue_IsDate_WithDelete(IsMandatory, refDate)
	{
		var oSrc = event.srcElement
		var dtStr = oSrc.value

		if (dtStr == "D" || dtStr == "d")
		{
			return true;
		}
	
		return CheckControlValue_IsDate(IsMandatory, refDate);
	}


	function CheckControlValue_IsDate(IsMandatory, refDate)
	{
		var oSrc = event.srcElement
		var dtStr = oSrc.value
		//var SrcName = oSrc.name

		if ((dtStr == "") && IsMandatory)
		{
			alert("This field is mandatory!")
			oSrc.focus()
			oSrc.select()
			return false
		}
		else if ((dtStr == "") && !IsMandatory)
		{
			return true;
		}

		var daysInMonth = DaysArray(12)
		var pos1=dtStr.indexOf(dtCh)
		var pos2=dtStr.indexOf(dtCh,pos1+1)

		var strDay;
		var strMonth;
		var strYear;
		
		if ( pos2==-1 )
		{
			strDay=dtStr.substring(0,pos1)
			strMonth=dtStr.substring(pos1+1)
			
			/*
			if (refDate == null)
			{	
				strYear = ((new Date()).getFullYear()).toString();
			}
			else
			{				
				var refPos1=refDate.indexOf(dtCh)
				var refPos2=refDate.indexOf(dtCh,refPos1+1)
				
				var refDay=refDate.substring(0,refPos1)
				var refMonth=refDate.substring(refPos1+1,refPos2)
				var refYear=refDate.substring(refPos2+1)
				
				if (parseInt(strMonth) > parseInt(refMonth) + 3)
				{
					strYear = refYear;
					alert("1");
				}
				else if (parseInt(refMonth) == parseInt(strMonth) + 3)
				{	
					if (parseInt(strDay) >= parseInt(refDay))
					{
						strYear = refYear;
						alert("2");
					}
					else
					{
						strYear = (parseInt(refYear) + 1).toString();
						alert("3");
					}
				}
				else
				{
					strYear = (parseInt(refYear) + 1).toString();
					alert("4");
				}
			}
			*/
			
			strYear = ((new Date()).getFullYear()).toString();

			dtStr = strDay + "/" + strMonth + "/" + strYear;
			pos1=dtStr.indexOf(dtCh)
			pos2=dtStr.indexOf(dtCh,pos1+1)

		}
		else
		{
			strDay=dtStr.substring(0,pos1)
			strMonth=dtStr.substring(pos1+1,pos2)
			strYear=dtStr.substring(pos2+1)
			
			if (strYear.length == 2)
			{
				strYear = ((new Date()).getFullYear()).toString().substring(0,2) + strYear;
			}
			
		}
		
		strYr=strYear
		if (strDay.charAt(0)=="0" && strDay.length>1) strDay=strDay.substring(1)
		if (strMonth.charAt(0)=="0" && strMonth.length>1) strMonth=strMonth.substring(1)
		for (var i = 1; i <= 3; i++) {
			if (strYr.charAt(0)=="0" && strYr.length>1) strYr=strYr.substring(1)
		}
		month=parseInt(strMonth)
		day=parseInt(strDay)
		year=parseInt(strYr)
		if (strMonth.length<1 || month<1 || month>12){
			
			if (!IsMandatory)
			{
				if (confirm('Invalid month, do you want to clear the value?'))
				{
					oSrc.value = "";
					return true;
				}
				else
				{
					oSrc.focus()
					oSrc.select()
				}
			}
			else
			{
				alert("Please enter a valid month")
				oSrc.focus()
				oSrc.select()
			}
			
			return false
		}
		if (strDay.length<1 || day<1 || day>31 || (month==2 && day>daysInFebruary(year)) || day > daysInMonth[month]){

			if (!IsMandatory)
			{
				if (confirm('Invalid day, do you want to clear the value?'))
				{
					oSrc.value = "";
					return true;
				}
				else
				{
					oSrc.focus()
					oSrc.select()
				}
			}
			else
			{
				alert("Please enter a valid day")
				oSrc.focus()
				oSrc.select()
			}
			return false
		}
		if (strYear.length != 4 || year==0 || year<minYear || year>maxYear){

			if (!IsMandatory)
			{
				if (confirm('Invalid year, do you want to clear the value?'))
				{
					oSrc.value = "";
					return true;
				}
				else
				{
					oSrc.focus()
					oSrc.select()
				}
			}
			else
			{
				alert("Please enter a valid 4 digit year between "+minYear+" and "+maxYear)
				oSrc.focus()
				oSrc.select()
			}
			return false
		}
		if (dtStr.indexOf(dtCh,pos2+1)!=-1 || isInteger(stripCharsInBag(dtStr, dtCh))==false){
			if (!IsMandatory)
			{
				if (confirm('Invalid date, do you want to clear the value?'))
				{
					oSrc.value = "";
					return true;
				}
				else
				{
					oSrc.focus()
					oSrc.select()
				}
			}
			else
			{
				alert("Please enter a valid date")
				oSrc.focus()
				oSrc.select()
			}
			return false
		}
	
	if (strDay.length == 1)
		strDay = "0" + strDay

	if (strMonth.length == 1)
		strMonth = "0" + strMonth
	
	//if (strYear.length == 4)
	//	strYear = strYear.substring(2);
	
	oSrc.value = strDay + "/" + strMonth + "/" + strYear;
	return true
	}

