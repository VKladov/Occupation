using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonSkinSource : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] _heads;
    [SerializeField] private SkinnedMeshRenderer[] _bodies;
    [SerializeField] private SkinnedMeshRenderer[] _feet;
    [SerializeField] private SkinnedMeshRenderer[] _backpacks;

    public SkinnedMeshRenderer GetRandomHead() => _heads.Random();
    public SkinnedMeshRenderer GetRandomBody() => _bodies.Random();
    public SkinnedMeshRenderer GetRandomLegs() => _feet.Random();
    public SkinnedMeshRenderer GetRandomBackpack() => _backpacks?.Length > 0 ? _backpacks?.Random() : null;
}
