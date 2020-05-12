using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using System.IO;
using Gatosyocora;

// ver 1.0
// Copyright (c) 2020 gatosyocora

namespace VRCDeveloperTool
{
    public class AnimatorControllerDuplicater : EditorWindow
    {
        private RuntimeAnimatorController runtimeAnimatorController;
        private List<ControllerAnimationClip> animationClips;

        private string saveFolder;
        private string endKeyword;

        public class ControllerAnimationClip
        {
            public AnimationClip clip;
            private List<int> controllerIndices;
            public bool isDuplicate;

            public ControllerAnimationClip(AnimationClip clip, bool isDuplicate = true)
            {
                this.clip = clip;
                controllerIndices = new List<int>();
                this.isDuplicate = isDuplicate;
            }

            public void AddIndex(int index)
            {
                controllerIndices.Add(index);
            }
        }

        [MenuItem("VRCDeveloperTool/AnimatorControllerDuplicater")]
        public static void Open()
        {
            GetWindow<AnimatorControllerDuplicater>("AnimatorControllerDuplicater");
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                runtimeAnimatorController = EditorGUILayout.ObjectField(
                                                "AnimatorController",
                                                runtimeAnimatorController,
                                                typeof(RuntimeAnimatorController),
                                                true) as RuntimeAnimatorController;

                EditorGUILayout.HelpBox("複製したいAnimatorControllerまたはAnimatorOverrideControllerを設定してください", MessageType.Info);

                if (check.changed && runtimeAnimatorController != null) 
                {
                    AnimatorController controller = runtimeAnimatorController as AnimatorController;
                    AnimatorOverrideController overrideController = runtimeAnimatorController as AnimatorOverrideController;

                    if (controller != null)
                    {
                        // AnimatorControllerからAnimationClipの取得
                    }
                    else if (overrideController != null)
                    {
                        // AnimatorOverrideControllerからAnimationClipの取得
                        // OverrideされたAnimationClipのみ取得
                        var baseAnimationController = overrideController.runtimeAnimatorController as AnimatorController;
                        animationClips = overrideController.animationClips
                                        .Where((x, i) => baseAnimationController.animationClips[i].name != x.name)
                                        .Distinct()
                                        .Select(x => new ControllerAnimationClip(x))
                                        .ToList();
                    }

                    saveFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(runtimeAnimatorController));
                    endKeyword = "_duplicated";
                }
            }

            if (animationClips != null)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("複製", EditorStyles.boldLabel, GUILayout.Width(50f));
                    EditorGUILayout.LabelField("AnimationClipの名前", EditorStyles.boldLabel);
                }
                foreach (var animationClip in animationClips)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        animationClip.isDuplicate = EditorGUILayout.ToggleLeft(string.Empty, animationClip.isDuplicate, GUILayout.Width(50f));
                        EditorGUILayout.LabelField(animationClip.clip.name);
                        if (GUILayout.Button("Select"))
                        {
                            Selection.activeObject = animationClip.clip;
                        }
                    }
                }
            }

            saveFolder = EditorGUILayout.TextField("保存先フォルダ", saveFolder);

            endKeyword = EditorGUILayout.TextField("複製後Assetのキーワード", endKeyword);

            EditorGUILayout.HelpBox("AnimatorControllerおよび選択したAnimationClipを複製します\n複製されると複製後のものがそれぞれ設定されます", MessageType.Info);

            using (new EditorGUI.DisabledGroupScope(runtimeAnimatorController == null))
            {
                if (GUILayout.Button("Duplicate AnimatorController & AnimationClips"))
                {
                    DuplicateAnimatorControllerAndAnimationClips();
                }
            }
        }

        private void DuplicateAnimatorControllerAndAnimationClips()
        {
            var controllerPath = AssetDatabase.GetAssetPath(runtimeAnimatorController);
            var newControllerPath = AssetDatabase.GenerateUniqueAssetPath(
                                        saveFolder + "\\"
                                        + GatoEditorUtility.AddKeywordToEnd(Path.GetFileNameWithoutExtension(controllerPath), endKeyword)
                                        + Path.GetExtension(controllerPath));

            var success = AssetDatabase.CopyAsset(controllerPath, newControllerPath);

            if (!success)
            {
                Debug.LogError("AnimatorControllerの複製に失敗しました");
                return;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            runtimeAnimatorController = AssetDatabase.LoadAssetAtPath(newControllerPath, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;

            foreach (var animationClip in animationClips)
            {
                if (!animationClip.isDuplicate) continue;

                var animClipPath = AssetDatabase.GetAssetPath(animationClip.clip);
                var newAnimClipPath = AssetDatabase.GenerateUniqueAssetPath(
                                        saveFolder + "\\"
                                        + GatoEditorUtility.AddKeywordToEnd(Path.GetFileNameWithoutExtension(animClipPath), endKeyword)
                                        + Path.GetExtension(animClipPath));

                var successAnimClip = AssetDatabase.CopyAsset(animClipPath, newAnimClipPath);

                if (!successAnimClip)
                {
                    Debug.LogErrorFormat("AnimationClip:{0}の複製に失敗しました", animationClip.clip.name);
                    return;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                animationClip.clip = AssetDatabase.LoadAssetAtPath(newAnimClipPath, typeof(AnimationClip)) as AnimationClip;

            }
        }
    }
}
