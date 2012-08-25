using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MColor 
{
	public static MColor Red = new MColor("red", new Color(1.0f,0.0f,0.1f,1.0f));
	public static MColor Green = new MColor("green", new Color(0.3f,1.0f,0.0f,1.0f));
	public static MColor Blue = new MColor("blue", new Color(0.0f,0.4f,1.0f,1.0f));
	
	public string name;
	public Color color;
		
	public MColor(string name, Color color)
	{
		this.name = name;
		this.color = color;
	}
}
