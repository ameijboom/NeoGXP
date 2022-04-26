using GXPEngine.Core;
using GXPEngine.GXPEngine.Core;

namespace GXPEngine;

public class Player : Sprite
{
    private Vec2 vec2 = new Vec2();
    
    public Player() : base(new Texture2D("square.png"))
    {
        
    }
}