using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float collectDistance = 3f;
    public TMP_Text statusText;
    public TMP_Text coinCountText; 
    
    private int totalCoins = 0;
    private int collectedCoins = 0;
    private Coin[] coins = new Coin[0];

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void FindAndManageCoins()
    {
        coins = FindObjectsOfType<Coin>();
        totalCoins = coins.Length;
        collectedCoins = 0;
        UpdateCoinCountUI();
    }

    void Update()
    {
        if (LocationManager.Instance == null || !LocationManager.Instance.IsLocationReady || coins.Length == 0) return;

        Vector2 myGPS = LocationManager.Instance.currentGPS;
        Coin closestCoin = null;
        float minDistance = float.MaxValue;

        foreach (Coin coin in coins)
        {
            if (coin == null || coin.collected) continue;
            float distance = GPSUtils.GetDistanceFromGPS(myGPS, coin.gpsPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCoin = coin;
            }
        }

        if (closestCoin != null && minDistance < collectDistance)
        {
            closestCoin.collected = true;
            closestCoin.PlaySound();
            Destroy(closestCoin.gameObject, 0.5f);
        
            collectedCoins++;
            UpdateCoinCountUI();
            UpdateStatusText($"코인 수거됨! (남은 거리: {minDistance:F1}m)");
        }
        else if (closestCoin != null)
        {
            UpdateStatusText($"다음 코인까지: {minDistance:F1}m");
        }
        else
        {
            UpdateStatusText("모든 코인 수집 완료!");
        }
    }

    void UpdateCoinCountUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = $"Coins\n{collectedCoins} / {totalCoins}";
        }
    }

    public void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}