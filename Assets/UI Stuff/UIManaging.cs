using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        string[] levelNames = {"Sandcat", "Volcano", "Dog"};
        levelCount = levelNames.Length;
        //List<VisualElement> Lvls = new List<VisualElement>();
        List<Button> ButR = root.Query<Button>("Right").ToList();
        List<Button> ButL = root.Query<Button>("Left").ToList();
        for (int i = 0; i < levelCount; i++){
            Lvls.Add(root.Query(levelNames[i]));
            ButR[i].RegisterCallback<ClickEvent>(ClickRight);
            ButL[i].RegisterCallback<ClickEvent>(ClickLeft);
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

    private void DisplayLvl(int LvlNum){
        for (int i = 0; i < levelCount; i++){
            Lvls[i].visible = false;
        }
        Lvls[LvlNum].visible = true;
    }
}
