using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MPlayerStat
{
	public int index;
	public string name;
	public int amount;
	public int max;
	
	public MPlayer player;
	
	public event Action<MPlayerStat> SignalChange;
	
	public MPlayerStat(int index, string name, int amount, int max)
	{
		this.index = index;
		this.name = name;
		this.amount = amount;
		this.max = max;
	}

	public bool CanBuy ()
	{
		return amount < max && player.dna >= (amount+1);
	}
	
	public void Buy()
	{
		if(!CanBuy()) return;
		
		MGame.instance.RemoveDNA(player, amount+1);
		amount = Math.Min(max,amount+1);
		if(SignalChange != null) SignalChange(this);
	}
	
	public void Leap()
	{
		amount = Math.Min(max,(int)((float)amount*1.25f));	
		if(SignalChange != null) SignalChange(this);
	}
}

public class MPlayer 
{
	public int index;
	public bool isHuman;
	public string name;
	public MColor color;
	
	public float angle;
	public MTower tower;
	
	public int maxBeasts = 50;
		
	public int maxFramesTillBeast = MConfig.SPAWN_RATE;//180;
	public int framesTillBeast;
	
	public float nextBeastCreationAngle = 0.0f;
	
	public List<MBeast> beasts = new List<MBeast>(80);
	
	public bool isDead = false;
	
	public int dna = 0;
	
	public MPlayerStat statSpeed;
	public MPlayerStat statAttack;
	public MPlayerStat statDefence;
	public MPlayerStat statHealth;
	public MPlayerStat[] stats;
	
	public int currentStatTotal;
	public int statTotal;
	
	public int leapLevel = 0;
	
	public int leapThreshold = 40;
	
	public int totalKills = 0;
	
	public bool isDirty = false;
	public bool areStatsDirty = false;
	
	public MPlayer(int index, bool isHuman, string name, MColor color)
	{
		this.index = index;
		this.isHuman = isHuman;
		this.name = name;
		this.color = color;
		
		framesTillBeast = maxFramesTillBeast;
		
		stats = new MPlayerStat[4];
		
		stats[0] = statSpeed = new MPlayerStat(0,"speed",0,30);
		stats[1] = statAttack = new MPlayerStat(1,"attack",0,30);
		stats[2] = statDefence = new MPlayerStat(2,"defence",0,30);
		stats[3] = statHealth = new MPlayerStat(3,"health",0,30);
		
		currentStatTotal = 0;
		statTotal = 0;
		
		foreach(MPlayerStat stat in stats)
		{
			stat.player = this;	
			statTotal += stat.max;
			stat.SignalChange += HandleStatChange;
		}
		
		isDirty = true;
	}
	
	public void AddKill()
	{
		totalKills ++;
		isDirty = true;
	}
	
	public void HandleTowerHit()
	{
		isDirty = true;
	}

	private void HandleStatChange (MPlayerStat stat)
	{
		currentStatTotal = 0;
		foreach(MPlayerStat eachStat in stats)
		{
			currentStatTotal += eachStat.amount;
		}
		
		if(currentStatTotal % leapThreshold == 0 && statTotal != 0)
		{
			int newLeapLevel = (int) Mathf.Floor ((float)currentStatTotal/(float)leapThreshold);
			
			if(leapLevel != newLeapLevel)
			{
				leapLevel = newLeapLevel;
				
				FSoundManager.PlaySound("EvolutionaryLeap",1.0f);
				
				if(currentStatTotal == statTotal)
				{
					if(isHuman) MGame.instance.ShowNote("MAX EVOLVED!", 3.0f);
				}
				else 
				{
					if(isHuman) MGame.instance.ShowNote("EVOLUTIONARY LEAP!\n +25% TO ALL MUTATIONS", 5.0f);
					
					foreach(MPlayerStat eachStat in stats)
					{
						eachStat.Leap();
					}
				}
			}
		}
		
		isDirty = true;
		areStatsDirty = true;
	}
}
