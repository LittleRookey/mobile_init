using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting of an <see cref="RaycastAbilitySO"/>
    /// </summary>
    public class RaycastAbility : AbilityBase
    {
        protected override AbilityBaseSO Ability => _raycastAbility;
        
        [SerializeField] private RaycastAbilitySO _raycastAbility;
        [HelpBox("If this is not set then Camera.main will be used")]
        [SerializeField] private Transform _raycastFrom;
        
        public bool RequiresTarget => _raycastAbility.RequiresAbilityTarget;
        
        #if UNITY_EDITOR
        [SerializeField] private bool _debugRay;
        #endif
        
        private RaycastHit _hitInfo;
        private Vector3 _hitPos;
        private Vector3 _hitNormal;
        private IAbilityTarget _target;

        protected override void OnStart()
        {
            if (!_raycastFrom)
                _raycastFrom = Camera.main.transform;
        }

        public override void Cast(Vector3 pos)
        {
            if (base.IsOnCooldown) return;
            var fromPos = _raycastFrom.position;
            var toPos = (pos - fromPos) * _raycastAbility.RaycastLength;
            
            #if UNITY_EDITOR
            if (_debugRay)
            {
                Debug.DrawRay(fromPos, toPos, Color.green, 2f);
            }
            #endif
            
            if (!Physics.Linecast(fromPos, toPos, out _hitInfo,
                _raycastAbility.RaycastLayers))
                return;
            _target = _hitInfo.collider.GetComponent<IAbilityTarget>();
            if (RequiresTarget && _target == null)
                return;
            base.CastAbility(_hitInfo.point, _target);
        }

        protected override void OnCast()
        {
            base.PlayCastSFX();
            AbilityHit(new ImpactPoint(_hitInfo.point, _hitInfo.normal), _target);
        }
    }
}