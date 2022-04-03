using Microsoft.Xna.Framework;
using System;
using Temporal.Content.Tiles.TemporalTower;
using Temporal.Content.Walls;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Temporal.Common.System.Generation
{
    internal class TemporalTowerGenPass : GenPass
    {
        internal int towerX = 0;
        internal int towerY = 0;

        internal int towerRoomWidth = 0;
        internal int towerRoomHeight = 0;

        internal int towerRoomMinX = 0;
        internal int towerRoomMinY = 0;
        internal int towerRoomMaxX = 0;
        internal int towerRoomMaxY = 0;

        internal int towerLastX = 0;
        internal int towerLastY = 0;

        private int towerRoomNum = 0;
        private int[] towerRoomX = new int[10];
        private int[] towerRoomY = new int[10];

        private int towerRoofID = -1;
        private int towerBasementID = -1;

        private int towerBasementHeight = 0;
        
        public TemporalTowerGenPass() : base("Temporal Tower", 1000f) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Erecting the Tower";

            ushort tileType = (ushort)ModContent.TileType<TemporalBrick>();
            ushort wallType = (ushort)ModContent.WallType<TemporalBrickWall>();

            Vector2 zero = Vector2.Zero;
            Point pos = CreateSpawnPoint();
            towerX = pos.X;
            towerY = pos.Y;
            // Get the Size of the Rooms
            towerRoomMinX = (int)Math.Floor(towerX - 12 * 0.8 - 5.0);
            towerRoomMinY = (int)Math.Ceiling(towerX + 12 * 0.8 + 5.0);
            towerRoomMaxX = (int)Math.Floor(towerY - 12 * 0.8 - 5.0);
            towerRoomMaxY = (int)Math.Ceiling(towerY + 12 * 0.8 + 5.0);
            // Get the Width and Height of Each Room
            towerRoomWidth = towerRoomMinY - towerRoomMinY;
            towerRoomHeight = towerRoomMaxY - towerRoomMaxY;

            progress.Set(0.1f);




            MakeTowerEntrance(towerX, towerY, tileType, wallType);

            int towerSegments = 1;

            int size = (int)((Main.maxTilesY / 1200) * 2f) - 1;
            switch(size)
            {
                case 1: towerSegments += WorldGen.genRand.Next(1, 2); break;
                case 2: towerSegments += WorldGen.genRand.Next(1, 2); break;
                case 3: towerSegments += WorldGen.genRand.Next(2, 4); break;
                default: break;
            }

            
            for (int i = 0; i < towerSegments; i++)
            {
                int x2 = towerX;
                int y2 = towerY - (towerRoomHeight * (i + 1));
                MakeTowerRoom(x2, y2, tileType, wallType);
            }
            progress.Set(0.2f);
            int roofPosition = towerY - (towerRoomHeight * (towerSegments + 1));

            int x3 = towerX;
            int y3 = roofPosition;
            MakeTowerChamber(x3, y3, tileType, wallType, ModContent.TileType<TemporalRoof>());
            progress.Set(0.3f);

            int x4 = towerX;
            int y4 = towerY + (int)(towerRoomHeight * 1.5f);
            MakeTowerBasement(x4, y4, tileType, wallType);
            progress.Set(0.4f);

            MakeRoomConnectors(tileType, wallType);
            progress.Set(0.5f);
            MakeTowerDungeon(tileType, wallType);
            progress.Set(0.6f);
            MakeBasementDungeon(tileType, wallType, WorldGen.genRand.Next(4));
        }

        internal Point CreateSpawnPoint()
        {
            int x = 0;
            int y = 0;
            int attempts = 0;
            bool valid = false;
            while(!valid)
            {
                int side = 1;
                if (WorldGen.genRand.NextBool(2))
                {
                    side = -side;
                }

                if(side == -1)
                {
                    x = WorldGen.genRand.Next((int)((Main.maxTilesX / 2) * 0.32f), (int)((Main.maxTilesX / 2) * 0.41f));
                } else
                {
                    x = WorldGen.genRand.Next((int)((Main.maxTilesX / 2) * 0.59f), (int)((Main.maxTilesX / 2) * 0.68f));
                }

                // Cast Down from the Sky
                for(y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if(tile.HasTile)
                    {
                        // Abort if too high
                        if (y < 175) break;

                        // Check if near dungeon
                        if(Vector2.Distance(new Vector2(Main.dungeonX, Main.dungeonY), new Vector2(x, y)) < 200) {
                            break;
                        }

                        // Check for Bad Tiles
                        ushort[] badTiles = new ushort[]
                        {
                            TileID.BlueDungeonBrick,
                            TileID.PinkDungeonBrick,
                            TileID.GreenDungeonBrick,
                            TileID.Sandstone,
                            TileID.SandstoneBrick,
                            TileID.CrimsonGrass,
                            TileID.CorruptGrass,
                            TileID.Crimsand,
                            TileID.Ebonsand,
                            TileID.Crimstone,
                            TileID.Ebonstone,
                            TileID.Cloud,
                            TileID.RainCloud,
                            TileID.SnowCloud
                        };

                        for(int i = 0; i < badTiles.Length; i++)
                        {
                            if(tile.TileType == badTiles[i])
                            {
                                break;
                            }
                        }

                        // Check Surrounding
                        int x1 = x - 20;
                        int x2 = x + 20;
                        int y1 = y - 20;
                        int y2 = y + 20;
                        bool isGood = true;
                        for(int i = x1; i < x2; i++)
                        {
                            for(int j = y2; j < y1; j++)
                            {
                                tile = Main.tile[i, j];
                                for(int k = 0; k < badTiles.Length; k++)
                                {
                                    if(tile.TileType == badTiles[i])
                                    {
                                        isGood = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if(!isGood)
                        {
                            break;
                        }

                        valid = true;
                        break;
                    }
                }

                attempts++;
                if(attempts > 10000)
                {
                    valid = true;
                }


            }
            return new Point(x, y - 10);
        }

        internal void MakeTowerEntrance(int x, int y, int tileType, int wallType)
        {

            Vector2 vector = default;
            vector.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            vector.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            Vector2 vector2 = default;
            vector2.X = x;
            vector2.Y = y;

            double num1 = vector2.X;
            double num2 = vector2.X;
            double num3 = vector2.Y;
            double num4 = vector2.Y;

            // Get Room Width and Height
            int x1 = (int)Math.Floor(vector2.X - 12 * 0.8 - 5.0);
            int x2 = (int)Math.Ceiling(vector2.X + 12 * 0.8 + 5.0);
            int y1 = (int)Math.Floor(vector2.Y - 12 * 0.8 - 5.0);
            int y2 = (int)Math.Ceiling(vector2.Y + 12 * 0.8 + 5.0);

            // Ensure within bounds
            if (x1 < 0) x1 = 0;
            if (x2 > Main.maxTilesX) x2 = Main.maxTilesX;
            if (y1 < 0) y1 = 0;
            if( y2 > Main.maxTilesY) y2 = Main.maxTilesY;

            // Store for Later
            int sx1 = x1;
            int sx2 = x2;
            int sy1 = y1;
            int sy2 = y2;

            Tile tile;
            // Build the "Block"
            for(int i = x1; i < x2; i++)
            {
                for(int j = y1; j < y2; j++)
                {
                    tile = Main.tile[i, j];
                    tile.ClearEverything();
                    WorldGen.PlaceTile(i, j, tileType, mute: true, forced: true);
                }
            }
            // Carve Out Room
            x1 = (int)Math.Floor(vector2.X - 12 * 0.8 - 2.0);
            x2 = (int)Math.Ceiling(vector2.X + 12 * 0.8 + 2.0);
            y1 = (int)Math.Floor(vector2.Y - 12 * 0.8 - 2.0);
            y2 = (int)Math.Ceiling(vector2.Y + 12 * 0.8 + 2.0);

            // Again Safety
            if (x1 < 0) x1 = 0;
            if (x2 > Main.maxTilesX) x2 = Main.maxTilesX;
            if (y1 < 0) y1 = 0;
            if (y2 > Main.maxTilesY) y2 = Main.maxTilesY;

            for(int i = x1; i < x2; i++)
            {
                for(int j = y1; j < y2; j++)
                {
                    tile = Main.tile[i, j];
                    tile.ClearEverything();
                    WorldGen.PlaceWall(i, j, wallType, true);
                }
            }


            towerRoomWidth = sx2 - sx1;
            towerRoomHeight = sy2 - sy1;
            towerRoomMinX = sx1;
            towerRoomMaxX = sx2;
            towerRoomMinY = sy1;
            towerRoomMaxY = sy2;

            towerRoomX[towerRoomNum] = x;
            towerRoomY[towerRoomNum] = y;
            towerRoomNum++;
        }

        internal void MakeTowerRoom(int x, int y, int tileType, int wallType)
        {
            Vector2 vector2 = default;
            vector2.X = x;
            vector2.Y = y;

            // Get Room Width and Height
            int x1 = (int)Math.Floor(vector2.X - 12 * 0.8 - 5.0);
            int x2 = (int)Math.Ceiling(vector2.X + 12 * 0.8 + 5.0);
            int y1 = (int)Math.Floor(vector2.Y - 12 * 0.8 - 5.0);
            int y2 = (int)Math.Ceiling(vector2.Y + 12 * 0.8 + 5.0);

            Tile tile;
            // Build the "Block"
            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    tile = Main.tile[i, j];
                    tile.ClearEverything();
                    WorldGen.PlaceTile(i, j, tileType, mute: true, forced: true);
                }
            }

            // Carve Out Room
            x1 = (int)Math.Floor(vector2.X - 12 * 0.8 - 2.0);
            x2 = (int)Math.Ceiling(vector2.X + 12 * 0.8 + 2.0);
            y1 = (int)Math.Floor(vector2.Y - 12 * 0.8 - 2.0);
            y2 = (int)Math.Ceiling(vector2.Y + 12 * 0.8 + 2.0);

            // Again Safety
            if (x1 < 0) x1 = 0;
            if (x2 > Main.maxTilesX) x2 = Main.maxTilesX;
            if (y1 < 0) y1 = 0;
            if (y2 > Main.maxTilesY) y2 = Main.maxTilesY;

            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    tile = Main.tile[i, j];
                    tile.ClearEverything();
                    WorldGen.PlaceWall(i, j, wallType, true);
                }
            }

            towerRoomX[towerRoomNum] = x;
            towerRoomY[towerRoomNum] = y;
            towerRoomNum++;
        }

        internal void MakeTowerChamber(int x, int y, int tileType, int wallType, int roofType)
        {
            Vector2 vector = default;
            vector.X = x;
            vector.Y = y;


            // Get Room Width and Height
            int x1 = (int)Math.Floor(vector.X - 12 * 0.8 - 5.0);
            int x2 = (int)Math.Ceiling(vector.X + 12 * 0.8 + 5.0);
            int y1 = (int)Math.Floor(vector.Y - 12 * 0.8 - 5.0);
            int y2 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 5.0);

            int y3 = y;
            int y4 = y2;

            Tile tile;
            for(int i = x1; i < x2; i++)
            {
                for(int j = y3; j < y4; j++)
                {
                    tile = Main.tile[i, j];
                    tile.ClearEverything();
                    WorldGen.PlaceTile(i, j, tileType, mute: true, forced: true);
                }
            }

            // Carve Out the Room
            x1 = (int)Math.Floor(vector.X - 12 * 0.8 - 2.0);
            x2 = (int)Math.Ceiling(vector.X + 12 * 0.8 + 2.0);
            y1 = (int)Math.Floor(vector.Y - 12 * 0.8 - 2.0);
            y2 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 2.0);

            y3 = y;
            y4 = y2;

            for(int i = x1; i < x2; i++)
            {
                for(int j = y3; j < y4; j++)
                {
                    tile = Main.tile[i, j];
                    tile.Clear(TileDataType.Tile);
                    WorldGen.PlaceWall(i, j, wallType, mute: true);
                }
            }


            // Add Roof
            x1 = (int)Math.Floor(vector.X - 12 * 0.8 - 5.0);
            x2 = (int)Math.Ceiling(vector.X + 12 * 0.8 + 5.0);
            y1 = (int)Math.Floor(vector.Y - 12 * 0.8 - 5.0);
            y2 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 5.0);

            int x3 = x1 - 8;
            int x4 = x2 + 8;
            y3 = y1 - 32;
            y4 = y;

            if (x3 < 0) x3 = 0;
            if (x4 > Main.maxTilesX) x4 = Main.maxTilesX;
            if (y3 < 0) y3 = 0;
            if(y4 > Main.maxTilesY) y4 = Main.maxTilesY;


            int oldIncrement = 0;
            int timesSame = 0;
            int timesSameMax = 3;
            int distanceSmall = 2;
            int distanceSmallSame = 0;
            int distanceSmallSameMax = 8;
            for (int j = y4; j >= y3; j--)
            {
                for (int i = x3; i < x4; i++)
                {
                    tile = Main.tile[i, j];
                    if(tile.TileType == tileType || tile.WallType == wallType)
                    {
                        continue;
                    }
                    WorldGen.PlaceTile(i, j, roofType, mute: true, forced: true);
                }

                int distanceApart = x4 - x3;

                float scale = (distanceApart * 0.93f);
                int increment = (int)Math.Max(Math.Round(distanceApart - scale), 0);
                oldIncrement = increment;
                if (increment == 0)
                {
                    if (oldIncrement == increment)
                    {
                        timesSame++;
                        if (timesSame == timesSameMax)
                        {
                            increment = 1;
                            timesSame = 0;
                            timesSameMax *= 2;

                        }
                        if (distanceApart == distanceSmall)
                        {
                            distanceSmallSame++;
                            if (distanceSmallSameMax == distanceSmallSame)
                            {
                                break;
                            }
                        }
                    }
                }

                x3 += increment;
                x4 -= increment;
            }

            // Clear Roof Tiles
            x1 = (int)Math.Floor(vector.X - 12 * 0.8 - 5.0);
            x2 = (int)Math.Ceiling(vector.X + 12 * 0.8 + 5.0);
            y1 = (int)Math.Floor(vector.Y - 12 * 0.8 - 5.0);
            y2 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 5.0);

            x3 = x1 + 3;
            x4 = x2 - 3;
            y3 = y1 - 12;
            y4 = y;

            if (x3 < 0) x3 = 0;
            if (x4 > Main.maxTilesX) x4 = Main.maxTilesX;
            if (y3 < 0) y3 = 0;
            if (y4 > Main.maxTilesY) y4 = Main.maxTilesY;


            for (int j = y4; j >= y3; j--)
            {
                for (int i = x3; i < x4; i++)
                {
                    tile = Main.tile[i, j];
                    if (tile.TileType == roofType)
                    {
                        tile.Clear(TileDataType.Tile);
                        WorldGen.PlaceWall(i, j, wallType);

                        Tile tile2 = Main.tile[i + 1, j];
                        Tile tile3 = Main.tile[i - 1, j];
                        Tile tile4 = Main.tile[i, j + 1];
                        Tile tile5 = Main.tile[i, j - 1];

                        if (tile2.HasTile) WorldGen.PlaceWall(i + 1, j, wallType);
                        if (tile3.HasTile) WorldGen.PlaceWall(i - 1, j, wallType);
                        if (tile4.HasTile) WorldGen.PlaceWall(i, j + 1, wallType);
                        if (tile5.HasTile) WorldGen.PlaceWall(i, j - 1, wallType);
                    }
                }

                int distanceApart = x4 - x3;

                float scale = (distanceApart * 0.9f);
                int increment = (int)Math.Max(Math.Round(distanceApart - scale), 0);
                if (increment == 0)
                {
                    tile = Main.tile[x3 + 1, j - 1];
                    tile.Clear(TileDataType.Tile);
                    WorldGen.PlaceWall(x3 + 1, j - 1, wallType);
                    tile = Main.tile[x4 - 2, j - 1];
                    tile.Clear(TileDataType.Tile);
                    WorldGen.PlaceWall(x4 - 2, j - 1, wallType);
                    tile = Main.tile[x3 + 1, j - 2];
                    tile.Clear(TileDataType.Tile);
                    WorldGen.PlaceWall(x3 + 1, j - 2, wallType);
                    tile = Main.tile[x4 - 2, j - 2];
                    tile.Clear(TileDataType.Tile);
                    WorldGen.PlaceWall(x4 - 2, j - 2, wallType);
                    tile = Main.tile[x3 + 1, j - 3];
                    tile.Clear(TileDataType.Tile);
                    WorldGen.PlaceWall(x3 + 1, j - 3, wallType);
                    tile = Main.tile[x4 - 2, j - 3];
                    tile.Clear(TileDataType.Tile);
                    WorldGen.PlaceWall(x4 - 2, j - 3, wallType);
                    break;
                }

                x3 += increment;
                x4 -= increment;
            }


            // Roof Slopes
            x1 = (int)Math.Floor(vector.X - 12 * 0.8 - 5.0);
            x2 = (int)Math.Ceiling(vector.X + 12 * 0.8 + 5.0);
            y1 = (int)Math.Floor(vector.Y - 12 * 0.8 - 5.0);
            y2 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 5.0);

            x3 = x1 - 8;
            x4 = x2 + 8;
            y3 = y1 - 32;
            y4 = y;

            if (x1 < 0) x1 = 0;
            if (x2 > Main.maxTilesX) x2 = Main.maxTilesX;
            if (y3 < 0) y3 = 0;
            if (y4 > Main.maxTilesY) y4 = Main.maxTilesY;

            for(int i = x3; i < x4; i++)
            {
                for(int j = y3; j < y4; j++)
                {
                    tile = Main.tile[i, j];
                    if(tile.TileType == roofType)
                    {
                        Tile tile2 = Main.tile[i, j - 1];
                        Tile tile3 = Main.tile[i, j + 1];
                        Tile tile4 = Main.tile[i - 1, j];
                        Tile tile5 = Main.tile[i + 1, j];

                        if (!tile2.HasTile && tile3.HasTile && !tile4.HasTile && tile5.HasTile)
                        {
                            tile.Slope = SlopeType.SlopeDownRight;
                        }
                        else if(!tile2.HasTile && tile3.HasTile && tile4.HasTile && !tile5.HasTile)
                        {
                            tile.Slope = SlopeType.SlopeDownLeft;
                        }
                        else if (tile2.HasTile && !tile3.HasTile && !tile4.HasTile && tile5.HasTile)
                        {
                            tile.Slope = SlopeType.SlopeUpRight;
                        }
                        else if (tile2.HasTile && !tile3.HasTile && tile4.HasTile && !tile5.HasTile)
                        {
                            tile.Slope = SlopeType.SlopeUpLeft;
                        }

                    }
                }
            }


            towerRoomX[towerRoomNum] = x;
            towerRoomY[towerRoomNum] = y;
            towerRoofID = towerRoomNum;
            towerRoomNum++;

        }

        internal void MakeTowerBasement(int x, int y, int tileType, int wallType)
        {
            Vector2 vector = new(x, y);
            // Get Room Width and Height
            int x1 = (int)Math.Floor(vector.X - 24 * 0.8 - 5.0);
            int x2 = (int)Math.Ceiling(vector.X + 24 * 0.8 + 5.0);
            int y1 = (int)Math.Floor(vector.Y - 20 * 0.8 - 5.0);
            int y2 = (int)Math.Ceiling(vector.Y + 20 * 0.8 + 5.0);

            int height = y2 - y1;

            Tile tile;
            // Build the "Block"
            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    tile = Main.tile[i, j];
                    tile.ClearEverything();
                    WorldGen.PlaceTile(i, j, tileType, mute: true, forced: true);
                }
            }

            x1 = (int)Math.Floor(vector.X - 24 * 0.8 - 2.0);
            x2 = (int)Math.Ceiling(vector.X + 24 * 0.8 + 2.0);
            y1 = (int)Math.Floor(vector.Y - 20 * 0.8 - 2.0);
            y2 = (int)Math.Ceiling(vector.Y + 20 * 0.8 + 2.0);

            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    tile = Main.tile[i, j];
                    tile.ClearEverything();
                    WorldGen.PlaceWall(i, j, wallType);
                }
            }

            towerBasementHeight = height;

            towerRoomX[towerRoomNum] = x;
            towerRoomY[towerRoomNum] = y;
            towerBasementID = towerRoomNum;
            towerRoomNum++;
        }

        internal void MakeRoomConnectors(int tileType, int wallType)
        {
            for(int i = 0; i < towerRoomNum; i++)
            {
                int x = towerRoomX[i];
                int y = towerRoomY[i];

                Vector2 vector = default;
                vector.X = x;
                vector.Y = y;


                // Carve Out Room
                int x1 = (int)Math.Floor(vector.X - 12 * 0.8 - 2.0);
                int x2 = (int)Math.Ceiling(vector.X + 12 * 0.8 + 2.0);
                int y1 = (int)Math.Floor(vector.Y - 12 * 0.8 - 5.0);
                int y2 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 5.0);

                // Again Safety
                if (x1 < 0) x1 = 0;
                if (x2 > Main.maxTilesX) x2 = Main.maxTilesX;
                if (y1 < 0) y1 = 0;
                if (y2 > Main.maxTilesY) y2 = Main.maxTilesY;

                if(i == 0)
                {
                    y2 -= 3;
                }
                if(i == towerRoofID)
                {
                    y1 = (int)vector.Y;
                }

                Tile tile;

                for(int j = x1; j < x2; j++)
                {
                    for(int k = y1; k < y2; k++)
                    {
                        tile = Main.tile[j, k];
                        if(tile.HasTile)
                        {
                            tile.Clear(TileDataType.Tile);
                            WorldGen.PlaceWall(j, k, wallType);
                        }
                    }
                }

                if(i == 0)
                {
                    MakeEntranceOpenings(x, y, tileType, wallType);

                    // Carve Out Passage to Basement
                    int x3 = x - 2;
                    int x4 = x + 2;
                    int y3 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 5.0) - 3;
                    int y4 = (towerRoomY[towerBasementID] - (towerBasementHeight / 2)) + 3;
                    for(int j = x3; j < x4; j++)
                    {
                        for(int k = y3; k < y4; k++)
                        {
                            tile = Main.tile[j, k];
                            if(tile.HasTile && tile.TileType == tileType)
                            {
                                tile.Clear(TileDataType.Tile);
                                WorldGen.PlaceWall(j, k, wallType);
                            }
                        }
                    }
                    // Place Platforms
                    for(int j = x3; j < x4; j++)
                    {
                        WorldGen.PlaceTile(j, y3, TileID.Platforms);
                        WorldGen.PlaceTile(j, y4 - 1, TileID.Platforms);
                    }
                }
            }
        }

        internal void MakeEntranceOpenings(int x, int y, int tileType, int wallType)
        {
            // Make Door Way
            Vector2 vector = new(x, y);
            int x1 = (int)Math.Floor(vector.X - 12 * 0.8 - 5.0);
            int x2 = (int)Math.Ceiling(vector.X + 12 * 0.8 + 5.0);
            int y1 = (int)Math.Floor(vector.Y - 12 * 0.8 - 5.0);
            int y2 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 5.0);

            int x3 = x1;
            int x4 = x2;
            int y3 = y2 - 6;
            int y4 = y2 - 3;

            Tile tile;
            for(int i = x3; i < x4; i++)
            {
                for(int j = y3; j < y4; j++)
                {
                    tile = Main.tile[i, j];
                    if(tile.TileType == tileType)
                    {
                        tile.Clear(TileDataType.Tile);

                        if(i != x3 && i != x3 + 1 && i != x4 - 2 && i != x4 - 1)
                        {
                            WorldGen.PlaceWall(i, j, wallType);
                        }
                    }
                }
            }

            // Create Base
            int x5 = x1;
            int x6 = x2;
            int y5 = y2 - 3;
            int y6 = y2 + 10 + towerBasementHeight;
            for (int j = y5; j < y6; j++)
            {
                for (int i = x5; i < x6; i++)
                {
                    tile = Main.tile[i, j];
                    if (tile.TileType != tileType && tile.WallType != wallType)
                    {
                        tile.ClearEverything();
                        WorldGen.PlaceTile(i, j, tileType);
                    }
                }
                if (j < towerRoomY[towerBasementID] - (towerBasementHeight / 2)) { 
                    x5--;
                    x6++;
                }
            }
        }

        internal void MakeTowerDungeon(int tileType, int wallType)
        {
            int x1 = towerRoomX[0];
            int y1 = towerRoomY[0];
            int x2 = towerRoomX[towerRoofID];
            int y2 = towerRoomY[towerRoofID];

            Vector2 vector = new(x1, y1);
            Vector2 vector2 = new(x2, y2);

            Tile tile;
            // Get Tower Bounds
            int x3 = (int)Math.Floor(vector.X - 12 * 0.8 - 2.0);
            int x4 = (int)Math.Ceiling(vector.X + 12 * 0.8 + 2.0);
            int y3 = (int)vector2.Y;
            int y4 = (int)Math.Ceiling(vector.Y + 12 * 0.8 + 2.0);

            // Make the Pillar
            int x5 = x1 - 2;
            int x6 = x1 + 2;
            int y5 = y3 + 12;
            int y6 = y4 - 6;

            for(int i = x5; i < x6; i++)
            {
                for(int j = y5; j < y6; j++)
                {
                    WorldGen.PlaceTile(i, j, tileType);
                }
            }

            // Top Floor Platform
            for (int i = x3; i < x4; i++)
            {
                tile = Main.tile[i, y5 + 1];
                if (tile.WallType == wallType && !tile.HasTile)
                {
                    WorldGen.PlaceTile(i, y5 + 1, TileID.Platforms);
                }
            }

            // Generate Floors
            int numFloors = (y6 - y5) / 8;
            int floorHeight = 8;

            for(int i = 0; i < numFloors; i++)
            {
                int x7 = x3;
                int x8 = x5;
                int x9 = x6;
                int x10 = x4;

                int y7 = (y6 - 1) - (floorHeight * i);

                int platformLength = WorldGen.genRand.Next(3, 5);
                int platformPosition = WorldGen.genRand.Next(x7 + 1, x8 - (platformLength + 1));
                // Left Side
                for (int j = x7; j < x8; j++)
                {
                    tile = Main.tile[j, y7];
                    if (tile.HasTile) continue;
                    if(j >= platformPosition && j <= platformPosition + platformLength)
                    {
                        WorldGen.PlaceTile(j, y7, TileID.Platforms);
                    } else
                    {
                        WorldGen.PlaceTile(j, y7, tileType);
                    }
                }

                platformLength = WorldGen.genRand.Next(3, 6);
                platformPosition = WorldGen.genRand.Next(x9 + 1, x10 - (platformLength + 1));
                // Right Side
                for (int j = x9; j < x10; j++)
                {
                    tile = Main.tile[j, y7];
                    if (tile.HasTile) continue;
                    if (j >= platformPosition && j <= platformPosition + platformLength)
                    {
                        WorldGen.PlaceTile(j, y7, TileID.Platforms);
                    }
                    else
                    {
                        WorldGen.PlaceTile(j, y7, tileType);
                    }
                }
            }
        }
    
        internal void MakeBasementDungeon(int tileType, int wallType, int roomType)
        {
            Vector2 vector = new(towerRoomX[towerBasementID], towerRoomY[towerBasementID]);

            // Get Room Width and Height
            int x1 = (int)Math.Floor(vector.X - 24 * 0.8 - 2.0);
            int x2 = (int)Math.Ceiling(vector.X + 24 * 0.8 + 2.0);
            int y1 = (int)Math.Floor(vector.Y - 20 * 0.8 - 2.0);
            int y2 = (int)Math.Ceiling(vector.Y + 20 * 0.8 + 2.0);

            switch(roomType)
            {
                case 0: MakeBasementDungeon_RoomTypeZero(x1, x2, y1, y2, tileType, wallType); break;


                default: MakeBasementDungeon_RoomTypeZero(x1, x2, y1, y2, tileType, wallType); break;
            }

        }

        internal void MakeBasementDungeon_RoomTypeZero(int x1, int x2, int y1, int y2, int tileType, int wallType)
        {
            Tile tile;
            
            // Create Top Platform

            int x5 = 20;
            int y5 = y1 + 5;
            for(int i = x1 + x5; i < x2 - x5; i++)
            {
                WorldGen.PlaceTile(i, y5, tileType, forced: true);
            }
            y5++;
            x5--;
            for (int i = x1 + x5; i < x2 - x5; i++)
            {
                WorldGen.PlaceTile(i, y5, tileType, forced: true);
            }
            y5++;
            x5 -= 3;
            for (int i = x1 + x5; i < x2 - x5; i++)
            {
                WorldGen.PlaceTile(i, y5, tileType, forced: true);
            }
            y5++;
            for (int i = x1 + x5; i < x2 - x5; i++)
            {
                WorldGen.PlaceTile(i, y5, tileType, forced: true);
            }
            y5++;
            for (int i = x1 + x5; i < x2 - x5; i++)
            {
                if(!(i >= x1 + x5 + 5 && i < x2 - x5 - 5))
                    WorldGen.PlaceTile(i, y5, tileType, forced: true);
            }
        }
    }
}
