namespace GXPEngine;

public class Climbable : Sprite
{
    public Climbable() : base("tempBackground.png")
    {
        collider.isTrigger = true;
    }
}