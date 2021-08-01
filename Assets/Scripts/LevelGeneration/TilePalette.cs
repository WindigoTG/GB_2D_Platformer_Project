using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelGeneration
{
    [Serializable]
    public class TilePalette
    {
        [SerializeField] Tile _mainTile;
        [SerializeField] Tile _topTile;
        [SerializeField] Tile _bottomTile;
        [SerializeField] Tile _leftTile;
        [SerializeField] Tile _rightTile;
        [SerializeField] Tile _topLeftTile;
        [SerializeField] Tile _topRightTile;
        [SerializeField] Tile _bottomLeftTile;
        [SerializeField] Tile _bottomRightTile;
        [SerializeField] Tile _topEndTile;
        [SerializeField] Tile _bottomEndTile;
        [SerializeField] Tile _leftEndTile;
        [SerializeField] Tile _rightEndTile;
        [SerializeField] Tile _verticalTile;
        [SerializeField] Tile _horisontalTile;
        [SerializeField] Tile _standAloneTile;

        public Tile MainTile => _mainTile;

        public Tile TopTile => _topTile;

        public Tile BottomTile => _bottomTile;

        public Tile LeftTile => _leftTile;

        public Tile RightTile => _rightTile;

        public Tile TopLeftTile => _topLeftTile;

        public Tile TopRightTile => _topRightTile;

        public Tile BottomLeftTile => _bottomLeftTile;

        public Tile BottomRightTile => _bottomRightTile;

        public Tile TopEndTile => _topEndTile;

        public Tile BottomEndTile => _bottomEndTile;

        public Tile LeftEndTile => _leftEndTile;

        public Tile RightEndTile => _rightEndTile;

        public Tile HorizontalTile => _horisontalTile;

        public Tile VerticalTile => _verticalTile;

        public Tile StandAloneTile => _standAloneTile;
    }
}