using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MGame : FMultiTouchableInterface
{
	public static MGame instance;
	
	public FContainer container;
	
	private FContainer _wadContainer;
	
	private List<MPlayer> _players = new List<MPlayer>();
	private MPlayer _human;
	private List<MTower> _towers = new List<MTower>();
	
	private int _wadCount = 0;
	private MWad[] _wads;
	
	public int frameCount = 0;
	
	public MGame(FContainer container)
	{
		instance = this;
		this.container = container;
		
		_players.Add(new MPlayer(0, true,"YOU",MColor.Green));
		_players.Add(new MPlayer(1, false,"RED",MColor.Red));
		_players.Add(new MPlayer(2, false,"BLUE",MColor.Blue));
		
		_human = _players[0];
		
		_wads = new MWad[_players.Count * (_human.maxWads + 20)];
		
		CreateTowers();
		
		container.AddChild(_wadContainer = new FContainer());
		
		Futile.instance.SignalUpdate += HandleUpdate;
		Futile.touchManager.AddMultiTouchTarget(this);
	}
	
	public void Destroy()
	{
		Futile.touchManager.RemoveMultiTouchTarget(this);
		Futile.instance.SignalUpdate -= HandleUpdate;
		MWad.pool.Clear();
		Futile.instance.shouldRunGCNextUpdate = true;
	}

	private void CreateTowers ()
	{
		float angle = 180.0f;
		
		foreach(MPlayer player in _players)
		{
			MTower tower = new MTower(player);
			
			player.angle = angle;
			player.tower = tower;
			
			tower.x = Mathf.Sin (angle*RXMath.DTOR) * MConfig.TOWER_CREATION_RADIUS;
			tower.y = Mathf.Cos (angle*RXMath.DTOR) * MConfig.TOWER_CREATION_RADIUS;
			
			container.AddChild(tower);
			_towers.Add (tower);
			
			angle += 360.0f/(float)_players.Count;
		}
	}
	
	protected void HandleUpdate ()
	{
		for(int p = 0; p<_players.Count; p++)
		{
			MPlayer player = _players[p];
			
			player.framesTillWad--;
			
			if(player.framesTillWad == 0)
			{
				player.framesTillWad = player.maxFramesTillWad;
				
				CreateWad(player);	
			}
		}
		
		for(int w = 0; w<_wadCount; w++)
		{
			MWad wad = _wads[w];
		}
		
		
		frameCount++;
	}
	
	public MWad GetNewWad()
	{
		MWad wad = (MWad.pool.Count == 0) ? new MWad() : MWad.pool.Pop();
		
		_wadContainer.AddChild(wad);
		return wad; 
	}
	
	public void CreateWad(MPlayer player)
	{
		if(player.wads.Count >= player.maxWads) return; //TODO: Show a "max wads limit reached!" indicator on screen
		
		MWad wad = GetNewWad();
		wad.Start(player);
		_wadContainer.AddChild(wad);
		if(_wads.Length <= _wadCount)
		{
			Array.Resize(ref _wads,_wadCount+20);	
		}
		_wads[_wadCount++] = wad; 
		player.wads.Add(wad);
		
		float creationAngle = player.angle + player.nextWadCreationAngle;
		
		wad.x = player.tower.x + Mathf.Sin (creationAngle*RXMath.DTOR) * (player.tower.radius+wad.radius); 
		wad.y = player.tower.y + Mathf.Cos (creationAngle*RXMath.DTOR) * (player.tower.radius+wad.radius); 
		
		player.nextWadCreationAngle = (player.nextWadCreationAngle + 30.0f)%360.0f;
	}
	
	public void RemoveWad(MWad wadToRemove)
	{
		wadToRemove.Destroy();
		_wads.RemoveItem(wadToRemove, ref _wadCount);
		
		wadToRemove.player.wads.Remove(wadToRemove); 
		MWad.pool.Add (wadToRemove);
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
