namespace Opsive.UltimateInventorySystem.UI.Item.ItemViewModules
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Interface for Item View Slot Container Item Action Binding,
    /// Used to preview an Item Action panel open, used, canceled.
    /// </summary>
    public interface IItemActionBindingItemViewModule
    {
        void OnOpenItemActionPanel(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase);
        void OnCannotOpenItemActionPanel(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase);
        void OnItemActionInvoked(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase, int itemActionIndex, ItemAction itemAction);
        void OnCloseItemActionPanel(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase);
        void OnCancelItemActionPanel(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase);
    }
    
    public class ItemActionBindingItemViewModule : ItemViewModule, IItemActionBindingItemViewModule
    {
        [Tooltip("Enable the GameObjects while the Item Action Panel is opened.")]
        [SerializeField] protected GameObject[] m_EnableWhilePanelOpen;
        [Tooltip("Disabled the GameObjects while the Item Action Panel is opened.")]
        [SerializeField] protected GameObject[] m_DisableWhilePanelOpen;
        [Tooltip("Invoke on cannot open panel.")]
        [SerializeField] protected UnityEvent m_OnCannotOpenPanel;
        [Tooltip("Invoked on panel open.")]
        [SerializeField] protected UnityEvent m_OnPanelOpen;
        [Tooltip("Invoked on panel closed.")]
        [SerializeField] protected UnityEvent m_OnPanelClosed;
        [Tooltip("Invoked on action canceled.")]
        [SerializeField] protected UnityEvent m_OnActionCanceled;
        [Tooltip("Invoked on action invoked.")]
        [SerializeField] protected UnityEvent m_OnActionInvoked;

        protected bool m_PanelIsOpen;
        
        public bool PanelIsOpen { get => m_PanelIsOpen; set => m_PanelIsOpen = value; }
        
        public override void Clear()
        {
            EnableGameObjects(m_EnableWhilePanelOpen, m_PanelIsOpen);
            EnableGameObjects(m_DisableWhilePanelOpen, !m_PanelIsOpen);
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

        public override void SetValue(ItemInfo info)
        {
            
        }

        public void OnOpenItemActionPanel(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase)
        {
            m_PanelIsOpen = true;
            m_OnPanelOpen?.Invoke();
            EnableGameObjects(m_EnableWhilePanelOpen, m_PanelIsOpen);
            EnableGameObjects(m_DisableWhilePanelOpen, !m_PanelIsOpen);
        }

        public void OnCloseItemActionPanel(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase)
        {
            m_PanelIsOpen = false;
            m_OnPanelClosed?.Invoke();
            EnableGameObjects(m_EnableWhilePanelOpen, m_PanelIsOpen);
            EnableGameObjects(m_DisableWhilePanelOpen, !m_PanelIsOpen);
        }

        public void OnCannotOpenItemActionPanel(
            ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase)
        {
            m_OnCannotOpenPanel?.Invoke();
        }

        public void OnItemActionInvoked(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase,
            int itemActionIndex, ItemAction itemAction)
        {
            m_OnActionInvoked?.Invoke();
        }

        public void OnCancelItemActionPanel(ItemViewSlotsContainerItemActionBindingBase itemViewSlotsContainerItemActionBindingBase)
        {
            m_OnActionCanceled?.Invoke();
        }
    }
}