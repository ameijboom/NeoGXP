using System.Buffers.Text;
using System.Reflection.Metadata;
using GXPEngine.GXPEngine.Core;
using GXPEngine.StageManagement;

namespace GXPEngine;

public class TestMovableBlock : Sprite
{
    private bool held;
    private Vec2 relative;

    private EasyDraw canvas;

    private Vec2 mousePos;


    public TestMovableBlock() : base("squareSmall.png")
    {
        canvas = new EasyDraw(width, height);
        AddChild(canvas);
    }

    private void Update()
    {
        mousePos = new Vec2(Input.mouseX, Input.mouseY);
        
        if (Input.GetMouseButtonDown(0) && HitTestPoint(mousePos.x, mousePos.y))
        {
            held = true;
            relative = new Vec2(mousePos.x - x, mousePos.y - y);
        }
        else if (Input.GetMouseButtonUp(0)) held = false;

        if (held)
        {
            SetXY(mousePos.x - relative.x, mousePos.y - relative.y);
            canvas.ClearTransparent();
            canvas.Text($"{x}, {y}");
        }
    }
}