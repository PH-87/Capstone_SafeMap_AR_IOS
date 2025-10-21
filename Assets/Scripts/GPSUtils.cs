using UnityEngine;

public static class GPSUtils
{
    public static float GetDistanceFromGPS(float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6371000f; // 지구 반지름(m)
        float φ1 = lat1 * Mathf.Deg2Rad;
        float φ2 = lat2 * Mathf.Deg2Rad;
        float Δφ = (lat2 - lat1) * Mathf.Deg2Rad;
        float Δλ = (lon2 - lon1) * Mathf.Deg2Rad;

        float a = Mathf.Sin(Δφ / 2) * Mathf.Sin(Δφ / 2) +
                  Mathf.Cos(φ1) * Mathf.Cos(φ2) *
                  Mathf.Sin(Δλ / 2) * Mathf.Sin(Δλ / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c;
    }

    // Vector2 버전 추가 (선택)
    public static float GetDistanceFromGPS(Vector2 a, Vector2 b)
    {
        return GetDistanceFromGPS(a.x, a.y, b.x, b.y);
    }
}