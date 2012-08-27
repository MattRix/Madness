using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MPlayerAI
{
	public MPlayer player;
	
	public int framesTillNextAction = 100;
	
	public List<MPlayer>otherPlayers = new List<MPlayer>();
	
	public MPlayerAI (MPlayer player, List<MPlayer>players)
	{
		this.player = player;
		
		foreach(MPlayer otherPlayer in players)
		{
			if(otherPlayer != player)
			{
				otherPlayers.Add(otherPlayer);	
			}
		}
		
		framesTillNextAction = RXRandom.Range(200,400);
		
	}
	
	public void Update()
	{
		if(player.isDead) return; //we're dead!
		
		//CHECK FOR AVAILABLE UPGRADES, AND BUY THE ONES WE CAN AFFORD!
		
		if(RXRandom.Float() < 1.0f/500.0f) //only buy upgrades sometimes
		{
			
			if(player.statAttack.CanBuy()) player.statAttack.Buy();
			if(player.statDefence.CanBuy()) player.statDefence.Buy();
			if(player.statHealth.CanBuy()) player.statHealth.Buy();
			if(player.statSpeed.CanBuy()) player.statSpeed.Buy();
			
		}
		
		
		framesTillNextAction--;
		
		if(framesTillNextAction == 0)
		{
			framesTillNextAction = RXRandom.Range(300,1600);
			
			if(RXRandom.Float() < 0.1f) //attack towards the center
			{
				MGame.instance.SetAttackTarget(player, new Vector2(0, 0));
			}
			else if(player.beasts.Count < 10) //regroup at home base
			{
				MGame.instance.SetAttackTarget(player, new Vector2(player.tower.x, player.tower.y));
			}
			else if(RXRandom.Float() < 0.3f) //attack player with most kills
			{
				int mostKills = -1;
				MPlayer mostKillsPlayer = null;
				
				for(int p = 0; p<otherPlayers.Count; p++)
				{
					if(!otherPlayers[p].isDead)
					{
						if(otherPlayers[p].totalKills > mostKills)
						{
							mostKillsPlayer = otherPlayers[p];
							mostKills = otherPlayers[p].totalKills;
						}
					}
				}
				
				if(mostKillsPlayer == null) return; //all the other players are dead, mwa ha ha!
				
				MGame.instance.SetAttackTarget(player, new Vector2(mostKillsPlayer.tower.x, mostKillsPlayer.tower.y));
			}
			else //attack player at random
			{
				int bestRandom = -1;
				MPlayer randomPlayer = null;
				
				for(int p = 0; p<otherPlayers.Count; p++)
				{
					if(!otherPlayers[p].isDead)
					{
						int playerRandom = RXRandom.Int(100);
						if(playerRandom > bestRandom)
						{
							randomPlayer = otherPlayers[p];
							bestRandom = playerRandom;
						}
					}
				}
				
				if(randomPlayer == null) return; //all the other players are dead, mwa ha ha!
				
				MGame.instance.SetAttackTarget(player, new Vector2(randomPlayer.tower.x, randomPlayer.tower.y));
			}
		}
	}
}

