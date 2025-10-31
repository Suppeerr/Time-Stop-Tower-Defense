using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyPath
{
    public Waypoint[] waypoints { get; private set; } = new Waypoint[0];
    public void addWaypoint(float x, float y)
    {
        if (waypoints.Length == 0)
        {
            waypoints = waypoints.Append(new Waypoint(x, y)).ToArray();
            return;
        }

        Waypoint prevPoint = waypoints[waypoints.Length - 1];
        waypoints = waypoints.Append(new Waypoint(x, y, prevPoint.x, prevPoint.y)).ToArray();
    }
}

public class Waypoint
{
    public float x, y;

    public float dist;
    public float x_modif { get; private set; }
    public float y_modif { get; private set; }
    public float? xvec { get; private set; }
    public float? yvec { get; private set; }
    public Waypoint(float x, float y, float prevX, float prevY)
    {
        this.x = x;
        this.y = y;
        xvec = x - prevX;
        yvec = y - prevY;
        float dist_modif = Mathf.Abs(xvec ?? 0) + Mathf.Abs(yvec ?? 0);
        x_modif = (xvec ?? 0) / dist_modif;
        y_modif = (yvec ?? 0) / dist_modif;
        dist = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(xvec ?? 0), 2) + Mathf.Pow(Mathf.Abs(yvec ?? 0), 2));
    }

    public Waypoint(float x, float y)
    {
        this.x = x;
        this.y = y;
        xvec = null;
        yvec = null;
    }
}