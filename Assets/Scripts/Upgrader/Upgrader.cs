using UnityEngine;
using TMPro;
using System.Collections;

public class Upgrader : MonoBehaviour
{
    // Upgrade data container
    [SerializeField] private UpgradeDataContainer dataContainer;

    // Currently available upgrade and its upgrader
    private UpgradeType currentUpgrade;
    private static Upgrader activeUpgrader = null;  

    // Current state and type of the active upgrader
    private UpgraderState currentState = UpgraderState.Locked;
    private UpgraderType upgraderType;    

    // Current upgrade number in the active upgrader's upgrade chain
    private int upgradeNum = 0;

    // Clickable and outline flash scripts
    private Clickable clickableScript;
    private OutlineFlash outlineFlashScript;

    // Upgrader UI fields
    [SerializeField] private TMP_Text upgradeIndicator;
    private string baseText;
    private Color baseGlowColor;
    private Color baseColor = Color.yellow;
    [SerializeField] private Color invalidGlowColor;
    private Color invalidColor = Color.red;

    // Upgrade indicator rect transform
    private RectTransform rt;
    
    // Upgrade indicator time out coroutine
    private Coroutine timeOutRoutine = null;

    // Coin and second costs of the currently available upgrade
    private int currentMoneyCost;
    private int currentSecondsCost;
    
    // Currently stored coin and second amounts
    private int currentMoney;
    private int currentSeconds;

    // Previously active camera
    private int lastCam = -1;

    // Static boolean that disables all upgrade interactions during text animations
    public static bool IsLocked { get; private set; } = false;

    // Defines the upgrader's states
    public enum UpgraderState
    {
        Locked,
        Blinking,
        Hidden,
        Clicked,
        Confirming,
        UpgradeBought,
        UnlockingUpgrade,
        Finished
    }

    void Start()
    {
        // Initializes fields
        clickableScript = this.GetComponent<Clickable>();
        outlineFlashScript = this.GetComponent<OutlineFlash>();

        rt = upgradeIndicator.GetComponent<RectTransform>();
        upgradeIndicator.color = baseColor;
        Material mat = upgradeIndicator.fontMaterial;
        baseGlowColor = mat.GetColor("_GlowColor");

        SetCurrentUpgrade();
    }

    // Sets the upgrader's state to a new state
    private void SetState(UpgraderState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        // Debug.Log("Current state: " + currentState);

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }

    // Exits the upgrader's old state
    private void ExitState(UpgraderState state)
    {
        if (timeOutRoutine != null)
        {
            StopCoroutine(timeOutRoutine);
        }
    }

    // Enters the upgrader's new state 
    private void EnterState(UpgraderState state)
    {
        if (state != UpgraderState.Blinking)
        {
            outlineFlashScript.StopFlashing(false);
        }

        switch (state)
        {
            case UpgraderState.Locked:
                UpdateVisuals(false, false, false);
                break;

            case UpgraderState.Blinking:
                outlineFlashScript.StartFlashing();
                UpdateVisuals(false, false);
                break;

            case UpgraderState.Hidden:
                UpdateVisuals(false, false);
                break;

            case UpgraderState.Clicked:
                upgradeIndicator.text = baseText;
                if (currentUpgrade == UpgradeType.MultiCharge)
                {
                    AdjustFontSize(1f, 1.3f);
                }
                else
                {
                    AdjustFontSize(1f, 1.5f);
                }      
                UpdateVisuals(true, true);
                timeOutRoutine = StartCoroutine(TimeOut(5f));
                break;

            case UpgraderState.Confirming:
                UpdateVisuals(true, true);
                upgradeIndicator.text = "Confirm?";
                AdjustFontSize(1.4f, 1.5f);
                timeOutRoutine = StartCoroutine(TimeOut(5f));
                break;

            case UpgraderState.UpgradeBought:
                UpdateVisuals(false, true, false);
                StartCoroutine(SuccessfulUpgrade());
                break;

            case UpgraderState.UnlockingUpgrade:
                UpdateVisuals(false, true, false);
                StartCoroutine(UnlockNewUpgrade());
                break;

            case UpgraderState.Finished:
                UpdateVisuals(false, false, false);
                currentMoneyCost = 9999999;
                currentSecondsCost = 9999999;
                break;
        }
    }

    // Updates upgrade fields to reflect the currently available upgrade
    private void SetCurrentUpgrade()
    {
        if (upgradeNum >= dataContainer.upgrades.Length)
        {
            SetState(UpgraderState.Finished);
            return;
        }

        UpgradeData upgrade = dataContainer.upgrades[upgradeNum];
        upgraderType = upgrade.upgraderType;
        currentUpgrade = upgrade.upgradeType;
        currentMoneyCost = upgrade.moneyCost;
        currentSecondsCost = upgrade.secondsCost;
        baseText = upgrade.text;
    }

    void OnMouseDown()
    {
        if (TimeStop.Instance.IsFrozen || !clickableScript.ClickableEnabled || BaseHealthManager.Instance.IsGameOver)
        {
            return;
        }

        // Resets the upgrader's time out timer
        if (timeOutRoutine != null)
        {
            StopCoroutine(timeOutRoutine);
        }

        CheckUpgrade();
    }

    // Checks to see whether an upgrade can be purchased when the upgrader is clicked
    private void CheckUpgrade()
    {
        if (TowerManager.Instance.GetTowerCount() < 3)
        {
            return;
        }

        RequestFocus();

        // Updates the active upgrader's state to clicked
        if (currentState == UpgraderState.Blinking || currentState == UpgraderState.Hidden)
        {
            SetState(UpgraderState.Clicked);
            return;
        }

        // If the upgrade can be purchased, either confirm the purchase or buy the upgrade
        if (CheckUpgradability())
        {
            switch (currentState)
            {    
                case UpgraderState.Clicked:
                    SetState(UpgraderState.Confirming);
                    break;

                case UpgraderState.Confirming:
                    SetState(UpgraderState.UpgradeBought);
                    break;
            }
        }
        else
        {
            // If the upgrade cannot be purchased, briefly flash the indicator red 
            StartCoroutine(FlashRed(0.4f));
            timeOutRoutine = StartCoroutine(TimeOut(5f));
        }
    }

    // Checks whether the conditions for an upgrade to be purchased are met
    private bool CheckUpgradability()
    {
        currentMoney = MoneyManager.Instance.GetMoney();
        currentSeconds = StoredTimeManager.Instance.GetSeconds();

        return currentMoney >= currentMoneyCost && currentSeconds >= currentSecondsCost;
    }

    // Requests to make the most recently clicked upgrader the active upgrader
    private void RequestFocus()
    {
        if (activeUpgrader == this)
        {
            if (currentState == UpgraderState.Finished)
            {
                activeUpgrader = null;
            }

            return;
        }

        if (activeUpgrader != null)
        {
            activeUpgrader.SetState(UpgraderState.Hidden);
        }

        activeUpgrader = this;
    }

    // Runs when an upgrade is successfully purchased
    private IEnumerator SuccessfulUpgrade()
    {
        IsLocked = true;
        MarkUpgradeBought();
        upgradeNum++;

        AdjustFontSize(1.2f, 1.4f);
        upgradeIndicator.text = "Upgrade\nBought!";
        
        MoneyManager.Instance.UpdateMoney(currentMoneyCost, true);
        StoredTimeManager.Instance.UpdateSeconds(currentSecondsCost, true);

        yield return new WaitForSeconds(3f); 

        IsLocked = false;
        if (upgradeNum >= dataContainer.upgrades.Length)
        {
            SetState(UpgraderState.Finished);
            yield break;
        }

        SetState(UpgraderState.UnlockingUpgrade);
    }

    // Unlocks the next upgrade in the currently active upgrader's upgrade chain
    private IEnumerator UnlockNewUpgrade()
    {
        IsLocked = true;
        AdjustFontSize(1f, 1.4f);
        upgradeIndicator.text = "New\nUpgrade\nUnlocked!";

        yield return new WaitForSeconds(3f);

        SetCurrentUpgrade();
        IsLocked = false;
        
        SetState(UpgraderState.Hidden);
    }

    // Marks an upgrade as purchased so its effects can be activated
    private void MarkUpgradeBought()
    {
        UpgradeManager.Instance.BuyUpgrade(currentUpgrade);
    }

    // Times out an upgrader's indicator after some time
    private IEnumerator TimeOut(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        
        SetState(UpgraderState.Hidden);
    }

    // Flashes an upgrader's indicator red if the upgrade cannot be purchased
    private IEnumerator FlashRed(float duration)
    {
        Material mat = upgradeIndicator.fontMaterial;

        // Set color and glow to red 
        upgradeIndicator.color = invalidColor;
        mat.SetColor("_FaceColor", invalidColor);
        mat.SetColor("_GlowColor", invalidGlowColor);

        yield return new WaitForSeconds(duration);

        // Revert back to original color and glow
        upgradeIndicator.color = baseColor;
        mat.SetColor("_FaceColor", baseColor);
        mat.SetColor("_GlowColor", baseGlowColor);
    }

    void Update()
    {
        ManageOutlineFlash();

        AdjustUI();
    }

    // Flashes an upgrader's outline whenever its upgrade can be purchased
    private void ManageOutlineFlash()
    {
        if (TimeStop.Instance.IsFrozen || 
            (IsLocked && activeUpgrader != this) ||
            (clickableScript.ClickableEnabled && (currentState == UpgraderState.Locked || currentState == UpgraderState.Finished)))
        {
            if (currentState != UpgraderState.Locked)
            {
                SetState(UpgraderState.Locked);
            }
            else
            {
                UpdateVisuals(false, false, false);
            }
            
            return;
        }

        if (currentState != UpgraderState.Locked && currentState != UpgraderState.Hidden && currentState != UpgraderState.Blinking)
        {
            return;
        }

        if (CheckUpgradability() && TowerManager.Instance.GetTowerCount() >= 3)
        { 
            if (currentState != UpgraderState.Blinking)
            {
                SetState(UpgraderState.Blinking);
            }
        }
        else 
        {
            if (currentState == UpgraderState.Blinking)
            {
                SetState(UpgraderState.Hidden);   
            }
        }
    }

    // Updates an upgrade indicator's visibility and an upgrader's interactability/outline visibility 
    private void UpdateVisuals(bool visible, bool indicating, bool clickable = true)
    {
        clickableScript.UpdateClickable(visible, clickable);
        upgradeIndicator.enabled = indicating;
    }

    // Adjusts an upgrade indicator's text position in response to camera changes
    private void AdjustUI()
    {
        int cam = CameraSwitcher.Instance.ActiveCam;
        if (upgraderType == UpgraderType.AutoCannon && cam != lastCam)
        {
            if (cam == 1)
            {
                SetVisualParameters(0f, 0f, 0f);
            }
            else if (cam == 2)
            {
                SetVisualParameters(-0.12f, 0f, -150f);
            }
            else
            {
                SetVisualParameters(1f, 0f, -4f);
            }

            lastCam = cam;
        }
    }

    // Adjusts an upgrade indicator's text font size 
    private void AdjustFontSize(float acSize, float tsSize)
    {
        switch (upgraderType)
        {
            case UpgraderType.AutoCannon:
                upgradeIndicator.fontSize = acSize;
                break;
            
            case UpgraderType.TimeStop:
                upgradeIndicator.fontSize = tsSize;
                break;
        }
    }

    // Sets the position of an upgrade indicator to specified coordinates 
    private void SetVisualParameters(float x, float y, float z)
    {
        rt.localPosition = new Vector3(x, y, z);
    }
}