using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UI;
using Unity.VisualScripting;

public class EnemyEffectHandler
{
    public List<BaseEffect> effectList = new List<BaseEffect>();
    public List<BaseEffect> effectRemovalList = new List<BaseEffect>();

    public bool recieveEffect(BaseEffect eff)
    {
        if (eff.triggerChance < 1 && Random.Range(0, 1) <= eff.triggerChance) {
            eff.on_apply_fail();
            return false;
        }
        //on stackable stuff here

        BaseEffect cmpr;
        if (eff.is_unique && (cmpr = effectList.Find(x => x.GetType().Name == eff.GetType().Name)) != null ? true : false)
        {
            if (!eff.if_unique_is_replace(cmpr))
            {
                return false;
            }
            _removeEffect(cmpr);
        }
        effectList.Add(eff);
        eff.on_apply();
        return true;
    }

    public void updateEffects()
    {
        foreach (BaseEffect effect in effectList)
        {
            //update effects

            effect.s_effect_update();
            if (effect.is_finished) effectRemovalList.Add(effect);
        }
        _removeEffects();
    }

    private void _removeEffects()
    {
        foreach (BaseEffect effect in effectRemovalList) _removeEffect(effect); //could just loop through effect list for is_finished
    }

    private void _removeEffect(BaseEffect effect)
    {
        if (effectList.Remove(effect) == false) Debug.Log("Failed to remove effect: " + effect.GetType().Name);
        else effect.on_remove();

    }


    //-
    //external effect removal calls the check if the effect is clearable
    public void clearEffects(List<BaseEffect> efrmv)
    {
        foreach (BaseEffect effect in efrmv) clearEffect(effect);
    }
    public void clearEffect(BaseEffect effect){
        if (effect.is_clearable) _removeEffect(effect);
    }





}

public class BaseEffect
{
    //setup variables
    public bool is_clearable = true; //can the effect be cleared early (passives etc)
    public bool is_stackable = true; //if the effects merges with each other to form one combined effect instance
    public bool is_unique = true; //only one copy of the effect may exist, may be ignored if stackable

    public BaseEffect[] unique_lockout; //other Effects that lock out this effect application
    public BaseEffect[] stack_additional; //other Effects that can stack with this effect

    public float duration = 10.0f; //effect base duration
    public float triggerChance = 1.0f; //effect application chance
    //stack?


    //function variables
    public float duration_remaining;
    public bool is_finished = false;

    public BaseEffect()
    {
        duration_remaining = duration;
    }

    public void s_effect_update()
    {
        //stuff

        duration_remaining -= Time.deltaTime;
        if (duration_remaining < 0) is_finished = true;
        on_update();
    }


    //overrides
    protected virtual void on_update() //on every update cycle
    {

    }
    public virtual void on_apply() //when the effect is applied
    {

    }
    public virtual void on_apply_fail() //when the effect fails the trigger chance
    {

    }
    public virtual void on_remove() //when the effect is removed
    {

    }
    public virtual void on_clear() //when the effect is forcibly cleared
    {

    }
    public virtual void on_unit_death() //when the unit this effect is attached to dies
    {

    }

    /// <summary>
    /// Function that specifies how the applied effect instance should be stacked with the existing target
    /// </summary>
    /// <param name="targ"> The target effect to stack to</param> 
    /// <returns></returns>
    public virtual void on_stack(BaseEffect targ)
    {
        //default stack behavior adds duration of applied effect to existing effect
        targ.duration += duration_remaining;
    }

    /// <summary>
    /// Function that evaluates which instance should be replaced if the effect is unique and another instance already exists on target
    /// </summary>
    /// <param name="cmpr"> The existing effect to be compared</param> 
    /// <returns>True - replace existing instance with self
    /// <br/>False - keep existing instance</returns>
    public virtual bool if_unique_is_replace(BaseEffect cmpr)
    {
        //default comparison to replace if duration is greater
        return cmpr.duration_remaining <= this.duration_remaining;
    }
    public virtual void on_replace() //if the effect is replaced with another
    {

    }
}
