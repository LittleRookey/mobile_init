using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Meshes;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components
{
    /// <summary>
    /// A telegraph is a visual representation of where an ability will strike.
    /// The wedge will be filled progressively during the ability's warmup-time (see <seealso cref="AbilityAnimationTimingsSO"/>) until it's full.
    /// </summary>
    public class WedgeTelegraph : MonoBehaviour
    {
        [HelpBox(
            "Enable this if the indicator is controlled via the AbilitySystemEvents instead of via direct reference.")]
        [SerializeField]
        private bool _eventDriven = true;

        [SerializeField] private MeshFilter _outerWedgeFilter;
        [SerializeField] private MeshRenderer _outerWedgeRenderer;

        [SerializeField] private MeshFilter _innerWedgeFilter;
        [SerializeField] private MeshRenderer _innerWedgeRenderer;

        [HelpBox(
            "The time between each update of the telegraph. Set to 0 if you want the telegraph to update every frame")]
        [SerializeField]
        private float _updateFrequency = 0f;

        private float _timeElapsed;
        private float _timeToNextUpdate;
        private float _timeToMaxSize;

        private WedgePlane _innerWedge;
        private WedgePlane _outerWedge;
        private float _endDistance;
        private float _angle;

        private void Awake()
        {
            this.enabled = false;
            _outerWedge = new WedgePlane(15, 45, false);
            _innerWedge = new WedgePlane(15, 45, true);
            _outerWedgeFilter.mesh = _outerWedge.Mesh;
            _innerWedgeFilter.mesh = _innerWedge.Mesh;
            if (!_eventDriven) return;
            AbilitySystemEvents.AbilityUsed += Show;
        }

        private void Show(AbilityBaseSO ability, Vector3 targetPosition)
        {
            if (!ability.AOEAbility || !ability.AnimationTimings || !ability.Telegraphed
                || (ability.Telegraphed && ability.AnimationTimings.WarmupTime <= 0)) return;
            if (!(ability is DirectionalAbilitySO directionalAbility)) return;
            _outerWedgeRenderer.enabled = true;
            _innerWedgeRenderer.enabled = true;
            _outerWedge.Regenerate(directionalAbility.Distance, directionalAbility.Angle);
            _endDistance = directionalAbility.Distance;
            _angle = directionalAbility.Angle;
            _timeElapsed = 0;
            _timeToNextUpdate = 0;
            _timeToMaxSize = ability.AnimationTimings.WarmupTime;
            this.enabled = true;
        }

        private void Update()
        {
            _timeElapsed += Time.deltaTime;
            _timeToNextUpdate -= Time.deltaTime;
            if (0 < _updateFrequency)
            {
                if (0 < _timeToNextUpdate)
                    return;
                
                _timeToNextUpdate = _updateFrequency;
            }

            var fraction = _timeElapsed / _timeToMaxSize;
            var curDistance = fraction * _endDistance;
            _innerWedge.Regenerate(curDistance, _angle);
            if (1 < fraction)
            {
                this.enabled = false;
                Hide();
            }
        }

        private void OnDestroy()
        {
            AbilitySystemEvents.AbilityUsed -= Show;
        }

        private void Hide()
        {
            this._outerWedgeRenderer.enabled = false;
            this._innerWedgeRenderer.enabled = false;
        }
    }
}