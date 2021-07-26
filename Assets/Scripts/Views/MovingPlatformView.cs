using UnityEngine;

public class MovingPlatformView
{
    Transform _platform;
    SliderJoint2D _sliderJoint;
    Vector2 _connectedAnchor;
    JointTranslationLimits2D _limits;

    public MovingPlatformView(SliderJoint2D joint, SpriteAnimatorController animationController)
    {
        _sliderJoint = joint;
        _platform = _sliderJoint.transform;
        _connectedAnchor = _sliderJoint.connectedAnchor;
        _limits = _sliderJoint.limits;
        animationController.StartAnimation(_sliderJoint.GetComponent<SpriteRenderer>(), Track.Idle, true, 10);
    }

    public bool HasReachedLimit => ((Vector2)_platform.position - _connectedAnchor).magnitude >= _limits.max;

    public void Invert()
    {
            var motor = _sliderJoint.motor;
            motor.motorSpeed *= -1;
            _sliderJoint.motor = motor;
    }
}
