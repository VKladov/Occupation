using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(ParticleSystem))]
public class VisualEffect : MonoBehaviour
{
    public event Action<VisualEffect> Stopped;
    
    public class Pool : MemoryPool<VisualEffect>
    {
        protected override void OnSpawned(VisualEffect item)
        {
            item.Stopped += ItemOnStopped;
        }

        private void ItemOnStopped(VisualEffect obj)
        {
            obj.Stopped -= ItemOnStopped;
            Despawn(obj);
        }
    }

    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Play(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(direction);
        _particleSystem.Play();
    }
    
    public void OnParticleSystemStopped()
    {
        Stopped?.Invoke(this);
    }
}
