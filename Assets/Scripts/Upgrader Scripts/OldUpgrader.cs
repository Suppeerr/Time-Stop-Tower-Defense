// using UnityEngine;
// using TMPro;
// using System.Collections;

// public class OldUpgrader : MonoBehaviour
// {
//     private StoredTimeManager storedTimeManagerScript;
//     private MoneyManager moneyManagerScript;
//     [SerializeField] private TMP_Text upgradeIndicator;
//     private RectTransform rt;
//     private Clickable clickableScript;
//     private OutlineFlash outlineFlashScript;
//     private Coroutine disappearRoutine;
//     private Coroutine confirmRoutine;
//     private static Upgrader activeUpgrader = null;

//     private Color baseGlowColor;
//     private Color baseColor = Color.yellow;
//     [SerializeField] private Color invalidGlowColor;
//     private Color invalidColor = Color.red;

//     [SerializeField] private bool isAutoCannon;
//     private int numberOfClicks = 0;
//     private int timeStopUpgrades = 0;
//     private bool upgraderFinished = false;
//     private bool isClickable = false;
//     private bool isConfirming = false;
//     private bool isUnlockingNewUpgrade = false;
//     private string baseText;
//     private int lastCam = -1;

//     private int currentMoney;
//     private int currentSeconds;
//     private float elapsed;

//     public static bool AutoCannonBought { get; private set; }
//     public static bool PreChargeBought { get; private set; }
//     public static bool TowerBoostBought { get; private set; }
//     public static bool MultiChargeBought { get; private set; }


//     void Start()
//     {
//         storedTimeManagerScript = GameObject.Find("Stored Time Manager")?.GetComponent<StoredTimeManager>();
//         moneyManagerScript = GameObject.Find("Money Manager")?.GetComponent<MoneyManager>();
//         clickableScript = this.GetComponent<Clickable>();
//         outlineFlashScript = this.GetComponent<OutlineFlash>();

//         rt = upgradeIndicator.GetComponent<RectTransform>();
//         upgradeIndicator.color = baseColor;
//         Material mat = upgradeIndicator.fontMaterial;
//         baseGlowColor = mat.GetColor("_GlowColor");
        
//         if (isAutoCannon)
//         {
//             baseText = "Enable Auto Cannon   6 Coins";
//             upgradeIndicator.text = baseText;
//         }
//         else
//         {
//             baseText = "Pre-Charge   5 Coins, 10s";
//             upgradeIndicator.text = baseText;
//         }
        
//         outlineFlashScript.StopFlashing();
//         clickableScript.enabled = false;
//         upgradeIndicator.enabled = false;
//     }

//     void Update()
//     {
//         OutlineFlashManager();

//         if (elapsed < 0.5f)
//         {
//             elapsed += Time.deltaTime;
//         }
//         else
//         {
//             elapsed = 0f;
//             currentMoney = moneyManagerScript.GetMoney();
//             currentSeconds = storedTimeManagerScript.GetSeconds();
//         }

//         int cam = CameraSwitch.ActiveCam;
//         if (isAutoCannon && cam != lastCam)
//         {
//             if (cam == 1)
//             {
//                 clickableScript.SetOutlineWidth(10f);
//                 rt.localPosition = new Vector3(0f, 0f, 0f);
//             }
//             else if (cam == 2)
//             {
//                 clickableScript.SetOutlineWidth(2f);
//                 rt.localPosition = new Vector3(-0.12f, 0f, -150f);
//             }
//             else
//             {
//                 clickableScript.SetOutlineWidth(10f);
//                 rt.localPosition = new Vector3(1f, 0f, -4f);
//             }

//             lastCam = cam;
//         }
//     }

//     private void OnMouseDown()
//     {
//         if (ProjectileManager.IsFrozen || upgraderFinished || BaseHealthManager.IsGameOver)
//         {
//             return;
//         }

//         RequestFocus();

//         currentMoney = moneyManagerScript.GetMoney();
//         currentSeconds = storedTimeManagerScript.GetSeconds();

//         if (isAutoCannon)
//         {
//             if (TowerManager.Instance.GetTowerCount() >= 3)
//             {
//                 numberOfClicks++;
//                 AutoCannonUpgrade();
//             }
//         }
//         else if (timeStopUpgrades == 0)
//         {
//             numberOfClicks++;
//             TimeStopUpgrade();
//         }
//         else if (timeStopUpgrades == 1)
//         {
//             numberOfClicks++;
//             TimeStopUpgrade();
//         }
//     }

//     // Auto Cannon Upgrade
//     private void AutoCannonUpgrade()
//     {
//         upgradeIndicator.enabled = true;
//         UpgradeChecker(6, 0, currentMoney >= 6, "Ignore Raycast");
//     }

//     // Time Stop Upgrades
//     private void TimeStopUpgrade()
//     {
//         upgradeIndicator.enabled = true;
//         if (timeStopUpgrades == 0)
//         {
//             UpgradeChecker(5, 10, currentMoney >= 5 && currentSeconds >= 10, "Ignore Time Stop");
//         }
//         else
//         {
//             UpgradeChecker(8, 15, currentMoney >= 8 && currentSeconds >= 15, "Ignore Time Stop");
//         }

//     }

//     private void UpgradeChecker(int money, int seconds, bool condition, string finishedLayerString)
//     {
//         if (condition)
//         {
//             if (numberOfClicks == 2)
//             {
//                 confirmRoutine = StartCoroutine(ConfirmUpgrade());
//                 return;
//             }

//             if (numberOfClicks == 3)
//             {
//                 numberOfClicks = 0;
//                 if (disappearRoutine != null)
//                 {
//                     StopCoroutine(disappearRoutine);
//                     StopCoroutine(confirmRoutine);
//                 }

//                 SuccessfulUpgrade(money, seconds);
                
//                 if (isAutoCannon)
//                 {
//                     AutoCannonBought = true;
//                     FinishUpgrader(finishedLayerString);
//                 }
//                 else if (timeStopUpgrades == 0)
//                 {
//                     PreChargeBought = true;
//                     timeStopUpgrades++;
//                     StartCoroutine(UnlockNewUpgrade());
//                 }
//                 else if (timeStopUpgrades == 1)
//                 {
//                     MultiChargeBought = true;
//                     FinishUpgrader(finishedLayerString);
//                 }
                
//                 return;
//             }
//         }
//         else if (numberOfClicks > 1)
//         {
//             StartCoroutine(FlashRed(0.4f));
//         }

//         if (disappearRoutine != null)
//         {
//             StopCoroutine(disappearRoutine);
//         }

//         disappearRoutine = StartCoroutine(WaitAndDisappear(5f));
//     }

//     private IEnumerator ConfirmUpgrade()
//     {
//         isConfirming = true;

//         if (isAutoCannon)
//         {
//             upgradeIndicator.fontSize = 1.4f;
//         }
//         else
//         {
//             upgradeIndicator.fontSize = 1.6f;
//         }

//         upgradeIndicator.text = "Confirm?";

//         yield return new WaitForSeconds(5f);

//         if (isAutoCannon)
//         {
//             upgradeIndicator.fontSize = 1f;
//         }

//         upgradeIndicator.text = baseText;
//         upgradeIndicator.enabled = false;
//         numberOfClicks = 0;

//         isConfirming = false;
//     }

//     private void SuccessfulUpgrade(int money, int seconds)
//     {
//         rt.sizeDelta = new Vector2(10, rt.sizeDelta.y);

//         if (isAutoCannon)
//         {
//             upgradeIndicator.fontSize = 1f;
//         }
//         else
//         {
//             upgradeIndicator.fontSize = 1.5f;
//         }

//         upgradeIndicator.text = "Upgrade Brought!";
//         moneyManagerScript.DecreaseMoney(money);
//         storedTimeManagerScript.DecreaseSeconds(seconds);
//     }

//     private IEnumerator UnlockNewUpgrade()
//     {
//         isUnlockingNewUpgrade = true;
//         clickableScript.enabled = false;
//         isClickable = true;
//         yield return new WaitForSeconds(3f); 
//         upgradeIndicator.fontSize = 1.3f;
//         upgradeIndicator.text = "New Upgrade Unlocked!";

//         yield return new WaitForSeconds(3f);

//         isClickable = false;
//         rt.sizeDelta = new Vector2(12, rt.sizeDelta.y);
//         baseText = "Multi-Charge   8 Coins, 15s";
//         upgradeIndicator.text = baseText;
//         upgradeIndicator.enabled = false;
//         isUnlockingNewUpgrade = false;
//     }

//     private void FinishUpgrader(string finishedLayer)
//     {
//         upgraderFinished = true;
//         gameObject.layer = LayerMask.NameToLayer(finishedLayer);
//         clickableScript.enabled = false;
//         outlineFlashScript.StopFlashing();
//         isConfirming = false;
//         isUnlockingNewUpgrade = false;
//         StartCoroutine(WaitAndDisappear(3f));
//     }

//     private void OutlineFlashManager()
//     {
//         if (isAutoCannon)
//         {
//             if (TowerManager.Instance.GetTowerCount() >= 3)
//             {
//                 EnableClickableAndFlash();
//             }
//         }
//         else if (timeStopUpgrades == 0 && currentMoney >= 5 && currentSeconds >= 10)
//         {
//             EnableClickableAndFlash();
//         }
//         else if (timeStopUpgrades == 1 && currentMoney >= 8 && currentSeconds >= 15)
//         {
//             EnableClickableAndFlash();
//         }
//     }

//     private void EnableClickableAndFlash()
//     {
//         if (isClickable)
//         {
//             return;
//         }

//         isClickable = true;
//         if (clickableScript.enabled == false)
//         {
//             clickableScript.enabled = true;
//         }
//         outlineFlashScript.StartFlashing();
//     }

//     private IEnumerator FlashRed(float duration)
//     {
//         // Get the font material instance
//         Material mat = upgradeIndicator.fontMaterial;

//         // Set color and glow to red 
//         upgradeIndicator.color = invalidColor;
//         mat.SetColor("_GlowColor", invalidGlowColor);

//         // Wait for duration
//         yield return new WaitForSeconds(duration);

//         // Revert back to original color and glow
//         upgradeIndicator.color = baseColor;
//         mat.SetColor("_GlowColor", baseGlowColor);
//     }

//     private void RequestFocus()
//     {
//         if (activeUpgrader != null && activeUpgrader != this)
//         {
//             activeUpgrader.ForceHideIndicator();
//         }

//         activeUpgrader = this;
//     }

//     private void ForceHideIndicator()
//     {
//         if (disappearRoutine != null)
//         {
//             StopCoroutine(disappearRoutine);
//         }
//         if (confirmRoutine != null)
//         {
//             StopCoroutine(confirmRoutine);
//         }

//         numberOfClicks = 0;
//         upgradeIndicator.text = baseText;
//         upgradeIndicator.enabled = false;

//         if (isAutoCannon)
//         {
//             upgradeIndicator.fontSize = 1f;
//         }

//         if (activeUpgrader != this)
//         {
//             activeUpgrader = null;
//         }
//     }

//     private IEnumerator WaitAndDisappear(float disappearAfter)
//     {
//         yield return new WaitForSeconds(disappearAfter);

//         if (isUnlockingNewUpgrade || isConfirming)
//         {
//             yield break;
//         }

//         ForceHideIndicator();
//     }
// }   
