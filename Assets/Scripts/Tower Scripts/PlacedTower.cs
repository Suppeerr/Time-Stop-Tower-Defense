using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlacedTower : MonoBehaviour
{
    // Tower placement animation fields
    private float placementTime = 0.3f;
    private float placementHeight = 3f;
    private bool isPlaced = false;

    // Raycasting tower layer
    private static LayerMask towerLayer;

    // Tower placement visual effects
    [SerializeField] private ParticleSystem dirtBurstPrefab;
    [SerializeField] private ParticleSystem impactRingPrefab;

    // Tower placement audio
    [SerializeField] private AudioSource towerPlaceSFX;

    // Ball spawner and block source scripts
    private BallSpawner ballSpawner;
    private BlockSource towerSchematic;

    // Tower sell price
    [SerializeField] private int sellPrice;

    void Awake()
    {
        // Initializes fields
        towerSchematic = GameObject.Find("Splitter Schematic")?.GetComponent<BlockSource>();
        ballSpawner = GetComponent<BallSpawner>();
        ballSpawner.enabled = false;
        towerLayer = LayerMask.GetMask("Tower");
    }

    void Update()
    {
        // Sells tower if x is pressed while cursor is hovering over 
        if (isPlaced && Keyboard.current.xKey.wasPressedThisFrame)
        {
            SellTower();
        }
    }

    // Places the tower with an animation
    public IEnumerator PlaceTower(Vector3 endPos)
    {
        towerPlaceSFX?.Play();
        
        Vector3 startPos = endPos + Vector3.up * placementHeight;
        MoneyManager.Instance.UpdateMoney(TowerManager.Instance.GetSplitterCost(), true);
        TowerManager.Instance.RegisterTower(this.gameObject);
        towerSchematic.UpdateUI();

        float elapsedDelay = 0f;

        while (elapsedDelay < placementTime)
        {
            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / placementTime;
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        // Plays dirt burst animation
        if (dirtBurstPrefab != null)
        {
            ParticleSystem d = Instantiate(dirtBurstPrefab, endPos + new Vector3(0f, -0.8f, 0f), Quaternion.Euler(-90, 0, 0));
            d.Play();
            Destroy(d.gameObject, d.main.duration);
        }

        // Plays ring impact animation
        if (dirtBurstPrefab != null)
        {
            ParticleSystem r = Instantiate(impactRingPrefab, endPos + new Vector3(0f, -0.8f, 0f), Quaternion.Euler(-90, 0, 0));
            r.Play();
            Destroy(r.gameObject, r.main.duration);
        }

        if (ballSpawner != null)
        {
            ballSpawner.enabled = true;
        }

        isPlaced = true;
    }

    // Sells tower and refunds some coins
    private void SellTower()
    {
        Ray ray = CameraSwitcher.Instance.CurrentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, towerLayer))
        {
            if (hit.collider.transform == this.transform)
            {
                MoneyManager.Instance.UpdateMoney(sellPrice);
                TowerManager.Instance.UnregisterTower(this.gameObject);
                towerSchematic.UpdateUI();

                Destroy(this.gameObject);
            }
        }
    }
}
