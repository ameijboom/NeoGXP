namespace GXPEngine.Blocks;

public class Block : Sprite
{
    public bool canGrappleOnto;
    public bool movable;
    public bool climbable;
    
    protected Block(string imagePath, bool canGrappleOnto_ = false, bool movable_ = false, bool climbable_ = false) : base(imagePath, true)
    {
        canGrappleOnto = canGrappleOnto_;
        movable = movable_;
        climbable = climbable_;
    }
}