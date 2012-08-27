using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MStatView : FContainer
{
	public MPlayerStat stat;
	
	private FLabel _title;
	private FButton _button;
	
	public MStatView(MPlayerStat stat)
	{
		this.stat = stat;
		stat.SignalChange += HandleStatChange;
		
		AddChild(_title = new FLabel("Cubano", ""));
		_title.anchorY = 0.0f;
		_title.y = 27.0f;
		_title.scale = 0.4f;
		
		AddChild(_button = new FButton("SquareButtonBG_normal.png", "SquareButtonBG_over.png","Click"));
		_button.AddLabel("Cubano", "", Color.white);
		_button.label.text = "";
		_button.label.scale = 0.75f;
		
		_button.isEnabled = false;
		
		_button.SignalRelease += HandleButtonClick;
		
		HandleStatChange (stat);
	}

	private void HandleButtonClick (FButton button)
	{
		if(stat.CanBuy())
		{
			FSoundManager.PlaySound("LevelUp",1.0f);
			stat.Buy();
		}
	}

	private void HandleStatChange (MPlayerStat stat)
	{
		_title.text = stat.name.ToUpper() + ": " + (stat.amount) + "/" + stat.max;
		
		if(stat.amount == stat.max)
		{
			_button.label.text = "MAXED!";
		}
		else
		{
			_button.label.text = (stat.amount+1)+"&";
		}
		
		Update();
	}
	
	public void Update()
	{
		if(stat.amount >= stat.max)
		{
			_button.sprite.color = Color.green;
			_button.label.color = Color.green;
			_button.isEnabled = false;
		}
		else if(stat.player.dna < stat.amount+1)
		{
			_button.sprite.color = Color.gray;	
			_button.label.color = Color.gray;	
			_button.isEnabled = false;
		}
		else 
		{
			_button.sprite.color = Color.white;
			_button.label.color = Color.white;
			_button.isEnabled = true;
		}
	}
	
	public FButton button
	{
		get {return _button;}	
	}
}
