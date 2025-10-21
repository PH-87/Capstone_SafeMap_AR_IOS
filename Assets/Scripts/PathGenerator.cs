using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq; // Newtonsoft.Json 사용

public class PathGenerator : MonoBehaviour
{
    public static List<Vector2> fullPath = new List<Vector2>();

    void Awake()
    {
        LoadPathFromJson("route");
    }

    void LoadPathFromJson(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile == null)
        {
            Debug.LogError($"[PathGenerator] JSON 파일을 찾을 수 없습니다: Resources/{fileName}.json");
            if(DebugManager.Instance != null) DebugManager.Instance.SetStatusColor(Color.red);
            return;
        }

        try
        {
            fullPath.Clear();
            
            // Newtonsoft.Json을 사용하여 파싱
            JObject data = JObject.Parse(jsonFile.text);
            JArray coords = (JArray)data["coords"];

            foreach (JArray coordPair in coords)
            {
                // 위도(latitude)와 경도(longitude) 순서가 바뀔 수 있으니 주의
                // 현재 [lat, lon] 순서를 가정
                float lat = (float)coordPair[0];
                float lon = (float)coordPair[1];
                fullPath.Add(new Vector2(lat, lon));
            }

            Debug.Log($"[PathGenerator] JSON에서 로드한 좌표 수: {fullPath.Count}");
            if (fullPath.Count > 0)
            {
                if(DebugManager.Instance != null) DebugManager.Instance.SetStatusColor(Color.blue);
            }
            else
            {
                if(DebugManager.Instance != null) DebugManager.Instance.SetStatusColor(Color.red);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PathGenerator] JSON 파싱 중 예외 발생: {ex.Message}");
            if(DebugManager.Instance != null) DebugManager.Instance.SetStatusColor(Color.red);
        }
    }
}