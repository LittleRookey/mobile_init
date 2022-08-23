using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting of an <see cref="TargetedAbilitySO"/>
    /// </summary>
    public class TargetedAbility : AbilityBase, ITargetedAbility
    {
        [SerializeField] private TargetedAbilitySO _targetedAbility;

        /// <summary>
        /// Returns if the ability should be casted on the caster. 
        /// </summary>
        public bool CastOnSelf => _targetedAbility.CastOnSelf;

        protected override AbilityBaseSO Ability => _targetedAbility;

        /// <summary>
        /// Returns if the ability requires an AbilityTarget to be cast, i.e. if it can't be cast on ground
        /// </summary>
        public bool RequiresTarget => _targetedAbility.RequiresAbilityTarget;

        public override void Cast(Vector3 pos)
        {
            base.Cast(CastOnSelf ? Invoker.Position : pos);
        }

        public void Cast(IAbilityTarget target)
        {
            base.CastAbility(target.Transform.position, target);
        }

        // public void Cast(Vector3 position)
        // {
        //     base.CastAbility(position);
        // }

        public void SetAbility(TargetedAbilitySO targetedAbilitySO)
        {
            this._targetedAbility = targetedAbilitySO;
        }
    }
}