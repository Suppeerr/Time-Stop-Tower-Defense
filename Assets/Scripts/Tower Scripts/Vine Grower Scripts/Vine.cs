using UnityEngine;

public class Vine : MonoBehaviour
{
    // Parent vine grower tower
    private VineGrower vineGrower;

    // Closest segment and vine position fields
    private ClosestSegmentInfo closestSegInfo;
    public float TrackDist { get; private set; }

    // Vine slow fields
    private float slowRadius = 1.2f;
    private float slowPercent = 0.3f;
    private float slowDuration = 2f;
    private float slowCooldown = 1f;
    private float slowTimer = 0f;

    // Vine charge fields
    private int maxCharges = 5;
    private int charges = 3;

    // Vine spawning fields
    private bool isActive = true;

    // Vine material fields
    [SerializeField] private Material normalVineMaterial;
    [SerializeField] private Material wiltedVineMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
    }

    // Initializes vine path position values 
    public void Init(ClosestSegmentInfo closestSegInfo, float trackDist, VineGrower vineGrower)
    {
        this.closestSegInfo = closestSegInfo;
        this.TrackDist = trackDist;
        this.vineGrower = vineGrower;
    }

    // Update is called once per frame
    void Update()
    {
        slowTimer -= Time.deltaTime;

        if (slowTimer <= 0f)
        {
            EnableSlow();
        }

        if (charges <= 0)
        {
            GetComponent<Renderer>().material = wiltedVineMaterial;
            isActive = false;
            Debug.Log("Charges depleted!");
        }
        else if (isActive == false)
        {
            GetComponent<Renderer>().material = normalVineMaterial;
            isActive = true;
        }
    }

    private void EnableSlow()
    {
        foreach (BaseEnemy enemy in LevelInstance.Instance.enemies)
        {
            if (charges > 0 && IsInRange(enemy))
            {
                Debug.Log("Enemy slowed!");
                EnemyObject enemyObj = enemy.visualObj.GetComponent<EnemyObject>();
                bool slowed = enemyObj.SlowEnemy(slowPercent, slowDuration);

                if (slowed)
                {
                    if (vineGrower.CheckFullyCharged())
                    {
                        vineGrower.ChangeLayer("Vine Tower");
                    }

                    charges--;
                    slowTimer = slowCooldown;
                    break;
                }
            }
        }
    }

    private bool IsInRange(BaseEnemy enemy)
    {
        float sqrRange = slowRadius * slowRadius;
        return (enemy.visualObj.transform.position - transform.position)
                .sqrMagnitude <= sqrRange;
    }

    public void AddCharges(int addedCharges)
    {
        charges = Mathf.Min(charges + addedCharges, maxCharges);
    }

    public int GetCharges()
    {
        return charges;
    }

    public int GetMaxCharges()
    {
        return maxCharges;
    }
}
