using com.next.common.domain;
using System;

[Serializable]
public class NSLedItemSummaryDef : DomainData
{
	private string itemNo;

	private decimal nsCommAmtInUSD;

	private decimal nsCommAmt;

	private int netQty;

	private decimal netAmt;

	private string customerType;

	public string ItemNo
	{
		get
		{
			return itemNo;
		}
		set
		{
			itemNo = value;
		}
	}

	public decimal NSCommAmtInUSD
	{
		get
		{
			return nsCommAmtInUSD;
		}
		set
		{
			nsCommAmtInUSD = value;
		}
	}

	public decimal NSCommAmt
	{
		get
		{
			return nsCommAmt;
		}
		set
		{
			nsCommAmt = value;
		}
	}

	public int NetQty
	{
		get
		{
			return netQty;
		}
		set
		{
			netQty = value;
		}
	}

	public decimal NetAmt
	{
		get
		{
			return netAmt;
		}
		set
		{
			netAmt = value;
		}
	}

	public string CustomerType
	{
		get
		{
			return customerType;
		}
		set
		{
			customerType = value;
		}
	}
}
