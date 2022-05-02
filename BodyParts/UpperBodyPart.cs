namespace GXPEngine.BodyParts;

public abstract class UpperBodyPart : BodyPart
{
    
    protected UpperBodyPart(string modelPath, int cols, int rows, int frames, Player target_) : base(modelPath,cols,rows,frames, target_) {}

    protected override void Update()
    {
    }

    public override void UpdatePosition()
    {
        SetXY(target.x,target.y);
    }
}