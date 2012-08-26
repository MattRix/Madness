using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MGame : FMultiTouchableInterface
{
	public static MGame instance;
	
	public FContainer container;
	
	public MEffectLayer effectLayer;
	
	private FContainer _beastContainer;
	
	private List<MPlayer> _players = new List<MPlayer>();
	private MPlayer _human;
	private List<MTower> _towers = new List<MTower>();
	
	private int _beastCount = 0;
	private MBeast[] _beasts;
	
	public int frameCount = 0;
	
	
	public MGame(FContainer container)
	{
		instance = this;
		this.container = container;
		
		_players.Add(new MPlayer(0, true,"YOU",MColor.Green));
		_players.Add(new MPlayer(1, false,"RED",MColor.Red));
		_players.Add(new MPlayer(2, false,"BLUE",MColor.Blue));
		
		_human = _players[0]; 
		
		_beasts = new MBeast[_players.Count * (_human.maxBeasts + 20)];
		
		CreateTowers();
		
		container.AddChild(_beastContainer = new FContainer());
		
		container.AddChild(effectLayer = new MEffectLayer());
		
		Futile.instance.SignalUpdate += HandleUpdate;
		Futile.touchManager.AddMultiTouchTarget(this);
	}
	
	public void Destroy()
	{
		Futile.touchManager.RemoveMultiTouchTarget(this);
		Futile.instance.SignalUpdate -= HandleUpdate;
		MBeast.pool.Clear();
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
			
			player.framesTillBeast--;
			
			if(player.framesTillBeast == 0)
			{
				player.framesTillBeast = player.maxFramesTillBeast;
				
				CreateBeast(player);	
			}
		}
		
		float wallRadius = MConfig.WALL_RADIUS - 27.0f;
		
		float towerRadius = MConfig.TOWER_RADIUS + 10.0f;
		
		float attackRadius = 80.0f;
		float nearbyRadius = 30.0f; //must be smaller than attackRadius
		
		Vector2 tempVector = new Vector2(0,0);
		
		int towerCount = _towers.Count;
		
		for(int b = 0; b<_beastCount; b++)
		{
			MBeast beast = _beasts[b];
			float x = beast.x;
			float y = beast.y;
			Vector2 velocity = beast.velocity;
			
			for(int c = 0; c<_beastCount; c++)
			{
				MBeast otherBeast = _beasts[c];
				if(otherBeast == beast) continue;
				
				float dx = Math.Abs(otherBeast.x - x);
				if(dx > attackRadius) continue;
				float dy = Math.Abs(otherBeast.y - y);
				if(dy > attackRadius) continue;
				
				int distance = preCalcSQRTs[(int)(dx*dx + dy*dy)];
				
				if(distance < attackRadius)
				{
					if(distance < nearbyRadius)
					{
						if(beast.player != otherBeast.player)
						{
							//attack enemy
						}
						
						//push away from enemy
						tempVector.x = 0.0001f + x-otherBeast.x;
						tempVector.y = y-otherBeast.y;
						tempVector.Normalize();
						velocity += tempVector;	
					}
					else 
					{
						//move toward enemy!
						if(beast.player != otherBeast.player)
						{
							tempVector.x = 0.0001f + x-otherBeast.x;
							tempVector.y = y-otherBeast.y;
							tempVector.Normalize();
							velocity -= tempVector;	
						}
					}
				}
			}
			
			if(beast.hasTarget)
			{
				tempVector.x = beast.target.x - beast.x;
				tempVector.y = beast.target.y - beast.y;
				tempVector.Normalize();
				velocity += tempVector * beast.speed;	
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
			
			beast.x += velocity.x;
			beast.y += velocity.y;
			
			velocity.x *= 0.1f;
			velocity.y *= 0.1f;
			
			beast.velocity = velocity;
		}
		
		frameCount++;
	}
	
	public void SetAttackTarget(MPlayer player, Vector2 targetPosition)
	{
		float distance = Mathf.Sqrt(targetPosition.x*targetPosition.x + targetPosition.y*targetPosition.y);
		
		if(distance > MConfig.WALL_RADIUS + 50.0f) return; //no attacking too far outside the wall
		
		List<MBeast> beasts = player.beasts;
		int beastCount = beasts.Count;
		for(int b = 0; b<beastCount; b++)
		{
			MBeast beast = beasts[b];
			beast.hasTarget = true;
			beast.target = targetPosition;
		}
		
		if(player.isHuman) effectLayer.ShowCrosshairForPlayer(player, targetPosition);
	}
	
	public MBeast GetNewBeast()
	{
		MBeast beast = (MBeast.pool.Count == 0) ? new MBeast() : MBeast.pool.Pop();
		
		_beastContainer.AddChild(beast);
		return beast; 
	}
	
	public void CreateBeast(MPlayer player)
	{
		if(player.beasts.Count >= player.maxBeasts) return; //TODO: Show a "max beasts limit reached!" indicator on screen
		
		MBeast beast = GetNewBeast();
		beast.Start(player);
		_beastContainer.AddChild(beast);
		if(_beasts.Length <= _beastCount)
		{
			Array.Resize(ref _beasts,_beastCount+20);	
		}
		_beasts[_beastCount++] = beast; 
		player.beasts.Add(beast);
		
		float creationAngle = player.angle + player.nextBeastCreationAngle;
		
		beast.x = player.tower.x + Mathf.Sin (creationAngle*RXMath.DTOR) * (MConfig.TOWER_RADIUS+beast.radius); 
		beast.y = player.tower.y + Mathf.Cos (creationAngle*RXMath.DTOR) * (MConfig.TOWER_RADIUS+beast.radius); 
		
		player.nextBeastCreationAngle = (player.nextBeastCreationAngle + 30.0f)%360.0f;
	} 
	
	public void RemoveBeast(MBeast beastToRemove)
	{
		beastToRemove.Destroy();
		_beasts.RemoveItem(beastToRemove, ref _beastCount);
		
		beastToRemove.player.beasts.Remove(beastToRemove); 
		MBeast.pool.Add (beastToRemove);
	}
	
	
	
	public void HandleMultiTouch(FTouch[] touches)
	{
		foreach(FTouch touch in touches)
		{
			if(touch.fingerId == 0 && touch.phase == TouchPhase.Began)
			{
				SetAttackTarget(_human, container.GlobalToLocal(touch.position));
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
