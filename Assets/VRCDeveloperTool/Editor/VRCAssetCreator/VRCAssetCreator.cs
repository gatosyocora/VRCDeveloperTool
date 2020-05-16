using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class VRCAssetCreator : Editor
{
#if VRC_SDK_VRCSDK2
    [MenuItem("Assets/Create/VRChat/CustomOverrideController", priority=0)]
    public static void CreateVRCCustomOverrideController()
    {
        var outputFolderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);

        var originalGuid = AssetDatabase.FindAssets("CustomOverrideEmpty").First();
        var originalPath = AssetDatabase.GUIDToAssetPath(originalGuid);
        var outputPath = outputFolderPath + "/" + Path.GetFileName(originalPath);
        DuplicateAsset(originalPath, outputPath);
    }
#endif

    private static void DuplicateAsset(string originalPath, string outputPath)
    {
        outputPath = AssetDatabase.GenerateUniqueAssetPath(outputPath);
        AssetDatabase.CopyAsset(originalPath, outputPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
