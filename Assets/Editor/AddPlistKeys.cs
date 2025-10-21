using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class AddPlistKeys
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;

        rootDict.SetString("NSCameraUsageDescription", "AR 콘텐츠를 위해 카메라 접근이 필요합니다.");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "사용자의 위치를 기반으로 코인을 생성합니다.");

        File.WriteAllText(plistPath, plist.WriteToString());
    }
}