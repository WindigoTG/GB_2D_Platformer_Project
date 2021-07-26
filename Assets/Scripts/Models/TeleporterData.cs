using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeleporterData
{
    [SerializeField] Transform _startTeleporter;
    [SerializeField] List<PairedTeleporterData> _teleporters;
    [SerializeField] Transform _finishTeleporter;

    public Transform StartTeleporter => _startTeleporter;
    public Transform FinishTeleporter => _finishTeleporter;
    public List<PairedTeleporterData> Teleporters => _teleporters;
}
