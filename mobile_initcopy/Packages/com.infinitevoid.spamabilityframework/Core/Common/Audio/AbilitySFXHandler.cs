using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Audio
{
    /// <summary>
    /// Handles SFX for a given ability
    /// </summary>
    public class AbilitySFXHandler
    {
        private readonly AudioSource _audioSource;
        private readonly AbilitySFXSO _abilitySfx;
        private readonly bool _canPlayWarmup;
        private readonly bool _canPlayCast;

        /// <summary>
        /// Creates a new <see cref="AbilitySFXHandler"/> with the given AudioSource (caster's) and sfx-mappings
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="abilitySfx"></param>
        public AbilitySFXHandler(AudioSource audioSource, AbilitySFXSO abilitySfx)
        {
            _audioSource = audioSource;
            _abilitySfx = abilitySfx;
            _canPlayWarmup = _abilitySfx && _abilitySfx.WarmupSfx && !_audioSource;
            _canPlayCast = _abilitySfx && _abilitySfx.CastSfx && !_audioSource;
        }

        public void PlayWarmupSfx()
        {
            if (!_canPlayWarmup) return;

            _audioSource.PlayOneShot(_abilitySfx.WarmupSfx);
        }

        public void PlayCastSfx()
        {
            if (!_canPlayCast) return;

            _audioSource.PlayOneShot(_abilitySfx.CastSfx);
        }

        /// <summary>
        /// Plays the ability's OnHitSFX from the caster's or the given target's AudioSource.
        /// Returns the length of the clip.
        /// </summary>
        /// <param name="target"></param>
        public float PlayOnHitSfx(IAbilityTarget target)
        {
            if(_audioSource)
                _audioSource.Stop();
            var audioSource = target != null && target.AudioSource
                ? target.AudioSource 
                : _audioSource;
            
            if (!_abilitySfx || _abilitySfx.OnHitSfx.Length == 0 || !audioSource) return 0;

            AudioClip clipToPlay = _abilitySfx.OnHitSfx[Random.Range(0, _abilitySfx.OnHitSfx.Length)];
            audioSource.PlayOneShot(clipToPlay);
            return clipToPlay.length;
        }
    }
}