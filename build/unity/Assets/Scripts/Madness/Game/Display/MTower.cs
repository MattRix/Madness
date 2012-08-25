using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MTower : FContainer
{
	private MPlayer _player;
	private FSprite _sprite;
		
	public MTower(MPlayer player)
	{
		_player = player;
		AddChild(_sprite = new FSprite("Tower.png"));
		_sprite.color = _player.color.color;
	}
}
