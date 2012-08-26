using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MWad : FContainer
{
	public static List<MWad> pool = new List<MWad>();
	
	private static FAtlasElement[] _walkElements;
	private static FAtlasElement[] _attackElements;
	private static FAtlasElement[] _walkAndAttackElements;
	
	public float radius = 20.0f;
	
	public MPlayer player;
	
	private FSprite _sprite;
	
	private bool _isEnabled = false;
	
	public Vector2 velocity = new Vector2(0,0);
	
	public bool hasTarget = false;
	
	public Vector2 target = new Vector2(0,0);
	
	public static void Init()
	{
		_walkElements = new FAtlasElement[10];
		_attackElements = new FAtlasElement[10];
		_walkAndAttackElements = new FAtlasElement[_walkElements.Length + _attackElements.Length];
		
		int allIndex = 0;
		
		for(int f = 0; f<_walkElements.Length; f++)
		{
			_walkAndAttackElements[allIndex++] = _walkElements[f] = Futile.atlasManager.GetElementWithName("Wad_walking_"+f+".png");	
		}
		
		for(int f = 0; f<_attackElements.Length; f++)
		{
			_walkAndAttackElements[allIndex++] = _attackElements[f] = Futile.atlasManager.GetElementWithName("Wad_attacking_"+f+".png");	
		}
	} 
	
	public MWad()
	{
		AddChild(_sprite = new FSprite(_walkElements[0].name));
		//_sprite.shader = FShader.AdditiveColor;
	}
	
	public void Start(MPlayer player)
	{
		this.player = player;
		
		_sprite.color = player.color.wadColor;
		
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
