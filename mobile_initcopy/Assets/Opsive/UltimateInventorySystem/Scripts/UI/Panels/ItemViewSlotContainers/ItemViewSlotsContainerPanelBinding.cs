﻿/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using System;
    using Opsive.UltimateInventorySystem.UI.Item;
    using UnityEngine;

    /// <summary>
    /// The inventory Menu component.
    /// </summary>
    public class ItemViewSlotsContainerPanelBinding : InventoryPanelBinding
    {
        [Tooltip("The inventory grid.")]
        [SerializeField] protected ItemViewSlotsContainerBase m_ItemViewSlotsContainer;
        [Tooltip("Draw on Initialize.")]
        [SerializeField] internal bool m_DrawOnInitialize = false;
        [Tooltip("Reset Draw will set draw the original values for example the navigator index back to 0 for grids.")]
        [SerializeField] protected bool m_ResetDrawOnOpen = true;
        [Tooltip("Draw on open.")]
        [SerializeField] internal bool m_DrawOnOpen = true;
        [Tooltip("Select Button 0 on Open.")]
        [SerializeField] internal bool m_SelectSlotOnOpen = true;
        [Tooltip("Select Button 0 on Open.")]
        [SerializeField] internal bool m_RememberSelectedSlot = false;

        protected int m_SelectedSlotIndex = 0;
        
        public bool RememberSelectedSlot { get => m_RememberSelectedSlot; set => m_RememberSelectedSlot = value; }
        public int SelectedSlotIndex { get => m_SelectedSlotIndex; set => m_SelectedSlotIndex = value; }

        public ItemViewSlotsContainerBase ItemViewSlotsContainer {
            get => m_ItemViewSlotsContainer;
            internal set => m_ItemViewSlotsContainer = value;
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="display">The display panel.</param>
        /// <param name="force"></param>
        public override void Initialize(DisplayPanel display, bool force)
        {
            var wasInitialized = m_IsInitialized;
            if (wasInitialized && !force) { return; }
            base.Initialize(display, force);

            if (m_DrawOnInitialize) {
                m_ItemViewSlotsContainer.Draw();
                m_ItemViewSlotsContainer.SelectSlot(GetIndexToSelect());
            }
        }

        /// <summary>
        /// Initialize before the inventory binding.
        /// </summary>
        protected override void OnInitializeBeforeInventoryBind()
        {
            if (m_ItemViewSlotsContainer == null) {
                Debug.LogError("The item view slots container must NOT be null", gameObject);
                return;
            }

            m_ItemViewSlotsContainer.SetDisplayPanel(m_DisplayPanel);

            m_ItemViewSlotsContainer.Initialize(false);
            m_ItemViewSlotsContainer.OnItemViewSlotSelected += HandleSlotSelected;

            var inventoryGridItemActionBinding = m_ItemViewSlotsContainer.GetComponent<ItemViewSlotsContainerItemActionBindingBase>();
            if (inventoryGridItemActionBinding != null) {
                inventoryGridItemActionBinding.CloseItemAction(false);
            }
        }

        private void HandleSlotSelected(ItemViewSlotEventData slotEventData)
        {
            SelectedSlotIndex = slotEventData?.Index ?? 0;
        }

        /// <summary>
        /// On inventory bound.
        /// </summary>
        protected override void OnInventoryBound()
        {
            if (m_ItemViewSlotsContainer == null) { return; }

            if (m_Inventory == null) {

                Debug.LogWarning($"Inventory is missing for the ItemViewSlotsContainer {m_ItemViewSlotsContainer}.", gameObject);
                return;
            }

            m_ItemViewSlotsContainer.SetInventory(m_Inventory);
            if (m_SelectSlotOnOpen) {
                m_ItemViewSlotsContainer.SelectSlot(GetIndexToSelect());
            }
        }

        protected virtual int GetIndexToSelect()
        {
            if (m_ItemViewSlotsContainer.SlotCount <= SelectedSlotIndex) {
                return 0;
            }
            
            return RememberSelectedSlot ? SelectedSlotIndex : 0;
        }

        /// <summary>
        /// On display panel open.
        /// </summary>
        public override void OnOpen()
        {
            base.OnOpen();

            if (m_ResetDrawOnOpen) {
                m_ItemViewSlotsContainer.ResetDraw();
            }else if (m_DrawOnOpen) {
                m_ItemViewSlotsContainer.Draw();
            }

            if (m_SelectSlotOnOpen) {
                m_ItemViewSlotsContainer.SelectSlot(GetIndexToSelect());
            }
        }

        private void OnDestroy()
        {
            if(m_ItemViewSlotsContainer == null){ return; }
            m_ItemViewSlotsContainer.OnItemViewSlotSelected -= HandleSlotSelected;
        }
    }
}