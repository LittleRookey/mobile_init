using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Meshes;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components
{
    /// <summary>
    /// An AOE indicator, i.e. a preview of the area where a given ability will hit.
    /// </summary>
    public class AoeIndicator : MonoBehaviour
    {
        [SerializeField] private IndicatorType _type = IndicatorType.Circle;
        [SerializeField] private float _scaleFactor = .47f;
        [SerializeField] private bool _eventDriven = true;
        [SerializeField] private float _distance = 10;
        [SerializeField] private float _angle = 30;
        [SerializeField] private Color _wedgePreviewColor = Color.red;
        [SerializeField] private bool _previewWedge = true;

        private Mesh _wedgeGizmoMesh;
        private Transform _transform;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private WedgePlane _wedgePlane;
        private void Awake()
        {
            this._transform = transform;
            TryGetComponent(out _meshRenderer);
            TryGetComponent(out _meshFilter);
            if (_type == IndicatorType.Wedge)
            {
                _wedgePlane = new WedgePlane(_distance, _angle);
                _meshFilter.mesh = _wedgePlane.Mesh;
            }

            if (!_eventDriven) return;
            AbilitySystemEvents.PlayerSelectedAbility += Show;
            AbilitySystemEvents.PlayerDeselectedAbility += Hide;
        }

        public void SetSize(float radius)
        {
        }
        public void SetPosition(Vector3 pos)
        {
            this._transform.position = pos.With(y: .1f);
        }

        private void Show(AbilityBaseSO _ = null)
        {
            _meshRenderer.enabled = true;
        }

        public void Show(DirectionalAbility directional)
        {
            this._distance = directional.Distance;
            this._angle = directional.Angle;
            GenerateWedge();
            Show();
        }

        public void Show(AbilityBase abilityBase)
        {
            var scale = abilityBase.AreaOfEffectRadius / _scaleFactor;
            this._transform.localScale = new Vector3(scale, scale, scale);
            Show();
        }

        public void Hide()
        {
            _meshRenderer.enabled = false;
        }

        private void OnValidate()
        {
            if (_type == IndicatorType.Wedge && _previewWedge)
            {
                if(_wedgePlane == null)
                    _wedgePlane = new WedgePlane(_distance, _angle);
                _wedgeGizmoMesh = _wedgePlane.Regenerate(_distance, _angle).Mesh;
            }
                
            else _wedgeGizmoMesh = null;
        }

        private void OnDrawGizmos()
        {
            if (_type == IndicatorType.Wedge && _wedgeGizmoMesh)
            {
                Gizmos.color = _wedgePreviewColor;
                Gizmos.DrawMesh(_wedgeGizmoMesh, transform.position, transform.rotation);
            }
        }

        private void OnDestroy()
        {
            AbilitySystemEvents.PlayerSelectedAbility -= Show;
            AbilitySystemEvents.PlayerDeselectedAbility -= Hide;
        }

        private void GenerateWedge()
        {
            _wedgePlane.Regenerate(_distance, _angle);
            _meshFilter.mesh = _wedgePlane.Mesh;
        }

        public enum IndicatorType
        {
            Circle,
            Wedge
        }
    }
}