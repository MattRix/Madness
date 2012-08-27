using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MEffectLayer : FContainer
{
	private int _nextAttackMarkIndex = 0;
	private int _attackMarkCount = 30;
	private FSprite[] _attackMarks;
		
	public MEffectLayer()
	{
		_attackMarks = new FSprite[_attackMarkCount];
		for(int a = 0; a<_attackMarkCount; a++)
		{
			FSprite attackMark = new FSprite("AttackMark.png");
			attackMark.shader = FShader.Additive;
			attackMark.alpha = 0.0f;
			
			AddChild(attackMark);
			_attackMarks[a] = attackMark;
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
		for(int a = 0; a<_attackMarkCount; a++)
		{
			FSprite attackMark = _attackMarks[a];
			
			if(attackMark.alpha > 0.0f)
			{
				attackMark.alpha -= 0.05f;	
			}
		}
	}
	
	//TODO: use something other than GoTween to do this... seems really overkill for something that happens ALL the time
	public void ShowAttackMarkForPlayer(MPlayer player, Vector2 position)
	{
		FSprite attackMark = _attackMarks[_nextAttackMarkIndex];
		attackMark.color = player.color.addColor;
		
		attackMark.x = position.x;
		attackMark.y = position.y;
		
		attackMark.alpha = 1.0f;
		
		_nextAttackMarkIndex = (_nextAttackMarkIndex+1)%_attackMarkCount;
	}

	
	

	public void CreateDNA (MPlayer player, Vector2 position)
	{
		FSprite coin = new FSprite("Coin.png");
		
		coin.shader = FShader.Additive;
		coin.color = player.color.color;
		
		AddChild(coin);
		
		coin.x = position.x;
		coin.y = position.y;
		coin.data = player;
		
		coin.scale = 1.0f;
		coin.alpha = 0.1f;
		
		Go.to (coin,0.8f,new TweenConfig().floatProp("alpha", 1.0f).floatProp("x",player.tower.x).floatProp("y",player.tower.y).onComplete(HandleDNAComplete));
		Go.to (coin,0.3f,new TweenConfig().floatProp("scale",0.3f).setDelay(0.5f));
	}
	
	private void HandleDNAComplete(AbstractTween tween)
	{
		FSprite coin = (tween as Tween).target as FSprite;
		coin.RemoveFromContainer();
		MGame.instance.AddDNA(coin.data as MPlayer);
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
