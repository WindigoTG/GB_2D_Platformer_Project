using UnityEngine;

public class CannonController : IUpdateable
{
    private CannonModel _cannon;

    public CannonController(PlayerPosition targetPosition)
    {
        _cannon = new CannonModel(targetPosition);
    }

    public void Update()
    {
        _cannon.Update();
    }
}
