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
    [SerializeField] private UpgraderType upgraderType;
    private UpgradeType currentUpgrade;
    private int currentMoneyCost;
    private int currentSecondsCost;
    private string baseText;
    private int upgradeNum;

    private int currentMoney;
    private int currentSeconds;
    private float elapsed;    

    public static bool AutoCannonBought { get; private set; }
    public static bool PreChargeBought { get; private set; }
    public static bool TowerBoostBought { get; private set; }
    public static bool MultiChargeBought { get; private set; }

    private int lastCam = -1;

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
        
        switch(upgraderType)
        {
            case UpgraderType.AutoCannon:
                SetCurrentUpgrade(UpgradeType.AutoCannon);
                break;

            case UpgraderType.TimeStop:
                SetCurrentUpgrade(UpgradeType.PreCharge);
                break;
        }
    }

    private void SetState(UpgraderState newState)
    {
        if (currentState == newState)
        {
            return;
        }

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
                AdjustFontSize(1.4f, 1.8f);
                timeOutRoutine = StartCoroutine(TimeOut(5f));
                break;

            case UpgraderState.UpgradeBought:
                rt.sizeDelta = new Vector2(10, rt.sizeDelta.y);
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

    private void SetCurrentUpgrade(UpgradeType type)
    {
        UpgradeData data = dataContainer.GetData(type);
        currentUpgrade = type;
        upgradeNum = data.upgradeNumber;
        currentMoneyCost = data.moneyCost;
        currentSecondsCost = data.secondsCost;
        baseText = data.text;
    }

    void OnMouseDown()
    {
        if (ProjectileManager.IsFrozen || !clickableScript.ClickableEnabled || BaseHealthManager.IsGameOver)
        {
            return;
        }

        if (timeOutRoutine != null)
        {
            StopCoroutine(timeOutRoutine);
        }

        Debug.Log("Current state: " + currentState);

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
        MarkUpgradeBought();

        AdjustFontSize(1f, 1.5f);
        upgradeIndicator.text = "Upgrade Bought!";
        
        moneyManagerScript.DecreaseMoney(currentMoneyCost);
        storedTimeManagerScript.DecreaseSeconds(currentSecondsCost);

        yield return new WaitForSeconds(3f); 

        if (upgraderType == UpgraderType.AutoCannon)
        {
            if (upgradeNum == 1)
            {
                SetState(UpgraderState.Finished);
                yield break;
            }
        }
        else if (upgraderType == UpgraderType.TimeStop)
        {
            if (upgradeNum == 2)
            {
                SetState(UpgraderState.Finished);
                yield break;
            }
        }

        SetState(UpgraderState.UnlockingUpgrade);
    }

    private IEnumerator UnlockNewUpgrade()
    {
        upgradeIndicator.fontSize = 1.3f;
        upgradeIndicator.text = "New Upgrade Unlocked!";

        yield return new WaitForSeconds(3f);

        rt.sizeDelta = new Vector2(12, rt.sizeDelta.y);
        upgradeIndicator.fontSize = 1f;

        SetCurrentUpgrade(UpgradeType.MultiCharge);
        
        SetState(UpgraderState.Hidden);
    }

    private void MarkUpgradeBought()
    {
        switch (currentUpgrade)
        {
            case UpgradeType.AutoCannon:
                AutoCannonBought = true;
                break;
            case UpgradeType.PreCharge:
                PreChargeBought = true;
                break;
            case UpgradeType.MultiCharge:
                MultiChargeBought = true;
                break;
        }
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
        mat.SetColor("_GlowColor", invalidGlowColor);

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Revert back to original color and glow
        upgradeIndicator.color = baseColor;
        mat.SetColor("_GlowColor", baseGlowColor);
    }

    private void RequestFocus()
    {
        if (activeUpgrader != null && activeUpgrader != this)
        {
            activeUpgrader.SetState(UpgraderState.Hidden);
        }

        activeUpgrader = this;
    }

    void Update()
    {
        if (clickableScript.ClickableEnabled && (currentState == UpgraderState.Locked || currentState == UpgraderState.Finished))
        {
            UpdateVisuals(false, false, false);
        }
    
        ManageOutlineFlash();

        AdjustUI();
    }

    private void ManageOutlineFlash()
    {
        if (currentState != UpgraderState.Locked && currentState != UpgraderState.Hidden && currentState != UpgraderState.Blinking)
        {
            return;
        }

        if (CheckUpgradability())
        {
            switch (upgraderType)
            {
                case UpgraderType.AutoCannon:
                    if (TowerManager.Instance.GetTowerCount() >= 3)
                    {
                        SetState(UpgraderState.Blinking);
                    }
                    break;
                    
                case UpgraderType.TimeStop:
                    SetState(UpgraderState.Blinking);
                    break;
            }   
        }
        else if (clickableScript.ClickableEnabled && currentState == UpgraderState.Blinking)
        {
            outlineFlashScript.StopFlashing(false);
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
                SetVisualParameters(10f, 0f, 0f, 0f);
            }
            else if (cam == 2)
            {
                SetVisualParameters(2f, -0.12f, 0f, -150f);
            }
            else
            {
                SetVisualParameters(10f, 1f, 0f, -4f);
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

    private void SetVisualParameters(float width, float x, float y, float z)
    {
        clickableScript.SetOutlineWidth(width);
        rt.localPosition = new Vector3(x, y, z);
    }
}