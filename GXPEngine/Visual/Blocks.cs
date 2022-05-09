namespace GXPEngine.Visual;

//Bricks

public class RedBrick : Sprite
{
    public RedBrick() : base("bricks/red.png", true, false){}
}

public class BlueBrick : Sprite
{
    public BlueBrick() : base("bricks/blue.png", true, false){}
}

public class GreenBrick : Sprite
{
    public GreenBrick() : base("bricks/green.png",true, false){}
}   

public class YellowBrick : Sprite
{
    public YellowBrick() : base("bricks/yellow.png", true, false){}
}


//Test

public class TestClimbableSurface : Sprite
{
    public TestClimbableSurface() : base("tempBackground.png", true,false){}
}

public class TestGrappleSurface : Sprite
{
    public TestGrappleSurface() : base("squareSmall.png", true, false){}
}