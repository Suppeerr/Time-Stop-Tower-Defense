using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class BlockSource : MonoBehaviour
{
    // Draggable tower prefab and money manager script
    [SerializeField] private GameObject draggablePrefab;
    private MoneyManager moneyManagerScript;
    private Coroutine flashRedRoutine = null;

    // Cost indicator text and parameters
    [SerializeField] private TMP_Text costIndicator;
    private Color baseColor = Color.white;
    private Color invalidColor = Color.red;
    [SerializeField] private Color baseGlowColor;
    [SerializeField] private Color invalidGlowColor;
    private int currentMoney;
    private float elapsed = 0f;

    void Start()
    {
        // Initializes script and text variables
        moneyManagerScript = GameObject.Find("Money Manager")?.GetComponent<MoneyManager>();
        UpdateUI();
        costIndicator.color = baseColor;
    }

    void Update()
    {
        if (elapsed < 0.05f)
        {
            elapsed += Time.deltaTime;
        }
        else
        {
            elapsed = 0f;
            currentMoney = moneyManagerScript.GetMoney();

            if (flashRedRoutine != null)
            {
                return;
            }

            if (currentMoney >= TowerManager.Instance.GetSplitterCost())
            {
                costIndicator.fontMaterial.SetFloat("_GlowPower", 0.3f);
            }
            else
            {
                costIndicator.fontMaterial.SetFloat("_GlowPower", 0f);
            }
        }
    }

    public void OnMouseDown()
    {
        if (ProjectileManager.IsFrozen || BaseHealthManager.IsGameOver)
        {
            return;
        }

        // Get a spawn position under the cursor (XZ plane)
        Ray ray = CameraSwitch.CurrentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 spawnPos = transform.position; // fallback position

        if (Physics.Raycast(ray, out hit))
        {
            spawnPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }

        // Check if money manager has enough money
        currentMoney = moneyManagerScript.GetMoney();

        // If there is enough money, start dragging the tower
        if (currentMoney >= TowerManager.Instance.GetSplitterCost())
        {
            // Spawn the draggable tower
            GameObject newTower = Instantiate(draggablePrefab, spawnPos, Quaternion.identity);

            // Begin dragging
            Draggable draggable = newTower.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.BeginDrag();
            }
        }
        else
        {
            flashRedRoutine = StartCoroutine(FlashRed(0.4f));
        }
    }

    public void UpdateUI()
    {
        costIndicator.text = TowerManager.Instance.GetSplitterCost() + " Coins";
    }
    
    private IEnumerator FlashRed(float duration)
    {
        // Get the font material instance
        Material mat = costIndicator.fontMaterial;

        // Set color and glow to red 
        costIndicator.color = invalidColor;
        mat.SetColor("_GlowColor", invalidGlowColor);
        costIndicator.fontMaterial.SetFloat("_GlowPower", 0.3f);

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Revert back to original color and glow
        costIndicator.color = baseColor;
        mat.SetColor("_GlowColor", baseGlowColor);
        costIndicator.fontMaterial.SetFloat("_GlowPower", 0f);
        flashRedRoutine = null;
    }
}   
