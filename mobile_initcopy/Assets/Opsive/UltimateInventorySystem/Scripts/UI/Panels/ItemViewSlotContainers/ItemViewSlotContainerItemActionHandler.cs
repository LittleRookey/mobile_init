﻿/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.Shared.Game;
    using Opsive.Shared.Input;
    using Opsive.UltimateInventorySystem.Input;
    using System;
    using UnityEngine;

    /// <summary>
    /// Trigger an item action for the selected Item View Slot.
    /// </summary>
    public class ItemViewSlotContainerItemActionHandler : MonoBehaviour
    {
        [Tooltip("The ITem Action Binding.")]
        [SerializeField] protected ItemViewSlotsContainerItemActionBindingBase m_ItemActionBinding;
        [Tooltip("The input used to trigger the default item action without specifying the action index.")]
        [SerializeField] protected SimpleInput m_DefaultInput;
        [Tooltip("The Input to use a specific action on an item.")]
        [SerializeField] protected IndexedInput[] m_ActionInputs;
        [Tooltip("The Input to use a specific action on an item.")]
        [SerializeField] protected bool m_HandleInputOnlyIfPanelSelected;
        
        protected PlayerInput m_PlayerInput;

        /// <summary>
        /// Initialize.
        /// </summary>
        protected virtual void Awake()
        {
            if (m_ItemActionBinding == null) {
                m_ItemActionBinding = GetComponent<ItemViewSlotsContainerItemActionBindingBase>();
            }

            m_ItemActionBinding.OnItemUserAssigned += HandleNewItemUser;
            HandleNewItemUser();
        }

        /// <summary>
        /// Handle a new item user being assigned.
        /// </summary>
        protected virtual void HandleNewItemUser()
        {
            m_PlayerInput = m_ItemActionBinding.ItemUser?.gameObject.GetCachedComponent<PlayerInput>();
            enabled = m_PlayerInput != null;
        }

        /// <summary>
        /// Check for the input.
        /// </summary>
        protected virtual void Update()
        {
            if(m_ItemActionBinding.ItemViewSlotsContainer == null){ return; }
            
            if (m_HandleInputOnlyIfPanelSelected) {
                var panel = m_ItemActionBinding.ItemViewSlotsContainer?.Panel;
                if (panel == null || (panel.Manager?.SelectedDisplayPanel != panel)) {
                    return;
                }
            }

            if (m_DefaultInput.CheckInput(m_PlayerInput)) {
                m_ItemActionBinding.TriggerItemAction();
            }
            
            for (int i = 0; i < m_ActionInputs.Length; i++) {
                if (m_ActionInputs[i].CheckInput(m_PlayerInput)) {
                    m_ItemActionBinding.UseItemActionOnSelectedSlot(m_ActionInputs[i].Index);
                }
            }
        }
    }
}