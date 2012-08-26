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
		
		float wallRadius = MConfig.WALL_RADIUS - 27.0f;
		
		float towerRadius = MConfig.TOWER_RADIUS + 10.0f;
		
		float attackRadius = 80.0f;
		float nearbyRadius = 30.0f; //must be smaller than attackRadius
		
		Vector2 tempVector = new Vector2(0,0);
		
		int towerCount = _towers.Count;
		
		for(int w = 0; w<_wadCount; w++)
		{
			MWad wad = _wads[w];
			float x = wad.x;
			float y = wad.y;
			Vector2 velocity = wad.velocity;
			
			for(int c = 0; c<_wadCount; c++)
			{
				MWad otherWad = _wads[c];
				if(otherWad == wad) continue;
				
				float dx = Math.Abs(otherWad.x - x);
				if(dx > attackRadius) continue;
				float dy = Math.Abs(otherWad.y - y);
				if(dy > attackRadius) continue;
				
				int distance = preCalcSQRTs[(int)(dx*dx + dy*dy)];
				
				if(distance < attackRadius)
				{
					if(distance < nearbyRadius)
					{
						if(wad.player != otherWad.player)
						{
							//attack enemy
						}
						
						//push away from enemy
						tempVector.x = 0.0001f + x-otherWad.x;
						tempVector.y = y-otherWad.y;
						tempVector.Normalize();
						velocity += tempVector;	
					}
					else 
					{
						//move toward enemy!
						if(wad.player != otherWad.player)
						{
							tempVector.x = 0.0001f + x-otherWad.x;
							tempVector.y = y-otherWad.y;
							tempVector.Normalize();
							velocity -= tempVector;	
						}
					}
				}
			}
			
			
			for(int t = 0; t<towerCount; t++)
			{
				MTower tower = _towers[t];	
				float dx = Math.Abs(tower.x - x);
				if(dx > attackRadius) continue;
				float dy = Math.Abs(tower.y - y);
				if(dy > attackRadius) continue;
				
				int distanceToTower = preCalcSQRTs[(int)(dx*dx + dy*dy)];
				
				if(distanceToTower < towerRadius)
				{
					tempVector.x = x-tower.x;
					tempVector.y = y-tower.y;
					tempVector.Normalize();
					velocity += tempVector;	
				}
			}
			
			
			float distanceToCenter = Mathf.Sqrt(x*x + y*y);
			
			if(distanceToCenter > wallRadius)
			{
				tempVector.x = x;
				tempVector.y = y;
				tempVector.Normalize();
				velocity -= tempVector * 2.0f;	
			}
			
			
			//MATCH VECTOR WITH FRIENDS?
			//PUSH FROM TOWER
			//PUSH FROM WALL
			//PULL TOWARDS ENEMIES
			//PUSH FROM TOO-CLOSE ENEMIES
			//PUSH FROM TOO-CLOSE FRIENDS
			//TURN TOWARDS CLOSEST ENEMY
			//ATTACK CLOSE ENEMY
			//APPLY VELOCITY
			
			wad.x += velocity.x;
			wad.y += velocity.y;
			
			velocity.x *= 0.1f;
			velocity.y *= 0.1f;
			
			wad.velocity = velocity;
		}
		
		frameCount++;
	}

	//from http://www.codecodex.com/wiki/Calculate_an_integer_square_root#C.23
	private static int IntSQRT(int num)
	{
		if (0 == num) { return 0; }  // Avoid zero divide  
	    int n = (num / 2) + 1;       // Initial estimate, never low  
	    int n1 = (n + (num / n)) / 2;  
	    while (n1 < n) {  
	        n = n1;  
	        n1 = (n + (num / n)) / 2;  
	    } // end while  
	    return n;  
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
		
		wad.x = player.tower.x + Mathf.Sin (creationAngle*RXMath.DTOR) * (MConfig.TOWER_RADIUS+wad.radius); 
		wad.y = player.tower.y + Mathf.Cos (creationAngle*RXMath.DTOR) * (MConfig.TOWER_RADIUS+wad.radius); 
		
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
	
	public static int[] preCalcSQRTs;
	
	static MGame()
	{
		preCalcSQRTs = new int[20000];
		
		for(int i = 0; i<20000; i++)
		{
			preCalcSQRTs[i] = GetIntSQRT(i);	
		}
	}
	
	public static int GetIntSQRT(int num) 
	{  
	    if (0 == num) { return 0; }  // Avoid zero divide  
	    int n = (num / 2) + 1;       // Initial estimate, never low  
	    int n1 = (n + (num / n)) / 2;  
	    while (n1 < n) {  
	        n = n1;  
	        n1 = (n + (num / n)) / 2;  
	    } // end while  
	    return n;  
	}
}
