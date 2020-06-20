using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoseConstraint : MonoBehaviour
{
	public class BoneInfo
	{
		public Transform transform { get; set; }
		public Vector3 position { get; set; }
		public Quaternion rotation { get; set; }
	}

	private List<BoneInfo> boneList;
	private Animator animator;

	// Start is called before the first frame update
	private void Start()
    {
		animator = GetComponent<Animator>();
		boneList = GetBoneInfo(animator);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
		SetBoneInfo(boneList);
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
						Debug.Log($"{boneName}");
						return new BoneInfo
						{
							transform = trans,
							position = trans.position,
							rotation = trans.rotation
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
			bone.transform.position = bone.position;
			bone.transform.rotation = bone.rotation;
		}
	}
}
