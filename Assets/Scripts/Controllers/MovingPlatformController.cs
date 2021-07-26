using UnityEngine;

public class MovingPlatformController : IUpdateableFixed
{
    MovingPlatformView[] _platforms;

    public MovingPlatformController(SpriteAnimatorController animatorController)
    {
        var platforms = Object.FindObjectsOfType<SliderJoint2D>();
        _platforms = new MovingPlatformView[platforms.Length];
        for (int i = 0; i < _platforms.Length; i++)
            _platforms[i] = new MovingPlatformView(platforms[i], animatorController);
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < _platforms.Length; i++)
            if (_platforms[i].HasReachedLimit)
                _platforms[i].Invert();
    }
}
