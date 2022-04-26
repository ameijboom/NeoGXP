using System;
using TiledMapParser;

namespace GXPEngine.StageManagement
{
    public class Stage : GameObject
    {
        private readonly Map stageData;
        private readonly int tileSize;

        public Stages stage { get;}
        

        /// <summary>
        /// Object that holds all information about the current stage including objects
        /// </summary>
        /// <param name="givenStage">A stage from the Stages.cs list</param>
        public Stage(Stages givenStage)
        {
            MyGame myGame = (MyGame) game;
            parent = myGame;
            
            stage = givenStage;
            string stagePath = "Tiled/" + stage + ".tmx";
            stageData = MapParser.ReadMap(stagePath);
            
            //TileSize is the same as width and width is the same as height
            tileSize = stageData.TileWidth;

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
                int pX = col * tileSize;
                int pY = row * tileSize;

                switch (tileNumbers[col, row])
                {
                    case 1:
                        TestMovableBlock testMovableBlock = new TestMovableBlock();
                        testMovableBlock.SetXY(pX,pY);
                        AddChild(testMovableBlock);
                        break;
                }
            }
        }
    }
}