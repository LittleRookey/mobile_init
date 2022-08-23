using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Data for an ability's SFX (Sound effects)
    /// </summary>
    [CreateAssetMenu(menuName = "Ability SFX", fileName = "abilitySfx.asset")]
    public class AbilitySFXSO : ScriptableObject
    {
        [SerializeField] private AudioClip _warmupSfx;
        [SerializeField] private AudioClip _castSfx;
        [SerializeField] private AudioClip[] _onHitSfx = new AudioClip[0];
        // [Header("Settings")]
        // [HelpBox("For this to work the ability must be targeted and cast on an IAbilityTarget")]
        // [SerializeField] private bool _playOnHitOnTarget;

        public AudioClip WarmupSfx => _warmupSfx;

        public AudioClip[] OnHitSfx => _onHitSfx;

        public AudioClip CastSfx => _castSfx;

    }
}