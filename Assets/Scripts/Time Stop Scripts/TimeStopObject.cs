using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TimeStopObject
{
    public List<GameObject> runeObjects;
    public List<GameObject> beamObjects;
    public float percent;
    [HideInInspector] public bool active = false;

    public void SetObjectActive(bool isActive)
    {
        active = isActive;
    }
}