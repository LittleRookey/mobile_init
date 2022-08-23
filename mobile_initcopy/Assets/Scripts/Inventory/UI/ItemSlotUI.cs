using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

        [Tooltip("������ ��� �̹���")]
        [SerializeField] private Image _itemUseBG;

        [Tooltip("������ �̸� �ؽ�Ʈ")]
        [SerializeField] private TextMeshProUGUI _itemNameText;

        [Tooltip("������ ���� �ؽ�Ʈ")]
        [SerializeField] private TextMeshProUGUI _amountText;

        [Tooltip("������ ��� �ؽ�Ʈ")]
        [SerializeField] private TextMeshProUGUI _itemUseText;

        [Tooltip("���� ���� �̹���")]
        [SerializeField] private Image equippedImage;

        [Tooltip("������ ��Ŀ���� �� ��Ÿ���� ���̶���Ʈ �̹���")]
        [SerializeField] private Image _highlightImage;

        [Space]
        [Tooltip("���̶���Ʈ �̹��� ���� ��")]
        [SerializeField] private float _highlightAlpha = 0.5f;

        [Tooltip("���̶���Ʈ �ҿ� �ð�")]
        [SerializeField] private float _highlightFadeDuration = 0.2f;

        [SerializeField] private Image _itemNameBG;
        [SerializeField] private Image _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Image _progressMagnifier;

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

        public bool isEquipped
        {
            get => _isEquipped;
            set
            {
                _isEquipped = value;
                //if (_isEquipped)
                //{
                //    _itemUseText.text = "����";
                //}
                //else
                //{
                //    _itemUseText.text = "����";
                //}
                //SetEquipped(!_isEquipped);
                //_itemUseBG.gameObject.SetActive(_isEquipped);
                equippedImage.gameObject.SetActive(_isEquipped);
            }
        }

        private bool _isEquipped = false;


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

        private Vector3 mag_initPos;

        #endregion
        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .
        private void Awake()
        {
            //InitComponents();
            //InitValues();
            mag_initPos = _progressMagnifier.transform.localPosition;
            UpdateProgress(0f);
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

        Vector3 initSlotPos = new Vector3(70f, 70f, 0f);
        
        private void InitValues()
        {
            // 1. Item Icon, Highlight Rect
            _iconRect.pivot = new Vector2(0.5f, 0.5f); // �ǹ��� �߾�
            _iconRect.anchorMin = Vector2.zero;        // ��Ŀ�� Top Left
            _iconRect.anchorMax = Vector2.zero;

            // �е� ����
            _iconRect.offsetMin = Vector2.one * (_padding);
            _iconRect.offsetMax = Vector2.one * (-_padding);

            IconRect.anchoredPosition = initSlotPos;
            // �����ܰ� ���̶���Ʈ ũ�Ⱑ �����ϵ���
            _highlightRect.pivot = _iconRect.pivot;
            //_highlightRect.anchorMin = _iconRect.anchorMin;
            //_highlightRect.anchorMax = _iconRect.anchorMax;
            //_highlightRect.offsetMin = _iconRect.offsetMin;
            //_highlightRect.offsetMax = _iconRect.offsetMax;

            // 2. Image
            _iconImage.raycastTarget = false;
            _highlightImage.raycastTarget = false;

            // 3. Deactivate Icon
            HideIcon();
            _highlightGo.SetActive(false);
        }

        private void SetEquipped(bool val)
        {
            _itemUseBG.gameObject.SetActive(val);

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

        public void InitSlot()
        {
            InitComponents();
            InitValues();
            _iconImage.sprite = null;
            _isEquipped = false;
            _itemNameBG.gameObject.SetActive(false);
            _itemUseBG.gameObject.SetActive(false);
            equippedImage.gameObject.SetActive(false);
            Highlight(false);
            HideIcon();
            HideText();
        }

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
                //gameObject.SetActive(true);
                _iconImage.color = Color.white;
                _amountText.color = Color.white;
            }
            else
            {
                //gameObject.SetActive(false); 
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
            {
                _highlightImage.color = new Color(
                    _highlightImage.color.r,
                    _highlightImage.color.g,
                    _highlightImage.color.b,
                    255f);
            }
            else
            {
                _highlightImage.color = new Color(
                    _highlightImage.color.r,
                    _highlightImage.color.g,
                    _highlightImage.color.b,
                    0f);
            }
                //StartCoroutine(nameof(HighlightFadeOutRoutine));
            
        }

        /// <summary> ���̶���Ʈ �̹����� ������ �̹����� ���/�ϴ����� ǥ�� </summary>
        public void SetHighlightOnTop(bool value)
        {
            if (value)
                _highlightRect.SetAsLastSibling();
            else
                _highlightRect.SetAsFirstSibling();
        }

        public void SetSizeOfIcon(float val)
        {
            _iconRect.sizeDelta = Vector2.one * val;
            //Debug.Log(IconRect.sizeDelta);
        }

        public void SelectSlot()
        {
            Highlight(true);
            _highlightImage.gameObject.SetActive(true);
        }

        public void SelectSlot(ItemData itemData)
        {
            if (!HasItem || !IsAccessible) return;

            Highlight(true);
            _highlightImage.gameObject.SetActive(true);
            _itemNameText.text = itemData.Name;
            bool useBG = false;
            _itemNameBG.gameObject.SetActive(true);
            if (itemData is WeaponItemData)
            {

                _itemUseText.text = "����";
                if (equippedImage.gameObject.activeInHierarchy)
                    _itemUseText.text = "����";
                useBG = true;
            }
            else if (itemData is ArmorItemData)
            {
                _itemUseText.text = "����";
                if (equippedImage.gameObject.activeInHierarchy)
                    _itemUseText.text = "����";
                useBG = true;
            }
            else if (itemData is PortionItemData)
            {
                _itemUseText.text = "���";
                useBG = true;
            }
            
            _itemUseBG.gameObject.SetActive(useBG);

        }

        Vector3 firstMove = new Vector3(-45f, 45f, 0f);
        Vector3 secondMove = new Vector3(0f, -90f, 0f);
        Vector3 thirdMove = new Vector3(90f, 0f, 0f);
        Vector3 fourthMove = new Vector3(0f, 90f, 0f);
        Vector3 fifthMove = new Vector3(-45f, -45f, 0f);

        public void StartProgress()
        {
            //_progressMagnifier.transform.localPosition = Vector3.zero;
            UpdateProgress(0f);
            _progressMagnifier.transform.localPosition = mag_initPos;
            _progressBar.gameObject.SetActive(true);

            //Sequence sq = DOTween.Sequence()
            //    .Append(_progressMagnifier.transform.DOLocalMove(firstMove, 0.4f).SetEase(Ease.Linear).SetRelative(true))
            //    .Append(_progressMagnifier.transform.DOLocalMove(secondMove, 0.4f).SetEase(Ease.Linear).SetRelative(true))
            //    .Append(_progressMagnifier.transform.DOLocalMove(thirdMove, 0.4f).SetEase(Ease.Linear).SetRelative(true))
            //    .Append(_progressMagnifier.transform.DOLocalMove(fourthMove, 0.4f).SetEase(Ease.Linear).SetRelative(true))
            //    .Append(_progressMagnifier.transform.DOLocalMove(fifthMove, 0.4f).SetEase(Ease.Linear).SetRelative(true));
            //.OnComplete(() => StopProgress());
            //sq.Kill();
            //sq.OnComplete(() => { sq.Kill(); });
        }

        public void StopProgress()
        {
            _progressBar.gameObject.SetActive(false);
        }

        string perc = "%";
        // total 1 second to see 
        public void UpdateProgress(float val)
        {
            _progressBar.fillAmount = val;
            _progressText.text = (val*100).ToString("F0") + perc;
        }

        public void DeselectSlot()
        {
            //if (!HasItem || !IsAccessible) return;

            _highlightGo.gameObject.SetActive(false);
            _itemNameBG.gameObject.SetActive(false);
            _itemUseBG.gameObject.SetActive(false);

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