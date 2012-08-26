using UnityEngine;
using System.Collections;
using System;

public class MTitlePage : MPage
{
	private FSprite _background;
	private FSprite _logo;
	private FButton _startButton;
	
	public MTitlePage()
	{
		
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
	
	
	override public void Start()
	{
		_background = new FSprite("Background.png");
		AddChild(_background);
	
		_logo = new FSprite("TitleLogo.png");
		_logo.x += 10;
		_logo.y -= 10;
		AddChild(_logo);
		
		_startButton = new FButton("CircleButtonBG_normal.png", "CircleButtonBG_over.png", "ClickSound");
		_startButton.AddLabel("Cubano","START!",Color.white);
		
		_startButton.x = Futile.screen.halfWidth-100;
		_startButton.y = -Futile.screen.halfHeight+100;
		
		AddChild(_startButton);

		_startButton.SignalRelease += HandleStartButtonRelease;
		
	}
	
	private void HandleStartButtonRelease (FButton button)
	{
		MMain.instance.GoToPage(MPageType.InGamePage);
	}
	
	protected void HandleUpdate ()
	{
		
	}

}

