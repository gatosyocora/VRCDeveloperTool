using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

// ver 1.1
// © 2018-9-24 gatosyocora

public class HandPoseAdder : Editor {

    private static string ORIGIN_ANIM_PATH = "Assets/VRCDeveloperTool/Editor/HandPoseAdder/Animations/"; // コピー元となるAnimationファイルが置いてあるディレクトリのパス

    // FINGER POINT
    [MenuItem("CONTEXT/Motion/Add Hand pose 'FINGER POINT'", false, 1)]
    private static void AddFPAnimationKeys(MenuCommand command)
    {
        AddHandPoseAnimationKeys(command, ORIGIN_ANIM_PATH + "FingerPoint.anim");
    }

    // FIST
    [MenuItem("CONTEXT/Motion/Add Hand pose 'FIST", false, 2)]
    private static void AddFISTAnimationKeys(MenuCommand command)
    {
        AddHandPoseAnimationKeys(command, ORIGIN_ANIM_PATH + "Fist.anim");
    }

    // HAND GUN
    [MenuItem("CONTEXT/Motion/Add Hand pose 'HAND GUN'", false, 3)]
    private static void AddHGAnimationKeys(MenuCommand command)
    {
        AddHandPoseAnimationKeys(command, ORIGIN_ANIM_PATH + "HandGun.anim");
    }

    // HAND OPEN
    [MenuItem("CONTEXT/Motion/Add Hand pose 'HAND OPEN'", false, 4)]
    private static void AddHOAnimationKeys(MenuCommand command)
    {
        AddHandPoseAnimationKeys(command, ORIGIN_ANIM_PATH + "HandOpen.anim");
    }

    // ROCKN ROLL
    [MenuItem("CONTEXT/Motion/Add Hand pose 'ROCKN ROLL'", false, 5)]
    private static void AddRRAnimationKeys(MenuCommand command)
    {
        AddHandPoseAnimationKeys(command, ORIGIN_ANIM_PATH + "RocknRoll.anim");
    }

    // THUMBS UP
    [MenuItem("CONTEXT/Motion/Add Hand pose 'THUMBS UP'", false, 6)]
    private static void AddTUAnimationKeys(MenuCommand command)
    {
        AddHandPoseAnimationKeys(command, ORIGIN_ANIM_PATH + "ThumbsUp.anim");
    }


    // VICTORY
    [MenuItem("CONTEXT/Motion/Add Hand pose 'VICTORY'", false, 7)]
    private static void AddVICTORYAnimationKeys(MenuCommand command)
    {
        AddHandPoseAnimationKeys(command, ORIGIN_ANIM_PATH + "Victory.anim");
    }

    // 特定のAnimationファイルのAnimationキー全てをコピーする
    public static void AddHandPoseAnimationKeys(MenuCommand command, string originPath)
    {
        AnimationClip targetClip = command.context as AnimationClip;

        AnimationClip originClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(originPath, typeof(AnimationClip));         // originPathよりAnimationClipの読み込み

        CopyAnimationKeys(originClip, targetClip);
    }

    // originClipに設定されたAnimationKeyをすべてtargetclipにコピーする
    public static void CopyAnimationKeys(AnimationClip originClip, AnimationClip targetClip)
    {
        foreach (var binding in AnimationUtility.GetCurveBindings(originClip).ToArray())
        {
            // AnimationClipよりAnimationCurveを取得
            AnimationCurve curve = AnimationUtility.GetEditorCurve(originClip, binding);
            // AnimationClipにキーリダクションを行ったAnimationCurveを設定
            AnimationUtility.SetEditorCurve(targetClip, binding, curve);
        }
    }

}
