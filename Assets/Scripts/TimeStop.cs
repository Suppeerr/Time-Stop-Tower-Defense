using UnityEngine;
using System;
using TMPro;

public class TimeStop : MonoBehaviour
{
    public static event Action<bool> timeStop;
    public TMP_Text durationText;
    private Color activeColor = Color.yellow; 
    private Color inactiveColor = Color.gray;
    public float duration = 5f;
    public float maxDur = 5f;
    public float rechargeRate = 1f;
    public float waitTime = 1f;
    private bool active = false;


    // Once timestop is triggered, all animated objects freeze for the duration
    void Update()
    {
        // Triggers timestop
        if (!active && Input.GetKeyDown(KeyCode.T) && duration > waitTime)
        {
            active = true;
            Debug.Log("Time has been stopped.");
            timeStop?.Invoke(true);

            if (duration < maxDur)
            {
                duration += Time.deltaTime;
            }
        }

        // Countdown when timestop is active
        if (active)
        {
            duration -= Time.deltaTime;
            if (duration < 0f)
            {
                duration = 0f;
            }
            if (duration <= 0f)
            {
                active = false;
                Debug.Log("Time has returned to normal.");
                timeStop?.Invoke(false);
            }
        }
        else
        {
            // Recharge duration when inactive
            if (duration < maxDur)
            {
                duration += rechargeRate * Time.deltaTime;
                if (duration > maxDur)
                {
                    duration = maxDur;
                }
            }
        }

        // Update UI
        if (durationText != null)
        {
            durationText.text = duration.ToString("F2");
            if (active)
            {
                durationText.color = activeColor;
                durationText.gameObject.SetActive(true);
            }
            else if (duration < maxDur)
            {
                durationText.color = inactiveColor;
                durationText.gameObject.SetActive(true);
            }
            else
            {
                durationText.gameObject.SetActive(false);
            }
        }
    }
}
