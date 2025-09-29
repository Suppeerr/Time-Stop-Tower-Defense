using System.Linq;
using UnityEngine;

public class enemyPathing
{
    public waypoint[] waypoints { get; private set; }
    public void addWaypoint(float x, float y)
    {
        if (waypoints.Length == 0)
        {
            waypoints = waypoints.Append(new waypoint(x, y)).ToArray();
            return;
        }
        
        waypoint prevPoint = waypoints[waypoints.Length - 1];
        waypoints = waypoints.Append(new waypoint(x, y, prevPoint.x,prevPoint.y)).ToArray();
    }
}

public class waypoint
{
    public float x, y;

    public float x_modif { get; private set; }
    public float y_modif { get; private set; }
    public float? xvec { get; private set; }
    public float? yvec { get; private set; }
    public waypoint(float x, float y, float prevX, float prevY)
    {
        this.x = x;
        this.y = y;
        xvec = x - prevX;
        yvec = y - prevY;
        float dist_modif = xvec ?? 0 + yvec ?? 0;
        x_modif = xvec ?? 0 / dist_modif;
        y_modif = yvec ?? 0 / dist_modif;
    }

    public waypoint(float x, float y)
    {
        this.x = x;
        this.y = y;
        xvec = null;
        yvec = null;
    }
}

//reference numbers: enemy hp 100~100000, projectile damage 25~x
public abstract partial class baseEnemy
{
    //base stats:
    public int baseHp; 
    public int baseDef;
    public int baseRes;

    public int hp;
    public int def;
    public int res;

    public float x;
    public float y;
    public float speed;

    public readonly enemyPathing spath;
    public int currentWaypoint = 1; 

    public abstract void OnSpawn();
    public abstract void OnDestroy();
    public abstract void OnDie();

    public virtual void takeDamage(damageInstance damage)
    {
        float damagerecieved = Mathf.Max(0, damage.damage);
        if (damage.isPercentage) damagerecieved = ((float)damage.damage) / 100 * baseHp;
        if (damage.damageMax != -1) Mathf.Clamp(damagerecieved, 0, damage.damageMax);
        float minDamage = damagerecieved * 0.05f;

        if (damage.isDef) damagerecieved = Mathf.Clamp(damagerecieved - def, minDamage, damagerecieved);
        if (damage.isRes) damagerecieved = Mathf.Clamp(damagerecieved * (1 - (float)res), minDamage, damagerecieved);
        hp -= (int)damagerecieved;
        if (hp < 0) OnDie();
    }

    //called per-update
    public void s_move()
    {
        float distance_traveled = speed * Time.deltaTime;
        waypoint targ_waypoint = spath.waypoints[currentWaypoint];

        float dist_to_next_waypoint = Mathf.Abs(targ_waypoint.x - x) - Mathf.Abs(distance_traveled * targ_waypoint.x_modif);
        if (dist_to_next_waypoint <= 0)
        {
            this.y = targ_waypoint.y;
            this.x = targ_waypoint.x;
            currentWaypoint += 1;
            targ_waypoint = spath.waypoints[currentWaypoint];
            distance_traveled = distance_traveled - dist_to_next_waypoint;
        }

        this.y += distance_traveled * targ_waypoint.x_modif;
        this.x += distance_traveled * targ_waypoint.y_modif;
    }
}

public partial class damageInstance
{
    public int damage;

    public bool isDef = true;
    public bool isRes = true;
    public bool isPercentage = false;
    public int damageMax = -1;

    public damageInstance(int dmg)
    {
        damage = dmg;
    }

    public damageInstance(int dmg, bool iDef, bool iRes)
    {
        damage = dmg;
        isDef = iDef;
        isRes = iRes;
    }
    public damageInstance(int dmg, bool iPercentage, int idmgM, bool iDef, bool iRes)
    {
        damage = dmg;
        isDef = iDef;
        isRes = iRes;
        isPercentage = iPercentage;
        damageMax = idmgM;
    }

    public damageInstance(int dmg, bool iPercentage, int idmgM)
    {
        damage = dmg;
        isPercentage = iPercentage;
        damageMax = idmgM;
    }
}


public class exampleEnemy : baseEnemy
{
    public bool testpassive_isDefboost = false;
    public override void OnSpawn()
    {
        //unity spawn effects and visuals?
    }
    public void OnSpawn(int bhp, int bdef)
    {
        hp = baseHp = bhp;
        def = baseDef = bdef;
        OnSpawn();
    }
    public override void OnDestroy()
    {
        //unity clear entity/memory effects? 
    }
    public override void OnDie()
    {
        //unity death effects and visuals
        OnDestroy();
    }

    public override void takeDamage(damageInstance damage)
    {
        //additional damage calc info if needed
        base.takeDamage(damage);

        //example passive that doubles defence and resistance when hp is under half at the cost of 25 hp.
        if (hp < (baseHp / 2) && !testpassive_isDefboost)
        {
            def *= 2;
            res *= 2;
            base.takeDamage(new damageInstance(25,false,false));
            testpassive_isDefboost=true;
        }
    }
}