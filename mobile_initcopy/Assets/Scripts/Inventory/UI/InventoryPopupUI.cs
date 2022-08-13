using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��¥ : 2021-04-13 PM 7:47:35
// �ۼ��� : Rito

namespace Litkey
{
    /// <summary> �κ��丮 UI ���� ��� ���� �˾��� ���� </summary>
    public class InventoryPopupUI : MonoBehaviour
    {
        /***********************************************************************
        *                               Fields
        ***********************************************************************/
        #region .
        // 1. ������ ������ Ȯ�� �˾�
        [Header("Confirmation Popup")]
        [SerializeField] private GameObject _confirmationPopupObject;
        [SerializeField] private Text _confirmationItemNameText;
        [SerializeField] private Text _confirmationText;
        [SerializeField] private Button _confirmationOkButton;     // Ok
        [SerializeField] private Button _confirmationCancelButton; // Cancel

        // 2. ���� �Է� �˾�
        [Header("Amount Input Popup")]
        [SerializeField] private GameObject _amountInputPopupObject;
        [SerializeField] private Text _amountInputItemNameText;
        [SerializeField] private InputField _amountInputField;
        [SerializeField] private Button _amountPlusButton;        // +
        [SerializeField] private Button _amountMinusButton;       // -
        [SerializeField] private Button _amountInputOkButton;     // Ok
        [SerializeField] private Button _amountInputCancelButton; // Cancel

        // Ȯ�� ��ư ������ �� ������ �̺�Ʈ
        private event Action OnConfirmationOK;
        private event Action<int> OnAmountInputOK;

        // ���� �Է� ���� ����
        private int _maxAmount;

        #endregion
        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .
        private void Awake()
        {
            InitUIEvents();
            HidePanel();
            HideConfirmationPopup();
            HideAmountInputPopup();
        }

        private void Update()
        {
            if (_confirmationPopupObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    _confirmationOkButton.onClick?.Invoke();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _confirmationCancelButton.onClick?.Invoke();
                }
            }
            else if (_amountInputPopupObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    _amountInputOkButton.onClick?.Invoke();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _amountInputCancelButton.onClick?.Invoke();
                }
            }
        }

        #endregion
        /***********************************************************************
        *                               Public Methods
        ***********************************************************************/
        #region .
        /// <summary> Ȯ��/��� �˾� ���� </summary>
        public void OpenConfirmationPopup(Action okCallback, string itemName)
        {
            ShowPanel();
            ShowConfirmationPopup(itemName);
            SetConfirmationOKEvent(okCallback);
        }
        /// <summary> ���� �Է� �˾� ���� </summary>
        public void OpenAmountInputPopup(Action<int> okCallback, int currentAmount, string itemName)
        {
            _maxAmount = currentAmount - 1;
            _amountInputField.text = "1";

            ShowPanel();
            ShowAmountInputPopup(itemName);
            SetAmountInputOKEvent(okCallback);
        }

        #endregion
        /***********************************************************************
        *                               Private Methods
        ***********************************************************************/
        #region .
        private void InitUIEvents()
        {
            // 1. Ȯ�� ��� �˾�
            _confirmationOkButton.onClick.AddListener(HidePanel);
            _confirmationOkButton.onClick.AddListener(HideConfirmationPopup);
            _confirmationOkButton.onClick.AddListener(() => OnConfirmationOK?.Invoke());

            _confirmationCancelButton.onClick.AddListener(HidePanel);
            _confirmationCancelButton.onClick.AddListener(HideConfirmationPopup);

            // 2. ���� �Է� �˾�
            _amountInputOkButton.onClick.AddListener(HidePanel);
            _amountInputOkButton.onClick.AddListener(HideAmountInputPopup);
            _amountInputOkButton.onClick.AddListener(() => OnAmountInputOK?.Invoke(int.Parse(_amountInputField.text)));

            _amountInputCancelButton.onClick.AddListener(HidePanel);
            _amountInputCancelButton.onClick.AddListener(HideAmountInputPopup);

            // [-] ��ư �̺�Ʈ
            _amountMinusButton.onClick.AddListener(() =>
            {
                int.TryParse(_amountInputField.text, out int amount);
                if (amount > 1)
                {
                    // Shift ������ 10�� ����
                    int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount - 10 : amount - 1;
                    if (nextAmount < 1)
                        nextAmount = 1;
                    _amountInputField.text = nextAmount.ToString();
                }
            });

            // [+] ��ư �̺�Ʈ
            _amountPlusButton.onClick.AddListener(() =>
            {
                int.TryParse(_amountInputField.text, out int amount);
                if (amount < _maxAmount)
                {
                    // Shift ������ 10�� ����
                    int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount + 10 : amount + 1;
                    if (nextAmount > _maxAmount)
                        nextAmount = _maxAmount;
                    _amountInputField.text = nextAmount.ToString();
                }
            });

            // �Է� �� ���� ����
            _amountInputField.onValueChanged.AddListener(str =>
            {
                int.TryParse(str, out int amount);
                bool flag = false;

                if (amount < 1)
                {
                    flag = true;
                    amount = 1;
                }
                else if (amount > _maxAmount)
                {
                    flag = true;
                    amount = _maxAmount;
                }

                if (flag)
                    _amountInputField.text = amount.ToString();
            });
        }

        private void ShowPanel() => gameObject.SetActive(true);
        private void HidePanel() => gameObject.SetActive(false);

        private void ShowConfirmationPopup(string itemName)
        {
            _confirmationItemNameText.text = itemName;
            _confirmationPopupObject.SetActive(true);
        }
        private void HideConfirmationPopup() => _confirmationPopupObject.SetActive(false);

        private void ShowAmountInputPopup(string itemName)
        {
            _amountInputItemNameText.text = itemName;
            _amountInputPopupObject.SetActive(true);
        }
        private void HideAmountInputPopup() => _amountInputPopupObject.SetActive(false);

        private void SetConfirmationOKEvent(Action handler) => OnConfirmationOK = handler;
        private void SetAmountInputOKEvent(Action<int> handler) => OnAmountInputOK = handler;


        #endregion

    }
}