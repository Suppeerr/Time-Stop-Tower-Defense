using UnityEngine;
using System.Collections.Generic;

// Defines the objects that change depending on the time stop duration
[System.Serializable]
public class TimeStopObject
{
    // Duration-dependent game objects 
    public List<GameObject> runeObjects;
    public List<GameObject> beamObjects;

    // Time stop duration threshold
    public float percent;

    // Boolean storing whether a given threshold has been reached
    [HideInInspector] public bool active = false;

    // Sets an object active if its threshold is reached
    public void SetObjectActive(bool isActive)
    {
        active = isActive;
    }
}