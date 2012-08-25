using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MGame 
{
	public static MGame instance;
	
	public FContainer container;
	
	private List<MPlayer> _players = new List<MPlayer>();
	private MPlayer _human;
	private List<MTower> _towers = new List<MTower>();
	
	public MGame(FContainer container)
	{
		instance = this;
		this.container = container;
		
		_players.Add(new MPlayer(true,"YOU",MColor.Green));
		_players.Add(new MPlayer(false,"ASTER",MColor.Red));
		_players.Add(new MPlayer(false,"ALBERT",MColor.Blue));
		
		_human = _players[0];
		
		CreateTowers();
		
	}

	private void CreateTowers ()
	{
		float angle = 180.0f;
		
		
		foreach(MPlayer player in _players)
		{
			MTower tower = new MTower(player);
			tower.x = Mathf.Sin (angle*RXMath.DTOR) * MConfig.TOWER_CREATION_RADIUS;
			tower.y = Mathf.Cos (angle*RXMath.DTOR) * MConfig.TOWER_CREATION_RADIUS;
			
			Debug.Log ("MAKE TOWER"+tower.x+"m"+tower.y);
			
			container.AddChild(tower);
			_towers.Add (tower);
			
			angle += 360.0f/(float)_players.Count;
		}
	}
	
	public void Destroy()
	{
		
	}
}
