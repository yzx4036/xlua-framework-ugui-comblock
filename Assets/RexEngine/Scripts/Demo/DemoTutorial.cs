using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RexEngine;

public class DemoTutorial:MonoBehaviour 
{
	public DialogueType dialogueType;

	public enum DialogueType
	{
		Intro,
		CallToAction,
		EnemyAbility,
		ReflectBullet,
		Gravity,
		Moveset,
		Ladder
	}

	void Awake() 
	{
		
	}

	void Start() 
	{
		DialoguePopup dialoguePopup = GetComponent<DialoguePopup>();
		dialoguePopup.pages = new List<DialogueManager.Page>();
		DialogueManager.Page page = new DialogueManager.Page();
		dialoguePopup.pages.Add(page);

		page.text = GetDialogue();
	}
	
	protected string GetDialogue()
	{
		string description = "";
		switch(dialogueType)
		{
			case DialogueType.Intro:
				description = "Welcome to Booster’s Adventure! This short game will show you some of the things you can do with Rex Engine. Every mechanic you’ll see here comes out-of-the-box!";
				break;
			case DialogueType.CallToAction:
				description = "Our hero, Booster the t-rex, is an inventor and a pilot! Right now, he’s in a jam: he’s lost the blueprints for his latest invention, and he needs your help to get them back!";
				break;
			case DialogueType.EnemyAbility:
				description = "Rex Engine also makes it easy to give enemies any and all of the abilities you can give the player. These enemies are thrilled with their newfound ability to jump. Watch out!";
				break;
			case DialogueType.ReflectBullet:
				description = "Rex Engine gives you both projectiles and the option to make attacks deflect them. Try attacking the bullets this miniboss fires at you!";
				break;
			case DialogueType.Gravity:
				description = "With Rex Engine, you can alter gravity on a whim. You can even lower gravity to give your game moon physics!";
				break;
			case DialogueType.Moveset:
				description = "Rex Engine lets you create multiple movesets for your characters and switch between them on the fly. Here, we’ve given Booster a completely unique moveset for swimming!";
				break;
			case DialogueType.Ladder:
				description = "With Rex Engine, enemies can climb ladders too! They can do everything the player can!";
				break;
			default:
				description = "Welcome to Booster’s Adventure! This short game will show you some of the things you can do with Rex Engine. Every mechanic you’ll see here comes out-of-the-box!";
				break;
		}

		return description;
	}
}
