using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple=true, Inherited=true)]
public class HelpAttribute : Attribute
{
	public readonly string toolTip = "";
	public readonly string description = "";

	public HelpAttribute(string description, string toolTip)
	{
		this.description = description;
		this.toolTip = toolTip;
	}

	
	public HelpAttribute(string description)
	{
		this.description = description;
	}
}
