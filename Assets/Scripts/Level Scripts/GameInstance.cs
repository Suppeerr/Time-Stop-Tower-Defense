using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor;

public static class GameInstance
{
    static LevelInstance currentLevel;
    static GameObject clevel;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    /*
    static void LStart()
    {
        Debug.Log("gameInst started");
        clevel = new GameObject();
        clevel.AddComponent<LevelInstance>();
        //currentLevel = ScriptableObject.CreateInstance<levelInstance>();
        //currentLevel = new levelInstance();
    }
    */
    
    static void LStart()
    {
        Debug.Log("gameInst started");
        SceneManager.sceneLoaded += OnSceneLoaded;
        //List<BaseEffect> effectList = new List<BaseEffect>();
        //BaseEffect eff = new BaseEffect();
        //effectList.Add(new BaseEffect());
        //BaseEffect y;
        //if (true && (y = effectList.Find(x => x.GetType().Name == eff.GetType().Name)) != null ? true : false) Debug.Log("p");
        //Debug.Log(y);
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay and Mechanics" || scene.name == "Level 1"){
            clevel = new GameObject();
            clevel.AddComponent<LevelInstance>();
        }
    }

    public enum difficultyType
    {
        Easy,
        Normal,
        Hard
    };

    public static difficultyType levelDifficulty = difficultyType.Normal;

    static void loadscene()
    {
        //check if scene name is a valid level
        //if scene name is valid, load new gameInstance.
    }
}