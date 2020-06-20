using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Threading.Tasks;
using System;
using UnityEditor.Animations;
#if VRC_SDK_VRCSDK2
using VRCSDK2;
#endif

// ver 1.0
// Copyright (c) 2020 gatosyocora

namespace VRCDeveloperTool
{
    public class VRCAvatarAnimationTester : EditorWindow
    {
#if VRC_SDK_VRCSDK2
		public VRC_AvatarDescriptor avatar;
#endif
		private Animator animator;
		private AnimatorOverrideController controller;
		private RuntimeAnimatorController defaultController;

		private Task emoteTask = null;

		public enum PlayingType 
		{
			NONE, OVERRIDE, EMOTE
		};
		private PlayingType playingType = PlayingType.NONE;
		public enum PlayingHand
        {
			NONE, RIGHT, LEFT, BOTH
        };
		private PlayingHand playingHand = PlayingHand.NONE;

		private AvatarMask mask;

		private static readonly string[] OVERRIDES = new string[]
		{
			"FIST", "HANDOPEN", "FINGERPOINT", "VICTORY", "ROCKNROLL", "HANDGUN", "THUMBSUP"
		};

		private static readonly string[] EMOTES = new string[]
		{
			"EMOTE1", "EMOTE2", "EMOTE3", "EMOTE4", "EMOTE5", "EMOTE6", "EMOTE7", "EMOTE8"
		};

		[MenuItem("VRCDeveloperTool/VRCAvatarAnimationTester")]
		public static void Open()
		{
			GetWindow<VRCAvatarAnimationTester>("VRCAvatarAnimationTester");
		}

        private void OnEnable()
        {
			// mask = Resources.Load<AvatarMask>("Tester/VRCAvatarMask") as AvatarMask;
		}

        private void Update()
        {
			// 再生中
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
				animator.runtimeAnimatorController = controller;

				if (playingType == PlayingType.OVERRIDE)
                {
					animator.SetInteger($"Emote", 0);
                    switch (playingHand)
                    {
                        case PlayingHand.NONE:
							animator.SetLayerWeight(animator.GetLayerIndex("HandLeft"), 0);
							animator.SetLayerWeight(animator.GetLayerIndex("HandRight"), 0);
							break;
                        case PlayingHand.RIGHT:
							animator.SetLayerWeight(animator.GetLayerIndex("HandLeft"), 0);
							animator.SetLayerWeight(animator.GetLayerIndex("HandRight"), 1);
							break;
                        case PlayingHand.LEFT:
							animator.SetLayerWeight(animator.GetLayerIndex("HandLeft"), 1);
							animator.SetLayerWeight(animator.GetLayerIndex("HandRight"), 0);
							break;
                        case PlayingHand.BOTH:
							animator.SetLayerWeight(animator.GetLayerIndex("HandLeft"), 1);
							animator.SetLayerWeight(animator.GetLayerIndex("HandRight"), 1);
							break;
                        default:
                            break;
                    }
				}
				else
                {
					animator.SetLayerWeight(animator.GetLayerIndex("HandLeft"), 0);
					animator.SetLayerWeight(animator.GetLayerIndex("HandRight"), 0);
				}
				// TODO: maskを設定すればしゃがみを回避できるがRuntimeにavatarMaskを変更できない
				//var animatorController = controller.runtimeAnimatorController as AnimatorController;
				//animatorController.layers[3].avatarMask = mask;
			}
			// 未再生
			else
            {
				if (animator != null && controller != null)
				{
					animator.runtimeAnimatorController = defaultController;
					animator.SetLayerWeight(animator.GetLayerIndex("HandLeft"), 0);
					animator.SetLayerWeight(animator.GetLayerIndex("HandRight"), 0);
					//var animatorController = controller.runtimeAnimatorController as AnimatorController;
					//animatorController.layers[3].avatarMask = null;
				}
			}
        }

        private void OnGUI()
		{
#if VRC_SDK_VRCSDK2
			using (var check = new EditorGUI.ChangeCheckScope())
            {
				avatar = EditorGUILayout.ObjectField("Avatar", avatar, typeof(VRC_AvatarDescriptor), true) as VRC_AvatarDescriptor;
				
				if (check.changed)
                {
					if (avatar != null)
                    {
						animator = avatar.gameObject.GetComponent<Animator>();
						controller = avatar.CustomStandingAnims;
					}
                    else
                    {
						animator = null;
						controller = null;
                    }
                }
			}
#endif
			using (new EditorGUILayout.HorizontalScope())
            {
				using (new EditorGUI.DisabledGroupScope(EditorApplication.isPlayingOrWillChangePlaymode || avatar == null))
                {
					if (GUILayout.Button("Play"))
					{
						defaultController = animator.runtimeAnimatorController;
						animator.runtimeAnimatorController = controller;
						EditorApplication.isPlaying = true;
					}
				}
				using (new EditorGUI.DisabledGroupScope(!EditorApplication.isPlayingOrWillChangePlaymode))
				{
					if (GUILayout.Button("Stop"))
					{
						EditorApplication.isPlaying = false;
						animator.runtimeAnimatorController = defaultController;
					}
				}
			}

			if (avatar == null && !EditorApplication.isPlaying)
            {
				EditorGUILayout.HelpBox("Avatarを設定してください", MessageType.Error);
            }

			if (avatar != null && animator != null && controller != null)
			{
				EditorGUILayout.HelpBox("Playを選択するとテストが実行できます", MessageType.Info);
			}

			EditorGUILayout.Space();

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Scene View"))
				{
					EditorApplication.ExecuteMenuItem("Window/General/Scene");
				}
				if (GUILayout.Button("Game View"))
				{
					EditorApplication.ExecuteMenuItem("Window/General/Game");
				}
			}

			EditorGUILayout.Space();

			using (new EditorGUI.DisabledGroupScope(!EditorApplication.isPlayingOrWillChangePlaymode))
            {
				if (GUILayout.Button("Reset All"))
				{
					playingType = PlayingType.NONE;
					playingHand = PlayingHand.NONE;
				}

				EditorGUILayout.Space();

				EditorGUILayout.LabelField("AnimationOverrides", EditorStyles.boldLabel);
				using (new EditorGUILayout.HorizontalScope())
                {
					EditorGUILayout.LabelField("NONE");
					if (GUILayout.Button("Left"))
                    {
						if (playingType == PlayingType.OVERRIDE &&
							playingHand == PlayingHand.BOTH)
                        {
							playingHand = PlayingHand.RIGHT;
						}
                        else
                        {
							playingType = PlayingType.NONE;
						}
						PlayOverride("Left", 0, animator);
					}
					if (GUILayout.Button("Right"))
					{
						if (playingType == PlayingType.OVERRIDE &&
							playingHand == PlayingHand.BOTH)
						{
							playingHand = PlayingHand.LEFT;
						}
						else
						{
							playingType = PlayingType.NONE;
						}
						PlayOverride("Right", 0, animator);
					}
				}
				for (int overrideNumber = 0; overrideNumber < OVERRIDES.Length; overrideNumber++)
				{
					AnimationClip overrideAnimation = null;
					string overrideName = string.Empty;

					if (controller != null)
					{
						overrideAnimation = controller[OVERRIDES[overrideNumber]];

						if (overrideAnimation.name != OVERRIDES[overrideNumber])
						{
							overrideName = $"({overrideAnimation.name})";
						}
					}

					// AnimationClipとOVERRIDES[overrideNumber]の名前が同じであれば未設定
					using (new EditorGUI.DisabledGroupScope(controller == null || overrideAnimation.name == OVERRIDES[overrideNumber]))
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField($"{OVERRIDES[overrideNumber]}{overrideName}");

						if (GUILayout.Button("Left"))
						{
							playingType = PlayingType.OVERRIDE;
							if (playingHand == PlayingHand.RIGHT ||
								playingHand == PlayingHand.BOTH)
                            {
								playingHand = PlayingHand.BOTH;
                            }
                            else
                            {
								playingHand = PlayingHand.LEFT;
                            }
							PlayOverride("Left", overrideNumber, animator);
						}

						if (GUILayout.Button("Right"))
						{
							playingType = PlayingType.OVERRIDE;
							if (playingHand == PlayingHand.LEFT ||
								playingHand == PlayingHand.BOTH)
							{
								playingHand = PlayingHand.BOTH;
							}
							else
							{
								playingHand = PlayingHand.RIGHT;
							}
							PlayOverride("Right", overrideNumber, animator);
						}
					}
				}
			}

			EditorGUILayout.Space();

			using (new EditorGUI.DisabledGroupScope(!EditorApplication.isPlayingOrWillChangePlaymode))
			{
				EditorGUILayout.LabelField("Emotes", EditorStyles.boldLabel);
				for (int emoteNumber = 0; emoteNumber < EMOTES.Length; emoteNumber++)
				{
					AnimationClip emoteAnimation = null;
					string buttonText = EMOTES[emoteNumber];
					if (controller != null)
                    {
						emoteAnimation = controller[EMOTES[emoteNumber]];
						buttonText = emoteAnimation.name;
					}
					
					// 取得できるAnimationClipの名前が"EMOTE*"だったら設定されていない
					using (new EditorGUI.DisabledGroupScope(emoteAnimation == null || emoteAnimation.name == EMOTES[emoteNumber]))
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField(EMOTES[emoteNumber]);

						if (GUILayout.Button(buttonText) && emoteAnimation != null)
						{
							if (animator.GetInteger($"Emote") != 0) return;

							playingType = PlayingType.EMOTE;
							playingHand = PlayingHand.NONE;
							PlayEmote(emoteNumber + 1, animator, emoteAnimation);
						}
					}
				}
			}
		}

		private void PlayOverride(string hand, int overrideNumber, Animator animator)
		{
			animator.SetInteger($"HandGesture{hand}", overrideNumber);
		}

		private async void PlayEmote(int emoteNumber, Animator animator, AnimationClip animationClip)
        {
			int waitMilliSecond = (int)(animationClip.length * 1000);
			if (animationClip == null) waitMilliSecond = 1000;
			animator.SetInteger($"Emote", emoteNumber);
			await Task.Delay(waitMilliSecond);
			animator.SetInteger($"Emote", 0);
        }
    }
}