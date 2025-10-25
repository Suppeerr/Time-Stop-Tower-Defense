using System.Collections.Generic;
using UnityEngine;

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

    public float x;
    public float y;
    public float z;
    private float speed;

    public EnemyType type;
    public EnemyPath spath;
    public int currentWaypoint = 1;
    public float curdist_traveled;

    public EnemyStatsContainer statsContainer;
    public GameObject visualObj;
    public GameObject enemyvObjPrefab;
    public EnemyHealthBar healthbar;
    public LevelInstance level;

    public void Init(GameObject prefab, LevelInstance level, EnemyPath spath, EnemyType eType)
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

        // Updates healthbar to indicate damage taken
        healthbar?.UpdateHealth(hp);

        // The enemy dies if its hp becomes 0 or less
        if (hp <= 0)
        {
            OnDeath();
            Clearself();
        }
    }


    public void As_spawn()
    {
        visualObj = GameObject.Instantiate(enemyvObjPrefab);
        x = spath.waypoints[0].x;
        y = spath.waypoints[0].y;
        z = 0;
        visualObj.transform.position = new Vector3(x, z, y);

        // Attach proxy
        var proxy = visualObj.AddComponent<EnemyProxy>();
        proxy.Init(this);

        // Gets healthbar
        healthbar = visualObj.GetComponentInChildren<EnemyHealthBar>();
        healthbar?.Init(baseHp);
    }
    public void As_update()
    {
        Move();
        //internal naming needs to be changed - I didnt realize unity used y for height instead of z
        visualObj.transform.position = new Vector3(x, z, y); //also change orientation > face vector direction
        //other internal enemy things
    }
    public void Clearself()
    {
        Object.Destroy(visualObj);
        level.queueRemove.Add(this);
        //s
    }
    public void Pathend()
    {
        OnReachEnd();
        Clearself();
        Debug.Log("enemy reached end");
    }

    //called per-update
    public void Move()
    {
        if (ProjectileManager.IsFrozen)
        {
            return;
        }
        float distance_traveled = speed * Time.deltaTime;
        Waypoint targ_waypoint = spath.waypoints[currentWaypoint];

        //debug
        float test_wp = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(targ_waypoint.x - x), 2) + Mathf.Pow(Mathf.Abs(targ_waypoint.y - y), 2));
        float dist_to_next_waypoint = test_wp - distance_traveled;
        //Debug.Log("position: (" + x + "," + y + ") waypoint: (" + targ_waypoint.x + "," + targ_waypoint.y + ") distance: "+ test_wp + " next wp:" + dist_to_next_waypoint + " targ_waypoint.x_modif: "+ targ_waypoint.x_modif + " targ_waypoint.y_modif: " + targ_waypoint.y_modif + " curdist_traveled: " + curdist_traveled + " targ_waypoint.dist: " + targ_waypoint.dist + " targ_waypoint.yvec: " + targ_waypoint.yvec);

        if (curdist_traveled + distance_traveled >= targ_waypoint.dist)
        {
            Debug.Log("new waypoint");
            this.y = targ_waypoint.y;
            this.x = targ_waypoint.x;
            distance_traveled = curdist_traveled + distance_traveled - targ_waypoint.dist;
            curdist_traveled = 0;
            currentWaypoint += 1;
            if (spath.waypoints.Length <= currentWaypoint)
            {
                this.Pathend();
                return;
            }
            targ_waypoint = spath.waypoints[currentWaypoint];
        }

        curdist_traveled += distance_traveled;
        this.y += distance_traveled * targ_waypoint.y_modif;
        this.x += distance_traveled * targ_waypoint.x_modif;
    }


    //overridable implementations for diff enemy types
    public virtual void OnReachEnd() { }
    public virtual void OnSpawn() { }
    public virtual void OnDestroy() { }
    public virtual void OnDeath() { }
}