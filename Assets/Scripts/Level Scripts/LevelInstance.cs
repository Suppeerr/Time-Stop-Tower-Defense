using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelInstance : MonoBehaviour
{
    public EnemyPath epath = new EnemyPath(); //could be array if we want multiple paths
    List<BaseEnemy> enemies = new List<BaseEnemy>();
    public List<BaseEnemy> queueRemove = new List<BaseEnemy>();
    bool s_enabled = false;
    GameObject ePrefab;

    public void Awake()
    {
        Debug.Log("levelInst started");
        if (SceneManager.GetActiveScene().name.Equals("Gameplay and Mechanics")) s_enabled = true;
        if (!s_enabled) return;

        Debug.Log("levelInst enabled");
        ePrefab = (GameObject)Resources.Load("Bandit");
        //this would be ideally loaded from a data structure or from file before the scene begins
        epath.addWaypoint(0, 0); 
        epath.addWaypoint(0, 15);
        epath.addWaypoint(-11,15);
        epath.addWaypoint(0, 0);

        InvokeRepeating(nameof(spawnEnemyTest), 1.0f, 3.0f);
    }
    public void Update()
    {
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

    public void spawnEnemyTest()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        Debug.Log("levelInst spawned enemy");
        enemies.Add(new BaseEnemy(3, ePrefab, this, epath));
    }
}