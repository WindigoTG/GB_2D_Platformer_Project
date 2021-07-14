using UnityEngine;

public class CannonController : IUpdateable
{
    private CannonModel _cannon;

    public CannonController(Transform targetTransform)
    {
        _cannon = new CannonModel(targetTransform);
    }

    public void Update()
    {
        _cannon.Update();
    }
}
