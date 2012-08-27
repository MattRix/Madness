using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MTower : FContainer
{
	static private int _towerFrameCount = 10;
	static private FAtlasElement[] _towerElements;
	
	static public void Init()
	{
		_towerElements = new FAtlasElement[_towerFrameCount];
		
		for(int e = 0; e<_towerFrameCount; e++)
		{
			_towerElements[e] = Futile.atlasManager.GetElementWithName("Tower_"+e+".png");	
		}
	}
	
	public MPlayer player;
	
	public float maxHealth = 250.0f;
	public float health;
	
	private FSprite _sprite;
		
	public MTower(MPlayer player)
	{
		this.player = player;
		
		health = maxHealth;
		
		AddChild(_sprite = new FSprite(_towerElements[0].name));
		_sprite.color = player.color.color;
		
		UpdateHealthPercent();
	}
	
	public void UpdateHealthPercent()
	{
		int healthFrame = (int) Math.Max(0,Math.Min(9,Mathf.Floor((1.0f-(health/maxHealth))*10.0f)));
		_sprite.element = _towerElements[healthFrame];
		player.HandleTowerHit();
	}
}
