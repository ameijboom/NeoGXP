
namespace GXPEngine;

public class MyGame : Game
{
    private MyGame() : base(800, 600, false)
    {
        Player player = new Player();
        AddChild(player);
    }

    private void Update()
    {
        
    }
    private static void Main()
    {
        new MyGame().Start();
    }
}