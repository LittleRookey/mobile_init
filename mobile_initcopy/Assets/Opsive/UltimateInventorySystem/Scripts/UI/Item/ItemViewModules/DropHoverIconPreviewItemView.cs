/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Item.ItemViewModules
{
    using System;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item.DragAndDrop;
    using Opsive.UltimateInventorySystem.UI.Views;
    using UnityEngine;using UnityEngine.Events;
    using UnityEngine.UI;

    [Serializable]
    public class IntUnityEvent : UnityEvent<int> { }

    /// <summary>
    /// A Item View UI component that lets you bind an icon to the item icon attribute.
    /// </summary>
    public class DropHoverIconPreviewItemView : ItemViewModuleSelectable, IItemViewSlotDropHoverSelectable
    {
        [Tooltip("The icon image.")]
        [SerializeField] protected Image m_ItemIcon;
        [Tooltip("The icon image.")]
        [SerializeField] protected Image m_ColorFilter;
        [Tooltip("The color when not condition have passed.")]
        [SerializeField] protected Color m_NoConditionsPassed;
        [Tooltip("The preview color when at least one condition has passed.")]
        [SerializeField] protected Color m_ConditionsPassed;
        [Tooltip("Should the Item View Source show the item within the destination Item View")]
        [SerializeField] protected bool m_DoNotPreviewInDestination;
        [Tooltip("Should the Item View Destination show the item within the source Item View")]
        [SerializeField] protected bool m_DoNotPreviewInSource;
        [Tooltip("Should the Item View Destination show the item within the source Item View if the Destination is Null.")]
        [SerializeField] protected bool m_DoNotPreviewInSourceIfDestinationIsNull;
        [Tooltip("Enable the GameObjects when the condition passes.")]
        [SerializeField] protected GameObject[] m_EnableOnAnyConditionPassed;
        [Tooltip("Enable the GameObjects when the condition does not pass.")]
        [SerializeField] protected GameObject[] m_EnableOnNoConditionPassed;
        [Tooltip("Enable the GameObjects when the condition does not pass.")]
        [SerializeField] protected IntUnityEvent m_OnIndexedConditionPassed;
        [Tooltip("Enable the GameObjects when the condition does not pass.")]
        [SerializeField] protected UnityEvent m_OnNoConditionPassed;
        [Tooltip("Enable the GameObjects when the condition does not pass.")]
        [SerializeField] protected UnityEvent m_OnDeselect;
        [Tooltip("Enable the GameObjects when the condition does not pass.")]
        [SerializeField] protected UnityEvent m_OnClear;

        /// <summary>
        /// Set the item info.
        /// </summary>
        /// <param name="info">The item .</param>
        public override void SetValue(ItemInfo info)
        {
            if (info.Item == null || info.Item.IsInitialized == false) {
                Clear();
                return;
            }
        }

        /// <summary>
        /// Clear the component.
        /// </summary>
        public override void Clear()
        {
            m_OnClear?.Invoke();
            
            m_ItemIcon.enabled = false;
            m_ColorFilter.enabled = false;

            EnableGameObjects(m_EnableOnAnyConditionPassed,false);
            EnableGameObjects(m_EnableOnNoConditionPassed,false);
        }

        /// <summary>
        /// Enable or disable all the gamobjects in the array.
        /// </summary>
        /// <param name="gameObjects">The gameobjects array.</param>
        /// <param name="enable">Enable or Disable?</param>
        private void EnableGameObjects(GameObject[] gameObjects, bool enable)
        {
            for (int i = 0; i < gameObjects.Length; i++) {
                if(gameObjects[i] == null){ continue; }
                gameObjects[i].SetActive(enable);
            }
        }

        /// <summary>
        /// Select with a drop handler.
        /// </summary>
        /// <param name="dropHandler">The drop handler.</param>
        public virtual void SelectWith(ItemViewDropHandler dropHandler)
        {
            var dropIndex =
                dropHandler.ItemViewSlotDropActionSet.GetFirstPassingConditionIndex(dropHandler);

            var noConditionPassed = dropIndex <= -1;
            
            TryPreviewColorFilter(dropHandler,dropIndex, true);

            TryPreviewInDestination(dropHandler,dropIndex, true);

            TryPreviewInSource(dropHandler,dropIndex, true);

            EnableGameObjects(m_EnableOnAnyConditionPassed,!noConditionPassed);
            EnableGameObjects(m_EnableOnNoConditionPassed,noConditionPassed);
            
            if(noConditionPassed) {
                m_OnNoConditionPassed?.Invoke();
            } else {
                m_OnIndexedConditionPassed?.Invoke(dropIndex);
            }
        }

        /// <summary>
        /// Preview the Color.
        /// </summary>
        /// <param name="dropHandler">The drop handler info.</param>
        /// <param name="dropIndex">The drop Index.</param>
        /// /// <param name="selected">Is the view selected.</param>
        protected virtual void TryPreviewColorFilter(ItemViewDropHandler dropHandler, int dropIndex, bool selected)
        {
            var noConditionPassed = dropIndex <= -1;

            if (selected == false) {
                m_ColorFilter.enabled = false;
                return;
            }
            
            m_ColorFilter.color = noConditionPassed ? m_NoConditionsPassed : m_ConditionsPassed;
            m_ColorFilter.enabled = true;
        }

        /// <summary>
        /// Preview the source.
        /// </summary>
        /// <param name="dropHandler">The drop handler info.</param>
        /// <param name="dropIndex">The drop index</param>
        /// <param name="selected">Is the view selected.</param>
        protected virtual void TryPreviewInSource(ItemViewDropHandler dropHandler, int dropIndex, bool selected)
        {
            if (selected && 
                (m_DoNotPreviewInSource
                 || (m_DoNotPreviewInSourceIfDestinationIsNull && dropHandler.DestinationItemInfo.Item == null))) {
                return;
            }
            
            var sourceViewModules = dropHandler.SlotCursorManager.SourceItemViewSlot.ItemView.Modules;
            var previewItemInfo = selected ? dropHandler.DestinationItemInfo : dropHandler.SourceItemInfo;

            for (int i = 0; i < sourceViewModules.Count; i++) {
                if (!(sourceViewModules[i] is ItemViewModule itemViewModule)) { continue; }

                itemViewModule.SetValue(previewItemInfo);
            }
        }

        /// <summary>
        /// Preview the destination.
        /// </summary>
        /// <param name="dropHandler">The drop handler info.</param>
        /// <param name="dropIndex">The drop Index.</param>
        /// /// <param name="selected">Is the view selected.</param>
        protected virtual void TryPreviewInDestination(ItemViewDropHandler dropHandler, int dropIndex, bool selected)
        {
            if(m_DoNotPreviewInDestination){ return; }
            
            var sourceItemInfo =  dropHandler.SlotCursorManager.SourceItemViewSlot.ItemInfo;
            var previewItemInfo = selected ? sourceItemInfo : ItemInfo;
            
            if (previewItemInfo.Item != null && previewItemInfo.Item.TryGetAttributeValue<Sprite>("Icon", out var icon)) {
                m_ItemIcon.sprite = icon;
                m_ItemIcon.enabled = true;
                return;
            }

            m_ItemIcon.enabled = false;
        }

        /// <summary>
        /// Deselect with the item view drop handler.
        /// </summary>
        /// <param name="dropHandler">The drop handler.</param>
        public virtual void DeselectWith(ItemViewDropHandler dropHandler)
        {
            var dropIndex =
                dropHandler.ItemViewSlotDropActionSet.GetFirstPassingConditionIndex(dropHandler);
            
            TryPreviewColorFilter(dropHandler,dropIndex, false);

            TryPreviewInDestination(dropHandler,dropIndex, false);

            TryPreviewInSource(dropHandler,dropIndex, false);

            Select(false);
        }

        /// <summary>
        /// Simple select/deselect.
        /// </summary>
        /// <param name="select">Select?</param>
        public override void Select(bool select)
        {
            if (select) { return; }
            m_OnDeselect?.Invoke();
            
            if (ItemInfo.Item == null || ItemInfo.Item.IsInitialized == false) {
                Clear();
                return;
            }

            EnableGameObjects(m_EnableOnAnyConditionPassed,false);
            EnableGameObjects(m_EnableOnNoConditionPassed,false);
        }
    }
}