using UnityEngine;
using System;
using System.Collections;

public class TestingPanel : MonoBehaviour
{
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private StoredTimeManager storedTimeManager;
    [SerializeField] private EnemyCounter enemyCounter;
    private RectTransform rt;
    private bool isOnScreen = false;
    private bool isMoving = false;
    private float moveTime = 1f;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void StartMoving()
    {
        StartCoroutine(MoveOnAndOffScreen());
    }

    public IEnumerator MoveOnAndOffScreen()
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;
        float elapsedDelay = 0f;
        if (!isOnScreen)
        {
            while (elapsedDelay < moveTime)
            {
            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / moveTime;
            rt.anchoredPosition = Vector3.Lerp(new Vector2(-2244f, -1163f), new Vector2(-1256f, -1163f), t);

            yield return null;
            }
            isOnScreen = true;
        }
        else
        {
            while (elapsedDelay < moveTime)
            {
            elapsedDelay += Time.deltaTime;
            float t = elapsedDelay / moveTime;
            rt.anchoredPosition = Vector3.Lerp(new Vector2(-1256f, -1163f), new Vector2(-2244f, -1163f), t);

            yield return null;
            }
            isOnScreen = false;
        }
        isMoving = false;

        yield return null;
    }

    public void UpdateCoins(int coins)
    {
        if (coins < 0 && moneyManager.GetMoney() + coins < 0)
        {
            return;
        }

        moneyManager.UpdateMoney(coins);
    }

    public void UpdateSeconds(int seconds)
    {
        if (!LevelStarter.HasLevelStarted && seconds < 0 && storedTimeManager.GetSeconds() + seconds < 0)
        {
            return;
        }

        storedTimeManager.UpdateSeconds(seconds);
    }

    public void UpdateHP(int hp)
    {
        if (hp < 0 && BaseHealthManager.Instance.GetBaseHp() + hp < 0)
        {
            return;
        }

        BaseHealthManager.Instance.UpdateBaseHP(hp);
    }

    public void ToggleGameOver()
    {
        BaseHealthManager.Instance.ToggleGameOver();
    }

    public void ToggleWin()
    {
        BaseHealthManager.Instance.ToggleWin();
    }

    public void RestartLevel()
    {
        BaseHealthManager.Instance.RestartLevel();
    }

    public void DestroyProjectiles()
    {
        ProjectileManager.Instance.DestroyAllProjectiles();
    }
}
