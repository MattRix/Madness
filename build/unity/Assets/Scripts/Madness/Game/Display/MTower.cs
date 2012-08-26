using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MTower : FContainer
{
	public MPlayer player;
	
	private FSprite _sprite;
		
	public MTower(MPlayer player)
	{
		this.player = player;
		
		AddChild(_sprite = new FSprite("Tower.png"));
		_sprite.color = player.color.color;
	}
}
