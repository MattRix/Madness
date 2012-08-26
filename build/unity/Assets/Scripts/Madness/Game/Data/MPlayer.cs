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
	
	public float angle;
	public MTower tower;
	
	public int maxBeasts = 80;
		
	public int maxFramesTillBeast = 30;//180;
	public int framesTillBeast;
	
	public float nextBeastCreationAngle = 0.0f;
	
	public List<MBeast> beasts = new List<MBeast>(80);
	
	public bool isDead = false;
	
	public int dna = 0;
	
	public MPlayer(int index, bool isHuman, string name, MColor color)
	{
		this.index = index;
		this.isHuman = isHuman;
		this.name = name;
		this.color = color;
		
		framesTillBeast = maxFramesTillBeast;
	}
}
