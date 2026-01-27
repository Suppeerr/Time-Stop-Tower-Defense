using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManagingv2 : MonoBehaviour
{
    private int CurrentLvl = 0;
    string[] sceneNames = {"Level 1", "Level 1", "Level 1"};
    [SerializeField] private List<GameObject> Lvls;
    private AudioSource[] sounds;
    private int levelCount;
    void Start()
    {
        levelCount = sceneNames.Length;
        sounds = this.GetComponents<AudioSource>();
    }

    public void LoadLvl()
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
}
