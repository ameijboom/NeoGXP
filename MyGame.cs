
using System.Net.NetworkInformation;
using GXPEngine.StageManagement;

namespace GXPEngine;

public class MyGame : Game
{
    private Player player;
    private MyGame() : base(800, 800, false)
    {
        StageLoader.LoadStage(Stages.Test);
        player = new Player(width/2.0f,height/2.0f);
        StageLoader.AddObject(player);
    }

    private void Update()
    {
        
    }
    private static void Main()
    {
        new MyGame().Start();
    }
}