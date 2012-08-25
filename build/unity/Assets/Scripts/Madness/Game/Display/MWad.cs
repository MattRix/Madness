using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MWad : FContainer
{
	public static List<MWad> pool = new List<MWad>();

	public float radius = 20.0f;
	
	public MPlayer player;
	
	private FSprite _sprite;
	
	private bool _isEnabled = false;
	
	
		
	public MWad()
	{
		AddChild(_sprite = new FSprite("Tower.png"));
	}
	
	public void Start(MPlayer player)
	{
		this.player = player;
		
		_sprite.color = player.color.color;
		
		this.isEnabled = true;
	}
	
	public void Destroy()
	{
		this.isEnabled = false;
	}
	
	public bool isEnabled
	{
		get {return _isEnabled;}
		set 
		{
			if(_isEnabled != value)
			{
				_isEnabled = value;
				
				if(_isEnabled)
				{
					_sprite.isVisible = true;	
				}
				else
				{
					_sprite.isVisible = false;	
				}
			}
		}
	}

}
