using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Litkey.InventorySystem
{
    public class ItemSlotUI : MonoBehaviour
    {
        /***********************************************************************
        *                               Option Fields
        ***********************************************************************/
        #region .
        [Tooltip("���� ������ �����ܰ� ���� ������ ����")]
        [SerializeField] private float _padding = 1f;

        [Tooltip("������ ������ �̹���")]
        [SerializeField] private Image _iconImage;

        [Tooltip("������ ���� �ؽ�Ʈ")]
        [SerializeField] private TextMeshProUGUI _amountText;

        [Tooltip("������ ��Ŀ���� �� ��Ÿ���� ���̶���Ʈ �̹���")]
        [SerializeField] private Image _highlightImage;

        [Space]
        [Tooltip("���̶���Ʈ �̹��� ���� ��")]
        [SerializeField] private float _highlightAlpha = 0.5f;

        [Tooltip("���̶���Ʈ �ҿ� �ð�")]
        [SerializeField] private float _highlightFadeDuration = 0.2f;

        #endregion
        /***********************************************************************
        *                               Properties
        ***********************************************************************/
        #region .
        /// <summary> ������ �ε��� </summary>
        public int Index { get; private set; }

        /// <summary> ������ �������� �����ϰ� �ִ��� ���� </summary>
        public bool HasItem => _iconImage.sprite != null;

        /// <summary> ���� ������ �������� ���� </summary>
        public bool IsAccessible => _isAccessibleSlot && _isAccessibleItem;

        public RectTransform SlotRect => _slotRect;
        public RectTransform IconRect => _iconRect;

        #endregion
        /***********************************************************************
        *                               Fields
        ***********************************************************************/
        #region .
        private InventoryUI _inventoryUI;

        private RectTransform _slotRect;
        private RectTransform _iconRect;
        private RectTransform _highlightRect;

        private GameObject _iconGo;
        private GameObject _textGo;
        private GameObject _highlightGo;

        private Image _slotImage;

        // ���� ���̶���Ʈ ���İ�
        private float _currentHLAlpha = 0f;

        private bool _isAccessibleSlot = true; // ���� ���ٰ��� ����
        private bool _isAccessibleItem = true; // ������ ���ٰ��� ����

        /// <summary> ��Ȱ��ȭ�� ������ ���� </summary>
        private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        /// <summary> ��Ȱ��ȭ�� ������ ���� </summary>
        private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        #endregion
        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .
        private void Awake()
        {
            InitComponents();
            InitValues();
        }

        #endregion
        /***********************************************************************
        *                               Private Methods
        ***********************************************************************/
        #region .
        private void InitComponents()
        {
            _inventoryUI = GetComponentInParent<InventoryUI>();

            // Rects
            _slotRect = GetComponent<RectTransform>();
            _iconRect = _iconImage.rectTransform;
            _highlightRect = _highlightImage.rectTransform;

            // Game Objects
            _iconGo = _iconRect.gameObject;
            _textGo = _amountText.gameObject;
            _highlightGo = _highlightImage.gameObject;

            // Images
            _slotImage = GetComponent<Image>();
        }
        private void InitValues()
        {
            // 1. Item Icon, Highlight Rect
            _iconRect.pivot = new Vector2(0.5f, 0.5f); // �ǹ��� �߾�
            _iconRect.anchorMin = Vector2.zero;        // ��Ŀ�� Top Left
            _iconRect.anchorMax = Vector2.one;

            // �е� ����
            _iconRect.offsetMin = Vector2.one * (_padding);
            _iconRect.offsetMax = Vector2.one * (-_padding);

            // �����ܰ� ���̶���Ʈ ũ�Ⱑ �����ϵ���
            _highlightRect.pivot = _iconRect.pivot;
            _highlightRect.anchorMin = _iconRect.anchorMin;
            _highlightRect.anchorMax = _iconRect.anchorMax;
            _highlightRect.offsetMin = _iconRect.offsetMin;
            _highlightRect.offsetMax = _iconRect.offsetMax;

            // 2. Image
            _iconImage.raycastTarget = false;
            _highlightImage.raycastTarget = false;

            // 3. Deactivate Icon
            HideIcon();
            _highlightGo.SetActive(false);
        }

        private void ShowIcon() => _iconGo.SetActive(true);
        private void HideIcon() => _iconGo.SetActive(false);

        private void ShowText() => _textGo.SetActive(true);
        private void HideText() => _textGo.SetActive(false);

        #endregion
        /***********************************************************************
        *                               Public Methods
        ***********************************************************************/
        #region .

        public void SetSlotIndex(int index) => Index = index;

        /// <summary> ���� ��ü�� Ȱ��ȭ/��Ȱ��ȭ ���� ���� </summary>
        public void SetSlotAccessibleState(bool value)
        {
            // �ߺ� ó���� ����
            if (_isAccessibleSlot == value) return;

            if (value)
            {
                _slotImage.color = Color.black;
            }
            else
            {
                _slotImage.color = InaccessibleSlotColor;
                HideIcon();
                HideText();
            }

            _isAccessibleSlot = value;
        }

        /// <summary> ������ Ȱ��ȭ/��Ȱ��ȭ ���� ���� </summary>
        public void SetItemAccessibleState(bool value)
        {
            // �ߺ� ó���� ����
            if (_isAccessibleItem == value) return;

            if (value)
            {
                gameObject.SetActive(true);
                _iconImage.color = Color.white;
                _amountText.color = Color.white;
            }
            else
            {
                gameObject.SetActive(false);
                _iconImage.color = InaccessibleIconColor;
                _amountText.color = InaccessibleIconColor;
            }

            _isAccessibleItem = value;
        }

        /// <summary> �ٸ� ���԰� ������ ������ ��ȯ </summary>
        public void SwapOrMoveIcon(ItemSlotUI other)
        {
            if (other == null) return;
            if (other == this) return; // �ڱ� �ڽŰ� ��ȯ �Ұ�
            if (!this.IsAccessible) return;
            if (!other.IsAccessible) return;

            var temp = _iconImage.sprite;

            // 1. ��� �������� �ִ� ��� : ��ȯ
            if (other.HasItem) SetItem(other._iconImage.sprite);

            // 2. ���� ��� : �̵�
            else RemoveItem();

            other.SetItem(temp);
        }

        /// <summary> ���Կ� ������ ��� </summary>
        public void SetItem(Sprite itemSprite)
        {
            //if (!this.IsAccessible) return;

            if (itemSprite != null)
            {
                _iconImage.sprite = itemSprite;
                ShowIcon();
            }
            else
            {
                RemoveItem();
            }
        }

        /// <summary> ���Կ��� ������ ���� </summary>
        public void RemoveItem()
        {
            _iconImage.sprite = null;
            HideIcon();
            HideText();
        }

        /// <summary> ������ �̹��� ���� ���� </summary>
        public void SetIconAlpha(float alpha)
        {
            _iconImage.color = new Color(
                _iconImage.color.r, _iconImage.color.g, _iconImage.color.b, alpha
            );
        }

        /// <summary> ������ ���� �ؽ�Ʈ ����(amount�� 1 ������ ��� �ؽ�Ʈ ��ǥ��) </summary>
        public void SetItemAmount(int amount)
        {
            //if (!this.IsAccessible) return;

            if (HasItem && amount > 1)
                ShowText();
            else
                HideText();

            _amountText.text = amount.ToString();
        }

        /// <summary> ���Կ� ���̶���Ʈ ǥ��/���� </summary>
        public void Highlight(bool show)
        {
            if (!this.IsAccessible) return;

            if (show)
                StartCoroutine(nameof(HighlightFadeInRoutine));
            else
                StartCoroutine(nameof(HighlightFadeOutRoutine));
        }

        /// <summary> ���̶���Ʈ �̹����� ������ �̹����� ���/�ϴ����� ǥ�� </summary>
        public void SetHighlightOnTop(bool value)
        {
            if (value)
                _highlightRect.SetAsLastSibling();
            else
                _highlightRect.SetAsFirstSibling();
        }

        #endregion
        /***********************************************************************
        *                               Coroutines
        ***********************************************************************/
        #region .
        /// <summary> ���̶���Ʈ ���İ� ������ ���� </summary>
        private IEnumerator HighlightFadeInRoutine()
        {
            StopCoroutine(nameof(HighlightFadeOutRoutine));
            _highlightGo.SetActive(true);

            float unit = _highlightAlpha / _highlightFadeDuration;

            for (; _currentHLAlpha <= _highlightAlpha; _currentHLAlpha += unit * Time.deltaTime)
            {
                _highlightImage.color = new Color(
                    _highlightImage.color.r,
                    _highlightImage.color.g,
                    _highlightImage.color.b,
                    _currentHLAlpha
                );

                yield return null;
            }
        }

        /// <summary> ���̶���Ʈ ���İ� 0%���� ������ ���� </summary>
        private IEnumerator HighlightFadeOutRoutine()
        {
            StopCoroutine(nameof(HighlightFadeInRoutine));

            float unit = _highlightAlpha / _highlightFadeDuration;

            for (; _currentHLAlpha >= 0f; _currentHLAlpha -= unit * Time.deltaTime)
            {
                _highlightImage.color = new Color(
                    _highlightImage.color.r,
                    _highlightImage.color.g,
                    _highlightImage.color.b,
                    _currentHLAlpha
                );

                yield return null;
            }

            _highlightGo.SetActive(false);
        }

        #endregion
    }
}