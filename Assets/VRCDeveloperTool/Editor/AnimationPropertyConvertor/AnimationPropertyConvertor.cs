using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

// ver 1.0
// © 2019 gatosyocora

namespace VRCDeveloperTool
{

    public class AnimationPropertyConvertor : EditorWindow
    {

        private SkinnedMeshRenderer avatarMesh;

        private List<AnimationClip> preAnimationClips;
        private List<AnimationProperty> animPropertyList;
        private string[] blendShapeNameList;

        private string saveFolder = "Assets/";

        private bool convertBlendShapeName = false;
        private bool isOpeningPreAnimationClips = true;
        private bool isOpeningAnimationPropertyList = true;
        private Vector2 propertyScrollPos = Vector2.zero;
        private bool isConvertAll = false;

        public class AnimationProperty
        {
            public string propertyType;
            public string preName;
            public string posName;
            public bool isConvert;
            public int selectedBlendShapeIndex;
            public List<int> animIndexHavingThisProperty;

            public AnimationProperty(string type, string name, int animIndex)
            {
                propertyType = type;
                preName = name;
                posName = preName;
                isConvert = false;
                selectedBlendShapeIndex = 0;

                animIndexHavingThisProperty = new List<int>() { animIndex };
            }

            public void AddAnimIndexHavingThisProperty(int animIndex)
            {
                animIndexHavingThisProperty.Add(animIndex);
            }

            public bool RemoveAnimIndexHavingThisProperty(int animIndex)
            {
                return animIndexHavingThisProperty.Remove(animIndex);
            }

            public bool existAnimHavingThisProperty()
            {
                return animIndexHavingThisProperty.Count > 0;
            }
        }

        [MenuItem("VRCDeveloperTool/AnimationPropertyConvertor")]
        private static void Create()
        {
            GetWindow<AnimationPropertyConvertor>("AnimationProperty Convertor");
        }

        private void OnEnable()
        {
            avatarMesh = null;

            preAnimationClips = new List<AnimationClip>();
            preAnimationClips.Add(null);

            animPropertyList = new List<AnimationProperty>();

            blendShapeNameList = null;
        }

        private void OnGUI()
        {
            convertBlendShapeName = EditorGUILayout.ToggleLeft("Convert BlendShapeName", convertBlendShapeName);

            if (convertBlendShapeName)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    avatarMesh = EditorGUILayout.ObjectField(
                        "Avatar's SkinnedMeshRenderer",
                        avatarMesh,
                        typeof(SkinnedMeshRenderer),
                        true
                    ) as SkinnedMeshRenderer;

                    if (check.changed && avatarMesh != null)
                        blendShapeNameList = GetBlendShapeNameList(avatarMesh);
                }
            }

            isOpeningPreAnimationClips = EditorGUILayout.Foldout(isOpeningPreAnimationClips, "Pre AnimationClips");
            if (isOpeningPreAnimationClips)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("+"))
                    {
                        preAnimationClips.Add(null);
                    }
                    if (GUILayout.Button("-") && preAnimationClips.Count > 1)
                    {
                        var animIndex = preAnimationClips.Count - 1;

                        CheckAndRemoveAnimProperties(ref animPropertyList, animIndex);

                        preAnimationClips.RemoveAt(animIndex);
                    }
                }
                using (new EditorGUI.IndentLevelScope())
                {
                    for (int animIndex = 0; animIndex < preAnimationClips.Count; animIndex++)
                    {
                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            preAnimationClips[animIndex] = EditorGUILayout.ObjectField(
                                "AnimationClip " + (animIndex + 1),
                                preAnimationClips[animIndex],
                                typeof(AnimationClip),
                                true
                            ) as AnimationClip;

                            if (check.changed)
                            {
                                CheckAndRemoveAnimProperties(ref animPropertyList, animIndex);

                                if (preAnimationClips[animIndex] != null)
                                    UpdateAnimationPropertyNameList(preAnimationClips[animIndex], ref animPropertyList, animIndex);
                            }
                        }
                    }
                }
            }
            EditorGUILayout.Space();

            isOpeningAnimationPropertyList = EditorGUILayout.Foldout(isOpeningAnimationPropertyList, "AnimationPropertyList");
            if (isOpeningAnimationPropertyList)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            isConvertAll = EditorGUILayout.Toggle(isConvertAll, GUILayout.Width(30f));

                            if (check.changed)
                                ChangeAllIsConvertParams(isConvertAll, ref animPropertyList);
                        }

                        EditorGUILayout.LabelField("prePropertyName", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("posPropertyName", EditorStyles.boldLabel);
                    }

                    using (var scrollPos = new GUILayout.ScrollViewScope(propertyScrollPos))
                    {
                        propertyScrollPos = scrollPos.scrollPosition;
                        foreach (var animProperty in animPropertyList)
                        {
                            if (!convertBlendShapeName || animProperty.propertyType == "blendShape")
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    animProperty.isConvert = EditorGUILayout.Toggle(animProperty.isConvert, GUILayout.Width(30f));

                                    if (convertBlendShapeName && avatarMesh != null)
                                    {
                                        EditorGUILayout.LabelField(animProperty.preName);
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField(animProperty.propertyType + "." + animProperty.preName);
                                    }

                                    using (var check = new EditorGUI.ChangeCheckScope())
                                    {

                                        if (convertBlendShapeName && avatarMesh != null)
                                        {
                                            animProperty.selectedBlendShapeIndex = EditorGUILayout.Popup(animProperty.selectedBlendShapeIndex, blendShapeNameList);
                                        }
                                        else
                                        {
                                            animProperty.posName = EditorGUILayout.TextField(animProperty.posName);
                                        }

                                        if (check.changed)
                                        {
                                            if (convertBlendShapeName && avatarMesh != null)
                                                animProperty.posName = blendShapeNameList[animProperty.selectedBlendShapeIndex];

                                            animProperty.isConvert = true;
                                        }

                                    }
                                }
                            }

                        }
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("AnimClipSaveFolder", saveFolder);

                if (GUILayout.Button("Select Folder", GUILayout.Width(100)))
                {
                    saveFolder = EditorUtility.OpenFolderPanel("Select saved folder", saveFolder, "");
                    var match = Regex.Match(saveFolder, @"Assets/.*");
                    saveFolder = match.Value + "/";
                    if (saveFolder == "/") saveFolder = "Assets/";
                }

            }

            if (GUILayout.Button("Convert Property & Save as New File"))
            {
                ConvertAndCreateAnimationClips(preAnimationClips, animPropertyList, saveFolder, convertBlendShapeName);
            }

        }

        /// <summary>
        /// アニメーションファイルのプロパティの名前を変更して新しいファイルとして書き出す
        /// </summary>
        /// <param name="baseAnimClips"></param>
        /// <param name="animPropertyList"></param>
        /// <param name="saveFolder"></param>
        /// <param name="convertBlendShapeName"></param>
        private void ConvertAndCreateAnimationClips(List<AnimationClip> baseAnimClips, List<AnimationProperty> animPropertyList, string saveFolder, bool convertBlendShapeName)
        {
            // 変換するプロパティ（isConvert == true）のものだけのリストをつくる
            var shoundConvertedPropertyList = animPropertyList.Where(x => x.isConvert).ToList<AnimationProperty>();

            foreach (var baseAnimClip in baseAnimClips)
            {
                if (baseAnimClip == null) continue;

                var convertedAnimClip = Object.Instantiate<AnimationClip>(baseAnimClip);

                var bindings = AnimationUtility.GetCurveBindings(convertedAnimClip);

                for (int i = 0; i < bindings.Length; i++)
                {
                    // binding.propertyName == blendShape.vrc.v_silみたいになっている
                    var propertyType = bindings[i].propertyName.Split('.')[0];
                    var blendShapeName = bindings[i].propertyName.Replace(propertyType + ".", "");

                    // blendShapeだけ変更するモードだったらそれ以外の場合は処理しない
                    if (convertBlendShapeName && propertyType != "blendShape") continue;

                    // 変換するプロパティのリストに含まれるプロパティだけ変換する
                    var targetAnimProperty = shoundConvertedPropertyList.Find(x => x.propertyType == propertyType && x.preName == blendShapeName);
                    if (targetAnimProperty != null)
                    {
                        var curve = AnimationUtility.GetEditorCurve(convertedAnimClip, bindings[i]);
                        AnimationUtility.SetEditorCurve(convertedAnimClip, bindings[i], null);

                        bindings[i].propertyName = targetAnimProperty.propertyType + "." + targetAnimProperty.posName;

                        AnimationUtility.SetEditorCurve(convertedAnimClip, bindings[i], curve);
                    }
                }

                AssetDatabase.CreateAsset(convertedAnimClip, AssetDatabase.GenerateUniqueAssetPath(saveFolder + baseAnimClip.name + ".anim"));

            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// アニメーションプロパティリストを更新する
        /// </summary>
        /// <param name="animClips"></param>
        /// <param name="animPropertyList"></param>
        private void UpdateAnimationPropertyNameList(AnimationClip animClips, ref List<AnimationProperty> animPropertyList, int animIndex)
        {
            var bindings = AnimationUtility.GetCurveBindings(animClips);

            foreach (var binding in bindings)
            {
                // binding.propertyName == blendShape.vrc.v_silみたいになっている
                var propertyType = binding.propertyName.Split('.')[0];
                var blendShapeName = binding.propertyName.Replace(propertyType + ".", "");

                // animPropertyListに含まれていないプロパティだけ追加する
                // すでに含まれている場合AnimIndexHavingThisPropertyに追加する
                var animProperty = animPropertyList.Find(x => x.propertyType == propertyType && x.preName == blendShapeName);
                if (animProperty == null)
                    animPropertyList.Add(new AnimationProperty(propertyType, blendShapeName, animIndex));
                else
                    animProperty.AddAnimIndexHavingThisProperty(animIndex);

            }
        }

        /// <summary>
        /// ブレンドシェイプの名前一覧を取得する
        /// </summary>
        /// <param name="meshRenderer"></param>
        /// <returns></returns>
        private string[] GetBlendShapeNameList(SkinnedMeshRenderer meshRenderer)
        {
            var mesh = meshRenderer.sharedMesh;
            if (mesh == null) return null;

            var blendShapeNameList = new string[mesh.blendShapeCount];

            for (int i = 0; i < mesh.blendShapeCount; i++)
                blendShapeNameList[i] = mesh.GetBlendShapeName(i);

            return blendShapeNameList;
        }

        /// <summary>
        /// すべてのプロパティリストのisConvertを変更する
        /// </summary>
        /// <param name="isConvertAll"></param>
        /// <param name="animPropertyList"></param>
        private void ChangeAllIsConvertParams(bool isConvertAll, ref List<AnimationProperty> animPropertyList)
        {
            foreach (var animProperty in animPropertyList)
            {
                animProperty.isConvert = isConvertAll;
            }
        }

        /// <summary>
        /// プロパティリストに含まれるすべてのプロパティのAnimIndexHavingThisPropertyからanimIndexを削除して
        /// リストに不要なプロパティか調べ、削除する
        /// </summary>
        /// <param name="animPropertyList"></param>
        /// <param name="animIndex"></param>
        private void CheckAndRemoveAnimProperties(ref List<AnimationProperty> animPropertyList, int animIndex)
        {
            foreach (var animProperty in animPropertyList.ToArray())
            {
                animProperty.RemoveAnimIndexHavingThisProperty(animIndex);
                if (!animProperty.existAnimHavingThisProperty())
                    animPropertyList.Remove(animProperty);
            }
        }
    }

}
