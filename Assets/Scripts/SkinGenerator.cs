using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SkinGenerator : MonoBehaviour
{
	[SerializeField] private SkinnedMeshRenderer _headRenderer;
	[SerializeField] private SkinnedMeshRenderer _bodyRenderer;
	[SerializeField] private SkinnedMeshRenderer _legsRenderer;
	[SerializeField] private SkinnedMeshRenderer _backpackRenderer;
	[SerializeField] private Transform _skeletonRoot;

	public void Setup(TeamSkinPreset skinPreset)
	{
		CopyRenderer(skinPreset.SkinPrefab.GetRandomHead(), _headRenderer);
		CopyRenderer(skinPreset.SkinPrefab.GetRandomBody(), _bodyRenderer);
		CopyRenderer(skinPreset.SkinPrefab.GetRandomLegs(), _legsRenderer);
		CopyRenderer(skinPreset.SkinPrefab.GetRandomBackpack(), _backpackRenderer);
	}

	private void CopyRenderer(SkinnedMeshRenderer origin, SkinnedMeshRenderer target)
	{
		if (origin == null || target == null)
		{
			return;
		}

		target.sharedMaterial = origin.sharedMaterial;
		target.sharedMesh = origin.sharedMesh;

		var childrenBones = _skeletonRoot.GetComponentsInChildren<Transform>();
		var allBones = childrenBones.ToDictionary(b => b.name);
		var originBones = origin.bones;
		var targetBones = new List<Transform>();
		foreach(var b in originBones)
		{
			if (allBones.TryGetValue(b.name, out var foundBone))
			{
				targetBones.Add(foundBone);
			}
		}
		target.bones = targetBones.ToArray();
	}
}