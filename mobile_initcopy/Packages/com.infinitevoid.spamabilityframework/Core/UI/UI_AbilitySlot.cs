using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace InfiniteVoid.SpamFramework.Core.UI
{
    /// <summary>
    /// Simple box which shows an ability's Icon and cooldown
    /// </summary>
    public class UI_AbilitySlot : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private AbilityBaseSO _ability;
        [SerializeField] private Image _cooldownOverlay;

        private float _timeOnCooldown;

        void Start()
        {
            this._image.sprite = _ability.Icon;
            AbilitySystemEvents.AbilityCooldownStart += OnAbilityCooldownStart;
            AbilitySystemEvents.AbilityCooldownTick += OmAbilityCooldownTick;
            this.enabled = false;
        }

        private void Update()
        {
            if(AbilityIsOnCooldown())
                UpdateTimeOnCooldown(Time.deltaTime);
            else 
                ResetCooldown();
        }

        private void OmAbilityCooldownTick(AbilityBaseSO ability, float tickAmount)
        {
                if (ability.Name == _ability.Name && !_ability.AutomaticCooldown)
                    UpdateTimeOnCooldown(tickAmount);
        }

        private void OnAbilityCooldownStart(AbilityBaseSO ability)
        {
            if (ability.Name != _ability.Name) return;

            if (_ability.AutomaticCooldown)
            {
                _timeOnCooldown = 0;
                this.enabled = true;
            }

            else
                SetOnCooldown();
        }

        public void SetAbility(AbilityBaseSO ability)
        {
            this._ability = ability;
            this._image.sprite = ability.Icon;
            ResetCooldown();
        }

        private void ResetCooldown()
        {
            _cooldownOverlay.fillAmount = 0;
            this.enabled = false;
        }

        public void Show(){
            SetChildrenActive(true);
        }

        public void Hide()
        {
            SetChildrenActive(false);
        }

        private void SetChildrenActive(bool active)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(active);
            }
        }

        private void SetOnCooldown()
        {
            _timeOnCooldown = 0f;
            _cooldownOverlay.fillAmount = 1;
            enabled = false;
        }

        private bool AbilityIsOnCooldown() => _timeOnCooldown < this._ability.Cooldown;

        private void UpdateTimeOnCooldown(float timeToAdd)
        {
            _timeOnCooldown += timeToAdd;
            var fraction = 1 - (_timeOnCooldown / this._ability.Cooldown);
            _cooldownOverlay.fillAmount = fraction;

            if (!AbilityIsOnCooldown())
                _cooldownOverlay.fillAmount = 0;
        }
    }
}