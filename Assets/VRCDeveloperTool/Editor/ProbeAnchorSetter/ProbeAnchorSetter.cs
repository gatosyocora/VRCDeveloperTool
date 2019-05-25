using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ver 1.0
// © 2019-2-16 gatosyocora

public class ProbeAnchorSetter : EditorWindow
{

    private GameObject targetObject = null;

    public enum TARGETPOS
    {
        HEAD,
        CHEST,
        //ARMATURE,
        ROOTOBJECT,
    }

    private TARGETPOS targetPos = TARGETPOS.HEAD;

    private const string TARGETOBJNAME = "Anchor Target";

    private bool isGettingSkinnedMeshRenderer = true;
    private bool isGettingMeshRenderer = true;

    private bool isOpeningRendererList = false;

    private List<SkinnedMeshRenderer> skinnedMeshList;
    private List<MeshRenderer> meshList;

    private bool[] isSettingToSkinnedMesh = null;
    private bool[] isSettingToMesh = null;

    private Vector2 leftScrollPos = Vector2.zero;

    [MenuItem("VRCDeveloperTool/ProbeAnchor Setter")]
    private static void Create()
    {
        GetWindow< ProbeAnchorSetter > ("ProbeAnchor Setter");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        targetObject = EditorGUILayout.ObjectField(
            "TargetObject",
            targetObject,
            typeof(GameObject),
            true
        ) as GameObject;

        if (EditorGUI.EndChangeCheck())
        {
            if (targetObject != null)
            {
                skinnedMeshList = GetSkinnedMeshList(targetObject);
                meshList = GetMeshList(targetObject);
                isSettingToSkinnedMesh = new bool[skinnedMeshList.Count];
                for (int i = 0; i < skinnedMeshList.Count; i++) isSettingToSkinnedMesh[i] = true;
                isSettingToMesh = new bool[meshList.Count];
                for (int i = 0; i < meshList.Count; i++) isSettingToMesh[i] = true;
            }
        }
        
        // 設定するRendererの選択
        isGettingSkinnedMeshRenderer = EditorGUILayout.Toggle("Set To SkinnedMeshRenderer", isGettingSkinnedMeshRenderer);
        isGettingMeshRenderer = EditorGUILayout.Toggle("Set To MeshRenderer", isGettingMeshRenderer);

        // ライティングの計算の基準とする位置を選択
        targetPos = (TARGETPOS)EditorGUILayout.EnumPopup("TargetPosition", targetPos);

        // Rendererの一覧を表示
        if (targetObject != null)
        {
            isOpeningRendererList = EditorGUILayout.Foldout(isOpeningRendererList, "Renderer List");

            if (isOpeningRendererList)
            {
                leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUI.skin.box);
                {

                    EditorGUI.indentLevel++;

                    int index = 0;

                    if (isGettingSkinnedMeshRenderer)
                    {
                        foreach (var skinnedMesh in skinnedMeshList)
                        {
                            EditorGUILayout.BeginHorizontal();
                            isSettingToSkinnedMesh[index] = EditorGUILayout.Toggle(skinnedMesh.gameObject.name, isSettingToSkinnedMesh[index]);
                            if (GUILayout.Button("Select"))
                                Selection.activeGameObject = skinnedMesh.gameObject;
                            EditorGUILayout.EndHorizontal();
                            index++;
                        }
                    }

                    index = 0;

                    if (isGettingMeshRenderer)
                    {
                        foreach (var mesh in meshList)
                        {
                            EditorGUILayout.BeginHorizontal();
                            isSettingToMesh[index] = EditorGUILayout.Toggle(mesh.gameObject.name, isSettingToMesh[index]);
                            if (GUILayout.Button("Select"))
                                Selection.activeGameObject = mesh.gameObject;
                            EditorGUILayout.EndHorizontal();
                            index++;
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndScrollView();
            }
        }

        EditorGUI.BeginDisabledGroup(targetObject == null);
        if (GUILayout.Button("Set ProbeAnchor"))
        {
            SetProbeAnchor(targetObject);
        }
        EditorGUI.EndDisabledGroup();

    }

    /// <summary>
    /// 特定のオブジェクト以下のRendererのProbeAnchorに設定する
    /// </summary>
    /// <param name="obj"></param>
    private void SetProbeAnchor(GameObject obj)
    {
        var animator = obj.GetComponent<Animator>();
        if (animator == null) return;

        // AnchorTargetを設定する基準の場所を取得
        Transform targetPosTrans = null;
        if (targetPos == TARGETPOS.HEAD)
        {
            targetPosTrans = animator.GetBoneTransform(HumanBodyBones.Head);
        }
        else if (targetPos == TARGETPOS.CHEST)
        {
            targetPosTrans = animator.GetBoneTransform(HumanBodyBones.Chest);
        }
        /*else if (targetPos == TARGETPOS.ARMATURE)
        {
            var hipsTrans = animator.GetBoneTransform(HumanBodyBones.Hips);
            targetPosTrans = hipsTrans.parent;
        }*/
        else if (targetPos == TARGETPOS.ROOTOBJECT)
        {
            targetPosTrans = obj.transform;
        }
        if (targetPosTrans == null) return;

        // AnchorTargetに設定用のオブジェクトを作成
        GameObject anchorTargetObj = GameObject.Find(obj.name + "/"+ TARGETOBJNAME);
        if (anchorTargetObj == null)
        {
            anchorTargetObj = new GameObject(TARGETOBJNAME);
            anchorTargetObj.transform.parent = obj.transform;
        }
        anchorTargetObj.transform.position = targetPosTrans.position;

        // SkiinedMeshRendererに設定
        if (isGettingSkinnedMeshRenderer)
        {
            int index = 0;
            var skinnedMeshes = skinnedMeshList;
            foreach (var skinnedMesh in skinnedMeshes)
            {
                if (isSettingToSkinnedMesh[index++])
                    skinnedMesh.probeAnchor = anchorTargetObj.transform;
                else
                    skinnedMesh.probeAnchor = null;
            }
        }

        // MeshRendererに設定
        if (isGettingMeshRenderer)
        {
            int index = 0;
            var meshes = meshList;
            foreach (var mesh in meshes)
            {
                if (isSettingToMesh[index++])
                    mesh.probeAnchor = anchorTargetObj.transform;
                else
                    mesh.probeAnchor = null;
            }
        }
    }

    /// <summary>
    /// 指定オブジェクト以下のSkinnedMeshRendererのリストを取得する
    /// </summary>
    /// <param name="parentObj">親オブジェクト</param>
    /// <returns>SkinnedMeshRendererのリスト</returns>
    private List<SkinnedMeshRenderer> GetSkinnedMeshList(GameObject parentObj)
    {
        var skinnedMeshList = new List<SkinnedMeshRenderer>();

        var skinnedMeshes = parentObj.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (var skinnedMesh in skinnedMeshes)
        {
            skinnedMeshList.Add(skinnedMesh);
        }

        return skinnedMeshList;
    }

    /// <summary>
    /// 指定オブジェクト以下のMeshRendererのリストを取得する
    /// </summary>
    /// <param name="parentObj">親オブジェクト</param>
    /// <returns>MeshRendererのリスト</returns>
    private List<MeshRenderer> GetMeshList(GameObject parentObj)
    {
        var meshList = new List<MeshRenderer>();

        var meshes = parentObj.GetComponentsInChildren<MeshRenderer>(true);

        foreach (var mesh in meshes)
        {
            meshList.Add(mesh);
        }

        return meshList;
    }


}
