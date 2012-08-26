using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MEffectLayer : FContainer
{
		
	public MEffectLayer()
	{
	
	}
	
	public void ShowCrosshairForPlayer(MPlayer player, Vector2 position)
	{
		FSprite crosshair = new FSprite("Crosshair.png");
		crosshair.shader = FShader.Additive;
		crosshair.color = player.color.color;
		
		AddChild(crosshair);
		crosshair.x = position.x;
		crosshair.y = position.y;
		
		Go.to (crosshair,0.2f,new TweenConfig().setDelay(0.1f).floatProp("alpha", 0.0f).onComplete(HandleCrosshairComplete));
		
	}
	
	private void HandleCrosshairComplete(AbstractTween tween)
	{
		FSprite sprite = (tween as Tween).target as FSprite;
		sprite.RemoveFromContainer();
	}
	
}
