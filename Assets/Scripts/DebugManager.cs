using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;
    private Camera arCamera;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // AR 카메라를 자동으로 찾아둡니다.
            arCamera = Camera.main;
        }
    }

    // 상태에 따라 AR 카메라의 배경색을 바꾸는 함수
    public void SetStatusColor(Color statusColor)
    {
        if (arCamera != null)
        {
            // AR 카메라는 기본적으로 배경을 그리지 않지만,
            // 디버깅을 위해 강제로 특정 색을 칠하도록 설정합니다.
            arCamera.clearFlags = CameraClearFlags.SolidColor;
            arCamera.backgroundColor = statusColor;
        }
    }
}