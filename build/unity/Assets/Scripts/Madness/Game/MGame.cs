using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MGame : FMultiTouchableInterface
{
	public static MGame instance;
	
	public static Color colorWhite = Color.white;
	public static Color colorRed = new Color(0.65f,0.0f,0.0f,1.0f);
	
	public FContainer container;
	public MInGamePage page;
	
	public MEffectLayer effectLayer;
	
	private FContainer _beastContainer;
	private FContainer _beastContainerSpecial;
	
	private List<MPlayer> _players = new List<MPlayer>();
	private MPlayer _human;
	private List<MTower> _towers = new List<MTower>();
	
	private int _beastCount = 0;
	private MBeast[] _beasts;
	
	public int frameCount = 0;
	
	private List<MBeast>_beastsThatDied = new List<MBeast>(20);
	
	private List<MTower>_towersThatWereDestroyed = new List<MTower>(4);
	
	private MPlayer _winningPlayer = null;
	
	public FContainer hudLayer;
	
	private FLabel _dnaLabel;
	private FLabel[] _playerLabels;
	private FLabel _beastLabel;
	private FLabel _evolutionLabel;
	
	
	
	public MGame(MInGamePage page)
	{
		instance = this;
		this.page = page;
		this.container = page;
		
		_players.Add(new MPlayer(0, true,"YOU",MColor.Green));
		_players.Add(new MPlayer(1, false,"RED",MColor.Red));
		_players.Add(new MPlayer(2, false,"BLUE",MColor.Blue));
		
		_human = _players[0]; 
		
		_beasts = new MBeast[_players.Count * (_human.maxBeasts + 20)];
		
		CreateTowers();
		
		
		container.AddChild(_beastContainer = new FContainer());
		container.AddChild(_beastContainerSpecial = new FContainer());
		
		container.AddChild(effectLayer = new MEffectLayer());
		
		container.AddChild(hudLayer = new FContainer());
		
		CreateUI();
		
		Futile.instance.SignalUpdate += HandleUpdate;
		Futile.touchManager.AddMultiTouchTarget(this);
		
		ShowNote("CLICK TO MOVE THE GREEN GUYS!\nDESTROY THE ENEMY CELLS!\nEVOLVE BY USING DNA!",10.0f);
	}
	
	private void CreateUI()
	{
		hudLayer.AddChild(_dnaLabel = new FLabel("Cubano", "0&"));
		_dnaLabel.anchorX = 1.0f;
		_dnaLabel.anchorY = 1.0f;
		_dnaLabel.x = Futile.screen.halfWidth - 3.0f;
		_dnaLabel.y = Futile.screen.halfHeight - 3.0f;
		
		hudLayer.AddChild(_beastLabel = new FLabel("Cubano", "30/80 BEASTS (MAXED)"));
		_beastLabel.scale = 0.5f;
		_beastLabel.anchorX = 1.0f;
		_beastLabel.anchorY = 0.0f;
		_beastLabel.x = Futile.screen.halfWidth - 3.0f;
		_beastLabel.y = -Futile.screen.halfHeight + 3.0f + 18.0f;
		
		hudLayer.AddChild(_evolutionLabel = new FLabel("Cubano", "4/100 EVOLUTIONS"));
		_evolutionLabel.scale = 0.5f;
		_evolutionLabel.anchorX = 1.0f;
		_evolutionLabel.anchorY = 0.0f;
		_evolutionLabel.x = Futile.screen.halfWidth - 3.0f;
		_evolutionLabel.y = -Futile.screen.halfHeight + 3.0f;
		
	}

	public void AddDNA (int amount)
	{
		_human.dna += amount;
		_dnaLabel.text = _human.dna + "&";
	}
	
	public void RemoveDNA (int amount)
	{
		_human.dna -= amount;
		_dnaLabel.text = _human.dna + "&";
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
			
			if(player.isDead) continue;
			
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
		
		bool isFrameEven = (frameCount % 2 == 0);
		
		for(int b = 0; b<_beastCount; b++)
		{
			MBeast beast = _beasts[b];
			float x = beast.x;
			float y = beast.y;
			MPlayer beastPlayer = beast.player;
			Vector2 velocity = beast.velocity;
			
			float deltaRotation = 0.0f;
			
			for(int c = 0; c<_beastCount; c++)
			{
				MBeast otherBeast = _beasts[c];
				if(otherBeast == beast) continue;
				
				float dx = otherBeast.x - x;
				if(dx > attackRadius || dx < -attackRadius) continue;
				float dy = otherBeast.y - y;
				if(dy > attackRadius || dy < -attackRadius) continue;
				
				int distance = preCalcSQRTs[(int)(dx*dx + dy*dy)];
				
				if(distance < attackRadius)
				{
					if(distance < nearbyRadius)
					{
						if(beastPlayer != otherBeast.player)
						{
							if(!beast.isAttacking)
							{
								//attack enemy
								beast.isAttacking = true;
								beast.isAttackingTower = false;
								beast.attackTarget = otherBeast;
								beast.attackFrame = 0;
							}
							
							//face the enemy you're attacking!
							float faceEnemyRotation = -Mathf.Atan2(dy, dx) * RXMath.RTOD + 90.0f;
							deltaRotation += RXMath.getDegreeDelta(beast.rotation,faceEnemyRotation) * 0.3f;
						} 
						
						//push away from other beast
						tempVector.x = 0.0001f + -dx;
						tempVector.y = -dy;
						tempVector.Normalize();
						velocity += tempVector;	//push away from enemy or other beast
					} 
					else if(distance < nearbyRadius + 4.0f) //fudge area, not too close or to far
					{
						if(beastPlayer != otherBeast.player)
						{
							if(!beast.isAttacking)
							{
								//attack enemy
								beast.attackFrame = 0;
								beast.isAttacking = true;
								beast.isAttackingTower = false;
								beast.attackTarget = otherBeast;
							}
							
							//face the enemy you're attacking!
							float faceEnemyRotation = -Mathf.Atan2(dy, dx) * RXMath.RTOD + 90.0f;
							deltaRotation += RXMath.getDegreeDelta(beast.rotation,faceEnemyRotation) * 0.3f;
						}
					}
					else 
					{
						//move toward enemy!
						if(beastPlayer != otherBeast.player)
						{
							tempVector.x = 0.0001f + -dx;
							tempVector.y = -dy;
							tempVector.Normalize();
							velocity -= tempVector * 0.1f; //push towards enemy
						}
					}
				}
			}
			
			for(int t = 0; t<towerCount; t++)
			{
				MTower tower = _towers[t];	
				float dx = tower.x - x;
				float dy = tower.y - y;
				
				float distanceToTower = Mathf.Sqrt(dx*dx + dy*dy);
				
				if(distanceToTower < towerRadius+attackRadius)
				{
					if(distanceToTower < towerRadius + 5.0) //5 unit happy zone
					{
						if(distanceToTower < towerRadius)
						{
							tempVector.x = dx;
							tempVector.y = dy;
							tempVector.Normalize();
							velocity -= tempVector * 2.0f;	//push away from tower
						}
						
						if(beastPlayer != tower.player)
						{
							if(!beast.isAttacking)
							{
								//ATTACK TOWER
								beast.attackFrame = 0;
								beast.isAttacking = true;
								beast.isAttackingTower = true;
								beast.attackTower = tower;
							}
							
							//face the tower you're attacking!
							float faceEnemyRotation = -Mathf.Atan2(dy, dx) * RXMath.RTOD + 90.0f;
							deltaRotation += RXMath.getDegreeDelta(beast.rotation,faceEnemyRotation) * 0.3f;
						}
					}
					else 
					{
						if(beastPlayer != tower.player)
						{
							tempVector.x = dx;
							tempVector.y = dy;
							tempVector.Normalize();
							velocity += tempVector * 0.1f;	//push towards the tower
						}
					}
				}
			}
			
			if(beast.isAttacking)
			{
				if(isFrameEven) beast.attackFrame ++; //only increment every other frame
				if(beast.attackFrame == 5)
				{
					if(beast.isAttackingTower)
					{
						
						if(beast.attackTower != null && beast.attackTower.health > 0)
						{
							MTower attackTower = beast.attackTower;
							//tower has 25 defence
							attackTower.health -= Math.Max(1.0f, beast.offence - 24.0f); //tower always takes 1 damage
							attackTower.UpdateHealthPercent();
							
							if(attackTower.health <= 0)
							{
								_towersThatWereDestroyed.Add (attackTower);
							}
							else 
							{
								effectLayer.ShowTowerHitForTower(attackTower);
							}
							
							//this will make the attack graphic show on the closest side of the tower
							tempVector.x = attackTower.x-x;
							tempVector.y = attackTower.y-y;
							tempVector.Normalize();
							tempVector *= -30.0f;  
							
							effectLayer.ShowAttackMarkForPlayer(beastPlayer, new Vector2(attackTower.x+tempVector.x,attackTower.y+tempVector.y));
						}
						
						beast.attackTower = null;
					}
					else //attack the other beast
					{
						MBeast attackTarget = beast.attackTarget;
						beast.attackFrame++;
						
						if(attackTarget != null && attackTarget.isEnabled)
						{
							
							if(attackTarget.health > 0)
							{
								float damage = Math.Max(1.0f, beast.offence - attackTarget.defence); //damage must be at least 1
								attackTarget.health -= damage;
								
								if(attackTarget.health <= 0)
								{
									if(!_beastsThatDied.Contains(attackTarget))
									{
										if(beastPlayer.isHuman)
										{
											effectLayer.CreateDNA(new Vector2(attackTarget.x,attackTarget.y), 1);
										}
										else 
										{
											beast.player.dna++;	
										}
										
										_beastsThatDied.Add(attackTarget);	
									}
								}
								
								attackTarget.sprite.shader = FShader.AdditiveColor;
								attackTarget.sprite.color = attackTarget.player.color.attackRedColor;
								_beastContainerSpecial.AddChild (attackTarget);
								attackTarget.blinkFrame = 7;
							}
							
							//this will make the attack graphic show on the closest side of the beast
							tempVector.x = attackTarget.x-x;
							tempVector.y = attackTarget.y-y;
							tempVector.Normalize();
							tempVector *= -10.0f;  
							
							effectLayer.ShowAttackMarkForPlayer(beastPlayer, new Vector2(attackTarget.x+tempVector.x,attackTarget.y+tempVector.y));
						}
						
						beast.attackTarget = null;
					}
					
				}
				else if (beast.attackFrame >= 19)
				{
					beast.isAttacking = false;
					beast.attackTarget = null;
					beast.attackTower = null;
					beast.attackFrame = 0;
				}
			}
			
			if(beast.hasTarget)
			{
				float dx = beast.target.x - x;
				
				float dy = beast.target.y - y;
				
				float distanceToTarget = Mathf.Sqrt (dx*dx + dy*dy);
				if(distanceToTarget > 50.0f)
				{
					tempVector.x = beast.target.x - beast.x;
					tempVector.y = beast.target.y - beast.y;
					tempVector.Normalize();
					velocity += tempVector * beast.speed * 0.1f; //push towards target
				}
			}
			
			float distanceToCenter = Mathf.Sqrt(x*x + y*y);
			
			if(distanceToCenter > wallRadius)
			{
				tempVector.x = x;
				tempVector.y = y;
				tempVector.Normalize();
				velocity -= tempVector * 2.0f; //push away from wall
			}
			
			
			//apply half the velocity 
			beast.x += velocity.x * 0.5f;
			beast.y += velocity.y * 0.5f;
			
			float goalRotation = -Mathf.Atan2(velocity.y, velocity.x) * RXMath.RTOD + 90.0f;
			deltaRotation += RXMath.getDegreeDelta(beast.rotation,goalRotation) * 0.06f;
			
			beast.rotation += Math.Max(-3.0f, Math.Min (3.0f, deltaRotation*0.9f));
			
			//ease the velocity
			velocity.x *= 0.45f;
			velocity.y *= 0.45f;
			
			float moveAmount = Math.Min(1.5f, 20.0f*Mathf.Abs(velocity.x*velocity.y)) + Math.Abs (deltaRotation);
			
			beast.velocity = velocity;
			
			beast.AdvanceFrame(moveAmount);
			
			if(beast.blinkFrame > 0)
			{
				beast.blinkFrame--;
				if(beast.blinkFrame == 0) //put it back to normal
				{
					beast.sprite.shader = FShader.Normal;
					beast.sprite.color = colorWhite;
					_beastContainer.AddChild(beast);
				}
			}

			
			//make it scale in
			if(beast.scale < 1.0f) 
			{
				beast.scale += 0.025f;	
			}
		}
		
		int diedCount = _beastsThatDied.Count;
		
		for(int d = 0; d<diedCount; d++)
		{ 
			MBeast beast = _beastsThatDied[d];
			if(beast.isEnabled)
			{
				effectLayer.ShowBeastExplosionForBeast(beast);
				RemoveBeast(beast);
			}
		}
		
		_beastsThatDied.Clear();
		
		for(int t = 0; t<_towersThatWereDestroyed.Count; t++)
		{
			MTower destroyedTower = _towersThatWereDestroyed[t];
			KillPlayer(destroyedTower.player);
			Debug.Log ("KILL PLAYER DESTROY TOWER ETC");
		}
		
		_towersThatWereDestroyed.Clear ();
		

		frameCount++;
	}
	
	private void KillPlayer(MPlayer player)
	{
		player.isDead = true;
		
		effectLayer.ShowTowerExplosionForTower(player.tower);
		player.tower.RemoveFromContainer();
		
		_towers.Remove(player.tower);
		
		for(int b = 0; b< player.beasts.Count; b++)
		{
			MBeast beast = player.beasts[b];
			if(beast.health > 0)
			{
				effectLayer.ShowBeastExplosionForBeast(beast);
				RemoveBeast(beast);
			}
		}
		
		int remainingPlayers = 0;
		MPlayer remainingPlayer = null;
		for(int p = 0; p<_players.Count; p++)
		{
			if(!_players[p].isDead)	
			{
				remainingPlayers++;
				remainingPlayer = _players[p];
			}
		}
		
		if(remainingPlayers == 1)
		{
			_winningPlayer = remainingPlayer;
			
			FLabel winLabel;
			
			if(_winningPlayer.isHuman)
			{
				winLabel  = new FLabel("Cubano","YOU WIN!");
			}
			else 
			{
				winLabel  = new FLabel("Cubano","YOU LOSE!");
			}
			
			container.AddChild(winLabel);
		
			winLabel.scale = 0.8f;
		
			Go.to (winLabel,6.0f,new TweenConfig().floatProp("scale",1.0f).onComplete(HandleWinComplete));
		}
		else if(player.isHuman)
		{
			ShowNote("YOU LOSE!\nPRESS BACK TO TRY AGAIN", 5.0f);
		}	
		else 
		{
			ShowNote (player.name.ToUpper()+" WAS ELIMINATED!", 5.0f);
		}
		
	}
	
	private void HandleWinComplete(AbstractTween tween)
	{
		page.ShowWinForPlayer(_winningPlayer, _players);
	}
	
	private void ShowNote(string message, float duration)
	{
		FLabel noteLabel = new FLabel("Cubano",message);
			
		noteLabel.scale = 0.8f;
			
		container.AddChild(noteLabel);
			
		Go.to (noteLabel,duration,new TweenConfig().floatProp("scale",1.0f).onComplete (HandleNoteComplete));
	}
	
	private void HandleNoteComplete(AbstractTween tween)
	{
		((tween as Tween).target as FLabel).RemoveFromContainer();
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
	
	public void CreateBeast(MPlayer player)
	{
		if(player.beasts.Count >= player.maxBeasts) return; //TODO: Show a "max beasts limit reached!" indicator on screen
		
		MBeast beast = MBeast.New();
		if(beast.container != _beastContainer) _beastContainer.AddChild(beast);
		beast.Start(player);
		
		if(_beasts.Length <= _beastCount)
		{
			Array.Resize(ref _beasts,_beastCount+20);	
		}
		
		_beasts[_beastCount++] = beast; 
		player.beasts.Add(beast);
		
		float creationAngle = player.angle + player.nextBeastCreationAngle;
		
		beast.x = player.tower.x + Mathf.Sin (creationAngle*RXMath.DTOR) * (MConfig.TOWER_RADIUS+20.0f); 
		beast.y = player.tower.y + Mathf.Cos (creationAngle*RXMath.DTOR) * (MConfig.TOWER_RADIUS+20.0f); 
		
		player.nextBeastCreationAngle = (player.nextBeastCreationAngle + 30.0f)%360.0f;
	} 
	
	public void RemoveBeast(MBeast beastToRemove)
	{
		beastToRemove.Destroy();
		_beasts.RemoveItem(beastToRemove, ref _beastCount); 
		
		//put it back in the right container
		if(beastToRemove.container == _beastContainerSpecial)
		{
			beastToRemove.sprite.shader = FShader.Normal;
			_beastContainerSpecial.RemoveChild(beastToRemove);
		}
		
		beastToRemove.player.beasts.Remove(beastToRemove); 
		
		//don't pool because it could cause problems
		beastToRemove.RemoveFromContainer();
		//MBeast.pool.Add (beastToRemove);
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
