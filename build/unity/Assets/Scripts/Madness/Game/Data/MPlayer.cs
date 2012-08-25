using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MPlayer 
{
	public bool isHuman;
	public string name;
	public MColor color;
	
		
	public MPlayer(bool isHuman, string name, MColor color)
	{
		this.isHuman = isHuman;
		this.name = name;
		this.color = color;
	}
}
