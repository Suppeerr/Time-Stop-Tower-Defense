using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct ClosestSegmentInfo
{
    public Vector3 point;
    public int startIndex;
    public float t;
}

public class VineGrower : MonoBehaviour
{
    private List<Vine> vines = new List<Vine>();
    [SerializeField] private GameObject vinePrefab;
    private EnemyWaypointPath path;
    private float spawnInterval = 10f;
    private int vineCap = 5;
    private Vine leftmostVine;
    private Vine rightmostVine;

    private IEnumerator Start()
    {
        while (LevelInstance.Instance == null
               || !LevelInstance.Instance.WaypointsLoaded
               || !LevelStarter.HasLevelStarted)
        {
            yield return null;
        }

        path = LevelInstance.Instance.epath;

        // Spawns a vine at the closest point in the track at set intervals
        SpawnVine();
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnVine();
        }
    }

    // Returns information on the closest segment on the track from a tower
    public ClosestSegmentInfo GetClosestSegmentInfo(Vector3 towerPos)
    {
        ClosestSegmentInfo info = new ClosestSegmentInfo();
        float closestDist = float.MaxValue;

        for (int i = 0; i < path.Waypoints.Count - 1; i++)
        {
            Vector3 startPos = path.Waypoints[i].position;
            Vector3 endPos = path.Waypoints[i + 1].position;
            Vector3 closest = GetClosestPointOnSegment(startPos, endPos, towerPos);

            float dist = (towerPos - closest).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                info.point = closest;
                info.startIndex = i;
                info.t = Vector3.Distance(closest, startPos) 
                         / Vector3.Distance(endPos, startPos);
            }
        }

        return info;
    }

    // Finds the closest point from a tower to a given segment of the track
    private Vector3 GetClosestPointOnSegment(Vector3 startPos, Vector3 endPos, Vector3 towerPos)
    {
        float t = Vector3.Dot(towerPos - startPos, endPos - startPos) 
                  / Vector3.Dot(endPos - startPos, endPos - startPos);
        t = Mathf.Clamp01(t);

        return startPos + t * (endPos - startPos);
    }

    // Spawns a vine on the track
    private void SpawnVine()
    {
        if (vines.Count >= vineCap)
        {
            return;
        }

        Debug.Log("Vine spawned!");
        ClosestSegmentInfo closestSegInfo = GetClosestSegmentInfo(transform.position);
        Vector3 spawnPos = closestSegInfo.point;
        int segIndex = closestSegInfo.startIndex;
        float segmentT = closestSegInfo.t;
        Quaternion randomRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        // Calculates each vine's distance along the track
        Waypoint start = path.Waypoints[segIndex];
        Waypoint end = path.Waypoints[segIndex + 1];
        float segLength = end.cumulativeDist - start.cumulativeDist;
        float vineDist = start.cumulativeDist + segmentT * segLength;
        
        if (vines.Count == 0)
        {
            GameObject vineObj = Instantiate(vinePrefab, spawnPos, randomRot);
            Vine vineScript = vineObj.GetComponent<Vine>();
            leftmostVine = vineScript;
            rightmostVine = vineScript;

            vineScript.Init(closestSegInfo, vineDist, this);
            vines.Add(vineScript);
            return;
        }
        if (vines.Count > 0)
        {        
            // Spawns subsequent vines left or right of any frontier vines
            float vineSpacing = Random.Range(0.8f, 1.2f);
            if (Random.value < 0.5f)
            {
                vineDist = rightmostVine.TrackDist + vineSpacing;
            } 
            else
            {
                vineDist = leftmostVine.TrackDist - vineSpacing;
            }
            spawnPos = ConvertPathToWorldSpace(vineDist, ref segIndex);
            GameObject vineObj = Instantiate(vinePrefab, spawnPos, randomRot);
            Vine vineScript = vineObj.GetComponent<Vine>();

            if (vineDist > leftmostVine.TrackDist)
            {
                rightmostVine = vineScript;
            }
            else
            {
                leftmostVine = vineScript;
            }

            vineScript.Init(closestSegInfo, vineDist, this);
            vines.Add(vineScript);
            return;
        }
    }

    // Randomly adds a set amount of charges to each vine of the vine tower
    public void RechargeVines()
    {
        foreach (Vine vine in vines)
        {
            if (Random.value < 0.5f)
            {
                vine.AddCharges(1);
            }
        }

        if (CheckFullyCharged())
        {
            ChangeLayer("Tower");
        }
        else
        {
            ChangeLayer("Vine Tower");
        }
    }

    // Gets the total number of charges across all vines
    private int GetTotalCharges()
    {
        int chargeCount = 0;
        foreach (Vine vine in vines)
        {
            chargeCount += vine.GetCharges();
        }

        return chargeCount;
    }

    // Returns whether or not all vines are fully charged
    public bool CheckFullyCharged()
    {
        return GetTotalCharges() / (float) vines.Count == vines[0].GetMaxCharges();
    }

    // Converts a vine's distance along the path to a world space point
    private Vector3 ConvertPathToWorldSpace(float newVineDist, ref int segIndex)
    {
        Waypoint start = path.Waypoints[segIndex];
        Waypoint end = path.Waypoints[segIndex + 1];

        while (newVineDist > end.cumulativeDist && segIndex < path.Waypoints.Count - 2)
        {
            segIndex++;
            start = path.Waypoints[segIndex];
            end = path.Waypoints[segIndex + 1];
        }

        while (newVineDist < start.cumulativeDist && segIndex > 0)
        {
            segIndex--;
            start = path.Waypoints[segIndex];
            end = path.Waypoints[segIndex + 1];
        }

        float t = (newVineDist - start.cumulativeDist) / (end.cumulativeDist - start.cumulativeDist);
        return Vector3.Lerp(start.position, end.position, t);
    }

    public void ChangeLayer(string layerToSwitch)
    {
        gameObject.layer = LayerMask.NameToLayer(layerToSwitch);
    }
}
