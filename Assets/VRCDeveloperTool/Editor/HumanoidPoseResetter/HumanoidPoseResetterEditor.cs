using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ver 1.1
// © 2019-1-6 gatosyocora

public class HumanoidPoseResetter : EditorWindow {

    private GameObject targetObject = null;

    private static HumanBodyBones[] boneList
        = {
            HumanBodyBones.Hips,
            HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm,
            HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm,
            HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.RightUpperLeg,HumanBodyBones.RightLowerLeg,
            /*HumanBodyBones.LeftThumbDistal, */HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbProximal,
            HumanBodyBones.LeftIndexDistal, HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexProximal,
            HumanBodyBones.LeftMiddleDistal, HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleProximal,
            HumanBodyBones.LeftRingDistal, HumanBodyBones.LeftRingIntermediate, HumanBodyBones.LeftRingProximal,
            HumanBodyBones.LeftLittleDistal, HumanBodyBones.LeftLittleIntermediate, HumanBodyBones.LeftLittleProximal,
            /*HumanBodyBones.RightThumbDistal, */HumanBodyBones.RightThumbIntermediate, HumanBodyBones.RightThumbProximal,
            HumanBodyBones.RightIndexDistal, HumanBodyBones.RightIndexIntermediate, HumanBodyBones.RightIndexProximal,
            HumanBodyBones.RightMiddleDistal, HumanBodyBones.RightMiddleIntermediate, HumanBodyBones.RightMiddleProximal,
            HumanBodyBones.RightRingDistal, HumanBodyBones.RightRingIntermediate, HumanBodyBones.RightRingProximal,
            HumanBodyBones.RightLittleDistal, HumanBodyBones.RightLittleIntermediate, HumanBodyBones.RightLittleProximal
    };

    [MenuItem("VRCDeveloperTool/HumanoidPose Resetter")]
    private static void Create()
    {
        GetWindow<HumanoidPoseResetter>("HumanoidPose Resetter");
    }

    [MenuItem("GameObject/VRCDeveloperTool/Reset Pose", false, 20)]
    public static void ResetPoseFromHierarchy(MenuCommand command)
    {
        var obj = command.context as GameObject;
        ResetPose(obj);
    }

    private void OnGUI()
    {
        targetObject = EditorGUILayout.ObjectField(
            "TargetObject",
            targetObject,
            typeof(GameObject),
            true
        ) as GameObject;

        guiAction();

    }

    private void guiAction()
    {

        EditorGUI.BeginDisabledGroup(targetObject == null);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Pose"))
        {
            ResetPose(targetObject);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }

    private static void ResetPose(GameObject obj)
    {
        Object prefab = PrefabUtility.GetPrefabParent(obj);
        string prefabPath = AssetDatabase.GetAssetPath(prefab);

        if (prefab == null) return;

        var prefabObj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        prefabObj.SetActive(true);

        /* 対象オブジェクトのポーズを取得 */
        Animator animator = obj.GetComponent<Animator>();
        if (animator == null) return;

        var boneTrans = new Transform[boneList.Length];

        for (int i = 0; i < boneList.Length; i++)
            boneTrans[i] = animator.GetBoneTransform(boneList[i]);


        /* 対象オブジェクトの親Prefabのポーズを取得 */
        Animator prefabAnim = prefabObj.GetComponent<Animator>();
        if (prefabAnim == null) return;

        var boneTrans_p = new Transform[boneList.Length];

        for (int i = 0; i < boneList.Length; i++)
            boneTrans_p[i] = prefabAnim.GetBoneTransform(boneList[i]);


        for (int j = 0; j < boneList.Length; j++)
        {
            var trans = boneTrans[j];
            var prefabTrans = boneTrans_p[j];

            if (trans == null)
            {
                Debug.Log("[Transform not found]:"+j + ":" + boneList[j]);
                continue;
            }

            Undo.RecordObject(trans, "Change Transform "+trans.name);
            
            trans.localPosition = prefabTrans.localPosition;
            trans.localRotation = prefabTrans.localRotation;
        }

        DestroyImmediate(prefabObj);

    }
}
