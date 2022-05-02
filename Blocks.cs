namespace GXPEngine;

public class WoodenBlock : MovableBlock
{
    public WoodenBlock() : base("wooden_block.png") {}
}

public class TempSquare : Sprite
{
    public TempSquare() : base("squareSmall.png"){}
}

//Bricks

public class RedBrick : Sprite
{
    public RedBrick() : base("bricks/red.png", true){}
}

public class BlueBrick : Sprite
{
    public BlueBrick() : base("bricks/blue.png", true){}
}

public class GreenBrick : Sprite
{
    public GreenBrick() : base("bricks/green.png", true){}
}   

public class YellowBrick : Sprite
{
    public YellowBrick() : base("bricks/yellow.png", true){}
}