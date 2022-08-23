using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting of an <see cref="DirectionalAbilitySO"/>
    /// </summary>
    public class DirectionalAbility : AbilityBase
    {
        [SerializeField] private DirectionalAbilitySO _directionalAbility;
        protected override AbilityBaseSO Ability => _directionalAbility;

        public float Angle => _directionalAbility.Angle;
        public float Distance => _directionalAbility.Distance;

        protected override void OnStart()
        {
        }

        public void SetAbility(DirectionalAbilitySO ability)
        {
            this._directionalAbility = ability;
        }

    }
}