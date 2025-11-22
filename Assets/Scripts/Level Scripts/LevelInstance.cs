using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class LevelInstance : MonoBehaviour
{
    public EnemyWaypointPath epath = new EnemyWaypointPath(); //could be array if we want multiple paths
    List<BaseEnemy> enemies = new List<BaseEnemy>();
    public List<BaseEnemy> queueRemove = new List<BaseEnemy>();
    bool s_enabled = false;
    private float spawnInterval;
    private float elapsed = 0f;
    private float lowStarting = 3.5f;
    private float highStarting = 5.5f;
    private float ramping = 0.04f;
    GameObject ePrefab;
    public static LevelInstance instance;

    public void Init()
    {
        instance = this;
        Debug.Log("levelInst started");
        if (SceneManager.GetActiveScene().name.Equals("Gameplay and Mechanics") || SceneManager.GetActiveScene().name.Equals("Level 1")) s_enabled = true;
        if (!s_enabled) return;

        Debug.Log("levelInst enabled");
        ePrefab = (GameObject)Resources.Load("Normal Bandit");
        //this would be ideally loaded from a data structure or from file before the scene begin
        LoadWaypointPrefabs();
        
        spawnInterval = Random.Range(1f, 3f);
    }

    public void LoadWaypointPrefabs()
    {
        GameObject pathObj = GameObject.FindWithTag("Path");

        if (pathObj == null)
        {
            Debug.LogError("No object tagged WaypointRoot found!");
            return;
        }

        Transform path = pathObj.transform;

        foreach (Transform child in path)
        {
            epath.addWaypoint(child.position);
            child.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        
        if (elapsed < spawnInterval)
        {
            elapsed += Time.deltaTime;
        }
        else
        {
            SpawnEnemyTest();
            spawnInterval = Random.Range(lowStarting, highStarting);
            elapsed = 0;
            if (lowStarting > 0)
            {
                lowStarting -= ramping;
                highStarting -= ramping;
            }
        }
         if (!s_enabled) return;

        // Temporary list to hold enemies to remove
        List<BaseEnemy> toRemove = new List<BaseEnemy>();

        foreach (BaseEnemy bEnemy in enemies)
        {
            // Only update enemies that still have a visual object
            if (bEnemy.visualObj != null)
            {
                bEnemy.As_update();
            }
            else
            {
                // Mark for removal if visualObj has been destroyed
                toRemove.Add(bEnemy);
            }
        }

        // Remove dead enemies from main list
        foreach (BaseEnemy dead in toRemove)
        {
            enemies.Remove(dead);
        }

        // Also remove any queued removals (if your Clearself uses queueRemove)
        foreach (BaseEnemy queued in queueRemove)
        {
            enemies.Remove(queued);
        }
        queueRemove.Clear();
    }

    public void SpawnEnemyTest()
    {
        Debug.Log("levelInst spawned enemy");
        BaseEnemy enemy = new BaseEnemy();
        enemy.Init(ePrefab, this, epath, EnemyType.NormalBandit);
        enemies.Add(enemy);
    }

    public BaseEnemy GetFirstEnemy()
    {
        float currentWaypointDist = -1;
        float furthestWaypoint = -1;
        BaseEnemy firstEnemy = null;

        foreach(var enemy in enemies)
        {
            float cw = enemy.GetCurrentWaypoint();
            float cd = enemy.GetCurDistTraveled();
            if (cw > furthestWaypoint || (cw == furthestWaypoint && cd > currentWaypointDist))
            {
                firstEnemy = enemy;
                furthestWaypoint = cw;
                currentWaypointDist = cd;
            }
        }
        
        return firstEnemy;
    }

    public static LevelInstance GetLevelInstance()
    {
        return instance;
    }
}