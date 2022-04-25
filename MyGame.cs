namespace GXPEngine;

public class MyGame : Game
{
    private EasyDraw canvas;
    
    
    private MyGame() : base(800, 600, false)
    {
        canvas = new EasyDraw(800, 600);
        AddChild(canvas);
        
        canvas.Ellipse(width/2.0f,height/2.0f,200,200);
    }

    private void Update()
    {
        
    }

    private static void Main()
    {
        new MyGame().Start();
    }
}