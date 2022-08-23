using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace InfiniteVoid.SpamFramework.Core.UI
{
    /// <summary>
    /// Simple slider that progressively fills until the current ability is cast 
    /// </summary>
    public class UI_CastTimer : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _image;
        private float _timeCasted;
        private float _castTime;

        private void Awake()
        {
            this.enabled = false;
            AbilitySystemEvents.AbilityUsed += (ability, _) =>
            {
                if (ability.CastTime <= 0) return;
                _castTime = ability.CastTime;
                if (_image)
                {
                    _image.enabled = true;
                    _image.sprite = ability.Icon;
                }

                _slider.value = 0;
                _slider.gameObject.SetActive(true);
                _timeCasted = 0f;
                this.enabled = true;
            };
            _slider.gameObject.SetActive(false);
            if (_image)
                _image.enabled = false;
        }

        private void Update()
        {
            _timeCasted += Time.deltaTime;
            var progress = _timeCasted / _castTime;
            _slider.value = progress;
            if (_castTime <= _timeCasted)
            {
                if (_image)
                    _image.enabled = false;
                _slider.gameObject.SetActive(false);
                this.enabled = false;
            }
        }
    }
}