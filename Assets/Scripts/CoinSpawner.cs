using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Linq;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform arCamera;
    public Button spawnButton;
    
    [Tooltip("코인이 바닥에서 뜰 높이(미터 단위)")]
    public float spawnHeightOffset = 1f;
    
    [Header("자동 재배치 설정")]
    public bool useAutoRelocation = true;
    public float relocationInterval = 5.0f;
    public TMP_Text relocationTimerText;

    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;
    private bool spawned = false;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        raycastManager = arCamera.GetComponent<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        if (relocationTimerText != null) relocationTimerText.gameObject.SetActive(false);
    }

    public void SpawnCoinsOnClick()
    {
        if (spawned) return;
        if (LocationManager.Instance == null || !LocationManager.Instance.IsLocationReady) return;
        StartCoroutine(SpawnCoinsSequence());
    }

    IEnumerator SpawnCoinsSequence()
    {
        if (PathGenerator.fullPath == null || PathGenerator.fullPath.Count == 0) yield break;
        
        float baseHeightY;
        if (!TryGetBaseHeight(out baseHeightY))
        {
            if (GameManager.Instance != null) GameManager.Instance.UpdateStatusText("오류: 바닥을 먼저 스캔해주세요!");
            yield break;
        }

        if (GameManager.Instance != null) GameManager.Instance.UpdateStatusText("방향 맞추는 중...");
        
        float compassWaitStartTime = Time.time;
        yield return new WaitUntil(() => Input.compass.timestamp > 0 || Time.time - compassWaitStartTime > 5f);
        yield return new WaitForSeconds(0.5f);

        // ▼▼▼ 핵심 수정: 월드를 회전하는 대신, 사용할 회전값만 계산합니다. ▼▼▼
        // AR 카메라의 현재 방향과 실제 북쪽 방향의 차이를 계산하여 보정 회전값을 만듭니다.
        Quaternion northRotation = Quaternion.Euler(0, arCamera.transform.eulerAngles.y - Input.compass.trueHeading, 0);
        Debug.Log($"방향 보정값 계산 완료. 현재 방향: {arCamera.transform.eulerAngles.y}, 북쪽: {Input.compass.trueHeading}");
        // ▲▲▲ 핵심 수정 ▲▲▲

        if (GameManager.Instance != null) GameManager.Instance.UpdateStatusText("코인 생성 중...");

        Vector2 baseGPS = LocationManager.Instance.currentGPS;

        foreach (Vector2 targetGPS in PathGenerator.fullPath)
        {
            // 1. 기준 GPS(내 위치)로부터 목표 GPS까지의 순수 미터 오프셋 계산 (Z가 북쪽)
            Vector2 offsetInMeters = GPSToMeters(targetGPS, baseGPS);
            Vector3 coinOffset = new Vector3(offsetInMeters.x, 0, offsetInMeters.y);
            
            // 2. 계산된 오프셋을 '방향 보정값'으로 회전시켜 AR 공간에 맞게 정렬
            Vector3 rotatedOffset = northRotation * coinOffset;
            
            // 3. AR 카메라 위치에 회전된 오프셋을 더해 최종 위치 결정
            Vector3 finalPosition = arCamera.transform.position + rotatedOffset;
            finalPosition.y = baseHeightY + spawnHeightOffset;

            // 높이 재조정 시도
            Ray coinRay = new Ray(new Vector3(finalPosition.x, arCamera.position.y + 2f, finalPosition.z), Vector3.down);
            if (raycastManager.Raycast(coinRay, hits, TrackableType.PlaneWithinPolygon))
            {
                finalPosition.y = hits[0].pose.position.y + spawnHeightOffset;
            }
            
            Instantiate(coinPrefab, finalPosition, Quaternion.identity).GetComponent<Coin>().gpsPosition = targetGPS;
        }

        spawned = true; 
        GameManager.Instance.FindAndManageCoins();
        
        if (spawnButton != null)
        {
            spawnButton.interactable = false;
            TMP_Text buttonText = spawnButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null) buttonText.text = "생성 완료";
        }

        if (useAutoRelocation)
        {
            StartCoroutine(AutoRelocateCoroutine());
        }
    }

    IEnumerator AutoRelocateCoroutine()
    {
        if (relocationTimerText != null) relocationTimerText.gameObject.SetActive(true);
        float timer = relocationInterval;

        while (spawned)
        {
            timer -= Time.deltaTime;
            if (relocationTimerText != null)
            {
                relocationTimerText.text = $"높이 조정까지\n{Mathf.CeilToInt(timer)}초";
            }

            if (timer <= 0)
            {
                RelocateExistingCoins();
                timer = relocationInterval;
            }
            yield return null;
        }
    }

    public void RelocateExistingCoins()
    {
        float newBaseHeight;
        if (TryGetBaseHeight(out newBaseHeight))
        {
            Coin[] existingCoins = FindObjectsOfType<Coin>();
            foreach (Coin coin in existingCoins)
            {
                if (coin != null && !coin.collected)
                {
                    Vector3 currentPos = coin.transform.position;
                    coin.transform.position = new Vector3(currentPos.x, newBaseHeight + spawnHeightOffset, currentPos.z);
                }
            }
            if (GameManager.Instance != null) GameManager.Instance.UpdateStatusText("코인 높이 재배치 완료!");
        }
    }
    
    bool TryGetBaseHeight(out float height)
    {
        height = 0f;
        if (planeManager != null && planeManager.trackables.count > 0)
        {
            ARPlane closestPlane = null;
            float closestDistance = float.MaxValue;
            foreach (var plane in planeManager.trackables)
            {
                float distance = Vector3.Distance(arCamera.position, plane.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlane = plane;
                }
            }

            if (closestPlane != null)
            {
                height = closestPlane.center.y;
                return true;
            }
        }
        return false;
    }

    Vector2 GPSToMeters(Vector2 targetGPS, Vector2 originGPS)
    {
        float R = 6371f * 1000f;
        float latOriginRad = originGPS.x * Mathf.Deg2Rad;
        float lonOriginRad = originGPS.y * Mathf.Deg2Rad;
        float latTargetRad = targetGPS.x * Mathf.Deg2Rad;
        float lonTargetRad = targetGPS.y * Mathf.Deg2Rad;
        float z = R * (latTargetRad - latOriginRad);
        float x = R * (lonTargetRad - lonOriginRad) * Mathf.Cos(latOriginRad);
        return new Vector2(x, z);
    }
}