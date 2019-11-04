using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ver 1.0
// Copyright (c) 2019 gatosyocora

namespace VRCDeveloperTool
{
    public class ShapeKeyMixer : EditorWindow
    {
        private SkinnedMeshRenderer renderer;

        private List<string> shapeKeyNames;
        private bool[] selectedShapeKeys;
        private bool isOpenedBlendShape = true;
        private string combinedShapeKeyName = "";
        private bool deleteOriginShapeKey = true;
        private Vector2 shapeKeyScrollPos = Vector2.zero;


        [MenuItem("VRCDeveloperTool/Mesh/ShapeKey Mixer")]
        private static void Open()
        {
            GetWindow<ShapeKeyMixer>("ShapeKey Mixer");
        }

        private void OnEnable()
        {
            renderer = null;
            shapeKeyNames = null;
            selectedShapeKeys = null;
            isOpenedBlendShape = true;
            combinedShapeKeyName = "";
            shapeKeyScrollPos = Vector2.zero;
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                renderer = EditorGUILayout.ObjectField(
                                "SkinnedMeshRenderer",
                                renderer,
                                typeof(SkinnedMeshRenderer),
                                true
                            ) as SkinnedMeshRenderer;


                if (check.changed)
                {
                    if (renderer != null)
                    {
                        shapeKeyNames = GetBlendShapeListFromRenderer(renderer);
                        selectedShapeKeys = new bool[shapeKeyNames.Count()];
                    }
                }
            }

            if (shapeKeyNames != null)
            {
                isOpenedBlendShape = EditorGUILayout.Foldout(isOpenedBlendShape, "Shape Keys");
                if (isOpenedBlendShape)
                {
                    using (new EditorGUI.IndentLevelScope())
                    using (var scroll = new EditorGUILayout.ScrollViewScope(shapeKeyScrollPos, GUI.skin.box))
                    {
                        shapeKeyScrollPos = scroll.scrollPosition;
                        for (int i = 0; i < shapeKeyNames.Count(); i++)
                        {
                            selectedShapeKeys[i] = EditorGUILayout.ToggleLeft(shapeKeyNames[i], selectedShapeKeys[i]);
                        }
                    }
                }
                deleteOriginShapeKey = EditorGUILayout.Toggle("Delete Origin ShapeKey", deleteOriginShapeKey);
                combinedShapeKeyName = EditorGUILayout.TextField("Mixed ShapeKey Name", combinedShapeKeyName);
            }

            using (new EditorGUI.DisabledScope(renderer == null || combinedShapeKeyName == "" || (selectedShapeKeys != null && selectedShapeKeys.Sum(x => x ? 1 : 0) <= 1)))
            {
                if (GUILayout.Button("Mix ShapeKeys"))
                {
                    // 2つ以上が選択されている
                    if (selectedShapeKeys.Sum(x => x ? 1 : 0) > 1)
                    {
                        // 選択されている要素のインデックスの配列
                        var selectedBlendShapeIndexs = selectedShapeKeys
                            .Select((isSelect, index) => new { Index = index, Value = isSelect })
                            .Where(x => x.Value)
                            .Select(x => x.Index)
                            .ToArray();

                        MixShapeKey(renderer, selectedBlendShapeIndexs, combinedShapeKeyName, deleteOriginShapeKey);
                    }

                    shapeKeyNames = GetBlendShapeListFromRenderer(renderer);
                    selectedShapeKeys = new bool[shapeKeyNames.Count()];
                }
            }
        }

        private bool MixShapeKey(SkinnedMeshRenderer renderer, int[] selectedShapeKeyIndexs, string combinedBlendShapeName, bool deleteOriginShapeKey)
        {
            var mesh = renderer.sharedMesh;
            if (mesh == null) return false;

            var mesh_custom = Instantiate(mesh);

            mesh_custom.ClearBlendShapes();

            int frameIndex = 0;
            string shapeKeyName;
            float weight;
            Vector3[] deltaVertices, deltaNormals, deltaTangents;

            var combinedDeltaVertices = new Vector3[mesh.vertexCount];
            var combinedDeltaNormals = new Vector3[mesh.vertexCount];
            var combinedDeltaTangents = new Vector3[mesh.vertexCount];
            float combinedWeight = 0;

            for (int blendShapeIndex = 0; blendShapeIndex < mesh.blendShapeCount; blendShapeIndex++)
            {
                deltaVertices = new Vector3[mesh.vertexCount];
                deltaNormals = new Vector3[mesh.vertexCount];
                deltaTangents = new Vector3[mesh.vertexCount];
                mesh.GetBlendShapeFrameVertices(blendShapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);
                weight = mesh.GetBlendShapeFrameWeight(blendShapeIndex, frameIndex);

                if (selectedShapeKeyIndexs.Contains(blendShapeIndex))
                {
                    for (int i = 0; i < mesh.vertexCount; i++)
                    {
                        combinedDeltaVertices[i] += deltaVertices[i];
                        combinedDeltaNormals[i] += deltaNormals[i];
                        combinedDeltaTangents[i] += deltaTangents[i];
                        combinedWeight = Mathf.Max(combinedWeight, weight);
                    }

                    if (!deleteOriginShapeKey)
                    {
                        shapeKeyName = mesh.GetBlendShapeName(blendShapeIndex);
                        mesh_custom.AddBlendShapeFrame(shapeKeyName, weight, deltaVertices, deltaNormals, deltaTangents);
                    }
                }
                else
                {
                    shapeKeyName = mesh.GetBlendShapeName(blendShapeIndex);
                    mesh_custom.AddBlendShapeFrame(shapeKeyName, weight, deltaVertices, deltaNormals, deltaTangents);
                }
            }

            if (selectedShapeKeyIndexs.Length > 0)
                mesh_custom.AddBlendShapeFrame(combinedShapeKeyName, combinedWeight, combinedDeltaVertices, combinedDeltaNormals, combinedDeltaTangents);

            Undo.RecordObject(renderer, "Renderer " + renderer.name);
            renderer.sharedMesh = mesh_custom;

            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(mesh)) + "/" + mesh.name + "_custom.asset";
            AssetDatabase.CreateAsset(mesh_custom, AssetDatabase.GenerateUniqueAssetPath(path));
            AssetDatabase.SaveAssets();

            return true;
        }

        /// <summary>
        /// SkinnedMeshRendererのもつメッシュのシェイプキーの名前のリストを取得する
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        private List<string> GetBlendShapeListFromRenderer(SkinnedMeshRenderer renderer)
        {
            List<string> shapeKeyNames = new List<string>();
            var mesh = renderer.sharedMesh;

            if (mesh != null)
                for (int i = 0; i < mesh.blendShapeCount; i++)
                    shapeKeyNames.Add(mesh.GetBlendShapeName(i));

            return shapeKeyNames;
        }
    }
}
