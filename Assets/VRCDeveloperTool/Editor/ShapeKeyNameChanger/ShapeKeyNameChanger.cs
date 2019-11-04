using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

// ver 1.1
// Copyright (c) 2019 gatosyocora

namespace VRCDeveloperTool
{
    public class ShapeKeyNameChanger : EditorWindow
    {
        private List<string> shapeKeyNames;
        private SkinnedMeshRenderer renderer;

        private string[] posNames;

        private bool useDuplication = false;

        private Vector2 scrollPos = Vector2.zero;

        private void OnEnable()
        {
            shapeKeyNames = null;
            renderer = null;
            posNames = null;
        }

        [MenuItem("VRCDeveloperTool/Mesh/ShapeKeyName Changer")]
        private static void Create()
        {
            GetWindow<ShapeKeyNameChanger>("ShapeKeyName Changer");
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                renderer = EditorGUILayout.ObjectField(
                    "Renderer",
                    renderer,
                    typeof(SkinnedMeshRenderer),
                    true
                ) as SkinnedMeshRenderer;

                if (check.changed)
                {
                    if (renderer == null)
                    {
                        shapeKeyNames = null;
                        posNames = null;
                    }
                    else
                    {
                        shapeKeyNames = GetBlendShapeListFromRenderer(renderer);
                        posNames = shapeKeyNames.ToArray();
                    }
                }
            }

            if (shapeKeyNames != null)
            {
                using (var pos = new GUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = pos.scrollPosition;

                    for (int i = 0; i < shapeKeyNames.Count; i++)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (var toggle = new EditorGUI.ChangeCheckScope())
                            {
                                EditorGUILayout.Toggle(shapeKeyNames[i] != posNames[i], GUILayout.Width(30));
                                if (toggle.changed && shapeKeyNames[i] != posNames[i])
                                {
                                    posNames[i] = shapeKeyNames[i];
                                }
                            }
                            posNames[i] = EditorGUILayout.TextField(shapeKeyNames[i], posNames[i]);
                        }
                    }
                }
            }

            useDuplication = EditorGUILayout.Toggle("Duplication ShapeKeys", useDuplication);

            using (new EditorGUI.DisabledScope(renderer == null))
            {
                if (GUILayout.Button("Change ShapeKeyName"))
                {
                    CreateNewShapeKeyNameMesh(renderer, posNames, useDuplication, shapeKeyNames);

                    shapeKeyNames = GetBlendShapeListFromRenderer(renderer);
                    posNames = shapeKeyNames.ToArray();

                }
            }
        }

        /// <summary>
        /// 新しい名称のシェイプキーを持つメッシュを作成し, SkinnedMeshRendererに設定する
        /// </summary>
        /// <param name="renderer">シェイプキーの名称を変更したいメッシュを持つSkinnedMeshRenderer</param>
        /// <param name="posShapeKeyNames">変更後のシェイプキーの名称のリスト</param>
        /// <returns></returns>
        private bool CreateNewShapeKeyNameMesh(SkinnedMeshRenderer renderer, string[] posShapeKeyNames, bool useDuplication, List<string> preShapeKeyNames)
        {
            var mesh = renderer.sharedMesh;
            if (mesh == null) return false;

            if (posShapeKeyNames.Length != mesh.blendShapeCount) return false;

            var mesh_custom = Object.Instantiate<Mesh>(mesh);

            mesh_custom.ClearBlendShapes();

            var frameIndex = 0;
            var shapeKeyName = string.Empty;
            Vector3[] deltaVertices, deltaNormals, deltaTangents;
            for (int blendShapeIndex = 0; blendShapeIndex < mesh.blendShapeCount; blendShapeIndex++)
            {
                deltaVertices = new Vector3[mesh.vertexCount];
                deltaNormals = new Vector3[mesh.vertexCount];
                deltaTangents = new Vector3[mesh.vertexCount];

                mesh.GetBlendShapeFrameVertices(blendShapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);
                var weight = mesh.GetBlendShapeFrameWeight(blendShapeIndex, frameIndex);
                shapeKeyName = posNames[blendShapeIndex];

                // 複製するかつ変更されるシェイプキー名であれば
                if (useDuplication && !preShapeKeyNames[blendShapeIndex].Equals(shapeKeyName))
                {
                    mesh_custom.AddBlendShapeFrame(preShapeKeyNames[blendShapeIndex], weight, deltaVertices, deltaNormals, deltaTangents);
                }

                mesh_custom.AddBlendShapeFrame(shapeKeyName, weight, deltaVertices, deltaNormals, deltaTangents);
            }

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

