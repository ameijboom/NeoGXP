using System;
using GXPEngine.BodyParts;
using GXPEngine.Core;
using GXPEngine.GXPEngine.Core;

namespace GXPEngine;

public class Player : Sprite
{
    public Vec2 velocity;
    public State currentState;
    public bool isGrounded;
    public bool wasGrounded;

    public float speed;
    private float jumpForce;
    public float gravitationalForce;

    public Collision? horizontalCollision;
    public Collision? verticalCollision;

    public LowerBodyPart? lowerBodyPart;
    public UpperBodyPart? upperBodyPart;
    
    
    public Vec2 position => new Vec2(x, y);
    
    public Player(float pX, float pY) : base("hitboxes/playerHitbox.png")
    {
        SetXY(pX,pY);

        velocity = new Vec2(0, 0);

        horizontalCollision = null;
        verticalCollision = null;

        lowerBodyPart = null;
        upperBodyPart = null;

        SetUpperBodyPart(new PlaceHolderUpperBodyPart(this));
        SetLowerBodyPart(new PlaceHolderLowerBodyPart(this));
    }

    public void SetUpperBodyPart(UpperBodyPart? newBodyPart)
    {
        if (upperBodyPart?.model.height > MyGame.partBaseSize.y)
        {
            int temp = upperBodyPart.model.height - (int) MyGame.partBaseSize.y;
            height -= temp;
            lowerBodyPart.y -= temp;
        }
        
        upperBodyPart?.Destroy();
        upperBodyPart = newBodyPart;
        upperBodyPart.SetXY(x,y);
    }

    public void SetLowerBodyPart(LowerBodyPart? newBodyPart)
    {
        if (lowerBodyPart?.model.height > MyGame.partBaseSize.y)
        {
            height -= lowerBodyPart.model.height - (int)MyGame.partBaseSize.y;
        }
        
        lowerBodyPart?.Destroy();
        lowerBodyPart = newBodyPart;
        lowerBodyPart.SetXY(x,y+16);
        lowerBodyPart.CheckIfGrounded();
    }
    

    private void Update()
    {
        wasGrounded = isGrounded;
        lowerBodyPart.HandleMovement();
    }

    /// <summary>
    /// Makes the player jump :O
    /// </summary>
    public void Jump()
    {
        //y = -100/60X + 1
        //y = 100/60X + 0.01f
        // if (Time.deltaTime >= 1) velocity.y -= jumpForce * (1 / 60 * Time.deltaTime);
    }


    /// <summary>
    /// The player has three states: Stand, Walk and Jump, these states are used in movement and
    /// to determine when gravity should be applied and when certain animations
    /// should be played.
    /// </summary>
    public enum State
    {
        Stand,
        Walk,
        Jump
    }

}