using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GXPEngine.BodyParts;
using GXPEngine.Core;
using GXPEngine.StageManagement;

namespace GXPEngine;

public class Hook : Sprite
{
    public readonly Vec2 velocity;
    private readonly MyGame myGame;
    public bool hasHit;
    
    public Hook(float x, float y, Vec2 velocity_) : base("bodyParts/test/red/hook.png")
    {
        myGame = (MyGame) game;
        parent = StageLoader.currentStage;
        SetXY(x,y);
        SetOrigin(width/2.0f,height/2.0f);
        collider.isTrigger = true;
        velocity = velocity_;

        hasHit = false;
    }

    private void Update()
    {
        if (!hasHit)
        {
            
            Collision? collision = MoveUntilCollision(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, StageLoader.currentStage.grappleSurfaces.GetChildren());

            if (collision != null)
            {
                if (collision.other is Hitbox {canGrappleOnto: true})
                {
                    hasHit = true;
                }
                else
                {
                    GrapplingHook? grapplingHook = (GrapplingHook) myGame.player.upperBodyPart!;
                    grapplingHook.hook = null;
                    Destroy();
                    StageLoader.currentStage.background.Clear(Color.LightCyan);
                }
            }
        
            if (x > StageLoader.currentStage.stageWidth + width || x < -width || y > StageLoader.currentStage.stageHeight + height || y < -height)
            {
                GrapplingHook? grapplingHook = (GrapplingHook) myGame.player.upperBodyPart!;
                grapplingHook.hook = null;
                StageLoader.currentStage.background.Clear(Color.LightCyan);
                Destroy();
            }
        }
        
        
    }

    private void Hit()
    {
        hasHit = true;
    }
}