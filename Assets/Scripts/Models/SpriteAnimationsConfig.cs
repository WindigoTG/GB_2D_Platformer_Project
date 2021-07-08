using System;
using System.Collections.Generic;
using UnityEngine;

public enum Track
{
    Idle = 0,
    Run = 1,
    Jump = 2,
    Fall = 3,
    Inactive = 4,
    PopUp = 5,
    Hide = 6
}

[CreateAssetMenu(fileName = "SpriteAnimationsConfig", menuName = "Configs/SpriteAnimationsConfig", order = 1)]
public class SpriteAnimationsConfig : ScriptableObject
{
    [Serializable]
    public class SpritesSequence
    {
        public Track Track;
        public List<Sprite> Sprites = new List<Sprite>();
    }
    public List<SpritesSequence> Sequences = new List<SpritesSequence>();
}
