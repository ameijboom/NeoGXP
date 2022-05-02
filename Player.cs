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

    public Collision? horizontalCollision;
    public Collision? verticalCollision;
    public Collision? lastHorizontalCollision;
    public Collision? lastVerticalCollision;

    public LowerBodyPart? lowerBodyPart;
    public UpperBodyPart? upperBodyPart;


    public Vec2 position => new Vec2(x, y);
    
    public Player(float pX, float pY) : base("hitboxes/playerHitbox.png")
    {
        SetXY(pX,pY);

        velocity = new Vec2(0, 0);

        horizontalCollision = null;
        verticalCollision = null;
        lastHorizontalCollision = null;
        lastVerticalCollision = null;

        lowerBodyPart = null;
        upperBodyPart = null;

        SetUpperBodyPart(new GrapplingHook(this));
        SetLowerBodyPart(new JumpingLegs(this));
    }

    public void SetUpperBodyPart(UpperBodyPart? newBodyPart)
    {
        if (upperBodyPart?.model.height > MyGame.partBaseSize.y)
        {
            int temp = upperBodyPart.model.height - (int) MyGame.partBaseSize.y;
            height -= temp;
            if (lowerBodyPart != null) lowerBodyPart.y -= temp;
        }
        
        upperBodyPart?.Destroy();
        upperBodyPart = newBodyPart;
        upperBodyPart?.SetXY(x, y);
    }

    public void SetLowerBodyPart(LowerBodyPart? newBodyPart)
    {
        if (lowerBodyPart?.model.height > MyGame.partBaseSize.y || lowerBodyPart?.model.height < MyGame.partBaseSize.y)
        {
            height -= lowerBodyPart.model.height - (int)MyGame.partBaseSize.y;
        }
        
        lowerBodyPart?.Destroy();
        lowerBodyPart = newBodyPart;
        lowerBodyPart?.SetXY(x,y+MyGame.partBaseSize.y);
        
        if (lastVerticalCollision != null) Console.WriteLine(lastVerticalCollision.normal.y);
        
        if (lowerBodyPart != null && lastVerticalCollision != null && HitTest(lastVerticalCollision.other)) y -= MyGame.partBaseSize.y;
        
        lowerBodyPart?.CheckIfGrounded();
    }
    

    private void Update()
    {
        wasGrounded = isGrounded;
        
        lowerBodyPart?.HandleMovement();
        lowerBodyPart?.UpdatePosition();
        upperBodyPart?.UpdatePosition();
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