//In this file are all classes for the different body parts

namespace GXPEngine.BodyParts
{
    //Red
    public class RedLowerBodyPart : LowerBodyPart {public RedLowerBodyPart() : base("bodyParts/test/red/lower.png",1,1,1){}}
    public class RedUpperBodyPart : UpperBodyPart {public RedUpperBodyPart() : base("bodyParts/test/red/upper.png", 1, 1,1){}}

    //Blue
    public class BlueLowerBodyPart : LowerBodyPart {public BlueLowerBodyPart() : base("bodyParts/test/blue/lower.png", 1, 1, 1){}}
    public class BlueUpperBodyPart : UpperBodyPart {public BlueUpperBodyPart() : base("bodyParts/test/blue/upper.png", 1, 1, 1){}}

    //Green
    public class GreenLowerBodyPart : LowerBodyPart {public GreenLowerBodyPart() : base("bodyParts/test/green/lower.png",1,1,1){}}
    public class GreenUpperBodyPart : UpperBodyPart {public GreenUpperBodyPart() : base("bodyParts/test/green/upper.png",1,1,1){}}
    
    //PlaceHolder
    public class PlaceHolderLowerBodyPart : LowerBodyPart {public PlaceHolderLowerBodyPart() : base("bodyParts/placeHolder/lower.png", 1, 1, 1){}}
    public class PlaceHolderUpperBodyPart : UpperBodyPart {public PlaceHolderUpperBodyPart() : base("bodyParts/placeHolder/upper.png", 1, 1, 1){}}
}
