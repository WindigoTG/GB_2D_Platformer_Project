using UnityEngine;

public class CannonController : IUpdateable
{
    private CannonModel[] _cannons;

    public CannonController(Transform[] cannonPositions,Transform targetTransform)
    {
        _cannons = new CannonModel[cannonPositions.Length];
        for (int i = 0; i < _cannons.Length; i++)
            _cannons[i] = new CannonModel(targetTransform, cannonPositions[i].position);
    }

    public void Update()
    {
        for (int i = 0; i < _cannons.Length; i++)
            _cannons[i].Update();
    }

    public void RegisterTarget(PlayerHealth playerHealth)
    {
        for (int i = 0; i < _cannons.Length; i++)
            _cannons[i].TargetHit += playerHealth.TakeDamage;
    }
}
