namespace GXPEngine.BodyParts;

public abstract class BodyPart : GameObject
{
    public AnimationSprite model;
    protected MyGame myGame;
    protected Player target;
    
    protected BodyPart(string modelPath, int cols, int rows, int frames, Player target_)
    {
        target = target_;
        myGame = (MyGame) game;
        parent = myGame;
        model = new AnimationSprite(modelPath, cols, rows, frames, true, false);
        AddChild(model);
    }

    protected abstract void Update();
    public abstract void UpdatePosition();
    protected void SetModel(string modelPath, int cols, int rows, int frames)
    {
        model = new AnimationSprite(modelPath, cols, rows, frames, true, false);
    }
}