using UnityEngine;

public class enemyObject : MonoBehaviour
{
    BaseEnemy s_edatastructure;

    //game object specifics go here, such as animations or hitboxes
}
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