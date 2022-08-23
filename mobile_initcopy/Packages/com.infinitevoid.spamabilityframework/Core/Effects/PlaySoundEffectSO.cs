using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Play sound", fileName = "playSound.asset")]
    public class PlaySoundEffectSO : AbilityEffectSO
    {
        protected override string _metaHelpDescription =>
            $"Plays one of the given AudioClips from the target's AudioSource. If more than one clip is given a random clip will be selected every time.";

        [SerializeField] private AudioClip[] _audioClips = new AudioClip[0];

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            if (_audioClips.Length == 0)
            {
#if UNITY_EDITOR
                LogWarning($"No audio clips are given for the effect: {this.name}");
#endif
                return;
            }

            if (!target.AudioSource) return;
            AudioClip clipToPlay = _audioClips[Random.Range(0, _audioClips.Length)];
            
#if UNITY_EDITOR
            if (clipToPlay == null)
            {
                Debug.LogError(
                    $"Clip to play was null in {this.name}. Did you forget to assign an AudioClip in the inspector?");
                return;
            }
#endif
            target.AudioSource.PlayOneShot(clipToPlay);
        }
    }
}