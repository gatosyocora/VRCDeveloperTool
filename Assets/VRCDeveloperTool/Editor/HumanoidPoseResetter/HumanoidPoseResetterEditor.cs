using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ver 1.2
// © 2019 gatosyocora

namespace VRCDeveloperTool
{
    public class HumanoidPoseResetterEditor : EditorWindow
    {

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
            GetWindow<HumanoidPoseResetterEditor>("HumanoidPose Resetter");
        }

        [MenuItem("GameObject/VRCDeveloperTool/Reset Pose", false, 20)]
        public static void ResetPoseFromHierarchy(MenuCommand command)
        {
            var obj = command.context as GameObject;
            HumanoidPoseResetter.ResetPose(obj);
        }

        private void OnGUI()
        {
            targetObject = EditorGUILayout.ObjectField(
                "TargetObject",
                targetObject,
                typeof(GameObject),
                true
            ) as GameObject;

            GuiAction();

        }

        private void GuiAction()
        {

            EditorGUI.BeginDisabledGroup(targetObject == null);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Pose"))
            {
                HumanoidPoseResetter.ResetPose(targetObject);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }
    }

}

