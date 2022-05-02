using System;
using System.Globalization;
using System.Text;

namespace GXPEngine.BodyParts;

public class LowerBodyPart : BodyPart
{
    protected float speedMultiplier;
    protected float speed;
    protected float jumpMultiplier;
    
    protected LowerBodyPart(string modelPath, int cols, int rows, int frames, Player target_) : base(modelPath, cols, rows, frames, target_)
    {
    }

    protected override void Update()
    {
        // Console.WriteLine($"State: {target.currentState}");
        //
        // if (target.verticalCollision != null)
        //     Console.WriteLine($"Horizontal Normal: {target.verticalCollision.normal}");


    }

    public virtual void HandleMovement()
    {
        if (target.horizontalCollision != null) target.lastHorizontalCollision = target.horizontalCollision;
        if (target.verticalCollision != null) target.lastVerticalCollision = target.verticalCollision;
        
        target.horizontalCollision = target.MoveUntilCollision(target.velocity.x * Time.deltaTime, 0);
        target.verticalCollision = target.MoveUntilCollision(0, target.velocity.y * Time.deltaTime);

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
        target.velocity.y += MyGame.globalGravity * Time.deltaTime;
        CheckIfGrounded();


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
        else if (target.verticalCollision is {normal.y: > 0.5f}) target.velocity.y = 0;
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
        else if (Input.GetKeyDown(Key.SPACE))
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
            // if (target.horizontalCollision != null) DoSlopedMovementIfPossible();
            // else target.velocity.x = -speed;
            target.velocity.x = -speed;
        }
        else if (Input.GetKey(Key.D))
        {
            // if (target.horizontalCollision != null) DoSlopedMovementIfPossible();
            // else target.velocity.x = speed;
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

    private void DoSlopedMovementIfPossible()
    {
        if (target.horizontalCollision == null) return;
        
        if (target.horizontalCollision.normal.x is < -0.5f and > -1 && target.horizontalCollision.normal.y < -0.5f)
        {
            target.velocity.x = speed; 
            target.velocity.y = -speed;
        }
        else if (target.horizontalCollision.normal.x is > 0.5f and < 1 && target.horizontalCollision.normal.y < -0.5f)
        {
            target.velocity.x = -speed;
            target.velocity.y = -speed;
        }
        else target.velocity.y = 0;
        
        // switch (target.horizontalCollision.normal.x)
        // {
        //     case < 0.5f and > 0:
        //         target.velocity.x = speed;
        //         target.velocity.y = -speed;
        //         break;
        //     case > -0.5f and < 0:
        //         target.velocity.x = -speed;
        //         target.velocity.y = -speed;
        //         break;
        // }
    }

    /// <summary>
    /// Might as well jump
    /// </summary>
    protected virtual void Jump()
    {
        target.velocity.y -= MyGame.globalJumpForce * jumpMultiplier;
    }
    
    
    /// <summary>
    /// Checks if the player is grounded based on if the collision normal y has a value lower than -0.5f to ensure jumping works on 45 deg slopes
    /// </summary>
    public virtual void CheckIfGrounded()
    {
        if (target.verticalCollision != null)
        {
            // Console.WriteLine($"Vertical Normal {target.verticalCollision.normal}");
            if (target.verticalCollision.normal.y < -0.5f) target.isGrounded = true;
            else target.isGrounded = false;
        }
        else target.isGrounded = false;
    }

    public override void UpdatePosition()
    {
        SetXY(target.x,target.y + 16);
    }
    
} 
