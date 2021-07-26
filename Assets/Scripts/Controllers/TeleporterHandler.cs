using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterHandler
{
    private TeleporterData _teleporterData;

    private SpriteAnimatorController _animatorController;

    private int _lastTeleporterPair;

    public event Action Finish;

    public TeleporterHandler(TeleporterData teleporterData, SpriteAnimatorController animationController)
    {
        _teleporterData = teleporterData;
        _animatorController = animationController;

        _animatorController.StartAnimation(_teleporterData.StartTeleporter.GetComponent<SpriteRenderer>(), Track.Idle, true, 10f);
        _animatorController.StartAnimation(_teleporterData.FinishTeleporter.GetComponent<SpriteRenderer>(), Track.Idle, true, 10f);
        for (int i = 0; i < _teleporterData.Teleporters.Count; i++)
        {
            _animatorController.StartAnimation(_teleporterData.Teleporters[i].FirstSpriteRenderer.GetComponent<SpriteRenderer>(), Track.Idle, true, 10f);
            _animatorController.StartAnimation(_teleporterData.Teleporters[i].SecondSpriteRenderer.GetComponent<SpriteRenderer>(), Track.Idle, true, 10f);
        }
    }

    public Transform GetDestination(Transform teleporter)
    {
        if (_teleporterData.StartTeleporter == teleporter)
            return null;
        if (_teleporterData.FinishTeleporter == teleporter)
        {
            Finish?.Invoke();
            return null;
        }
        for (int i = 0; i< _teleporterData.Teleporters.Count; i++)
        {
            var pair = _teleporterData.Teleporters[i].GetPair(teleporter);
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
        var pair = _teleporterData.Teleporters[_lastTeleporterPair].GetPair(teleporter);
        if (pair == null)
            for (int i = 0; i < _teleporterData.Teleporters.Count; i++)
            {
                pair = _teleporterData.Teleporters[i].GetPair(teleporter);
                if (pair != null)
                    _lastTeleporterPair = i;
            }
        if (pair != null)
            return _teleporterData.Teleporters[_lastTeleporterPair].IsUnlocked;
        else
            return false;
    }

    public void Unlock(Transform teleporter)
    {
        var pair = _teleporterData.Teleporters[_lastTeleporterPair].GetPair(teleporter);
        if (pair == null)
            for (int i = 0; i < _teleporterData.Teleporters.Count; i++)
            {
                pair = _teleporterData.Teleporters[i].GetPair(teleporter);
                if (pair != null)
                    _lastTeleporterPair = i;
            }
        if (pair != null)
            _teleporterData.Teleporters[_lastTeleporterPair].SetUnlocked();
    }

    public Transform GetStart()
    {
        return _teleporterData.StartTeleporter;
    }
}
