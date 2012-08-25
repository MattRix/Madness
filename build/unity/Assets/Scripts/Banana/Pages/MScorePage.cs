using UnityEngine;
using System.Collections;
using System;

public class MScorePage : MPage
{
	private FSprite _background;

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
		
	}
	
	protected void HandleUpdate ()
	{
		
	}

}

