//In this file are all classes for the different body parts

using GXPEngine.Core;
using GXPEngine.StageManagement;

namespace GXPEngine.BodyParts
{
    
    
    //Red
    public class JumpingLegs : LowerBodyPart {
        public JumpingLegs(Player target_) : base("bodyParts/test/red/lower.png", 1, 1, 1, target_)
        {
            jumpMultiplier = 1;
            speedMultiplier = 1;

            speed = speedMultiplier * MyGame.globalSpeed;
        }
    }

    public class GrapplingHook : UpperBodyPart
    {
        public GrapplingHook(Player target_) : base("bodyParts/test/red/upper.png", 1, 1, 1, target_)
        {
            
        }
    }

    //Blue
    public class ExtendyLegs : LowerBodyPart
    {
        private int extendSpeed;
        private float crouchIntensity;
        public ExtendyLegs(Player target_) : base("bodyParts/test/blue/lower.png", 1, 1, 1, target_)
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
                boundaryCollision = target.MoveUntilCollision(0, -extendSpeed);
                if (boundaryCollision == null)
                {
                    target.height += extendSpeed;
                    model.height += extendSpeed;
                }
            }
            else if (Input.GetKey(Key.DOWN))
            {
                boundaryCollision = target.MoveUntilCollision(0,extendSpeed);
                if (boundaryCollision is {normal: {y: < -0.5f}} && target.height > MyGame.partBaseSize.y*(1+crouchIntensity))
                {
                    target.height -= extendSpeed;
                    model.height -= extendSpeed;
                }


            }
        }
    }
    public class BlueUpperBodyPart : UpperBodyPart {public BlueUpperBodyPart(Player target_) : base("bodyParts/test/blue/upper.png", 1, 1, 1, target_){}}

    //Green
    public class SpiderLegs : LowerBodyPart
    {
        private bool inSpiderForm;
        private float climbSpeed;

        public SpiderLegs(Player target_) : base("bodyParts/test/green/lower.png", 1, 1, 1, target_)
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
            foreach (Climbable climbable in StageLoader.GetClimbables())
            {
                if (climbable.HitTest(target))
                {
                     inSpiderForm = true;
                }
            }
            
            if (!inSpiderForm) base.HandleMovement();

            
            if (inSpiderForm)
            {
                if (Input.GetKey(Key.W))
                {
                    target.MoveUntilCollision(0, -climbSpeed * Time.deltaTime);
                }

                if (Input.GetKey(Key.A))
                {
                    target.MoveUntilCollision(-climbSpeed * Time.deltaTime, 0);
                }

                if (Input.GetKey(Key.D))
                {
                    target.MoveUntilCollision(climbSpeed * Time.deltaTime, 0);
                }

                if (Input.GetKey(Key.S))
                {
                    target.MoveUntilCollision(0, climbSpeed * Time.deltaTime);
                }
            }
            
        }
    }
    public class GreenUpperBodyPart : UpperBodyPart {public GreenUpperBodyPart(Player target_) : base("bodyParts/test/green/upper.png",1,1,1, target_){}}
    
    //PlaceHolder
    public class PlaceHolderLowerBodyPart : LowerBodyPart {public PlaceHolderLowerBodyPart(Player target_) : base("bodyParts/placeHolder/lower.png", 1, 1, 1, target_){}}
    public class PlaceHolderUpperBodyPart : UpperBodyPart {public PlaceHolderUpperBodyPart(Player target_) : base("bodyParts/placeHolder/upper.png", 1, 1, 1, target_){}}
}
