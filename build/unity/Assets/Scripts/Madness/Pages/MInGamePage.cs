using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MInGamePage : MPage
{
	
	private FSprite _background;
	private FButton _backButton;
	
	private MGame _game;
	
	public MInGamePage()
	{
		
	}
	
	override public void HandleAddedToStage()
	{
		base.HandleAddedToStage();	
	}
	
	override public void HandleRemovedFromStage()
	{
		
		_game.Destroy();
		base.HandleRemovedFromStage();	
	}
	
	override public void Start()
	{
		_background = new FSprite("Background.png");
		AddChild(_background);
		
		_backButton = new FButton("CircleButtonBG_normal.png", "CircleButtonBG_over.png", "Click");
		_backButton.AddLabel("Cubano","BACK!",Color.white);
		AddChild (_backButton);
		
		_backButton.x = -Futile.screen.halfWidth+100;
		_backButton.y = Futile.screen.halfHeight-100;
		
		_backButton.SignalRelease += HandleStartButtonRelease;
		
		_game = new MGame(this);
		
		this.alpha = 0;
		Go.to (this, 0.5f, new TweenConfig().floatProp("alpha",1.0f));
	}
	
	private void HandleStartButtonRelease (FButton button)
	{
		MMain.instance.GoToPage(MPageType.TitlePage);
	}
	
	public void ShowWin()
	{
		//MMain.instance.GoToPage(MPageType.ScorePage);
	}
	
	public FButton backButton
	{
		get {return _backButton;}	
	}

}

