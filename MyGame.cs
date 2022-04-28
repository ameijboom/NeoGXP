
using GXPEngine.BodyParts;
using GXPEngine.GXPEngine.Core;
using GXPEngine.StageManagement;

namespace GXPEngine;

public class MyGame : Game
{
    public Player player;

    public static Vec2 partBaseSize = new(16,16);
    public static Vec2 playerBaseSize = new(32, 32);
    private MyGame() : base(800, 800, false)
    {
        StageLoader.LoadStage(Stages.Test);
        player = new Player(width/2.0f,height/2.0f)
        {
            parent = this
        };
 
    }

    private void Update()
    {
        //Red
        if (Input.GetKeyDown(Key.NUMPAD_1)) player.SetUpperBodyPart(new RedUpperBodyPart(player));
        if (Input.GetKeyDown(Key.NUMPAD_2)) player.SetLowerBodyPart(new JumpingLegs(player));
        
        //Blue
        if (Input.GetKeyDown(Key.NUMPAD_3)) player.SetUpperBodyPart(new BlueUpperBodyPart(player));
        if (Input.GetKeyDown(Key.NUMPAD_4)) player.SetLowerBodyPart(new ExtendyLegs(player));

        //Green
        if (Input.GetKeyDown(Key.NUMPAD_5)) player.SetUpperBodyPart(new GreenUpperBodyPart(player));
        if (Input.GetKeyDown(Key.NUMPAD_6)) player.SetLowerBodyPart(new GreenLowerBodyPart(player));
        
        if (Input.GetKeyDown(Key.R))
        {
            player.SetUpperBodyPart(new PlaceHolderUpperBodyPart(player));
            player.SetLowerBodyPart(new PlaceHolderLowerBodyPart(player));
        }
    }
    private static void Main()
    {
        new MyGame().Start();
    }
}