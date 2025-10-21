using UnityEngine;
using System.Collections;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;
    public Vector2 currentGPS;
    public bool IsLocationReady = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        StartCoroutine(InitializeGPS());
    }

    IEnumerator InitializeGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("오류: 위치 권한이 없습니다.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("오류: GPS 초기화에 실패했습니다.");
            yield break;
        }

        IsLocationReady = true;
        Input.compass.enabled = true; // 나침반 활성화

        Debug.Log("✅ GPS 준비 완료!");
        if (GameManager.Instance != null) GameManager.Instance.UpdateStatusText("GPS 준비 완료!\n바닥을 스캔하고 버튼을 누르세요.");

        while (true)
        {
            currentGPS = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            yield return new WaitForSeconds(1f);
        }
    }
}