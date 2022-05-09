using System;
using GXPEngine.StageManagement;

namespace GXPEngine.BodyParts;

public class LowerBodyPart : BodyPart
{
    protected float speedMultiplier;
    protected float speed;
    protected float jumpMultiplier;

    public bool disableKeyAndGravityMovement;
    
    protected LowerBodyPart(string modelPath, int cols, int rows, int frames, Player player_) : base(modelPath, cols, rows, frames, player_)
    {
        disableKeyAndGravityMovement = false;
    }

    protected override void Update()
    {
        // Console.WriteLine($"State: {player.currentState}");
        //
        // if (player.verticalCollision != null)
        //     Console.WriteLine($"Horizontal Normal: {player.verticalCollision.normal}");


    }

    public virtual void HandleMovement()
    {
        if (player.horizontalCollision != null) player.lastHorizontalCollision = player.horizontalCollision;
        if (player.verticalCollision != null) player.lastVerticalCollision = player.verticalCollision;
        
        player.horizontalCollision = player.MoveUntilCollision(player.velocity.x * Time.deltaTime, 0, StageLoader.currentStage.surfaces.GetChildren());
        player.verticalCollision = player.MoveUntilCollision(0, player.velocity.y * Time.deltaTime, StageLoader.currentStage.surfaces.GetChildren());

        if (!disableKeyAndGravityMovement)
        {
            switch (player.currentState)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    protected virtual void JumpState()
    {
        player.velocity.y += MyGame.globalGravity * Time.deltaTime;
        CheckIfGrounded();


        if (Input.GetKey(Key.D) && Input.GetKey(Key.A))
        {
            player.velocity.x = 0;
        } 
        if (Input.GetKey(Key.D))
        {
            player.velocity.x = speed;
        }
        else if (Input.GetKey(Key.A))
        {
            player.velocity.x = -speed;
        }

        if (player.isGrounded && Input.GetKey(Key.A) != Input.GetKey(Key.D))
        {
            player.velocity.y = 0;
            player.currentState = Player.State.Walk;
        }
        else if (player.isGrounded)
        {
            player.velocity.y = 0;
            player.currentState = Player.State.Stand;
        }
        else if (player.verticalCollision is {normal.y: > 0.5f}) player.velocity.y = 0;
    }

    protected virtual void StandState()
    {
        player.velocity.SetXY(0, 0);

        if (!player.isGrounded)
        {
            player.currentState = Player.State.Jump;
            return;
        }

        if (Input.GetKey(Key.A) != Input.GetKey(Key.D))
        {
            player.currentState = Player.State.Walk;
        }
        else if (Input.GetKeyDown(Key.SPACE))
        {
            Jump();
            player.currentState = Player.State.Jump;
        }
    }

    protected virtual void WalkState()
    {
        CheckIfGrounded();

        if (Input.GetKey(Key.A) == Input.GetKey(Key.D))
        {
            player.currentState = Player.State.Stand;
            return;
        }
        if (Input.GetKey(Key.A))
        {
            // if (target.horizontalCollision != null) DoSlopedMovementIfPossible();
            // else target.velocity.x = -speed;
            player.velocity.x = -speed;
        }
        if (Input.GetKey(Key.D))
        {
            // if (target.horizontalCollision != null) DoSlopedMovementIfPossible();
            // else target.velocity.x = speed;
            player.velocity.x = speed;
        }

        if (Input.GetKey(Key.SPACE))
        {
            Jump();
            player.currentState = Player.State.Jump;
        }
        else if (!player.isGrounded && !player.wasGrounded)
        {
            player.currentState = Player.State.Jump;
        }
    }

    private void DoSlopedMovementIfPossible()
    {
        if (player.horizontalCollision == null) return;
        
        if (player.horizontalCollision.normal.x is < -0.5f and > -1 && player.horizontalCollision.normal.y < -0.5f)
        {
            player.velocity.x = speed; 
            player.velocity.y = -speed;
        }
        else if (player.horizontalCollision.normal.x is > 0.5f and < 1 && player.horizontalCollision.normal.y < -0.5f)
        {
            player.velocity.x = -speed;
            player.velocity.y = -speed;
        }
        else player.velocity.y = 0;
        
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
        player.velocity.y -= MyGame.globalJumpForce * jumpMultiplier;
    }
    
    
    /// <summary>
    /// Checks if the player is grounded based on if the collision normal y has a value lower than -0.5f to ensure jumping works on 45 deg slopes
    /// </summary>
    public virtual void CheckIfGrounded()
    {
        if (player.verticalCollision != null)
        {
            // Console.WriteLine($"Vertical Normal {target.verticalCollision.normal}");
            if (player.verticalCollision.normal.y < -0.5f) player.isGrounded = true;
            else player.isGrounded = false;
        }
        else player.isGrounded = false;
    }

    public override void UpdatePosition()
    {
        SetXY(player.x,player.y + 16);
    }
    
} 
