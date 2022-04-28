using System;
using GXPEngine.Core;

namespace GXPEngine.BodyParts;

public class LowerBodyPart : BodyPart
{
    protected float speed;
    protected float gravitationalForce;
    protected float jumpForce;
    
    protected LowerBodyPart(string modelPath, int cols, int rows, int frames, Player target_) : base(modelPath, cols, rows, frames, target_)
    {
    }

    protected override void Update()
    {
        UpdatePosition();
    }

    public virtual void HandleMovement()
    {
        target.horizontalCollision = target.MoveUntilCollision(target.velocity.x, 0);
        target.verticalCollision = target.MoveUntilCollision(0, target.velocity.y);

        switch (target.currentState)
        {
            case Player.State.Stand:
                StandState();
                break;

            case Player.State.Walk:
                WalkState();
                break;

            case Player.State.Jump:
                JumpState();
                break;
        }
    }
    
    protected virtual void JumpState()
    {
        CheckIfGrounded();
        target.velocity.y += gravitationalForce;

        if (Input.GetKey(Key.D) == Input.GetKey(Key.A))
        {
            target.velocity.x = 0;
        }
        else if (Input.GetKey(Key.D))
        {
            target.velocity.x = speed;
        }
        else if (Input.GetKey(Key.A))
        {
            target.velocity.x = -speed;
        }

        if (target.isGrounded && Input.GetKey(Key.A) != Input.GetKey(Key.D))
        {
            target.velocity.y = 0;
            target.currentState = Player.State.Walk;
        }
        else if (target.isGrounded)
        {
            target.velocity.y = 0;
            target.currentState = Player.State.Stand;
        }
        else if (target.verticalCollision != null && Math.Abs(target.verticalCollision.normal.y - 1) < 0.001f) target.velocity.y = 0;
    }

    protected virtual void StandState()
    {
        target.velocity.SetXY(0, 0);

        if (!target.isGrounded)
        {
            target.currentState = Player.State.Jump;
            return;
        }

        if (Input.GetKey(Key.A) != Input.GetKey(Key.D))
        {
            target.currentState = Player.State.Walk;
        }
        else if (Input.GetKey(Key.SPACE))
        {
            Jump();
            target.currentState = Player.State.Jump;
        }
    }

    protected virtual void WalkState()
    {
        CheckIfGrounded();

        if (Input.GetKey(Key.A) == Input.GetKey(Key.D))
        {
            target.currentState = Player.State.Stand;
            return;
        }
        else if (Input.GetKey(Key.A))
        {
            target.velocity.x = -speed;
        }
        else if (Input.GetKey(Key.D))
        {
            target.velocity.x = speed;
        }

        if (Input.GetKey(Key.SPACE))
        {
            Jump();
            target.currentState = Player.State.Jump;
        }
        else if (!target.isGrounded && !target.wasGrounded)
        {
            target.currentState = Player.State.Jump;
        }
    }

    /// <summary>
    /// Might as well jump
    /// </summary>
    protected virtual void Jump()
    {
        target.velocity.y -= jumpForce;
    }
    
    
    /// <summary>
    /// Checks if the player is grounded based on if the collision normal y has a value lower than -0.5f to ensure jumping works on 45 deg slopes
    /// </summary>
    public virtual void CheckIfGrounded()
    {
        if (target.verticalCollision != null)
        {
            Console.WriteLine($"Normal {target.verticalCollision.normal}");
            if (target.verticalCollision.normal.y < -0.5f) target.isGrounded = true;
        }
        else target.isGrounded = false;
    }

    protected override void UpdatePosition()
    {
        SetXY(target.x,target.y + 16);
    }
    
} 
