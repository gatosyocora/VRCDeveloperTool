using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ver 1.0
// © 2019 gatosyocora

public class AnimationClipCombiner : EditorWindow {

    private AnimationClip mainAnimClip = null;
    private List<AnimationClip> subAnimClip = null;
    private bool isCreateNewAnimClip = true;

    private string savedAnimFileName = "animFile";
    private string savedFolderPath = "";

    [MenuItem("VRCDeveloperTool/AnimationClip Combiner")]
    private static void Create()
    {
        GetWindow<AnimationClipCombiner> ("AnimationClip Combiner");
    }

    private void OnEnable()
    {
        subAnimClip = new List<AnimationClip>();
        subAnimClip.Add(null);
    }

    private void OnGUI()
    {
        mainAnimClip = EditorGUILayout.ObjectField(
            "Main AnimationClip",
            mainAnimClip,
            typeof(AnimationClip),
            true
        ) as AnimationClip;



        for (int i = 0; i < subAnimClip.Count; i++)
        {
            subAnimClip[i] = EditorGUILayout.ObjectField(
                "Sub AnimationClip " + (i+1),
                subAnimClip[i],
                typeof(AnimationClip),
                true
            ) as AnimationClip;
        }

        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("+"))
            {
                subAnimClip.Add(null);
            }
            if (GUILayout.Button("-"))
            {
                if (subAnimClip.Count > 1)
                    subAnimClip.RemoveAt(subAnimClip.Count - 1);
            }
        }

        isCreateNewAnimClip = EditorGUILayout.ToggleLeft("新しいAnimationファイルとして作成する", isCreateNewAnimClip);
        
        if (isCreateNewAnimClip)
        {
            savedAnimFileName = EditorGUILayout.TextField("保存ファイル名", savedAnimFileName);
            EditorGUILayout.LabelField("保存先フォルダ", savedFolderPath);
        }
        
        if (GUILayout.Button("Combine Animation Clips"))
        {
            CombineAnimationClips();
        }
    }

    private void CombineAnimationClips()
    {

    }

}
