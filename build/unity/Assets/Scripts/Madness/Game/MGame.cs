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
	
	private List<MWad> _wads = new List<MWad>();
	
	public int frameCount = 0;
	
	public MGame(FContainer container)
	{
		instance = this;
		this.container = container;
		
		_players.Add(new MPlayer(0, true,"YOU",MColor.Green));
		_players.Add(new MPlayer(1, false,"RED",MColor.Red));
		_players.Add(new MPlayer(2, false,"BLUE",MColor.Blue));
		
		_human = _players[0];
		
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
		MWad wad = GetNewWad();
		wad.Start(player);
		_wadContainer.AddChild(wad);
		_wads.Add(wad); 
		player.wads.Add(wad);
	}
	
	public void RemoveWad(MWad wad)
	{
		wad.Destroy();
		_wads.Remove(wad);
		wad.player.wads.Remove(wad); 
		MWad.pool.Add (wad);
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
