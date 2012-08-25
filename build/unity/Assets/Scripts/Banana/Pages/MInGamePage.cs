using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MInGamePage : MPage, FMultiTouchableInterface
{
	
	private FSprite _background;
	
	public MInGamePage()
	{
		
	}
	
	override public void HandleAddedToStage()
	{
		Futile.touchManager.AddMultiTouchTarget(this);
		Futile.instance.SignalUpdate += HandleUpdate;
		base.HandleAddedToStage();	
	}
	
	override public void HandleRemovedFromStage()
	{
		Futile.touchManager.RemoveMultiTouchTarget(this);
		Futile.instance.SignalUpdate -= HandleUpdate;
		base.HandleRemovedFromStage();	
	}
	
	override public void Start()
	{
		_background = new FSprite("Background.png");
		AddChild(_background);
	}
	


	protected void HandleUpdate ()
	{
	}
	
	public void HandleMultiTouch(FTouch[] touches)
	{
		foreach(FTouch touch in touches)
		{
			if(touch.phase == TouchPhase.Began)
			{
				
			}
		}
	}
	

}

