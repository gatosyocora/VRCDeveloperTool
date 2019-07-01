using UnityEngine;
using UnityEditor;

// ver 1.1
// © 2018-9-25 gatosyocora

namespace VRCDeveloperTool
{

    public class HandPoseSettingData : ScriptableObject
    {
        private static string EditorPath = "Assets/VRCDeveloperTool/Editor/HandPoseAdder/";

        /*
        int handPoseNum = 0;

        struct handPoseData
        {
            public string handPoseName;
            public AnimationClip handPoseAnimClip;
        }

        List<handPoseData> handPoses;
        */

        public string handPoseName; // 手の形の名前

        public AnimationClip handPoseAnimClip; // 手の形のAnimationキーを持つAnimationClip

        // 設定データを保存するファイルを作成
        public static void CreateSettingData(string name, AnimationClip animClip)
        {
            var settingData = CreateInstance<HandPoseSettingData>();
            settingData.handPoseName = name;
            settingData.handPoseAnimClip = animClip;

            AssetDatabase.CreateAsset(settingData, EditorPath + "SettingData.asset");
            AssetDatabase.Refresh();
        }

        // 設定データを保存したファイルからデータを読み込み
        public static HandPoseSettingData LoadSettingData()
        {
            var settingData = AssetDatabase.LoadAssetAtPath<HandPoseSettingData>(EditorPath + "SettingData.asset");
            return settingData;
        }

    }

}
