namespace GXPEngine.BreakableStuffs;

public class WallNormal : Breakable
{
	public WallNormal(int pX, int pY) : base(pX, pY - 48, "placeholders/1x4-wall.png")
	{
		//breakAnimationInfo = ();
	}
}

public class WallSmall : Breakable
{
	public WallSmall(int pX, int pY) : base(pX, pY, "placeholders/1x4-pencil.png")
	{
		//breakAnimationInfo = ();
	}
}

public class Pencil : Breakable
{
	public Pencil(int pX, int pY) : base(pX, pY - 48, "placeholders/1x1-wall.png")
	{
		//breakAnimationInfo = ();
	}
}

public class Painting : Breakable
{
	public Painting(int pX, int pY) : base(pX, pY - 114, "placeholders/3x8-painting.png")
	{
		//breakAnimationInfo = ();
	}
}