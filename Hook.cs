using GXPEngine.Blocks;
using GXPEngine.Core;
using GXPEngine.GXPEngine.Core;

namespace GXPEngine;

public class Hook : Sprite
{
    private readonly Vec2 velocity;
    private readonly MyGame myGame;
    public bool hasHit;
    
    public Hook(float x, float y, Vec2 velocity_) : base("bodyParts/test/red/hook.png")
    {
        myGame = (MyGame) game;
        SetXY(x,y);
        collider.isTrigger = true;
        velocity = velocity_;

        hasHit = false;
    }

    private void Update()
    {
        if (!hasHit)
        {
            Collision? collision = MoveUntilCollision(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime);

            if (collision != null)
            {
                if (collision.other is Block {canGrappleOnto: true})
                {
                    hasHit = true;
                }
                else LateDestroy();
            }
        
            if (x > myGame.width + width || x < -width || y > myGame.height + height || y < -height)
            {
                LateDestroy();
            }
        }
    }

    private void Hit()
    {
        hasHit = true;
    }
}