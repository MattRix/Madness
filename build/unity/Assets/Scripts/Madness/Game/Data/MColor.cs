using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MColor 
{
	public static MColor Red = new MColor("red", new Color(1.0f,0.0f,0.1f,1.0f), new Color(1.0f,0.6f,0.62f,1.0f));
	public static MColor Green = new MColor("green", new Color(0.3f,1.0f,0.0f,1.0f), new Color(0.8f,1.0f,0.4f,1.0f));
	public static MColor Blue = new MColor("blue", new Color(0.0f,0.4f,1.0f,1.0f), new Color(0.7f,0.8f,1.0f,1.0f));
	
	public string name;
	public Color color;
	public Color addColor; //used for additiveColor mode, it's subtler
	public Color wadColor; //used for wads, it's brighter
		
	public MColor(string name, Color color, Color wadColor)
	{
		this.name = name;
		this.color = color;
		this.addColor = new Color(color.r*0.3f,color.g*0.1f,color.b*0.3f,1.0f);
		this.wadColor = wadColor;
	}
}
