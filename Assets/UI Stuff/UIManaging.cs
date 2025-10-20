using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManaging : MonoBehaviour
{   
    /*
    private VisualElement[] levels;
    private Button[] rights;
    private Button[] lefts;
    */

    private int CurrentLvl = 0;
    private List<VisualElement> Lvls = new List<VisualElement>();
    private int levelCount;
    private List<string> sceneNames = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        string[] levelNames = {"Sandcat", "Volcano", "Dog"};
        string[] temp = {"Level 1 Map", "Money System", "Gameplay and Mechanics"};
        levelCount = levelNames.Length;
        //List<VisualElement> Lvls = new List<VisualElement>();
        List<Button> ButR = root.Query<Button>("Right").ToList();
        List<Button> ButL = root.Query<Button>("Left").ToList();
        List<Button> ButPlay = root.Query<Button>("Play").ToList();
        for (int i = 0; i < levelCount; i++){
            sceneNames.Add(temp[i]);
            Lvls.Add(root.Query(levelNames[i]));
            ButR[i].RegisterCallback<ClickEvent>(ClickRight);
            ButL[i].RegisterCallback<ClickEvent>(ClickLeft);
            ButPlay[i].RegisterCallback<ClickEvent>(ClickPlay);
            //SceneManager.UnloadSceneAsync(sceneNames[i]);
        }
        /*
        for (int i = 0; i < 3; i++){
            List<Button> ButR = root.Query("Right").ToList();
            List<Button> ButL = root.Query("Left").ToList();
            levels[i] = root.Q<VisualElement>(levelNames[i]);
            rights[i] = levels[i].Q<VisualElement>("LevelSelect").Q<Button>("Right");
            rights[i] = levels[i].Q<VisualElement>("LevelSelect").Q<Button>("Right");
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClickRight(ClickEvent clickedR){
        Debug.Log("Clicked Right");
        CurrentLvl = (CurrentLvl+1) % levelCount;
        Debug.Log(CurrentLvl);
        DisplayLvl(CurrentLvl);
    }

    private void ClickLeft(ClickEvent clickedL){
        Debug.Log("Clicked Left");
        CurrentLvl = (CurrentLvl-1) % levelCount;
        if (CurrentLvl < 0)
            CurrentLvl+=levelCount;
        Debug.Log(CurrentLvl);
        DisplayLvl(CurrentLvl);
    }

    private void ClickPlay(ClickEvent clickedP){
        LoadScene(CurrentLvl);
    }

    private void DisplayLvl(int LvlNum){
        for (int i = 0; i < levelCount; i++){
            Lvls[i].visible = false;
        }
        Lvls[LvlNum].visible = true;
    }

    private void LoadScene(int LvlNum){
        /*
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneNames[LvlNum]);
        //loading.allowSceneActivation = false;
        Debug.Log("Clicked");
        while(!loading.isDone)
            yield return null;
        Debug.Log("Clicked2");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneNames[LvlNum]));
        Debug.Log("Active Scene:" + SceneManager.GetActiveScene().name);
        loading.allowSceneActivation = true;
        */
        SceneManager.LoadScene(sceneNames[LvlNum], LoadSceneMode.Single);
    }
}
