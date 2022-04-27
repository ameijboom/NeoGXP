namespace GXPEngine.BodyParts;

public abstract class BodyPart : GameObject
{
    private AnimationSprite model;

    protected BodyPart(string modelPath, int cols, int rows, int frames)
    {
        model = new AnimationSprite(modelPath, cols, rows, frames, true, false);
        AddChild(model);
    }

    protected void SetModel(string modelPath, int cols, int rows, int frames)
    {
        model = new AnimationSprite(modelPath, cols, rows, frames, true, false);
    }
}