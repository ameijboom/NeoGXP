using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine.Visual;
using TiledMapParser;

namespace GXPEngine.StageManagement
{
    public class Stage : GameObject
    {
        private readonly Map stageData;
        public readonly int tileWidth;
        public readonly int tileHeight;
        public readonly int stageWidth;
        public readonly int stageHeight;

        public Stages stage { get;}



        public Pivot climbableSurfaces;
        public Pivot grappleSurfaces;
        public Pivot surfaces;

        
        // public List<Hitbox> climbableSurfaces;
        // public List<Hitbox> grappleSurfaces;
        // public List<Hitbox> surfaces;

        private MyGame myGame;

        public SpriteBatch spriteBatch;
        public Pivot backgroundSprites;

        public EasyDraw background;

        

        /// <summary>
        /// Object that holds all information about the current stage including objects
        /// </summary>
        /// <param name="givenStage">A stage from the Stages.cs list</param>
        public Stage(Stages givenStage)
        {
            myGame = (MyGame) game;
            parent = myGame;


            surfaces = new Pivot();
            grappleSurfaces = new Pivot();
            climbableSurfaces = new Pivot();
            
            stage = givenStage;
            string stagePath = "Tiled/" + stage + ".tmx";
            stageData = MapParser.ReadMap(stagePath);

            spriteBatch = new SpriteBatch();
            backgroundSprites = new Pivot();
            
            
            tileWidth = stageData.TileWidth;
            tileHeight = stageData.TileHeight;
            stageWidth = stageData.Width * tileWidth;
            stageHeight = stageData.Height * tileHeight;

            background = new EasyDraw(stageWidth, stageHeight, false);
            background.Clear(Color.LightCyan);
            AddChildAt(background,0);

            

            if (stageData.Layers is not {Length: > 0})
            {
                throw new Exception("Tile file " + stagePath + " does not contain a layer!");
            }

            LoadStage();

        }

        private void Update()
        {
            // Console.WriteLine($"Player: {myGame.player.Index}");
            //
            // Console.WriteLine($"BackgroundSprites: {backgroundSprites.Index}");
            //
            // Console.WriteLine($"SpriteBatch: {spriteBatch.Index}");


            if (myGame.player.lowerBodyPart?.Index < GetChildCount())
            {
                AddChildAt(myGame.player.lowerBodyPart,GetChildCount());
            }

            if (myGame.player.upperBodyPart?.Index < myGame.player.lowerBodyPart?.Index)
            {
                AddChildAt(myGame.player.upperBodyPart, GetChildCount());
            }
            
        }

        /// <summary>
        /// Loads in the stage from tiled
        /// </summary>
        private void LoadStage()
        {
            Layer mainLayer = stageData.Layers[0];
            
            short [,] tileNumbers = mainLayer.GetTileArray();

            for (int col = 0; col < mainLayer.Width; col++)
            for (int row = 0; row < mainLayer.Height; row++)
            {
                int pX = col * tileWidth;
                int pY = row * tileHeight;

                switch (tileNumbers[col, row])
                {
                    case 1:
                        TestGrappleSurface testGrappleSurface = new();
                        testGrappleSurface.SetXY(pX,pY);
                        spriteBatch.AddChild(testGrappleSurface);
                        break;
                    
                    case 2:
                        TestClimbableSurface testClimbableSurface = new();
                        testClimbableSurface.SetXY(pX,pY-16);
                        backgroundSprites.AddChild(testClimbableSurface);
                        break;
                    
                    case 3:
                        // WoodenHitbox woodenHitbox = new ();
                        // woodenHitbox.SetXY(pX,pY);
                        // spriteBatch.AddChild(woodenHitbox);
                        break;
                    
                    case 4:
                        BlueBrick blueBrick = new ();
                        blueBrick.SetXY(pX,pY);
                        spriteBatch.AddChild(blueBrick);
                        break;
                    
                    case 5:
                        GreenBrick greenBrick = new ();
                        greenBrick.SetXY(pX,pY);
                        spriteBatch.AddChild(greenBrick);
                        break;
                    
                    case 6:
                        RedBrick redBrick = new ();
                        redBrick.SetXY(pX,pY);
                        spriteBatch.AddChild(redBrick);
                        break;
                    
                    case 7:
                        YellowBrick yellowBrick = new ();
                        yellowBrick.SetXY(pX,pY);
                        spriteBatch.AddChild(yellowBrick);
                        break;
                    
                    case 8:
                        myGame.player = new Player(pX, pY - 13);
                        MyGame.initialPlayerPosition = new Vec2(pX, pY - 13);
                        AddChildAt(myGame.player, 4);
                        break;
                }
            }
            
            AddChildAt(spriteBatch, 1);
            spriteBatch.Freeze();
            
            AddChildAt(backgroundSprites,2);


            foreach (ObjectGroup objectGroup in stageData.ObjectGroups)
            {
                if (objectGroup.Objects == null) continue;
                
                foreach (TiledObject tiledObject in objectGroup.Objects)
                {
                    Hitbox hitbox;

                    void SetDimensionsCorrect(Hitbox pHitbox)
                    {
                        pHitbox.SetXY(tiledObject.X,tiledObject.Y);
                        pHitbox.width = (int) tiledObject.Width;
                        pHitbox.height = (int) tiledObject.Height;
                    }
                    
                    switch (objectGroup.Name)
                    {
                        case "normal":
                            hitbox = new Hitbox();
                            SetDimensionsCorrect(hitbox);
                            surfaces.AddChild(hitbox);
                            break;
                    
                        case "climbable":
                            hitbox = new Hitbox(climbable_: true);
                            SetDimensionsCorrect(hitbox);
                            climbableSurfaces.AddChild(hitbox);
                            break;
                    
                        case "grapple":
                            hitbox = new Hitbox(canGrappleOnto_: true);
                            SetDimensionsCorrect(hitbox);
                            grappleSurfaces.AddChild(hitbox);
                            break;
                    }
                }
            }
            
            AddChild(grappleSurfaces);
            AddChild(climbableSurfaces);
            AddChild(surfaces);
        }
    }
}