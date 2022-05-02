using System;
using GXPEngine.GXPEngine.Core;
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

        private MyGame myGame;

        

        /// <summary>
        /// Object that holds all information about the current stage including objects
        /// </summary>
        /// <param name="givenStage">A stage from the Stages.cs list</param>
        public Stage(Stages givenStage)
        {
            myGame = (MyGame) game;
            parent = myGame;
            
            stage = givenStage;
            string stagePath = "Tiled/" + stage + ".tmx";
            stageData = MapParser.ReadMap(stagePath);
            
            //TileSize is the same as width and width is the same as height
            tileWidth = stageData.TileWidth;
            tileHeight = stageData.TileHeight;
            stageWidth = stageData.Width * tileWidth;
            stageHeight = stageData.Height * tileHeight;

            if (stageData.Layers == null || stageData.Layers.Length <= 0)
            {
                throw new Exception("Tile file " + stagePath + " does not contain a layer!");
            }

            LoadStage();

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
                        TempSquare tempSquare = new TempSquare();
                        tempSquare.SetXY(pX,pY);
                        AddChild(tempSquare);
                        break;
                    
                    case 2:
                        Climbable climbable = new Climbable();
                        climbable.SetXY(pX,pY);
                        AddChildAt(climbable, 0);
                        break;
                    
                    case 3:
                        WoodenBlock woodenBlock = new WoodenBlock();
                        woodenBlock.SetXY(pX,pY);
                        AddChild(woodenBlock);
                        break;
                    
                    case 4:
                        BlueBrick blueBrick = new BlueBrick();
                        blueBrick.SetXY(pX,pY);
                        AddChild(blueBrick);
                        break;
                    
                    case 5:
                        GreenBrick greenBrick = new GreenBrick();
                        greenBrick.SetXY(pX,pY);
                        AddChild(greenBrick);
                        break;
                    
                    case 6:
                        RedBrick redBrick = new RedBrick();
                        redBrick.SetXY(pX,pY);
                        AddChild(redBrick);
                        break;
                    
                    case 7:
                        YellowBrick yellowBrick = new YellowBrick();
                        yellowBrick.SetXY(pX,pY);
                        AddChild(yellowBrick);
                        break;
                    
                    case 8:
                        myGame.player = new Player(pX, pY-13)
                        {
                            parent = game
                        };
                        MyGame.initialPlayerPosition = new Vec2(pX, pY - 13);
                        break;
                }
            }
        }
    }
}