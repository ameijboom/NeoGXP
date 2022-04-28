//In this file are all classes for the different body parts

using GXPEngine.Core;

namespace GXPEngine.BodyParts
{
    
    
    //Red
    public class JumpingLegs : LowerBodyPart {
        public JumpingLegs(Player target_) : base("bodyParts/test/red/lower.png", 1, 1, 1, target_)
        {
            speed = 8;
            gravitationalForce = 1;
            jumpForce = 25;        
        }
    }
    public class RedUpperBodyPart : UpperBodyPart {public RedUpperBodyPart(Player target_) : base("bodyParts/test/red/upper.png", 1, 1,1, target_){}}

    //Blue
    public class ExtendyLegs : LowerBodyPart {
        public ExtendyLegs(Player target_) : base("bodyParts/test/blue/lower.png", 1, 1, 1, target_)
        {
            speed = 8;
            gravitationalForce = 1;
            jumpForce = 0;
        }

        public override void HandleMovement()
        {
            base.HandleMovement();
            
            Collision? boundaryCollision;


            if (Input.GetKey(Key.UP))
            {
                boundaryCollision = target.MoveUntilCollision(0, -1);
                if (boundaryCollision == null)
                {
                    target.height += 1;
                    model.height += 1;
                }
            }
            else if (Input.GetKey(Key.DOWN))
            {
                boundaryCollision = target.MoveUntilCollision(0,1);
                if (boundaryCollision is {normal: {y: < -0.5f}} && target.height > MyGame.partBaseSize.y*1.5f)
                {
                    target.height -= 1;
                    model.height -= 1;
                }


            }
        }
    }
    public class BlueUpperBodyPart : UpperBodyPart {public BlueUpperBodyPart(Player target_) : base("bodyParts/test/blue/upper.png", 1, 1, 1, target_){}}

    //Green
    public class GreenLowerBodyPart : LowerBodyPart {public GreenLowerBodyPart(Player target_) : base("bodyParts/test/green/lower.png",1,1,1, target_){}}
    public class GreenUpperBodyPart : UpperBodyPart {public GreenUpperBodyPart(Player target_) : base("bodyParts/test/green/upper.png",1,1,1, target_){}}
    
    //PlaceHolder
    public class PlaceHolderLowerBodyPart : LowerBodyPart {public PlaceHolderLowerBodyPart(Player target_) : base("bodyParts/placeHolder/lower.png", 1, 1, 1, target_){}}
    public class PlaceHolderUpperBodyPart : UpperBodyPart {public PlaceHolderUpperBodyPart(Player target_) : base("bodyParts/placeHolder/upper.png", 1, 1, 1, target_){}}
}
