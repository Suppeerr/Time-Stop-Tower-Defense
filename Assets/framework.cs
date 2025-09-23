using UnityEngine;

//reference numbers: enemy hp 100~100000, projectile damage 25~x
public abstract partial class baseEnemy
{
    //base stats:
    public int baseHp; 
    public int baseDef;

    public int hp;
    public int def;

    public abstract void OnSpawn();
    public abstract void OnDestroy();
    public abstract void OnDie();

    public virtual void takeDamage(float damage)
    {
        //direct cast rounds down to 0
        hp = (int)Mathf.Clamp(damage - def, damage * 0.05f, damage);
        if (hp < 0) OnDie();
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

    public override void takeDamage(float damage)
    {
        //additional damage calc info if needed
        base.takeDamage(damage);

        //example passive that doubles defence when hp is under half
        if (hp < (baseHp / 2) && !testpassive_isDefboost)
        {
            def *= 2;
            testpassive_isDefboost=true;
        }
    }
}