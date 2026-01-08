using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

//base stats: (could be stored as a seperate data structure)
//reference numbers: enemy hp 100~100000, projectile damage 25~x
public class BaseEnemy
{
    // Base enemy stats
    private int baseHp;
    private int baseDef;
    private int baseRes;

    // Current enemy stats
    private int hp;
    private int def;
    private int res;

    // Enemy movement fields
    private Vector3 s_position;
    private float speed;
    private Quaternion visObjbaseRot;
    private EnemyWaypointPath spath;
    private int currentWaypoint = 1;
    private float curdist_traveled;

    // Enemy type and stats container
    private EnemyType type;
    private EnemyStatsContainer statsContainer;

    // Enemy instance
    public GameObject visualObj;
    private GameObject enemyvObjPrefab;

    // Enemy visuals
    private EnemyHealthBar healthbar;
    private GameObject damageIndicatorPrefab;
    private EnemyCounter enemyCounterScript;

    // Level instance
    private LevelInstance level;
    
    // Initializes an enemy with its stats and then spawns it
    public async Task Init(GameObject prefab, LevelInstance level, EnemyWaypointPath spath, EnemyType eType)
    {
        // Assigns stat values to enemies
        type = eType;
        string statsAddress = "ScriptableObjects/Stats & Data Containers/EnemyStatsContainer";
        statsContainer = await AddressableLoader.GetAsset<EnemyStatsContainer>(statsAddress);

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
        
        await As_spawn();
    }

    // Called whenever an enemy takes damage
    public virtual void TakeDamage(DamageInstance damage)
    {
        float damageRecieved = Mathf.Max(0, damage.damage);
        if (damage.isPercentage) damageRecieved = ((float)damage.damage) / 100 * baseHp;
        if (damage.damageMax != -1) Mathf.Clamp(damageRecieved, 0, damage.damageMax);
        float minDamage = damageRecieved * 0.05f;

        if (damage.isDef) damageRecieved = Mathf.Clamp(damageRecieved - def, minDamage, damageRecieved);
        if (damage.isRes) damageRecieved = Mathf.Clamp(damageRecieved * (1 - (float)res), minDamage, damageRecieved);
        hp -= (int)damageRecieved;

        // Updates healthbar to indicate damage taken
        healthbar?.UpdateHealth(hp);

        // Spawns a damage indicator visual above the enemy's head
        EnemyDamageIndicator damageIndicator = UnityEngine.Object.Instantiate(damageIndicatorPrefab).GetComponent<EnemyDamageIndicator>();
        damageIndicator.ShowDamage(damageRecieved, visualObj.transform);

        // The enemy dies if its hp becomes 0 or less
        if (hp <= 0)
        {
            enemyCounterScript.IncrementCount();
            OnDeath();
            _s_clearself();
        }
    }

    // Called on enemy spawn
    public async Task As_spawn()
    {
        visualObj = GameObject.Instantiate(enemyvObjPrefab);
        visualObj.SetActive(false);

        this.s_position = spath.waypoints[0].position;
        visualObj.transform.position = s_position;
        visObjbaseRot = visualObj.transform.rotation;

        // Attach proxy
        var proxy = visualObj.AddComponent<EnemyProxy>();
        proxy.Init(this);

        // Gets damage indicator
        string indicatorAddress = "Enemies/Enemy Damage Indicator";
        damageIndicatorPrefab = await AddressableLoader.GetAsset<GameObject>(indicatorAddress);

        // Gets healthbar
        healthbar = visualObj.GetComponentInChildren<EnemyHealthBar>();
        healthbar?.Init(baseHp);

        // Gets enemies defeated counter
        enemyCounterScript = GameObject.Find("Enemies Defeated Manager")?.GetComponent<EnemyCounter>();
        visualObj.SetActive(true);
    }

    // Called each frame the enemy is active
    public void As_update()
    {
        _s_move();
        visualObj.transform.position = s_position;
        //other internal enemy things
    }

    // Called when enemy should be destroyed
    private void _s_clearself()
    {
        Object.Destroy(visualObj);
        level.queueRemove.Add(this);
    }

    // Called when enemy reaches end of track
    private void _s_pathend()
    {
        OnReachEnd();
        _s_clearself();
        BaseHealthManager.Instance.UpdateBaseHP(-50);
        Debug.Log("enemy reached end");
    }

    // Moves enemy along the track
    private void _s_move()
    {   //... change positioning later - jack (self)
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

    // Gets the enemy's distance from the previous waypoint
    public float GetCurDistTraveled()
    {
        return curdist_traveled;
    }

    // Gets the enemy's previous waypoint number
    public float GetCurrentWaypoint()
    {
        return currentWaypoint;
    }


    // Overridable implementations for diff enemy types
    public virtual void OnReachEnd() { }
    public virtual void OnSpawn() { }
    public virtual void OnDestroy() { }
    public virtual void OnDeath()
    {
        CoinSpawner.Instance.SpawnCoin(enemyPos: visualObj.transform.position);
    }
}