using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ver 1.0
// Copyright (c) 2020 gatosyocora
// Provided by @vjroba

namespace VRCDeveloperTool
{
    public class ShapeKeyDeleter : EditorWindow
    {
        private SkinnedMeshRenderer renderer;

        private List<string> shapeKeyNames;
        private bool[] selectedShapeKeys;
        private bool isOpenedBlendShape = true;
        private Vector2 shapeKeyScrollPos = Vector2.zero;


        [MenuItem("VRCDeveloperTool/Mesh/ShapeKey Deleter")]
        private static void Open()
        {
            GetWindow<ShapeKeyDeleter>("ShapeKey Deleter");
        }

        private void OnEnable()
        {
            renderer = null;
            shapeKeyNames = null;
            selectedShapeKeys = null;
            isOpenedBlendShape = true;
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
            }

            using (new EditorGUI.DisabledScope(renderer == null || (selectedShapeKeys != null && selectedShapeKeys.All(x => !x))))
            {
                if (GUILayout.Button("Delete ShapeKeys"))
                {
                    // 選択されている要素のインデックスの配列
                    var selectedBlendShapeIndexs = selectedShapeKeys
                        .Select((isSelect, index) => new { Index = index, Value = isSelect })
                        .Where(x => x.Value)
                        .Select(x => x.Index)
                        .ToArray();

                    DeleteShapeKey(renderer, selectedBlendShapeIndexs);

                    shapeKeyNames = GetBlendShapeListFromRenderer(renderer);
                    selectedShapeKeys = new bool[shapeKeyNames.Count()];
                }
            }
        }

        private bool DeleteShapeKey(SkinnedMeshRenderer renderer, int[] selectedShapeKeyIndexs)
        {
            var mesh = renderer.sharedMesh;
            if (mesh == null) return false;

            var mesh_custom = Instantiate(mesh);

            mesh_custom.ClearBlendShapes();

            int frameIndex = 0;
            string shapeKeyName;
            float weight;
            Vector3[] deltaVertices, deltaNormals, deltaTangents;

            for (int blendShapeIndex = 0; blendShapeIndex < mesh.blendShapeCount; blendShapeIndex++)
            {
                deltaVertices = new Vector3[mesh.vertexCount];
                deltaNormals = new Vector3[mesh.vertexCount];
                deltaTangents = new Vector3[mesh.vertexCount];
                mesh.GetBlendShapeFrameVertices(blendShapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);
                weight = mesh.GetBlendShapeFrameWeight(blendShapeIndex, frameIndex);

                if (!selectedShapeKeyIndexs.Contains(blendShapeIndex))
                {
                    shapeKeyName = mesh.GetBlendShapeName(blendShapeIndex);
                    mesh_custom.AddBlendShapeFrame(shapeKeyName, weight, deltaVertices, deltaNormals, deltaTangents);
                }
            }

            Undo.RecordObject(renderer, "Renderer " + renderer.name);
            renderer.sharedMesh = mesh_custom;

            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(mesh)) + "/" + mesh.name + "_shapekeydeleted.asset";
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
