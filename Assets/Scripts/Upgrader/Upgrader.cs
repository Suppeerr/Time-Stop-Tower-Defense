using UnityEngine;
using TMPro;
using System.Collections;

public class Upgrader : MonoBehaviour
{
    private StoredTimeManager storedTimeManagerScript;
    private MoneyManager moneyManagerScript;
    private TMP_Text upgradeIndicator;

    private Color baseGlowColor;
    private Color baseColor = Color.yellow;
    [SerializeField] private Color invalidGlowColor;
    private Color invalidColor = Color.red;

    [SerializeField] private bool isAutoCannon;
    private int numberOfClicks = 0;
    private bool upgraderFinished = false;

    public static bool AutoCannonBought { get; private set; }
    public static bool PreChargeBought { get; private set; }
    public static bool TowerBoostBought { get; private set; }
    public static bool MultiChargeBought { get; private set; }


    void Start()
    {
        storedTimeManagerScript = GameObject.Find("Stored Time Manager")?.GetComponent<StoredTimeManager>();
        moneyManagerScript = GameObject.Find("Money Manager")?.GetComponent<MoneyManager>();
        upgradeIndicator = GetComponentInChildren<TMP_Text>();

        upgradeIndicator.color = baseColor;
        Material mat = upgradeIndicator.fontMaterial;
        baseGlowColor = mat.GetColor("_GlowColor");
        
        if (isAutoCannon)
        {
            upgradeIndicator.text = "Enable Auto Cannon   8 Coins";
        }
        else
        {
            upgradeIndicator.text = "Pre-Charge Ability   5 Coins, 10s";
        }
        
        upgradeIndicator.enabled = false;
    }

    private void OnMouseDown()
    {
        numberOfClicks++;
        if (ProjectileManager.IsFrozen || upgraderFinished || BaseHealthManager.IsGameOver)
        {
            return;
        }

        int money = moneyManagerScript.GetMoney();
        int seconds = storedTimeManagerScript.GetSeconds();
        upgradeIndicator.enabled = true;
        
        // Auto Cannon Upgrade
        if (isAutoCannon)
        {
            if (money >= 8)
            {
                if (numberOfClicks == 2)
                {
                    StartCoroutine(ConfirmUpgrade(upgradeIndicator.text));
                    return;
                }

                if (numberOfClicks == 3)
                {
                    StopAllCoroutines();
                    StartCoroutine(SuccessfulUpgrade(8, 0));
                    FinishUpgrader("Ignore Raycast");
                    AutoCannonBought = true;
                    return;
                }
                
            }
            else if (numberOfClicks > 1)
            {
                StartCoroutine(FlashRed(0.4f));
            }
            StartCoroutine(WaitAndDisappear(5f));
            return;
        }

        if (!isAutoCannon && seconds >= 10)
        {
            StartCoroutine(SuccessfulUpgrade(0, 10));
        }
        else
        {
            StartCoroutine(FlashRed(0.4f));
        }
    }

    private IEnumerator ConfirmUpgrade(string regularText)
    {
        upgradeIndicator.text = "Confirm?";

        yield return new WaitForSeconds(5f);
        upgradeIndicator.text = regularText;
        numberOfClicks = 0;
    }

    private IEnumerator SuccessfulUpgrade(int money, int seconds)
    {
        upgradeIndicator.text = "Upgrade brought!";
        moneyManagerScript.DecreaseMoney(money);
        storedTimeManagerScript.DecreaseSeconds(seconds);

        StartCoroutine(WaitAndDisappear(2f));
        yield return null;
    }

    private void FinishUpgrader(string finishedLayer)
    {
        upgraderFinished = true;
        gameObject.layer = LayerMask.NameToLayer(finishedLayer);
    }

    private IEnumerator FlashRed(float duration)
    {
        // Get the font material instance
        Material mat = upgradeIndicator.fontMaterial;

        // Set color and glow to red 
        upgradeIndicator.color = invalidColor;
        mat.SetColor("_GlowColor", invalidGlowColor);

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Revert back to original color and glow
        upgradeIndicator.color = baseColor;
        mat.SetColor("_GlowColor", baseGlowColor);
    }

    private IEnumerator WaitAndDisappear(float disappearAfter)
    {
        yield return new WaitForSeconds(disappearAfter);
        upgradeIndicator.enabled = false;
    }
}   
