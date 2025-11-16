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
    public Quaternion faceDirection;
    public float dist;

    public Waypoint(Vector3 position, Vector3 source)
    {
        this.position = position;
        dist = (position - source).magnitude;
        modif = (position - source).normalized;
        faceDirection = Quaternion.LookRotation((source - position).normalized);
        Debug.Log($"Created New Waypoint: at {position}, modif {modif} ({dist})");
    }

    public Waypoint(Vector3 position)
    {
        this.position = position;
    }
}