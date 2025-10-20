using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay and Mechanics"){
            Debug.Log("gameInst started");
            clevel = new GameObject();
            clevel.AddComponent<LevelInstance>();
        }
    }

}