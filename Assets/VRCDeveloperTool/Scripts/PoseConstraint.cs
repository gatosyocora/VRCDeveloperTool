using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace VRCDeveloperTool 
{
    public class PoseConstraint : MonoBehaviour
	{
		[Serializable]
		public class BoneInfo
		{
			public Transform Transform { get; set; }
			public Vector3 Position { get; set; }
			public Quaternion Rotation { get; set; }
		}

		private List<BoneInfo> boneList;

		public Animator animator;
		public bool Active = true;

        public void Start()
        {
			boneList = GetBoneInfo(animator);
			Active = true;
		}

        private void LateUpdate()
		{
			if (Active && boneList != null) SetBoneInfo(boneList);
		}

		public void UpdateBoneInfo(Animator animator)
        {
			this.animator = animator;
			EditorUtility.SetDirty(this);
			boneList = GetBoneInfo(animator);
        }

		private List<BoneInfo> GetBoneInfo(Animator animator)
		{
			return Enum.GetNames(typeof(HumanBodyBones))
					.Select(boneName =>
					{
						if (Enum.TryParse(boneName, out HumanBodyBones bone))
						{
							Transform trans;
							try
							{
								trans = animator.GetBoneTransform(bone);
							}
							catch (IndexOutOfRangeException)
							{
								return null;
							}

							if (trans is null) return null;
							return new BoneInfo
							{
								Transform = trans,
								Position = trans.position,
								Rotation = trans.rotation
							};
						}
						else
						{
							return null;
						}
					})
					.Where(x => x != null)
					.ToList();
		}

		private void SetBoneInfo(List<BoneInfo> boneList)
		{
			foreach (var bone in boneList)
			{
				bone.Transform.position = bone.Position;
				bone.Transform.rotation = bone.Rotation;
			}
		}
    }
}