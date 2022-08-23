using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations
{
    /// <summary>
    /// Simple marker component for quickly making a gameobject targetable by abilities.
    /// Use this if you don't want to manually implement <see cref="IAbilityTarget"/> in your own class
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AbilityTarget : MonoBehaviour, IAbilityTarget
    {
        [SerializeField] private AudioSource _audioSource;
        private Transform _transform;
        private Collider _collider;
        public Transform Transform => _transform;
        public AudioSource AudioSource => _audioSource;

        public Vector3 BasePosition => new Vector3(_transform.position.x,
            _collider.bounds.center.y - (_collider.bounds.size.y / 2), _transform.position.z);

        private void Awake()
        {
            _transform = transform;
            TryGetComponent(out _collider);
            TryGetComponent(out _audioSource);
        }
    }
}
