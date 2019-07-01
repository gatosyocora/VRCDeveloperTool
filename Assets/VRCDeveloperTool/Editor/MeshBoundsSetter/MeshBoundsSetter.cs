using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// ver 1.02
// © 2019-2-1 gatosyocora

namespace VRCDeveloperTool
{

    public class MeshBoundsSetter : EditorWindow
    {

        private GameObject targetObject = null;

        private Vector3 boundsScale = new Vector3(1, 2, 1);

        private List<GameObject> exclusions = new List<GameObject>();

        [MenuItem("VRCDeveloperTool/MeshBounds Setter")]
        private static void Create()
        {
            GetWindow<MeshBoundsSetter>("MeshBounds Setter");
        }

        private void OnGUI()
        {
            targetObject = EditorGUILayout.ObjectField(
                "TargetObject",
                targetObject,
                typeof(GameObject),
                true
            ) as GameObject;

            boundsScale = EditorGUILayout.Vector3Field("Bounds Scale", boundsScale);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Exclusions");

            if (GUILayout.Button("+"))
            {
                exclusions.Add(null);
            }
            if (GUILayout.Button("-"))
            {
                if (exclusions.Count > 0)
                    exclusions.RemoveAt(exclusions.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < exclusions.Count; i++)
            {
                exclusions[i] = EditorGUILayout.ObjectField(
                    "Object " + (i + 1),
                    exclusions[i],
                    typeof(GameObject),
                    true
                ) as GameObject;
            }

            guiAction();

        }

        private void guiAction()
        {

            EditorGUI.BeginDisabledGroup(targetObject == null);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Bounds"))
            {
                BoundsSetter(targetObject);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private void BoundsSetter(GameObject parentObj)
        {
            var objs = GetAllChildrens(parentObj);

            foreach (var obj in objs)
            {
                // 除外リストに含まれていれば処理しない
                if (exclusions.Contains(obj)) continue;

                var mesh = obj.GetComponent<MeshRenderer>();
                var skinnedMesh = obj.GetComponent<SkinnedMeshRenderer>();

                if (mesh == null && skinnedMesh == null) continue;

                // Mesh Rendererの場合
                if (mesh != null)
                {
                    Undo.RecordObject(mesh, "Change Transform " + mesh.name);
                }
                // SkinnedMeshRendererの場合
                else
                {
                    Undo.RecordObject(skinnedMesh, "Change Transform " + skinnedMesh.name);

                    var objScale = skinnedMesh.gameObject.transform.localScale;
                    var meshBoundsScale = new Vector3(boundsScale.x / objScale.x, boundsScale.y / objScale.y, boundsScale.z / objScale.z);
                    skinnedMesh.localBounds = new Bounds(Vector3.zero, meshBoundsScale);
                }

            }

        }

        // 指定オブジェクトの子オブジェクト以降をすべて取得する
        private List<GameObject> GetAllChildrens(GameObject parentObj)
        {
            List<GameObject> objs = new List<GameObject>();

            var childTransform = parentObj.GetComponentsInChildren<Transform>();

            foreach (Transform child in childTransform)
            {
                objs.Add(child.gameObject);
            }

            return objs;
        }
    }
}

