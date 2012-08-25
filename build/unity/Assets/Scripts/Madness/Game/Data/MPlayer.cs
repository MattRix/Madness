using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MPlayer 
{
	public int index;
	public bool isHuman;
	public string name;
	public MColor color;
	
	public int maxFramesTillWad = 50;
	public int framesTillWad;
	
	public List<MWad> wads = new List<MWad>();
		
	public MPlayer(int index, bool isHuman, string name, MColor color)
	{
		this.index = index;
		this.isHuman = isHuman;
		this.name = name;
		this.color = color;
		
		framesTillWad = maxFramesTillWad;
	}
}
