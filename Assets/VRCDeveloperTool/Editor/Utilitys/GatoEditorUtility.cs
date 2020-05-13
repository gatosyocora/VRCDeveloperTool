using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

// MIT License
/*
 * Copyright 2020 gatosyocora
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

namespace VRCDeveloperTool
{
    public class GatoEditorUtility
    {
        private const char BSLASH = '\\';

        /// <summary>
        /// インデントなしのHelpbox
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        public static void NonIndentHelpBox(string message, MessageType messageType)
        {
            var currentIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.HelpBox(message, messageType);
            EditorGUI.indentLevel = currentIndentLevel;
        }

        /// <summary>
        /// インデントなしのButton
        /// </summary>
        /// <param name="text"></param>
        /// <param name="action"></param>
        public static void NonIndentButton(string text, Action action)
        {
            var currentIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            if (GUILayout.Button(text))
            {
                action.Invoke();
            }
            EditorGUI.indentLevel = currentIndentLevel;
        }

        /// <summary>
        /// パス内で存在しないフォルダを作成する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CreateNoExistFolders(string path)
        {
            string directoryPath;
            if (string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                directoryPath = path;
            }
            else
            {
                directoryPath = Path.GetDirectoryName(path);
            }

            if (!Directory.Exists(directoryPath))
            {
                var directories = directoryPath.Split(BSLASH);

                directoryPath = "Assets";
                for (int i = 1; i < directories.Length; i++)
                {
                    if (!Directory.Exists(directoryPath + BSLASH + directories[i]))
                    {
                        AssetDatabase.CreateFolder(directoryPath, directories[i]);
                    }

                    directoryPath += BSLASH + directories[i];
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 任意のアセットを複製する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="newAssetName"></param>
        /// <param name="saveFolderPath"></param>
        /// <returns></returns>
        public static T DuplicateAsset<T>(T source, string newAssetPath) where T : UnityEngine.Object
        {
            var sourcePath = AssetDatabase.GetAssetPath(source);
            return DuplicateAsset<T>(sourcePath, newAssetPath);
        }

        public static T DuplicateAsset<T>(string sourcePath, string newAssetPath) where T : UnityEngine.Object
        {
            var newFolderPath = Path.GetDirectoryName(newAssetPath);
            CreateNoExistFolders(newFolderPath);
            var newPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);
            AssetDatabase.CopyAsset(sourcePath, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var newAsset = AssetDatabase.LoadAssetAtPath(newPath, typeof(T)) as T;

            return newAsset;
        }

        /// <summary>
        /// 最後にキーワードを追加する（重複なし）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string AddKeywordToEnd(string target, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return target;

            var normalString = Regex.Replace(target, keyword + ".*", string.Empty);
            return normalString + keyword;
        }

        /// <summary>
        /// 特定のオブジェクトから特定のオブジェクトまでのパスを取得する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetHierarchyPathFromObj1ToObj2(GameObject obj1, GameObject obj2)
        {
            string path = obj2.name;
            var parent = obj2.transform.parent;
            while (parent != null)
            {
                if (parent.gameObject.name == obj1.name) return path;

                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        /// <summary>
        /// 特定のオブジェクトまでのパスを取得する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetHierarchyPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                if (parent.parent == null) return path;

                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        /// <summary>
        /// フォルダ名からフォルダパスを取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFolderPathFromName(string folderName)
        {
            var guid = AssetDatabase.FindAssets(folderName + " t:Folder").FirstOrDefault();
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        /// <summary>
        /// 複製された2つのオブジェクト間で片方の特定のTransformに対応したTransformを取得する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="duplicated"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Transform GetCorrespondTransformBetweenDuplicatedObjects(GameObject source, GameObject duplicated, Transform target)
        {
            if (source.transform == target) return duplicated.transform;

            var path = GetHierarchyPathFromObj1ToObj2(source, target.gameObject);

            return duplicated.transform.Find(path);
        }
    }
}