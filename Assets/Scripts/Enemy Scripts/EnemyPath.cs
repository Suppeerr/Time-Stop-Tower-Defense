using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyWaypointPath
{
    public List<Waypoint> Waypoints { get; private set; } = new List<Waypoint>();
    public void AddWaypoint(float x, float y, float z)
    {
        AddWaypoint(new Vector3(x, y, z));
    }

    public void AddWaypoint(Vector3 position)
    {
        float cumuDist = 0f;
        if (Waypoints.Count == 0)
        {
            Waypoints.Add(new Waypoint(position, position, cumuDist)); 
        }
        else
        {
            Waypoint prev = Waypoints[Waypoints.Count - 1];
            cumuDist = prev.cumulativeDist + Vector3.Distance(prev.position, position);
            Waypoints.Add(new Waypoint(position, prev.position, cumuDist)); 

        }
    }
}

public class Waypoint
{
    public Vector3 position;
    public Vector3 modif;
    public Quaternion faceDirection;
    public float cumulativeDist;
    public float dist;

    public Waypoint(Vector3 position, Vector3 source, float cumulativeDist)
    {
        this.position = position;
        modif = (position - source).normalized;
        faceDirection = Quaternion.LookRotation((source - position).normalized);
        this.cumulativeDist = cumulativeDist;
        dist = (position - source).magnitude;

        // Debug.Log($"Created New Waypoint: at {position}, modif {modif} ({dist})");
    }

    public Waypoint(Vector3 position)
    {
        this.position = position;
    }
}