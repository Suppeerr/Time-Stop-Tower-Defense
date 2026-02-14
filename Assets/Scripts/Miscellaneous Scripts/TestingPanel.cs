using UnityEngine;
using System;
using System.Collections;

public class TestingPanel : MonoBehaviour
{
    // Scripts 
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private StoredTimeManager storedTimeManager;
    [SerializeField] private EnemyCounter enemyCounter;

    // The panel's rect transform
    private RectTransform rt;

    // Movement and position fields
    private bool isOnScreen = false;
    private bool isMoving = false;
    private float moveTime = 1f;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    // Smoothly moves the testing panel on and off the screen
    public void MoveOnAndOffScreen()
    {
        if (isMoving)
        {
            return;
        }

        isMoving = true;

        if (!isOnScreen)
        {
            // Moves panel on screen
            StartCoroutine(MoveUI(new Vector2(-471, -236), new Vector2(-257, -236)));
            isOnScreen = true;
        }
        else
        {
            // Moves panel off screen
            StartCoroutine(MoveUI(new Vector2(-257, -236), new Vector2(-471, -236)));
            isOnScreen = false;
        }
        isMoving = false;
    }

    // Moves the panel's UI to a specified location
    private IEnumerator MoveUI(Vector2 from, Vector2 to)
    {
        float elapsedDelay = 0f;

        while (elapsedDelay < moveTime)
        {
            elapsedDelay += Time.unscaledDeltaTime;
            float t = elapsedDelay / moveTime;
            rt.anchoredPosition = Vector3.Lerp(from, to, t);

            yield return null;
        }
    }

    // Increases or decreases coins by a specified amount
    public void UpdateCoins(int coins)
    {
        if (coins < 0 && moneyManager.GetMoney() + coins < 0)
        {
            return;
        }

        moneyManager.UpdateMoney(coins);
    }

    // Increases or decreases seconds by a specified amount
    public void UpdateSeconds(int seconds)
    {
        if (seconds < 0 && storedTimeManager.GetSeconds() + seconds < 0)
        {
            return;
        }

        storedTimeManager.UpdateSeconds(seconds);
    }

    // Increases or decreases base hp by a specified amount
    public void UpdateHP(int hp)
    {
        if (hp < 0 && BaseHealthManager.Instance.GetBaseHp() + hp < 0)
        {
            return;
        }

        BaseHealthManager.Instance.UpdateBaseHP(hp);
    }

    // Toggles game over
    public void ToggleGameOver()
    {
        BaseHealthManager.Instance.ToggleGameOver();
    }

    // Toggles win
    public void ToggleWin()
    {
        BaseHealthManager.Instance.ToggleWin();
    }

    // Restarts the level
    public void RestartLevel()
    {
        BaseHealthManager.Instance.RestartLevel();
    }

    // Destroys all projectiles on screen
    public void DestroyProjectiles()
    {
        ProjectileManager.Instance.DestroyAllProjectiles();
    }
}
