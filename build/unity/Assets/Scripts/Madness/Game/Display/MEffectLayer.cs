using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MEffectLayer : FContainer
{
		
	public MEffectLayer()
	{
	
	}

	public void ShowBeastExplosionForBeast (MBeast beast)
	{
		MExplosion explosion = new MExplosion(true);
		explosion.x = beast.x;
		explosion.y = beast.y;
		explosion.rotation = RXRandom.Float()*360.0f;
		explosion.shader = FShader.Additive;
		explosion.color = beast.player.color.color;
		
		AddChild(explosion);
	}
	
	public void ShowTowerExplosionForTower (MTower tower)
	{
		MExplosion explosion = new MExplosion(false);
		explosion.x = tower.x;
		explosion.y = tower.y;
		explosion.rotation = RXRandom.Float()*360.0f;
		//explosion.shader = FShader.Additive;
		explosion.color = tower.player.color.addColor; //not that it's not additive but uses addColor
		
		AddChildAtIndex(explosion,0);
	}
	
	static public Color red = Color.red;
	
	public void ShowTowerHitForTower (MTower tower)
	{
		FSprite towerHit = new FSprite("TowerHit.png");
		towerHit.shader = FShader.Additive;
		towerHit.color = red; 
		
		AddChild(towerHit);
		towerHit.x = tower.x;
		towerHit.y = tower.y;
		
		Go.to (towerHit,0.2f,new TweenConfig().floatProp("alpha", 0.0f).onComplete(HandleTowerHitComplete));
	}
	
	private void HandleTowerHitComplete(AbstractTween tween)
	{
		FSprite sprite = (tween as Tween).target as FSprite;
		sprite.RemoveFromContainer();
	}
	
	//TODO: use something other than GoTween to do this... seems really overkill for something that happens ALL the time
	public void ShowAttackMarkForPlayer(MPlayer player, Vector2 position)
	{
		FSprite attackMark = new FSprite("AttackMark.png");
		attackMark.shader = FShader.Additive;
		attackMark.color = player.color.addColor;
		
		AddChild(attackMark);
		attackMark.x = position.x;
		attackMark.y = position.y;
		
		Go.to (attackMark,0.2f,new TweenConfig().floatProp("alpha", 0.0f).onComplete(HandleAttackMarkComplete));
	}
	
	private void HandleAttackMarkComplete(AbstractTween tween)
	{
		FSprite sprite = (tween as Tween).target as FSprite;
		sprite.RemoveFromContainer();
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
