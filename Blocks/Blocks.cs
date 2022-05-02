namespace GXPEngine.Blocks;

public class WoodenBlock : MovableBlock
{
    public WoodenBlock() : base("wooden_block.png") {}
}

public class TempSquare : Sprite
{
    public TempSquare() : base("squareSmall.png"){}
}

//Bricks

public class RedBrick : Block
{
    public RedBrick() : base("bricks/red.png"){}
}

public class BlueBrick : Block
{
    public BlueBrick() : base("bricks/blue.png", canGrappleOnto_: true){}
}

public class GreenBrick : Block
{
    public GreenBrick() : base("bricks/green.png"){}
}   

public class YellowBrick : Block
{
    public YellowBrick() : base("bricks/yellow.png"){}
}