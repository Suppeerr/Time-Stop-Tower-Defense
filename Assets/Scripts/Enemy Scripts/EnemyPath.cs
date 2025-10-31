using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enemyWaypointPath
{
    public Waypoint[] waypoints { get; private set; } = new Waypoint[0];
    public void addWaypoint(float x, float y, float z)
    {
        addWaypoint(new Vector3(x, y, z));
    }

    public void addWaypoint(Vector3 position)
    {
        if (waypoints.Length == 0)
        {
            waypoints = waypoints.Append(new Waypoint(position)).ToArray();
            return;
        }

        Waypoint prevPoint = waypoints[waypoints.Length - 1];
        waypoints = waypoints.Append(new Waypoint(position, prevPoint.position)).ToArray(); //this datastructure probally shouldn't be an array...
    }

}

public class Waypoint
{

    public Vector3 position;

    public Vector3 modif;

    public Quaternion facedirection;

    public float x, y;

    public float dist;
    public float vdist;
    public float x_modif { get; private set; }
    public float y_modif { get; private set; }
    public float? xvec { get; private set; }
    public float? yvec { get; private set; }

    public void waypoint2(Vector3 position, Vector3 source)
    {
        this.position = position;
        vdist = (position - source).magnitude;
        modif = (position - source).normalized;
        Debug.Log($"Created New Waypoint: at {position}, modif {modif} ({(position - source).x /vdist},{(position - source).y / vdist} => {x_modif},{y_modif}), dist {vdist} ({dist}): vec {(position - source)}, ({xvec}, {yvec} > {Mathf.Abs(xvec ?? 0) + Mathf.Abs(yvec ?? 0)})");
        x_modif = modif.x;
        y_modif = modif.y;
        facedirection = Quaternion.LookRotation((source - position).normalized);
        dist = vdist;
    }
    public Waypoint(Vector3 position, Vector3 source)
    {
        this.x = position.x;
        this.y = position.y;
        float prevX = source.x;
        float prevY = source.y;
        xvec = x - prevX;
        yvec = y - prevY;
        float dist_modif = Mathf.Abs(xvec ?? 0) + Mathf.Abs(yvec ?? 0);
        x_modif = (xvec ?? 0) / dist_modif;
        y_modif = (yvec ?? 0) / dist_modif;
        dist = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(xvec ?? 0), 2) + Mathf.Pow(Mathf.Abs(yvec ?? 0), 2));
        waypoint2(position, source);
    }

    public Waypoint(Vector3 position)
    {
        this.position = position;
        this.x = position.x;
        this.y = position.y;
        xvec = null;
        yvec = null;
    }
}