using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelGeneration
{
    public class LevelGenerator
    {
        private const int CountWall = 4;

        private Tilemap _tileMap;
        private LevelGenerationConfig _levelConfig;
        private TilePalette _currentPalette;

        private int[,] _map;
        private MarchingSquaresLevelGenerator _marchingSquaresGeneratorLevel = new MarchingSquaresLevelGenerator();


        public LevelGenerator(LevelGenerationConfig levelGenerationConfig)
        {
            _levelConfig = levelGenerationConfig;

            var grid = new GameObject("GeneratedGrid").AddComponent<Grid>();
            var tileMap = new GameObject("GeneratedTileMap").AddComponent<Tilemap>();
            tileMap.gameObject.AddComponent<TilemapRenderer>();
            tileMap.transform.parent = grid.transform;
            _tileMap = tileMap;

            _map = new int[_levelConfig.MapWidth, _levelConfig.MapHeight];

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            RandomFillLevel();

            for (var i = 0; i < _levelConfig.SmoothFactor; i++)
                SmoothMap();

            AdjustEdgeTiles();

            DrawTilesOnMap();

            //_marchingSquaresGeneratorLevel.GenerateGrid(_map, 1);
            //_marchingSquaresGeneratorLevel.DrawTilesOnMap(_tileMap, _levelConfig.LowerLevelPalette.MainTile);

        }

        private void RandomFillLevel()
        {
            var seed = Time.time.ToString();
            var pseudoRandom = new System.Random(seed.GetHashCode());

            for (var x = 0; x < _levelConfig.MapWidth; x++)
            {
                for (var y = 0; y < _levelConfig.MapHeight; y++)
                {
                    if (x == 0 || x == _levelConfig.MapWidth - 1 || y == 0 || y == _levelConfig.MapHeight - 1)
                        _map[x, y] = 1;
                    else
                        _map[x, y] = (pseudoRandom.Next(0, 100) < _levelConfig.RandomFillPercent) ? 1 : 0;
                }
            }
        }

        private void SmoothMap()
        {
            for (var x = 0; x < _levelConfig.MapWidth; x++)
            {
                for (var y = 0; y < _levelConfig.MapHeight; y++)
                {
                    var neighbourWallTiles = GetSurroundingWallCount(x, y);

                    if (neighbourWallTiles > CountWall)
                        _map[x, y] = 1;
                    else if (neighbourWallTiles < CountWall)
                        _map[x, y] = 0;
                }
            }
        }

        private int GetSurroundingWallCount(int gridX, int gridY)
        {
            var wallCount = 0;

            for (var neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (var neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < _levelConfig.MapWidth && neighbourY >= 0 && neighbourY < _levelConfig.MapHeight)
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                            wallCount += _map[neighbourX, neighbourY];
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }

            return wallCount;
        }

        private void DrawTilesOnMap()
        {
            if (_map == null)
                return;

            for (var x = 0; x < _levelConfig.MapWidth; x++)
            {
                for (var y = 0; y < _levelConfig.MapHeight; y++)
                {
                    if (y < _levelConfig.MapHeight / 2)
                        _currentPalette = _levelConfig.LowerLevelPalette;
                    else
                        _currentPalette = _levelConfig.UpperLevelPalette;

                    var positionTile = new Vector3Int(-_levelConfig.MapWidth / 2 + x, -_levelConfig.MapHeight / 2 + y, 0);

                    if (_map[x, y] == 1)
                        _tileMap.SetTile(positionTile, _currentPalette.MainTile);
                    if (_map[x, y] == 2)
                        _tileMap.SetTile(positionTile, _currentPalette.BottomTile);
                    if (_map[x, y] == 3)
                        _tileMap.SetTile(positionTile, _currentPalette.TopTile);
                    if (_map[x, y] == 4)
                        _tileMap.SetTile(positionTile, _currentPalette.RightTile);
                    if (_map[x, y] == 5)
                        _tileMap.SetTile(positionTile, _currentPalette.LeftTile);
                    if (_map[x, y] == 6)
                        _tileMap.SetTile(positionTile, _currentPalette.BottomRightTile);
                    if (_map[x, y] == 7)
                        _tileMap.SetTile(positionTile, _currentPalette.BottomLeftTile);
                    if (_map[x, y] == 8)
                        _tileMap.SetTile(positionTile, _currentPalette.TopRightTile);
                    if (_map[x, y] == 9)
                        _tileMap.SetTile(positionTile, _currentPalette.TopLeftTile);
                    if (_map[x, y] == 10)
                        _tileMap.SetTile(positionTile, _currentPalette.TopEndTile);
                    if (_map[x, y] == 11)
                        _tileMap.SetTile(positionTile, _currentPalette.BottomEndTile);
                    if (_map[x, y] == 12)
                        _tileMap.SetTile(positionTile, _currentPalette.RightEndTile);
                    if (_map[x, y] == 13)
                        _tileMap.SetTile(positionTile, _currentPalette.LeftEndTile);
                    if (_map[x, y] == 14)
                        _tileMap.SetTile(positionTile, _currentPalette.HorizontalTile);
                    if (_map[x, y] == 15)
                        _tileMap.SetTile(positionTile, _currentPalette.VerticalTile);
                    if (_map[x, y] == 16)
                        _tileMap.SetTile(positionTile, _currentPalette.StandAloneTile);
                }
            }
        }

        private void AdjustEdgeTiles()
        {
            for (var x = 0; x < _levelConfig.MapWidth; x++)
            {
                for (var y = 0; y < _levelConfig.MapHeight; y++)
                {
                    if (_map[x, y] != 0)
                        _map[x, y] = GetNewTileValue(x, y);
                }
            }
        }

        private int GetNewTileValue(int gridX, int gridY)
        {
            bool leftNeighbour = (gridX == 0) || (_map[gridX - 1, gridY] != 0);

            bool rightNeighbour = (gridX == _levelConfig.MapWidth -1 ) || (_map[gridX + 1, gridY] != 0);

            bool bottomNeighbour = (gridY == 0) || (_map[gridX, gridY - 1] != 0);

            bool topNeighbour = (gridY == _levelConfig.MapHeight - 1) || (_map[gridX, gridY + 1] != 0);


            if (topNeighbour == true && bottomNeighbour == false && leftNeighbour == true && rightNeighbour == true)
                return 2;
            if (topNeighbour == false && bottomNeighbour == true && leftNeighbour == true && rightNeighbour == true)
                return 3;
            if (topNeighbour == true && bottomNeighbour == true && leftNeighbour == true && rightNeighbour == false)
                return 4;
            if (topNeighbour == true && bottomNeighbour == true && leftNeighbour == false && rightNeighbour == true)
                return 5;
            if (topNeighbour == true && bottomNeighbour == false && leftNeighbour == true && rightNeighbour == false)
                return 6;
            if (topNeighbour == true && bottomNeighbour == false && leftNeighbour == false && rightNeighbour == true)
                return 7;
            if (topNeighbour == false && bottomNeighbour == true && leftNeighbour == true && rightNeighbour == false)
                return 8;
            if (topNeighbour == false && bottomNeighbour == true && leftNeighbour == false && rightNeighbour == true)
                return 9;
            if (topNeighbour == false && bottomNeighbour == true && leftNeighbour == false && rightNeighbour == false)
                return 10;
            if (topNeighbour == true && bottomNeighbour == false && leftNeighbour == false && rightNeighbour == false)
                return 11;
            if (topNeighbour == false && bottomNeighbour == false && leftNeighbour == true && rightNeighbour == false)
                return 12;
            if (topNeighbour == false && bottomNeighbour == false && leftNeighbour == false && rightNeighbour == true)
                return 13;
            if (topNeighbour == false && bottomNeighbour == false && leftNeighbour == true && rightNeighbour == true)
                return 14;
            if (topNeighbour == true && bottomNeighbour == true && leftNeighbour == false && rightNeighbour == false)
                return 15;
            if (topNeighbour == false && bottomNeighbour == false && leftNeighbour == false && rightNeighbour == false)
                return 16;
            return _map[gridX, gridY];
        }
    }
}
