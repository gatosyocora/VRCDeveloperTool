using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
using System.IO;

// ver 1.00
// (C) 2019 gatosyocora

namespace VRCDeveloperTool
{
   public class ShapeKeyReorder : EditorWindow
    {
        private ReorderableList blendShapeReorderableList;
        private SkinnedMeshRenderer renderer;

        private Vector2 scrollPos = Vector2.zero;

        public class BlendShape
        {
            public int index;
            public string name;

            public BlendShape(int index, string name)
            {
                this.index = index;
                this.name = name;
            }
        }

        [MenuItem("VRCDeveloperTool/ShapeKey Reorder")]
        private static void Open()
        {
            GetWindow<ShapeKeyReorder>("ShapeKey Reorder");
        }

        private void OnEnable()
        {
            renderer = null;
            blendShapeReorderableList = null;
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                renderer = EditorGUILayout.ObjectField(
                                            "Renderer",
                                            renderer,
                                            typeof(SkinnedMeshRenderer)
                           ) as SkinnedMeshRenderer;

                if (check.changed)
                {
                    if (renderer != null)
                    {
                        var blendShapePairList = new List<BlendShape>();
                        var mesh = renderer.sharedMesh;
                        var blendShapeCount = mesh.blendShapeCount;
                        for (int i = 0; i < blendShapeCount; i++)
                        {
                            blendShapePairList.Add(new BlendShape(i, mesh.GetBlendShapeName(i)));
                        }
                        blendShapeReorderableList = InitializeReorderableList<BlendShape>(blendShapePairList);
                    }
                    else
                    {
                        blendShapeReorderableList = null;
                    }
                }
            }

            if (blendShapeReorderableList != null)
            {
                using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scroll.scrollPosition;
                    blendShapeReorderableList.DoLayoutList();
                }
            }

            using (new EditorGUI.DisabledGroupScope(renderer == null))
            {
                EditorGUILayout.LabelField("Auto Sort");
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("UnSort"))
                    {
                        blendShapeReorderableList.list
                            = blendShapeReorderableList.list
                            .Cast<BlendShape>()
                            .OrderBy(x => x.index)
                            .ToList();
                    }

                    if (GUILayout.Button("VRChat Default"))
                    {
                        blendShapeReorderableList.list
                         = SortByVRChatDefault(
                             blendShapeReorderableList.list as List<BlendShape>
                           );
                    }

                    if (GUILayout.Button("A-Z"))
                    {
                        blendShapeReorderableList.list
                            = blendShapeReorderableList.list
                            .Cast<BlendShape>()
                            .OrderBy(x => x.name)
                            .ToList();
                    }

                    if (GUILayout.Button("Z-A"))
                    {
                        blendShapeReorderableList.list
                            = blendShapeReorderableList.list
                            .Cast<BlendShape>()
                            .OrderByDescending(x => x.name)
                            .ToList();
                    }
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Change ShapeKey order"))
                {
                    CreateNewShapeKeyNameMesh(renderer, blendShapeReorderableList.list as List<BlendShape>);
                }
            }
        }

        /// <summary>
        /// シェイプキーの順番を変えたメッシュを作成し, SkinnedMeshRendererに設定する
        /// </summary>
        /// <param name="renderer">シェイプキーの順番を変更したいメッシュを持つSkinnedMeshRenderer</param>
        /// <param name="reorderdBlendShapeList">変更後のシェイプキーの名称のリスト</param>
        /// <returns></returns>
        private bool CreateNewShapeKeyNameMesh(SkinnedMeshRenderer renderer, List<BlendShape> reorderdBlendShapeList)
        {
            var mesh = renderer.sharedMesh;
            if (mesh == null) return false;

            if (reorderdBlendShapeList.Count != mesh.blendShapeCount) return false;

            var mesh_custom = Object.Instantiate<Mesh>(mesh);

            mesh_custom.ClearBlendShapes();

            var blendShapeIndex = 0;
            var frameIndex = 0;
            var shapeKeyName = "";
            Vector3[] deltaVertices, deltaNormals, deltaTangents;
            for (int i = 0; i < reorderdBlendShapeList.Count; i++)
            {
                deltaVertices = new Vector3[mesh.vertexCount];
                deltaNormals = new Vector3[mesh.vertexCount];
                deltaTangents = new Vector3[mesh.vertexCount];

                blendShapeIndex = reorderdBlendShapeList[i].index;

                mesh.GetBlendShapeFrameVertices(blendShapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);
                var weight = mesh.GetBlendShapeFrameWeight(blendShapeIndex, frameIndex);
                shapeKeyName = reorderdBlendShapeList[i].name;

                mesh_custom.AddBlendShapeFrame(shapeKeyName, weight, deltaVertices, deltaNormals, deltaTangents);
            }

            Undo.RecordObject(renderer, "Renderer " + renderer.name);
            renderer.sharedMesh = mesh_custom;

            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(mesh)) + "/" + mesh.name + "_reorderd.asset";
            AssetDatabase.CreateAsset(mesh_custom, AssetDatabase.GenerateUniqueAssetPath(path));
            AssetDatabase.SaveAssets();

            return true;
        }

        /// <summary>
        /// シェイプキーの順番をVRChat標準のものに変更する
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<BlendShape> SortByVRChatDefault(List<BlendShape> list)
        {
            var vrcBlendShapes
                = new string[]{
                    "vrc.blink_left",
                    "vrc.blink_right",
                    "vrc.lowerlid_left",
                    "vrc.lowerlid_right",
                };

            var newList = new List<BlendShape>();

            for (int i = 0; i < vrcBlendShapes.Length; i++)
            {
                var index = list.Select(x => x.name)
                                .ToList()
                                .IndexOf(vrcBlendShapes[i]);

                if (index == -1) continue;

                var blendShape = list[index];
                list.RemoveAt(index);
                newList.Add(blendShape);
            }

            newList.AddRange(list);

            return newList;
        }

        /// <summary>
        /// ReorderableListを作成する
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private ReorderableList InitializeReorderableList<T>(List<T> list)
        {
            var reorderbleList = new ReorderableList(list, typeof(T));
            reorderbleList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "BlendShape");
            reorderbleList.drawElementCallback = (rect, index, isActive, isFoused) =>
            {
                var item = reorderbleList.list[index] as BlendShape;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(rect, item.name);
            };
            reorderbleList.displayAdd = false;
            reorderbleList.displayRemove = false;

            return reorderbleList;
        }
    }
}

