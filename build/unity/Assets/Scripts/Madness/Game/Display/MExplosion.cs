using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MExplosion : FSprite
{
	static private int _explosionFrameCount = 16;
	static private FAtlasElement[] _beastExplosionElements;
	static private FAtlasElement[] _towerExplosionElements;
	
	static public void Init()
	{
		_beastExplosionElements = new FAtlasElement[_explosionFrameCount];
		_towerExplosionElements = new FAtlasElement[_explosionFrameCount];
		
		for(int e = 0; e<_explosionFrameCount; e++)
		{
			_beastExplosionElements[e] = Futile.atlasManager.GetElementWithName("BeastExplosion_"+e+".png");	
		}
		
		for(int e = 0; e<_explosionFrameCount; e++)
		{
			_towerExplosionElements[e] = Futile.atlasManager.GetElementWithName("BeastExplosion_"+e+".png");	
		}
	}
	
	public bool isBeast;
	private int _frameCount = 0;
	
	public MExplosion(bool isBeast) : base("BeastExplosion_0.png")
	{
		this.isBeast = isBeast;
		
		if(isBeast)
		{
			this.scale = 0.6f;	
		}
	}
	
	override public void HandleAddedToStage()
	{
		Futile.instance.SignalUpdate += HandleUpdate;
		base.HandleAddedToStage();	
	}
	
	override public void HandleRemovedFromStage()
	{
		Futile.instance.SignalUpdate -= HandleUpdate;
		base.HandleRemovedFromStage();	
	}
	
	private void HandleUpdate()
	{
		int useFrame = _frameCount/2; //hold on each animation frame for 2 frames;
		
		if(isBeast)
		{
			this.element = _beastExplosionElements[useFrame];
		}
		else 
		{
			this.element = _towerExplosionElements[useFrame];
		}
			
		if(useFrame >= _explosionFrameCount-1)
		{
			this.RemoveFromContainer();	//remove us, we're done!
		}
		
		_frameCount++;
	}
}
