using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlacedTower : MonoBehaviour
{
    // Tower placement animation
    private float placementTime = 0.3f;
    private float placementHeight = 3f;
    private bool isPlaced = false;
    public ParticleSystem dirtBurstPrefab;
    public ParticleSystem impactRingPrefab;

    // Tower placement audio
    public AudioSource towerPlaceSFX;

    // Raycasting
    private static LayerMask towerLayer;

    // Money manager script
    private MoneyManager moneyManagerScript;

    // Ball spawner script
    private BallSpawner ballSpawner;

    // Block source script
    private BlockSource towerSchematic;

    void Awake()
    {
        moneyManagerScript = GameObject.Find("Money Manager")?.GetComponent<MoneyManager>();
        towerSchematic = GameObject.Find("Splitter Schematic")?.GetComponent<BlockSource>();
        ballSpawner = GetComponent<BallSpawner>();
        ballSpawner.enabled = false;
        towerLayer = LayerMask.GetMask("Tower");
    }

    void Update()
    {
        SellTower();
    }

    // Places the tower with animations
    public IEnumerator PlaceTower(Vector3 endPos)
    {
        towerPlaceSFX?.Play();
        float elapsedDelay = 0f;
        Vector3 startPos = endPos + Vector3.up * placementHeight;
        moneyManagerScript.DecreaseMoney(TowerManager.Instance.GetSplitterCost());
        TowerManager.Instance.RegisterTower(this.gameObject);
        towerSchematic.UpdateUI();

        while (elapsedDelay < placementTime)
        {
            while (ProjectileManager.IsFrozen)
            {
                yield return null;
            }

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

    // Sells tower if x pressed while cursor is hovering over
    private void SellTower()
    {
        // Tower Selling
        if (isPlaced && Keyboard.current.xKey.wasPressedThisFrame)
        {
            Ray ray = CameraSwitch.CurrentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, towerLayer))
            {
                if (hit.collider.transform == this.transform)
                {
                    moneyManagerScript.UpdateMoney(4);
                    TowerManager.Instance.UnregisterTower(this.gameObject);
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
