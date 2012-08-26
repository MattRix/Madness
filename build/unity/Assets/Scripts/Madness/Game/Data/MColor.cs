using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MColor 
{
	public static MColor[] colors = new MColor[3];
	
	public static MColor Red = new MColor(0, "red", new Color(1.0f,0.0f,0.1f,1.0f), new Color(1.0f,0.3f,0.3f,1.0f));
	public static MColor Green = new MColor(1, "green", new Color(0.3f,1.0f,0.0f,1.0f), new Color(0.3f,1.0f,0.3f,1.0f));
	public static MColor Blue = new MColor(2, "blue", new Color(0.0f,0.4f,1.0f,1.0f), new Color(0.3f,0.5f,1.0f,1.0f));
	
	public int index;
	public string name;
	public Color color;
	public Color addColor; //used for additiveColor mode, it's subtler
	public Color beastColor; //used for beasts, it's brighter
		
	public MColor(int index, string name, Color color, Color beastColor)
	{
		this.index = index;
		this.name = name;
		this.color = color;
		this.addColor = new Color(color.r*0.3f,color.g*0.1f,color.b*0.3f,1.0f);
		this.beastColor = beastColor;
		
		colors[index] = this;
	}
}
