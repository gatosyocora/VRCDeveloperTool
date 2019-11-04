using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

// ver 1.0.1
// Copyright (c) 2019 gatosyocora

namespace VRCDeveloperTool
{
    public class SubMeshDeleter : EditorWindow
    {
        private SkinnedMeshRenderer renderer;
        private List<SubMeshInfo> subMeshList;
        private int triangleCount = 0;

        private string saveFolder = "Assets/";
        private bool isOpenedSubMesh = true;
        private Vector2 subMeshScrollPos = Vector2.zero;

        [MenuItem("VRCDeveloperTool/Mesh/SubMesh Deleter")]
        private static void Open()
        {
            GetWindow<SubMeshDeleter>("SubMeshDeleter");
        }

        private void OnEnable()
        {
            renderer = null;
            subMeshList = null;
            triangleCount = 0;
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
                        var mesh = renderer.sharedMesh;
                        if (mesh != null)
                        {
                            subMeshList = GetSubMeshList(mesh);
                            triangleCount = GetMeshTriangleCount(mesh);
                            saveFolder = GetMeshPath(mesh);
                        }
                    }
                    else
                    {
                        subMeshList = null;
                    }
                }
            }

            if (subMeshList != null)
            {
                isOpenedSubMesh = EditorGUILayout.Foldout(isOpenedSubMesh, "SubMesh");
                if (isOpenedSubMesh)
                {
                    using (var scroll = new EditorGUILayout.ScrollViewScope(subMeshScrollPos))
                    {
                        subMeshScrollPos = scroll.scrollPosition;
                        for (int i = 0; i < subMeshList.Count(); i++)
                        {
                            subMeshList[i].selected = EditorGUILayout.ToggleLeft("subMesh " + (i + 1) + "(" + renderer.sharedMaterials[i].name + "):" + subMeshList[i].triangleCount, subMeshList[i].selected);
                        }
                    }
                        
                }

            }

            EditorGUILayout.LabelField("Triangle Count", triangleCount+"");

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Mesh SaveFolder", saveFolder);

                if (GUILayout.Button("Select Folder", GUILayout.Width(100)))
                {
                    saveFolder = EditorUtility.OpenFolderPanel("Select saved folder", saveFolder, "");
                    var match = Regex.Match(saveFolder, @"Assets/.*");
                    saveFolder = match.Value + "/";
                    if (saveFolder == "/") saveFolder = "Assets/";
                }
            }

            using (new EditorGUI.DisabledGroupScope(subMeshList == null || subMeshList.Count() <= 1))
            {
                if (GUILayout.Button("Delete SubMesh"))
                {
                    DeleteSelectedSubMesh(renderer, subMeshList);

                    var mesh = renderer.sharedMesh;
                    if (mesh != null)
                    {
                        subMeshList = GetSubMeshList(mesh);
                        triangleCount = GetMeshTriangleCount(mesh);
                    }
                }
            }
        }

        /// <summary>
        /// 選択中のサブメッシュを削除する
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="subMeshList"></param>
        /// <returns></returns>
        private bool DeleteSelectedSubMesh(SkinnedMeshRenderer renderer, List<SubMeshInfo> subMeshList)
        {
            // 削除する頂点インデックスのリスト（読み取り専用, 降順）
            var deleteVerticesIndicesUniqueDescending
                = subMeshList
                    .Where(x => x.selected)
                    .SelectMany(x => x.verticesIndices)
                    .Distinct()
                    .OrderByDescending(x => x)
                    .ToList()
                    .AsReadOnly();

            // 削除するサブメッシュのインデックスのリスト
            var deleteSubMeshIndexList
                = subMeshList
                    .Select((value, index) => new { Value = value, Index = index })
                    .Where(x => x.Value.selected)
                    .Select(x => x.Index)
                    .ToList()
                    .AsReadOnly();

            var mesh = renderer.sharedMesh;
            var mesh_custom = Instantiate(mesh);

            mesh_custom.Clear();

            // 頂点を削除
            var vertices = mesh.vertices.ToList();
            var boneWeights = mesh.boneWeights.ToList();
            var uvs = mesh.uv.ToList();
            var normals = mesh.normals.ToList();
            var tangents = mesh.tangents.ToList();
            var uv2s = mesh.uv2.ToList();
            var uv3s = mesh.uv3.ToList();
            var uv4s = mesh.uv4.ToList();
            foreach (var deleteVertexIndex in deleteVerticesIndicesUniqueDescending)
            {
                vertices.RemoveAt(deleteVertexIndex);
                boneWeights.RemoveAt(deleteVertexIndex);
                normals.RemoveAt(deleteVertexIndex);
                tangents.RemoveAt(deleteVertexIndex);
                if (deleteVertexIndex < uvs.Count())
                    uvs.RemoveAt(deleteVertexIndex);
                if (deleteVertexIndex < uv2s.Count())
                    uv2s.RemoveAt(deleteVertexIndex);
                if (deleteVertexIndex < uv3s.Count())
                    uv3s.RemoveAt(deleteVertexIndex);
                if (deleteVertexIndex < uv4s.Count())
                    uv4s.RemoveAt(deleteVertexIndex);
            }
            mesh_custom.SetVertices(vertices);
            mesh_custom.boneWeights = boneWeights.ToArray();
            mesh_custom.normals = normals.ToArray();
            mesh_custom.tangents = tangents.ToArray();
            mesh_custom.SetUVs(0, uvs);
            mesh_custom.SetUVs(1, uv2s);
            mesh_custom.SetUVs(2, uv3s);
            mesh_custom.SetUVs(3, uv4s);

            // サブメッシュごとにポリゴンを処理
            mesh_custom.subMeshCount = mesh.subMeshCount - deleteSubMeshIndexList.Count();
            var subMeshNumber = 0;
            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
            {
                if (deleteSubMeshIndexList.Contains(subMeshIndex)) continue;

                var subMeshTriangles = mesh.GetTriangles(subMeshIndex);
                // インデックスがずれるので各頂点への対応付けが必要
                // インデックスが大きいものから順に処理していく
                // O(n*m)
                foreach (var deleteVerticesIndex in deleteVerticesIndicesUniqueDescending) // n
                    for (int i = 0; i < subMeshTriangles.Count(); i++) // m
                        if (subMeshTriangles[i] > deleteVerticesIndex)
                            subMeshTriangles[i]--;
                mesh_custom.SetTriangles(subMeshTriangles, subMeshNumber++);
            }

            // BlendShapeを設定する
            string blendShapeName;
            float frameWeight;
            var deltaVertices = new Vector3[mesh.vertexCount];
            var deltaNormals = new Vector3[mesh.vertexCount];
            var deltaTangents = new Vector3[mesh.vertexCount];
            List<Vector3> deltaVerticesList, deltaNormalsList, deltaTangentsList;
            for (int blendshapeIndex = 0; blendshapeIndex < mesh.blendShapeCount; blendshapeIndex++)
            {
                blendShapeName = mesh.GetBlendShapeName(blendshapeIndex);
                frameWeight = mesh.GetBlendShapeFrameWeight(blendshapeIndex, 0);
                mesh.GetBlendShapeFrameVertices(blendshapeIndex, 0, deltaVertices, deltaNormals, deltaTangents);
                deltaVerticesList = deltaVertices.ToList();
                deltaNormalsList = deltaNormals.ToList();
                deltaTangentsList = deltaTangents.ToList();
                foreach (var deleteVertexIndex in deleteVerticesIndicesUniqueDescending)
                {
                    deltaVerticesList.RemoveAt(deleteVertexIndex);
                    deltaNormalsList.RemoveAt(deleteVertexIndex);
                    deltaTangentsList.RemoveAt(deleteVertexIndex);
                }
                mesh_custom.AddBlendShapeFrame(blendShapeName, frameWeight,
                    deltaVerticesList.ToArray(),
                    deltaNormalsList.ToArray(),
                    deltaTangentsList.ToArray());
            }

            AssetDatabase.CreateAsset(mesh_custom, AssetDatabase.GenerateUniqueAssetPath(saveFolder + mesh.name + "_deleteSubmesh.asset"));
            AssetDatabase.SaveAssets();

            Undo.RecordObject(renderer, "Change mesh " + mesh_custom.name);
            renderer.sharedMesh = mesh_custom;

            // 削除したサブメッシュのマテリアルを参照から外す
            var materials = renderer.sharedMaterials.ToList();
            for (var index = materials.Count() - 1; index >= 0; index--)
                if (deleteSubMeshIndexList.Contains(index))
                    materials.RemoveAt(index);
            renderer.sharedMaterials = materials.ToArray();

            return true;
        }

        /// <summary>
        /// サブメッシュのリストを取得する
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        private List<SubMeshInfo> GetSubMeshList(Mesh mesh)
        {
            List<SubMeshInfo> subMeshList = new List<SubMeshInfo>();

            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
            {
                var meshInfo = new SubMeshInfo(mesh, subMeshIndex);
                subMeshList.Add(meshInfo);
            }

            return subMeshList;
        }

        /// <summary>
        /// Meshのポリゴン数を取得する
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        private int GetMeshTriangleCount(Mesh mesh)
        {
            return mesh.triangles.Length / 3;
        }

        /// <summary>
        /// mesh保存先のパスを取得する
        /// </summary>
        /// <param name="Mesh"></param>
        /// <returns></returns>
        private string GetMeshPath(Mesh mesh)
        {
            return Path.GetDirectoryName(AssetDatabase.GetAssetPath(mesh))+"/";
        }

        public class SubMeshInfo
        {
            public int subMeshIndex;
            public int[] verticesIndices;
            public int vertexCount;
            public int triangleCount;
            public bool selected = false;

            public SubMeshInfo(Mesh mesh, int subMeshIndex)
            {
                this.subMeshIndex = subMeshIndex;
                this.verticesIndices = mesh.GetIndices(subMeshIndex);
                vertexCount = verticesIndices.Length;
                triangleCount = mesh.GetTriangles(subMeshIndex).Length / 3;
            }
        }
    }
}
