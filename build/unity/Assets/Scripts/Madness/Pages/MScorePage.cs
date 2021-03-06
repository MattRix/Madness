using UnityEngine;
using System.Collections;
using System;

public class MScorePage : MPage
{
	private FSprite _background;
	private FButton _againButton;

	public MScorePage()
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
		
		_againButton = new FButton("CircleButtonBG_normal.png", "CircleButtonBG_over.png", "Click");
		_againButton.AddLabel("Cubano","AGAIN!",Color.white);
		AddChild (_againButton);
		
		_againButton.x = -Futile.screen.halfWidth+100;
		_againButton.y = Futile.screen.halfHeight-100;
		
		_againButton.SignalRelease += HandleStartButtonRelease;
		
		this.alpha = 0.0f;
		Go.to (this, 0.5f, new TweenConfig().floatProp("alpha",1.0f));
	}
	
	private void HandleStartButtonRelease (FButton button)
	{
		MMain.instance.GoToPage(MPageType.TitlePage);
	}
	
	protected void HandleUpdate ()
	{
		
	}

}

