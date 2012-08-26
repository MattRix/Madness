using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MPageType
{
	None,
	TitlePage,
	InGamePage,
	ScorePage
}

public class MMain : MonoBehaviour
{	
	public static MMain instance;
	
	private MPageType _currentPageType = MPageType.None;
	private MPage _currentPage = null;
	
	private void Start()
	{
		instance = this; 
		
		Go.defaultEaseType = EaseType.Linear;
		Go.duplicatePropertyRule = DuplicatePropertyRuleType.RemoveRunningProperty;
		
		//Time.timeScale = 0.1f;
		
		FutileParams fparams = new FutileParams(true,true,false,false);
		
		fparams.AddResolutionLevel(512.0f,	0.5f,	1.0f,	"_Scale2"); //Unity Editor
		fparams.AddResolutionLevel(1024.0f,	1.0f,	1.0f,	"_Scale2"); //iPhone retina
		
		fparams.origin = new Vector2(0.5f,0.5f);
		
		Futile.instance.Init (fparams);
		 
		Futile.atlasManager.LoadAtlas("Atlases/Game");
		Futile.atlasManager.LoadAtlas("Atlases/Background");
		
		Futile.atlasManager.LoadFont("Cubano","Cubano"+Futile.resourceSuffix+".png", "Atlases/Cubano"+Futile.resourceSuffix);
		
		MBeast.Init(); //sets up the MBeast animation elements
		MExplosion.Init(); //sets up the explosion elements
		MTower.Init (); //sets up the tower elements
		
		GoToPage(MPageType.TitlePage);
	}

	public void GoToPage (MPageType pageType)
	{
		if(_currentPageType == pageType) return; //we're already on the same page, so don't bother doing anything
		
		MPage pageToCreate = null;
		
		if(pageType == MPageType.TitlePage)
		{
			pageToCreate = new MTitlePage();
		}
		else if (pageType == MPageType.InGamePage)
		{
			pageToCreate = new MInGamePage();
		}  
		else if (pageType == MPageType.ScorePage)
		{
			pageToCreate = new MScorePage();
		}
		
		if(pageToCreate != null) //destroy the old page and create a new one
		{
			_currentPageType = pageType;	
			
			if(_currentPage != null)
			{
				Futile.stage.RemoveChild(_currentPage);
			}
			 
			_currentPage = pageToCreate;
			Futile.stage.AddChild(_currentPage);
			_currentPage.Start();
		}
		
	}
	
}









