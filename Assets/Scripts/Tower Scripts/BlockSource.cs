using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class BlockSource : MonoBehaviour
{
    // Draggable tower prefab
    [SerializeField] private GameObject draggablePrefab;

    // Red flash coroutine for invalid purchases
    private Coroutine flashRedRoutine = null;

    // Cost indicator text and parameters
    [SerializeField] private TMP_Text costIndicator;
    private Color baseColor = Color.white;
    private Color invalidColor = Color.red;
    [SerializeField] private Color baseGlowColor;
    [SerializeField] private Color invalidGlowColor;

    void Start()
    {
        // Initializes text variables
        UpdateUI();
        costIndicator.color = baseColor;
    }

    void Update()
    {
        UpdateGlow();
    }

    public void OnMouseDown()
    {
        if (TimeStop.Instance.IsFrozen || BaseHealthManager.IsGameOver)
        {
            return;
        }

        // Get a spawn position under the cursor in the XZ plane
        Ray ray = CameraSwitcher.Instance.CurrentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 spawnPos = transform.position; 

        if (Physics.Raycast(ray, out hit))
        {
            spawnPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }

        // If there is enough money, start dragging the tower
        if (MoneyManager.Instance.GetMoney() >= TowerManager.Instance.GetSplitterCost())
        {
            GameObject newTower = Instantiate(draggablePrefab, spawnPos, Quaternion.identity);

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

    // Updates the tower price indicator to reflect the current cost
    public void UpdateUI()
    {
        costIndicator.text = TowerManager.Instance.GetSplitterCost() + " Coins";
    }

    // Makes the tower price indicator glow when the tower is purchasable 
    private void UpdateGlow()
    {
        if (flashRedRoutine != null)
        {
            return;
        }

        if (MoneyManager.Instance.GetMoney() >= TowerManager.Instance.GetSplitterCost())
        {
            costIndicator.fontMaterial.SetFloat("_GlowPower", 0.3f);
        }
        else
        {
            costIndicator.fontMaterial.SetFloat("_GlowPower", 0f);
        }
    }
    
    // Flashes the tower price indicator red briefly
    private IEnumerator FlashRed(float duration)
    {
        Material mat = costIndicator.fontMaterial;

        costIndicator.color = invalidColor;
        mat.SetColor("_GlowColor", invalidGlowColor);
        costIndicator.fontMaterial.SetFloat("_GlowPower", 0.3f);

        yield return new WaitForSeconds(duration);

        costIndicator.color = baseColor;
        mat.SetColor("_GlowColor", baseGlowColor);
        costIndicator.fontMaterial.SetFloat("_GlowPower", 0f);
        flashRedRoutine = null;
    }
}   
