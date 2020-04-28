using UnityEngine;
using UnityEditor;
#if VRC_SDK_VRCSDK2
using VRCSDK2;
#endif
using System.Collections.Generic;

// ver 1.21
// © 2018 gatosyocora

namespace VRCDeveloperTool
{
    public class ComponentAdder : EditorWindow
    {

        GameObject targetObject = null;

        private enum AddType
        {
            Current_Children_Only,
            All_Childrens,
        };

        AddType addType;

        private bool isRigidbody = false;
        private bool useGravityFlag = true;
        private bool isKinematicFlag = false;
        private bool freezePosFlag = false;
        private bool freezeRotFlag = false;

        private bool isObjectSync = false;
        private bool syncPhysicsFlag = true;
        private bool collisionTransferFlag = true;

        private bool isPickup = false;

        private bool isBoxCollider = false;
        private bool isTriggerFlag = false;

        [MenuItem("VRCDeveloperTool/Component Adder")]
        private static void Create()
        {
            GetWindow<ComponentAdder>("Component Adder");
        }

        private void OnGUI()
        {
            targetObject = EditorGUILayout.ObjectField(
                "ParentObject",
                targetObject,
                typeof(GameObject),
                true
            ) as GameObject;

            addType = (AddType)EditorGUILayout.EnumPopup("Add Type", addType);

            guiRigidbody();

            guiObjectSync();

            guiPickup();

            guiBoxCollider();

            guiAction();

        }

        // 指定オブジェクトの直接的な子オブジェクトをすべて取得する
        private List<GameObject> getCurrentChildrens(GameObject parentObj)
        {
            List<GameObject> objs = new List<GameObject>(parentObj.transform.childCount);

            foreach (Transform child in parentObj.transform)
            {
                objs.Add(child.gameObject);

            }
            return objs;
        }

        // 指定オブジェクトの子オブジェクト以降をすべて取得する
        private List<GameObject> getAllChildrens(GameObject parentObj)
        {
            List<GameObject> objs = new List<GameObject>();

            var childTransform = parentObj.GetComponentsInChildren<Transform>();

            foreach (Transform child in childTransform)
            {
                objs.Add(child.gameObject);
            }

            return objs;
        }

        // 特定のオブジェクトにコンポーネントを追加する
        private void AddComponentObject(GameObject obj)
        {
            if (isRigidbody)
            {
                var rigid = obj.GetComponent<Rigidbody>();
                if (rigid == null)
                    rigid = obj.AddComponent<Rigidbody>();
                rigid.isKinematic = isKinematicFlag;
                rigid.useGravity = useGravityFlag;
                rigid.constraints = 0;
                if (freezePosFlag) rigid.constraints |= RigidbodyConstraints.FreezePosition;
                if (freezeRotFlag) rigid.constraints |= RigidbodyConstraints.FreezeRotation;
            }
            if (isObjectSync)
            {
#if VRC_SDK_VRCSDK2
                if (obj.GetComponent<VRC_ObjectSync>() == null)
                {
                    var com = obj.AddComponent<VRC_ObjectSync>();
                }
#endif
            }
            if (isPickup)
            {
#if VRC_SDK_VRCSDK2
                if (obj.GetComponent<VRC_Pickup>() == null)
                {
                    var com = obj.AddComponent<VRC_Pickup>();
                }
#endif
            }
            if (isBoxCollider)
            {
                if (obj.GetComponent<Collider>() == null || obj.GetComponent<BoxCollider>() != null)
                {
                    var com = obj.GetComponent<BoxCollider>();
                    if (com == null)
                        com = obj.AddComponent<BoxCollider>();
                    com.isTrigger = isTriggerFlag;
                }
            }
        }

        // 特定のオブジェクトのコンポーネントを削除する
        private void DeleteComponentObject(GameObject obj)
        {
            if (isPickup)
            {
#if VRC_SDK_VRCSDK2
                var com = obj.GetComponent<VRC_Pickup>();
                if (com != null) DestroyImmediate(com);
#endif
            }
            if (isRigidbody)
            {
                var com = obj.GetComponent<Rigidbody>();
                if (com != null) DestroyImmediate(com);
            }
            if (isObjectSync)
            {
#if VRC_SDK_VRCSDK2
                var com = obj.GetComponent<VRC_ObjectSync>();
                if (com != null) DestroyImmediate(com);
#endif
            }
            if (isBoxCollider)
            {
                var com = obj.GetComponent<BoxCollider>();
                if (com != null) DestroyImmediate(com);
            }
        }

        private void guiRigidbody()
        {
            isRigidbody = EditorGUILayout.BeginToggleGroup("Rigidbody", isRigidbody);
            if (isRigidbody)
            {
                useGravityFlag = EditorGUILayout.Toggle("useGravity", useGravityFlag);
                isKinematicFlag = EditorGUILayout.Toggle("isKinematic", isKinematicFlag);
                freezePosFlag = EditorGUILayout.Toggle("Freeze Positions", freezePosFlag);
                freezeRotFlag = EditorGUILayout.Toggle("Freeze Rotations", freezeRotFlag);
            }
            EditorGUILayout.EndToggleGroup();
        }

        private void guiObjectSync()
        {
            isObjectSync = EditorGUILayout.BeginToggleGroup("VRC_ObjectSync", isObjectSync);
            //syncPhysicsFlag = EditorGUILayout.Toggle("Synchronize Physics", syncPhysicsFlag);
            //collisionTransferFlag = EditorGUILayout.Toggle("Collision Transfer", collisionTransferFlag);
#if VRC_SDK_VRCSDK2
#else
            if (isObjectSync)
            {
                EditorGUILayout.HelpBox("VRCSDK2をインポートしてください", MessageType.Error);
            }
#endif
            EditorGUILayout.EndToggleGroup();
        }

        private void guiPickup()
        {
            isPickup = EditorGUILayout.BeginToggleGroup("VRC_Pickup", isPickup);
#if VRC_SDK_VRCSDK2
#else
            if (isPickup)
            {
                EditorGUILayout.HelpBox("VRCSDK2をインポートしてください", MessageType.Error);
            }
#endif
            EditorGUILayout.EndToggleGroup();
        }

        private void guiBoxCollider()
        {
            isBoxCollider = EditorGUILayout.BeginToggleGroup("BoxCollider", isBoxCollider);
            if (isBoxCollider)
            {
                isTriggerFlag = EditorGUILayout.Toggle("isTrigger", isTriggerFlag);
            }
            EditorGUILayout.EndToggleGroup();
        }

        private void guiAction()
        {

            EditorGUI.BeginDisabledGroup(targetObject == null);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add/Change Components"))
            {
                List<GameObject> objs;

                if (addType == AddType.Current_Children_Only)
                {
                    objs = getCurrentChildrens(targetObject);

                }
                else
                {
                    objs = getAllChildrens(targetObject);
                }

                foreach (GameObject obj in objs)
                {
                    AddComponentObject(obj);
                }
            }
            if (GUILayout.Button("Delete Components"))
            {
                List<GameObject> objs;

                if (addType == AddType.Current_Children_Only)
                {
                    objs = getCurrentChildrens(targetObject);

                }
                else
                {
                    objs = getAllChildrens(targetObject);
                }

                foreach (GameObject obj in objs)
                {
                    DeleteComponentObject(obj);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }
    }
}

