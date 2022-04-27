
using GXPEngine.BodyParts;
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
        //Red
        if (Input.GetKeyDown(Key.NUMPAD_1)) player.SetUpperBodyPart(new RedUpperBodyPart());
        if (Input.GetKeyDown(Key.NUMPAD_2)) player.SetLowerBodyPart(new RedLowerBodyPart());
        
        //Blue
        if (Input.GetKeyDown(Key.NUMPAD_3)) player.SetUpperBodyPart(new BlueUpperBodyPart());
        if (Input.GetKeyDown(Key.NUMPAD_4)) player.SetLowerBodyPart(new BlueLowerBodyPart());

        //Green
        if (Input.GetKeyDown(Key.NUMPAD_5)) player.SetUpperBodyPart(new GreenUpperBodyPart());
        if (Input.GetKeyDown(Key.NUMPAD_6)) player.SetLowerBodyPart(new GreenLowerBodyPart());
        
        if (Input.GetKeyDown(Key.R))
        {
            player.SetUpperBodyPart(new PlaceHolderUpperBodyPart());
            player.SetLowerBodyPart(new PlaceHolderLowerBodyPart());
        }
    }
    private static void Main()
    {
        new MyGame().Start();
    }
}