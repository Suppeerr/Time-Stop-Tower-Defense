using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagingv2 : MonoBehaviour
{
    private int CurrentLvl = 0;
    string[] sceneNames = {"Level 1", "Level 1", "Level 1"};

    [SerializeField] private List<GameObject> Lvls;
    [SerializeField] private List<GameObject> LvlImages;
    [SerializeField] private GameObject difOverlay;
    [SerializeField] private List<GameObject> difBackgrounds;
    [SerializeField] private List<GameObject> difIcons;
    private AudioSource[] sounds;
    private int levelCount;
    [SerializeField] private GameObject LeftBut;
    [SerializeField] private GameObject RightBut;
    [SerializeField] private GameObject StartBut;
    private bool Selected;
    void Start()
    {
        levelCount = sceneNames.Length;
        sounds = this.GetComponents<AudioSource>();
    }

    private void LoadLvl()
    {
        sounds[1].Play();
        SceneManager.LoadScene(sceneNames[CurrentLvl], LoadSceneMode.Single);
    }

    private void DisplayLevel(int level)
    {
        for (int i = 0; i < levelCount; i++)
        {
            Lvls[i].SetActive(false);
        }
        Lvls[level].SetActive(true);
    }

    private void backgroundColor(int level)
    {
        for (int i = 0; i < levelCount; i++)
        {
            difBackgrounds[i].GetComponent<Image>().color = Lvls[CurrentLvl].GetComponent<Image>().color;
        }
    }

    public void ClickRight()
    {
        CurrentLvl = (CurrentLvl+1) % levelCount;
        DisplayLevel(CurrentLvl);
        sounds[1].Play();
    }

    public void ClickLeft()
    {
        CurrentLvl = (CurrentLvl-1) % levelCount;
        if (CurrentLvl < 0)
            CurrentLvl+=levelCount;
        DisplayLevel(CurrentLvl);
        sounds[0].Play();
    }

    public void ClickStart()
    {
        if (!Selected){
            difOverlay.SetActive(true);
            LeftBut.SetActive(false);
            RightBut.SetActive(false);
            backgroundColor(CurrentLvl);
            LvlImages[CurrentLvl].SetActive(false);
            SelectDiff(1);
            Selected = true;
        } else
        {
            LoadLvl();
        }
    }

    public void ClickBack()
    {
        sounds[0].Play();
        Selected = false;
        difOverlay.SetActive(false);
        LeftBut.SetActive(true);
        RightBut.SetActive(true);
        DisplayLevel(CurrentLvl);
        LvlImages[CurrentLvl].SetActive(true);
    }

    private void SelectDiff(int difficulty)
    {
        sounds[1].Play();
        backgroundColor(CurrentLvl);
        if (difficulty == 0)
        {
            difBackgrounds[0].GetComponent<Image>().color *= 0.8f;
        } else if (difficulty == 1)
        {
            difBackgrounds[1].GetComponent<Image>().color *= 0.8f;   
        } else if (difficulty == 2)
        {
            difBackgrounds[2].GetComponent<Image>().color *= 0.8f;
        }
    }

    public void ChooseEasy()
    {
        GameInstance.levelDifficulty = GameInstance.difficultyType.Easy;
        SelectDiff(0);
    }

    public void ChooseMedium()
    {
        GameInstance.levelDifficulty = GameInstance.difficultyType.Normal;
        SelectDiff(1);
    }

    public void ChooseHard()
    {
        GameInstance.levelDifficulty = GameInstance.difficultyType.Hard;
        SelectDiff(2);
    }
}
