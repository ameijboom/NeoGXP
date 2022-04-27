using System;
using GXPEngine.BodyParts;
using GXPEngine.Core;
using GXPEngine.GXPEngine.Core;

namespace GXPEngine;

public class Player : Sprite
{
    private Vec2 velocity;
    private State currentState;
    private bool isGrounded;
    private bool wasGrounded;
    
    private float speed;
    private float jumpForce;
    private float gravitationalForce;

    private Collision? horizontalCollision;
    private Collision? verticalCollision;

    private LowerBodyPart lowerBodyPart;
    private UpperBodyPart upperBodyPart;
    
    public Vec2 position => new Vec2(x, y);
    
    public Player(float pX, float pY) : base("hitboxes/playerHitbox.png")
    {
        SetXY(pX,pY);

        velocity = new Vec2(0, 0);

        horizontalCollision = null;
        verticalCollision = null;

        speed = 5;
        gravitationalForce = 0.5f;
        jumpForce = 15;

        upperBodyPart = new PlaceHolderUpperBodyPart();
        lowerBodyPart = new PlaceHolderLowerBodyPart();
        lowerBodyPart.y = 16;
        AddChild(upperBodyPart);
        AddChild(lowerBodyPart);

        CheckIfGrounded();
    }

    public void SetUpperBodyPart(UpperBodyPart newBodyPart)
    {
        RemoveChild(upperBodyPart);
        upperBodyPart = newBodyPart;
        AddChild(upperBodyPart);
    }

    public void SetLowerBodyPart(LowerBodyPart newBodyPart)
    {
        RemoveChild(lowerBodyPart);
        lowerBodyPart = newBodyPart;
        lowerBodyPart.y = 16;
        AddChild(lowerBodyPart);
    }

    private void Update()
    {
        wasGrounded = isGrounded;


        // Console.WriteLine($"vel {velocity}");
        // Console.WriteLine( currentState);
        
        horizontalCollision = MoveUntilCollision(velocity.x,0);
        verticalCollision = MoveUntilCollision(0, velocity.y);
        
        switch (currentState)
        {
            case State.Stand:
                velocity.SetXY(0, 0);

                if (!isGrounded)
                {
                    currentState = State.Jump;
                    break;
                }

                if (Input.GetKey(Key.A) != Input.GetKey(Key.D))
                {
                    currentState = State.Walk;
                }
                else if (Input.GetKeyDown(Key.SPACE))
                {
                    Jump();
                    currentState = State.Jump;
                }
                
                break;
            
            case State.Walk:
                CheckIfGrounded();

                if (Input.GetKey(Key.A) == Input.GetKey(Key.D))
                {
                    currentState = State.Stand;
                    break;
                }
                else if (Input.GetKey(Key.A))
                {
                    velocity.x = -speed;
                }
                else if (Input.GetKey(Key.D))
                {
                    velocity.x = speed;
                }

                if (Input.GetKey(Key.SPACE))
                {
                    Jump();
                    currentState = State.Jump;
                }
                else if (!isGrounded && !wasGrounded)
                {
                    currentState = State.Jump;
                }
                
                break;
            
            case State.Jump:
                CheckIfGrounded();
                velocity.y += gravitationalForce;

                if (Input.GetKey(Key.D) == Input.GetKey(Key.A))
                {
                    velocity.x = 0;
                }
                else if (Input.GetKey(Key.D))
                {
                    velocity.x = speed;
                }
                else if (Input.GetKey(Key.A))
                {
                    velocity.x = -speed;
                }

                if (isGrounded && Input.GetKey(Key.A) != Input.GetKey(Key.D))
                {
                    velocity.y = 0;
                    currentState = State.Walk;
                }
                else if (isGrounded)
                {
                    velocity.y = 0;
                    currentState = State.Stand;
                }
                else if (verticalCollision != null && Math.Abs(verticalCollision.normal.y - 1) < 0.001f) velocity.y = 0;

                break;
        }
    }

    /// <summary>
    /// Makes the player jump :O
    /// </summary>
    private void Jump()
    {
        velocity.y -= jumpForce;
    }

    /// <summary>
    /// Checks if the player is grounded based on if the collision normal y has a value lower than -0.5f to ensure jumping works on 45 deg slopes
    /// </summary>
    private void CheckIfGrounded()
    {
        if (verticalCollision != null)
        {
            Console.WriteLine($"Normal {verticalCollision.normal}");
            if (verticalCollision.normal.y < -0.5f) isGrounded = true;
        }
        else isGrounded = false;
    }

    /// <summary>
    /// The player has three states: Stand, Walk and Jump, these states are used in movement and
    /// to determine when gravity should be applied and when certain animations
    /// should be played.
    /// </summary>
    private enum State
    {
        Stand,
        Walk,
        Jump
    }

}