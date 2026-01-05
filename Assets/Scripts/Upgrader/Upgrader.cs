using UnityEngine;
using TMPro;
using System.Collections;

public class Upgrader : MonoBehaviour
{
    private UpgraderState currentState = UpgraderState.Locked;

    private StoredTimeManager storedTimeManagerScript;
    private MoneyManager moneyManagerScript;
    [SerializeField] private TMP_Text upgradeIndicator;
    private RectTransform rt;
    private Clickable clickableScript;
    private OutlineFlash outlineFlashScript;
    private static Upgrader activeUpgrader = null;
    private Coroutine timeOutRoutine = null;

    private Color baseGlowColor;
    private Color baseColor = Color.yellow;
    [SerializeField] private Color invalidGlowColor;
    private Color invalidColor = Color.red;

    // Upgrade data
    [SerializeField] private UpgradeDataContainer dataContainer;
    private UpgraderType upgraderType;
    private UpgradeType currentUpgrade;
    private int currentMoneyCost;
    private int currentSecondsCost;
    private string baseText;
    private int upgradeNum = 0;

    private int currentMoney;
    private int currentSeconds;
    private float elapsed;    

    private int lastCam = -1;

    public static bool IsLocked { get; private set; } = false;

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
        storedTimeManagerScript = GameObject.Find("Stored Time Manager")?.GetComponent<StoredTimeManager>();
        moneyManagerScript = GameObject.Find("Money Manager")?.GetComponent<MoneyManager>();

        clickableScript = this.GetComponent<Clickable>();
        outlineFlashScript = this.GetComponent<OutlineFlash>();

        rt = upgradeIndicator.GetComponent<RectTransform>();
        upgradeIndicator.color = baseColor;
        Material mat = upgradeIndicator.fontMaterial;
        baseGlowColor = mat.GetColor("_GlowColor");

        SetCurrentUpgrade();
    }

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

    private void EnterState(UpgraderState state)
    {
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
                outlineFlashScript.StopFlashing(false);
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

    private void ExitState(UpgraderState state)
    {
        if (timeOutRoutine != null)
        {
            StopCoroutine(timeOutRoutine);
        }
    }

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
        if (ProjectileManager.Instance.IsFrozen || !clickableScript.ClickableEnabled || BaseHealthManager.IsGameOver)
        {
            return;
        }

        if (timeOutRoutine != null)
        {
            StopCoroutine(timeOutRoutine);
        }

        CheckUpgrade();
    }

    private void CheckUpgrade()
    {
        if (upgraderType == UpgraderType.AutoCannon && TowerManager.Instance.GetTowerCount() < 3)
        {
            return;
        }

        RequestFocus();

        if (currentState == UpgraderState.Blinking || currentState == UpgraderState.Hidden)
        {
            SetState(UpgraderState.Clicked);
            return;
        }

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
            StartCoroutine(FlashRed(0.4f));
            timeOutRoutine = StartCoroutine(TimeOut(5f));
        }
    }

    private bool CheckUpgradability()
    {
        currentMoney = moneyManagerScript.GetMoney();
        currentSeconds = storedTimeManagerScript.GetSeconds();

        return currentMoney >= currentMoneyCost && currentSeconds >= currentSecondsCost;
    }

    private IEnumerator SuccessfulUpgrade()
    {
        IsLocked = true;
        MarkUpgradeBought();
        upgradeNum++;

        AdjustFontSize(1.2f, 1.4f);
        upgradeIndicator.text = "Upgrade\nBought!";
        
        moneyManagerScript.UpdateMoney(currentMoneyCost, true);
        storedTimeManagerScript.UpdateSeconds(currentSecondsCost, true);

        yield return new WaitForSeconds(3f); 

        IsLocked = false;
        if (upgradeNum >= dataContainer.upgrades.Length)
        {
            SetState(UpgraderState.Finished);
            yield break;
        }

        SetState(UpgraderState.UnlockingUpgrade);
    }

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

    private void MarkUpgradeBought()
    {
        UpgradeManager.Instance.BuyUpgrade(currentUpgrade);
    }

    private IEnumerator TimeOut(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        
        SetState(UpgraderState.Hidden);
    }

    private IEnumerator FlashRed(float duration)
    {
        // Get the font material instance
        Material mat = upgradeIndicator.fontMaterial;

        // Set color and glow to red 
        upgradeIndicator.color = invalidColor;
        mat.SetColor("_FaceColor", invalidColor);
        mat.SetColor("_GlowColor", invalidGlowColor);

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Revert back to original color and glow
        upgradeIndicator.color = baseColor;
        mat.SetColor("_FaceColor", baseColor);
        mat.SetColor("_GlowColor", baseGlowColor);
    }

    private void RequestFocus()
    {
        if (activeUpgrader == this)
        {
            return;
        }

        if (activeUpgrader != null)
        {
            var state = activeUpgrader.currentState;

            if (IsLocked || state == UpgraderState.Finished)
            {
                return;
            }

            activeUpgrader.SetState(UpgraderState.Hidden);
        }

        activeUpgrader = this;
    }

    void Update()
    {
        ManageOutlineFlash();

        AdjustUI();
    }

    private void ManageOutlineFlash()
    {
        if (ProjectileManager.Instance.IsFrozen || 
            (IsLocked && activeUpgrader != this) ||
            (clickableScript.ClickableEnabled && (currentState == UpgraderState.Locked || currentState == UpgraderState.Finished)))
        {
            UpdateVisuals(false, false, false);
            SetState(UpgraderState.Locked);
        }

        if (currentState != UpgraderState.Locked && currentState != UpgraderState.Hidden && currentState != UpgraderState.Blinking)
        {
            return;
        }

        if (CheckUpgradability() && TowerManager.Instance.GetTowerCount() >= 3)
        { 
            SetState(UpgraderState.Blinking);
        }
        else if (clickableScript.ClickableEnabled && currentState == UpgraderState.Blinking)
        {
            SetState(UpgraderState.Hidden);   
        }
    }

    private void UpdateVisuals(bool visible, bool indicating, bool clickable = true)
    {
        clickableScript.UpdateClickable(visible, clickable);
        upgradeIndicator.enabled = indicating;
    }

    private void AdjustUI()
    {
        int cam = CameraSwitch.ActiveCam;
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

    private void SetVisualParameters(float x, float y, float z)
    {
        rt.localPosition = new Vector3(x, y, z);
    }
}