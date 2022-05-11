using System.Drawing;
using GXPEngine.BodyParts;
using GXPEngine.Core;
using GXPEngine.StageManagement;

namespace GXPEngine;

public class Hook : Sprite
{
    private readonly Vec2 velocity;
    private readonly MyGame myGame;
    public bool hasHit;
    
    public Hook(float x, float y, Vec2 velocity_) : base("bodyParts/test/red/hook.png")
    {
        myGame = (MyGame) game;
        if (StageLoader.currentStage != null) parent = StageLoader.currentStage;
        SetXY(x,y);
        SetOrigin(width/2.0f,height/2.0f);
        collider.isTrigger = true;
        velocity = velocity_;

        hasHit = false;
    }

    private void Update()
    {
        if (hasHit) return;
        
        Collision? collision = MoveUntilCollision(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, StageLoader.currentStage?.surfaces.GetChildren()!);

        if (collision != null)
        {
            Kill();
        }
            
            
        foreach (Sprite sprite in StageLoader.currentStage.grappleSurfaces.GetChildren())
        {
            if (HitTest(sprite))
            {
                hasHit = true;
            }
        }

        if (x > StageLoader.currentStage.stageWidth + width || x < -width || y > StageLoader.currentStage.stageHeight + height || y < -height)
        {
            Kill();
        }
    }

    private void Kill()
    {
        GrapplingHook? grapplingHook = (GrapplingHook) myGame.player.upperBodyPart!;
        grapplingHook.hook = null;
        StageLoader.currentStage.background.Clear(Color.LightCyan);
        Destroy();
    }
}