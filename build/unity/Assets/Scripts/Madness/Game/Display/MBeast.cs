using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MBeastElementSet
{
	public FAtlasElement[] walkElements;
	public FAtlasElement[] attackElements;
	public FAtlasElement[] walkAndAttackElements;
}

public class MBeastType
{
	public static MBeastType[] beastTypes = new MBeastType[3];
	
	public static MBeastType A = new MBeastType(0,"A");
	public static MBeastType B = new MBeastType(1,"B");
	public static MBeastType C = new MBeastType(2,"C");
	
	public int index;
	public string name;
	
	public MBeastType(int index, string name)
	{
		this.index = index;
		this.name = name;
	}
}

public class MBeast : FContainer
{
	public static List<MBeast> pool = new List<MBeast>();
	
	public static MBeastElementSet[] _elementSets;
	
	private MBeastElementSet _elementSet;
	
	public float radius = 20.0f;
	
	public MPlayer player;
	
	private FSprite _sprite;
	
	private bool _isEnabled = false;
	
	public Vector2 velocity;
	
	public bool hasTarget = false;
	
	public Vector2 target = new Vector2(0,0);
	
	public float speed;
	
	public MBeastType beastType;
	
	public static void Init()
	{
		_elementSets = new MBeastElementSet[MColor.colors.Length*MBeastType.beastTypes.Length];
		
		int e = 0;
		for(int c = 0; c<MColor.colors.Length; c++)
		{
			for(int t = 0; t<MBeastType.beastTypes.Length; t++)
			{
				MBeastElementSet elementSet = new MBeastElementSet();
				_elementSets[e] = elementSet;
				elementSet.walkElements = new FAtlasElement[10];
				elementSet.attackElements = new FAtlasElement[10];
				elementSet.walkAndAttackElements = new FAtlasElement[elementSet.walkElements.Length + elementSet.attackElements.Length];
			
				int allIndex = 0;
				
				for(int f = 0; f<elementSet.walkElements.Length; f++)
				{
					elementSet.walkAndAttackElements[allIndex++] = elementSet.walkElements[f] = Futile.atlasManager.GetElementWithName(c+"_"+t+"/Beast_walking_"+f+".png");	
				}
				
				for(int f = 0; f<elementSet.attackElements.Length; f++)
				{
					elementSet.walkAndAttackElements[allIndex++] = elementSet.attackElements[f] = Futile.atlasManager.GetElementWithName(c+"_"+t+"/Beast_attacking_"+f+".png");	
				}
				e++;
			}
		}
	}  
	
	public MBeast()
	{
		AddChild(_sprite = new FSprite(_elementSets[0].walkElements[0].name));
		//_sprite.shader = FShader.AdditiveColor;
	}
	
	public void Start(MPlayer player)
	{
		this.player = player;
		
		hasTarget = false;
		target = new Vector2(0,0);
		velocity = new Vector2(0,0);
		speed = 1.0f;
		beastType = MBeastType.A;
		
		_elementSet = _elementSets[player.color.index*MBeastType.beastTypes.Length + beastType.index];
		_sprite.element = _elementSet.walkElements[0];
		
		this.isEnabled = true;
	}
	
	public void Destroy()
	{
		this.isEnabled = false; 
	}
	
	public bool isEnabled
	{
		get {return _isEnabled;}
		set 
		{
			if(_isEnabled != value)
			{
				_isEnabled = value;
				
				if(_isEnabled)
				{
					_sprite.isVisible = true;	
				}
				else
				{
					_sprite.isVisible = false;	
					
				}
			}
		}
	}

}
