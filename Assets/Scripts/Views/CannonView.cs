using UnityEngine;

public class CannonView
{
    Transform _barrel;
    Transform _cannon;
    Transform _muzzle;
    Quaternion _rotation;

    public CannonView(GameObject cannon)
    {
        _cannon = cannon.transform;
        _barrel = _cannon.gameObject.GetComponentInChildren<MeshFilter>().transform; //Ищем по маркерному компоненту
        _muzzle = _cannon.gameObject.GetComponentInChildren<Grid>().transform;
    }

    public Vector3 BarrelPosition => _barrel.position;
    public Vector3 MuzzlePosition => _muzzle.position;
    public Quaternion MuzzleRotation => _muzzle.rotation;

    public void SetPosition(Vector3 position)
    {
        _cannon.position = position;
    }

    public void RotateToward(Quaternion targetRotation, float rotationSpeed)
    {
        _rotation = Quaternion.RotateTowards(_barrel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        _barrel.rotation = _rotation;
    }
}
