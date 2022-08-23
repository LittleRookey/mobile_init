using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Audio;
using InfiniteVoid.SpamFramework.Core.Common.VFX;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components
{
    /// <summary>
    /// A projectile is an object that travels in world-space until it collides or dies.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private GameObject _projectileVisual;
        [SerializeField] private CircleTelegraph _telegraph;
        private event Action Deactivated;
        private float _speed;
        private float _timeToLive;
        private bool _deactivateOnHit;
        private Rigidbody _rby;
        private bool _usePhysics;
        private Vector3 _velocity;

        private Transform _transform;
        private IAbilityTarget _target;
        private AudioSource _audioSource;

        // Cached field for less garbage
        private bool _destroyOnTargetPoint;
        private Vector3 _targetPoint;
        private Collider _collider;
        private bool _telegraphed;
        private float _distanceCheck;
        private bool _hasLifetime;
        private AbilityEffectsApplier _abilityEffectsApplier;
        private AbilitySFXHandler _abilitySfxHandler;
        private AbilityTargetsResolver _targetResolver;
        private AbilityInvoker _projectileInvoker;
        private IAbilityVFXHandler _abilityVfxHandler;
        private float _deactivationTimer;
        private float _onHitClipLength;
        private AudioClip _inFlightSound;
        private bool _stopOnHit;
        private bool _shouldMove;

        private void Awake()
        {
            TryGetComponent(out _rby);
            TryGetComponent(out _audioSource);
            TryGetComponent(out _collider);
            _transform = transform;
            if (_telegraph)
                _telegraph.transform.SetParent(null);
            _audioSource.loop = true;
        }

        public void Init(ProjectileAbilitySO ability, Transform parent = null, Action decreaseNumProjectileAliveAction = null)
        {
            _abilityEffectsApplier = new AbilityEffectsApplier(ability);
            _abilitySfxHandler = new AbilitySFXHandler(_audioSource, ability.SFX);
            _targetResolver = new AbilityTargetsResolver(ability);
            this.Deactivated += decreaseNumProjectileAliveAction;
            if (ability.VFX == null)
            {
                _abilityVfxHandler = NullAbilityVFXHandler.Instance;
            }
            else
            {
                _abilityVfxHandler =
                    new AbilityVFXHandler(ability,
                        null,
                        getOnHitVfxInstance: () =>
                        {
                            var vfxTransform = Instantiate(ability.VFX.OnHitVfx, transform.position, Quaternion.identity).transform;
                            if(parent != null)
                                vfxTransform.SetParent(parent);
                            return vfxTransform;
                        });
            }
        }

        /// <summary>
        /// Sets the projectile to move in the direction of the given target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="projectileSo"></param>
        /// <param name="onProjectileDeath"></param>
        /// <param name="projectileInvoker"></param>
        public void MoveInDirection(Vector3 target,
            ProjectileAbilitySO projectileSo,
            AbilityInvoker projectileInvoker)
        {
            SetupForMove(projectileInvoker, projectileSo, target, false);
            _destroyOnTargetPoint = false;
        }

        /// <summary>
        /// Sets the projectile to move to the given point.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="timeToLive"></param>
        /// <param name="onProjectileDeath"></param>
        /// <param name="onProjectileHit"></param>
        /// <param name="distanceCheck"></param>
        public void MoveTo(Vector3 point,
            ProjectileAbilitySO projectileSo,
            AbilityInvoker projectileInvoker,
            float distanceCheck = .1f)
        {
            SetupForMove(projectileInvoker, projectileSo, point);
            _destroyOnTargetPoint = true;
            _distanceCheck = distanceCheck;
            this._targetPoint = point;
        }

        public void MoveToTarget(IAbilityTarget target,
            ProjectileAbilitySO projectileSo,
            AbilityInvoker projectileInvoker)
        {
            var targetPos = target.Transform.position;
            SetupForMove(projectileInvoker, projectileSo, targetPos);
            this._target = target;
        }

        private void SetupForMove(AbilityInvoker projectileInvoker, ProjectileAbilitySO projectileSo, Vector3 target, bool? telegraphedOverride = null)
        {
            ResetProjectile();
            SetFieldsFromProjectileData(projectileSo);
            if (telegraphedOverride.HasValue)
                _telegraphed = telegraphedOverride.Value;
            if (_telegraphed && _telegraph)
                _telegraph.Show(projectileSo.EffectRadius, target);
            _projectileInvoker = projectileInvoker;
            _transform.LookAt(target);
            GetVelocity(target);
            PlayInFlightSound(projectileSo);
        }

        private void PlayInFlightSound(ProjectileAbilitySO projectileSo)
        {
            if (!projectileSo.InFlightSound) return;
            
            _audioSource.clip = projectileSo.InFlightSound;
            _audioSource.Play();
        }

        private void ResetProjectile()
        {
            this.gameObject.SetActive(true);
            this._usePhysics = !_rby.isKinematic;
            this._destroyOnTargetPoint = false;
            this._target = null;
            this.enabled = true;
            this._shouldMove = true;
            this._projectileVisual.SetActive(true);
            this._deactivationTimer = 0;
            this._collider.enabled = true;
            if(_audioSource.isPlaying)
                _audioSource.Stop();
        }

        private void SetFieldsFromProjectileData(ProjectileAbilitySO projectileSo)
        {
            this._speed = projectileSo.ProjectileSpeed;
            this._deactivateOnHit = projectileSo.DeactivateOnHit;
            this._stopOnHit = projectileSo.StopOnHit;
            this._timeToLive = projectileSo.TimeToLive;
            this._hasLifetime = 0 < projectileSo.TimeToLive;
            this._telegraphed = projectileSo.TelegraphOnCast && _telegraph != null;
            this._inFlightSound = projectileSo.InFlightSound;
        }

        private void Update()
        {
            if (0 < _deactivationTimer)
            {
                float deltatime = Time.deltaTime;
                _abilityEffectsApplier.Update(deltatime);
                _deactivationTimer -= deltatime;
                if (_deactivationTimer <= 0)
                {
                    this.Deactivated?.Invoke();
                    this.gameObject.SetActive(false);
                }
            }

            if (!_shouldMove) return;
            if (_target != null)
            {
                if (!_target.Transform)
                {
                    Deactivate();
                    return;
                }

                var targetPos = _target.Transform.position;
                if (_telegraph)
                    _telegraph.SetPosition(targetPos);
                GetVelocity(targetPos);
                this._transform.LookAt(targetPos);
            }

            if (_hasLifetime)
            {
                _timeToLive -= Time.deltaTime;
                if (_timeToLive <= 0)
                {
                    Deactivate();
                    return;
                }
            }

            if (_destroyOnTargetPoint)
            {
                if (Distance.IsLessThan(_targetPoint, _transform.position, _distanceCheck))
                {
                    OnHitTarget(null, Vector3.up);
                    return;
                }
            }

            if (_usePhysics) return;

            _transform.position += _velocity * Time.deltaTime;
        }


        private void GetVelocity(Vector3 target)
        {
            var direction = (target - _transform.position).normalized;
            this._velocity = direction * _speed;
        }

        private void FixedUpdate()
        {
            if (!_usePhysics) return;
            _rby.velocity = _velocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (0 < _deactivationTimer) return;
            OnHitTarget(collision.collider.GetComponent<IAbilityTarget>(), collision.GetContact(0).normal);
        }

        private void OnHitTarget([CanBeNull] IAbilityTarget target, Vector3 normal)
        {
            var curPos = _transform.position;
            _targetResolver.ResolveTargets(_projectileInvoker, curPos, target);
            _abilityVfxHandler.ExecuteOnHitVFX(new ImpactPoint(curPos, normal), _targetResolver.ValidTargets);
            _abilityEffectsApplier.ApplyEffects(curPos, _targetResolver.ValidTargets, _projectileInvoker);
            _onHitClipLength = _abilitySfxHandler.PlayOnHitSfx(null);

            if (_deactivateOnHit)
                Deactivate();
            else if (_stopOnHit)
            {
                _collider.enabled = false;
                _shouldMove = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (0 < _deactivationTimer) return;
            OnHitTarget(other.GetComponent<IAbilityTarget>(), -transform.forward);
        }
        
        private void Deactivate()
        {
            _projectileVisual.SetActive(false);
            _collider.enabled = false;
            _shouldMove = false;
            if (_telegraph)
                _telegraph.Hide();
            // Add extra offset to make sure effects or sound isn't cut off 
            float extraDeactivationTimeOffset = .2f;
            _deactivationTimer = Mathf.Max(_onHitClipLength, _abilityEffectsApplier.MaxEffectTime) + extraDeactivationTimeOffset;
        }

        private void OnDestroy()
        {
            _abilityVfxHandler.Destroy(Destroy);
            if(_telegraph)
                Destroy(_telegraph.gameObject);
            Deactivated = null;
        }
    }
}