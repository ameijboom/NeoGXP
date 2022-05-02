namespace GXPEngine.Blocks;

public class Climbable : Block
{
    public Climbable() : base("tempBackground.png", climbable_:true)
    {
        collider.isTrigger = true;
    }
}