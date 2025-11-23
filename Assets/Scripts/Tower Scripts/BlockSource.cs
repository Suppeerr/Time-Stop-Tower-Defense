using UnityEngine;
using TMPro;
using System.Collections;

public class BlockSource : MonoBehaviour
{
    public GameObject draggablePrefab; // assign draggable prefab in inspector
    public GameObject moneyManager;
    private MoneyManager moneyManagerScript;
    private int splitterCost = 5;
    [SerializeField] private TMP_Text costIndicator;
    [SerializeField] private Color baseGlowColor;
    private Color baseColor = Color.white;
    [SerializeField] private Color invalidGlowColor;
    private Color invalidColor = Color.red;

    void Start()
    {
        moneyManagerScript = moneyManager.GetComponent<MoneyManager>();
        costIndicator.text = splitterCost + " Coins";
        costIndicator.color = baseColor;
    }

    private void OnMouseDown()
    {
        if (ProjectileManager.IsFrozen || !LevelStarter.HasLevelStarted || BaseHealthManager.IsGameOver)
        {
            return;
        }

        // Get a spawn position under the cursor (XZ plane)
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 spawnPos = transform.position; // fallback position

        if (Physics.Raycast(ray, out hit))
        {
            spawnPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }

        // Check if money manager has enough money
        int money = moneyManagerScript.GetMoney();
        if (money > 4)
        {
            // Spawn the block
            GameObject newBlock = Instantiate(draggablePrefab, spawnPos, Quaternion.identity);

            // Tell it to start dragging right away
            Draggable draggable = newBlock.GetComponent<Draggable>();
            if (draggable != null)
            {
                // Start dragging immediately
                draggable.BeginDrag(moneyManagerScript);
            }
            else
            {
                Debug.LogError("Spawned prefab is missing Draggable script!");
            }
        }
        else
        {
            StartCoroutine(FlashRed(0.4f));
        }
    }
    
    private IEnumerator FlashRed(float duration)
    {
        // Get the font material instance
        Material mat = costIndicator.fontMaterial;

        // Set color and glow to red 
        costIndicator.color = invalidColor;
        mat.SetColor("_GlowColor", invalidGlowColor);

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Revert back to original color and glow
        costIndicator.color = baseColor;
        mat.SetColor("_GlowColor", baseGlowColor);
    }
}   
