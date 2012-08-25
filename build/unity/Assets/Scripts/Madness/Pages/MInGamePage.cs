using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MInGamePage : MPage, FMultiTouchableInterface
{
	
	private FSprite _background;
	private FButton _backButton;
	
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
		
		_backButton = new FButton("CircleButtonBG_normal.png", "CircleButtonBG_over.png", "ClickSound");
		_backButton.AddLabel("Cubano","BACK!",Color.white);
		AddChild (_backButton);
		
		_backButton.x = -Futile.screen.halfWidth+50;
		_backButton.y = Futile.screen.halfHeight-50;
		
		_backButton.SignalRelease += HandleStartButtonRelease;
		
	}
	
	private void HandleStartButtonRelease (FButton button)
	{
		MMain.instance.GoToPage(MPageType.TitlePage);
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

