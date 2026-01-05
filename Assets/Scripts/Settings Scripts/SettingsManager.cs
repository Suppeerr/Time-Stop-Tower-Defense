using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsMenu;
    private bool menuOpen;
    private float cooldown;
    private float elapsed;
     

    void Update()
    {
        if (menuOpen)
        {
            settingsMenu.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(false);
        }
    }
    
    public void HandleClick()
    {
        Debug.Log("Settings Button Clicked!");
        menuOpen = !menuOpen;
    }

    public void STARTPAUSING(){
        if (Time.timeScale == 0){
            Time.timeScale = 1;
        } else {
            Time.timeScale = 0;
        }
    }
}
