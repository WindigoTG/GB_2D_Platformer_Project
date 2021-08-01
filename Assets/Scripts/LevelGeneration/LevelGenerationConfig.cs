using UnityEngine;

namespace LevelGeneration
{
    [CreateAssetMenu(fileName = "LevelGenerationConfig", menuName = "Configs/LevelGeneration", order = 3)]
    public class LevelGenerationConfig : ScriptableObject
    {
        [SerializeField] TilePalette _upperLevelpalette;

        [SerializeField] TilePalette _lowerLevelpalette;

        [SerializeField] int _mapWidth;

        [SerializeField] int _mapHeight;

        [SerializeField] int __smoothFactor;

        [SerializeField] [Range(0, 100)] int _randomFillPercent;

        public int MapWidth => _mapWidth;

        public int MapHeight => _mapHeight;

        public int SmoothFactor => __smoothFactor;

        public int RandomFillPercent => _randomFillPercent;

        public TilePalette UpperLevelPalette => _upperLevelpalette;

        public TilePalette LowerLevelPalette => _lowerLevelpalette;
    }
}

