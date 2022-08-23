using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components
{
    /// <summary>
    /// Simple example component to cast a given ability at the position of the mouse
    /// </summary>
    public class CastAbilityAtMousePosition : MonoBehaviour
    {
        [SerializeField] private AbilityBase _ability;

        [HelpBox("If this is not set then Camera.main will be used.")] [SerializeField]
        private Camera _raycastCamera;

        [SerializeField] private LayerMask _raycastLayers;

        [HelpBox("This determines the height of the aim. A high value means the aim will be further away")]
        [SerializeField]
        private float _distanceAlongRayToAim = 10f;

        [SerializeField] private float _rayMaxDistance = 100f;

        private Ray _ray;
        

        private void Awake()
        {
            _raycastCamera = Camera.main;
        }

        void Update()
        {
            if (!AbilitySystemInput.GetLeftMouseButton())
            {
                if (_ability.ContinuousCast)
                    _ability.StopCasting();
                return;
            }

            var mousePos = AbilitySystemInput.MousePosition;
            _ray = _raycastCamera.ScreenPointToRay(mousePos);
            if (_ability is ITargetedAbility targetedAbility)
            {
                CastTargetedAbility(_ray, targetedAbility);
                return;
            }

            _ability.Cast(_ray.GetPoint(_distanceAlongRayToAim));
        }

        public void SetAbility(AbilityBase ability) => _ability = ability;

        private void CastTargetedAbility(Ray ray, ITargetedAbility targetedAbility)
        {
            if (!Physics.Raycast(ray, out var hit, 0 < _rayMaxDistance ? _rayMaxDistance : Mathf.Infinity,
                _raycastLayers)) return;

            if (targetedAbility.RequiresTarget)
            {
                var target = hit.transform.GetComponent<IAbilityTarget>();
                if (target != null)
                    targetedAbility.Cast(target);
                return;
            }

            targetedAbility.Cast(hit.point);
        }
    }
}