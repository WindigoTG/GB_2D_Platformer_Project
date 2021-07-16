using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterHandler : MonoBehaviour
{
    [SerializeField] Transform _startTeleporter;
    [SerializeField] List<PairedTeleporterData> _teleporters;
    [SerializeField] Transform _finishTeleporter;

    private SpriteAnimatorController _animatorController;

    private int _lastTeleporterPair;

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
            {
                _lastTeleporterPair = i;
                return pair;
            }
        }

        return null;
    }

    public bool IsTeleporterUnlocked(Transform teleporter)
    {
        var pair = _teleporters[_lastTeleporterPair].GetPair(teleporter);
        if (pair == null)
            for (int i = 0; i < _teleporters.Count; i++)
            {
                pair = _teleporters[i].GetPair(teleporter);
                if (pair != null)
                    _lastTeleporterPair = i;
            }
        if (pair != null)
            return _teleporters[_lastTeleporterPair].IsUnlocked;
        else
            return false;
    }

    public void Unlock(Transform teleporter)
    {
        var pair = _teleporters[_lastTeleporterPair].GetPair(teleporter);
        if (pair == null)
            for (int i = 0; i < _teleporters.Count; i++)
            {
                pair = _teleporters[i].GetPair(teleporter);
                if (pair != null)
                    _lastTeleporterPair = i;
            }
        if (pair != null)
            _teleporters[_lastTeleporterPair].SetUnlocked();
    }

    public Transform GetStart()
    {
        return _startTeleporter;
    }
}
