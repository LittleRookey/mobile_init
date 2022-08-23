using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components
{
    /// <summary>
    /// A telegraph is a visual representation of where an ability will strike.
    /// The circle will be filled progressively during the ability's warmup-time (see <seealso cref="AbilityAnimationTimingsSO"/>) until it's full.
    /// </summary>
    public class CircleTelegraph : MonoBehaviour
    {
        [HelpBox(
            "Get this value by placing a sphere over the AOE-quad and scale the sphere so it exactly matches the edges of the indicator. The scale factor will then be .5/SPHERE_SCALE")]
        [SerializeField]
        private float _scaleFactor = .47f;

        [HelpBox(
            "Enable this if the indicator is controlled via the AbilitySystemMediator instead of via direct reference.")]
        [SerializeField]
        private bool _eventDriven = true;

        [SerializeField] private Transform _outerRingTransform;
        [SerializeField] private MeshRenderer _outerRingRenderer;

        [SerializeField] private Transform _innerRingTransform;
        [SerializeField] private MeshRenderer _innerRingRenderer;
        private float _timeElapsed;
        private float _timeToMaxSize;
        private float _endScale;
        private float _delayedStart;

        private void Awake()
        {
            this.enabled = false;
            if (!_eventDriven) return;
            AbilitySystemEvents.AbilityUsed += Show;
        }

        public void Show(float radius, Vector3 position)
        {
            InitTelegraph(radius, position);
        }

        public void Show(AbilityBaseSO ability, Vector3 targetPosition)
        {
            if (!ability.AOEAbility || !ability.AnimationTimings || !ability.Telegraphed) return;
            float delayedStart = 0f;
            if (ability.TelegraphOnCast)
            {
                delayedStart = ability.AnimationTimings != null ? ability.AnimationTimings.WarmupTime : 0;
            }

            InitTelegraph(ability.EffectRadius, targetPosition, ability.AnimationTimings.WarmupTime, delayedStart);
        }

        private void InitTelegraph(float radius, Vector3 pos, float timeToMaxSize = 0f, float delayedStart = 0f)
        {
            SetSize(radius);
            SetPosition(pos);
            this._delayedStart = delayedStart;
            _timeToMaxSize = timeToMaxSize;
            _innerRingTransform.localScale = Vector3.zero;

            var hasDelayedStart = delayedStart <= 0;
            var animate = 0 < timeToMaxSize;
            _outerRingRenderer.enabled = hasDelayedStart;
            _innerRingRenderer.enabled = animate;
            _timeElapsed = 0;
            this.enabled = animate;
        }

        private void Update()
        {
            _timeElapsed += Time.deltaTime;
            if (0 < _delayedStart)
            {
                if (_timeElapsed < _delayedStart) return;
                _timeElapsed = _delayedStart = 0;
                _outerRingRenderer.enabled = true;
                this.enabled = false;
                return;
            }

            var fraction = _timeElapsed / _timeToMaxSize;
            var scale = fraction * _endScale;
            _innerRingTransform.localScale = new Vector3(scale, scale, scale);
            if (1 < fraction)
            {
                this.enabled = false;
                Hide();
            }
        }

        public void Hide()
        {
            this._outerRingRenderer.enabled = false;
            this._innerRingRenderer.enabled = false;
        }

        private void SetSize(float radius)
        {
            _endScale = radius / _scaleFactor;
            this._outerRingTransform.localScale = new Vector3(_endScale, _endScale, _endScale);
        }

        private void OnDestroy()
        {
            if(_eventDriven)
                AbilitySystemEvents.AbilityUsed -= Show;
        }

        public void SetPosition(Vector3 pos)
        {
            this.transform.position = pos.With(y: .1f);
        }
    }
}