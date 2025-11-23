using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

//base stats: (could be stored as a seperate data structure)
//reference numbers: enemy hp 100~100000, projectile damage 25~x
public class BaseEnemy
{
    //base stats: (could be stored as a seperate data structure)
    private int baseHp;
    private int baseDef;
    private int baseRes;

    private int hp;
    private int def;
    private int res;

    public Vector3 s_position;
    private float speed;

    public EnemyType type;
    public EnemyWaypointPath spath;
    public int currentWaypoint = 1;
    public float curdist_traveled;

    public EnemyStatsContainer statsContainer;
    public GameObject visualObj;
    public GameObject enemyvObjPrefab;
    public EnemyHealthBar healthbar;
    public EnemyDamageIndicator damageIndicator;
    public EnemyCounter enemyCounterScript;
    public LevelInstance level;

    public Quaternion visObjbaseRot;

    public void Init(GameObject prefab, LevelInstance level, EnemyWaypointPath spath, EnemyType eType)
    {
        //other init also goes here...
        // Assigns stat values to enemies
        type = eType;
        statsContainer = Resources.Load<EnemyStatsContainer>("EnemyStatsContainer 1");

        EnemyStats stats = statsContainer.GetStats(type);
        if (stats == null)
        {
            Debug.LogError($"No stats found for enemy type {type}");
            return;
        }
        baseHp = stats.hp;
        hp = baseHp;
        speed = stats.spd;
        baseDef = stats.def;
        def = baseDef;
        enemyvObjPrefab = prefab;
        this.level = level;
        this.spath = spath;
        
        As_spawn();
    }

    public virtual void TakeDamage(DamageInstance damage)
    {
        float damagerecieved = Mathf.Max(0, damage.damage);
        if (damage.isPercentage) damagerecieved = ((float)damage.damage) / 100 * baseHp;
        if (damage.damageMax != -1) Mathf.Clamp(damagerecieved, 0, damage.damageMax);
        float minDamage = damagerecieved * 0.05f;

        if (damage.isDef) damagerecieved = Mathf.Clamp(damagerecieved - def, minDamage, damagerecieved);
        if (damage.isRes) damagerecieved = Mathf.Clamp(damagerecieved * (1 - (float)res), minDamage, damagerecieved);
        hp -= (int)damagerecieved;

        // Spawns a damage indicator visual above the enemy's head
        damageIndicator?.ShowDamage(damagerecieved);
        if (damageIndicator == null)
        {
            Debug.Log("Damage indicator is null.");
        }

        // Updates healthbar to indicate damage taken
        healthbar?.UpdateHealth(hp);

        // The enemy dies if its hp becomes 0 or less
        if (hp <= 0)
        {
            enemyCounterScript.IncrementCount();
            OnDeath();
            _s_clearself();
        }
    }

    public void As_spawn()
    {
        visualObj = GameObject.Instantiate(enemyvObjPrefab);
        this.s_position = spath.waypoints[0].position;
        visualObj.transform.position = s_position;
        visObjbaseRot = visualObj.transform.rotation;

        // Attach proxy
        var proxy = visualObj.AddComponent<EnemyProxy>();
        proxy.Init(this);

        // Gets damage indicator
        damageIndicator = visualObj.GetComponentInChildren<EnemyDamageIndicator>();

        // Gets healthbar
        healthbar = visualObj.GetComponentInChildren<EnemyHealthBar>();
        healthbar?.Init(baseHp);

        // Gets enemies defeated counter
        enemyCounterScript = GameObject.Find("Enemies Defeated Manager")?.GetComponent<EnemyCounter>();
    }
    public void As_update()
    {
        _s_move();
        visualObj.transform.position = s_position;
        //other internal enemy things
    }
    private void _s_clearself()
    {
        Object.Destroy(visualObj);
        level.queueRemove.Add(this);
        //s
    }
    private void _s_pathend()
    {
        OnReachEnd();
        _s_clearself();
        BaseHealthManager.Instance.UpdateBaseHP(-50);
        Debug.Log("enemy reached end");
    }

    //called per-update
    private void _s_move()
    {   //... change positioning later - jack (self)
        if (ProjectileManager.IsFrozen)
        {
            return;
        }

        float distance_traveled = speed * Time.deltaTime;
        Waypoint targ_waypoint = spath.waypoints[currentWaypoint];

        if (curdist_traveled + distance_traveled >= targ_waypoint.dist)
        {
            this.s_position = targ_waypoint.position;
            distance_traveled = curdist_traveled + distance_traveled - targ_waypoint.dist;
            curdist_traveled = 0;
            currentWaypoint += 1;
            if (spath.waypoints.Length <= currentWaypoint)
            {
                this._s_pathend();
                return;
            }
            targ_waypoint = spath.waypoints[currentWaypoint];
            visualObj.transform.rotation = visObjbaseRot * spath.waypoints[currentWaypoint].faceDirection;
            Vector3 testv = visualObj.transform.eulerAngles;
            visualObj.transform.rotation = Quaternion.Euler(-testv.z, testv.y, -testv.x); //correct facing direction
        }

        curdist_traveled += distance_traveled;
        s_position += distance_traveled * targ_waypoint.modif;
    }

    public float GetCurDistTraveled()
    {
        return curdist_traveled;
    }

    public float GetCurrentWaypoint()
    {
        return currentWaypoint;
    }


    //overridable implementations for diff enemy types
    public virtual void OnReachEnd() { }
    public virtual void OnSpawn() { }
    public virtual void OnDestroy() { }
    public virtual void OnDeath()
    {
        CoinSpawner.Instance.SpawnCoin(false, enemyPos: visualObj.transform.position);
    }
}