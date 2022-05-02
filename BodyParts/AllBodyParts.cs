//In this file are all classes for the different body parts

using System;
using System.Diagnostics;
using GXPEngine.Blocks;
using GXPEngine.Core;
using GXPEngine.GXPEngine.Core;
using GXPEngine.StageManagement;

namespace GXPEngine.BodyParts
{
    
    
    //Red
    public class JumpingLegs : LowerBodyPart {
        public JumpingLegs(Player player_) : base("bodyParts/test/red/lower.png", 1, 1, 1, player_)
        {
            jumpMultiplier = 1;
            speedMultiplier = 1;

            speed = speedMultiplier * MyGame.globalSpeed;
        }
    }

    public class GrapplingHook : UpperBodyPart
    {
        private readonly float grapplePower;
        private Hook? hook;
        private bool pulling;
        private readonly float pullStartingPower;
        private float pullPower;
        private readonly float pullPowerIncrement;
        private readonly float maxPullPower;
        
        public GrapplingHook(Player player_) : base("bodyParts/test/red/upper.png", 1, 1, 1, player_)
        {
            SetAbilityModel("bodyParts/test/red/ability.png",1,1,1);

            grapplePower = 1.5f;
            pullPower = 0.05f;
            pullStartingPower = MyGame.globalSpeed * pullPower;
            
            pullPowerIncrement = 0.01f;
            maxPullPower = 1;


            hook = null;
            pulling = false;
        }

        protected override void UseAbility()
        {
            if (abilityModel == null) return;

            if (hook != null && !pulling && hook.hasHit)
            {
                pulling = true;
                if (player.lowerBodyPart != null) player.lowerBodyPart.disableKeyAndGravityMovement = true;
            }
            else if (hook is {hasHit: false})
            {
                hook.Destroy();
                hook = null;
            }
            else if (pulling && hook is {hasHit: true})
            {
                StopPulling();
            }
            else
            {
                pulling = false;
                hook = new Hook(x + abilityModel.x, y + abilityModel.y/2.0f, abilityDirection.SetMagnitude(grapplePower * MyGame.globalSpeed));
                myGame.AddChildAt(hook, 1);
                hook.rotation = abilityModel.rotation;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (pulling && hook != null)
            {
                Vec2 direction = new Vec2(hook.x, hook.y) - new Vec2(player.x + MyGame.partBaseSize.x / 2.0f, player.y + MyGame.partBaseSize.y);
                
                if (direction.Magnitude() < 20 || direction.y > 0)
                {
                    StopPulling();
                    return;
                }

                player.velocity = direction.SetMagnitude(pullPower);

                if (pullPower < maxPullPower)
                {
                    pullPower += pullPowerIncrement * Time.deltaTime;
                }

                if (player.horizontalCollision != null || player.verticalCollision != null)
                {
                    StopPulling();
                }

            }
        }

        private void StopPulling()
        {
            pulling = false;
            hook?.LateDestroy();
            hook = null;
            player.lowerBodyPart?.CheckIfGrounded();

            if (!player.isGrounded) player.currentState = Player.State.Jump;
            
            player.lowerBodyPart!.disableKeyAndGravityMovement = false;
        }
    }

    //Blue
    public class ExtendyLegs : LowerBodyPart
    {
        private readonly int extendSpeed;
        private readonly float crouchIntensity;
        public ExtendyLegs(Player player_) : base("bodyParts/test/blue/lower.png", 1, 1, 1, player_)
        {
            jumpMultiplier = 0;
            speedMultiplier = 1;
            speed = MyGame.globalSpeed * speedMultiplier;
            extendSpeed = 2;
            crouchIntensity = 0.2f;
        }

        public override void HandleMovement()
        {
            base.HandleMovement();
            
            Collision? boundaryCollision;


            if (Input.GetKey(Key.UP))
            {
                boundaryCollision = player.MoveUntilCollision(0, -extendSpeed);
                if (boundaryCollision == null)
                {
                    player.height += extendSpeed;
                    model.height += extendSpeed;
                }
            }
            else if (Input.GetKey(Key.DOWN))
            {
                boundaryCollision = player.MoveUntilCollision(0,extendSpeed);
                if (boundaryCollision is {normal: {y: < -0.5f}} && player.height > MyGame.partBaseSize.y*(1+crouchIntensity))
                {
                    player.height -= extendSpeed;
                    model.height -= extendSpeed;
                }


            }
        }
    }

    public class BlueUpperBodyPart : UpperBodyPart
    {
        public BlueUpperBodyPart(Player player_) : base("bodyParts/test/blue/upper.png", 1, 1, 1, player_)
        {
            SetAbilityModel("bodyParts/test/blue/ability.png",1,1,1);
        }
        protected override void UseAbility()
        {
        }
    }

    //Green
    public class SpiderLegs : LowerBodyPart
    {
        private bool inSpiderForm;
        private float climbSpeed;

        public SpiderLegs(Player player_) : base("bodyParts/test/green/lower.png", 1, 1, 1, player_)
        {
            jumpMultiplier = 0;
            inSpiderForm = false;
            speedMultiplier = 1;

            speed = MyGame.globalSpeed * speedMultiplier;

            const float climbMultiplier = 0.3f;
            climbSpeed = climbMultiplier * speed;
        }

        public override void HandleMovement()
        {

            inSpiderForm = false;

            foreach (Block block in StageLoader.currentStage.blocks)
            {
                if (block.climbable)
                {
                    if (player.HitTest(block)) inSpiderForm = true;
                } 
            }
            
            if (!inSpiderForm) base.HandleMovement();

            
            if (inSpiderForm)
            {
                if (Input.GetKey(Key.W))
                {
                    player.MoveUntilCollision(0, -climbSpeed * Time.deltaTime);
                }

                if (Input.GetKey(Key.A))
                {
                    player.MoveUntilCollision(-climbSpeed * Time.deltaTime, 0);
                }

                if (Input.GetKey(Key.D))
                {
                    player.MoveUntilCollision(climbSpeed * Time.deltaTime, 0);
                }

                if (Input.GetKey(Key.S))
                {
                    player.MoveUntilCollision(0, climbSpeed * Time.deltaTime);
                }
            }
            
        }
    }

    public class GreenUpperBodyPart : UpperBodyPart
    {
        public GreenUpperBodyPart(Player player_) : base("bodyParts/test/green/upper.png", 1, 1, 1, player_)
        {
            SetAbilityModel("bodyParts/test/green/ability.png",1,1,1);
        }
        
        protected override void UseAbility()
        {
        }
    }
    
    //PlaceHolder
    public class PlaceHolderLowerBodyPart : LowerBodyPart {public PlaceHolderLowerBodyPart(Player player_) : base("bodyParts/placeHolder/lower.png", 1, 1, 1, player_){}}

    public class PlaceHolderUpperBodyPart : UpperBodyPart
    {
        public PlaceHolderUpperBodyPart(Player player_) : base("bodyParts/placeHolder/upper.png", 1, 1, 1, player_){}
        protected override void UseAbility()
        {
        }
    }
}
