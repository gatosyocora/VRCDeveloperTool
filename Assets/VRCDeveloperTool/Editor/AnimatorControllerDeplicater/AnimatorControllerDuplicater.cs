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
        private static RuntimeAnimatorController tempController;
        private List<ControllerAnimationClip> animationClips;

        private string saveFolder;
        private string endKeyword;

        private bool isOverrideController = true;

        public class ControllerAnimationClip
        {
            public AnimationClip clip;
            public List<int> controllerIndices;
            public bool isDuplicate;

            public ControllerAnimationClip(AnimationClip clip, int index, bool isDuplicate = true)
            {
                this.clip = clip;
                controllerIndices = new List<int>();
                AddIndex(index);
                this.isDuplicate = isDuplicate;
            }

            public void AddIndex(int index)
            {
                controllerIndices.Add(index);
            }
        }

        [MenuItem("CONTEXT/RuntimeAnimatorController/Duplicate Controller And Clips")]
        private static void GetSelectController(MenuCommand menuCommand)
        {
            tempController = menuCommand.context as RuntimeAnimatorController;
            Open();
        }

        [MenuItem("VRCDeveloperTool/AnimatorControllerDuplicater")]
        public static void Open()
        {
            GetWindow<AnimatorControllerDuplicater>("AnimatorControllerDuplicater");
        }

        private void OnGUI()
        {
            if (tempController != null)
            {
                runtimeAnimatorController = tempController;
                tempController = null;
                LoadRuntimeControllerInfo(runtimeAnimatorController);
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                runtimeAnimatorController = EditorGUILayout.ObjectField(
                                                "AnimatorController",
                                                runtimeAnimatorController,
                                                typeof(RuntimeAnimatorController),
                                                true) as RuntimeAnimatorController;

                EditorGUILayout.HelpBox("複製したいAnimatorOverrideControllerを設定してください", MessageType.Info);

                if (!isOverrideController)
                    EditorGUILayout.HelpBox("まだAnimatorControllerは未対応です", MessageType.Error);

                if (check.changed && runtimeAnimatorController != null) 
                {
                    LoadRuntimeControllerInfo(runtimeAnimatorController);
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

            EditorGUILayout.HelpBox("AnimatorControllerおよび選択したAnimationClipを複製します\n複製されると複製後のものがそれぞれ設定されます\n複製後Assetのキーワードに設定した文字がそれぞれの名前の末尾につきます", MessageType.Info);

            using (new EditorGUI.DisabledGroupScope(runtimeAnimatorController == null || !isOverrideController))
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

                foreach (var index in animationClip.controllerIndices)
                {
                    runtimeAnimatorController.animationClips[index] = animationClip.clip;
                }
            }

            if (isOverrideController)
            {
                var overrideController = runtimeAnimatorController as AnimatorOverrideController;
                var baseAnimClips = overrideController.runtimeAnimatorController.animationClips;

                foreach (var animationClip in animationClips)
                {
                    foreach (var index in animationClip.controllerIndices)
                    {
                        var baseAnimClipName = baseAnimClips[index].name;
                        overrideController[baseAnimClipName] = animationClip.clip;
                    }
                }
            }
            else
            {

            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private List<ControllerAnimationClip> DistinctControllerAnimationClips(List<ControllerAnimationClip> animClips)
        {
            var distinctedAnimClips = new List<ControllerAnimationClip>();
            var animClipDictionary = new Dictionary<string, ControllerAnimationClip>();

            for (int i = 0; i < animClips.Count; i++)
            {
                var animName = animClips[i].clip.name;
                if (!animClipDictionary.ContainsKey(animName))
                {
                    animClipDictionary.Add(animName, animClips[i]);
                }
                else
                {
                    var animClipData = animClipDictionary[animName];
                    animClipData.AddIndex(animClips[i].controllerIndices.First());
                }
            }

            distinctedAnimClips = animClipDictionary.Select(x => x.Value).ToList();
            return distinctedAnimClips;
        }

        private void LoadRuntimeControllerInfo(RuntimeAnimatorController runtimeAnimatorController)
        {
            AnimatorController controller = runtimeAnimatorController as AnimatorController;
            AnimatorOverrideController overrideController = runtimeAnimatorController as AnimatorOverrideController;

            if (controller != null)
            {
                isOverrideController = false;
                // AnimatorControllerからAnimationClipの取得
            }
            else if (overrideController != null)
            {
                isOverrideController = true;
                // AnimatorOverrideControllerからAnimationClipの取得
                // OverrideされたAnimationClipのみ取得
                var baseAnimationController = overrideController.runtimeAnimatorController as AnimatorController;
                animationClips = overrideController.animationClips
                                .Select((x, index) => new { Value = x, Index = index })
                                .Where(x => baseAnimationController.animationClips[x.Index].name != x.Value.name)
                                .Select(x => new ControllerAnimationClip(x.Value, x.Index))
                                .ToList();

                animationClips = DistinctControllerAnimationClips(animationClips);

            }

            saveFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(runtimeAnimatorController));
            endKeyword = string.Empty;
        }
    }
}
