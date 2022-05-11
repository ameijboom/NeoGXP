using System;
using GXPEngine.StageManagement;

namespace GXPEngine;

public class BreakableBlock : Sprite
{
	private Hitbox hitbox;
	
	public BreakableBlock(int pX, int pY) : base("placeholders/colors.png")
	{
		SetXY(pX,pY);
		
		Console.WriteLine($"{x}, {y}");
		hitbox = new Hitbox();		
		hitbox.SetXY(x,y);
		hitbox.width = width;
		hitbox.height = height;
		StageLoader.currentStage.surfaces.AddChild(hitbox);

		
		collider.isTrigger = true;
		
		
		
	}

	public void Break()
	{
		Animation animation = new Animation("placeholders/placeHolders.png", 4, 1, 4, true);
		animation.SetXY(x,y);
		StageLoader.currentStage.animations.AddChild(animation);
		
		StageLoader.currentStage.breakableBlocks.RemoveChild(hitbox);
		hitbox.Destroy();
		LateDestroy();
	}
}