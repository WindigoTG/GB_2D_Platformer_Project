using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterHandler : MonoBehaviour
{
    [SerializeField] Transform _startTeleporter;
    [SerializeField] List<TeleporterData> _teleporters;
    [SerializeField] Transform _finishTeleporter;

    private SpriteAnimatorController _animatorController;

    public event Action Finish;

    public static TeleporterHandler Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (_animatorController != null)
        {
            _animatorController.StartAnimation(_startTeleporter.GetComponent<SpriteRenderer>(), Track.Idle, true, 10f);
            _animatorController.StartAnimation(_finishTeleporter.GetComponent<SpriteRenderer>(), Track.Idle, true, 10f);
            for (int i = 0; i < _teleporters.Count; i++)
            {
                _animatorController.StartAnimation(_teleporters[i].FirstSpriteRenderer.GetComponent<SpriteRenderer>(), Track.Idle, true, 10f);
                _animatorController.StartAnimation(_teleporters[i].SecondSpriteRenderer.GetComponent<SpriteRenderer>(), Track.Idle, true, 10f);
            }
        }
    }

    public void SetAnimationController(SpriteAnimatorController animationController)
    {
        _animatorController = animationController;
    }

    public Transform GetDestination(Transform teleporter)
    {
        if (_startTeleporter == teleporter)
            return null;
        if (_finishTeleporter == teleporter)
        {
            Finish?.Invoke();
            return null;
        }
        for (int i = 0; i<_teleporters.Count; i++)
        {
            var pair = _teleporters[i].GetPair(teleporter);
            if (pair != null)
                return pair;        }

        return null;
    }

    public Transform GetStart()
    {
        return _startTeleporter;
    }
}
