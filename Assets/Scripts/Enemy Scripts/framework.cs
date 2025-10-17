// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public static class GameInstance
// {

//     static LevelInstance currentLevel;
//     static GameObject clevel;
//     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//     static void LStart()
//     {
//         Debug.Log("gameInst started");
//         clevel = new GameObject();
//         clevel.AddComponent<LevelInstance>();
//         //currentLevel = ScriptableObject.CreateInstance<levelInstance>();
//         //currentLevel = new levelInstance();
//     }
// }

// public class LevelInstance : MonoBehaviour
// {
//     public enemyPathing epath = new enemyPathing(); //could be array if we want multiple paths
//     List<baseEnemy> enemies = new List<baseEnemy>();
//     public List<baseEnemy> queueRemove = new List<baseEnemy>();
//     bool s_enabled = false;
//     GameObject ePrefab;

//     public void Awake()
//     {
//         Debug.Log("levelInst started");
//         if (SceneManager.GetActiveScene().name.Equals("Gameplay and Mechanics")) s_enabled = true;
//         if (!s_enabled) return;

//         Debug.Log("levelInst enabled");
//         ePrefab = (GameObject)Resources.Load("Bandit");
//         //this would be ideally loaded from a data structure or from file before the scene begins
//         epath.addWaypoint(0, 0); 
//         epath.addWaypoint(0, 15);
//         epath.addWaypoint(-11,15);
//         epath.addWaypoint(0, 0);

//         InvokeRepeating(nameof(spawnEnemyTest), 1.0f, 5.0f);
//     }
//     public void Update()
//     {
//         if (!s_enabled) return;
//         foreach (baseEnemy bEnemy in enemies) {
//             bEnemy.As_update();
//         }
//         foreach (baseEnemy bEnemy in queueRemove)
//         {
//             enemies.Remove(bEnemy);
//         }
//     }

//     public void spawnEnemyTest()
//     {
//         Debug.Log("levelInst spawned enemy");
//         enemies.Add(new baseEnemy(3,ePrefab,this,epath));
//     }
// }

// public class enemyPathing
// {
//     public Waypoint[] waypoints { get; private set; } = new Waypoint[0];
//     public void addWaypoint(float x, float y)
//     {
//         if (waypoints.Length == 0)
//         {
//             waypoints = waypoints.Append(new Waypoint(x, y)).ToArray();
//             return;
//         }
        
//         Waypoint prevPoint = waypoints[waypoints.Length - 1];
//         waypoints = waypoints.Append(new Waypoint(x, y, prevPoint.x,prevPoint.y)).ToArray();
//     }
// }

// public class Waypoint
// {
//     public float x, y;

//     public float dist;
//     public float x_modif { get; private set; }
//     public float y_modif { get; private set; }
//     public float? xvec { get; private set; }
//     public float? yvec { get; private set; }
//     public Waypoint(float x, float y, float prevX, float prevY)
//     {
//         this.x = x;
//         this.y = y;
//         xvec = x - prevX;
//         yvec = y - prevY;
//         float dist_modif = Mathf.Abs(xvec ?? 0) + Mathf.Abs(yvec ?? 0);
//         x_modif = (xvec ?? 0) / dist_modif;
//         y_modif = (yvec ?? 0) / dist_modif;
//         dist = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(xvec ?? 0), 2) + Mathf.Pow(Mathf.Abs(yvec ?? 0), 2));
//     }

//     public Waypoint(float x, float y)
//     {
//         this.x = x;
//         this.y = y;
//         xvec = null;
//         yvec = null;
//     }
// }

// //reference numbers: enemy hp 100~100000, projectile damage 25~x
// public partial class baseEnemy
// {
//     //base stats: (could be stored as a seperate data structure)
//     public int baseHp; 
//     public int baseDef;
//     public int baseRes;

//     public int hp;
//     public int def;
//     public int res;

//     public float x;
//     public float y;
//     public float z;
//     public float speed;

//     public readonly enemyPathing spath;
//     public int currentWaypoint = 1;
//     public float curdist_traveled;

//     public GameObject visualObj;
//     public GameObject enemyvObjPrefab;
//     public LevelInstance level;

//     public baseEnemy(float speed, GameObject prefab, LevelInstance level, enemyPathing spath)
//     {
//         //other init also goes here...
//         this.speed = speed;
//         enemyvObjPrefab = prefab;
//         this.level = level;
//         this.spath = spath;

//         As_spawn();
//     }

//     public virtual void TakeDamage(DamageInstance damage)
//     {
//         float damagerecieved = Mathf.Max(0, damage.damage);
//         if (damage.isPercentage) damagerecieved = ((float)damage.damage) / 100 * baseHp;
//         if (damage.damageMax != -1) Mathf.Clamp(damagerecieved, 0, damage.damageMax);
//         float minDamage = damagerecieved * 0.05f;

//         if (damage.isDef) damagerecieved = Mathf.Clamp(damagerecieved - def, minDamage, damagerecieved);
//         if (damage.isRes) damagerecieved = Mathf.Clamp(damagerecieved * (1 - (float)res), minDamage, damagerecieved);
//         hp -= (int)damagerecieved;
//         if (hp < 0) OnDeath();
//     }


//     public void As_spawn()
//     {
//         visualObj = GameObject.Instantiate(enemyvObjPrefab);
//         x = spath.waypoints[0].x;
//         y = spath.waypoints[0].y;
//         z = 0;
//         visualObj.transform.position = new Vector3(x, z, y);
//     }
//     public void As_update()
//     {
//         Move();
//         //internal naming needs to be changed - I didnt realize unity used y for height instead of z
//         visualObj.transform.position = new Vector3(x, z, y); //also change orientation > face vector direction
//         //other internal enemy things
//     }
//     public void Clearself()
//     {
//         Object.Destroy(visualObj);
//         level.queueRemove.Add(this);
//         //s
//     }
//     public void Pathend()
//     {
//         OnReachEnd();
//         Clearself();
//         Debug.Log("enemy reached end");
//     }

//     //called per-update
//     public void Move()
//     {
//         float distance_traveled = speed * Time.deltaTime; 
//         Waypoint targ_waypoint = spath.waypoints[currentWaypoint];
        
//         //debug
//         float test_wp = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(targ_waypoint.x - x), 2) + Mathf.Pow(Mathf.Abs(targ_waypoint.y - y), 2));
//         float dist_to_next_waypoint = test_wp - distance_traveled;
//         //Debug.Log("position: (" + x + "," + y + ") waypoint: (" + targ_waypoint.x + "," + targ_waypoint.y + ") distance: "+ test_wp + " next wp:" + dist_to_next_waypoint + " targ_waypoint.x_modif: "+ targ_waypoint.x_modif + " targ_waypoint.y_modif: " + targ_waypoint.y_modif + " curdist_traveled: " + curdist_traveled + " targ_waypoint.dist: " + targ_waypoint.dist + " targ_waypoint.yvec: " + targ_waypoint.yvec);
        
//         if (curdist_traveled + distance_traveled >= targ_waypoint.dist)
//         {
//             Debug.Log("new waypoint");
//             this.y = targ_waypoint.y;
//             this.x = targ_waypoint.x;
//             distance_traveled = curdist_traveled + distance_traveled - targ_waypoint.dist;
//             curdist_traveled = 0;
//             currentWaypoint += 1;
//             if (spath.waypoints.Length <= currentWaypoint)
//             {
//                 this.Pathend();
//                 return;
//             }
//             targ_waypoint = spath.waypoints[currentWaypoint];
//         }

//         curdist_traveled += distance_traveled;
//         this.y += distance_traveled * targ_waypoint.y_modif;
//         this.x += distance_traveled * targ_waypoint.x_modif;
//     }


//     //overridable implementations for diff enemy types
//     public virtual void OnReachEnd() { }
//     public virtual void OnSpawn() { }
//     public virtual void OnDestroy() { }
//     public virtual void OnDeath() { }
// }

// public class enemyObject : MonoBehaviour
// {
//     baseEnemy s_edatastructure;

//     //game object specifics go here, such as animations or hitboxes
// }
// public partial class DamageInstance
// {
//     public int damage;

//     public bool isDef = true;
//     public bool isRes = true;
//     public bool isPercentage = false;
//     public int damageMax = -1;

//     public DamageInstance(int dmg)
//     {
//         damage = dmg;
//     }

//     public DamageInstance(int dmg, bool iDef, bool iRes)
//     {
//         damage = dmg;
//         isDef = iDef;
//         isRes = iRes;
//     }
//     public DamageInstance(int dmg, bool iPercentage, int idmgM, bool iDef, bool iRes)
//     {
//         damage = dmg;
//         isDef = iDef;
//         isRes = iRes;
//         isPercentage = iPercentage;
//         damageMax = idmgM;
//     }

//     public DamageInstance(int dmg, bool iPercentage, int idmgM)
//     {
//         damage = dmg;
//         isPercentage = iPercentage;
//         damageMax = idmgM;
//     }
// }
















// /*
// public class exampleEnemy : baseEnemy
// {
//     public bool testpassive_isDefboost = false;

//     public override void takeDamage(damageInstance damage)
//     {
//         //additional damage calc info if needed
//         base.takeDamage(damage);

//         //example passive that doubles defence and resistance when hp is under half at the cost of 25 hp.
//         if (hp < (baseHp / 2) && !testpassive_isDefboost)
//         {
//             def *= 2;
//             res *= 2;
//             base.takeDamage(new damageInstance(25,false,false));
//             testpassive_isDefboost=true;
//         }
//     }
// }*/