using UnityEngine;
using System.Collections;

public class EnemyObject : MonoBehaviour
{
    // Enemy field
    public BaseEnemy enemy;

    // Enemy slowing fields
    public bool IsEnemySlowed { get; private set; } = false;

    public void Init(BaseEnemy enemy)
    {
        this.enemy = enemy;
    }

    // Slows the enemy by a specified percentage
    public bool SlowEnemy(float slowPercent, float slowDur)
    {
        if (IsEnemySlowed)
        {
            return false;
        }
        StartCoroutine(SlowRoutine(slowPercent, slowDur));
        return true;
    }

    private IEnumerator SlowRoutine(float slowPercent, float slowDur)
    {
        enemy.SetSpeedMultiplier(1f - slowPercent);
        IsEnemySlowed = true;

        yield return new WaitForSeconds(slowDur);

        enemy.SetSpeedMultiplier(1f);
        IsEnemySlowed = false;
    }

    //game object specifics go here, such as animations or hitboxes
}

// Defines enemy traits such as defense, resistance, and damage instances
public partial class DamageInstance
{
    public int damage;

    public bool isDef = true;
    public bool isRes = true;
    public bool isPercentage = false;
    public int damageMax = -1;

    public DamageInstance(int dmg)
    {
        damage = dmg;
    }

    public DamageInstance(int dmg, bool iDef, bool iRes)
    {
        damage = dmg;
        isDef = iDef;
        isRes = iRes;
    }
    public DamageInstance(int dmg, bool iPercentage, int idmgM, bool iDef, bool iRes)
    {
        damage = dmg;
        isDef = iDef;
        isRes = iRes;
        isPercentage = iPercentage;
        damageMax = idmgM;
    }

    public DamageInstance(int dmg, bool iPercentage, int idmgM)
    {
        damage = dmg;
        isPercentage = iPercentage;
        damageMax = idmgM;
    }
}