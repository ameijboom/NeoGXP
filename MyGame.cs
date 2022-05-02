using System;
using GXPEngine.BodyParts;
using GXPEngine.GXPEngine.Core;
using GXPEngine.StageManagement;
using SkiaSharp;

namespace GXPEngine;

public class MyGame : Game
{
    public Player player;

    public static Vec2 partBaseSize = new(16,16);
    public static Vec2 playerBaseSize = new(32, 32);
    public static Vec2 initialPlayerPosition = new Vec2(0, 0);
    public static float globalGravity = 0.01f;
    public static float globalSpeed = 0.5f;
    public static float globalJumpForce = 1.5f;
    
    //From what x does the screen start scrolling
    private readonly int scrollX;
    
    
    private MyGame() : base(1200, 720, false)
    {
        scrollX = width / 2;


        targetFps = 60;
        
        EasyDraw background = new EasyDraw(width, height, false);
        background.Clear(SKColors.LightCyan);
        AddChild(background);
        
        
        
        StageLoader.LoadStage(Stages.Test);
        // player = new Player(width/2.0f,height/2.0f)
        // {
        //     parent = this
        // };
 
    }

    private void Update()
    {
        // Scroll();
        
        // Console.WriteLine($"Time: {Time.deltaTime}");
        
        //Red
        if (Input.GetKeyDown(Key.NUMPAD_1) || Input.GetKeyDown(49)) player.SetUpperBodyPart(new GrapplingHook(player));
        if (Input.GetKeyDown(Key.NUMPAD_2) || Input.GetKeyDown(50)) player.SetLowerBodyPart(new JumpingLegs(player));
        
        //Blue
        if (Input.GetKeyDown(Key.NUMPAD_3) || Input.GetKeyDown(51)) player.SetUpperBodyPart(new BlueUpperBodyPart(player));
        if (Input.GetKeyDown(Key.NUMPAD_4) || Input.GetKeyDown(52)) player.SetLowerBodyPart(new ExtendyLegs(player));

        //Green
        if (Input.GetKeyDown(Key.NUMPAD_5) || Input.GetKeyDown(53)) player.SetUpperBodyPart(new GreenUpperBodyPart(player));
        if (Input.GetKeyDown(Key.NUMPAD_6) || Input.GetKeyDown(54)) player.SetLowerBodyPart(new SpiderLegs(player));
        
        if (Input.GetKeyDown(Key.R))
        {
            player.SetUpperBodyPart(new GrapplingHook(player));
            player.SetLowerBodyPart(new JumpingLegs(player));
            player.SetXY(initialPlayerPosition.x, initialPlayerPosition.y);
        }
    }
    
    void Scroll()
    {
        if (StageLoader.currentStage != null)
        {
			
            //If the player is to the left of the center of the screen it will move to the left with the player until it hits the start of the stage
            if (player.x + StageLoader.currentStage.x < scrollX)
            {
                StageLoader.currentStage.x = scrollX - player.x;
            }
            if (player.x + StageLoader.currentStage.x > width - scrollX)
            {
                StageLoader.currentStage.x = width - scrollX - player.x;
            }
				
            //If the player is to the right of the center of the screen it will move to the right with the player until it hits the end of the stage
            if (StageLoader.currentStage.x > 0)
            {
                StageLoader.currentStage.x = 0;
            }
            else if (StageLoader.currentStage.x < -StageLoader.currentStage.stageWidth + game.width)
            {
                StageLoader.currentStage.x = -StageLoader.currentStage.stageWidth + game.width;
            }

				
				
        }
    }
    private static void Main()
    {
        new MyGame().Start();
    }
}